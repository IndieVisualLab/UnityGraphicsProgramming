using UnityEngine;

public class SimpleComputeShader_Array : MonoBehaviour
{
    public ComputeShader computeShader;
    int kernelIndex_KernelFunction_A;
    int kernelIndex_KernelFunction_B;

    // ComputeShader で算出した結果を保存しておくバッファ。

    ComputeBuffer intComputeBuffer;

    void Start()
    {
        // (1) カーネルのインデックスを保存します。

        this.kernelIndex_KernelFunction_A = this.computeShader.FindKernel("KernelFunction_A");
        this.kernelIndex_KernelFunction_B = this.computeShader.FindKernel("KernelFunction_B");

        // (2) ComputeShader で計算した結果を保存するためのバッファ (ComputeBuffer) を設定します。
        // ComputeShader 内に、同じ型で同じ名前のバッファが定義されている必要があります。

        // ComputeBuffer は どの程度の領域を確保するかを指定して初期化する必要があります。
        // この例だと int 4 つ分です。

        this.intComputeBuffer = new ComputeBuffer(4, sizeof(int));
        this.computeShader.SetBuffer
            (this.kernelIndex_KernelFunction_A, "intBuffer", this.intComputeBuffer);

        // (3) 必要なら ComputeShader にパラメータを渡します。

        this.computeShader.SetInt("intValue", 1);

        // (3) ComputeShader を Dispatch メソッドで実行します。
        // 指定したインデックスのカーネルを指定したグループ数で実行します。
        // グループ数は X * Y * Z で指定します。この例では 1 * 1 * 1 = 1 グループです。

        this.computeShader.Dispatch(this.kernelIndex_KernelFunction_A, 1, 1, 1);

        // (4) 実行結果を取得して確認します。

        int[] result = new int[4];

        this.intComputeBuffer.GetData(result);

        Debug.Log("RESULT : KernelFunction_A");

        for (int i = 0; i < 4; i++)
        {
            Debug.Log(result[i]);
        }

        // (5) ComputerShader 内にある異なるカーネルを実行します。
        // ここではカーネル「KernelFunction_A」で使ったバッファを使いまわします。

        this.computeShader.SetBuffer
            (this.kernelIndex_KernelFunction_B, "intBuffer", this.intComputeBuffer);
        this.computeShader.Dispatch(this.kernelIndex_KernelFunction_B, 1, 1, 1);

        this.intComputeBuffer.GetData(result);

        Debug.Log("RESULT : KernelFunction_B");

        for (int i = 0; i < 4; i++)
        {
            Debug.Log(result[i]);
        }

        // (5) 使い終わったバッファは必要なら解放します。

        this.intComputeBuffer.Release();
    }
}