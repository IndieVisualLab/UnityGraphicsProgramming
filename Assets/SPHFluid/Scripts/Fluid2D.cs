using UnityEngine;
using System.Runtime.InteropServices;

namespace Kodai.Fluid.SPH {
    public struct FluidParticle {
        public Vector2 Position;
        public Vector2 Velocity;
    };

    public class Fluid2D : FluidBase<FluidParticle> {
        
        [SerializeField] private float ballRadius = 0.1f;           // 粒子位置初期化時の円半径
        [SerializeField] private float MouseInteractionRadius = 1f; // マウスインタラクションの範囲の広さ
        
        private bool isMouseDown;
        private Vector3 screenToWorldPointPos;

        /// <summary>
        /// パーティクル初期位置の設定
        /// </summary>
        /// <param name="particles"></param>
        protected override void InitParticleData(ref FluidParticle[] particles) {
            for (int i = 0; i < NumParticles; i++) {
                particles[i].Velocity = Vector2.zero;
                particles[i].Position = range / 2f + Random.insideUnitCircle * ballRadius;  // 円形に粒子を初期化する
            }
        }

        /// <summary>
        /// ComputeShaderの定数バッファに追加する
        /// </summary>
        /// <param name="cs"></param>
        protected override void AdditionalCSParams(ComputeShader cs) {

            if (Input.GetMouseButtonDown(0)) {
                isMouseDown = true;
            }

            if(Input.GetMouseButtonUp(0)) {
                isMouseDown = false;
            }

            if (isMouseDown) {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10f;
                screenToWorldPointPos = Camera.main.ScreenToWorldPoint(mousePos);
            }

            cs.SetVector("_MousePos", screenToWorldPointPos);
            cs.SetFloat("_MouseRadius", MouseInteractionRadius);
            cs.SetBool("_MouseDown", isMouseDown);
        }

    }
}