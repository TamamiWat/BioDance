Shader "Custom/Particle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Size("Size", Float) = 1.0
    }

    CGINCLUDE
        #include "UnityCG.cginc"

        struct Particle
        {
            float3 position;
            float3 velocity;
            float lifetime;
        };
        StructuredBuffer<Particle> _particle;

        struct appdata
        {
            uint vertex : SV_VertexID;
            float2 uv : TEXCOORD0; 
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float4 color : COLOR;
            float2 uv : TEXCOORD0; 
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _Color;
        float _Size;

        v2f vert (appdata v)
        {
            v2f o;
            Particle p = _particle[v.vertex];
            float4 pos = float4(p.position, 1.0);

            // Apply size scaling
            pos.xyz += _Size;

            // Transform to clip space
            o.vertex = UnityObjectToClipPos(pos);

            // Set color and alpha based on lifetime
            float alpha = saturate(p.lifetime); // Ensure alpha is between 0 and 1
            o.color = _Color * alpha;
            o.uv = v.uv;

            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.uv) * i.color;
            return col;
        }
    ENDCG

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            ENDCG
        }
    }
}