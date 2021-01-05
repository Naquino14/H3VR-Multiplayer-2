// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_Sapper
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MF2_Sapper : FVRPhysicalObject
  {
    [Header("VFX")]
    public GameObject SapperEffect;
    public Transform Dial;
    public Transform DialPointUp;
    public Transform DialPointDown;
    private float m_sappingPower = 15f;
    private bool m_isSapperActive;
    private float detectPulse = 0.1f;
    public GameObject PulseExplosion;
    private bool[] switches = new bool[2];
    public Transform Switch0;
    public Transform Switch1;
    private HashSet<Rigidbody> rbs = new HashSet<Rigidbody>();

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.m_isSapperActive)
      {
        this.detectPulse -= Time.deltaTime;
        if ((double) this.detectPulse <= 0.0)
        {
          this.detectPulse = 0.1f;
          this.Pulse();
        }
        this.m_sappingPower -= Time.deltaTime;
        if ((double) this.m_sappingPower <= 0.0)
          this.ShutOff();
      }
      else if ((double) this.m_sappingPower < 15.0)
        this.m_sappingPower += Time.deltaTime;
      this.Dial.localPosition = Vector3.Lerp(this.DialPointUp.localPosition, this.DialPointDown.localPosition, this.m_sappingPower / 15f);
    }

    private void Pulse() => Object.Instantiate<GameObject>(this.PulseExplosion, this.transform.position, Quaternion.identity);

    private void SetPower(bool isOn)
    {
      if (!isOn)
      {
        this.m_isSapperActive = false;
        this.SapperEffect.SetActive(false);
      }
      else
      {
        if (!isOn)
          return;
        this.m_isSapperActive = true;
        this.SapperEffect.SetActive(true);
      }
    }

    private void UpdatePowerState()
    {
      if (this.switches[0] && this.switches[1])
        this.SetPower(true);
      else
        this.SetPower(false);
      this.Switch0.localEulerAngles = !this.switches[0] ? new Vector3(18f, 0.0f, 0.0f) : new Vector3(-18f, 0.0f, 0.0f);
      if (this.switches[1])
        this.Switch1.localEulerAngles = new Vector3(-18f, 0.0f, 0.0f);
      else
        this.Switch1.localEulerAngles = new Vector3(18f, 0.0f, 0.0f);
    }

    private void ShutOff()
    {
      this.switches[0] = false;
      this.switches[1] = false;
      this.UpdatePowerState();
    }

    public void ToggleSwitch(int which)
    {
      this.switches[which] = !this.switches[which];
      this.UpdatePowerState();
    }
  }
}
