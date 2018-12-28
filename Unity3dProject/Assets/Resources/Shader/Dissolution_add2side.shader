Shader "Effect/Dissolution_add2side" {
    Properties {
        _ColorAlpha ("Color&Alpha", Color) = (1,1,1,1)
        _DiffuseTexture ("Diffuse Texture", 2D) = "white" {}
        _N_mask ("N_mask", Float ) = 0.3
        _T_mask ("T_mask", 2D) = "white" {}
        _C_BYcolor ("C_BYcolor", Color) = (1,0,0,1)
        _N_BY_QD ("N_BY_QD", Float ) = 3
        _N_BY_KD ("N_BY_KD", Float ) = 0.01
			_ColorScale("Color Scale",range(0,8)) = 1
			_ColorMax("Color Max",Float) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha One
            ZWrite Off
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _T_mask; uniform float4 _T_mask_ST;
            uniform sampler2D _DiffuseTexture; uniform float4 _DiffuseTexture_ST;
            uniform float4 _ColorAlpha;
            uniform float _N_mask;
            uniform float _N_BY_KD;
            uniform float4 _C_BYcolor;
            uniform float _N_BY_QD;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
			float _ColorScale;
			float _ColorMax;
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _DiffuseTexture_var = tex2D(_DiffuseTexture,TRANSFORM_TEX(i.uv0, _DiffuseTexture));
                float3 emissive = ((_ColorAlpha.rgb*_DiffuseTexture_var.rgb)*i.vertexColor.rgb);
                float node_1423 = (i.vertexColor.a*_N_mask);
                float4 _T_mask_var = tex2D(_T_mask,TRANSFORM_TEX(i.uv0, _T_mask));
                float node_5746_if_leA = step(node_1423,_T_mask_var.r);
                float node_5746_if_leB = step(_T_mask_var.r,node_1423);
                float node_3073 = 0.0;
                float node_6272 = 1.0;
                float node_5746 = lerp((node_5746_if_leA*node_3073)+(node_5746_if_leB*node_6272),node_6272,node_5746_if_leA*node_5746_if_leB);
                float node_5217_if_leA = step(node_1423,(_T_mask_var.r+_N_BY_KD));
                float node_5217_if_leB = step((_T_mask_var.r+_N_BY_KD),node_1423);
                float node_1274 = (node_5746-lerp((node_5217_if_leA*node_3073)+(node_5217_if_leB*node_6272),node_6272,node_5217_if_leA*node_5217_if_leB));
                float3 finalColor = emissive + ((node_1274*_C_BYcolor.rgb)*_N_BY_QD);
				finalColor = min(finalColor*_ColorScale, _ColorMax);
                return fixed4(finalColor,(_ColorAlpha.a*(_DiffuseTexture_var.a*(node_5746+node_1274))));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
