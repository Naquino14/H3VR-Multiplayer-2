using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RotwienerNest : ZosigQuestManager
	{
		public enum NestState
		{
			Protected,
			Exposed,
			Destroyed
		}

		public List<GameObject> Prefab_Nodules_Final;

		public List<Transform> NoduleSpawnPoints;

		private List<RotwienerNest_Nodule> m_finalNodules = new List<RotwienerNest_Nodule>();

		public List<RotwienerNest_Tendril> RootTendrils;

		public NestState State;

		[Header("Flag details")]
		public string AssociatedFlag;

		public int AssociatedFlagValue = 1;

		private bool m_isDestroyed;

		private ZosigGameManager M;

		[Header("StateGeo")]
		public GameObject Geo_Protected;

		public GameObject Geo_Exposed;

		public GameObject Geo_Destroyed;

		public Transform Bounce_Outer;

		public Transform Bounce_Inner;

		[Header("DestructionEvent")]
		public List<Transform> DestructionSpawnPoint;

		public List<GameObject> DestructionSpawns;

		private float m_bounceX;

		private float m_bounceY;

		private float m_bounceZ;

		private float m_checkTick = 0.1f;

		private void Bounce()
		{
			if (State == NestState.Protected)
			{
				m_bounceX = Mathf.Repeat(m_bounceX + Time.deltaTime * 0.3f, 1f);
				m_bounceY = Mathf.Repeat(m_bounceY + Time.deltaTime * 0.3f, 1f);
				m_bounceZ = Mathf.Repeat(m_bounceZ + Time.deltaTime * 0.3f, 1f);
				Vector3 localScale = new Vector3(1f + Mathf.Sin(m_bounceX * (float)Math.PI * 2f) * 0.025f, 1f + Mathf.Sin(m_bounceY * (float)Math.PI * 2f) * 0.025f, 1f + Mathf.Sin(m_bounceZ * (float)Math.PI * 2f) * 0.025f);
				Bounce_Outer.localScale = localScale;
			}
			else if (State == NestState.Exposed)
			{
				m_bounceX = Mathf.Repeat(m_bounceX + Time.deltaTime * 0.5f, 1f);
				m_bounceY = Mathf.Repeat(m_bounceY + Time.deltaTime * 0.5f, 1f);
				m_bounceZ = Mathf.Repeat(m_bounceZ + Time.deltaTime * 0.5f, 1f);
				Vector3 localScale2 = new Vector3(1f + Mathf.Sin(m_bounceX * (float)Math.PI * 2f) * 0.025f, 1f + Mathf.Sin(m_bounceY * (float)Math.PI * 2f) * 0.05f, 1f + Mathf.Sin(m_bounceZ * (float)Math.PI * 2f) * 0.025f);
				Bounce_Inner.localScale = localScale2;
			}
		}

		public override void Init(ZosigGameManager m)
		{
			M = m;
			InitializeFromFlagM();
			m_bounceY = 0.33f;
			m_bounceZ = 0.66f;
		}

		private void Test1()
		{
			SetState(NestState.Exposed, isInit: false);
		}

		private void Test2()
		{
			SetState(NestState.Destroyed, isInit: false);
		}

		private void InitializeFromFlagM()
		{
			if (M.FlagM.GetFlagValue(AssociatedFlag) >= AssociatedFlagValue)
			{
				if (GM.ZMaster.IsVerboseDebug)
				{
					Debug.Log(base.gameObject.name + " set to destroyed by flag:" + AssociatedFlag);
				}
				for (int i = 0; i < RootTendrils.Count; i++)
				{
					RootTendrils[i].Init(null, RotwienerNest_Tendril.TendrilState.Destroyed);
				}
				SetState(NestState.Destroyed, isInit: true);
			}
			else
			{
				for (int j = 0; j < RootTendrils.Count; j++)
				{
					RootTendrils[j].Init(null, RotwienerNest_Tendril.TendrilState.Protected);
				}
				SetState(NestState.Protected, isInit: true);
			}
		}

		private void SetState(NestState s, bool isInit)
		{
			if (!isInit && State == s)
			{
				return;
			}
			State = s;
			switch (s)
			{
			case NestState.Protected:
				Geo_Protected.SetActive(value: true);
				Geo_Exposed.SetActive(value: false);
				Geo_Destroyed.SetActive(value: false);
				break;
			case NestState.Exposed:
			{
				Geo_Protected.SetActive(value: false);
				Geo_Exposed.SetActive(value: true);
				Geo_Destroyed.SetActive(value: false);
				for (int j = 0; j < NoduleSpawnPoints.Count; j++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(Prefab_Nodules_Final[UnityEngine.Random.Range(0, Prefab_Nodules_Final.Count)], NoduleSpawnPoints[j].position, NoduleSpawnPoints[j].rotation);
					RotwienerNest_Nodule component = gameObject.GetComponent<RotwienerNest_Nodule>();
					component.SetState(RotwienerNest_Nodule.NoduleState.Naked, isInit);
					component.SetType(RotwienerNest_Nodule.NoduleType.Core, this, null);
					m_finalNodules.Add(component);
				}
				break;
			}
			case NestState.Destroyed:
				Geo_Protected.SetActive(value: false);
				Geo_Exposed.SetActive(value: false);
				Geo_Destroyed.SetActive(value: true);
				if (M.FlagM.GetFlagValue(AssociatedFlag) < AssociatedFlagValue)
				{
					M.FlagM.SetFlag(AssociatedFlag, AssociatedFlagValue);
				}
				if (!isInit)
				{
					for (int i = 0; i < DestructionSpawns.Count; i++)
					{
						UnityEngine.Object.Instantiate(DestructionSpawns[i], DestructionSpawnPoint[i].position, DestructionSpawnPoint[i].rotation);
					}
				}
				break;
			}
		}

		private void Update()
		{
			m_checkTick -= Time.deltaTime;
			if (m_checkTick <= 0f)
			{
				m_checkTick = UnityEngine.Random.Range(0.1f, 0.3f);
				CheckState();
			}
			Bounce();
		}

		private void CheckState()
		{
			if (State == NestState.Protected)
			{
				if (areAllTendrilsDestroyed())
				{
					SetState(NestState.Exposed, isInit: false);
				}
			}
			else if (State == NestState.Exposed && areAllNodulesDestroyed())
			{
				SetState(NestState.Destroyed, isInit: false);
			}
		}

		private bool areAllNodulesDestroyed()
		{
			if (m_finalNodules.Count < 1)
			{
				return true;
			}
			return false;
		}

		public void NoduleDestroyed(RotwienerNest_Nodule n)
		{
			m_finalNodules.Remove(n);
			UnityEngine.Object.Destroy(n.gameObject);
		}

		private bool areAllTendrilsDestroyed()
		{
			for (int i = 0; i < RootTendrils.Count; i++)
			{
				if (RootTendrils[i].CanSupplyNodulePower())
				{
					return false;
				}
			}
			return true;
		}
	}
}
