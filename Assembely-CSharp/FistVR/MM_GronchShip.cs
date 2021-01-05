using UnityEngine;

namespace FistVR
{
	public class MM_GronchShip : MonoBehaviour
	{
		public enum GShipState
		{
			Hover,
			MoveToPoint,
			Spinning,
			FiringSequence,
			SpawningSequence,
			FireEverything,
			Dying
		}

		public ConfigurableSoldierBotSpawner BotSpawner;

		public Rigidbody RB;

		public Transform Head;

		public Transform[] NavPoints;

		public Transform[] SpawnPoints;

		public GameObject SpawnEffectPrefab;

		public MM_GronchShip_Gun[] Guns;

		private MM_GronchShip_Gun m_curGun;

		public GShipState State = GShipState.MoveToPoint;

		private int m_currentNavPoint;

		private int m_nextNavPoint;

		private Vector3 m_currentDir;

		private Vector3 m_targetDir;

		private float m_hoverTickDown = 5f;

		private float m_moveToPointLerp;

		private float m_moveSpeed = 1f;

		private float m_spinDownTick = 5f;

		private float m_spawnSequenceTick = 1f;

		private int m_spawnAmountLeft = 5;

		private float m_timeSinceStart;

		public MM_GronchShip_DamagePiece[] DamagePieces;

		[Header("AudioStuff")]
		public AudioSource AUD;

		public AudioClip[] AudClip_Intro;

		public AudioClip[] AudClip_Firing;

		public AudioClip[] AudClip_Dodging;

		public AudioClip[] AudClip_Spawning;

		public AudioClip[] AudClip_MegaAttack;

		public AudioClip[] AudClip_Spinning;

		public AudioClip[] AudClip_Dying;

		private float m_speakingTick = 5f;

		[Header("DeathStuff")]
		public GameObject[] DeathVFXPrefabs;

		public Transform[] DeathVFXPoints;

		public GameObject DeathVFXFinal;

		private float splodeTick = 0.1f;

		private float m_dyingLerp;

		private Vector3 m_startPos;

		private Vector3 m_endPos;

		private bool m_isDeathSequenceFired;

		private void Start()
		{
			SetState(State);
			if (m_speakingTick >= 1f && !AUD.isPlaying)
			{
				AUD.clip = AudClip_Intro[Random.Range(0, AudClip_Intro.Length)];
				AUD.Play();
				m_speakingTick = 0f;
			}
		}

		private void Update()
		{
			m_timeSinceStart += Time.deltaTime;
			DeathCheck();
			StateUpdate();
			if (m_speakingTick <= 15f)
			{
				m_speakingTick += Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.D))
			{
				SetState(GShipState.Dying);
			}
		}

		private void RotateHeadTowardsPlayer()
		{
			Vector3 forward = GM.CurrentPlayerBody.Torso.position - Head.transform.position;
			forward.y = 0f;
			Head.rotation = Quaternion.RotateTowards(Head.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 5f);
			Head.localEulerAngles = new Vector3(0f, Head.localEulerAngles.y, 0f);
		}

		private void DeathCheck()
		{
			bool flag = false;
			int num = 0;
			for (int i = 0; i < Guns.Length; i++)
			{
				if (Guns[i] != null && !Guns[i].IsDestroyed())
				{
					num++;
				}
			}
			int num2 = 0;
			for (int j = 0; j < DamagePieces.Length; j++)
			{
				if (DamagePieces[j] != null && !DamagePieces[j].IsDestroyed())
				{
					num2++;
				}
			}
			if (num2 == 0 || num == 0)
			{
				flag = true;
			}
			if (flag && State != GShipState.Dying)
			{
				SetState(GShipState.Dying);
			}
		}

		private void StateUpdate()
		{
			switch (State)
			{
			case GShipState.Hover:
				StateUpdate_Hover();
				break;
			case GShipState.MoveToPoint:
				StateUpdate_MoveToPoint();
				break;
			case GShipState.Spinning:
				StateUpdate_Spinning();
				break;
			case GShipState.FiringSequence:
				StateUpdate_FiringSequence();
				RotateHeadTowardsPlayer();
				break;
			case GShipState.SpawningSequence:
				StateUpdate_SpawningSequence();
				break;
			case GShipState.FireEverything:
				StateUpdate_FireEverything();
				RotateHeadTowardsPlayer();
				break;
			case GShipState.Dying:
				StateUpdate_Dying();
				break;
			}
		}

