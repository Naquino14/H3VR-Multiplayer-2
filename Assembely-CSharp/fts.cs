using System;
using UnityEngine;

public class fts
{
	public static bool IsZero(double d)
	{
		return d > -1E-09 && d < 1E-09;
	}

	public static int SolveQuadric(double c0, double c1, double c2, out double s0, out double s1)
	{
		s0 = double.NaN;
		s1 = double.NaN;
		double num = c1 / (2.0 * c0);
		double num2 = c2 / c0;
		double num3 = num * num - num2;
		if (IsZero(num3))
		{
			s0 = 0.0 - num;
			return 1;
		}
		if (num3 < 0.0)
		{
			return 0;
		}
		double num4 = Math.Sqrt(num3);
		s0 = num4 - num;
		s1 = 0.0 - num4 - num;
		return 2;
	}

	public static int SolveCubic(double c0, double c1, double c2, double c3, out double s0, out double s1, out double s2)
	{
		s0 = double.NaN;
		s1 = double.NaN;
		s2 = double.NaN;
		double num = c1 / c0;
		double num2 = c2 / c0;
		double num3 = c3 / c0;
		double num4 = num * num;
		double num5 = 0.33333333333333331 * (-0.33333333333333331 * num4 + num2);
		double num6 = 0.5 * (2.0 / 27.0 * num * num4 - 0.33333333333333331 * num * num2 + num3);
		double num7 = num5 * num5 * num5;
		double num8 = num6 * num6 + num7;
		int num9;
		if (IsZero(num8))
		{
			if (IsZero(num6))
			{
				s0 = 0.0;
				num9 = 1;
			}
			else
			{
				double num10 = Math.Pow(0.0 - num6, 0.33333333333333331);
				s0 = 2.0 * num10;
				s1 = 0.0 - num10;
				num9 = 2;
			}
		}
		else if (num8 < 0.0)
		{
			double num11 = 0.33333333333333331 * Math.Acos((0.0 - num6) / Math.Sqrt(0.0 - num7));
			double num12 = 2.0 * Math.Sqrt(0.0 - num5);
			s0 = num12 * Math.Cos(num11);
			s1 = (0.0 - num12) * Math.Cos(num11 + Math.PI / 3.0);
			s2 = (0.0 - num12) * Math.Cos(num11 - Math.PI / 3.0);
			num9 = 3;
		}
		else
		{
			double num13 = Math.Sqrt(num8);
			double num14 = Math.Pow(num13 - num6, 0.33333333333333331);
			double num15 = 0.0 - Math.Pow(num13 + num6, 0.33333333333333331);
			s0 = num14 + num15;
			num9 = 1;
		}
		double num16 = 0.33333333333333331 * num;
		if (num9 > 0)
		{
			s0 -= num16;
		}
		if (num9 > 1)
		{
			s1 -= num16;
		}
		if (num9 > 2)
		{
			s2 -= num16;
		}
		return num9;
	}

