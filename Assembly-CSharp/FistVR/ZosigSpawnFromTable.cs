// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigSpawnFromTable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FistVR
{
  public class ZosigSpawnFromTable : MonoBehaviour
  {
    [Header("Flags")]
    public bool NeedsFlag;
    public string FlagNeeded;
    public int FlagNeededValue = 1;
    public bool FlagNeedsEquality = true;
    public bool HasBlockingFlag;
    public string BlockingFlag;
    [Header("Objects")]
    public FVRObject Item;
    public bool UsesTable;
    public List<ZosigItemSpawnTable> Tables;
    public float Incidence = 1f;
    public int MinItems = 1;
    public int MaxItems = 2;
    public bool StripOnSpawn;
    [Header("TrackAnRespawn")]
    public bool DoesTrackRespawn;
    public float RespawnRange = 100f;
    public float RespawnCooldown = 200f;
    private float m_respawnCoolDown = 200f;
    private float m_respawnCheckTick = 1f;
    private GameObject m_respawnTrack;
    [Header("Container")]
    public bool UsesContainer;
    public FVRObject ContainerPrefab;
    public bool UseContainerFlag;
    public string ContainerFlag;
    public int ContainterFlagValue;
    [Header("SpawnPositions")]
    public bool UsesSpawnPoints;
    public List<Transform> SpawnPoints;
    [Header("Details")]
    public int Num_Mags = 2;
    public int Num_Rounds = 4;
    public bool IsReloadamaticAmmoDefault = true;
    public int MinAmmo = -1;
    public int MaxAmmo = 30;
    [Header("BuyBuddy Stuff")]
    public ObjectTableDef BuyBuddyTable;
    public bool BuyBuddyIsLargeCase;
    public string BuyBuddyUnlockFlag = "BuyBuddy_Test";
    public List<int> BuyBuddyPrice;
    private bool m_hasSpawned;
    public ZosigFlagManager F;

    public void Init() => this.F = GM.ZMaster.FlagM;

    public void Update()
    {
      if (!this.DoesTrackRespawn || !this.m_hasSpawned)
        return;
      if ((double) this.m_respawnCoolDown > 0.0)
        this.m_respawnCoolDown -= Time.deltaTime;
      else if ((double) this.m_respawnCheckTick > 0.0)
      {
        this.m_respawnCheckTick -= Time.deltaTime;
      }
      else
      {
        this.m_respawnCheckTick = 5f;
        if ((!((Object) this.m_respawnTrack == (Object) null) ? (double) Vector3.Distance(this.m_respawnTrack.transform.position, this.transform.position) : (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position)) <= (double) this.RespawnRange)
          return;
        this.m_hasSpawned = false;
      }
    }

    public void SpawnKernel()
    {
      if (this.m_hasSpawned || this.HasBlockingFlag && this.F.GetFlagValue(this.BlockingFlag) > 0 || this.NeedsFlag && this.F.GetFlagValue(this.FlagNeeded) != this.FlagNeededValue && this.FlagNeedsEquality || this.NeedsFlag && this.F.GetFlagValue(this.FlagNeeded) < this.FlagNeededValue && !this.FlagNeedsEquality)
        return;
      if (this.UsesSpawnPoints)
      {
        if ((double) Random.Range(0.0f, 0.99f) <= (double) this.Incidence)
        {
          int num = Random.Range(this.MinItems, this.MaxItems + 1);
          this.SpawnPoints.Shuffle<Transform>();
          this.SpawnPoints.Shuffle<Transform>();
          for (int index = 0; index < num; ++index)
            AnvilManager.Run(this.SpawnObject(this.SpawnPoints[index].position, this.SpawnPoints[index].rotation));
        }
      }
      else
        AnvilManager.Run(this.SpawnObject(this.transform.position, this.transform.rotation));
      if (GM.ZMaster.IsVerboseDebug)
        UnityEngine.Debug.Log((object) ("Spawning:" + this.gameObject.name));
      this.m_hasSpawned = true;
    }

    private void StripObject(GameObject g)
    {
      Component[] componentsInChildren = g.GetComponentsInChildren<Component>();
      for (int index = componentsInChildren.Length - 1; index >= 0; --index)
      {
        if (!(componentsInChildren[index] is MeshRenderer) && !(componentsInChildren[index] is MeshFilter) && (!(componentsInChildren[index] is Transform) && !(componentsInChildren[index] is Collider)) && !(componentsInChildren[index] is AudioSource))
          Object.Destroy((Object) componentsInChildren[index]);
      }
    }

    [DebuggerHidden]
    private IEnumerator SpawnObject(Vector3 pos, Quaternion rot) => (IEnumerator) new ZosigSpawnFromTable.\u003CSpawnObject\u003Ec__Iterator0()
    {
      pos = pos,
      rot = rot,
      \u0024this = this
    };
  }
}
