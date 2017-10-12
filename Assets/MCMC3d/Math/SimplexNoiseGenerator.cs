using UnityEngine;
using System.Collections;


namespace komietty.Math
{
    public class SimplexNoiseGenerator
    {
        private int[] A = new int[3];
        private float s, u, v, w;
        private int i, j, k;
        private float onethird = 0.333333333f;
        private float onesixth = 0.166666667f;
        private int[] T;

        public SimplexNoiseGenerator()
        {
            if (T == null)
            {
                System.Random rand = new System.Random();
                T = new int[8];
                for (int q = 0; q < 8; q++)
                    T[q] = rand.Next();
            }
        }

        public SimplexNoiseGenerator(string seed)
        {
            T = new int[8];
            string[] seed_parts = seed.Split(new char[] { ' ' });

            for (int q = 0; q < 8; q++)
            {
                int b;
                try
                {
                    b = int.Parse(seed_parts[q]);
                }
                catch
                {
                    b = 0x0;
                }
                T[q] = b;
            }
        }

        public SimplexNoiseGenerator(int[] seed)
        { // {0x16, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a}
            T = seed;
        }

        public string GetSeed()
        {
            string seed = "";

            for (int q = 0; q < 8; q++)
            {
                seed += T[q].ToString();
                if (q < 7)
                    seed += " ";
            }

            return seed;
        }

        public float coherentNoise(float x, float y, float z, int octaves = 1, int multiplier = 25, float amplitude = 0.5f, float lacunarity = 2, float persistence = 0.9f)
        {
            Vector3 v3 = new Vector3(x, y, z) / multiplier;
            float val = 0;
            for (int n = 0; n < octaves; n++)
            {
                val += noise(v3.x, v3.y, v3.z) * amplitude;
                v3 *= lacunarity;
                amplitude *= persistence;
            }
            return val;
        }

        public int getDensity(Vector3 loc)
        {
            float val = coherentNoise(loc.x, loc.y, loc.z);
            return (int)Mathf.Lerp(0, 255, val);
        }

        public float getDensityFloat(Vector3 loc)
        {
            float val = noise(loc.x, loc.y, loc.z);
            return Mathf.Lerp(0f, 1f, val);
        }

        // Simplex Noise Generator
        public float noise(float x, float y, float z)
        {
            s = (x + y + z) * onethird;
            i = fastfloor(x + s);
            j = fastfloor(y + s);
            k = fastfloor(z + s);

            s = (i + j + k) * onesixth;
            u = x - i + s;
            v = y - j + s;
            w = z - k + s;

            A[0] = 0; A[1] = 0; A[2] = 0;

            int hi = u >= w ? u >= v ? 0 : 1 : v >= w ? 1 : 2;
            int lo = u < w ? u < v ? 0 : 1 : v < w ? 1 : 2;

            return kay(hi) + kay(3 - hi - lo) + kay(lo) + kay(0);
        }

        float kay(int a)
        {
            s = (A[0] + A[1] + A[2]) * onesixth;
            float x = u - A[0] + s;
            float y = v - A[1] + s;
            float z = w - A[2] + s;
            float t = 0.6f - x * x - y * y - z * z;
            int h = shuffle(i + A[0], j + A[1], k + A[2]);
            A[a]++;
            if (t < 0) return 0;
            int b5 = h >> 5 & 1;
            int b4 = h >> 4 & 1;
            int b3 = h >> 3 & 1;
            int b2 = h >> 2 & 1;
            int b1 = h & 3;

            float p = b1 == 1 ? x : b1 == 2 ? y : z;
            float q = b1 == 1 ? y : b1 == 2 ? z : x;
            float r = b1 == 1 ? z : b1 == 2 ? x : y;

            p = b5 == b3 ? -p : p;
            q = b5 == b4 ? -q : q;
            r = b5 != (b4 ^ b3) ? -r : r;
            t *= t;
            return 8 * t * t * (p + (b1 == 0 ? q + r : b2 == 0 ? q : r));
        }

        int shuffle(int i, int j, int k)
        {
            return b(i, j, k, 0) + b(j, k, i, 1) + b(k, i, j, 2) + b(i, j, k, 3) + b(j, k, i, 4) + b(k, i, j, 5) + b(i, j, k, 6) + b(j, k, i, 7);
        }

        int b(int i, int j, int k, int B)
        {
            return T[b(i, B) << 2 | b(j, B) << 1 | b(k, B)];
        }

        int b(int N, int B)
        {
            return N >> B & 1;
        }

        int fastfloor(float n)
        {
            return n > 0 ? (int)n : (int)n - 1;
        }
    }
}