// Upgrade NOTE: upgraded instancing buffer 'MyProperties' to new syntax.

Shader "Unlit/ParticleShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
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
            #pragma multi_compile_instancing


            #include "UnityCG.cginc"

            struct Particle
            {
                float3 position;
                float4 color;
                float scale;
            };

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 color : COLOR;
            };

            UNITY_INSTANCING_BUFFER_START (MyProperties)
            UNITY_DEFINE_INSTANCED_PROP (float4, _Color)
#define _Color_arr MyProperties
            UNITY_INSTANCING_BUFFER_END(MyProperties)

            StructuredBuffer<Particle> _ParticleBuffer;

            v2f vert (appdata v, uint instanceId : SV_InstanceID)
            {
                Particle p = _ParticleBuffer[instanceId];
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                float3 pos = (v.vertex.xyz * p.scale) + p.position;
                o.vertex = mul(UNITY_MATRIX_VP, float4(pos, 1.0));
                o.color = p.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                return UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                //return i.color;
            }
            ENDCG
        }
    }
}
