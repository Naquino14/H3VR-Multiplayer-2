using UnityEngine;

public class Destruct : MonoBehaviour
{
	public GameObject fracturedPrefab;

	public GameObject smokeParticle;

	public AudioClip shatter;

	private void Start()
	{
		if (fracturedPrefab == null)
		{
			Debug.LogError("Fractured prefab not assigned for object: " + base.gameObject.name);
		}
	}
}
