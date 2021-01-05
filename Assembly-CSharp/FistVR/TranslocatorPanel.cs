// Decompiled with JetBrains decompiler
// Type: FistVR.TranslocatorPanel
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class TranslocatorPanel : MonoBehaviour
  {
    public bool RequiresFlags = true;
    public List<Translocator> EndPoints;
    private float m_panelUpdateTick = 1f;
    public List<string> FlagsForPoweredState = new List<string>();
    public List<int> FlagsValueNeededForPoweredState = new List<int>();
    public List<bool> IsEndPointPowered = new List<bool>();
    public List<string> EndPointNames = new List<string>();
    public List<Text> Labels = new List<Text>();
    private int m_selectedOption = -1;
    public Translocator ControlledTranslocator;
    public AudioEvent AudEvent_Success;
    public AudioEvent AudEvent_Failure;
    public Color Color_Selected;
    public Color Color_UnSelected;

    private void Start()
    {
      for (int index = 0; index < this.Labels.Count; ++index)
        this.Labels[index].color = this.Color_UnSelected;
    }

    private void Update()
    {
      this.m_panelUpdateTick -= Time.deltaTime;
      if ((double) this.m_panelUpdateTick > 0.0)
        return;
      this.m_panelUpdateTick = 1f;
      this.UpdatePanelText();
    }

    private void UpdatePanelText()
    {
      if (!this.RequiresFlags)
        return;
      for (int index = 0; index < this.FlagsForPoweredState.Count; ++index)
      {
        if (GM.ZMaster.FlagM.GetFlagValue(this.FlagsForPoweredState[index]) >= this.FlagsValueNeededForPoweredState[index])
        {
          this.IsEndPointPowered[index] = true;
          this.Labels[index].text = this.EndPointNames[index];
        }
        else
        {
          this.IsEndPointPowered[index] = false;
          this.Labels[index].text = "-OFFLINE-";
        }
      }
    }

    private void SetSelectedOption(int index)
    {
      SM.PlayGenericSound(this.AudEvent_Success, this.transform.position);
      for (int index1 = 0; index1 < this.Labels.Count; ++index1)
      {
        if (index1 == index)
          this.Labels[index1].color = this.Color_Selected;
        else
          this.Labels[index1].color = this.Color_UnSelected;
      }
    }

    public void SelectItem0(int i)
    {
      if (this.IsEndPointPowered[0])
      {
        this.SetSelectedOption(0);
        this.ControlledTranslocator.EndPoint = this.EndPoints[0];
      }
      else
        SM.PlayGenericSound(this.AudEvent_Failure, this.transform.position);
    }

    public void SelectItem1(int i)
    {
      if (this.IsEndPointPowered[1])
      {
        this.SetSelectedOption(1);
        this.ControlledTranslocator.EndPoint = this.EndPoints[1];
      }
      else
        SM.PlayGenericSound(this.AudEvent_Failure, this.transform.position);
    }

    public void SelectItem2(int i)
    {
      if (this.IsEndPointPowered[2])
      {
        this.SetSelectedOption(2);
        this.ControlledTranslocator.EndPoint = this.EndPoints[2];
      }
      else
        SM.PlayGenericSound(this.AudEvent_Failure, this.transform.position);
    }

    public void SelectItem3(int i)
    {
      if (this.IsEndPointPowered[3])
      {
        this.SetSelectedOption(3);
        this.ControlledTranslocator.EndPoint = this.EndPoints[3];
      }
      else
        SM.PlayGenericSound(this.AudEvent_Failure, this.transform.position);
    }

    public void SelectItem4(int i)
    {
      if (this.IsEndPointPowered[4])
      {
        this.SetSelectedOption(4);
        this.ControlledTranslocator.EndPoint = this.EndPoints[4];
      }
      else
        SM.PlayGenericSound(this.AudEvent_Failure, this.transform.position);
    }

    public void SelectItem5(int i)
    {
      if (this.IsEndPointPowered[5])
      {
        this.SetSelectedOption(5);
        this.ControlledTranslocator.EndPoint = this.EndPoints[5];
      }
      else
        SM.PlayGenericSound(this.AudEvent_Failure, this.transform.position);
    }

    public void SelectItem6(int i)
    {
      if (this.IsEndPointPowered[6])
      {
        this.SetSelectedOption(6);
        this.ControlledTranslocator.EndPoint = this.EndPoints[6];
      }
      else
        SM.PlayGenericSound(this.AudEvent_Failure, this.transform.position);
    }
  }
}
