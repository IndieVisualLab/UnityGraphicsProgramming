using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using komietty.Math;

public class DemoRejection3d : MonoBehaviour {

    public int lEdge = 20;
    public int limit = 1000;
    public int loop = 400;
    public float threshold = 0.75f;
    public float minLength = 3f;
    public float pnoiseScale = 10f;
    public float pnoiseAspect = 1f;
    public bool isVertexAnimation;
    public GameObject prefab;
    Rejection3d rejection3d;

    struct DistributionData
    {
        public int texindex;
        public Vector3 position;
    }

    List<DistributionData> distributionDataList = new List<DistributionData>();

    void Start()
    {
        rejection3d = new Rejection3d(Vector3.zero, pnoiseScale, pnoiseAspect);
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        for (int i = 0; i < loop; i++) // or while(true)
        {
            yield return new WaitForSeconds(0.001f);
            foreach (var pos in rejection3d.Sequence(limit, threshold))
            {
                var pos_ = pos * lEdge;
                Instantiate(prefab, pos_, Quaternion.identity);
            }
        }
    }
}
