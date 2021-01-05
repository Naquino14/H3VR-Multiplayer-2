using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigContainer_WeaponCase : ZosigContainer
	{
		public GunCase_Cover Cover;

		public List<GunCase_Latch> Latches;

		public Transform Point_Gun;

		public List<Transform> Points_MagClipSpeedloader;

		public List<Transform> Points_Cartridge;

		public List<Transform> Points_Attachment;

		private bool m_hasSpawnedContents;

		public bool UsesLatches = true;

		private FVRObject m_storedObject_gun;

		private FVRObject m_storedObject_attachA;

		private FVRObject m_storedObject_attachB;

		private FVRObject m_magazineClipSpeedLoaderRound;

		private int m_numClipSpeedLoaderRound;

		public override void PlaceObjectsInContainer(FVRObject obj1, int minAmmo = -1, int maxAmmo = 30)
		{
			base.PlaceObjectsInContainer(obj1);
			if (obj1 != null)
			{
				LoadIntoCrate(obj1, minAmmo, maxAmmo);
			}
		}

		public void LoadIntoCrate(FVRObject obj, int minAmmo, int maxAmmo)
		{
			FVRObject fVRObject = null;
			int numClipSpeedLoaderRound = 0;
			if (obj == null)
			{
				base.gameObject.name = "case" + Random.Range(0, 314);
				Debug.Log("oh shit null case:" + base.gameObject.name);
			}
			if (obj.Category == FVRObject.ObjectCategory.Firearm)
			{
				fVRObject = obj.GetRandomAmmoObject(obj, null, minAmmo, maxAmmo);
				numClipSpeedLoaderRound = ((!(fVRObject != null) || fVRObject.Category != FVRObject.ObjectCategory.Cartridge) ? 2 : 5);
			}
			FVRObject gun = null;
			FVRObject requiredAttachment_A = null;
			FVRObject requiredAttachment_B = null;
			if (obj.Category == FVRObject.ObjectCategory.Attachment)
			{
				requiredAttachment_A = obj;
				if (obj.RequiredSecondaryPieces.Count > 0)
				{
					requiredAttachment_B = obj.RequiredSecondaryPieces[0];
				}
			}
			else
			{
				gun = obj;
			}
			PlaceWeaponInContainer(gun, requiredAttachment_A, requiredAttachment_B, fVRObject, numClipSpeedLoaderRound);
		}

		private void PlaceWeaponInContainer(FVRObject gun, FVRObject requiredAttachment_A, FVRObject requiredAttachment_B, FVRObject magazineClipSpeedLoaderRound, int numClipSpeedLoaderRound)
		{
			m_storedObject_gun = gun;
			m_storedObject_attachA = requiredAttachment_A;
			m_storedObject_attachB = requiredAttachment_B;
			m_magazineClipSpeedLoaderRound = magazineClipSpeedLoaderRound;
			m_numClipSpeedLoaderRound = numClipSpeedLoaderRound;
			m_containsItems = true;
		}

		public void SetMagMinMax(int min, int max)
		{
		}

		private void Start()
		{
			if (!UsesLatches)
			{
				Cover.ForceOpen();
			}
		}

		private void Update()
		{
			if (m_isOpen || !m_containsItems)
			{
				return;
			}
			if (UsesLatches)
			{
				if (Latches[0].IsOpen() && Latches[1].IsOpen() && !m_hasSpawnedContents)
				{
					if (GM.ZMaster != null)
					{
						GM.ZMaster.FlagM.AddToFlag("s_l", 1);
					}
					m_hasSpawnedContents = true;
					AnvilManager.Run(SpawnItemsInCrate());
				}
			}
			else if (Cover.IsHeld && !m_hasSpawnedContents)
			{
				m_hasSpawnedContents = true;
				AnvilManager.Run(SpawnItemsInCrate());
			}
		}

		private IEnumerator SpawnItemsInCrate()
		{
			FlagOpen();
			if (m_storedObject_gun != null)
			{
				yield return m_storedObject_gun.GetGameObjectAsync();
				GameObject Ggun = Object.Instantiate(m_storedObject_gun.GetGameObject(), Point_Gun.position, Point_Gun.rotation);
			}
			if (m_storedObject_attachA != null)
			{
				yield return m_storedObject_attachA.GetGameObjectAsync();
				GameObject Gattach1 = Object.Instantiate(m_storedObject_attachA.GetGameObject(), Points_Attachment[0].position, Points_Attachment[0].rotation);
			}
			if (m_storedObject_attachB != null)
			{
				yield return m_storedObject_attachB.GetGameObjectAsync();
				GameObject Gattach2 = Object.Instantiate(m_storedObject_attachB.GetGameObject(), Points_Attachment[1].position, Points_Attachment[1].rotation);
			}
			if (!(m_magazineClipSpeedLoaderRound != null))
			{
				yield break;
			}
			yield return m_magazineClipSpeedLoaderRound.GetGameObjectAsync();
			for (int i = 0; i < m_numClipSpeedLoaderRound; i++)
			{
				switch (m_magazineClipSpeedLoaderRound.Category)
				{
				case FVRObject.ObjectCategory.Cartridge:
					Object.Instantiate(m_magazineClipSpeedLoaderRound.GetGameObject(), Points_Cartridge[i].position, Points_Cartridge[i].rotation);
					break;
				case FVRObject.ObjectCategory.Clip:
					Object.Instantiate(m_magazineClipSpeedLoaderRound.GetGameObject(), Points_MagClipSpeedloader[i].position, Points_MagClipSpeedloader[i].rotation);
					break;
				case FVRObject.ObjectCategory.Magazine:
					Object.Instantiate(m_magazineClipSpeedLoaderRound.GetGameObject(), Points_MagClipSpeedloader[i].position, Points_MagClipSpeedloader[i].rotation);
					break;
				case FVRObject.ObjectCategory.SpeedLoader:
					Object.Instantiate(m_magazineClipSpeedLoaderRound.GetGameObject(), Points_MagClipSpeedloader[i].position, Points_MagClipSpeedloader[i].rotation);
					break;
				}
			}
		}
	}
}
