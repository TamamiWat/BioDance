Shader"Custom/Trail"
{
    Properties
    {
        _Width ("Width", Float) = 0.1
        _Color ("Color", Color) = (1,0,0,1)
        _ScaleX ("Scale X", Float) = 1
        _ScaleY ("Scale Y", Float) = 1
        _Speed ("Speed", Float) = 1
        _Life ("Life", Float) = 10
        _StartColor ("StartColor", Color) = (1, 1, 1, 1)
        _EndColor("EndColor", Color) = (0, 0, 0, 1)
    }
    CGINCLUDE
        #include "UnityCG.cginc"
        #define PI 3.141519265359

        struct appdata
        {
            uint vertex : SV_VertexID;
        };

        struct v2f
        {
            float4 vertex : SV_Position;
            float4 color : COLOR;
        };
        
        struct user
        {
            float3 pos;
        };

        StructuredBuffer<user> _UserInputBuffer;

        float4 _Color;
        int _VertexNum;
        float _ScaleX;
        float _ScaleY;
        float _Speed;
        float3 _TapPos;
        float _Life;
        float4 _StartColor;
        float4 _EndColor;



        v2f vert (appdata v)
        {
            uint id = v.vertex;
            float3 pos = _UserInputBuffer[id].pos;
            float4 position = float4(pos, 1);
            //float4 pos = _TapPos;
    
            v2f o;
            o.vertex = UnityObjectToClipPos(position);
            o.color = _Color;
            return o;
        }
        
        fixed4 frag(v2f i) : SV_Target
        {
            return _Color;
        }
        
    ENDCG
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Cull Off
            Lighting Off
            BlendOp Add, Add
            Blend One One, One One
            ZWrite On
            ZTest Always
            CGPROGRAM

            #pragma target 5.0
            #pragma vertex vert
            //#pragma geometry geom
            #pragma fragment frag
            ENDCG

        }        
    }
}
