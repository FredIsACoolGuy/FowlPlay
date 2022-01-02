Shader "FowlPlay/Grass"
{

    Properties
    {
        [Header(Color)][Space]
        _Color ("Color", Color) = (1,1,1,1)
        _Color2 ("Secondary Color", Color) = (1,1,1,1)
        _ColShiftVel ("Secondary Color Velocity", Vector) = (0,0,0,0)

        [Header(Base Texture)][Space]
        _MainTex ("Texture", 2D) = "white" {}
        _MainTexVel ("Texture Velocity", Vector) = (0,0,0,0)
        _TexStrength ("Texture Strength", Range(0,1)) = 0.1

        [Header(Distortion)][Space]
        _DistortTex ("Texture", 2D) = "white" {}
        _DistortSpeed ("Speed", Float) = 1
        _DistortStrength ("Strength", Range(0,0.2)) = 0.05
        _DistortDir ("Direction", Vector) = (0,0,0,0)

        [Header(Additive)][Space]
        _AddTex ("Texture", 2D) = "white" {}
        _AddTexVel ("Texture Velocity", Vector) = (0,0,0,0)
        _AddTexStrength ("Tex Strength", Range(0,2)) = 1

        [Header(Subtractive)][Space]
        _SubTexHard ("Hard Tex", 2D) = "white" {}
        _HardSubVel ("Hard Tex Velocity", Vector) = (0,0,0,0)
        _HardSubStrength ("Hard Tex Strength", Range(0,1)) = 0.3
        _SubTexSoft ("Soft Tex", 2D) = "white" {}
        _SoftSubVel ("Soft Tex Velocity", Vector) = (0,0,0,0)
        _SoftSubStrength ("Soft Tex Strength", Range(0,1)) = 0.3

        [Header(Shadows)][Space]
        _ShadowFactor ("Percent of Shadow", Range(0,1)) = 0.95
        _ShadowStrength ("Strength", Range(0,1)) = 0.05
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

            //Color
            half4 _Color;
            half4 _Color2;
            float2 _ColShiftVel;

            //Base Texture
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _MainTexVel;
            half _TexStrength;

            //Distortion
            sampler2D _DistortTex;
            float4 _DistortTex_ST;
            float _DistortSpeed;
            half _DistortStrength;
            half2 _DistortDir;

            //Additive
            sampler2D _AddTex;
            float4 _AddTex_ST;
            float2 _AddTexVel;
            half _AddTexStrength;

            //Subtractive
            sampler2D _SubTexHard;
            float4 _SubTexHard_ST;
            float2 _HardSubVel;
            half _HardSubStrength;

            sampler2D _SubTexSoft;
            float4 _SubTexSoft_ST;
            float2 _SoftSubVel;
            half _SoftSubStrength;

            //Shadows
            half _ShadowFactor;
            half _ShadowStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                TRANSFER_SHADOW(o);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                //color
                half4 col = _Color;
                half col2Mask = tex2D(_DistortTex, TRANSFORM_TEX(float2(i.uv.y, i.uv.x), _DistortTex) + _ColShiftVel * _Time[1]).r;
                col.rgb = lerp(col.rgb, _Color2.rgb, col2Mask * _Color2.a);
                
                //distortion
                float distortStrength = (tex2D(_DistortTex, TRANSFORM_TEX(i.uv, _DistortTex)).r + _Time[1] * _DistortSpeed) % 1;
                distortStrength = sin(distortStrength * 3.14159265) * 0.5 + 0.5;

                half2 distortedUV = i.uv + normalize(_DistortDir) * _DistortStrength * distortStrength;
                distortedUV = distortedUV % 1;

                //textures
                half tex = tex2D(_MainTex, TRANSFORM_TEX(distortedUV, _MainTex) + _MainTexVel * _Time[1]).r;

                tex -= (1 - tex2D(_SubTexHard, TRANSFORM_TEX(i.uv, _SubTexHard) + _HardSubVel * _Time[1]).r) * _HardSubStrength;
                tex = saturate(tex);

                tex -= tex2D(_SubTexSoft, TRANSFORM_TEX(i.uv, _SubTexSoft) + _SoftSubVel * _Time[1]).r * _SoftSubStrength;
                tex = saturate(tex);

                tex += tex2D(_AddTex, TRANSFORM_TEX(i.uv, _AddTex) + _AddTexVel * _Time[1]).r * _AddTexStrength;
                tex = saturate(tex);

                col -= tex * _TexStrength;

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
