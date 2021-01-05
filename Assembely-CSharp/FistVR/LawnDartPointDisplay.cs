using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class LawnDartPointDisplay : MonoBehaviour
	{
		public Text LabelText;

		private bool m_isFadingDown;

		private float m_alpha = 1f;

		private Color m_color = new Color(1f, 1f, 1f, 1f);

		private float m_upwardSpeed = 4f;

		public LawnDartGame Game;

		public void Activate(string txt, Vector3 pos, float upwardSpeed, int fireworks)
		{
			CancelInvoke();
			LabelText.text = txt;
			m_alpha = 1f;
			m_color.a = m_alpha;
			LabelText.color = m_color;
			base.transform.position = pos;
			m_upwardSpeed = upwardSpeed;
			m_isFadingDown = true;
			for (int i = 0; i < fireworks; i++)
			{
				Invoke("FireFireWork", 3.5f - (float)i);
			}
		}

		private void FireFireWork()
		{
			Game.FireWork(base.transform.position + Random.onUnitSphere * 3f);
		}

		public void Update()
		{
			if (m_isFadingDown)
			{
				if (m_alpha > 0f)
				{
					m_alpha -= Time.deltaTime * 0.25f;
				}
				else
				{
					m_alpha = 0f;
					m_isFadingDown = false;
				}
				base.transform.position += Vector3.up * m_upwardSpeed * Time.deltaTime;
			}
			m_color.a = m_alpha;
			LabelText.color = m_color;
			if (m_alpha <= 0f)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
