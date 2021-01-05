using UnityEngine;

namespace FistVR
{
	public class BallisticTestingAimer : FVRPhysicalObject
	{
		[Header("Entity Spawner Panel Params")]
		public GameObject Indicator_Locked;

		public GameObject Indicator_Unlocked;

		public AudioEvent[] AudClips_DeviceBeeps;

		[Header("Laser")]
		public Transform SpawnLaserPoint;

		private RaycastHit m_hit;

		public LayerMask LM_SpawnRay;

		public Transform SpawnLaserCylinder;

		private bool LaserOn;

		private int m_projShape;

		private int m_projData;

		private int m_numProjectiles = 1;

		private float m_spread;

		public GameObject[] ProjectilePrefabs;

		public Transform Muzzle;

		[Header("NewAudioImplementation")]
		public FVRFirearmAudioSet AudioClipSet;

		protected SM.AudioSourcePool m_pool_shot;

		protected SM.AudioSourcePool m_pool_tail;

		protected SM.AudioSourcePool m_pool_mechanics;

		protected SM.AudioSourcePool m_pool_handling;

		private float m_refireTick = 0.1f;

		public FVRTailSoundClass TailClass;

		protected override void Awake()
		{
			base.Awake();
			m_pool_shot = SM.CreatePool(3, 3, FVRPooledAudioType.GunShot);
			if (AudioClipSet == null)
			{
				Debug.Log(base.gameObject.name);
			}
			m_pool_tail = SM.CreatePool(AudioClipSet.TailConcurrentLimit, AudioClipSet.TailConcurrentLimit, FVRPooledAudioType.GunTail);
			m_pool_handling = SM.CreatePool(3, 3, FVRPooledAudioType.GunHand);
			m_pool_mechanics = SM.CreatePool(3, 3, FVRPooledAudioType.GunMech);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (m_hand.Input.TouchpadDown && m_hand.Input.TouchpadAxes.magnitude > 0.2f)
			{
				if (Vector2.Angle(m_hand.Input.TouchpadAxes, Vector2.down) < 45f)
				{
					ToggleKinematicLocked();
					if (IsKinematicLocked)
					{
						SM.PlayGenericSound(AudClips_DeviceBeeps[0], base.transform.position);
					}
					else
					{
						SM.PlayGenericSound(AudClips_DeviceBeeps[1], base.transform.position);
					}
					m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
				}
				else if (Vector2.Angle(m_hand.Input.TouchpadAxes, Vector2.right) < 45f)
				{
					ToggleLaser();
					m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
				}
				else if (Vector2.Angle(m_hand.Input.TouchpadAxes, Vector2.left) < 45f)
				{
					ToggleLaser();
					m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
				}
			}
			if (m_hand.Input.TriggerPressed && m_hasTriggeredUpSinceBegin)
			{
				m_refireTick -= Time.deltaTime;
				if (m_refireTick <= 0f)
				{
					m_refireTick = 0.2f;
					Fire();
				}
			}
		}

		public void SetRoundShape(int i)
		{
			m_projShape = i;
		}

		public void SetRoundData(int i)
		{
			m_projData = i;
		}

		public void SetNumProjectiles(int i)
		{
			m_numProjectiles = i;
		}

		public void SetSpread(float f)
		{
			m_spread = f;
		}

		public void Fire()
		{
			PlayAudioGunShot();
			for (int i = 0; i < m_numProjectiles; i++)
			{
				GameObject gameObject = null;
				GameObject gameObject2 = ProjectilePrefabs[m_projData];
				if (gameObject2 != null)
				{
					gameObject = Object.Instantiate(gameObject2, Muzzle.position, Muzzle.rotation);
					gameObject.transform.Rotate(new Vector3(Random.Range(0f - m_spread, m_spread), Random.Range(0f - m_spread, m_spread), 0f));
					BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
					component.ProjType = (BallisticProjectileType)m_projShape;
					component.Fire(component.MuzzleVelocityBase, gameObject.transform.forward, null);
				}
			}
		}

