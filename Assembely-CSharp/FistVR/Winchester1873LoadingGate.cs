using UnityEngine;

namespace FistVR
{
	public class Winchester1873LoadingGate : MonoBehaviour
	{
		public Transform LoadingGateObject;

		public Vector2 LoadingGateRotRange;

		private float curRot;

		private float tarRot;

		public float Range = 0.02f;

		private void Update()
		{
			bool flag = false;
			float num = 1f;
			if (!(GM.CurrentMovementManager != null))
			{
				return;
			}
			for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
			{
				if (GM.CurrentMovementManager.Hands[i] != null && GM.CurrentMovementManager.Hands[i].CurrentInteractable != null && GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArmRound)
				{
					float num2 = Vector3.Distance(GM.CurrentMovementManager.Hands[i].CurrentInteractable.transform.position, base.transform.position);
					if (num2 < num)
					{
						num = num2;
						flag = true;
					}
				}
			}
			if (flag)
			{
				if (num <= Range)
				{
					tarRot = (Range - num) / Range;
				}
				else
				{
					tarRot = 0f;
				}
			}
			else
			{
				tarRot = 0f;
			}
			if (tarRot != curRot)
			{
				curRot = tarRot;
				LoadingGateObject.localEulerAngles = new Vector3(0f, Mathf.Lerp(LoadingGateRotRange.x, LoadingGateRotRange.y, curRot * 2f));
			}
		}
	}
}
