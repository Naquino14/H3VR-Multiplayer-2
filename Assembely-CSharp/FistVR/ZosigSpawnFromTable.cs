using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigSpawnFromTable : MonoBehaviour
	{
		[Header("Flags")]
		public bool NeedsFlag;

		public string FlagNeeded;

		public int FlagNeededValue = 1;

		public bool FlagNeedsEquality = true;

		public bool HasBlockingFlag;

		public string BlockingFlag;

		[Header("Objects")]
		public FVRObject Item;

		public bool UsesTable;

		public List<ZosigItemSpawnTable> Tables;

		public float Incidence = 1f;

		public int MinItems = 1;

		public int MaxItems = 2;

		public bool StripOnSpawn;

		[Header("TrackAnRespawn")]
		public bool DoesTrackRespawn;

		public float RespawnRange = 100f;

		public float RespawnCooldown = 200f;

		private float m_respawnCoolDown = 200f;

		private float m_respawnCheckTick = 1f;

		private GameObject m_respawnTrack;

		[Header("Container")]
		public bool UsesContainer;

		public FVRObject ContainerPrefab;

		public bool UseContainerFlag;

		public string ContainerFlag;

		public int ContainterFlagValue;

		[Header("SpawnPositions")]
		public bool UsesSpawnPoints;

		public List<Transform> SpawnPoints;

		[Header("Details")]
		public int Num_Mags = 2;

		public int Num_Rounds = 4;

		public bool IsReloadamaticAmmoDefault = true;

		public int MinAmmo = -1;

		public int MaxAmmo = 30;

		[Header("BuyBuddy Stuff")]
		public ObjectTableDef BuyBuddyTable;

		public bool BuyBuddyIsLargeCase;

		public string BuyBuddyUnlockFlag = "BuyBuddy_Test";

		public List<int> BuyBuddyPrice;

		private bool m_hasSpawned;

		public ZosigFlagManager F;

		public void Init()
		{
			F = GM.ZMaster.FlagM;
		}

		public void Update()
		{
			if (!DoesTrackRespawn || !m_hasSpawned)
			{
				return;
			}
			if (m_respawnCoolDown > 0f)
			{
				m_respawnCoolDown -= Time.deltaTime;
				return;
			}
			if (m_respawnCheckTick > 0f)
			{
				m_respawnCheckTick -= Time.deltaTime;
				return;
			}
			m_respawnCheckTick = 5f;
			float num = 0f;
			num = ((!(m_respawnTrack == null)) ? Vector3.Distance(m_respawnTrack.transform.position, base.transform.position) : Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position));
			if (num > RespawnRange)
			{
				m_hasSpawned = false;
			}
		}

		public void SpawnKernel()
		{
			if (m_hasSpawned || (HasBlockingFlag && F.GetFlagValue(BlockingFlag) > 0) || (NeedsFlag && F.GetFlagValue(FlagNeeded) != FlagNeededValue && FlagNeedsEquality) || (NeedsFlag && F.GetFlagValue(FlagNeeded) < FlagNeededValue && !FlagNeedsEquality))
			{
				return;
			}
			if (UsesSpawnPoints)
			{
				float num = Random.Range(0f, 0.99f);
				if (num <= Incidence)
				{
					int num2 = Random.Range(MinItems, MaxItems + 1);
					SpawnPoints.Shuffle();
					SpawnPoints.Shuffle();
					for (int i = 0; i < num2; i++)
					{
						AnvilManager.Run(SpawnObject(SpawnPoints[i].position, SpawnPoints[i].rotation));
					}
				}
			}
			else
			{
				AnvilManager.Run(SpawnObject(base.transform.position, base.transform.rotation));
			}
			if (GM.ZMaster.IsVerboseDebug)
			{
				Debug.Log("Spawning:" + base.gameObject.name);
			}
			m_hasSpawned = true;
		}

		private void StripObject(GameObject g)
		{
			Component[] componentsInChildren = g.GetComponentsInChildren<Component>();
			for (int num = componentsInChildren.Length - 1; num >= 0; num--)
			{
				if (!(componentsInChildren[num] is MeshRenderer) && !(componentsInChildren[num] is MeshFilter) && !(componentsInChildren[num] is Transform) && !(componentsInChildren[num] is Collider) && !(componentsInChildren[num] is AudioSource))
				{
					Object.Destroy(componentsInChildren[num]);
				}
			}
		}

		private IEnumerator SpawnObject(Vector3 pos, Quaternion rot)
		{
			FVRObject ItemToSpawn2 = null;
			FVRObject ItemToSpawnIntoContainer_1 = null;
			if (UsesContainer)
			{
				ItemToSpawn2 = ContainerPrefab;
				if (UsesTable)
				{
					ZosigItemSpawnTable zosigItemSpawnTable = Tables[Random.Range(0, Tables.Count)];
					int index = Random.Range(0, zosigItemSpawnTable.Objects.Count);
					ItemToSpawnIntoContainer_1 = zosigItemSpawnTable.Objects[index];
				}
				else
				{
					ItemToSpawnIntoContainer_1 = Item;
				}
			}
			else if (UsesTable)
			{
				ZosigItemSpawnTable zosigItemSpawnTable2 = Tables[Random.Range(0, Tables.Count)];
				ItemToSpawn2 = zosigItemSpawnTable2.Objects[Random.Range(0, zosigItemSpawnTable2.Objects.Count)];
			}
			else
			{
				ItemToSpawn2 = Item;
			}
			yield return ItemToSpawn2.GetGameObjectAsync();
			GameObject g = Object.Instantiate(ItemToSpawn2.GetGameObject(), pos, rot);
			if (DoesTrackRespawn)
			{
				m_respawnTrack = g;
				m_respawnCoolDown = RespawnCooldown;
				m_respawnCheckTick = 1f;
			}
			if (StripOnSpawn)
			{
				StripObject(g);
			}
			if (g.GetComponent<Reloadamatic>() != null)
			{
				Reloadamatic component = g.GetComponent<Reloadamatic>();
				component.SetSpawnsDefault(IsReloadamaticAmmoDefault);
			}
			if (g.GetComponent<ZosigContainer>() != null)
			{
				ZosigContainer component2 = g.GetComponent<ZosigContainer>();
				component2.PlaceObjectsInContainer(ItemToSpawnIntoContainer_1);
				if (UseContainerFlag)
				{
					component2.SetFlagOnOpenDestroy = true;
					component2.FlagToSet = ContainerFlag;
					component2.ValueToSet = ContainterFlagValue;
				}
			}
			if (g.GetComponent<BuyBuddy>() != null)
			{
				BuyBuddy component3 = g.GetComponent<BuyBuddy>();
				component3.ConfigureBuddy(BuyBuddyTable, BuyBuddyIsLargeCase, BuyBuddyUnlockFlag, BuyBuddyPrice);
			}
		}
	}
}
