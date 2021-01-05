// Decompiled with JetBrains decompiler
// Type: FistVR.SamplerPlatter_Buffet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class SamplerPlatter_Buffet : MonoBehaviour
  {
    public List<FVRPhysicalObject> objs;
    public List<FVRObject> Prefabs;
    public List<Transform> SpawnPoints;
    private List<FVRPhysicalObject> m_spawnedObjects = new List<FVRPhysicalObject>();

    private void Start() => this.InitialSpawn();

    private void InitialSpawn()
    {
      for (int index = 0; index < this.Prefabs.Count; ++index)
        this.m_spawnedObjects.Add(Object.Instantiate<GameObject>(this.Prefabs[index].GetGameObject(), this.SpawnPoints[index].position, this.SpawnPoints[index].rotation).GetComponent<FVRPhysicalObject>());
    }

    public void ResetBuffet()
    {
      for (int index = 0; index < this.m_spawnedObjects.Count; ++index)
      {
        if ((Object) this.m_spawnedObjects[index] == (Object) null)
        {
          this.RespawnIndex(index);
        }
        else
        {
          FVRPhysicalObject spawnedObject = this.m_spawnedObjects[index];
          FVRPhysicalObject fvrPhysicalObject1;
          if (!spawnedObject.IsHeld && !((Object) spawnedObject.QuickbeltSlot != (Object) null))
          {
            switch (spawnedObject)
            {
              case FVRFireArmMagazine _:
                if (!(spawnedObject as FVRFireArmMagazine).HasARound())
                {
                  FVRPhysicalObject fvrPhysicalObject2 = spawnedObject;
                  fvrPhysicalObject1 = this.RespawnIndex(index);
                  Object.Destroy((Object) fvrPhysicalObject2.gameObject);
                  continue;
                }
                continue;
              case FVRFireArmAttachment _:
                if ((Object) (spawnedObject as FVRFireArmAttachment).curMount == (Object) null && (double) Vector3.Distance(this.m_spawnedObjects[index].transform.position, GM.CurrentPlayerBody.transform.position) > 6.0)
                {
                  this.MoveToIndex(index);
                  continue;
                }
                continue;
              case FVRStrikeAnyWhereMatch _:
              case Speedloader _:
              case FVRFireArmClip _:
                FVRPhysicalObject fvrPhysicalObject3 = spawnedObject;
                fvrPhysicalObject1 = this.RespawnIndex(index);
                Object.Destroy((Object) fvrPhysicalObject3.gameObject);
                continue;
              default:
                if ((double) Vector3.Distance(this.m_spawnedObjects[index].transform.position, GM.CurrentPlayerBody.transform.position) > 6.0)
                {
                  this.MoveToIndex(index);
                  continue;
                }
                continue;
            }
          }
        }
      }
    }

    private FVRPhysicalObject RespawnIndex(int i)
    {
      FVRPhysicalObject component = Object.Instantiate<GameObject>(this.Prefabs[i].GetGameObject(), this.SpawnPoints[i].position, this.SpawnPoints[i].rotation).GetComponent<FVRPhysicalObject>();
      this.m_spawnedObjects[i] = component;
      return component;
    }

    private void MoveToIndex(int i)
    {
      this.m_spawnedObjects[i].transform.position = this.SpawnPoints[i].position;
      this.m_spawnedObjects[i].transform.rotation = this.SpawnPoints[i].rotation;
      this.m_spawnedObjects[i].RootRigidbody.velocity = Vector3.zero;
      this.m_spawnedObjects[i].RootRigidbody.angularVelocity = Vector3.zero;
    }

    [ContextMenu("Migrate")]
    public void Migrate()
    {
      for (int index = 0; index < this.objs.Count; ++index)
      {
        this.Prefabs[index] = this.objs[index].ObjectWrapper;
        Transform transform = new GameObject().transform;
        transform.gameObject.name = "_SpawnPoint" + this.objs[index].gameObject.name;
        transform.transform.parent = this.transform;
        transform.position = this.objs[index].transform.position;
        transform.rotation = this.objs[index].transform.rotation;
        this.SpawnPoints[index] = transform;
      }
    }
  }
}
