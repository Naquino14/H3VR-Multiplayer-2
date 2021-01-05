using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DinoFracture
{
	public sealed class FractureEngine : MonoBehaviour
	{
		private struct FractureInstance
		{
			public AsyncFractureResult Result;

			public IEnumerator Enumerator;

			public FractureInstance(AsyncFractureResult result, IEnumerator enumerator)
			{
				Result = result;
				Enumerator = enumerator;
			}
		}

		private static FractureEngine _instance;

		private bool _suspended;

		private List<FractureInstance> _runningFractures = new List<FractureInstance>();

		private static FractureEngine Instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject gameObject = new GameObject("Fracture Engine");
					_instance = gameObject.AddComponent<FractureEngine>();
				}
				return _instance;
			}
		}

		public static bool Suspended
		{
			get
			{
				return Instance._suspended;
			}
			set
			{
				Instance._suspended = value;
			}
		}

		public static bool HasFracturesInProgress => Instance._runningFractures.Count > 0;

		private void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = null;
			}
		}

		public static AsyncFractureResult StartFracture(FractureDetails details, FractureGeometry callback, Transform piecesParent, bool transferMass, bool hideAfterFracture)
		{
			AsyncFractureResult asyncFractureResult = new AsyncFractureResult();
			if (Suspended)
			{
				asyncFractureResult.SetResult(null, default(Bounds));
			}
			else if (details.Asynchronous)
			{
				IEnumerator enumerator = Instance.WaitForResults(FractureBuilder.Fracture(details), callback, piecesParent, transferMass, hideAfterFracture, asyncFractureResult);
				if (enumerator.MoveNext())
				{
					Instance._runningFractures.Add(new FractureInstance(asyncFractureResult, enumerator));
				}
			}
			else
			{
				IEnumerator enumerator2 = Instance.WaitForResults(FractureBuilder.Fracture(details), callback, piecesParent, transferMass, hideAfterFracture, asyncFractureResult);
				while (enumerator2.MoveNext())
				{
					Debug.LogWarning("DinoFracture: Sync fracture taking more than one iteration");
				}
			}
			return asyncFractureResult;
		}

		private void OnEditorUpdate()
		{
			UpdateFractures();
			if (_runningFractures.Count == 0)
			{
				Object.DestroyImmediate(base.gameObject);
			}
		}

		private void Update()
		{
			UpdateFractures();
		}

		private void UpdateFractures()
		{
			for (int num = _runningFractures.Count - 1; num >= 0; num--)
			{
				if (_runningFractures[num].Result.StopRequested)
				{
					_runningFractures.RemoveAt(num);
				}
				else if (!_runningFractures[num].Enumerator.MoveNext())
				{
					_runningFractures.RemoveAt(num);
				}
			}
		}

		private IEnumerator WaitForResults(AsyncFractureOperation operation, FractureGeometry callback, Transform piecesParent, bool transferMass, bool hideAfterFracture, AsyncFractureResult result)
		{
			while (!operation.IsComplete)
			{
				yield return null;
			}
			Rigidbody origBody = null;
			if (transferMass)
			{
				origBody = callback.GetComponent<Rigidbody>();
			}
			float density = 0f;
			if (origBody != null)
			{
				float mass = origBody.mass;
				origBody.SetDensity(1f);
				float mass2 = origBody.mass;
				density = mass / mass2;
				origBody.mass = mass;
			}
			List<FracturedMesh> meshes = operation.Result.GetMeshes();
			GameObject rootGO = new GameObject(callback.gameObject.name + " - Fracture Root");
			rootGO.transform.parent = piecesParent ?? callback.transform.parent;
			rootGO.transform.position = callback.transform.position;
			rootGO.transform.rotation = callback.transform.rotation;
			rootGO.transform.localScale = Vector3.one;
			Material[] sharedMaterials = callback.GetComponent<Renderer>().sharedMaterials;
			for (int i = 0; i < meshes.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(callback.FractureTemplate);
				gameObject.name = "Fracture Object " + i;
				gameObject.transform.parent = rootGO.transform;
				gameObject.transform.localPosition = meshes[i].Offset;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(value: true);
				MeshFilter component = gameObject.GetComponent<MeshFilter>();
				component.mesh = meshes[i].Mesh;
				MeshRenderer component2 = gameObject.GetComponent<MeshRenderer>();
				if (component2 != null)
				{
					Material[] array = new Material[sharedMaterials.Length - meshes[i].EmptyTriangleCount + 1];
					int num = 0;
					for (int j = 0; j < sharedMaterials.Length; j++)
					{
						if (!meshes[i].EmptyTriangles[j])
						{
							array[num++] = sharedMaterials[j];
						}
					}
					if (!meshes[i].EmptyTriangles[sharedMaterials.Length])
					{
						array[num] = callback.InsideMaterial;
					}
					component2.sharedMaterials = array;
				}
				MeshCollider component3 = gameObject.GetComponent<MeshCollider>();
				if (component3 != null)
				{
					component3.sharedMesh = component.sharedMesh;
				}
				if (transferMass && origBody != null)
				{
					Rigidbody component4 = gameObject.GetComponent<Rigidbody>();
					if (component4 != null)
					{
						component4.SetDensity(density);
						component4.mass = component4.mass;
					}
				}
				FractureGeometry component5 = gameObject.GetComponent<FractureGeometry>();
				if (component5 != null)
				{
					component5.InsideMaterial = callback.InsideMaterial;
					component5.FractureTemplate = callback.FractureTemplate;
					component5.PiecesParent = callback.PiecesParent;
					component5.NumGenerations = callback.NumGenerations - 1;
					component5.DistributeMass = callback.DistributeMass;
				}
			}
			OnFractureEventArgs args = new OnFractureEventArgs(callback, rootGO);
			if (Application.isPlaying)
			{
				callback.gameObject.SendMessage("OnFracture", args, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				callback.OnFracture(args);
			}
			if (hideAfterFracture)
			{
				callback.gameObject.SetActive(value: false);
			}
			if (Application.isPlaying)
			{
				Transform transform = rootGO.transform;
				for (int k = 0; k < transform.childCount; k++)
				{
					transform.GetChild(k).gameObject.SendMessage("OnFracture", args, SendMessageOptions.DontRequireReceiver);
				}
			}
			result.SetResult(rootGO, operation.Result.EntireMeshBounds);
		}
	}
}
