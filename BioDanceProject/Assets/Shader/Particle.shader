Shader "Custom/Particle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("COLOR", Color) = (1, 1, 1, 1)
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
            float4 color : COLOR;
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _Color;

        v2f vert (appdata v)
        {
            v2f o;
            Particle p = _particle[v.vertex];
            o.vertex = UnityObjectToClipPos(p.position);
            //calc alpha use lifetime
            o.color.a = p.lifetime; 
            o.color.rgb = _Color; 
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.uv);
            return col*i.color;
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
