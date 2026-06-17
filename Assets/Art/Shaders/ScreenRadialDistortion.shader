Shader "Hidden/ProjectHunter/ScreenRadialDistortion"
{
    Properties
    {
        _Strength ("Strength", Float) = 0
        _Radius ("Radius", Float) = 0.45
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

        ZWrite Off
        Cull Off
        ZTest Always

        Pass
        {
            Name "ScreenRadialDistortion"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Strength;
            float _Radius;
            float2 _Center;

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                return half4(1, 0, 0, 1);

                float2 uv = input.texcoord.xy;
                float2 toPixel = uv - _Center;
                float distanceFromCenter = length(toPixel);
                float radius = max(_Radius, 0.0001);
                float mask = smoothstep(radius, 0.0, distanceFromCenter);
                float2 direction = distanceFromCenter > 0.0001 ? toPixel / distanceFromCenter : 0.0.xx;
                float2 sampleUv = saturate(uv + direction * mask * _Strength);

                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, sampleUv);
            }
            ENDHLSL
        }
    }
}
