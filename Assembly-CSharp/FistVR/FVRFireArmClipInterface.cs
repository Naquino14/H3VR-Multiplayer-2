// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmClipInterface
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFireArmClipInterface : FVRInteractiveObject
  {
    public FVRFireArmClip Clip;
    private float m_clipLoadTick;

    public override void BeginInteraction(FVRViveHand hand)
    {
      if ((!((Object) this.Clip.FireArm != (Object) null) || !((Object) this.Clip.FireArm.Magazine != (Object) null) || !this.Clip.FireArm.Magazine.IsFull()) && this.Clip.m_numRounds > 0)
        return;
      this.EndInteraction(hand);
      this.Clip.FireArm.EjectClip();
      hand.ForceSetInteractable((FVRInteractiveObject) this.Clip);
      this.Clip.BeginInteraction(hand);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.Clip.State == FVRFireArmClip.ClipState.Free)
        this.ForceBreakInteraction();
      else if ((double) this.m_clipLoadTick > 0.0)
      {
        this.m_clipLoadTick -= Time.deltaTime;
      }
      else
      {
        Vector3 velLinearWorld = hand.Input.VelLinearWorld;
        if ((double) velLinearWorld.magnitude > 0.100000001490116 && (double) Vector3.Angle(velLinearWorld, -this.transform.up) < 45.0)
        {
          this.Clip.LoadOneRoundFromClipToMag();
          this.m_clipLoadTick = 0.03f;
        }
      }
      if (hand.IsInStreamlinedMode)
      {
        if (!hand.Input.AXButtonDown || !((Object) this.Clip != (Object) null))
          return;
        this.EndInteraction(hand);
        this.Clip.FireArm.EjectClip();
        hand.ForceSetInteractable((FVRInteractiveObject) this.Clip);
        this.Clip.BeginInteraction(hand);
      }
      else
      {
        if (!hand.Input.TouchpadDown || (double) hand.Input.TouchpadAxes.magnitude <= 0.25 || ((double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) > 45.0 || !((Object) this.Clip != (Object) null)))
          return;
        this.EndInteraction(hand);
        this.Clip.FireArm.EjectClip();
        hand.ForceSetInteractable((FVRInteractiveObject) this.Clip);
        this.Clip.BeginInteraction(hand);
      }
    }
  }
}
