// Decompiled with JetBrains decompiler
// Type: QuatAveraging
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public static class QuatAveraging
{
  public static Quaternion AverageQuaternion(
    ref Vector4 cumulative,
    Quaternion newRotation,
    Quaternion firstRotation,
    int addAmount)
  {
    if (!QuatAveraging.AreQuaternionsClose(newRotation, firstRotation))
      newRotation = QuatAveraging.InverseSignQuaternion(newRotation);
    float num = 1f / (float) addAmount;
    cumulative.w += newRotation.w;
    float w = cumulative.w * num;
    cumulative.x += newRotation.x;
    float x = cumulative.x * num;
    cumulative.y += newRotation.y;
    float y = cumulative.y * num;
    cumulative.z += newRotation.z;
    float z = cumulative.z * num;
    return QuatAveraging.NormalizeQuaternion(x, y, z, w);
  }

  public static Quaternion NormalizeQuaternion(float x, float y, float z, float w)
  {
    float num = (float) (1.0 / ((double) w * (double) w + (double) x * (double) x + (double) y * (double) y + (double) z * (double) z));
    w *= num;
    x *= num;
    y *= num;
    z *= num;
    return new Quaternion(x, y, z, w);
  }

  public static Quaternion InverseSignQuaternion(Quaternion q) => new Quaternion(-q.x, -q.y, -q.z, -q.w);

  public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2) => (double) Quaternion.Dot(q1, q2) >= 0.0;
}
