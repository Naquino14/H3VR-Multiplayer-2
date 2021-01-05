using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class MainMenuLoadSceneButton : FVRPointable
	{
		public MainMenuScreen Screen;

		private Color m_colorUnselected;

		private Color m_colorSelected = Color.white;

		public Image ButtonImage;

		private float m_scaleTick;

		private void Awake()
		{
			m_colorUnselected = ButtonImage.color;
		}

		public override void OnPoint(FVRViveHand hand)
		{
			base.OnPoint(hand);
			if (hand.Input.TriggerDown)
			{
				Screen.LoadScene();
			}
		}

		public override void Update()
		{
			base.Update();
			if (m_isBeingPointedAt)
			{
				float num = Mathf.Clamp(m_scaleTick + Time.deltaTime * 5f, 0f, 1f);
				if (num > m_scaleTick)
				{
					m_scaleTick = num;
					ButtonImage.color = Color.Lerp(m_colorUnselected, m_colorSelected, m_scaleTick);
				}
			}
			else
			{
				float num2 = Mathf.Clamp(m_scaleTick - Time.deltaTime * 5f, 0f, 1f);
				if (num2 < m_scaleTick)
				{
					m_scaleTick = num2;
					ButtonImage.color = Color.Lerp(m_colorUnselected, m_colorSelected, m_scaleTick);
				}
			}
		}
	}
}
