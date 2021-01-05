using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Flaregun : FVRFireArm
	{
		[Header("Flaregun Params")]
		public Renderer[] GunUndamaged;

		public Renderer[] GunDamaged;

		public FVRFireArmChamber Chamber;

		public Axis HingeAxis;

		public Transform Hinge;

		public float RotOut = 35f;

		public bool CanUnlatch = true;

		public bool CanFlick = true;

		public bool IsHighPressureTolerant;

		private bool m_isHammerCocked;

		private bool m_isTriggerReset = true;

		private bool m_isLatched = true;

		private bool m_isDestroyed;

		private float TriggerFloat;

		public Transform Hammer;

		public bool HasVisibleHammer = true;

		public bool CanCockHammer = true;

		public bool CocksOnOpen;

		private float m_hammerXRot;

		public Axis HammerAxis;

		public InterpStyle HammerInterp = InterpStyle.Rotation;

		public float HammerMinRot;

		public float HammerMaxRot = -70f;

		public Transform Trigger;

		public Vector2 TriggerForwardBackRots;

		public Transform Muzzle;

		public ParticleSystem SmokePSystem;

		public ParticleSystem DestroyPSystem;

		public bool DeletesCartridgeOnFire;

		public bool IsFallingBlock;

		public Transform FallingBlock;

		public Vector3 FallingBlockPos_Up;

		public Vector3 FallingBlockPos_Down;

		protected override void Awake()
		{
			base.Awake();
			if (CanUnlatch)
			{
				Chamber.IsAccessible = false;
			}
			else
			{
				Chamber.IsAccessible = true;
			}
		}

		protected override void FVRUpdate()
		{
			if (HasVisibleHammer)
			{
				if (m_isHammerCocked)
				{
					m_hammerXRot = Mathf.Lerp(m_hammerXRot, HammerMaxRot, Time.deltaTime * 12f);
				}
				else
				{
					m_hammerXRot = Mathf.Lerp(m_hammerXRot, 0f, Time.deltaTime * 25f);
				}
				Hammer.localEulerAngles = new Vector3(m_hammerXRot, 0f, 0f);
			}
			if (!m_isLatched && Vector3.Angle(Vector3.up, Chamber.transform.forward) < 70f && Chamber.IsFull && Chamber.IsSpent)
			{
				PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
				Chamber.EjectRound(Chamber.transform.position + Chamber.transform.forward * -0.08f, Chamber.transform.forward * -0.01f, Vector3.right);
			}
		}

		public void ToggleLatchState()
		{
			if (m_isLatched)
			{
				Unlatch();
			}
			else if (!m_isLatched)
			{
				Latch();
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			float num = 0f;
			switch (HingeAxis)
			{
			case Axis.X:
				num = base.transform.InverseTransformDirection(hand.Input.VelAngularWorld).x;
				break;
			case Axis.Y:
				num = base.transform.InverseTransformDirection(hand.Input.VelAngularWorld).y;
				break;
			case Axis.Z:
				num = base.transform.InverseTransformDirection(hand.Input.VelAngularWorld).z;
				break;
			}
			if (num > 15f && CanUnlatch && !m_isHammerCocked && CanFlick)
			{
				Unlatch();
			}
			else if (num < -15f && CanUnlatch && CanFlick)
			{
				Latch();
			}
			bool flag = false;
			bool flag2 = false;
			if (!IsAltHeld)
			{
				if (hand.IsInStreamlinedMode)
				{
					if (hand.Input.BYButtonDown)
					{
						flag2 = true;
					}
					if (hand.Input.AXButtonDown)
					{
						flag = true;
					}
				}
				else if (hand.Input.TouchpadDown)
				{
					Vector2 touchpadAxes = hand.Input.TouchpadAxes;
					if (touchpadAxes.magnitude > 0.2f && Vector2.Angle(touchpadAxes, Vector2.down) < 45f && CanCockHammer)
					{
						CockHammer();
					}
					else if (touchpadAxes.magnitude > 0.2f && (Vector2.Angle(touchpadAxes, Vector2.left) < 45f || Vector2.Angle(touchpadAxes, Vector2.right) < 45f) && CanUnlatch)
					{
						ToggleLatchState();
					}
				}
			}
			if (flag)
			{
				CockHammer();
			}
			if (flag2)
			{
				ToggleLatchState();
			}
			if (m_isDestroyed)
			{
				return;
			}
			if (m_hasTriggeredUpSinceBegin && !IsAltHeld)
			{
				TriggerFloat = hand.Input.TriggerFloat;
			}
			else
			{
				TriggerFloat = 0f;
			}
			float x = Mathf.Lerp(TriggerForwardBackRots.x, TriggerForwardBackRots.y, TriggerFloat);
			Trigger.localEulerAngles = new Vector3(x, 0f, 0f);
			if (TriggerFloat > 0.7f)
			{
				if (m_isTriggerReset && m_isHammerCocked)
				{
					m_isTriggerReset = false;
					m_isHammerCocked = false;
					if (Hammer != null)
					{
						SetAnimatedComponent(Hammer, HammerMinRot, HammerInterp, HammerAxis);
					}
					PlayAudioEvent(FirearmAudioEventType.HammerHit);
					Fire();
				}
			}
			else if (TriggerFloat < 0.2f && !m_isTriggerReset)
			{
				m_isTriggerReset = true;
			}
		}

		private void Fire()
		{
			if (m_isLatched && Chamber.Fire())
			{
				base.Fire(Chamber, GetMuzzle(), doBuzz: true);
				FireMuzzleSmoke();
				bool twoHandStabilized = IsTwoHandStabilized();
				bool foregripStabilized = base.AltGrip != null;
				bool shoulderStabilized = IsShoulderStabilized();
				if (Chamber.GetRound().IsHighPressure && !IsHighPressureTolerant)
				{
					Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
					Destroy();
				}
				else if (Chamber.GetRound().IsHighPressure && IsHighPressureTolerant)
				{
					Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
				}
				PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
				if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
				{
					Chamber.IsSpent = false;
					Chamber.UpdateProxyDisplay();
				}
				else if (Chamber.GetRound().IsCaseless)
				{
					Chamber.SetRound(null);
				}
				if (DeletesCartridgeOnFire)
				{
					Chamber.SetRound(null);
				}
			}
		}

		private void Unlatch()
		{
			if (m_isLatched)
			{
				PlayAudioEvent(FirearmAudioEventType.BreachOpen);
				switch (HingeAxis)
				{
				case Axis.X:
					Hinge.localEulerAngles = new Vector3(RotOut, 0f, 0f);
					break;
				case Axis.Y:
					Hinge.localEulerAngles = new Vector3(0f, RotOut, 0f);
					break;
				case Axis.Z:
					Hinge.localEulerAngles = new Vector3(0f, 0f, RotOut);
					break;
				}
				m_isLatched = false;
				Chamber.IsAccessible = true;
				if (CocksOnOpen)
				{
					CockHammer();
				}
				if (IsFallingBlock)
				{
					FallingBlock.localPosition = FallingBlockPos_Down;
				}
			}
		}

		private void Latch()
		{
			if (!m_isLatched)
			{
				PlayAudioEvent(FirearmAudioEventType.BreachClose);
				Hinge.localEulerAngles = new Vector3(0f, 0f, 0f);
				m_isLatched = true;
				Chamber.IsAccessible = false;
				if (IsFallingBlock)
				{
					FallingBlock.localPosition = FallingBlockPos_Up;
				}
			}
		}

		private void CockHammer()
		{
			if (!m_isHammerCocked)
			{
				PlayAudioEvent(FirearmAudioEventType.Prefire);
				m_isHammerCocked = true;
				if (Hammer != null)
				{
					SetAnimatedComponent(Hammer, HammerMaxRot, HammerInterp, HammerAxis);
				}
			}
		}

		private void Destroy()
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				DestroyPSystem.Emit(25);
				for (int i = 0; i < GunUndamaged.Length; i++)
				{
					GunUndamaged[i].enabled = false;
					GunDamaged[i].enabled = true;
				}
			}
		}

		public override List<FireArmRoundClass> GetChamberRoundList()
		{
			if (Chamber.IsFull && !Chamber.IsSpent)
			{
				List<FireArmRoundClass> list = new List<FireArmRoundClass>();
				list.Add(Chamber.GetRound().RoundClass);
				return list;
			}
			return null;
		}

		public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
		{
			if (rounds.Count > 0)
			{
				Chamber.Autochamber(rounds[0]);
			}
		}

		public override List<string> GetFlagList()
		{
			return null;
		}

		public override void SetFromFlagList(List<string> flags)
		{
		}
	}
}
