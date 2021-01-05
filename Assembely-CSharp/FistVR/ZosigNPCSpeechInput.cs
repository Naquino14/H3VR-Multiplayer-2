using UnityEngine;

namespace FistVR
{
	public class ZosigNPCSpeechInput : FVRPointable
	{
		public ZosigNPCInterface Interface;

		public Renderer Rend;

		public ZosigNPCProfile.NPCLine Line;

		private bool m_usesRend;

		public string ColorName = "_Color";

		public Color ColorUnselected;

		public Color ColorSelected;

		private float m_scaleTick;

		private void Awake()
		{
			if (Rend != null)
			{
				m_usesRend = true;
			}
			if (m_usesRend)
			{
				Rend.material.SetColor(ColorName, ColorUnselected);
			}
		}

		public void SetLine(ZosigNPCProfile.NPCLine line)
		{
			Rend.material.SetTexture("_MainTex", Interface.M.NPCSpeechIcons[(int)line.Type]);
			Line = line;
		}

		public void ClearLine()
		{
			Line = null;
		}

		public override void OnPoint(FVRViveHand hand)
		{
			base.OnPoint(hand);
			if (hand.Input.TriggerDown)
			{
				Interface.SpeakLine(Line);
			}
		}

		public override void Update()
		{
			base.Update();
			if (!m_usesRend)
			{
				return;
			}
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
	}
}
