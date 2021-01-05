using UnityEngine;

namespace FistVR
{
	public class OmniGameManager : MonoBehaviour
	{
		public enum OmniGMScreen
		{
			TileScreen,
			DetailsScreen
		}

		public OmniSequenceLibrary Library;

		private OmniGMScreen m_screen;

		public GameObject[] Screens;

		private void ReDrawScreen()
		{
			if (!Screens[(int)m_screen].activeSelf)
			{
				Screens[(int)m_screen].SetActive(value: true);
			}
			for (int i = 0; i < Screens.Length; i++)
			{
				if (i != (int)m_screen && Screens[i].activeSelf)
				{
					Screens[i].SetActive(value: false);
				}
			}
			switch (m_screen)
			{
			}
		}

		public void GotoTheme(int i)
		{
		}

		public void GotoCategory(int i)
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
