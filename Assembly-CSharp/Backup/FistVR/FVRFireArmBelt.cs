// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmBelt
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFireArmBelt : MonoBehaviour
  {
    public FVRFirearmBeltDisplayData DisplayData;
    public AnimationCurve AngleFromUpToInOutLerp;
    private float cycler;
    public FVRPhysicalObject rb;

    private void Update()
    {
      Vector3 normalized = (this.transform.up - this.transform.right).normalized;
      float num1 = Vector3.Angle(normalized, Vector3.up);
      float num2 = 0.0f;
      if (this.rb.IsHeld)
      {
        num2 = (float) ((double) Vector3.Dot(this.rb.m_hand.Input.VelLinearWorld.normalized, normalized) * (double) this.rb.m_hand.Input.VelLinearWorld.magnitude * 4.0);
        Debug.Log((object) num2);
      }
      this.DisplayData.ChainInterpolatedInOut.SetInterp(this.DisplayData.Chain_In, this.DisplayData.Chain_Out, this.AngleFromUpToInOutLerp.Evaluate(num1 - num2));
      this.cycler -= Time.deltaTime * 10f;
      if ((double) this.cycler < 0.0)
        ++this.cycler;
      this.DisplayData.ChainInterpolated01.SetInterp(this.DisplayData.ChainInterpolatedInOut, this.cycler);
    }
  }
}
