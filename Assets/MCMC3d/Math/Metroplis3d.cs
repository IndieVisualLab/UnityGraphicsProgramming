using UnityEngine;
using System.Collections.Generic;

namespace komietty.Math
{
    public class Metropolis3d
    {
        public static readonly int limitResetLoopCount = 100;
        public static readonly int weightReferenceloopCount = 500;
        public Vector4[] Data { get; private set; }
        public Vector3 Scale { get; private set; }

        Vector3 _curr;
        float _currDensity = 0f;

        public Metropolis3d(Vector4[] data, Vector3 scale)
        {
            this.Data = data;
            this.Scale = scale;
        }

        public void Reset()
        {
            for (var i = 0; _currDensity <= 0f && i < limitResetLoopCount; i++)
            {
                _curr = new Vector3(Scale.x * Random.value, Scale.y * Random.value, Scale.z * Random.value);
                _currDensity = Density(_curr);
            }
        }

		public IEnumerable<Vector3> Chain(int nInitialize, int limit, float threshold)
        {
            Reset();

            for (var i = 0; i < nInitialize; i++)
                Next(threshold);

            for (var i = 0; i < limit; i++)
            {
                yield return _curr;
                Next(threshold);
            }
        }

        void Next(float threshold)
        {
            Vector3 next = GaussianDistribution3d.GenerateRandomPointStandard() + _curr;

            var densityNext = Density(next);
            bool flag1 = _currDensity <= 0f || Mathf.Min(1f, densityNext / _currDensity) >= Random.value;
            bool flag2 = densityNext > threshold;
            if (flag1 && flag2)
            {
                _curr = next;
                _currDensity = densityNext;
            }
        }

        float Density(Vector3 pos)
        {
            float weight = 0f;
            for (int i = 0; i < weightReferenceloopCount; i++)
            {
                int id = (int)Mathf.Floor(Random.value * (Data.Length - 1));
                Vector3 posi = Data[id];
                float mag = Vector3.SqrMagnitude(pos - posi);
                weight += Mathf.Exp(-mag) * Data[id].w;
            }
            return weight;
        }
    }
}