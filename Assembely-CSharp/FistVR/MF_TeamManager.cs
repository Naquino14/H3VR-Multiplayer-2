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

		public MF_GameMode GetMode()
		{
			return m_currentGameMode;
		}

		private void Start()
		{
		}

		public void InitGameMode(MF_GameMode mode, int RedTeamSize, int BlueTeamSize, MF_TeamColor playerTeam, float RedTeamSpawnCadence, float BlueTeamSpawnCadence, MF_PlayArea playArea)
		{
			m_currentGameMode = mode;
			GM.MFFlags.PlayerTeam = playerTeam;
			RedTeam.ResetZoneSet();
			BlueTeam.ResetZoneSet();
			RedTeam.LoadZoneList(GetZoneSet(mode, playArea, MF_ZoneCategory.SpawnRed), MF_ZoneType.Spawn);
			RedTeam.LoadZoneList(GetZoneSet(mode, playArea, MF_ZoneCategory.RedSide), MF_ZoneType.OwnSide);
			RedTeam.LoadZoneList(GetZoneSet(mode, playArea, MF_ZoneCategory.Neutral), MF_ZoneType.Center);
			RedTeam.LoadZoneList(GetZoneSet(mode, playArea, MF_ZoneCategory.BlueSide), MF_ZoneType.EnemySide);
			BlueTeam.LoadZoneList(GetZoneSet(mode, playArea, MF_ZoneCategory.SpawnBlue), MF_ZoneType.Spawn);
			BlueTeam.LoadZoneList(GetZoneSet(mode, playArea, MF_ZoneCategory.BlueSide), MF_ZoneType.OwnSide);
			BlueTeam.LoadZoneList(GetZoneSet(mode, playArea, MF_ZoneCategory.Neutral), MF_ZoneType.Center);
			BlueTeam.LoadZoneList(GetZoneSet(mode, playArea, MF_ZoneCategory.RedSide), MF_ZoneType.EnemySide);
			RedTeam.Init(this, MF_TeamColor.Red, BlueTeam, RedTeamSize, RedTeamSpawnCadence);
			BlueTeam.Init(this, MF_TeamColor.Blue, RedTeam, BlueTeamSize, BlueTeamSpawnCadence);
			int num = Random.Range(0, 3);
			int num2 = Random.Range(0, 3);
			int num3 = Random.Range(0, 3);
			PlayerRespawnZone = GetPlayerRespawnZone(mode, playArea, playerTeam);
			m_hasInit = true;
		}

		public void Update()
		{
			if (m_hasInit)
			{
				float deltaTime = Time.deltaTime;
				RedTeam.Tick(deltaTime);
				BlueTeam.Tick(deltaTime);
			}
		}

		public void RearmSosig(Sosig s, MF_Class c)
		{
			SosigEnemyTemplate sosigEnemyTemplate = EnemyTemplatesByClassIndex_Red[(int)c];
			GameObject gameObject = null;
			if (sosigEnemyTemplate.WeaponOptions.Count > 0)
			{
				gameObject = sosigEnemyTemplate.WeaponOptions[Random.Range(0, sosigEnemyTemplate.WeaponOptions.Count)].GetGameObject();
			}
			if (gameObject != null)
			{
				Vector3 position = s.transform.position;
				Quaternion rotation = s.transform.rotation;
				SosigWeapon component = Object.Instantiate(gameObject, position + Vector3.up * 0.1f, rotation).GetComponent<SosigWeapon>();
				component.SetAutoDestroy(b: true);
				component.O.SpawnLockable = false;
				if (component.Type == SosigWeapon.SosigWeaponType.Gun)
				{
					s.Inventory.FillAmmoWithType(component.AmmoType);
				}
				if (component != null)
				{
					s.InitHands();
					s.ForceEquip(component);
					component.SetAmmoClamping(b: true);
				}
			}
		}

		public Sosig SpawnEnemy(SosigEnemyTemplate t, Transform point, int IFF, bool IsAssault, Vector3 pointOfInterest, bool AllowSecondary, MF_TeamColor teamcolor)
		{
			GameObject weaponPrefab = null;
			if (t.WeaponOptions.Count > 0)
			{
				weaponPrefab = t.WeaponOptions[Random.Range(0, t.WeaponOptions.Count)].GetGameObject();
			}
			GameObject weaponPrefab2 = null;
			if (t.WeaponOptions_Secondary.Count > 0 && AllowSecondary)
			{
				float num = Random.Range(0f, 1f);
				if (num > t.SecondaryChance)
				{
					weaponPrefab2 = t.WeaponOptions_Secondary[Random.Range(0, t.WeaponOptions_Secondary.Count)].GetGameObject();
				}
			}
			SosigConfigTemplate t2 = t.ConfigTemplates[0];
			Sosig sosig = SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)].GetGameObject(), weaponPrefab, weaponPrefab2, point.position, point.rotation, t2, t.OutfitConfig[Random.Range(0, t.OutfitConfig.Count)], IFF, IsAssault, pointOfInterest, teamcolor);
			sosig.SetInvulnMaterial(UberMats[(int)teamcolor]);
			return sosig;
		}

		private Sosig SpawnEnemySosig(GameObject prefab, GameObject weaponPrefab, GameObject weaponPrefab2, Vector3 pos, Quaternion rot, SosigConfigTemplate t, SosigOutfitConfig o, int IFF, bool IsAssault, Vector3 pointOfInterest, MF_TeamColor teamcolor)
		{
			GameObject gameObject = Object.Instantiate(prefab, pos, rot);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			componentInChildren.Configure(t);
			componentInChildren.E.IFFCode = IFF;
			if (weaponPrefab != null)
			{
				SosigWeapon component = Object.Instantiate(weaponPrefab, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
				component.SetAutoDestroy(b: true);
				component.O.SpawnLockable = false;
				if (component.Type == SosigWeapon.SosigWeaponType.Gun)
				{
					componentInChildren.Inventory.FillAmmoWithType(component.AmmoType);
				}
				if (component != null)
				{
					componentInChildren.InitHands();
					componentInChildren.ForceEquip(component);
					component.SetAmmoClamping(b: true);
				}
				if (weaponPrefab2 != null)
				{
					SosigWeapon component2 = Object.Instantiate(weaponPrefab2, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
					component2.SetAutoDestroy(b: true);
					component2.O.SpawnLockable = false;
					component2.SetAmmoClamping(b: true);
					if (component2.Type == SosigWeapon.SosigWeaponType.Gun)
					{
						componentInChildren.Inventory.FillAmmoWithType(component2.AmmoType);
					}
					if (component2 != null)
					{
						componentInChildren.ForceEquip(component2);
					}
				}
			}
			float num = 0f;
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Headwear)
			{
				SpawnAccesoryToLink(o.Headwear, componentInChildren.Links[0], teamcolor);
			}
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Facewear)
			{
				SpawnAccesoryToLink(o.Facewear, componentInChildren.Links[0], teamcolor);
			}
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Torsowear)
			{
				SpawnAccesoryToLink(o.Torsowear, componentInChildren.Links[1], teamcolor);
			}
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Pantswear)
			{
				SpawnAccesoryToLink(o.Pantswear, componentInChildren.Links[2], teamcolor);
			}
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Pantswear_Lower)
			{
				SpawnAccesoryToLink(o.Pantswear_Lower, componentInChildren.Links[3], teamcolor);
			}
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Backpacks)
			{
				SpawnAccesoryToLink(o.Backpacks, componentInChildren.Links[1], teamcolor);
			}
			if (t.UsesLinkSpawns)
			{
				for (int i = 0; i < componentInChildren.Links.Count; i++)
				{
					float num2 = Random.Range(0f, 1f);
					if (num2 < t.LinkSpawnChance[i])
					{
						componentInChildren.Links[i].RegisterSpawnOnDestroy(t.LinkSpawns[i]);
					}
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
				float num3 = Random.Range(0f, 1f);
				bool flag = false;
				if (num3 > 0.25f)
				{
					flag = true;
				}
				componentInChildren.CommandGuardPoint(pointOfInterest, hardguard: true);
				componentInChildren.SetDominantGuardDirection(Random.onUnitSphere);
			}
			componentInChildren.SetGuardInvestigateDistanceThreshold(25f);
			return componentInChildren;
		}

		private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l, MF_TeamColor teamcolor)
		{
			GameObject gameObject = Object.Instantiate(gs[Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
			gameObject.transform.SetParent(l.transform);
			SosigWearable component = gameObject.GetComponent<SosigWearable>();
			component.RegisterWearable(l);
		}

		private MF_Zone GetPlayerRespawnZone(MF_GameMode mode, MF_PlayArea playArea, MF_TeamColor color)
		{
			MF_ZoneModeConfig mF_ZoneModeConfig = null;
			for (int i = 0; i < ZoneModeConfigs.Count; i++)
			{
				if (mode == ZoneModeConfigs[i].Mode)
				{
					mF_ZoneModeConfig = ZoneModeConfigs[i];
					break;
				}
			}
			MF_PlayAreaConfig playAreaConfig = mF_ZoneModeConfig.GetPlayAreaConfig(playArea);
			return color switch
			{
				MF_TeamColor.Red => playAreaConfig.PlayerSpawnZone_Red, 
				MF_TeamColor.Blue => playAreaConfig.PlayerSpawnZone_Blue, 
				_ => null, 
			};
		}

		private List<MF_Zone> GetZoneSet(MF_GameMode mode, MF_PlayArea playArea, MF_ZoneCategory category)
		{
			MF_ZoneModeConfig mF_ZoneModeConfig = null;
			for (int i = 0; i < ZoneModeConfigs.Count; i++)
			{
				if (mode == ZoneModeConfigs[i].Mode)
				{
					mF_ZoneModeConfig = ZoneModeConfigs[i];
					break;
				}
			}
			return mF_ZoneModeConfig?.GetPlayAreaConfig(playArea).GetZoneSet(category);
		}
	}
}
