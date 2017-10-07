using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling {

	public enum ProceduralModelingMaterial {
		Standard,
		UV,
		Normal
	};

	[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
	[ExecuteInEditMode]
	public abstract class ProceduralModelingBase : MonoBehaviour {

		public MeshFilter Filter {
			get {
				if(filter == null) {
					filter = GetComponent<MeshFilter>();
				}
				return filter;
			}
		}

		public MeshRenderer Renderer {
			get {
				if(renderer == null) {
					renderer = GetComponent<MeshRenderer>();
				}
				return renderer;
			}
		}

		MeshFilter filter;
		new MeshRenderer renderer;

		[SerializeField] protected ProceduralModelingMaterial materialType = ProceduralModelingMaterial.UV;

		protected virtual void Start () {
			Rebuild();
		}

		public void Rebuild() {
			if(Filter.sharedMesh != null) {
				if(Application.isPlaying) {
					Destroy(Filter.sharedMesh);
				} else {
					DestroyImmediate(Filter.sharedMesh);
				}
			} 
			Filter.sharedMesh = Build();
			Renderer.sharedMaterial = LoadMaterial(materialType);
		}

		protected virtual Material LoadMaterial(ProceduralModelingMaterial type) {
			switch(type) {
			case ProceduralModelingMaterial.UV:
				return Resources.Load<Material>("Materials/UV");
			case ProceduralModelingMaterial.Normal:
				return Resources.Load<Material>("Materials/Normal");
			}
			return Resources.Load<Material>("Materials/Standard");
		}

		protected abstract Mesh Build();

	}
		
}

