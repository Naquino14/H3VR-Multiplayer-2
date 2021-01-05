using UnityEngine;

namespace FistVR
{
	public class G11RecoilingSystem : MonoBehaviour
	{
		private float m_displacement;

		private float m_lerp = 1f;

		public float CurveSpeed = 5f;

		public AnimationCurve RecoilCurve_Normal;

		public AnimationCurve RecoilCurve_Powerful;

		private bool isCurrentCurvePowerful;

		public Transform RecoilingPart;

		public void Recoil(bool isPowerful)
		{
			m_lerp = 0f;
			isCurrentCurvePowerful = isPowerful;
		}

		private void Update()
		{
			if (m_lerp < 1f)
			{
				m_lerp += Time.deltaTime * CurveSpeed;
				m_lerp = Mathf.Clamp(m_lerp, 0f, 1f);
				float num = 0f;
				num = (m_displacement = ((!isCurrentCurvePowerful) ? RecoilCurve_Normal.Evaluate(m_lerp) : RecoilCurve_Powerful.Evaluate(m_lerp)));
				RecoilingPart.localPosition = new Vector3(RecoilingPart.localPosition.x, RecoilingPart.localPosition.y, m_displacement);
			}
		}
	}
}
