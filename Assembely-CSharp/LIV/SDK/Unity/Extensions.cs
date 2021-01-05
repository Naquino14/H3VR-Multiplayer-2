using UnityEngine;

namespace LIV.SDK.Unity
{
	public static class Extensions
	{
		private static float _copysign(float sizeval, float signval)
		{
			return ((int)Mathf.Sign(signval) != 1) ? (0f - Mathf.Abs(sizeval)) : Mathf.Abs(sizeval);
		}

		public static Quaternion GetRotation(this Matrix4x4 matrix)
		{
			Quaternion quaternion = default(Quaternion);
			quaternion.w = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 + matrix.m11 + matrix.m22)) / 2f;
			quaternion.x = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 - matrix.m11 - matrix.m22)) / 2f;
			quaternion.y = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 + matrix.m11 - matrix.m22)) / 2f;
			quaternion.z = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 - matrix.m11 + matrix.m22)) / 2f;
			Quaternion result = quaternion;
			result.x = _copysign(result.x, matrix.m21 - matrix.m12);
			result.y = _copysign(result.y, matrix.m02 - matrix.m20);
			result.z = _copysign(result.z, matrix.m10 - matrix.m01);
			return result;
		}

		public static Vector3 GetPosition(this Matrix4x4 matrix)
		{
			float m = matrix.m03;
			float m2 = matrix.m13;
			float m3 = matrix.m23;
			return new Vector3(m, m2, m3);
		}
	}
}
