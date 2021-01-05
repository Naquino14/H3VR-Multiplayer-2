// Decompiled with JetBrains decompiler
// Type: FistVR.TacticalFlashlight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TacticalFlashlight : FVRFireArmAttachmentInterface
  {
    private bool IsOn;
    public GameObject LightParts;
    public AudioEvent AudEvent_LaserOnClip;
    public AudioEvent AudEvent_LaserOffClip;
    public AlloyAreaLight FlashlightLight;
    public AudioSource BackUpAudio;

    public override void OnAttach() => base.OnAttach();

    public override void OnDetach()
    {
      base.OnDetach();
      this.IsOn = false;
      this.LightParts.SetActive(this.IsOn);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
          this.ToggleOn();
      }
      else if (hand.Input.TouchpadDown && (double) touchpadAxes.magnitude > 0.25 && (double) Vector2.Angle(touchpadAxes, Vector2.up) <= 45.0)
        this.ToggleOn();
      base.UpdateInteraction(hand);
    }

    private void ToggleOn()
    {
      this.IsOn = !this.IsOn;
      this.LightParts.SetActive(this.IsOn);
      if (this.IsOn)
        this.FlashlightLight.Intensity = !GM.CurrentSceneSettings.IsSceneLowLight ? 0.5f : 2f;
      if (this.IsOn)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_LaserOnClip, this.transform.position);
      else
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_LaserOffClip, this.transform.position);
    }
  }
}
