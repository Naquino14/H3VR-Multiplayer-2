using UnityEngine;

public class TempMissle : MonoBehaviour
{
	public float TurnSpeed = 180f;

	public float LinearSpeed = 10f;

	public Transform Target;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 target = Target.position - base.transform.position;
		Vector3 forward = Vector3.RotateTowards(base.transform.forward, target, TurnSpeed * Time.deltaTime, 1f);
		base.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
		float magnitude = target.magnitude;
		Vector3 vector = base.transform.forward * LinearSpeed * Time.deltaTime;
		vector = Vector3.ClampMagnitude(vector, magnitude);
		base.transform.position += vector;
	}
}
