using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class KeyCodeDisplay : MonoBehaviour
	{
		public int MaxDigits;

		public Text DisplayText;

		public string MyText = string.Empty;

		public void ButtonMessage(int i)
		{
			if (i < 10 && i > -1 && MyText.Length < MaxDigits)
			{
				MyText += i;
			}
			DisplayText.text = MyText;
		}

		public void ClearNumber(int i)
		{
			MyText = string.Empty;
			DisplayText.text = string.Empty;
		}
	}
}
