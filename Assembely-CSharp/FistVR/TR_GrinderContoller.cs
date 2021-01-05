using UnityEngine;

namespace FistVR
{
	public class TR_GrinderContoller : MonoBehaviour, IRoomTriggerable
	{
		public GameObject GrinderBladesPrefab;

		public AudioSource GrinderSound;

		private TR_Grinder[] Grinders;

		private float m_startYPos = 10f;

		private float m_endYPos = 0.6f;

		private float m_curYPos = 10f;

		private float m_YSpeed = 1f;

		private float m_startZPos;

		private float m_endZPos;

		private float m_curZPos;

		private float m_timeLeft = 50f;

		private float m_maxTime = 50f;

		private float m_grinderVolume;

		private float m_grinderPitch = 0.4f;

		private bool m_isGrinding;

		private bool m_isLowering;

		private bool m_isSliding;

		private bool m_isRaising;

		private int m_life = 10;

		private int m_Maxlife = 10;

		private RedRoom m_room;

		public GameObject ShatterableMeatPrefab_Metal;

		public GameObject ShatterableMeatPrefab_Meat;

		public Transform MeatSpawnPoint;

		private bool m_isSpawningMeat;

		private Vector3 meatSpawnMin = Vector3.zero;

		private Vector3 meatSpawnMax = Vector3.zero;

		private int m_meatLeftToSpawn = 10;

		private float m_meatSpawnTick = 0.25f;

		public void SetRoom(RedRoom room)
		{
			m_room = room;
		}

		public void Init(int roomTileSize, RedRoom room)
		{
			GM.MGMaster.Narrator.PlayTrapRoomInit();
			GetComponent<AudioSource>().Play();
			m_room = room;
			switch (roomTileSize)
			{
			case 2:
			{
				Grinders = new TR_Grinder[2];
				m_startZPos = 0f - ((float)roomTileSize - 0.5f);
				m_endZPos = (float)roomTileSize - 0.5f;
				m_curZPos = m_startZPos;
				for (int j = 0; j < 2; j++)
				{
					GameObject gameObject2 = Object.Instantiate(GrinderBladesPrefab, new Vector3(0f, 10f, 0f), Quaternion.identity);
					gameObject2.transform.SetParent(base.transform);
					gameObject2.transform.localPosition = new Vector3(-1f + (float)j * 2f, m_startYPos, m_startZPos);
					gameObject2.transform.localEulerAngles = Vector3.zero;
					Grinders[j] = gameObject2.GetComponent<TR_Grinder>();
					Grinders[j].StartSpinning();
					Grinders[j].SetGController(this);
					GrinderSound.transform.localPosition = new Vector3(0f, m_startYPos, m_startZPos);
					meatSpawnMin = new Vector3(-1.5f, 8f, 0f);
					meatSpawnMax = new Vector3(1.5f, 8f, m_endZPos);
					m_meatLeftToSpawn = 7;
					m_life = 6;
					m_Maxlife = 6;
					m_isSpawningMeat = true;
				}
				break;
			}
			case 3:
			{
				Grinders = new TR_Grinder[3];
				m_startZPos = 0f - ((float)roomTileSize - 0.5f);
				m_endZPos = (float)roomTileSize - 0.5f;
				m_curZPos = m_startZPos;
				for (int k = 0; k < 3; k++)
				{
					GameObject gameObject3 = Object.Instantiate(GrinderBladesPrefab, new Vector3(0f, 10f, 0f), Quaternion.identity);
					gameObject3.transform.SetParent(base.transform);
					gameObject3.transform.localPosition = new Vector3(-2f + (float)k * 2f, m_startYPos, m_startZPos);
					gameObject3.transform.localEulerAngles = Vector3.zero;
					Grinders[k] = gameObject3.GetComponent<TR_Grinder>();
					Grinders[k].StartSpinning();
					Grinders[k].SetGController(this);
					GrinderSound.transform.localPosition = new Vector3(0f, m_startYPos, m_startZPos);
					meatSpawnMin = new Vector3(-2.5f, 8f, 1f);
					meatSpawnMax = new Vector3(2.5f, 8f, m_endZPos);
					m_meatLeftToSpawn = 9;
					m_life = 8;
					m_Maxlife = 8;
					m_isSpawningMeat = true;
				}
				break;
			}
			case 4:
			{
				Grinders = new TR_Grinder[4];
				m_startZPos = 0f - ((float)roomTileSize - 0.5f);
				m_endZPos = (float)roomTileSize - 0.5f;
				m_curZPos = m_startZPos;
				for (int i = 0; i < 4; i++)
				{
					GameObject gameObject = Object.Instantiate(GrinderBladesPrefab, new Vector3(0f, 10f, 0f), Quaternion.identity);
					gameObject.transform.SetParent(base.transform);
					gameObject.transform.localPosition = new Vector3(-3f + (float)i * 2f, m_startYPos, m_startZPos);
					gameObject.transform.localEulerAngles = Vector3.zero;
					Grinders[i] = gameObject.GetComponent<TR_Grinder>();
					Grinders[i].StartSpinning();
					Grinders[i].SetGController(this);
					GrinderSound.transform.localPosition = new Vector3(0f, m_startYPos, m_startZPos);
					meatSpawnMin = new Vector3(-3.5f, 8f, 2f);
					meatSpawnMax = new Vector3(3.5f, 8f, m_endZPos);
					m_meatLeftToSpawn = 12;
					m_life = 10;
					m_Maxlife = 10;
					m_isSpawningMeat = true;
				}
				break;
			}
			}
			BeginLowering();
		}

