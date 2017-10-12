using System.Collections.Generic;
using UnityEngine;


namespace komietty.Math
{
    public class Rejection3d
    {
        public Vector3 PnoiseOrigin { get; private set; }
        public float NoiseScale { get; private set; }
        public float NoiseAspect { get; private set; }
        SimplexNoiseGenerator sn;

        public Rejection3d(Vector3 origin, float scale, float aspect)
        {
            this.PnoiseOrigin = origin;
            this.NoiseScale = scale;
            this.NoiseAspect = aspect;
            this.sn = new SimplexNoiseGenerator();
        }

        public IEnumerable<Vector3> Sequence(int limit, float threshold)
        {
            float randomX;
            float randomY;
            float randomZ;
            float noiseValue;

            for (int i = 0; i < limit; i++)
            {
                randomX = Random.value;
                randomY = Random.value;
                randomZ = Random.value;
                noiseValue = sn.getDensityFloat(
                    new Vector3(randomX, randomY, randomZ) * NoiseScale
                    );

                if (noiseValue > threshold)
                    yield return new Vector3(randomX, randomY, randomZ);
                else
                    Debug.Log("False");

            }
        }
    }
}

