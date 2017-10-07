using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling {

	public class Quad : ProceduralModelingBase {

		[SerializeField, Range(0.1f, 10f)] protected float size = 1f;

		protected override Mesh Build() {
			var mesh = new Mesh();

			// Quadの横幅と縦幅がそれぞれsizeの長さになるように半分の長さを求める
			var hsize = size * 0.5f; 

			// Quadの頂点データ
			var vertices = new Vector3[] {
				new Vector3(-hsize,  hsize, 0f), // 0つ目の頂点 Quadの左上の位置
				new Vector3( hsize,  hsize, 0f), // 1つ目の頂点 Quadの右上の位置
				new Vector3( hsize, -hsize, 0f), // 2つ目の頂点 Quadの右下の位置
				new Vector3(-hsize, -hsize, 0f)  // 3つ目の頂点 Quadの左下の位置
			};

			// Quadのuv座標データ
			var uv = new Vector2[] {
				new Vector2(0f, 0f), // 0つ目の頂点のuv座標
				new Vector2(1f, 0f), // 1つ目の頂点のuv座標
				new Vector2(1f, 1f), // 2つ目の頂点のuv座標
				new Vector2(0f, 1f)  // 3つ目の頂点のuv座標
			};

			// Quadの法線データ
			var normals = new Vector3[] {
				new Vector3(0f, 0f, -1f), // 0つ目の頂点の法線
				new Vector3(0f, 0f, -1f), // 1つ目の頂点の法線
				new Vector3(0f, 0f, -1f), // 2つ目の頂点の法線
				new Vector3(0f, 0f, -1f)  // 3つ目の頂点の法線
			};

			// Quadの面データ 頂点のindexを3つ並べて1つの面(三角形)として認識する
			var triangles = new int[] {
				0, 1, 2, // 1つ目の三角形
				2, 3, 0  // 2つ目の三角形
			};

			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.normals = normals;
			mesh.triangles = triangles;

			// Meshが占める境界領域を計算する（cullingに必要）
			mesh.RecalculateBounds();

			return mesh;
		}

	}

}

