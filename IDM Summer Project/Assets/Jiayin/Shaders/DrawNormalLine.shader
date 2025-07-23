Shader "Unlit/DrawNormalLine"
{

    Properties {
   _EdgeControl ("Edge Control", Range(0, 10)) = 0.5
   _EdgeColor("EdgeColor",Color)=(1,1,1,1)
   _PixelCount("PicxelCount",Range(0,2048)) =100
    }

    SubShader {
        Tags {
            "RenderPipeline" = "UniversalPipeline" 
            "RenderType" = "Opaque"
        }

        HLSLINCLUDE
        
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

  
        CBUFFER_START(UnityPerMaterial)
        float _EdgeControl;
        float3 _EdgeColor;
        uint _PixelCount;
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
            TEXTURE2D(_SColorTex);
            SAMPLER(sampler_SColorTex);
            float4 _NormalTex_TexelSize;
            float4 _DepthTex_TexelSize;
            
            Varings vert(Attributes v) { 
                Varings o;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = positionInputs.positionCS;

                o.uv = v.uv;
                return o;
            }

half luminance(half3 c) {
    return dot(c, half3(0.2125, 0.7154, 0.0721));
}

half sobel(TEXTURE2D_PARAM(mtex, mtexSampler), float2 texel,float2 uv)
{
    const half Gx[9]= {
        -1, 0, 1,
        -2, 0, 2,
        -1, 0, 1
    };
    const half Gy[9]= {
        -1, -2, -1,
        0, 0, 0,
        1, 2, 1
    };

    half edgex=0;
    half edgey=0;

for(int j=0;j<9;j++)
{
    half2 offsetuv = uv+ half2((j % 3) - 1, (j / 3) - 1) *texel;
    half Texcolor = luminance(SAMPLE_TEXTURE2D(mtex, mtexSampler, offsetuv).xyz);
   // half Texcolor =SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, offsetuv).z;
    edgex += Texcolor * Gx[j];
    edgey += Texcolor * Gy[j];
}

half edge=max( abs(edgex) , abs(edgey));
return edge;
}

            float4 frag(Varings i) : SV_Target { 
              float2 pixeluv=floor(i.uv*_PixelCount)/_PixelCount;
             float3 pixelSceneColor=SAMPLE_TEXTURE2D(_SColorTex,sampler_SColorTex,pixeluv).xyz;

                float3 normal = SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, pixeluv).xyz; 
                float3 depth=SAMPLE_TEXTURE2D(_DepthTex,sampler_DepthTex,i.uv).xyz;
                float3 sceneColor=SAMPLE_TEXTURE2D(_SColorTex,sampler_SColorTex,i.uv).xyz;
              //  return half4(sceneColor,1);
                half edge= sobel(TEXTURE2D_ARGS(_NormalTex, sampler_NormalTex), _NormalTex_TexelSize.xy,pixeluv);
              //  edge=sobel(TEXTURE2D_ARGS(_DepthTex,sampler_DepthTex),_DepthTex_TexelSize.xy,i.uv);
             //   return float4(depth,1);
             edge=step(edge,_EdgeControl);


                return float4(lerp(pixelSceneColor,_EdgeColor,saturate(1-edge)),1);
             // return float4(edge.xxx,1);
               // return float4(lerp(normac,float3(0,0,0),edge),1);
            }
            ENDHLSL
        }
    
}
}
