// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Transparent SEQUEL" {
Properties {
	[NoScaleOffset] _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Scale ("Texture Scale", Float) = 1.0
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100

	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha

	Pass {
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				// float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 screenpos : TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Scale;

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = mul(unity_ObjectToWorld, v.normal);
				// o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				// o.texcoord = v.texcoord;
				o.texcoord = mul(unity_ObjectToWorld, v.vertex);
				o.screenpos = ComputeScreenPos(o.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 t;
				
				// don't know anything about linear algebra and it's late right now, so.. :)
				// this special-cases out the three axes lol,
				if (abs(dot(i.normal, float3(0, 0, 1))) > 0.4) {
					t = i.texcoord.xy;
				} else if (abs(dot(i.normal, float3(1, 0, 0))) > 0.4) {
					t = i.texcoord.zy;
				} else {
					t = i.texcoord.xz;
				}
				
				// sure.
				float aspectRatio = _ScreenParams.x / _ScreenParams.y;
				float2 screenUV = i.screenpos.xy / i.screenpos.w;
				screenUV.x *= aspectRatio;
				
				float pos = frac((i.texcoord.z - i.texcoord.x) / 64 + (screenUV.y / 3) - screenUV.x);
				float trans = saturate(1 - (1 + step(pos, 0.5) - step(pos, 0.58) + step(pos, 0.75) - step(pos, 0.8)));
				trans *= i.texcoord.y / 32;
				
				fixed4 other = fixed4(1, 1, 1, trans);
				fixed4 col = tex2D(_MainTex, t / _Scale);
				col = lerp(other, col, col.w);
				
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
		ENDCG
	}
}

}
