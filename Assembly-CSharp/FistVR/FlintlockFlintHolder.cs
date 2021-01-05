// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockFlintHolder
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlintlockFlintHolder : FVRInteractiveObject
  {
    public FlintlockFlintScrew Screw;
    public GameObject FlintPrefab;
    public AudioEvent AudEvent_Remove;
    public AudioEvent AudEvent_Replace;
    public Transform FlintPos;
    private float TimeTilFlintReplace = 1f;

    public override bool IsInteractable() => this.Screw.SState == FlintlockFlintScrew.ScrewState.Unscrewed;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      if (this.Screw.Weapon.HasFlint())
      {
        Vector3 u = this.Screw.Weapon.RemoveFlint();
        this.ExtractFlint(hand, u);
        this.Screw.Weapon.PlayAudioAsHandling(this.AudEvent_Remove, this.transform.position);
      }
      base.SimpleInteraction(hand);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.TimeTilFlintReplace <= 0.0)
        return;
      this.TimeTilFlintReplace -= Time.deltaTime;
    }

    private void ExtractFlint(FVRViveHand h, Vector3 u)
    {
      this.TimeTilFlintReplace = 1f;
      FlintlockFlint component = Object.Instantiate<GameObject>(this.FlintPrefab, this.FlintPos.position, this.FlintPos.rotation).GetComponent<FlintlockFlint>();
      component.m_flintUses = u;
      component.UpdateState();
      h.ForceSetInteractable((FVRInteractiveObject) component);
      component.BeginInteraction(h);
    }

    public void OnTriggerEnter(Collider other)
    {
      if ((double) this.TimeTilFlintReplace > 0.0 || this.Screw.Weapon.HasFlint() || (Object) other.attachedRigidbody == (Object) null)
        return;
      GameObject gameObject = other.attachedRigidbody.gameObject;
      if (!gameObject.CompareTag("flintlock_flint"))
        return;
      this.Screw.Weapon.AddFlint(gameObject.GetComponent<FlintlockFlint>().m_flintUses);
      this.Screw.Weapon.PlayAudioAsHandling(this.AudEvent_Replace, this.transform.position);
      Object.Destroy((Object) other.gameObject);
    }
  }
}
