Shader "Custom/BulletShader" // <--- Nama Shader BEDA
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 0, 0, 1) // Merah default
        _PulseSpeed("Pulse Speed", Float) = 10.0
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

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionHCS : SV_POSITION; };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _PulseSpeed;
            CBUFFER_END

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                // Rumus Matematika: Sinus waktu bikin naik turun (0 ke 1)
                float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5; 
                
                // Warna jadi terang gelap otomatis
                return _BaseColor * (0.5 + pulse); 
            }
            ENDHLSL
        }
    }
}