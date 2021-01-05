// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Vibration_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Action_Vibration_Source : SteamVR_Action_Out_Source
  {
    protected SteamVR_Action_Vibration vibrationAction;

    public event SteamVR_Action_Vibration.ActiveChangeHandler onActiveChange;

    public event SteamVR_Action_Vibration.ActiveChangeHandler onActiveBindingChange;

    public event SteamVR_Action_Vibration.ExecuteHandler onExecute;

    public override bool active => this.activeBinding && this.setActive;

    public override bool activeBinding => true;

    public override bool lastActive { get; protected set; }

    public override bool lastActiveBinding => true;

    public float timeLastExecuted { get; protected set; }

    public override void Initialize()
    {
      base.Initialize();
      this.lastActive = true;
    }

    public override void Preinitialize(
      SteamVR_Action wrappingAction,
      SteamVR_Input_Sources forInputSource)
    {
      base.Preinitialize(wrappingAction, forInputSource);
      this.vibrationAction = (SteamVR_Action_Vibration) wrappingAction;
    }

    public void Execute(
      float secondsFromNow,
      float durationSeconds,
      float frequency,
      float amplitude)
    {
      if (SteamVR_Input.isStartupFrame)
        return;
      this.timeLastExecuted = Time.realtimeSinceStartup;
      EVRInputError evrInputError = OpenVR.Input.TriggerHapticVibrationAction(this.handle, secondsFromNow, durationSeconds, frequency, amplitude, this.inputSourceHandle);
      if (evrInputError != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> TriggerHapticVibrationAction (" + this.fullPath + ") error: " + evrInputError.ToString() + " handle: " + this.handle.ToString()));
      if (this.onExecute == null)
        return;
      this.onExecute(this.vibrationAction, this.inputSource, secondsFromNow, durationSeconds, frequency, amplitude);
    }
  }
}
