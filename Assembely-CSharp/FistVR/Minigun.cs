using UnityEngine;

namespace FistVR
{
	public class Minigun : FVRFireArm
	{
		public enum MotorState
		{
			Off,
			SpinningUp,
			Firing,
			SpinningDown,
			Idling
		}

		[Header("Minigun Config")]
		public Transform ForeEnd;

		private float m_foreRot;

		[Header("Bullet Config")]
		public GameObject Projectile;

		private int m_numBullets;

		public int m_maxBullets = 14;

		public bool UsesDisplay = true;

		public int m_firingFrameDelay = 1;

		public int m_tickToFire = 1;

		private bool m_isTriggerEngaged;

		private bool m_isMotorSpinningUp;

		private float m_motorRate;

		private float m_tarMotorRate;

		private float m_timeSinceLastShot = 1f;

		public FVRTailSoundClass TailClass = FVRTailSoundClass.FullPower;

		public MotorState m_curState;

		public MotorState m_lastState;

		[Header("Proxy Bullet Config")]
		public GameObject[] ProxyBullets;

		public MeshFilter[] ProxyMeshFilters;

		public Renderer[] ProxyRenderers;

		public FVRLoadedRound[] LoadedRounds;

		[Header("Audio Config")]
		public AudioSource MinigunMotorAudioSource;

		public AudioClip ClipMotorIdle;

		public AudioClip ClipFiring;

		public AudioClip ClipEmpty;

		public bool UsesEmptyClip;

		public ParticleSystem Sparks;

		public ParticleSystem Eject_Shells;

		public ParticleSystem Eject_Links;

		public bool PlaysShotSound = true;

		public Renderer MinigunFore;

		public bool ChangesColor = true;

		private float m_heat;

		private bool m_hasCycledUp;

		private int DestabilizedShots;

		protected override void Awake()
		{
			base.Awake();
			UpdateDisplay();
			base.RootRigidbody.maxAngularVelocity = 15f;
		}

		public override int GetTutorialState()
		{
			if (Magazine != null && !Magazine.HasARound())
			{
				return 3;
			}
			if (base.AltGrip != null)
			{
				return 2;
			}
			if (Magazine == null)
			{
				return 0;
			}
			return 1;
		}

		private void UpdateBulletResevoir()
		{
			if (Magazine != null && m_numBullets < m_maxBullets)
			{
				LoadRoundFromMag();
			}
		}

		private void LoadRoundFromMag()
		{
			MinigunBox minigunBox = (MinigunBox)Magazine;
			if (minigunBox.HasAmmo())
			{
				minigunBox.RemoveRound();
				LoadedRounds[m_numBullets].LR_Class = minigunBox.RoundClass;
				LoadedRounds[m_numBullets].LR_Mesh = AM.GetRoundMesh(minigunBox.RoundType, minigunBox.RoundClass);
				LoadedRounds[m_numBullets].LR_Material = AM.GetRoundMaterial(minigunBox.RoundType, minigunBox.RoundClass);
				LoadedRounds[m_numBullets].LR_ProjectilePrefab = minigunBox.ProjectilePrefab;
				m_numBullets++;
				UpdateDisplay();
			}
		}

		private void UpdateDisplay()
		{
			if (!UsesDisplay)
			{
				return;
			}
			for (int i = 0; i < ProxyBullets.Length; i++)
			{
				if (m_numBullets > i)
				{
					if (!ProxyBullets[i].activeSelf)
					{
						ProxyBullets[i].SetActive(value: true);
					}
					ProxyMeshFilters[i].mesh = LoadedRounds[i].LR_Mesh;
					ProxyRenderers[i].material = LoadedRounds[i].LR_Material;
				}
				else if (ProxyBullets[i].activeSelf)
				{
					ProxyBullets[i].SetActive(value: false);
				}
			}
		}

