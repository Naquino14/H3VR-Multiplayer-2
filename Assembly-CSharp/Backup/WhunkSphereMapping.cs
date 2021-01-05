// Decompiled with JetBrains decompiler
// Type: WhunkSphereMapping
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public static class WhunkSphereMapping
{
  public static float GetSignedAngle(Vector3 from, Vector3 to, Vector3 axis)
  {
    from = Vector3.ProjectOnPlane(from, axis).normalized;
    Vector3 rhs = Vector3.Cross(axis, to);
    float num = Mathf.Sign(Vector3.Dot(from, rhs));
    return Vector3.Angle(from, to) * num;
  }

  public static Vector2 PositionToUVSpherical(
    Vector3 localPosition,
    Vector3 axis,
    Vector3 seamDirection)
  {
    Vector3 normalized = localPosition.normalized;
    seamDirection = -seamDirection;
    axis = -axis;
    return new Vector2((float) ((double) WhunkSphereMapping.GetSignedAngle(normalized, seamDirection, axis) / 360.0 + 0.5), Vector3.Angle(normalized, axis) / 180f);
  }
}
