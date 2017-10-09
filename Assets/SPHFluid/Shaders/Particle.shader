Shader "Custom/SPH2D" {
	Properties {
		_MainTex("Texture",         2D) = "black" {}
		_ParticleRadius("Particle Radius", Float) = 0.05
		_WaterColor("WaterColor", Color) = (1, 1, 1, 1)
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float4 _MainTex_ST;
	fixed4 _WaterColor;

	float  _ParticleRadius;
	float4x4 _InvViewMatrix;

	struct v2g {
		float4 pos   : SV_POSITION;
		float4 color : COLOR;
	};

	struct g2f {
		float4 pos   : POSITION;
		float2 tex   : TEXCOORD0;
		float4 color : COLOR;
	};

	struct FluidParticle {
		float2 position;
		float2 velocity;
	};

	StructuredBuffer<FluidParticle> _ParticlesBuffer;

	// --------------------------------------------------------------------
	// Vertex Shader
	// --------------------------------------------------------------------
	v2g vert(uint id : SV_VertexID) {

		v2g o = (v2g)0;
		o.pos = float4(_ParticlesBuffer[id].position.xy, 0, 1);
		o.color = float4(0, 0.1, 0.1, 1);
		return o;
	}

	// --------------------------------------------------------------------
	// Geometry Shader
	// --------------------------------------------------------------------

	[maxvertexcount(4)]
	void geom(point v2g IN[1], inout TriangleStream<g2f> triStream) {

		float size = _ParticleRadius * 2;
		float halfS = _ParticleRadius;

		g2f pIn = (g2f)0;

		for (int x = 0; x < 2; x++) {
			for (int y = 0; y < 2; y++) {
				float4x4 billboardMatrix = UNITY_MATRIX_V;
				billboardMatrix._m03 = billboardMatrix._m13 = billboardMatrix._m23 = billboardMatrix._m33 = 0;

				float2 uv = float2(x, y);

				pIn.pos = IN[0].pos + mul(float4((uv * 2 - float2(1, 1)) * halfS, 0, 1), billboardMatrix);

				pIn.pos = mul(UNITY_MATRIX_VP, pIn.pos);

				pIn.color = IN[0].color;
				pIn.tex = uv;

				triStream.Append(pIn);
			}
		}
		triStream.RestartStrip();

	}

	// --------------------------------------------------------------------
	// Fragment Shader
	// --------------------------------------------------------------------
	fixed4 frag(g2f input) : SV_Target {
		return tex2D(_MainTex, input.tex)*_WaterColor;
	}

	ENDCG

	SubShader {
		Tags{ "RenderType" = "Transparent" "RenderType" = "Transparent" }
		LOD 300

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma target   5.0
			#pragma vertex   vert
			#pragma geometry geom
			#pragma fragment frag
			ENDCG
		}
	}
}