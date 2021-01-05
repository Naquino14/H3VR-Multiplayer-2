using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class AutoMeaterHitZone : MonoBehaviour, IFVRDamageable
	{
		public AutoMeater M;

		public AutoMeater.AMHitZoneType Type;

		public float ArmorThreshold = 1500f;

		public float ArmorThresholdReductionRate = 0.1f;

		public float LifeUntilFailure = 5000f;

		public GameObject SpawnOnDestruction1;

		public GameObject SpawnOnDestruction2;

		private bool m_isDestroyed;

		public List<GameObject> DisableOnDestroy = new List<GameObject>();

		public bool DestroysGunOnDestruction;

		[Header("MagazineConnection")]
		public bool IsSpecificMagazine;

		public int FirearmIndex;

		public void Damage(Damage d)
		{
			float dam_TotalKinetic = d.Dam_TotalKinetic;
			float num = 1f;
			if (dam_TotalKinetic > ArmorThreshold)
			{
				dam_TotalKinetic -= ArmorThreshold;
				ArmorThreshold = 0f;
				LifeUntilFailure -= dam_TotalKinetic;
				num = 0.5f;
			}
			else
			{
				dam_TotalKinetic *= ArmorThresholdReductionRate;
				ArmorThreshold -= dam_TotalKinetic;
				num = 1f;
			}
			if (LifeUntilFailure < 0f && !m_isDestroyed)
			{
				m_isDestroyed = true;
				M.DestroyComponent(Type, SpawnOnDestruction1, SpawnOnDestruction2, base.transform, DestroysGunOnDestruction);
				for (int i = 0; i < DisableOnDestroy.Count; i++)
				{
					if (DisableOnDestroy[i] != null)
					{
						DisableOnDestroy[i].SetActive(value: false);
					}
				}
			}
			else
			{
				M.DamageEvent(base.transform.position - d.strikeDir, num, Type);
			}
		}

		public void BlowUp()
		{
			if (m_isDestroyed)
			{
				return;
			}
			m_isDestroyed = true;
			M.DestroyComponent(Type, SpawnOnDestruction1, SpawnOnDestruction2, base.transform, DestroysGunOnDestruction);
			for (int i = 0; i < DisableOnDestroy.Count; i++)
			{
				if (DisableOnDestroy[i] != null)
				{
					DisableOnDestroy[i].SetActive(value: false);
				}
			}
		}
	}
}
