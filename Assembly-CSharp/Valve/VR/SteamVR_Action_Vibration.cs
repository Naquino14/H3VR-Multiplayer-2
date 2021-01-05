// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Vibration
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Action_Vibration : SteamVR_Action_Out<SteamVR_Action_Vibration_Source_Map, SteamVR_Action_Vibration_Source>, ISerializationCallbackReceiver
  {
    public event SteamVR_Action_Vibration.ActiveChangeHandler onActiveChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= value;
    }

    public event SteamVR_Action_Vibration.ActiveChangeHandler onActiveBindingChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange -= value;
    }

    public event SteamVR_Action_Vibration.ExecuteHandler onExecute
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onExecute += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onExecute -= value;
    }

    public void Execute(
      float secondsFromNow,
      float durationSeconds,
      float frequency,
      float amplitude,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].Execute(secondsFromNow, durationSeconds, frequency, amplitude);
    }

    public void AddOnActiveChangeListener(
      SteamVR_Action_Vibration.ActiveChangeHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveChange += functionToCall;
    }

    public void RemoveOnActiveChangeListener(
      SteamVR_Action_Vibration.ActiveChangeHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveChange -= functionToStopCalling;
    }

    public void AddOnActiveBindingChangeListener(
      SteamVR_Action_Vibration.ActiveChangeHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveBindingChange += functionToCall;
    }

    public void RemoveOnActiveBindingChangeListener(
      SteamVR_Action_Vibration.ActiveChangeHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveBindingChange -= functionToStopCalling;
    }

    public void AddOnExecuteListener(
      SteamVR_Action_Vibration.ExecuteHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onExecute += functionToCall;
    }

    public void RemoveOnExecuteListener(
      SteamVR_Action_Vibration.ExecuteHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onExecute -= functionToStopCalling;
    }

    public override float GetTimeLastChanged(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].timeLastExecuted;

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize() => this.InitAfterDeserialize();

    public override bool IsUpdating(SteamVR_Input_Sources inputSource) => this.sourceMap.IsUpdating(inputSource);

    public delegate void ActiveChangeHandler(
      SteamVR_Action_Vibration fromAction,
      SteamVR_Input_Sources fromSource,
      bool active);

    public delegate void ExecuteHandler(
      SteamVR_Action_Vibration fromAction,
      SteamVR_Input_Sources fromSource,
      float secondsFromNow,
      float durationSeconds,
      float frequency,
      float amplitude);
  }
}
