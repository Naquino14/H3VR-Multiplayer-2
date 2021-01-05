// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.BuggyController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem.Sample
{
  public class BuggyController : MonoBehaviour
  {
    public Transform modelJoystick;
    public float joystickRot = 20f;
    public Transform modelTrigger;
    public float triggerRot = 20f;
    public BuggyBuddy buggy;
    public Transform buttonBrake;
    public Transform buttonReset;
    public Canvas ui_Canvas;
    public Image ui_rpm;
    public Image ui_speed;
    public RectTransform ui_steer;
    public float ui_steerangle;
    public Vector2 ui_fillAngles;
    public Transform resetToPoint;
    public SteamVR_Action_Vector2 actionSteering = SteamVR_Input.GetAction<SteamVR_Action_Vector2>(nameof (buggy), "Steering");
    public SteamVR_Action_Single actionThrottle = SteamVR_Input.GetAction<SteamVR_Action_Single>(nameof (buggy), "Throttle");
    public SteamVR_Action_Boolean actionBrake = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(nameof (buggy), "Brake");
    public SteamVR_Action_Boolean actionReset = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(nameof (buggy), "Reset");
    private float usteer;
    private Interactable interactable;
    private Quaternion trigSRot;
    private Quaternion joySRot;
    private Coroutine resettingRoutine;
    private Vector3 initialScale;
    private float buzztimer;

    private void Start()
    {
      this.joySRot = this.modelJoystick.localRotation;
      this.trigSRot = this.modelTrigger.localRotation;
      this.interactable = this.GetComponent<Interactable>();
      this.StartCoroutine(this.DoBuzz());
      this.buggy.controllerReference = this.transform;
      this.initialScale = this.buggy.transform.localScale;
    }

    private void Update()
    {
      Vector2 vector2 = Vector2.zero;
      float t = 0.0f;
      float num = 0.0f;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      if ((bool) (Object) this.interactable.attachedToHand)
      {
        SteamVR_Input_Sources handType = this.interactable.attachedToHand.handType;
        vector2 = this.actionSteering.GetAxis(handType);
        t = this.actionThrottle.GetAxis(handType);
        flag2 = this.actionBrake.GetState(handType);
        flag3 = this.actionReset.GetState(handType);
        num = !flag2 ? 0.0f : 1f;
        flag1 = this.actionReset.GetStateDown(handType);
      }
      if (flag1 && this.resettingRoutine == null)
        this.resettingRoutine = this.StartCoroutine(this.DoReset());
      if ((Object) this.ui_Canvas != (Object) null)
      {
        this.ui_Canvas.gameObject.SetActive((bool) (Object) this.interactable.attachedToHand);
        this.usteer = Mathf.Lerp(this.usteer, vector2.x, Time.deltaTime * 9f);
        this.ui_steer.localEulerAngles = Vector3.forward * this.usteer * -this.ui_steerangle;
        this.ui_rpm.fillAmount = Mathf.Lerp(this.ui_rpm.fillAmount, Mathf.Lerp(this.ui_fillAngles.x, this.ui_fillAngles.y, t), Time.deltaTime * 4f);
        this.ui_speed.fillAmount = Mathf.Lerp(this.ui_fillAngles.x, this.ui_fillAngles.y, 1f - Mathf.Exp(-this.buggy.speed / 40f));
      }
      this.modelJoystick.localRotation = this.joySRot;
      this.modelJoystick.Rotate(vector2.y * -this.joystickRot, vector2.x * -this.joystickRot, 0.0f, Space.Self);
      this.modelTrigger.localRotation = this.trigSRot;
      this.modelTrigger.Rotate(t * -this.triggerRot, 0.0f, 0.0f, Space.Self);
      this.buttonBrake.localScale = new Vector3(1f, 1f, !flag2 ? 1f : 0.4f);
      this.buttonReset.localScale = new Vector3(1f, 1f, !flag3 ? 1f : 0.4f);
      this.buggy.steer = vector2;
      this.buggy.throttle = t;
      this.buggy.handBrake = num;
      this.buggy.controllerReference = this.transform;
    }

    [DebuggerHidden]
    private IEnumerator DoReset() => (IEnumerator) new BuggyController.\u003CDoReset\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    [DebuggerHidden]
    private IEnumerator DoBuzz() => (IEnumerator) new BuggyController.\u003CDoBuzz\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }
}
