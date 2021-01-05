using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FlintlockWeapon : FVRFireArm
	{
		public enum HState
		{
			Uncocked,
			Halfcock,
			Fullcock
		}

		public enum FlintState
		{
			New,
			Used,
			Worn,
			Broken
		}

		[Header("Flintlock")]
		public List<FlintlockFlashPan> FlashPans;

		private int m_curFlashpan;

		public FlintlockPseudoRamRod RamRod;

		[Header("Trigger")]
		public Transform Trigger;

		public Vector2 TriggerRots = new Vector2(0f, 5f);

		private float m_triggerFloat;

		private float m_lastTriggerFloat;

		[Header("Hammer")]
		public Transform Hammer;

		public HState HammerState;

		public Vector3 HammerRots = new Vector3(0f, 20f, 45f);

		private float m_curHammerRot;

		private float m_tarHammerRot;

		[Header("Flint")]
		public FlintState FState;

		private Vector3 m_flintUses = Vector3.one;

		private bool m_hasFlint = true;

		public MeshFilter FlintMesh;

		public MeshRenderer FlintRenderer;

		public List<Mesh> FlintMeshes;

		public FlintlockFlintScrew FlintlockScrew;

		public FlintlockFlintHolder FlintlockHolder;

		public ParticleSystem Sparks;

		[Header("Audio")]
		public AudioEvent AudEvent_HammerCock;

		public AudioEvent AudEvent_HammerHalfCock;

		public AudioEvent AudEvent_HammerHit_Clean;

		public AudioEvent AudEvent_HammerHit_Dull;

		public AudioEvent AudEvent_Spark;

		public AudioEvent AudEvent_FlintBreak;

		public AudioEvent AudEvent_FlintHolderScrew;

		public AudioEvent AudEvent_FlintHolderUnscrew;

		public AudioEvent AudEvent_FlintRemove;

		public AudioEvent AudEvent_FlintReplace;

		[Header("Destruction")]
		public List<GameObject> DisableOnDestroy;

		public List<GameObject> EnableOnDestroy;

		private bool m_isDestroyed;

		public List<GameObject> SpawnOnDestroy;

		public Transform SpawnOnDestroyPoint;

		public GameObject RamRodProj;

		private float FireRefire = 0.2f;

		public bool HasFlint()
		{
			return m_hasFlint;
		}

		protected override void Awake()
		{
			base.Awake();
			for (int i = 0; i < FlashPans.Count; i++)
			{
				FlashPans[i].SetWeapon(this);
			}
			m_flintUses = new Vector3(Random.Range(8, 15), Random.Range(5, 9), Random.Range(4, 8));
		}

		public void Blowup()
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				for (int i = 0; i < DisableOnDestroy.Count; i++)
				{
					DisableOnDestroy[i].SetActive(value: false);
				}
				for (int j = 0; j < EnableOnDestroy.Count; j++)
				{
					EnableOnDestroy[j].SetActive(value: true);
				}
				for (int k = 0; k < SpawnOnDestroy.Count; k++)
				{
					Object.Instantiate(SpawnOnDestroy[k], SpawnOnDestroyPoint.position, SpawnOnDestroyPoint.rotation);
				}
			}
		}

		public void AddFlint(Vector3 uses)
		{
			m_hasFlint = true;
			m_flintUses = uses;
			FlintRenderer.enabled = true;
			if (m_flintUses.x > 0f)
			{
				SetFlintState(FlintState.New);
			}
			else if (m_flintUses.y > 0f)
			{
				SetFlintState(FlintState.Used);
			}
			else if (m_flintUses.z > 0f)
			{
				SetFlintState(FlintState.Worn);
			}
			else
			{
				SetFlintState(FlintState.Broken);
			}
		}

		public Vector3 RemoveFlint()
		{
			m_hasFlint = false;
			FlintRenderer.enabled = false;
			return m_flintUses;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			if (m_hasTriggeredUpSinceBegin && !IsAltHeld)
			{
				m_triggerFloat = hand.Input.TriggerFloat;
			}
			else
			{
				m_triggerFloat = 0f;
			}
			if (m_hasTriggeredUpSinceBegin && !m_isDestroyed)
			{
				if (hand.IsInStreamlinedMode)
				{
					if (hand.Input.BYButtonDown)
					{
						MoveToHalfCock();
					}
					if (hand.Input.AXButtonDown)
					{
						MoveToFullCock();
					}
				}
				else if (hand.Input.TouchpadDown)
				{
					if (hand.Input.TouchpadWestPressed || hand.Input.TouchpadEastPressed)
					{
						MoveToHalfCock();
					}
					else if (hand.Input.TouchpadSouthPressed)
					{
						MoveToFullCock();
					}
				}
			}
			base.UpdateInteraction(hand);
		}

		private bool HitWithFlint()
		{
			if (!m_hasFlint)
			{
				return false;
			}
			switch (FState)
			{
			case FlintState.New:
				m_flintUses.x -= 1f;
				if (m_flintUses.x <= 0f)
				{
					SetFlintState(FlintState.Used);
					PlayAudioAsHandling(AudEvent_FlintBreak, FlintRenderer.transform.position);
				}
				return true;
			case FlintState.Used:
				m_flintUses.y -= 1f;
				if (m_flintUses.y <= 0f)
				{
					SetFlintState(FlintState.Worn);
					PlayAudioAsHandling(AudEvent_FlintBreak, FlintRenderer.transform.position);
				}
				return true;
			case FlintState.Worn:
				m_flintUses.z -= 1f;
				if (m_flintUses.z <= 0f)
				{
					SetFlintState(FlintState.Broken);
					PlayAudioAsHandling(AudEvent_FlintBreak, FlintRenderer.transform.position);
				}
				return true;
			case FlintState.Broken:
				return false;
			default:
				return false;
			}
		}

		private void SetFlintState(FlintState f)
		{
			FState = f;
			FlintMesh.mesh = FlintMeshes[(int)f];
		}

		public void Fire(float recoilMult = 1f)
		{
			if (FireRefire < 0.1f)
			{
				return;
			}
			FireRefire = 0f;
			if (m_hand != null)
			{
				m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
				if (base.AltGrip != null && base.AltGrip.m_hand != null)
				{
					base.AltGrip.m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
				}
			}
			GM.CurrentSceneSettings.OnShotFired(this);
			if (IsSuppressed())
			{
				GM.CurrentPlayerBody.VisibleEvent(0.1f);
			}
			else
			{
				GM.CurrentPlayerBody.VisibleEvent(2f);
			}
			Recoil(IsTwoHandStabilized(), IsForegripStabilized(), IsShoulderStabilized(), null, recoilMult);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (FireRefire < 0.2f)
			{
				FireRefire += Time.deltaTime;
			}
			if (m_triggerFloat != m_lastTriggerFloat)
			{
				Trigger.localEulerAngles = new Vector3(Mathf.Lerp(TriggerRots.x, TriggerRots.y, m_triggerFloat), 0f, 0f);
				m_lastTriggerFloat = m_triggerFloat;
			}
			if (m_isDestroyed)
			{
				return;
			}
			if (m_curHammerRot != m_tarHammerRot)
			{
				float num = 7200f;
				if (HammerState == HState.Halfcock || HammerState == HState.Fullcock)
				{
					num = 360f;
				}
				m_curHammerRot = Mathf.MoveTowards(m_curHammerRot, m_tarHammerRot, Time.deltaTime * num);
				SetAnimatedComponent(Hammer, m_curHammerRot, InterpStyle.Rotation, Axis.X);
			}
			if (m_triggerFloat > 0.7f && HammerState == HState.Fullcock)
			{
				ReleaseHammer();
			}
		}

		private void ReleaseHammer()
		{
			if (HammerState != HState.Fullcock)
			{
				return;
			}
			HammerState = HState.Uncocked;
			m_tarHammerRot = HammerRots.x;
			if (HitWithFlint())
			{
				PlayAudioAsHandling(AudEvent_HammerHit_Clean, Hammer.position);
				if (FlashPans[m_curFlashpan].FrizenState == FlintlockFlashPan.FState.Down)
				{
					PlayAudioAsHandling(AudEvent_Spark, Hammer.position);
					Sparks.Emit(15);
				}
				FlashPans[m_curFlashpan].HammerHit(FState, Flint: true);
			}
			else
			{
				FlashPans[m_curFlashpan].HammerHit(FState, Flint: false);
				PlayAudioAsHandling(AudEvent_HammerHit_Dull, Hammer.position);
			}
		}

		private void MoveToHalfCock()
		{
			if (HammerState == HState.Uncocked)
			{
				HammerState = HState.Halfcock;
				m_tarHammerRot = HammerRots.y;
				PlayAudioAsHandling(AudEvent_HammerHalfCock, Hammer.position);
			}
		}

		private void MoveToFullCock()
		{
			if (HammerState != HState.Fullcock)
			{
				HammerState = HState.Fullcock;
				m_tarHammerRot = HammerRots.z;
				PlayAudioAsHandling(AudEvent_HammerCock, Hammer.position);
			}
		}
	}
}
