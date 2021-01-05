﻿// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Single_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Action_Single_Source : SteamVR_Action_In_Source, ISteamVR_Action_Single, ISteamVR_Action_In_Source, ISteamVR_Action_Source
  {
    protected static uint actionData_size;
    public float changeTolerance = Mathf.Epsilon;
    protected InputAnalogActionData_t actionData = new InputAnalogActionData_t();
    protected InputAnalogActionData_t lastActionData = new InputAnalogActionData_t();
    protected SteamVR_Action_Single singleAction;

    public event SteamVR_Action_Single.AxisHandler onAxis;

    public event SteamVR_Action_Single.ActiveChangeHandler onActiveChange;

    public event SteamVR_Action_Single.ActiveChangeHandler onActiveBindingChange;

    public event SteamVR_Action_Single.ChangeHandler onChange;

    public event SteamVR_Action_Single.UpdateHandler onUpdate;

    public float axis => this.active ? this.actionData.x : 0.0f;

    public float lastAxis => this.active ? this.lastActionData.x : 0.0f;

    public float delta => this.active ? this.actionData.deltaX : 0.0f;

    public float lastDelta => this.active ? this.lastActionData.deltaX : 0.0f;

    public override bool changed { get; protected set; }

    public override bool lastChanged { get; protected set; }

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
      this.singleAction = (SteamVR_Action_Single) wrappingAction;
    }

    public override void Initialize()
    {
      base.Initialize();
      if (SteamVR_Action_Single_Source.actionData_size != 0U)
        return;
      SteamVR_Action_Single_Source.actionData_size = (uint) Marshal.SizeOf(typeof (InputAnalogActionData_t));
    }

    public override void UpdateValue()
    {
      this.lastActionData = this.actionData;
      this.lastActive = this.active;
      EVRInputError analogActionData = OpenVR.Input.GetAnalogActionData(this.handle, ref this.actionData, SteamVR_Action_Single_Source.actionData_size, SteamVR_Input_Source.GetHandle(this.inputSource));
      if (analogActionData != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetAnalogActionData error (" + this.fullPath + "): " + analogActionData.ToString() + " handle: " + this.handle.ToString()));
      this.updateTime = Time.realtimeSinceStartup;
      this.changed = false;
      if (this.active)
      {
        if ((double) this.delta > (double) this.changeTolerance || (double) this.delta < -(double) this.changeTolerance)
        {
          this.changed = true;
          this.changedTime = Time.realtimeSinceStartup + this.actionData.fUpdateTime;
          if (this.onChange != null)
            this.onChange(this.singleAction, this.inputSource, this.axis, this.delta);
        }
        if ((double) this.axis != 0.0 && this.onAxis != null)
          this.onAxis(this.singleAction, this.inputSource, this.axis, this.delta);
        if (this.onUpdate != null)
          this.onUpdate(this.singleAction, this.inputSource, this.axis, this.delta);
      }
      if (this.onActiveBindingChange != null && this.lastActiveBinding != this.activeBinding)
        this.onActiveBindingChange(this.singleAction, this.inputSource, this.activeBinding);
      if (this.onActiveChange == null || this.lastActive == this.active)
        return;
      this.onActiveChange(this.singleAction, this.inputSource, this.activeBinding);
    }
  }
}