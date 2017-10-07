using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling {

	public class TerrainPlane : ParametricPlaneBase {

		[SerializeField, Range(0.1f, 10f)] protected float uScale = 1.2f, vScale = 1.5f;
		[SerializeField] protected float uOffset = 0f, vOffset = 0f;

		protected override float Depth (float u, float v)
		{
			return Mathf.PerlinNoise(u * uScale + uOffset, v * vScale + vOffset) * depth;
		}

	}
		
}

