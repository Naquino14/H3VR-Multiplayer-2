// Decompiled with JetBrains decompiler
// Type: FistVR.Flashlight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Flashlight : FVRPhysicalObject
  {
    public bool IsOn;
    public GameObject LightParts;
    public AudioSource Aud;
    public AudioClip LaserOnClip;
    public AudioClip LaserOffClip;
    public AlloyAreaLight FlashlightLight;
    public Transform[] Poses;
    private int m_curPose;

    protected override void Awake()
    {
      base.Awake();
      this.LightParts.SetActive(this.IsOn);
      if (this.IsOn)
        this.FlashlightLight.Intensity = !GM.CurrentSceneSettings.IsSceneLowLight ? 0.5f : 0.9f;
      if (this.IsOn)
        this.Aud.PlayOneShot(this.LaserOnClip, 1f);
      else
        this.Aud.PlayOneShot(this.LaserOffClip, 1f);
    }

    private void CyclePose()
    {
      ++this.m_curPose;
      if (this.m_curPose >= this.Poses.Length)
        this.m_curPose = 0;
      this.PoseOverride.localPosition = this.Poses[this.m_curPose].localPosition;
      this.PoseOverride.localRotation = this.Poses[this.m_curPose].localRotation;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
          this.ToggleOn();
      }
      else if (hand.Input.TouchpadDown)
        this.ToggleOn();
      if (hand.Input.TriggerDown && this.m_hasTriggeredUpSinceBegin)
        this.CyclePose();
      base.UpdateInteraction(hand);
    }

    public void ToggleOn()
    {
      this.IsOn = !this.IsOn;
      this.LightParts.SetActive(this.IsOn);
      if (this.IsOn)
        this.FlashlightLight.Intensity = !GM.CurrentSceneSettings.IsSceneLowLight ? 0.5f : 0.9f;
      if (this.IsOn)
        this.Aud.PlayOneShot(this.LaserOnClip, 1f);
      else
        this.Aud.PlayOneShot(this.LaserOffClip, 1f);
    }
  }
}
