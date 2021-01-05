// Decompiled with JetBrains decompiler
// Type: FistVR.GPSosigManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class GPSosigManager : GPPlaceable
  {
    private List<Sosig> m_spawnedSosigs = new List<Sosig>();

    private void Start()
    {
      GM.CurrentSceneSettings.SosigKillEvent += new FVRSceneSettings.SosigKill(this.OnSosigKill);
      GM.CurrentGamePlannerManager.SosigManager = this;
    }

    private void OnDestroy() => GM.CurrentSceneSettings.SosigKillEvent -= new FVRSceneSettings.SosigKill(this.OnSosigKill);

    public override void Init(GPSceneMode mode)
    {
      this.FlushAllSosigs();
      this.m_spawnedSosigs.Clear();
      base.Init(mode);
    }

    private void OnSosigKill(Sosig s)
    {
      if ((Object) s == (Object) null)
        return;
      if (this.m_spawnedSosigs.Contains(s))
        this.m_spawnedSosigs.Remove(s);
      s.TickDownToClear(3f);
    }

    public void FlushAllSosigs()
    {
    }

    public Sosig SpawnSosig(
      SosigEnemyTemplate t,
      Transform point,
      GPSosigManager.InitialState initialState,
      GPSosigManager.Equipment equipState,
      GPSosigManager.Ammo ammoState,
      int teamIFF,
      int assaultGroup,
      int patrolGroup)
    {
      Sosig componentInChildren = Object.Instantiate<GameObject>((GameObject) null, point.position, point.rotation).GetComponentInChildren<Sosig>();
      componentInChildren.E.IFFCode = teamIFF;
      componentInChildren.SetOriginalIFFTeam(teamIFF);
      SosigOutfitConfig sosigOutfitConfig = t.OutfitConfig[Random.Range(0, t.OutfitConfig.Count)];
      this.SpawnAccesoryToLink(sosigOutfitConfig.Headwear, componentInChildren.Links[0], sosigOutfitConfig.Chance_Headwear);
      this.SpawnAccesoryToLink(sosigOutfitConfig.Facewear, componentInChildren.Links[0], sosigOutfitConfig.Chance_Headwear);
      this.SpawnAccesoryToLink(sosigOutfitConfig.Eyewear, componentInChildren.Links[0], sosigOutfitConfig.Chance_Headwear);
      this.SpawnAccesoryToLink(sosigOutfitConfig.Torsowear, componentInChildren.Links[1], sosigOutfitConfig.Chance_Headwear);
      this.SpawnAccesoryToLink(sosigOutfitConfig.Pantswear, componentInChildren.Links[2], sosigOutfitConfig.Chance_Headwear);
      this.SpawnAccesoryToLink(sosigOutfitConfig.Pantswear_Lower, componentInChildren.Links[3], sosigOutfitConfig.Chance_Headwear);
      this.SpawnAccesoryToLink(sosigOutfitConfig.Backpacks, componentInChildren.Links[1], sosigOutfitConfig.Chance_Headwear);
      componentInChildren.SetGuardInvestigateDistanceThreshold(25f);
      this.m_spawnedSosigs.Add(componentInChildren);
      return componentInChildren;
    }

    private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l, float chance)
    {
      if ((double) Random.Range(0.0f, 1f) > (double) chance)
        return;
      GameObject gameObject = Object.Instantiate<GameObject>(gs[Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }

    public enum SpawnWhen
    {
      OnBegin,
      OnEvent,
    }

    public enum InitialState
    {
      Guard,
      Assault,
      Inactive,
      Wander,
      Patrol,
    }

    public enum Equipment
    {
      All,
      Primary,
      NoNades,
      None,
    }

    public enum Ammo
    {
      Standard,
      Maxed,
      None,
    }
  }
}
