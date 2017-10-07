using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling {

	public class Plane : ProceduralModelingBase {

		// Planeの列数(widthSegments)と行数(heightSegments)
		[SerializeField, Range(2, 30)] protected int widthSegments = 8, heightSegments = 8;

		// Planeの横幅と縦幅
		[SerializeField, Range(0.1f, 10f)] protected float width = 1f, height = 1f;

		protected override Mesh Build() {
			var mesh = new Mesh();

			var vertices = new List<Vector3>();
			var uv = new List<Vector2>();
			var normals = new List<Vector3>();

			// 頂点のグリッド上での位置の割合(0.0 ~ 1.0)を算出するための行列数の逆数
			var winv = 1f / (widthSegments - 1);
			var hinv = 1f / (heightSegments - 1);

			for(int y = 0; y < heightSegments; y++) {
				// 行の位置の割合(0.0 ~ 1.0)
				var ry = y * hinv;

				for(int x = 0; x < widthSegments; x++) {
					// 列の位置の割合(0.0 ~ 1.0)
					var rx = x * winv;

					vertices.Add(new Vector3(
						(rx - 0.5f) * width, 
						0f,
						(0.5f - ry) * height
					));
					uv.Add(new Vector2(rx, ry));
					normals.Add(new Vector3(0f, 1f, 0f));
				}
			}

			var triangles = new List<int>();

			for(int y = 0; y < heightSegments - 1; y++) {
				for(int x = 0; x < widthSegments - 1; x++) {
					int index = y * widthSegments + x;
					var a = index;
					var b = index + 1;
					var c = index + 1 + widthSegments;
					var d = index + widthSegments;

					triangles.Add(a);
					triangles.Add(b);
					triangles.Add(c);

					triangles.Add(c);
					triangles.Add(d);
					triangles.Add(a);
				}
			}

			mesh.vertices = vertices.ToArray();
			mesh.uv = uv.ToArray();
			mesh.normals = normals.ToArray();
			mesh.triangles = triangles.ToArray();

			mesh.RecalculateBounds();

			return mesh;
		}

	}

}

