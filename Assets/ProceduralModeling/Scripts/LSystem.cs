using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling {

	public class LSystem : MonoBehaviour {

		[SerializeField] Color color = Color.black;
		[SerializeField, Range(1, 8)] protected int depth = 5;
		[SerializeField, Range(1f, 5f)] protected float length = 2f;
		[SerializeField, Range(0.5f, 0.9f)] protected float attenuation = 0.635f;
		[SerializeField, Range(0f, 90f)] protected float angle = 20f;

		Material lineMat;

		void OnEnable () {
			var shader = Shader.Find("Hidden/Internal-Colored");
			if(shader == null) {
				Debug.LogWarning("Shader Hidden/Internal-Colored not found");
			}
			lineMat = new Material(shader);
		}

		void DrawLSystem(int depth, float length = 2f) {
			lineMat.SetColor("_Color", color);
			lineMat.SetPass(0);

			DrawFractal(transform.localToWorldMatrix, depth, length);
		}

		void DrawFractal(Matrix4x4 current, int depth, float length) {
			if(depth <= 0) return;

			GL.MultMatrix(current);
			GL.Begin(GL.LINES);
			GL.Vertex(Vector3.zero);
			GL.Vertex(new Vector3(0f, length, 0f));
			GL.End();

			GL.PushMatrix();
			var ml = current * Matrix4x4.TRS(new Vector3(0f, length, 0f), Quaternion.AngleAxis(-angle, Vector3.forward), Vector3.one);
			DrawFractal(ml, depth - 1, length * attenuation);
			GL.PopMatrix();

			GL.PushMatrix();
			var mr = current * Matrix4x4.TRS(new Vector3(0f, length, 0f), Quaternion.AngleAxis(angle, Vector3.forward), Vector3.one);
			DrawFractal(mr, depth - 1, length * attenuation);
			GL.PopMatrix();
		}

		void OnRenderObject () {
			DrawLSystem(depth, length);
		}

	}
		
}

