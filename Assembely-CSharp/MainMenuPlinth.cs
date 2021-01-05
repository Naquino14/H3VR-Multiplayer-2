using UnityEngine;

public class MainMenuPlinth : MonoBehaviour
{
	private Vector3 startPos;

	private float startRot;

	private float sinTick;

	private float sinSpeed;

	private void Awake()
	{
		startPos = base.transform.position;
		sinTick = Random.Range(0f, 10f);
		startRot = base.transform.localEulerAngles.y;
		sinSpeed = Random.Range(0.02f, 0.06f);
	}

	private void Start()
	{
	}

	private void Update()
	{
		sinTick += Time.deltaTime * sinSpeed;
		Vector3 position = startPos;
		position.x = startPos.x + Mathf.PerlinNoise(Time.time * 0.05f, startPos.y);
		position.z = startPos.z + Mathf.PerlinNoise(Time.time * 0.08f, startPos.y + 2f);
		position.y = startPos.y + Mathf.Sin(sinTick) * 50f;
		base.transform.position = position;
		startRot += (Mathf.PerlinNoise(Time.time * 0.1f, startPos.y) - 0.5f) * Time.deltaTime * 40f;
		base.transform.localEulerAngles = new Vector3(0f, startRot, 0f);
	}
}
