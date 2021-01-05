using System;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class MM_EAPAUnlocker : MonoBehaviour
	{
		[Serializable]
		public class EAPA
		{
			public FVRObject O1;

			public FVRObject O2;

			public FVRObject O3;

			public FVRObject O4;

			public FVRObject O5;
		}

		public Image[] ButtonImages;

		public Sprite[] ButtonSprites;

		public AudioSource Aud;

		public AudioClip AudClip_Unlock;

		public AudioClip AudClip_Spawn;

		public AudioClip AudClip_Fail;

		public int[] Costs_Bronze;

		public int[] Costs_Silver;

		public int[] Costs_Gold;

		public GameObject Prefab_EAPABox;

		public Transform EAPABox_SpawnPoint;

		public EAPA[] EAPAs;

		public AudioSource Music;

		public void Start()
		{
			for (int i = 0; i < ButtonImages.Length; i++)
			{
				if (GM.MMFlags.IsEAPAUnlocked(i))
				{
					ButtonImages[i].sprite = ButtonSprites[i];
				}
			}
		}

		public void UnlockEAPA(int i)
		{
			if (GM.MMFlags.IsEAPAUnlocked(i))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Prefab_EAPABox, EAPABox_SpawnPoint.position, EAPABox_SpawnPoint.rotation);
				MM_EAPACrate component = gameObject.GetComponent<MM_EAPACrate>();
				GameObject g = null;
				GameObject g2 = null;
				GameObject g3 = null;
				GameObject g4 = null;
				GameObject g5 = null;
				if (EAPAs[i].O1 != null)
				{
					g = EAPAs[i].O1.GetGameObject();
				}
				if (EAPAs[i].O2 != null)
				{
					g2 = EAPAs[i].O2.GetGameObject();
				}
				if (EAPAs[i].O3 != null)
				{
					g3 = EAPAs[i].O3.GetGameObject();
				}
				if (EAPAs[i].O4 != null)
				{
					g4 = EAPAs[i].O4.GetGameObject();
				}
				if (EAPAs[i].O5 != null)
				{
					g5 = EAPAs[i].O5.GetGameObject();
				}
				component.SetGOs(g, g2, g3, g4, g5);
				PlaySound(AudClip_Spawn);
				return;
			}
			bool flag = true;
			if (Costs_Bronze[i] > 0 && !GM.MMFlags.HasCurrency(MMCurrency.MarinatedMedallions, Costs_Bronze[i]))
			{
				flag = false;
			}
			if (Costs_Silver[i] > 0 && !GM.MMFlags.HasCurrency(MMCurrency.TenderloinTokens, Costs_Silver[i]))
			{
				flag = false;
			}
			if (Costs_Gold[i] > 0 && !GM.MMFlags.HasCurrency(MMCurrency.NutriciousNuggets, Costs_Gold[i]))
			{
				flag = false;
			}
			if (flag)
			{
				GM.MMFlags.RemoveCurrency(MMCurrency.MarinatedMedallions, Costs_Bronze[i]);
				GM.MMFlags.RemoveCurrency(MMCurrency.TenderloinTokens, Costs_Silver[i]);
				GM.MMFlags.RemoveCurrency(MMCurrency.NutriciousNuggets, Costs_Gold[i]);
				GM.MMFlags.UnlockEAPA(i);
				GM.MMFlags.SaveToFile();
				PlaySound(AudClip_Unlock);
				ButtonImages[i].sprite = ButtonSprites[i];
				if (!Music.isPlaying)
				{
					Music.Play();
				}
			}
			else
			{
				PlaySound(AudClip_Fail);
			}
		}

		private void PlaySound(AudioClip c)
		{
			if (!Aud.isPlaying)
			{
				Aud.clip = c;
				Aud.Play();
			}
		}
	}
}
