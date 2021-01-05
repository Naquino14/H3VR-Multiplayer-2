namespace FistVR
{
	public class FVRFireArmBeltGrabTrigger : FVRInteractiveObject
	{
		public FVRFireArmMagazine Mag;

		public override bool IsInteractable()
		{
			if (Mag.FireArm == null)
			{
				return false;
			}
			if (Mag.FireArm.HasBelt)
			{
				return false;
			}
			if (!Mag.HasARound())
			{
				return false;
			}
			return base.IsInteractable();
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (Mag.FireArm != null)
			{
				Mag.FireArm.BeltDD.BeltGrabbed(Mag, m_hand);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (base.IsHeld && Mag.FireArm != null)
			{
				Mag.FireArm.BeltDD.BeltGrabUpdate(Mag, m_hand);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			if (Mag.FireArm != null)
			{
				Mag.FireArm.BeltDD.BeltReleased(Mag, m_hand);
			}
			Mag.UpdateBulletDisplay();
			base.EndInteraction(hand);
		}
	}
}
