using UnityEngine;

namespace FistVR
{
	public class RotwienerNest_Tendril : MonoBehaviour
	{
		public enum TendrilState
		{
			Protected,
			Exposed,
			Destroyed
		}

		public GameObject Prefab_Nodule;

		public Transform Point_Nodule;

		private RotwienerNest_Nodule n_spawnedNodule;

		public float NoduleSize = 1f;

		public GameObject Geo_Whole;

		public GameObject Geo_Severed;

		public TendrilState State;

		public RotwienerNest_Tendril ChildTendril;

		private RotwienerNest_Tendril m_parent;

		public void SetTendrilParent()
		{
		}

		public void Init(RotwienerNest_Tendril parent, TendrilState state)
		{
			m_parent = parent;
			if (ChildTendril != null)
			{
				ChildTendril.Init(this, state);
			}
			switch (state)
			{
			case TendrilState.Protected:
				SetState(TendrilState.Protected, isInit: true);
				break;
			case TendrilState.Destroyed:
				SetState(TendrilState.Destroyed, isInit: true);
				break;
			}
		}

		public void SetState(TendrilState s, bool isInit)
		{
			State = s;
			switch (s)
			{
			case TendrilState.Protected:
				Geo_Whole.SetActive(value: true);
				Geo_Severed.SetActive(value: false);
				if (isInit)
				{
					GameObject gameObject = Object.Instantiate(Prefab_Nodule, Point_Nodule.position, Point_Nodule.rotation);
					RotwienerNest_Nodule component = gameObject.GetComponent<RotwienerNest_Nodule>();
					gameObject.transform.localScale = new Vector3(NoduleSize, NoduleSize, NoduleSize);
					n_spawnedNodule = component;
					n_spawnedNodule.SetType(RotwienerNest_Nodule.NoduleType.Tendril, null, this);
				}
				if (n_spawnedNodule != null)
				{
					n_spawnedNodule.SetState(RotwienerNest_Nodule.NoduleState.Protected, isInit);
				}
				break;
			case TendrilState.Exposed:
				Geo_Whole.SetActive(value: true);
				Geo_Severed.SetActive(value: false);
				if (n_spawnedNodule != null)
				{
					n_spawnedNodule.SetState(RotwienerNest_Nodule.NoduleState.Unprotected, isInit);
				}
				break;
			case TendrilState.Destroyed:
				Geo_Whole.SetActive(value: false);
				Geo_Severed.SetActive(value: true);
				if (n_spawnedNodule != null)
				{
					n_spawnedNodule.SetState(RotwienerNest_Nodule.NoduleState.Destroyed, isInit);
				}
				break;
			}
		}

		public void Update()
		{
			if (State == TendrilState.Protected && (ChildTendril == null || ChildTendril.State == TendrilState.Destroyed))
			{
				SetState(TendrilState.Exposed, isInit: false);
			}
		}

		public void NoduleDestroyed(RotwienerNest_Nodule n)
		{
			n_spawnedNodule = null;
			Object.Destroy(n.gameObject);
			SetState(TendrilState.Destroyed, isInit: false);
		}

		public bool CanSupplyNodulePower()
		{
			if (State == TendrilState.Destroyed)
			{
				return false;
			}
			return true;
		}
	}
}
