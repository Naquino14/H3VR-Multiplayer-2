using UnityEngine;

namespace FistVR
{
	public class ThingThrower : MonoBehaviour
	{
		public ThingThrowerScreen TTScreen;

		public FVRObject[] Things;

		private int m_currentThingToThrow;

		private bool m_isThrowing;

		private int m_throwSequencesLeft = 1;

		private int m_numThrowsPerSequence = 3;

		private int m_numThrowsLeft = 3;

		private float m_sequenceBreather = 10f;

		private float m_throwTime = 5f;

		private float m_tickDownToThrow = 15f;

		private int m_numObjectsPerThrow = 1;

		public Transform LaunchPos1;

		public Transform LaunchPos2;

		private float m_AngularRangeX;

		private float m_AngularRangeY = 5f;

		private float m_VelocityBase = 18f;

		private float m_VelocityRange = 1f;

		private float m_timeBetweenObjects = 0.5f;

		public AudioSource LaunchSoundSource;

		public AudioClip LaunchSound;

		private void Awake()
		{
			TTScreen.OBS_TargetToThrow.SetSelectedButton(0);
			TTScreen.OBS_NumberOfThrows.SetSelectedButton(2);
			TTScreen.OBS_NumberOfTargetsPerThrow.SetSelectedButton(0);
			TTScreen.OBS_TimeBetweenThrows.SetSelectedButton(2);
		}

		private void Start()
		{
			for (int i = 0; i < Things.Length; i++)
			{
				Things[i].GetGameObject();
			}
		}

		public void BeginSequences()
		{
			m_tickDownToThrow = 5f;
			m_throwSequencesLeft = 1;
			m_numThrowsLeft = m_numThrowsPerSequence;
			m_isThrowing = true;
			TTScreen.OBS_TargetToThrow.gameObject.SetActive(value: false);
			TTScreen.OBS_NumberOfThrows.gameObject.SetActive(value: false);
			TTScreen.OBS_NumberOfTargetsPerThrow.gameObject.SetActive(value: false);
			TTScreen.OBS_TimeBetweenThrows.gameObject.SetActive(value: false);
			TTScreen.StartButton.SetActive(value: false);
			TTScreen.StopButton.SetActive(value: true);
		}

		public void StopSequences()
		{
			m_isThrowing = false;
			TTScreen.OBS_TargetToThrow.gameObject.SetActive(value: true);
			TTScreen.OBS_NumberOfThrows.gameObject.SetActive(value: true);
			TTScreen.OBS_NumberOfTargetsPerThrow.gameObject.SetActive(value: true);
			TTScreen.OBS_TimeBetweenThrows.gameObject.SetActive(value: true);
			TTScreen.StartButton.SetActive(value: true);
			TTScreen.StopButton.SetActive(value: false);
		}

		public void SetNumThrows(int i)
		{
			m_numThrowsPerSequence = i + 1;
			m_numThrowsLeft = i;
		}

		public void SetTargetToThrow(int i)
		{
			m_currentThingToThrow = i;
		}

		public void SetNumTargetsPerThrow(int i)
		{
			m_numObjectsPerThrow = i + 1;
		}

		public void SetTimeBetweenThrows(int i)
		{
			switch (i)
			{
			case 0:
				m_throwTime = 2f;
				break;
			case 1:
				m_throwTime = 3f;
				break;
			case 2:
				m_throwTime = 5f;
				break;
			case 3:
				m_throwTime = 8f;
				break;
			case 4:
				m_throwTime = 10f;
				break;
			}
		}

		private void Update()
		{
			UpdateThrower();
		}

		private void UpdateThrower()
		{
			if (m_isThrowing)
			{
				if (m_throwSequencesLeft <= 0)
				{
					m_isThrowing = false;
					StopSequences();
				}
				else if (m_numThrowsLeft <= 0)
				{
					m_throwSequencesLeft--;
					m_numThrowsLeft = m_numThrowsPerSequence;
					m_tickDownToThrow = m_sequenceBreather;
				}
				else if (m_tickDownToThrow <= 0f)
				{
					m_tickDownToThrow = m_throwTime;
					m_numThrowsLeft--;
					Throw();
				}
				else
				{
					m_tickDownToThrow -= Time.deltaTime;
				}
			}
		}

		private void Throw()
		{
			for (int i = 0; i < m_numObjectsPerThrow; i++)
			{
				Invoke("ThrowThing", (float)i * m_timeBetweenObjects);
			}
		}

		private void ThrowThing()
		{
			GameObject gameObject = Object.Instantiate(Things[m_currentThingToThrow].GetGameObject(), Vector3.Lerp(LaunchPos1.position, LaunchPos2.position, Random.Range(0f, 1f)), LaunchPos1.rotation);
			gameObject.transform.Rotate(new Vector3(Random.Range(0f - m_AngularRangeX, m_AngularRangeX), Random.Range(0f - m_AngularRangeY, m_AngularRangeY), 0f));
			gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * (m_VelocityBase + Random.Range(0f - m_VelocityRange, m_VelocityRange));
			gameObject.GetComponent<Rigidbody>().angularVelocity = gameObject.transform.up * 10f;
			LaunchSoundSource.pitch = Random.Range(0.97f, 1.03f);
			LaunchSoundSource.PlayOneShot(LaunchSound, 0.4f);
		}
	}
}
