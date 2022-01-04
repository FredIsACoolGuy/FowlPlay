Shader "FowlPlay/VFX/Attack Guide"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.2
        _AlphaCutoff2 ("Outline Cutoff", Range(0,1)) = 0.9
        _Color ("Base Color", Color) = (1,1,1,1)
        _Color2 ("Secondary Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _NoiseTex ("Noise Tex", 2D) = "white" {}
        _NoiseScrollSpeed ("Scroll Speed", Range(0, 2)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed _AlphaCutoff;
            fixed _AlphaCutoff2;
            half4 _Color;
            half4 _Color2;
            half4 _OutlineColor;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float _NoiseScrollSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed mainTex = tex2D(_MainTex, i.uv).r;
                fixed clipValue = mainTex - _AlphaCutoff;
                //fixed outline = clipValue < 0 && fwidth(clipValue) + clipValue >= 0 ? 1 : 0;
                //outline = fwidth(outline) > 0 ? 1 : outline;
                clip(clipValue /*+ outline*/);

                float2 noiseUV = float2(i.uv.x - _NoiseScrollSpeed * _Time[1], i.uv.y);
                fixed noise = tex2D(_NoiseTex, TRANSFORM_TEX(noiseUV, _NoiseTex)).r;
                half4 col = lerp(_Color, _Color2, noise);
                //col = outline ? _OutlineColor : col;
                col = mainTex < _AlphaCutoff2 ? _OutlineColor : col;

                return col;
            }
            ENDCG
        }
    }
}
