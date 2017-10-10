using UnityEngine;

namespace Render
{
    public class RenderEffect : MonoBehaviour
    {
        public TextureEvent OnCreateTex;
        public RenderTexture Output { get; private set; }

        [SerializeField] string propName = "_PropName";
        [SerializeField] Material[] effects;
        [SerializeField] bool show = true;
        [SerializeField] RenderTextureFormat format = RenderTextureFormat.ARGBFloat;
        [SerializeField] TextureWrapMode wrapMode;
        [SerializeField] int downSample = 0;

        RenderTexture[] rts = new RenderTexture[2];

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha6))
                show = !show;
        }

        void OnRenderImage(RenderTexture s, RenderTexture d)
        {
            CheckRTs(s);
            Graphics.Blit(s, rts[0]);
            foreach (var m in effects)
            {
                Graphics.Blit(rts[0], rts[1], m);
                SwapRTs();
            }

            Graphics.Blit(rts[0], Output);
            Shader.SetGlobalTexture(propName, Output);
            if (show)
                Graphics.Blit(Output, d);
            else
                Graphics.Blit(s, d);
        }

        void CheckRTs(RenderTexture s)
        {
            if (rts[0] == null || rts[0].width != s.width >> downSample || rts[0].height != s.height >> downSample)
            {
                for (var i = 0; i < rts.Length; i++)
                {
                    var rt = rts[i];
                    rts[i] = RenderUtility.CreateRenderTexture(s.width >> downSample, s.height >> downSample, 16, format, wrapMode, FilterMode.Bilinear, rt);
                }
                Output = RenderUtility.CreateRenderTexture(s.width >> downSample, s.height >> downSample, 16, format, wrapMode, FilterMode.Bilinear, Output);
                OnCreateTex.Invoke(Output);
            }
        }

        void SwapRTs()
        {
            var tmp = rts[0];
            rts[0] = rts[1];
            rts[1] = tmp;
        }

        void OnDisabled()
        {
            foreach (var rt in rts)
                RenderUtility.ReleaseRenderTexture(rt);
                RenderUtility.ReleaseRenderTexture(Output);
        }

        [System.Serializable]
        public class TextureEvent : UnityEngine.Events.UnityEvent<Texture> { }
    }
}