		private void StateUpdate_Dying()
		{
			if (!m_isDeathSequenceFired)
			{
				m_isDeathSequenceFired = true;
				BotSpawner.GronchIsDead();
				m_startPos = base.transform.position;
				m_endPos = new Vector3(m_startPos.x, 10f, m_startPos.z);
			}
			if (splodeTick > 0f)
			{
				splodeTick -= Time.deltaTime;
			}
			else
			{
				splodeTick = Random.Range(0.2f, 0.3f);
				Object.Instantiate(DeathVFXPrefabs[Random.Range(0, DeathVFXPrefabs.Length)], DeathVFXPoints[Random.Range(0, DeathVFXPoints.Length)].position, Random.rotation);
			}
			m_dyingLerp += Time.deltaTime * 0.2f;
			Vector3 b = Vector3.Lerp(m_startPos, m_endPos, m_dyingLerp * m_dyingLerp);
			RB.MovePosition(Vector3.Lerp(RB.position, b, Time.deltaTime * 6f));
			Vector3 forward = base.transform.forward;
			forward = Quaternion.AngleAxis(1500f * Time.deltaTime * m_dyingLerp, Vector3.up) * forward;
			RB.MoveRotation(Quaternion.LookRotation(forward, Vector3.up));
			if (m_dyingLerp >= 1f)
			{
				GameObject.Find("_AudioMusic").GetComponent<MM_MusicManager>().FadeOutMusic();
				Object.Instantiate(DeathVFXFinal, base.transform.position, base.transform.rotation);
				Object.Destroy(base.gameObject);
			}
		}

		private void StateUpdate_Hover()
		{
			if (m_hoverTickDown > 0f)
			{
				m_hoverTickDown -= Time.deltaTime;
				Vector3 b = NavPoints[m_currentNavPoint].position + Vector3.up * Mathf.Sin(Time.time * 2f) * 5f;
				RB.MovePosition(Vector3.Lerp(RB.position, b, Time.deltaTime * 3f));
			}
			else
			{
				SetState(GetNextState(GShipState.Hover));
			}
		}

		private void StateUpdate_MoveToPoint()
		{
			if (m_moveToPointLerp >= 1f)
			{
				m_moveToPointLerp = 1f;
				Vector3 b = Vector3.Lerp(NavPoints[m_currentNavPoint].position, NavPoints[m_nextNavPoint].position, m_moveToPointLerp) + Vector3.up * Mathf.Sin(Time.time * 2f) * 2f;
				RB.MovePosition(Vector3.Lerp(RB.position, b, Time.deltaTime * 3f));
				m_currentNavPoint = m_nextNavPoint;
				SetState(GetNextState(GShipState.MoveToPoint));
			}
			else
			{
				m_moveToPointLerp += Time.deltaTime * m_moveSpeed;
				Vector3 b2 = Vector3.Lerp(NavPoints[m_currentNavPoint].position, NavPoints[m_nextNavPoint].position, m_moveToPointLerp) + Vector3.up * Mathf.Sin(Time.time * 2f) * 2f;
				RB.MovePosition(Vector3.Lerp(RB.position, b2, Time.deltaTime * 3f));
				RB.MoveRotation(Quaternion.LookRotation(Vector3.Lerp(m_currentDir, m_targetDir, m_moveToPointLerp), Vector3.up));
			}
		}

		private void StateUpdate_Spinning()
		{
			if (m_spinDownTick > 0f)
			{
				m_spinDownTick -= Time.deltaTime;
				Vector3 forward = base.transform.forward;
				forward = Quaternion.AngleAxis(720f * Time.deltaTime, Vector3.up) * forward;
				RB.MoveRotation(Quaternion.LookRotation(forward, Vector3.up));
				Vector3 b = NavPoints[m_currentNavPoint].position + Vector3.up * Mathf.Sin(Time.time * 10f) * 5f;
				RB.MovePosition(Vector3.Lerp(RB.position, b, Time.deltaTime * 6f));
			}
			else
			{
				SetState(GetNextState(GShipState.Spinning));
			}
		}

