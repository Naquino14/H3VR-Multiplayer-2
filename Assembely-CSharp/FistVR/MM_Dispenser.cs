using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class MM_Dispenser : MonoBehaviour
	{
		public Image Display;

		public Text TextReadout;

		public string CurrencyName;

		public MMCurrency Currency;

		public Transform SpawnPoint;

		public FVRObject CurrencyPrefab;

		public Sprite Sprite_Known;

		public Sprite Sprite_Unknown;

		public AudioSource Aud;

		public AudioClip AudClip_Dispense;

		public AudioClip AudClip_Fail;

		private float tick = 1f;

		private void Start()
		{
			UpdateReadOut();
			tick = Random.Range(0.5f, 1f);
		}

		private void Update()
		{
			tick -= Time.deltaTime;
			if (tick < 0f)
			{
				tick = Random.Range(0.5f, 1f);
				UpdateReadOut();
			}
		}

		private void UpdateReadOut()
		{
			if (GM.MMFlags.IsCurrencyKnown(Currency))
			{
				int num = GM.MMFlags.MMMTCs[(int)Currency];
				string currencyName = CurrencyName;
				currencyName += "\n";
				currencyName = currencyName + "[" + num + "]";
				currencyName += "\n";
				currencyName += "Click To Dispense";
				TextReadout.text = currencyName;
				Display.sprite = Sprite_Known;
			}
			else
			{
				TextReadout.text = "Unknown Currency\n[0]";
				Display.sprite = Sprite_Unknown;
			}
		}

		public void DispenseButton()
		{
			if (GM.MMFlags.HasCurrency(Currency))
			{
				GameObject gameObject = Object.Instantiate(CurrencyPrefab.GetGameObject(), SpawnPoint.position, SpawnPoint.rotation);
				FVRPhysicalObject component = gameObject.GetComponent<FVRPhysicalObject>();
				component.RootRigidbody.velocity = -base.transform.forward;
				Aud.PlayOneShot(AudClip_Dispense, 0.5f);
				GM.MMFlags.RemoveCurrency(Currency, 1);
				GM.MMFlags.SaveToFile();
			}
			else
			{
				Aud.PlayOneShot(AudClip_Fail, 0.5f);
			}
			UpdateReadOut();
		}
	}
}
