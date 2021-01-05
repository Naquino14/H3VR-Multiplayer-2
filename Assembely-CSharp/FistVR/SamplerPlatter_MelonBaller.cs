using UnityEngine;

namespace FistVR
{
	public class SamplerPlatter_MelonBaller : MonoBehaviour
	{
		public AudioEvent LaunchSound;

		public Transform[] LaunchPoints;

		private int m_currentLaunchPoint;

		public GameObject Payload;

		public float[] LaunchSpeeds;

		public Vector2 LaunchRefires = new Vector2(0.5f, 5.5f);

		public float LaunchRefire = 5.5f;

		private float m_refire = 2f;

		private bool m_isFiring;

		public AudioEvent Boop;

		private void Start()
		{
		}

		private void Update()
		{
			if (m_isFiring)
			{
				if (m_refire > 0f)
				{
					m_refire -= Time.deltaTime;
					return;
				}
				Fire();
				m_refire = LaunchRefire;
			}
		}

		public void SetSpeed(int i)
		{
			switch (i)
			{
			case 4:
				LaunchRefire = Mathf.Lerp(LaunchRefires.x, LaunchRefires.y, 0f);
				break;
			case 3:
				LaunchRefire = Mathf.Lerp(LaunchRefires.x, LaunchRefires.y, 0.25f);
				break;
			case 2:
				LaunchRefire = Mathf.Lerp(LaunchRefires.x, LaunchRefires.y, 0.5f);
				break;
			case 1:
				LaunchRefire = Mathf.Lerp(LaunchRefires.x, LaunchRefires.y, 0.75f);
				break;
			case 0:
				LaunchRefire = Mathf.Lerp(LaunchRefires.x, LaunchRefires.y, 1f);
				break;
			}
			SM.PlayGenericSound(Boop, base.transform.position);
		}

		public void TurnOn()
		{
			m_refire = LaunchRefire;
			m_isFiring = true;
			SM.PlayGenericSound(Boop, base.transform.position);
		}

		public void TurnOff()
		{
			m_isFiring = false;
			SM.PlayGenericSound(Boop, base.transform.position);
		}

		private void Fire()
		{
			Transform transform = LaunchPoints[m_currentLaunchPoint];
			SM.PlayGenericSound(LaunchSound, transform.position);
			GameObject gameObject = Object.Instantiate(Payload, transform.position, Random.rotation);
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			component.velocity = transform.forward * LaunchSpeeds[(int)GM.Options.SimulationOptions.ObjectGravityMode] * Random.Range(0.9f, 1.1f);
			component.velocity += transform.right * Random.Range(-0.5f, 0.5f);
			m_currentLaunchPoint++;
			if (m_currentLaunchPoint >= LaunchPoints.Length)
			{
				m_currentLaunchPoint = 0;
			}
		}
	}
}
