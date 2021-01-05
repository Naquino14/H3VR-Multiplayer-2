// Decompiled with JetBrains decompiler
// Type: LIV.SDK.Unity.Extensions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace LIV.SDK.Unity
{
  public static class Extensions
  {
    private static float _copysign(float sizeval, float signval) => (int) Mathf.Sign(signval) == 1 ? Mathf.Abs(sizeval) : -Mathf.Abs(sizeval);

    public static Quaternion GetRotation(this Matrix4x4 matrix)
    {
      Quaternion quaternion = new Quaternion()
      {
        w = Mathf.Sqrt(Mathf.Max(0.0f, 1f + matrix.m00 + matrix.m11 + matrix.m22)) / 2f,
        x = Mathf.Sqrt(Mathf.Max(0.0f, 1f + matrix.m00 - matrix.m11 - matrix.m22)) / 2f,
        y = Mathf.Sqrt(Mathf.Max(0.0f, 1f - matrix.m00 + matrix.m11 - matrix.m22)) / 2f,
        z = Mathf.Sqrt(Mathf.Max(0.0f, 1f - matrix.m00 - matrix.m11 + matrix.m22)) / 2f
      };
      quaternion.x = Extensions._copysign(quaternion.x, matrix.m21 - matrix.m12);
      quaternion.y = Extensions._copysign(quaternion.y, matrix.m02 - matrix.m20);
      quaternion.z = Extensions._copysign(quaternion.z, matrix.m10 - matrix.m01);
      return quaternion;
    }

    public static Vector3 GetPosition(this Matrix4x4 matrix) => new Vector3(matrix.m03, matrix.m13, matrix.m23);
  }
}
