using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TNH_WeaponCrate : MonoBehaviour
	{
		public GunCase_Cover Cover;

		public List<GunCase_Latch> Latches;

		public Transform Point_Gun;

		public List<Transform> Points_Attachment;

		public List<Transform> Points_MagClipSpeedloader;

		public List<Transform> Points_Cartridge;

		private bool m_hasSpawnedContents;

		protected bool m_isOpen;

		protected bool m_containsItems;

		private FVRObject m_storedObject_gun;

		private FVRObject m_storedObject_attachA;

		private FVRObject m_storedObject_attachB;

		private FVRObject m_magazineClipSpeedLoaderRound;

		private int m_numClipSpeedLoaderRound;

		public TNH_Manager M;

		private bool m_usesFVRObjects = true;

		private GameObject m_storedGO_gun;

		private GameObject m_storedGO_ammo;

		private GameObject m_storedGO_extra1;

		private GameObject m_storedGO_extra2;

		private void Start()
		{
		}

		public void PlaceWeaponInContainer(FVRObject gun, FVRObject requiredAttachment_A, FVRObject requiredAttachment_B, FVRObject magazineClipSpeedLoaderRound, int numClipSpeedLoaderRound)
		{
			m_storedObject_gun = gun;
			m_storedObject_attachA = requiredAttachment_A;
			m_storedObject_attachB = requiredAttachment_B;
			m_magazineClipSpeedLoaderRound = magazineClipSpeedLoaderRound;
			m_numClipSpeedLoaderRound = numClipSpeedLoaderRound;
			m_containsItems = true;
			m_usesFVRObjects = true;
		}

		public void PlaceWeaponInContainer(GameObject gun, GameObject ammo, GameObject extra1, GameObject extra2)
		{
			m_storedGO_gun = gun;
			m_storedGO_ammo = ammo;
			m_storedGO_extra1 = extra1;
			m_storedGO_extra2 = extra2;
			m_containsItems = true;
			m_usesFVRObjects = false;
		}

		private void Update()
		{
			if (!m_isOpen && m_containsItems && Latches[0].IsOpen() && Latches[1].IsOpen() && !m_hasSpawnedContents && m_containsItems)
			{
				m_hasSpawnedContents = true;
				AnvilManager.Run(SpawnItemsInCrate());
				SpawnObjectsRaw();
			}
		}

		private void SpawnObjectsRaw()
		{
			if (m_storedGO_gun != null)
			{
				GameObject gameObject = Object.Instantiate(m_storedGO_gun, Point_Gun.position, Point_Gun.rotation);
			}
			if (m_storedGO_ammo != null)
			{
				GameObject gameObject2 = Object.Instantiate(m_storedGO_ammo, Points_MagClipSpeedloader[0].position, Points_MagClipSpeedloader[0].rotation);
				GameObject gameObject3 = Object.Instantiate(m_storedGO_ammo, Points_MagClipSpeedloader[1].position, Points_MagClipSpeedloader[1].rotation);
				GameObject gameObject4 = Object.Instantiate(m_storedGO_ammo, Points_MagClipSpeedloader[2].position, Points_MagClipSpeedloader[2].rotation);
			}
			if (m_storedGO_extra1 != null)
			{
				GameObject gameObject5 = Object.Instantiate(m_storedGO_extra1, Points_Attachment[0].position, Points_Attachment[0].rotation);
			}
			if (m_storedGO_extra2 != null)
			{
				GameObject gameObject6 = Object.Instantiate(m_storedGO_extra1, Points_Attachment[1].position, Points_Attachment[1].rotation);
			}
		}

		private IEnumerator SpawnItemsInCrate()
		{
			if (m_storedObject_gun != null)
			{
				yield return m_storedObject_gun.GetGameObjectAsync();
				GameObject Ggun = Object.Instantiate(m_storedObject_gun.GetGameObject(), Point_Gun.position, Point_Gun.rotation);
				if (M != null)
				{
					M.AddObjectToTrackedList(Ggun);
				}
			}
			if (m_storedObject_attachA != null)
			{
				yield return m_storedObject_attachA.GetGameObjectAsync();
				GameObject Gattach1 = Object.Instantiate(m_storedObject_attachA.GetGameObject(), Points_Attachment[0].position, Points_Attachment[0].rotation);
				if (M != null)
				{
					M.AddObjectToTrackedList(Gattach1);
				}
			}
			if (m_storedObject_attachB != null)
			{
				yield return m_storedObject_attachB.GetGameObjectAsync();
				GameObject Gattach2 = Object.Instantiate(m_storedObject_attachB.GetGameObject(), Points_Attachment[1].position, Points_Attachment[1].rotation);
				if (M != null)
				{
					M.AddObjectToTrackedList(Gattach2);
				}
			}
			if (!(m_magazineClipSpeedLoaderRound != null))
			{
				yield break;
			}
			yield return m_magazineClipSpeedLoaderRound.GetGameObjectAsync();
			int num2 = m_numClipSpeedLoaderRound;
			if (m_storedObject_gun != null)
			{
				num2 = Mathf.Clamp(num2, m_storedObject_gun.MagazineCapacity, 8);
			}
			for (int i = 0; i < m_numClipSpeedLoaderRound; i++)
			{
				switch (m_magazineClipSpeedLoaderRound.Category)
				{
				case FVRObject.ObjectCategory.Cartridge:
				{
					GameObject g4 = Object.Instantiate(m_magazineClipSpeedLoaderRound.GetGameObject(), Points_Cartridge[i].position, Points_Cartridge[i].rotation);
					if (M != null)
					{
						M.AddObjectToTrackedList(g4);
					}
					break;
				}
				case FVRObject.ObjectCategory.Clip:
				{
					GameObject g2 = Object.Instantiate(m_magazineClipSpeedLoaderRound.GetGameObject(), Points_MagClipSpeedloader[i].position, Points_MagClipSpeedloader[i].rotation);
					if (M != null)
					{
						M.AddObjectToTrackedList(g2);
					}
					break;
				}
				case FVRObject.ObjectCategory.Magazine:
				{
					GameObject g3 = Object.Instantiate(m_magazineClipSpeedLoaderRound.GetGameObject(), Points_MagClipSpeedloader[i].position, Points_MagClipSpeedloader[i].rotation);
					if (M != null)
					{
						M.AddObjectToTrackedList(g3);
					}
					break;
				}
				case FVRObject.ObjectCategory.SpeedLoader:
				{
					GameObject g = Object.Instantiate(m_magazineClipSpeedLoaderRound.GetGameObject(), Points_MagClipSpeedloader[i].position, Points_MagClipSpeedloader[i].rotation);
					if (M != null)
					{
						M.AddObjectToTrackedList(g);
					}
					break;
				}
				}
			}
		}
	}
}
