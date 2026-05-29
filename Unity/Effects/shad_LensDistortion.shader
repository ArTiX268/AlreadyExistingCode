Shader "Custom/shad_LensDistortion"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _Strength("Strength", Range(1.0, 5.0)) = 1.0
        _Zoom("Zoom", Range(1.0, 10.0)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

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
                float _Strength;
                float _Zoom;
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
                float2 uv = IN.uv * 2.0 - 1.0;
                float len = length(uv);

                len = pow(len, _Strength);
                uv = normalize(uv);
                uv *= len;

                uv /= _Zoom;
                uv = 0.5 * (uv + 1.0);

                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                return color;
            }
            ENDHLSL
        }
    }
}
