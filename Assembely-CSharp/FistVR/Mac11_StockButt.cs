using UnityEngine;

namespace FistVR
{
	public class Mac11_StockButt : FVRInteractiveObject
	{
		[Header("Mac11 Stock Butt Config")]
		public Transform Stock;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			float num = 0f;
			Vector3 vector = hand.transform.position - base.transform.position;
			Vector3 from = Vector3.ProjectOnPlane(vector, Stock.right);
			num = ((Vector3.Angle(from, Stock.up) > 90f) ? (-90f) : ((!(Vector3.Angle(from, Stock.forward) < 90f)) ? (Vector3.Angle(from, Stock.up) * -1f) : 0f));
			base.transform.localEulerAngles = new Vector3(num, 0f, 0f);
		}
	}
}
