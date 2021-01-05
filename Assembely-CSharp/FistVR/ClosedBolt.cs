using UnityEngine;

namespace FistVR
{
	public class ClosedBolt : FVRInteractiveObject
	{
		public enum BoltPos
		{
			Forward,
			ForwardToMid,
			Locked,
			LockedToRear,
			Rear
		}

		[Header("Bolt")]
		public ClosedBoltWeapon Weapon;

		public float Speed_Forward;

		public float Speed_Rearward;

		public float Speed_Held;

		public float SpringStiffness = 5f;

		public BoltPos CurPos;

		public BoltPos LastPos;

		public Transform Point_Bolt_Forward;

		public Transform Point_Bolt_LockPoint;

		public Transform Point_Bolt_Rear;

		public Transform Point_Bolt_SafetyLock;

		public bool HasLastRoundBoltHoldOpen = true;

		public bool UsesAKSafetyLock;

		public bool DoesClipHoldBoltOpen = true;

		private float m_curBoltSpeed;

		private float m_boltZ_current;

		private float m_boltZ_heldTarget;

		private float m_boltZ_forward;

		private float m_boltZ_lock;

		private float m_boltZ_rear;

		private float m_boltZ_safetylock;

		private bool m_isBoltLocked;

		private bool m_isHandleHeld;

		private float m_handleLerp;

		[Header("Reciprocating Barrel")]
		public bool HasReciprocatingBarrel;

		public Transform Barrel;

		public Vector3 BarrelForward;

		public Vector3 BarrelRearward;

		private bool m_isBarrelReciprocating;

		[Header("Hammer")]
		public bool HasHammer;

		public Transform Hammer;

		public Vector3 HammerForward;

		public Vector3 HammerRearward;

		[Header("Rotating Bit")]
		public bool HasRotatingPart;

		public Transform RotatingPart;

		public Vector3 RotatingPartNeutralEulers;

		public Vector3 RotatingPartLeftEulers;

		public Vector3 RotatingPartRightEulers;

		[Header("Z Rot Part")]
		public bool HasZRotPart;

		public Transform ZRotPiece;

		public AnimationCurve ZRotCurve;

		public Vector2 ZAngles;

		[Header("Z Scale Part")]
		public bool HasZScalePart;

		public Transform ZScalePiece;

		public AnimationCurve ZScaleCurve;

		public bool ZRotPieceDips;

		public float DipMagnitude;

		protected override void Awake()
		{
			base.Awake();
			m_boltZ_current = base.transform.localPosition.z;
			m_boltZ_forward = Point_Bolt_Forward.localPosition.z;
			m_boltZ_lock = Point_Bolt_LockPoint.localPosition.z;
			m_boltZ_rear = Point_Bolt_Rear.localPosition.z;
			if (UsesAKSafetyLock)
			{
				m_boltZ_safetylock = Point_Bolt_SafetyLock.localPosition.z;
			}
		}

		public float GetBoltLerpBetweenLockAndFore()
		{
			return Mathf.InverseLerp(m_boltZ_lock, m_boltZ_forward, m_boltZ_current);
		}

		public float GetBoltLerpBetweenRearAndFore()
		{
			return Mathf.InverseLerp(m_boltZ_rear, m_boltZ_forward, m_boltZ_current);
		}

		public void LockBolt()
		{
			if (!m_isBoltLocked)
			{
				m_isBoltLocked = true;
			}
		}

		public void ReleaseBolt()
		{
			if (m_isBoltLocked)
			{
				if (!base.IsHeld)
				{
					Weapon.PlayAudioEvent(FirearmAudioEventType.BoltRelease);
				}
				m_isBoltLocked = false;
			}
		}

