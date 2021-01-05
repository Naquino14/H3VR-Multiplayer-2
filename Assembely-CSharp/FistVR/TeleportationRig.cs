using UnityEngine;

namespace FistVR
{
	public class TeleportationRig : MonoBehaviour
	{
		public Transform CircularIndicatorTransform;

		public Renderer CircularIndicatorRenderer;

		public Transform CircularIndicatorBillboard;

		public Transform HeadModel;

		public Transform LeftHandModel;

		public Transform RightHandModel;

		private float m_circularOffsetStart = 0.5f;

		private float m_circularOffsetEnd;

		private float m_indicatorTick;

		private void Start()
		{
			CircularIndicatorRenderer.material.SetTextureOffset("_MainTex", new Vector2(0f, m_circularOffsetStart));
			m_indicatorTick = 0f;
		}

		public void SetCircularIndicatorTick(float t)
		{
			float y = Mathf.Lerp(m_circularOffsetStart, m_circularOffsetEnd, t);
			CircularIndicatorRenderer.material.SetTextureOffset("_MainTex", new Vector2(0f, y));
		}

		public void SetRigPositions(Vector3 RootPosition, Transform Head, Transform LeftHand, Transform RightHand, bool indicatorAtHeadXZ, bool displayCircle)
		{
			if (indicatorAtHeadXZ)
			{
				CircularIndicatorTransform.localPosition = new Vector3(Head.localPosition.x, 1.8f, Head.localPosition.z);
			}
			else
			{
				CircularIndicatorTransform.localPosition = new Vector3(0f, 1.8f, 0f);
			}
			if (displayCircle)
			{
				CircularIndicatorBillboard.gameObject.SetActive(value: true);
				CircularIndicatorBillboard.LookAt(Head.position, Vector3.up);
			}
			else
			{
				CircularIndicatorBillboard.gameObject.SetActive(value: false);
			}
			base.transform.position = RootPosition;
			HeadModel.localPosition = Head.localPosition;
			LeftHandModel.localPosition = LeftHand.localPosition;
			RightHandModel.localPosition = RightHand.localPosition;
			HeadModel.localRotation = Head.localRotation;
			LeftHandModel.localRotation = LeftHand.localRotation;
			RightHandModel.localRotation = RightHand.localRotation;
		}
	}
}
