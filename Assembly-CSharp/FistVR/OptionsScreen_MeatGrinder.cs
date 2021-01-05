// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_MeatGrinder
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_MeatGrinder : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_Combo;
    public OptionsPanel_ButtonSet OBS_Side1;
    public OptionsPanel_ButtonSet OBS_Side2;
    public OptionsPanel_ButtonSet OBS_Side3;
    public OptionsPanel_ButtonSet OBS_Dessert;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_Combo.SetSelectedButton((int) GM.Options.MeatGrinderFlags.MGMode);
      this.OBS_Side1.SetSelectedButton((int) GM.Options.MeatGrinderFlags.AIMood);
      this.OBS_Side2.SetSelectedButton((int) GM.Options.MeatGrinderFlags.PrimaryLight);
      this.OBS_Side3.SetSelectedButton((int) GM.Options.MeatGrinderFlags.SecondaryLight);
      this.OBS_Dessert.SetSelectedButton((int) GM.Options.MeatGrinderFlags.NarratorMode);
    }

    public void SetCombo(int i)
    {
      GM.Options.MeatGrinderFlags.MGMode = (MeatGrinderFlags.MeatGrinderMode) i;
      GM.Options.SaveToFile();
    }

    public void SetSide1(int i)
    {
      GM.Options.MeatGrinderFlags.AIMood = (MeatGrinderMaster.EventAI.EventAIMood) i;
      GM.Options.SaveToFile();
    }

    public void SetSide2(int i)
    {
      GM.Options.MeatGrinderFlags.PrimaryLight = (MeatGrinderFlags.LightSourceOption) i;
      GM.Options.SaveToFile();
    }

    public void SetSide3(int i)
    {
      GM.Options.MeatGrinderFlags.SecondaryLight = (MeatGrinderFlags.LightSourceOption) i;
      GM.Options.SaveToFile();
    }

    public void SetDessert(int i)
    {
      GM.Options.MeatGrinderFlags.NarratorMode = (MeatGrinderFlags.MeatGrinderNarratorMode) i;
      GM.Options.SaveToFile();
    }
  }
}
