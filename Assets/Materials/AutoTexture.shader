Shader "AutoTexture (Diffuse)" {

Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Texture", 2D) = "white" {}
	_Scale ("Texture Scale", Float) = 1.0
	[KeywordEnum(YZ, XZ, XY, AUTO)] _RepeatDirection ("Repeat Direction", Int) = 0
}
SubShader {
	Tags { "RenderType"="Opaque" }
	
	CGPROGRAM
	
	#pragma surface surf Lambert noforwardadd
	
	#pragma shader_feature _REPEATDIRECTION_YZ _REPEATDIRECTION_XZ _REPEATDIRECTION_XY _REPEATDIRECTION_AUTO
	
	sampler2D _MainTex;
	fixed4 _Color;
	float _Scale;
	
	struct Input {
		float3 worldPos;
	};
	
	void surf (Input IN, inout SurfaceOutput o) {
		fixed2 uv = fixed2(0, 0);
		#ifdef _REPEATDIRECTION_YZ
		uv = IN.worldPos.yz;
		#elif _REPEATDIRECTION_XZ
		uv = IN.worldPos.xz;
		#elif _REPEATDIRECTION_XY
		uv = IN.worldPos.xy + fixed2(IN.worldPos.z, 0);
		#endif
		
		fixed4 c = tex2D(_MainTex, uv * _Scale) * _Color;
		
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	
	ENDCG
}

Fallback "Mobile/Diffuse"

}
