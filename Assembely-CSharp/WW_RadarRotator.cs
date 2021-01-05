using UnityEngine;

public class WW_RadarRotator : MonoBehaviour
{
	public Transform Reference;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 upwards = Vector3.ProjectOnPlane(Vector3.forward, Reference.forward);
		base.transform.rotation = Quaternion.LookRotation(Reference.forward, upwards);
	}
}
