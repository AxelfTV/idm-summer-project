Shader "Unlit/MeshFog"
{
Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _WaterSurface("WaterSurface",2D)= "white" {}
        _WaterNormal("WaterNormal",2D)="white"{}
        _NormalStrength("normal strength",float)=0.5
        _Color ("FresnelColor", Color) = (1,1,1,1)
        _LightColor("LightColor", Color) = (0,0,1,1)
        _WaterGradient ("Water Gradient", 2D) = "white" {}
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _StylishShadow ("Stylish Shadow", Float) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
        _ShadowColor2("ShadowColor2",color)=(0.5,0.5,0.5,0.5)
        _FadeDistance("Fade Distance", Float) = 10.0
        _WeaveStrength("Weave Strength",float)=1

        _FogCut("FogCut",float)=0.5

//foam
        _FoamSpeed("FoamSpeed",float)=1
        _FoamDistance("FoamDistance",float)=1
        _FoamCutoff("FoamCutOff",float)=0.5
        _FoamStrength("FoamStrength",float)=0.2

//interact object

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline" 
            "Queue"="Transparent" "DecalMeshForwardEmissive" = "True" }
        //LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "../ShaderTools/NoiseLib.hlsl"
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS:TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 tangentWS : TEXCOORD2;
                float3 binormalWS : TEXCOORD3;
                float3 positionWS : TEXCOORD4;
                float4 shadowCoord : TEXCOORD5;
                float4 screenUV : TEXCOORD6; 

            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float3 _LightColor;
                float _ShadowThreshold;
                float4 _ShadowColor;
                float4 _ShadowColor2;
                float _StylishShadow;//shadow edge 
                float _FadeDistance;
                float _WeaveStrength;
                float _NormalStrength;

                //foam
                float _FoamSpeed;
                float _FoamDistance;
                float _FoamCutoff;
                float _FoamStrength;
                //interact objects

                float _FogCut;
                     
                float4 _WaterNormal_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_WaterGradient); SAMPLER(sampler_WaterGradient);
            TEXTURE2D(_WaterSurface); SAMPLER(sampler_WaterSurface);
            TEXTURE2D(_WaterNormal); SAMPLER(sampler_WaterNormal);
//            TEXTURE2D(_CameraDepthTexture);       SAMPLER(sampler_CameraDepthTexture);

            // float4 ComputeScreenPos(float4 positionCS,float ProjectionSign)
            // {
            //     float4 screenPos = positionCS*0.5f;
            //     screenPos.xy = (screenPos.x,screenPos.y*ProjectionSign) + screenPos.w; // Convert to [0,1] range
            //     screenPos.zw=positionCS.zw;
            //     return screenPos;
            // }
            // float SampleSceneDepth(float2 uv)
            // {
            //     // Sample the scene depth texture
            //     float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture,uv).r;
            //     return depth;
            // }

            // float GetDepthFade(float3 worldPos, float fadeDistance)
            // {
            //     float4 positionCS = TransformWorldToHClip(worldPos);
            //     float4 ScreenPosition = ComputeScreenPos(TransformWorldToHClip(worldPos));
            //     float EyeDepth = LinearEyeDepth(SampleSceneDepth(ScreenPosition.xy/ScreenPosition.w),_ZBufferParams);
            // //    return ScreenPosition.y/ ScreenPosition.w;
            //     return EyeDepth;
            //     return saturate((EyeDepth - positionCS.w) / fadeDistance);
            // }
            float2 DistortUV_float(float2 UV, float Amount)
            {
                float time = _Time.y;

                UV.y += Amount * 0.01 * (sin(UV.x * 3.5 + time * 0.35) + sin(UV.x * 4.8 + time * 1.05) + sin(UV.x * 7.3 + time * 0.45)) / 3.0;
                UV.x += Amount * 0.12 * (sin(UV.y * 4.0 + time * 0.50) + sin(UV.y * 6.8 + time * 0.75) + sin(UV.y * 11.3 + time * 0.2)) / 3.0;
                UV.y += Amount * 0.12 * (sin(UV.x * 4.2 + time * 0.64) + sin(UV.x * 6.3 + time * 1.65) + sin(UV.x * 8.2 + time * 0.45)) / 3.0;
                return UV;
            }

            float3 GerstnerWave(float3 position, float steepness, float wavelength, float speed, float direction, inout float3 tangent, inout float3 binormal)
{
    direction = direction * 2 - 1;
    float2 d = normalize(float2(cos(3.14 * direction), sin(3.14 * direction)));
    float k = 2 * 3.14 / wavelength;                                           
    float f = k * (dot(d, position.xz) - speed * _Time.y);
    float a = steepness / k;

    tangent += float3(
    -d.x * d.x * (steepness * sin(f)),
    d.x * (steepness * cos(f)),
    -d.x * d.y * (steepness * sin(f))
    );

    binormal += float3(
    -d.x * d.y * (steepness * sin(f)),
    d.y * (steepness * cos(f)),
    -d.y * d.y * (steepness * sin(f))
    );

    return float3(
    d.x * (a * cos(f)),
    a * sin(f),
    d.y * (a * cos(f))
    );
}

