// Decompiled with JetBrains decompiler
// Type: FistVR.BangerDial
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BangerDial : FVRInteractiveObject
  {
    public Banger Banger;
    public Transform Root;
    public float DialTick;
    private bool m_isPrimed;
    private bool m_hasDinged;
    public AudioEvent AudEvent_Ding;
    public AudioEvent AudEvent_WindTick;
    public AudioEvent AudEvent_TickDown;
    private int lastTickDown;
    private Vector3 lastHandForward = Vector3.zero;
    private Vector3 lastMountForward = Vector3.zero;
    private int lastWindTickRound;

    public void TickDown()
    {
      if (!this.m_isPrimed || this.IsHeld)
        return;
      this.DialTick -= Time.deltaTime;
      int roundedToFactor = this.GetRoundedToFactor(this.DialTick, 1f);
      if (roundedToFactor != this.lastTickDown)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_TickDown, this.transform.position);
      this.lastTickDown = roundedToFactor;
      if ((double) this.DialTick > 0.0 || this.m_hasDinged)
        return;
      this.m_hasDinged = true;
      this.Banger.StartExploding();
      SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.AudEvent_Ding, this.transform.position, Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.transform.position) / 343f);
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      this.lastHandForward = hand.transform.up;
      this.lastMountForward = this.Root.transform.up;
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (this.IsHeld)
      {
        this.m_isPrimed = true;
        this.m_hasDinged = false;
        float dialTick = this.DialTick;
        Vector3 lhs1 = Vector3.ProjectOnPlane(this.m_hand.transform.up, this.transform.forward);
        Vector3 rhs1 = Vector3.ProjectOnPlane(this.lastHandForward, this.transform.forward);
        this.DialTick += Mathf.Atan2(Vector3.Dot(this.transform.forward, Vector3.Cross(lhs1, rhs1)), Vector3.Dot(lhs1, rhs1)) * 57.29578f * 0.2f;
        Vector3 lhs2 = Vector3.ProjectOnPlane(this.Root.transform.up, this.transform.forward);
        Vector3 rhs2 = Vector3.ProjectOnPlane(this.lastMountForward, this.transform.forward);
        this.DialTick += Mathf.Atan2(Vector3.Dot(this.transform.forward, Vector3.Cross(lhs2, rhs2)), Vector3.Dot(lhs2, rhs2)) * 57.29578f * 0.2f;
        this.DialTick = Mathf.Clamp(this.DialTick, 0.0f, 60f);
        int roundedToFactor = this.GetRoundedToFactor(this.DialTick, 5f);
        if (roundedToFactor != this.lastWindTickRound)
          SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_WindTick, this.transform.position);
        this.lastHandForward = this.m_hand.transform.up;
        this.lastMountForward = this.Root.transform.up;
        this.lastTickDown = this.GetRoundedToFactor(this.DialTick, 1f);
        this.lastWindTickRound = roundedToFactor;
      }
      this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, (float) (-(double) this.DialTick * 6.0));
    }

    private int GetRoundedToFactor(float input, float factor) => Mathf.RoundToInt(input / factor) * (int) factor;
  }
}
