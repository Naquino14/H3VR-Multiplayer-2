using UnityEngine;

namespace DinoFracture
{
	public abstract class FractureGeometry : MonoBehaviour
	{
		public Material InsideMaterial;

		public GameObject FractureTemplate;

		public Transform PiecesParent;

		public int NumFracturePieces = 5;

		public int NumIterations = 2;

		public int NumGenerations = 1;

		public float FractureRadius;

		public FractureUVScale UVScale = FractureUVScale.Piece;

		public bool DistributeMass = true;

		private bool _processingFracture;

		public AsyncFractureResult Fracture()
		{
			if (NumGenerations == 0 || _processingFracture)
			{
				return null;
			}
			return FractureInternal(Vector3.zero);
		}

		public AsyncFractureResult Fracture(Vector3 localPos)
		{
			if (NumGenerations == 0 || _processingFracture)
			{
				return null;
			}
			return FractureInternal(localPos);
		}

		protected AsyncFractureResult Fracture(FractureDetails details, bool hideAfterFracture)
		{
			if (NumGenerations == 0 || _processingFracture)
			{
				return null;
			}
			if (FractureTemplate == null || FractureTemplate.GetComponent<MeshFilter>() == null)
			{
				Debug.LogError("DinoFracture: A fracture template with a MeshFilter component is required.");
			}
			_processingFracture = true;
			if (details.Mesh == null)
			{
				MeshFilter component = GetComponent<MeshFilter>();
				SkinnedMeshRenderer component2 = GetComponent<SkinnedMeshRenderer>();
				if (component == null && component2 == null)
				{
					Debug.LogError("DinoFracture: A mesh filter required if a mesh is not supplied.");
					return null;
				}
				Mesh mesh;
				if (component != null)
				{
					mesh = component.sharedMesh;
				}
				else
				{
					mesh = new Mesh();
					component2.BakeMesh(mesh);
				}
				details.Mesh = mesh;
			}
			if (details.MeshScale == Vector3.zero)
			{
				details.MeshScale = base.transform.localScale;
			}
			Transform piecesParent = ((!(PiecesParent == null)) ? PiecesParent : null);
			return FractureEngine.StartFracture(details, this, piecesParent, DistributeMass, hideAfterFracture);
		}

		protected void StopFracture()
		{
			_processingFracture = false;
		}

		protected abstract AsyncFractureResult FractureInternal(Vector3 localPos);

		internal virtual void OnFracture(OnFractureEventArgs args)
		{
			if (args.OriginalObject == this)
			{
				_processingFracture = false;
			}
		}
	}
}
