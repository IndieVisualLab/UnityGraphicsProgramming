using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace StableFluid
{
    public struct GPUThreads
    {
        public int x;
        public int y;
        public int z;

        public GPUThreads(uint x, uint y, uint z)
        {
            this.x = (int)x;
            this.y = (int)y;
            this.z = (int)z;
        }
    }

    public static class DirectCompute5_0
    {
        //Use DirectCompute 5.0 on DirectX11 hardware.
        public const int MAX_THREAD   = 1024;
        public const int MAX_X        = 1024;
        public const int MAX_Y        = 1024;
        public const int MAX_Z        = 64;
        public const int MAX_DISPATCH = 65535;
        public const int MAX_PROCESS  = MAX_DISPATCH * MAX_THREAD;
    } 

    public abstract class SolverBase : MonoBehaviour
    {
        #region Variables

        protected enum ComputeKernels
        {
            AddSourceDensity,
            DiffuseDensity,
            AdvectDensity,
            AdvectDensityFromExt,
            SwapDensity,

            AddSourceVelocity,
            DiffuseVelocity,
            AdvectVelocity,
            SwapVelocity,
            ProjectStep1,
            ProjectStep2,
            ProjectStep3,

            Draw
        }

        protected Dictionary<ComputeKernels, int> kernelMap = new Dictionary<ComputeKernels, int>();
        protected GPUThreads    gpuThreads;
        protected RenderTexture solverTex;
        protected RenderTexture densityTex;
        protected RenderTexture velocityTex;
        protected RenderTexture prevTex;
        protected string solverProp = "solver";
        protected string densityProp = "density";
        protected string velocityProp = "velocity";
        protected string prevProp = "prev";
        protected string sourceProp = "source";
        protected string diffProp = "diff";
        protected string viscProp = "visc";
        protected string dtProp = "dt";
        protected string velocityCoefProp = "velocityCoef";
        protected string densityCoefProp = "densityCoef";
        protected int solverId, densityId, velocityId, prevId, sourceId, diffId, viscId, dtId, velocityCoefId, densityCoefId, solverTexId;
        protected int width, height;


        [SerializeField]
        protected ComputeShader computeShader;

        [SerializeField]
        protected string solverTexProp = "SolverTex";

        [SerializeField]
        protected float diff;

        [SerializeField]
        protected float visc;

        [SerializeField]
        protected float velocityCoef;

        [SerializeField]
        protected float densityCoef;

        [SerializeField]
        protected bool isDensityOnly = false;

        [SerializeField]
        protected int lod = 0;

        [SerializeField]
        protected bool debug = false;

        [SerializeField]
        protected Material debugMat;

        [SerializeField] RenderTexture sourceTex;
        public RenderTexture SorceTex { set { sourceTex = value; } get { return sourceTex; } }

        #endregion

        #region unity builtin

        protected virtual void Start()
        {
            Initialize();
        }

        protected virtual void Update()
        {
            if (width != Screen.width || height != Screen.height) InitializeComputeShader();
            computeShader.SetFloat(diffId, diff);
            computeShader.SetFloat(viscId, visc);
            computeShader.SetFloat(diffId, diff);
            computeShader.SetFloat(viscId, visc);
            computeShader.SetFloat(dtId, Time.deltaTime);
            computeShader.SetFloat(velocityCoefId, velocityCoef);
            computeShader.SetFloat(densityCoefId, densityCoef);

            if (!isDensityOnly) VelocityStep();
            DensityStep();

            computeShader.SetTexture(kernelMap[ComputeKernels.Draw], densityId, densityTex);
            computeShader.SetTexture(kernelMap[ComputeKernels.Draw], velocityId, velocityTex);
            computeShader.SetTextureFromGlobal(kernelMap[ComputeKernels.Draw], solverId, solverTexId);
            computeShader.Dispatch(kernelMap[ComputeKernels.Draw], Mathf.CeilToInt(solverTex.width / gpuThreads.x), Mathf.CeilToInt(solverTex.height / gpuThreads.y), 1);

            Shader.SetGlobalTexture(solverTexId, solverTex);
        }

        void OnDestroy()
        {
            CleanUp();
        }

        #endregion

        #region Initialize

        protected virtual void Initialize()
        {
            uint threadX, threadY, threadZ;

            InitialCheck();

            kernelMap = System.Enum.GetValues(typeof(ComputeKernels))
                .Cast<ComputeKernels>()
                .ToDictionary(t => t, t => computeShader.FindKernel(t.ToString()));

            computeShader.GetKernelThreadGroupSizes(kernelMap[ComputeKernels.Draw], out threadX, out threadY, out threadZ);

            gpuThreads = new GPUThreads(threadX, threadY, threadZ);
            solverTexId = Shader.PropertyToID(solverTexProp);

            solverId = Shader.PropertyToID(solverProp);
            densityId = Shader.PropertyToID(densityProp);
            velocityId = Shader.PropertyToID(velocityProp);
            prevId = Shader.PropertyToID(prevProp);
            sourceId = Shader.PropertyToID(sourceProp);
            diffId = Shader.PropertyToID(diffProp);
            viscId = Shader.PropertyToID(viscProp);
            dtId = Shader.PropertyToID(dtProp);
            velocityCoefId = Shader.PropertyToID(velocityCoefProp);
            densityCoefId = Shader.PropertyToID(densityCoefProp);

            InitializeComputeShader();

            if (debug)
            {
                if (debugMat == null) return;
                debugMat.mainTexture = solverTex;
            }
        }

        protected virtual void InitialCheck()
        {
            Assert.IsTrue(SystemInfo.graphicsShaderLevel >= 50, "Under the DirectCompute5.0 (DX11 GPU) doesn't work : StableFluid");
            Assert.IsTrue(gpuThreads.x * gpuThreads.y * gpuThreads.z <= DirectCompute5_0.MAX_PROCESS, "Resolution is too heigh : Stablefluid");
            Assert.IsTrue(gpuThreads.x <= DirectCompute5_0.MAX_X, "THREAD_X is too large : StableFluid");
            Assert.IsTrue(gpuThreads.y <= DirectCompute5_0.MAX_Y, "THREAD_Y is too large : StableFluid");
            Assert.IsTrue(gpuThreads.z <= DirectCompute5_0.MAX_Z, "THREAD_Z is too large : StableFluid");
        }

        protected abstract void InitializeComputeShader();

        #endregion


        #region StableFluid gpu kernel steps

        protected abstract void DensityStep();

        protected abstract void VelocityStep();

        #endregion

        #region render texture

        public RenderTexture CreateRenderTexture(int width, int height, int depth, RenderTextureFormat format, RenderTexture rt = null)
        {
            if (rt != null)
            {
                if (rt.width == width && rt.height == height) return rt;
            }

            ReleaseRenderTexture(rt);
            rt = new RenderTexture(width, height, depth, format);
            rt.enableRandomWrite = true;
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.filterMode = FilterMode.Point;
            rt.Create();
            ClearRenderTexture(rt, Color.clear);
            return rt;
        }

        public RenderTexture CreateVolumetricRenderTexture(int width, int height, int volumeDepth, int depth, RenderTextureFormat format, RenderTexture rt = null)
        {
            if (rt != null)
            {
                if (rt.width == width && rt.height == height) return rt;
            }

            ReleaseRenderTexture(rt);
            rt = new RenderTexture(width, height, depth, format);
            rt.dimension = TextureDimension.Tex3D;
            rt.volumeDepth = volumeDepth;
            rt.enableRandomWrite = true;
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.filterMode = FilterMode.Point;
            rt.Create();
            ClearRenderTexture(rt, Color.clear);
            return rt;
        }

        public void ReleaseRenderTexture(RenderTexture rt)
        {
            if (rt == null) return;

            rt.Release();
            Destroy(rt);
        }

        public void ClearRenderTexture(RenderTexture target, Color bg)
        {
            var active = RenderTexture.active;
            RenderTexture.active = target;
            GL.Clear(true, true, bg);
            RenderTexture.active = active;
        }

        #endregion

        #region release

        void CleanUp()
        {
            ReleaseRenderTexture(solverTex);
            ReleaseRenderTexture(densityTex);
            ReleaseRenderTexture(velocityTex);
            ReleaseRenderTexture(prevTex);

#if UNITY_EDITOR
            Debug.Log("Buffer released");
#endif
        }

        #endregion
    }
}
