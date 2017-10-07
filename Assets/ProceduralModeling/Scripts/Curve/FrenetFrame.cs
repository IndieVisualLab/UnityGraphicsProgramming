using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling {

	public class FrenetFrame {
		public Vector3 Tangent { get { return tangent; } }
		public Vector3 Normal { get { return normal; } }
		public Vector3 Binormal { get { return binormal; } }

		Vector3 tangent, normal, binormal;

		public FrenetFrame(Vector3 t, Vector3 n, Vector3 bn) {
			tangent = t;
			normal = n;
			binormal = bn;
		}
	}
		
}