		private void ConsumeRound()
		{
			if (m_numBullets > 0)
			{
				for (int i = 0; i < m_numBullets - 1; i++)
				{
					LoadedRounds[i].LR_Class = LoadedRounds[i + 1].LR_Class;
					LoadedRounds[i].LR_Mesh = LoadedRounds[i + 1].LR_Mesh;
					LoadedRounds[i].LR_Material = LoadedRounds[i + 1].LR_Material;
					LoadedRounds[i].LR_ProjectilePrefab = LoadedRounds[i + 1].LR_ProjectilePrefab;
				}
				m_numBullets--;
			}
			UpdateDisplay();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_timeSinceLastShot < 1f)
			{
				m_timeSinceLastShot += Time.deltaTime;
			}
			m_heat -= Time.deltaTime * 0.01f;
			m_heat = Mathf.Clamp(m_heat, 0f, 1f);
			if (ChangesColor)
			{
				MinigunFore.material.SetFloat("_EmissionWeight", m_heat * m_heat);
			}
			UpdateBulletResevoir();
			if (!base.IsHeld)
			{
				m_isTriggerEngaged = false;
				m_tarMotorRate = 0f;
			}
			else if (m_isTriggerEngaged)
			{
				m_tarMotorRate = m_hand.Input.TriggerFloat;
			}
			else if (m_hasCycledUp)
			{
				m_tarMotorRate = 0f;
			}
			if (m_motorRate <= 0.01f)
			{
				m_hasCycledUp = false;
			}
			if (m_motorRate > 0.85f)
			{
				m_hasCycledUp = true;
			}
			if (m_tarMotorRate > m_motorRate)
			{
				m_motorRate = Mathf.MoveTowards(m_motorRate, m_tarMotorRate, Time.deltaTime);
			}
			else
			{
				m_motorRate = Mathf.MoveTowards(m_motorRate, m_tarMotorRate, Time.deltaTime * 0.6f);
			}
			m_motorRate = Mathf.Clamp(m_motorRate, 0f, 1f);
			m_foreRot += Time.deltaTime * 1800f * m_motorRate;
			m_foreRot = Mathf.Repeat(m_foreRot, 360f);
			ForeEnd.localEulerAngles = new Vector3(0f, 0f, m_foreRot);
			if (m_motorRate <= 0f)
			{
				m_curState = MotorState.Off;
			}
			else if (m_motorRate >= 0.9f)
			{
				if (m_numBullets > 0)
				{
					m_curState = MotorState.Firing;
				}
				else
				{
					m_curState = MotorState.Idling;
				}
			}
			else if (m_motorRate > 0.3f)
			{
				if (m_isTriggerEngaged)
				{
					m_curState = MotorState.Idling;
				}
				else
				{
					m_curState = MotorState.SpinningDown;
				}
			}
			else if (m_isTriggerEngaged)
			{
				m_curState = MotorState.SpinningUp;
			}
			else
			{
				m_curState = MotorState.SpinningDown;
			}
			if (m_curState == MotorState.SpinningUp && m_lastState != MotorState.SpinningUp)
			{
				PlayAudioEvent(FirearmAudioEventType.HandleUp);
			}
			if (m_curState == MotorState.SpinningDown && m_lastState != MotorState.SpinningDown)
			{
				PlayAudioEvent(FirearmAudioEventType.HandleDown);
			}
			if (m_curState == MotorState.Firing && MinigunMotorAudioSource.clip != ClipFiring)
			{
				MinigunMotorAudioSource.clip = ClipFiring;
			}
			if (m_curState == MotorState.Idling && MinigunMotorAudioSource.clip != ClipMotorIdle)
			{
				MinigunMotorAudioSource.clip = ClipMotorIdle;
			}
			if ((m_curState == MotorState.Firing || m_curState == MotorState.Idling) && !MinigunMotorAudioSource.isPlaying)
			{
				MinigunMotorAudioSource.Play();
			}
			if (m_curState != MotorState.Firing && m_curState != MotorState.Idling && MinigunMotorAudioSource.isPlaying)
			{
				MinigunMotorAudioSource.Stop();
			}
			if (m_curState == MotorState.Firing)
			{
				if (m_tickToFire <= 0)
				{
					m_tickToFire = m_firingFrameDelay;
					Fire();
				}
				else
				{
					m_tickToFire--;
				}
			}
			m_lastState = m_curState;
		}

		private void Fire()
		{
			if (m_numBullets <= 0)
			{
				return;
			}
			if (m_hand != null)
			{
				m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
			}
			if (base.AltGrip != null)
			{
				base.AltGrip.m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
			}
			FireMuzzleSmoke();
			if (GM.CurrentSceneSettings.IsSceneLowLight)
			{
				Sparks.Emit(8);
			}
			else
			{
				Sparks.Emit(2);
			}
			Sparks.transform.position = GetMuzzle().position;
			Eject_Shells.Emit(1);
			Eject_Links.Emit(1);
			m_heat += 0.001f;
			Vector3 position = GetMuzzle().position;
			GameObject gameObject = Object.Instantiate(LoadedRounds[0].LR_ProjectilePrefab, position, GetMuzzle().rotation);
			BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
			component.Fire(gameObject.transform.forward, this);
			if (PlaysShotSound)
			{
				if (IsSuppressed())
				{
					m_pool_shot.PlayClip(AudioClipSet.Shots_Suppressed, MuzzlePos.position);
					m_pool_tail.PlayClipPitchOverride(SM.GetTailSet(FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment()), position, AudioClipSet.TailPitchMod_Main);
				}
				else
				{
					m_pool_shot.PlayClip(AudioClipSet.Shots_Main, MuzzlePos.position);
					m_pool_tail.PlayClipPitchOverride(SM.GetTailSet(TailClass, GM.CurrentPlayerBody.GetCurrentSoundEnvironment()), position, AudioClipSet.TailPitchMod_Main);
				}
			}
			ConsumeRound();
			if (base.IsHeld && base.AltGrip == null)
			{
				DestabilizedShots++;
				if (DestabilizedShots > 5)
				{
					if (m_hand != null)
					{
						m_hand.EndInteractionIfHeld(this);
						ForceBreakInteraction();
					}
					base.RootRigidbody.AddForceAtPosition((-base.transform.forward + base.transform.up + Random.onUnitSphere * 0.25f) * 30f, MuzzlePos.position, ForceMode.Impulse);
					base.RootRigidbody.AddRelativeTorque(Vector3.right * 20f, ForceMode.Impulse);
					DestabilizedShots = 0;
				}
				m_curState = MotorState.SpinningDown;
			}
			else
			{
				DestabilizedShots = 0;
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			m_isTriggerEngaged = false;
			if (!IsAltHeld && m_hand.Input.TriggerPressed && m_hasTriggeredUpSinceBegin)
			{
				m_isTriggerEngaged = true;
			}
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (m_curState == MotorState.SpinningUp)
			{
				base.RootRigidbody.angularVelocity += base.transform.forward * 2f * m_motorRate;
			}
			if (m_curState == MotorState.Firing)
			{
				base.RootRigidbody.velocity += Random.onUnitSphere * 0.4f;
				base.RootRigidbody.angularVelocity += Random.onUnitSphere * 1.4f;
			}
		}
	}
}
