Shader "Custom/BackGround"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0, 0, 1)
        _Speed ("Speed", Range(0.0, 10.0)) = 0.01
        _RandomSize ("Random Size", Range(0.0, 10.0)) = 1.0
        _Seed ("Seed", Range(0.0, 10.0)) = 1.0
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "CalcFunc.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            float4 _Color;
            float _Speed;
            float _RandomSize;
            float _Seed;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv + _Time.y * 0.001;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            { 
                float2 st = i.uv;
                float4 color = _Color;

                float t = 1.0;
                t = abs(1.0-sin(_Time.y*_Speed))*5.0;
                st += gradientNoise(st*2.0, 0.0) * t; // Animate the coordinate space
                color.xyz = float3(1.0, 1.0, 1.0) * smoothstep(0.18, 0.2,gradientNoise(st, 0.0)); // Big black drops
                color.xyz += smoothstep(0.15, 0.2,gradientNoise(st*10.0, 0.0)); // Black splatter
                color.xyz -= smoothstep(0.35, 0.4,gradientNoise(st*10.0, 0.0)); // Holes on splatter

                return float4(color.rgb,0.5);

            }
            ENDCG
        }
    }
}
