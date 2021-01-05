// Decompiled with JetBrains decompiler
// Type: FistVR.LAM
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LAM : FVRFireArmAttachmentInterface
  {
    public LAM.LAMState LState;
    public AudioEvent AudEvent_LAMON;
    public AudioEvent AudEvent_LAMOFF;
    [Header("LaserPart")]
    public GameObject BeamEffect;
    public GameObject BeamHitPoint;
    public Transform Aperture;
    public LayerMask LM;
    private RaycastHit m_hit;
    [Header("LightPart")]
    public GameObject LightParts;
    public AlloyAreaLight FlashlightLight;

    private void CycleMode()
    {
      int num = (int) (this.LState + 1);
      if (num > 3)
        num = 0;
      this.LState = (LAM.LAMState) num;
      if (this.LState == LAM.LAMState.Off)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_LAMOFF, this.transform.position);
      else
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_LAMON, this.transform.position);
      if (this.LState == LAM.LAMState.Laser || this.LState == LAM.LAMState.LaserLight)
      {
        this.BeamHitPoint.SetActive(true);
        this.BeamEffect.SetActive(true);
      }
      else
      {
        this.BeamHitPoint.SetActive(false);
        this.BeamEffect.SetActive(false);
      }
      if (this.LState == LAM.LAMState.Light || this.LState == LAM.LAMState.LaserLight)
      {
        this.LightParts.SetActive(true);
        if (GM.CurrentSceneSettings.IsSceneLowLight)
          this.FlashlightLight.Intensity = 2f;
        else
          this.FlashlightLight.Intensity = 0.5f;
      }
      else
        this.LightParts.SetActive(false);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
          this.CycleMode();
      }
      else if (hand.Input.TouchpadDown && (double) touchpadAxes.magnitude > 0.25 && (double) Vector2.Angle(touchpadAxes, Vector2.up) <= 45.0)
        this.CycleMode();
      base.UpdateInteraction(hand);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.LState != LAM.LAMState.Laser && this.LState != LAM.LAMState.LaserLight)
        return;
      Vector3 vector3 = this.Aperture.position + this.Aperture.forward * 100f;
      float num1 = 100f;
      if (Physics.Raycast(this.Aperture.position, this.Aperture.forward, out this.m_hit, 100f, (int) this.LM, QueryTriggerInteraction.Ignore))
      {
        vector3 = this.m_hit.point;
        num1 = this.m_hit.distance;
      }
      float num2 = Mathf.Lerp(0.01f, 0.2f, num1 * 0.01f);
      this.BeamHitPoint.transform.position = vector3;
      this.BeamHitPoint.transform.localScale = new Vector3(num2, num2, num2);
    }

    public override void OnDetach()
    {
      base.OnDetach();
      this.LState = LAM.LAMState.Off;
      this.BeamHitPoint.SetActive(false);
      this.BeamEffect.SetActive(false);
    }

    public enum LAMState
    {
      Off,
      Laser,
      Light,
      LaserLight,
    }
  }
}
