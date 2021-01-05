// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigContainer_Destoyable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigContainer_Destoyable : ZosigContainer, IFVRDamageable
  {
    public List<Rigidbody> Shards;
    private bool m_hasShattered;
    public List<Transform> SpawnPoints;
    public float ChanceEmpty = 0.4f;
    public float ChanceDynamicAmmoSpawn = 0.05f;
    public AudioEvent AudEvent_Shatter;
    private float m_sinceSpawn;

    public void Update()
    {
      if ((double) this.m_sinceSpawn >= 5.0)
        return;
      this.m_sinceSpawn += Time.deltaTime;
    }

    public override void PlaceObjectsInContainer(FVRObject obj1, int minAmmo = -1, int maxAmmo = 30)
    {
      this.m_storedObject1 = obj1;
      base.PlaceObjectsInContainer(obj1);
    }

    public void Damage(FistVR.Damage d)
    {
      if ((double) d.Dam_TotalKinetic <= 20.0)
        return;
      this.SpawnItemsAndShatter();
    }

    private void OnCollisionEnter(Collision col)
    {
      if ((double) col.relativeVelocity.magnitude <= 5.0 || (double) this.m_sinceSpawn <= 3.0)
        return;
      this.SpawnItemsAndShatter();
    }

    private void SpawnItemsAndShatter()
    {
      if (!this.m_containsItems || this.m_hasShattered)
        return;
      this.m_hasShattered = true;
      if ((Object) GM.ZMaster != (Object) null)
        GM.ZMaster.FlagM.AddToFlag("s_l", 1);
      this.FlagOpen();
      for (int index = 0; index < this.Shards.Count; ++index)
        this.Shards[index].transform.SetParent((Transform) null);
      SM.PlayGenericSound(this.AudEvent_Shatter, this.transform.position);
      if ((double) Random.Range(0.0f, 1f) > (double) this.ChanceEmpty)
      {
        float num = Random.Range(0.0f, 1f);
        bool flag = false;
        if ((double) num <= (double) this.ChanceDynamicAmmoSpawn)
        {
          FVRObject randomEquippedFirearm = GM.ZMaster.GetRandomEquippedFirearm();
          if ((Object) randomEquippedFirearm != (Object) null)
          {
            if (randomEquippedFirearm.CompatibleMagazines.Count > 0)
            {
              Object.Instantiate<GameObject>(randomEquippedFirearm.CompatibleMagazines[Random.Range(0, randomEquippedFirearm.CompatibleMagazines.Count)].GetGameObject(), this.SpawnPoints[0].position, this.SpawnPoints[0].rotation);
              flag = true;
            }
            else if (randomEquippedFirearm.CompatibleClips.Count > 0)
            {
              Object.Instantiate<GameObject>(randomEquippedFirearm.CompatibleClips[Random.Range(0, randomEquippedFirearm.CompatibleMagazines.Count)].GetGameObject(), this.SpawnPoints[0].position, this.SpawnPoints[0].rotation);
              flag = true;
            }
            else if (randomEquippedFirearm.CompatibleClips.Count > 0)
            {
              Object.Instantiate<GameObject>(randomEquippedFirearm.CompatibleClips[Random.Range(0, randomEquippedFirearm.CompatibleMagazines.Count)].GetGameObject(), this.SpawnPoints[0].position, this.SpawnPoints[0].rotation);
              flag = true;
            }
          }
        }
        if (!flag && (Object) this.m_storedObject1 != (Object) null)
        {
          Object.Instantiate<GameObject>(this.m_storedObject1.GetGameObject(), this.SpawnPoints[0].position, this.SpawnPoints[0].rotation);
          if (this.m_storedObject1.RequiredSecondaryPieces.Count > 0)
            Object.Instantiate<GameObject>(this.m_storedObject1.RequiredSecondaryPieces[0].GetGameObject(), this.SpawnPoints[1].position, this.SpawnPoints[1].rotation);
        }
      }
      for (int index = 0; index < this.Shards.Count; ++index)
        this.Shards[index].gameObject.SetActive(true);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
