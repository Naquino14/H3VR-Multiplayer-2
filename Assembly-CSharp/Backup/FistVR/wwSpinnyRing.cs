// Decompiled with JetBrains decompiler
// Type: FistVR.wwSpinnyRing
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwSpinnyRing : FVRInteractiveObject
  {
    public wwSpinnyPuzzle Puzzle;
    private bool m_isLocked;
    public Transform LockPos;
    private float m_speed;
    private Vector3 m_lastForward = Vector3.zero;
    public AudioSource Aud;

    public override bool IsInteractable() => !this.m_isLocked && base.IsInteractable();

    protected override void Awake()
    {
      base.Awake();
      this.m_lastForward = this.transform.forward;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 forward = Vector3.ProjectOnPlane(hand.transform.position - this.transform.position, this.transform.up);
      forward.Normalize();
      this.transform.rotation = Quaternion.LookRotation(forward, this.transform.up);
      this.m_speed = Vector3.Angle(this.m_lastForward, this.transform.forward) * Time.deltaTime;
      this.m_lastForward = this.transform.forward;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      if (this.m_isLocked || (double) Vector3.Angle(this.transform.forward, this.LockPos.forward) >= 5.0)
        return;
      this.LockPiece(true);
    }

    public void LockPiece(bool stateEvent)
    {
      this.m_isLocked = true;
      this.transform.localEulerAngles = this.LockPos.localEulerAngles;
      this.transform.localPosition = new Vector3(0.0f, -0.03f, 0.0f);
      if (stateEvent)
        this.Aud.Play();
      this.Puzzle.PieceLocked();
    }
  }
}
