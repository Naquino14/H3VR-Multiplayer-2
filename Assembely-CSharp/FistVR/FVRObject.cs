using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Definition", menuName = "Object IDs/ObjectID", order = 0)]
	public class FVRObject : AnvilAsset
	{
		public enum ObjectCategory
		{
			Uncategorized = 0,
			Firearm = 1,
			Magazine = 2,
			Clip = 3,
			Cartridge = 4,
			Attachment = 5,
			SpeedLoader = 6,
			Thrown = 7,
			MeleeWeapon = 10,
			Explosive = 20,
			Powerup = 25,
			Target = 30,
			Tool = 40,
			Toy = 41,
			Firework = 42,
			Ornament = 43,
			Loot = 50,
			VFX = 51
		}

		public enum OTagSet
		{
			Real,
			GroundedFictional,
			SciFiFictional,
			Meme,
			MF,
			Holiday,
			TNH
		}

		public enum OTagEra
		{
			None,
			Colonial,
			WildWest,
			TurnOfTheCentury,
			WW1,
			WW2,
			PostWar,
			Modern,
			Futuristic,
			Medieval
		}

		public enum OTagFirearmSize
		{
			None,
			Pocket,
			Pistol,
			Compact,
			Carbine,
			FullSize,
			Bulky,
			Oversize
		}

		public enum OTagFirearmAction
		{
			None,
			BreakAction,
			BoltAction,
			Revolver,
			PumpAction,
			LeverAction,
			Automatic,
			RollingBlock,
			OpenBreach,
			Preloaded,
			SingleActionRevolver
		}

		public enum OTagFirearmFiringMode
		{
			None,
			SemiAuto,
			Burst,
			FullAuto,
			SingleFire
		}

		public enum OTagFirearmFeedOption
		{
			None,
			BreachLoad,
			InternalMag,
			BoxMag,
			StripperClip,
			EnblocClip
		}

		public enum OTagFirearmRoundPower
		{
			None,
			Tiny,
			Pistol,
			Shotgun,
			Intermediate,
			FullPower,
			AntiMaterial,
			Ordnance,
			Exotic,
			Fire
		}

		public enum OTagFirearmMount
		{
			None,
			Picatinny,
			Russian,
			Muzzle,
			Stock,
			Bespoke
		}

		public enum OTagFirearmCountryOfOrigin
		{
			None = 0,
			Fictional = 1,
			UnitedStatesOfAmerica = 10,
			MuricanRemnants = 11,
			BritishEmpire = 20,
			UnitedKingdom = 21,
			KingdomOfFrance = 30,
			FrenchSecondRepublic = 0x1F,
			SecondFrenchEmpire = 0x20,
			FrenchThirdRepublic = 33,
			VichyFrance = 34,
			FrenchFourthRepublic = 35,
			FrenchRepublic = 36,
			GermanEmpire = 40,
			WeimarRepublic = 41,
			GermanReich = 42,
			WestGermany = 43,
			GermanDemocraticRepublic = 44,
			FederalRepublicOfGermany = 45,
			TsardomOfRussia = 50,
			RussianEmpire = 51,
			UnionOfSovietSocialistRepublics = 52,
			RussianFederation = 53,
			KingdomOfBelgium = 60,
			KingdomOfItaly = 70,
			ItalianRepublic = 71,
			SwedishEmpire = 90,
			UnitedKingdomsOfSwedenAndNorway = 91,
			KingdomOfSweden = 92,
			KingdomOfNorway = 100,
			KingdomOfFinland = 110,
			RepublicOfFinland = 111,
			Czechoslovakia = 120,
			CzechRepublic = 121,
			Ukraine = 130,
			SwissConfederation = 140,
			FirstSpanishRepublic = 150,
			SecondSpanishRepublic = 151,
			SpanishState = 152,
			KingdomOfSpain = 153,
			AustrianEmpire = 160,
			AustroHungarianEmpire = 161,
			RepublicOfAustria = 162,
			FirstHungarianRepublic = 170,
			HungarianRepublic = 171,
			KingdomOfHungary = 172,
			HungarianPeoplesRepublic = 173,
			RepublicOfCroatia = 190,
			RepublicOfKorea = 200,
			DemocraticRepublicOfVietnam = 210,
			StateOfIsrael = 220,
			FederativeRepublicOfBrazil = 230,
			EmpireOfJapan = 240,
			RepublicOfSouthAfrica = 250,
			GovernmentOfTheRepublicOfPolandInExile = 262,
			RepublicOfPoland = 263,
			PeoplesRepublicOfChina = 270,
			FormerYugoslavicRepublicOfMacedonia = 280
		}

		public enum OTagAttachmentFeature
		{
			None,
			IronSight,
			Magnification,
			Reflex,
			Suppression,
			Stock,
			Laser,
			Illumination,
			Grip,
			Decoration,
			RecoilMitigation,
			BarrelExtension,
			Adapter,
			Bayonet,
			ProjectileWeapon,
			Bipod
		}

		public enum OTagMeleeStyle
		{
			None,
			Tactical,
			Tool,
			Improvised,
			Medieval,
			Shield,
			PowerTool
		}

		public enum OTagMeleeHandedness
		{
			None,
			OneHanded,
			TwoHanded
		}

		public enum OTagPowerupType
		{
			None = -1,
			Health,
			QuadDamage,
			InfiniteAmmo,
			Invincibility,
			GhostMode,
			FarOutMeat,
			MuscleMeat,
			HomeTown,
			SnakeEye,
			Blort,
			Regen,
			Cyclops,
			WheredIGo,
			ChillOut
		}

		public enum OTagThrownType
		{
			None,
			ManualFuse,
			Pinned,
			Strange
		}

		public enum OTagThrownDamageType
		{
			None,
			Kinetic,
			Explosive,
			Fire,
			Utility
		}

		public string ItemID;

		public string DisplayName;

		public ObjectCategory Category;

		public string SpawnedFromId;

		[Header("Values")]
		public float Mass;

		public int MagazineCapacity;

		public bool RequiresPicatinnySight;

		[Header("Tags")]
		public OTagEra TagEra;

		public OTagSet TagSet;

		public OTagFirearmSize TagFirearmSize;

		public OTagFirearmAction TagFirearmAction;

		public OTagFirearmRoundPower TagFirearmRoundPower;

		[SearchableEnum]
		public OTagFirearmCountryOfOrigin TagFirearmCountryOfOrigin;

		public int TagFirearmFirstYear;

		public List<OTagFirearmFiringMode> TagFirearmFiringModes;

		public List<OTagFirearmFeedOption> TagFirearmFeedOption;

		public List<OTagFirearmMount> TagFirearmMounts;

		public OTagFirearmMount TagAttachmentMount;

		public OTagAttachmentFeature TagAttachmentFeature;

		public OTagMeleeStyle TagMeleeStyle;

		public OTagMeleeHandedness TagMeleeHandedness;

		public OTagPowerupType TagPowerupType;

		public OTagThrownType TagThrownType;

		public OTagThrownDamageType TagThrownDamageType;

		[Header("RelatedAssets")]
		public List<FVRObject> CompatibleMagazines;

		public List<FVRObject> CompatibleClips;

		public List<FVRObject> CompatibleSpeedLoaders;

		public List<FVRObject> CompatibleSingleRounds;

		public List<FVRObject> BespokeAttachments;

		public List<FVRObject> RequiredSecondaryPieces;

		public int MinCapacityRelated = -1;

		public int MaxCapacityRelated = -1;

		[Header("EcoSystemStuff")]
		public int CreditCost;

		public bool OSple = true;

		[ContextMenu("PrintBespoke")]
		public void PrintBespoke()
		{
			for (int i = 0; i < BespokeAttachments.Count; i++)
			{
				Debug.Log(DisplayName + " has " + BespokeAttachments[i].DisplayName);
			}
		}

		public FVRObject GetMagazineWithinCapacity(int cap)
		{
			List<FVRObject> list = new List<FVRObject>();
			for (int i = 0; i < CompatibleMagazines.Count; i++)
			{
				if (CompatibleMagazines[i].MagazineCapacity <= cap)
				{
					list.Add(CompatibleMagazines[i]);
				}
			}
			if (list.Count > 0)
			{
				return list[Random.Range(0, list.Count)];
			}
			return CompatibleMagazines[0];
		}

		public FVRObject GetRandomAmmoObject(FVRObject o, List<OTagEra> eras = null, int Min = -1, int Max = -1, List<OTagSet> sets = null)
		{
			if (o.CompatibleMagazines.Count > 0)
			{
				List<FVRObject> list = new List<FVRObject>();
				for (int i = 0; i < o.CompatibleMagazines.Count; i++)
				{
					if ((o.CompatibleMagazines[i].MagazineCapacity <= Max || Max == -1) && (o.CompatibleMagazines[i].MagazineCapacity >= Min || Min == -1))
					{
						list.Add(o.CompatibleMagazines[i]);
					}
				}
				if (list.Count > 0)
				{
					return list[Random.Range(0, list.Count)];
				}
				return o.CompatibleMagazines[0];
			}
			if (o.CompatibleClips.Count > 0)
			{
				return o.CompatibleClips[Random.Range(0, o.CompatibleClips.Count)];
			}
			if (o.CompatibleSpeedLoaders.Count > 0)
			{
				return o.CompatibleSpeedLoaders[Random.Range(0, o.CompatibleSpeedLoaders.Count)];
			}
			if (o.CompatibleSingleRounds.Count > 0)
			{
				if ((eras == null || eras.Count < 1) && (sets == null || sets.Count < 1))
				{
					return o.CompatibleSingleRounds[Random.Range(0, o.CompatibleSingleRounds.Count)];
				}
				List<FVRObject> list2 = new List<FVRObject>();
				for (int j = 0; j < o.CompatibleSingleRounds.Count; j++)
				{
					bool flag = true;
					if (eras != null && eras.Count > 0 && !eras.Contains(o.CompatibleSingleRounds[j].TagEra))
					{
						flag = false;
					}
					if (sets != null && sets.Count > 0 && !sets.Contains(o.CompatibleSingleRounds[j].TagSet))
					{
						flag = false;
					}
					if (flag)
					{
						list2.Add(o.CompatibleSingleRounds[j]);
					}
				}
				if (list2.Count > 0)
				{
					FVRObject result = list2[Random.Range(0, list2.Count)];
					list2.Clear();
					return result;
				}
				return o.CompatibleSingleRounds[0];
			}
			return null;
		}

		[ContextMenu("GenerateItemIDFromDisplayName")]
		public void GenerateItemIDFromDisplayName()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < DisplayName.Length; i++)
			{
				if (DisplayName[i].ToString() == "_")
				{
					stringBuilder.Append(" ");
				}
				else
				{
					stringBuilder.Append(DisplayName[i]);
				}
			}
			DisplayName = stringBuilder.ToString();
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int j = 0; j < DisplayName.Length; j++)
			{
				if (DisplayName[j].ToString() != " " && DisplayName[j].ToString() != ".")
				{
					stringBuilder2.Append(DisplayName[j]);
				}
			}
			ItemID = stringBuilder2.ToString();
		}

		[ContextMenu("SetNamesFromFileName")]
		public void SetNamesFromFileName()
		{
			ItemID = base.name;
			DisplayName = base.name;
		}

		[ContextMenu("PopulateMinMaxCapacity")]
		public void PopulateMinMaxCapacity()
		{
		}

		[ContextMenu("PopulateAttachments")]
		public void PopulateAttachments()
		{
			if (Category != ObjectCategory.Firearm && Category == ObjectCategory.MeleeWeapon)
			{
			}
		}

		[ContextMenu("PopulateRoundPower")]
		public void PopulateRoundPower()
		{
			if (Category == ObjectCategory.Firearm && CompatibleMagazines.Count >= 1)
			{
			}
		}

		[ContextMenu("MigrateFirearmDataToMagazine")]
		public void MigrateFirearmDataToMagazine()
		{
			if (Category == ObjectCategory.Firearm && CompatibleMagazines.Count >= 1)
			{
			}
		}
	}
}
