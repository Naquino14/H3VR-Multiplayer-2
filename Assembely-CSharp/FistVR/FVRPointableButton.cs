using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class FVRPointableButton : FVRPointable
	{
		public Color ColorUnselected;

		public Color ColorSelected;

		public Button Button;

		public Image Image;

		public Text Text;

		public Renderer Rend;

		private bool m_usesRend;

		public string ColorName = "_Color";

		private float m_scaleTick;

		private void Awake()
		{
			if (Button == null)
			{
				Button = GetComponent<Button>();
			}
			if (Image != null)
			{
				Image.color = ColorUnselected;
			}
			if (Text != null)
			{
				Text.color = ColorUnselected;
			}
			if (Rend != null)
			{
				m_usesRend = true;
			}
		}

		public override void OnPoint(FVRViveHand hand)
		{
			base.OnPoint(hand);
			if (hand.Input.TriggerDown && Button != null)
			{
				Button.onClick.Invoke();
			}
		}

		public void ForceUpdate()
		{
			if (Image != null)
			{
				Image.color = Color.Lerp(ColorUnselected, ColorSelected, m_scaleTick);
			}
			if (Text != null)
			{
				Text.color = Color.Lerp(ColorUnselected, ColorSelected, m_scaleTick);
			}
		}

		[ContextMenu("SetButton")]
		public void SetButton()
		{
			Button = GetComponent<Button>();
		}

		[ContextMenu("SetText")]
		public void SetText()
		{
			Text = GetComponent<Text>();
		}

		[ContextMenu("SetImage")]
		public void SetImage()
		{
			Image = GetComponent<Image>();
		}

		[ContextMenu("SetRenderer")]
		public void SetRenderer()
		{
			Rend = GetComponent<Renderer>();
		}

		public override void Update()
		{
			base.Update();
			if (m_usesRend)
			{
				if (m_isBeingPointedAt)
				{
					float num = Mathf.Clamp(m_scaleTick + Time.deltaTime * 5f, 0f, 1f);
					if (num > m_scaleTick)
					{
						m_scaleTick = num;
						Rend.material.SetColor(ColorName, Color.Lerp(ColorUnselected, ColorSelected, m_scaleTick));
					}
				}
				else
				{
					float num2 = Mathf.Clamp(m_scaleTick - Time.deltaTime * 5f, 0f, 1f);
					if (num2 < m_scaleTick)
					{
						m_scaleTick = num2;
						Rend.material.SetColor(ColorName, Color.Lerp(ColorUnselected, ColorSelected, m_scaleTick));
					}
				}
			}
			if (Image != null)
			{
				if (m_isBeingPointedAt)
				{
					float num3 = Mathf.Clamp(m_scaleTick + Time.deltaTime * 5f, 0f, 1f);
					if (num3 > m_scaleTick)
					{
						m_scaleTick = num3;
						Image.color = Color.Lerp(ColorUnselected, ColorSelected, m_scaleTick);
					}
				}
				else
				{
					float num4 = Mathf.Clamp(m_scaleTick - Time.deltaTime * 5f, 0f, 1f);
					if (num4 < m_scaleTick)
					{
						m_scaleTick = num4;
						Image.color = Color.Lerp(ColorUnselected, ColorSelected, m_scaleTick);
					}
				}
			}
			if (!(Text != null))
			{
				return;
			}
			if (m_isBeingPointedAt)
			{
				float num5 = Mathf.Clamp(m_scaleTick + Time.deltaTime * 5f, 0f, 1f);
				if (num5 > m_scaleTick)
				{
					m_scaleTick = num5;
					Text.color = Color.Lerp(ColorUnselected, ColorSelected, m_scaleTick);
				}
			}
			else
			{
				float num6 = Mathf.Clamp(m_scaleTick - Time.deltaTime * 5f, 0f, 1f);
				if (num6 < m_scaleTick)
				{
					m_scaleTick = num6;
					Text.color = Color.Lerp(ColorUnselected, ColorSelected, m_scaleTick);
				}
			}
		}
	}
}
