Shader "Unlit/DrawNormalLine"
{

    Properties {
   _EdgeControl ("Edge Control", Range(0, 1)) = 0.5
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
            float4 _NormalTex_TexelSize;

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

half sobel(half3 tex,float2 texel,float2 uv)
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
    half Texcolor = luminance(SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, offsetuv).xyz);
    edgex += Texcolor * Gx[j];
    edgey += Texcolor * Gy[j];
}

half edge= abs(edgex) + abs(edgey);
return edge;
}

            float4 frag(Varings i) : SV_Target { 
                float3 c = SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, i.uv).xyz; 
                half edge= sobel(c, _NormalTex_TexelSize.xy,i.uv);
             //   return float4(c,1);
                return float4(step(edge,_EdgeControl).xxx,1);
               // return float4(lerp(c,float3(0,0,0),edge),1);
            }
            ENDHLSL
        }
    
}
}
