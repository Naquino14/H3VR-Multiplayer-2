using UnityEngine;

public static class QuatAveraging
{
	public static Quaternion AverageQuaternion(ref Vector4 cumulative, Quaternion newRotation, Quaternion firstRotation, int addAmount)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		if (!AreQuaternionsClose(newRotation, firstRotation))
		{
			newRotation = InverseSignQuaternion(newRotation);
		}
		float num5 = 1f / (float)addAmount;
		cumulative.w += newRotation.w;
		num = cumulative.w * num5;
		cumulative.x += newRotation.x;
		num2 = cumulative.x * num5;
		cumulative.y += newRotation.y;
		num3 = cumulative.y * num5;
		cumulative.z += newRotation.z;
		num4 = cumulative.z * num5;
		return NormalizeQuaternion(num2, num3, num4, num);
	}

	public static Quaternion NormalizeQuaternion(float x, float y, float z, float w)
	{
		float num = 1f / (w * w + x * x + y * y + z * z);
		w *= num;
		x *= num;
		y *= num;
		z *= num;
		return new Quaternion(x, y, z, w);
	}

	public static Quaternion InverseSignQuaternion(Quaternion q)
	{
		return new Quaternion(0f - q.x, 0f - q.y, 0f - q.z, 0f - q.w);
	}

	public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2)
	{
		float num = Quaternion.Dot(q1, q2);
		if (num < 0f)
		{
			return false;
		}
		return true;
	}
}
