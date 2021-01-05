// Decompiled with JetBrains decompiler
// Type: FistVR.MG_DestroyableWithSpawn
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_DestroyableWithSpawn : FVRDestroyableObject
  {
    private MeatGrinderMaster m_master;
    public Transform SpawnPos;
    public float baseThreshold = 0.8f;

    public void SetMGMaster(MeatGrinderMaster master) => this.m_master = master;

    public override void DestroyEvent()
    {
      if (!this.m_isDestroyed)
        this.SpawnRandomLoot();
      base.DestroyEvent();
    }

    private void SpawnRandomLoot()
    {
      if ((double) Random.Range(0.0f, 1f) < (double) this.baseThreshold)
        return;
      float num = Random.Range(0.0f, 1f);
      if ((double) num > 0.949999988079071)
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Handgun1, this.SpawnPos);
      else if ((double) num > 0.899999976158142)
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Handgun2, this.SpawnPos);
      else if ((double) num > 0.850000023841858)
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Handgun3, this.SpawnPos);
      else if ((double) num > 0.800000011920929)
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Shotgun1, this.SpawnPos);
      else if ((double) num > 0.75)
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Shotgun2, this.SpawnPos);
      else if ((double) num > 0.699999988079071)
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_Shotgun3, this.SpawnPos);
      else if ((double) num > 0.649999976158142)
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_RareGun1, this.SpawnPos);
      else if ((double) num > 0.600000023841858)
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_RareGun2, this.SpawnPos);
      else if ((double) num > 0.550000011920929)
        GM.MGMaster.SpawnAmmoAtPlaceForGun(GM.MGMaster.LTEntry_RareGun3, this.SpawnPos);
      else if ((double) num > 0.300000011920929)
        GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LT_Melee.GetRandomObject(), this.SpawnPos);
      else if ((double) num > 0.100000001490116)
        GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LT_Attachments.GetRandomObject(), this.SpawnPos);
      else
        GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LT_Junk.GetRandomObject(), this.SpawnPos);
    }
  }
}
