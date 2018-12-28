// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:34220,y:32058,varname:node_9361,prsc:2|emission-1790-OUT,alpha-3190-OUT;n:type:ShaderForge.SFN_Tex2d,id:6512,x:31707,y:32388,ptovrint:False,ptlb:bing_tietu,ptin:_bing_tietu,varname:_bing_tietu,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5fc44215729fdd241aaebcda019d4f0d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Fresnel,id:7339,x:31745,y:33117,varname:node_7339,prsc:2|EXP-7796-OUT;n:type:ShaderForge.SFN_Slider,id:7796,x:31438,y:33089,ptovrint:False,ptlb:fre_fanwei,ptin:_fre_fanwei,varname:_fre_fanwei,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.018124,max:10;n:type:ShaderForge.SFN_Add,id:1790,x:33383,y:32166,varname:node_1790,prsc:2|A-6988-OUT,B-4730-OUT;n:type:ShaderForge.SFN_Multiply,id:431,x:31935,y:33049,varname:node_431,prsc:2|A-7443-OUT,B-7339-OUT;n:type:ShaderForge.SFN_Slider,id:7443,x:31454,y:32904,ptovrint:False,ptlb:fre_liangdu,ptin:_fre_liangdu,varname:_fre_liangdu,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.892034,max:10;n:type:ShaderForge.SFN_Multiply,id:6099,x:32136,y:32937,varname:node_6099,prsc:2|A-431-OUT,B-1306-RGB;n:type:ShaderForge.SFN_Color,id:1306,x:32028,y:33132,ptovrint:False,ptlb:fre_yanse,ptin:_fre_yanse,varname:_fre_yanse,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:3729,x:31739,y:31773,ptovrint:False,ptlb:wenli_tietu,ptin:_wenli_tietu,varname:_wenli_tietu,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3609dcd50b8ed7f47b6275487a736467,ntxv:0,isnm:False|MIP-6613-OUT;n:type:ShaderForge.SFN_Multiply,id:7754,x:32541,y:31848,varname:node_7754,prsc:2|A-4036-OUT,B-7946-OUT;n:type:ShaderForge.SFN_Slider,id:8914,x:31660,y:32662,ptovrint:False,ptlb:tietu_liangdu,ptin:_tietu_liangdu,varname:_tietu_liangdu,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:7293,x:32063,y:32509,varname:node_7293,prsc:2|A-6512-RGB,B-8914-OUT;n:type:ShaderForge.SFN_Multiply,id:8605,x:32256,y:32552,varname:node_8605,prsc:2|A-7293-OUT,B-3726-RGB;n:type:ShaderForge.SFN_Color,id:3726,x:31981,y:32755,ptovrint:False,ptlb:tietu_yanse_copy,ptin:_tietu_yanse_copy,varname:_tietu_yanse_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:0.5;n:type:ShaderForge.SFN_Multiply,id:4036,x:32249,y:31698,varname:node_4036,prsc:2|A-8891-OUT,B-3729-B;n:type:ShaderForge.SFN_Slider,id:8891,x:31739,y:31607,ptovrint:False,ptlb:wenli_liangdu_copy,ptin:_wenli_liangdu_copy,varname:_wenli_liangdu_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7575247,max:10;n:type:ShaderForge.SFN_Fresnel,id:1082,x:32614,y:32807,varname:node_1082,prsc:2|EXP-1059-OUT;n:type:ShaderForge.SFN_Slider,id:1059,x:32308,y:32834,ptovrint:False,ptlb:fre_fanwei_touming,ptin:_fre_fanwei_touming,varname:_fre_fanwei_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4273505,max:10;n:type:ShaderForge.SFN_VertexColor,id:7794,x:32433,y:33094,varname:node_7794,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3190,x:33039,y:32880,varname:node_3190,prsc:2|A-5395-OUT,B-7794-A;n:type:ShaderForge.SFN_OneMinus,id:4648,x:32753,y:32584,varname:node_4648,prsc:2|IN-1082-OUT;n:type:ShaderForge.SFN_Lerp,id:5395,x:32958,y:32700,varname:node_5395,prsc:2|A-1082-OUT,B-4648-OUT,T-2588-OUT;n:type:ShaderForge.SFN_Slider,id:2588,x:32424,y:32674,ptovrint:False,ptlb:touming_moshixuanze,ptin:_touming_moshixuanze,varname:_fre_fanwei_touming_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Power,id:653,x:32877,y:32071,varname:node_653,prsc:2|VAL-7754-OUT,EXP-7133-OUT;n:type:ShaderForge.SFN_Slider,id:7133,x:32443,y:32249,ptovrint:False,ptlb:wenli_power,ptin:_wenli_power,varname:_wenli_liangdu_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.940171,max:10;n:type:ShaderForge.SFN_Desaturate,id:4730,x:33084,y:32126,varname:node_4730,prsc:2|COL-653-OUT;n:type:ShaderForge.SFN_Fresnel,id:263,x:31236,y:31977,varname:node_263,prsc:2|EXP-1992-OUT;n:type:ShaderForge.SFN_Multiply,id:6613,x:31501,y:31933,varname:node_6613,prsc:2|A-3829-OUT,B-263-OUT;n:type:ShaderForge.SFN_Slider,id:3829,x:30997,y:31824,ptovrint:False,ptlb:fre_wenli_laingdu,ptin:_fre_wenli_laingdu,varname:_fre_liangdu_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.005712,max:10;n:type:ShaderForge.SFN_Multiply,id:7946,x:31714,y:32028,varname:node_7946,prsc:2|A-6613-OUT,B-5464-RGB;n:type:ShaderForge.SFN_Color,id:5464,x:31485,y:32100,ptovrint:False,ptlb:fre_yanse_copy,ptin:_fre_yanse_copy,varname:_fre_yanse_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:6988,x:32559,y:32457,varname:node_6988,prsc:2|A-8605-OUT,B-6099-OUT;n:type:ShaderForge.SFN_Slider,id:1992,x:30769,y:32053,ptovrint:False,ptlb:fre_wenli_fanwei,ptin:_fre_wenli_fanwei,varname:node_1992,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5.935803,max:10;proporder:6512-7796-7443-1306-3729-8914-3726-8891-1059-2588-7133-3829-5464-1992;pass:END;sub:END;*/