	public static int SolveQuartic(double c0, double c1, double c2, double c3, double c4, out double s0, out double s1, out double s2, out double s3)
	{
		s0 = double.NaN;
		s1 = double.NaN;
		s2 = double.NaN;
		s3 = double.NaN;
		double[] array = new double[4];
		double num = c1 / c0;
		double num2 = c2 / c0;
		double num3 = c3 / c0;
		double num4 = c4 / c0;
		double num5 = num * num;
		double num6 = -0.375 * num5 + num2;
		double num7 = 0.125 * num5 * num - 0.5 * num * num2 + num3;
		double num8 = -3.0 / 256.0 * num5 * num5 + 0.0625 * num5 * num2 - 0.25 * num * num3 + num4;
		int num9;
		if (IsZero(num8))
		{
			array[3] = num7;
			array[2] = num6;
			array[1] = 0.0;
			array[0] = 1.0;
			num9 = SolveCubic(array[0], array[1], array[2], array[3], out s0, out s1, out s2);
		}
		else
		{
			array[3] = 0.5 * num8 * num6 - 0.125 * num7 * num7;
			array[2] = 0.0 - num8;
			array[1] = -0.5 * num6;
			array[0] = 1.0;
			SolveCubic(array[0], array[1], array[2], array[3], out s0, out s1, out s2);
			double num10 = s0;
			double num11 = num10 * num10 - num8;
			double num12 = 2.0 * num10 - num6;
			if (IsZero(num11))
			{
				num11 = 0.0;
			}
			else
			{
				if (!(num11 > 0.0))
				{
					return 0;
				}
				num11 = Math.Sqrt(num11);
			}
			if (IsZero(num12))
			{
				num12 = 0.0;
			}
			else
			{
				if (!(num12 > 0.0))
				{
					return 0;
				}
				num12 = Math.Sqrt(num12);
			}
			array[2] = num10 - num11;
			array[1] = ((!(num7 < 0.0)) ? num12 : (0.0 - num12));
			array[0] = 1.0;
			num9 = SolveQuadric(array[0], array[1], array[2], out s0, out s1);
			array[2] = num10 + num11;
			array[1] = ((!(num7 < 0.0)) ? (0.0 - num12) : num12);
			array[0] = 1.0;
			if (num9 == 0)
			{
				num9 += SolveQuadric(array[0], array[1], array[2], out s0, out s1);
			}
			if (num9 == 1)
			{
				num9 += SolveQuadric(array[0], array[1], array[2], out s1, out s2);
			}
			if (num9 == 2)
			{
				num9 += SolveQuadric(array[0], array[1], array[2], out s2, out s3);
			}
		}
		double num13 = 0.25 * num;
		if (num9 > 0)
		{
			s0 -= num13;
		}
		if (num9 > 1)
		{
			s1 -= num13;
		}
		if (num9 > 2)
		{
			s2 -= num13;
		}
		if (num9 > 3)
		{
			s3 -= num13;
		}
		return num9;
	}

	public static float ballistic_range(float speed, float gravity, float initial_height)
	{
		float f = (float)Math.PI / 4f;
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		return speed * num / gravity * (speed * num2 + Mathf.Sqrt(speed * speed * num2 * num2 + 2f * gravity * initial_height));
	}

	public static int solve_ballistic_arc(Vector3 proj_pos, float proj_speed, Vector3 target, float gravity, out Vector3 s0, out Vector3 s1)
	{
		s0 = Vector3.zero;
		s1 = Vector3.zero;
		Vector3 vector = target - proj_pos;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		float magnitude = vector2.magnitude;
		float num = proj_speed * proj_speed;
		float num2 = proj_speed * proj_speed * proj_speed * proj_speed;
		float y = vector.y;
		float num3 = magnitude;
		float x = gravity * num3;
		float num4 = num2 - gravity * (gravity * num3 * num3 + 2f * y * num);
		if (num4 < 0f)
		{
			return 0;
		}
		num4 = Mathf.Sqrt(num4);
		float num5 = Mathf.Atan2(num - num4, x);
		float num6 = Mathf.Atan2(num + num4, x);
		int num7 = ((num5 == num6) ? 1 : 2);
		Vector3 normalized = vector2.normalized;
		s0 = normalized * Mathf.Cos(num5) * proj_speed + Vector3.up * Mathf.Sin(num5) * proj_speed;
		if (num7 > 1)
		{
			s1 = normalized * Mathf.Cos(num6) * proj_speed + Vector3.up * Mathf.Sin(num6) * proj_speed;
		}
		return num7;
	}

