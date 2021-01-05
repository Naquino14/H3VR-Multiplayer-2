// Decompiled with JetBrains decompiler
// Type: FistVR.FVRBeamerSwitch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRBeamerSwitch : FVRInteractiveObject
  {
    [Header("Beamer Switch Config")]
    public FVRBeamer Beamer;
    public GBeamer GBeamer;
    public int SwitchIndex;
    private bool m_switchedOn;
    public AudioEvent AudEvent_Switch;
    private AudioSource aud;

    protected override void Awake()
    {
      base.Awake();
      this.m_switchedOn = false;
      this.aud = this.GetComponent<AudioSource>();
    }

    public override void SimpleInteraction(FVRViveHand hand) => this.ToggleSwitch(true);

    public void ToggleSwitch(bool isHandInteraction)
    {
      this.m_switchedOn = !this.m_switchedOn;
      if ((Object) this.aud == (Object) null)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Switch, this.transform.position);
      else
        this.aud.Play();
      if (this.m_switchedOn)
      {
        this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 20f);
        if ((Object) this.Beamer != (Object) null)
          this.Beamer.SetSwitchState(this.SwitchIndex, true);
        if (!((Object) this.GBeamer != (Object) null))
          return;
        this.GBeamer.SetSwitchState(this.SwitchIndex, true);
      }
      else
      {
        this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -20f);
        if ((Object) this.Beamer != (Object) null)
          this.Beamer.SetSwitchState(this.SwitchIndex, false);
        if (!((Object) this.GBeamer != (Object) null))
          return;
        this.GBeamer.SetSwitchState(this.SwitchIndex, false);
      }
    }
  }
}
