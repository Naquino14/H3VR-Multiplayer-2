using UnityEngine;

public static class WhunkSphereMapping
{
	public static float GetSignedAngle(Vector3 from, Vector3 to, Vector3 axis)
	{
		from = Vector3.ProjectOnPlane(from, axis).normalized;
		Vector3 rhs = Vector3.Cross(axis, to);
		float num = Mathf.Sign(Vector3.Dot(from, rhs));
		float num2 = Vector3.Angle(from, to);
		return num2 * num;
	}

	public static Vector2 PositionToUVSpherical(Vector3 localPosition, Vector3 axis, Vector3 seamDirection)
	{
		Vector3 normalized = localPosition.normalized;
		seamDirection = -seamDirection;
		axis = -axis;
		return new Vector2(GetSignedAngle(normalized, seamDirection, axis) / 360f + 0.5f, Vector3.Angle(normalized, axis) / 180f);
	}
}
