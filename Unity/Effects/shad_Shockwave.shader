Shader "Custom/Shockwave"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _Radius("Radius", Range(0.0, 1.0)) = 0.2
        _Width("Width", Range(0.0, 0.5)) = 0.1
        _Aspect("Aspect", Vector) = (0.0, 0.0, 0.0, 0.0)
        _Center("Center", Vector) = (0.0, 0.0, 0.0, 0.0)
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" }


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
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float _Radius;
                float _Width;
                float4 _Aspect;
                float4 _Center;
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
                float2 dir = _Center.xy - IN.uv;
                float d = length(dir / _Aspect.xy) - _Radius;

                d *= 1. - smoothstep(0., _Width, abs(d));
                dir = normalize(dir);
                

                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv + dir * d);
                

                if (length(_Center.xy - IN.uv) + _Width * 0.1 < _Radius) // Greyscale
                {
                    color.xyz = (color.x + color.y + color.z) / 3; 
                }

                return color;
            }
            ENDHLSL
        }
    }
}