		public bool IsBoltLocked()
		{
			return m_isBoltLocked;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (HasRotatingPart)
			{
				Vector3 normalized = (base.transform.position - m_hand.PalmTransform.position).normalized;
				if (Vector3.Dot(normalized, base.transform.right) > 0f)
				{
					RotatingPart.localEulerAngles = RotatingPartLeftEulers;
				}
				else
				{
					RotatingPart.localEulerAngles = RotatingPartRightEulers;
				}
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			if (HasRotatingPart)
			{
				RotatingPart.localEulerAngles = RotatingPartNeutralEulers;
			}
			if (!m_isBoltLocked)
			{
				m_curBoltSpeed = Speed_Forward;
			}
			if (CurPos > BoltPos.Forward)
			{
				Weapon.PlayAudioEvent(FirearmAudioEventType.BoltRelease);
			}
			base.EndInteraction(hand);
		}

		public void UpdateHandleHeldState(bool state, float lerp)
		{
			m_isHandleHeld = state;
			m_handleLerp = lerp;
		}

		public void ImpartFiringImpulse()
		{
			m_curBoltSpeed = Speed_Rearward;
			if (CurPos == BoltPos.Forward)
			{
				m_isBarrelReciprocating = true;
			}
		}

		public bool IsBoltForwardOfSafetyLock()
		{
			if (m_boltZ_current > m_boltZ_safetylock)
			{
				return true;
			}
			return false;
		}

		public void UpdateBolt()
		{
			bool flag = false;
			if (base.IsHeld || m_isHandleHeld)
			{
				flag = true;
			}
			if (base.IsHeld)
			{
				Vector3 closestValidPoint = GetClosestValidPoint(Point_Bolt_Forward.position, Point_Bolt_Rear.position, m_hand.Input.Pos);
				m_boltZ_heldTarget = Weapon.transform.InverseTransformPoint(closestValidPoint).z;
			}
			else if (m_isHandleHeld)
			{
				m_boltZ_heldTarget = Mathf.Lerp(m_boltZ_forward, m_boltZ_rear, m_handleLerp);
			}
			Vector2 vector = new Vector2(m_boltZ_rear, m_boltZ_forward);
			if (m_boltZ_current <= m_boltZ_lock && m_isBoltLocked)
			{
				vector = new Vector2(m_boltZ_rear, m_boltZ_lock);
			}
			if (UsesAKSafetyLock && Weapon.IsWeaponOnSafe() && m_boltZ_current <= m_boltZ_safetylock)
			{
				vector = new Vector2(m_boltZ_safetylock, m_boltZ_forward);
			}
			if (DoesClipHoldBoltOpen && Weapon.Clip != null)
			{
				vector = new Vector2(m_boltZ_rear, m_boltZ_lock);
			}
			if (flag)
			{
				m_curBoltSpeed = 0f;
			}
			else if ((CurPos < BoltPos.LockedToRear && m_curBoltSpeed >= 0f) || LastPos >= BoltPos.Rear)
			{
				m_curBoltSpeed = Mathf.MoveTowards(m_curBoltSpeed, Speed_Forward, Time.deltaTime * SpringStiffness);
			}
			float boltZ_current = m_boltZ_current;
			float boltZ_current2 = m_boltZ_current;
			if (flag)
			{
				boltZ_current2 = m_boltZ_heldTarget;
				boltZ_current = Mathf.MoveTowards(m_boltZ_current, boltZ_current2, Speed_Held * Time.deltaTime);
			}
			else
			{
				boltZ_current = m_boltZ_current + m_curBoltSpeed * Time.deltaTime;
			}
			boltZ_current = Mathf.Clamp(boltZ_current, vector.x, vector.y);
			if (Mathf.Abs(boltZ_current - m_boltZ_current) > Mathf.Epsilon)
			{
				m_boltZ_current = boltZ_current;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, m_boltZ_current);
				if (HasReciprocatingBarrel && m_isBarrelReciprocating)
				{
					float t = 1f - GetBoltLerpBetweenLockAndFore();
					Barrel.localPosition = Vector3.Lerp(BarrelForward, BarrelRearward, t);
				}
				if (HasZRotPart)
				{
					float num = 1f - GetBoltLerpBetweenLockAndFore();
					float t2 = ZRotCurve.Evaluate(num);
					ZRotPiece.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(ZAngles.x, ZAngles.y, t2));
					if (ZRotPieceDips)
					{
						ZRotPiece.localPosition = Vector3.Lerp(Vector3.zero, Vector3.down * DipMagnitude, num);
					}
				}
				if (HasZScalePart)
				{
					float time = 1f - GetBoltLerpBetweenLockAndFore();
					float z = ZScaleCurve.Evaluate(time);
					ZScalePiece.localScale = new Vector3(1f, 1f, z);
				}
			}
			else
			{
				m_curBoltSpeed = 0f;
			}
			if (HasHammer)
			{
				if (Weapon.IsHammerCocked)
				{
					Hammer.localEulerAngles = HammerRearward;
				}
				else
				{
					float boltLerpBetweenLockAndFore = GetBoltLerpBetweenLockAndFore();
					Hammer.localEulerAngles = Vector3.Lerp(HammerRearward, HammerForward, boltLerpBetweenLockAndFore);
				}
			}
			BoltPos curPos = CurPos;
			curPos = ((!(Mathf.Abs(m_boltZ_current - m_boltZ_forward) < 0.001f)) ? ((Mathf.Abs(m_boltZ_current - m_boltZ_lock) < 0.001f) ? BoltPos.Locked : ((Mathf.Abs(m_boltZ_current - m_boltZ_rear) < 0.001f) ? BoltPos.Rear : ((m_boltZ_current > m_boltZ_lock) ? BoltPos.ForwardToMid : BoltPos.LockedToRear))) : BoltPos.Forward);
			int curPos2 = (int)CurPos;
			CurPos = curPos;
			if (CurPos >= BoltPos.ForwardToMid)
			{
				Weapon.IsBreachOpenForGasOut = true;
			}
			else
			{
				Weapon.IsBreachOpenForGasOut = false;
			}
			if (CurPos == BoltPos.Rear && LastPos != BoltPos.Rear)
			{
				BoltEvent_SmackRear();
			}
			if (CurPos == BoltPos.Locked && LastPos != BoltPos.Locked)
			{
				BoltEvent_BoltCaught();
			}
			if (CurPos >= BoltPos.Locked && LastPos < BoltPos.Locked)
			{
				BoltEvent_EjectRound();
			}
			if (CurPos < BoltPos.Locked && LastPos > BoltPos.ForwardToMid)
			{
				BoltEvent_ExtractRoundFromMag();
			}
			if (CurPos == BoltPos.Forward && LastPos != 0)
			{
				BoltEvent_ArriveAtFore();
			}
			if (CurPos >= BoltPos.Locked && ((HasLastRoundBoltHoldOpen && Weapon.Magazine != null && !Weapon.Magazine.HasARound() && !Weapon.Chamber.IsFull) || Weapon.IsBoltCatchButtonHeld))
			{
				LockBolt();
			}
			LastPos = CurPos;
		}

