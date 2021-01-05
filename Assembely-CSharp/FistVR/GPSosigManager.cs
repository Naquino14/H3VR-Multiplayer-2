using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class GPSosigManager : GPPlaceable
	{
		public enum SpawnWhen
		{
			OnBegin,
			OnEvent
		}

		public enum InitialState
		{
			Guard,
			Assault,
			Inactive,
			Wander,
			Patrol
		}

		public enum Equipment
		{
			All,
			Primary,
			NoNades,
			None
		}

		public enum Ammo
		{
			Standard,
			Maxed,
			None
		}

		private List<Sosig> m_spawnedSosigs = new List<Sosig>();

		private void Start()
		{
			GM.CurrentSceneSettings.SosigKillEvent += OnSosigKill;
			GM.CurrentGamePlannerManager.SosigManager = this;
		}

		private void OnDestroy()
		{
			GM.CurrentSceneSettings.SosigKillEvent -= OnSosigKill;
		}

		public override void Init(GPSceneMode mode)
		{
			FlushAllSosigs();
			m_spawnedSosigs.Clear();
			base.Init(mode);
		}

		private void OnSosigKill(Sosig s)
		{
			if (!(s == null))
			{
				if (m_spawnedSosigs.Contains(s))
				{
					m_spawnedSosigs.Remove(s);
				}
				s.TickDownToClear(3f);
			}
		}

		public void FlushAllSosigs()
		{
		}

		public Sosig SpawnSosig(SosigEnemyTemplate t, Transform point, InitialState initialState, Equipment equipState, Ammo ammoState, int teamIFF, int assaultGroup, int patrolGroup)
		{
			GameObject original = null;
			GameObject gameObject = Object.Instantiate(original, point.position, point.rotation);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			componentInChildren.E.IFFCode = teamIFF;
			componentInChildren.SetOriginalIFFTeam(teamIFF);
			SosigOutfitConfig sosigOutfitConfig = t.OutfitConfig[Random.Range(0, t.OutfitConfig.Count)];
			SpawnAccesoryToLink(sosigOutfitConfig.Headwear, componentInChildren.Links[0], sosigOutfitConfig.Chance_Headwear);
			SpawnAccesoryToLink(sosigOutfitConfig.Facewear, componentInChildren.Links[0], sosigOutfitConfig.Chance_Headwear);
			SpawnAccesoryToLink(sosigOutfitConfig.Eyewear, componentInChildren.Links[0], sosigOutfitConfig.Chance_Headwear);
			SpawnAccesoryToLink(sosigOutfitConfig.Torsowear, componentInChildren.Links[1], sosigOutfitConfig.Chance_Headwear);
			SpawnAccesoryToLink(sosigOutfitConfig.Pantswear, componentInChildren.Links[2], sosigOutfitConfig.Chance_Headwear);
			SpawnAccesoryToLink(sosigOutfitConfig.Pantswear_Lower, componentInChildren.Links[3], sosigOutfitConfig.Chance_Headwear);
			SpawnAccesoryToLink(sosigOutfitConfig.Backpacks, componentInChildren.Links[1], sosigOutfitConfig.Chance_Headwear);
			componentInChildren.SetGuardInvestigateDistanceThreshold(25f);
			m_spawnedSosigs.Add(componentInChildren);
			return componentInChildren;
		}

		private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l, float chance)
		{
			if (!(Random.Range(0f, 1f) > chance))
			{
				GameObject gameObject = Object.Instantiate(gs[Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
				gameObject.transform.SetParent(l.transform);
				SosigWearable component = gameObject.GetComponent<SosigWearable>();
				component.RegisterWearable(l);
			}
		}
	}
}
