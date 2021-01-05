using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class Keyboard : MonoBehaviour
	{
		public List<KeyboardKey> Keys;

		public Text ActiveText;

		private string m_textString = string.Empty;

		private bool m_isCaps;

		private bool m_clearCapsAfterNextKey = true;

		public AudioSource KeyBoardAudio;

		private float timeSinceHit = 1f;

		public float reHitThreshold = 0.1f;

		public void SetActiveText(Text t)
		{
			ActiveText = t;
			m_textString = ActiveText.text;
			UpdateActiveText();
		}

		private void UpdateActiveText()
		{
			if (ActiveText != null)
			{
				ActiveText.text = m_textString;
			}
		}

		private void Update()
		{
			if (timeSinceHit < 1f)
			{
				timeSinceHit += Time.deltaTime;
			}
		}

		private void Start()
		{
			ReDraw();
		}

		private void SetCaps(bool b)
		{
			m_isCaps = b;
			ReDraw();
		}

		private void ToggleCaps()
		{
			SetCaps(!m_isCaps);
		}

		private void ReDraw()
		{
			for (int i = 0; i < Keys.Count; i++)
			{
				if (m_isCaps)
				{
					Keys[i].Text.text = Keys[i].UpperCase;
				}
				else
				{
					Keys[i].Text.text = Keys[i].LowerCase;
				}
			}
		}

		public void KeyInput(KeyboardKey.KeyBoardKeyType type, string lowercase, string uppercase)
		{
			if (timeSinceHit < reHitThreshold)
			{
				return;
			}
			timeSinceHit = 0f;
			KeyBoardAudio.pitch = Random.Range(0.9f, 1f);
			KeyBoardAudio.PlayOneShot(KeyBoardAudio.clip, 0.3f);
			switch (type)
			{
			case KeyboardKey.KeyBoardKeyType.AlphaNumeric:
				if (m_isCaps)
				{
					m_textString += uppercase;
					if (m_clearCapsAfterNextKey)
					{
						SetCaps(b: false);
					}
				}
				else
				{
					m_textString += lowercase;
				}
				break;
			case KeyboardKey.KeyBoardKeyType.Space:
				m_textString += " ";
				break;
			case KeyboardKey.KeyBoardKeyType.Tab:
				m_textString += "    ";
				break;
			case KeyboardKey.KeyBoardKeyType.Shift:
				ToggleCaps();
				m_clearCapsAfterNextKey = true;
				break;
			case KeyboardKey.KeyBoardKeyType.Caps:
				if (m_isCaps)
				{
					SetCaps(b: false);
					m_clearCapsAfterNextKey = true;
				}
				else
				{
					SetCaps(b: true);
					m_clearCapsAfterNextKey = false;
				}
				break;
			case KeyboardKey.KeyBoardKeyType.Backspace:
				if (m_textString.Length > 0)
				{
					m_textString = m_textString.Substring(0, m_textString.Length - 1);
				}
				break;
			}
			UpdateActiveText();
		}
	}
}
