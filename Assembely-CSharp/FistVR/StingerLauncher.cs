using UnityEngine;

namespace FistVR
{
	public class StingerLauncher : FVRFireArm
	{
		[Header("Stinger Launcher Config")]
		public StingerLauncherFore Fore;

		public GameObject MissilePrefab;

		private bool m_hasMissile = true;

		public GameObject MissileDisplay;

		private bool m_isTargetttingEngaged;

		private bool m_hasTargetLock;

		private bool m_isCameraUnlocked;

		public Transform TargettingDirection;

		public float MaxAngleToTarget = 3f;

		public LayerMask TargettingLM;

		private AIEntity m_lockingEntity;

		private AIEntity m_targetEntity;

		private float m_lockTime;

		public AudioSource AudSource_TargetSound;

		public AudioClip AudClip_Targetting;

		public AudioClip AudClip_TargetLock;

		public AudioEvent AudEvent_Chirp;

		public Transform Trigger;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (!IsAltHeld && hand.Input.TriggerDown && m_hasTriggeredUpSinceBegin && m_hasTargetLock && m_isCameraUnlocked)
			{
				Fire();
			}
			if (!IsAltHeld && m_hasTriggeredUpSinceBegin)
			{
				float triggerFloat = hand.Input.TriggerFloat;
				SetAnimatedComponent(Trigger, triggerFloat * 20f, InterpStyle.Rotation, Axis.X);
			}
		}

		private void ToggleTargettingEnabled()
		{
			if (!m_isTargetttingEngaged)
			{
				m_isTargetttingEngaged = true;
				return;
			}
			m_isTargetttingEngaged = false;
			m_hasTargetLock = false;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (!m_hasMissile || !base.IsHeld || Magazine == null || !Magazine.HasFuel())
			{
				m_isTargetttingEngaged = false;
				m_hasTargetLock = false;
				m_isCameraUnlocked = false;
				m_targetEntity = null;
				m_lockingEntity = null;
				m_lockTime = 0f;
			}
			else if (base.IsHeld)
			{
				bool flag = false;
				bool flag2 = false;
				if (m_hand.IsInStreamlinedMode)
				{
					if (m_hand.Input.BYButtonDown)
					{
						flag = true;
					}
					if (Fore.IsHeld && Fore.m_hand.Input.BYButtonPressed)
					{
						flag2 = true;
					}
				}
				else
				{
					if (m_hand.Input.TouchpadDown)
					{
						flag = true;
					}
					if (Fore.IsHeld && Fore.m_hand.Input.TouchpadPressed)
					{
						flag2 = true;
					}
				}
				if (flag && m_hasTriggeredUpSinceBegin)
				{
					PlayAudioEvent(FirearmAudioEventType.FireSelector);
					ToggleTargettingEnabled();
				}
				if (m_hasTargetLock && m_targetEntity != null)
				{
					Vector3 from = m_targetEntity.GetPos() - TargettingDirection.position;
					if (Vector3.Angle(from, TargettingDirection.forward) > MaxAngleToTarget)
					{
						m_hasTargetLock = false;
						m_targetEntity = null;
						m_lockingEntity = null;
						m_lockTime = 0f;
					}
				}
				if (!m_hasTargetLock && m_isTargetttingEngaged)
				{
					Collider[] array = Physics.OverlapCapsule(TargettingDirection.position, TargettingDirection.position + TargettingDirection.forward * 2000f, 3f, TargettingLM, QueryTriggerInteraction.Collide);
					float num = 2400f;
					Collider collider = null;
					bool flag3 = false;
					Vector3 position = TargettingDirection.position;
					for (int i = 0; i < array.Length; i++)
					{
						float num2 = Vector3.Distance(array[i].transform.position, position);
						if (num2 > 20f && num2 < num && array[i].GetComponent<AIEntity>() != null)
						{
							flag3 = true;
							collider = array[i];
							num = num2;
						}
					}
					if (!flag3)
					{
						array = Physics.OverlapCapsule(TargettingDirection.position + TargettingDirection.forward * 500f, TargettingDirection.position + TargettingDirection.forward * 2000f, 20f, TargettingLM, QueryTriggerInteraction.Collide);
						num = 2400f;
						collider = null;
						flag3 = false;
						position = TargettingDirection.position;
						for (int j = 0; j < array.Length; j++)
						{
							float num3 = Vector3.Distance(array[j].transform.position, position);
							if (num3 > 20f && num3 < num && array[j].GetComponent<AIEntity>() != null)
							{
								flag3 = true;
								collider = array[j];
								num = num3;
							}
						}
					}
					if (flag3)
					{
						m_lockingEntity = collider.GetComponent<AIEntity>();
					}
				}
				if (m_lockingEntity != null && m_targetEntity == null)
				{
					m_lockTime += Time.deltaTime;
					if (m_lockTime >= 3.5f)
					{
						m_targetEntity = m_lockingEntity;
						m_hasTargetLock = true;
						SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Chirp, AudSource_TargetSound.transform.position);
					}
				}
				if (m_hasTargetLock && flag2)
				{
					m_isCameraUnlocked = true;
					MaxAngleToTarget = 30f;
				}
				else
				{
					m_isCameraUnlocked = false;
					MaxAngleToTarget = 3f;
				}
			}
			if (!base.IsHeld || !m_hasMissile)
			{
				if (AudSource_TargetSound.isPlaying)
				{
					AudSource_TargetSound.Stop();
				}
			}
			else if (m_hasTargetLock)
			{
				AudSource_TargetSound.clip = AudClip_TargetLock;
				if (!AudSource_TargetSound.isPlaying)
				{
					AudSource_TargetSound.Play();
				}
			}
			else if (m_isTargetttingEngaged)
			{
				AudSource_TargetSound.clip = AudClip_Targetting;
				if (!AudSource_TargetSound.isPlaying)
				{
					AudSource_TargetSound.Play();
				}
			}
			else if (AudSource_TargetSound.isPlaying)
			{
				AudSource_TargetSound.Stop();
			}
			if (Magazine != null && Magazine.HasFuel())
			{
				Magazine.DrainFuel(Time.deltaTime);
			}
		}

		public void Fire()
		{
			if (!m_hasMissile)
			{
				return;
			}
			GameObject gameObject = Object.Instantiate(MissilePrefab, GetMuzzle().position, GetMuzzle().rotation);
			StingerMissile component = gameObject.GetComponent<StingerMissile>();
			component.Fire(m_targetEntity);
			FireMuzzleSmoke();
			PlayAudioGunShot(IsHighPressure: true, FVRTailSoundClass.Launcher, FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			if (m_hand != null)
			{
				m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
				if (base.AltGrip != null && base.AltGrip.m_hand != null)
				{
					base.AltGrip.m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
				}
			}
			GM.CurrentSceneSettings.OnShotFired(this);
			GM.CurrentPlayerBody.VisibleEvent(4f);
			m_hasMissile = false;
			MissileDisplay.SetActive(value: false);
			m_isTargetttingEngaged = false;
			m_hasTargetLock = false;
			m_isCameraUnlocked = false;
			m_targetEntity = null;
			m_lockingEntity = null;
			m_lockTime = 0f;
		}
	}
}
