using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigEnemySpawnZone : MonoBehaviour
	{
		[Serializable]
		public class ZosigSpawnGroup
		{
			public ZosigEnemyTemplate Template;

			public SosigEnemyTemplate STemplate;

			public int MinSpawnedInGroup = 1;

			public int MaxSpawnedInGroup = 1;

			public int IFF = 1;

			public List<ZosigEnemyTemplate> TemplateSelection;

			public List<SosigEnemyTemplate> STemplateSelection;

			public SosigEnemyTemplate GetTemplate()
			{
				if (STemplate != null)
				{
					return STemplate;
				}
				return STemplateSelection[UnityEngine.Random.Range(0, STemplateSelection.Count)];
			}
		}

		public ZosigSpawnManager M;

		[Header("Flag Settings")]
		public bool RequiresFlagToSpawn;

		public bool HasBlockingFlag;

		public string RequiredFlag;

		public int RequiredFlagValue;

		public string BlockingFlag;

		public int BlockingFlagThreshold;

		[Header("Volumes And Spawn Points")]
		public List<Transform> VolumesToCheck;

		public List<Transform> SpawnPoints;

		[Header("Spawn Setting Params")]
		public bool CanSpawnInView;

		public Vector2 SpawnRange = new Vector2(20f, 200f);

		public int MaxCanBeAlive = 5;

		public float DespawnRange = 500f;

		public bool DoesSpawnOnEntry;

		public bool UsesSpawnEffect;

		public GameObject SpawnEffect;

		[Header("Spawn Group Params")]
		public List<ZosigSpawnGroup> SpawnGroups;

		public Vector2 RefireTickRangeAfterSpawnFailure = new Vector2(30f, 300f);

		public Vector2 RefireTickRangeAfterSpawnSuccess = new Vector2(30f, 300f);

		public Vector2 TickShortenOnGunShot = new Vector2(0.25f, 0.6f);

		[Header("Max TOTAL Count Params")]
		public bool UsesMaxTotalSpawnedCount;

		public int MaxToSpawnEver;

		public string FlagSetWhenSpawnedAll;

		public int FlagValueSetWhenSpawnedAll;

		[Header("Max Died Of Mine Params")]
		public bool UsesMaxDiedOfMine;

		public int MaxDiedOfMineThreshold;

		public string FlagSetWhenMaxDiedOfMine;

		public int FlagValueSetWhenMaxDiedOfMine;

		private bool m_hasHitMaxDiedOfMineThreshold;

		[Header("Patrol Settings")]
		public bool IsPatrolZone;

		public List<Transform> PatrolPoints;

		public List<Transform> PatrolSpawnPoints;

		private int m_curPatrolPoint;

		private bool m_isPatrollingUp = true;

		private int m_spawnedSofar;

		private int m_numOfMineWhoveDied;

		private bool m_hasShortenedSpawnTimeThisCycleYet;

		private float m_timeUntilSpawnCheck = 100f;

		private List<Sosig> m_spawnedSosigs = new List<Sosig>();

		private ZosigGameManager Manager;

		private float m_timeTilSonicEventPulse;

		private bool isSpawnProvoked;

		private float m_spawnReductionMult = 1f;

		private bool m_wasIn;

		private void Start()
		{
			GM.CurrentSceneSettings.SosigKillEvent += CheckIfDeadSosigWasMine;
			GM.CurrentSceneSettings.PerceiveableSoundEvent += SonicEvent;
			m_timeUntilSpawnCheck = UnityEngine.Random.Range(RefireTickRangeAfterSpawnFailure.x, RefireTickRangeAfterSpawnFailure.y);
		}

		private void OnDestroy()
		{
			GM.CurrentSceneSettings.SosigKillEvent -= CheckIfDeadSosigWasMine;
			GM.CurrentSceneSettings.PerceiveableSoundEvent -= SonicEvent;
		}

		public void Init(ZosigGameManager manager)
		{
			Manager = manager;
			if (UsesMaxDiedOfMine && UsesMaxTotalSpawnedCount && Manager.FlagM.GetFlagValue(FlagSetWhenMaxDiedOfMine) >= FlagValueSetWhenMaxDiedOfMine)
			{
				m_hasHitMaxDiedOfMineThreshold = true;
				m_spawnedSofar = MaxToSpawnEver;
			}
		}

		public void Tick(float time)
		{
			if (m_timeUntilSpawnCheck >= 0f)
			{
				m_timeUntilSpawnCheck -= Time.deltaTime;
			}
			if (m_timeTilSonicEventPulse > 0f)
			{
				m_timeTilSonicEventPulse -= Time.deltaTime;
			}
			else
			{
				m_timeTilSonicEventPulse = UnityEngine.Random.Range(5f, 10f);
				if (isSpawnProvoked)
				{
					isSpawnProvoked = false;
					m_timeUntilSpawnCheck *= m_spawnReductionMult;
					m_spawnReductionMult = 1f;
				}
			}
			if (!IsPatrolZone)
			{
				return;
			}
			if (m_spawnedSosigs.Count > 0)
			{
				bool flag = true;
				for (int i = 0; i < m_spawnedSosigs.Count; i++)
				{
					float num = Vector3.Distance(m_spawnedSosigs[i].transform.position, PatrolPoints[m_curPatrolPoint].position);
					if (num > 10f)
					{
						flag = false;
					}
				}
				if (flag)
				{
					if (m_curPatrolPoint + 1 >= PatrolPoints.Count && m_isPatrollingUp)
					{
						m_isPatrollingUp = false;
					}
					if (m_curPatrolPoint == 0)
					{
						m_isPatrollingUp = true;
					}
					if (m_isPatrollingUp)
					{
						m_curPatrolPoint++;
					}
					else
					{
						m_curPatrolPoint--;
					}
					for (int j = 0; j < m_spawnedSosigs.Count; j++)
					{
						m_spawnedSosigs[j].CommandAssaultPoint(PatrolPoints[m_curPatrolPoint].position);
					}
				}
				for (int k = 0; k < m_spawnedSosigs.Count; k++)
				{
					if (m_spawnedSosigs[k].CurrentOrder == Sosig.SosigOrder.Wander)
					{
						m_spawnedSosigs[k].CurrentOrder = Sosig.SosigOrder.Assault;
					}
					m_spawnedSosigs[k].FallbackOrder = Sosig.SosigOrder.Assault;
				}
			}
			else
			{
				m_curPatrolPoint = 0;
				m_isPatrollingUp = true;
			}
		}

		public void SonicEvent(float loudness, float maxDistanceHeard, Vector3 pos, int iffcode)
		{
			if (!IsPatrolZone && iffcode == 0 && !((float)m_spawnedSosigs.Count >= (float)MaxCanBeAlive * 0.5f))
			{
				float num = UnityEngine.Random.Range(60f, 150f);
				if (num < loudness)
				{
					float t = 1f - (loudness - num) / 140f;
					float a = Mathf.Lerp(TickShortenOnGunShot.x, TickShortenOnGunShot.x, t);
					isSpawnProvoked = true;
					m_spawnReductionMult = Mathf.Min(a, m_spawnReductionMult);
				}
			}
		}

		public void Check()
		{
			CheckDespawn();
			Vector3 position = GM.CurrentPlayerBody.Head.position;
			bool flag = false;
			if (IsPlayerInAnyVolumes(position))
			{
				if (!m_wasIn)
				{
					m_wasIn = true;
					if (DoesSpawnOnEntry)
					{
						flag = true;
					}
				}
			}
			else
			{
				m_wasIn = false;
			}
			if (m_timeUntilSpawnCheck > 0f && !flag)
			{
				return;
			}
			m_timeUntilSpawnCheck = UnityEngine.Random.Range(RefireTickRangeAfterSpawnFailure.x, RefireTickRangeAfterSpawnFailure.y);
			if ((RequiresFlagToSpawn && GM.ZMaster.FlagM.GetFlagValue(RequiredFlag) != RequiredFlagValue) || (HasBlockingFlag && GM.ZMaster.FlagM.GetFlagValue(BlockingFlag) > BlockingFlagThreshold) || m_spawnedSosigs.Count >= MaxCanBeAlive || (UsesMaxTotalSpawnedCount && m_spawnedSofar >= MaxToSpawnEver))
			{
				return;
			}
			Vector3 forward = GM.CurrentPlayerBody.Head.forward;
			if (!IsPlayerInAnyVolumes(position))
			{
				return;
			}
			bool flag2 = false;
			ZosigSpawnGroup zosigSpawnGroup = SpawnGroups[UnityEngine.Random.Range(0, SpawnGroups.Count)];
			int num = UnityEngine.Random.Range(zosigSpawnGroup.MinSpawnedInGroup, zosigSpawnGroup.MaxSpawnedInGroup + 1);
			List<Transform> list = new List<Transform>();
			if (IsPatrolZone)
			{
				for (int i = 0; i < PatrolSpawnPoints.Count; i++)
				{
					list.Add(PatrolSpawnPoints[i]);
				}
			}
			else
			{
				for (int j = 0; j < SpawnPoints.Count; j++)
				{
					list.Add(SpawnPoints[j]);
				}
			}
			if (list.Count > 0)
			{
				list.Shuffle();
			}
			for (int k = 0; k < num; k++)
			{
				if (list.Count < 1)
				{
					break;
				}
				bool flag3 = false;
				int index = 0;
				for (int num2 = list.Count - 1; num2 >= 0; num2--)
				{
					if (IsUsefulPoint(list[num2].position, position, forward))
					{
						flag3 = true;
						index = num2;
						break;
					}
				}
				if (flag3)
				{
					Transform point = list[index];
					list.RemoveAt(index);
					SpawnEnemy(zosigSpawnGroup.GetTemplate(), point, zosigSpawnGroup.IFF);
					flag2 = true;
					m_spawnedSofar++;
				}
			}
			list.Clear();
			if (flag2)
			{
				m_timeUntilSpawnCheck = UnityEngine.Random.Range(RefireTickRangeAfterSpawnSuccess.x, RefireTickRangeAfterSpawnSuccess.y);
			}
		}

		private void CheckDespawn()
		{
			if (m_spawnedSosigs.Count < 1)
			{
				return;
			}
			float num = 0f;
			for (int i = 0; i < VolumesToCheck.Count; i++)
			{
				num = Mathf.Max(num, DistanceFromClosestPointOnZone(VolumesToCheck[i]));
			}
			if (num < DespawnRange)
			{
				return;
			}
			for (int num2 = m_spawnedSosigs.Count - 1; num2 >= 0; num2--)
			{
				Vector3 from = m_spawnedSosigs[num2].transform.position - GM.CurrentPlayerBody.transform.position;
				float num3 = Vector3.Angle(from, GM.CurrentPlayerBody.Head.forward);
				if (num3 > 90f)
				{
					DespawnSosig(m_spawnedSosigs[num2]);
					m_spawnedSosigs.RemoveAt(num2);
				}
			}
		}

		private void DespawnSosig(Sosig s)
		{
			for (int i = 0; i < s.Links.Count; i++)
			{
				s.DestroyAllHeldObjects();
				if (s.Links[i] != null)
				{
					UnityEngine.Object.Destroy(s.Links[i].gameObject);
				}
				UnityEngine.Object.Destroy(s.gameObject);
				m_spawnedSofar--;
			}
		}

		private float DistanceFromClosestPointOnZone(Transform volume)
		{
			return Vector3.Distance(new Bounds(volume.position, volume.localScale).ClosestPoint(GM.CurrentPlayerBody.transform.position), GM.CurrentPlayerBody.transform.position);
		}

		private bool IsUsefulPoint(Vector3 p, Vector3 playerPos, Vector3 facing)
		{
			float num = Vector3.Distance(p, playerPos);
			if (num < SpawnRange.x || num > SpawnRange.y)
			{
				return false;
			}
			if (!CanSpawnInView)
			{
				Vector3 from = p - playerPos;
				float num2 = Vector3.Angle(from, facing);
				if (num2 < 60f)
				{
					return false;
				}
			}
			return true;
		}

		private void SpawnEnemy(SosigEnemyTemplate t, Transform point, int IFF)
		{
			Sosig item = SpawnEnemySosig(t, point.position, point.forward, IFF);
			m_spawnedSosigs.Add(item);
		}

		private Sosig SpawnEnemySosig(SosigEnemyTemplate template, Vector3 position, Vector3 forward, int IFF)
		{
			FVRObject fVRObject = template.SosigPrefabs[UnityEngine.Random.Range(0, template.SosigPrefabs.Count)];
			SosigConfigTemplate t = template.ConfigTemplates[UnityEngine.Random.Range(0, template.ConfigTemplates.Count)];
			SosigOutfitConfig w = template.OutfitConfig[UnityEngine.Random.Range(0, template.OutfitConfig.Count)];
			Sosig sosig = SpawnSosigAndConfigureSosig(fVRObject.GetGameObject(), position, Quaternion.LookRotation(forward, Vector3.up), t, w);
			sosig.InitHands();
			sosig.Inventory.Init();
			sosig.Inventory.FillAllAmmo();
			sosig.E.IFFCode = IFF;
			if (template.WeaponOptions.Count > 0)
			{
				SosigWeapon sosigWeapon = SpawnWeapon(template.WeaponOptions);
				sosigWeapon.SetAutoDestroy(b: true);
				sosigWeapon.SetAmmoClamping(b: true);
				sosigWeapon.O.SpawnLockable = false;
				sosig.ForceEquip(sosigWeapon);
			}
			bool flag = false;
			float num = UnityEngine.Random.Range(0f, 1f);
			if (num <= template.SecondaryChance)
			{
				flag = true;
			}
			if (template.WeaponOptions_Secondary.Count > 0 && flag)
			{
				SosigWeapon sosigWeapon2 = SpawnWeapon(template.WeaponOptions_Secondary);
				sosigWeapon2.SetAutoDestroy(b: true);
				sosigWeapon2.SetAmmoClamping(b: true);
				sosigWeapon2.O.SpawnLockable = false;
				sosig.ForceEquip(sosigWeapon2);
			}
			bool flag2 = false;
			num = UnityEngine.Random.Range(0f, 1f);
			if (num <= template.TertiaryChance)
			{
				flag2 = true;
			}
			if (template.WeaponOptions_Tertiary.Count > 0 && flag2)
			{
				SosigWeapon sosigWeapon3 = SpawnWeapon(template.WeaponOptions_Tertiary);
				sosigWeapon3.SetAutoDestroy(b: true);
				sosigWeapon3.SetAmmoClamping(b: true);
				sosigWeapon3.O.SpawnLockable = false;
				sosig.ForceEquip(sosigWeapon3);
			}
			if (IsPatrolZone)
			{
				sosig.CurrentOrder = Sosig.SosigOrder.Assault;
				sosig.FallbackOrder = Sosig.SosigOrder.Assault;
				sosig.CommandAssaultPoint(PatrolPoints[0].position);
			}
			else
			{
				sosig.CurrentOrder = Sosig.SosigOrder.Wander;
				sosig.FallbackOrder = Sosig.SosigOrder.Wander;
			}
			if (UsesSpawnEffect)
			{
				UnityEngine.Object.Instantiate(SpawnEffect, position, Quaternion.identity);
			}
			return sosig;
		}

		private Sosig SpawnSosigAndConfigureSosig(GameObject prefab, Vector3 pos, Quaternion rot, SosigConfigTemplate t, SosigOutfitConfig w)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, pos, rot);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			float num = 0f;
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Headwear)
			{
				SpawnAccesoryToLink(w.Headwear, componentInChildren.Links[0]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Facewear)
			{
				SpawnAccesoryToLink(w.Facewear, componentInChildren.Links[0]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Eyewear)
			{
				SpawnAccesoryToLink(w.Eyewear, componentInChildren.Links[0]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Torsowear)
			{
				SpawnAccesoryToLink(w.Torsowear, componentInChildren.Links[1]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Pantswear)
			{
				SpawnAccesoryToLink(w.Pantswear, componentInChildren.Links[2]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Pantswear_Lower)
			{
				SpawnAccesoryToLink(w.Pantswear_Lower, componentInChildren.Links[3]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Backpacks)
			{
				SpawnAccesoryToLink(w.Backpacks, componentInChildren.Links[1]);
			}
			if (t.UsesLinkSpawns)
			{
				for (int i = 0; i < componentInChildren.Links.Count; i++)
				{
					float num2 = UnityEngine.Random.Range(0f, 1f);
					if (num2 < t.LinkSpawnChance[i])
					{
						componentInChildren.Links[i].RegisterSpawnOnDestroy(t.LinkSpawns[i]);
					}
				}
			}
			componentInChildren.Configure(t);
			return componentInChildren;
		}

		private SosigWeapon SpawnWeapon(List<FVRObject> o)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(o[UnityEngine.Random.Range(0, o.Count)].GetGameObject(), new Vector3(0f, 30f, 0f), Quaternion.identity);
			return gameObject.GetComponent<SosigWeapon>();
		}

		private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(gs[UnityEngine.Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
			gameObject.transform.SetParent(l.transform);
			SosigWearable component = gameObject.GetComponent<SosigWearable>();
			component.RegisterWearable(l);
		}

		private void IncrementNumOfMineWhoveDied()
		{
			m_numOfMineWhoveDied++;
			if (UsesMaxDiedOfMine && !m_hasHitMaxDiedOfMineThreshold && m_numOfMineWhoveDied >= MaxDiedOfMineThreshold)
			{
				m_hasHitMaxDiedOfMineThreshold = true;
				GM.ZMaster.FlagM.SetFlag(FlagSetWhenMaxDiedOfMine, FlagValueSetWhenMaxDiedOfMine);
				if (GM.ZMaster.IsVerboseDebug)
				{
					Debug.Log("Set flag: " + FlagSetWhenMaxDiedOfMine + " to " + FlagValueSetWhenMaxDiedOfMine + " because " + base.gameObject.name + " hit its kill-X-Sosigs threshold");
				}
			}
		}

		public void CheckIfDeadSosigWasMine(Sosig s)
		{
			if (m_spawnedSosigs.Count < 1 || !m_spawnedSosigs.Contains(s))
			{
				return;
			}
			if (s.GetDiedFromIFF() == 0)
			{
				switch (s.GetDiedFromClass())
				{
				case Damage.DamageClass.Projectile:
					GM.ZMaster.FlagM.AddToFlag("s_g", 1);
					break;
				case Damage.DamageClass.Melee:
					GM.ZMaster.FlagM.AddToFlag("s_m", 1);
					break;
				}
			}
			IncrementNumOfMineWhoveDied();
			s.TickDownToClear(5f);
			m_spawnedSosigs.Remove(s);
		}

		public bool IsPlayerInAnyVolumes(Vector3 p)
		{
			bool result = false;
			for (int i = 0; i < VolumesToCheck.Count; i++)
			{
				if (TestVolumeBool(VolumesToCheck[i], p))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool TestVolumeBool(Transform t, Vector3 pos)
		{
			bool result = true;
			Vector3 vector = t.InverseTransformPoint(pos);
			if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
			{
				result = false;
			}
			return result;
		}
	}
}
