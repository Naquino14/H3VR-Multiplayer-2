using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class CubeGameSequenceSelectorv1 : MonoBehaviour
	{
		public Image[] SelectionLabels;

		public GameObject CanvasRoot;

		public int curSelectedSequence;

		public Color UnSelectedColor;

		public Color SelectedColor;

		private AudioSource aud;

		public void Awake()
		{
			UpdateLabelDisplay();
			aud = GetComponent<AudioSource>();
		}

		public void SelectSequence(int i)
		{
			curSelectedSequence = i;
			UpdateLabelDisplay();
			aud.PlayOneShot(aud.clip, 0.15f);
		}

		private void UpdateLabelDisplay()
		{
			for (int i = 0; i < SelectionLabels.Length; i++)
			{
				SelectionLabels[i].color = UnSelectedColor;
			}
			SelectionLabels[curSelectedSequence].color = SelectedColor;
		}
	}
}
