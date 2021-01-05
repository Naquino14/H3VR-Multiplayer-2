// Decompiled with JetBrains decompiler
// Type: FistVR.TAH_Manager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FistVR
{
  public class TAH_Manager : MonoBehaviour
  {
    public GameObject TAHMenu;
    public TAH_Manager.TAHLogicStyle LogicStyle;
    public List<TAH_SupplyPoint> SupplyPoints;
    public List<TAH_DefensePoint> DefensePoints;
    public TAH_Manager.TAHGameState State;
    private int m_curDefensePointIndex = -1;
    private int m_lastDefensePointIndex = -1;
    private int m_curSupplyPointIndex = -1;
    private int m_lastSupplyPointIndex = -1;
    private int m_pointsTaken;
    public SV_LootTable LTList_PowerUps;
    public GameObject TAHCratePrefabLarge;
    public GameObject TAHCratePrefabSmall;
    public List<TAH_WaveDefinition> WaveDefinitions = new List<TAH_WaveDefinition>();
    private List<TAH_WaveDefinition> m_holdSequence = new List<TAH_WaveDefinition>();
    private float m_holdTimer;
    private float m_waveTimer;
    private int m_currentWave;
    private int m_maxWave;
    private List<GameObject> SpawnedBots = new List<GameObject>();
    private List<GameObject> SpawnedBots_Attacking = new List<GameObject>();
    public TAH_BotSpawnProfile[] SupplyPointBotProfiles;
    public TAH_BotSpawnProfile[] DefensePointBotProfiles;
    public List<TAH_MobSpawnGroup> MobSpawnGroups;
    private List<GameObject> m_spawnedMobs = new List<GameObject>();
    private HashSet<FVRPhysicalObject> m_knownObjsHash = new HashSet<FVRPhysicalObject>();
    private List<FVRPhysicalObject> m_knownObjs = new List<FVRPhysicalObject>();
    private int knownObjectCheckIndex;
    public bool UsesOldReticle = true;
    public Transform Reticle_Root;
    public Transform Reticle_TAH;
    public Transform Reticle_Supply;
    public Transform Reticle_Beacon;
    public TAH_Reticle TAHReticle;
    public GameObject FastProjectile;
    public GameObject ItemSpawner;
    public FVRFMODController FMODController;
    public LootTable LT_TestAllGuns = new LootTable();
    public List<LootTable> LTChart_CaseWeapon = new List<LootTable>();
    public LootTable LT_Firearms_SideArm = new LootTable();
    public LootTable LT_Firearms_SecondaryWeapon_LowCap = new LootTable();
    public LootTable LT_Firearms_SecondaryWeapon_HighCap = new LootTable();
    public LootTable LT_Firearms_SniperRifle = new LootTable();
    public LootTable LT_Firearms_PrimaryWeapon_LowCap = new LootTable();
    public LootTable LT_Firearms_PrimaryWeapon_MedCap = new LootTable();
    public LootTable LT_Firearms_PrimaryWeapon_HighCap = new LootTable();
    public LootTable LT_Firearms_Shotgun_LowCap = new LootTable();
    public LootTable LT_Firearms_Shotgun_HighCap = new LootTable();
    public LootTable LT_Firearms_SmallOrdnance = new LootTable();
    public LootTable LT_FirearmsSpecial = new LootTable();
    public LootTable LT_Grenades = new LootTable();
    public LootTable LT_Health = new LootTable();
    public LootTable LT_PowerUps = new LootTable();
    public LootTable LT_CommonAttachments = new LootTable();
    public LootTable LT_RareAttachments = new LootTable();
    public LootTable LT_MeleeWeapons = new LootTable();
    public LootTable LT_Utility = new LootTable();
    public LootTable LT_RequiredAttachments = new LootTable();
    private int m_ltStartMin;
    private int m_ltStartMax = 1;
    private int m_ltDiffModMin;
    private int m_ltDiffModMax = 1;
    private int m_ltDiffMinCap;
    private int m_maxWaveType;
    private int m_waveDifficultyMod;
    private float m_botHealthMultiplier = 1f;
    private float m_lootThrownThreshold = 0.9f;
    private float m_lootRareAttachThreshold = 0.8f;
    private float m_lootCommonAttachThreshold = 0.5f;
    private float m_timeUntilWeCanEventCheck = 60f;
    private Vector2 m_eventTriggerTickRange = new Vector2(20f, 60f);
    private HashSet<TAH_SupplyPoint> m_eventFiredSoFar_SupplyPoint = new HashSet<TAH_SupplyPoint>();
    private int m_checkIndex_SupplyPoint;
    private List<GameObject> m_eventSpawns = new List<GameObject>();
    public TAH_BotSpawnProfile[] EventProfiles_MeatZombies;
    public TAH_BotSpawnProfile[] EventProfiles_SpecOps;
    public TAH_BotSpawnProfile[] EventProfiles_FastZombies;
    public GameObject[] EventPrefabs_MeatCrabs;
    private int m_generatedStyle;
    private float m_takeMusicIntensity = 1f;
    private float m_takeMusicIntensityTarget = 1f;
    private float m_fireThreshold;
    private float m_botKillThreshold;
    private float PingTimer = 5f;

    public int GetDifficulty() => this.m_pointsTaken;

    private void Awake()
    {
      GM.TAHMaster = this;
      if (!this.UsesOldReticle)
        return;
      this.Reticle_Root.gameObject.SetActive(false);
    }

    private void Start()
    {
      this.FMODController.SetMasterVolume(0.25f);
      this.GenerateLootTables();
      GM.CurrentSceneSettings.ObjectPickedUpEvent += new FVRSceneSettings.FVRObjectPickedUp(this.AddFVRObjectToTrackedList);
      GM.CurrentPlayerBody.SetHealthThreshold(10000f);
      if (GM.TAHSettings.TAHOption_ItemSpawner == 1)
        this.ItemSpawner.SetActive(true);
      else
        this.ItemSpawner.SetActive(false);
      if (GM.TAHSettings.TAHOption_Music != 1)
        ;
      GM.CurrentSceneSettings.KillEvent += new FVRSceneSettings.BotKill(this.OnBotKill);
      GM.CurrentSceneSettings.ShotFiredEvent += new FVRSceneSettings.ShotFired(this.OnShotFired);
      GM.CurrentSceneSettings.BotShotFiredEvent += new FVRSceneSettings.BotShotFired(this.OnBotShotFired);
    }

    private void OnDisable()
    {
      GM.CurrentSceneSettings.KillEvent -= new FVRSceneSettings.BotKill(this.OnBotKill);
      GM.CurrentSceneSettings.ShotFiredEvent -= new FVRSceneSettings.ShotFired(this.OnShotFired);
      GM.CurrentSceneSettings.BotShotFiredEvent -= new FVRSceneSettings.BotShotFired(this.OnBotShotFired);
    }

    private void OnBotKill(Damage d)
    {
      if (this.State != TAH_Manager.TAHGameState.Taking)
        return;
      this.TakingBotKill();
    }

    private void OnShotFired(FVRFireArm firearm)
    {
      if (this.State != TAH_Manager.TAHGameState.Taking)
        return;
      this.TakingGunShot();
    }

    private void OnBotShotFired(wwBotWurstModernGun gun)
    {
      if (this.State != TAH_Manager.TAHGameState.Taking)
        return;
      this.TakingGunShot();
    }

    public void ItemSpawnerState(bool b) => this.ItemSpawner.SetActive(b);

    public void MusicState(bool b)
    {
    }

    private void GenerateLootTables()
    {
      this.LTChart_CaseWeapon.Clear();
      this.m_generatedStyle = GM.TAHSettings.TAHOption_LootProgression;
      List<FVRObject.OTagFirearmSize> otagFirearmSizeList1 = new List<FVRObject.OTagFirearmSize>()
      {
        FVRObject.OTagFirearmSize.Carbine,
        FVRObject.OTagFirearmSize.Compact,
        FVRObject.OTagFirearmSize.FullSize,
        FVRObject.OTagFirearmSize.Pistol,
        FVRObject.OTagFirearmSize.Pocket
      };
      List<FVRObject.OTagFirearmSize> sizes1 = new List<FVRObject.OTagFirearmSize>()
      {
        FVRObject.OTagFirearmSize.Oversize,
        FVRObject.OTagFirearmSize.Bulky
      };
      List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList1 = new List<FVRObject.OTagFirearmRoundPower>()
      {
        FVRObject.OTagFirearmRoundPower.Tiny,
        FVRObject.OTagFirearmRoundPower.Pistol
      };
      List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList2 = new List<FVRObject.OTagFirearmRoundPower>()
      {
        FVRObject.OTagFirearmRoundPower.Shotgun
      };
      List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList3 = new List<FVRObject.OTagFirearmRoundPower>()
      {
        FVRObject.OTagFirearmRoundPower.Intermediate,
        FVRObject.OTagFirearmRoundPower.FullPower
      };
      List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList4 = new List<FVRObject.OTagFirearmRoundPower>()
      {
        FVRObject.OTagFirearmRoundPower.Ordnance
      };
      if (this.m_generatedStyle == 0)
      {
        List<FVRObject.OTagEra> eras1 = new List<FVRObject.OTagEra>()
        {
          FVRObject.OTagEra.Modern,
          FVRObject.OTagEra.PostWar,
          FVRObject.OTagEra.TurnOfTheCentury,
          FVRObject.OTagEra.WildWest,
          FVRObject.OTagEra.WW1,
          FVRObject.OTagEra.WW2
        };
        LootTable ltFirearmsSideArm = this.LT_Firearms_SideArm;
        LootTable.LootTableType lootTableType1 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList1 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList2 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Pocket,
          FVRObject.OTagFirearmSize.Pistol,
          FVRObject.OTagFirearmSize.Compact
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList5 = firearmRoundPowerList1;
        int num1 = (int) lootTableType1;
        List<FVRObject.OTagEra> eras2 = otagEraList1;
        List<FVRObject.OTagFirearmSize> sizes2 = otagFirearmSizeList2;
        List<FVRObject.OTagFirearmRoundPower> roundPowers1 = firearmRoundPowerList5;
        ltFirearmsSideArm.Initialize((LootTable.LootTableType) num1, eras2, sizes2, roundPowers: roundPowers1, maxCapacity: 20);
        LootTable secondaryWeaponLowCap = this.LT_Firearms_SecondaryWeapon_LowCap;
        LootTable.LootTableType lootTableType2 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList2 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList3 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList6 = firearmRoundPowerList1;
        int num2 = (int) lootTableType2;
        List<FVRObject.OTagEra> eras3 = otagEraList2;
        List<FVRObject.OTagFirearmSize> sizes3 = otagFirearmSizeList3;
        List<FVRObject.OTagFirearmRoundPower> roundPowers2 = firearmRoundPowerList6;
        secondaryWeaponLowCap.Initialize((LootTable.LootTableType) num2, eras3, sizes3, roundPowers: roundPowers2, maxCapacity: 32);
        LootTable secondaryWeaponHighCap = this.LT_Firearms_SecondaryWeapon_HighCap;
        LootTable.LootTableType lootTableType3 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList3 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList4 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList7 = firearmRoundPowerList1;
        int num3 = (int) lootTableType3;
        List<FVRObject.OTagEra> eras4 = otagEraList3;
        List<FVRObject.OTagFirearmSize> sizes4 = otagFirearmSizeList4;
        List<FVRObject.OTagFirearmRoundPower> roundPowers3 = firearmRoundPowerList7;
        secondaryWeaponHighCap.Initialize((LootTable.LootTableType) num3, eras4, sizes4, roundPowers: roundPowers3, minCapacity: 33, maxCapacity: 100);
        LootTable firearmsSniperRifle = this.LT_Firearms_SniperRifle;
        LootTable.LootTableType lootTableType4 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList4 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList5 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize,
          FVRObject.OTagFirearmSize.Bulky
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList8 = firearmRoundPowerList3;
        List<FVRObject.OTagFirearmAction> otagFirearmActionList1 = new List<FVRObject.OTagFirearmAction>()
        {
          FVRObject.OTagFirearmAction.BoltAction
        };
        int num4 = (int) lootTableType4;
        List<FVRObject.OTagEra> eras5 = otagEraList4;
        List<FVRObject.OTagFirearmSize> sizes5 = otagFirearmSizeList5;
        List<FVRObject.OTagFirearmAction> actions1 = otagFirearmActionList1;
        List<FVRObject.OTagFirearmRoundPower> roundPowers4 = firearmRoundPowerList8;
        firearmsSniperRifle.Initialize((LootTable.LootTableType) num4, eras5, sizes5, actions1, roundPowers: roundPowers4);
        LootTable primaryWeaponLowCap = this.LT_Firearms_PrimaryWeapon_LowCap;
        LootTable.LootTableType lootTableType5 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList5 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList6 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmAction> otagFirearmActionList2 = new List<FVRObject.OTagFirearmAction>()
        {
          FVRObject.OTagFirearmAction.Automatic
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList9 = firearmRoundPowerList3;
        int num5 = (int) lootTableType5;
        List<FVRObject.OTagEra> eras6 = otagEraList5;
        List<FVRObject.OTagFirearmSize> sizes6 = otagFirearmSizeList6;
        List<FVRObject.OTagFirearmAction> actions2 = otagFirearmActionList2;
        List<FVRObject.OTagFirearmRoundPower> roundPowers5 = firearmRoundPowerList9;
        primaryWeaponLowCap.Initialize((LootTable.LootTableType) num5, eras6, sizes6, actions2, roundPowers: roundPowers5, maxCapacity: 16);
        LootTable primaryWeaponMedCap = this.LT_Firearms_PrimaryWeapon_MedCap;
        LootTable.LootTableType lootTableType6 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList6 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList7 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmAction> otagFirearmActionList3 = new List<FVRObject.OTagFirearmAction>()
        {
          FVRObject.OTagFirearmAction.Automatic
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList10 = firearmRoundPowerList3;
        int num6 = (int) lootTableType6;
        List<FVRObject.OTagEra> eras7 = otagEraList6;
        List<FVRObject.OTagFirearmSize> sizes7 = otagFirearmSizeList7;
        List<FVRObject.OTagFirearmAction> actions3 = otagFirearmActionList3;
        List<FVRObject.OTagFirearmRoundPower> roundPowers6 = firearmRoundPowerList10;
        primaryWeaponMedCap.Initialize((LootTable.LootTableType) num6, eras7, sizes7, actions3, roundPowers: roundPowers6, minCapacity: 17, maxCapacity: 44);
        LootTable primaryWeaponHighCap = this.LT_Firearms_PrimaryWeapon_HighCap;
        LootTable.LootTableType lootTableType7 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList7 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList8 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmAction> otagFirearmActionList4 = new List<FVRObject.OTagFirearmAction>()
        {
          FVRObject.OTagFirearmAction.Automatic
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList11 = firearmRoundPowerList3;
        int num7 = (int) lootTableType7;
        List<FVRObject.OTagEra> eras8 = otagEraList7;
        List<FVRObject.OTagFirearmSize> sizes8 = otagFirearmSizeList8;
        List<FVRObject.OTagFirearmAction> actions4 = otagFirearmActionList4;
        List<FVRObject.OTagFirearmRoundPower> roundPowers7 = firearmRoundPowerList11;
        primaryWeaponHighCap.Initialize((LootTable.LootTableType) num7, eras8, sizes8, actions4, roundPowers: roundPowers7, minCapacity: 45, maxCapacity: 100);
        LootTable firearmsShotgunLowCap = this.LT_Firearms_Shotgun_LowCap;
        LootTable.LootTableType lootTableType8 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList8 = eras1;
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList12 = firearmRoundPowerList2;
        int num8 = (int) lootTableType8;
        List<FVRObject.OTagEra> eras9 = otagEraList8;
        List<FVRObject.OTagFirearmRoundPower> roundPowers8 = firearmRoundPowerList12;
        firearmsShotgunLowCap.Initialize((LootTable.LootTableType) num8, eras9, roundPowers: roundPowers8, maxCapacity: 8);
        LootTable firearmsShotgunHighCap = this.LT_Firearms_Shotgun_HighCap;
        LootTable.LootTableType lootTableType9 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList9 = eras1;
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList13 = firearmRoundPowerList2;
        int num9 = (int) lootTableType9;
        List<FVRObject.OTagEra> eras10 = otagEraList9;
        List<FVRObject.OTagFirearmRoundPower> roundPowers9 = firearmRoundPowerList13;
        firearmsShotgunHighCap.Initialize((LootTable.LootTableType) num9, eras10, roundPowers: roundPowers9, minCapacity: 9);
        LootTable firearmsSmallOrdnance = this.LT_Firearms_SmallOrdnance;
        LootTable.LootTableType lootTableType10 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList10 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList9 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList14 = firearmRoundPowerList4;
        int num10 = (int) lootTableType10;
        List<FVRObject.OTagEra> eras11 = otagEraList10;
        List<FVRObject.OTagFirearmSize> sizes9 = otagFirearmSizeList9;
        List<FVRObject.OTagFirearmRoundPower> roundPowers10 = firearmRoundPowerList14;
        firearmsSmallOrdnance.Initialize((LootTable.LootTableType) num10, eras11, sizes9, roundPowers: roundPowers10);
        this.LT_FirearmsSpecial.Initialize(LootTable.LootTableType.Firearm, eras1, sizes1);
        LootTable ltGrenades = this.LT_Grenades;
        LootTable.LootTableType lootTableType11 = LootTable.LootTableType.Thrown;
        List<FVRObject.OTagEra> otagEraList11 = eras1;
        List<FVRObject.OTagThrownType> otagThrownTypeList = new List<FVRObject.OTagThrownType>()
        {
          FVRObject.OTagThrownType.Pinned
        };
        int num11 = (int) lootTableType11;
        List<FVRObject.OTagEra> eras12 = otagEraList11;
        List<FVRObject.OTagThrownType> thrownTypes = otagThrownTypeList;
        ltGrenades.Initialize((LootTable.LootTableType) num11, eras12, thrownTypes: thrownTypes);
        LootTable commonAttachments = this.LT_CommonAttachments;
        LootTable.LootTableType lootTableType12 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList12 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList1 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.BarrelExtension,
          FVRObject.OTagAttachmentFeature.Grip,
          FVRObject.OTagAttachmentFeature.IronSight,
          FVRObject.OTagAttachmentFeature.Reflex
        };
        int num12 = (int) lootTableType12;
        List<FVRObject.OTagEra> eras13 = otagEraList12;
        List<FVRObject.OTagAttachmentFeature> features1 = attachmentFeatureList1;
        commonAttachments.Initialize((LootTable.LootTableType) num12, eras13, features: features1);
        LootTable ltRareAttachments = this.LT_RareAttachments;
        LootTable.LootTableType lootTableType13 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList13 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList2 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.Laser,
          FVRObject.OTagAttachmentFeature.Magnification,
          FVRObject.OTagAttachmentFeature.Suppression,
          FVRObject.OTagAttachmentFeature.Stock
        };
        int num13 = (int) lootTableType13;
        List<FVRObject.OTagEra> eras14 = otagEraList13;
        List<FVRObject.OTagAttachmentFeature> features2 = attachmentFeatureList2;
        ltRareAttachments.Initialize((LootTable.LootTableType) num13, eras14, features: features2);
        LootTable ltMeleeWeapons = this.LT_MeleeWeapons;
        LootTable.LootTableType lootTableType14 = LootTable.LootTableType.Melee;
        List<FVRObject.OTagEra> otagEraList14 = eras1;
        List<FVRObject.OTagMeleeStyle> otagMeleeStyleList = new List<FVRObject.OTagMeleeStyle>()
        {
          FVRObject.OTagMeleeStyle.Tactical
        };
        List<FVRObject.OTagMeleeHandedness> otagMeleeHandednessList = new List<FVRObject.OTagMeleeHandedness>()
        {
          FVRObject.OTagMeleeHandedness.OneHanded
        };
        int num14 = (int) lootTableType14;
        List<FVRObject.OTagEra> eras15 = otagEraList14;
        List<FVRObject.OTagMeleeStyle> meleeStyles = otagMeleeStyleList;
        List<FVRObject.OTagMeleeHandedness> meleeHandedness = otagMeleeHandednessList;
        ltMeleeWeapons.Initialize((LootTable.LootTableType) num14, eras15, meleeStyles: meleeStyles, meleeHandedness: meleeHandedness);
        LootTable requiredAttachments = this.LT_RequiredAttachments;
        LootTable.LootTableType lootTableType15 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList15 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList3 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.IronSight,
          FVRObject.OTagAttachmentFeature.Reflex
        };
        List<FVRObject.OTagFirearmMount> otagFirearmMountList = new List<FVRObject.OTagFirearmMount>()
        {
          FVRObject.OTagFirearmMount.Picatinny
        };
        int num15 = (int) lootTableType15;
        List<FVRObject.OTagEra> eras16 = otagEraList15;
        List<FVRObject.OTagFirearmMount> mounts = otagFirearmMountList;
        List<FVRObject.OTagAttachmentFeature> features3 = attachmentFeatureList3;
        requiredAttachments.Initialize((LootTable.LootTableType) num15, eras16, mounts: mounts, features: features3);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SideArm);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_Shotgun_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SecondaryWeapon_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SniperRifle);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_PrimaryWeapon_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SmallOrdnance);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_PrimaryWeapon_MedCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SecondaryWeapon_HighCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_Shotgun_HighCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_PrimaryWeapon_HighCap);
        this.m_ltStartMin = 0;
        this.m_ltStartMax = 2;
        this.m_ltDiffModMin = -2;
        this.m_ltDiffModMax = 2;
        this.m_ltDiffMinCap = 5;
        this.m_maxWaveType = 18;
        this.m_waveDifficultyMod = 0;
        this.m_botHealthMultiplier = 1f;
        this.m_lootThrownThreshold = 0.75f;
        this.m_lootRareAttachThreshold = 0.65f;
        this.m_lootCommonAttachThreshold = 0.4f;
      }
      else if (this.m_generatedStyle == 1)
      {
        List<FVRObject.OTagEra> eras1 = new List<FVRObject.OTagEra>()
        {
          FVRObject.OTagEra.TurnOfTheCentury,
          FVRObject.OTagEra.WW1,
          FVRObject.OTagEra.WW2
        };
        LootTable ltFirearmsSideArm = this.LT_Firearms_SideArm;
        LootTable.LootTableType lootTableType1 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList1 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList2 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Pocket,
          FVRObject.OTagFirearmSize.Pistol
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList5 = firearmRoundPowerList1;
        int num1 = (int) lootTableType1;
        List<FVRObject.OTagEra> eras2 = otagEraList1;
        List<FVRObject.OTagFirearmSize> sizes2 = otagFirearmSizeList2;
        List<FVRObject.OTagFirearmRoundPower> roundPowers1 = firearmRoundPowerList5;
        ltFirearmsSideArm.Initialize((LootTable.LootTableType) num1, eras2, sizes2, roundPowers: roundPowers1, maxCapacity: 20);
        LootTable secondaryWeaponLowCap = this.LT_Firearms_SecondaryWeapon_LowCap;
        LootTable.LootTableType lootTableType2 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList2 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList3 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList6 = firearmRoundPowerList1;
        int num2 = (int) lootTableType2;
        List<FVRObject.OTagEra> eras3 = otagEraList2;
        List<FVRObject.OTagFirearmSize> sizes3 = otagFirearmSizeList3;
        List<FVRObject.OTagFirearmRoundPower> roundPowers2 = firearmRoundPowerList6;
        secondaryWeaponLowCap.Initialize((LootTable.LootTableType) num2, eras3, sizes3, roundPowers: roundPowers2, maxCapacity: 20);
        LootTable secondaryWeaponHighCap = this.LT_Firearms_SecondaryWeapon_HighCap;
        LootTable.LootTableType lootTableType3 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList3 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList4 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList7 = firearmRoundPowerList1;
        int num3 = (int) lootTableType3;
        List<FVRObject.OTagEra> eras4 = otagEraList3;
        List<FVRObject.OTagFirearmSize> sizes4 = otagFirearmSizeList4;
        List<FVRObject.OTagFirearmRoundPower> roundPowers3 = firearmRoundPowerList7;
        secondaryWeaponHighCap.Initialize((LootTable.LootTableType) num3, eras4, sizes4, roundPowers: roundPowers3, minCapacity: 21, maxCapacity: 100);
        LootTable firearmsSniperRifle = this.LT_Firearms_SniperRifle;
        LootTable.LootTableType lootTableType4 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList4 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList5 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize,
          FVRObject.OTagFirearmSize.Bulky
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList8 = firearmRoundPowerList3;
        List<FVRObject.OTagFirearmAction> otagFirearmActionList1 = new List<FVRObject.OTagFirearmAction>()
        {
          FVRObject.OTagFirearmAction.BoltAction
        };
        int num4 = (int) lootTableType4;
        List<FVRObject.OTagEra> eras5 = otagEraList4;
        List<FVRObject.OTagFirearmSize> sizes5 = otagFirearmSizeList5;
        List<FVRObject.OTagFirearmAction> actions1 = otagFirearmActionList1;
        List<FVRObject.OTagFirearmRoundPower> roundPowers4 = firearmRoundPowerList8;
        firearmsSniperRifle.Initialize((LootTable.LootTableType) num4, eras5, sizes5, actions1, roundPowers: roundPowers4);
        LootTable primaryWeaponLowCap = this.LT_Firearms_PrimaryWeapon_LowCap;
        LootTable.LootTableType lootTableType5 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList5 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList6 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmAction> otagFirearmActionList2 = new List<FVRObject.OTagFirearmAction>()
        {
          FVRObject.OTagFirearmAction.Automatic
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList9 = firearmRoundPowerList3;
        int num5 = (int) lootTableType5;
        List<FVRObject.OTagEra> eras6 = otagEraList5;
        List<FVRObject.OTagFirearmSize> sizes6 = otagFirearmSizeList6;
        List<FVRObject.OTagFirearmAction> actions2 = otagFirearmActionList2;
        List<FVRObject.OTagFirearmRoundPower> roundPowers5 = firearmRoundPowerList9;
        primaryWeaponLowCap.Initialize((LootTable.LootTableType) num5, eras6, sizes6, actions2, roundPowers: roundPowers5, maxCapacity: 15);
        LootTable primaryWeaponMedCap = this.LT_Firearms_PrimaryWeapon_MedCap;
        LootTable.LootTableType lootTableType6 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList6 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList7 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmAction> otagFirearmActionList3 = new List<FVRObject.OTagFirearmAction>()
        {
          FVRObject.OTagFirearmAction.Automatic
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList10 = firearmRoundPowerList3;
        int num6 = (int) lootTableType6;
        List<FVRObject.OTagEra> eras7 = otagEraList6;
        List<FVRObject.OTagFirearmSize> sizes7 = otagFirearmSizeList7;
        List<FVRObject.OTagFirearmAction> actions3 = otagFirearmActionList3;
        List<FVRObject.OTagFirearmRoundPower> roundPowers6 = firearmRoundPowerList10;
        primaryWeaponMedCap.Initialize((LootTable.LootTableType) num6, eras7, sizes7, actions3, roundPowers: roundPowers6, minCapacity: 16, maxCapacity: 100);
        LootTable firearmsSmallOrdnance = this.LT_Firearms_SmallOrdnance;
        LootTable.LootTableType lootTableType7 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList7 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList8 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList11 = firearmRoundPowerList4;
        int num7 = (int) lootTableType7;
        List<FVRObject.OTagEra> eras8 = otagEraList7;
        List<FVRObject.OTagFirearmSize> sizes8 = otagFirearmSizeList8;
        List<FVRObject.OTagFirearmRoundPower> roundPowers7 = firearmRoundPowerList11;
        firearmsSmallOrdnance.Initialize((LootTable.LootTableType) num7, eras8, sizes8, roundPowers: roundPowers7);
        LootTable firearmsShotgunLowCap = this.LT_Firearms_Shotgun_LowCap;
        LootTable.LootTableType lootTableType8 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList8 = eras1;
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList12 = firearmRoundPowerList2;
        int num8 = (int) lootTableType8;
        List<FVRObject.OTagEra> eras9 = otagEraList8;
        List<FVRObject.OTagFirearmRoundPower> roundPowers8 = firearmRoundPowerList12;
        firearmsShotgunLowCap.Initialize((LootTable.LootTableType) num8, eras9, roundPowers: roundPowers8);
        this.LT_FirearmsSpecial.Initialize(LootTable.LootTableType.Firearm, eras1, sizes1);
        LootTable ltGrenades = this.LT_Grenades;
        LootTable.LootTableType lootTableType9 = LootTable.LootTableType.Thrown;
        List<FVRObject.OTagEra> otagEraList9 = eras1;
        List<FVRObject.OTagThrownType> otagThrownTypeList = new List<FVRObject.OTagThrownType>()
        {
          FVRObject.OTagThrownType.Pinned
        };
        int num9 = (int) lootTableType9;
        List<FVRObject.OTagEra> eras10 = otagEraList9;
        List<FVRObject.OTagThrownType> thrownTypes = otagThrownTypeList;
        ltGrenades.Initialize((LootTable.LootTableType) num9, eras10, thrownTypes: thrownTypes);
        LootTable commonAttachments = this.LT_CommonAttachments;
        LootTable.LootTableType lootTableType10 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList10 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList1 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.Adapter,
          FVRObject.OTagAttachmentFeature.BarrelExtension,
          FVRObject.OTagAttachmentFeature.Grip,
          FVRObject.OTagAttachmentFeature.IronSight,
          FVRObject.OTagAttachmentFeature.Reflex,
          FVRObject.OTagAttachmentFeature.Stock
        };
        int num10 = (int) lootTableType10;
        List<FVRObject.OTagEra> eras11 = otagEraList10;
        List<FVRObject.OTagAttachmentFeature> features1 = attachmentFeatureList1;
        commonAttachments.Initialize((LootTable.LootTableType) num10, eras11, features: features1);
        LootTable ltRareAttachments = this.LT_RareAttachments;
        LootTable.LootTableType lootTableType11 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList11 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList2 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.Laser,
          FVRObject.OTagAttachmentFeature.Magnification,
          FVRObject.OTagAttachmentFeature.Suppression
        };
        int num11 = (int) lootTableType11;
        List<FVRObject.OTagEra> eras12 = otagEraList11;
        List<FVRObject.OTagAttachmentFeature> features2 = attachmentFeatureList2;
        ltRareAttachments.Initialize((LootTable.LootTableType) num11, eras12, features: features2);
        LootTable ltMeleeWeapons = this.LT_MeleeWeapons;
        LootTable.LootTableType lootTableType12 = LootTable.LootTableType.Melee;
        List<FVRObject.OTagEra> otagEraList12 = eras1;
        List<FVRObject.OTagMeleeStyle> otagMeleeStyleList = new List<FVRObject.OTagMeleeStyle>()
        {
          FVRObject.OTagMeleeStyle.Improvised,
          FVRObject.OTagMeleeStyle.Tactical,
          FVRObject.OTagMeleeStyle.Tool
        };
        List<FVRObject.OTagMeleeHandedness> otagMeleeHandednessList = new List<FVRObject.OTagMeleeHandedness>()
        {
          FVRObject.OTagMeleeHandedness.OneHanded
        };
        int num12 = (int) lootTableType12;
        List<FVRObject.OTagEra> eras13 = otagEraList12;
        List<FVRObject.OTagMeleeStyle> meleeStyles = otagMeleeStyleList;
        List<FVRObject.OTagMeleeHandedness> meleeHandedness = otagMeleeHandednessList;
        ltMeleeWeapons.Initialize((LootTable.LootTableType) num12, eras13, meleeStyles: meleeStyles, meleeHandedness: meleeHandedness);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SideArm);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_Shotgun_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SecondaryWeapon_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SniperRifle);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_PrimaryWeapon_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SecondaryWeapon_HighCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SmallOrdnance);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_PrimaryWeapon_MedCap);
        this.m_ltStartMin = 0;
        this.m_ltStartMax = 2;
        this.m_ltDiffModMin = -2;
        this.m_ltDiffModMax = 2;
        this.m_ltDiffMinCap = 5;
        this.m_maxWaveType = 12;
        this.m_waveDifficultyMod = -1;
        this.m_botHealthMultiplier = 0.8f;
        this.m_lootThrownThreshold = 0.65f;
        this.m_lootRareAttachThreshold = 1.65f;
        this.m_lootCommonAttachThreshold = 1.4f;
      }
      else if (this.m_generatedStyle == 2)
      {
        List<FVRObject.OTagEra> eras1 = new List<FVRObject.OTagEra>()
        {
          FVRObject.OTagEra.Colonial,
          FVRObject.OTagEra.TurnOfTheCentury,
          FVRObject.OTagEra.WildWest
        };
        LootTable ltFirearmsSideArm = this.LT_Firearms_SideArm;
        LootTable.LootTableType lootTableType1 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList1 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList2 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Pocket,
          FVRObject.OTagFirearmSize.Pistol
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList5 = firearmRoundPowerList1;
        int num1 = (int) lootTableType1;
        List<FVRObject.OTagEra> eras2 = otagEraList1;
        List<FVRObject.OTagFirearmSize> sizes2 = otagFirearmSizeList2;
        List<FVRObject.OTagFirearmRoundPower> roundPowers1 = firearmRoundPowerList5;
        ltFirearmsSideArm.Initialize((LootTable.LootTableType) num1, eras2, sizes2, roundPowers: roundPowers1, maxCapacity: 20);
        LootTable secondaryWeaponLowCap = this.LT_Firearms_SecondaryWeapon_LowCap;
        LootTable.LootTableType lootTableType2 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList2 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList3 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList6 = firearmRoundPowerList1;
        int num2 = (int) lootTableType2;
        List<FVRObject.OTagEra> eras3 = otagEraList2;
        List<FVRObject.OTagFirearmSize> sizes3 = otagFirearmSizeList3;
        List<FVRObject.OTagFirearmRoundPower> roundPowers2 = firearmRoundPowerList6;
        secondaryWeaponLowCap.Initialize((LootTable.LootTableType) num2, eras3, sizes3, roundPowers: roundPowers2, maxCapacity: 30);
        LootTable primaryWeaponLowCap = this.LT_Firearms_PrimaryWeapon_LowCap;
        LootTable.LootTableType lootTableType3 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList3 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList4 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize,
          FVRObject.OTagFirearmSize.Bulky
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList7 = firearmRoundPowerList3;
        int num3 = (int) lootTableType3;
        List<FVRObject.OTagEra> eras4 = otagEraList3;
        List<FVRObject.OTagFirearmSize> sizes4 = otagFirearmSizeList4;
        List<FVRObject.OTagFirearmRoundPower> roundPowers3 = firearmRoundPowerList7;
        primaryWeaponLowCap.Initialize((LootTable.LootTableType) num3, eras4, sizes4, roundPowers: roundPowers3, maxCapacity: 30);
        LootTable firearmsShotgunLowCap = this.LT_Firearms_Shotgun_LowCap;
        LootTable.LootTableType lootTableType4 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList4 = eras1;
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList8 = firearmRoundPowerList2;
        int num4 = (int) lootTableType4;
        List<FVRObject.OTagEra> eras5 = otagEraList4;
        List<FVRObject.OTagFirearmRoundPower> roundPowers4 = firearmRoundPowerList8;
        firearmsShotgunLowCap.Initialize((LootTable.LootTableType) num4, eras5, roundPowers: roundPowers4, maxCapacity: 2);
        LootTable firearmsShotgunHighCap = this.LT_Firearms_Shotgun_HighCap;
        LootTable.LootTableType lootTableType5 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList5 = eras1;
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList9 = firearmRoundPowerList2;
        int num5 = (int) lootTableType5;
        List<FVRObject.OTagEra> eras6 = otagEraList5;
        List<FVRObject.OTagFirearmRoundPower> roundPowers5 = firearmRoundPowerList9;
        firearmsShotgunHighCap.Initialize((LootTable.LootTableType) num5, eras6, roundPowers: roundPowers5, minCapacity: 3);
        this.LT_FirearmsSpecial.Initialize(LootTable.LootTableType.Firearm, eras1, sizes1);
        LootTable ltGrenades = this.LT_Grenades;
        LootTable.LootTableType lootTableType6 = LootTable.LootTableType.Thrown;
        List<FVRObject.OTagThrownType> otagThrownTypeList = new List<FVRObject.OTagThrownType>()
        {
          FVRObject.OTagThrownType.ManualFuse
        };
        int num6 = (int) lootTableType6;
        List<FVRObject.OTagThrownType> thrownTypes = otagThrownTypeList;
        ltGrenades.Initialize((LootTable.LootTableType) num6, thrownTypes: thrownTypes);
        LootTable commonAttachments = this.LT_CommonAttachments;
        LootTable.LootTableType lootTableType7 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList6 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList1 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.Stock
        };
        int num7 = (int) lootTableType7;
        List<FVRObject.OTagEra> eras7 = otagEraList6;
        List<FVRObject.OTagAttachmentFeature> features1 = attachmentFeatureList1;
        commonAttachments.Initialize((LootTable.LootTableType) num7, eras7, features: features1);
        LootTable ltRareAttachments = this.LT_RareAttachments;
        LootTable.LootTableType lootTableType8 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList7 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList2 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.Stock
        };
        int num8 = (int) lootTableType8;
        List<FVRObject.OTagEra> eras8 = otagEraList7;
        List<FVRObject.OTagAttachmentFeature> features2 = attachmentFeatureList2;
        ltRareAttachments.Initialize((LootTable.LootTableType) num8, eras8, features: features2);
        LootTable ltMeleeWeapons = this.LT_MeleeWeapons;
        LootTable.LootTableType lootTableType9 = LootTable.LootTableType.Melee;
        List<FVRObject.OTagEra> otagEraList8 = eras1;
        List<FVRObject.OTagMeleeStyle> otagMeleeStyleList = new List<FVRObject.OTagMeleeStyle>()
        {
          FVRObject.OTagMeleeStyle.Tool
        };
        List<FVRObject.OTagMeleeHandedness> otagMeleeHandednessList = new List<FVRObject.OTagMeleeHandedness>()
        {
          FVRObject.OTagMeleeHandedness.OneHanded
        };
        int num9 = (int) lootTableType9;
        List<FVRObject.OTagEra> eras9 = otagEraList8;
        List<FVRObject.OTagMeleeStyle> meleeStyles = otagMeleeStyleList;
        List<FVRObject.OTagMeleeHandedness> meleeHandedness = otagMeleeHandednessList;
        ltMeleeWeapons.Initialize((LootTable.LootTableType) num9, eras9, meleeStyles: meleeStyles, meleeHandedness: meleeHandedness);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_Shotgun_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SideArm);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SecondaryWeapon_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_PrimaryWeapon_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_Shotgun_HighCap);
        this.m_ltStartMin = 0;
        this.m_ltStartMax = 2;
        this.m_ltDiffModMin = -2;
        this.m_ltDiffModMax = 2;
        this.m_ltDiffMinCap = 1;
        this.m_maxWaveType = 8;
        this.m_waveDifficultyMod = -2;
        this.m_botHealthMultiplier = 0.4f;
        this.m_lootThrownThreshold = 0.65f;
        this.m_lootRareAttachThreshold = 1.65f;
        this.m_lootCommonAttachThreshold = 1.4f;
      }
      else if (this.m_generatedStyle == 3)
      {
        List<FVRObject.OTagEra> eras1 = new List<FVRObject.OTagEra>()
        {
          FVRObject.OTagEra.Modern,
          FVRObject.OTagEra.PostWar
        };
        LootTable ltFirearmsSideArm = this.LT_Firearms_SideArm;
        LootTable.LootTableType lootTableType1 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList1 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList2 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Pocket,
          FVRObject.OTagFirearmSize.Pistol,
          FVRObject.OTagFirearmSize.Compact
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList5 = firearmRoundPowerList1;
        int num1 = (int) lootTableType1;
        List<FVRObject.OTagEra> eras2 = otagEraList1;
        List<FVRObject.OTagFirearmSize> sizes2 = otagFirearmSizeList2;
        List<FVRObject.OTagFirearmRoundPower> roundPowers1 = firearmRoundPowerList5;
        ltFirearmsSideArm.Initialize((LootTable.LootTableType) num1, eras2, sizes2, roundPowers: roundPowers1, maxCapacity: 20);
        LootTable secondaryWeaponLowCap = this.LT_Firearms_SecondaryWeapon_LowCap;
        LootTable.LootTableType lootTableType2 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList2 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList3 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList6 = firearmRoundPowerList1;
        int num2 = (int) lootTableType2;
        List<FVRObject.OTagEra> eras3 = otagEraList2;
        List<FVRObject.OTagFirearmSize> sizes3 = otagFirearmSizeList3;
        List<FVRObject.OTagFirearmRoundPower> roundPowers2 = firearmRoundPowerList6;
        secondaryWeaponLowCap.Initialize((LootTable.LootTableType) num2, eras3, sizes3, roundPowers: roundPowers2, maxCapacity: 32);
        LootTable secondaryWeaponHighCap = this.LT_Firearms_SecondaryWeapon_HighCap;
        LootTable.LootTableType lootTableType3 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList3 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList4 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList7 = firearmRoundPowerList1;
        int num3 = (int) lootTableType3;
        List<FVRObject.OTagEra> eras4 = otagEraList3;
        List<FVRObject.OTagFirearmSize> sizes4 = otagFirearmSizeList4;
        List<FVRObject.OTagFirearmRoundPower> roundPowers3 = firearmRoundPowerList7;
        secondaryWeaponHighCap.Initialize((LootTable.LootTableType) num3, eras4, sizes4, roundPowers: roundPowers3, minCapacity: 33, maxCapacity: 100);
        LootTable firearmsSniperRifle = this.LT_Firearms_SniperRifle;
        LootTable.LootTableType lootTableType4 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList4 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList5 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize,
          FVRObject.OTagFirearmSize.Bulky
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList8 = firearmRoundPowerList3;
        List<FVRObject.OTagFirearmAction> otagFirearmActionList1 = new List<FVRObject.OTagFirearmAction>()
        {
          FVRObject.OTagFirearmAction.BoltAction
        };
        int num4 = (int) lootTableType4;
        List<FVRObject.OTagEra> eras5 = otagEraList4;
        List<FVRObject.OTagFirearmSize> sizes5 = otagFirearmSizeList5;
        List<FVRObject.OTagFirearmAction> actions1 = otagFirearmActionList1;
        List<FVRObject.OTagFirearmRoundPower> roundPowers4 = firearmRoundPowerList8;
        firearmsSniperRifle.Initialize((LootTable.LootTableType) num4, eras5, sizes5, actions1, roundPowers: roundPowers4);
        LootTable primaryWeaponLowCap = this.LT_Firearms_PrimaryWeapon_LowCap;
        LootTable.LootTableType lootTableType5 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList5 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList6 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmAction> otagFirearmActionList2 = new List<FVRObject.OTagFirearmAction>()
        {
          FVRObject.OTagFirearmAction.Automatic
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList9 = firearmRoundPowerList3;
        int num5 = (int) lootTableType5;
        List<FVRObject.OTagEra> eras6 = otagEraList5;
        List<FVRObject.OTagFirearmSize> sizes6 = otagFirearmSizeList6;
        List<FVRObject.OTagFirearmAction> actions2 = otagFirearmActionList2;
        List<FVRObject.OTagFirearmRoundPower> roundPowers5 = firearmRoundPowerList9;
        primaryWeaponLowCap.Initialize((LootTable.LootTableType) num5, eras6, sizes6, actions2, roundPowers: roundPowers5, maxCapacity: 16);
        LootTable primaryWeaponMedCap = this.LT_Firearms_PrimaryWeapon_MedCap;
        LootTable.LootTableType lootTableType6 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList6 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList7 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmAction> otagFirearmActionList3 = new List<FVRObject.OTagFirearmAction>()
        {
          FVRObject.OTagFirearmAction.Automatic
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList10 = firearmRoundPowerList3;
        int num6 = (int) lootTableType6;
        List<FVRObject.OTagEra> eras7 = otagEraList6;
        List<FVRObject.OTagFirearmSize> sizes7 = otagFirearmSizeList7;
        List<FVRObject.OTagFirearmAction> actions3 = otagFirearmActionList3;
        List<FVRObject.OTagFirearmRoundPower> roundPowers6 = firearmRoundPowerList10;
        primaryWeaponMedCap.Initialize((LootTable.LootTableType) num6, eras7, sizes7, actions3, roundPowers: roundPowers6, minCapacity: 17, maxCapacity: 44);
        LootTable primaryWeaponHighCap = this.LT_Firearms_PrimaryWeapon_HighCap;
        LootTable.LootTableType lootTableType7 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList7 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList8 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmAction> otagFirearmActionList4 = new List<FVRObject.OTagFirearmAction>()
        {
          FVRObject.OTagFirearmAction.Automatic
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList11 = firearmRoundPowerList3;
        int num7 = (int) lootTableType7;
        List<FVRObject.OTagEra> eras8 = otagEraList7;
        List<FVRObject.OTagFirearmSize> sizes8 = otagFirearmSizeList8;
        List<FVRObject.OTagFirearmAction> actions4 = otagFirearmActionList4;
        List<FVRObject.OTagFirearmRoundPower> roundPowers7 = firearmRoundPowerList11;
        primaryWeaponHighCap.Initialize((LootTable.LootTableType) num7, eras8, sizes8, actions4, roundPowers: roundPowers7, minCapacity: 45, maxCapacity: 100);
        LootTable firearmsShotgunLowCap = this.LT_Firearms_Shotgun_LowCap;
        LootTable.LootTableType lootTableType8 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList8 = eras1;
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList12 = firearmRoundPowerList2;
        int num8 = (int) lootTableType8;
        List<FVRObject.OTagEra> eras9 = otagEraList8;
        List<FVRObject.OTagFirearmRoundPower> roundPowers8 = firearmRoundPowerList12;
        firearmsShotgunLowCap.Initialize((LootTable.LootTableType) num8, eras9, roundPowers: roundPowers8, maxCapacity: 8);
        LootTable firearmsShotgunHighCap = this.LT_Firearms_Shotgun_HighCap;
        LootTable.LootTableType lootTableType9 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList9 = eras1;
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList13 = firearmRoundPowerList2;
        int num9 = (int) lootTableType9;
        List<FVRObject.OTagEra> eras10 = otagEraList9;
        List<FVRObject.OTagFirearmRoundPower> roundPowers9 = firearmRoundPowerList13;
        firearmsShotgunHighCap.Initialize((LootTable.LootTableType) num9, eras10, roundPowers: roundPowers9, minCapacity: 9);
        LootTable firearmsSmallOrdnance = this.LT_Firearms_SmallOrdnance;
        LootTable.LootTableType lootTableType10 = LootTable.LootTableType.Firearm;
        List<FVRObject.OTagEra> otagEraList10 = eras1;
        List<FVRObject.OTagFirearmSize> otagFirearmSizeList9 = new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize
        };
        List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList14 = firearmRoundPowerList4;
        int num10 = (int) lootTableType10;
        List<FVRObject.OTagEra> eras11 = otagEraList10;
        List<FVRObject.OTagFirearmSize> sizes9 = otagFirearmSizeList9;
        List<FVRObject.OTagFirearmRoundPower> roundPowers10 = firearmRoundPowerList14;
        firearmsSmallOrdnance.Initialize((LootTable.LootTableType) num10, eras11, sizes9, roundPowers: roundPowers10);
        this.LT_FirearmsSpecial.Initialize(LootTable.LootTableType.Firearm, eras1, sizes1);
        LootTable ltGrenades = this.LT_Grenades;
        LootTable.LootTableType lootTableType11 = LootTable.LootTableType.Thrown;
        List<FVRObject.OTagThrownType> otagThrownTypeList = new List<FVRObject.OTagThrownType>()
        {
          FVRObject.OTagThrownType.Pinned
        };
        int num11 = (int) lootTableType11;
        List<FVRObject.OTagThrownType> thrownTypes = otagThrownTypeList;
        ltGrenades.Initialize((LootTable.LootTableType) num11, thrownTypes: thrownTypes);
        LootTable commonAttachments = this.LT_CommonAttachments;
        LootTable.LootTableType lootTableType12 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList11 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList1 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.BarrelExtension,
          FVRObject.OTagAttachmentFeature.Grip,
          FVRObject.OTagAttachmentFeature.IronSight,
          FVRObject.OTagAttachmentFeature.Reflex
        };
        int num12 = (int) lootTableType12;
        List<FVRObject.OTagEra> eras12 = otagEraList11;
        List<FVRObject.OTagAttachmentFeature> features1 = attachmentFeatureList1;
        commonAttachments.Initialize((LootTable.LootTableType) num12, eras12, features: features1);
        LootTable ltRareAttachments = this.LT_RareAttachments;
        LootTable.LootTableType lootTableType13 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList12 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList2 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.Laser,
          FVRObject.OTagAttachmentFeature.Magnification,
          FVRObject.OTagAttachmentFeature.Suppression,
          FVRObject.OTagAttachmentFeature.Stock
        };
        int num13 = (int) lootTableType13;
        List<FVRObject.OTagEra> eras13 = otagEraList12;
        List<FVRObject.OTagAttachmentFeature> features2 = attachmentFeatureList2;
        ltRareAttachments.Initialize((LootTable.LootTableType) num13, eras13, features: features2);
        LootTable ltMeleeWeapons = this.LT_MeleeWeapons;
        LootTable.LootTableType lootTableType14 = LootTable.LootTableType.Melee;
        List<FVRObject.OTagEra> otagEraList13 = eras1;
        List<FVRObject.OTagMeleeStyle> otagMeleeStyleList = new List<FVRObject.OTagMeleeStyle>()
        {
          FVRObject.OTagMeleeStyle.Tactical
        };
        List<FVRObject.OTagMeleeHandedness> otagMeleeHandednessList = new List<FVRObject.OTagMeleeHandedness>()
        {
          FVRObject.OTagMeleeHandedness.OneHanded
        };
        int num14 = (int) lootTableType14;
        List<FVRObject.OTagEra> eras14 = otagEraList13;
        List<FVRObject.OTagMeleeStyle> meleeStyles = otagMeleeStyleList;
        List<FVRObject.OTagMeleeHandedness> meleeHandedness = otagMeleeHandednessList;
        ltMeleeWeapons.Initialize((LootTable.LootTableType) num14, eras14, meleeStyles: meleeStyles, meleeHandedness: meleeHandedness);
        LootTable requiredAttachments = this.LT_RequiredAttachments;
        LootTable.LootTableType lootTableType15 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList14 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList3 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.IronSight,
          FVRObject.OTagAttachmentFeature.Reflex
        };
        List<FVRObject.OTagFirearmMount> otagFirearmMountList = new List<FVRObject.OTagFirearmMount>()
        {
          FVRObject.OTagFirearmMount.Picatinny
        };
        int num15 = (int) lootTableType15;
        List<FVRObject.OTagEra> eras15 = otagEraList14;
        List<FVRObject.OTagFirearmMount> mounts = otagFirearmMountList;
        List<FVRObject.OTagAttachmentFeature> features3 = attachmentFeatureList3;
        requiredAttachments.Initialize((LootTable.LootTableType) num15, eras15, mounts: mounts, features: features3);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SideArm);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_Shotgun_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SecondaryWeapon_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SniperRifle);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_PrimaryWeapon_LowCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SmallOrdnance);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_PrimaryWeapon_MedCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SecondaryWeapon_HighCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_Shotgun_HighCap);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_PrimaryWeapon_HighCap);
        this.m_ltStartMin = 0;
        this.m_ltStartMax = 2;
        this.m_ltDiffModMin = -2;
        this.m_ltDiffModMax = 2;
        this.m_ltDiffMinCap = 5;
        this.m_maxWaveType = 18;
        this.m_waveDifficultyMod = 0;
        this.m_botHealthMultiplier = 1f;
        this.m_lootThrownThreshold = 0.75f;
        this.m_lootRareAttachThreshold = 0.65f;
        this.m_lootCommonAttachThreshold = 0.4f;
      }
      else if (this.m_generatedStyle == 4)
      {
        List<FVRObject.OTagEra> eras1 = new List<FVRObject.OTagEra>()
        {
          FVRObject.OTagEra.Modern,
          FVRObject.OTagEra.PostWar,
          FVRObject.OTagEra.Colonial,
          FVRObject.OTagEra.TurnOfTheCentury,
          FVRObject.OTagEra.WildWest,
          FVRObject.OTagEra.WW1,
          FVRObject.OTagEra.WW2
        };
        this.LT_Firearms_SideArm.Initialize(LootTable.LootTableType.Firearm, eras1, new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Pocket,
          FVRObject.OTagFirearmSize.Pistol
        }, maxCapacity: 20);
        LootTable ltGrenades = this.LT_Grenades;
        LootTable.LootTableType lootTableType1 = LootTable.LootTableType.Thrown;
        List<FVRObject.OTagThrownType> otagThrownTypeList = new List<FVRObject.OTagThrownType>()
        {
          FVRObject.OTagThrownType.Pinned
        };
        int num1 = (int) lootTableType1;
        List<FVRObject.OTagThrownType> thrownTypes = otagThrownTypeList;
        ltGrenades.Initialize((LootTable.LootTableType) num1, thrownTypes: thrownTypes);
        LootTable commonAttachments = this.LT_CommonAttachments;
        LootTable.LootTableType lootTableType2 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList1 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList1 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.BarrelExtension,
          FVRObject.OTagAttachmentFeature.Grip,
          FVRObject.OTagAttachmentFeature.IronSight,
          FVRObject.OTagAttachmentFeature.Reflex
        };
        int num2 = (int) lootTableType2;
        List<FVRObject.OTagEra> eras2 = otagEraList1;
        List<FVRObject.OTagAttachmentFeature> features1 = attachmentFeatureList1;
        commonAttachments.Initialize((LootTable.LootTableType) num2, eras2, features: features1);
        LootTable ltRareAttachments = this.LT_RareAttachments;
        LootTable.LootTableType lootTableType3 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList2 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList2 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.Laser,
          FVRObject.OTagAttachmentFeature.Magnification,
          FVRObject.OTagAttachmentFeature.Suppression,
          FVRObject.OTagAttachmentFeature.Stock
        };
        int num3 = (int) lootTableType3;
        List<FVRObject.OTagEra> eras3 = otagEraList2;
        List<FVRObject.OTagAttachmentFeature> features2 = attachmentFeatureList2;
        ltRareAttachments.Initialize((LootTable.LootTableType) num3, eras3, features: features2);
        LootTable ltMeleeWeapons = this.LT_MeleeWeapons;
        LootTable.LootTableType lootTableType4 = LootTable.LootTableType.Melee;
        List<FVRObject.OTagEra> otagEraList3 = eras1;
        List<FVRObject.OTagMeleeStyle> otagMeleeStyleList = new List<FVRObject.OTagMeleeStyle>()
        {
          FVRObject.OTagMeleeStyle.Tactical
        };
        List<FVRObject.OTagMeleeHandedness> otagMeleeHandednessList = new List<FVRObject.OTagMeleeHandedness>()
        {
          FVRObject.OTagMeleeHandedness.OneHanded
        };
        int num4 = (int) lootTableType4;
        List<FVRObject.OTagEra> eras4 = otagEraList3;
        List<FVRObject.OTagMeleeStyle> meleeStyles = otagMeleeStyleList;
        List<FVRObject.OTagMeleeHandedness> meleeHandedness = otagMeleeHandednessList;
        ltMeleeWeapons.Initialize((LootTable.LootTableType) num4, eras4, meleeStyles: meleeStyles, meleeHandedness: meleeHandedness);
        LootTable requiredAttachments = this.LT_RequiredAttachments;
        LootTable.LootTableType lootTableType5 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList4 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList3 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.IronSight,
          FVRObject.OTagAttachmentFeature.Reflex
        };
        List<FVRObject.OTagFirearmMount> otagFirearmMountList = new List<FVRObject.OTagFirearmMount>()
        {
          FVRObject.OTagFirearmMount.Picatinny
        };
        int num5 = (int) lootTableType5;
        List<FVRObject.OTagEra> eras5 = otagEraList4;
        List<FVRObject.OTagFirearmMount> mounts = otagFirearmMountList;
        List<FVRObject.OTagAttachmentFeature> features3 = attachmentFeatureList3;
        requiredAttachments.Initialize((LootTable.LootTableType) num5, eras5, mounts: mounts, features: features3);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SideArm);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SideArm);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SideArm);
        this.m_ltStartMin = 0;
        this.m_ltStartMax = 2;
        this.m_ltDiffModMin = -2;
        this.m_ltDiffModMax = 2;
        this.m_ltDiffMinCap = 1;
        this.m_maxWaveType = 18;
        this.m_waveDifficultyMod = 0;
        this.m_botHealthMultiplier = 1f;
        this.m_lootThrownThreshold = 0.75f;
        this.m_lootRareAttachThreshold = 0.65f;
        this.m_lootCommonAttachThreshold = 0.4f;
      }
      else if (this.m_generatedStyle == 5)
      {
        List<FVRObject.OTagEra> eras1 = new List<FVRObject.OTagEra>()
        {
          FVRObject.OTagEra.Modern,
          FVRObject.OTagEra.PostWar,
          FVRObject.OTagEra.TurnOfTheCentury,
          FVRObject.OTagEra.WildWest,
          FVRObject.OTagEra.WW1,
          FVRObject.OTagEra.WW2,
          FVRObject.OTagEra.Futuristic
        };
        this.LT_Firearms_SideArm.Initialize(LootTable.LootTableType.Firearm, eras1, new List<FVRObject.OTagFirearmSize>()
        {
          FVRObject.OTagFirearmSize.Pocket,
          FVRObject.OTagFirearmSize.Pistol,
          FVRObject.OTagFirearmSize.Compact,
          FVRObject.OTagFirearmSize.Carbine,
          FVRObject.OTagFirearmSize.FullSize,
          FVRObject.OTagFirearmSize.Bulky
        });
        this.LT_FirearmsSpecial.Initialize(LootTable.LootTableType.Firearm, eras1, sizes1);
        this.LT_Grenades.Initialize(LootTable.LootTableType.Thrown, eras1);
        LootTable commonAttachments = this.LT_CommonAttachments;
        LootTable.LootTableType lootTableType1 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList1 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList1 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.BarrelExtension,
          FVRObject.OTagAttachmentFeature.Grip,
          FVRObject.OTagAttachmentFeature.IronSight,
          FVRObject.OTagAttachmentFeature.Reflex
        };
        int num1 = (int) lootTableType1;
        List<FVRObject.OTagEra> eras2 = otagEraList1;
        List<FVRObject.OTagAttachmentFeature> features1 = attachmentFeatureList1;
        commonAttachments.Initialize((LootTable.LootTableType) num1, eras2, features: features1);
        LootTable ltRareAttachments = this.LT_RareAttachments;
        LootTable.LootTableType lootTableType2 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList2 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList2 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.Laser,
          FVRObject.OTagAttachmentFeature.Magnification,
          FVRObject.OTagAttachmentFeature.Suppression,
          FVRObject.OTagAttachmentFeature.Stock
        };
        int num2 = (int) lootTableType2;
        List<FVRObject.OTagEra> eras3 = otagEraList2;
        List<FVRObject.OTagAttachmentFeature> features2 = attachmentFeatureList2;
        ltRareAttachments.Initialize((LootTable.LootTableType) num2, eras3, features: features2);
        this.LT_MeleeWeapons.Initialize(LootTable.LootTableType.Melee, eras1);
        LootTable requiredAttachments = this.LT_RequiredAttachments;
        LootTable.LootTableType lootTableType3 = LootTable.LootTableType.Attachments;
        List<FVRObject.OTagEra> otagEraList3 = eras1;
        List<FVRObject.OTagAttachmentFeature> attachmentFeatureList3 = new List<FVRObject.OTagAttachmentFeature>()
        {
          FVRObject.OTagAttachmentFeature.IronSight,
          FVRObject.OTagAttachmentFeature.Reflex
        };
        List<FVRObject.OTagFirearmMount> otagFirearmMountList = new List<FVRObject.OTagFirearmMount>()
        {
          FVRObject.OTagFirearmMount.Picatinny
        };
        int num3 = (int) lootTableType3;
        List<FVRObject.OTagEra> eras4 = otagEraList3;
        List<FVRObject.OTagFirearmMount> mounts = otagFirearmMountList;
        List<FVRObject.OTagAttachmentFeature> features3 = attachmentFeatureList3;
        requiredAttachments.Initialize((LootTable.LootTableType) num3, eras4, mounts: mounts, features: features3);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SideArm);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SideArm);
        this.LTChart_CaseWeapon.Add(this.LT_Firearms_SideArm);
        this.m_ltStartMin = 0;
        this.m_ltStartMax = 2;
        this.m_ltDiffModMin = -2;
        this.m_ltDiffModMax = 2;
        this.m_ltDiffMinCap = 5;
        this.m_maxWaveType = 18;
        this.m_waveDifficultyMod = 0;
        this.m_botHealthMultiplier = 1f;
        this.m_lootThrownThreshold = 0.75f;
        this.m_lootRareAttachThreshold = 0.65f;
        this.m_lootCommonAttachThreshold = 0.4f;
      }
      LootTable ltHealth = this.LT_Health;
      LootTable.LootTableType lootTableType16 = LootTable.LootTableType.Powerup;
      List<FVRObject.OTagPowerupType> otagPowerupTypeList1 = new List<FVRObject.OTagPowerupType>()
      {
        FVRObject.OTagPowerupType.Health
      };
      int num16 = (int) lootTableType16;
      List<FVRObject.OTagPowerupType> powerupTypes1 = otagPowerupTypeList1;
      ltHealth.Initialize((LootTable.LootTableType) num16, powerupTypes: powerupTypes1);
      LootTable ltPowerUps = this.LT_PowerUps;
      LootTable.LootTableType lootTableType17 = LootTable.LootTableType.Powerup;
      List<FVRObject.OTagPowerupType> otagPowerupTypeList2 = new List<FVRObject.OTagPowerupType>()
      {
        FVRObject.OTagPowerupType.GhostMode,
        FVRObject.OTagPowerupType.InfiniteAmmo,
        FVRObject.OTagPowerupType.Invincibility,
        FVRObject.OTagPowerupType.QuadDamage
      };
      int num17 = (int) lootTableType17;
      List<FVRObject.OTagPowerupType> powerupTypes2 = otagPowerupTypeList2;
      ltPowerUps.Initialize((LootTable.LootTableType) num17, powerupTypes: powerupTypes2);
    }

    public void BeginGame()
    {
      if (this.m_generatedStyle != GM.TAHSettings.TAHOption_LootProgression)
        this.GenerateLootTables();
      this.TAHMenu.SetActive(false);
      this.ItemSpawnerState(false);
      this.m_curSupplyPointIndex = this.GetValidSupplyPointIndex();
      this.m_lastSupplyPointIndex = this.m_curSupplyPointIndex;
      TAH_SupplyPoint supplyPoint = this.SupplyPoints[this.m_curSupplyPointIndex];
      this.SpawnEquipmentAtPointFromLootTables(supplyPoint, true);
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(supplyPoint.PlayerSpawnPoint.position, true, supplyPoint.PlayerSpawnPoint.forward);
      this.UpdateHealth();
      this.InitiateTake(true);
    }

    public void TouchToEndTake()
    {
      if (this.State != TAH_Manager.TAHGameState.Taking)
        return;
      this.EndTake();
    }

    public void UpdateHealth()
    {
      if (GM.TAHSettings.TAHOption_PlayerHealth == 0)
        GM.CurrentPlayerBody.SetHealthThreshold(2000f);
      else if (GM.TAHSettings.TAHOption_PlayerHealth == 1)
        GM.CurrentPlayerBody.SetHealthThreshold(10000f);
      else if (GM.TAHSettings.TAHOption_PlayerHealth == 2)
        GM.CurrentPlayerBody.SetHealthThreshold(10f);
      else if (GM.TAHSettings.TAHOption_PlayerHealth == 3)
        GM.CurrentPlayerBody.SetHealthThreshold(4000f);
      else if (GM.TAHSettings.TAHOption_PlayerHealth == 4)
      {
        GM.CurrentPlayerBody.SetHealthThreshold(40000f);
      }
      else
      {
        if (GM.TAHSettings.TAHOption_PlayerHealth != 5)
          return;
        GM.CurrentPlayerBody.SetHealthThreshold(100000f);
      }
    }

    private void Update()
    {
      switch (this.State)
      {
        case TAH_Manager.TAHGameState.Taking:
          this.UpdateGameState_Taking();
          break;
        case TAH_Manager.TAHGameState.Holding:
          this.UpdateGameState_Holding();
          break;
      }
      this.BotCleanup();
      if (GM.TAHSettings.TAHOption_Music != 1)
        return;
      this.FMODController.Tick(Time.deltaTime);
    }

    private void BotCleanup()
    {
      if (this.SpawnedBots.Count > 0)
      {
        for (int index = this.SpawnedBots.Count - 1; index >= 0; --index)
        {
          if ((Object) this.SpawnedBots[index] == (Object) null)
            this.SpawnedBots.RemoveAt(index);
        }
      }
      if (this.SpawnedBots_Attacking.Count <= 0)
        return;
      for (int index = this.SpawnedBots_Attacking.Count - 1; index >= 0; --index)
      {
        if ((Object) this.SpawnedBots_Attacking[index] == (Object) null)
          this.SpawnedBots_Attacking.RemoveAt(index);
      }
    }

    private void ObjectCleanup()
    {
      if (this.m_knownObjs.Count <= 0)
        return;
      ++this.knownObjectCheckIndex;
      if (this.knownObjectCheckIndex >= this.m_knownObjs.Count)
        this.knownObjectCheckIndex = 0;
      if ((Object) this.m_knownObjs[this.knownObjectCheckIndex] == (Object) null)
      {
        this.m_knownObjsHash.Remove(this.m_knownObjs[this.knownObjectCheckIndex]);
        this.m_knownObjs.RemoveAt(this.knownObjectCheckIndex);
      }
      else
      {
        if ((double) Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.m_knownObjs[this.knownObjectCheckIndex].transform.position) <= 30.0)
          return;
        this.m_knownObjsHash.Remove(this.m_knownObjs[this.knownObjectCheckIndex]);
        Object.Destroy((Object) this.m_knownObjs[this.knownObjectCheckIndex].gameObject);
        this.m_knownObjs.RemoveAt(this.knownObjectCheckIndex);
      }
    }

    private void UpdateGameState_Taking()
    {
      TAH_DefensePoint defensePoint = this.DefensePoints[this.m_curDefensePointIndex];
      TAH_SupplyPoint supplyPoint = this.SupplyPoints[this.m_curSupplyPointIndex];
      Vector3 vector3 = GM.CurrentPlayerBody.LeftHand.position + GM.CurrentPlayerBody.LeftHand.forward * -0.2f;
      if (this.UsesOldReticle)
      {
        this.Reticle_Root.transform.position = vector3;
        this.Reticle_Root.transform.localScale = Vector3.one * 0.08f;
      }
      else
        this.TAHReticle.transform.position = vector3;
      Vector3 forward1 = defensePoint.PointCircle.transform.position - vector3;
      Vector3 forward2 = supplyPoint.PlayerSpawnPoint.transform.position - vector3;
      forward1.Normalize();
      forward2.Normalize();
      this.Reticle_TAH.rotation = Quaternion.LookRotation(forward1, Vector3.up);
      this.Reticle_Supply.rotation = Quaternion.LookRotation(forward2, Vector3.up);
      if (this.LogicStyle == TAH_Manager.TAHLogicStyle.HL2)
      {
        if ((double) this.m_timeUntilWeCanEventCheck > 0.0)
        {
          this.m_timeUntilWeCanEventCheck -= Time.deltaTime;
        }
        else
        {
          ++this.m_checkIndex_SupplyPoint;
          if (this.m_checkIndex_SupplyPoint >= this.SupplyPoints.Count)
            this.m_checkIndex_SupplyPoint = 0;
          if (!this.m_eventFiredSoFar_SupplyPoint.Contains(this.SupplyPoints[this.m_checkIndex_SupplyPoint]) && (double) Vector3.Distance(this.SupplyPoints[this.m_checkIndex_SupplyPoint].transform.position, GM.CurrentPlayerBody.transform.position) < 3.0)
          {
            this.TriggerEvent_SupplyPoint(this.SupplyPoints[this.m_checkIndex_SupplyPoint]);
            this.m_timeUntilWeCanEventCheck = Random.Range(this.m_eventTriggerTickRange.x, this.m_eventTriggerTickRange.y);
            this.m_eventFiredSoFar_SupplyPoint.Add(this.SupplyPoints[this.m_checkIndex_SupplyPoint]);
          }
        }
      }
      this.m_takeMusicIntensityTarget = (double) this.m_botKillThreshold <= 0.5 || (double) this.m_fireThreshold <= 10.0 ? 1f : 2f;
      this.m_takeMusicIntensity = Mathf.MoveTowards(this.m_takeMusicIntensity, this.m_takeMusicIntensityTarget, Time.deltaTime * 0.5f);
      if ((double) this.m_fireThreshold > 0.0)
        this.m_fireThreshold -= Time.deltaTime;
      else if ((double) this.m_botKillThreshold > 0.0)
        this.m_botKillThreshold -= Time.deltaTime * 1f;
      if (GM.TAHSettings.TAHOption_Music != 1)
        return;
      this.FMODController.SetIntParameterByIndex(0, "Intensity", this.m_takeMusicIntensity);
    }

    public void TriggerEvent_SupplyPoint(TAH_SupplyPoint point)
    {
      float num1 = Random.Range((float) this.GetDifficulty() * 0.25f, (float) this.GetDifficulty() + 3f);
      if ((double) num1 > 18.0)
      {
        int num2 = Random.Range(2, point.BotAttackSpawnPoints.Length);
        for (int index1 = 0; index1 < point.BotAttackSpawnPoints.Length; ++index1)
        {
          int index2 = (int) Random.Range((float) this.GetDifficulty() - 15f, (float) this.GetDifficulty() * 1f);
          if (index2 < 0)
            index2 = 0;
          if (index2 >= this.EventProfiles_FastZombies.Length)
            index2 = this.EventProfiles_FastZombies.Length - 1;
          TAH_BotSpawnProfile profilesFastZomby = this.EventProfiles_FastZombies[index2];
          this.SpawnBot(profilesFastZomby.GetRandomPrefab(), profilesFastZomby.GetRandomWeapon(), profilesFastZomby.GetRandomConfig(), point.BotAttackSpawnPoints[index1], true, false, point.NavGroup, false, profilesFastZomby.GetRandomSecondaryWeapon());
          --num2;
          if (num2 <= 0)
            break;
        }
      }
      else if ((double) num1 > 12.0)
      {
        for (int index = 0; index < point.BotAttackSpawnPoints.Length; ++index)
        {
          Transform attackSpawnPoint = point.BotAttackSpawnPoints[index];
          this.m_spawnedMobs.Add(Object.Instantiate<GameObject>(this.EventPrefabs_MeatCrabs[1], attackSpawnPoint.position + Vector3.up * 0.3f, attackSpawnPoint.rotation));
        }
      }
      else if ((double) num1 > 8.0)
      {
        int num2 = Random.Range(2, point.BotAttackSpawnPoints.Length);
        for (int index = 0; index < point.BotAttackSpawnPoints.Length; ++index)
        {
          TAH_BotSpawnProfile eventProfilesSpecOp = this.EventProfiles_SpecOps[Random.Range(0, this.EventProfiles_SpecOps.Length)];
          this.SpawnBot(eventProfilesSpecOp.GetRandomPrefab(), eventProfilesSpecOp.GetRandomWeapon(), eventProfilesSpecOp.GetRandomConfig(), point.BotAttackSpawnPoints[index], true, false, point.NavGroup, false, eventProfilesSpecOp.GetRandomSecondaryWeapon());
          --num2;
          if (num2 <= 0)
            break;
        }
      }
      else if ((double) num1 > 4.0)
      {
        int num2 = Random.Range(1, point.BotAttackSpawnPoints.Length);
        for (int index1 = 0; index1 < point.BotAttackSpawnPoints.Length; ++index1)
        {
          int index2 = (int) Random.Range((float) this.GetDifficulty() - 1f, (float) this.GetDifficulty() * 1f);
          if (index2 < 0)
            index2 = 0;
          if (index2 >= this.EventProfiles_MeatZombies.Length)
            index2 = this.EventProfiles_MeatZombies.Length - 1;
          TAH_BotSpawnProfile profilesMeatZomby = this.EventProfiles_MeatZombies[index2];
          this.SpawnBot(profilesMeatZomby.GetRandomPrefab(), profilesMeatZomby.GetRandomWeapon(), profilesMeatZomby.GetRandomConfig(), point.BotAttackSpawnPoints[index1], true, false, point.NavGroup, false, profilesMeatZomby.GetRandomSecondaryWeapon());
          --num2;
          if (num2 <= 0)
            break;
        }
      }
      else
      {
        for (int index = 0; index < point.BotAttackSpawnPoints.Length; ++index)
        {
          Transform attackSpawnPoint = point.BotAttackSpawnPoints[index];
          this.m_spawnedMobs.Add(Object.Instantiate<GameObject>(this.EventPrefabs_MeatCrabs[0], attackSpawnPoint.position + Vector3.up * 0.3f, attackSpawnPoint.rotation));
        }
      }
    }

    private void TakingBotKill()
    {
      this.m_botKillThreshold += 3f;
      this.m_botKillThreshold = Mathf.Clamp(this.m_botKillThreshold, 0.0f, 10f);
    }

    private void TakingGunShot()
    {
      this.m_fireThreshold += 3f;
      this.m_fireThreshold = Mathf.Clamp(this.m_fireThreshold, 0.0f, 20f);
    }

    private void UpdateGameState_Holding()
    {
      this.ObjectCleanup();
      this.m_holdTimer -= Time.deltaTime;
      if ((double) this.m_holdTimer <= -20.0)
      {
        this.EndHold();
      }
      else
      {
        TAH_DefensePoint defensePoint = this.DefensePoints[this.m_curDefensePointIndex];
        this.PingTimer -= Time.deltaTime;
        if ((double) this.PingTimer < 0.0)
        {
          this.PingTimer = Random.Range(10f, 15f);
          GM.CurrentSceneSettings.PingReceivers(defensePoint.PointCircle.transform.position);
        }
        this.m_waveTimer -= Time.deltaTime;
        TAH_DefensePoint.TAH_BotSpawner spawner = defensePoint.Spawner;
        spawner.TimeTilSpawnBot -= Time.deltaTime;
        if ((double) spawner.TimeTilSpawnBot <= 0.0 && spawner.NumLeftToSpawn > 0)
        {
          --spawner.NumLeftToSpawn;
          defensePoint.Spawner.TimeTilSpawnBot = spawner.SpawnCooldownTime;
          Transform spawnPoint = spawner.SpawnPoints[spawner.GetSpawnPointIndex()];
          this.SpawnBot(spawner.SpawnProfile.GetRandomPrefab(), spawner.SpawnProfile.GetRandomWeapon(), spawner.SpawnProfile.GetRandomConfig(), spawnPoint, true, false, defensePoint.NavGroup, true, spawner.SpawnProfile.GetRandomSecondaryWeapon());
        }
        if (this.SpawnedBots_Attacking.Count == 0 && spawner.NumLeftToSpawn == 0 && (double) this.m_waveTimer > 5.0)
        {
          this.m_waveTimer -= Time.deltaTime * 1f;
          this.m_holdTimer -= Time.deltaTime * 1f;
        }
        if ((double) this.m_waveTimer < 0.0 && this.m_currentWave < this.m_maxWave)
        {
          ++this.m_currentWave;
          defensePoint.BeginWave(this.m_holdSequence[this.m_currentWave]);
          this.m_waveTimer = this.m_holdSequence[this.m_currentWave].TimeForWave;
          if (GM.TAHSettings.TAHOption_Music == 1)
            this.FMODController.SetIntParameterByIndex(1, "Intensity", (float) this.m_holdSequence[this.m_currentWave].WaveIntensity);
        }
        Vector3 vector3 = GM.CurrentPlayerBody.LeftHand.position + GM.CurrentPlayerBody.LeftHand.forward * -0.2f;
        if (!this.UsesOldReticle)
          this.TAHReticle.transform.position = vector3;
        if (this.m_currentWave != this.m_maxWave || this.SpawnedBots_Attacking.Count != 0)
          return;
        bool flag = false;
        if (spawner.NumLeftToSpawn == 0)
          flag = true;
        if (!flag)
          return;
        this.EndHold();
      }
    }

    private void InitiateTake(bool isStart)
    {
      this.m_takeMusicIntensity = 1f;
      this.m_takeMusicIntensityTarget = 1f;
      this.m_fireThreshold = 0.0f;
      this.m_botKillThreshold = 0.0f;
      this.State = TAH_Manager.TAHGameState.Taking;
      if (GM.TAHSettings.TAHOption_Music == 1)
        this.FMODController.SwitchTo(0, 2f, false, false);
      int supplyPointIndex1 = this.GetValidSupplyPointIndex();
      int defensePointIndex1 = this.GetValidDefensePointIndex();
      this.m_lastSupplyPointIndex = this.m_curSupplyPointIndex;
      this.m_lastDefensePointIndex = this.m_curDefensePointIndex;
      this.m_curSupplyPointIndex = supplyPointIndex1;
      this.m_curDefensePointIndex = defensePointIndex1;
      TAH_SupplyPoint supplyPoint = this.SupplyPoints[this.m_curSupplyPointIndex];
      TAH_DefensePoint defensePoint = this.DefensePoints[this.m_curDefensePointIndex];
      this.SpawnEquipmentAtPointFromLootTables(supplyPoint, false);
      this.SpawnBotsAtSupplyPoint(supplyPoint, supplyPoint);
      this.SpawnBotsAtDefensePoint(defensePoint, defensePoint);
      if (this.LogicStyle == TAH_Manager.TAHLogicStyle.Classic)
      {
        if (!isStart)
        {
          int supplyPointIndex2 = this.GetValidSupplyPointIndex();
          int supplyPointIndex3 = this.GetValidSupplyPointIndex(supplyPointIndex2);
          this.SpawnBotsAtSupplyPoint(this.SupplyPoints[supplyPointIndex2], this.SupplyPoints[supplyPointIndex3]);
          this.SpawnBotsAtSupplyPoint(this.SupplyPoints[supplyPointIndex3], this.SupplyPoints[supplyPointIndex2]);
        }
        int defensePointIndex2 = this.GetValidDefensePointIndex();
        int defensePointIndex3 = this.GetValidDefensePointIndex(defensePointIndex2);
        this.SpawnBotsAtDefensePoint(this.DefensePoints[defensePointIndex2], this.DefensePoints[defensePointIndex2]);
        this.SpawnBotsAtDefensePoint(this.DefensePoints[defensePointIndex3], this.DefensePoints[defensePointIndex3]);
      }
      else if (this.LogicStyle == TAH_Manager.TAHLogicStyle.HL2)
        this.StartCoroutine(this.LevelConfiguration_Take(isStart));
      this.m_eventFiredSoFar_SupplyPoint.Clear();
      this.m_timeUntilWeCanEventCheck = Random.Range(this.m_eventTriggerTickRange.x, this.m_eventTriggerTickRange.y);
      this.m_checkIndex_SupplyPoint = 0;
      if (isStart && this.LogicStyle == TAH_Manager.TAHLogicStyle.HL2)
        this.m_eventFiredSoFar_SupplyPoint.Add(this.SupplyPoints[this.m_lastSupplyPointIndex]);
      defensePoint.PointCircle.SetActive(true);
      defensePoint.BeginTouch.SetActive(true);
      if (this.UsesOldReticle)
      {
        this.Reticle_Root.gameObject.SetActive(true);
      }
      else
      {
        this.TAHReticle.DeRegisterTrackedType(TAH_ReticleContact.ContactType.Hold);
        this.TAHReticle.RegisterTrackedObject(defensePoint.transform, TAH_ReticleContact.ContactType.Hold);
        this.TAHReticle.DeRegisterTrackedType(TAH_ReticleContact.ContactType.Supply);
        this.TAHReticle.RegisterTrackedObject(supplyPoint.transform, TAH_ReticleContact.ContactType.Supply);
      }
      ++this.m_pointsTaken;
      if (GM.TAHSettings.TAHOption_DifficultyProgression <= 0)
        return;
      ++this.m_pointsTaken;
    }

    [DebuggerHidden]
    private IEnumerator LevelConfiguration_Take(bool isStart) => (IEnumerator) new TAH_Manager.\u003CLevelConfiguration_Take\u003Ec__Iterator0()
    {
      isStart = isStart,
      \u0024this = this
    };

    private void InitiateHold()
    {
      this.State = TAH_Manager.TAHGameState.Holding;
      if (GM.TAHSettings.TAHOption_Music == 1)
        this.FMODController.SwitchTo(1, 0.0f, true, false);
      if (GM.TAHSettings.TAHOption_Music == 1)
        this.FMODController.SetIntParameterByIndex(1, "Intensity", 2f);
      TAH_SupplyPoint supplyPoint = this.SupplyPoints[this.m_curSupplyPointIndex];
      TAH_DefensePoint defensePoint = this.DefensePoints[this.m_curDefensePointIndex];
      defensePoint.InitiateDefense();
      int num1 = Mathf.RoundToInt(Mathf.Lerp(3f, 10f, (float) this.GetDifficulty() * 0.08f));
      this.m_holdSequence.Clear();
      int num2 = Mathf.Clamp(this.GetDifficulty() + this.m_waveDifficultyMod, 0, (int) ((double) this.m_maxWaveType * 1.5));
      this.m_holdTimer = 0.0f;
      for (int index = 0; index < num1; ++index)
        this.m_holdSequence.Add(this.WaveDefinitions[Random.Range(Random.Range((int) ((double) num2 * 0.150000005960464), (int) ((double) num2 * 0.400000005960464)), Mathf.Clamp(Mathf.Min(this.WaveDefinitions.Count, num2 + 2), 0, this.m_maxWaveType))]);
      for (int index = 0; index < this.m_holdSequence.Count; ++index)
        this.m_holdTimer += this.m_holdSequence[index].TimeForWave;
      this.m_holdTimer += 5f;
      this.m_currentWave = 0;
      this.m_maxWave = this.m_holdSequence.Count - 1;
      defensePoint.BeginWave(this.m_holdSequence[this.m_currentWave]);
      this.m_waveTimer = this.m_holdSequence[this.m_currentWave].TimeForWave;
      if (!this.UsesOldReticle)
        return;
      this.Reticle_Root.gameObject.SetActive(false);
    }

    public void EndTake()
    {
      this.ClearActiveBots();
      this.InitiateHold();
    }

    public void EndHold()
    {
      TAH_SupplyPoint supplyPoint = this.SupplyPoints[this.m_curSupplyPointIndex];
      this.DefensePoints[this.m_curDefensePointIndex].EndDefense();
      this.ClearActiveBots();
      this.ObjectCleanup();
      this.InitiateTake(false);
    }

    private void SpawnEquipmentAtPointFromLootTables(TAH_SupplyPoint point, bool isStart)
    {
      int difficulty = this.GetDifficulty();
      int num1 = difficulty + this.m_ltDiffModMin;
      int min = difficulty + this.m_ltDiffModMax;
      if (isStart)
      {
        num1 = this.m_ltStartMin;
        min = this.m_ltStartMax;
      }
      LootTable lootTable = this.LTChart_CaseWeapon[Mathf.Clamp(Random.Range(Mathf.Clamp(num1, 0, this.m_ltDiffMinCap), Mathf.Clamp(min, min, this.LTChart_CaseWeapon.Count + this.m_ltDiffModMax) + 1), 0, this.LTChart_CaseWeapon.Count - 1)];
      FVRObject randomObject1 = lootTable.GetRandomObject();
      FVRObject randomAmmoObject = lootTable.GetRandomAmmoObject(randomObject1, lootTable.Eras);
      FVRObject fvrObject1 = (FVRObject) null;
      FVRObject fvrObject2 = (FVRObject) null;
      FVRObject fvrObject3 = (FVRObject) null;
      if (randomObject1.RequiredSecondaryPieces.Count > 0)
      {
        fvrObject1 = randomObject1.RequiredSecondaryPieces[0];
        if (randomObject1.RequiredSecondaryPieces.Count > 1)
          fvrObject2 = randomObject1.RequiredSecondaryPieces[1];
        if (randomObject1.RequiredSecondaryPieces.Count > 2)
          fvrObject3 = randomObject1.RequiredSecondaryPieces[2];
      }
      else if (randomObject1.RequiresPicatinnySight)
      {
        fvrObject1 = this.LT_RequiredAttachments.GetRandomObject();
        if (fvrObject1.RequiredSecondaryPieces.Count > 0)
          fvrObject2 = fvrObject1.RequiredSecondaryPieces[0];
      }
      else if (randomObject1.BespokeAttachments.Count > 0 && (double) Random.Range(0.0f, 1f) > 0.75)
      {
        fvrObject1 = lootTable.GetRandomBespokeAttachment(randomObject1);
        if (fvrObject1.RequiredSecondaryPieces.Count > 0)
          fvrObject2 = fvrObject1.RequiredSecondaryPieces[0];
      }
      TAH_WeaponCrate tahWeaponCrate;
      if (randomObject1.TagFirearmSize >= FVRObject.OTagFirearmSize.Compact)
      {
        if ((Object) point.CrateLarge != (Object) null)
        {
          tahWeaponCrate = point.CrateLarge;
        }
        else
        {
          GameObject gameObject = Object.Instantiate<GameObject>(this.TAHCratePrefabLarge, point.SpawnPos_CrateLarge.position, point.SpawnPos_CrateLarge.rotation);
          point.CrateLarge = gameObject.GetComponent<TAH_WeaponCrate>();
          tahWeaponCrate = point.CrateLarge;
        }
      }
      else if ((Object) point.CrateSmall != (Object) null)
      {
        tahWeaponCrate = point.CrateSmall;
      }
      else
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.TAHCratePrefabSmall, point.SpawnPos_CrateSmall.position, point.SpawnPos_CrateSmall.rotation);
        point.CrateSmall = gameObject.GetComponent<TAH_WeaponCrate>();
        tahWeaponCrate = point.CrateSmall;
      }
      tahWeaponCrate.gameObject.SetActive(true);
      tahWeaponCrate.ResetCrate();
      tahWeaponCrate.Manager = this;
      GameObject gameObject1 = randomObject1.GetGameObject();
      GameObject go_mag = (GameObject) null;
      if ((Object) randomAmmoObject != (Object) null)
        go_mag = randomAmmoObject.GetGameObject();
      GameObject go_attach1 = (GameObject) null;
      if ((Object) fvrObject1 != (Object) null)
        go_attach1 = fvrObject1.GetGameObject();
      GameObject go_attach2 = (GameObject) null;
      if ((Object) fvrObject2 != (Object) null)
        go_attach2 = fvrObject2.GetGameObject();
      GameObject go_attach3 = (GameObject) null;
      if ((Object) fvrObject3 != (Object) null)
        go_attach3 = fvrObject2.GetGameObject();
      if (isStart)
      {
        this.AddObjectToTrackedList(Object.Instantiate<GameObject>(this.LT_MeleeWeapons.GetRandomObject().GetGameObject(), point.SpawnPoint_MeleeWeapon.position, point.SpawnPoint_MeleeWeapon.rotation));
      }
      else
      {
        float num2 = Random.Range(0.0f, 1f);
        if ((double) num2 > (double) this.m_lootThrownThreshold)
        {
          if (this.LT_Grenades.Loot.Count > 0)
            this.AddObjectToTrackedList(Object.Instantiate<GameObject>(this.LT_Grenades.GetRandomObject().GetGameObject(), point.SpawnPoint_MeleeWeapon.position, point.SpawnPoint_MeleeWeapon.rotation));
        }
        else if ((double) num2 > (double) this.m_lootRareAttachThreshold)
        {
          if (this.LT_RareAttachments.Loot.Count > 0)
            this.AddObjectToTrackedList(Object.Instantiate<GameObject>(this.LT_RareAttachments.GetRandomObject().GetGameObject(), point.SpawnPoint_MeleeWeapon.position, point.SpawnPoint_MeleeWeapon.rotation));
        }
        else if ((double) num2 > (double) this.m_lootCommonAttachThreshold && this.LT_CommonAttachments.Loot.Count > 0)
          this.AddObjectToTrackedList(Object.Instantiate<GameObject>(this.LT_CommonAttachments.GetRandomObject().GetGameObject(), point.SpawnPoint_MeleeWeapon.position, point.SpawnPoint_MeleeWeapon.rotation));
      }
      if (this.LT_FirearmsSpecial.Loot.Count > 0 && (double) Random.Range(10f, 25f) < (double) this.GetDifficulty() && (double) Random.Range(0.0f, 1f) > 0.800000011920929)
      {
        FVRObject randomObject2 = this.LT_FirearmsSpecial.GetRandomObject();
        this.AddObjectToTrackedList(Object.Instantiate<GameObject>(randomObject2.GetGameObject(), point.SpawnPoint_Large1.position, point.SpawnPoint_Large1.rotation));
        this.AddObjectToTrackedList(Object.Instantiate<GameObject>(this.LT_FirearmsSpecial.GetRandomAmmoObject(randomObject2).GetGameObject(), point.SpawnPoint_Large2.position, point.SpawnPoint_Large2.rotation));
      }
      tahWeaponCrate.PlaceItemsInCrate(gameObject1, go_mag, go_attach1, go_attach2, go_attach3);
      float num3 = 10f - (float) this.GetDifficulty();
      double num4 = (double) Mathf.Clamp(num3, 0.0f, 10f);
      if ((double) Random.Range(-0.5f, 10f) >= (double) num3)
        return;
      this.AddObjectToTrackedList(Object.Instantiate<GameObject>(((double) Random.Range(0.0f, 1f) <= 0.300000011920929 ? (AnvilAsset) this.LT_PowerUps.GetRandomObject() : (AnvilAsset) this.LT_Health.GetRandomObject()).GetGameObject(), point.SpawnPos_PowerUp.position, point.SpawnPos_PowerUp.rotation));
    }

    private void SpawnBotsAtSupplyPoint(TAH_SupplyPoint point, TAH_SupplyPoint moveToPoint)
    {
      for (int index = 0; index < point.BotSpawnPoints.Length; ++index)
      {
        TAH_BotSpawnProfile supplyPointBotProfile = this.SupplyPointBotProfiles[Mathf.Clamp(Random.Range(0, (int) (float) this.GetDifficulty()), 0, this.SupplyPointBotProfiles.Length - 1)];
        this.SpawnBot(supplyPointBotProfile.GetRandomPrefab(), supplyPointBotProfile.GetRandomWeapon(), supplyPointBotProfile.GetRandomConfig(), point.BotSpawnPoints[index], false, false, moveToPoint.NavGroup, false, supplyPointBotProfile.GetRandomSecondaryWeapon());
      }
    }

    private void SpawnBotsAtDefensePoint(TAH_DefensePoint point, TAH_DefensePoint moveToPoint)
    {
      for (int index = 0; index < point.StaticBotSpawnPoints.Length; ++index)
      {
        TAH_BotSpawnProfile defensePointBotProfile = this.DefensePointBotProfiles[Mathf.Clamp(Random.Range(0, (int) (float) this.GetDifficulty()), 0, this.SupplyPointBotProfiles.Length - 1)];
        this.SpawnBot(defensePointBotProfile.GetRandomPrefab(), defensePointBotProfile.GetRandomWeapon(), defensePointBotProfile.GetRandomConfig(), point.StaticBotSpawnPoints[index], false, false, moveToPoint.NavGroup, false, defensePointBotProfile.GetRandomSecondaryWeapon());
      }
    }

    public void AddObjectToTrackedList(GameObject g)
    {
      FVRPhysicalObject component = g.GetComponent<FVRPhysicalObject>();
      if (!((Object) component != (Object) null))
        return;
      this.AddFVRObjectToTrackedList(component);
    }

    private void AddFVRObjectToTrackedList(FVRPhysicalObject g)
    {
      if (!this.m_knownObjsHash.Add(g))
        return;
      this.m_knownObjs.Add(g);
    }

    private void SpawnBot(
      GameObject prefab,
      GameObject weapon,
      wwBotWurstConfig config,
      Transform point,
      bool isAggro,
      bool isStatic,
      wwBotWurstNavPointGroup ngroup,
      bool isHoldBot,
      GameObject weapon2 = null)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(prefab, point.position, point.rotation);
      wwBotWurst component1 = gameObject.GetComponent<wwBotWurst>();
      wwBotWurstModernGun component2 = Object.Instantiate<GameObject>(weapon, component1.ModernGunMount.position, component1.ModernGunMount.rotation).GetComponent<wwBotWurstModernGun>();
      component1.ModernGuns.Add(component2);
      component2.Bot = component1;
      component2.transform.SetParent(component1.ModernGunMount.parent);
      if (GM.TAHSettings.TAHOption_BotBullets > 0)
        component2.SetUseFastProjectile(true);
      if ((Object) weapon2 != (Object) null)
      {
        wwBotWurstModernGun component3 = Object.Instantiate<GameObject>(weapon2, component1.ModernGunMount.position, component1.ModernGunMount.rotation).GetComponent<wwBotWurstModernGun>();
        component1.ModernGuns.Add(component3);
        component3.Bot = component1;
        component3.transform.SetParent(component1.ModernGunMount.parent);
        if (GM.TAHSettings.TAHOption_BotBullets > 0)
          component3.SetUseFastProjectile(true);
      }
      component1.Config = config;
      component1.ReConfig(config, this.m_botHealthMultiplier);
      if (GM.TAHSettings.TAHOption_PlayerHealth == 0)
        component1.HealthOverride(500f, 1000f, 1000f);
      component1.NavPointGroup = ngroup;
      if (isAggro)
        component1.ShotEvent(GM.CurrentPlayerBody.transform.position);
      if (isStatic)
        component1.State = wwBotWurst.BotState.StandingAround;
      GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
      if (isHoldBot)
        this.SpawnedBots_Attacking.Add(gameObject);
      else
        this.SpawnedBots.Add(gameObject);
    }

    private void ClearActiveBots()
    {
      this.ClearStandardBots();
      this.ClearAttackingBots();
      this.ClearMobs();
      this.ClearEventMobs();
    }

    private void ClearStandardBots()
    {
      for (int index = this.SpawnedBots.Count - 1; index >= 0; --index)
      {
        if ((Object) this.SpawnedBots[index] != (Object) null)
          Object.Destroy((Object) this.SpawnedBots[index]);
      }
      this.SpawnedBots.Clear();
    }

    private void ClearAttackingBots()
    {
      for (int index = this.SpawnedBots_Attacking.Count - 1; index >= 0; --index)
      {
        if ((Object) this.SpawnedBots_Attacking[index] != (Object) null)
          Object.Destroy((Object) this.SpawnedBots_Attacking[index]);
      }
      this.SpawnedBots_Attacking.Clear();
    }

    private void ClearMobs()
    {
      for (int index = this.m_spawnedMobs.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_spawnedMobs[index] != (Object) null)
          Object.Destroy((Object) this.m_spawnedMobs[index]);
      }
      this.m_spawnedMobs.Clear();
    }

    private void ClearEventMobs()
    {
      for (int index = this.m_eventSpawns.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_eventSpawns[index] != (Object) null)
          Object.Destroy((Object) this.m_eventSpawns[index]);
      }
      this.m_eventSpawns.Clear();
    }

    private void PlayerDied()
    {
    }

    private int GetValidSupplyPointIndex()
    {
      List<int> intList = new List<int>();
      for (int index = 0; index < this.SupplyPoints.Count; ++index)
        intList.Add(index);
      if (intList.Contains(this.m_curSupplyPointIndex))
        intList.Remove(this.m_curSupplyPointIndex);
      if (intList.Contains(this.m_lastSupplyPointIndex))
        intList.Remove(this.m_lastSupplyPointIndex);
      int num = intList[Random.Range(0, intList.Count)];
      return (double) Vector3.Distance(this.SupplyPoints[num].transform.position, GM.CurrentPlayerBody.transform.position) > 40.0 ? num : this.GetValidSupplyPointIndex(num);
    }

    private int GetValidDefensePointIndex()
    {
      List<int> intList = new List<int>();
      for (int index = 0; index < this.DefensePoints.Count; ++index)
        intList.Add(index);
      if (intList.Contains(this.m_curDefensePointIndex))
        intList.Remove(this.m_curDefensePointIndex);
      if (intList.Contains(this.m_lastDefensePointIndex))
        intList.Remove(this.m_lastDefensePointIndex);
      int num = intList[Random.Range(0, intList.Count)];
      return (double) Vector3.Distance(this.DefensePoints[num].transform.position, GM.CurrentPlayerBody.transform.position) > 40.0 ? num : this.GetValidDefensePointIndex(num);
    }

    private int GetValidSupplyPointIndex(int exlucludeAlso)
    {
      List<int> intList = new List<int>();
      for (int index = 0; index < this.SupplyPoints.Count; ++index)
        intList.Add(index);
      if (intList.Contains(this.m_curSupplyPointIndex))
        intList.Remove(this.m_curSupplyPointIndex);
      if (intList.Contains(this.m_lastSupplyPointIndex))
        intList.Remove(this.m_lastSupplyPointIndex);
      if (intList.Contains(exlucludeAlso))
        intList.Remove(exlucludeAlso);
      int num = intList[Random.Range(0, intList.Count)];
      return (double) Vector3.Distance(this.SupplyPoints[num].transform.position, GM.CurrentPlayerBody.transform.position) > 40.0 ? num : this.GetValidSupplyPointIndex(num);
    }

    private int GetValidDefensePointIndex(int exlucludeAlso)
    {
      List<int> intList = new List<int>();
      for (int index = 0; index < this.DefensePoints.Count; ++index)
        intList.Add(index);
      if (intList.Contains(this.m_curDefensePointIndex))
        intList.Remove(this.m_curDefensePointIndex);
      if (intList.Contains(this.m_lastDefensePointIndex))
        intList.Remove(this.m_lastDefensePointIndex);
      if (intList.Contains(exlucludeAlso))
        intList.Remove(exlucludeAlso);
      int num = intList[Random.Range(0, intList.Count)];
      return (double) Vector3.Distance(this.DefensePoints[num].transform.position, GM.CurrentPlayerBody.transform.position) > 40.0 ? num : this.GetValidDefensePointIndex(num);
    }

    public enum TAHLogicStyle
    {
      Classic,
      HL2,
    }

    public enum TAHGameState
    {
      WaitingToStart,
      Taking,
      Holding,
    }
  }
}
