Shader "FowlPlay/Temp Grass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)

        [Header(Distortion)]
        _DistortStrength ("Max Range", Float) = 0.5
        _DistortSpeed ("Speed", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
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
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            half4 _Color;

            float _DistortStrength;
            float _DistortSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;

                //displacement
                //float distortStrength = tex2D(_NoiseTex, float2((_Time[1] * _DistortSpeed) % 1, 0)).r;
                float distortStrength = v.uv.y * _DistortStrength * sin(_Time[1] * _DistortSpeed);

                o.vertex.x += distortStrength;

                //set v2f values
                o.vertex = UnityObjectToClipPos(o.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));

                return col;
            }
            ENDCG
        }
    }
}
