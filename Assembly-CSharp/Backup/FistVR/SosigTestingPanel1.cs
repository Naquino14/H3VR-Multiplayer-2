// Decompiled with JetBrains decompiler
// Type: FistVR.SosigTestingPanel1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class SosigTestingPanel1 : MonoBehaviour
  {
    public Transform[] SosigSpawningPoint;
    public List<GameObject> SosigPrefabs;
    public List<GameObject> SosigGunPrefabs;
    public SosigConfigTemplate BaseTemplate;
    public SosigConfigTemplate Template;
    public SosigWearableConfig[] WearableConfig_Team1;
    public SosigWearableConfig[] WearableConfig_Team2;
    public List<GameObject> Sosigcessories_Face;
    public List<GameObject> Sosigcessories_Helmet;
    public List<GameObject> Sosigcessories_Head;
    public List<GameObject> Sosigcessories_Torso;
    public List<GameObject> Sosigcessories_Backpack;
    public List<GameObject> Sosigcessories_UpperLink;
    public List<GameObject> Sosigcessories_LowerLink;
    public List<Material> SosigMats;
    public List<Transform> RespawnPoints;
    private List<GameObject> m_spawnedSosigs = new List<GameObject>();
    private List<GameObject> m_spawnedSosigGuns = new List<GameObject>();
    [Header("Team Fight Annex")]
    public List<Transform> AssaultPoints_Team1;
    public List<Transform> AssaultPoints_Team2;
    public List<Transform> SpawnPoints_Team1;
    public List<Transform> SpawnPoints_Team2;
    public List<GameObject> SosigWeapons_SMGs;
    public List<GameObject> SosigWeapons_Rifles;
    public List<GameObject> SosigWeapons_Handguns;
    public List<GameObject> SosigWeapons_Shotguns;
    public List<GameObject> SosigWeapons_Melee;
    public List<GameObject> SosigWeapons_Shield;
    public List<GameObject> SosigWeapons_Grenades;
    public bool m_isTeamFightEnabled;
    public int m_maxTeamSizeTeam1;
    public int m_maxTeamSizeTeam2;
    public int m_team1EquipMode;
    public int m_team2EquipMode;
    public int m_team1ArmorMode;
    public int m_team2ArmorMode;
    private int m_nextSpawnPointTeam1;
    private int m_nextSpawnPointTeam2;
    private List<Sosig> m_spawnedTeam1 = new List<Sosig>();
    private List<Sosig> m_spawnedTeam2 = new List<Sosig>();
    private float m_spawnSpeed = 3f;
    private float m_nextTeam1Spawn = 3f;
    private float m_nextTeam2Spawn = 3f;
    public Text NumBotsActive;

    public void TeamFight_Enabled() => this.m_isTeamFightEnabled = true;

    public void TeamFight_Disabled()
    {
      this.m_isTeamFightEnabled = false;
      if (this.m_spawnedTeam1.Count > 0)
      {
        for (int index = this.m_spawnedTeam1.Count - 1; index >= 0; --index)
        {
          if ((Object) this.m_spawnedTeam1[index] != (Object) null)
            this.m_spawnedTeam1[index].ClearSosig();
        }
        this.m_spawnedTeam1.Clear();
      }
      if (this.m_spawnedTeam2.Count <= 0)
        return;
      for (int index = this.m_spawnedTeam2.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_spawnedTeam2[index] != (Object) null)
          this.m_spawnedTeam2[index].ClearSosig();
      }
      this.m_spawnedTeam2.Clear();
    }

    public void SetTeam1EquipMode(int i) => this.m_team1EquipMode = i;

    public void SetTeam2EquipMode(int i) => this.m_team2EquipMode = i;

    public void SetTeam1ArmorMode(int i) => this.m_team1ArmorMode = i;

    public void SetTeam2ArmorMode(int i) => this.m_team2ArmorMode = i;

    public void SetTeamSizeTeam1(int i) => this.m_maxTeamSizeTeam1 = i;

    public void SetTeamSizeTeam2(int i) => this.m_maxTeamSizeTeam2 = i;

    public void Update() => this.UpdateTeamFight();

    public void SetPlayerHealth(int i) => GM.CurrentPlayerBody.SetHealthThreshold((float) i);

    private void UpdateTeamFight()
    {
      if ((Object) this.NumBotsActive != (Object) null)
        this.NumBotsActive.text = (this.m_spawnedTeam1.Count + this.m_spawnedTeam2.Count).ToString() + " Bots";
      if (this.m_spawnedTeam1.Count > 0)
      {
        for (int index = this.m_spawnedTeam1.Count - 1; index >= 0; --index)
        {
          if (this.m_spawnedTeam1[index].BodyState == Sosig.SosigBodyState.Dead)
          {
            this.m_spawnedTeam1[index].TickDownToClear(5f);
            this.m_spawnedTeam1.RemoveAt(index);
          }
        }
      }
      if (this.m_spawnedTeam2.Count > 0)
      {
        for (int index = this.m_spawnedTeam2.Count - 1; index >= 0; --index)
        {
          if (this.m_spawnedTeam2[index].BodyState == Sosig.SosigBodyState.Dead)
          {
            this.m_spawnedTeam2[index].TickDownToClear(5f);
            this.m_spawnedTeam2.RemoveAt(index);
          }
        }
      }
      if (!this.m_isTeamFightEnabled)
        return;
      if (this.m_spawnedTeam1.Count < this.m_maxTeamSizeTeam1 && (double) this.m_nextTeam1Spawn > 0.0)
        this.m_nextTeam1Spawn -= Time.deltaTime;
      if (this.m_spawnedTeam2.Count < this.m_maxTeamSizeTeam2 && (double) this.m_nextTeam2Spawn > 0.0)
        this.m_nextTeam2Spawn -= Time.deltaTime;
      if ((double) this.m_nextTeam1Spawn <= 0.0)
      {
        this.m_nextTeam1Spawn = Random.Range(this.m_spawnSpeed, this.m_spawnSpeed * 1.2f);
        this.SpawnTeamFightSosig(0);
      }
      if ((double) this.m_nextTeam2Spawn > 0.0)
        return;
      this.m_nextTeam2Spawn = Random.Range(this.m_spawnSpeed, this.m_spawnSpeed * 1.2f);
      this.SpawnTeamFightSosig(1);
    }

    private void SpawnTeamFightSosig(int team)
    {
      Transform transform1;
      Transform transform2;
      List<Sosig> sosigList;
      if (team == 0)
      {
        transform1 = this.SpawnPoints_Team1[Random.Range(0, this.m_nextSpawnPointTeam1)];
        ++this.m_nextSpawnPointTeam1;
        if (this.m_nextSpawnPointTeam1 >= this.SpawnPoints_Team1.Count)
          this.m_nextSpawnPointTeam1 = 0;
        transform2 = this.AssaultPoints_Team1[Random.Range(0, this.AssaultPoints_Team1.Count)];
        sosigList = this.m_spawnedTeam1;
      }
      else
      {
        transform1 = this.SpawnPoints_Team2[Random.Range(0, this.m_nextSpawnPointTeam2)];
        ++this.m_nextSpawnPointTeam2;
        if (this.m_nextSpawnPointTeam2 >= this.SpawnPoints_Team2.Count)
          this.m_nextSpawnPointTeam2 = 0;
        transform2 = this.AssaultPoints_Team2[Random.Range(0, this.AssaultPoints_Team2.Count)];
        sosigList = this.m_spawnedTeam2;
      }
      SosigWearableConfig w = (SosigWearableConfig) null;
      switch (team)
      {
        case 0:
          int num1 = this.m_team1ArmorMode;
          if (num1 == 0)
            num1 = Random.Range(1, 4);
          w = this.WearableConfig_Team1[num1 - 1];
          break;
        case 1:
          int num2 = this.m_team2ArmorMode;
          if (num2 == 0)
            num2 = Random.Range(1, 4);
          w = this.WearableConfig_Team2[num2 - 1];
          break;
      }
      Sosig sosig = this.SpawnEnemySosig(this.SosigPrefabs[0], transform1.position, transform1.rotation, this.Template, w);
      sosig.E.IFFCode = team + 1;
      sosig.CommandAssaultPoint(transform2.position);
      sosigList.Add(sosig);
      SosigWeapon teamFightGun = this.GetTeamFightGun(team, false);
      teamFightGun.transform.position = transform1.position;
      teamFightGun.SetAutoDestroy(true);
      if (!((Object) teamFightGun != (Object) null))
        return;
      sosig.InitHands();
      sosig.ForceEquip(teamFightGun);
    }

    private void SpawnAccesoryToLink(List<GameObject> gs, SosigLink l)
    {
      if (gs.Count < 1)
        return;
      GameObject gameObject = Object.Instantiate<GameObject>(gs[Random.Range(0, gs.Count)], l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }

    private Sosig SpawnEnemySosig(
      GameObject prefab,
      Vector3 pos,
      Quaternion rot,
      SosigConfigTemplate t,
      SosigWearableConfig w)
    {
      Sosig componentInChildren = Object.Instantiate<GameObject>(prefab, pos, rot).GetComponentInChildren<Sosig>();
      componentInChildren.Inventory.FillAllAmmo();
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Headwear)
        this.SpawnAccesoryToLink(w.Headwear, componentInChildren.Links[0]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Facewear)
        this.SpawnAccesoryToLink(w.Facewear, componentInChildren.Links[0]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Torsowear)
        this.SpawnAccesoryToLink(w.Torsowear, componentInChildren.Links[1]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear)
        this.SpawnAccesoryToLink(w.Pantswear, componentInChildren.Links[2]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear_Lower)
        this.SpawnAccesoryToLink(w.Pantswear_Lower, componentInChildren.Links[3]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Backpacks)
        this.SpawnAccesoryToLink(w.Backpacks, componentInChildren.Links[1]);
      componentInChildren.Configure(t);
      return componentInChildren;
    }

    public SosigWeapon GetTeamFightGun(int team, bool grenade, bool isShield = false)
    {
      int num = 0;
      switch (team)
      {
        case 0:
          num = this.m_team1EquipMode;
          break;
        case 1:
          num = this.m_team2EquipMode;
          break;
      }
      List<GameObject> gameObjectList = this.SosigWeapons_SMGs;
      if (num == 0)
        num = Random.Range(1, 6);
      switch (num - 1)
      {
        case 0:
          gameObjectList = this.SosigWeapons_SMGs;
          break;
        case 1:
          gameObjectList = this.SosigWeapons_Rifles;
          break;
        case 2:
          gameObjectList = this.SosigWeapons_Handguns;
          break;
        case 3:
          gameObjectList = this.SosigWeapons_Shotguns;
          break;
        case 4:
          gameObjectList = this.SosigWeapons_Melee;
          break;
      }
      if (grenade)
        gameObjectList = this.SosigWeapons_Grenades;
      if (isShield)
        gameObjectList = this.SosigWeapons_Shield;
      return Object.Instantiate<GameObject>(gameObjectList[Random.Range(0, gameObjectList.Count)], new Vector3(0.0f, 30f, 0.0f), Quaternion.identity).GetComponent<SosigWeapon>();
    }

    public void Spawn1Sosigs()
    {
      for (int index = 0; index < 1; ++index)
      {
        Transform transform = this.SosigSpawningPoint[Random.Range(0, this.SosigSpawningPoint.Length)];
        this.SpawnASosig(transform.position, transform.rotation, Random.Range(0, this.SosigMats.Count));
      }
    }

    public void Spawn5Sosigs()
    {
      for (int index = 0; index < 5; ++index)
      {
        Transform transform = this.SosigSpawningPoint[Random.Range(0, this.SosigSpawningPoint.Length)];
        this.SpawnASosig(transform.position, transform.rotation, Random.Range(0, this.SosigMats.Count));
      }
    }

    private Sosig SpawnASosig(Vector3 pointToSpawn, Quaternion rotation, int matIndex)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.SosigPrefabs[Random.Range(0, this.SosigPrefabs.Count)], pointToSpawn, Quaternion.identity);
      this.m_spawnedSosigs.Add(gameObject);
      Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
      componentInChildren.Inventory.FillAllAmmo();
      float num = Random.Range(0.0f, 1f);
      Random.Range(0.0f, 1f);
      if ((double) num > 0.600000023841858)
        this.SpawnAccesoryToLink(this.Sosigcessories_Face, componentInChildren.Links[0], matIndex);
      if ((double) Random.Range(0.0f, 1f) > 0.100000001490116)
      {
        this.SpawnAccesoryToLink(this.Sosigcessories_Helmet, componentInChildren.Links[0], matIndex);
        if ((double) Random.Range(0.0f, 1f) > 0.800000011920929)
          this.SpawnAccesoryToLink(this.Sosigcessories_Head, componentInChildren.Links[0], matIndex);
      }
      if ((double) Random.Range(0.0f, 1f) > 0.200000002980232)
        this.SpawnAccesoryToLink(this.Sosigcessories_Torso, componentInChildren.Links[1], matIndex);
      if ((double) Random.Range(0.0f, 1f) > 0.5)
      {
        this.SpawnAccesoryToLink(this.Sosigcessories_UpperLink, componentInChildren.Links[2], matIndex);
        if ((double) Random.Range(0.0f, 1f) > 0.800000011920929)
          this.SpawnAccesoryToLink(this.Sosigcessories_LowerLink, componentInChildren.Links[3], matIndex);
      }
      if ((double) Random.Range(0.0f, 1f) > 0.800000011920929)
      {
        this.SpawnAccesoryToLink(this.Sosigcessories_Backpack, componentInChildren.Links[1], matIndex);
        if ((double) Random.Range(0.0f, 1f) > 0.800000011920929)
          this.SpawnAccesoryToLink(this.Sosigcessories_Backpack, componentInChildren.Links[2], matIndex);
      }
      componentInChildren.Configure(this.Template);
      return componentInChildren;
    }

    private void SpawnAccesoryToLink(List<GameObject> gs, SosigLink l, int MatIndex)
    {
      if (gs.Count < 1)
        return;
      GameObject gameObject = Object.Instantiate<GameObject>(gs[Random.Range(0, gs.Count)], l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
      MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
      if (!((Object) component != (Object) null) || !this.SosigMats.Contains(component.sharedMaterial))
        return;
      component.material = this.SosigMats[MatIndex];
    }

    public void SpawnSosigun(int gun)
    {
      for (int index = 0; index < 1; ++index)
      {
        Transform transform = this.SosigSpawningPoint[Random.Range(0, this.SosigSpawningPoint.Length)];
        this.m_spawnedSosigGuns.Add(Object.Instantiate<GameObject>(this.SosigGunPrefabs[gun], transform.position + Vector3.up * 2f, Random.rotation));
      }
    }

    public void SpawnRandomSosigun()
    {
      for (int index = 0; index < 5; ++index)
      {
        Transform transform = this.SosigSpawningPoint[Random.Range(0, this.SosigSpawningPoint.Length)];
        this.m_spawnedSosigGuns.Add(Object.Instantiate<GameObject>(this.SosigGunPrefabs[Random.Range(0, this.SosigGunPrefabs.Count)], transform.position + Vector3.up * 2f, Random.rotation));
      }
    }

    public void ClearSosigs()
    {
      if (this.m_spawnedSosigs.Count == 0)
        return;
      for (int index = this.m_spawnedSosigs.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_spawnedSosigs[index] != (Object) null)
          Object.Destroy((Object) this.m_spawnedSosigs[index]);
      }
      SosigLink[] objectsOfType = Object.FindObjectsOfType<SosigLink>();
      if (objectsOfType.Length > 0)
      {
        for (int index = objectsOfType.Length - 1; index >= 0; --index)
        {
          if ((Object) objectsOfType[index] != (Object) null)
            Object.Destroy((Object) objectsOfType[index].gameObject);
        }
      }
      this.m_spawnedSosigs.Clear();
    }

    public void ClearSosigGuns()
    {
      if (this.m_spawnedSosigGuns.Count == 0)
        return;
      this.SetSosigOrder(0);
      for (int index = this.m_spawnedSosigGuns.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_spawnedSosigGuns[index] != (Object) null)
          Object.Destroy((Object) this.m_spawnedSosigGuns[index]);
      }
      this.m_spawnedSosigGuns.Clear();
    }

    public void SetPlayerIFF(int iff)
    {
      switch (iff)
      {
        case 0:
          GM.CurrentPlayerBody.SetPlayerIFF(-3);
          break;
        case 1:
          GM.CurrentPlayerBody.SetPlayerIFF(0);
          GM.CurrentSceneSettings.DeathResetPoint = this.RespawnPoints[0];
          break;
        case 2:
          GM.CurrentPlayerBody.SetPlayerIFF(1);
          GM.CurrentSceneSettings.DeathResetPoint = this.RespawnPoints[1];
          break;
        case 3:
          GM.CurrentPlayerBody.SetPlayerIFF(2);
          GM.CurrentSceneSettings.DeathResetPoint = this.RespawnPoints[2];
          break;
      }
    }

    public void SetSosigIFF(int iff)
    {
      Sosig[] objectsOfType = Object.FindObjectsOfType<Sosig>();
      for (int index = 0; index < objectsOfType.Length; ++index)
      {
        if (objectsOfType[index].CurrentOrder != Sosig.SosigOrder.Disabled)
        {
          switch (iff)
          {
            case 0:
              objectsOfType[index].E.IFFCode = 1;
              continue;
            case 1:
              objectsOfType[index].E.IFFCode = index + 1;
              continue;
            case 2:
              objectsOfType[index].E.IFFCode = (double) index >= (double) objectsOfType.Length / 2.0 ? 2 : 1;
              continue;
            default:
              continue;
          }
        }
      }
    }

    public void SetSosigOrder(int order)
    {
      Sosig[] objectsOfType = Object.FindObjectsOfType<Sosig>();
      for (int index = 0; index < objectsOfType.Length; ++index)
      {
        if (order == 0)
        {
          objectsOfType[index].CurrentOrder = Sosig.SosigOrder.Disabled;
          objectsOfType[index].FallbackOrder = Sosig.SosigOrder.Disabled;
        }
        else
        {
          objectsOfType[index].CurrentOrder = Sosig.SosigOrder.Wander;
          objectsOfType[index].FallbackOrder = Sosig.SosigOrder.Wander;
        }
      }
    }
  }
}
