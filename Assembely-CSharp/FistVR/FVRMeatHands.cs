using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRMeatHands : MonoBehaviour
	{
		public FVRViveHand hand;

		public List<Transform> FingerJoints_0;

		public List<Transform> WaggleJoints_0;

		public List<WaggleJoint> WaggleJointsCore_0;

		public List<Transform> FingerJoints_1;

		public List<Transform> WaggleJoints_1;

		public List<WaggleJoint> WaggleJointsCore_1;

		public List<Transform> FingerJoints_2;

		public List<Transform> WaggleJoints_2;

		public List<WaggleJoint> WaggleJointsCore_2;

		public List<Transform> FingerJoints_3;

		public List<Transform> WaggleJoints_3;

		public List<WaggleJoint> WaggleJointsCore_3;

		public List<Transform> FingerJoints_Thumb;

		public List<Transform> WaggleJoints_Thumb;

		public List<WaggleJoint> WaggleJointsCore_Thumb;

		private void Update()
		{
			if (hand.CMode != ControlMode.Index)
			{
				return;
			}
			for (int i = 0; i < FingerJoints_0.Count; i++)
			{
				float fingerCurl_Index = hand.Input.FingerCurl_Index;
				float num = Mathf.Lerp(10f, 75f, hand.Input.FingerCurl_Index);
				float num2 = WaggleJoints_0[i].localEulerAngles.x + num;
				if (num2 > 360f)
				{
					num2 -= 360f;
				}
				if (num2 < -360f)
				{
					num2 += 360f;
				}
				float x = Mathf.Lerp(num2, num, fingerCurl_Index);
				FingerJoints_0[i].localEulerAngles = new Vector3(x, 0f, 0f);
				WaggleJointsCore_0[i].Execute();
			}
			for (int j = 0; j < FingerJoints_1.Count; j++)
			{
				float fingerCurl_Middle = hand.Input.FingerCurl_Middle;
				float num3 = Mathf.Lerp(10f, 75f, hand.Input.FingerCurl_Middle);
				float num4 = WaggleJoints_1[j].localEulerAngles.x + num3;
				if (num4 > 360f)
				{
					num4 -= 360f;
				}
				if (num4 < -360f)
				{
					num4 += 360f;
				}
				float x2 = Mathf.Lerp(num4, num3, fingerCurl_Middle);
				FingerJoints_1[j].localEulerAngles = new Vector3(x2, 0f, 0f);
				WaggleJointsCore_1[j].Execute();
			}
			for (int k = 0; k < FingerJoints_2.Count; k++)
			{
				float fingerCurl_Ring = hand.Input.FingerCurl_Ring;
				float num5 = Mathf.Lerp(10f, 75f, hand.Input.FingerCurl_Ring);
				float num6 = WaggleJoints_2[k].localEulerAngles.x + num5;
				if (num6 > 360f)
				{
					num6 -= 360f;
				}
				if (num6 < -360f)
				{
					num6 += 360f;
				}
				float x3 = Mathf.Lerp(num6, num5, fingerCurl_Ring);
				FingerJoints_2[k].localEulerAngles = new Vector3(x3, 0f, 0f);
				WaggleJointsCore_2[k].Execute();
			}
			for (int l = 0; l < FingerJoints_3.Count; l++)
			{
				float fingerCurl_Pinky = hand.Input.FingerCurl_Pinky;
				float num7 = Mathf.Lerp(10f, 75f, hand.Input.FingerCurl_Pinky);
				float num8 = WaggleJoints_3[l].localEulerAngles.x + num7;
				if (num8 > 360f)
				{
					num8 -= 360f;
				}
				if (num8 < -360f)
				{
					num8 += 360f;
				}
				float x4 = Mathf.Lerp(num8, num7, fingerCurl_Pinky);
				FingerJoints_3[l].localEulerAngles = new Vector3(x4, 0f, 0f);
				WaggleJointsCore_3[l].Execute();
			}
			for (int m = 0; m < FingerJoints_Thumb.Count; m++)
			{
				float fingerCurl_Thumb = hand.Input.FingerCurl_Thumb;
				float num9 = Mathf.Lerp(10f, 75f, hand.Input.FingerCurl_Thumb);
				float num10 = WaggleJoints_Thumb[m].localEulerAngles.x + num9;
				if (num10 > 360f)
				{
					num10 -= 360f;
				}
				if (num10 < -360f)
				{
					num10 += 360f;
				}
				float x5 = Mathf.Lerp(num10, num9, fingerCurl_Thumb);
				FingerJoints_Thumb[m].localEulerAngles = new Vector3(x5, 0f, 0f);
				WaggleJointsCore_Thumb[m].Execute();
			}
		}
	}
}