		private void StateUpdate_FiringSequence()
		{
			if (m_curGun == null || m_curGun.IsDestroyed() || m_curGun.IsFiringSequenceCompleted())
			{
				SetState(GetNextState(GShipState.FiringSequence));
			}
		}

		private void StateUpdate_FireEverything()
		{
			bool flag = true;
			for (int i = 0; i < Guns.Length; i++)
			{
				if (Guns[i] != null && !Guns[i].IsFiringSequenceCompleted())
				{
					flag = false;
				}
			}
			if (flag)
			{
				SetState(GetNextState(GShipState.FireEverything));
			}
		}

		private void StateUpdate_SpawningSequence()
		{
			m_spawnSequenceTick -= Time.deltaTime;
			if (m_spawnSequenceTick <= 0f)
			{
				SpawnBot();
				m_spawnAmountLeft--;
				m_spawnSequenceTick = Random.Range(0.5f, 1f);
			}
			Vector3 b = NavPoints[m_currentNavPoint].position + Vector3.up * Mathf.Sin(Time.time * 2f) * 5f;
			RB.MovePosition(Vector3.Lerp(RB.position, b, Time.deltaTime * 3f));
			if (m_spawnAmountLeft <= 0)
			{
				SetState(GetNextState(GShipState.SpawningSequence));
			}
		}

		private void SpawnBot()
		{
			SetBotSettingsBasedOnDifficulty();
			BotSpawner.SpawnManualAtPoint(SpawnPoints[m_currentNavPoint]);
		}

		private void SetBotSettingsBasedOnDifficulty()
		{
			if (m_timeSinceStart > 240f)
			{
				BotSpawner.SetSetting_Gun(4);
				BotSpawner.SetSetting_Armor(3);
				BotSpawner.SetSetting_Health(1);
				BotSpawner.SetSetting_Movement(2);
			}
			else if (m_timeSinceStart > 180f)
			{
				BotSpawner.SetSetting_Gun(4);
				BotSpawner.SetSetting_Armor(3);
				BotSpawner.SetSetting_Health(1);
				BotSpawner.SetSetting_Movement(1);
			}
			else if (m_timeSinceStart > 120f)
			{
				BotSpawner.SetSetting_Gun(4);
				BotSpawner.SetSetting_Armor(3);
				BotSpawner.SetSetting_Health(0);
				BotSpawner.SetSetting_Movement(1);
			}
			else if (m_timeSinceStart > 60f)
			{
				BotSpawner.SetSetting_Gun(4);
				BotSpawner.SetSetting_Armor(0);
				BotSpawner.SetSetting_Health(0);
				BotSpawner.SetSetting_Movement(0);
			}
			else if (m_timeSinceStart > 30f)
			{
				BotSpawner.SetSetting_Gun(0);
				BotSpawner.SetSetting_Armor(0);
				BotSpawner.SetSetting_Health(0);
				BotSpawner.SetSetting_Movement(0);
			}
		}

