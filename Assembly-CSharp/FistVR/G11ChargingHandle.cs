// Decompiled with JetBrains decompiler
// Type: FistVR.G11ChargingHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class G11ChargingHandle : FVRInteractiveObject
  {
    public OpenBoltReceiver Weapon;
    public Transform RotPiece;
    public AudioEvent HandleCrank;
    private float m_curRot;
    public Transform FlapPiece;
    private Vector3 lastHandForward = Vector3.zero;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.SetFlapState(true);
      this.lastHandForward = Vector3.ProjectOnPlane(this.m_hand.transform.up, this.transform.right);
    }

    public override void EndInteraction(FVRViveHand hand) => base.EndInteraction(hand);

    private void SetFlapState(bool isOut)
    {
      if (isOut)
        this.FlapPiece.localEulerAngles = new Vector3(0.0f, 90f, -90f);
      else
        this.FlapPiece.localEulerAngles = new Vector3(0.0f, 90f, 0.0f);
    }

    protected override void FVRFixedUpdate()
    {
      if (this.IsHeld)
      {
        float curRot = this.m_curRot;
        Vector3 lhs = Vector3.ProjectOnPlane(this.m_hand.transform.up, -this.transform.right);
        Vector3 rhs = Vector3.ProjectOnPlane(this.lastHandForward, -this.transform.right);
        float num = Mathf.Atan2(Vector3.Dot(-this.transform.right, Vector3.Cross(lhs, rhs)), Vector3.Dot(lhs, rhs)) * 57.29578f;
        if ((double) num > 0.0)
          this.m_curRot -= num;
        if ((double) curRot > -180.0 && (double) this.m_curRot <= -180.0)
          SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.HandleCrank, this.transform.position);
        if ((double) this.m_curRot <= -360.0)
        {
          this.RotPiece.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
          this.Weapon.Bolt.SetBoltToRear();
          SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.HandleCrank, this.transform.position);
          this.m_curRot = 0.0f;
          this.SetFlapState(false);
          this.m_hand.EndInteractionIfHeld((FVRInteractiveObject) this);
          this.ForceBreakInteraction();
        }
        else
          this.RotPiece.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_curRot);
        this.lastHandForward = lhs;
      }
      base.FVRFixedUpdate();
    }
  }
}
