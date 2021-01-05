using UnityEngine;

namespace FistVR
{
	public class AttachableStock : FVRFireArmAttachmentInterface
	{
		public Transform Point_Stock;

		public override void OnAttach()
		{
			base.OnAttach();
			(Attachment.curMount.Parent as FVRFireArm).StockPos = Point_Stock;
			(Attachment.curMount.Parent as FVRFireArm).HasActiveShoulderStock = true;
		}

		public override void OnDetach()
		{
			(Attachment.curMount.Parent as FVRFireArm).StockPos = null;
			(Attachment.curMount.Parent as FVRFireArm).HasActiveShoulderStock = false;
			base.OnDetach();
		}
	}
}
