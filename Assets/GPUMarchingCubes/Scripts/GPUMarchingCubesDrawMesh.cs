using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GPUMarchingCubesDrawMesh : MonoBehaviour {
    
    #region public
    public int segmentNum = 32;                 // グリッドの一辺の分割数
    
    [Range(0,1)]
    public float threashold = 0.5f;             // メッシュ化するスカラー値のしきい値
    public Material mat;                        // レンダリング用のマテリアル

    public Color DiffuseColor = Color.green;    // ディフューズカラー
    public Color EmissionColor = Color.black;   // 発光色
    public float EmissionIntensity = 0;         // 発光の強さ

    [Range(0,1)]
    public float metallic = 0;                  // メタリック感
    [Range(0, 1)]
    public float glossiness = 0.5f;             // 光沢感
    #endregion

    #region private
    int vertexMax = 0;                          // 頂点数
    Mesh[] meshs = null;                        // Mesh配列
    Material[] materials = null;                // Meshごとのマテリアル配列
    float renderScale = 1f / 32f;               // 表示スケール

    MarchingCubesDefines mcDefines = null;      // MarchingCubes用定数配列群
    #endregion

    void Initialize()
    {
        vertexMax = segmentNum * segmentNum * segmentNum;
        
        Debug.Log("VertexMax " + vertexMax);

        // 1Cubeの大きさをsegmentNumで分割してレンダリング時の大きさを決める
        renderScale = 1f / segmentNum;

        CreateMesh();

        // シェーダーで使うMarchingCubes用の定数配列の初期化
        mcDefines = new MarchingCubesDefines();
    }

    void CreateMesh()
    {
        // Meshの頂点数は65535が上限なので、Meshを分割する
        int vertNum = 65535;
        int meshNum = Mathf.CeilToInt((float)vertexMax / vertNum);  // 分割するMeshの数
        Debug.Log("meshNum " + meshNum );

        meshs = new Mesh[meshNum];
        materials = new Material[meshNum];

        // Meshのバウンズ計算
        Bounds bounds = new Bounds(
            transform.position, 
            new Vector3(segmentNum, segmentNum, segmentNum) * renderScale
        );

        int id = 0;
        for (int i = 0; i < meshNum; i++)
        {
            // 頂点作成
            Vector3[] vertices = new Vector3[vertNum];
            int[] indices = new int[vertNum];
            for(int j = 0; j < vertNum; j++)
            {
                vertices[j].x = (id % segmentNum);
                vertices[j].y = ((id / segmentNum) % segmentNum);
                vertices[j].z = ((id / (segmentNum * segmentNum)) % segmentNum);

                indices[j] = j;
                id++;
            }

            // Mesh作成
            meshs[i] = new Mesh();
            meshs[i].vertices = vertices;
            meshs[i].SetIndices(indices, MeshTopology.Points, 0);   // GeometryShaderでポリゴンを作るのでMeshTopologyはPointsで良い
            meshs[i].bounds = bounds;

            materials[i] = new Material(mat);
        }
    }

    void RenderMesh()
    {
        // 描画するサイズ、位置、姿勢を事前に計算する
        Vector3 halfSize = new Vector3(segmentNum, segmentNum, segmentNum) * renderScale * 0.5f;
        Matrix4x4 trs = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        for (int i = 0; i < meshs.Length; i++)
        {
            materials[i].SetPass(0);    // 描画するパスをセット

            // 各種変数を渡す
            materials[i].SetInt("_SegmentNum", segmentNum);
            materials[i].SetFloat("_Scale", renderScale);
            materials[i].SetFloat("_Threashold", threashold);
            materials[i].SetFloat("_Metallic", metallic);
            materials[i].SetFloat("_Glossiness", glossiness);
            materials[i].SetFloat("_EmissionIntensity", EmissionIntensity);
            
            materials[i].SetVector("_HalfSize", halfSize);
            materials[i].SetColor("_DiffuseColor", DiffuseColor);
            materials[i].SetColor("_EmissionColor", EmissionColor);
            materials[i].SetMatrix("_Matrix", trs);

            materials[i].SetBuffer("vertexOffset", mcDefines.vertexOffsetBuffer);
            materials[i].SetBuffer("cubeEdgeFlags", mcDefines.cubeEdgeFlagsBuffer);
            materials[i].SetBuffer("edgeConnection", mcDefines.edgeConnectionBuffer);
            materials[i].SetBuffer("edgeDirection", mcDefines.edgeDirectionBuffer);
            materials[i].SetBuffer("triangleConnectionTable", mcDefines.triangleConnectionTableBuffer);

            Graphics.DrawMesh(meshs[i], Matrix4x4.identity, materials[i], 0);
        }
    }

    // Use this for initialization
    void Start ()
    {
        Initialize();
    }

    void Update()
    {
        RenderMesh();
    }

    void OnDestroy()
    {
        mcDefines.ReleaseBuffer();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
