using UnityEngine;

namespace FistVR
{
	public class wwPuzzlePiece : FVRPhysicalObject
	{
		[Header("PuzzlePiece Stuff")]
		public string PuzzleID;

		public int PieceID;

		public bool IsLockedIntoPlace;

		public override bool IsInteractable()
		{
			if (IsLockedIntoPlace)
			{
				return false;
			}
			return base.IsInteractable();
		}

		public override bool IsDistantGrabbable()
		{
			if (IsLockedIntoPlace)
			{
				return false;
			}
			return base.IsDistantGrabbable();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (IsLockedIntoPlace && !base.RootRigidbody.isKinematic)
			{
				base.RootRigidbody.isKinematic = true;
			}
			else if (!IsLockedIntoPlace && base.RootRigidbody.isKinematic)
			{
				base.RootRigidbody.isKinematic = false;
			}
		}
	}
}
