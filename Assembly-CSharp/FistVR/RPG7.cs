// Decompiled with JetBrains decompiler
// Type: FistVR.RPG7
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RPG7 : FVRFireArm
  {
    [Header("RPG-7 Config")]
    public RPG7Foregrip RPGForegrip;
    public FVRFireArmChamber Chamber;
    public Transform Trigger;
    public Vector2 TriggerRots;
    public Transform Hammer;
    public Vector2 HammerRots;
    private bool m_isHammerCocked;

    public override int GetTutorialState()
    {
      if (!this.Chamber.IsFull)
        return 0;
      if ((Object) this.AltGrip == (Object) null)
        return 1;
      return !this.m_isHammerCocked ? 2 : 3;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.IsAltHeld)
        return;
      bool flag = false;
      if (hand.IsInStreamlinedMode && hand.Input.AXButtonDown)
        flag = true;
      else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown)
        flag = true;
      if (flag)
        this.CockHammer();
      if (hand.Input.TriggerDown)
        this.Fire();
      this.UpdateTriggerRot(hand.Input.TriggerFloat);
    }

    public void UpdateTriggerRot(float f) => this.Trigger.localEulerAngles = new Vector3(Mathf.Lerp(this.TriggerRots.x, this.TriggerRots.y, f), 0.0f, 0.0f);

    public void CockHammer()
    {
      if (this.m_isHammerCocked)
        return;
      this.PlayAudioEvent(FirearmAudioEventType.Prefire);
      this.m_isHammerCocked = true;
      this.Hammer.localEulerAngles = new Vector3(this.HammerRots.y, 0.0f, 0.0f);
    }

    public void Fire()
    {
      if (!this.m_isHammerCocked)
        return;
      this.m_isHammerCocked = false;
      this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
      this.Hammer.localEulerAngles = new Vector3(this.HammerRots.x, 0.0f, 0.0f);
      if (!this.Chamber.IsFull || this.Chamber.IsSpent)
        return;
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      this.Recoil(this.IsTwoHandStabilized(), (Object) this.AltGrip != (Object) null, this.IsShoulderStabilized());
      this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      this.PlayAudioEvent(FirearmAudioEventType.Shots_Suppressed);
      this.Chamber.SetRound((FVRFireArmRound) null);
    }
  }
}
