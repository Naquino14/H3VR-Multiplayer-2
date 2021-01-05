// Decompiled with JetBrains decompiler
// Type: FistVR.BurningFlare
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BurningFlare : MonoBehaviour
  {
    public Color FlareColor;
    public float Life;
    private float m_lifeLeft;
    public AnimationCurve IntensityOverTimeCurve;
    public AnimationCurve RadiusOverTimeCurve;
    private float m_timeTilFlicker;

    private void Awake() => this.m_lifeLeft = this.Life;

    private void Update()
    {
      this.m_lifeLeft -= Time.deltaTime;
      if ((double) this.m_lifeLeft <= 0.0)
        return;
      this.m_timeTilFlicker -= Time.deltaTime;
      if ((double) this.m_timeTilFlicker > 0.0)
        return;
      this.m_timeTilFlicker = Random.Range(0.03f, 0.1f);
      if (GM.CurrentSceneSettings.IsSceneLowLight)
        FXM.InitiateMuzzleFlash(this.transform.position + Vector3.up * 0.1f, this.transform.forward, this.IntensityOverTimeCurve.Evaluate(this.Life - this.m_lifeLeft) * 12f, this.FlareColor, this.RadiusOverTimeCurve.Evaluate(this.Life - this.m_lifeLeft) * 80f);
      else
        FXM.InitiateMuzzleFlashLowPriority(this.transform.position + Vector3.up * 0.1f, this.transform.forward, this.IntensityOverTimeCurve.Evaluate(this.Life - this.m_lifeLeft) * 0.3f, this.FlareColor, this.RadiusOverTimeCurve.Evaluate(this.Life - this.m_lifeLeft) * 0.3f);
    }
  }
}
