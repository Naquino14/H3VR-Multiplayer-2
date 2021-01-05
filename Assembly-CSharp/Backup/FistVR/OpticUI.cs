// Decompiled with JetBrains decompiler
// Type: FistVR.OpticUI
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class OpticUI : MonoBehaviour
  {
    public List<Text> SettingNames;
    public Transform Frame;
    public GameObject Arrow_Left;
    public GameObject Arrow_Right;
    private Amplifier m_amp;

    public void SetAmp(Amplifier a) => this.m_amp = a;

    public void UISetSetting(int i)
    {
      if (!((Object) this.m_amp != (Object) null))
        return;
      this.m_amp.GoToSetting(i);
      this.UpdateUI(this.m_amp);
    }

    public void UIArrowUp()
    {
      if (!((Object) this.m_amp != (Object) null))
        return;
      this.m_amp.SetCurSettingUp(false);
      this.UpdateUI(this.m_amp);
    }

    public void UIArrowDown()
    {
      if (!((Object) this.m_amp != (Object) null))
        return;
      this.m_amp.SetCurSettingDown();
      this.UpdateUI(this.m_amp);
    }

    public void UpdateUI(LadderSight L)
    {
      for (int index = 0; index < this.SettingNames.Count; ++index)
        this.UpdateTextAndArrows(L, index);
      this.UpdateFrame(L);
    }

    public void UpdateUI(Amplifier A)
    {
      for (int index = 0; index < this.SettingNames.Count; ++index)
        this.UpdateTextAndArrows(A, index);
      this.UpdateFrame(A);
    }

    private void UpdateTextAndArrows(LadderSight L, int index)
    {
      if (index >= 1)
      {
        this.SettingNames[index].text = string.Empty;
        this.SettingNames[index].gameObject.SetActive(false);
      }
      else
      {
        this.SettingNames[index].gameObject.SetActive(true);
        this.SettingNames[index].text = L.RangeNames[L.Setting];
        if (L.Setting > 0)
          this.Arrow_Left.SetActive(true);
        else
          this.Arrow_Left.SetActive(false);
        if (L.Setting < L.RangeNames.Count - 1)
          this.Arrow_Right.SetActive(true);
        else
          this.Arrow_Right.SetActive(false);
      }
    }

    private void UpdateTextAndArrows(Amplifier A, int index)
    {
      if (index >= A.OptionTypes.Count)
      {
        this.SettingNames[index].text = string.Empty;
      }
      else
      {
        switch (A.OptionTypes[index])
        {
          case OpticOptionType.Zero:
            this.SettingNames[index].text = "Base Zero: " + A.ZeroDistances[A.ZeroDistanceIndex].ToString() + "m";
            if (A.ZeroDistanceIndex > 0)
              this.Arrow_Left.SetActive(true);
            else
              this.Arrow_Left.SetActive(false);
            if (A.ZeroDistanceIndex < A.ZeroDistances.Count - 1)
            {
              this.Arrow_Right.SetActive(true);
              break;
            }
            this.Arrow_Right.SetActive(false);
            break;
          case OpticOptionType.Magnification:
            this.SettingNames[index].text = "Magnification: " + A.ZoomSettings[A.m_zoomSettingIndex].Magnification.ToString() + "x";
            if (A.m_zoomSettingIndex > 0)
              this.Arrow_Left.SetActive(true);
            else
              this.Arrow_Left.SetActive(false);
            if (A.m_zoomSettingIndex < A.ZoomSettings.Count - 1)
            {
              this.Arrow_Right.SetActive(true);
              break;
            }
            this.Arrow_Right.SetActive(false);
            break;
          case OpticOptionType.ElevationTweak:
            this.SettingNames[index].text = "Elevation: " + ((float) A.ElevationStep * 0.25f).ToString() + "MOA";
            this.Arrow_Left.SetActive(true);
            this.Arrow_Right.SetActive(true);
            break;
          case OpticOptionType.WindageTweak:
            this.SettingNames[index].text = "Windage: " + ((float) A.WindageStep * 0.25f).ToString() + "MOA";
            this.Arrow_Left.SetActive(true);
            this.Arrow_Right.SetActive(true);
            break;
        }
      }
    }

    private void UpdateFrame(Amplifier A)
    {
      if (A.OptionTypes.Count == 0)
        this.Frame.gameObject.SetActive(false);
      else
        this.Frame.localPosition = this.SettingNames[A.CurSelectedOptionIndex].transform.localPosition;
    }

    private void UpdateFrame(LadderSight L) => this.Frame.localPosition = this.SettingNames[0].transform.localPosition;
  }
}
