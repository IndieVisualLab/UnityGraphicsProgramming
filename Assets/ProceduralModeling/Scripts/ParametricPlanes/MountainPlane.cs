using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling {

	public class MountainPlane : ParametricPlaneBase {

		// 山の高さの勾配の変化具合
		[SerializeField, Range(0.5f, 3f)] protected float power = 1.25f;

		protected override float Depth (float u, float v)
		{
			return Distance(u) * Distance(v) * depth;
		}

		protected float Distance(float v) {
			return Mathf.Pow((0.5f - Mathf.Abs(0.5f - v)), power);
		}

	}
		
}

