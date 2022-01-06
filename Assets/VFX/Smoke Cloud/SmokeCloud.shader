Shader "FowlPlay/VFX/Smoke Cloud"
{
    Properties
    {
        [Header(Lighting)]
        _Color ("Color", Color) = (1,1,1,1)
        _ShadowFactor ("Percent of Shadow", Range(0,1)) = 0.96
        _ShadowStrength ("Strength", Range(0,1)) = 0.05

        [Header(Displacement)]
        _DisplaceMap ("Displacement Map", 2D) = "white" {}
        _DisplaceStrength ("Strength", Range(0,3)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGINCLUDE

        sampler2D _DisplaceMap;
        float4 _DisplaceMap_ST;
        float _DisplaceStrength;

        float4 GetVertex(float4 vertex, float3 normal, float4 texCoord0, float2 texCoord1){

            float4 displaceMap = tex2Dlod(_DisplaceMap, float4(texCoord0.xy * _DisplaceMap_ST.xy + _DisplaceMap_ST.zw * _Time[1] + texCoord0.zw, 0, 0));
            float mapValue[4] = {displaceMap.x, displaceMap.y, displaceMap.z, displaceMap.a};

            float displaceStrength = _DisplaceStrength * texCoord1.x * mapValue[floor(texCoord1.y * 3.9999)];

            return vertex + float4(normal * displaceStrength, 0);
        }

        ENDCG

        Pass
        {
            Tags {"LightMode" = "ForwardBase"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ SHADOWS_SCREEN

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                half4 color : COLOR;
                float4 texCoord0 : TEXCOORD0;
                float2 texCoord1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                half4 color : COLOR;
                SHADOW_COORDS(0)
            };

            half4 _Color;
            fixed _ShadowFactor;
            fixed _ShadowStrength;

            v2f vert (appdata v)
            {
                v2f o;
                float4 vertex = GetVertex(v.vertex, v.normal, v.texCoord0, v.texCoord1);
                o.pos = UnityObjectToClipPos(vertex);
                TRANSFER_SHADOW(o);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color * i.color;
                float shadow = SHADOW_ATTENUATION(i);
                col *= shadow < _ShadowFactor ? 1-_ShadowStrength : 1;

                return col;
            }
            ENDCG
        }
        
        Pass
        {
            Tags {"LightMode" = "ForwardAdd"}
            ZWrite Off
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
                float3 normal : NORMAL;
                float4 texCoord0 : TEXCOORD0;
                float2 texCoord1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                SHADOW_COORDS(0)
            };

            half4 _Color;
            fixed _ShadowFactor;
            fixed _ShadowStrength;

            v2f vert (appdata v)
            {
                v2f o;
                float4 vertex = GetVertex(v.vertex, v.normal, v.texCoord0, v.texCoord1);
                o.pos = UnityObjectToClipPos(vertex);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float shadow = SHADOW_ATTENUATION(i);
                return shadow < _ShadowFactor ? -_ShadowStrength : 0;
            }
            ENDCG
        }

        Pass
        {
            Tags {"LightMode" = "ShadowCaster"}
            
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texCoord0 : TEXCOORD0;
                float2 texCoord1 : TEXCOORD1;
            };

            float4 vert(appdata v) : SV_POSITION
            {
                float4 pos = GetVertex(v.vertex, v.normal, v.texCoord0, v.texCoord1);
                pos = UnityClipSpaceShadowCasterPos(pos, v.normal);
                return UnityApplyLinearShadowBias(pos);
            }

            half4 frag() : SV_Target
            {
                return 0;
            }

            ENDCG
        }
    }
}
