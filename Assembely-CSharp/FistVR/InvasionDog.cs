using UnityEngine;

namespace FistVR
{
	public class InvasionDog : MonoBehaviour
	{
		public ConfigurableSoldierBotSpawner Spawner;

		public FVRPhysicalObject PO;

		public GameObject Sound;

		private void Update()
		{
			if (PO.IsHeld && Vector3.Distance(GM.CurrentPlayerBody.Head.position + Vector3.up * -0.15f, base.transform.position) < 0.15f)
			{
				BS();
			}
		}

		public void BS()
		{
			GameObject.Find("_AudioMusic").GetComponent<MM_MusicManager>().PlayMusic();
			Object.Instantiate(Sound, base.transform.position, base.transform.rotation);
			Spawner.SpawnGronch();
			Object.Destroy(base.gameObject);
		}
	}
}
