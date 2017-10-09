using UnityEngine;
using System.Collections;

namespace Kodai.Fluid.SPH {

    [RequireComponent(typeof(Fluid2D))]
    public class FluidRenderer : MonoBehaviour {

        public Fluid2D solver;
        public Material RenderParticleMat;
        public Color WaterColor;

        void OnRenderObject() {
            DrawParticle();
        }

        void DrawParticle() {

            RenderParticleMat.SetPass(0);
            RenderParticleMat.SetColor("_WaterColor", WaterColor);
            RenderParticleMat.SetBuffer("_ParticlesBuffer", solver.ParticlesBufferRead);
            Graphics.DrawProcedural(MeshTopology.Points, solver.NumParticles);
        }
    }
}