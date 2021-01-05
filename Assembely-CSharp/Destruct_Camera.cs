using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Raycast destroyer (GabroMedia)")]
public class Destruct_Camera : MonoBehaviour
{
	private Camera cam;

	private bool inCycle;

	private GameObject smoke;

	public AudioClip[] shotSound;

	[Range(10f, 60f)]
	public float removeDebrisDelay;

	[Range(5f, 40f)]
	public float impactForce;

	[Range(0f, 1f)]
	public float gunShotFrequency;

	private void Start()
	{
		cam = GetComponent<Camera>();
	}

	private void Update()
	{
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity) && Input.GetMouseButton(0) && !inCycle)
		{
			StartCoroutine(delay(gunShotFrequency));
			AudioSource.PlayClipAtPoint(shotSound[Random.Range(0, shotSound.Length)], base.transform.position);
			if ((bool)hitInfo.transform.GetComponent<Destruct>())
			{
				Destruct component = hitInfo.transform.GetComponent<Destruct>();
				BreakObject(component, ray.direction, hitInfo.point);
			}
		}
	}

	private void BreakObject(Destruct instance, Vector3 rayDirection, Vector3 hitPoint)
	{
		GameObject fracturedPrefab = instance.fracturedPrefab;
		GameObject smokeParticle = instance.smokeParticle;
		AudioClip shatter = instance.shatter;
		Vector3 position = instance.transform.position;
		Quaternion rotation = instance.transform.rotation;
		Object.Destroy(instance.gameObject);
		if ((bool)smokeParticle)
		{
			Vector3 position2 = new Vector3(position.x, position.y + 1f, position.z);
			smoke = Object.Instantiate(smokeParticle, position2, rotation);
		}
		AudioSource.PlayClipAtPoint(shatter, position);
		GameObject gameObject = Object.Instantiate(fracturedPrefab, position, rotation);
		gameObject.name = "FracturedClone";
		Rigidbody[] componentsInChildren = gameObject.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rigidbody in componentsInChildren)
		{
			rigidbody.AddForceAtPosition(rayDirection * impactForce, hitPoint, ForceMode.Impulse);
		}
		StartCoroutine(removeDebris(gameObject, removeDebrisDelay));
		if ((bool)smoke)
		{
			Object.Destroy(smoke, 2f);
		}
	}

	private IEnumerator delay(float secs)
	{
		inCycle = true;
		yield return new WaitForSeconds(secs);
		inCycle = false;
	}

	private IEnumerator removeDebris(GameObject go, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Object.Destroy(go);
	}
}
