using UnityEngine;

namespace FistVR
{
	public class TR_SpikeCeilingController : MonoBehaviour, IRoomTriggerable
	{
		public TR_SpikeCeilingBase[] Bases;

		private TR_SpikeCeilingPlate[] Plates;

		public GameObject SpikeCeilingPlatePrefab;

		public AlloyAreaLight MyLight;

		private float curIntensity;

		private float tarIntensity;

		private bool m_isTriggered;

		private bool m_isDescending;

		private bool m_isRetracting;

		private float m_trapHeight = 11f;

		private RedRoom m_room;

		public AudioSource CeilingAudio;

		public AudioClip RetractClip;

		public AudioClip ExpandClip;

		public AudioSource RumblingAudio;

		private float m_tickTilToggle = 1f;

		private bool m_isRetracted;

		public void SetRoom(RedRoom room)
		{
			m_room = room;
		}

		public void Init(int roomTileSize, RedRoom room)
		{
			GM.MGMaster.Narrator.PlayTrapRoomInit();
			GetComponent<AudioSource>().Play();
			m_room = room;
			m_isTriggered = true;
			MyLight.gameObject.SetActive(value: true);
			tarIntensity = 0.45f;
			for (int i = 0; i < Bases.Length; i++)
			{
				Bases[i].gameObject.SetActive(value: true);
				Bases[i].transform.position = new Vector3(Bases[i].transform.position.x, 0f, Bases[i].transform.position.z);
			}
			int num = 0;
			switch (roomTileSize)
			{
			case 2:
			{
				Plates = new TR_SpikeCeilingPlate[4];
				for (int l = 0; l < 2; l++)
				{
					for (int m = 0; m < 2; m++)
					{
						GameObject gameObject2 = Object.Instantiate(SpikeCeilingPlatePrefab, Vector3.zero, Quaternion.identity);
						gameObject2.transform.SetParent(base.transform);
						gameObject2.transform.localPosition = new Vector3(-1f + (float)l * 2f, 11f, -1f + (float)m * 2f);
						Plates[num] = gameObject2.GetComponent<TR_SpikeCeilingPlate>();
						num++;
					}
				}
				break;
			}
			case 3:
			{
				Plates = new TR_SpikeCeilingPlate[9];
				for (int n = 0; n < 3; n++)
				{
					for (int num2 = 0; num2 < 3; num2++)
					{
						GameObject gameObject3 = Object.Instantiate(SpikeCeilingPlatePrefab, Vector3.zero, Quaternion.identity);
						gameObject3.transform.SetParent(base.transform);
						gameObject3.transform.localPosition = new Vector3(-2f + (float)n * 2f, 11f, -2f + (float)num2 * 2f);
						Plates[num] = gameObject3.GetComponent<TR_SpikeCeilingPlate>();
						num++;
					}
				}
				break;
			}
			case 4:
			{
				Plates = new TR_SpikeCeilingPlate[16];
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 4; k++)
					{
						GameObject gameObject = Object.Instantiate(SpikeCeilingPlatePrefab, Vector3.zero, Quaternion.identity);
						gameObject.transform.SetParent(base.transform);
						gameObject.transform.localPosition = new Vector3(-3f + (float)j * 2f, 11f, -3f + (float)k * 2f);
						Plates[num] = gameObject.GetComponent<TR_SpikeCeilingPlate>();
						num++;
					}
				}
				break;
			}
			}
			m_isDescending = true;
			RumblingAudio.Play();
		}

		private void StopTrap()
		{
			m_isDescending = false;
			tarIntensity = 0f;
			m_isRetracting = true;
			m_room.OpenDoors(playSound: true);
			Invoke("KillTrap", 15f);
		}

		private void KillTrap()
		{
			Object.Destroy(base.gameObject);
		}

		private void Descend()
		{
			m_trapHeight -= Time.deltaTime * 0.17f;
			CeilingAudio.transform.localPosition = new Vector3(0f, m_trapHeight, 0f);
			int num = 0;
			for (int i = 0; i < Bases.Length; i++)
			{
				if (Bases[i].CurHeight >= m_trapHeight)
				{
					num++;
				}
			}
			if (num >= 3 || m_trapHeight <= 0.8f)
			{
				StopTrap();
				return;
			}
			for (int j = 0; j < Bases.Length; j++)
			{
				Bases[j].LowerTo(m_trapHeight);
			}
			for (int k = 0; k < Plates.Length; k++)
			{
				Plates[k].transform.localPosition = new Vector3(Plates[k].transform.localPosition.x, m_trapHeight, Plates[k].transform.localPosition.z);
			}
			if (m_tickTilToggle <= 0f)
			{
				m_tickTilToggle = 3f;
				ToggleSpikes();
			}
			else
			{
				m_tickTilToggle -= Time.deltaTime;
			}
		}

		private void ToggleSpikes()
		{
			m_isRetracted = !m_isRetracted;
			if (m_isRetracted)
			{
				for (int i = 0; i < Plates.Length; i++)
				{
					Plates[i].Retract();
					CeilingAudio.PlayOneShot(RetractClip, 0.15f);
				}
			}
			else
			{
				for (int j = 0; j < Plates.Length; j++)
				{
					Plates[j].Expand();
					CeilingAudio.PlayOneShot(ExpandClip, 0.15f);
				}
			}
		}

		public void Update()
		{
			if (m_isTriggered)
			{
				if (curIntensity < tarIntensity)
				{
					curIntensity += Time.deltaTime * 0.3f;
					MyLight.Intensity = curIntensity;
					RumblingAudio.volume = curIntensity * 1.5f;
				}
				if (m_isDescending)
				{
					Descend();
				}
			}
			if (m_isRetracting)
			{
				base.transform.position += Vector3.up * Time.deltaTime;
				if (curIntensity > tarIntensity)
				{
					curIntensity -= Time.deltaTime * 0.2f;
					MyLight.Intensity = curIntensity;
					RumblingAudio.volume = curIntensity * 1.5f;
				}
				else
				{
					MyLight.gameObject.SetActive(value: false);
					RumblingAudio.Stop();
				}
			}
		}
	}
}