Shader "Shader Forge/bing" {
    Properties {
        _bing_tietu ("bing_tietu", 2D) = "white" {}
        _fre_fanwei ("fre_fanwei", Range(0, 10)) = 1.018124
        _fre_liangdu ("fre_liangdu", Range(0, 10)) = 2.892034
        _fre_yanse ("fre_yanse", Color) = (1,1,1,1)
        _wenli_tietu ("wenli_tietu", 2D) = "white" {}
        _tietu_liangdu ("tietu_liangdu", Range(0, 10)) = 1
        _tietu_yanse_copy ("tietu_yanse_copy", Color) = (1,1,1,0.5)
        _wenli_liangdu_copy ("wenli_liangdu_copy", Range(0, 10)) = 0.7575247
        _fre_fanwei_touming ("fre_fanwei_touming", Range(0, 10)) = 0.4273505
        _touming_moshixuanze ("touming_moshixuanze", Range(0, 10)) = 1
        _wenli_power ("wenli_power", Range(0, 10)) = 1.940171
        _fre_wenli_laingdu ("fre_wenli_laingdu", Range(0, 10)) = 4.005712
        _fre_yanse_copy ("fre_yanse_copy", Color) = (1,1,1,1)
        _fre_wenli_fanwei ("fre_wenli_fanwei", Range(0, 10)) = 5.935803
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
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            //#pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _bing_tietu; uniform float4 _bing_tietu_ST;
            uniform float _fre_fanwei;
            uniform float _fre_liangdu;
            uniform float4 _fre_yanse;
            uniform sampler2D _wenli_tietu; uniform float4 _wenli_tietu_ST;
            uniform float _tietu_liangdu;
            uniform float4 _tietu_yanse_copy;
            uniform float _wenli_liangdu_copy;
            uniform float _fre_fanwei_touming;
            uniform float _touming_moshixuanze;
            uniform float _wenli_power;
            uniform float _fre_wenli_laingdu;
            uniform float4 _fre_yanse_copy;
            uniform float _fre_wenli_fanwei;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _bing_tietu_var = tex2D(_bing_tietu,TRANSFORM_TEX(i.uv0, _bing_tietu));
                float node_6613 = (_fre_wenli_laingdu*pow(1.0-max(0,dot(normalDirection, viewDirection)),_fre_wenli_fanwei));
                float4 _wenli_tietu_var = tex2Dlod(_wenli_tietu,float4(TRANSFORM_TEX(i.uv0, _wenli_tietu),0.0,node_6613));
                float3 node_1790 = ((((_bing_tietu_var.rgb*_tietu_liangdu)*_tietu_yanse_copy.rgb)*((_fre_liangdu*pow(1.0-max(0,dot(normalDirection, viewDirection)),_fre_fanwei))*_fre_yanse.rgb))+dot(pow(((_wenli_liangdu_copy*_wenli_tietu_var.b)*(node_6613*_fre_yanse_copy.rgb)),_wenli_power),float3(0.3,0.59,0.11)));
                float3 emissive = node_1790;
                float3 finalColor = emissive;
                float node_1082 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_fre_fanwei_touming);
                fixed4 finalRGBA = fixed4(finalColor,(lerp(node_1082,(1.0 - node_1082),_touming_moshixuanze)*i.vertexColor.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
