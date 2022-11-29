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

			v2f vert(appdata_t v) {
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

			fixed4 frag(v2f i) : SV_Target {
				// Contains the uhhhhh the...
				// the two axes that the texture's UV coordinates will vary
				// in, I guess.
				float2 t;
				
				// don't know anything about linear algebra and it's late right now, so.. :)
				// this special-cases out the three axes lol,
				if (abs(dot(i.normal, float3(0, 0, 1))) > 0.4) {
					// Texture lies on XY plane.
					t = i.texcoord.xy;
				} else if (abs(dot(i.normal, float3(1, 0, 0))) > 0.4) {
					// Texture lies on YZ plane.
					// (and maintains the Y-up-ness of this world.)
					t = i.texcoord.zy;
				} else {
					// Texture lies flat on XZ plane.
					t = i.texcoord.xz;
				}
				
				// "normalized" screen-space coordinates.
				// normalized in quotes because they're kinda saturated, but
				// the horizontal component is scaled to the aspect ratio,
				// meaning the aspect ratio will be correct, etc.
				float aspectRatio = _ScreenParams.x / _ScreenParams.y;
				float2 screenUV = i.screenpos.xy / i.screenpos.w;
				screenUV.x *= aspectRatio;
				
				// The x position into the shiny effect's pattern function.
				// (Takes into account distance from the camera!)
				float pos = frac((i.texcoord.z - i.texcoord.x) / 64 + (screenUV.y / 3) - screenUV.x);
				
				// This contains the whole bar pattern of the shiny window.
				float trans = saturate(1 - (1 + step(pos, 0.65) - step(pos, 0.7) + step(pos, 0.75) - step(pos, 0.8)));
				// It's stored as a single transparency channel.
				
				// ...and then multiply that transparency channel by
				trans *= i.texcoord.y / 32;
				
				// TODO: multiply the transparency also by the dot product of
				// uhhhhhhhhh the camera. maybe make it solid 0.75 alpha white
				// at side angles, just to mask the strange wavy artifacts.
				
				// Make an all-white "texture" from the shiny pattern.
				fixed4 other = fixed2(1, trans).xxxy;
				
				// Get the actual other texture we promised we were gonna use.
				fixed4 col = tex2D(_MainTex, t / _Scale);
				
				// Mix the reflection pattern with the window texture.
				col = lerp(other, col, col.w);
				
				// Unity macro garbage to apply fog, I guess.
				UNITY_APPLY_FOG(i.fogCoord, col);
				
				// Hooray.
				return col;
			}
		ENDCG
	}
}

}
