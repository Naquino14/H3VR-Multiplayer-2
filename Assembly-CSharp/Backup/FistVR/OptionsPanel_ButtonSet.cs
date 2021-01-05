// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsPanel_ButtonSet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class OptionsPanel_ButtonSet : MonoBehaviour
  {
    public Color SelectedColor;
    public Color UnSelectedColor;
    public Color HighlightedColor;
    public Image[] ButtonImagesInSet;
    public bool UsesPointableButtons;
    public FVRPointableButton[] ButtonsInSet;
    private bool m_isHighLighted;
    public int selectedButton;

    private void Awake() => this.UpdateButtonVisual();

    public void SetSelectedButton(int index)
    {
      this.selectedButton = index;
      this.UpdateButtonVisual();
    }

    public void SetSelectedButton(bool b)
    {
      this.selectedButton = Convert.ToInt32(!b);
      this.UpdateButtonVisual();
    }

    private void UpdateButtonVisual()
    {
      if (this.UsesPointableButtons)
      {
        for (int index = 0; index < this.ButtonsInSet.Length; ++index)
        {
          this.ButtonsInSet[index].ColorUnselected = this.UnSelectedColor;
          this.ButtonsInSet[index].ColorSelected = this.HighlightedColor;
        }
        this.ButtonsInSet[this.selectedButton].ColorUnselected = this.SelectedColor;
        this.ButtonsInSet[this.selectedButton].ColorSelected = this.HighlightedColor;
        for (int index = 0; index < this.ButtonsInSet.Length; ++index)
          this.ButtonsInSet[index].ForceUpdate();
      }
      else
      {
        for (int index = 0; index < this.ButtonImagesInSet.Length; ++index)
          this.ButtonImagesInSet[index].color = this.UnSelectedColor;
        this.ButtonImagesInSet[this.selectedButton].color = this.SelectedColor;
      }
    }

    public void EnableAllButtons()
    {
      for (int index = 0; index < this.ButtonImagesInSet.Length; ++index)
        this.ButtonImagesInSet[index].color = this.SelectedColor;
    }
  }
}
