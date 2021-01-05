using UnityEngine;

namespace FistVR
{
	public class wwFinaleDoor : MonoBehaviour
	{
		public Vector3 UpPosition;

		public Vector3 DownPosition;

		private float m_doorLerp;

		public float DoorLerpSpeed = 1f;

		private bool m_isOpening;

		private int m_doorState;

		public GameObject KeyProxy;

		public GameObject KeyDetectProxy;

		private AudioSource Aud;

		public bool TestDoor;

		public int Index;

		public void Awake()
		{
			Aud = GetComponent<AudioSource>();
			if (TestDoor)
			{
				Invoke("OpenDoor", 3f);
			}
			KeyProxy.SetActive(value: false);
			KeyDetectProxy.SetActive(value: true);
		}

		public void OpenDoor()
		{
			if (m_doorState == 0)
			{
				KeyProxy.SetActive(value: true);
				KeyDetectProxy.SetActive(value: false);
				m_isOpening = true;
				Aud.Play();
			}
		}

		public void ConfigureDoorState(int state)
		{
			m_doorState = state;
			switch (state)
			{
			case 0:
				base.transform.localPosition = UpPosition;
				m_doorLerp = 0f;
				m_isOpening = false;
				KeyProxy.SetActive(value: false);
				KeyDetectProxy.SetActive(value: true);
				break;
			case 1:
				base.transform.localPosition = DownPosition;
				m_doorLerp = 1f;
				m_isOpening = false;
				KeyProxy.SetActive(value: true);
				KeyDetectProxy.SetActive(value: false);
				break;
			}
		}

		public void Update()
		{
			if (m_isOpening)
			{
				if (m_doorLerp < 1f)
				{
					m_doorLerp += Time.deltaTime * DoorLerpSpeed;
				}
				else
				{
					m_isOpening = false;
					m_doorLerp = 1f;
					m_doorState = 1;
				}
				base.transform.localPosition = Vector3.Lerp(UpPosition, DownPosition, m_doorLerp);
			}
		}
	}
}
