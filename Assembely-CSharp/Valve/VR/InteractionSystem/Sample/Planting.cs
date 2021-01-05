using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class Planting : MonoBehaviour
	{
		public SteamVR_Action_Boolean plantAction;

		public Hand hand;

		public GameObject prefabToPlant;

		private void OnEnable()
		{
			if (hand == null)
			{
				hand = GetComponent<Hand>();
			}
			if (plantAction == null)
			{
				Debug.LogError("<b>[SteamVR Interaction]</b> No plant action assigned");
			}
			else
			{
				plantAction.AddOnChangeListener(OnPlantActionChange, hand.handType);
			}
		}

		private void OnDisable()
		{
			if (plantAction != null)
			{
				plantAction.RemoveOnChangeListener(OnPlantActionChange, hand.handType);
			}
		}

		private void OnPlantActionChange(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue)
		{
			if (newValue)
			{
				Plant();
			}
		}

		public void Plant()
		{
			StartCoroutine(DoPlant());
		}

		private IEnumerator DoPlant()
		{
			Vector3 plantPosition;
			if (Physics.Raycast(hand.transform.position, Vector3.down, out var hitInfo))
			{
				plantPosition = hitInfo.point + Vector3.up * 0.05f;
			}
			else
			{
				plantPosition = hand.transform.position;
				plantPosition.y = Player.instance.transform.position.y;
			}
			GameObject planting = Object.Instantiate(prefabToPlant);
			planting.transform.position = plantPosition;
			planting.transform.rotation = Quaternion.Euler(0f, Random.value * 360f, 0f);
			planting.GetComponentInChildren<MeshRenderer>().material.SetColor("_TintColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
			Rigidbody rigidbody = planting.GetComponent<Rigidbody>();
			if (rigidbody != null)
			{
				rigidbody.isKinematic = true;
			}
			Vector3 initialScale = Vector3.one * 0.01f;
			Vector3 targetScale = Vector3.one * (1f + Random.value * 0.25f);
			float startTime = Time.time;
			float overTime = 0.5f;
			float endTime = startTime + overTime;
			while (Time.time < endTime)
			{
				planting.transform.localScale = Vector3.Slerp(initialScale, targetScale, (Time.time - startTime) / overTime);
				yield return null;
			}
			if (rigidbody != null)
			{
				rigidbody.isKinematic = false;
			}
		}
	}
}
