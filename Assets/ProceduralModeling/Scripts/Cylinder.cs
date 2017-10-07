using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling {

	public class Cylinder : ProceduralModelingBase {

		[SerializeField, Range(0.1f, 10f)] protected float height = 3f, radius = 1f;
		[SerializeField, Range(3, 32)] protected int segments = 16;
		[SerializeField] bool openEnded = true;

		// 円を形作るための2π
		const float PI2 = Mathf.PI * 2f;

		protected override Mesh Build() {
			var mesh = new Mesh();

			var vertices = new List<Vector3>();
			var normals = new List<Vector3>();
			var uvs = new List<Vector2>();
			var triangles = new List<int>();

			// 上端の高さと、下端の高さ
			float top = height * 0.5f, bottom = -height * 0.5f;

			// 側面を構成する頂点データを生成
			GenerateCap(segments + 1, top, bottom, radius, vertices, uvs, normals, true);

			// 側面の三角形を構築する際、円上の頂点を参照するために、
			// indexが円を一周するための除数
			var len = (segments + 1) * 2;

			// 上端と下端をつなぎ合わせて側面を構築
			for (int i = 0; i < segments + 1; i++) {
				int idx = i * 2;
				int a = idx, b = idx + 1, c = (idx + 2) % len, d = (idx + 3) % len;
				triangles.Add(a);
				triangles.Add(c);
				triangles.Add(b);

				triangles.Add(d);
				triangles.Add(b);
				triangles.Add(c);
			}

			// 上端と下端の蓋を生成
			if(openEnded) {
				// 蓋のモデルのための頂点は、ライティング時に異なった法線を利用するために、側面とは共有せずに新しく追加する
				GenerateCap(segments + 1, top, bottom, radius, vertices, uvs, normals, false);

				// 上端の蓋の真ん中の頂点
				vertices.Add(new Vector3(0f, top, 0f));
				uvs.Add(new Vector2(0.5f, 1f));
				normals.Add(new Vector3(0f, 1f, 0f));

				// 下端の蓋の真ん中の頂点
				vertices.Add(new Vector3(0f, bottom, 0f)); // bottom
				uvs.Add(new Vector2(0.5f, 0f));
				normals.Add(new Vector3(0f, -1f, 0f));

				var it = vertices.Count - 2;
				var ib = vertices.Count - 1;

				// 側面の分の頂点indexを参照しないようにするためのoffset
				var offset = len;

				// 上端の蓋の面
				for (int i = 0; i < len; i += 2) {
					triangles.Add(it);
					triangles.Add((i + 2) % len + offset);
					triangles.Add(i + offset);
				}

				// 下端の蓋の面
				for (int i = 1; i < len; i += 2) {
					triangles.Add(ib);
					triangles.Add(i + offset);
					triangles.Add((i + 2) % len + offset);
				}
			}

			mesh.vertices = vertices.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.normals = normals.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.RecalculateBounds();

			return mesh;
		}

		void GenerateCap(int segments, float top, float bottom, float radius, List<Vector3> vertices, List<Vector2> uvs, List<Vector3> normals, bool side) {
			for (int i = 0; i < segments; i++) {
				// 0.0 ~ 1.0
				float ratio = (float)i / (segments - 1);

				// 0.0 ~ 2π
				float rad = ratio * PI2;

				// 円周に沿って上端と下端に均等に頂点を配置する
				float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
				float x = cos * radius, z = sin * radius;
				Vector3 tp = new Vector3(x, top, z), bp = new Vector3(x, bottom, z);

				// 上端
				vertices.Add(tp); 
				uvs.Add(new Vector2(ratio, 1f));

				// 下端
				vertices.Add(bp); 
				uvs.Add(new Vector2(ratio, 0f));

				if(side) {
					// 側面の外側を向く法線
					var normal = new Vector3(cos, 0f, sin);
					normals.Add(normal);
					normals.Add(normal);
				} else {
					normals.Add(new Vector3(0f, 1f, 0f)); // 蓋の上を向く法線
					normals.Add(new Vector3(0f, -1f, 0f)); // 蓋の下を向く法線
				}
			}

		}

	}

}

