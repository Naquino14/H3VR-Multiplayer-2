using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TAH_WeaponCrate : MonoBehaviour
	{
		public TAH_Manager Manager;

		public GunCase_Cover Cover;

		public List<GunCase_Latch> Latches;

		public Transform Point_Gun;

		public Transform Point_Mag;

		public Transform Point_Attachment1;

		public Transform Point_Attachment2;

		public Transform Point_Attachment3;

		private bool m_isOpen;

		private bool m_containsItems;

		private List<GameObject> SpawnedStuff = new List<GameObject>();

		private GameObject m_storedPrefab_Gun;

		private GameObject m_storedPrefab_Mag;

		private GameObject m_storedPrefab_Attachment1;

		private GameObject m_storedPrefab_Attachment2;

		private GameObject m_storedPrefab_Attachment3;

		public void ResetCrate()
		{
			ClearUnusedSpawnsFromWorld();
			Cover.Reset();
			Latches[0].Reset();
			Latches[1].Reset();
			m_storedPrefab_Gun = null;
			m_storedPrefab_Mag = null;
			m_storedPrefab_Attachment1 = null;
			m_storedPrefab_Attachment2 = null;
			m_storedPrefab_Attachment3 = null;
			m_isOpen = false;
			m_containsItems = false;
		}

		private void Update()
		{
			if (!m_isOpen && m_containsItems && Latches[0].IsOpen() && Latches[1].IsOpen())
			{
				SpawnItemsInCrate();
			}
		}

		public void PlaceItemsInCrate(GameObject go_gun, GameObject go_mag, GameObject go_attach1, GameObject go_attach2, GameObject go_attach3)
		{
			m_containsItems = true;
			m_storedPrefab_Gun = go_gun;
			m_storedPrefab_Mag = go_mag;
			m_storedPrefab_Attachment1 = go_attach1;
			m_storedPrefab_Attachment2 = go_attach2;
			m_storedPrefab_Attachment3 = go_attach3;
		}

		private void SpawnItemsInCrate()
		{
			if (!m_containsItems)
			{
				return;
			}
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			GameObject gameObject3 = null;
			GameObject gameObject4 = null;
			GameObject gameObject5 = null;
			if (m_storedPrefab_Gun != null)
			{
				gameObject = Object.Instantiate(m_storedPrefab_Gun, Point_Gun.position, Point_Gun.rotation);
			}
			if (m_storedPrefab_Mag != null)
			{
				gameObject2 = Object.Instantiate(m_storedPrefab_Mag, Point_Mag.position, Point_Mag.rotation);
				FVRFireArmMagazine component = gameObject2.GetComponent<FVRFireArmMagazine>();
				if (component != null && component.RoundType != FireArmRoundType.aFlameThrowerFuel)
				{
					component.ReloadMagWithType(AM.GetRandomValidRoundClass(component.RoundType));
				}
			}
			if (m_storedPrefab_Attachment1 != null)
			{
				gameObject3 = Object.Instantiate(m_storedPrefab_Attachment1, Point_Attachment1.position, Point_Attachment1.rotation);
			}
			if (m_storedPrefab_Attachment2 != null)
			{
				gameObject4 = Object.Instantiate(m_storedPrefab_Attachment2, Point_Attachment2.position, Point_Attachment2.rotation);
			}
			if (m_storedPrefab_Attachment3 != null)
			{
				gameObject5 = Object.Instantiate(m_storedPrefab_Attachment3, Point_Attachment3.position, Point_Attachment3.rotation);
			}
			if (gameObject != null)
			{
				Manager.AddObjectToTrackedList(gameObject);
			}
			if (gameObject2 != null)
			{
				Manager.AddObjectToTrackedList(gameObject2);
			}
			if (gameObject3 != null)
			{
				Manager.AddObjectToTrackedList(gameObject3);
			}
			if (gameObject4 != null)
			{
				Manager.AddObjectToTrackedList(gameObject4);
			}
			if (gameObject5 != null)
			{
				Manager.AddObjectToTrackedList(gameObject5);
			}
			m_containsItems = false;
		}

		public void ClearUnusedSpawnsFromWorld()
		{
			for (int num = SpawnedStuff.Count - 1; num >= 0; num--)
			{
				if (!(SpawnedStuff[num] == null))
				{
					float num2 = Vector3.Distance(SpawnedStuff[num].transform.position, GM.CurrentPlayerBody.transform.position);
					if (num2 > 12f)
					{
						Object.Destroy(SpawnedStuff[num]);
						SpawnedStuff.RemoveAt(num);
					}
				}
			}
		}
	}
}
