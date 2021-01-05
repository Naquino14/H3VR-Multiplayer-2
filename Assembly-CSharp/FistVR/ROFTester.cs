// Decompiled with JetBrains decompiler
// Type: FistVR.ROFTester
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ROFTester : MonoBehaviour, IFVRDamageable
  {
    public List<float> ShotTimes = new List<float>();
    public float RPM;

    public void Update()
    {
      if (Input.GetKey(KeyCode.R))
        this.ShotTimes.Clear();
      if (!Input.GetKey(KeyCode.P))
        return;
      Debug.Log((object) ("ROF: " + (object) this.RPM));
    }

    public void Damage(FistVR.Damage dam)
    {
      this.ShotTimes.Add(Time.time);
      this.UpdateROF();
    }

    public void UpdateROF() => this.RPM = (float) this.ShotTimes.Count / (this.ShotTimes[this.ShotTimes.Count - 1] - this.ShotTimes[0]) * 60f;
  }
}
