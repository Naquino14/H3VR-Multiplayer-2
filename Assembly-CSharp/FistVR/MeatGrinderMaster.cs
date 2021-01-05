// Decompiled with JetBrains decompiler
// Type: FistVR.MeatGrinderMaster
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MeatGrinderMaster : MonoBehaviour
  {
    [Header("Narrator")]
    public MG_Narrator Narrator;
    public Transform PlayerRig;
    public GameObject StartingRoomPrefab;
    private MG_StartingRoom m_startingRoom;
    public FVRSceneSettings SceneSettings;
    public RedRoom.Quadrant PlayerQuadrant;
    [Header("Critical Item Prefabs")]
    public FVRObject FlashLightPrefab;
    public FVRObject FlipzoPrefab;
    public FVRObject TacticalFlashlightPrefab;
    public FVRObject GlowStickPrefab;
    public FVRObject BoxOfMatchesPrefab;
    public GameObject ItemSpawnerPrefab;
    public bool m_hasSpawnedSeconaryLight;
    [Header("Rooms")]
    public RedRoom[] SmallRooms;
    public RedRoom[] MediumRooms;
    public RedRoom[] LargeRooms;
    private int m_nextSmallRoomToConfigure;
    private int m_nextMediumRoomToConfigure;
    private int m_nextLargeRoomToConfigure;
    [Header("Trap Room Config")]
    public GameObject[] TrapRoomPrefabs;
    [Header("Meat Room Config")]
    public GameObject[] MeatRoomPrefabs;
    [Header("LottoRoom Config")]
    public GameObject[] LottoRoomPrefabs;
    [Header("Cabinet Config")]
    public GameObject[] CabinetPrefabs;
    public Transform[] PossibleCabinetSpawns;
    public float CabinetSpawnChance = 0.2f;
    [Header("Metal Shelves Config")]
    public GameObject[] MetalShelfPrefabs;
    public Transform[] PossibleShelfSpawns;
    public float ShelfSpawnChance = 0.45f;
    [Header("Industrial Shelves Config")]
    public GameObject IndustrialShelfPrefabShort;
    public GameObject IndustrialShelfPrefabLong;
    public float IndustrialShelfChance = 0.45f;
    public Transform[] PossibleIndustrialShelfShortSpawns;
    public Transform[] PossibleIndustrialShelfLongSpawns;
    [Header("SmashyStools")]
    public GameObject[] SmashyStoolPrefabs;
    public float SmashyStoolChance = 0.25f;
    public Transform[] PossibleSmashyStoolSpawns;
    [Header("MeatPiles")]
    public GameObject[] MeatPilePrefabs;
    public float MeatPileChance = 0.4f;
    public Transform[] PossibleMeatPileSpawns;
    [Header("CookedMeatPiles")]
    public GameObject[] CookedMeatPilePrefabs;
    public float CookedMeatPileChance = 0.3f;
    public Transform[] CookedPossibleMeatPileSpawns;
    [Header("LaserMines")]
    public GameObject LaserMinePrefab;
    private float LaserMineChance = 0.2f;
    public Transform[] LaserMinesSpawnPoints;
    [Header("Loot Tables")]
    public LootTable LT_Handguns = new LootTable();
    public LootTable LT_Shotguns = new LootTable();
    public LootTable LT_RareGuns = new LootTable();
    public LootTable LT_SuperRareGuns = new LootTable();
    public LootTable LT_Melee = new LootTable();
    public LootTable LT_Attachments = new LootTable();
    public LootTable LT_Junk = new LootTable();
    public LootTable LT_Powerups = new LootTable();
    public FVRObject LTEntry_Handgun1;
    public FVRObject LTEntry_Handgun2;
    public FVRObject LTEntry_Handgun3;
    public FVRObject LTEntry_Shotgun1;
    public FVRObject LTEntry_Shotgun2;
    public FVRObject LTEntry_Shotgun3;
    public FVRObject LTEntry_RareGun1;
    public FVRObject LTEntry_RareGun2;
    public FVRObject LTEntry_RareGun3;
    public FVRObject LTEntry_SuperRareGun1;
    [Header("Player stuff")]
    public FVRViveHand[] hands;
    private HashSet<FVRInteractiveObject> m_objectsThatHaveBeenHeld;
    private float m_TimeLeft = 1800f;
    public float[] TimeThresholds;
    public bool[] TimeThresholdPassed;
    private List<MG_Cabinet> ToSpawnCabinets;
    private List<MH_MetalShelf> ToSpawnShelves;
    [Header("Event AI Stuff")]
    public MeatGrinderMaster.EventAI EventAIDude;
    public MG_EventAIConfig[] AIConfigs;
    private GameObject StartingWeiner1;
    private GameObject StartingWeiner2;
    [Header("Agent Stuff")]
    public RedRoom MeatRoom1;
    public RedRoom MeatRoom2;
    public RedRoom MeatRoom3;
    private GameObject Weiner1;
    private GameObject Weiner2;
    private GameObject Slicer1;
    private GameObject Slicer2;
    private GameObject Weiner3;
    private GameObject Weiner4;
    private GameObject Slicer3;
    private GameObject Slicer4;
    [Header("Bot Prefabs")]
    public GameObject[] HydraulicBotPrefabs;
    public GameObject SlicerPrefab;
    public GameObject[] FlameShotgunBotPrefabs;
    public GameObject FlyingHotDogSwarmPrefab;
    public GameObject FlamingMeatball;
    public GameObject LetterM;
    public GameObject ScreamingJerry;
    public Transform[] PossibleWeinerBotSpawnPositions;
    public Transform[] PossibleSlicerSpawnPositions;
    public Transform[] PossibleShotgunBotSpawnPositions_Boiler;
    public Transform[] PossibleShotgunBotSpawnPositions_Office;
    public Transform[] PossibleShotgunBotSpawnPositions_Freezer;
    public Transform[] PossibleShotgunBotSpawnPositions_Restaraunt;
    public wwBotWurstNavPointGroup FlameShotgunNavGroup_Boiler;
    public wwBotWurstNavPointGroup FlameShotgunNavGroup_Office;
    public wwBotWurstNavPointGroup FlameShotgunNavGroup_Freezer;
    public wwBotWurstNavPointGroup FlameShotgunNavGroup_Restaraunt;
    public List<wwBotWurst> SpawnedShotgunBots_Boiler = new List<wwBotWurst>();
    public List<wwBotWurst> SpawnedShotgunBots_Office = new List<wwBotWurst>();
    public List<wwBotWurst> SpawnedShotgunBots_Freezer = new List<wwBotWurst>();
    public List<wwBotWurst> SpawnedShotgunBots_Restaraunt = new List<wwBotWurst>();
    private float m_botCheckTick = 10f;
    private int badGuyThresholdAdd;
    public bool IsDead;
    private float m_PlayerHealth = 1f;
    private bool hasWarnedPlayerHealth1;
    public bool IsCountingDown = true;

    private void Awake()
    {
      GM.MGMaster = this;
      this.m_objectsThatHaveBeenHeld = new HashSet<FVRInteractiveObject>();
      this.GenerateLootTables();
      this.ConfigureRooms();
      this.ToSpawnCabinets = new List<MG_Cabinet>();
      this.ToSpawnShelves = new List<MH_MetalShelf>();
      this.SpawnItems();
      this.EventAIDude.Config = this.AIConfigs[(int) GM.Options.MeatGrinderFlags.AIMood];
      this.EventAIDude.Init();
      if (GM.Options.MeatGrinderFlags.MGMode != MeatGrinderFlags.MeatGrinderMode.KidsMeatyMeal)
        return;
      GM.CurrentSceneSettings.DoesDamageGetRegistered = false;
    }

    private void Start()
    {
      if (GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.KidsMeatyMeal)
        GM.CurrentSceneSettings.DoesDamageGetRegistered = false;
      if (GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.BuildYourOwnMeat || GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.KidsMeatyMeal)
        GM.CurrentSceneSettings.IsSpawnLockingEnabled = true;
      this.MeatRoom2.CloseDoors(false);
      this.MeatRoom3.CloseDoors(false);
    }

    private void GenerateLootTables()
    {
      List<FVRObject.OTagFirearmFiringMode> firearmFiringModeList1 = new List<FVRObject.OTagFirearmFiringMode>()
      {
        FVRObject.OTagFirearmFiringMode.Burst,
        FVRObject.OTagFirearmFiringMode.FullAuto
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
        FVRObject.OTagFirearmRoundPower.Pistol,
        FVRObject.OTagFirearmRoundPower.Intermediate,
        FVRObject.OTagFirearmRoundPower.FullPower
      };
      List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList4 = new List<FVRObject.OTagFirearmRoundPower>()
      {
        FVRObject.OTagFirearmRoundPower.Intermediate,
        FVRObject.OTagFirearmRoundPower.FullPower
      };
      List<FVRObject.OTagEra> otagEraList1 = new List<FVRObject.OTagEra>()
      {
        FVRObject.OTagEra.Modern,
        FVRObject.OTagEra.PostWar,
        FVRObject.OTagEra.WW2,
        FVRObject.OTagEra.WW1,
        FVRObject.OTagEra.TurnOfTheCentury
      };
      LootTable ltHandguns = this.LT_Handguns;
      LootTable.LootTableType lootTableType1 = LootTable.LootTableType.Firearm;
      List<FVRObject.OTagEra> otagEraList2 = otagEraList1;
      List<FVRObject.OTagFirearmSize> otagFirearmSizeList1 = new List<FVRObject.OTagFirearmSize>()
      {
        FVRObject.OTagFirearmSize.Pocket,
        FVRObject.OTagFirearmSize.Pistol,
        FVRObject.OTagFirearmSize.Compact
      };
      List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList5 = firearmRoundPowerList1;
      List<FVRObject.OTagFirearmFiringMode> firearmFiringModeList2 = firearmFiringModeList1;
      int num1 = (int) lootTableType1;
      List<FVRObject.OTagEra> eras1 = otagEraList2;
      List<FVRObject.OTagFirearmSize> sizes1 = otagFirearmSizeList1;
      List<FVRObject.OTagFirearmFiringMode> excludeModes = firearmFiringModeList2;
      List<FVRObject.OTagFirearmRoundPower> roundPowers1 = firearmRoundPowerList5;
      ltHandguns.Initialize((LootTable.LootTableType) num1, eras1, sizes1, excludeModes: excludeModes, roundPowers: roundPowers1, maxCapacity: 30);
      LootTable ltShotguns = this.LT_Shotguns;
      LootTable.LootTableType lootTableType2 = LootTable.LootTableType.Firearm;
      List<FVRObject.OTagEra> otagEraList3 = otagEraList1;
      List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList6 = firearmRoundPowerList2;
      int num2 = (int) lootTableType2;
      List<FVRObject.OTagEra> eras2 = otagEraList3;
      List<FVRObject.OTagFirearmRoundPower> roundPowers2 = firearmRoundPowerList6;
      ltShotguns.Initialize((LootTable.LootTableType) num2, eras2, roundPowers: roundPowers2, maxCapacity: 8);
      LootTable ltRareGuns = this.LT_RareGuns;
      LootTable.LootTableType lootTableType3 = LootTable.LootTableType.Firearm;
      List<FVRObject.OTagEra> otagEraList4 = otagEraList1;
      List<FVRObject.OTagFirearmSize> otagFirearmSizeList2 = new List<FVRObject.OTagFirearmSize>()
      {
        FVRObject.OTagFirearmSize.Compact,
        FVRObject.OTagFirearmSize.Carbine,
        FVRObject.OTagFirearmSize.FullSize
      };
      List<FVRObject.OTagFirearmAction> otagFirearmActionList1 = new List<FVRObject.OTagFirearmAction>()
      {
        FVRObject.OTagFirearmAction.Automatic
      };
      List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList7 = firearmRoundPowerList3;
      int num3 = (int) lootTableType3;
      List<FVRObject.OTagEra> eras3 = otagEraList4;
      List<FVRObject.OTagFirearmSize> sizes2 = otagFirearmSizeList2;
      List<FVRObject.OTagFirearmAction> actions1 = otagFirearmActionList1;
      List<FVRObject.OTagFirearmRoundPower> roundPowers3 = firearmRoundPowerList7;
      ltRareGuns.Initialize((LootTable.LootTableType) num3, eras3, sizes2, actions1, roundPowers: roundPowers3, maxCapacity: 32);
      LootTable ltSuperRareGuns = this.LT_SuperRareGuns;
      LootTable.LootTableType lootTableType4 = LootTable.LootTableType.Firearm;
      List<FVRObject.OTagEra> otagEraList5 = otagEraList1;
      List<FVRObject.OTagFirearmSize> otagFirearmSizeList3 = new List<FVRObject.OTagFirearmSize>()
      {
        FVRObject.OTagFirearmSize.Compact,
        FVRObject.OTagFirearmSize.Carbine,
        FVRObject.OTagFirearmSize.FullSize
      };
      List<FVRObject.OTagFirearmAction> otagFirearmActionList2 = new List<FVRObject.OTagFirearmAction>()
      {
        FVRObject.OTagFirearmAction.Automatic
      };
      List<FVRObject.OTagFirearmRoundPower> firearmRoundPowerList8 = firearmRoundPowerList4;
      int num4 = (int) lootTableType4;
      List<FVRObject.OTagEra> eras4 = otagEraList5;
      List<FVRObject.OTagFirearmSize> sizes3 = otagFirearmSizeList3;
      List<FVRObject.OTagFirearmAction> actions2 = otagFirearmActionList2;
      List<FVRObject.OTagFirearmRoundPower> roundPowers4 = firearmRoundPowerList8;
      ltSuperRareGuns.Initialize((LootTable.LootTableType) num4, eras4, sizes3, actions2, roundPowers: roundPowers4, minCapacity: 20);
      LootTable ltMelee = this.LT_Melee;
      LootTable.LootTableType lootTableType5 = LootTable.LootTableType.Melee;
      List<FVRObject.OTagEra> otagEraList6 = otagEraList1;
      List<FVRObject.OTagMeleeStyle> otagMeleeStyleList = new List<FVRObject.OTagMeleeStyle>()
      {
        FVRObject.OTagMeleeStyle.Tactical
      };
      List<FVRObject.OTagMeleeHandedness> otagMeleeHandednessList = new List<FVRObject.OTagMeleeHandedness>()
      {
        FVRObject.OTagMeleeHandedness.OneHanded
      };
      int num5 = (int) lootTableType5;
      List<FVRObject.OTagEra> eras5 = otagEraList6;
      List<FVRObject.OTagMeleeStyle> meleeStyles = otagMeleeStyleList;
      List<FVRObject.OTagMeleeHandedness> meleeHandedness = otagMeleeHandednessList;
      ltMelee.Initialize((LootTable.LootTableType) num5, eras5, meleeStyles: meleeStyles, meleeHandedness: meleeHandedness);
      LootTable ltAttachments = this.LT_Attachments;
      LootTable.LootTableType lootTableType6 = LootTable.LootTableType.Attachments;
      List<FVRObject.OTagEra> otagEraList7 = otagEraList1;
      List<FVRObject.OTagAttachmentFeature> attachmentFeatureList = new List<FVRObject.OTagAttachmentFeature>()
      {
        FVRObject.OTagAttachmentFeature.BarrelExtension,
        FVRObject.OTagAttachmentFeature.Grip,
        FVRObject.OTagAttachmentFeature.IronSight,
        FVRObject.OTagAttachmentFeature.Reflex,
        FVRObject.OTagAttachmentFeature.Laser
      };
      int num6 = (int) lootTableType6;
      List<FVRObject.OTagEra> eras6 = otagEraList7;
      List<FVRObject.OTagAttachmentFeature> features = attachmentFeatureList;
      ltAttachments.Initialize((LootTable.LootTableType) num6, eras6, features: features);
      LootTable ltPowerups = this.LT_Powerups;
      LootTable.LootTableType lootTableType7 = LootTable.LootTableType.Powerup;
      List<FVRObject.OTagPowerupType> otagPowerupTypeList1 = new List<FVRObject.OTagPowerupType>()
      {
        FVRObject.OTagPowerupType.Health,
        FVRObject.OTagPowerupType.GhostMode,
        FVRObject.OTagPowerupType.InfiniteAmmo,
        FVRObject.OTagPowerupType.Invincibility,
        FVRObject.OTagPowerupType.QuadDamage
      };
      int num7 = (int) lootTableType7;
      List<FVRObject.OTagPowerupType> powerupTypes1 = otagPowerupTypeList1;
      ltPowerups.Initialize((LootTable.LootTableType) num7, powerupTypes: powerupTypes1);
      LootTable ltJunk = this.LT_Junk;
      LootTable.LootTableType lootTableType8 = LootTable.LootTableType.Powerup;
      List<FVRObject.OTagPowerupType> otagPowerupTypeList2 = new List<FVRObject.OTagPowerupType>()
      {
        FVRObject.OTagPowerupType.Health,
        FVRObject.OTagPowerupType.GhostMode,
        FVRObject.OTagPowerupType.InfiniteAmmo,
        FVRObject.OTagPowerupType.Invincibility,
        FVRObject.OTagPowerupType.QuadDamage
      };
      int num8 = (int) lootTableType8;
      List<FVRObject.OTagPowerupType> powerupTypes2 = otagPowerupTypeList2;
      ltJunk.Initialize((LootTable.LootTableType) num8, powerupTypes: powerupTypes2);
      this.LTEntry_Handgun1 = this.LT_Handguns.GetRandomObject();
      this.LT_Handguns.Loot.Remove(this.LTEntry_Handgun1);
      this.LTEntry_Handgun2 = this.LT_Handguns.GetRandomObject();
      this.LT_Handguns.Loot.Remove(this.LTEntry_Handgun2);
      this.LTEntry_Handgun3 = this.LT_Handguns.GetRandomObject();
      this.LT_Handguns.Loot.Remove(this.LTEntry_Handgun3);
      this.LTEntry_Shotgun1 = this.LT_Shotguns.GetRandomObject();
      this.LT_Shotguns.Loot.Remove(this.LTEntry_Shotgun1);
      this.LTEntry_Shotgun2 = this.LT_Shotguns.GetRandomObject();
      this.LT_Shotguns.Loot.Remove(this.LTEntry_Shotgun2);
      this.LTEntry_Shotgun3 = this.LT_Shotguns.GetRandomObject();
      this.LT_Shotguns.Loot.Remove(this.LTEntry_Shotgun3);
      this.LTEntry_RareGun1 = this.LT_RareGuns.GetRandomObject();
      this.LT_RareGuns.Loot.Remove(this.LTEntry_RareGun1);
      this.LTEntry_RareGun2 = this.LT_RareGuns.GetRandomObject();
      this.LT_RareGuns.Loot.Remove(this.LTEntry_RareGun2);
      this.LTEntry_RareGun3 = this.LT_RareGuns.GetRandomObject();
      this.LT_RareGuns.Loot.Remove(this.LTEntry_RareGun3);
      this.LTEntry_SuperRareGun1 = this.LT_SuperRareGuns.GetRandomObject();
      this.LT_SuperRareGuns.Loot.Remove(this.LTEntry_SuperRareGun1);
    }

    public void SpawnObjectAtPlace(FVRObject obj, Transform t) => this.SpawnObjectAtPlace(obj, t.position, t.rotation);

    public void SpawnAmmoAtPlaceForGun(FVRObject gun, Transform t) => this.SpawnAmmoAtPlaceForGun(gun, t.position, t.rotation);

    public void SpawnObjectAtPlace(FVRObject obj, Vector3 pos, Quaternion rotation)
    {
      FVRFireArmMagazine component = UnityEngine.Object.Instantiate<GameObject>(obj.GetGameObject(), pos, rotation).GetComponent<FVRFireArmMagazine>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || component.RoundType == FireArmRoundType.aFlameThrowerFuel)
        return;
      component.ReloadMagWithTypeUpToPercentage(AM.GetRandomValidRoundClass(component.RoundType), Mathf.Clamp(UnityEngine.Random.Range(0.3f, 1f), 0.1f, 1f));
    }

    public void SpawnAmmoAtPlaceForGun(FVRObject gun, Vector3 pos, Quaternion rotation)
    {
      if (gun.CompatibleMagazines.Count > 0)
        this.SpawnObjectAtPlace(gun.CompatibleMagazines[UnityEngine.Random.Range(0, gun.CompatibleMagazines.Count - 1)], pos, rotation);
      else if (gun.CompatibleClips.Count > 0)
        this.SpawnObjectAtPlace(gun.CompatibleClips[UnityEngine.Random.Range(0, gun.CompatibleClips.Count - 1)], pos, rotation);
      else if (gun.CompatibleSpeedLoaders.Count > 0)
      {
        this.SpawnObjectAtPlace(gun.CompatibleSpeedLoaders[UnityEngine.Random.Range(0, gun.CompatibleSpeedLoaders.Count - 1)], pos, rotation);
      }
      else
      {
        if (gun.CompatibleSingleRounds.Count <= 0)
          return;
        int num = UnityEngine.Random.Range(2, 5);
        Vector3 vector3 = pos;
        for (int index = 0; index < num; ++index)
        {
          Vector3 pos1 = vector3 + Vector3.up * (0.05f * (float) index);
          this.SpawnObjectAtPlace(gun.CompatibleSingleRounds[UnityEngine.Random.Range(0, gun.CompatibleSingleRounds.Count - 1)], pos1, rotation);
        }
      }
    }

    public void SpawnGunAmmoPairToTransformList(FVRObject gun, Transform[] pointArray)
    {
      if (pointArray.Length <= 0)
        return;
      this.SpawnObjectAtPlace(gun, pointArray[0]);
      int index = pointArray.Length - 1;
      this.SpawnAmmoAtPlaceForGun(gun, pointArray[index]);
    }

    private void Update()
    {
      this.CheckHandContents();
      this.CheckTime();
      this.CheckBotCounts();
      this.EventAIDude.Tick();
    }

    public void SpawnBadGuySetInitial() => this.badGuyThresholdAdd = 2;

    public void SpawnBadGuySet1() => this.badGuyThresholdAdd = 4;

    public void SpawnBadGuySet2() => this.badGuyThresholdAdd = 6;

    private void CheckBotCounts()
    {
      if ((double) this.m_botCheckTick > 0.0)
      {
        this.m_botCheckTick -= Time.deltaTime;
      }
      else
      {
        this.m_botCheckTick = UnityEngine.Random.Range(15f, 45f);
        int num = UnityEngine.Random.Range(0, 3);
        GameObject prefab = (double) UnityEngine.Random.Range(0.0f, 1f) <= 0.5 ? this.HydraulicBotPrefabs[UnityEngine.Random.Range(0, this.HydraulicBotPrefabs.Length)] : this.FlameShotgunBotPrefabs[UnityEngine.Random.Range(0, this.FlameShotgunBotPrefabs.Length)];
        switch (num)
        {
          case 0:
            this.CheckNSpawnBotWurst(this.SpawnedShotgunBots_Boiler, prefab, this.FlameShotgunNavGroup_Boiler, this.PossibleShotgunBotSpawnPositions_Boiler);
            break;
          case 1:
            this.CheckNSpawnBotWurst(this.SpawnedShotgunBots_Office, prefab, this.FlameShotgunNavGroup_Office, this.PossibleShotgunBotSpawnPositions_Office);
            break;
          case 2:
            this.CheckNSpawnBotWurst(this.SpawnedShotgunBots_Freezer, prefab, this.FlameShotgunNavGroup_Freezer, this.PossibleShotgunBotSpawnPositions_Freezer);
            break;
          case 3:
            this.CheckNSpawnBotWurst(this.SpawnedShotgunBots_Restaraunt, prefab, this.FlameShotgunNavGroup_Restaraunt, this.PossibleShotgunBotSpawnPositions_Restaraunt);
            break;
        }
      }
    }

    private void CheckNSpawnBotWurst(
      List<wwBotWurst> list,
      GameObject prefab,
      wwBotWurstNavPointGroup Navgroup,
      Transform[] SpawnPoints)
    {
      for (int index = list.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) list[index] == (UnityEngine.Object) null)
          list.RemoveAt(index);
      }
      int num1 = (int) GM.Options.MeatGrinderFlags.AIMood;
      if (GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.Classic)
        num1 = -1;
      int num2 = num1 + this.badGuyThresholdAdd;
      if (num2 < 0 || list.Count > num2)
        return;
      int index1 = UnityEngine.Random.Range(0, SpawnPoints.Length);
      if ((double) Vector3.Distance(GM.CurrentPlayerBody.transform.position, SpawnPoints[index1].position) <= 15.0)
        return;
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, SpawnPoints[index1].position, SpawnPoints[index1].rotation);
      wwBotWurst component = gameObject.GetComponent<wwBotWurst>();
      component.NavPointGroup = Navgroup;
      list.Add(component);
      GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
    }

    private void ConfigureRooms()
    {
      this.ShuffleRooms(this.SmallRooms);
      this.ShuffleRooms(this.SmallRooms);
      this.ShuffleRooms(this.MediumRooms);
      this.ShuffleRooms(this.MediumRooms);
      this.ShuffleRooms(this.LargeRooms);
      this.ShuffleRooms(this.LargeRooms);
      this.ShuffleRooms(this.LargeRooms);
      this.ShuffleRooms(this.LargeRooms);
      this.ConfigureStartingRoom(this.LargeRooms[this.m_nextLargeRoomToConfigure]);
      ++this.m_nextLargeRoomToConfigure;
      float num1 = UnityEngine.Random.Range(0.0f, 1f);
      int i1 = 0;
      if ((double) num1 > 0.5)
        ++i1;
      int index1;
      int index2;
      int index3;
      switch (UnityEngine.Random.Range(0, 4))
      {
        case 0:
          index1 = 0;
          index2 = 1;
          index3 = 2;
          break;
        case 1:
          index1 = 1;
          index2 = 0;
          index3 = 2;
          break;
        case 2:
          index1 = 2;
          index2 = 1;
          index3 = 0;
          break;
        default:
          index1 = 1;
          index2 = 2;
          index3 = 0;
          break;
      }
      GameObject trig1 = UnityEngine.Object.Instantiate<GameObject>(this.MeatRoomPrefabs[index1], this.SmallRooms[this.m_nextSmallRoomToConfigure].transform.position, Quaternion.identity);
      this.SmallRooms[this.m_nextSmallRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Meat);
      this.SmallRooms[this.m_nextSmallRoomToConfigure].SetTriggerable(trig1);
      trig1.GetComponent<IMeatRoomAble>().SetMeatID(i1);
      int i2 = i1 + 1;
      GameObject trig2 = UnityEngine.Object.Instantiate<GameObject>(this.MeatRoomPrefabs[index2], this.MediumRooms[this.m_nextMediumRoomToConfigure].transform.position, Quaternion.identity);
      this.MediumRooms[this.m_nextMediumRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Meat);
      this.MediumRooms[this.m_nextMediumRoomToConfigure].SetTriggerable(trig2);
      trig2.GetComponent<IMeatRoomAble>().SetMeatID(i2);
      int i3 = i2 + 1;
      GameObject trig3 = UnityEngine.Object.Instantiate<GameObject>(this.MeatRoomPrefabs[index3], this.LargeRooms[this.m_nextLargeRoomToConfigure].transform.position, Quaternion.identity);
      this.LargeRooms[this.m_nextLargeRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Meat);
      this.LargeRooms[this.m_nextLargeRoomToConfigure].SetTriggerable(trig3);
      trig3.GetComponent<IMeatRoomAble>().SetMeatID(i3);
      float num2 = UnityEngine.Random.Range(0.0f, 1f);
      if ((double) num2 > 0.75)
      {
        this.MeatRoom1 = this.SmallRooms[this.m_nextSmallRoomToConfigure];
        this.MeatRoom2 = this.MediumRooms[this.m_nextMediumRoomToConfigure];
        this.MeatRoom3 = this.LargeRooms[this.m_nextLargeRoomToConfigure];
      }
      else if ((double) num2 > 0.5)
      {
        this.MeatRoom2 = this.SmallRooms[this.m_nextSmallRoomToConfigure];
        this.MeatRoom3 = this.MediumRooms[this.m_nextMediumRoomToConfigure];
        this.MeatRoom1 = this.LargeRooms[this.m_nextLargeRoomToConfigure];
      }
      else if ((double) num2 > 0.25)
      {
        this.MeatRoom3 = this.SmallRooms[this.m_nextSmallRoomToConfigure];
        this.MeatRoom1 = this.MediumRooms[this.m_nextMediumRoomToConfigure];
        this.MeatRoom2 = this.LargeRooms[this.m_nextLargeRoomToConfigure];
      }
      else
      {
        this.MeatRoom2 = this.SmallRooms[this.m_nextSmallRoomToConfigure];
        this.MeatRoom1 = this.MediumRooms[this.m_nextMediumRoomToConfigure];
        this.MeatRoom3 = this.LargeRooms[this.m_nextLargeRoomToConfigure];
      }
      this.MeatRoom2.CloseDoors(false);
      this.MeatRoom3.CloseDoors(false);
      ++this.m_nextSmallRoomToConfigure;
      ++this.m_nextMediumRoomToConfigure;
      ++this.m_nextLargeRoomToConfigure;
      GameObject trig4 = UnityEngine.Object.Instantiate<GameObject>(this.TrapRoomPrefabs[index1], this.SmallRooms[this.m_nextSmallRoomToConfigure].transform.position, Quaternion.identity);
      this.SmallRooms[this.m_nextSmallRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Trap);
      this.SmallRooms[this.m_nextSmallRoomToConfigure].SetTriggerable(trig4);
      trig4.GetComponent<IRoomTriggerable>().SetRoom(this.SmallRooms[this.m_nextSmallRoomToConfigure]);
      GameObject trig5 = UnityEngine.Object.Instantiate<GameObject>(this.TrapRoomPrefabs[index2], this.MediumRooms[this.m_nextMediumRoomToConfigure].transform.position, Quaternion.identity);
      this.MediumRooms[this.m_nextMediumRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Trap);
      this.MediumRooms[this.m_nextMediumRoomToConfigure].SetTriggerable(trig5);
      trig5.GetComponent<IRoomTriggerable>().SetRoom(this.MediumRooms[this.m_nextMediumRoomToConfigure]);
      GameObject trig6 = UnityEngine.Object.Instantiate<GameObject>(this.TrapRoomPrefabs[index3], this.LargeRooms[this.m_nextLargeRoomToConfigure].transform.position, Quaternion.identity);
      this.LargeRooms[this.m_nextLargeRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Trap);
      this.LargeRooms[this.m_nextLargeRoomToConfigure].SetTriggerable(trig6);
      trig6.GetComponent<IRoomTriggerable>().SetRoom(this.LargeRooms[this.m_nextLargeRoomToConfigure]);
      ++this.m_nextSmallRoomToConfigure;
      ++this.m_nextMediumRoomToConfigure;
      ++this.m_nextLargeRoomToConfigure;
      GameObject trig7 = UnityEngine.Object.Instantiate<GameObject>(this.LottoRoomPrefabs[UnityEngine.Random.Range(0, this.LottoRoomPrefabs.Length)], this.SmallRooms[this.m_nextSmallRoomToConfigure].transform.position, Quaternion.identity);
      this.SmallRooms[this.m_nextSmallRoomToConfigure].SetRoomType(RedRoom.RedRoomType.MonsterCloset);
      this.SmallRooms[this.m_nextSmallRoomToConfigure].SetTriggerable(trig7);
      trig7.GetComponent<IRoomTriggerable>().SetRoom(this.SmallRooms[this.m_nextSmallRoomToConfigure]);
      GameObject trig8 = UnityEngine.Object.Instantiate<GameObject>(this.LottoRoomPrefabs[UnityEngine.Random.Range(0, this.LottoRoomPrefabs.Length)], this.MediumRooms[this.m_nextMediumRoomToConfigure].transform.position, Quaternion.identity);
      this.MediumRooms[this.m_nextMediumRoomToConfigure].SetRoomType(RedRoom.RedRoomType.MonsterCloset);
      this.MediumRooms[this.m_nextMediumRoomToConfigure].SetTriggerable(trig8);
      trig8.GetComponent<IRoomTriggerable>().SetRoom(this.MediumRooms[this.m_nextMediumRoomToConfigure]);
      GameObject trig9 = UnityEngine.Object.Instantiate<GameObject>(this.LottoRoomPrefabs[UnityEngine.Random.Range(0, this.LottoRoomPrefabs.Length)], this.LargeRooms[this.m_nextLargeRoomToConfigure].transform.position, Quaternion.identity);
      this.LargeRooms[this.m_nextLargeRoomToConfigure].SetRoomType(RedRoom.RedRoomType.MonsterCloset);
      this.LargeRooms[this.m_nextLargeRoomToConfigure].SetTriggerable(trig9);
      trig9.GetComponent<IRoomTriggerable>().SetRoom(this.LargeRooms[this.m_nextLargeRoomToConfigure]);
      ++this.m_nextSmallRoomToConfigure;
      ++this.m_nextMediumRoomToConfigure;
      ++this.m_nextLargeRoomToConfigure;
      GameObject trig10 = UnityEngine.Object.Instantiate<GameObject>(this.LottoRoomPrefabs[UnityEngine.Random.Range(0, this.LottoRoomPrefabs.Length)], this.MediumRooms[this.m_nextMediumRoomToConfigure].transform.position, Quaternion.identity);
      this.MediumRooms[this.m_nextMediumRoomToConfigure].SetRoomType(RedRoom.RedRoomType.MonsterCloset);
      this.MediumRooms[this.m_nextMediumRoomToConfigure].SetTriggerable(trig10);
      trig10.GetComponent<IRoomTriggerable>().SetRoom(this.MediumRooms[this.m_nextMediumRoomToConfigure]);
      GameObject trig11 = UnityEngine.Object.Instantiate<GameObject>(this.LottoRoomPrefabs[UnityEngine.Random.Range(0, this.LottoRoomPrefabs.Length)], this.SmallRooms[this.m_nextSmallRoomToConfigure].transform.position, Quaternion.identity);
      this.SmallRooms[this.m_nextSmallRoomToConfigure].SetRoomType(RedRoom.RedRoomType.MonsterCloset);
      this.SmallRooms[this.m_nextSmallRoomToConfigure].SetTriggerable(trig11);
      trig7.GetComponent<IRoomTriggerable>().SetRoom(this.SmallRooms[this.m_nextSmallRoomToConfigure]);
      ++this.m_nextSmallRoomToConfigure;
      ++this.m_nextMediumRoomToConfigure;
    }

    private void ConfigureStartingRoom(RedRoom room)
    {
      GameObject trig = UnityEngine.Object.Instantiate<GameObject>(this.StartingRoomPrefab, room.transform.position, room.transform.rotation);
      this.m_startingRoom = trig.GetComponent<MG_StartingRoom>();
      room.SetRoomType(RedRoom.RedRoomType.Starting);
      room.SetTriggerable(trig);
      this.PlayerRig.transform.position = this.m_startingRoom.PlayerStartPos.position;
      this.SceneSettings.DeathResetPoint.position = this.m_startingRoom.PlayerStartPos.position;
      int num = 0;
      while (num < this.m_startingRoom.Spawn_CardboardBox.Length)
        ++num;
      if (GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.BuildYourOwnMeat || GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.KidsMeatyMeal)
        UnityEngine.Object.Instantiate<GameObject>(this.ItemSpawnerPrefab, this.m_startingRoom.Spawn_ItemSpawner.position, this.m_startingRoom.Spawn_ItemSpawner.rotation);
      this.m_objectsThatHaveBeenHeld.Add((FVRInteractiveObject) UnityEngine.Object.Instantiate<GameObject>(this.LT_Melee.GetRandomObject().GetGameObject(), this.m_startingRoom.Spawn_Melee.position, this.m_startingRoom.Spawn_Melee.rotation).GetComponent<FVRPhysicalObject>());
      UnityEngine.Object.Instantiate<GameObject>(this.LTEntry_Handgun1.GetGameObject(), this.m_startingRoom.Spawn_StartingPistol.position, this.m_startingRoom.Spawn_StartingPistol.rotation);
      UnityEngine.Object.Instantiate<GameObject>(this.LTEntry_Shotgun1.GetGameObject(), this.m_startingRoom.Spawn_StartingShotgun.position, this.m_startingRoom.Spawn_StartingShotgun.rotation);
      this.SpawnAmmoAtPlaceForGun(this.LTEntry_Handgun1, this.m_startingRoom.Spawn_StartingPistolMag.position, this.m_startingRoom.Spawn_StartingPistolMag.rotation);
      this.SpawnAmmoAtPlaceForGun(this.LTEntry_Shotgun1, this.m_startingRoom.Spawn_StartingShotgunRounds.position, this.m_startingRoom.Spawn_StartingShotgunRounds.rotation);
      this.SpawnLight(this.m_startingRoom.Spawn_FlashLight.position, this.m_startingRoom.Spawn_FlashLight.rotation, false, GM.Options.MeatGrinderFlags.PrimaryLight);
      this.SpawnLight(this.m_startingRoom.Spawn_AmbientLight.position, this.m_startingRoom.Spawn_AmbientLight.rotation, true, GM.Options.MeatGrinderFlags.SecondaryLight);
    }

    public GameObject SpawnLight(
      Vector3 pos,
      Quaternion rot,
      bool isSecondary,
      MeatGrinderFlags.LightSourceOption sourceOption)
    {
      GameObject gameObject = (GameObject) null;
      MeatGrinderFlags.LightSourceOption lightSourceOption = MeatGrinderFlags.LightSourceOption.FlashLight;
      if (sourceOption == MeatGrinderFlags.LightSourceOption.Random)
      {
        float num = UnityEngine.Random.Range(0.0f, 1f);
        if (isSecondary)
          num -= 0.15f;
        if ((double) num > 0.75)
          lightSourceOption = MeatGrinderFlags.LightSourceOption.FlashLight;
        else if ((double) num > 0.5)
          lightSourceOption = MeatGrinderFlags.LightSourceOption.GlowStick;
        else if ((double) num > 0.25)
          lightSourceOption = MeatGrinderFlags.LightSourceOption.Lighter;
        else if ((double) num >= 0.0)
          lightSourceOption = MeatGrinderFlags.LightSourceOption.BoxOfMatches;
      }
      else
        lightSourceOption = sourceOption;
      switch (lightSourceOption)
      {
        case MeatGrinderFlags.LightSourceOption.FlashLight:
          if (!isSecondary)
          {
            gameObject = UnityEngine.Object.Instantiate<GameObject>(this.FlashLightPrefab.GetGameObject(), pos, rot);
            gameObject.GetComponent<Flashlight>().ToggleOn();
            break;
          }
          gameObject = UnityEngine.Object.Instantiate<GameObject>(this.TacticalFlashlightPrefab.GetGameObject(), pos, rot);
          break;
        case MeatGrinderFlags.LightSourceOption.Lighter:
          gameObject = UnityEngine.Object.Instantiate<GameObject>(this.FlipzoPrefab.GetGameObject(), pos, rot);
          break;
        case MeatGrinderFlags.LightSourceOption.GlowStick:
          gameObject = UnityEngine.Object.Instantiate<GameObject>(this.GlowStickPrefab.GetGameObject(), pos, rot);
          break;
        case MeatGrinderFlags.LightSourceOption.BoxOfMatches:
          gameObject = UnityEngine.Object.Instantiate<GameObject>(this.BoxOfMatchesPrefab.GetGameObject(), pos, rot);
          break;
      }
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null && isSecondary)
        this.m_hasSpawnedSeconaryLight = true;
      return gameObject;
    }

    private void SpawnItems()
    {
      for (int index = 0; index < this.PossibleCabinetSpawns.Length; ++index)
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) this.CabinetSpawnChance)
        {
          MG_Cabinet component = UnityEngine.Object.Instantiate<GameObject>(this.CabinetPrefabs[UnityEngine.Random.Range(0, this.CabinetPrefabs.Length)], this.PossibleCabinetSpawns[index].position, this.PossibleCabinetSpawns[index].rotation).GetComponent<MG_Cabinet>();
          component.Init();
          this.ToSpawnCabinets.Add(component);
        }
      }
      for (int index = 0; index < this.PossibleShelfSpawns.Length; ++index)
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) this.ShelfSpawnChance)
          this.ToSpawnShelves.Add(UnityEngine.Object.Instantiate<GameObject>(this.MetalShelfPrefabs[UnityEngine.Random.Range(0, this.MetalShelfPrefabs.Length)], this.PossibleShelfSpawns[index].position, this.PossibleShelfSpawns[index].rotation).GetComponent<MH_MetalShelf>());
      }
      for (int index = 0; index < this.PossibleIndustrialShelfShortSpawns.Length; ++index)
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) this.IndustrialShelfChance)
          UnityEngine.Object.Instantiate<GameObject>(this.IndustrialShelfPrefabShort, this.PossibleIndustrialShelfShortSpawns[index].position, this.PossibleIndustrialShelfShortSpawns[index].rotation).GetComponent<MG_IndustrialShelf>().Init();
      }
      for (int index = 0; index < this.PossibleIndustrialShelfLongSpawns.Length; ++index)
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) this.IndustrialShelfChance)
          UnityEngine.Object.Instantiate<GameObject>(this.IndustrialShelfPrefabLong, this.PossibleIndustrialShelfLongSpawns[index].position, this.PossibleIndustrialShelfLongSpawns[index].rotation).GetComponent<MG_IndustrialShelf>().Init();
      }
      for (int index = 0; index < this.PossibleSmashyStoolSpawns.Length; ++index)
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) this.SmashyStoolChance)
          UnityEngine.Object.Instantiate<GameObject>(this.SmashyStoolPrefabs[UnityEngine.Random.Range(0, this.SmashyStoolPrefabs.Length)], this.PossibleSmashyStoolSpawns[index].position, this.PossibleSmashyStoolSpawns[index].rotation);
      }
      for (int index = 0; index < this.PossibleMeatPileSpawns.Length; ++index)
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) this.MeatPileChance)
          UnityEngine.Object.Instantiate<GameObject>(this.MeatPilePrefabs[UnityEngine.Random.Range(0, this.MeatPilePrefabs.Length)], this.PossibleMeatPileSpawns[index].position, this.PossibleMeatPileSpawns[index].rotation).GetComponent<MG_DestroyableWithSpawn>().SetMGMaster(this);
      }
      for (int index = 0; index < this.CookedPossibleMeatPileSpawns.Length; ++index)
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) this.CookedMeatPileChance)
          UnityEngine.Object.Instantiate<GameObject>(this.CookedMeatPilePrefabs[UnityEngine.Random.Range(0, this.CookedMeatPilePrefabs.Length)], this.CookedPossibleMeatPileSpawns[index].position, this.CookedPossibleMeatPileSpawns[index].rotation).GetComponent<MG_DestroyableWithSpawn>().SetMGMaster(this);
      }
      for (int index = 0; index < this.LaserMinesSpawnPoints.Length; ++index)
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) this.LaserMineChance)
          UnityEngine.Object.Instantiate<GameObject>(this.LaserMinePrefab, this.LaserMinesSpawnPoints[index].position, this.LaserMinesSpawnPoints[index].rotation);
      }
      for (int index = 0; index < this.ToSpawnCabinets.Count; ++index)
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) > 0.400000005960464)
        {
          float num = UnityEngine.Random.Range(0.0f, 1f);
          if ((double) num > 0.970000028610229)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_SuperRareGun1, this.ToSpawnCabinets[index].GetRandomSpawnTransform());
          else if ((double) num > 0.899999976158142)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_RareGun1, this.ToSpawnCabinets[index].GetRandomSpawnTransform());
          else if ((double) num > 0.800000011920929)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_RareGun2, this.ToSpawnCabinets[index].GetRandomSpawnTransform());
          else if ((double) num > 0.600000023841858)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_Shotgun2, this.ToSpawnCabinets[index].GetRandomSpawnTransform());
          else if ((double) num > 0.449999988079071)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_Shotgun1, this.ToSpawnCabinets[index].GetRandomSpawnTransform());
          else if ((double) num > 0.300000011920929)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_Handgun3, this.ToSpawnCabinets[index].GetRandomSpawnTransform());
          else if ((double) num > 0.200000002980232)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_Handgun2, this.ToSpawnCabinets[index].GetRandomSpawnTransform());
          else if ((double) num > 0.100000001490116)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_Handgun1, this.ToSpawnCabinets[index].GetRandomSpawnTransform());
          else
            this.SpawnObjectAtPlace(this.LT_Attachments.GetRandomObject(), this.ToSpawnCabinets[index].GetRandomSpawnTransform());
        }
      }
      for (int index = 0; index < this.ToSpawnShelves.Count; ++index)
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) > 0.100000001490116)
        {
          float num = UnityEngine.Random.Range(0.0f, 1f);
          if ((double) num > 0.949999988079071)
            this.SpawnGunAmmoPairToTransformList(this.LTEntry_RareGun1, this.ToSpawnShelves[index].SpawnPositions);
          else if ((double) num > 0.899999976158142)
            this.SpawnGunAmmoPairToTransformList(this.LTEntry_RareGun2, this.ToSpawnShelves[index].SpawnPositions);
          else if ((double) num > 0.850000023841858)
            this.SpawnGunAmmoPairToTransformList(this.LTEntry_Shotgun2, this.ToSpawnShelves[index].SpawnPositions);
          else if ((double) num > 0.800000011920929)
            this.SpawnGunAmmoPairToTransformList(this.LTEntry_Handgun2, this.ToSpawnShelves[index].SpawnPositions);
          else if ((double) num > 0.75)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_Handgun1, this.ToSpawnShelves[index].SpawnPositions[0]);
          else if ((double) num > 0.699999988079071)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_Handgun2, this.ToSpawnShelves[index].SpawnPositions[0]);
          else if ((double) num > 0.649999976158142)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_Shotgun1, this.ToSpawnShelves[index].SpawnPositions[0]);
          else if ((double) num > 0.600000023841858)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_Shotgun2, this.ToSpawnShelves[index].SpawnPositions[0]);
          else if ((double) num > 0.550000011920929)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_RareGun1, this.ToSpawnShelves[index].SpawnPositions[0]);
          else if ((double) num > 0.5)
            this.SpawnAmmoAtPlaceForGun(this.LTEntry_RareGun2, this.ToSpawnShelves[index].SpawnPositions[0]);
          else if ((double) num > 0.300000011920929)
            this.SpawnObjectAtPlace(this.LT_Melee.GetRandomObject(), this.ToSpawnShelves[index].SpawnPositions[0]);
          else if ((double) num > 0.100000001490116)
            this.SpawnObjectAtPlace(this.LT_Attachments.GetRandomObject(), this.ToSpawnShelves[index].SpawnPositions[0]);
          else
            this.SpawnObjectAtPlace(this.LT_Junk.GetRandomObject(), this.ToSpawnShelves[index].SpawnPositions[0]);
        }
      }
    }

    private void CheckHandContents()
    {
      for (int index = 0; index < this.hands.Length; ++index)
      {
        if (this.hands[index].CurrentInteractable is FVRPhysicalObject && !this.m_objectsThatHaveBeenHeld.Contains(this.hands[index].CurrentInteractable))
        {
          this.m_objectsThatHaveBeenHeld.Add(this.hands[index].CurrentInteractable);
          if (this.hands[index].CurrentInteractable is FVRFireArm)
            GM.MGMaster.Narrator.PlayFoundRareItem();
          else if (this.hands[index].CurrentInteractable is FVRFireArm)
            GM.MGMaster.Narrator.PlayFoundNormalItem();
          else if (this.hands[index].CurrentInteractable is FVRMeleeWeapon)
            GM.MGMaster.Narrator.PlayFoundNormalItem();
          else if (this.hands[index].CurrentInteractable is FVRFireArmAttachment)
            GM.MGMaster.Narrator.PlayFoundSpecialItem();
          else if (this.hands[index].CurrentInteractable is AnimalNoiseMaker || this.hands[index].CurrentInteractable is SodaCan)
            GM.MGMaster.Narrator.PlayFoundJunkItem();
        }
      }
    }

    private void ShuffleRooms(RedRoom[] rooms)
    {
      for (int min = 0; min < rooms.Length; ++min)
      {
        RedRoom room = rooms[min];
        int index = UnityEngine.Random.Range(min, rooms.Length);
        rooms[min] = rooms[index];
        rooms[index] = room;
      }
    }

    private void CheckTime()
    {
      if (this.IsCountingDown && GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.Classic)
        this.m_TimeLeft -= Time.deltaTime * 0.95f;
      for (int i = 0; i < this.TimeThresholds.Length; ++i)
      {
        if ((double) this.m_TimeLeft <= (double) this.TimeThresholds[i] && !this.TimeThresholdPassed[i])
        {
          this.TimeThresholdPassed[i] = true;
          GM.MGMaster.Narrator.PlayTimeWarning(i);
        }
      }
      this.m_PlayerHealth = GM.GetPlayerHealth();
      if ((double) this.m_PlayerHealth < 0.25 && !this.hasWarnedPlayerHealth1)
      {
        this.hasWarnedPlayerHealth1 = true;
        GM.MGMaster.Narrator.PlayAboutToDie();
      }
      if ((double) this.m_TimeLeft < -3.0 && !this.IsDead)
      {
        this.IsDead = true;
        GM.CurrentPlayerBody.Hitboxes[0].Damage(12f);
        GM.MGMaster.Narrator.PlayDiedOutOfHealth();
      }
      if ((double) this.m_PlayerHealth > 0.0 || this.IsDead)
        return;
      GM.MGMaster.Narrator.PlayDiedOutOfHealth();
      this.IsDead = true;
    }

    [Serializable]
    public class EventAI
    {
      public MeatGrinderMaster Master;
      private HashSet<MeatGrinderMaster.EventAI.EventType> EventsTriggered = new HashSet<MeatGrinderMaster.EventAI.EventType>();
      private MeatGrinderMaster.EventAI.EventType m_lastEvent;
      public MG_EventAIConfig Config;
      private float m_EventTick = 90f;
      public Transform[] PossibleEventSpawnPoints;
      private int m_pointIndexToCheck;
      private Transform m_current_BackPoint;
      private Transform m_current_SidePoint;
      private Transform m_current_FrontPoint;
      private List<GameObject> RemoveIfDistant = new List<GameObject>();
      private List<FVRFireArmMagazine> SpawnedMags = new List<FVRFireArmMagazine>();
      private List<GameObject> SpawnedAgents = new List<GameObject>();
      [Header("Special Spawn Entries")]
      public GameObject Puff;
      public FVRObject Obj_Minigun;
      public FVRObject Obj_MinigunMag;
      public FVRObject Obj_Light50;
      public FVRObject Obj_Light50Mag;
      public FVRObject Obj_ChainSaw;
      public GameObject Present;
      public GameObject Hazard_Weiner;
      [Header("AudioEvents")]
      public AudioSource EventAudioSource;
      public AudioClip[] EventAudioClips;

      public void Init()
      {
        if (!GM.Options.MeatGrinderFlags.HasNarratorDoneLongIntro)
          return;
        this.m_EventTick = UnityEngine.Random.Range(30f, 40f);
      }

      public void Tick()
      {
        this.PointChecking();
        this.m_EventTick -= Time.deltaTime;
        if ((double) this.m_EventTick <= 0.0)
          this.EventTime();
        this.RemoveCheck();
      }

      private void RemoveCheck()
      {
        for (int index = this.SpawnedMags.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) this.SpawnedMags[index] == (UnityEngine.Object) null)
            this.SpawnedMags.RemoveAt(index);
          else if (!this.SpawnedMags[index].IsIntegrated && this.SpawnedMags[index].m_numRounds == 0 && (!this.SpawnedMags[index].IsHeld && (UnityEngine.Object) this.SpawnedMags[index].QuickbeltSlot == (UnityEngine.Object) null) && ((UnityEngine.Object) this.SpawnedMags[index].FireArm == (UnityEngine.Object) null && this.SpawnedMags[index].RootRigidbody.IsSleeping()))
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.SpawnedMags[index].gameObject);
            this.SpawnedMags.RemoveAt(index);
          }
        }
        for (int index = this.SpawnedAgents.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) this.SpawnedAgents[index] == (UnityEngine.Object) null)
            this.SpawnedAgents.RemoveAt(index);
        }
        for (int index = this.RemoveIfDistant.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) this.RemoveIfDistant[index] != (UnityEngine.Object) null)
          {
            if ((double) Vector3.Distance(this.RemoveIfDistant[index].transform.position, GM.CurrentPlayerBody.transform.position) > 20.0)
              UnityEngine.Object.Destroy((UnityEngine.Object) this.RemoveIfDistant[index]);
            this.RemoveIfDistant.RemoveAt(index);
          }
          else
            this.RemoveIfDistant.RemoveAt(index);
        }
      }

      private void PointChecking()
      {
        if ((UnityEngine.Object) GM.CurrentPlayerBody == (UnityEngine.Object) null)
          return;
        Transform possibleEventSpawnPoint = this.PossibleEventSpawnPoints[this.m_pointIndexToCheck];
        Vector3 b = new Vector3(possibleEventSpawnPoint.position.x, 0.0f, possibleEventSpawnPoint.position.z);
        Vector3 a = new Vector3(GM.CurrentPlayerBody.Head.position.x, 0.0f, GM.CurrentPlayerBody.Head.position.z);
        Vector3 forward = GM.CurrentPlayerBody.Head.forward;
        forward.y = 0.0f;
        forward.Normalize();
        float num1 = Vector3.Angle(b - a, forward);
        float num2 = Vector3.Distance(a, b);
        if ((double) num2 > 3.0 && (double) num2 < 10.0)
        {
          if ((double) num1 < 45.0)
            this.m_current_FrontPoint = possibleEventSpawnPoint;
          else if ((double) num1 < 135.0)
            this.m_current_SidePoint = possibleEventSpawnPoint;
          else
            this.m_current_BackPoint = possibleEventSpawnPoint;
        }
        ++this.m_pointIndexToCheck;
        if (this.m_pointIndexToCheck < this.PossibleEventSpawnPoints.Length)
          return;
        this.m_pointIndexToCheck = 0;
      }

      private bool isPointValid(Transform point, MeatGrinderMaster.EventAI.PointCheck check)
      {
        if ((UnityEngine.Object) point == (UnityEngine.Object) null)
          return false;
        Vector3 b = new Vector3(point.position.x, 0.0f, point.position.z);
        Vector3 a = new Vector3(GM.CurrentPlayerBody.Head.position.x, 0.0f, GM.CurrentPlayerBody.Head.position.z);
        Vector3 forward = GM.CurrentPlayerBody.Head.forward;
        forward.y = 0.0f;
        forward.Normalize();
        float num1 = Vector3.Angle(b - a, forward);
        float num2 = Vector3.Distance(a, b);
        if ((double) num2 < 3.0 || (double) num2 > 10.0)
          return false;
        switch (check)
        {
          case MeatGrinderMaster.EventAI.PointCheck.Front:
            if ((double) num1 > 45.0)
              return false;
            break;
          case MeatGrinderMaster.EventAI.PointCheck.Side:
            if ((double) num1 < 45.0 || (double) num1 > 135.0)
              return false;
            break;
          case MeatGrinderMaster.EventAI.PointCheck.Rear:
            if ((double) num1 < 135.0)
              return false;
            break;
        }
        return true;
      }

      private bool isFacingGood(MeatGrinderMaster.EventAI.PointCheck check)
      {
        switch (check)
        {
          case MeatGrinderMaster.EventAI.PointCheck.Front:
            return this.isPointValid(this.m_current_FrontPoint, MeatGrinderMaster.EventAI.PointCheck.Front);
          case MeatGrinderMaster.EventAI.PointCheck.Side:
            return this.isPointValid(this.m_current_SidePoint, MeatGrinderMaster.EventAI.PointCheck.Side);
          case MeatGrinderMaster.EventAI.PointCheck.Rear:
            return this.isPointValid(this.m_current_BackPoint, MeatGrinderMaster.EventAI.PointCheck.Rear);
          default:
            return false;
        }
      }

      private void EventTime()
      {
        MeatGrinderMaster.EventAI.MGEvent weightedRandomEntry = this.Config.GetWeightedRandomEntry();
        if (weightedRandomEntry.IsEventExclusive && this.EventsTriggered.Contains(weightedRandomEntry.Type) || (weightedRandomEntry.Type == this.m_lastEvent || this.Master.Narrator.AUD.isPlaying))
          return;
        switch (weightedRandomEntry.Type)
        {
          case MeatGrinderMaster.EventAI.EventType.eSpawn_Ammo:
            if (!this.SpawnAmmo())
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawn_Chainsaw:
            if (!this.SpawnGun(this.Obj_ChainSaw))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawn_AntiMaterialRifle:
            if (!this.SpawnGun(this.Obj_Light50, this.Obj_Light50Mag))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawn_Minigun:
            if (!this.SpawnGun(this.Obj_Minigun, this.Obj_MinigunMag))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawn_Present:
            if (!this.SpawnGun(this.Obj_ChainSaw))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eTrigger_Sound:
            if (!this.SoundFlashEventRandom(true, false))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eTrigger_Flash:
            if (!this.SoundFlashEventRandom(false, true))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eTrigger_SoundFlash:
            if (!this.SoundFlashEventRandom(true, true))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawnHazard_ExplodingWeiner:
            if (!this.SpawnSplodingWeiners())
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawnHazard_GiantSawBlade:
            return;
          case MeatGrinderMaster.EventAI.EventType.eSpawnAgent_FlamingMeatball:
            if (!this.SpawnFlamingMeatball())
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawnAgent_KetchupBot:
            if (!this.SpawnBot(this.Master.FlameShotgunBotPrefabs[1], true, true))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawnAgent_MustardBot:
            if (!this.SpawnBot(this.Master.FlameShotgunBotPrefabs[2], true, true))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawnAgent_ShotgunBot:
            if (!this.SpawnBot(this.Master.FlameShotgunBotPrefabs[0], true, true))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawnAgent_KneumaticBot:
            if (!this.SpawnBot(this.Master.HydraulicBotPrefabs[0], true, true))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawnAgent_DrillWeiners:
            if (!this.SpawnBot(this.Master.FlyingHotDogSwarmPrefab, false, false))
              return;
            break;
          case MeatGrinderMaster.EventAI.EventType.eSpawnAgent_LetterM:
            return;
          case MeatGrinderMaster.EventAI.EventType.eSpawnAgent_ScreamingJerry:
            if (this.SpawnBot(this.Master.ScreamingJerry, false, false))
              ;
            return;
          case MeatGrinderMaster.EventAI.EventType.eTrigger_Goober:
            return;
        }
        this.m_EventTick = UnityEngine.Random.Range(weightedRandomEntry.TickRange.x, weightedRandomEntry.TickRange.y);
        this.EventsTriggered.Add(weightedRandomEntry.Type);
        this.m_lastEvent = weightedRandomEntry.Type;
      }

      private void SpawnPuff(Vector3 pos) => UnityEngine.Object.Instantiate<GameObject>(this.Puff, pos, UnityEngine.Random.rotation);

      private bool SpawnBot(GameObject g, bool addToremovelist, bool NeedsNav)
      {
        if (this.SpawnedAgents.Count > 6 || !this.isFacingGood(MeatGrinderMaster.EventAI.PointCheck.Side))
          return false;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(g, this.m_current_SidePoint.position, Quaternion.LookRotation(new Vector3(UnityEngine.Random.Range(1f, -1f), 0.0f, UnityEngine.Random.Range(1f, -1f)), Vector3.up));
        if (addToremovelist)
          this.RemoveIfDistant.Add(gameObject);
        this.SpawnedAgents.Add(gameObject);
        if (NeedsNav)
        {
          GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
          wwBotWurst component = gameObject.GetComponent<wwBotWurst>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          {
            if ((double) this.m_current_SidePoint.position.z > 0.0)
            {
              Vector3 position = this.m_current_SidePoint.position;
              component.NavPointGroup = (double) position.x <= 0.0 ? this.Master.FlameShotgunNavGroup_Office : this.Master.FlameShotgunNavGroup_Boiler;
            }
            else
            {
              Vector3 position = this.m_current_SidePoint.position;
              component.NavPointGroup = (double) position.x <= 0.0 ? this.Master.FlameShotgunNavGroup_Restaraunt : this.Master.FlameShotgunNavGroup_Freezer;
            }
          }
        }
        return true;
      }

      private bool SpawnFlamingMeatball()
      {
        if (!this.isFacingGood(MeatGrinderMaster.EventAI.PointCheck.Rear))
          return false;
        UnityEngine.Object.Instantiate<GameObject>(this.Master.FlamingMeatball, this.m_current_BackPoint.position + Vector3.up, UnityEngine.Random.rotation);
        return true;
      }

      private bool SpawnSplodingWeiners()
      {
        if (!this.isFacingGood(MeatGrinderMaster.EventAI.PointCheck.Side))
          return false;
        UnityEngine.Object.Instantiate<GameObject>(this.Hazard_Weiner, this.m_current_SidePoint.position + Vector3.up * 1.5f, UnityEngine.Random.rotation);
        return true;
      }

      private bool SpawnAmmo() => this.SpawnedMags.Count <= 6 && this.isPointValid(this.m_current_FrontPoint, MeatGrinderMaster.EventAI.PointCheck.Front);

      private bool SpawnGun(MGItemSpawnChartEntry entry)
      {
        if (!this.isPointValid(this.m_current_FrontPoint, MeatGrinderMaster.EventAI.PointCheck.Front))
          return false;
        Vector3 position = this.m_current_FrontPoint.position + Vector3.up * 0.7f;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(entry.Obj.GetGameObject(), position, UnityEngine.Random.rotation);
        FVRFireArm component = gameObject.GetComponent<FVRFireArm>();
        UnityEngine.Object.Instantiate<GameObject>(entry.Mag.GetGameObject(), component.GetMagMountPos(false).position, component.GetMagMountPos(false).rotation).GetComponent<FVRFireArmMagazine>().Load(component);
        this.RemoveIfDistant.Add(gameObject);
        return true;
      }

      private bool SpawnGun(FVRObject Gun, FVRObject Mag)
      {
        if (!this.isPointValid(this.m_current_FrontPoint, MeatGrinderMaster.EventAI.PointCheck.Front))
          return false;
        Vector3 position = this.m_current_FrontPoint.position + Vector3.up * 0.7f;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Gun.GetGameObject(), position, UnityEngine.Random.rotation);
        FVRFireArm component = gameObject.GetComponent<FVRFireArm>();
        UnityEngine.Object.Instantiate<GameObject>(Mag.GetGameObject(), component.GetMagMountPos(false).position, component.GetMagMountPos(false).rotation).GetComponent<FVRFireArmMagazine>().Load(component);
        this.RemoveIfDistant.Add(gameObject);
        return true;
      }

      private bool SpawnGun(FVRObject Gun)
      {
        if (!this.isPointValid(this.m_current_FrontPoint, MeatGrinderMaster.EventAI.PointCheck.Front))
          return false;
        Vector3 position = this.m_current_FrontPoint.position + Vector3.up * 0.7f;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Gun.GetGameObject(), position, UnityEngine.Random.rotation);
        gameObject.GetComponent<FVRFireArm>();
        this.RemoveIfDistant.Add(gameObject);
        return true;
      }

      private bool SpawnGun(GameObject Gun)
      {
        if (!this.isPointValid(this.m_current_FrontPoint, MeatGrinderMaster.EventAI.PointCheck.Front))
          return false;
        Vector3 position = this.m_current_FrontPoint.position + Vector3.up * 0.7f;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Gun, position, UnityEngine.Random.rotation);
        gameObject.GetComponent<FVRFireArm>();
        this.RemoveIfDistant.Add(gameObject);
        return true;
      }

      private bool SoundFlashEventRandom(bool Sound, bool Flash)
      {
        float num = UnityEngine.Random.Range(0.0f, 1f);
        if ((double) num > 0.899999976158142)
          return this.SoundFlashEvent(Sound, Flash, MeatGrinderMaster.EventAI.PointCheck.Front);
        return (double) num > 0.5 ? this.SoundFlashEvent(Sound, Flash, MeatGrinderMaster.EventAI.PointCheck.Side) : this.SoundFlashEvent(Sound, Flash, MeatGrinderMaster.EventAI.PointCheck.Rear);
      }

      private bool SoundFlashEvent(
        bool Sound,
        bool Flash,
        MeatGrinderMaster.EventAI.PointCheck side)
      {
        if (!this.isFacingGood(side))
          return false;
        Vector3 pos = Vector3.zero;
        switch (side)
        {
          case MeatGrinderMaster.EventAI.PointCheck.Front:
            pos = this.m_current_FrontPoint.position;
            break;
          case MeatGrinderMaster.EventAI.PointCheck.Side:
            pos = this.m_current_SidePoint.position;
            break;
          case MeatGrinderMaster.EventAI.PointCheck.Rear:
            pos = this.m_current_BackPoint.position;
            break;
        }
        if (Flash)
          FXM.InitiateMuzzleFlash(pos, UnityEngine.Random.onUnitSphere, UnityEngine.Random.Range(2f, 4f), Color.white, UnityEngine.Random.Range(2f, 5f));
        if (Sound)
        {
          this.EventAudioSource.transform.position = pos;
          this.EventAudioSource.pitch = UnityEngine.Random.Range(0.7f, 1.1f);
          this.EventAudioSource.clip = this.EventAudioClips[UnityEngine.Random.Range(0, this.EventAudioClips.Length)];
          this.EventAudioSource.volume = UnityEngine.Random.Range(0.4f, 1f);
          this.EventAudioSource.Play();
        }
        return true;
      }

      public enum EventAIMood
      {
        None,
        Nice,
        Nasty,
        Nightmarish,
      }

      public enum EventType
      {
        eSpawn_Ammo = 0,
        eSpawn_Pistol1 = 1,
        eSpawn_Pistol2 = 2,
        eSpawn_Shotgun1 = 3,
        eSpawn_Shotgun2 = 4,
        eSpawn_SMG1 = 5,
        eSpawn_SMG2 = 6,
        eSpawn_Rifle1 = 7,
        eSpawn_Rifle2 = 8,
        eSpawn_Chainsaw = 9,
        eSpawn_AntiMaterialRifle = 10, // 0x0000000A
        eSpawn_Minigun = 11, // 0x0000000B
        eSpawn_Present = 12, // 0x0000000C
        eTrigger_Sound = 20, // 0x00000014
        eTrigger_Flash = 21, // 0x00000015
        eTrigger_SoundFlash = 22, // 0x00000016
        eSpawnHazard_ExplodingWeiner = 30, // 0x0000001E
        eSpawnHazard_GiantSawBlade = 31, // 0x0000001F
        eSpawnAgent_FlamingMeatball = 40, // 0x00000028
        eSpawnAgent_KetchupBot = 41, // 0x00000029
        eSpawnAgent_MustardBot = 42, // 0x0000002A
        eSpawnAgent_ShotgunBot = 43, // 0x0000002B
        eSpawnAgent_KneumaticBot = 44, // 0x0000002C
        eSpawnAgent_DrillWeiners = 45, // 0x0000002D
        eSpawnAgent_LetterM = 46, // 0x0000002E
        eSpawnAgent_ScreamingJerry = 47, // 0x0000002F
        eTrigger_Goober = 60, // 0x0000003C
      }

      [Serializable]
      public class MGEvent
      {
        public MeatGrinderMaster.EventAI.EventType Type;
        public float Incidence;
        public Vector2 TickRange;
        public bool IsEventExclusive;
        [HideInInspector]
        public float FinalWeight;
      }

      private enum PointCheck
      {
        Front,
        Side,
        Rear,
      }
    }
  }
}
