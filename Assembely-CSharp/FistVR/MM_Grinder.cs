using System;
using UnityEngine;

namespace FistVR
{
	public class MM_Grinder : MonoBehaviour
	{
		[Serializable]
		public class Recipe
		{
			public MMCurrency Type1;

			public MMCurrency Type2;

			public MMCurrency Result;
		}

		public Transform EjectionPoint;

		public Transform InFunnelFXPoint;

		public GameObject Prefab_Expel;

		public GameObject Prefab_Funnel;

		private float m_curRot;

		private float m_rotTilShot = 36f;

		public int amountOfOtherObjects;

		public int[] contents = new int[18];

		public AudioSource Aud;

		public AudioClip Audclip_DispenseRecipe;

		public AudioClip Audclip_DispenseSolo;

		public AudioClip Audclip_DispenseJunk;

		public AudioClip Audclip_Insert;

		public FVRObject[] CurrencyObjs;

		public Recipe[] Recipes;

		public void Crank(float f)
		{
			float value = f * 0.4f;
			value = Mathf.Clamp(value, 0f, 2f);
			m_curRot += value;
			if (m_curRot > 180f)
			{
				m_curRot -= 360f;
			}
			m_rotTilShot -= value;
			if (m_rotTilShot <= 0f)
			{
				Smash();
				m_rotTilShot = 36f;
			}
		}

		private void Smash()
		{
			for (int i = 0; i < Recipes.Length; i++)
			{
				if (contents[(int)Recipes[i].Type1] > 0 && contents[(int)Recipes[i].Type2] > 0)
				{
					contents[(int)Recipes[i].Type1]--;
					contents[(int)Recipes[i].Type2]--;
					GameObject gameObject = UnityEngine.Object.Instantiate(CurrencyObjs[(int)Recipes[i].Result].GetGameObject(), EjectionPoint.position, UnityEngine.Random.rotation);
					UnityEngine.Object.Instantiate(Prefab_Funnel, EjectionPoint.position, UnityEngine.Random.rotation);
					gameObject.GetComponent<Rigidbody>().velocity = EjectionPoint.forward * UnityEngine.Random.Range(0.5f, 2.5f);
					Aud.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
					Aud.PlayOneShot(Audclip_DispenseRecipe, UnityEngine.Random.Range(0.7f, 1f));
					return;
				}
			}
			for (int num = contents.Length - 1; num >= 0; num--)
			{
				if (contents[num] > 0)
				{
					contents[num]--;
					GameObject gameObject2 = UnityEngine.Object.Instantiate(CurrencyObjs[num].GetGameObject(), EjectionPoint.position, UnityEngine.Random.rotation);
					gameObject2.GetComponent<Rigidbody>().velocity = EjectionPoint.forward * UnityEngine.Random.Range(0.5f, 2.5f);
					Aud.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
					Aud.PlayOneShot(Audclip_DispenseSolo, UnityEngine.Random.Range(0.7f, 1f));
					return;
				}
			}
			if (amountOfOtherObjects > 0)
			{
				amountOfOtherObjects--;
				int num2 = UnityEngine.Random.Range(0, 3);
				GameObject gameObject3 = UnityEngine.Object.Instantiate(CurrencyObjs[num2].GetGameObject(), EjectionPoint.position, UnityEngine.Random.rotation);
				gameObject3.GetComponent<Rigidbody>().velocity = EjectionPoint.forward * UnityEngine.Random.Range(0.5f, 2.5f);
				Aud.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				Aud.PlayOneShot(Audclip_DispenseJunk, UnityEngine.Random.Range(0.7f, 1f));
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			Checkcol(col);
		}

		private void Checkcol(Collider col)
		{
			if (!(col.attachedRigidbody != null))
			{
				return;
			}
			FVRPhysicalObject component = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
			if (component != null && !(component.QuickbeltSlot != null))
			{
				if (component is MM_Currency)
				{
					MM_Currency mM_Currency = component as MM_Currency;
					int num = contents[(int)mM_Currency.Type];
					num++;
					contents[(int)mM_Currency.Type] = num;
				}
				else
				{
					amountOfOtherObjects++;
				}
				UnityEngine.Object.Instantiate(Prefab_Funnel, InFunnelFXPoint.position, UnityEngine.Random.rotation);
				Aud.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				Aud.PlayOneShot(Audclip_Insert, UnityEngine.Random.Range(0.7f, 1f));
				UnityEngine.Object.Destroy(component.gameObject);
			}
		}
	}
}
