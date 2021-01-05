using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RonchWeapon : MonoBehaviour
	{
		public enum FiringState
		{
			Idle,
			CyclingUp,
			Firing,
			DoneFiring
		}

		public enum ProjectileType
		{
			Projectile,
			Stinger,
			Beam
		}

		public RonchMaster Master;

		[Header("State")]
		public FiringState FireState;

		public float Time_CycleUp = 1f;

		public float Time_Firing = 1f;

		public bool SendsFireCompleteMessage;

		public AudioEvent AudEvent_CycleUp;

		[Header("Mecha")]
		public Transform Muzzle;

		public float Spread;

		public int NumShotsPerSalvo = 1;

		public float RefireRate = 0.2f;

		[Header("Projectile")]
		public GameObject Prefab_Projectile;

		public float FlightVelocityMultiplier = 0.2f;

		public ProjectileType Type;

		[Header("Audio Params")]
		public wwBotWurstGunSoundConfig GunShotProfile;

		private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();

		[Header("MuzzleFX")]
		public bool UsesMuzzleFire;

		public bool DoesFlashOnFire;

		public List<ParticleSystem> PSystemsMuzzle;

		public int MuzzlePAmount;

		private float m_stateTickDown = 1f;

		private float m_refireTickDown = 1f;

		private int m_shotsLeft;

		private Transform targetPos;

		private bool m_canFire = true;

		public void SetTargetPos(Transform t)
		{
			targetPos = t;
		}

		protected void Start()
		{
			if (GunShotProfile != null)
			{
				PrimeDics();
			}
		}

		public void BeginFiringSequence()
		{
			if (m_canFire && FireState == FiringState.Idle)
			{
				FireState = FiringState.CyclingUp;
				m_stateTickDown = Time_CycleUp;
			}
		}

		private void InitiateFiringState()
		{
			FireState = FiringState.Firing;
			m_stateTickDown = Time_Firing;
			m_refireTickDown = RefireRate;
			m_shotsLeft = NumShotsPerSalvo;
		}

		private void InitiateDoneFiringState()
		{
			FireState = FiringState.DoneFiring;
		}

		public void AbortFiringState()
		{
			FireState = FiringState.Idle;
		}

		public void DestroyGun()
		{
			AbortFiringState();
			m_canFire = false;
		}

		public void Update()
		{
			StateUpdate();
		}

		private void StateUpdate()
		{
			if (FireState == FiringState.CyclingUp)
			{
				m_stateTickDown -= Time.deltaTime;
				if (m_stateTickDown <= 0f)
				{
					InitiateFiringState();
				}
			}
			else if (FireState == FiringState.Firing)
			{
				if (m_refireTickDown > 0f)
				{
					m_refireTickDown -= Time.deltaTime;
				}
				else
				{
					Fire();
				}
				m_stateTickDown -= Time.deltaTime;
				if (m_stateTickDown <= 0f)
				{
					InitiateDoneFiringState();
				}
			}
			else if (FireState == FiringState.DoneFiring)
			{
				if (SendsFireCompleteMessage)
				{
					Master.FiringComplete();
				}
				FireState = FiringState.Idle;
			}
		}

		private void Fire()
		{
			m_shotsLeft--;
			m_refireTickDown = RefireRate;
			switch (Type)
			{
			case ProjectileType.Projectile:
			{
				GameObject gameObject2 = Object.Instantiate(Prefab_Projectile, Muzzle.position, Muzzle.rotation);
				gameObject2.transform.Rotate(new Vector3(Random.Range(0f - Spread, Spread), Random.Range(0f - Spread, Spread), 0f));
				gameObject2.GetComponent<BallisticProjectile>().FlightVelocityMultiplier = FlightVelocityMultiplier;
				gameObject2.GetComponent<BallisticProjectile>().Fire(gameObject2.transform.forward, null);
				FVRSoundEnvironment fVRSoundEnvironment = PlayShotEvent(Muzzle.position);
				break;
			}
			case ProjectileType.Stinger:
			{
				GameObject gameObject = Object.Instantiate(Prefab_Projectile, Muzzle.position, Muzzle.rotation);
				gameObject.transform.Rotate(new Vector3(Random.Range(0f - Spread, Spread), Random.Range(0f - Spread, Spread), 0f));
				StingerMissile component = gameObject.GetComponent<StingerMissile>();
				component.SetMotorPower(20f);
				component.SetMaxSpeed(25f);
				component.SetTurnSpeed(Random.Range(2.4f, 3f));
				component.Fire(targetPos.position, 40f);
				break;
			}
			}
			if (UsesMuzzleFire)
			{
				for (int i = 0; i < PSystemsMuzzle.Count; i++)
				{
					PSystemsMuzzle[i].Emit(MuzzlePAmount);
				}
			}
			if (DoesFlashOnFire)
			{
				FXM.InitiateMuzzleFlash(Muzzle.position, Muzzle.forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
			}
			if ((float)m_shotsLeft <= 0f)
			{
				InitiateDoneFiringState();
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

		private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(FVRSoundEnvironment e)
		{
			return m_shotDic[e];
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
