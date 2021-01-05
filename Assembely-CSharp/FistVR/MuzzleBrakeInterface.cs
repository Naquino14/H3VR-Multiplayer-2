namespace FistVR
{
	public class MuzzleBrakeInterface : MuzzleDeviceInterface
	{
		private MuzzleBrake tempBrake;

		protected override void Awake()
		{
			base.Awake();
			tempBrake = Attachment as MuzzleBrake;
		}

		public override void OnAttach()
		{
			tempBrake = Attachment as MuzzleBrake;
			if (Attachment.curMount.GetRootMount().Parent is FVRFireArm)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.GetRootMount().Parent as FVRFireArm;
				fVRFireArm.RegisterMuzzleBrake(tempBrake);
			}
			base.OnAttach();
		}

		public override void OnDetach()
		{
			if (Attachment.curMount.GetRootMount().Parent is FVRFireArm)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.GetRootMount().Parent as FVRFireArm;
				fVRFireArm.RegisterMuzzleBrake(null);
			}
			base.OnDetach();
		}
	}
}