		public void PlayAudioGunShot()
		{
			Vector3 position = base.transform.position;
			PlayAudioEvent(FirearmAudioEventType.Shots_Main);
			if (AudioClipSet.UsesTail_Main)
			{
				AudioEvent tailSet = SM.GetTailSet(TailClass, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
				m_pool_tail.PlayClipPitchOverride(tailSet, position, AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			BoundsLaser();
			if (IsKinematicLocked)
			{
				if (Indicator_Unlocked.activeSelf)
				{
					Indicator_Unlocked.SetActive(value: false);
				}
				if (!Indicator_Locked.activeSelf)
				{
					Indicator_Locked.SetActive(value: true);
				}
			}
			else
			{
				if (!Indicator_Unlocked.activeSelf)
				{
					Indicator_Unlocked.SetActive(value: true);
				}
				if (Indicator_Locked.activeSelf)
				{
					Indicator_Locked.SetActive(value: false);
				}
			}
		}

		private void ToggleLaser()
		{
			if (LaserOn)
			{
				LaserOn = false;
				SM.PlayGenericSound(AudClips_DeviceBeeps[0], base.transform.position);
			}
			else
			{
				LaserOn = true;
				SM.PlayGenericSound(AudClips_DeviceBeeps[1], base.transform.position);
			}
		}

		private void BoundsLaser()
		{
			if (LaserOn)
			{
				if (!SpawnLaserCylinder.gameObject.activeSelf)
				{
					SpawnLaserCylinder.gameObject.SetActive(value: true);
				}
				if (Physics.Raycast(SpawnLaserPoint.position, SpawnLaserPoint.forward, out m_hit, 10f, LM_SpawnRay, QueryTriggerInteraction.Ignore))
				{
					SpawnLaserCylinder.localScale = new Vector3(0.005f, 0.005f, m_hit.distance);
				}
				else
				{
					SpawnLaserCylinder.localScale = new Vector3(0.005f, 0.005f, 0.005f);
				}
			}
			else if (SpawnLaserCylinder.gameObject.activeSelf)
			{
				SpawnLaserCylinder.gameObject.SetActive(value: false);
			}
		}

		public void PlayAudioEvent(FirearmAudioEventType eType)
		{
			Vector3 position = base.transform.position;
			switch (eType)
			{
			case FirearmAudioEventType.Shots_Main:
				m_pool_shot.PlayClip(AudioClipSet.Shots_Main, position);
				break;
			case FirearmAudioEventType.Shots_Suppressed:
				m_pool_shot.PlayClip(AudioClipSet.Shots_Suppressed, position);
				break;
			case FirearmAudioEventType.Shots_LowPressure:
				m_pool_shot.PlayClip(AudioClipSet.Shots_LowPressure, position);
				break;
			case FirearmAudioEventType.BoltRelease:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltRelease, position);
				break;
			case FirearmAudioEventType.BoltSlideBack:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltSlideBack, position);
				break;
			case FirearmAudioEventType.BoltSlideBackHeld:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltSlideBackHeld, position);
				break;
			case FirearmAudioEventType.BoltSlideBackLocked:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltSlideBackLocked, position);
				break;
			case FirearmAudioEventType.BoltSlideForward:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltSlideForward, position);
				break;
			case FirearmAudioEventType.BoltSlideForwardHeld:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltSlideForwardHeld, position);
				break;
			case FirearmAudioEventType.CatchOnSear:
				m_pool_mechanics.PlayClip(AudioClipSet.CatchOnSear, position);
				break;
			case FirearmAudioEventType.ChamberManual:
				m_pool_mechanics.PlayClip(AudioClipSet.ChamberManual, position);
				break;
			case FirearmAudioEventType.HammerHit:
				m_pool_mechanics.PlayClip(AudioClipSet.HammerHit, position);
				break;
			case FirearmAudioEventType.Prefire:
				m_pool_mechanics.PlayClip(AudioClipSet.Prefire, position);
				break;
			case FirearmAudioEventType.HandleGrab:
				m_pool_handling.PlayClip(AudioClipSet.HandleGrab, position);
				break;
			case FirearmAudioEventType.HandleBack:
				m_pool_handling.PlayClip(AudioClipSet.HandleBack, position);
				break;
			case FirearmAudioEventType.HandleForward:
				m_pool_handling.PlayClip(AudioClipSet.HandleForward, position);
				break;
			case FirearmAudioEventType.HandleBackEmpty:
				m_pool_handling.PlayClip(AudioClipSet.HandleBackEmpty, position);
				break;
			case FirearmAudioEventType.HandleForwardEmpty:
				m_pool_handling.PlayClip(AudioClipSet.HandleForwardEmpty, position);
				break;
			case FirearmAudioEventType.HandleUp:
				m_pool_handling.PlayClip(AudioClipSet.HandleUp, position);
				break;
			case FirearmAudioEventType.HandleDown:
				m_pool_handling.PlayClip(AudioClipSet.HandleDown, position);
				break;
			case FirearmAudioEventType.Safety:
				m_pool_handling.PlayClip(AudioClipSet.Safety, position);
				break;
			case FirearmAudioEventType.FireSelector:
				m_pool_handling.PlayClip(AudioClipSet.FireSelector, position);
				break;
			case FirearmAudioEventType.TriggerReset:
				m_pool_handling.PlayClip(AudioClipSet.TriggerReset, position);
				break;
			case FirearmAudioEventType.BreachOpen:
				m_pool_handling.PlayClip(AudioClipSet.BreachOpen, position);
				break;
			case FirearmAudioEventType.BreachClose:
				m_pool_handling.PlayClip(AudioClipSet.BreachClose, position);
				break;
			case FirearmAudioEventType.MagazineIn:
				m_pool_handling.PlayClip(AudioClipSet.MagazineIn, position);
				break;
			case FirearmAudioEventType.MagazineOut:
				m_pool_handling.PlayClip(AudioClipSet.MagazineOut, position);
				break;
			case FirearmAudioEventType.MagazineInsertRound:
				m_pool_handling.PlayClip(AudioClipSet.MagazineInsertRound, position);
				break;
			case FirearmAudioEventType.MagazineEjectRound:
				m_pool_handling.PlayClip(AudioClipSet.MagazineEjectRound, position);
				break;
			case FirearmAudioEventType.TopCoverRelease:
				m_pool_handling.PlayClip(AudioClipSet.TopCoverRelease, position);
				break;
			case FirearmAudioEventType.TopCoverUp:
				m_pool_handling.PlayClip(AudioClipSet.TopCoverUp, position);
				break;
			case FirearmAudioEventType.TopCoverDown:
				m_pool_handling.PlayClip(AudioClipSet.TopCoverDown, position);
				break;
			case FirearmAudioEventType.StockOpen:
				m_pool_handling.PlayClip(AudioClipSet.StockOpen, position);
				break;
			case FirearmAudioEventType.StockClosed:
				m_pool_handling.PlayClip(AudioClipSet.StockClosed, position);
				break;
			case FirearmAudioEventType.BipodOpen:
				m_pool_handling.PlayClip(AudioClipSet.BipodOpen, position);
				break;
			case FirearmAudioEventType.BipodClosed:
				m_pool_handling.PlayClip(AudioClipSet.BipodClosed, position);
				break;
			case FirearmAudioEventType.BeltGrab:
				m_pool_handling.PlayClip(AudioClipSet.BeltGrab, position);
				break;
			case FirearmAudioEventType.BeltRelease:
				m_pool_handling.PlayClip(AudioClipSet.BeltRelease, position);
				break;
			case FirearmAudioEventType.BeltSeat:
				m_pool_handling.PlayClip(AudioClipSet.BeltSeat, position);
				break;
			}
		}
	}
}
