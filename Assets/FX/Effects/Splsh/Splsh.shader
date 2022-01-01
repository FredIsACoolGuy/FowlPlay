Shader "FowlPlay/Splsh"
{
    Properties
    {
        [Header(Shadows)]
        _ShadowFactor ("Percent of Shadow", Range(0,1)) = 0.5
        _ShadowStrength ("Strength", Range(0,1)) = 0.05

        [Header(Color)]
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,1,1,1)

        [Header(Foam)]
        _FoamColor ("Color", Color) = (1,1,1,1)
        _FoamTexCutoff ("Foam Offset", Range(-5,5)) = 0.5
        _Foaminess ("Foaminess", Range(-3,5)) = 0.5
        _FoamScrollSpeed ("Scroll Speed", Vector) = (1,1,0,0)
        
        [Header(Anim)]
        _Duration ("Duration (Wave1,Wave2,NaN,NaN)", Vector) = (2,2,0,0)
        _MaxHeight ("Max Height (Wave1,Wave2,NaN,NaN)", Vector) = (2,1,0,0)
        _WaveRadii ("Radii (Min1,Max1,Min2,Max2)", Vector) = (0.5,1,0.75,2)
    }

    CustomEditor "ShadowMatEditor"
    
    SubShader
    {
        Tags { "RenderType" = "Opaque"}
        LOD 100

        CGINCLUDE

            float2 _Duration;
            float2 _MaxHeight;
            float4 _WaveRadii;

            float4 getPos(float4 vertex, float3 normal, float2 uv){

                float4 pos = vertex;

                //get proper variables by uv (wave 1 on left, wave 2 on right)
                float duration = uv.x < 0.5 ? _Duration.x : _Duration.y;
                float maxHeight = uv.x < 0.5 ? _MaxHeight.x : _MaxHeight.y;
                float minRad = uv.x < 0.5 ? _WaveRadii.x : _WaveRadii.z;
                float maxRad = uv.x < 0.5 ? _WaveRadii.y : _WaveRadii.w;
                float time = _Time[1] % (1 + (_Duration.x > _Duration.y ? _Duration.x : _Duration.y));

                //dist
                float dist = uv.y * uv.y * (maxRad - minRad) + minRad;
                dist = lerp(minRad, dist, time / duration);
                pos.xyz = pos.xyz + normal * dist;

                //height
                float dur = duration * 0.5;
                float height = -((time - dur) / dur) * ((time - dur) / dur) * maxHeight + maxHeight;
                pos.y = height * uv.y;

                pos = height < 0 ? 0 : pos;

                return pos;
            }

        ENDCG

        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            Cull Off

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                SHADOW_COORDS(4)
            };
            
            float _ShadowFactor;
            float _ShadowStrength;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            half4 _Color;

            //foam
            half4 _FoamColor;
            float _FoamTexCutoff;
            float _Foaminess;
            float3 _FoamScrollSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(getPos(v.vertex, v.normal, v.uv));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);;
                o.normal = mul(v.normal, unity_ObjectToWorld);
                TRANSFER_SHADOW(o);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = _Color;

                //foam
                half foam = 1-tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex) + _FoamScrollSpeed.xy * _Time[1]).r;
                foam -= i.uv.y * _Foaminess;
                half3 tempCol = foam < _FoamTexCutoff ? _FoamColor.rgb : col.rgb;

                float time = _Time[1] % (1 + (_Duration.x > _Duration.y ? _Duration.x : _Duration.y));
                float duration = i.uv.x < 0.5 ? _Duration.x : _Duration.y;
                float timeFactor = saturate((duration - time) / (duration * 0.5));
                col.rgb = lerp(col.rgb, tempCol, _FoamColor.a * timeFactor);

                //shadows
                float shadow = SHADOW_ATTENUATION(i);
                col -= shadow < _ShadowFactor ? _ShadowStrength : 0;

                return col;
            }
            ENDCG
        }
        
        Pass
        {
            Tags { "LightMode" = "ForwardAdd" }
            Cull Off
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
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
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
                o.pos = UnityObjectToClipPos(getPos(v.vertex, v.normal, v.uv));
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
        
        Pass //Shadow Caster
        {
            Tags { "LightMode" = "ShadowCaster" }
            Cull Off

            CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            #include "UnityCG.cginc"

            struct appdata {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            float4 vert (appdata v) : SV_POSITION {
                float4 vertex = getPos(v.vertex, v.normal, v.uv);
                vertex = UnityClipSpaceShadowCasterPos(vertex, v.normal);
                return UnityApplyLinearShadowBias(vertex);
            }

            half4 frag () : SV_Target {
                return 0;
            }

            ENDCG
        }
    }
}
