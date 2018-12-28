#ifndef EFFECTSCOMMON_INCLUDED
#define EFFECTSCOMMON_INCLUDED

sampler2D _MainCameraDepthTexture;

#define BLOOM_VERTEX_DECLARATION(index) float4 scrPos	: TEXCOORD##index;
#define BLOOM_VERTEX_COMPUTE(o)  o.scrPos = ComputeScreenPos(o.vertex); \
												COMPUTE_EYEDEPTH(o.scrPos.z);
#define BLOOM_FRAGMENT_COMPUTE(i) float depth = tex2Dproj(_MainCameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r; \
									depth = LinearEyeDepth(depth); \
									if (i.scrPos.z - depth >  -0.001f)discard;

#endif