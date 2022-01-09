Shader "Unlit/Grid"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 0.1)
        _Thickness("Thickness", Range(0, 1)) = 0.1
        _Attenuation("Attenuation", Float) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha


        Pass
        {
            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(2)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            uniform float4 _MousePos;
            float4 _Color;
            float _Thickness;
            float _Attenuation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                float diffX = abs(i.worldPos.x - round(i.worldPos.x));
                float diffZ = abs(i.worldPos.z - round(i.worldPos.z));
                //if (diffX > _Thickness && diffZ > _Thickness)
                //    discard;

                float distanceToMouse = distance(_MousePos.xyz, i.worldPos);
                
                // sample the texture
                fixed4 col = _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                col.a *= saturate(1 / (distanceToMouse * distanceToMouse) * _Attenuation);

                col.a *= saturate(1 / min(diffX * (1 - _Thickness), diffZ * (1 - _Thickness)) * _Thickness);

                return col;
            }
            ENDCG
        }
    }
}
