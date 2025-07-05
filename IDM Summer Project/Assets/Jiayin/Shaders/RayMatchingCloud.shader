Shader "Hidden/PostProcessing/RayMatchingCloud"
{
    Properties
    {
        _BoundingsMin("Boundings Min", Vector) = (-10, -10, -10, 1)
        _BoundingsMax("Boundings Max", Vector) = (10, 10, 10, 1)
        _NoiseTex("NoiseTex",3D) = "white" {}
        _NoiseScale("NoiseScale",float)=1.0
        _Step("Step",float)=0.1
        _ColorA("Color A", Color) = (1, 1, 1, 1)
        _ColorB("Color B", Color) = (1, 1, 1, 1)
        _ColorOffsetA("Color Offset A", float) = 1.0    
        _ColorOffsetB("Color Offset B", float) = 1.0
    }
    SubShader
    {
        tags{"RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        Cull Off 
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D_X(_CameraOpaqueTexture);
			SAMPLER(sampler_CameraOpaqueTexture);
            TEXTURE2D_X_FLOAT(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            sampler3D _NoiseTex;

            CBUFFER_START(UnityPerMaterial)
            float _BlendMultiply;
            float4 _TintColor;
            float4x4 _ProjectionMatrix;
            float4x4 _I_ViewMatrix;

            float3 _BoundingsMin;
            float3 _BoundingsMax;

            float _NoiseScale;
            float  _Step;
            float3 _ColorA;
            float3 _ColorB; 
            float _ColorOffsetA;
            float _ColorOffsetB;
            CBUFFER_END


            float4 GetWorldPosition(float depth, float2 uv)
            {
                float4 view_vector = mul(_ProjectionMatrix, float4 (uv * 2.0 - 1.0, depth, 1.0));
                view_vector.xyz/= view_vector.w; // Perspective divide
                float4 world_vector = mul( _I_ViewMatrix,float4 (view_vector.xyz,1));
                return world_vector;
            }
            float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 rayDirection)
            {
                float3 invDir = 1.0 / rayDirection;
                float3 t0 = (boundsMin - rayOrigin) * invDir;
                float3 t1 = (boundsMax - rayOrigin) * invDir;
                float3 tmin = min(t0, t1);
                float3 tmax = max(t0, t1);

                float dstA = max(max(tmin.x, tmin.y), tmin.z); 
                float dstB = min(min(tmax.x, tmax.y), tmax.z); 

                float dstToBox=max(dstA, 0.0);
                float dstInsideBox=max((dstB-dstToBox),0)
;                return float2(dstToBox, dstInsideBox);
            }
            float SampleNoise(float3 uvw)
            {
                float3 noise =tex3D(_NoiseTex,uvw*_NoiseScale);

                return noise.x; 
            }
 float3 LightMatch(float3 position, float3 dstTravelled)
            {
                    Light mainLight = GetMainLight();
                    float3 lightDir = mainLight.direction;

                    float DstInsideBox=rayBoxDst(_BoundingsMin, _BoundingsMax, position, lightDir).y;
                   float stepSize=DstInsideBox/10;
                    float TotalDensity=0;

                    for(int step=0;step<8;step++)
                    {
                        position+=lightDir*stepSize;
                        TotalDensity+=max(0, SampleNoise(position+_Time.y)* stepSize);
                    }
                    float Transmittance = exp(-TotalDensity);

                    float3 CloudColor=lerp(_ColorA,mainLight.color,saturate(Transmittance*_ColorOffsetA));
                    CloudColor=lerp(_ColorB,CloudColor,saturate(pow(Transmittance*_ColorOffsetB,3)));
                    return CloudColor;
            } 
            float4 cloudRayMarching(float3 startPoint, float3 direction,float dstLimit) 
            {
                float sumDesity = 1;
                float dstTravelled=0;
                float stepSize=exp(_Step)*0.1;
                float3 LightEmerge=0;
                for(int j=0;j<32;j++)
                {
                    // if(dstTravelled<dstLimit)
                    // {
                    //     float3 rayPos=startPoint + direction * dstTravelled;
                    //     sumDesity += pow(SampleNoise(rayPos+_Time.y),5); 
                    // }
                    if(dstTravelled<dstLimit)
                    {                    
                        float3 rayPos=startPoint + direction * dstTravelled*stepSize;
                        float density = SampleNoise(rayPos+_Time.y);
                        if(density>0)
                        {
                            float3 lightTransmittance = LightMatch(rayPos, dstTravelled);
                            LightEmerge+= lightTransmittance * density * stepSize;
                            sumDesity*=exp(-density * stepSize); 

                            if(sumDesity<0.01)break;                           
                        }
                    }
                    dstTravelled +=stepSize; 
                }
                return float4(LightEmerge,sumDesity) ;
            }

                 

            float4 Frag(Varyings i) : SV_Target
            {
      
                float depth=SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture,  i.texcoord).r;
           //      return float4(depth.xxx,1);
                float4 worldPos =GetWorldPosition(depth, i.texcoord);

                 float3 rayPos = _WorldSpaceCameraPos;
        


                 float3 worldViewDir = normalize(worldPos.xyz - rayPos.xyz) ;
                 
                float depthEyeLiner=length(worldPos.xyz - _WorldSpaceCameraPos.xyz);
                 float2 rayToContainerInfo= rayBoxDst(_BoundingsMin, _BoundingsMax, rayPos, worldViewDir);
                float dstToBox= rayToContainerInfo.x;
                float dstInsideBox= rayToContainerInfo.y;

                float3 entryPoint=rayPos+ worldViewDir * dstToBox;

                float dstLimit=min(depthEyeLiner-dstToBox, dstInsideBox);
          //      return float4(dstLimit.xxxx);
                 float4 cloud = cloudRayMarching(entryPoint.xyz, worldViewDir,dstLimit);
                
                float4 color = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, i.texcoord);
                color = lerp(cloud,color,cloud.w);
                 return float4(color);
            }
            ENDHLSL
        }
    }
}