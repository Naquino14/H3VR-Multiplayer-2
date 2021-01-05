using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class WW_Checkpoint : MonoBehaviour
	{
		public List<ObjectTableDef> PoolsUsed;

		public TNH_WeaponCrate Crate;

		public bool HasMessage;

		public int MessageToUnlock = 30;

		public Transform SatDish;

		public float ActivationRange = 8f;

		public bool ForcePic;

		public void Init(ObjectTable requiredPic)
		{
			ObjectTableDef objectTableDef = PoolsUsed[Random.Range(0, PoolsUsed.Count)];
			ObjectTable objectTable = new ObjectTable();
			objectTable.Initialize(objectTableDef);
			FVRObject randomObject = objectTable.GetRandomObject();
			FVRObject fVRObject = null;
			int numClipSpeedLoaderRound = 0;
			FVRObject fVRObject2 = null;
			FVRObject requiredAttachment_B = null;
			if (randomObject.Category == FVRObject.ObjectCategory.Firearm)
			{
				fVRObject = randomObject.GetRandomAmmoObject(randomObject, objectTableDef.Eras, objectTableDef.MinAmmoCapacity, objectTableDef.MaxAmmoCapacity, objectTableDef.Sets);
				numClipSpeedLoaderRound = ((!(fVRObject != null) || fVRObject.Category != FVRObject.ObjectCategory.Cartridge) ? 2 : 2);
				if (randomObject.RequiresPicatinnySight || ForcePic)
				{
					fVRObject2 = requiredPic.GetRandomObject();
					if (fVRObject2.RequiredSecondaryPieces.Count > 0)
					{
						requiredAttachment_B = fVRObject2.RequiredSecondaryPieces[0];
					}
					else if (randomObject.BespokeAttachments.Count > 0)
					{
						fVRObject2 = randomObject.BespokeAttachments[Random.Range(0, randomObject.BespokeAttachments.Count)];
					}
				}
			}
			Crate.PlaceWeaponInContainer(randomObject, fVRObject2, requiredAttachment_B, fVRObject, numClipSpeedLoaderRound);
		}
	}
}
