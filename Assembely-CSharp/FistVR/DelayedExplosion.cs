using UnityEngine;

namespace FistVR
{
	public class DelayedExplosion : MonoBehaviour
	{
		public float FuseTime = 1f;

		private bool HasSploded;

		public GameObject VFXPrefab;

		public GameObject ForcePrefab;

		private void Awake()
		{
			FuseTime *= Random.Range(0.6f, 1.2f);
		}

		private void Update()
		{
			if (!HasSploded)
			{
				FuseTime -= Time.deltaTime;
				if (FuseTime <= 0f)
				{
					HasSploded = true;
					Object.Instantiate(VFXPrefab, base.transform.position, Quaternion.identity);
					Object.Instantiate(ForcePrefab, base.transform.position, Quaternion.identity);
					Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
