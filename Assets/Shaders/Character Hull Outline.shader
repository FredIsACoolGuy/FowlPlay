Shader "FowlPlay/Character Hull"
{

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)

        [Header(Shadows)]
        _ShadowFactor ("Percent of Shadow", Range(0,1)) = 0.95
        _ShadowStrength ("Strength", Range(0,1)) = 0.05

        [Header(Outline)]
        _OutlineColor ("Color", Color) = (1,1,1,1)
        _OutlineThickness ("Thickness", Range(0, 0.6)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ SHADOWS_SCREEN

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                SHADOW_COORDS(1)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _Color;
            float _ShadowFactor;
            float _ShadowStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                TRANSFER_SHADOW(o);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float shadow = SHADOW_ATTENUATION(i);
                col *= shadow < _ShadowFactor ? 1-_ShadowStrength : 1;

                return col * _Color;
            }
            ENDCG
        }
        
        Pass
        {
            Tags { "LightMode" = "ForwardAdd" }
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ SHADOWS_SCREEN

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                SHADOW_COORDS(1)
            };

            float _ShadowFactor;
            float _ShadowStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW(o);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float shadow = SHADOW_ATTENUATION(i);
                half4 col = shadow < _ShadowFactor ? -_ShadowStrength : 0;

                return col;
            }
            ENDCG
        }

        
        Pass
        {
            Cull Front
            Tags { "LightMode" = "Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            half4 _OutlineColor;
            float _OutlineThickness;

            float4 vert (appdata v) : SV_POSITION
            {
                float4 normal = float4(normalize(mul(unity_ObjectToWorld, v.normal).rgb) * _OutlineThickness, 0);
                float4 pos = mul(unity_ObjectToWorld, v.vertex);
                pos += normal;
                pos = mul(UNITY_MATRIX_VP, pos);
                return pos;
            }

            fixed4 frag () : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        Pass //Shadow Caster
        {
            Tags { "LightMode" = "ShadowCaster" }

            CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            float4 vert (appdata v) : SV_POSITION {
                float4 vertex = UnityClipSpaceShadowCasterPos(v.vertex.xyz, v.normal);
                return UnityApplyLinearShadowBias(vertex);
            }

            half4 frag () : SV_Target {
                return 0;
            }

            ENDCG
        }
    }
}
