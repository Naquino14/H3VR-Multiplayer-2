// Decompiled with JetBrains decompiler
// Type: fts
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

public class fts
{
  public static bool IsZero(double d) => d > -1E-09 && d < 1E-09;

  public static int SolveQuadric(double c0, double c1, double c2, out double s0, out double s1)
  {
    s0 = double.NaN;
    s1 = double.NaN;
    double num1 = c1 / (2.0 * c0);
    double num2 = c2 / c0;
    double d = num1 * num1 - num2;
    if (fts.IsZero(d))
    {
      s0 = -num1;
      return 1;
    }
    if (d < 0.0)
      return 0;
    double num3 = Math.Sqrt(d);
    s0 = num3 - num1;
    s1 = -num3 - num1;
    return 2;
  }

  public static int SolveCubic(
    double c0,
    double c1,
    double c2,
    double c3,
    out double s0,
    out double s1,
    out double s2)
  {
    s0 = double.NaN;
    s1 = double.NaN;
    s2 = double.NaN;
    double num1 = c1 / c0;
    double num2 = c2 / c0;
    double num3 = c3 / c0;
    double num4 = num1 * num1;
    double num5 = 1.0 / 3.0 * (-1.0 / 3.0 * num4 + num2);
    double d1 = 0.5 * (2.0 / 27.0 * num1 * num4 - 1.0 / 3.0 * num1 * num2 + num3);
    double num6 = num5 * num5 * num5;
    double d2 = d1 * d1 + num6;
    int num7;
    if (fts.IsZero(d2))
    {
      if (fts.IsZero(d1))
      {
        s0 = 0.0;
        num7 = 1;
      }
      else
      {
        double num8 = Math.Pow(-d1, 1.0 / 3.0);
        s0 = 2.0 * num8;
        s1 = -num8;
        num7 = 2;
      }
    }
    else if (d2 < 0.0)
    {
      double d3 = 1.0 / 3.0 * Math.Acos(-d1 / Math.Sqrt(-num6));
      double num8 = 2.0 * Math.Sqrt(-num5);
      s0 = num8 * Math.Cos(d3);
      s1 = -num8 * Math.Cos(d3 + Math.PI / 3.0);
      s2 = -num8 * Math.Cos(d3 - Math.PI / 3.0);
      num7 = 3;
    }
    else
    {
      double num8 = Math.Sqrt(d2);
      double num9 = Math.Pow(num8 - d1, 1.0 / 3.0);
      double num10 = -Math.Pow(num8 + d1, 1.0 / 3.0);
      s0 = num9 + num10;
      num7 = 1;
    }
    double num11 = 1.0 / 3.0 * num1;
    if (num7 > 0)
      s0 -= num11;
    if (num7 > 1)
      s1 -= num11;
    if (num7 > 2)
      s2 -= num11;
    return num7;
  }

  public static int SolveQuartic(
    double c0,
    double c1,
    double c2,
    double c3,
    double c4,
    out double s0,
    out double s1,
    out double s2,
    out double s3)
  {
    s0 = double.NaN;
    s1 = double.NaN;
    s2 = double.NaN;
    s3 = double.NaN;
    double[] numArray = new double[4];
    double num1 = c1 / c0;
    double num2 = c2 / c0;
    double num3 = c3 / c0;
    double num4 = c4 / c0;
    double num5 = num1 * num1;
    double num6 = -0.375 * num5 + num2;
    double num7 = 0.125 * num5 * num1 - 0.5 * num1 * num2 + num3;
    double d1 = -3.0 / 256.0 * num5 * num5 + 1.0 / 16.0 * num5 * num2 - 0.25 * num1 * num3 + num4;
    int num8;
    if (fts.IsZero(d1))
    {
      numArray[3] = num7;
      numArray[2] = num6;
      numArray[1] = 0.0;
      numArray[0] = 1.0;
      num8 = fts.SolveCubic(numArray[0], numArray[1], numArray[2], numArray[3], out s0, out s1, out s2);
    }
    else
    {
      numArray[3] = 0.5 * d1 * num6 - 0.125 * num7 * num7;
      numArray[2] = -d1;
      numArray[1] = -0.5 * num6;
      numArray[0] = 1.0;
      fts.SolveCubic(numArray[0], numArray[1], numArray[2], numArray[3], out s0, out s1, out s2);
      double num9 = s0;
      double d2 = num9 * num9 - d1;
      double d3 = 2.0 * num9 - num6;
      double num10;
      if (fts.IsZero(d2))
      {
        num10 = 0.0;
      }
      else
      {
        if (d2 <= 0.0)
          return 0;
        num10 = Math.Sqrt(d2);
      }
      double num11;
      if (fts.IsZero(d3))
      {
        num11 = 0.0;
      }
      else
      {
        if (d3 <= 0.0)
          return 0;
        num11 = Math.Sqrt(d3);
      }
      numArray[2] = num9 - num10;
      numArray[1] = num7 >= 0.0 ? num11 : -num11;
      numArray[0] = 1.0;
      num8 = fts.SolveQuadric(numArray[0], numArray[1], numArray[2], out s0, out s1);
      numArray[2] = num9 + num10;
      numArray[1] = num7 >= 0.0 ? -num11 : num11;
      numArray[0] = 1.0;
      if (num8 == 0)
        num8 += fts.SolveQuadric(numArray[0], numArray[1], numArray[2], out s0, out s1);
      if (num8 == 1)
        num8 += fts.SolveQuadric(numArray[0], numArray[1], numArray[2], out s1, out s2);
      if (num8 == 2)
        num8 += fts.SolveQuadric(numArray[0], numArray[1], numArray[2], out s2, out s3);
    }
    double num12 = 0.25 * num1;
    if (num8 > 0)
      s0 -= num12;
    if (num8 > 1)
      s1 -= num12;
    if (num8 > 2)
      s2 -= num12;
    if (num8 > 3)
      s3 -= num12;
    return num8;
  }

