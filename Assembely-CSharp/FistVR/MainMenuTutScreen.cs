using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class MainMenuTutScreen : MonoBehaviour
	{
		private int m_curScreen;

		[Multiline(3)]
		public string[] Descrips;

		public Sprite[] Images;

		public Text DescripText;

		public Text Counter;

		public Image Image;

		public void Increment()
		{
			m_curScreen++;
			if (m_curScreen > 3)
			{
				m_curScreen = 0;
			}
			UpdateScreen();
		}

		public void Decrement()
		{
			m_curScreen--;
			if (m_curScreen < 0)
			{
				m_curScreen = 3;
			}
			UpdateScreen();
		}

		private void UpdateScreen()
		{
			DescripText.text = Descrips[m_curScreen];
			Counter.text = m_curScreen + 1 + "/4";
			Image.sprite = Images[m_curScreen];
		}
	}
}
