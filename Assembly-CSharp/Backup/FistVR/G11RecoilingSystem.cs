// Decompiled with JetBrains decompiler
// Type: FistVR.G11RecoilingSystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class G11RecoilingSystem : MonoBehaviour
  {
    private float m_displacement;
    private float m_lerp = 1f;
    public float CurveSpeed = 5f;
    public AnimationCurve RecoilCurve_Normal;
    public AnimationCurve RecoilCurve_Powerful;
    private bool isCurrentCurvePowerful;
    public Transform RecoilingPart;

    public void Recoil(bool isPowerful)
    {
      this.m_lerp = 0.0f;
      this.isCurrentCurvePowerful = isPowerful;
    }

    private void Update()
    {
      if ((double) this.m_lerp >= 1.0)
        return;
      this.m_lerp += Time.deltaTime * this.CurveSpeed;
      this.m_lerp = Mathf.Clamp(this.m_lerp, 0.0f, 1f);
      this.m_displacement = !this.isCurrentCurvePowerful ? this.RecoilCurve_Normal.Evaluate(this.m_lerp) : this.RecoilCurve_Powerful.Evaluate(this.m_lerp);
      this.RecoilingPart.localPosition = new Vector3(this.RecoilingPart.localPosition.x, this.RecoilingPart.localPosition.y, this.m_displacement);
    }
  }
}
