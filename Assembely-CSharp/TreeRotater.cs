using UnityEngine;

public class TreeRotater : MonoBehaviour
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
		base.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
	}
}
