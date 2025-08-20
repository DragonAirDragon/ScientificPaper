Shader "Custom/InstancedIndirectUnlit"
{
    Properties { _Color("Color", Color) = (1,1,1,1) }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag

            // ВКЛЮЧАЕМ инстансинг и процедурный путь
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup

            #include "UnityCG.cginc"
            #include "UnityInstancing.cginc"

            fixed4 _Color;

            // Буфер матриц доступен только когда включен процедурный инстансинг
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            StructuredBuffer<float4x4> _Matrices;
            #endif

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f { float4 pos : SV_POSITION; };

            // Вызывается Unity перед вертекс-функцией на каждом инстансе
            void setup()
            {
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                // unity_InstanceID — индекс текущего инстанса
                float4x4 m = _Matrices[unity_InstanceID];
                unity_ObjectToWorld = m;
                unity_WorldToObject = unity_ObjectToWorld; // ок для без-масштабных, для общего случая можно инвертировать
                #endif
            }

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                v2f o;
                o.pos = mul(UNITY_MATRIX_VP, worldPos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
