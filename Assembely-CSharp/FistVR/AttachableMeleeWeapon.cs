namespace FistVR
{
	public class AttachableMeleeWeapon : FVRFireArmAttachment
	{
		public override bool CanAttach()
		{
			if (MP.IsJointedToObject)
			{
				return false;
			}
			return true;
		}

		public override bool CanDetach()
		{
			if (curMount != null && curMount.GetRootMount().MyObject.MP.IsJointedToObject)
			{
				return false;
			}
			return true;
		}
	}
}
