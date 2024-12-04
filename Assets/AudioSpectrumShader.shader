Shader "Unlit/AudioSpectrumShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("Scale", float) = 1
        _Health ("Health", float) = 1
        _LeftColor ("Left Color", Color) = (0, 1 , 0, 1)
        _RightColor ("Right Color", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #define SPECTRUMLENGTH 1024
            #define CUTOFF 800
            #define WINDOWLENGTH (SPECTRUMLENGTH - CUTOFF)

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
            float _Spectrum[SPECTRUMLENGTH];
            float _Scale;
            float _Health;
            float4 _LeftColor;
            float4 _RightColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float bin = i.uv.x * WINDOWLENGTH;
                float t = bin == floor(bin) ? 0 : (bin - floor(bin)) / (ceil(bin) - floor(bin));

                t = floor(t * 1.5) / 1.5;

                float height = _Spectrum[floor(bin)] + (_Spectrum[ceil(bin)] - _Spectrum[floor(bin)]) * t;
                height *= _Scale;
                int paint = (1 - i.uv.y) <= height ? 1 : 0;

                return ((i.uv.x >= _Health) ? _RightColor : _LeftColor) * float4(1, 1, 1, paint);
            }
            ENDCG
        }
    }
}