using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class OptionsPanel_Screenmanager : MonoBehaviour
	{
		public GameObject[] Screens;

		public Transform[] Lasers;

		public Transform[] RedLasers;

		public Transform[] BlueLasers;

		public Transform[] ScreenCorners;

		public LayerMask ButtonMask;

		private RaycastHit m_hit;

		public AudioSource audsource;

		public AudioClip audConfirm;

		public OptionsScreen_Controls OPS_Controls;

		public OptionsScreen_GUns OPS_Guns;

		public OptionsScreen_Movement OPS_Movement;

		public OptionsScreen_Quality OPS_Quality;

		public bool IsDebug;

		public Text DebugText;

		public int n;

		public GameObject n2;

		public void SetScreen(int index)
		{
			for (int i = 0; i < Screens.Length; i++)
			{
				if (i != index)
				{
					Screens[i].SetActive(value: false);
				}
			}
			Screens[index].SetActive(value: true);
			Screens[index].SendMessage("InitScreen", SendMessageOptions.DontRequireReceiver);
		}

		public void RefreshScreens()
		{
			OPS_Controls.InitScreen();
			OPS_Guns.InitScreen();
			OPS_Movement.InitScreen();
			OPS_Quality.InitScreen();
			if (IsDebug)
			{
				if (!DebugText.gameObject.activeSelf)
				{
					DebugText.gameObject.SetActive(value: true);
				}
				else
				{
					DebugText.gameObject.SetActive(value: false);
				}
			}
		}

		public void Update()
		{
			if (IsDebug)
			{
				DebugText.text = GM.CurrentPlayerBody.DebugString;
			}
		}

		public void ntest()
		{
			n++;
			if (n > 5)
			{
				Object.Instantiate(n2, base.transform.position + Vector3.up, base.transform.rotation);
			}
		}
	}
}
