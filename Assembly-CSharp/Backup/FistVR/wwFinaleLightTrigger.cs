// Decompiled with JetBrains decompiler
// Type: FistVR.wwFinaleLightTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwFinaleLightTrigger : MonoBehaviour
  {
    public wwFinaleManager Manager;
    public int LightTriggerIndex;
    public AudioSource Aud;
    public bool TriggersNormalPA;
    public bool TriggersSuppresedPA;
    public bool TriggersSilentPA;

    public void Awake() => this.Aud = this.GetComponent<AudioSource>();

    public void OnTriggerEnter(Collider col)
    {
      if (this.LightTriggerIndex >= 0)
      {
        this.Manager.SwitchToFinaleLight(this.LightTriggerIndex);
      }
      else
      {
        this.Manager.DisableAllFinaleLights();
        this.Manager.EnableOutdoorLight();
      }
      if (this.TriggersNormalPA)
        this.Manager.ParkManager.PASystem.EngageStandardMode();
      else if (this.TriggersSuppresedPA)
      {
        this.Manager.ParkManager.PASystem.EngageSuppressedMode();
      }
      else
      {
        if (!this.TriggersSilentPA)
          return;
        this.Manager.ParkManager.PASystem.EngageSilentMode();
      }
    }
  }
}
