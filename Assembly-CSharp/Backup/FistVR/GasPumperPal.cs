// Decompiled with JetBrains decompiler
// Type: FistVR.GasPumperPal
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class GasPumperPal : MonoBehaviour
  {
    public Transform JerryCanPlacement;
    public List<GameObject> FueledObjects;
    private bool m_hasFuel;
    public string FlagWhenFueled;
    public int FlagValueWhenFueled;
    public FVRObject JerryCan;
    private FistVR.JerryCan m_can;
    public AudioEvent AudEvent_Click;
    public Transform OnButton;
    public Transform OffButton;
    public AudioEvent AudEvent_Load;

    public void Start()
    {
      if (GM.ZMaster.FlagM.GetFlagValue(this.FlagWhenFueled) >= this.FlagValueWhenFueled)
        this.SetFueledState(true);
      else
        this.SetFueledState(false);
    }

    public bool HasFuel() => this.m_hasFuel;

    private void Update()
    {
      if (this.m_hasFuel || GM.ZMaster.FlagM.GetFlagValue(this.FlagWhenFueled) < this.FlagValueWhenFueled)
        return;
      this.m_hasFuel = true;
    }

    private void SetFueledState(bool f)
    {
      if (f)
      {
        this.m_hasFuel = true;
      }
      else
      {
        this.m_hasFuel = false;
        this.TurnOff(1);
      }
    }

    public void TurnOn(int af)
    {
      SM.PlayGenericSound(this.AudEvent_Click, this.OnButton.position);
      if (!this.m_hasFuel)
        return;
      for (int index = 0; index < this.FueledObjects.Count; ++index)
        this.FueledObjects[index].SendMessage(nameof (TurnOn), SendMessageOptions.DontRequireReceiver);
    }

    public void TurnOff(int af)
    {
      SM.PlayGenericSound(this.AudEvent_Click, this.OffButton.position);
      for (int index = 0; index < this.FueledObjects.Count; ++index)
        this.FueledObjects[index].SendMessage(nameof (TurnOff), SendMessageOptions.DontRequireReceiver);
    }
  }
}