float3 GerstnerWaves_float(float3 position, float steepness, float wavelength, float speed, float4 directions)
{
    float3 Offset = 0;
    float3 tangent = float3(1, 0, 0);
    float3 binormal = float3(0, 0, 1);

    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.x, tangent, binormal);
    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.y, tangent, binormal);
    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.z, tangent, binormal);
    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.w, tangent, binormal);

  //  normal = normalize(cross(binormal, tangent));
    return Offset;
    //TBN = transpose(float3x3(tangent, binormal, normal));
}
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionWS += GerstnerWaves_float(OUT.positionWS, 0.1*_WeaveStrength, 0.5*_WeaveStrength, 0.1*_WeaveStrength, float4(0.25, 0.5, 0.75, 1.0));
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.tangentWS=normalize(TransformObjectToWorldDir(IN.tangentOS.xyz));
                OUT.binormalWS=cross(OUT.normalWS,OUT.tangentWS)*IN.tangentOS.w;
                OUT.uv = IN.uv ;
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                OUT.screenUV=ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {

                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS);
      
                float2 uv=IN.screenUV.xy/IN.screenUV.w;
                float wavenoise= FBMvalueNoise(IN.uv*30+_Time.y*0.2);
                wavenoise*=_FogCut;
                float depth=SampleSceneDepth(uv);                
                float eyedepth=LinearEyeDepth(depth,_ZBufferParams);
    
                float Waterdepth=1-saturate((eyedepth-IN.screenUV.w)/_FadeDistance);
                //mix water color
                float4 waterColor =float4( wavenoise.xxx,1);
                waterColor.a=(1-Waterdepth)*0.5;

                float3 WaterSurface = SAMPLE_TEXTURE2D(_WaterSurface, sampler_WaterSurface, DistortUV_float(IN.uv,1)).rgb;
                
             //   waterColor.rgb = lerp(waterColor.rgb,_WaterColor , step(0.05,WaterSurface.b));
                float FoamDepth=1-saturate((eyedepth-IN.screenUV.w)/_FoamDistance);
                float2 foamUV=float2(uv.x, FoamDepth+_Time.y*_FoamSpeed);
                float3 foamTex= SAMPLE_TEXTURE2D(_WaterSurface, sampler_WaterSurface, foamUV).rgb;
                float fomes=step(_FoamCutoff,FoamDepth*(foamTex.r)*_FoamStrength);
                waterColor+=fomes;
                 //fresnel
                float fresnel = pow(1.0 - saturate(dot(IN.normalWS, viewDir)), 8); 
                waterColor.rgb = lerp(waterColor.rgb, _Color.rgb, fresnel);
              //  return half4(waterColor);
           
           
           
                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb*_Color;
                Light mainLight = GetMainLight(IN.shadowCoord);
                
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

                // lighting

                float3 normalTS=SAMPLE_TEXTURE2D(_WaterNormal,sampler_WaterNormal,float2(IN.uv.x+_Time.y*0.1,IN.uv.y)*_WaterNormal_ST.xy+_WaterNormal_ST.zw).xyz*2-1;
                normalTS = normalize(lerp(float3(0,0,1), normalTS, _NormalStrength));
                float3x3 TBN=float3x3(IN.tangentWS, IN.binormalWS,IN.normalWS);
                float3 normalWS=normalize(mul(normalTS,TBN));
              //  return float4(normalWS,1);
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half shadowAtten=MainLightRealtimeShadow(IN.shadowCoord);
                float shadow=step(0.4,NdotL*shadowAtten);

                //apply light
                waterColor.rgb*=_LightColor;
                return float4(waterColor);

            }
            ENDHLSL
        }

        // 阴影投射
        // UsePass "Universal Render Pipeline/Lit/DepthOnly"
        // UsePass "Universal Render Pipeline/Lit/DepthNormals"
        // UsePass "Universal Render Pipeline/Lit/ShadowCaster"

    }
}
