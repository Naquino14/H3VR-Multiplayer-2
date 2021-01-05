// Decompiled with JetBrains decompiler
// Type: FistVR.SimpleLauncher
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SimpleLauncher : FVRFireArm
  {
    [Header("Simple Launcher Config")]
    public FVRFireArmChamber Chamber;
    public bool HasTrigger = true;
    public bool AlsoPlaysSuppressedSound = true;
    public bool DeletesCartridgeOnFire = true;
    public bool FireOnCol;
    public float ColThresh = 1f;
    public Collider HammerCol;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.IsAltHeld || !hand.Input.TriggerDown || (!this.m_hasTriggeredUpSinceBegin || !this.HasTrigger))
        return;
      this.Fire();
    }

    public void Fire()
    {
      if (!this.Chamber.IsFull || this.Chamber.IsSpent)
        return;
      this.Chamber.Fire();
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      this.Recoil(this.IsTwoHandStabilized(), (Object) this.AltGrip != (Object) null, this.IsShoulderStabilized());
      this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      if (this.AlsoPlaysSuppressedSound)
        this.PlayAudioEvent(FirearmAudioEventType.Shots_Suppressed);
      if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
      {
        this.Chamber.IsSpent = false;
        this.Chamber.UpdateProxyDisplay();
      }
      else
      {
        if (!this.DeletesCartridgeOnFire)
          return;
        this.Chamber.SetRound((FVRFireArmRound) null);
      }
    }

    public override void OnCollisionEnter(Collision col)
    {
      base.OnCollisionEnter(col);
      if (!this.FireOnCol || ((double) col.relativeVelocity.magnitude <= (double) this.ColThresh || !((Object) col.contacts[0].thisCollider == (Object) this.HammerCol)))
        return;
      this.Fire();
    }
  }
}
