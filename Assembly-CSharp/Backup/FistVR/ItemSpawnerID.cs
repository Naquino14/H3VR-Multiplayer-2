// Decompiled with JetBrains decompiler
// Type: FistVR.ItemSpawnerID
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Definition", menuName = "ItemSpawner/ID", order = 0)]
  public class ItemSpawnerID : ScriptableObject
  {
    [Header("Display")]
    public bool IsDisplayedInMainEntry = true;
    public string DisplayName;
    public Sprite Sprite;
    public string SubHeading;
    [Multiline(4)]
    public string Description;
    public ItemSpawnerControlInfographic Infographic;
    [Header("Categorization")]
    public ItemSpawnerID.EItemCategory Category;
    public ItemSpawnerID.ESubCategory SubCategory;
    public string ItemID;
    [Header("Prefabs")]
    public FVRObject MainObject;
    public FVRObject SecondObject;
    public ItemSpawnerID[] Secondaries;
    [Header("Spawning")]
    public bool UsesLargeSpawnPad;
    public bool UsesHugeSpawnPad;
    [Header("Eco System")]
    public int UnlockCost;
    public bool IsUnlockedByDefault;
    [Header("Reward Setting")]
    public bool IsReward;
    public static readonly string[] SubCatNames = new string[46]
    {
      "None",
      "Automatic Pistol",
      "Revolver",
      "Machine Pistol",
      "Break-Action Shotgun",
      "Tube Fed Shotgun",
      "Magazine Fed Shotgun",
      "Submachinegun",
      "Personal Defense Weapon",
      "Lever Action Rifle",
      "Carbine",
      "Assault Rifle",
      "Battle Rifle",
      "Bolt Action Rifle",
      "Anti-Material Rifle",
      "Hand Grenade",
      "Machinegun",
      "Ordnance",
      "Iron Sight",
      "Reflex Sight",
      "Magnifing Optic",
      "Muzzle Device",
      "Laser/Light",
      "Rail Adapter",
      "Decorative Attachment",
      "Tactical Melee Weapon",
      "Improvised Melee Weapon",
      "Thrown Melee Weapon",
      "Utility Object",
      "Target",
      "Horseshoe",
      "Tippy Toy",
      "Firework",
      "Breach Loading Handgun",
      "Lever Action Handgun",
      "Lever Action Shotgun",
      "Garage Tool",
      "Power Tool",
      "Medieval Arm",
      "Goofy Melee",
      "Farm Tool",
      "Shield",
      "Bolt Action",
      "Foregrip",
      "Stock",
      "Backpack"
    };

    [ContextMenu("AttachObjectIDToThis")]
    public void AttachMainObjectIDToThis()
    {
    }

    [ContextMenu("CopyDisplayNameFromObject")]
    public void CopyDisplayNameFromObject() => this.DisplayName = this.MainObject.DisplayName;

    [ContextMenu("AutoID")]
    public void AutoID()
    {
      string str = this.Category.ToString() + this.SubCategory.ToString() + this.DisplayName;
      str.Replace(" ", string.Empty);
      str.Replace("_", string.Empty);
      this.ItemID = str;
    }

    public enum EItemCategory
    {
      Pistol,
      Shotgun,
      SMG_Rifle,
      Support,
      Attachment,
      Melee,
      Misc,
      Magazine,
      Clip,
      MeatFortress,
      GAMN,
      TARG,
      BARR,
      STRU,
      FURN,
      SIGN,
    }

    public enum ESubCategory
    {
      None = 0,
      Automatic = 1,
      Revolver = 2,
      MachinePistol = 3,
      BreakAction = 4,
      TubeFed = 5,
      MagazineFed = 6,
      SMG = 7,
      PDW = 8,
      LeverAction = 9,
      Carbine = 10, // 0x0000000A
      AssaultRifle = 11, // 0x0000000B
      BattleRifle = 12, // 0x0000000C
      BoltAction = 13, // 0x0000000D
      AntiMaterial = 14, // 0x0000000E
      Grenade = 15, // 0x0000000F
      Machinegun = 16, // 0x00000010
      Ordnance = 17, // 0x00000011
      IronSight = 18, // 0x00000012
      ReflexSight = 19, // 0x00000013
      Magnifier_Scope = 20, // 0x00000014
      Suppressor = 21, // 0x00000015
      Laser_Light = 22, // 0x00000016
      RailAdapter = 23, // 0x00000017
      Decorative = 24, // 0x00000018
      Tactical = 25, // 0x00000019
      Improvised = 26, // 0x0000001A
      Thrown = 27, // 0x0000001B
      Utility = 28, // 0x0000001C
      Target = 29, // 0x0000001D
      Horseshoe = 30, // 0x0000001E
      TippyToy = 31, // 0x0000001F
      Firework = 32, // 0x00000020
      BreechloadingHandgun = 33, // 0x00000021
      LeverActionHandgun = 34, // 0x00000022
      LeverActionShotgun = 35, // 0x00000023
      Garage = 36, // 0x00000024
      PowerTools = 37, // 0x00000025
      Medieval = 38, // 0x00000026
      GoofyMelee = 39, // 0x00000027
      FarmTools = 40, // 0x00000028
      Shields = 41, // 0x00000029
      BoltActionPistol = 42, // 0x0000002A
      Foregrip = 43, // 0x0000002B
      Stock = 44, // 0x0000002C
      Backpack = 45, // 0x0000002D
      Bayonet = 46, // 0x0000002E
      UnderBarrelWeapon = 47, // 0x0000002F
      BangerJunk = 48, // 0x00000030
      RailHat = 49, // 0x00000031
      MuzzleLoadedPistol = 50, // 0x00000032
      Derringer = 51, // 0x00000033
      BreechloadingRifle = 52, // 0x00000034
      MuzzleLoadedRifle = 53, // 0x00000035
      MF_Scout = 100, // 0x00000064
      MF_Soldier = 101, // 0x00000065
      MF_Pyro = 102, // 0x00000066
      MF_Demo = 103, // 0x00000067
      MF_Heavy = 104, // 0x00000068
      MF_Engineer = 105, // 0x00000069
      MF_Medic = 106, // 0x0000006A
      MF_Sniper = 107, // 0x0000006B
      MF_Spy = 108, // 0x0000006C
      MF_Generic = 109, // 0x0000006D
      GAMN_Man = 200, // 0x000000C8
      GAMN_Pan = 201, // 0x000000C9
      GAMN_Bot = 202, // 0x000000CA
      GAMN_Con = 203, // 0x000000CB
      GAMN_Goa = 204, // 0x000000CC
      GAMN_Tri = 205, // 0x000000CD
      TARG_Ste = 210, // 0x000000D2
      TARG_Des = 211, // 0x000000D3
      BARR_Mil = 220, // 0x000000DC
      BARR_Spo = 221, // 0x000000DD
      BARR_Nat = 222, // 0x000000DE
      STRU_Pla = 230, // 0x000000E6
      STRU_WaW = 231, // 0x000000E7
      FURN_Tab = 240, // 0x000000F0
      FURN_She = 241, // 0x000000F1
      FURN_Sea = 242, // 0x000000F2
      SIGN_Dir = 250, // 0x000000FA
      SIGN_Goa = 251, // 0x000000FB
      SIGN_Tea = 252, // 0x000000FC
      SIGN_Dec = 253, // 0x000000FD
    }
  }
}
