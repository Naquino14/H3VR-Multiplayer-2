using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class EncryptionBotCrystal : MonoBehaviour
	{
		[Serializable]
		public class Crystal
		{
			public GameObject GO_Off;

			public GameObject GO_On;

			public GameObject GO_Phys;

			public GameObject Splode;

			public EncryptionBotCrystalWeakPoint WP;

			public bool Alive = true;

			public bool Active;

			public float Life = 5000f;

			public void Activate()
			{
				if (Alive)
				{
					Active = true;
					GO_On.SetActive(value: true);
					GO_Off.SetActive(value: false);
				}
			}

			public void Deactivate()
			{
				if (Alive)
				{
					Active = false;
					GO_On.SetActive(value: false);
					GO_Off.SetActive(value: true);
				}
			}

			public void Kill()
			{
				if (Alive)
				{
					UnityEngine.Object.Instantiate(Splode, GO_Off.transform.position, UnityEngine.Random.rotation);
					Alive = false;
					GO_Phys.SetActive(value: false);
					GO_On.SetActive(value: false);
					GO_Off.SetActive(value: false);
				}
			}

			public void Damage(float d)
			{
				if (Active && Alive)
				{
					Life -= d;
					if (Life <= 0f)
					{
						Kill();
					}
				}
			}

			public void Regrow()
			{
				if (!Alive)
				{
					Alive = true;
					Active = false;
					GO_On.SetActive(value: false);
					GO_Off.SetActive(value: true);
					GO_Phys.SetActive(value: true);
				}
			}
		}

		public enum BotState
		{
			Deactivated,
			Activating,
			Activated,
			Deactivating,
			Exploding
		}

		public enum FireControlState
		{
			Thinking,
			Pulses,
			BeamUppercut,
			BotSpawn
		}

		public bool IsBoss;

		public AIEntity E;

		public Rigidbody RB;

		public List<Crystal> Crystals;

		public List<GameObject> SpawnOnDestroy;

		public List<Transform> DestroyPoints;

		public float DamThreshold;

		public BotState State;

		private float m_activateTick;

		public float ActivateSpeed = 1f;

		public float DeactivateSpeed = 1f;

		public float CooldownSpeed = 1f;

		private float m_cooldownTick = 1f;

		public float ExplodingSpeed = 2f;

		private float m_explodingTick;

		public float DetonationRange = 10f;

		public float TalkRange = 250f;

		[Header("Targetting")]
		public AITargetPrioritySystem Priority;

		[Header("Audio")]
		public AudioEvent AudEvent_Passive;

		public AudioEvent AudEvent_Activating;

		public AudioEvent AudEvent_Deactivating;

		public AudioEvent AudEvent_Scream;

		public AudioEvent AudEvent_LaserChargeUp;

		public AudioSource AudSource_LaserHit;

		public wwBotWurstGunSoundConfig GunShotProfile;

		private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();

		public ParticleSystem ExplodingParticles;

		public LayerMask LM_GroundCast;

		public Vector2 DesiredHeight = new Vector2(4f, 6f);

		private float m_desiredHeight = 4f;

		private float m_tickDownToSpeak = 1f;

		private bool m_hasPriority;

		private FireControlState m_FCState;

		private int m_numBots;

		public List<GameObject> BotPrefabs;

		private List<GameObject> m_spawnedBots = new List<GameObject>();

		public List<Transform> BotSpawnPositions;

		public List<Transform> BotConnectionBeams;

		public GameObject EnergyShield;

		private float m_TimeTilFCChange = 1f;

		private float m_spawnTickDown = 10f;

		private Crystal PulseCrystal1;

		private Crystal PulseCrystal2;

		private Crystal PulseCrystal3;

		private int howManyMorePulseCrystalShots = 6;

		private int whichPulseCrystalNext;

		private float PulseCrystalRefire = 0.2f;

		public GameObject PulseCrystalProjectilePrefab;

		public LayerMask LM_FireClear;

		public int NumShots = 1;

		public float ProjectileSpread = 0.2f;

		private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(FVRSoundEnvironment e)
		{
			return m_shotDic[e];
		}

		private void Start()
		{
			PrimeDics();
			E.AIEventReceiveEvent += EventReceive;
			m_tickDownToSpeak = UnityEngine.Random.Range(5f, 20f);
			m_desiredHeight = UnityEngine.Random.Range(DesiredHeight.x, DesiredHeight.y);
			if (Priority != null)
			{
				m_hasPriority = true;
				Priority.Init(E, 5, 3f, 1.5f);
			}
			for (int i = 0; i < Crystals.Count; i++)
			{
				Crystals[i].WP.SetMC(Crystals[i]);
			}
			if (IsBoss)
			{
				SpawnBotGroup();
			}
		}

		private void OnDestroy()
		{
			E.AIEventReceiveEvent -= EventReceive;
		}

		public void Regrow()
		{
			Crystals.Shuffle();
			for (int i = 0; i < Crystals.Count; i++)
			{
				if (!Crystals[i].Alive)
				{
					Crystals[i].Regrow();
					break;
				}
			}
		}

		public void EventReceive(AIEvent e)
		{
			if (e.IsEntity && e.Entity.IFFCode != E.IFFCode && e.Type == AIEvent.AIEType.Visual)
			{
				Priority.ProcessEvent(e);
				m_cooldownTick = 1f;
				if (State == BotState.Deactivated)
				{
					SetState(BotState.Activating);
				}
			}
		}

		private int GetNumActiveCrystals()
		{
			int num = 0;
			for (int i = 0; i < Crystals.Count; i++)
			{
				if (Crystals[i].Alive && Crystals[i].Active)
				{
					num++;
				}
			}
			return num;
		}

		private void DeActivateAllCrystals()
		{
			for (int i = 0; i < Crystals.Count; i++)
			{
				Crystals[i].Deactivate();
			}
		}

		private void ActivateAllCrystals()
		{
			for (int i = 0; i < Crystals.Count; i++)
			{
				Crystals[i].Activate();
			}
		}

		public void CrystalHit(Crystal c, float amount)
		{
			if (c.Alive && c.Active)
			{
				float num = Mathf.Max(0f, amount - DamThreshold);
				if (num > 0f)
				{
					c.Damage(amount);
				}
			}
		}

		private void ActivateRandomCrystal()
		{
			Crystals.Shuffle();
			for (int i = 0; i < Crystals.Count; i++)
			{
				if (Crystals[i].Alive && !Crystals[i].Active)
				{
					Crystals[i].Activate();
					break;
				}
			}
		}

		private Crystal GetClosestToPoint(Vector3 p)
		{
			float num = 3000f;
			Crystal result = null;
			for (int i = 0; i < Crystals.Count; i++)
			{
				if (Crystals[i].Alive && !Crystals[i].Active)
				{
					float num2 = Vector3.Distance(Crystals[i].GO_On.transform.position, p);
					if (num2 < num)
					{
						num = num2;
						result = Crystals[i];
					}
				}
			}
			return result;
		}

		private Crystal ActivateClosestInactiveCrystalToTarget()
		{
			Crystals.Shuffle();
			float num = 3000f;
			Crystal crystal = null;
			for (int i = 0; i < Crystals.Count; i++)
			{
				if (Crystals[i].Alive && !Crystals[i].Active)
				{
					float num2 = Vector3.Distance(Crystals[i].GO_On.transform.position, Priority.GetTargetPoint());
					if (num2 < num)
					{
						num = num2;
						crystal = Crystals[i];
					}
				}
			}
			crystal?.Activate();
			return crystal;
		}

		private void BotUpdate()
		{
			if (!IsBoss)
			{
				return;
			}
			if (m_spawnedBots.Count > 0)
			{
				for (int num = m_spawnedBots.Count - 1; num >= 0; num--)
				{
					if (m_spawnedBots[num] == null)
					{
						m_spawnedBots.RemoveAt(num);
					}
				}
			}
			for (int i = 0; i < m_spawnedBots.Count && !(m_spawnedBots[i] == null); i++)
			{
				Crystal closestToPoint = GetClosestToPoint(m_spawnedBots[i].transform.position);
				Vector3 position = base.transform.position;
				if (closestToPoint != null && closestToPoint.GO_On != null)
				{
					position = closestToPoint.GO_On.transform.position;
				}
				Vector3 forward = m_spawnedBots[i].transform.position - position;
				BotConnectionBeams[i].transform.position = position;
				BotConnectionBeams[i].transform.localScale = new Vector3(1f, 1f, forward.magnitude);
				BotConnectionBeams[i].gameObject.SetActive(value: true);
				BotConnectionBeams[i].transform.rotation = Quaternion.LookRotation(forward);
			}
			if (m_spawnedBots.Count < 3 || m_spawnedBots[2] == null)
			{
				BotConnectionBeams[2].gameObject.SetActive(value: false);
			}
			if (m_spawnedBots.Count < 2 || m_spawnedBots[1] == null)
			{
				BotConnectionBeams[1].gameObject.SetActive(value: false);
			}
			if (m_spawnedBots.Count < 1 || m_spawnedBots[0] == null)
			{
				BotConnectionBeams[0].gameObject.SetActive(value: false);
			}
			m_numBots = m_spawnedBots.Count;
			if (m_numBots <= 0 && EnergyShield.activeSelf)
			{
				LowerShield();
			}
		}

		private void SpawnBotGroup()
		{
			BotSpawnPositions.Shuffle();
			bool flag = false;
			for (int i = 0; i < 3; i++)
			{
				if (BotSpawnPositions.Count > 0)
				{
					for (int num = BotSpawnPositions.Count - 1; num >= 0; num--)
					{
						Transform point = BotSpawnPositions[i];
						SpawnBot(point);
						flag = true;
						BotSpawnPositions.RemoveAt(i);
						if (m_spawnedBots.Count >= 3)
						{
							break;
						}
					}
				}
				if (m_spawnedBots.Count >= 3)
				{
					break;
				}
			}
			if (flag)
			{
				RaiseShield();
			}
		}

		private void RaiseShield()
		{
			EnergyShield.SetActive(value: true);
		}

		private void LowerShield()
		{
			EnergyShield.SetActive(value: false);
		}

		private void SpawnBot(Transform point)
		{
			GameObject original = BotPrefabs[UnityEngine.Random.Range(0, BotPrefabs.Count)];
			GameObject item = UnityEngine.Object.Instantiate(original, point.position, point.rotation);
			m_spawnedBots.Add(item);
		}

		private void FireControl()
		{
			if (m_FCState == FireControlState.Thinking)
			{
				m_TimeTilFCChange -= Time.deltaTime;
				if (m_TimeTilFCChange <= 0f)
				{
					float num = UnityEngine.Random.Range(0f, 1f);
					if (IsBoss && m_numBots <= 0 && BotSpawnPositions.Count > 0)
					{
						SetFCState(FireControlState.BotSpawn);
					}
					else if (num >= 0f)
					{
						SetFCState(FireControlState.Pulses);
					}
					else
					{
						SetFCState(FireControlState.BeamUppercut);
					}
				}
			}
			else if (m_FCState == FireControlState.Pulses)
			{
				if (howManyMorePulseCrystalShots <= 0)
				{
					SetFCState(FireControlState.Thinking);
				}
				PulseCrystalRefire -= Time.deltaTime;
				if (PulseCrystalRefire <= 0f)
				{
					if (whichPulseCrystalNext == 0 && PulseCrystal1 != null)
					{
						FirePulseShot(PulseCrystal1);
					}
					if (whichPulseCrystalNext == 1 && PulseCrystal2 != null)
					{
						FirePulseShot(PulseCrystal2);
					}
					if (whichPulseCrystalNext == 2 && PulseCrystal3 != null)
					{
						FirePulseShot(PulseCrystal3);
					}
					howManyMorePulseCrystalShots--;
					whichPulseCrystalNext++;
					if (whichPulseCrystalNext > 2)
					{
						whichPulseCrystalNext = 0;
					}
					PulseCrystalRefire = UnityEngine.Random.Range(0.2f, 0.8f);
					PulseCrystalRefire *= PulseCrystalRefire;
				}
			}
			else if (m_FCState == FireControlState.BotSpawn)
			{
				m_spawnTickDown -= Time.deltaTime;
				if (m_spawnTickDown <= 0f)
				{
					SpawnBotGroup();
					SetFCState(FireControlState.Thinking);
				}
			}
			else if (m_FCState != FireControlState.BeamUppercut)
			{
			}
		}

		private void FirePulseShot(Crystal c)
		{
			if (!c.Alive)
			{
				return;
			}
			Vector3 forward = Priority.GetTargetPoint() - c.GO_Off.transform.position;
			if (!Physics.Raycast(c.GO_On.transform.position, forward.normalized, 4f, LM_FireClear))
			{
				RB.AddForceAtPosition(-forward.normalized * UnityEngine.Random.Range(0.2f, 2f), c.GO_Off.transform.position, ForceMode.VelocityChange);
				FVRSoundEnvironment se = PlayShotEvent(c.GO_Off.transform.position);
				float soundTravelDistanceMultByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(se);
				for (int i = 0; i < NumShots; i++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(PulseCrystalProjectilePrefab, c.GO_Off.transform.position, Quaternion.LookRotation(forward));
					gameObject.transform.Rotate(new Vector3(UnityEngine.Random.Range(0f - ProjectileSpread, ProjectileSpread), UnityEngine.Random.Range(0f - ProjectileSpread, ProjectileSpread), 0f));
					BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
					component.FlightVelocityMultiplier = UnityEngine.Random.Range(0.4f, 0.5f);
					float muzzleVelocityBase = component.MuzzleVelocityBase;
					component.Fire(muzzleVelocityBase, gameObject.transform.forward, null);
					component.SetSource_IFF(E.IFFCode);
				}
				FXM.InitiateMuzzleFlash(c.GO_Off.transform.position, UnityEngine.Random.onUnitSphere, 4f, new Color(1f, 0.1f, 0.1f), 8f);
			}
		}

		private FVRSoundEnvironment PlayShotEvent(Vector3 source)
		{
			float num = Vector3.Distance(source, GM.CurrentPlayerBody.Head.position);
			float delay = num / 343f;
			FVRSoundEnvironment environment = SM.GetReverbEnvironment(base.transform.position).Environment;
			wwBotWurstGunSoundConfig.BotGunShotSet shotSet = GetShotSet(environment);
			if (num < 20f)
			{
				SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.NPCShotNear, shotSet.ShotSet_Near, source, shotSet.ShotSet_Distant.VolumeRange, shotSet.ShotSet_Distant.PitchRange, delay);
			}
			else if (num < 100f)
			{
				float num2 = Mathf.Lerp(0.4f, 0.2f, (num - 20f) / 80f);
				SM.PlayCoreSoundDelayedOverrides(vol: new Vector2(num2 * 0.95f, num2), type: FVRPooledAudioType.NPCShotFarDistant, ClipSet: shotSet.ShotSet_Far, pos: source, pitch: shotSet.ShotSet_Distant.PitchRange, delay: delay);
			}
			else
			{
				SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.NPCShotFarDistant, shotSet.ShotSet_Distant, source, shotSet.ShotSet_Distant.VolumeRange, shotSet.ShotSet_Distant.PitchRange, delay);
			}
			return environment;
		}

		private void SetFCState(FireControlState fcs)
		{
			m_FCState = fcs;
			switch (fcs)
			{
			case FireControlState.Thinking:
				m_TimeTilFCChange = UnityEngine.Random.Range(1.5f, 6f);
				DeActivateAllCrystals();
				break;
			case FireControlState.Pulses:
				PulseCrystalRefire = UnityEngine.Random.Range(0.8f, 2.5f);
				howManyMorePulseCrystalShots = 0;
				PulseCrystal1 = ActivateClosestInactiveCrystalToTarget();
				if (PulseCrystal1 != null)
				{
					howManyMorePulseCrystalShots += UnityEngine.Random.Range(2, 4);
				}
				PulseCrystal2 = ActivateClosestInactiveCrystalToTarget();
				if (PulseCrystal1 != null)
				{
					howManyMorePulseCrystalShots += UnityEngine.Random.Range(1, 5);
				}
				PulseCrystal3 = ActivateClosestInactiveCrystalToTarget();
				if (PulseCrystal1 != null)
				{
					howManyMorePulseCrystalShots += UnityEngine.Random.Range(1, 4);
				}
				ActivateRandomCrystal();
				ActivateRandomCrystal();
				break;
			case FireControlState.BotSpawn:
				m_spawnTickDown = 5f;
				ActivateAllCrystals();
				break;
			}
		}

		private void Update()
		{
			if (State != BotState.Exploding)
			{
				Priority.Compute();
			}
			BotUpdate();
			ParticleSystem.EmissionModule emission = ExplodingParticles.emission;
			int num = 0;
			for (int i = 0; i < Crystals.Count; i++)
			{
				if (Crystals[i].Alive)
				{
					num++;
				}
			}
			if (num <= 0)
			{
				Explode();
			}
			switch (State)
			{
			case BotState.Deactivated:
				m_tickDownToSpeak -= Time.deltaTime;
				if (m_tickDownToSpeak <= 0f)
				{
					m_tickDownToSpeak = UnityEngine.Random.Range(8f, 20f);
					if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 80f)
					{
						FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Passive, base.transform.position);
						fVRPooledAudioSource.FollowThisTransform(base.transform);
					}
					emission.rateOverTimeMultiplier = 0f;
				}
				if (Physics.Raycast(base.transform.position, -Vector3.up, m_desiredHeight, LM_GroundCast))
				{
					RB.AddForce(Vector3.up * 20f, ForceMode.Acceleration);
				}
				break;
			case BotState.Activating:
				m_activateTick += Time.deltaTime * ActivateSpeed;
				if (m_activateTick >= 1f)
				{
					SetState(BotState.Activated);
				}
				emission.rateOverTimeMultiplier = 0f;
				break;
			case BotState.Activated:
				m_cooldownTick -= Time.deltaTime * CooldownSpeed;
				FireControl();
				if (m_cooldownTick <= 0f)
				{
					SetState(BotState.Deactivating);
				}
				emission.rateOverTimeMultiplier = 0f;
				break;
			case BotState.Deactivating:
				m_activateTick -= Time.deltaTime * ActivateSpeed;
				if (m_activateTick <= 0f)
				{
					m_activateTick = 0f;
					SetState(BotState.Deactivated);
				}
				emission.rateOverTimeMultiplier = 0f;
				break;
			case BotState.Exploding:
				emission.rateOverTimeMultiplier = 80f;
				m_explodingTick += Time.deltaTime * ExplodingSpeed;
				if (m_explodingTick >= 1f)
				{
					Shatter();
				}
				break;
			}
		}

		private void SetState(BotState S)
		{
			if (State == BotState.Exploding || State == S)
			{
				return;
			}
			State = S;
			switch (State)
			{
			case BotState.Deactivated:
				m_activateTick = 0f;
				break;
			case BotState.Activating:
				m_activateTick = 0f;
				if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 250f)
				{
					FVRPooledAudioSource fVRPooledAudioSource2 = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Activating, base.transform.position);
					fVRPooledAudioSource2.FollowThisTransform(base.transform);
				}
				break;
			case BotState.Activated:
				m_cooldownTick = 1f;
				m_activateTick = 1f;
				break;
			case BotState.Deactivating:
				m_activateTick = 1f;
				SetFCState(FireControlState.Thinking);
				if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 250f)
				{
					FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Deactivating, base.transform.position);
					fVRPooledAudioSource.FollowThisTransform(base.transform);
				}
				break;
			case BotState.Exploding:
				break;
			}
		}

		public void Explode()
		{
			if (State != BotState.Exploding)
			{
				SetState(BotState.Exploding);
				m_explodingTick = 0f;
				FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Scream, base.transform.position);
				fVRPooledAudioSource.FollowThisTransform(base.transform);
			}
		}

		private void Shatter()
		{
			for (int i = 0; i < SpawnOnDestroy.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(SpawnOnDestroy[i], DestroyPoints[i].position, DestroyPoints[i].rotation);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.AddExplosionForce(UnityEngine.Random.Range(1, 10), base.transform.position + UnityEngine.Random.onUnitSphere, 5f);
				}
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void PrimeDics()
		{
			if (!(GunShotProfile != null))
			{
				return;
			}
			for (int i = 0; i < GunShotProfile.ShotSets.Count; i++)
			{
				for (int j = 0; j < GunShotProfile.ShotSets[i].EnvironmentsUsed.Count; j++)
				{
					m_shotDic.Add(GunShotProfile.ShotSets[i].EnvironmentsUsed[j], GunShotProfile.ShotSets[i]);
				}
			}
		}
	}
}
