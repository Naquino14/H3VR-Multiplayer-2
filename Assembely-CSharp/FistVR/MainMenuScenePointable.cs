using UnityEngine;

namespace FistVR
{
	public class MainMenuScenePointable : FVRPointable
	{
		private Vector3 m_startScale;

		private float m_scaleTick;

		public MainMenuScreen Screen;

		public MainMenuSceneDef Def;

		private void Awake()
		{
			m_startScale = base.transform.localScale;
		}

		public override void Update()
		{
			base.Update();
			if (m_isBeingPointedAt)
			{
				float num = Mathf.Clamp(m_scaleTick + Time.deltaTime * 3f, 0f, 1f);
				if (num > m_scaleTick)
				{
					m_scaleTick = num;
					base.transform.localScale = Vector3.Lerp(m_startScale, m_startScale * 1.25f, m_scaleTick);
				}
			}
			else
			{
				float num2 = Mathf.Clamp(m_scaleTick - Time.deltaTime * 3f, 0f, 1f);
				if (num2 < m_scaleTick)
				{
					m_scaleTick = num2;
					base.transform.localScale = Vector3.Lerp(m_startScale, m_startScale * 1.25f, m_scaleTick);
				}
			}
		}

		public override void OnPoint(FVRViveHand hand)
		{
			base.OnPoint(hand);
			if (hand.Input.TriggerDown)
			{
				Screen.SetSelectedScene(Def);
			}
		}
	}
}
