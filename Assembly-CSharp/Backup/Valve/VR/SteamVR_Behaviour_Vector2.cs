// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Behaviour_Vector2
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Behaviour_Vector2 : MonoBehaviour
  {
    public SteamVR_Action_Vector2 vector2Action;
    [Tooltip("The device this action should apply to. Any if the action is not device specific.")]
    public SteamVR_Input_Sources inputSource;
    [Tooltip("Fires whenever the action's value has changed since the last update.")]
    public SteamVR_Behaviour_Vector2Event onChange;
    [Tooltip("Fires whenever the action's value has been updated.")]
    public SteamVR_Behaviour_Vector2Event onUpdate;
    [Tooltip("Fires whenever the action's value has been updated and is non-zero.")]
    public SteamVR_Behaviour_Vector2Event onAxis;
    public SteamVR_Behaviour_Vector2.ChangeHandler onChangeEvent;
    public SteamVR_Behaviour_Vector2.UpdateHandler onUpdateEvent;
    public SteamVR_Behaviour_Vector2.AxisHandler onAxisEvent;

    public bool isActive => this.vector2Action.GetActive(this.inputSource);

    protected virtual void OnEnable()
    {
      if ((SteamVR_Action) this.vector2Action == (SteamVR_Action) null)
        Debug.LogError((object) "[SteamVR] Vector2 action not set.", (Object) this);
      else
        this.AddHandlers();
    }

    protected virtual void OnDisable() => this.RemoveHandlers();

    protected void AddHandlers()
    {
      this.vector2Action[this.inputSource].onUpdate += new SteamVR_Action_Vector2.UpdateHandler(this.SteamVR_Behaviour_Vector2_OnUpdate);
      this.vector2Action[this.inputSource].onChange += new SteamVR_Action_Vector2.ChangeHandler(this.SteamVR_Behaviour_Vector2_OnChange);
      this.vector2Action[this.inputSource].onAxis += new SteamVR_Action_Vector2.AxisHandler(this.SteamVR_Behaviour_Vector2_OnAxis);
    }

    protected void RemoveHandlers()
    {
      if (!((SteamVR_Action) this.vector2Action != (SteamVR_Action) null))
        return;
      this.vector2Action[this.inputSource].onUpdate -= new SteamVR_Action_Vector2.UpdateHandler(this.SteamVR_Behaviour_Vector2_OnUpdate);
      this.vector2Action[this.inputSource].onChange -= new SteamVR_Action_Vector2.ChangeHandler(this.SteamVR_Behaviour_Vector2_OnChange);
      this.vector2Action[this.inputSource].onAxis -= new SteamVR_Action_Vector2.AxisHandler(this.SteamVR_Behaviour_Vector2_OnAxis);
    }

    private void SteamVR_Behaviour_Vector2_OnUpdate(
      SteamVR_Action_Vector2 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector2 newAxis,
      Vector2 newDelta)
    {
      if (this.onUpdate != null)
        this.onUpdate.Invoke(this, fromSource, newAxis, newDelta);
      if (this.onUpdateEvent == null)
        return;
      this.onUpdateEvent(this, fromSource, newAxis, newDelta);
    }

    private void SteamVR_Behaviour_Vector2_OnChange(
      SteamVR_Action_Vector2 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector2 newAxis,
      Vector2 newDelta)
    {
      if (this.onChange != null)
        this.onChange.Invoke(this, fromSource, newAxis, newDelta);
      if (this.onChangeEvent == null)
        return;
      this.onChangeEvent(this, fromSource, newAxis, newDelta);
    }

    private void SteamVR_Behaviour_Vector2_OnAxis(
      SteamVR_Action_Vector2 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector2 newAxis,
      Vector2 newDelta)
    {
      if (this.onAxis != null)
        this.onAxis.Invoke(this, fromSource, newAxis, newDelta);
      if (this.onAxisEvent == null)
        return;
      this.onAxisEvent(this, fromSource, newAxis, newDelta);
    }

    public string GetLocalizedName(params EVRInputStringBits[] localizedParts) => (SteamVR_Action) this.vector2Action != (SteamVR_Action) null ? this.vector2Action.GetLocalizedOriginPart(this.inputSource, localizedParts) : (string) null;

    public delegate void AxisHandler(
      SteamVR_Behaviour_Vector2 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector2 newAxis,
      Vector2 newDelta);

    public delegate void ChangeHandler(
      SteamVR_Behaviour_Vector2 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector2 newAxis,
      Vector2 newDelta);

    public delegate void UpdateHandler(
      SteamVR_Behaviour_Vector2 fromAction,
      SteamVR_Input_Sources fromSource,
      Vector2 newAxis,
      Vector2 newDelta);
  }
}
