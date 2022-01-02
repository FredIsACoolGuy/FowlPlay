Shader "FowlPlay/Water"
{
    Properties
    {
        [Header(Shadows)]
        _ShadowFactor ("Percent of Shadow", Range(0,1)) = 0.95
        _ShadowStrength ("Strength", Range(0,1)) = 0.05

        [Header(Body)]
        _Color ("Color", Color) = (0,1,1,1)
        _SubTex ("Col Shift Tex", 2D) = "white" {}
        _ColShiftTint ("Col Shift Tint", Color) = (1,1,1,1)
        _ColShiftScrollSpeed ("Col Shift Scroll Speed", Vector) = (0.2,0.2,0,0)

        [Header(Waves)]
        _MainTex ("Texture", 2D) = "white" {}
        _WaveColor ("Color", Color) = (1,1,1,1)
        _WaveTexCutoff ("Intensity", Range(0,1)) = 0.5
        _WaveScrollSpeed ("Scroll Speed", Vector) = (1,1,0,0)

        [Header(Sub Waves)]
        _SubWaveTexCutoff ("Intensity", Range(0,1)) = 0.5
        _SubWaveScrollSpeed ("Scroll Speed", Vector) = (1,1,0,0)
        _SubNoiseScrollSpeed ("Sub Noise Scroll Speed", Vector) = (1,1,0,0)
        _SubNoiseFactor ("Sub Noise Intensity", Range(0,3)) = 0.2

        [Header(Foam)]
        _FoamColor ("Color", Color) = (1,1,1,1)
        _FoamTexCutoff ("Foaminess", Range(0,0.995)) = 0.5
        _FoamScrollSpeed ("Scroll Speed", Vector) = (1,1,0,0)
        _FoamDepthCutoff ("Depth Cutoff", Range(0,8)) = 1

        [Header(Depth)]
        _DepthColor ("Color", Color) = (0,0,0.5,1)
        _DepthColorCutoff ("Depth Cutoff", Range(0,5)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
        LOD 100

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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD1;
                SHADOW_COORDS(4)
            };

            sampler2D _CameraDepthTexture;
            
            float _ShadowFactor;
            float _ShadowStrength;

            //main body
            half4 _Color;
            float2 _ColShiftScrollSpeed;
            half4 _ColShiftTint;

            sampler2D _SubTex;
            float4 _SubTex_ST;

            //waves
            sampler2D _MainTex;
            float4 _MainTex_ST;

            half4 _WaveColor;
            half _WaveTexCutoff;
            float3 _WaveScrollSpeed;

            half _SubWaveTexCutoff;
            float3 _SubWaveScrollSpeed;
            float2 _SubNoiseScrollSpeed;
            half _SubNoiseFactor;

            //foam
            half4 _FoamColor;
            half _FoamTexCutoff;
            float _FoamDepthCutoff;
            float3 _FoamScrollSpeed;

            //depth color
            half4 _DepthColor;
            float _DepthColorCutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                o.uv = v.uv;
                TRANSFER_SHADOW(o);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = _Color;
                half colShift = tex2D(_SubTex, TRANSFORM_TEX(i.uv, _SubTex) + _ColShiftScrollSpeed * _Time[1]).r;
                col.rgb -= colShift * _ColShiftTint.a;
                col.rgb += colShift * _ColShiftTint.rgb * _ColShiftTint.a;

                //get depth info
                float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos))).r;
                float diff = abs(depth - i.screenPos.w);

                //depth color
                float depthDiff = diff / _DepthColorCutoff;
                col = lerp(col, _DepthColor, saturate(1-depthDiff));

                //waves
                half waves = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex) + _WaveScrollSpeed.xy * _Time[1]).r;
                waves += _WaveScrollSpeed.z * _Time[1];
                waves = waves % 1;
                waves = waves > 1-_WaveTexCutoff ? 1 : 0;

                //sub waves
                half subWaves = tex2D(_MainTex, TRANSFORM_TEX(float2(i.uv.y, -i.uv.x), _MainTex) + _SubWaveScrollSpeed.xy * _Time[1]).r;
                waves -= subWaves > 1-_SubWaveTexCutoff ? 1 : 0;
                waves = saturate(waves);

                half subNoise = tex2D(_SubTex, TRANSFORM_TEX(float2(i.uv.y, -i.uv.x), _SubTex) + _SubNoiseScrollSpeed * _Time[1]).r / _SubNoiseFactor;
                //waves -= subNoise * _SubNoiseFactor;

                col.rgb = lerp (col.rgb, _WaveColor.rgb, waves * _WaveColor.a * subNoise);

                //foam
                float foamDiff = diff / _FoamDepthCutoff;
                half foam = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex) + _FoamScrollSpeed.xy * _Time[1]).r;
                foam += foamDiff;
                foam = saturate(foam);
                
                col = foam < _FoamTexCutoff ? _FoamColor : col;

                //shadows
                #if defined(SHADOWS_SCREEN)
                float shadow = SHADOW_ATTENUATION(i);
                col -= shadow < _ShadowFactor ? _ShadowStrength : 0;
                #endif
                
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
                #if defined(SHADOWS_SCREEN)
                float shadow = SHADOW_ATTENUATION(i);
                half4 col = shadow < _ShadowFactor ? -_ShadowStrength : 0;

                return col;
                #else
                return 0;
                #endif
            }
            ENDCG
        }
        
        Pass //Shadow Caster
        {
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite Off

            CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            float4 vert (appdata v) : SV_POSITION {
                float4 vertex = UnityClipSpaceShadowCasterPos(v.vertex, v.normal);
                return UnityApplyLinearShadowBias(vertex);
            }

            half4 frag () : SV_Target {
                return 0;
            }

            ENDCG
        }
    }
}
