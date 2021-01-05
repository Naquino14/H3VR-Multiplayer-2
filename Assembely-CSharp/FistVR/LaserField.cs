using UnityEngine;

namespace FistVR
{
	public class LaserField : MonoBehaviour
	{
		public GameObject FieldObject;

		private bool m_isActive = true;

		private float m_distCheckTick = 0.25f;

		public AudioSource FieldBuzz;

		private bool m_isBuzzActive;

		public void Start()
		{
			m_distCheckTick = Random.Range(0.1f, 0.25f);
		}

		public void Update()
		{
			if (m_isActive)
			{
				m_distCheckTick -= Time.deltaTime;
				if (m_distCheckTick <= 0f)
				{
					m_distCheckTick = Random.Range(0.1f, 0.25f);
					SoundCheck();
				}
			}
		}

		public void ShutDown()
		{
			if (FieldObject.activeSelf)
			{
				FieldObject.SetActive(value: false);
				m_isActive = true;
			}
		}

		private void SoundCheck()
		{
			float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position);
			if (num < 8f && !m_isBuzzActive)
			{
				m_isBuzzActive = true;
				FieldBuzz.Play();
			}
			else if (num > 10f && m_isBuzzActive)
			{
				m_isBuzzActive = false;
				FieldBuzz.Stop();
			}
		}
	}
}
