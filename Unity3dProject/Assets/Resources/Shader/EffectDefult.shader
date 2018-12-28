

Shader  "Effect/Default" 
{
    Properties
	{
		[HideInInspector]_AlphaCtrl("AlphaCtrl",range(0,1)) = 1
		[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)] 		_SrcFactor ("SrcFactor()", Float) = 5
		[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)] 		_DstFactor ("DstFactor()", Float) = 10

		[HideInInspector][Enum(UnityEngine.Rendering.CullMode)] 		_CullMode ("消隐模式(CullMode)", int) = 0
		[Enum(LessEqual,4,Always,8)]									_ZTestMode ("深度测试(ZTest)", int) = 4
		//[Toggle] _ZWrite ("写入深度(ZWrite)", int) = 0
		
		[Toggle] _RgbAsAlpha ("颜色输出至透明(RgbAsAlpha)", int) = 0
		[Toggle] _IsBillboard("Billboard（公告板）",float) = 0

		
        _Color ("Color", color) = (1,0,0,1)
        _Multiplier	("亮度",range(1,100)) = 1

        _Zoffset("Zoffset",float) = 0
		
        _MainTex ("MainTex", 2D) = "white" {}
        _MainTexRot ("Tex rotation", range(0,360) ) = 0
        _MainTexX ("TexUscroll", range(-10,10) ) = 0
        _MainTexY ("TexVscroll", range(-10,10)) = 0
		
		[Toggle] _IsMask("IsMask",float) = 0
		_MaskTex ("mask", 2D) = "white" {}
		_MaskTexRot ("mask_rotation", range(0,360)) = 0
		_MaskTexX ("maskUscroll", range(-10,10)) = 0
		_MaskTexY ("maskVscroll", range(-10,10)) = 0
		
		//[Header(Dissolve Settings)]
		 _IsDissolve("IsMask",float) = 0
        _DissolveTex ("dissolveTex", 2D) = "white" {}
        _Dissolve ("dissolveValue", Range(0, 1)) = 0
        _DissolveColor1("dissolveColor1",color) = (1,0,0,1)
		_DissolveColor1Strengh("dissolveColor1Strengh",Range(0,50)) = 1
        _DissolveColor2("dissolveColor2",color) = (0,0,0,1)
		_DissolveColor2Strengh("dissolveColor2Strengh",Range(0,50)) = 1
        _DissolveRot ("dissolve rotation", Range(0,360) ) = 0
		_DissolveX("DissolveX", Range(0,30)) = 0
		_DissolveY("DissolveY", Range(0,30)) = 0
        _DissolveEdge ("dissolve edge", Range(0, 0.8)) = 0.1
        _DissolveEdgeOffset ("dissolve edge Offset", Range(0,1)) = 0.5
        [Toggle] _DissolveAlpha ("dissolve alpha", int) = 0
       // [HideInInspector][Toggle] _UseVertexAlpha ("Use Vertex Alpha", int) = 0

		
		[Toggle] _IsFlow("IsFlow",float) = 0
		_FlowTex ("flow", 2D) = "black" {}
		_FlowTexRot ("flow rotation", Range(0,360) ) = 0
		_FlowScale ("flow value", Range(0, 2)) = 0
		[Toggle]_IsXY("IsXY",float) = 0
		_FlowScaleX("flow value x", Range(0, 2)) = 0
		_FlowScaleY("flow value y", Range(0, 2)) = 0
		_FlowTexX ("flowVscroll", Range(-10,10) ) = 0
		_FlowTexY ("flowUscroll",  Range(-10,10)) = 0

		_StencilComp("Stencil Comparison",Range(0,30)) = 8
		_Stencil("Stencil ID",Range(0,30)) = 0
		_StencilOp("Stencil Operation",Range(0,30)) = 0
		_StencilWtriteMask("Stencil Write Mask",Range(0,255))=255
		_StencilReadMask("Stencil Read Mask",Range(0,255))=255


		[HideInInspector]_PointCache("PointCache", 2D ) = "gray" {}//点缓存贴图,压缩后的贴图会严重影响坐标还原精度,建议用RGB24,另外Non Power of 2和 Mip Map当然也要关掉
      	[HideInInspector]_Scale("Scale",float) = 1//适配导入模型的缩放值
        [HideInInspector]_Range("Range",float) = 1//适配点缓存图片的Range值(在图片名字的结尾)
        [HideInInspector]_Samples("Frames",float) = 1//缓存的采样数，有时比实际采样帧数少一些会有更平滑的动画效果
        [HideInInspector]_Progress("Progress",Range(0,1)) = 0//动画播放进度,不用shader里的time是为了以后更方便控制
    }
	
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWtriteMask]
		}

		Blend [_SrcFactor] [_DstFactor]
		Cull [_CullMode]
		//ZWrite [_ZWrite]
		ZWrite off
		ZTest [_ZTestMode]

		Offset [_Zoffset],-1

		Pass
		{
			CGPROGRAM


			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma target 2.0
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "BloomCommon.cginc"

			#pragma multi_compile_instancing

			#pragma multi_compile _ MaskTex_On
			#pragma multi_compile _ FlowTex_On

			#pragma multi_compile _ PointCache_On
			#pragma multi_compile _ Dissolve_On

			#pragma multi_compile _	MultiplyBlend_On AddBlend_On
			#pragma multi_compile RT_PASS_OFF RT_PASS_ON

			float2 TransFormUV(float2 argUV,float4 argTM)
			{
				float2 result = argUV.xy * argTM.xy + (argTM.zw + float2(0.5,0.5) - argTM.xy * 0.5);
				return result;
			}

			half2 RotateUV(half2 uv,half uvRotate)
			{
				half2 outUV;
				half s;
				half c;
				s = sin(uvRotate/57.2958);
				c = cos(uvRotate/57.2958);
				
				outUV = uv - half2(0.5f, 0.5f);
				outUV = half2(outUV.x * c - outUV.y * s, outUV.x * s + outUV.y * c);
				outUV = outUV + half2(0.5f, 0.5f);
				return outUV;
			}

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;

				#if PointCache_On
					float2 uv_index:TEXCOORD1;
				#endif
               
				float4 vertexColor : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct fragData
			{
				float4 uv12 : TEXCOORD0;
				float4 uv34 : TEXCOORD1;

				#ifdef RT_PASS_ON
				BLOOM_VERTEX_DECLARATION(2)
				#endif
				
				float4 vertex : SV_POSITION;
				float4 vertexColor : COLOR;

				float4 worldPosition : TEXCOORD3;
                #ifdef SOFTPARTICLES_ON
					fixed4 projPos : TEXCOORD4;
                #endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			float _AlphaCtrl, _IsBillboard;

			float _Multiplier;

            sampler2D _MainTex;
		//	float4 _MainTex_ST;
			float _MainTexRot;
            float _MainTexX;
            float _MainTexY;

            float4 _ClipRect, _Color;

            #if MaskTex_On
				sampler2D _MaskTex;
				float4 _MaskTex_ST;
				float _MaskTexRot;
				float _MaskTexX;
				float _MaskTexY;
			#endif

			

			#if Dissolve_On	
				sampler2D _DissolveTex ;
				float4 _DissolveTex_ST;
				float _Dissolve, _DissolveX, _DissolveY;
				float _DissolveRot, _DissolveColor1Strengh, _DissolveColor2Strengh;
				fixed4 _DissolveColor1;
				fixed4 _DissolveColor2;

				half _DissolveEdge;
				//half 2;
				half _DissolveEdgeOffset;
       			half _DissolveAlpha;
        		//half _UseVertexAlpha;

			#endif

			#if FlowTex_On
				sampler2D _FlowTex ;
				float4 _FlowTex_ST;
				float _FlowTexRot;
				float _FlowScale, _FlowScaleX, _FlowScaleY, _IsXY;
				float _FlowTexX;
				float _FlowTexY;
			#endif

			#if PointCache_On
				sampler2D _PointCache;
	            float _Scale;
	            float _Samples;
	            float _Range;
			#endif

			const float threshold = 0.5;

			float _SrcFactor, _IsMask, _IsDissolve, _IsFlow;
			float _RgbAsAlpha;


			    UNITY_INSTANCING_BUFFER_START(Props)
            	//UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
				//#define _Color_arr Props
            	UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				#define _MainTex_ST_arr Props

            	#if PointCache_On
            	UNITY_DEFINE_INSTANCED_PROP(float, _Progress)
				#define _Progress_arr Props
            	#endif

        	UNITY_INSTANCING_BUFFER_END(Props)



			fragData vert(appdata v)
			{
				fragData o ;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);




				o.worldPosition = mul(unity_ObjectToWorld,v.vertex);

				#if PointCache_On
					float progress = UNITY_ACCESS_INSTANCED_PROP(_Progress_arr, _Progress);

	                float index = v.uv_index.x;

	                float tSpace = 1.0f/_Samples;
	                float tCur = floor(progress/tSpace);
	                float tNext = tCur+1;

	                float curP = tCur*tSpace;
	                float nextP = clamp(tNext*tSpace,0,1.0f);
	                float lerpVal = (progress-curP)*_Samples;

	                float4 colCur = tex2Dlod(_PointCache,float4(index,1.0f-curP,0,0));//当前帧点缓存像素
	                float4 colNext = tex2Dlod(_PointCache,float4(index,1.0f-nextP,0,0));//下一帧点缓存像素
	                float4 pcColor = lerp(colCur,colNext,lerpVal);//过渡帧像素

	                //将归一化的像素信息还原为坐标
	                float3 cache = _Range*(pcColor.xyz-0.5f);
	                cache.x*=-1;

	                v.vertex.xyz += cache*_Scale;
				#endif
				half4 bbVertex = fixed4((1.0 / length(unity_WorldToObject[0].xyz)) * v.vertex.x + UNITY_MATRIX_MV[0].w, (1.0 / length(unity_WorldToObject[1].xyz)) * v.vertex.y + UNITY_MATRIX_MV[1].w, (1.0 / length(unity_WorldToObject[2].xyz)) * v.vertex.z + UNITY_MATRIX_MV[2].w, 1.0);
				half4 Billboard = mul(UNITY_MATRIX_P, bbVertex);
				half4 NoBillboard = UnityObjectToClipPos(v.vertex);
				o.vertex = lerp(NoBillboard, Billboard, _IsBillboard);
                #ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
                #endif
				o.uv12.xy = TransFormUV (v.uv ,UNITY_ACCESS_INSTANCED_PROP(_MainTex_ST_arr, _MainTex_ST));

				o.uv12.xy = RotateUV(o.uv12.xy,_MainTexRot);
				o.uv12.xy += _Time.z * float2(_MainTexX,_MainTexY);

				#if MaskTex_On
					o.uv12.zw = TransFormUV(v.uv,_MaskTex_ST);
					o.uv12.zw = RotateUV(o.uv12.zw,_MaskTexRot);
					o.uv12.zw += _Time.z * float2(_MaskTexX,_MaskTexY);
				#endif



				#if Dissolve_On	
					o.uv34.xy = TransFormUV(v.uv ,_DissolveTex_ST);
					o.uv34.xy = RotateUV(o.uv34.xy,_DissolveRot);
					o.uv34.xy += _Time.z * float2(_DissolveX, _DissolveY);
				#endif

				#if FlowTex_On
					o.uv34.zw = TransFormUV(v.uv,_FlowTex_ST);
					o.uv34.zw = RotateUV(o.uv34.zw,_FlowTexRot);
					o.uv34.zw +=  _Time.z * float2(_FlowTexX,_FlowTexY);
				#endif

				o.vertexColor = v.vertexColor  ;

				//o.vertexColor.a = saturate(o.vertexColor.a);
				#ifdef RT_PASS_ON
				BLOOM_VERTEX_COMPUTE(o)
				#endif
				//UNITY_TRANSFER_FOG(o, o.vertex); // fog
				return o;
			}
			
			fixed4 frag (fragData i) : SV_Target
			{

               #ifdef SOFTPARTICLES_ON
				fixed sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
			    fixed partZ = i.projPos.z;
			    fixed fade = saturate(_InvFade * (sceneZ - partZ));
			    i.color.a *= fade;
               #endif


				float2 flowUV = i.uv12.xy;

				#if FlowTex_On
				fixed4 flowColor = tex2D(_FlowTex, i.uv34.zw);
				flowUV = i.uv12.xy + (flowColor.xy - 0.5) * lerp(_FlowScale,half2(_FlowScaleX, _FlowScaleY), _IsXY);
				#endif
					
				fixed4 texColor = tex2D(_MainTex, flowUV);
				fixed4 result = texColor;

				#if MaskTex_On
				fixed4 maskColor = tex2D(_MaskTex, i.uv12.zw);
				result.a *= maskColor.r;
				#endif

				float gray = LinearRgbToLuminance(result.rgb);
				float aa[2] = {result.a,gray};
				result.a *= aa[_RgbAsAlpha];

			
			//result *= i.vertexColor;

				#if Dissolve_On//新版
				result.rgb*=i.vertexColor.rgb;

				fixed4 dissolveColor = tex2D(_DissolveTex, i.uv34.xy);
				dissolveColor *= result.a;
				float clipRange = 1-(1.0-_Dissolve)*i.vertexColor.a;
				float clipValue = smoothstep(dissolveColor.r+_DissolveEdge,dissolveColor.r,clipRange/(1-_DissolveEdge));

				float willClip[2] = {0,0.001};
				clip(clipValue-willClip[_DissolveAlpha]);
				float percentage = _Dissolve / dissolveColor.r;
				float lerpEdge = sign(percentage - _DissolveEdgeOffset - _DissolveEdge);
				fixed3 edgeColor = lerp(_DissolveColor2.rgb*_DissolveColor2Strengh, _DissolveColor1.rgb*_DissolveColor1Strengh, saturate(lerpEdge));
				float lerpOut = sign(percentage - _DissolveEdgeOffset);
				float alpah[2] = {clipValue,1};
				if(_DissolveEdge!=0)
				result.a *= alpah[_DissolveAlpha];
				result.rgb = lerp(result.rgb*_Color, edgeColor, saturate(lerpOut));
				#else
				result *= i.vertexColor;
				#endif

			    result *= _Multiplier;	
				

				#if MultiplyBlend_On
				fixed4 multiplyColor = lerp(half4(1,1,1,1), result, result.a);
				result = lerp(result, multiplyColor, _SrcFactor == 0);
				#endif
				
				result.a *= _AlphaCtrl;

				float c = step(length(_ClipRect),0);
				float maskClip = UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
				float maskClipChose[2] = {maskClip,1};
				result.a *= maskClipChose[c];
				
				// clamp alpha, fix bugs of alpha out of range
				result.a = saturate(result.a);

				#ifdef RT_PASS_ON
				BLOOM_FRAGMENT_COMPUTE(i)
				#endif
                #if Dissolve_On
				result.a *= _Color.a;
                #else
				result*= _Color;
                #endif

				return result;
			}
			ENDCG
		}

	}
	CustomEditor "EffectShaderGUI"

}
