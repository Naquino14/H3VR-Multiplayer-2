using UnityEngine;

public class rotate : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(0.2f, 0f, 0f);
	}
}
