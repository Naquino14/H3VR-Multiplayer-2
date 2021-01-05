// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Behaviour_Vector3
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Behaviour_Vector3 : MonoBehaviour
  {
    public SteamVR_Action_Vector3 vector3Action;
    [Tooltip("The device this action should apply to. Any if the action is not device specific.")]
    public SteamVR_Input_Sources inputSource;
    [Tooltip("Fires whenever the action's value has changed since the last update.")]
    public SteamVR_Behaviour_Vector3Event onChange;
    [Tooltip("Fires whenever the action's value has been updated.")]
    public SteamVR_Behaviour_Vector3Event onUpdate;
    [Tooltip("Fires whenever the action's value has been updated and is non-zero.")]
    public SteamVR_Behaviour_Vector3Event onAxis;
    public SteamVR_Behaviour_Vector3.ChangeHandler onChangeEvent;
    public SteamVR_Behaviour_Vector3.UpdateHandler onUpdateEvent;
    public SteamVR_Behaviour_Vector3.AxisHandler onAxisEvent;

    public bool isActive => this.vector3Action.GetActive(this.inputSource);

    protected virtual void OnEnable()
    {
      if ((SteamVR_Action) this.vector3Action == (SteamVR_Action) null)
        Debug.LogError((object) "[SteamVR] Vector3 action not set.", (Object) this);
      else
        this.AddHandlers();
    }

    protected virtual void OnDisable() => this.RemoveHandlers();

    protected void AddHandlers()
    {
      this.vector3Action[this.inputSource].onUpdate += new SteamVR_Action_Vector3.UpdateHandler(this.SteamVR_Behaviour_Vector3_OnUpdate);
      this.vector3Action[this.inputSource].onChange += new SteamVR_Action_Vector3.ChangeHandler(this.SteamVR_Behaviour_Vector3_OnChange);
      this.vector3Action[this.inputSource].onAxis += new SteamVR_Action_Vector3.AxisHandler(this.SteamVR_Behaviour_Vector3_OnAxis);
    }

    protected void RemoveHandlers()
    {
      if (!((SteamVR_Action) this.vector3Action != (SteamVR_Action) null))
        return;
      this.vector3Action[this.inputSource].onUpdate -= new SteamVR_Action_Vector3.UpdateHandler(this.SteamVR_Behaviour_Vector3_OnUpdate);
      this.vector3Action[this.inputSource].onChange -= new SteamVR_Action_Vector3.ChangeHandler(this.SteamVR_Behaviour_Vector3_OnChange);
      this.vector3Action[this.inputSource].onAxis -= new SteamVR_Action_Vector3.AxisHandler(this.SteamVR_Behaviour_Vector3_OnAxis);
    }

    private void SteamVR_Behaviour_Vector3_OnUpdate(
      SteamVR_Action_Vector3 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector3 newAxis,
      Vector3 newDelta)
    {
      if (this.onUpdate != null)
        this.onUpdate.Invoke(this, fromSource, newAxis, newDelta);
      if (this.onUpdateEvent == null)
        return;
      this.onUpdateEvent(this, fromSource, newAxis, newDelta);
    }

    private void SteamVR_Behaviour_Vector3_OnChange(
      SteamVR_Action_Vector3 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector3 newAxis,
      Vector3 newDelta)
    {
      if (this.onChange != null)
        this.onChange.Invoke(this, fromSource, newAxis, newDelta);
      if (this.onChangeEvent == null)
        return;
      this.onChangeEvent(this, fromSource, newAxis, newDelta);
    }

    private void SteamVR_Behaviour_Vector3_OnAxis(
      SteamVR_Action_Vector3 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector3 newAxis,
      Vector3 newDelta)
    {
      if (this.onAxis != null)
        this.onAxis.Invoke(this, fromSource, newAxis, newDelta);
      if (this.onAxisEvent == null)
        return;
      this.onAxisEvent(this, fromSource, newAxis, newDelta);
    }

    public string GetLocalizedName(params EVRInputStringBits[] localizedParts) => (SteamVR_Action) this.vector3Action != (SteamVR_Action) null ? this.vector3Action.GetLocalizedOriginPart(this.inputSource, localizedParts) : (string) null;

    public delegate void AxisHandler(
      SteamVR_Behaviour_Vector3 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector3 newAxis,
      Vector3 newDelta);

    public delegate void ChangeHandler(
      SteamVR_Behaviour_Vector3 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector3 newAxis,
      Vector3 newDelta);

    public delegate void UpdateHandler(
      SteamVR_Behaviour_Vector3 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector3 newAxis,
      Vector3 newDelta);
  }
}
