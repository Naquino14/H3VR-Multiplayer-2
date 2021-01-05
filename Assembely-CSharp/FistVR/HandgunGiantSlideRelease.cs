using UnityEngine;

namespace FistVR
{
	public class HandgunGiantSlideRelease : FVRInteractiveObject
	{
		public Transform UpPoint;

		public Transform DownPoint;

		public Handgun Handgun;

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (base.IsHeld)
			{
				Vector3 closestValidPoint = GetClosestValidPoint(UpPoint.position, DownPoint.position, base.m_handPos);
				if (base.m_handPos.y > closestValidPoint.y)
				{
					Handgun.IsSlideLockExternalPushedUp = true;
				}
				else
				{
					Handgun.IsSlideLockExternalPushedUp = false;
				}
				if (base.m_handPos.y < closestValidPoint.y)
				{
					Handgun.IsSlideLockExternalHeldDown = true;
				}
				else
				{
					Handgun.IsSlideLockExternalHeldDown = false;
				}
			}
			else
			{
				Handgun.IsSlideLockExternalPushedUp = false;
				Handgun.IsSlideLockExternalHeldDown = false;
			}
		}
	}
}
