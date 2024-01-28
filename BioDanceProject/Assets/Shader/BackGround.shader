Shader "Custom/BackGround"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0, 0, 1)
        _ColorA ("ColorA", Color) = (1, 0, 0, 1)
        _ColorB ("ColorB", Color) = (1, 0, 0, 1)
        _ColorC ("ColorC", Color) = (1, 0, 0, 1)
        _ColorD ("ColorD", Color) = (1, 0, 0, 1)
        _UVSpeed ("UV Speed", Range(0.0, 1.0)) = 0.01
        _Speed ("Speed", Range(0.0, 10.0)) = 0.01
        _RandomSize ("Random Size", Range(0.0, 10.0)) = 1.0
        _Seed ("Seed", Range(0.0, 10.0)) = 1.0
    }
    SubShader
    {
        CGINCLUDE
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
            float4 _ColorA;
            float4 _ColorB;
            float4 _ColorC;
            float4 _ColorD;
            float _Speed;
            float _RandomSize;
            float _Seed;
            float _UVSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv + float2(_Time.y * _UVSpeed, 0.0);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            { 
                // float2 st = i.uv;
                // float4 color = _Color;
                // float3 rgb = color.xyz;

                // float t = 1.0;
                // t = abs(10.0-sin(_Time.y*_Speed))*10.0;
                // st += gradientNoise(st*12.0, 0.0) * t; // Animate the coordinate space
                // color.xyz = float3(1.0, 1.0, 1.0) * smoothstep(0.18, 0.2,gradientNoise(st, 0.0)); 
                //                             // Big black drops
                // color.xyz += smoothstep(0.15, 0.2,gradientNoise(st*10.0, 5.0)); 
                //                             // Black splatter
                // color.xyz -= smoothstep(0.35, 0.4,gradientNoise(st*10.0, 10.0)); 
                //                             // Holes on splatter
                
                // return float4(color.rgb,0.5);
                float2 uv = i.uv;
                float3 color = _Color.rgb;
                float3 colA = _ColorA.rgb;
                float3 colB = _ColorB.rgb;
                float3 colC = _ColorC.rgb;
                float3 colD = _ColorD.rgb;
                
                float2 q = float2(0.0, 0.0);
                q.x = fbm(uv);
                q.y = fbm(uv + float2(1.0, 1.0));

                float2 r = float2(0.0, 0.0);
                r.x = fbm( uv + 3.552*q + float2(-0.520,0.500)+ 0.042*_Time.y);
                r.y = fbm( uv + 2.112*q + float2(-0.550,0.350)+ 0.014*_Time.y);

                float f = fbm(uv + r);

                color = lerp(colA, colB, clamp((f*f)*5.256,1.552,3.432));
                color = lerp(color, colC, clamp(length(q), 0.936, 2.032));
                color = lerp(color, colD, clamp(length(r.x), 0.064, 1.392));
                float4 result = float4((f*f*f + 0.200*f*f + 1.420*f)*color,1.0);
                return result;
            }
            

        ENDCG        

        Pass
        {
            Tags { "RenderType"="Transparent" }
            //Blend SrcAlpha OneMinusSrcAlpha
            //LOD 100

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }
    }
}
