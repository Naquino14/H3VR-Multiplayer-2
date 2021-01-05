// Decompiled with JetBrains decompiler
// Type: FistVR.AffixedFlashlight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AffixedFlashlight : FVRInteractiveObject
  {
    private bool IsOn;
    public GameObject LightParts;
    public AudioEvent AudEvent_LaserOnClip;
    public AudioEvent AudEvent_LaserOffClip;
    public Light FlashlightLight;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.ToggleOn();
    }

    private void ToggleOn()
    {
      this.IsOn = !this.IsOn;
      this.LightParts.SetActive(this.IsOn);
      if (this.IsOn)
        this.FlashlightLight.intensity = !GM.CurrentSceneSettings.IsSceneLowLight ? 0.5f : 2f;
      if (this.IsOn)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_LaserOnClip, this.transform.position);
      else
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_LaserOffClip, this.transform.position);
    }
  }
}
