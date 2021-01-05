using UnityEngine;

namespace FistVR
{
	public class FVRFireArmBelt : MonoBehaviour
	{
		public FVRFirearmBeltDisplayData DisplayData;

		public AnimationCurve AngleFromUpToInOutLerp;

		private float cycler;

		public FVRPhysicalObject rb;

		private void Update()
		{
			Vector3 normalized = (base.transform.up - base.transform.right).normalized;
			float num = Vector3.Angle(normalized, Vector3.up);
			float num2 = 0f;
			if (rb.IsHeld)
			{
				num2 = Vector3.Dot(rb.m_hand.Input.VelLinearWorld.normalized, normalized);
				num2 = num2 * rb.m_hand.Input.VelLinearWorld.magnitude * 4f;
				Debug.Log(num2);
			}
			num -= num2;
			float l = AngleFromUpToInOutLerp.Evaluate(num);
			DisplayData.ChainInterpolatedInOut.SetInterp(DisplayData.Chain_In, DisplayData.Chain_Out, l);
			cycler -= Time.deltaTime * 10f;
			if (cycler < 0f)
			{
				cycler += 1f;
			}
			DisplayData.ChainInterpolated01.SetInterp(DisplayData.ChainInterpolatedInOut, cycler);
		}
	}
}