  public static float ballistic_range(float speed, float gravity, float initial_height)
  {
    float f = 0.7853982f;
    float num1 = Mathf.Cos(f);
    float num2 = Mathf.Sin(f);
    return (float) ((double) speed * (double) num1 / (double) gravity * ((double) speed * (double) num2 + (double) Mathf.Sqrt((float) ((double) speed * (double) speed * (double) num2 * (double) num2 + 2.0 * (double) gravity * (double) initial_height))));
  }

  public static int solve_ballistic_arc(
    Vector3 proj_pos,
    float proj_speed,
    Vector3 target,
    float gravity,
    out Vector3 s0,
    out Vector3 s1)
  {
    s0 = Vector3.zero;
    s1 = Vector3.zero;
    Vector3 vector3_1 = target - proj_pos;
    Vector3 vector3_2 = new Vector3(vector3_1.x, 0.0f, vector3_1.z);
    float magnitude = vector3_2.magnitude;
    float num1 = proj_speed * proj_speed;
    float num2 = proj_speed * proj_speed * proj_speed * proj_speed;
    float y = vector3_1.y;
    float num3 = magnitude;
    float x = gravity * num3;
    float f1 = num2 - gravity * (float) ((double) gravity * (double) num3 * (double) num3 + 2.0 * (double) y * (double) num1);
    if ((double) f1 < 0.0)
      return 0;
    float num4 = Mathf.Sqrt(f1);
    float f2 = Mathf.Atan2(num1 - num4, x);
    float f3 = Mathf.Atan2(num1 + num4, x);
    int num5 = (double) f2 == (double) f3 ? 1 : 2;
    Vector3 normalized = vector3_2.normalized;
    s0 = normalized * Mathf.Cos(f2) * proj_speed + Vector3.up * Mathf.Sin(f2) * proj_speed;
    if (num5 > 1)
      s1 = normalized * Mathf.Cos(f3) * proj_speed + Vector3.up * Mathf.Sin(f3) * proj_speed;
    return num5;
  }

