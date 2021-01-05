using UnityEngine;

public class RandomPitchAwake : MonoBehaviour
{
	public AudioSource aud;

	public float min = 0.9f;

	public float max = 1.1f;

	private void Awake()
	{
		aud.pitch = Random.Range(min, max);
	}

	private void Update()
	{
	}
}
