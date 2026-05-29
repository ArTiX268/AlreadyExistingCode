Shader "Custom/shad_Bloom"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _BloomColor("BloomColor", Color) = (1.0, 1.0, 1.0, 1.0)
        _Threshold("Threshold", Range(0.0, 1.0)) = 0.5
        _Brightness("Brightness", Range(1.0, 20.0)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BloomColor;
                float _Threshold;
                float _Brightness;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                //Lum = 0.2125 * r + 0.7152 * g + 0.0722 * b
                float lum = color.x * 0.2125 + color.y * 0.7152 + color.z * 0.0722;
                if (lum >= _Threshold)
                {
                    color *= _Brightness;
                }

                return color;
            }
            ENDHLSL
        }
    }
}
