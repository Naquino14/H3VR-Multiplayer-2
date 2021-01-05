// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Boolean
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Action_Boolean : SteamVR_Action_In<SteamVR_Action_Boolean_Source_Map, SteamVR_Action_Boolean_Source>, ISteamVR_Action_Boolean, ISerializationCallbackReceiver, ISteamVR_Action_In_Source, ISteamVR_Action_Source
  {
    public event SteamVR_Action_Boolean.ChangeHandler onChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onChange -= value;
    }

    public event SteamVR_Action_Boolean.UpdateHandler onUpdate
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onUpdate += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onUpdate -= value;
    }

    public event SteamVR_Action_Boolean.StateHandler onState
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onState += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onState -= value;
    }

    public event SteamVR_Action_Boolean.StateDownHandler onStateDown
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onStateDown += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onStateDown -= value;
    }

    public event SteamVR_Action_Boolean.StateUpHandler onStateUp
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onStateUp += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onStateUp -= value;
    }

    public event SteamVR_Action_Boolean.ActiveChangeHandler onActiveChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= value;
    }

    public event SteamVR_Action_Boolean.ActiveChangeHandler onActiveBindingChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange -= value;
    }

    public bool state => this.sourceMap[SteamVR_Input_Sources.Any].state;

    public bool stateDown => this.sourceMap[SteamVR_Input_Sources.Any].stateDown;

    public bool stateUp => this.sourceMap[SteamVR_Input_Sources.Any].stateUp;

    public bool lastState => this.sourceMap[SteamVR_Input_Sources.Any].lastState;

    public bool lastStateDown => this.sourceMap[SteamVR_Input_Sources.Any].lastStateDown;

    public bool lastStateUp => this.sourceMap[SteamVR_Input_Sources.Any].lastStateUp;

    public bool GetStateDown(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].stateDown;

    public bool GetStateUp(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].stateUp;

    public bool GetState(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].state;

    public bool GetLastStateDown(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastStateDown;

    public bool GetLastStateUp(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastStateUp;

    public bool GetLastState(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastState;

    public void AddOnActiveChangeListener(
      SteamVR_Action_Boolean.ActiveChangeHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveChange += functionToCall;
    }

    public void RemoveOnActiveChangeListener(
      SteamVR_Action_Boolean.ActiveChangeHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveChange -= functionToStopCalling;
    }

    public void AddOnActiveBindingChangeListener(
      SteamVR_Action_Boolean.ActiveChangeHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveBindingChange += functionToCall;
    }

    public void RemoveOnActiveBindingChangeListener(
      SteamVR_Action_Boolean.ActiveChangeHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveBindingChange -= functionToStopCalling;
    }

    public void AddOnChangeListener(
      SteamVR_Action_Boolean.ChangeHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onChange += functionToCall;
    }

    public void RemoveOnChangeListener(
      SteamVR_Action_Boolean.ChangeHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onChange -= functionToStopCalling;
    }

    public void AddOnUpdateListener(
      SteamVR_Action_Boolean.UpdateHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onUpdate += functionToCall;
    }

    public void RemoveOnUpdateListener(
      SteamVR_Action_Boolean.UpdateHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onUpdate -= functionToStopCalling;
    }

    public void AddOnStateDownListener(
      SteamVR_Action_Boolean.StateDownHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onStateDown += functionToCall;
    }

    public void RemoveOnStateDownListener(
      SteamVR_Action_Boolean.StateDownHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onStateDown -= functionToStopCalling;
    }

    public void AddOnStateUpListener(
      SteamVR_Action_Boolean.StateUpHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onStateUp += functionToCall;
    }

    public void RemoveOnStateUpListener(
      SteamVR_Action_Boolean.StateUpHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onStateUp -= functionToStopCalling;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize() => this.InitAfterDeserialize();

    public delegate void StateDownHandler(
      SteamVR_Action_Boolean fromAction,
      SteamVR_Input_Sources fromSource);

    public delegate void StateUpHandler(
      SteamVR_Action_Boolean fromAction,
      SteamVR_Input_Sources fromSource);

    public delegate void StateHandler(
      SteamVR_Action_Boolean fromAction,
      SteamVR_Input_Sources fromSource);

    public delegate void ActiveChangeHandler(
      SteamVR_Action_Boolean fromAction,
      SteamVR_Input_Sources fromSource,
      bool active);

    public delegate void ChangeHandler(
      SteamVR_Action_Boolean fromAction,
      SteamVR_Input_Sources fromSource,
      bool newState);

    public delegate void UpdateHandler(
      SteamVR_Action_Boolean fromAction,
      SteamVR_Input_Sources fromSource,
      bool newState);
  }
}
