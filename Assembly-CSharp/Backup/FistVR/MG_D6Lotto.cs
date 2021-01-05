// Decompiled with JetBrains decompiler
// Type: FistVR.MG_D6Lotto
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_D6Lotto : FVRPhysicalObject
  {
    public RedRoom m_room;
    private bool m_hasBeenPickedUp;
    private bool m_hasBeenReleased;
    private bool m_hasBeenRolled;
    public Transform[] Facings;
    public Rigidbody[] Shards;
    public GameObject[] Spawns;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.m_hasBeenPickedUp = true;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.m_hasBeenReleased = true;
      this.RootRigidbody.angularVelocity = Random.onUnitSphere * 5f;
    }

    public override bool IsInteractable() => !this.m_hasBeenReleased && base.IsInteractable();

    public override bool IsDistantGrabbable() => false;

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (!this.m_hasBeenReleased || (double) this.RootRigidbody.velocity.magnitude >= 0.100000001490116 || (double) this.RootRigidbody.angularVelocity.magnitude >= 0.100000001490116)
        return;
      this.RootRigidbody.isKinematic = true;
      this.CheckFacingAndRoll();
    }

    private void CheckFacingAndRoll()
    {
      int facing = 0;
      float num1 = 180f;
      for (int index = 1; index < this.Facings.Length; ++index)
      {
        float num2 = Vector3.Angle(this.Facings[index].forward, Vector3.up);
        if ((double) num2 < (double) num1)
        {
          facing = index;
          num1 = num2;
        }
      }
      this.SpawnBasedOnFacing(facing);
    }

    private void SpawnBasedOnFacing(int facing)
    {
      this.m_hasBeenRolled = true;
      for (int index = 0; index < this.Shards.Length; ++index)
      {
        this.Shards[index].transform.SetParent((Transform) null);
        this.Shards[index].gameObject.SetActive(true);
      }
      if (facing == 4)
      {
        GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LTEntry_Handgun3, this.transform.position + Vector3.up * 0.3f, Random.rotation);
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Handgun3, this.transform.position + Vector3.up * 0.5f, Random.rotation);
      }
      else if (facing == 5)
      {
        GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LTEntry_Shotgun3, this.transform.position + Vector3.up * 0.3f, Random.rotation);
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Shotgun3, this.transform.position + Vector3.up * 0.5f, Random.rotation);
      }
      else if (facing == 6)
      {
        GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LTEntry_RareGun3, this.transform.position + Vector3.up * 0.3f, Random.rotation);
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_RareGun3, this.transform.position + Vector3.up * 0.5f, Random.rotation);
      }
      if (facing == 1)
        Object.Instantiate<GameObject>(GM.MGMaster.FlyingHotDogSwarmPrefab, this.transform.position, Quaternion.identity);
      for (int index = 0; index < this.Spawns.Length; ++index)
        Object.Instantiate<GameObject>(this.Spawns[index], this.transform.position, this.transform.rotation);
      this.m_room.OpenDoors(true);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
