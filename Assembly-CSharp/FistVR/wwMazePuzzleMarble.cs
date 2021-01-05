// Decompiled with JetBrains decompiler
// Type: FistVR.wwMazePuzzleMarble
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwMazePuzzleMarble : FVRInteractiveObject
  {
    public bool IsMarbleLocked;
    public Transform Maze;
    public Collider ToRayCastAgainst;
    public LayerMask CastMask;
    public float SphereRadius = 0.1f;

    public override bool IsInteractable() => !this.IsMarbleLocked && base.IsInteractable();

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 vector3 = Vector3.ProjectOnPlane(hand.transform.position - this.transform.position, this.Maze.up);
      if (Physics.SphereCast(this.transform.position, this.SphereRadius, vector3.normalized, out RaycastHit _, vector3.magnitude, (int) this.CastMask, QueryTriggerInteraction.Ignore))
        return;
      this.transform.position = this.transform.position + vector3;
    }
  }
}
