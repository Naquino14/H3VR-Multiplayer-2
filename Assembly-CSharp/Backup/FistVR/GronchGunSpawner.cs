// Decompiled with JetBrains decompiler
// Type: FistVR.GronchGunSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FistVR
{
  public class GronchGunSpawner : MonoBehaviour
  {
    private GronchJobManager m_M;
    private bool m_isSpawning;
    private float m_timeTilSpawn = 1f;
    private Vector2 TickRange = new Vector2(2f, 5f);
    public List<Transform> Positions;
    private List<GameObject> m_spawned = new List<GameObject>();
    private HashSet<FVRFireArm> m_spawnedFA = new HashSet<FVRFireArm>();
    public ObjectTableDef ObjectTableDef;
    private ObjectTable table;
    public Collider GronchTarget;
    public List<Transform> GronchTargetPlaces;
    private RaycastHit r;
    private bool m_isLoading;

    public void Awake()
    {
      this.table = new ObjectTable();
      this.table.Initialize(this.ObjectTableDef);
    }

    private void Start() => GM.CurrentSceneSettings.ShotFiredEvent += new FVRSceneSettings.ShotFired(this.ShotFired);

    private void OnDestroy() => GM.CurrentSceneSettings.ShotFiredEvent -= new FVRSceneSettings.ShotFired(this.ShotFired);

    private void ShotFired(FVRFireArm f)
    {
      if (!this.m_isSpawning || !this.m_spawnedFA.Contains(f))
        return;
      this.m_spawnedFA.Remove(f);
      this.m_M.DidJobStuff();
      if (!this.GronchTarget.Raycast(new Ray(f.MuzzlePos.position, f.MuzzlePos.forward), out this.r, 100f))
        return;
      this.m_M.Promotion();
      this.GronchTargetPlaces.Shuffle<Transform>();
      this.GronchTarget.transform.position = this.GronchTargetPlaces[0].position;
    }

    public void BeginJob(GronchJobManager m)
    {
      this.m_M = m;
      this.m_isSpawning = true;
      this.m_spawned.Clear();
      this.GronchTargetPlaces.Shuffle<Transform>();
      this.GronchTargetPlaces.Shuffle<Transform>();
      this.GronchTarget.transform.position = this.GronchTargetPlaces[0].position;
      this.GronchTarget.gameObject.SetActive(true);
    }

    public void EndJob(GronchJobManager m)
    {
      this.m_M = (GronchJobManager) null;
      this.m_isSpawning = false;
      this.GronchTarget.gameObject.SetActive(false);
      this.ClearSpawned();
    }

    public void PlayerDied(GronchJobManager m) => this.m_M = m;

    private void ClearSpawned()
    {
      for (int index = this.m_spawned.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_spawned[index] != (Object) null)
          Object.Destroy((Object) this.m_spawned[index]);
      }
      this.m_spawned.Clear();
      this.m_spawnedFA.Clear();
    }

    private void Update()
    {
      if (!this.m_isSpawning)
        return;
      if (!this.m_isLoading)
        this.m_timeTilSpawn -= Time.deltaTime;
      if ((double) this.m_timeTilSpawn > 0.0)
        return;
      this.m_timeTilSpawn = Random.Range(this.TickRange.x, this.TickRange.y);
      AnvilManager.Run(this.SpawnObject(this.table));
    }

    [DebuggerHidden]
    private IEnumerator SpawnObject(ObjectTable table) => (IEnumerator) new GronchGunSpawner.\u003CSpawnObject\u003Ec__Iterator0()
    {
      table = table,
      \u0024this = this
    };
  }
}
