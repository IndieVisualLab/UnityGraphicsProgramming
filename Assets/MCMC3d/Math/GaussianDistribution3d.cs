using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace komietty.Math
{
    public static class GaussianDistribution3d
    {
        public static float ProbablityStandard()
        {
            var x = Random.value;
            var y = Random.value;
            var z = Random.value;
            return Mathf.Exp(-0.5f * (x * x + y * y + z * z)) / Mathf.Pow(2 * Mathf.PI, 1.5f);
        }

        public static Vector3 GenerateRandomPointStandard()
        {
            var x = RandomGenerator.rand_gaussian(0f, 1f);
            var y = RandomGenerator.rand_gaussian(0f, 1f);
            var z = RandomGenerator.rand_gaussian(0f, 1f);
            return new Vector3(x, y, z);
        }

        public static Vector3 GenerateRandomPoint(Vector3 arg, Matrix4x4 sigma)
        {
            var c00 = sigma.m00 / Mathf.Sqrt(sigma.m00);
            var c10 = sigma.m10 / Mathf.Sqrt(sigma.m00);
            var c20 = sigma.m21 / Mathf.Sqrt(sigma.m00);
            var c11 = Mathf.Sqrt(sigma.m11 - c10 * c10);
            var c21 = (sigma.m21 - c20 * c10) / c11;
            var c22 = Mathf.Sqrt(sigma.m22 - (c20 * c20 + c21 * c21));
            var r1 = RandomGenerator.rand_gaussian(0f, 1f);
            var r2 = RandomGenerator.rand_gaussian(0f, 1f);
            var r3 = RandomGenerator.rand_gaussian(0f, 1f);
            var x = c00 * r1;
            var y = c10 * r1 + c11 * r2;
            var z = c20 * r1 + c21 * r2 + c22 * r3;
            return new Vector3(x, y, z);
        }
    }
}