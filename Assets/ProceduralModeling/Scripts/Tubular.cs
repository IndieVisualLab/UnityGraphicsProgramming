using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ProceduralModeling {

	public class Tubular : ProceduralModelingBase {

		[SerializeField] protected CatmullRomCurve curve;

		[SerializeField, Range(2, 50)] protected int tubularSegments = 20, radialSegments = 8;
		[SerializeField, Range(0.1f, 5f)] protected float radius = 0.5f;
		[SerializeField] protected bool closed = false;

		const float PI2 = Mathf.PI * 2f;

		protected override Mesh Build() {
			var vertices = new List<Vector3>();
			var normals = new List<Vector3>();
			var tangents = new List<Vector4>();
			var uvs = new List<Vector2>();
			var triangles = new List<int>();

			// 曲線からFrenet frameを取得
			var frames = curve.ComputeFrenetFrames(tubularSegments, closed);

			// Tubularの頂点データを生成
			for(int i = 0; i < tubularSegments; i++) {
				GenerateSegment(curve, frames, vertices, normals, tangents, i);
			}
			// 閉じた筒型を生成する場合は曲線の始点に最後の頂点を配置し、閉じない場合は曲線の終点に配置する
			GenerateSegment(curve, frames, vertices, normals, tangents, (!closed) ? tubularSegments : 0);

			// 曲線の始点から終点に向かってuv座標を設定していく
			for (int i = 0; i <= tubularSegments; i++) {
				for (int j = 0; j <= radialSegments; j++) {
					float u = 1f * j / radialSegments;
					float v = 1f * i / tubularSegments;
					uvs.Add(new Vector2(u, v));
				}
			}

			// 側面を構築
			for (int j = 1; j <= tubularSegments; j++) {
				for (int i = 1; i <= radialSegments; i++) {
					int a = (radialSegments + 1) * (j - 1) + (i - 1);
					int b = (radialSegments + 1) * j + (i - 1);
					int c = (radialSegments + 1) * j + i;
					int d = (radialSegments + 1) * (j - 1) + i;

					triangles.Add(a); triangles.Add(d); triangles.Add(b);
					triangles.Add(b); triangles.Add(d); triangles.Add(c);
				}
			}

			var mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.tangents = tangents.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.triangles = triangles.ToArray();
			return mesh;
		}

		void GenerateSegment(
			CurveBase curve, 
			List<FrenetFrame> frames, 
			List<Vector3> vertices, 
			List<Vector3> normals, 
			List<Vector4> tangents, 
			int index
		) {
			// 0.0 ~ 1.0
			var u = 1f * index / tubularSegments;

			var p = curve.GetPointAt(u);
			var fr = frames[index];

			var N = fr.Normal;
			var B = fr.Binormal;

			for(int j = 0; j <= radialSegments; j++) {
				// 0.0 ~ 2π
				float rad = 1f * j / radialSegments * PI2;

				// 円周に沿って均等に頂点を配置する
				float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
				var v = (cos * N + sin * B).normalized;
				vertices.Add(p + radius * v);
				normals.Add(v);

				var tangent = fr.Tangent;
				tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));
			}
		}

		#if UNITY_EDITOR
		void OnDrawGizmosSelected () {
			// DrawCurve();
			// DrawFrenetFrames();
		}

		void DrawCurve() {
			const float size = 0.025f;

			Gizmos.matrix = transform.localToWorldMatrix;
			var frames = curve.ComputeFrenetFrames(tubularSegments, closed);
			for(int i = 0, n = tubularSegments; i < n; i++) {
				var u0 = 1f * i / tubularSegments;
				var p0 = curve.GetPointAt(u0);

				// draw line
				if(i < n - 1) {
					var u1 = 1f * (i + 1) / tubularSegments;
					var p1 = curve.GetPointAt(u1);
					Gizmos.color = Color.white;
					Gizmos.DrawLine(p0, p1);
				}

				Gizmos.color = Color.green;
				Gizmos.DrawSphere(p0, size);

				var frame = frames[i];
				var N = frame.Normal;
				var B = frame.Binormal;

				Gizmos.color = Color.yellow;
				var radius = size * 4f;
				for(int j = 0; j <= radialSegments; j++) {
					// 0.0 ~ 2π
					float rad0 = 1f * j / radialSegments * PI2;
					float rad1 = 1f * (j + 1) / radialSegments * PI2;

					float cos0 = Mathf.Cos(rad0), sin0 = Mathf.Sin(rad0);
					float cos1 = Mathf.Cos(rad1), sin1 = Mathf.Sin(rad1);

					var normal0 = (cos0 * N + sin0 * B).normalized;
					var normal1 = (cos1 * N + sin1 * B).normalized;
					var v0 = (p0 + radius * normal0);
					var v1 = (p0 + radius * normal1);
					Gizmos.DrawLine(v0, v1);
				}

			}
		}

		void DrawFrenetFrames() {
			Handles.matrix = transform.localToWorldMatrix;
			const float size = 0.05f;

			var frames = curve.ComputeFrenetFrames(tubularSegments, closed);
			for(int i = 0, n = frames.Count; i < n; i++) {
				var u = 1f * i / tubularSegments;

				var p = curve.GetPointAt(u);
				var frame = frames[i];

				Handles.color = Color.white;
				Handles.RectangleHandleCap(0, p, Quaternion.LookRotation(frame.Tangent), size * 2f, EventType.Repaint);

				Handles.color = Color.red;
				Handles.ArrowHandleCap(0, p, Quaternion.LookRotation(frame.Tangent), size, EventType.Repaint);

				Handles.color = Color.green;
				Handles.ArrowHandleCap(0, p, Quaternion.LookRotation(frame.Normal), size, EventType.Repaint);

				Handles.color = Color.blue;
				Handles.ArrowHandleCap(0, p, Quaternion.LookRotation(frame.Binormal), size, EventType.Repaint);
			}
		}

		#endif

	}

}

