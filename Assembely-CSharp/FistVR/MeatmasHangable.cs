using UnityEngine;

namespace FistVR
{
	public class MeatmasHangable : FVRPhysicalObject
	{
		private SaveableTreeSystem Tree;

		public HangableDef Def;

		protected override void Awake()
		{
			base.Awake();
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			if (Tree != null)
			{
				Vector3 a = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
				Vector3 b = new Vector3(Tree.transform.position.x, 0f, Tree.transform.position.z);
				if (Vector3.Distance(a, b) <= 2.5f)
				{
					SetIsKinematicLocked(b: true);
				}
				else
				{
					SetIsKinematicLocked(b: false);
				}
			}
		}
	}
}