	public static int solve_ballistic_arc(Vector3 proj_pos, float proj_speed, Vector3 target_pos, Vector3 target_velocity, float gravity, out Vector3 s0, out Vector3 s1)
	{
		s0 = Vector3.zero;
		s1 = Vector3.zero;
		double num = gravity;
		double num2 = proj_pos.x;
		double num3 = proj_pos.y;
		double num4 = proj_pos.z;
		double num5 = target_pos.x;
		double num6 = target_pos.y;
		double num7 = target_pos.z;
		double num8 = target_velocity.x;
		double num9 = target_velocity.y;
		double num10 = target_velocity.z;
		double num11 = proj_speed;
		double num12 = num5 - num2;
		double num13 = num7 - num4;
		double num14 = num6 - num3;
		double num15 = -0.5 * num;
		double c = num15 * num15;
		double c2 = 2.0 * num9 * num15;
		double c3 = num9 * num9 + 2.0 * num14 * num15 - num11 * num11 + num8 * num8 + num10 * num10;
		double c4 = 2.0 * num14 * num9 + 2.0 * num12 * num8 + 2.0 * num13 * num10;
		double c5 = num14 * num14 + num12 * num12 + num13 * num13;
		double[] array = new double[4];
		int num16 = SolveQuartic(c, c2, c3, c4, c5, out array[0], out array[1], out array[2], out array[3]);
		Array.Sort(array);
		Vector3[] array2 = new Vector3[2];
		int num17 = 0;
		for (int i = 0; i < num16; i++)
		{
			if (num17 >= 2)
			{
				break;
			}
			double num18 = array[i];
			if (!(num18 <= 0.0))
			{
				array2[num17].x = (float)((num12 + num8 * num18) / num18);
				array2[num17].y = (float)((num14 + num9 * num18 - num15 * num18 * num18) / num18);
				array2[num17].z = (float)((num13 + num10 * num18) / num18);
				num17++;
			}
		}
		if (num17 > 0)
		{
			s0 = array2[0];
		}
		if (num17 > 1)
		{
			s1 = array2[1];
		}
		return num17;
	}

	public static bool solve_ballistic_arc_lateral(Vector3 proj_pos, float lateral_speed, Vector3 target_pos, float max_height, out Vector3 fire_velocity, out float gravity)
	{
		fire_velocity = Vector3.zero;
		gravity = float.NaN;
		Vector3 vector = target_pos - proj_pos;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		float magnitude = vector2.magnitude;
		if (magnitude == 0f)
		{
			return false;
		}
		float num = magnitude / lateral_speed;
		fire_velocity = vector2.normalized * lateral_speed;
		float y = proj_pos.y;
		float y2 = target_pos.y;
		gravity = -4f * (y - 2f * max_height + y2) / (num * num);
		fire_velocity.y = (0f - (3f * y - 4f * max_height + y2)) / num;
		return true;
	}

	public static bool solve_ballistic_arc_lateral(Vector3 proj_pos, float lateral_speed, Vector3 target, Vector3 target_velocity, float max_height_offset, out Vector3 fire_velocity, out float gravity, out Vector3 impact_point)
	{
		fire_velocity = Vector3.zero;
		gravity = 0f;
		impact_point = Vector3.zero;
		Vector3 vector = new Vector3(target_velocity.x, 0f, target_velocity.z);
		Vector3 vector2 = target - proj_pos;
		vector2.y = 0f;
		float num = Vector3.Dot(vector, vector) - lateral_speed * lateral_speed;
		float num2 = 2f * Vector3.Dot(vector2, vector);
		float num3 = Vector3.Dot(vector2, vector2);
		double s;
		double s2;
		int num4 = SolveQuadric(num, num2, num3, out s, out s2);
		bool flag = num4 > 0 && s > 0.0;
		bool flag2 = num4 > 1 && s2 > 0.0;
		if (!flag && !flag2)
		{
			return false;
		}
		float num5 = ((!flag || !flag2) ? ((!flag) ? ((float)s2) : ((float)s)) : Mathf.Min((float)s, (float)s2));
		impact_point = target + target_velocity * num5;
		Vector3 vector3 = impact_point - proj_pos;
		fire_velocity = new Vector3(vector3.x, 0f, vector3.z).normalized * lateral_speed;
		float y = proj_pos.y;
		float num6 = Mathf.Max(proj_pos.y, impact_point.y) + max_height_offset;
		float y2 = impact_point.y;
		gravity = -4f * (y - 2f * num6 + y2) / (num5 * num5);
		fire_velocity.y = (0f - (3f * y - 4f * num6 + y2)) / num5;
		return true;
	}
}
