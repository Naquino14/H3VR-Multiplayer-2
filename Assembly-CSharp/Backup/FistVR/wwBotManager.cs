// Decompiled with JetBrains decompiler
// Type: FistVR.wwBotManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class wwBotManager : MonoBehaviour
  {
    public wwParkManager ParkManager;
    private bool m_ShouldBotsBeActive;
    public wwDeputyBadge Badge;
    public List<GameObject> SpawnedBots = new List<GameObject>();
    private int[] m_state_Bandits = new int[10];
    private int m_nextBotToSpawn;
    public Renderer[] Posters;
    public Texture2D[] PosterTextures;
    public GameObject[] CompletionCheckMarks;
    private float CheckBanditsCompletionTick = 0.1f;
    public wwBotManager.BotProfile[] BanditProfiles;
    public wwBotManager.BotProfile[] BonanzaBotProfiles;
    [Header("Civilian Bot Config")]
    public wwBotManager.BotProfile CivilianBotProfile_StartingArea;
    private List<GameObject> SpawnedCivilianBots_StartingArea = new List<GameObject>();
    private float CivBot_SpawnTick_StartingArea;
    public wwBotManager.BotProfile CivilianBotProfile_MidTown;
    private List<GameObject> SpawnedCivilianBots_MidTown = new List<GameObject>();
    private float CivBot_SpawnTick_MidTown;
    public wwBotManager.BotProfile CivilianBotProfile_DownTown;
    private List<GameObject> SpawnedCivilianBots_DownTown = new List<GameObject>();
    private float CivBot_SpawnTick_DownTown;
    private bool m_isInBonanzaMode;
    [Header("Other Config")]
    public AudioEvent PayoutSoundEvent;
    public ParticleSystem PSystemPayout;
    public GameObject PayoutPuffPrefab;
    private int BonanzaIndex;

    private void Start()
    {
      for (int index = 0; index < 3; ++index)
        this.SpawnCivvieBot(this.CivilianBotProfile_StartingArea, this.SpawnedCivilianBots_StartingArea);
      for (int index = 0; index < 3; ++index)
        this.SpawnCivvieBot(this.CivilianBotProfile_MidTown, this.SpawnedCivilianBots_MidTown);
      for (int index = 0; index < 3; ++index)
        this.SpawnCivvieBot(this.CivilianBotProfile_DownTown, this.SpawnedCivilianBots_DownTown);
    }

    public void ConfigureBotState(int[] states, int nextBotToSpawn)
    {
      this.m_state_Bandits = states;
      this.m_nextBotToSpawn = nextBotToSpawn;
      if (this.m_state_Bandits[this.m_nextBotToSpawn] == 1 || this.m_state_Bandits[this.m_nextBotToSpawn] == 2)
      {
        this.m_state_Bandits[this.m_nextBotToSpawn] = 0;
        this.ParkManager.RegisterBotStateChange(this.m_nextBotToSpawn, 0);
      }
      if (this.m_nextBotToSpawn == 9 && states[9] == 3)
        this.m_isInBonanzaMode = true;
      if (this.m_nextBotToSpawn < 9)
      {
        if ((UnityEngine.Object) this.PosterTextures[this.m_nextBotToSpawn] != (UnityEngine.Object) null)
        {
          foreach (Renderer poster in this.Posters)
            poster.materials[0].SetTexture("_MainTex", (Texture) this.PosterTextures[this.m_nextBotToSpawn]);
        }
      }
      else
      {
        foreach (Renderer poster in this.Posters)
          poster.materials[0].SetTexture("_MainTex", (Texture) this.PosterTextures[10]);
      }
      this.UpdateCheckMarks(states);
    }

    public void UpdateCheckMarks(int[] states)
    {
      for (int index = 0; index < states.Length; ++index)
      {
        if (states[index] == 0)
          this.CompletionCheckMarks[index].SetActive(false);
        else
          this.CompletionCheckMarks[index].SetActive(true);
      }
    }

    public void DespawnActiveBots()
    {
      if (this.SpawnedBots.Count <= 0)
        return;
      if (this.m_state_Bandits[this.m_nextBotToSpawn] == 1)
      {
        this.m_state_Bandits[this.m_nextBotToSpawn] = 0;
        this.ParkManager.RegisterBotStateChange(this.m_nextBotToSpawn, 0);
      }
      for (int index = this.SpawnedBots.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) this.SpawnedBots[index] != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.SpawnedBots[index]);
      }
      this.SpawnedBots.Clear();
    }

    public void SpawnBot(int index)
    {
      this.m_state_Bandits[index] = 1;
      this.ParkManager.RegisterBotStateChange(index, 1);
      wwBotManager.BotProfile banditProfile = this.BanditProfiles[this.m_nextBotToSpawn];
      GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(banditProfile.BanditPrefab, banditProfile.SpawnGroup.Spawn_MainBandit.position, banditProfile.SpawnGroup.Spawn_MainBandit.rotation);
      gameObject1.GetComponent<wwBotWurst>().ConfigBot(this.m_nextBotToSpawn, false, this, banditProfile.NavGroup, (Transform) null);
      this.SpawnedBots.Add(gameObject1);
      GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject1);
      for (int index1 = 0; index1 < banditProfile.PossePrefabs.Length; ++index1)
      {
        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(banditProfile.PossePrefabs[index1], banditProfile.SpawnGroup.Spawns_Posse[index1].position, banditProfile.SpawnGroup.Spawns_Posse[index1].rotation);
        gameObject2.GetComponent<wwBotWurst>().ConfigBot(-1, true, this, banditProfile.NavGroup, (Transform) null);
        this.SpawnedBots.Add(gameObject2);
        GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject2);
      }
    }

    public void BotKilled(int index, Vector3 v)
    {
      if (index >= 0 && index < 10)
      {
        this.m_state_Bandits[index] = 2;
        this.ParkManager.RegisterBotStateChange(index, 2);
      }
      else
      {
        if (index < 10 && index >= 0)
          return;
        GM.Omni.OmniUnlocks.GainCurrency(100);
        GM.Omni.SaveUnlocksToFile();
        UnityEngine.Object.Instantiate<GameObject>(this.PayoutPuffPrefab, v, Quaternion.identity);
      }
    }

    public void BotHatRetrieved(int index)
    {
      this.m_state_Bandits[index] = 3;
      GM.Omni.OmniUnlocks.GainCurrency(this.BanditProfiles[this.m_nextBotToSpawn].RewardWhenReturned);
      GM.Omni.SaveUnlocksToFile();
      this.ParkManager.RegisterBotStateChange(index, 3);
      SM.PlayGenericSound(this.PayoutSoundEvent, GM.CurrentPlayerBody.transform.position);
      this.PSystemPayout.Emit(100);
      if (this.m_nextBotToSpawn < 9)
      {
        ++this.m_nextBotToSpawn;
        this.ParkManager.RegisterNextBotToSpawn(this.m_nextBotToSpawn);
        if ((UnityEngine.Object) this.PosterTextures[this.m_nextBotToSpawn] != (UnityEngine.Object) null)
        {
          foreach (Renderer poster in this.Posters)
            poster.materials[0].SetTexture("_MainTex", (Texture) this.PosterTextures[this.m_nextBotToSpawn]);
        }
      }
      else
      {
        this.ParkManager.UnlockChest(1);
        foreach (Renderer poster in this.Posters)
          poster.materials[0].SetTexture("_MainTex", (Texture) this.PosterTextures[10]);
      }
      this.CompletionCheckMarks[index].SetActive(true);
    }

    private void SpawnRandomBonanzaBotSet()
    {
      wwBotManager.BotProfile bonanzaBotProfile = this.BonanzaBotProfiles[this.BonanzaIndex];
      ++this.BonanzaIndex;
      if (this.BonanzaIndex >= this.BonanzaBotProfiles.Length)
        this.BonanzaIndex = 4;
      for (int index = 0; index < bonanzaBotProfile.PossePrefabs.Length; ++index)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(bonanzaBotProfile.PossePrefabs[index], bonanzaBotProfile.SpawnGroup.Spawns_Posse[index].position, bonanzaBotProfile.SpawnGroup.Spawns_Posse[index].rotation);
        gameObject.GetComponent<wwBotWurst>().ConfigBot(11, true, this, bonanzaBotProfile.NavGroup, (Transform) null);
        this.SpawnedBots.Add(gameObject);
        GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
      }
    }

    private void CivilianBotTracker()
    {
      this.TrackCivBots(this.CivilianBotProfile_StartingArea, this.SpawnedCivilianBots_StartingArea, ref this.CivBot_SpawnTick_StartingArea);
      this.TrackCivBots(this.CivilianBotProfile_MidTown, this.SpawnedCivilianBots_MidTown, ref this.CivBot_SpawnTick_MidTown);
      this.TrackCivBots(this.CivilianBotProfile_DownTown, this.SpawnedCivilianBots_DownTown, ref this.CivBot_SpawnTick_DownTown);
    }

    private void TrackCivBots(
      wwBotManager.BotProfile profile,
      List<GameObject> botList,
      ref float tick)
    {
      if ((double) tick > 0.0)
        tick -= Time.deltaTime;
      if (botList.Count > 0)
      {
        for (int index = botList.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) botList[index] == (UnityEngine.Object) null)
            botList.RemoveAt(index);
        }
      }
      if (botList.Count >= 3 || (double) tick > 0.0)
        return;
      tick = UnityEngine.Random.Range(10f, 15f);
      this.SpawnCivvieBot(profile, botList);
    }

    private void SpawnCivvieBot(wwBotManager.BotProfile profile, List<GameObject> botList)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(profile.BanditPrefab, profile.SpawnGroup.Spawns_Posse[UnityEngine.Random.Range(0, profile.SpawnGroup.Spawns_Posse.Length)].position, profile.SpawnGroup.Spawns_Posse[UnityEngine.Random.Range(0, profile.SpawnGroup.Spawns_Posse.Length)].rotation);
      gameObject.GetComponent<wwBotWurst>().NavPointGroup = profile.NavGroup;
      botList.Add(gameObject);
    }

    private void Update()
    {
      this.CivilianBotTracker();
      this.m_ShouldBotsBeActive = !((UnityEngine.Object) this.Badge.QuickbeltSlot == (UnityEngine.Object) null);
      if (this.m_nextBotToSpawn == 9 && GM.WWSaveGame.Flags.State_Bandits[9] == 3)
        this.m_isInBonanzaMode = true;
      if (!this.m_ShouldBotsBeActive)
        this.DespawnActiveBots();
      else if (this.m_isInBonanzaMode)
      {
        if (this.SpawnedBots.Count <= 0)
          this.SpawnRandomBonanzaBotSet();
      }
      else if (this.m_state_Bandits[this.m_nextBotToSpawn] == 0)
        this.SpawnBot(this.m_nextBotToSpawn);
      if (this.ParkManager.RewardChests[1].GetState() >= 1)
        return;
      if ((double) this.CheckBanditsCompletionTick > 0.0)
      {
        this.CheckBanditsCompletionTick -= Time.deltaTime;
      }
      else
      {
        this.CheckBanditsCompletionTick = 1f;
        bool flag = false;
        if (this.m_nextBotToSpawn == 9 && GM.WWSaveGame.Flags.State_Bandits[9] == 3)
          flag = true;
        if (!flag)
          return;
        this.ParkManager.UnlockChest(1);
      }
    }

    [Serializable]
    public class BotProfile
    {
      public GameObject BanditPrefab;
      public GameObject[] PossePrefabs;
      public wwBotWurstNavPointGroup NavGroup;
      public wwBotWurstSpawnPointGroup SpawnGroup;
      public int RewardWhenReturned;
    }
  }
}
