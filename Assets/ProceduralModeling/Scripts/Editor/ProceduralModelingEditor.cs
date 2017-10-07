using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace ProceduralModeling {

	[CustomEditor (typeof(ProceduralModelingBase), true)]
	public class ProceduralModelingEditor : Editor {

		public override void OnInspectorGUI() {
			EditorGUI.BeginChangeCheck();
			base.OnInspectorGUI();
			if(EditorGUI.EndChangeCheck()) {
				var pm = target as ProceduralModelingBase;
				pm.Rebuild();
			}
		}

	}
		
}