		private void SetState(GShipState newState)
		{
			State = newState;
			AUD.volume = 0.35f;
			switch (newState)
			{
			case GShipState.Hover:
				m_hoverTickDown = Random.Range(5f, 10f);
				break;
			case GShipState.MoveToPoint:
				m_nextNavPoint = Random.Range(0, NavPoints.Length);
				m_moveToPointLerp = 0f;
				m_currentDir = base.transform.forward;
				m_targetDir = Random.onUnitSphere;
				m_targetDir.y = 0f;
				m_targetDir.Normalize();
				m_moveSpeed = Random.Range(0.2f, 0.65f);
				if (m_speakingTick >= 10f && !AUD.isPlaying)
				{
					AUD.clip = AudClip_Dodging[Random.Range(0, AudClip_Dodging.Length)];
					AUD.Play();
					m_speakingTick = 0f;
				}
				break;
			case GShipState.Spinning:
				m_spinDownTick = Random.Range(2.5f, 4f);
				if (m_speakingTick >= 3f && !AUD.isPlaying)
				{
					AUD.clip = AudClip_Spinning[Random.Range(0, AudClip_Spinning.Length)];
					AUD.Play();
					m_speakingTick = 0f;
				}
				break;
			case GShipState.FiringSequence:
				m_curGun = GetBestGunToFire();
				if (m_curGun == null)
				{
					SetState(GShipState.MoveToPoint);
				}
				m_curGun.InitiateFiringSequence();
				if (m_speakingTick >= 6f && !AUD.isPlaying)
				{
					AUD.clip = AudClip_Firing[Random.Range(0, AudClip_Firing.Length)];
					AUD.Play();
					m_speakingTick = 0f;
				}
				break;
			case GShipState.SpawningSequence:
			{
				int min = Mathf.RoundToInt(Mathf.Lerp(1f, 3f, m_timeSinceStart * 0.002f));
				int max = Mathf.RoundToInt(Mathf.Lerp(3f, 6f, m_timeSinceStart * 0.002f));
				m_spawnAmountLeft = Random.Range(min, max);
				m_spawnSequenceTick = Random.Range(0.5f, 1f);
				if (m_speakingTick >= 6f && !AUD.isPlaying)
				{
					AUD.clip = AudClip_Spawning[Random.Range(0, AudClip_Spawning.Length)];
					AUD.Play();
					m_speakingTick = 0f;
				}
				break;
			}
			case GShipState.FireEverything:
				FireGoodGuns();
				if (m_speakingTick >= 2f && !AUD.isPlaying)
				{
					AUD.clip = AudClip_MegaAttack[Random.Range(0, AudClip_MegaAttack.Length)];
					AUD.Play();
					m_speakingTick = 0f;
				}
				break;
			case GShipState.Dying:
				AUD.Stop();
				AUD.clip = AudClip_Dying[Random.Range(0, AudClip_Dying.Length)];
				AUD.Play();
				break;
			}
		}

		private GShipState GetNextState(GShipState curState)
		{
			float num = Random.Range(0f, 1f);
			switch (curState)
			{
			case GShipState.Hover:
				if (num > 0.6f)
				{
					return GShipState.SpawningSequence;
				}
				return GShipState.MoveToPoint;
			case GShipState.MoveToPoint:
				if (num > 0.9f)
				{
					return GShipState.MoveToPoint;
				}
				if (num > 0.4f)
				{
					return GShipState.FiringSequence;
				}
				if (num > 0.1f)
				{
					return GShipState.SpawningSequence;
				}
				return GShipState.Spinning;
			case GShipState.Spinning:
				if (num > 0.2f)
				{
					return GShipState.FireEverything;
				}
				return GShipState.MoveToPoint;
			case GShipState.FiringSequence:
				return GShipState.Hover;
			case GShipState.SpawningSequence:
				return GShipState.MoveToPoint;
			case GShipState.FireEverything:
				return GShipState.Hover;
			default:
				return curState;
			}
		}

		private void FireGoodGuns()
		{
			for (int i = 0; i < Guns.Length; i++)
			{
				if (!(Guns[i] == null) && !Guns[i].IsDestroyed())
				{
					Vector3 to = GM.CurrentPlayerBody.transform.position - base.transform.root.position;
					to.y = 0f;
					if (Vector3.Angle(Guns[i].transform.forward, to) <= 120f)
					{
						Guns[i].InitiateFiringSequence();
					}
				}
			}
		}

		private MM_GronchShip_Gun GetBestGunToFire()
		{
			MM_GronchShip_Gun mM_GronchShip_Gun = null;
			MM_GronchShip_Gun mM_GronchShip_Gun2 = null;
			for (int i = 0; i < Guns.Length; i++)
			{
				if (!(Guns[i] == null))
				{
					if (!Guns[i].IsDestroyed() && mM_GronchShip_Gun == null)
					{
						mM_GronchShip_Gun = Guns[i];
					}
					Vector3 to = GM.CurrentPlayerBody.transform.position - base.transform.root.position;
					to.y = 0f;
					if (Vector3.Angle(Guns[i].transform.forward, to) <= 45f)
					{
						mM_GronchShip_Gun2 = Guns[i];
					}
				}
			}
			if (mM_GronchShip_Gun2 != null)
			{
				return mM_GronchShip_Gun2;
			}
			return mM_GronchShip_Gun;
		}

		public void GunDestroyed()
		{
		}
	}
}
