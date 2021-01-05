using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class ButtonExample : MonoBehaviour
	{
		public HoverButton hoverButton;

		public GameObject prefab;

		private void Start()
		{
			hoverButton.onButtonDown.AddListener(OnButtonDown);
		}

		private void OnButtonDown(Hand hand)
		{
			StartCoroutine(DoPlant());
		}

		private IEnumerator DoPlant()
		{
			GameObject planting = Object.Instantiate(prefab);
			planting.transform.position = base.transform.position;
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
