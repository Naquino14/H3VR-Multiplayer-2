// Decompiled with JetBrains decompiler
// Type: FistVR.InitConfigBox
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class InitConfigBox : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_Option;
    public GameObject BTN_Accept;
    public GameObject BTN_Confirm;
    public AudioEvent AudEvent_Beep;
    public AudioEvent AudEvent_Boop;

    private void Start()
    {
      if (GM.Options.ControlOptions.HasConfirmedControls)
        this.gameObject.SetActive(false);
      this.OBS_Option.SetSelectedButton((int) GM.Options.ControlOptions.CCM);
      this.BTN_Confirm.SetActive(false);
    }

    public void SetCCM(int i)
    {
      GM.Options.ControlOptions.CCM = (ControlOptions.CoreControlMode) i;
      GM.Options.SaveToFile();
      SM.PlayGenericSound(this.AudEvent_Beep, this.transform.position);
      this.BTN_Accept.SetActive(true);
      this.BTN_Confirm.SetActive(false);
    }

    public void ButtonPress_Accept()
    {
      SM.PlayGenericSound(this.AudEvent_Boop, this.transform.position);
      this.BTN_Accept.SetActive(false);
      this.BTN_Confirm.SetActive(true);
    }

    public void ButtonPress_Confirm()
    {
      SM.PlayGenericSound(this.AudEvent_Boop, this.transform.position);
      GM.Options.ControlOptions.HasConfirmedControls = true;
      this.gameObject.SetActive(false);
    }

    public void Update()
    {
      for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
      {
        if (GM.CurrentMovementManager.Hands[index].HasInit && (GM.CurrentMovementManager.Hands[index].DMode == DisplayMode.Vive || GM.CurrentMovementManager.Hands[index].DMode == DisplayMode.WMR))
          this.gameObject.SetActive(false);
      }
    }
  }
}
