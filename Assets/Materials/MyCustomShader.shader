Shader "Custom/MyCustomShader"
{
    Properties
    {
        // Property Dasar (Texture & Warna Dasar)
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        
        // --- SYARAT TUGAS: Property untuk Animasi Script ---
        // [HDR] biar warnanya bisa terang banget (Glow)
        [HDR] _EmissionColor("Emission Color", Color) = (0,0,0,0) 
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
                half4 _BaseColor;
                float4 _BaseMap_ST;
                // Deklarasi variabel Emission agar bisa dibaca GPU
                half4 _EmissionColor; 
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
                // Ambil warna dari Texture
                half4 textureColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                
                // Gabungkan: Texture * Warna Dasar + Emission (Animasi)
                half4 finalColor = (textureColor * _BaseColor) + _EmissionColor;
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}