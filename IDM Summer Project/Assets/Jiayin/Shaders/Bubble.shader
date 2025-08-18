Shader "Unlit/Bubble"
{
    
    Properties
    {
        _Color("Color",Color)=(1,1,1,1)
        _FresnelColor("FresnelColor",Color)=(1,1,1,1)
        _FresnelPower("FresnelPower",float)=4
        _TwistSpeed("TwistSpeed",float)=1
        _TwistAmount("TwistAmount",float)=0.5
        _BubbleLife("BubbleLife",Range(0,1))=0

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline" 
            "Queue"="Transparent" "UniversalMaterialType"="Lit" }
        //LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
       //     Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "../ShaderTools/NoiseLib.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
                float4 screenUV : TEXCOORD4; 
            };

            CBUFFER_START(UnityPerMaterial)
            float _BubbleLife;

            float  _TwistSpeed;
            float _TwistAmount;

            float4  _Color;
            float4 _FresnelColor;
            float _FresnelPower;
            CBUFFER_END
            


            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float noise= FBMvalueNoise(IN.positionOS.xy*5+_Time.y*_TwistSpeed);
                IN.positionOS.xyz+=IN.normalOS*noise*_TwistAmount*(0.1+_BubbleLife);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv ;
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                OUT.screenUV=ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {

            float3 positionOS=TransformWorldToObject(IN.positionWS);
            float3 normalWS = normalize(IN.normalWS);
            float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS.xyz);
            float fresnel = pow(1.0 - saturate(dot(normalWS, viewDir)), _FresnelPower);

            float4 color = lerp(_Color, _FresnelColor, fresnel);

            float Noise=voronoiNoise(positionOS.xy*5*_BubbleLife+_Time.y);
            clip(Noise-_BubbleLife);

            float smallNoise=voronoiNoise(float2(IN.uv.x*15,IN.uv.y*5+_Time.y*_TwistSpeed))*(1-fresnel);
            float smallBubbleMask=step(0.03,smallNoise);
          //  return float4(smallBubbleMask.xxx,1);
            float4 finalColor=lerp(_FresnelColor,color,smallBubbleMask);
            return finalColor;
            

            }
            ENDHLSL
        }



    }
    FallBack "Universal Render Pipeline/Lit"
}
