Shader "Unlit/Rocks"
{
    Properties
    {
        _Noise("Noise", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _RandomTranslation("Random Translation", Float) = 1
        _Scale("Scale", Float) = 1
        _Cutoff("Cutoff", Range(0, 1)) = 0.1
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
            #pragma geometry geom
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            
            #include "RocksBase.cginc"

            float4 _Color;
            
            fixed4 frag(g2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Color;
                float3 normal = i.normal;
                // apply fog

                float atten = UNITY_SHADOW_ATTENUATION(i, i.worldPos);
                half diffuse = saturate(saturate(dot(normal, _WorldSpaceLightPos0)));
                half specular = 0;
                //col *= max((diffuse + specular + modelDiffuse) * atten * _LightColor0, UNITY_LIGHTMODEL_AMBIENT * _Ambient);
                //col = saturate(col);


                col *= max((diffuse + specular) * atten * _LightColor0, UNITY_LIGHTMODEL_AMBIENT);

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "RocksBase.cginc"

            float4 frag(g2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }

            ENDCG
        }
    }
}
