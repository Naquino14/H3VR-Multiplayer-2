// Decompiled with JetBrains decompiler
// Type: FistVR.BoltActionRifle_Handle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BoltActionRifle_Handle : FVRInteractiveObject
  {
    public BoltActionRifle Rifle;
    public bool UsesQuickRelease;
    public Transform BoltActionHandleRoot;
    public Transform BoltActionHandle;
    public float BaseRotOffset;
    private float rotAngle;
    public float MinRot;
    public float MaxRot;
    public float UnlockThreshold = 70f;
    public Transform Point_Forward;
    public Transform Point_Rearward;
    public Vector3 HandPosOffset = new Vector3(0.0f, 0.0f, 0.0f);
    private bool m_wasTPInitiated;
    public bool UsesExtraRotationPiece;
    public Transform ExtraRotationPiece;
    public BoltActionRifle_Handle.BoltActionHandleState HandleState;
    public BoltActionRifle_Handle.BoltActionHandleState LastHandleState;
    public BoltActionRifle_Handle.BoltActionHandleRot HandleRot = BoltActionRifle_Handle.BoltActionHandleRot.Down;
    public BoltActionRifle_Handle.BoltActionHandleRot LastHandleRot = BoltActionRifle_Handle.BoltActionHandleRot.Down;
    private Vector3 m_localHandPos_BoltDown;
    private Vector3 m_localHandPos_BoltUp;
    private Vector3 m_localHandPos_BoltBack;
    private float fakeBoltDrive;

    protected override void Awake()
    {
      base.Awake();
      this.CalculateHandPoses();
    }

    public void TPInitiate() => this.m_wasTPInitiated = true;

    public override void EndInteraction(FVRViveHand hand)
    {
      this.m_wasTPInitiated = false;
      base.EndInteraction(hand);
    }

    public override bool IsInteractable() => this.Rifle.CanBoltMove() && base.IsInteractable();

    private void CalculateHandPoses()
    {
      this.m_localHandPos_BoltDown = this.Rifle.transform.InverseTransformPoint(this.transform.position);
      Vector3 vector3 = this.transform.position - this.BoltActionHandleRoot.position;
      Vector3 position = Quaternion.AngleAxis(Mathf.Abs(this.MaxRot - this.MinRot) + 10f, this.BoltActionHandleRoot.forward) * vector3 + this.BoltActionHandleRoot.position;
      this.m_localHandPos_BoltUp = this.Rifle.transform.InverseTransformPoint(position);
      this.m_localHandPos_BoltBack = this.Rifle.transform.InverseTransformPoint(position + -this.BoltActionHandleRoot.forward * (0.005f + Vector3.Distance(this.Point_Forward.position, this.Point_Rearward.position)));
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      Debug.DrawLine(this.Rifle.transform.TransformPoint(this.m_localHandPos_BoltDown), this.Rifle.transform.TransformPoint(this.m_localHandPos_BoltUp), Color.red);
      Debug.DrawLine(this.Rifle.transform.TransformPoint(this.m_localHandPos_BoltUp), this.Rifle.transform.TransformPoint(this.m_localHandPos_BoltBack), Color.blue);
    }

    public void DriveBolt(float amount)
    {
      if ((Object) this.Rifle.Clip != (Object) null)
      {
        this.Rifle.EjectClip();
      }
      else
      {
        this.fakeBoltDrive += amount;
        this.fakeBoltDrive = Mathf.Clamp(this.fakeBoltDrive, 0.0f, 1f);
        this.ManipulateBoltUsingPosition(this.Rifle.transform.TransformPoint((double) this.fakeBoltDrive >= 0.5 ? Vector3.Lerp(this.m_localHandPos_BoltUp, this.m_localHandPos_BoltBack, (float) (((double) this.fakeBoltDrive - 0.5) * 2.0)) : Vector3.Slerp(this.m_localHandPos_BoltDown, this.m_localHandPos_BoltUp, this.fakeBoltDrive * 2f)), true);
        float lerp = Mathf.InverseLerp(this.Point_Forward.localPosition.z, this.Point_Rearward.localPosition.z, this.BoltActionHandleRoot.localPosition.z);
        this.HandleState = (double) lerp >= 0.0500000007450581 ? ((double) lerp <= 0.949999988079071 ? BoltActionRifle_Handle.BoltActionHandleState.Mid : BoltActionRifle_Handle.BoltActionHandleState.Rear) : BoltActionRifle_Handle.BoltActionHandleState.Forward;
        this.Rifle.UpdateBolt(this.HandleState, lerp);
        this.LastHandleState = this.HandleState;
      }
    }

    private bool ManipulateBoltUsingPosition(Vector3 pos, bool touchpadDrive)
    {
      bool flag = false;
      if (this.HandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward)
      {
        Vector3 rhs = Vector3.ProjectOnPlane(pos - this.BoltActionHandle.position, this.BoltActionHandleRoot.transform.forward);
        rhs = rhs.normalized;
        Vector3 right = this.BoltActionHandleRoot.transform.right;
        this.rotAngle = Mathf.Atan2(Vector3.Dot(this.BoltActionHandleRoot.forward, Vector3.Cross(right, rhs)), Vector3.Dot(right, rhs)) * 57.29578f;
        this.rotAngle += this.BaseRotOffset;
        this.rotAngle = Mathf.Clamp(this.rotAngle, this.MinRot, this.MaxRot);
        this.BoltActionHandle.localEulerAngles = new Vector3(0.0f, 0.0f, this.rotAngle);
        if (this.UsesExtraRotationPiece)
          this.ExtraRotationPiece.localEulerAngles = new Vector3(0.0f, 0.0f, this.rotAngle);
        this.HandleRot = (double) this.rotAngle < (double) this.UnlockThreshold ? ((double) Mathf.Abs(this.rotAngle - this.MinRot) >= 2.0 ? BoltActionRifle_Handle.BoltActionHandleRot.Mid : BoltActionRifle_Handle.BoltActionHandleRot.Down) : BoltActionRifle_Handle.BoltActionHandleRot.Up;
        if (this.HandleRot == BoltActionRifle_Handle.BoltActionHandleRot.Up && this.LastHandleRot != BoltActionRifle_Handle.BoltActionHandleRot.Up)
        {
          this.Rifle.PlayAudioEvent(FirearmAudioEventType.HandleUp);
          if (this.Rifle.CockType == BoltActionRifle.HammerCockType.OnUp)
            this.Rifle.CockHammer();
        }
        else if (this.HandleRot == BoltActionRifle_Handle.BoltActionHandleRot.Down && this.LastHandleRot != BoltActionRifle_Handle.BoltActionHandleRot.Down)
        {
          this.Rifle.PlayAudioEvent(FirearmAudioEventType.HandleDown);
          if (this.Rifle.CockType == BoltActionRifle.HammerCockType.OnClose)
            this.Rifle.CockHammer();
          flag = true;
        }
        this.LastHandleRot = this.HandleRot;
      }
      if ((double) this.rotAngle >= (double) this.UnlockThreshold)
      {
        Vector3 vector3 = this.HandPosOffset.x * this.BoltActionHandleRoot.right + this.HandPosOffset.y * this.BoltActionHandleRoot.up + this.HandPosOffset.z * this.BoltActionHandleRoot.forward;
        this.BoltActionHandleRoot.position = this.GetClosestValidPoint(this.Point_Forward.position, this.Point_Rearward.position, pos - vector3);
      }
      return flag;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      if ((Object) this.Rifle.Clip != (Object) null)
      {
        this.Rifle.EjectClip();
      }
      else
      {
        bool flag = this.ManipulateBoltUsingPosition(hand.transform.position, false);
        float lerp = Mathf.InverseLerp(this.Point_Forward.localPosition.z, this.Point_Rearward.localPosition.z, this.BoltActionHandleRoot.localPosition.z);
        this.HandleState = (double) lerp >= 0.0500000007450581 ? ((double) lerp <= 0.949999988079071 ? BoltActionRifle_Handle.BoltActionHandleState.Mid : BoltActionRifle_Handle.BoltActionHandleState.Rear) : BoltActionRifle_Handle.BoltActionHandleState.Forward;
        if (this.HandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward && this.LastHandleState != BoltActionRifle_Handle.BoltActionHandleState.Forward || (this.HandleState != BoltActionRifle_Handle.BoltActionHandleState.Rear || this.LastHandleState == BoltActionRifle_Handle.BoltActionHandleState.Rear))
          ;
        this.Rifle.UpdateBolt(this.HandleState, lerp);
        this.LastHandleState = this.HandleState;
        base.UpdateInteraction(hand);
        if (!flag || !this.UsesQuickRelease || !this.m_wasTPInitiated || !this.Rifle.IsAltHeld && this.Rifle.IsHeld)
          return;
        hand.Buzz(hand.Buzzer.Buzz_BeginInteraction);
        this.EndInteraction(hand);
        this.Rifle.BeginInteraction(hand);
        hand.ForceSetInteractable((FVRInteractiveObject) this.Rifle);
        if (hand.Input.TriggerPressed)
          return;
        this.Rifle.SetHasTriggeredUp();
      }
    }

    public enum BoltActionHandleState
    {
      Forward,
      Mid,
      Rear,
    }

    public enum BoltActionHandleRot
    {
      Up,
      Mid,
      Down,
    }
  }
}
