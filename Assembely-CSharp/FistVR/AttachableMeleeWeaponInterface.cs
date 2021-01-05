namespace FistVR
{
	public class AttachableMeleeWeaponInterface : FVRFireArmAttachmentInterface
	{
		public override void OnAttach()
		{
			base.OnAttach();
			if (Attachment.curMount.GetRootMount().Parent is FVRFireArm)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.GetRootMount().Parent as FVRFireArm;
				fVRFireArm.RegisterAttachedMeleeWeapon(Attachment as AttachableMeleeWeapon);
			}
		}

		public override void OnDetach()
		{
			if (Attachment.curMount.GetRootMount().Parent is FVRFireArm)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.GetRootMount().Parent as FVRFireArm;
				fVRFireArm.RegisterAttachedMeleeWeapon(null);
			}
			base.OnDetach();
		}
	}
}
