// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Vector2_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Action_Vector2_Source : SteamVR_Action_In_Source, ISteamVR_Action_Vector2, ISteamVR_Action_In_Source, ISteamVR_Action_Source
  {
    protected static uint actionData_size;
    public float changeTolerance = Mathf.Epsilon;
    protected InputAnalogActionData_t actionData = new InputAnalogActionData_t();
    protected InputAnalogActionData_t lastActionData = new InputAnalogActionData_t();
    protected SteamVR_Action_Vector2 vector2Action;

    public event SteamVR_Action_Vector2.AxisHandler onAxis;

    public event SteamVR_Action_Vector2.ActiveChangeHandler onActiveChange;

    public event SteamVR_Action_Vector2.ActiveChangeHandler onActiveBindingChange;

    public event SteamVR_Action_Vector2.ChangeHandler onChange;

    public event SteamVR_Action_Vector2.UpdateHandler onUpdate;

    public Vector2 axis { get; protected set; }

    public Vector2 lastAxis { get; protected set; }

    public Vector2 delta { get; protected set; }

    public Vector2 lastDelta { get; protected set; }

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
      this.vector2Action = (SteamVR_Action_Vector2) wrappingAction;
    }

    public override void Initialize()
    {
      base.Initialize();
      if (SteamVR_Action_Vector2_Source.actionData_size != 0U)
        return;
      SteamVR_Action_Vector2_Source.actionData_size = (uint) Marshal.SizeOf(typeof (InputAnalogActionData_t));
    }

    public override void UpdateValue()
    {
      this.lastActionData = this.actionData;
      this.lastActive = this.active;
      this.lastAxis = this.axis;
      this.lastDelta = this.delta;
      EVRInputError analogActionData = OpenVR.Input.GetAnalogActionData(this.handle, ref this.actionData, SteamVR_Action_Vector2_Source.actionData_size, SteamVR_Input_Source.GetHandle(this.inputSource));
      if (analogActionData != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetAnalogActionData error (" + this.fullPath + "): " + analogActionData.ToString() + " handle: " + this.handle.ToString()));
      this.updateTime = Time.realtimeSinceStartup;
      this.axis = new Vector2(this.actionData.x, this.actionData.y);
      this.delta = new Vector2(this.actionData.deltaX, this.actionData.deltaY);
      this.changed = false;
      if (this.active)
      {
        if ((double) this.delta.magnitude > (double) this.changeTolerance)
        {
          this.changed = true;
          this.changedTime = Time.realtimeSinceStartup + this.actionData.fUpdateTime;
          if (this.onChange != null)
            this.onChange(this.vector2Action, this.inputSource, this.axis, this.delta);
        }
        if (this.axis != Vector2.zero && this.onAxis != null)
          this.onAxis(this.vector2Action, this.inputSource, this.axis, this.delta);
        if (this.onUpdate != null)
          this.onUpdate(this.vector2Action, this.inputSource, this.axis, this.delta);
      }
      if (this.onActiveBindingChange != null && this.lastActiveBinding != this.activeBinding)
        this.onActiveBindingChange(this.vector2Action, this.inputSource, this.activeBinding);
      if (this.onActiveChange == null || this.lastActive == this.active)
        return;
      this.onActiveChange(this.vector2Action, this.inputSource, this.activeBinding);
    }
  }
}
