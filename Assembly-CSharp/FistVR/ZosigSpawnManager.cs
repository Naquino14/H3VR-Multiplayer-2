// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigSpawnManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigSpawnManager : MonoBehaviour
  {
    private List<ZosigTestSpawnEnemy> m_enemyTestSpawns = new List<ZosigTestSpawnEnemy>();
    private int m_spawnNPCTestIndex;
    private List<ZosigSpawnFromTable> m_spawnsFromTable = new List<ZosigSpawnFromTable>();
    private int m_spawnFromTableCheckIndex;
    private List<ZosigNpcSpawnPoint> m_npcSpawns = new List<ZosigNpcSpawnPoint>();
    private int m_spawnNPCCheckIndex;
    private List<ZosigEnemySpawnZone> m_zosigSpawnZones = new List<ZosigEnemySpawnZone>();
    private int m_zosigSpawnZoneIndex;
    private List<ZosigTurretSpawn> m_zosigTurrets = new List<ZosigTurretSpawn>();
    private int m_zosigTurretSpawnIndex;
    private bool m_hasInit;
    [Header("NPC STUFF")]
    public GameObject TempNPCPrefabEventuallyUseProfile;
    public GameObject NPCSpeechInferfacePrefab;
    public List<ZosigNPCProfile> Profiles;
    public List<SosigSpeechSet> NPCSpeechSets;

    public void Init()
    {
      this.m_hasInit = true;
      ZosigSpawnFromTable[] objectsOfType1 = Object.FindObjectsOfType<ZosigSpawnFromTable>();
      for (int index = 0; index < objectsOfType1.Length; ++index)
      {
        this.m_spawnsFromTable.Add(objectsOfType1[index]);
        objectsOfType1[index].Init();
      }
      foreach (ZosigNpcSpawnPoint zosigNpcSpawnPoint in Object.FindObjectsOfType<ZosigNpcSpawnPoint>())
        this.m_npcSpawns.Add(zosigNpcSpawnPoint);
      foreach (ZosigTestSpawnEnemy zosigTestSpawnEnemy in Object.FindObjectsOfType<ZosigTestSpawnEnemy>())
        this.m_enemyTestSpawns.Add(zosigTestSpawnEnemy);
      ZosigEnemySpawnZone[] objectsOfType2 = Object.FindObjectsOfType<ZosigEnemySpawnZone>();
      for (int index = 0; index < objectsOfType2.Length; ++index)
      {
        this.m_zosigSpawnZones.Add(objectsOfType2[index]);
        objectsOfType2[index].M = this;
        objectsOfType2[index].Init(GM.ZMaster);
      }
      foreach (ZosigTurretSpawn zosigTurretSpawn in Object.FindObjectsOfType<ZosigTurretSpawn>())
        this.m_zosigTurrets.Add(zosigTurretSpawn);
    }

    public void Update()
    {
      if (!this.m_hasInit)
        return;
      if (this.m_spawnsFromTable.Count > 0)
        this.SpawnFromTableLoop();
      if (this.m_npcSpawns.Count > 0)
        this.NPCSpawnLoop();
      if (this.m_enemyTestSpawns.Count > 0)
        this.TestEnemySpawnLoop();
      if (this.m_zosigSpawnZones.Count > 0)
        this.ZosigSpawnLoop();
      if (this.m_zosigTurrets.Count <= 0)
        return;
      this.TurretSpawnLoop();
    }

    public void TurretSpawnLoop()
    {
      ++this.m_zosigTurretSpawnIndex;
      if (this.m_zosigTurretSpawnIndex >= this.m_zosigTurrets.Count)
        this.m_zosigTurretSpawnIndex = 0;
      this.m_zosigTurrets[this.m_zosigTurretSpawnIndex].SpawnKernel(Time.deltaTime);
    }

    public void ZosigSpawnLoop()
    {
      float deltaTime = Time.deltaTime;
      for (int index = 0; index < this.m_zosigSpawnZones.Count; ++index)
        this.m_zosigSpawnZones[index].Tick(deltaTime);
      ++this.m_zosigSpawnZoneIndex;
      if (this.m_zosigSpawnZoneIndex >= this.m_zosigSpawnZones.Count)
        this.m_zosigSpawnZoneIndex = 0;
      this.m_zosigSpawnZones[this.m_zosigSpawnZoneIndex].Check();
    }

    public void SpawnFromTableLoop()
    {
      ++this.m_spawnFromTableCheckIndex;
      if (this.m_spawnFromTableCheckIndex >= this.m_spawnsFromTable.Count)
        this.m_spawnFromTableCheckIndex = 0;
      this.m_spawnsFromTable[this.m_spawnFromTableCheckIndex].SpawnKernel();
    }

    public void TestEnemySpawnLoop()
    {
      ++this.m_spawnNPCTestIndex;
      if (this.m_spawnNPCTestIndex >= this.m_enemyTestSpawns.Count)
        this.m_spawnNPCTestIndex = 0;
      ZosigTestSpawnEnemy enemyTestSpawn = this.m_enemyTestSpawns[this.m_spawnNPCTestIndex];
      if (enemyTestSpawn.HasSpawned)
        return;
      enemyTestSpawn.HasSpawned = true;
      SosigConfigTemplate configTemplate = enemyTestSpawn.Template.ConfigTemplates[0];
      SosigOutfitConfig o = enemyTestSpawn.Template.OutfitConfig[0];
      this.SpawnEnemySosig(enemyTestSpawn.Template.SosigPrefabs[0].GetGameObject(), enemyTestSpawn.Template.WeaponOptions[Random.Range(0, enemyTestSpawn.Template.WeaponOptions.Count)].GetGameObject(), enemyTestSpawn.transform.position, enemyTestSpawn.transform.rotation, configTemplate, o, 1);
    }

    private Sosig SpawnEnemySosig(
      GameObject prefab,
      GameObject weapon,
      Vector3 pos,
      Quaternion rot,
      SosigConfigTemplate t,
      SosigOutfitConfig o,
      int IFF)
    {
      Sosig componentInChildren = Object.Instantiate<GameObject>(prefab, pos, rot).GetComponentInChildren<Sosig>();
      componentInChildren.Configure(t);
      componentInChildren.Inventory.FillAllAmmo();
      componentInChildren.E.IFFCode = IFF;
      SosigWeapon w = (SosigWeapon) null;
      if ((Object) weapon != (Object) null)
      {
        w = Object.Instantiate<GameObject>(weapon, pos + Vector3.up * 2f + Vector3.right * 0.6f, rot).GetComponent<SosigWeapon>();
        w.SetAutoDestroy(true);
        w.O.SpawnLockable = false;
      }
      componentInChildren.CurrentOrder = Sosig.SosigOrder.GuardPoint;
      componentInChildren.FallbackOrder = Sosig.SosigOrder.GuardPoint;
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Headwear)
        this.SpawnAccesoryToLink(o.Headwear, componentInChildren.Links[0]);
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Facewear)
        this.SpawnAccesoryToLink(o.Facewear, componentInChildren.Links[0]);
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Eyewear)
        this.SpawnAccesoryToLink(o.Eyewear, componentInChildren.Links[0]);
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Torsowear)
        this.SpawnAccesoryToLink(o.Torsowear, componentInChildren.Links[1]);
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Pantswear)
        this.SpawnAccesoryToLink(o.Pantswear, componentInChildren.Links[2]);
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Pantswear_Lower)
        this.SpawnAccesoryToLink(o.Pantswear_Lower, componentInChildren.Links[3]);
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Backpacks)
        this.SpawnAccesoryToLink(o.Backpacks, componentInChildren.Links[1]);
      if (t.UsesLinkSpawns)
      {
        for (int index = 0; index < componentInChildren.Links.Count; ++index)
        {
          if ((double) Random.Range(0.0f, 1f) < (double) t.LinkSpawnChance[index])
            componentInChildren.Links[index].RegisterSpawnOnDestroy(t.LinkSpawns[index]);
        }
      }
      if ((Object) w != (Object) null)
      {
        componentInChildren.InitHands();
        componentInChildren.ForceEquip(w);
      }
      return componentInChildren;
    }

    private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(gs[Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }

    public void NPCSpawnLoop()
    {
      ++this.m_spawnNPCCheckIndex;
      if (this.m_spawnNPCCheckIndex >= this.m_npcSpawns.Count)
        this.m_spawnNPCCheckIndex = 0;
      ZosigNpcSpawnPoint npcSpawn = this.m_npcSpawns[this.m_spawnNPCCheckIndex];
      if (npcSpawn.HasSpawned || npcSpawn.NeedsFlag && GM.ZMaster.FlagM.GetFlagValue(npcSpawn.FlagToSpawn) < npcSpawn.FlagValueOrHigherToSpawn)
        return;
      Sosig sosig = this.SpawnEnemySosig(npcSpawn.Template.SosigPrefabs[0].GetGameObject(), npcSpawn.Template.WeaponOptions[0].GetGameObject(), npcSpawn.transform.position, npcSpawn.transform.rotation, npcSpawn.Template.ConfigTemplates[0], npcSpawn.Template.OutfitConfig[0], 0);
      ZosigNPCInterface component = Object.Instantiate<GameObject>(this.NPCSpeechInferfacePrefab, sosig.Links[0].transform.position, sosig.Links[0].transform.rotation).GetComponent<ZosigNPCInterface>();
      sosig.Configure(npcSpawn.Template.ConfigTemplates[0]);
      component.S = sosig;
      component.Profile = this.Profiles[npcSpawn.NPCIndex];
      sosig.Speech = this.NPCSpeechSets[npcSpawn.NPCIndex];
      component.S.E.IFFCode = 0;
      npcSpawn.HasSpawned = true;
    }

    public Sosig SpawnNPCToPoint(SosigEnemyTemplate Template, int index, Transform point)
    {
      Sosig sosig = this.SpawnEnemySosig(Template.SosigPrefabs[0].GetGameObject(), Template.WeaponOptions[0].GetGameObject(), point.transform.position, point.transform.rotation, Template.ConfigTemplates[0], Template.OutfitConfig[0], 0);
      sosig.Configure(Template.ConfigTemplates[0]);
      sosig.Speech = this.NPCSpeechSets[index];
      sosig.E.IFFCode = 0;
      return sosig;
    }
  }
}
