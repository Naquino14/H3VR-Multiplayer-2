// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Vector3
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Action_Vector3 : SteamVR_Action_In<SteamVR_Action_Vector3_Source_Map, SteamVR_Action_Vector3_Source>, ISteamVR_Action_Vector3, ISerializationCallbackReceiver, ISteamVR_Action_In_Source, ISteamVR_Action_Source
  {
    public event SteamVR_Action_Vector3.ChangeHandler onChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onChange -= value;
    }

    public event SteamVR_Action_Vector3.UpdateHandler onUpdate
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onUpdate += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onUpdate -= value;
    }

    public event SteamVR_Action_Vector3.AxisHandler onAxis
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onAxis += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onAxis -= value;
    }

    public event SteamVR_Action_Vector3.ActiveChangeHandler onActiveChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= value;
    }

    public event SteamVR_Action_Vector3.ActiveChangeHandler onActiveBindingChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange -= value;
    }

    public Vector3 axis => this.sourceMap[SteamVR_Input_Sources.Any].axis;

    public Vector3 lastAxis => this.sourceMap[SteamVR_Input_Sources.Any].lastAxis;

    public Vector3 delta => this.sourceMap[SteamVR_Input_Sources.Any].delta;

    public Vector3 lastDelta => this.sourceMap[SteamVR_Input_Sources.Any].lastDelta;

    public Vector3 GetAxis(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].axis;

    public Vector3 GetAxisDelta(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].delta;

    public Vector3 GetLastAxis(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastAxis;

    public Vector3 GetLastAxisDelta(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastDelta;

    public void AddOnActiveChangeListener(
      SteamVR_Action_Vector3.ActiveChangeHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveChange += functionToCall;
    }

    public void RemoveOnActiveChangeListener(
      SteamVR_Action_Vector3.ActiveChangeHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveChange -= functionToStopCalling;
    }

    public void AddOnActiveBindingChangeListener(
      SteamVR_Action_Vector3.ActiveChangeHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveBindingChange += functionToCall;
    }

    public void RemoveOnActiveBindingChangeListener(
      SteamVR_Action_Vector3.ActiveChangeHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onActiveBindingChange -= functionToStopCalling;
    }

    public void AddOnChangeListener(
      SteamVR_Action_Vector3.ChangeHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onChange += functionToCall;
    }

    public void RemoveOnChangeListener(
      SteamVR_Action_Vector3.ChangeHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onChange -= functionToStopCalling;
    }

    public void AddOnUpdateListener(
      SteamVR_Action_Vector3.UpdateHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onUpdate += functionToCall;
    }

    public void RemoveOnUpdateListener(
      SteamVR_Action_Vector3.UpdateHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onUpdate -= functionToStopCalling;
    }

    public void AddOnAxisListener(
      SteamVR_Action_Vector3.AxisHandler functionToCall,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onAxis += functionToCall;
    }

    public void RemoveOnAxisListener(
      SteamVR_Action_Vector3.AxisHandler functionToStopCalling,
      SteamVR_Input_Sources inputSource)
    {
      this.sourceMap[inputSource].onAxis -= functionToStopCalling;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize() => this.InitAfterDeserialize();

    public delegate void AxisHandler(
      SteamVR_Action_Vector3 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector3 axis,
      Vector3 delta);

    public delegate void ActiveChangeHandler(
      SteamVR_Action_Vector3 fromAction,
      SteamVR_Input_Sources fromSource,
      bool active);

    public delegate void ChangeHandler(
      SteamVR_Action_Vector3 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector3 axis,
      Vector3 delta);

    public delegate void UpdateHandler(
      SteamVR_Action_Vector3 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector3 axis,
      Vector3 delta);
  }
}
