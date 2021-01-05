using System;
using UnityEngine;

namespace FistVR
{
	public class AdventBoxLever : FVRInteractiveObject
	{
		public Transform LeverRoot;

		public Transform RotPosUp;

		public Transform RotPosDown;

		private bool m_hasSwitched;

		public AdventBox Box;

		public WW_Bunker Bunker;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - LeverRoot.position;
			Vector3 vector2 = Vector3.ProjectOnPlane(vector, LeverRoot.right);
			float a = Vector3.Angle(vector2, LeverRoot.forward);
			float num = Mathf.Min(a, 45f);
			Vector3 vector3 = Vector3.RotateTowards(LeverRoot.forward, vector2, num * ((float)Math.PI / 180f), 1f);
			base.transform.rotation = Quaternion.LookRotation(vector3, Vector3.up);
			if (!m_hasSwitched && Vector3.Angle(vector3, RotPosDown.forward) < 10f)
			{
				m_hasSwitched = true;
				if (Bunker != null)
				{
					Bunker.OpenField();
				}
				else
				{
					Box.OpenBox();
				}
			}
		}

		public void SetPulled()
		{
			m_hasSwitched = true;
			base.transform.rotation = RotPosDown.rotation;
		}

		private void Update()
		{
			if (base.IsHeld)
			{
				return;
			}
			if (m_hasSwitched)
			{
				if (Vector3.Angle(RotPosDown.forward, base.transform.forward) > 1f)
				{
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, RotPosDown.rotation, Time.deltaTime * 2f);
				}
			}
			else if (Vector3.Angle(RotPosUp.forward, base.transform.forward) > 1f)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, RotPosUp.rotation, Time.deltaTime * 2f);
			}
		}
	}
}
