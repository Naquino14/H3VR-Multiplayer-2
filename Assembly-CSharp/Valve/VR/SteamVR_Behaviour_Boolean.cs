// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Behaviour_Boolean
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Behaviour_Boolean : MonoBehaviour
  {
    [Tooltip("The SteamVR boolean action that this component should use")]
    public SteamVR_Action_Boolean booleanAction;
    [Tooltip("The device this action should apply to. Any if the action is not device specific.")]
    public SteamVR_Input_Sources inputSource;
    public SteamVR_Behaviour_BooleanEvent onChange;
    public SteamVR_Behaviour_BooleanEvent onUpdate;
    public SteamVR_Behaviour_BooleanEvent onPress;
    public SteamVR_Behaviour_BooleanEvent onPressDown;
    public SteamVR_Behaviour_BooleanEvent onPressUp;

    public event SteamVR_Behaviour_Boolean.ChangeHandler onChangeEvent;

    public event SteamVR_Behaviour_Boolean.UpdateHandler onUpdateEvent;

    public event SteamVR_Behaviour_Boolean.StateHandler onPressEvent;

    public event SteamVR_Behaviour_Boolean.StateDownHandler onPressDownEvent;

    public event SteamVR_Behaviour_Boolean.StateUpHandler onPressUpEvent;

    public bool isActive => this.booleanAction[this.inputSource].active;

    public SteamVR_ActionSet actionSet => (SteamVR_Action) this.booleanAction != (SteamVR_Action) null ? this.booleanAction.actionSet : (SteamVR_ActionSet) null;

    protected virtual void OnEnable()
    {
      if ((SteamVR_Action) this.booleanAction == (SteamVR_Action) null)
        Debug.LogError((object) "[SteamVR] Boolean action not set.", (Object) this);
      else
        this.AddHandlers();
    }

    protected virtual void OnDisable() => this.RemoveHandlers();

    protected void AddHandlers()
    {
      this.booleanAction[this.inputSource].onUpdate += new SteamVR_Action_Boolean.UpdateHandler(this.SteamVR_Behaviour_Boolean_OnUpdate);
      this.booleanAction[this.inputSource].onChange += new SteamVR_Action_Boolean.ChangeHandler(this.SteamVR_Behaviour_Boolean_OnChange);
      this.booleanAction[this.inputSource].onState += new SteamVR_Action_Boolean.StateHandler(this.SteamVR_Behaviour_Boolean_OnState);
      this.booleanAction[this.inputSource].onStateDown += new SteamVR_Action_Boolean.StateDownHandler(this.SteamVR_Behaviour_Boolean_OnStateDown);
      this.booleanAction[this.inputSource].onStateUp += new SteamVR_Action_Boolean.StateUpHandler(this.SteamVR_Behaviour_Boolean_OnStateUp);
    }

    protected void RemoveHandlers()
    {
      if (!((SteamVR_Action) this.booleanAction != (SteamVR_Action) null))
        return;
      this.booleanAction[this.inputSource].onUpdate -= new SteamVR_Action_Boolean.UpdateHandler(this.SteamVR_Behaviour_Boolean_OnUpdate);
      this.booleanAction[this.inputSource].onChange -= new SteamVR_Action_Boolean.ChangeHandler(this.SteamVR_Behaviour_Boolean_OnChange);
      this.booleanAction[this.inputSource].onState -= new SteamVR_Action_Boolean.StateHandler(this.SteamVR_Behaviour_Boolean_OnState);
      this.booleanAction[this.inputSource].onStateDown -= new SteamVR_Action_Boolean.StateDownHandler(this.SteamVR_Behaviour_Boolean_OnStateDown);
      this.booleanAction[this.inputSource].onStateUp -= new SteamVR_Action_Boolean.StateUpHandler(this.SteamVR_Behaviour_Boolean_OnStateUp);
    }

    private void SteamVR_Behaviour_Boolean_OnStateUp(
      SteamVR_Action_Boolean fromAction,
      SteamVR_Input_Sources fromSource)
    {
      if (this.onPressUp != null)
        this.onPressUp.Invoke(this, fromSource, false);
      if (this.onPressUpEvent == null)
        return;
      this.onPressUpEvent(this, fromSource);
    }

    private void SteamVR_Behaviour_Boolean_OnStateDown(
      SteamVR_Action_Boolean fromAction,
      SteamVR_Input_Sources fromSource)
    {
      if (this.onPressDown != null)
        this.onPressDown.Invoke(this, fromSource, true);
      if (this.onPressDownEvent == null)
        return;
      this.onPressDownEvent(this, fromSource);
    }

    private void SteamVR_Behaviour_Boolean_OnState(
      SteamVR_Action_Boolean fromAction,
      SteamVR_Input_Sources fromSource)
    {
      if (this.onPress != null)
        this.onPress.Invoke(this, fromSource, true);
      if (this.onPressEvent == null)
        return;
      this.onPressEvent(this, fromSource);
    }

    private void SteamVR_Behaviour_Boolean_OnUpdate(
      SteamVR_Action_Boolean fromAction,
      SteamVR_Input_Sources fromSource,
      bool newState)
    {
      if (this.onUpdate != null)
        this.onUpdate.Invoke(this, fromSource, newState);
      if (this.onUpdateEvent == null)
        return;
      this.onUpdateEvent(this, fromSource, newState);
    }

    private void SteamVR_Behaviour_Boolean_OnChange(
      SteamVR_Action_Boolean fromAction,
      SteamVR_Input_Sources fromSource,
      bool newState)
    {
      if (this.onChange != null)
        this.onChange.Invoke(this, fromSource, newState);
      if (this.onChangeEvent == null)
        return;
      this.onChangeEvent(this, fromSource, newState);
    }

    public string GetLocalizedName(params EVRInputStringBits[] localizedParts) => (SteamVR_Action) this.booleanAction != (SteamVR_Action) null ? this.booleanAction.GetLocalizedOriginPart(this.inputSource, localizedParts) : (string) null;

    public delegate void StateDownHandler(
      SteamVR_Behaviour_Boolean fromAction,
      SteamVR_Input_Sources fromSource);

    public delegate void StateUpHandler(
      SteamVR_Behaviour_Boolean fromAction,
      SteamVR_Input_Sources fromSource);

    public delegate void StateHandler(
      SteamVR_Behaviour_Boolean fromAction,
      SteamVR_Input_Sources fromSource);

    public delegate void ActiveChangeHandler(
      SteamVR_Behaviour_Boolean fromAction,
      SteamVR_Input_Sources fromSource,
      bool active);

    public delegate void ChangeHandler(
      SteamVR_Behaviour_Boolean fromAction,
      SteamVR_Input_Sources fromSource,
      bool newState);

    public delegate void UpdateHandler(
      SteamVR_Behaviour_Boolean fromAction,
      SteamVR_Input_Sources fromSource,
      bool newState);
  }
}
