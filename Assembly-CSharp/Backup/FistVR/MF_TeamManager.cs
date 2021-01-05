// Decompiled with JetBrains decompiler
// Type: FistVR.MF_TeamManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MF_TeamManager : MonoBehaviour
  {
    [Header("Prefabs")]
    public GameObject Prefab_CapturePoint;
    [Header("Teams")]
    public MF_Team RedTeam;
    public MF_Team BlueTeam;
    public int IFF_Red = 1;
    public int IFF_Blue = 2;
    [Header("SquadDefs")]
    public List<MF_SquadDefinition> SquadDefs;
    [Header("Zone Mode Configs")]
    public List<MF_ZoneModeConfig> ZoneModeConfigs;
    [Header("SosigShit")]
    public List<SosigEnemyTemplate> EnemyTemplatesByClassIndex_Red;
    public List<SosigEnemyTemplate> EnemyTemplatesByClassIndex_Blue;
    private bool m_hasInit;
    private MF_GameMode m_currentGameMode;
    public MF_Zone PlayerRespawnZone;
    public Material[] UberMats;

    public MF_GameMode GetMode() => this.m_currentGameMode;

    private void Start()
    {
    }

    public void InitGameMode(
      MF_GameMode mode,
      int RedTeamSize,
      int BlueTeamSize,
      MF_TeamColor playerTeam,
      float RedTeamSpawnCadence,
      float BlueTeamSpawnCadence,
      MF_PlayArea playArea)
    {
      this.m_currentGameMode = mode;
      GM.MFFlags.PlayerTeam = playerTeam;
      this.RedTeam.ResetZoneSet();
      this.BlueTeam.ResetZoneSet();
      this.RedTeam.LoadZoneList(this.GetZoneSet(mode, playArea, MF_ZoneCategory.SpawnRed), MF_ZoneType.Spawn);
      this.RedTeam.LoadZoneList(this.GetZoneSet(mode, playArea, MF_ZoneCategory.RedSide), MF_ZoneType.OwnSide);
      this.RedTeam.LoadZoneList(this.GetZoneSet(mode, playArea, MF_ZoneCategory.Neutral), MF_ZoneType.Center);
      this.RedTeam.LoadZoneList(this.GetZoneSet(mode, playArea, MF_ZoneCategory.BlueSide), MF_ZoneType.EnemySide);
      this.BlueTeam.LoadZoneList(this.GetZoneSet(mode, playArea, MF_ZoneCategory.SpawnBlue), MF_ZoneType.Spawn);
      this.BlueTeam.LoadZoneList(this.GetZoneSet(mode, playArea, MF_ZoneCategory.BlueSide), MF_ZoneType.OwnSide);
      this.BlueTeam.LoadZoneList(this.GetZoneSet(mode, playArea, MF_ZoneCategory.Neutral), MF_ZoneType.Center);
      this.BlueTeam.LoadZoneList(this.GetZoneSet(mode, playArea, MF_ZoneCategory.RedSide), MF_ZoneType.EnemySide);
      this.RedTeam.Init(this, MF_TeamColor.Red, this.BlueTeam, RedTeamSize, RedTeamSpawnCadence);
      this.BlueTeam.Init(this, MF_TeamColor.Blue, this.RedTeam, BlueTeamSize, BlueTeamSpawnCadence);
      Random.Range(0, 3);
      Random.Range(0, 3);
      Random.Range(0, 3);
      this.PlayerRespawnZone = this.GetPlayerRespawnZone(mode, playArea, playerTeam);
      this.m_hasInit = true;
    }

    public void Update()
    {
      if (!this.m_hasInit)
        return;
      float deltaTime = Time.deltaTime;
      this.RedTeam.Tick(deltaTime);
      this.BlueTeam.Tick(deltaTime);
    }

    public void RearmSosig(Sosig s, MF_Class c)
    {
      SosigEnemyTemplate sosigEnemyTemplate = this.EnemyTemplatesByClassIndex_Red[(int) c];
      GameObject original = (GameObject) null;
      if (sosigEnemyTemplate.WeaponOptions.Count > 0)
        original = sosigEnemyTemplate.WeaponOptions[Random.Range(0, sosigEnemyTemplate.WeaponOptions.Count)].GetGameObject();
      if (!((Object) original != (Object) null))
        return;
      Vector3 position = s.transform.position;
      Quaternion rotation = s.transform.rotation;
      SosigWeapon component = Object.Instantiate<GameObject>(original, position + Vector3.up * 0.1f, rotation).GetComponent<SosigWeapon>();
      component.SetAutoDestroy(true);
      component.O.SpawnLockable = false;
      if (component.Type == SosigWeapon.SosigWeaponType.Gun)
        s.Inventory.FillAmmoWithType(component.AmmoType);
      if (!((Object) component != (Object) null))
        return;
      s.InitHands();
      s.ForceEquip(component);
      component.SetAmmoClamping(true);
    }

    public Sosig SpawnEnemy(
      SosigEnemyTemplate t,
      Transform point,
      int IFF,
      bool IsAssault,
      Vector3 pointOfInterest,
      bool AllowSecondary,
      MF_TeamColor teamcolor)
    {
      GameObject weaponPrefab = (GameObject) null;
      if (t.WeaponOptions.Count > 0)
        weaponPrefab = t.WeaponOptions[Random.Range(0, t.WeaponOptions.Count)].GetGameObject();
      GameObject weaponPrefab2 = (GameObject) null;
      if (t.WeaponOptions_Secondary.Count > 0 && AllowSecondary && (double) Random.Range(0.0f, 1f) > (double) t.SecondaryChance)
        weaponPrefab2 = t.WeaponOptions_Secondary[Random.Range(0, t.WeaponOptions_Secondary.Count)].GetGameObject();
      SosigConfigTemplate configTemplate = t.ConfigTemplates[0];
      Sosig sosig = this.SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)].GetGameObject(), weaponPrefab, weaponPrefab2, point.position, point.rotation, configTemplate, t.OutfitConfig[Random.Range(0, t.OutfitConfig.Count)], IFF, IsAssault, pointOfInterest, teamcolor);
      sosig.SetInvulnMaterial(this.UberMats[(int) teamcolor]);
      return sosig;
    }

    private Sosig SpawnEnemySosig(
      GameObject prefab,
      GameObject weaponPrefab,
      GameObject weaponPrefab2,
      Vector3 pos,
      Quaternion rot,
      SosigConfigTemplate t,
      SosigOutfitConfig o,
      int IFF,
      bool IsAssault,
      Vector3 pointOfInterest,
      MF_TeamColor teamcolor)
    {
      Sosig componentInChildren = Object.Instantiate<GameObject>(prefab, pos, rot).GetComponentInChildren<Sosig>();
      componentInChildren.Configure(t);
      componentInChildren.E.IFFCode = IFF;
      if ((Object) weaponPrefab != (Object) null)
      {
        SosigWeapon component1 = Object.Instantiate<GameObject>(weaponPrefab, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
        component1.SetAutoDestroy(true);
        component1.O.SpawnLockable = false;
        if (component1.Type == SosigWeapon.SosigWeaponType.Gun)
          componentInChildren.Inventory.FillAmmoWithType(component1.AmmoType);
        if ((Object) component1 != (Object) null)
        {
          componentInChildren.InitHands();
          componentInChildren.ForceEquip(component1);
          component1.SetAmmoClamping(true);
        }
        if ((Object) weaponPrefab2 != (Object) null)
        {
          SosigWeapon component2 = Object.Instantiate<GameObject>(weaponPrefab2, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
          component2.SetAutoDestroy(true);
          component2.O.SpawnLockable = false;
          component2.SetAmmoClamping(true);
          if (component2.Type == SosigWeapon.SosigWeaponType.Gun)
            componentInChildren.Inventory.FillAmmoWithType(component2.AmmoType);
          if ((Object) component2 != (Object) null)
            componentInChildren.ForceEquip(component2);
        }
      }
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Headwear)
        this.SpawnAccesoryToLink(o.Headwear, componentInChildren.Links[0], teamcolor);
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Facewear)
        this.SpawnAccesoryToLink(o.Facewear, componentInChildren.Links[0], teamcolor);
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Torsowear)
        this.SpawnAccesoryToLink(o.Torsowear, componentInChildren.Links[1], teamcolor);
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Pantswear)
        this.SpawnAccesoryToLink(o.Pantswear, componentInChildren.Links[2], teamcolor);
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Pantswear_Lower)
        this.SpawnAccesoryToLink(o.Pantswear_Lower, componentInChildren.Links[3], teamcolor);
      if ((double) Random.Range(0.0f, 1f) < (double) o.Chance_Backpacks)
        this.SpawnAccesoryToLink(o.Backpacks, componentInChildren.Links[1], teamcolor);
      if (t.UsesLinkSpawns)
      {
        for (int index = 0; index < componentInChildren.Links.Count; ++index)
        {
          if ((double) Random.Range(0.0f, 1f) < (double) t.LinkSpawnChance[index])
            componentInChildren.Links[index].RegisterSpawnOnDestroy(t.LinkSpawns[index]);
        }
      }
      if (IsAssault)
      {
        componentInChildren.CommandAssaultPoint(pointOfInterest);
        componentInChildren.CurrentOrder = Sosig.SosigOrder.Assault;
        componentInChildren.FallbackOrder = Sosig.SosigOrder.Assault;
      }
      else
      {
        float num = Random.Range(0.0f, 1f);
        bool flag = false;
        if ((double) num > 0.25)
          flag = true;
        componentInChildren.CommandGuardPoint(pointOfInterest, true);
        componentInChildren.SetDominantGuardDirection(Random.onUnitSphere);
      }
      componentInChildren.SetGuardInvestigateDistanceThreshold(25f);
      return componentInChildren;
    }

    private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l, MF_TeamColor teamcolor)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(gs[Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }

    private MF_Zone GetPlayerRespawnZone(
      MF_GameMode mode,
      MF_PlayArea playArea,
      MF_TeamColor color)
    {
      MF_ZoneModeConfig mfZoneModeConfig = (MF_ZoneModeConfig) null;
      for (int index = 0; index < this.ZoneModeConfigs.Count; ++index)
      {
        if (mode == this.ZoneModeConfigs[index].Mode)
        {
          mfZoneModeConfig = this.ZoneModeConfigs[index];
          break;
        }
      }
      MF_PlayAreaConfig playAreaConfig = mfZoneModeConfig.GetPlayAreaConfig(playArea);
      if (color == MF_TeamColor.Red)
        return playAreaConfig.PlayerSpawnZone_Red;
      return color == MF_TeamColor.Blue ? playAreaConfig.PlayerSpawnZone_Blue : (MF_Zone) null;
    }

    private List<MF_Zone> GetZoneSet(
      MF_GameMode mode,
      MF_PlayArea playArea,
      MF_ZoneCategory category)
    {
      MF_ZoneModeConfig mfZoneModeConfig = (MF_ZoneModeConfig) null;
      for (int index = 0; index < this.ZoneModeConfigs.Count; ++index)
      {
        if (mode == this.ZoneModeConfigs[index].Mode)
        {
          mfZoneModeConfig = this.ZoneModeConfigs[index];
          break;
        }
      }
      return mfZoneModeConfig?.GetPlayAreaConfig(playArea).GetZoneSet(category);
    }
  }
}
