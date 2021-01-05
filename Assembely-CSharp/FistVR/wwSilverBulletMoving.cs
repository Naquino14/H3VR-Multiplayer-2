using UnityEngine;

namespace FistVR
{
	public class wwSilverBulletMoving : MonoBehaviour
	{
		public float Speed = 90f;

		private void Start()
		{
		}

		private void Update()
		{
			base.transform.position = base.transform.position + Vector3.up * Speed * Time.deltaTime;
			if (base.transform.position.y > 1000f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
