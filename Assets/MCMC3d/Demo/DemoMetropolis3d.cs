using System.Collections;
using UnityEngine;
using komietty.Math;

public class DemoMetropolis3d : MonoBehaviour
{
    public int lEdge = 20;
    public int nInitialize = 100;
    public int nlimit = 100;
    public int loop = 400;
    public float threshold = -100;
    public GameObject[] prefabArr = new GameObject[0];
    Vector4[] data;
    Metropolis3d metropolis;

    void Start()
    {
        data = new Vector4[lEdge * lEdge * lEdge];
        Prepare();
        metropolis = new Metropolis3d(data, lEdge * Vector3.one);
        StartCoroutine(Generate());
    }

    void Prepare()
    {
        var sn = new SimplexNoiseGenerator();
        for (int x = 0; x < lEdge; x++)
            for (int y = 0; y < lEdge; y++)
                for (int z = 0; z < lEdge; z++)
                {
                    var i = x + lEdge * y + lEdge * lEdge * z;
                    var val = sn.noise(x, y, z);
                    data[i] = new Vector4(x, y, z, val);
                }
    }

    IEnumerator Generate()
    {
        for (int i = 0; i < loop; i++) // or while(true)
        {
            int rand = (int)Mathf.Floor(Random.value * prefabArr.Length);
            var prefab = prefabArr[rand];
            yield return new WaitForSeconds(0.1f);
            foreach (var pos in metropolis.Chain(nInitialize, nlimit, threshold))
            {
                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }
}
