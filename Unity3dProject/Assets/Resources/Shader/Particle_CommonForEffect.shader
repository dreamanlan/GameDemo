// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Particles/CommonForEffect" {
Properties {
    _TintColor ("Tint Color", Color) = (1,1,1,1)
	[Enum(One,1,OneMinusSrcAlpha,10)] _BlendMode("BlendMode（混合模式） ",float) = 1
	[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode（单双面显示）", float) = 2
	[Enum(Always,0,On,2)] _IsFront("最上层显示", float) = 2
	[Enum(UnityEngine.Rendering.RenderQueue)] _RenderQueue("渲染层级", float) = 2
	_Rotate("旋转",Range(-360,360)) = 0
	[Toggle] _IsBillboard("Billboard（公告板）",float) = 0
	[Space(50)]
    _MainTex ("MainTexRGB", 2D) = "white" {}
    _MainTexAlpha("MainTexAlpha", 2D) = "white" {}
	[KeywordEnum(R,G,B)] _RGB(" ",float) = 1
	_Brightness("亮度",float)=1
	_MaxClamp ("最大亮度",Range(0,16)) = 1
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend SrcAlpha [_BlendMode]
    ColorMask RGB
	Cull[_Cull]
	Lighting Off 
	ZWrite Off
	ZTest [_IsFront]
   
    SubShader {
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
       //     #pragma multi_compile_fog
			#pragma shader_feature _RGB_R _RGB_G _RGB_B  
            #include "UnityCG.cginc"

            sampler2D _MainTex,_MainTexAlpha;
            fixed4 _TintColor;
			fixed _RGB, _Rotate, _IsBillboard;
            struct appdata_t {
                fixed4 vertex : POSITION;
                fixed4 color : COLOR;
                fixed2 texcoord : TEXCOORD0;
				fixed2 texcoord1 : TEXCOORD2;
				float2 vertexOffset : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                fixed4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                fixed2 texcoord : TEXCOORD0;
				fixed2 texcoord1 : TEXCOORD3;
                UNITY_FOG_COORDS(1)
                #ifdef SOFTPARTICLES_ON
                fixed4 projPos : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _MainTex_ST, _MainTexAlpha_ST;
			uniform sampler2D_float _CameraDepthTexture;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                fixed4 NoBillboard = UnityObjectToClipPos(v.vertex);

				/*fixed4x4 bbmv = UNITY_MATRIX_MV;
				bbmv._m00 = -1.0 / length(unity_WorldToObject[0].xyz);
				bbmv._m10 = 0.0f;
				bbmv._m20 = 0.0f;
				bbmv._m01 = 0.0f;
				bbmv._m11 = -1.0 / length(unity_WorldToObject[1].xyz);
				bbmv._m21 = 0.0f;
				bbmv._m02 = 0.0f;
				bbmv._m12 = 0.0f;
				bbmv._m22 = -1.0 / length(unity_WorldToObject[2].xyz);
				fixed4 Billboard = mul(UNITY_MATRIX_P, mul(bbmv, v.vertex));*/
				
				if (_IsBillboard > 0.0)
				{
					fixed4 bbVertex = fixed4((-1.0 / length(unity_WorldToObject[0].xyz)) * v.vertex.x + UNITY_MATRIX_MV[0].w, (-1.0 / length(unity_WorldToObject[1].xyz)) * v.vertex.y + UNITY_MATRIX_MV[1].w, (-1.0 / length(unity_WorldToObject[2].xyz)) * v.vertex.z + UNITY_MATRIX_MV[2].w, 1.0);
					fixed4 Billboard = mul(UNITY_MATRIX_P, bbVertex);
					o.vertex = Billboard;
				}
				else
				{
					o.vertex = NoBillboard;
				}

				//o.vertex = lerp(NoBillboard, Billboard, _IsBillboard);
				#ifdef SOFTPARTICLES_ON
                o.projPos = ComputeScreenPos (o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif
               
                o.color = v.color;
				fixed i_cos = cos(_Rotate*0.0174);
				fixed i_sin = sin(_Rotate*0.0174);
                o.texcoord = TRANSFORM_TEX((mul(v.texcoord - fixed2(0.5, 0.5), fixed2x2(i_cos, -i_sin, i_sin, i_cos)) + fixed2(0.5, 0.5)),_MainTex);
				o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _MainTex);
           //     UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed _Brightness;
			fixed _MaxClamp;
            fixed4 frag (v2f i) : SV_Target
            {
                #ifdef SOFTPARTICLES_ON
                fixed sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                fixed partZ = i.projPos.z;
                fixed fade = saturate (_InvFade * (sceneZ-partZ));
                i.color.a *= fade;
                #endif



                fixed4 col =  i.color * _TintColor * tex2D(_MainTex, i.texcoord);
				fixed4 col_A = tex2D(_MainTexAlpha, i.texcoord);
				#if  _RGB_R  
				col.a = col_A.r * col.a;
                 
                #elif _RGB_G  
				col.a = col_A.g* col.a;
                 
                #elif _RGB_B  
				col.a = col_A.b* col.a;
                #endif 
				col.a *= _TintColor.a* col.a;
            //    UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
				col = col* _Brightness;
				col.xyz = clamp(col.xyz, 0, _MaxClamp);

                return col;
            }
            ENDCG
        }
    }
}
}
