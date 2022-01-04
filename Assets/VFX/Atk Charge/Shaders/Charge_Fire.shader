Shader "FowlPlay/VFX/Charge Fire"
{
    Properties
    {
        [Header(Debug)]
        _DebugLineWeight ("Debug Line Weight", Range(0,0.1)) = 0.05
        _UVDisplace ("UV Displace Strength", Vector) = (0.2, 0.2, 0.2, 0.2)

        [Header(Tex and Color)]
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1, 0.5, 0, 1)
        _Color2 ("Background Color", Color) = (1, 0, 0, 0.3)
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.9

        [Header(Animation)]
        _NoiseTex ("Noise Tex", 2D) = "white" {}
        _AnimYStrength ("Strength", Range(0,10)) = 3
        _SpeedY ("Speed (y1,y2,seed,Nan)", Vector) = (1, 1, 0, 0)
        _SpeedX ("Speed (x1,x2,seed,Nan)", Vector) = (1, 1, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

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

            fixed _DebugLineWeight;
            fixed4 _UVDisplace;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            half4 _Color;
            half4 _Color2;
            half _AlphaCutoff;

            sampler2D _NoiseTex;
            float _AnimYStrength;
            float3 _SpeedY; //min, max, seed
            float3 _SpeedX;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            #define FLARE_COUNT 4
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = 0;
                fixed4 noise = tex2D(_NoiseTex, float2(_SpeedX.z, _SpeedY.z));
                fixed noises[4] = {noise.x,noise.y,noise.z,noise.w};

                [unroll]
                for (int j=FLARE_COUNT-1; j>-1; j--){

                    fixed2 uv = i.uv;

                    //animate uvs
                    uv.x += _Time[1] * lerp(_SpeedX.x, _SpeedX.y, noises[j]);

                    //fix uvs
                    fixed uvFaceX = (i.uv.x * 13) % 1;
                    fixed4 displaceStength = _UVDisplace / 13;
                    uv.x = uvFaceX < 1-i.uv.y ? uv.x + displaceStength.x * uvFaceX * i.uv.y : uv.x;
                    uv.x = uvFaceX > 1-i.uv.y ? uv.x + displaceStength.y * (1-uvFaceX) * (1-i.uv.y) : uv.x;

                    fixed heightNoise = tex2D(_NoiseTex, fixed2(i.uv.x, _Time[1] * lerp(_SpeedY.x, _SpeedY.y, noises[j]))).g;
                    uv.y *= heightNoise * _AnimYStrength / FLARE_COUNT * j + 1;

                    fixed4 texCol = uv.y > 1 ? 0 : tex2D(_MainTex, TRANSFORM_TEX(uv, _MainTex));
                    col = col.a < _AlphaCutoff && texCol.r > _AlphaCutoff ? lerp(_Color, _Color2, j / (FLARE_COUNT - 0.9999)) : col;
                    col.a = texCol.r > _AlphaCutoff ? 1 : col.a;
                }

                clip(col.a - _AlphaCutoff);

                //debug lines
                //col = uvFaceX > (1 - _DebugLineWeight) ? fixed4(0,0,1,1) : col;
                //col = abs(uvFaceX - (1-i.uv.y)) < _DebugLineWeight ? fixed4(0.7,0,1,1) : col;
                //col.rg = float2(uvFaceX, uv.y);

                return col;
            }
            ENDCG
        }
    }
}
