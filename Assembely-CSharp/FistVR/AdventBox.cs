using UnityEngine;

namespace FistVR
{
	public class AdventBox : MonoBehaviour
	{
		public int Day;

		public GameObject[] Pictures;

		private bool m_isOpen;

		public FVRObject[] ObjsToSpawn;

		public Transform[] PositionToSpawn;

		public Transform Door;

		public Transform StartDoorPos;

		public Transform FinalDoorPos;

		private float m_doorOpenTick;

		private bool m_isDoorOpening;

		public AudioSource BoxAudio;

		public GameObject Fetti;

		public GameObject Blocker;

		public AdventBoxLever Lever;

		public GameObject TextDescription;

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			Pictures[Day - 1].SetActive(value: true);
		}

		public void OpenBox()
		{
			if (m_isOpen)
			{
				return;
			}
			m_isOpen = true;
			m_isDoorOpening = true;
			Blocker.SetActive(value: false);
			Fetti.SetActive(value: true);
			TextDescription.SetActive(value: true);
			for (int i = 0; i < ObjsToSpawn.Length; i++)
			{
				if (ObjsToSpawn[i] != null)
				{
					Object.Instantiate(ObjsToSpawn[i].GetGameObject(), PositionToSpawn[i].position, PositionToSpawn[i].rotation);
				}
			}
			BoxAudio.Play();
			UpdateFlag();
		}

		private void Update()
		{
			if (m_isDoorOpening)
			{
				if (m_doorOpenTick < 1f)
				{
					m_doorOpenTick += Time.deltaTime * 0.1f;
					Door.transform.position = Vector3.Lerp(StartDoorPos.position, FinalDoorPos.position, m_doorOpenTick);
				}
				else
				{
					m_isDoorOpening = false;
				}
			}
		}

		private void UpdateFlag()
		{
		}
	}
}
