// Decompiled with JetBrains decompiler
// Type: FistVR.LaserPointer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LaserPointer : FVRFireArmAttachmentInterface
  {
    private bool IsOn;
    public GameObject BeamEffect;
    public GameObject BeamHitPoint;
    public Transform Aperture;
    public LayerMask LM;
    private RaycastHit m_hit;
    public AudioEvent AudEvent_LaserOnClip;
    public AudioEvent AudEvent_LaserOffClip;

    protected override void Awake()
    {
      base.Awake();
      this.BeamHitPoint.transform.SetParent((Transform) null);
    }

    public override void OnDestroy() => Object.Destroy((Object) this.BeamHitPoint);

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

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.IsOn)
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
      this.IsOn = false;
      this.BeamHitPoint.SetActive(this.IsOn);
      this.BeamEffect.SetActive(this.IsOn);
    }

    private void ToggleOn()
    {
      this.IsOn = !this.IsOn;
      this.BeamHitPoint.SetActive(this.IsOn);
      this.BeamEffect.SetActive(this.IsOn);
      if (this.IsOn)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_LaserOnClip, this.transform.position);
      else
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_LaserOffClip, this.transform.position);
    }
  }
}
