using UnityEngine;

namespace FistVR
{
	public class SceneLoader : MonoBehaviour
	{
		public FVRPhysicalObject PO;

		private bool isLoading;

		public string LevelName = "MeatGrinder";

		private void Awake()
		{
		}

		private void Update()
		{
			if (PO.IsHeld && Vector3.Distance(GM.CurrentPlayerBody.Head.position - GM.CurrentPlayerBody.Head.up * 0.15f, base.transform.position) < 0.15f)
			{
				LoadMG();
			}
		}

		private void LoadMG()
		{
			if (!isLoading)
			{
				GetComponent<AudioSource>().Play();
				isLoading = true;
				SteamVR_LoadLevel.Begin(LevelName);
			}
		}
	}
}
