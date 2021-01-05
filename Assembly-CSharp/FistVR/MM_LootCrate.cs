// Decompiled with JetBrains decompiler
// Type: FistVR.MM_LootCrate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MM_LootCrate : MonoBehaviour, IFVRDamageable
  {
    public FVRObject m_storedObject1;
    public FVRObject m_storedObject2;
    public FVRObject Object3;
    public FVRObject Object4;
    public Transform[] SpawnPoints;
    public Rigidbody[] Shards;
    private bool m_isDestroyed;
    public GameObject ShatterFX_Prefab;
    public Transform ShatterFX_Point;
    private bool m_hasObjs;
    public List<PowerupType> PowerupTypes_Health;
    public List<PowerupType> PowerupTypes;

    public void Damage(FistVR.Damage d)
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      this.Destroy();
    }

    public void Init(FVRObject obj1, FVRObject obj2, FVRObject obj3, FVRObject obj4)
    {
      this.m_storedObject1 = obj1;
      this.m_storedObject2 = obj2;
      this.Object3 = obj3;
      this.Object4 = obj4;
      this.m_hasObjs = true;
    }

    private void Destroy()
    {
      Object.Instantiate<GameObject>(this.ShatterFX_Prefab, this.ShatterFX_Point.position, this.ShatterFX_Point.rotation);
      for (int index = 0; index < this.Shards.Length; ++index)
      {
        this.Shards[index].transform.SetParent((Transform) null);
        this.Shards[index].gameObject.SetActive(true);
      }
      if (this.m_hasObjs)
      {
        if ((Object) this.m_storedObject1 != (Object) null)
        {
          Object.Instantiate<GameObject>(this.m_storedObject1.GetGameObject(), this.SpawnPoints[0].position, this.SpawnPoints[0].rotation);
          if (this.m_storedObject1.CompatibleMagazines.Count > 0)
          {
            FVRFireArmMagazine component = Object.Instantiate<GameObject>(this.m_storedObject1.GetMagazineWithinCapacity(200).GetGameObject(), this.SpawnPoints[1].position, this.SpawnPoints[1].rotation).GetComponent<FVRFireArmMagazine>();
            if ((Object) component != (Object) null && component.RoundType != FireArmRoundType.aFlameThrowerFuel && component.RoundType != FireArmRoundType.aFlameThrowerFuel)
              component.ReloadMagWithType(AM.GetRandomValidRoundClass(component.RoundType));
          }
          else if (this.m_storedObject1.CompatibleClips.Count > 0)
          {
            FVRFireArmClip component = Object.Instantiate<GameObject>(this.m_storedObject1.CompatibleClips[Random.Range(0, this.m_storedObject1.CompatibleClips.Count)].GetGameObject(), this.SpawnPoints[1].position, this.SpawnPoints[1].rotation).GetComponent<FVRFireArmClip>();
            if ((Object) component != (Object) null)
              component.ReloadClipWithType(AM.GetRandomValidRoundClass(component.RoundType));
          }
          else if (this.m_storedObject1.CompatibleSpeedLoaders.Count > 0)
          {
            Speedloader component = Object.Instantiate<GameObject>(this.m_storedObject1.CompatibleSpeedLoaders[Random.Range(0, this.m_storedObject1.CompatibleSpeedLoaders.Count)].GetGameObject(), this.SpawnPoints[1].position, this.SpawnPoints[1].rotation).GetComponent<Speedloader>();
            if ((Object) component != (Object) null)
              component.ReloadClipWithType(AM.GetRandomValidRoundClass(component.Chambers[0].Type));
          }
          else if (this.m_storedObject1.CompatibleSingleRounds.Count > 0)
            Object.Instantiate<GameObject>(this.m_storedObject1.CompatibleSingleRounds[Random.Range(0, this.m_storedObject1.CompatibleSingleRounds.Count)].GetGameObject(), this.SpawnPoints[1].position, this.SpawnPoints[1].rotation);
        }
        if ((Object) this.m_storedObject2 != (Object) null)
        {
          Object.Instantiate<GameObject>(this.m_storedObject2.GetGameObject(), this.SpawnPoints[2].position, this.SpawnPoints[2].rotation);
          if (this.m_storedObject2.RequiredSecondaryPieces.Count > 0)
            Object.Instantiate<GameObject>(this.m_storedObject2.RequiredSecondaryPieces[0].GetGameObject(), this.SpawnPoints[3].position, this.SpawnPoints[3].rotation);
        }
        if ((Object) this.Object3 != (Object) null)
        {
          RW_Powerup component = Object.Instantiate<GameObject>(this.Object3.GetGameObject(), this.SpawnPoints[4].position, this.SpawnPoints[4].rotation).GetComponent<RW_Powerup>();
          if ((Object) component != (Object) null)
            component.PowerupType = this.PowerupTypes_Health[Random.Range(0, this.PowerupTypes_Health.Count)];
        }
        if ((Object) this.Object4 != (Object) null)
        {
          RW_Powerup component = Object.Instantiate<GameObject>(this.Object4.GetGameObject(), this.SpawnPoints[5].position, this.SpawnPoints[5].rotation).GetComponent<RW_Powerup>();
          if ((Object) component != (Object) null)
            component.PowerupType = this.PowerupTypes[Random.Range(0, this.PowerupTypes.Count)];
        }
      }
      Object.Destroy((Object) this.gameObject);
    }
  }
}