		private void BeginLowering()
		{
			m_isLowering = true;
			GrinderSound.Play();
			m_isGrinding = true;
		}

		public void DamageGrinder()
		{
			if (!m_isGrinding)
			{
				return;
			}
			m_life--;
			if (m_life <= 0)
			{
				m_isGrinding = false;
				for (int i = 0; i < Grinders.Length; i++)
				{
					SpinDown();
					Grinders[i].StopSpinning();
					Invoke("Raise", 3f);
				}
			}
			else if ((float)m_life / (float)m_Maxlife < 0.7f)
			{
				for (int j = 0; j < Grinders.Length; j++)
				{
					Grinders[j].FirePEffects[0].SetActive(value: true);
					Grinders[j].FirePEffects[1].SetActive(value: true);
				}
			}
			else if ((float)m_life / (float)m_Maxlife < 0.4f)
			{
				for (int k = 0; k < Grinders.Length; k++)
				{
					Grinders[k].SmokePEffects[0].SetActive(value: true);
					Grinders[k].SmokePEffects[1].SetActive(value: true);
				}
			}
		}

		private void Raise()
		{
			m_isRaising = true;
			m_room.OpenDoors(playSound: true);
		}

		private void SpinDown()
		{
			m_isSliding = false;
		}

		private void Update()
		{
			if (m_isSpawningMeat && m_meatLeftToSpawn > 0)
			{
				if (m_meatSpawnTick > 0f)
				{
					m_meatSpawnTick -= Time.deltaTime;
				}
				else
				{
					m_meatLeftToSpawn--;
					m_meatSpawnTick = Random.Range(0.75f, 1.5f);
					MeatSpawnPoint.localPosition = new Vector3(Random.Range(meatSpawnMin.x, meatSpawnMax.x), Random.Range(meatSpawnMin.y, meatSpawnMax.y), Random.Range(meatSpawnMin.z, meatSpawnMax.z));
					Object.Instantiate(ShatterableMeatPrefab_Metal, MeatSpawnPoint.position, Random.rotation);
					MeatSpawnPoint.localPosition = new Vector3(Random.Range(meatSpawnMin.x, meatSpawnMax.x), Random.Range(meatSpawnMin.y, meatSpawnMax.y), Random.Range(meatSpawnMin.z, meatSpawnMax.z));
					Object.Instantiate(ShatterableMeatPrefab_Meat, MeatSpawnPoint.position, Random.rotation);
				}
			}
			if (m_isLowering)
			{
				m_curYPos -= Time.deltaTime * m_YSpeed;
				if (m_curYPos < m_endYPos)
				{
					m_curYPos = m_endYPos;
					m_isLowering = false;
					m_isSliding = true;
				}
				for (int i = 0; i < Grinders.Length; i++)
				{
					Grinders[i].transform.localPosition = new Vector3(Grinders[i].transform.localPosition.x, m_curYPos, Grinders[i].transform.localPosition.z);
				}
				GrinderSound.transform.localPosition = new Vector3(0f, m_curYPos, GrinderSound.transform.localPosition.z);
				m_grinderVolume = Mathf.Lerp(m_grinderVolume, 0.4f, Time.deltaTime * 0.3f);
				m_grinderPitch = Mathf.Lerp(m_grinderPitch, 0.7f, Time.deltaTime * 0.3f);
				GrinderSound.volume = m_grinderVolume;
				GrinderSound.pitch = m_grinderPitch;
			}
			else if (m_isSliding)
			{
				m_timeLeft -= Time.deltaTime;
				if (m_timeLeft < 0f)
				{
					m_timeLeft = 0f;
					m_isSliding = false;
				}
				float t = 1f - m_timeLeft / m_maxTime;
				m_curZPos = Mathf.Lerp(m_startZPos, m_endZPos, t);
				for (int j = 0; j < Grinders.Length; j++)
				{
					Grinders[j].transform.localPosition = new Vector3(Grinders[j].transform.localPosition.x, Grinders[j].transform.localPosition.y, m_curZPos);
				}
				GrinderSound.transform.localPosition = new Vector3(0f, GrinderSound.transform.localPosition.y, m_curZPos);
			}
			else if (m_isRaising)
			{
				m_grinderVolume = Mathf.Lerp(m_grinderVolume, 0f, Time.deltaTime * 0.7f);
				m_grinderPitch = Mathf.Lerp(m_grinderPitch, 0f, Time.deltaTime * 0.7f);
				GrinderSound.volume = m_grinderVolume;
				GrinderSound.pitch = m_grinderPitch;
				m_curYPos += Time.deltaTime * m_YSpeed;
				if (m_curYPos > m_startYPos)
				{
					Object.Destroy(base.gameObject);
				}
				for (int k = 0; k < Grinders.Length; k++)
				{
					Grinders[k].transform.localPosition = new Vector3(Grinders[k].transform.localPosition.x, m_curYPos, Grinders[k].transform.localPosition.z);
				}
				GrinderSound.transform.localPosition = new Vector3(0f, m_curYPos, GrinderSound.transform.localPosition.z);
			}
			else
			{
				m_grinderVolume = Mathf.Lerp(m_grinderVolume, 0f, Time.deltaTime * 0.7f);
				m_grinderPitch = Mathf.Lerp(m_grinderPitch, 0f, Time.deltaTime * 0.7f);
				GrinderSound.volume = m_grinderVolume;
				GrinderSound.pitch = m_grinderPitch;
			}
		}
	}
}
