using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling {

	/*
	 */
	public abstract class ParametricPlaneBase : Plane {

		[SerializeField] protected float depth = 1f;

		protected override Mesh Build() {
			var mesh = base.Build();

			var vertices = mesh.vertices;

			// 頂点のグリッド上での位置の割合(0.0 ~ 1.0)を算出するための行列数の逆数
			var winv = 1f / (widthSegments - 1);
			var hinv = 1f / (heightSegments - 1);

			for(int y = 0; y < heightSegments; y++) {
				// 行の位置の割合(0.0 ~ 1.0)
				var ry = y * hinv;
				for(int x = 0; x < widthSegments; x++) {
					// 列の位置の割合(0.0 ~ 1.0)
					var rx = x * winv;

					int index = y * widthSegments + x;
					vertices[index].y = Depth(rx, ry);
				}
			}

			mesh.vertices = vertices;
			mesh.RecalculateBounds();

			// 法線方向を自動算出
			mesh.RecalculateNormals();

			return mesh;
		}

		/*
		 * (u, v) = (0.0 ~ 1.0, 0.0 ~ 1.0)に位置する頂点の高さを返す
		 */
		protected abstract float Depth(float u, float v);

	}

}

