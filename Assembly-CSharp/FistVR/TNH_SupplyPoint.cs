// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_SupplyPoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TNH_SupplyPoint : MonoBehaviour
  {
    public TNH_Manager M;
    public TNH_TakeChallenge T;
    public Transform Bounds;
    public List<AICoverPoint> CoverPoints;
    public Transform SpawnPoint_PlayerSpawn;
    public List<Transform> SpawnPoints_Sosigs_Defense;
    public List<Transform> SpawnPoints_Turrets;
    public List<Transform> SpawnPoints_Panels;
    public List<Transform> SpawnPoints_Boxes;
    public List<Transform> SpawnPoint_Tables;
    public Transform SpawnPoint_CaseLarge;
    public Transform SpawnPoint_CaseSmall;
    public Transform SpawnPoint_Melee;
    public List<Transform> SpawnPoints_SmallItem;
    public Transform SpawnPoint_Shield;
    [Header("Debug")]
    public bool ShowPoint_PlayerSpawn;
    public bool ShowPoints_Sosigs_Defense;
    public bool ShowPoints_Turrets;
    public bool ShowPoints_Screens;
    public bool ShowPoints_Boxes;
    public bool ShowPoints_SpawnTable;
    public Mesh GizmoMesh_SosigAttack;
    public Mesh GizmoMesh_Panel;
    public Mesh GizmoMesh_Box;
    public Mesh GizmoMesh_Table;
    public Mesh GizmoMesh_CaseLarge;
    public Mesh GizmoMesh_CaseSmall;
    public Mesh GizmoMesh_Melee;
    private List<GameObject> m_trackedObjects = new List<GameObject>();
    private List<Sosig> m_activeSosigs = new List<Sosig>();
    private List<AutoMeater> m_activeTurrets = new List<AutoMeater>();
    private List<GameObject> m_spawnBoxes = new List<GameObject>();
    private GameObject m_constructor;
    private GameObject m_panel;
    private bool m_hasBeenVisited;
    private TAH_ReticleContact m_contact;

    public void SetContact(TAH_ReticleContact c) => this.m_contact = c;

    private void Start() => GM.CurrentSceneSettings.SosigKillEvent += new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);

    private void OnDestroy() => GM.CurrentSceneSettings.SosigKillEvent -= new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);

    public void CheckIfDeadSosigWasMine(Sosig s)
    {
      if (!this.m_activeSosigs.Contains(s))
        return;
      s.TickDownToClear(3f);
      this.m_activeSosigs.Remove(s);
    }

    public void Configure(
      TNH_TakeChallenge t,
      bool spawnSosigs,
      bool spawnDefenses,
      bool spawnConstructor,
      TNH_SupplyPoint.SupplyPanelType panelType,
      int minBoxPiles,
      int maxBoxPiles)
    {
      this.T = t;
      if (spawnSosigs)
        this.SpawnTakeEnemyGroup();
      if (spawnDefenses)
        this.SpawnDefenses();
      if (spawnConstructor)
      {
        this.SpawnConstructor();
        this.SpawnSecondaryPanel(panelType);
      }
      if (maxBoxPiles > 0)
        this.SpawnBoxes(minBoxPiles, maxBoxPiles);
      this.m_hasBeenVisited = false;
    }

    public void TestVisited()
    {
      if (this.m_hasBeenVisited || !this.TestVolumeBool(this.Bounds, GM.CurrentPlayerBody.transform.position))
        return;
      this.m_hasBeenVisited = true;
      if (!((Object) this.m_contact != (Object) null))
        return;
      this.m_contact.SetVisited(true);
    }

    private void SpawnConstructor()
    {
      this.SpawnPoints_Panels.Shuffle<Transform>();
      this.m_constructor = this.M.SpawnObjectConstructor(this.SpawnPoints_Panels[0]);
    }

    private void SpawnSecondaryPanel(TNH_SupplyPoint.SupplyPanelType t)
    {
      if (this.M.EquipmentMode == TNHSetting_EquipmentMode.Spawnlocking && t == TNH_SupplyPoint.SupplyPanelType.MagDuplicator)
        t = TNH_SupplyPoint.SupplyPanelType.GunRecycler;
      switch (t)
      {
        case TNH_SupplyPoint.SupplyPanelType.AmmoReloader:
          this.m_panel = this.M.SpawnAmmoReloader(this.SpawnPoints_Panels[1]);
          break;
        case TNH_SupplyPoint.SupplyPanelType.MagDuplicator:
          this.m_panel = this.M.SpawnMagDuplicator(this.SpawnPoints_Panels[1]);
          break;
        case TNH_SupplyPoint.SupplyPanelType.GunRecycler:
          this.m_panel = this.M.SpawnGunRecycler(this.SpawnPoints_Panels[1]);
          break;
      }
    }

    private void SpawnBoxes(int min, int max)
    {
      float num1 = Random.Range(0.0f, 1f);
      bool flag1 = false;
      if ((double) num1 > 0.200000002980232)
        flag1 = true;
      Random.Range(0.0f, 1f);
      bool flag2 = false;
      if ((double) num1 > 0.100000001490116)
        flag2 = true;
      Random.Range(0.0f, 1f);
      bool flag3 = false;
      if ((double) num1 > 0.600000023841858)
        flag3 = true;
      Random.Range(0.0f, 1f);
      bool flag4 = false;
      if ((double) num1 > 0.800000011920929)
        flag4 = true;
      this.SpawnPoints_Boxes.Shuffle<Transform>();
      int num2 = Random.Range(min, max + 1);
      if (num2 < 1)
        return;
      for (int index1 = 0; index1 < num2; ++index1)
      {
        Transform spawnPointsBox = this.SpawnPoints_Boxes[index1];
        int num3 = Random.Range(1, 3);
        for (int index2 = 0; index2 < num3; ++index2)
        {
          Vector3 position = spawnPointsBox.position + Vector3.up * 0.1f + Vector3.up * 0.85f * (float) index2;
          Quaternion rotation = Quaternion.Slerp(spawnPointsBox.rotation, Random.rotation, 0.1f);
          this.m_spawnBoxes.Add(Object.Instantiate<GameObject>(this.M.Prefabs_ShatterableCrates[Random.Range(0, this.M.Prefabs_ShatterableCrates.Count)], position, rotation));
        }
      }
      this.m_spawnBoxes.Shuffle<GameObject>();
      int index = 0;
      if (flag1 && this.m_spawnBoxes.Count > index)
      {
        this.m_spawnBoxes[index].GetComponent<TNH_ShatterableCrate>().SetHoldingToken(this.M);
        ++index;
      }
      if (flag2 && this.m_spawnBoxes.Count > index)
      {
        this.m_spawnBoxes[index].GetComponent<TNH_ShatterableCrate>().SetHoldingHealth(this.M);
        ++index;
      }
      if (flag3 && this.m_spawnBoxes.Count > index)
      {
        this.m_spawnBoxes[index].GetComponent<TNH_ShatterableCrate>().SetHoldingHealth(this.M);
        ++index;
      }
      if (!flag4 || this.m_spawnBoxes.Count <= index)
        return;
      this.m_spawnBoxes[index].GetComponent<TNH_ShatterableCrate>().SetHoldingHealth(this.M);
      int num4 = index + 1;
    }

    private void SpawnTakeEnemyGroup()
    {
      this.SpawnPoints_Sosigs_Defense.Shuffle<Transform>();
      this.SpawnPoints_Sosigs_Defense.Shuffle<Transform>();
      int num = Mathf.Clamp(Random.Range(this.T.NumGuards - 1, this.T.NumGuards + 1), 0, 5);
      for (int index = 0; index < num; ++index)
      {
        Transform point = this.SpawnPoints_Sosigs_Defense[index];
        this.m_activeSosigs.Add(this.M.SpawnEnemy(ManagerSingleton<IM>.Instance.odicSosigObjsByID[this.T.GID], point, this.T.IFFUsed, false, point.position, false));
      }
    }

    private void SpawnDefenses()
    {
      FVRObject turretPrefab = this.M.GetTurretPrefab(this.T.TurretType);
      int num = Mathf.Clamp(Random.Range(this.T.NumTurrets - 1, this.T.NumTurrets + 1), 0, 5);
      for (int index = 0; index < num; ++index)
      {
        Vector3 position = this.SpawnPoints_Turrets[index].position + Vector3.up * 0.25f;
        this.m_activeTurrets.Add(Object.Instantiate<GameObject>(turretPrefab.GetGameObject(), position, this.SpawnPoints_Turrets[index].rotation).GetComponent<AutoMeater>());
      }
    }

    public void ConfigureAtBeginning(TNH_CharacterDef c)
    {
      this.m_trackedObjects.Clear();
      if (this.M.ItemSpawnerMode == TNH_ItemSpawnerMode.On)
      {
        this.M.ItemSpawner.transform.position = this.SpawnPoints_Panels[0].position + Vector3.up * 0.8f;
        this.M.ItemSpawner.transform.rotation = this.SpawnPoints_Panels[0].rotation;
        this.M.ItemSpawner.SetActive(true);
      }
      for (int index = 0; index < this.SpawnPoint_Tables.Count; ++index)
        this.m_trackedObjects.Add(Object.Instantiate<GameObject>(this.M.Prefab_MetalTable, this.SpawnPoint_Tables[index].position, this.SpawnPoint_Tables[index].rotation));
      if (c.Has_Weapon_Primary)
      {
        TNH_CharacterDef.LoadoutEntry weaponPrimary = c.Weapon_Primary;
        int minAmmo = -1;
        int maxAmmo = -1;
        FVRObject randomObject;
        if (weaponPrimary.ListOverride.Count > 0)
        {
          randomObject = weaponPrimary.ListOverride[Random.Range(0, weaponPrimary.ListOverride.Count)];
        }
        else
        {
          ObjectTableDef tableDef = weaponPrimary.TableDefs[Random.Range(0, weaponPrimary.TableDefs.Count)];
          ObjectTable objectTable = new ObjectTable();
          objectTable.Initialize(tableDef);
          randomObject = objectTable.GetRandomObject();
          minAmmo = tableDef.MinAmmoCapacity;
          maxAmmo = tableDef.MaxAmmoCapacity;
        }
        GameObject gameObject = this.M.SpawnWeaponCase(this.M.Prefab_WeaponCaseLarge, this.SpawnPoint_CaseLarge.position, this.SpawnPoint_CaseLarge.forward, randomObject, weaponPrimary.Num_Mags_SL_Clips, weaponPrimary.Num_Rounds, minAmmo, maxAmmo, weaponPrimary.AmmoObjectOverride);
        this.m_trackedObjects.Add(gameObject);
        gameObject.GetComponent<TNH_WeaponCrate>().M = this.M;
      }
      if (c.Has_Weapon_Secondary)
      {
        TNH_CharacterDef.LoadoutEntry weaponSecondary = c.Weapon_Secondary;
        int minAmmo = -1;
        int maxAmmo = -1;
        FVRObject randomObject;
        if (weaponSecondary.ListOverride.Count > 0)
        {
          randomObject = weaponSecondary.ListOverride[Random.Range(0, weaponSecondary.ListOverride.Count)];
        }
        else
        {
          ObjectTableDef tableDef = weaponSecondary.TableDefs[Random.Range(0, weaponSecondary.TableDefs.Count)];
          ObjectTable objectTable = new ObjectTable();
          objectTable.Initialize(tableDef);
          randomObject = objectTable.GetRandomObject();
          minAmmo = tableDef.MinAmmoCapacity;
          maxAmmo = tableDef.MaxAmmoCapacity;
        }
        GameObject gameObject = this.M.SpawnWeaponCase(this.M.Prefab_WeaponCaseSmall, this.SpawnPoint_CaseSmall.position, this.SpawnPoint_CaseSmall.forward, randomObject, weaponSecondary.Num_Mags_SL_Clips, weaponSecondary.Num_Rounds, minAmmo, maxAmmo, weaponSecondary.AmmoObjectOverride);
        this.m_trackedObjects.Add(gameObject);
        gameObject.GetComponent<TNH_WeaponCrate>().M = this.M;
      }
      if (c.Has_Weapon_Tertiary)
      {
        TNH_CharacterDef.LoadoutEntry weaponTertiary = c.Weapon_Tertiary;
        FVRObject randomObject;
        if (weaponTertiary.ListOverride.Count > 0)
        {
          randomObject = weaponTertiary.ListOverride[Random.Range(0, weaponTertiary.ListOverride.Count)];
        }
        else
        {
          ObjectTableDef tableDef = weaponTertiary.TableDefs[Random.Range(0, weaponTertiary.TableDefs.Count)];
          ObjectTable objectTable = new ObjectTable();
          objectTable.Initialize(tableDef);
          randomObject = objectTable.GetRandomObject();
        }
        this.M.AddObjectToTrackedList(Object.Instantiate<GameObject>(randomObject.GetGameObject(), this.SpawnPoint_Melee.position, this.SpawnPoint_Melee.rotation));
      }
      if (c.Has_Item_Primary)
      {
        TNH_CharacterDef.LoadoutEntry itemPrimary = c.Item_Primary;
        FVRObject randomObject;
        if (itemPrimary.ListOverride.Count > 0)
        {
          randomObject = itemPrimary.ListOverride[Random.Range(0, itemPrimary.ListOverride.Count)];
        }
        else
        {
          ObjectTableDef tableDef = itemPrimary.TableDefs[Random.Range(0, itemPrimary.TableDefs.Count)];
          ObjectTable objectTable = new ObjectTable();
          objectTable.Initialize(tableDef);
          randomObject = objectTable.GetRandomObject();
        }
        this.M.AddObjectToTrackedList(Object.Instantiate<GameObject>(randomObject.GetGameObject(), this.SpawnPoints_SmallItem[0].position, this.SpawnPoints_SmallItem[0].rotation));
      }
      if (c.Has_Item_Secondary)
      {
        TNH_CharacterDef.LoadoutEntry itemSecondary = c.Item_Secondary;
        FVRObject randomObject;
        if (itemSecondary.ListOverride.Count > 0)
        {
          randomObject = itemSecondary.ListOverride[Random.Range(0, itemSecondary.ListOverride.Count)];
        }
        else
        {
          ObjectTableDef tableDef = itemSecondary.TableDefs[Random.Range(0, itemSecondary.TableDefs.Count)];
          ObjectTable objectTable = new ObjectTable();
          objectTable.Initialize(tableDef);
          randomObject = objectTable.GetRandomObject();
        }
        this.M.AddObjectToTrackedList(Object.Instantiate<GameObject>(randomObject.GetGameObject(), this.SpawnPoints_SmallItem[1].position, this.SpawnPoints_SmallItem[1].rotation));
      }
      if (c.Has_Item_Tertiary)
      {
        TNH_CharacterDef.LoadoutEntry itemTertiary = c.Item_Tertiary;
        FVRObject randomObject;
        if (itemTertiary.ListOverride.Count > 0)
        {
          randomObject = itemTertiary.ListOverride[Random.Range(0, itemTertiary.ListOverride.Count)];
        }
        else
        {
          ObjectTableDef tableDef = itemTertiary.TableDefs[Random.Range(0, itemTertiary.TableDefs.Count)];
          ObjectTable objectTable = new ObjectTable();
          objectTable.Initialize(tableDef);
          randomObject = objectTable.GetRandomObject();
        }
        this.M.AddObjectToTrackedList(Object.Instantiate<GameObject>(randomObject.GetGameObject(), this.SpawnPoints_SmallItem[2].position, this.SpawnPoints_SmallItem[2].rotation));
      }
      if (!c.Has_Item_Shield)
        return;
      TNH_CharacterDef.LoadoutEntry itemShield = c.Item_Shield;
      FVRObject randomObject1;
      if (itemShield.ListOverride.Count > 0)
      {
        randomObject1 = itemShield.ListOverride[Random.Range(0, itemShield.ListOverride.Count)];
      }
      else
      {
        ObjectTableDef tableDef = itemShield.TableDefs[Random.Range(0, itemShield.TableDefs.Count)];
        ObjectTable objectTable = new ObjectTable();
        objectTable.Initialize(tableDef);
        randomObject1 = objectTable.GetRandomObject();
      }
      this.M.AddObjectToTrackedList(Object.Instantiate<GameObject>(randomObject1.GetGameObject(), this.SpawnPoint_Shield.position, this.SpawnPoint_Shield.rotation));
    }

    public void ClearConfiguration() => this.DeleteAllActiveEntities();

    public void DeleteAllActiveEntities()
    {
      this.DeleteSosigs();
      this.DeleteTurrets();
      this.DeleteBoxes();
      if (this.m_trackedObjects.Count > 0)
      {
        for (int index = this.m_trackedObjects.Count - 1; index >= 0; --index)
        {
          if ((Object) this.m_trackedObjects[index] != (Object) null)
            Object.Destroy((Object) this.m_trackedObjects[index].gameObject);
        }
      }
      if ((Object) this.m_constructor != (Object) null)
      {
        this.m_constructor.GetComponent<TNH_ObjectConstructor>().ClearCase();
        Object.Destroy((Object) this.m_constructor);
        this.m_constructor = (GameObject) null;
      }
      if (!((Object) this.m_panel != (Object) null))
        return;
      Object.Destroy((Object) this.m_panel);
      this.m_panel = (GameObject) null;
    }

    private void DeleteSosigs()
    {
      for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_activeSosigs[index] != (Object) null)
          this.m_activeSosigs[index].DeSpawnSosig();
      }
      this.m_activeSosigs.Clear();
    }

    private void DeleteTurrets()
    {
      for (int index = this.m_activeTurrets.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_activeTurrets[index] != (Object) null)
          Object.Destroy((Object) this.m_activeTurrets[index].gameObject);
      }
      this.m_activeTurrets.Clear();
    }

    public bool IsPointInBounds(Vector3 p) => this.TestVolumeBool(this.Bounds, p);

    private void DeleteBoxes()
    {
      for (int index = this.m_spawnBoxes.Count - 1; index >= 0; --index)
      {
        if (this.m_spawnBoxes != null)
          Object.Destroy((Object) this.m_spawnBoxes[index]);
      }
      this.m_spawnBoxes.Clear();
    }

    public bool TestVolumeBool(Transform t, Vector3 pos)
    {
      bool flag = true;
      Vector3 vector3 = t.InverseTransformPoint(pos);
      if ((double) Mathf.Abs(vector3.x) > 0.5 || (double) Mathf.Abs(vector3.y) > 0.5 || (double) Mathf.Abs(vector3.z) > 0.5)
        flag = false;
      return flag;
    }

    private void OnDrawGizmos()
    {
      if (this.ShowPoints_SpawnTable)
      {
        Gizmos.color = new Color(1f, 0.2f, 0.2f);
        for (int index = 0; index < this.SpawnPoint_Tables.Count; ++index)
          Gizmos.DrawMesh(this.GizmoMesh_Table, this.SpawnPoint_Tables[index].position, this.SpawnPoint_Tables[index].rotation);
        Gizmos.DrawMesh(this.GizmoMesh_CaseLarge, this.SpawnPoint_CaseLarge.position, this.SpawnPoint_CaseLarge.rotation);
        Gizmos.DrawMesh(this.GizmoMesh_CaseSmall, this.SpawnPoint_CaseSmall.position, this.SpawnPoint_CaseSmall.rotation);
        for (int index = 0; index < this.SpawnPoints_SmallItem.Count; ++index)
          Gizmos.DrawSphere(this.SpawnPoints_SmallItem[index].position, 0.1f);
        Gizmos.DrawMesh(this.GizmoMesh_Melee, this.SpawnPoint_Melee.position, this.SpawnPoint_Melee.rotation);
        Gizmos.DrawCube(this.SpawnPoint_Shield.position, new Vector3(0.6f, 1f, 0.6f));
      }
      if (this.ShowPoint_PlayerSpawn)
      {
        Gizmos.color = new Color(1f, 0.2f, 0.2f);
        Gizmos.DrawMesh(this.GizmoMesh_SosigAttack, this.SpawnPoint_PlayerSpawn.position, this.SpawnPoint_PlayerSpawn.rotation);
      }
      if (this.ShowPoints_Sosigs_Defense)
      {
        for (int index = 0; index < this.SpawnPoints_Sosigs_Defense.Count; ++index)
        {
          Gizmos.color = new Color(0.0f, 0.8f, 0.8f);
          Gizmos.DrawMesh(this.GizmoMesh_SosigAttack, this.SpawnPoints_Sosigs_Defense[index].position, this.SpawnPoints_Sosigs_Defense[index].rotation);
        }
      }
      if (this.ShowPoints_Turrets)
      {
        for (int index = 0; index < this.SpawnPoints_Turrets.Count; ++index)
        {
          Gizmos.color = new Color(0.0f, 0.2f, 1f);
          Gizmos.DrawMesh(this.GizmoMesh_SosigAttack, this.SpawnPoints_Turrets[index].position, this.SpawnPoints_Turrets[index].rotation);
        }
      }
      if (this.ShowPoints_Screens)
      {
        for (int index = 0; index < this.SpawnPoints_Panels.Count; ++index)
        {
          Gizmos.color = new Color(0.0f, 1f, 1f);
          Gizmos.DrawMesh(this.GizmoMesh_Panel, this.SpawnPoints_Panels[index].position, this.SpawnPoints_Panels[index].rotation);
        }
      }
      if (!this.ShowPoints_Boxes)
        return;
      for (int index = 0; index < this.SpawnPoints_Boxes.Count; ++index)
      {
        Gizmos.color = new Color(1f, 1f, 0.0f);
        Gizmos.DrawMesh(this.GizmoMesh_Box, this.SpawnPoints_Boxes[index].position, this.SpawnPoints_Boxes[index].rotation);
      }
    }

    public enum SupplyPanelType
    {
      AmmoReloader,
      MagDuplicator,
      GunRecycler,
    }
  }
}
