using UnityEngine;

public class RotationAnimation : MonoBehaviour
{
	public enum RotationVectorEnum
	{
		X,
		Y,
		Z
	}

	public AnimationCurve rotationCurve;

	public RotationVectorEnum RotationVector = RotationVectorEnum.Z;

	public float speed = 0.25f;

	public float intensity = 2f;

	public float timeOffset;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		Vector3 vector = Vector3.zero;
		if (RotationVector == RotationVectorEnum.X)
		{
			vector = Vector3.right;
		}
		else if (RotationVector == RotationVectorEnum.Y)
		{
			vector = Vector3.up;
		}
		else if (RotationVector == RotationVectorEnum.Z)
		{
			vector = Vector3.forward;
		}
		float num = (Time.fixedTime + timeOffset) * speed;
		int num2 = 1;
		if (Mathf.Floor(num) % 2f == 0f)
		{
			num2 = -1;
		}
		base.transform.Rotate((float)num2 * intensity * vector * rotationCurve.Evaluate(num));
	}
}
