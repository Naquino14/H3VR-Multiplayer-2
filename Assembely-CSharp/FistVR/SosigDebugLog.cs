using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class SosigDebugLog : MonoBehaviour
	{
		public Sosig S;

		public Text Readout_Stun;

		public Text Readout_Confusion;

		private void Start()
		{
		}

		private void Update()
		{
			Readout_Stun.text = S.m_stunTimeLeft.ToString();
			Readout_Confusion.text = S.m_confusedTime.ToString();
		}
	}
}
