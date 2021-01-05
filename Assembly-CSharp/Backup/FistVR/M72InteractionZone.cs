// Decompiled with JetBrains decompiler
// Type: FistVR.M72InteractionZone
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class M72InteractionZone : FVRInteractiveObject
  {
    public M72 M72;
    public M72InteractionZone.M72InteractionType IntType;

    public override bool IsInteractable()
    {
      switch (this.IntType)
      {
        case M72InteractionZone.M72InteractionType.Cap:
          return this.M72.CanCapBeToggled();
        case M72InteractionZone.M72InteractionType.TubeRear:
          return this.M72.CanTubeBeGrabbed();
        default:
          return base.IsInteractable();
      }
    }

    public override void SimpleInteraction(FVRViveHand hand)
    {
      switch (this.IntType)
      {
        case M72InteractionZone.M72InteractionType.Safety:
          this.M72.ToggleSafety();
          break;
        case M72InteractionZone.M72InteractionType.Cap:
          this.M72.ToggleCap();
          break;
      }
      base.SimpleInteraction(hand);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.IntType != M72InteractionZone.M72InteractionType.TubeRear)
        return;
      this.transform.position = this.GetClosestValidPoint(this.M72.Tube_Front.position, this.M72.Tube_Rear.position, this.m_handPos);
      Mathf.InverseLerp(this.M72.Tube_Front.localPosition.z, this.M72.Tube_Rear.localPosition.z, this.transform.localPosition.z);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      if (this.IntType != M72InteractionZone.M72InteractionType.TubeRear)
        return;
      float num1 = Vector3.Distance(this.transform.position, this.M72.Tube_Front.position);
      float num2 = Vector3.Distance(this.transform.position, this.M72.Tube_Rear.position);
      if ((double) num1 < 0.00999999977648258)
      {
        this.M72.TState = M72.TubeState.Forward;
        this.M72.PlayAudioEvent(FirearmAudioEventType.StockClosed);
      }
      else if ((double) num2 < 0.00999999977648258)
      {
        this.M72.TState = M72.TubeState.Rear;
        this.M72.PlayAudioEvent(FirearmAudioEventType.StockOpen);
        this.M72.PopUpRearSight();
      }
      else
        this.M72.TState = M72.TubeState.Mid;
    }

    public enum M72InteractionType
    {
      Safety,
      Cap,
      TubeRear,
    }
  }
}
