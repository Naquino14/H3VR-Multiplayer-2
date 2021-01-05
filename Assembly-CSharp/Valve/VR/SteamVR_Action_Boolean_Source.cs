// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Boolean_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Action_Boolean_Source : SteamVR_Action_In_Source, ISteamVR_Action_Boolean, ISteamVR_Action_In_Source, ISteamVR_Action_Source
  {
    protected static uint actionData_size;
    protected InputDigitalActionData_t actionData = new InputDigitalActionData_t();
    protected InputDigitalActionData_t lastActionData = new InputDigitalActionData_t();
    protected SteamVR_Action_Boolean booleanAction;

    public event SteamVR_Action_Boolean.StateDownHandler onStateDown;

    public event SteamVR_Action_Boolean.StateUpHandler onStateUp;

    public event SteamVR_Action_Boolean.StateHandler onState;

    public event SteamVR_Action_Boolean.ActiveChangeHandler onActiveChange;

    public event SteamVR_Action_Boolean.ActiveChangeHandler onActiveBindingChange;

    public event SteamVR_Action_Boolean.ChangeHandler onChange;

    public event SteamVR_Action_Boolean.UpdateHandler onUpdate;

    public bool state => this.active && this.actionData.bState;

    public bool stateDown => this.active && this.actionData.bState && this.actionData.bChanged;

    public bool stateUp => this.active && !this.actionData.bState && this.actionData.bChanged;

    public override bool changed
    {
      get => this.active && this.actionData.bChanged;
      protected set
      {
      }
    }

    public bool lastState => this.lastActionData.bState;

    public bool lastStateDown => this.lastActionData.bState && this.lastActionData.bChanged;

    public bool lastStateUp => !this.lastActionData.bState && this.lastActionData.bChanged;

    public override bool lastChanged
    {
      get => this.lastActionData.bChanged;
      protected set
      {
      }
    }

    public override ulong activeOrigin => this.active ? this.actionData.activeOrigin : 0UL;

    public override ulong lastActiveOrigin => this.lastActionData.activeOrigin;

    public override bool active => this.activeBinding && this.action.actionSet.IsActive(this.inputSource);

    public override bool activeBinding => this.actionData.bActive;

    public override bool lastActive { get; protected set; }

    public override bool lastActiveBinding => this.lastActionData.bActive;

    public override void Preinitialize(
      SteamVR_Action wrappingAction,
      SteamVR_Input_Sources forInputSource)
    {
      base.Preinitialize(wrappingAction, forInputSource);
      this.booleanAction = (SteamVR_Action_Boolean) wrappingAction;
    }

    public override void Initialize()
    {
      base.Initialize();
      if (SteamVR_Action_Boolean_Source.actionData_size != 0U)
        return;
      SteamVR_Action_Boolean_Source.actionData_size = (uint) Marshal.SizeOf(typeof (InputDigitalActionData_t));
    }

    public override void UpdateValue()
    {
      this.lastActionData = this.actionData;
      this.lastActive = this.active;
      EVRInputError digitalActionData = OpenVR.Input.GetDigitalActionData(this.action.handle, ref this.actionData, SteamVR_Action_Boolean_Source.actionData_size, this.inputSourceHandle);
      if (digitalActionData != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetDigitalActionData error (" + this.action.fullPath + "): " + digitalActionData.ToString() + " handle: " + this.action.handle.ToString()));
      if (this.changed)
        this.changedTime = Time.realtimeSinceStartup + this.actionData.fUpdateTime;
      this.updateTime = Time.realtimeSinceStartup;
      if (this.active)
      {
        if (this.onStateDown != null && this.stateDown)
          this.onStateDown(this.booleanAction, this.inputSource);
        if (this.onStateUp != null && this.stateUp)
          this.onStateUp(this.booleanAction, this.inputSource);
        if (this.onState != null && this.state)
          this.onState(this.booleanAction, this.inputSource);
        if (this.onChange != null && this.changed)
          this.onChange(this.booleanAction, this.inputSource, this.state);
        if (this.onUpdate != null)
          this.onUpdate(this.booleanAction, this.inputSource, this.state);
      }
      if (this.onActiveBindingChange != null && this.lastActiveBinding != this.activeBinding)
        this.onActiveBindingChange(this.booleanAction, this.inputSource, this.activeBinding);
      if (this.onActiveChange == null || this.lastActive == this.active)
        return;
      this.onActiveChange(this.booleanAction, this.inputSource, this.activeBinding);
    }
  }
}
