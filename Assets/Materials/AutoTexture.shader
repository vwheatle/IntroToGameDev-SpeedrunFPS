Shader "World Space Diffuse" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Texture Scale", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Lambert
		
		sampler2D _MainTex;
		fixed4 _Color;
		float _Scale;
		
		struct Input {
			float3 worldPos;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.worldPos.xz * _Scale) * _Color;
			
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}
