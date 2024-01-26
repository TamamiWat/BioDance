Shader"Custom/Trail"
{
    Properties
    {
        _Width ("Width", Float) = 0.1
        _Color ("Color", Color) = (1,0,0,1)
        _ScaleX ("Scale X", Float) = 1
        _ScaleY ("Scale Y", Float) = 1
        _Speed ("Speed", Float) = 1
    }
    CGINCLUDE
        #include "UnityCG.cginc"
        #define PI 3.141519265359

        struct v2f
        {
            float4 vertex : SV_Position;
        };
        
        struct user
        {
            float3 pos;
        };

        StructuredBuffer<user> _InputBuffer;

        float4 _Color;
        int _VertexNum;
        float _ScaleX;
        float _ScaleY;
        float _Speed;
        float3 _TapPos;


        v2f vert (uint id : SV_VertexID)
        {
            float div = (float) id / _VertexNum;
            float4 pos = float4((div - 0.5) * _ScaleX + _TapPos.x,
                                        sin(div * 2 * PI + _Time.y * _Speed) * _ScaleY + _TapPos.y,
                                            _TapPos.z, 1);
            //float4 pos = _TapPos;
    
            v2f o;
            o.vertex = UnityObjectToClipPos(pos);
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
            CGPROGRAM

            #pragma target 3.5
            #pragma vertex vert
            //#pragma geometry geom
            #pragma fragment frag
            ENDCG

        }        
    }
}
