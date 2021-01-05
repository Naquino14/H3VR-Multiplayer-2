using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MM_GunCase : MonoBehaviour
	{
		public GunCase_Cover Cover;

		public List<GunCase_Latch> Latches;

		public Transform Point_Gun;

		public Transform Point_Mag;

		public Transform Point_Attachment1;

		public Transform Point_Attachment2;

		public Transform Point_Attachment3;

		private bool m_isOpen;

		private bool m_containsItems;

		private GameObject m_storedPrefab_Gun;

		private GameObject m_storedPrefab_Mag;

		private GameObject m_storedPrefab_Attachment1;

		private GameObject m_storedPrefab_Attachment2;

		private GameObject m_storedPrefab_Attachment3;

		public bool AutoSpawn;

		public FVRObject FO_Gun;

		public FVRObject FO_Mag;

		public FVRObject FO_Attachment1;

		public FVRObject FO_Attachment2;

		public FVRObject FO_Attachment3;

		public void Start()
		{
			if (AutoSpawn)
			{
				m_containsItems = true;
				if (FO_Gun != null)
				{
					m_storedPrefab_Gun = FO_Gun.GetGameObject();
				}
				if (FO_Mag != null)
				{
					m_storedPrefab_Mag = FO_Mag.GetGameObject();
				}
				if (FO_Attachment1 != null)
				{
					m_storedPrefab_Attachment1 = FO_Attachment1.GetGameObject();
				}
				if (FO_Attachment2 != null)
				{
					m_storedPrefab_Attachment2 = FO_Attachment2.GetGameObject();
				}
				if (FO_Attachment3 != null)
				{
					m_storedPrefab_Attachment3 = FO_Attachment3.GetGameObject();
				}
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

		private void Update()
		{
			if (!m_isOpen && m_containsItems && Latches[0].IsOpen() && Latches[1].IsOpen())
			{
				SpawnItemsInCrate();
			}
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
			m_containsItems = false;
		}
	}
}
