namespace FistVR
{
	public class MG_MeatChunk : FVRPhysicalObject
	{
		public bool CanMeatBePickedUp;

		public int MeatID;

		public bool PlaysAcquireWhenGrabbed;

		private bool m_hasPlayedAcquire;

		public override void BeginInteraction(FVRViveHand hand)
		{
			if (PlaysAcquireWhenGrabbed && !m_hasPlayedAcquire)
			{
				m_hasPlayedAcquire = true;
				GM.MGMaster.Narrator.PlayMeatAcquire(MeatID);
			}
			base.BeginInteraction(hand);
		}

		public override bool IsInteractable()
		{
			return CanMeatBePickedUp;
		}

		public override bool IsDistantGrabbable()
		{
			if (!m_hasPlayedAcquire)
			{
				return false;
			}
			return base.IsDistantGrabbable();
		}
	}
}
