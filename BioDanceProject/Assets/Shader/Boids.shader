Shader "Custom/Boids"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Texture ("Texture", 2D) = "white"{}
        _Seed ("Seed", Int) = 0
        _SizeX ("SizeX", Int) = 1
        _SizeY ("SizeY", Int) = 1        
    }

    CGINCLUDE
        #include "UnityCG.cginc"
        #include "CalcFunc.cginc"

        struct BoidData{
            float3 velocity;
            float3 position;
            float4 color;
        };

		StructuredBuffer<BoidData> _BoidDataBuffer;
		

        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };


        struct v2f
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 color : COLOR;
            UNITY_FOG_COORDS(1)
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        UNITY_INSTANCING_BUFFER_START(Props)
                // メッシュごとに変えるプロパティはここに定義する
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

        //fixed4 _Color;
        float3 _ObjectScale;
        sampler2D _MainTex;
        float4 _MainTex_ST;
        int _Seed;
        int _SizeX;
        int _SizeY;

        v2f vert(appdata v, uint id : SV_InstanceID)
        {   
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            float4x4 o2w = (float4x4)0;
            BoidData boidData = _BoidDataBuffer[id];
            float3 pos = boidData.position;
            float3 scale = _ObjectScale;

            // Define a matrix to convert from object coordinates to world coordinates
            
            o2w._11_22_33_44 = float4(scale.xyz, 1.0); //input scale

            //calculate rotation from velocity
            float rotY = 
				atan2(boidData.velocity.x, boidData.velocity.z);
            float rotX = 
				-asin(boidData.velocity.y / (length(boidData.velocity.xyz) + 1e-8));
            
            float4x4 rotMatrix = ConvertEulerToRotateMatrix(float3(rotX, rotY, 0));

            //apply rotation to matrix
            o2w = mul(rotMatrix, o2w);
            //apply shift to matrix
            o2w._14_24_34 += pos.xyz;
            v.vertex = mul(o2w, v.vertex);
            v.normal = normalize(mul(o2w, v.normal));
            UNITY_TRANSFER_INSTANCE_ID(v, o);
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            UNITY_TRANSFER_FOG(o, o.vertex);
            o.color = boidData.color;
            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            float4 col = i.color;
            col.a *= 0.2;
            return col;
        }
    ENDCG

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend OneMinusDstColor One
        LOD 200
        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            ENDCG
        }
    }
}
