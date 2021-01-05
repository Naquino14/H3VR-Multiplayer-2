using UnityEngine;

public class SpinnyHat : MonoBehaviour
{
	private float r;

	private void Start()
	{
	}

	private void Update()
	{
		r += Time.deltaTime * 180f;
		r = Mathf.Repeat(r, 360f);
		base.transform.localEulerAngles = new Vector3(0f, r, 0f);
	}
}
