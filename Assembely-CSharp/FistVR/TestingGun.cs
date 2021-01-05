using UnityEngine;

namespace FistVR
{
	public class TestingGun : MonoBehaviour
	{
		public GameObject BulletPrefab;

		public float MuzzleVel;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				GameObject gameObject = Object.Instantiate(BulletPrefab, base.transform.position, base.transform.rotation);
				gameObject.GetComponent<BallisticProjectile>().Fire(base.transform.forward, null);
			}
		}
	}
}
