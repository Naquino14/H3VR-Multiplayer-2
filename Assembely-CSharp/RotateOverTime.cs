using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
	public Vector3 angularVelocity = Vector3.forward * 45f;

	private void Update()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles + angularVelocity * Time.deltaTime;
		localEulerAngles.x = Mathf.Repeat(localEulerAngles.x, 360f);
		localEulerAngles.y = Mathf.Repeat(localEulerAngles.y, 360f);
		localEulerAngles.z = Mathf.Repeat(localEulerAngles.z, 360f);
		base.transform.localEulerAngles = localEulerAngles;
	}
}
