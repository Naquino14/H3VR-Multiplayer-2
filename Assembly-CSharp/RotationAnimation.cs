// Decompiled with JetBrains decompiler
// Type: RotationAnimation
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class RotationAnimation : MonoBehaviour
{
  public AnimationCurve rotationCurve;
  public RotationAnimation.RotationVectorEnum RotationVector = RotationAnimation.RotationVectorEnum.Z;
  public float speed = 0.25f;
  public float intensity = 2f;
  public float timeOffset;

  private void Start()
  {
  }

  private void FixedUpdate()
  {
    Vector3 vector3 = Vector3.zero;
    if (this.RotationVector == RotationAnimation.RotationVectorEnum.X)
      vector3 = Vector3.right;
    else if (this.RotationVector == RotationAnimation.RotationVectorEnum.Y)
      vector3 = Vector3.up;
    else if (this.RotationVector == RotationAnimation.RotationVectorEnum.Z)
      vector3 = Vector3.forward;
    float num1 = (Time.fixedTime + this.timeOffset) * this.speed;
    int num2 = 1;
    if ((double) Mathf.Floor(num1) % 2.0 == 0.0)
      num2 = -1;
    this.transform.Rotate((float) num2 * this.intensity * vector3 * this.rotationCurve.Evaluate(num1));
  }

  public enum RotationVectorEnum
  {
    X,
    Y,
    Z,
  }
}
