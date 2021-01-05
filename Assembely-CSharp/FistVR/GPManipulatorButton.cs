using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class GPManipulatorButton : MonoBehaviour
	{
		public float MaxPointingRange = 5f;

		protected bool m_isBeingPointedAt;

		public Color ColorUnselected;

		public Color ColorSelected;

		public Button Button;

		public Renderer Rend;

		public string ColorName = "_Color";

		private float m_scaleTick;

		private void Awake()
		{
			if (Button == null)
			{
				Button = GetComponent<Button>();
			}
		}

		public virtual void OnPoint()
		{
			m_isBeingPointedAt = true;
			BeginHoverDisplay();
		}

		public virtual void EndPoint()
		{
			m_isBeingPointedAt = false;
			EndHoverDisplay();
		}

		public virtual void BeginHoverDisplay()
		{
		}

		public virtual void EndHoverDisplay()
		{
		}

		public void Update()
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

		[ContextMenu("SetButton")]
		public void SetButton()
		{
			Button = GetComponent<Button>();
		}

		[ContextMenu("SetRenderer")]
		public void SetRenderer()
		{
			Rend = GetComponent<Renderer>();
		}
	}
}
