using UnityEngine;

public class BushPrepper : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	[ContextMenu("rot")]
	public void Rot()
	{
		float x = Random.Range(-15f, 15f);
		float z = Random.Range(-15f, 15f);
		base.transform.eulerAngles = new Vector3(x, Random.Range(0f, 360f), z);
		float num = Random.Range(1f, 2f);
		base.transform.localScale = new Vector3(num, num, num);
	}
}
