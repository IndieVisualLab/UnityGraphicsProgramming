using System;
using UnityEngine;

namespace StableFluid
{
    public class MouseSourceProvider : MonoBehaviour
    {
        private string source2dProp = "_Source";
        private string sourceRadiusProp = "_Radius";
        private int source2dId, sourceRadiusId;
        private Vector3 lastMousePos;

        [SerializeField]
        private Material addSourceMat;

        [SerializeField]
        private float sourceRadius = 0.03f;

        public RenderTexture addSourceTex;
        public SourceEvent OnSourceUpdated;

        void Awake()
        {
            source2dId = Shader.PropertyToID(source2dProp);
            sourceRadiusId = Shader.PropertyToID(sourceRadiusProp);
        }

        void Update()
        {
            InitializeSourceTex(Screen.width, Screen.height);
            UpdateSource();
        }

        void OnDestroy()
        {
            ReleaseForceField();
        }

        void InitializeSourceTex(int width, int height)
        {
            if (addSourceTex == null || addSourceTex.width != width || addSourceTex.height != height)
            {
                ReleaseForceField();
                addSourceTex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
            }
        }

        void UpdateSource()
        {
            var mousePos = Input.mousePosition;
            var dpdt = UpdateMousePos(mousePos);
            var velocitySource = Vector2.zero;
            var uv = Vector2.zero;

            if (Input.GetMouseButton(0))
            {
                uv = Camera.main.ScreenToViewportPoint(mousePos);
                velocitySource = Vector2.ClampMagnitude(dpdt, 1f);
                addSourceMat.SetVector(source2dId, new Vector4(velocitySource.x, velocitySource.y, uv.x, uv.y));
                addSourceMat.SetFloat(sourceRadiusId, sourceRadius);
                Graphics.Blit(null, addSourceTex, addSourceMat);
                NotifySourceTexUpdate();
            }
            else
            {
                NotifyNoSourceTexUpdate();
            }
        }

        void NotifySourceTexUpdate()
        {
            OnSourceUpdated.Invoke(addSourceTex);
        }

        void NotifyNoSourceTexUpdate()
        {
            OnSourceUpdated.Invoke(null);
        }

        Vector3 UpdateMousePos(Vector3 mousePos)
        {
            var dpdt = mousePos - lastMousePos;
            lastMousePos = mousePos;
            return dpdt;
        }

        void ReleaseForceField()
        {
            Destroy(addSourceTex);
        }

        [Serializable]
        public class SourceEvent : UnityEngine.Events.UnityEvent<RenderTexture> { }
    }
}