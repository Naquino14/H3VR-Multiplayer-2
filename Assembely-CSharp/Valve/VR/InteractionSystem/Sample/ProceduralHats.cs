using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class ProceduralHats : MonoBehaviour
	{
		public GameObject[] hats;

		public float hatSwitchTime;

		private void Start()
		{
			SwitchToHat(0);
		}

		private void OnEnable()
		{
			StartCoroutine(HatSwitcher());
		}

		private IEnumerator HatSwitcher()
		{
			while (true)
			{
				yield return new WaitForSeconds(hatSwitchTime);
				Transform cam = Camera.main.transform;
				while (Vector3.Angle(cam.forward, base.transform.position - cam.position) < 90f)
				{
					yield return new WaitForSeconds(0.1f);
				}
				ChooseHat();
			}
		}

		private void ChooseHat()
		{
			SwitchToHat(Random.Range(0, hats.Length));
		}

		private void SwitchToHat(int hat)
		{
			for (int i = 0; i < hats.Length; i++)
			{
				hats[i].SetActive(hat == i);
			}
		}
	}
}
