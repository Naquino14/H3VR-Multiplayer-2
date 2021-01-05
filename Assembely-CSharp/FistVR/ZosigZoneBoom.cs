using UnityEngine;

namespace FistVR
{
	public class ZosigZoneBoom : MonoBehaviour
	{
		private float m_tick = 0.1f;

		private void Start()
		{
		}

		private void OnTriggerStay(Collider col)
		{
			FVRPlayerHitbox component = col.GetComponent<FVRPlayerHitbox>();
			if (component != null)
			{
				m_tick -= Time.deltaTime;
				if (m_tick <= 0f)
				{
					m_tick = Random.Range(0.05f, 0.2f);
				}
			}
		}
	}
}
