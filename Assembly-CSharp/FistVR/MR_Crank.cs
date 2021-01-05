// Decompiled with JetBrains decompiler
// Type: FistVR.MR_Crank
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MR_Crank : FVRInteractiveObject
  {
    private float m_crankedAmount;
    private Vector2 m_crankLimits = new Vector2(0.0f, 7200f);
    private Vector3 m_lastCrankDir;
    private Vector3 m_curCrankDir;
    public Transform YAxisPoint;
    public Transform SpikeTrap;
    public bool m_isCrankHeld;
    private float m_CrankDelta;
    public AudioSource CrankAudio;

    protected override void Awake()
    {
      base.Awake();
      this.m_lastCrankDir = this.transform.forward;
      this.m_curCrankDir = this.transform.forward;
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.m_isCrankHeld = true;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.m_curCrankDir = Vector3.ProjectOnPlane(hand.transform.position - this.transform.position, this.transform.up).normalized;
      Vector3 lastCrankDir = this.m_lastCrankDir;
      if (!this.Crank(-Mathf.Clamp(Mathf.Atan2(Vector3.Dot(this.transform.up, Vector3.Cross(lastCrankDir, this.m_curCrankDir)), Vector3.Dot(lastCrankDir, this.m_curCrankDir)) * 57.29578f, -1.2f, 1.2f)))
        return;
      this.transform.localEulerAngles = new Vector3(0.0f, -this.m_crankedAmount, 0.0f);
      this.m_lastCrankDir = this.transform.forward;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.m_isCrankHeld = false;
      base.EndInteraction(hand);
    }

    private bool Crank(float angle)
    {
      if ((double) this.m_crankedAmount + (double) angle <= (double) this.m_crankLimits.x || (double) this.m_crankedAmount + (double) angle >= (double) this.m_crankLimits.y)
        return false;
      this.m_crankedAmount += angle;
      this.m_crankedAmount = Mathf.Clamp(this.m_crankedAmount, 0.0f, 3600f);
      this.m_CrankDelta = Mathf.Abs(angle);
      this.SpikeTrap.transform.localPosition = new Vector3(0.0f, this.m_crankedAmount * (1f / 1000f), 0.0f);
      return true;
    }

    public void Update()
    {
      if (!this.m_isCrankHeld && (double) this.m_crankedAmount > 0.0 && (double) this.m_crankedAmount < 7000.0)
      {
        this.m_crankedAmount -= Time.deltaTime * 360f;
        this.m_crankedAmount = Mathf.Clamp(this.m_crankedAmount, 0.0f, 3600f);
        this.transform.localEulerAngles = new Vector3(0.0f, -this.m_crankedAmount, 0.0f);
        this.m_lastCrankDir = this.transform.forward;
        this.SpikeTrap.transform.localPosition = new Vector3(0.0f, this.m_crankedAmount * (1f / 1000f), 0.0f);
        this.m_CrankDelta = 1f;
        this.CrankAudio.pitch = 0.8f;
      }
      else
        this.CrankAudio.pitch = 0.5f;
      if ((double) this.m_CrankDelta > 0.0)
        this.m_CrankDelta -= Time.deltaTime * 6f;
      this.CrankAudio.volume = this.m_CrankDelta;
      if ((double) this.m_CrankDelta <= 0.0 && this.CrankAudio.isPlaying)
        this.CrankAudio.Stop();
      if ((double) this.m_CrankDelta <= 0.0 || this.CrankAudio.isPlaying)
        return;
      this.CrankAudio.Play();
    }
  }
}
