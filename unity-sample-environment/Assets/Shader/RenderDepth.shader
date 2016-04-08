Shader "Custom/ReplacementShader" {
 
     SubShader {
         Tags { "RenderType"="Opaque" }
         
 
         CGINCLUDE
 
         struct VIN {
             float4 pos : POSITION0;
         };
 
         struct V2F {
             float4 pos  : POSITION0;
             float4 p    :TEXCOORD0;
         };
 
         V2F myVert( VIN i ) {
             V2F o;
             o.pos = mul( UNITY_MATRIX_MVP, i.pos );
             o.p = mul( UNITY_MATRIX_MVP , i.pos );
             return o;
         }
 
         fixed4 myFrag(V2F i) : COLOR {
             // Here my farplane is 10, hardcoded, you can pass it like any other parameter from the unity script.
             return fixed4(i.p.z / 10 , i.p.z / 10 , i.p.z / 10 ,1);
         } 
 
         ENDCG
 
         pass {
             CGPROGRAM
             #pragma fragment myFrag
             #pragma vertex myVert
             #pragma fragmentoption ARB_precision_hint_fastest
             #pragma target 2.0
             ENDCG
         }
     } 
 }