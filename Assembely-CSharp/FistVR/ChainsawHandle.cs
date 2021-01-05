using UnityEngine;

namespace FistVR
{
	public class ChainsawHandle : FVRInteractiveObject
	{
		[Header("ChainsawHandle Params")]
		public Transform BasePoint;

		public Chainsaw Chainsaw;

		public Transform Cable;

		private Vector3 dirToHand = Vector3.zero;

		private float m_curCableLength;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			dirToHand = hand.transform.position - BasePoint.position;
			dirToHand = Vector3.ClampMagnitude(dirToHand, 1.2f);
			m_curCableLength = dirToHand.magnitude;
			Chainsaw.SetCableLength(m_curCableLength);
			base.transform.position = BasePoint.position + dirToHand.normalized * Mathf.Clamp(m_curCableLength - 0.05f, 0.01f, dirToHand.magnitude);
			Cable.LookAt(base.transform.position, Vector3.up);
			float num = Vector3.Distance(Cable.position, base.transform.position);
			Cable.localScale = new Vector3(0.002f, 0.002f, num + 0.02f);
			base.transform.rotation = Quaternion.LookRotation(dirToHand, -hand.transform.forward);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (!base.IsHeld)
			{
				Cable.LookAt(base.transform.position, Vector3.up);
				float num = Vector3.Distance(Cable.position, base.transform.position);
				Cable.localScale = new Vector3(0.002f, 0.002f, num + 0.02f);
				m_curCableLength -= Time.deltaTime * 10f;
				m_curCableLength = Mathf.Clamp(m_curCableLength, 0.002f, m_curCableLength);
				dirToHand = Vector3.ClampMagnitude(dirToHand, m_curCableLength);
				if (m_curCableLength > 0.002f)
				{
					base.transform.position = BasePoint.position + dirToHand;
					base.transform.rotation = BasePoint.rotation;
				}
				else if (m_curCableLength == 0.002f)
				{
					base.transform.position = BasePoint.position + dirToHand;
					base.transform.rotation = BasePoint.rotation;
					m_curCableLength = 0.001f;
				}
			}
		}
	}
}
