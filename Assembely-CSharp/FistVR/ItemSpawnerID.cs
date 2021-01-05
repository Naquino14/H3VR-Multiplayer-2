using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Definition", menuName = "ItemSpawner/ID", order = 0)]
	public class ItemSpawnerID : ScriptableObject
	{
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
			SIGN
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
			Carbine = 10,
			AssaultRifle = 11,
			BattleRifle = 12,
			BoltAction = 13,
			AntiMaterial = 14,
			Grenade = 0xF,
			Machinegun = 0x10,
			Ordnance = 17,
			IronSight = 18,
			ReflexSight = 19,
			Magnifier_Scope = 20,
			Suppressor = 21,
			Laser_Light = 22,
			RailAdapter = 23,
			Decorative = 24,
			Tactical = 25,
			Improvised = 26,
			Thrown = 27,
			Utility = 28,
			Target = 29,
			Horseshoe = 30,
			TippyToy = 0x1F,
			Firework = 0x20,
			BreechloadingHandgun = 33,
			LeverActionHandgun = 34,
			LeverActionShotgun = 35,
			Garage = 36,
			PowerTools = 37,
			Medieval = 38,
			GoofyMelee = 39,
			FarmTools = 40,
			Shields = 41,
			BoltActionPistol = 42,
			Foregrip = 43,
			Stock = 44,
			Backpack = 45,
			Bayonet = 46,
			UnderBarrelWeapon = 47,
			BangerJunk = 48,
			RailHat = 49,
			MuzzleLoadedPistol = 50,
			Derringer = 51,
			BreechloadingRifle = 52,
			MuzzleLoadedRifle = 53,
			MF_Scout = 100,
			MF_Soldier = 101,
			MF_Pyro = 102,
			MF_Demo = 103,
			MF_Heavy = 104,
			MF_Engineer = 105,
			MF_Medic = 106,
			MF_Sniper = 107,
			MF_Spy = 108,
			MF_Generic = 109,
			GAMN_Man = 200,
			GAMN_Pan = 201,
			GAMN_Bot = 202,
			GAMN_Con = 203,
			GAMN_Goa = 204,
			GAMN_Tri = 205,
			TARG_Ste = 210,
			TARG_Des = 211,
			BARR_Mil = 220,
			BARR_Spo = 221,
			BARR_Nat = 222,
			STRU_Pla = 230,
			STRU_WaW = 231,
			FURN_Tab = 240,
			FURN_She = 241,
			FURN_Sea = 242,
			SIGN_Dir = 250,
			SIGN_Goa = 251,
			SIGN_Tea = 252,
			SIGN_Dec = 253
		}

		[Header("Display")]
		public bool IsDisplayedInMainEntry = true;

		public string DisplayName;

		public Sprite Sprite;

		public string SubHeading;

		[Multiline(4)]
		public string Description;

		public ItemSpawnerControlInfographic Infographic;

		[Header("Categorization")]
		public EItemCategory Category;

		public ESubCategory SubCategory;

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
		public void CopyDisplayNameFromObject()
		{
			DisplayName = MainObject.DisplayName;
		}

		[ContextMenu("AutoID")]
		public void AutoID()
		{
			string text = Category.ToString();
			text += SubCategory;
			text += DisplayName;
			text.Replace(" ", string.Empty);
			text.Replace("_", string.Empty);
			ItemID = text;
		}
	}
}
