// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_Movement
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class OptionsScreen_Movement : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_MovementMode;
    public Text TXT_ModeName;
    public Text TXT_ModeDescrip;
    public List<string> ModeNames;
    [Multiline(9)]
    public List<string> Descrips;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_MovementMode.SetSelectedButton((int) GM.Options.MovementOptions.CurrentMovementMode);
      this.TXT_ModeName.text = this.ModeNames[(int) GM.Options.MovementOptions.CurrentMovementMode];
      this.TXT_ModeDescrip.text = this.Descrips[(int) GM.Options.MovementOptions.CurrentMovementMode];
    }

    public void SetMovementMode(int i)
    {
      GM.CurrentMovementManager.Mode = (FVRMovementManager.MovementMode) i;
      GM.Options.MovementOptions.CurrentMovementMode = (FVRMovementManager.MovementMode) i;
      this.TXT_ModeName.text = this.ModeNames[(int) GM.Options.MovementOptions.CurrentMovementMode];
      this.TXT_ModeDescrip.text = this.Descrips[(int) GM.Options.MovementOptions.CurrentMovementMode];
      GM.Options.SaveToFile();
    }
  }
}