  public static int solve_ballistic_arc(
    Vector3 proj_pos,
    float proj_speed,
    Vector3 target_pos,
    Vector3 target_velocity,
    float gravity,
    out Vector3 s0,
    out Vector3 s1)
  {
    s0 = Vector3.zero;
    s1 = Vector3.zero;
    double num1 = (double) gravity;
    double x1 = (double) proj_pos.x;
    double y1 = (double) proj_pos.y;
    double z1 = (double) proj_pos.z;
    double x2 = (double) target_pos.x;
    double y2 = (double) target_pos.y;
    double z2 = (double) target_pos.z;
    double x3 = (double) target_velocity.x;
    double y3 = (double) target_velocity.y;
    double z3 = (double) target_velocity.z;
    double num2 = (double) proj_speed;
    double num3 = x2 - x1;
    double num4 = z2 - z1;
    double num5 = y2 - y1;
    double num6 = -0.5 * num1;
    double c0 = num6 * num6;
    double c1 = 2.0 * y3 * num6;
    double c2 = y3 * y3 + 2.0 * num5 * num6 - num2 * num2 + x3 * x3 + z3 * z3;
    double c3 = 2.0 * num5 * y3 + 2.0 * num3 * x3 + 2.0 * num4 * z3;
    double c4 = num5 * num5 + num3 * num3 + num4 * num4;
    double[] array = new double[4];
    int num7 = fts.SolveQuartic(c0, c1, c2, c3, c4, out array[0], out array[1], out array[2], out array[3]);
    Array.Sort<double>(array);
    Vector3[] vector3Array = new Vector3[2];
    int index1 = 0;
    for (int index2 = 0; index2 < num7 && index1 < 2; ++index2)
    {
      double num8 = array[index2];
      if (num8 > 0.0)
      {
        vector3Array[index1].x = (float) ((num3 + x3 * num8) / num8);
        vector3Array[index1].y = (float) ((num5 + y3 * num8 - num6 * num8 * num8) / num8);
        vector3Array[index1].z = (float) ((num4 + z3 * num8) / num8);
        ++index1;
      }
    }
    if (index1 > 0)
      s0 = vector3Array[0];
    if (index1 > 1)
      s1 = vector3Array[1];
    return index1;
  }

  public static bool solve_ballistic_arc_lateral(
    Vector3 proj_pos,
    float lateral_speed,
    Vector3 target_pos,
    float max_height,
    out Vector3 fire_velocity,
    out float gravity)
  {
    fire_velocity = Vector3.zero;
    gravity = float.NaN;
    Vector3 vector3_1 = target_pos - proj_pos;
    Vector3 vector3_2 = new Vector3(vector3_1.x, 0.0f, vector3_1.z);
    float magnitude = vector3_2.magnitude;
    if ((double) magnitude == 0.0)
      return false;
    float num1 = magnitude / lateral_speed;
    fire_velocity = vector3_2.normalized * lateral_speed;
    float y1 = proj_pos.y;
    float num2 = max_height;
    float y2 = target_pos.y;
    gravity = (float) (-4.0 * ((double) y1 - 2.0 * (double) num2 + (double) y2) / ((double) num1 * (double) num1));
    fire_velocity.y = (float) -(3.0 * (double) y1 - 4.0 * (double) num2 + (double) y2) / num1;
    return true;
  }

  public static bool solve_ballistic_arc_lateral(
    Vector3 proj_pos,
    float lateral_speed,
    Vector3 target,
    Vector3 target_velocity,
    float max_height_offset,
    out Vector3 fire_velocity,
    out float gravity,
    out Vector3 impact_point)
  {
    fire_velocity = Vector3.zero;
    gravity = 0.0f;
    impact_point = Vector3.zero;
    Vector3 vector3_1 = new Vector3(target_velocity.x, 0.0f, target_velocity.z);
    Vector3 vector3_2 = target - proj_pos;
    vector3_2.y = 0.0f;
    double s0;
    double s1;
    int num1 = fts.SolveQuadric((double) (Vector3.Dot(vector3_1, vector3_1) - lateral_speed * lateral_speed), (double) (2f * Vector3.Dot(vector3_2, vector3_1)), (double) Vector3.Dot(vector3_2, vector3_2), out s0, out s1);
    bool flag1 = num1 > 0 && s0 > 0.0;
    bool flag2 = num1 > 1 && s1 > 0.0;
    if (!flag1 && !flag2)
      return false;
    float num2 = !flag1 || !flag2 ? (!flag1 ? (float) s1 : (float) s0) : Mathf.Min((float) s0, (float) s1);
    impact_point = target + target_velocity * num2;
    Vector3 vector3_3 = impact_point - proj_pos;
    fire_velocity = new Vector3(vector3_3.x, 0.0f, vector3_3.z).normalized * lateral_speed;
    float y1 = proj_pos.y;
    float num3 = Mathf.Max(proj_pos.y, impact_point.y) + max_height_offset;
    float y2 = impact_point.y;
    gravity = (float) (-4.0 * ((double) y1 - 2.0 * (double) num3 + (double) y2) / ((double) num2 * (double) num2));
    fire_velocity.y = (float) -(3.0 * (double) y1 - 4.0 * (double) num3 + (double) y2) / num2;
    return true;
  }
}
