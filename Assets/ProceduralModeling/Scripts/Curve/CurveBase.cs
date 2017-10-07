using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ProceduralModeling {

	[System.Serializable]
    public abstract class CurveBase {

		public List<Vector3> Points { get { return points; } }

		[SerializeField] protected List<Vector3> points = new List<Vector3>() { Vector3.zero, Vector3.right, Vector3.up, Vector3.left };
        [SerializeField] protected bool closed = false;

        protected float[] cacheArcLengths;
        bool needsUpdate;

        protected abstract Vector3 GetPoint(float t);

        protected virtual Vector3 GetTangent(float t) {
            var delta = 0.001f;
            var t1 = t - delta;
            var t2 = t + delta;

            // Capping in case of danger
            if (t1 < 0f) t1 = 0f;
            if (t2 > 1f) t2 = 1f;

            var pt1 = GetPoint(t1);
            var pt2 = GetPoint(t2);
            return (pt2 - pt1).normalized;
        }

        public Vector3 GetPointAt(float u) {
            float t = GetUtoTmapping(u);
            return GetPoint(t);
        }

        public Vector3 GetTangentAt(float u) {
            float t = GetUtoTmapping(u);
            return GetTangent(t);
        }

        float[] GetLengths(int divisions = -1) {
            if (divisions < 0) {
                divisions = 200;
            }

            if (this.cacheArcLengths != null &&
                    (this.cacheArcLengths.Length == divisions + 1) &&
                    !this.needsUpdate) {
                return this.cacheArcLengths;
            }

            this.needsUpdate = false;

            var cache = new float[divisions + 1];
            Vector3 current, last = this.GetPoint(0f);

            cache[0] = 0f;

            float sum = 0f;
            for (int p = 1; p <= divisions; p ++ ) {
                current = this.GetPoint(1f * p / divisions);
                sum += Vector3.Distance(current, last);
                cache[p] = sum;
                last = current;
            }

            this.cacheArcLengths = cache;
            return cache;
        }

        protected float GetUtoTmapping(float u) {
            var arcLengths = this.GetLengths();

            int i = 0, il = arcLengths.Length;

            float targetArcLength = u * arcLengths[il - 1];

            int low = 0, high = il - 1;
            float comparison;

            while ( low <= high ) {

                i = Mathf.FloorToInt(low + (high - low) / 2f);
                comparison = arcLengths[i] - targetArcLength;

                if (comparison < 0f) {
                    low = i + 1;
                } else if (comparison > 0f) {
                    high = i - 1;
                } else {
                    high = i;
                    break;
                }

            }

            i = high;

            if (Mathf.Approximately(arcLengths[i], targetArcLength)) {
                return 1f * i / ( il - 1 );
            }

            var lengthBefore = arcLengths[i];
            var lengthAfter = arcLengths[i + 1];

            var segmentLength = lengthAfter - lengthBefore;

            var segmentFraction = ( targetArcLength - lengthBefore ) / segmentLength;

            var t = 1f * (i + segmentFraction) / (il - 1);

            return t;
        }

        public List<FrenetFrame> ComputeFrenetFrames (int segments, bool closed = false) {
            var tangent = GetTangentAt(0f).normalized;
            var tx = Mathf.Abs(tangent.x);
            var ty = Mathf.Abs(tangent.y);
            var tz = Mathf.Abs(tangent.z);

            var normal = new Vector3();
            var min = float.MaxValue;
            if (tx <= min) {
                min = tx;
                normal.Set(1, 0, 0);
            }
            if (ty <= min) {
                min = ty;
                normal.Set(0, 1, 0);
            }
            if (tz <= min) {
                normal.Set(0, 0, 1);
            }

            var vec = Vector3.Cross(tangent, normal).normalized;
            normal = Vector3.Cross(tangent, vec);
            var binormal = Vector3.Cross(tangent, normal);
			return ComputeFrenetFrames(segments, normal, binormal, closed);
        }

		public List<FrenetFrame> ComputeFrenetFrames(int segments, Vector3 normal, Vector3 binormal, bool closed = false) {
            var tangents = new Vector3[segments + 1];
            var normals = new Vector3[segments + 1];
            var binormals = new Vector3[segments + 1];

            for (int i = 0; i <= segments; i++) {
                var u = (1f * i) / segments;
                tangents[i] = GetTangentAt(u).normalized;
            }

			normals[0] = normal;
			binormals[0] = binormal;

            float theta;

            for (int i = 1; i <= segments; i++) {
                // copy previous
                normals[i] = normals[i - 1];
                binormals[i] = binormals[i - 1];

				var axis = Vector3.Cross(tangents[i - 1], tangents[i]);
                if (axis.magnitude > float.Epsilon) {
                    axis.Normalize();

                    float dot = Vector3.Dot(tangents[i - 1], tangents[i]);

                    theta = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f));

                    normals[i] = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, axis) * normals[i];
                }

                binormals[i] = Vector3.Cross(tangents[i], normals[i]).normalized;
            }

            if (closed) {
                theta = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normals[0], normals[segments]), -1f, 1f));
                theta /= segments;

                if (Vector3.Dot(tangents[0], Vector3.Cross(normals[0], normals[segments])) > 0f) {
                    theta = - theta;
                }

                for (int i = 1; i <= segments; i++) {
                    normals[i] = (Quaternion.AngleAxis(Mathf.Deg2Rad * theta * i, tangents[i]) * normals[i]);
                    binormals[i] = Vector3.Cross(tangents[i], normals[i]);
                }
            }

            var frames = new List<FrenetFrame>();
            int n = tangents.Length;
            for(int i = 0; i < n; i++) {
                var frame = new FrenetFrame(tangents[i], normals[i], binormals[i]);
                frames.Add(frame);
            }
            return frames;
		}

    }

}

