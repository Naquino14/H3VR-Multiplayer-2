using UnityEngine;

namespace FistVR
{
	public class FVRFirearmMovingProxyRound : MonoBehaviour
	{
		public bool IsFull;

		public bool IsSpent;

		public FVRFireArmRound Round;

		public Transform ProxyRound;

		public MeshFilter ProxyMesh;

		public MeshRenderer ProxyRenderer;

		public void Init(Transform t)
		{
			ProxyRound = base.transform;
			ProxyRound.SetParent(t);
			ProxyRound.localPosition = Vector3.zero;
			ProxyRound.localEulerAngles = Vector3.zero;
			ProxyMesh = base.gameObject.AddComponent<MeshFilter>();
			ProxyRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}

		public void UpdateProxyDisplay()
		{
			if (Round == null)
			{
				ProxyMesh.mesh = null;
				ProxyRenderer.material = null;
				ProxyRenderer.enabled = false;
				return;
			}
			if (IsSpent)
			{
				ProxyMesh.mesh = Round.FiredRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
				ProxyRenderer.material = Round.FiredRenderer.sharedMaterial;
			}
			else
			{
				ProxyMesh.mesh = AM.GetRoundMesh(Round.RoundType, Round.RoundClass);
				ProxyRenderer.material = AM.GetRoundMaterial(Round.RoundType, Round.RoundClass);
			}
			ProxyRenderer.enabled = true;
		}

		public void ClearProxy()
		{
			Round = null;
			IsFull = false;
			IsSpent = true;
			UpdateProxyDisplay();
		}

		public void SetFromPrefabReference(GameObject go)
		{
			Round = go.GetComponent<FVRFireArmRound>();
			IsFull = true;
			IsSpent = false;
			UpdateProxyDisplay();
		}
	}
}
