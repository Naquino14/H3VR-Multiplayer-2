using UnityEngine;

namespace FistVR
{
	public class G11ChargingHandle : FVRInteractiveObject
	{
		public OpenBoltReceiver Weapon;

		public Transform RotPiece;

		public AudioEvent HandleCrank;

		private float m_curRot;

		public Transform FlapPiece;

		private Vector3 lastHandForward = Vector3.zero;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			SetFlapState(isOut: true);
			lastHandForward = Vector3.ProjectOnPlane(m_hand.transform.up, base.transform.right);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
		}

		private void SetFlapState(bool isOut)
		{
			if (isOut)
			{
				FlapPiece.localEulerAngles = new Vector3(0f, 90f, -90f);
			}
			else
			{
				FlapPiece.localEulerAngles = new Vector3(0f, 90f, 0f);
			}
		}

		protected override void FVRFixedUpdate()
		{
			if (base.IsHeld)
			{
				float curRot = m_curRot;
				Vector3 lhs = Vector3.ProjectOnPlane(m_hand.transform.up, -base.transform.right);
				Vector3 rhs = Vector3.ProjectOnPlane(lastHandForward, -base.transform.right);
				float num = Mathf.Atan2(Vector3.Dot(-base.transform.right, Vector3.Cross(lhs, rhs)), Vector3.Dot(lhs, rhs)) * 57.29578f;
				if (num > 0f)
				{
					m_curRot -= num;
				}
				if (curRot > -180f && m_curRot <= -180f)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, HandleCrank, base.transform.position);
				}
				if (m_curRot <= -360f)
				{
					RotPiece.localEulerAngles = new Vector3(0f, 0f, 0f);
					Weapon.Bolt.SetBoltToRear();
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, HandleCrank, base.transform.position);
					m_curRot = 0f;
					SetFlapState(isOut: false);
					m_hand.EndInteractionIfHeld(this);
					ForceBreakInteraction();
				}
				else
				{
					RotPiece.localEulerAngles = new Vector3(0f, 0f, m_curRot);
				}
				lastHandForward = lhs;
			}
			base.FVRFixedUpdate();
		}
	}
}
