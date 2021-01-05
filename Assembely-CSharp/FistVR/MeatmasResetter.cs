using UnityEngine;

namespace FistVR
{
	public class MeatmasResetter : MonoBehaviour
	{
		public FVRPhysicalObject PO;

		private bool isLoading;

		private void Start()
		{
		}

		private void Update()
		{
			if (PO.IsHeld && Vector3.Distance(GM.CurrentPlayerBody.Head.position, base.transform.position) < 0.15f)
			{
				ResetMeatmas();
			}
		}

		private void ResetMeatmas()
		{
			if (!isLoading)
			{
				GetComponent<AudioSource>().Play();
				isLoading = true;
				SteamVR_LoadLevel.Begin("Xmas");
			}
		}
	}
}
