Shader "Custom/SkyCloud"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color0 ("Color0", Color) = (1,1,1,1)
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2("Color2",Color)=(1,1,1,1)
        _Color3("Color3",Color)=(1,1,1,1)
        _HeightFactor("Height Factor", float) = 1
        _HeightMove("Height Move",float) = 0
        _HeightTex("Height Texture", 2D) = "white" {}
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _StylishShadow ("Stylish Shadow", Float) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
        _ShadowColor2("ShadowColor2",color)=(0.5,0.5,0.5,0.5)
        _OutLineColor("OutlineColor",Color)=(0,0,0,1)
        _OutLineBlend("OutlineBlend",Range(0,1)) = 1  

        _NoiseScale("NoiseScale",float)=1
        _CloudSpeed("CloudSpeed",float)=1
        _CloudDensity("CloutDensity",float)=1
        _ClipDensity("ClipDensity",float)=1
        _DensityScale("density scale",float)=2
        _DensitySpeed("densitySpeed",float)=1
        _VertexOffsetDensity("_VertexOffsetDensity",float)=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline" 
            "Queue"="Transparent"  "DecalMeshForwardEmissive" = "True" }
        //LOD 200
        Cull off
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "../ShaderTools/NoiseLib.hlsl"
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float3 tangent:TANGENT;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
                float4 screenUV : TEXCOORD4; 

                float3 debugvalue:TEXCOORD5;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color0;
                float4 _Color1;
                float4 _Color2;
                float4 _Color3;
                float _ShadowThreshold;
                float4 _ShadowColor;
                float4 _ShadowColor2;
                float _StylishShadow;//shadow edge 
                float4 _OutLineColor;
                float _OutLineBlend;
                //height mix
                float _HeightFactor;
                float _HeightMove;

                float4 _HeightTex_ST;

                //cloud
                float _NoiseScale;
                float _CloudSpeed;
                float _CloudDensity;
                float _ClipDensity;
                float _DensityScale;
                float _DensitySpeed;
                float _VertexOffsetDensity;
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalLineTex);
            SAMPLER(sampler_NormalLineTex);
            TEXTURE2D(_HeightTex); SAMPLER(sampler_HeightTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
               
                //Cloud vertex move
                float noise=FBMvalueNoise(IN.uv*_NoiseScale+_CloudSpeed*_Time.y);              
        //      float densityNoise=FBMvalueNoise(IN.uv*_DensityScale+_DensitySpeed*_Time.y);
                float cloudNoise=saturate(noise);
              // IN.positionOS.xyz+=IN.normalOS.xyz*cloudNoise*_VertexOffsetDensity;
                float3 pos=IN.positionOS.xyz+IN.normalOS.xyz*cloudNoise*_VertexOffsetDensity;
                //recalcutate normal
            float2 du = float2(0.01, 0);
            float2 dv = float2(0, 0.01);   
            float3 Binormal=cross(IN.normalOS,IN.tangent);

             float3 posU=(IN.positionOS.xyz+0.01*IN.tangent)+(IN.normalOS.xyz*saturate(FBMvalueNoise(IN.uv+du)*_NoiseScale+_CloudSpeed*_Time.y));
             float3 posV=(IN.positionOS.xyz+0.01*Binormal)+(IN.normalOS.xyz*saturate(FBMvalueNoise(IN.uv+dv)*_NoiseScale+_CloudSpeed*_Time.y));
            
            // float3 posU=(IN.positionOS.xyz+0.1*IN.tangent);
            // float3 posV=(IN.positionOS.xyz+0.1*Binormal);
            
            float3 modifyTangent=posU-pos;
            float3 modifyBitangent=posV-pos;
            float3 modifiedNormal=normalize(cross((modifyBitangent),(modifyTangent)));
            
        //    OUT.debugvalue=normalize(modifiedNormal);

                OUT.positionWS = TransformObjectToWorld(pos);
                OUT.normalWS = TransformObjectToWorldNormal(modifiedNormal);
                OUT.uv = IN.uv ;
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                OUT.screenUV=ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
               // return float4 (IN.normalWS,1);

           //     return float4(cloudNoise.xxx,heightMap*cloudNoise);
            
                //Color
                float heightTex=SAMPLE_TEXTURE2D(_HeightTex, sampler_HeightTex, IN.uv*_HeightTex_ST.xy+_HeightTex_ST.zw+_Time.y*0.1).r;
                _Color2.rgb = lerp(_Color2.rgb, _Color3.rgb, saturate(heightTex));
                _Color0.rgb = lerp(_Color0.rgb, _Color1.rgb, saturate(heightTex));
                float3 positionOS= TransformWorldToObject(IN.positionWS);
                 float heightMap=positionOS.y*_HeightFactor+_HeightMove;
                float Grassmask =saturate( floor(heightMap*heightTex*10/3)/2);
                float3 BaseColor= lerp(_Color0.rgb, _Color2.rgb, Grassmask);
                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb*BaseColor;

                //Cloud Noise
                float noise=FBMvalueNoise(IN.uv*_NoiseScale+_CloudSpeed*_Time.y);              
                float densityNoise=FBMvalueNoise(IN.uv*_DensityScale+_DensitySpeed*_Time.y);
              //  float cloudNoise=saturate(step(_CloudDensity,pow(noise*densityNoise, 0.5)));
                float cloudNoise=saturate(pow(noise*densityNoise,_CloudDensity));
                
      
               
                float cloudAlpha=heightMap*cloudNoise*heightTex;    
                clip(1-(1-cloudAlpha)*_ClipDensity);
                
                Light mainLight = GetMainLight(IN.shadowCoord);
              //  return half4(albedo,1);
               //point light support
                half3 totalLight = mainLight.color.rgb * mainLight.distanceAttenuation;
                #if defined(_ADDITIONAL_LIGHTS)
                uint lightCount = GetAdditionalLightsCount();
                for (uint i = 0; i < lightCount; ++i)
                {
                    Light addLight = GetAdditionalLight(i, IN.positionWS);
                    totalLight += addLight.color.rgb * addLight.distanceAttenuation;
                }
                #endif

                // Toon shadow
                half NdotL = saturate(dot(IN.normalWS, mainLight.direction));
               // return half4(NdotL.xxx,1);
              //  half shadowAtten = mainLight.shadowAttenuation;
                half shadowAtten=MainLightRealtimeShadow(IN.shadowCoord);
         //      return half4(NdotL.xxx,1);
                half toonStep1 = NdotL * shadowAtten > _ShadowThreshold ? 1.0: 0.0;
                half toonStep2 = NdotL * shadowAtten>_ShadowThreshold + _StylishShadow ? 1.0 : 0.0;
          
                //decal support
             //   float2 decaluv = IN.screenUV.xy / IN.screenUV.w;
              //  float4 decal0 = SAMPLE_TEXTURE2D(_DBufferTexture0, sampler_DBufferTexture0, decaluv);
             
              

                float4 shadowColor=lerp(_ShadowColor, _ShadowColor2, NdotL * shadowAtten);
                half3 baseColor = lerp(shadowColor.rgb*albedo*totalLight, albedo * totalLight, toonStep2);
              //  baseColor = lerp(baseColor,_OutLineColor.rgb,(1-normalLine)*_OutLineBlend);
              //  ApplyDecalToBaseColor(IN.positionCS, baseColor);
                return half4(baseColor*cloudAlpha,  cloudAlpha);
            }
            ENDHLSL
        }

        // 阴影投射
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
    //    UsePass "Universal Render Pipeline/Lit/ShadowCaster"

    }
    FallBack "Universal Render Pipeline/Lit"
}