using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class GunCase_Cover : FVRInteractiveObject
	{
		public List<GunCase_Latch> Latches;

		public Transform Root;

		private float rotAngle;

		public float MinRot;

		public float MaxRot;

		private bool m_forceOpen;

		public void ForceOpen()
		{
			m_forceOpen = true;
		}

		public void LockCase()
		{
			for (int i = 0; i < Latches.Count; i++)
			{
				Latches[i].gameObject.SetActive(value: false);
			}
		}

		public void UnlockCase()
		{
			for (int i = 0; i < Latches.Count; i++)
			{
				Latches[i].gameObject.SetActive(value: true);
			}
		}

		public override bool IsInteractable()
		{
			if (m_forceOpen || (Latches[0].IsOpen() && Latches[1].IsOpen()))
			{
				return true;
			}
			return false;
		}

		public void Reset()
		{
			base.transform.localEulerAngles = Vector3.zero;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - base.transform.position;
			vector = Vector3.ProjectOnPlane(vector, Root.right).normalized;
			Vector3 forward = Root.transform.forward;
			rotAngle = Mathf.Atan2(Vector3.Dot(Root.right, Vector3.Cross(forward, vector)), Vector3.Dot(forward, vector)) * 57.29578f;
			if (rotAngle > 0f)
			{
				rotAngle -= 360f;
			}
			if (Mathf.Abs(rotAngle - MinRot) < 5f)
			{
				rotAngle = MinRot;
			}
			if (Mathf.Abs(rotAngle - MaxRot) < 5f)
			{
				rotAngle = MaxRot;
			}
			if (rotAngle >= MinRot && rotAngle <= MaxRot)
			{
				base.transform.localEulerAngles = new Vector3(rotAngle, 0f, 0f);
			}
		}
	}
}
