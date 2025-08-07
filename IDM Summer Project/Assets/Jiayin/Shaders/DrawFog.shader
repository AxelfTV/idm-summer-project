Shader "Unlit/DrawFog"
{
    Properties
    {
         _PixelCount("PicxelCount",Range(0,2048)) =100
         _FogStart("FogStart",float)=1
         _FogEnd("FogEnd",float)=1
         _fogColor("fogColor",Color)=(1,1,1,1)
    }
   SubShader {
        Tags {
            "RenderPipeline" = "UniversalPipeline" 
            "RenderType" = "Opaque"
        }

        HLSLINCLUDE
        
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        CBUFFER_START(UnityPerMaterial)
        uint _PixelCount;
        float _FogStart;
        float _FogEnd;
        float3 _fogColor;
        CBUFFER_END

        ENDHLSL

        Pass {
            Tags { "LightMode" = "UniversalForward" } 

            HLSLPROGRAM

            #pragma vertex vert 
            #pragma fragment frag 

            struct Attributes { 
                float4 vertex : POSITION; 
                float2 uv : TEXCOORD0; 
            };

            struct Varings { 
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION; 
            };


            TEXTURE2D(_NormalTex);
            SAMPLER(sampler_NormalTex);
            TEXTURE2D(_DepthTex);
            SAMPLER(sampler_DepthTex);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _NormalTex_TexelSize;
            float4 _DepthTex_TexelSize;

            
            Varings vert(Attributes v) { 
                Varings o;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = positionInputs.positionCS;
                o.uv = v.uv;
                return o;
            }
             float4 frag(Varings i) : SV_Target { 
                float2 pixeluv=floor(i.uv*_PixelCount)/_PixelCount;
                float3 normal = SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, i.uv).xyz; 
                float3 depth=SAMPLE_TEXTURE2D(_DepthTex,sampler_DepthTex,i.uv).xyz;
                float3 cameraColor=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv).xyz;

      
                float3 worldPos = ComputeWorldSpacePosition(i.uv, depth.x, UNITY_MATRIX_I_VP);
             //   return float4(worldPos,1);
                float fog=(_FogEnd-worldPos.y)/(_FogEnd-_FogStart);
                fog=saturate(fog);
                cameraColor=lerp(cameraColor,_fogColor,fog);
  //              return float4(depth.xxx,1);
                return float4(cameraColor,1);
             }
            ENDHLSL
        }
    }
}