		private void BoltEvent_ArriveAtFore()
		{
			Weapon.ChamberRound();
			if (HasReciprocatingBarrel && m_isBarrelReciprocating)
			{
				m_isBarrelReciprocating = false;
				Barrel.localPosition = BarrelForward;
			}
			if (base.IsHeld)
			{
				Weapon.PlayAudioEvent(FirearmAudioEventType.BoltSlideForwardHeld);
			}
			else
			{
				Weapon.PlayAudioEvent(FirearmAudioEventType.BoltSlideForward);
			}
		}

		private void BoltEvent_EjectRound()
		{
			Weapon.EjectExtractedRound();
			Weapon.CockHammer();
		}

		private void BoltEvent_ExtractRoundFromMag()
		{
			Weapon.BeginChamberingRound();
		}

		private void BoltEvent_BoltCaught()
		{
			if (m_isBoltLocked)
			{
				Weapon.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackLocked);
			}
		}

		private void BoltEvent_SmackRear()
		{
			if ((base.IsHeld || m_isHandleHeld) && (!Weapon.BoltLocksWhenNoMagazineFound || Weapon.Magazine != null))
			{
				ReleaseBolt();
			}
			if (Weapon.EjectsMagazineOnEmpty && Weapon.Magazine != null && !Weapon.Magazine.HasARound())
			{
				Weapon.EjectMag();
			}
			if (Weapon.BoltLocksWhenNoMagazineFound && Weapon.Magazine == null)
			{
				LockBolt();
			}
			if (base.IsHeld)
			{
				Weapon.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackHeld);
			}
			else
			{
				Weapon.PlayAudioEvent(FirearmAudioEventType.BoltSlideBack);
			}
		}
	}
}
