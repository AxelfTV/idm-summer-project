Shader "Unlit/ScreenLight"
{
    Properties
    {
        _MainTex ("Shafts (RG channels)", 2D) = "white" {}
        _Tint ("Shaft Tint", Color) = (1,1,1,1)
        _BlendSpeed ("Blend Speed", Float) = 1.0
        _Strength("Strength",float)=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        Cull Off
        ColorMask RGBA
        Blend one one

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Tint;
                float _BlendSpeed;
                float _Scale;
                float _Strength;
            CBUFFER_END

         struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 tangentOS:TANGENT;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 screenUV : TEXCOORD4; 

                float3 tangentWS : TEXCOORD6;
                float3 binormalWS : TEXCOORD7;
            };


            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv ;
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);

                OUT.screenUV=ComputeScreenPos(OUT.positionCS);

                OUT.tangentWS=normalize(TransformObjectToWorldDir(IN.tangentOS.xyz));
                OUT.binormalWS=cross(OUT.normalWS,OUT.tangentWS)*IN.tangentOS.w;
                return OUT;
            }

             float4 frag(Varyings IN) : SV_Target
            {
            

                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                float t = (sin(_Time.y * _BlendSpeed) * 0.5) + 0.5; 
                float mask = lerp(tex.r, tex.g, t);
                float srcAlpha = mask;
               
                _Strength*=sin(_Time.y)*0.5+0.5;
         //       float3 outRGB = _Tint.rgb*_Strength*mask+SceneColor.rgb;

                 return float4(_Tint.rgb*_Strength*mask, 1);
            }

            ENDHLSL
        } 
    } 

    FallBack "Universal Render Pipeline/Lit"
}
 
