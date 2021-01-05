using System;
using UnityEngine;

namespace DinoFracture
{
	public class PreFracturedGeometry : FractureGeometry
	{
		public GameObject GeneratedPieces;

		public Bounds EntireMeshBounds;

		private Action<PreFracturedGeometry> _completionCallback;

		private AsyncFractureResult _runningFracture;

		private bool _ignoreOnFractured;

		public AsyncFractureResult RunningFracture => _runningFracture;

		private void Start()
		{
			Prime();
		}

		public void Prime()
		{
			if (GeneratedPieces != null)
			{
				bool activeSelf = base.gameObject.activeSelf;
				base.gameObject.SetActive(value: false);
				GeneratedPieces.SetActive(value: true);
				GeneratedPieces.SetActive(value: false);
				base.gameObject.SetActive(activeSelf);
			}
		}

		public void GenerateFractureMeshes(Action<PreFracturedGeometry> completedCallback)
		{
			GenerateFractureMeshes(Vector3.zero, completedCallback);
		}

		public void GenerateFractureMeshes(Vector3 localPoint, Action<PreFracturedGeometry> completedCallback)
		{
			if (Application.isPlaying)
			{
				Debug.LogWarning("DinoFracture: Creating pre-fractured pieces at runtime.  This can be slow if there a lot of pieces.");
			}
			if (GeneratedPieces != null)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(GeneratedPieces);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(GeneratedPieces);
				}
			}
			FractureDetails details = default(FractureDetails);
			details.NumPieces = NumFracturePieces;
			details.NumIterations = NumIterations;
			details.UVScale = FractureUVScale.Piece;
			details.Asynchronous = !Application.isPlaying;
			details.FractureCenter = localPoint;
			details.FractureRadius = FractureRadius;
			_runningFracture = Fracture(details, hideAfterFracture: false);
			_completionCallback = completedCallback;
			if (Application.isPlaying)
			{
				if (!_runningFracture.IsComplete)
				{
					Debug.LogError("DinoFracture: Prefracture task is not complete");
				}
				OnPreFractureComplete();
			}
		}

		public void StopRunningFracture()
		{
			_runningFracture.StopFracture();
			_runningFracture = null;
			StopFracture();
		}

		private void EditorUpdate()
		{
			if (_runningFracture != null && _runningFracture.IsComplete)
			{
				OnPreFractureComplete();
			}
		}

		private void OnPreFractureComplete()
		{
			GeneratedPieces = _runningFracture.PiecesRoot;
			EntireMeshBounds = _runningFracture.EntireMeshBounds;
			GeneratedPieces.SetActive(value: false);
			_runningFracture = null;
			if (_completionCallback != null)
			{
				_completionCallback(this);
			}
		}

		protected override AsyncFractureResult FractureInternal(Vector3 localPos)
		{
			if (base.gameObject.activeSelf)
			{
				if (GeneratedPieces == null)
				{
					GenerateFractureMeshes(localPos, null);
					EnableFracturePieces();
				}
				else
				{
					EnableFracturePieces();
					OnFractureEventArgs value = new OnFractureEventArgs(this, GeneratedPieces);
					_ignoreOnFractured = true;
					base.gameObject.SendMessage("OnFracture", value, SendMessageOptions.DontRequireReceiver);
					_ignoreOnFractured = false;
					Transform transform = GeneratedPieces.transform;
					for (int i = 0; i < transform.childCount; i++)
					{
						transform.GetChild(i).gameObject.SendMessage("OnFracture", value, SendMessageOptions.DontRequireReceiver);
					}
				}
				base.gameObject.SetActive(value: false);
				AsyncFractureResult asyncFractureResult = new AsyncFractureResult();
				asyncFractureResult.SetResult(GeneratedPieces, EntireMeshBounds);
				return asyncFractureResult;
			}
			AsyncFractureResult asyncFractureResult2 = new AsyncFractureResult();
			asyncFractureResult2.SetResult(null, default(Bounds));
			return asyncFractureResult2;
		}

		private void EnableFracturePieces()
		{
			Transform transform = GeneratedPieces.transform;
			Transform transform2 = base.transform;
			transform.position = transform2.position;
			transform.rotation = transform2.rotation;
			transform.localScale = Vector3.one;
			GeneratedPieces.SetActive(value: true);
		}

		internal override void OnFracture(OnFractureEventArgs args)
		{
			if (!_ignoreOnFractured)
			{
				base.OnFracture(args);
				GeneratedPieces = args.FracturePiecesRootObject;
				GeneratedPieces.SetActive(value: false);
			}
		}
	}
}
