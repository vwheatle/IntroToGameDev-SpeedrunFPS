Shader "Ghost" {

Properties {
	[NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
	_Color ("Glow Color", Color) = (0.75, 1.0, 1.0, 0.6)
}
SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	
	Pass {
		CGPROGRAM
		#pragma target 2.0
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog
		
		// Thanks, IQ!!
		// https://iquilezles.org/articles/functions/
		float cubicPulse(float center, float width, float x) {
			x = abs(x - center);
			if(x > width) return 0.0;
			x /= width;
			return 1.0 - x * x * (3.0 - 2.0 * x);
		}
		
		float sineNormal(float x) { return abs(sin(x) * 0.5 + 0.5); }
		float cosineNormal(float x) { return abs(cos(x) * 0.5 + 0.5); }
		
		float2 rotate(float2 x, float deg) {
			deg = deg * 180.0 / 3.141592;
			return float2(
				x.x * cos(deg) + x.y * sin(deg),
				x.y * cos(deg) - x.x * sin(deg)
			);
		}
		
		#include "UnityCG.cginc"
		
		struct appdata_t {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};
		
		struct v2f {
			float4 vertex : SV_POSITION;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
			float4 w_vertex : TEXCOORD2;
			UNITY_FOG_COORDS(1)
			UNITY_VERTEX_OUTPUT_STEREO
		};
		
		sampler2D _MainTex;
		fixed4 _Color;
		
		v2f vert(appdata_t v) {
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.normal = UnityObjectToWorldNormal(v.normal);
			o.texcoord = v.texcoord;
			o.w_vertex = v.vertex;
			UNITY_TRANSFER_FOG(o,o.vertex);
			return o;
		}
		
		fixed4 frag(v2f i) : SV_Target {
			// float scanTime = abs(_Time.y + i.vertex.z * 64.0 + cos(i.vertex.x / 128.0) + sin(i.vertex.y / 32.0)) % 1.0;
			float scanTime = abs(sin(_Time.y + i.w_vertex.x + i.w_vertex.y * 2.25) * 0.4 + 0.5);
			
			// make  *something* happen
			float2 lolIdk = lerp(i.texcoord.xy, i.normal.xy, 0.1);
			
			scanTime = max(scanTime, max(
				// THERE NEEDS TO BE MORE OVERLAPPING GRIDLINES
				max(
					cubicPulse(0.125, 0.0625, frac(lolIdk.x * 14.0)),
					cubicPulse(0.125, 0.0625, frac(lolIdk.y * 14.0))
				) * 0.3,
				max(
					cubicPulse(0.125, 0.0625, frac(rotate(lolIdk, 45.0).x * 12.0)),
					cubicPulse(0.125, 0.0625, frac(rotate(lolIdk, 45.0).y * 12.0))
				) * 0.6
			));
			fixed4 scannedColor = fixed4(_Color.xyz, _Color.a * scanTime);
			
			// Get the actual other texture we promised we were gonna use.
			fixed4 col = tex2D(_MainTex, i.texcoord) * scannedColor;
			
			// Unity macro garbage to apply fog, I guess.
			UNITY_APPLY_FOG(i.fogCoord, col);
			
			// Hooray.
			return col;
		}
		
		ENDCG
	}
}

Fallback "Diffuse"

}
