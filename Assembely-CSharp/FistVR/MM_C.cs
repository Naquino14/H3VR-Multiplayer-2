using UnityEngine;

namespace FistVR
{
	public class MM_C : MonoBehaviour
	{
		private float checktick = 10f;

		public GameObject prefab;

		private bool c;

		public AudioSource A;

		private float tickdown = 25f;

		private float mTick;

		private void Update()
		{
			if (checktick > 0f)
			{
				checktick -= Time.deltaTime;
			}
			if (checktick <= 0f)
			{
				checktick = Random.Range(5f, 10f);
				Check();
			}
			if (c)
			{
				if (tickdown > 0f)
				{
					tickdown -= Time.deltaTime;
					return;
				}
				if (mTick > 0f)
				{
					mTick -= Time.deltaTime;
					return;
				}
				mTick = Random.Range(1f, 3f);
				STime();
			}
		}

		private void STime()
		{
			Vector3 position = GM.CurrentPlayerBody.transform.position + Vector3.up * 80f;
			position += Random.onUnitSphere * 2f;
			GameObject gameObject = Object.Instantiate(prefab, position, Random.rotation);
			BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
			component.Fire(60f, -Vector3.up, null);
		}

		private void Check()
		{
			if (c)
			{
				return;
			}
			for (int i = 15; i < GM.MMFlags.MMMTCs.Length; i++)
			{
				int num = 1000;
				if (GM.MMFlags.MMMTCs[i] > num)
				{
					c = true;
					if (!A.isPlaying)
					{
						A.Play();
					}
				}
			}
		}
	}
}
