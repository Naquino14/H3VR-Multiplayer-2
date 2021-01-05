using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class BearTrapInteractiblePiece : FVRInteractiveObject
	{
		public enum BearTrapState
		{
			Open,
			Closed,
			InteractingWith
		}

		public Transform BaseTransform;

		public Transform BladeTransform;

		public Transform Reference_Open;

		public Transform Reference_Closed;

		public AudioEvent AudEvent_SnapShut;

		public AudioEvent AudEvent_LockOpen;

		public BearTrapState State;

		public LayerMask LM_Damage;

		public Transform DamagePoint;

		public float DamageRadius;

		public Transform fakeDamPoint;

		public Transform fakeDamDir;

		private BearTrap m_trap;

		public void SetBearTrap(BearTrap trap)
		{
			m_trap = trap;
		}

		public override bool IsInteractable()
		{
			if (State == BearTrapState.Closed)
			{
				return true;
			}
			return false;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			State = BearTrapState.InteractingWith;
			Vector3 vector = hand.transform.position - BaseTransform.position;
			Vector3 vector2 = Vector3.ProjectOnPlane(vector, BladeTransform.forward);
			float num = Vector3.Angle(vector2, Vector3.up);
			if (num < 24f)
			{
				BladeTransform.rotation = Reference_Closed.rotation;
			}
			else if (num > 60f)
			{
				if (GM.ZMaster != null)
				{
					GM.ZMaster.FlagM.AddToFlag("s_t", 1);
				}
				LockOpen();
				m_trap.ForceOpen();
				ForceBreakInteraction();
			}
			else
			{
				BladeTransform.rotation = Quaternion.LookRotation(Reference_Closed.forward, vector2);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			Vector3 vector = hand.transform.position - BaseTransform.position;
			Vector3 from = Vector3.ProjectOnPlane(vector, BladeTransform.forward);
			float num = Vector3.Angle(from, Vector3.up);
			if (num < 24f)
			{
				Close();
			}
			else if (num > 60f)
			{
				LockOpen();
				m_trap.ForceOpen();
			}
			else
			{
				SnapShut();
			}
			base.EndInteraction(hand);
		}

		public void Open()
		{
			BladeTransform.rotation = Reference_Open.rotation;
			State = BearTrapState.Open;
		}

		public void Close()
		{
			BladeTransform.rotation = Reference_Closed.rotation;
			State = BearTrapState.Closed;
		}

		public void LockOpen()
		{
			if (State != 0)
			{
				BladeTransform.rotation = Reference_Open.rotation;
				float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
				float delay = num / 343f;
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, AudEvent_LockOpen, base.transform.position, delay);
				State = BearTrapState.Open;
			}
		}

		public void SnapShut()
		{
			if (State == BearTrapState.Closed)
			{
				return;
			}
			BladeTransform.rotation = Reference_Closed.rotation;
			float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
			float delay = num / 343f;
			SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, AudEvent_SnapShut, base.transform.position, delay);
			State = BearTrapState.Closed;
			Collider[] array = Physics.OverlapSphere(DamagePoint.position, DamageRadius, LM_Damage, QueryTriggerInteraction.Ignore);
			if (array.Length <= 0)
			{
				return;
			}
			HashSet<IFVRDamageable> hashSet = new HashSet<IFVRDamageable>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].attachedRigidbody != null)
				{
					IFVRDamageable component = array[i].attachedRigidbody.GetComponent<IFVRDamageable>();
					if (component != null && !hashSet.Contains(component))
					{
						hashSet.Add(component);
						Damage damage = new Damage();
						damage.Dam_Cutting = 15000f;
						damage.Dam_Piercing = 15000f;
						damage.Dam_TotalKinetic = 30000f;
						damage.damageSize = 0.1f;
						damage.Class = Damage.DamageClass.Environment;
						damage.hitNormal = -fakeDamDir.forward;
						damage.point = fakeDamPoint.position;
						damage.Source_IFF = 0;
						damage.strikeDir = fakeDamDir.forward;
						component.Damage(damage);
					}
				}
			}
			hashSet.Clear();
		}

		public bool CanSnapShut()
		{
			if (State == BearTrapState.Open)
			{
				return true;
			}
			return false;
		}
	}
}
