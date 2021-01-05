using UnityEngine;

namespace FistVR
{
	public class ZosigDestroyable : MonoBehaviour, IFVRDamageable
	{
		public string AssociatedFlag;

		public int AssociatedFlagValue = 1;

		public GameObject Visuals_UnDestroyed;

		public GameObject Visuals_Destroyed;

		[Header("Damage Stuff")]
		public float Integrity = 30000f;

		private bool m_isDestroyed;

		private ZosigGameManager M;

		public GameObject OnDestroyPrefab;

		public Transform OnDestroyPoint;

		public void Init(ZosigGameManager m)
		{
			M = m;
			InitializeFromFlagM();
		}

		private void InitializeFromFlagM()
		{
			if (M.FlagM.GetFlagValue(AssociatedFlag) >= AssociatedFlagValue)
			{
				m_isDestroyed = true;
				SetDestroyedVisual(b: true);
				if (GM.ZMaster.IsVerboseDebug)
				{
					Debug.Log(base.gameObject.name + " set to destroyed by flag:" + AssociatedFlag);
				}
			}
		}

		private void SetDestroyedVisual(bool b)
		{
			if (b)
			{
				if (Visuals_Destroyed != null)
				{
					Visuals_Destroyed.SetActive(value: true);
				}
				if (Visuals_UnDestroyed != null)
				{
					Visuals_UnDestroyed.SetActive(value: false);
				}
			}
			else
			{
				if (Visuals_Destroyed != null)
				{
					Visuals_Destroyed.SetActive(value: false);
				}
				if (Visuals_UnDestroyed != null)
				{
					Visuals_UnDestroyed.SetActive(value: true);
				}
			}
		}

		public void Damage(Damage d)
		{
			if (!m_isDestroyed)
			{
				float num = d.Dam_TotalKinetic;
				if (d.Class == FistVR.Damage.DamageClass.Explosive)
				{
					num *= 300f;
				}
				if (d.Class == FistVR.Damage.DamageClass.Melee)
				{
					num *= 0.01f;
				}
				Integrity -= num;
				if (Integrity < 0f)
				{
					DestroyMe();
				}
			}
		}

		private void DestroyMe()
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				SetDestroyedVisual(b: true);
				if (M.FlagM.GetFlagValue(AssociatedFlag) < AssociatedFlagValue)
				{
					M.FlagM.SetFlag(AssociatedFlag, AssociatedFlagValue);
				}
				if (OnDestroyPrefab != null)
				{
					Object.Instantiate(OnDestroyPrefab, OnDestroyPoint.position, OnDestroyPoint.rotation);
				}
			}
		}
	}
}
