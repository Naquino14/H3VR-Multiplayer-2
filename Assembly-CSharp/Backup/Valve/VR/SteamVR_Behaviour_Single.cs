// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Behaviour_Single
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Behaviour_Single : MonoBehaviour
  {
    public SteamVR_Action_Single singleAction;
    [Tooltip("The device this action should apply to. Any if the action is not device specific.")]
    public SteamVR_Input_Sources inputSource;
    [Tooltip("Fires whenever the action's value has changed since the last update.")]
    public SteamVR_Behaviour_SingleEvent onChange;
    [Tooltip("Fires whenever the action's value has been updated.")]
    public SteamVR_Behaviour_SingleEvent onUpdate;
    [Tooltip("Fires whenever the action's value has been updated and is non-zero.")]
    public SteamVR_Behaviour_SingleEvent onAxis;
    public SteamVR_Behaviour_Single.ChangeHandler onChangeEvent;
    public SteamVR_Behaviour_Single.UpdateHandler onUpdateEvent;
    public SteamVR_Behaviour_Single.AxisHandler onAxisEvent;

    public bool isActive => this.singleAction.GetActive(this.inputSource);

    protected virtual void OnEnable()
    {
      if ((SteamVR_Action) this.singleAction == (SteamVR_Action) null)
        Debug.LogError((object) "[SteamVR] Single action not set.", (Object) this);
      else
        this.AddHandlers();
    }

    protected virtual void OnDisable() => this.RemoveHandlers();

    protected void AddHandlers()
    {
      this.singleAction[this.inputSource].onUpdate += new SteamVR_Action_Single.UpdateHandler(this.SteamVR_Behaviour_Single_OnUpdate);
      this.singleAction[this.inputSource].onChange += new SteamVR_Action_Single.ChangeHandler(this.SteamVR_Behaviour_Single_OnChange);
      this.singleAction[this.inputSource].onAxis += new SteamVR_Action_Single.AxisHandler(this.SteamVR_Behaviour_Single_OnAxis);
    }

    protected void RemoveHandlers()
    {
      if (!((SteamVR_Action) this.singleAction != (SteamVR_Action) null))
        return;
      this.singleAction[this.inputSource].onUpdate -= new SteamVR_Action_Single.UpdateHandler(this.SteamVR_Behaviour_Single_OnUpdate);
      this.singleAction[this.inputSource].onChange -= new SteamVR_Action_Single.ChangeHandler(this.SteamVR_Behaviour_Single_OnChange);
      this.singleAction[this.inputSource].onAxis -= new SteamVR_Action_Single.AxisHandler(this.SteamVR_Behaviour_Single_OnAxis);
    }

    private void SteamVR_Behaviour_Single_OnUpdate(
      SteamVR_Action_Single fromAction,
      SteamVR_Input_Sources fromSource,
      float newAxis,
      float newDelta)
    {
      if (this.onUpdate != null)
        this.onUpdate.Invoke(this, fromSource, newAxis, newDelta);
      if (this.onUpdateEvent == null)
        return;
      this.onUpdateEvent(this, fromSource, newAxis, newDelta);
    }

    private void SteamVR_Behaviour_Single_OnChange(
      SteamVR_Action_Single fromAction,
      SteamVR_Input_Sources fromSource,
      float newAxis,
      float newDelta)
    {
      if (this.onChange != null)
        this.onChange.Invoke(this, fromSource, newAxis, newDelta);
      if (this.onChangeEvent == null)
        return;
      this.onChangeEvent(this, fromSource, newAxis, newDelta);
    }

    private void SteamVR_Behaviour_Single_OnAxis(
      SteamVR_Action_Single fromAction,
      SteamVR_Input_Sources fromSource,
      float newAxis,
      float newDelta)
    {
      if (this.onAxis != null)
        this.onAxis.Invoke(this, fromSource, newAxis, newDelta);
      if (this.onAxisEvent == null)
        return;
      this.onAxisEvent(this, fromSource, newAxis, newDelta);
    }

    public string GetLocalizedName(params EVRInputStringBits[] localizedParts) => (SteamVR_Action) this.singleAction != (SteamVR_Action) null ? this.singleAction.GetLocalizedOriginPart(this.inputSource, localizedParts) : (string) null;

    public delegate void AxisHandler(
      SteamVR_Behaviour_Single fromAction,
      SteamVR_Input_Sources fromSource,
      float newAxis,
      float newDelta);

    public delegate void ChangeHandler(
      SteamVR_Behaviour_Single fromAction,
      SteamVR_Input_Sources fromSource,
      float newAxis,
      float newDelta);

    public delegate void UpdateHandler(
      SteamVR_Behaviour_Single fromAction,
      SteamVR_Input_Sources fromSource,
      float newAxis,
      float newDelta);
  }
}
