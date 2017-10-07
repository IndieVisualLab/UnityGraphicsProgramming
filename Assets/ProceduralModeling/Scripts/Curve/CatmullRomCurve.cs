using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling {

	public class CubicPoly3D {
		Vector3 c0, c1, c2, c3;

		public CubicPoly3D(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float tension = 0.5f) {
			var t0 = tension * (v2 - v0);
			var t1 = tension * (v3 - v1);

			c0 = v1;
            c1 = t0;
            c2 = -3f * v1 + 3f * v2 - 2f * t0 - t1;
            c3 = 2f * v1 - 2f * v2 + t0 + t1;
		}

		public Vector3 Calculate(float t) {
			var t2 = t * t;
			var t3 = t2 * t;
			return c0 + c1 * t + c2 * t2 + c3 * t3;
		}
	}

	[System.Serializable]
    public class CatmullRomCurve : CurveBase {

        protected override Vector3 GetPoint(float t) {
            var points = this.points;
            var l = points.Count;

            var point = (l - (this.closed ? 0 : 1)) * t;
            var intPoint = Mathf.FloorToInt(point);
            var weight = point - intPoint;

            if (this.closed) {
                intPoint += intPoint > 0 ? 0 : (Mathf.FloorToInt(Mathf.Abs(intPoint) / points.Count) + 1) * points.Count;
            } else if (weight == 0 && intPoint == l - 1) {
                intPoint = l - 2;
                weight = 1;
            }

            Vector3 tmp, p0, p1, p2, p3; // 4 points
            if (this.closed || intPoint > 0) {
                p0 = points[(intPoint - 1) % l];
            } else {
                // extrapolate first point
                tmp = (points[0] - points[1]) + points[0];
                p0 = tmp;
            }

            p1 = points[intPoint % l];
            p2 = points[(intPoint + 1) % l];

            if (this.closed || intPoint + 2 < l) {
                p3 = points[(intPoint + 2) % l];
            } else {
                // extrapolate last point
                tmp = (points[l - 1] - points[l - 2]) + points[l - 1];
                p3 = tmp;
            }

			var poly = new CubicPoly3D(p0, p1, p2, p3);
			return poly.Calculate(weight);
        }
    }

}

