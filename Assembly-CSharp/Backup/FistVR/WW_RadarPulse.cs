// Decompiled with JetBrains decompiler
// Type: FistVR.WW_RadarPulse
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class WW_RadarPulse : MonoBehaviour
  {
    public AnimationCurve XZSize;
    public AnimationCurve YSize;
    private float m_pulseTick;
    private float m_pulseSPeed = 0.4f;

    private void Start()
    {
    }

    private void Update()
    {
      this.m_pulseTick += Time.deltaTime * this.m_pulseSPeed;
      float num = this.XZSize.Evaluate(this.m_pulseTick);
      float y = this.YSize.Evaluate(this.m_pulseTick);
      this.transform.localScale = new Vector3(num, y, num);
      if ((double) this.m_pulseTick < 1.0)
        return;
      Object.Destroy((Object) this.gameObject);
    }
  }
}
