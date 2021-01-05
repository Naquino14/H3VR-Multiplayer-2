using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FirearmSaver : MonoBehaviour
	{
		public ItemSpawnerUI ItemSpawner;

		public bool TryToScanGun()
		{
			Collider[] array = Physics.OverlapBox(base.transform.position, new Vector3(0.26f, 0.45f, 0.9f), base.transform.rotation);
			FVRFireArm fVRFireArm = null;
			Speedloader item = null;
			FVRFireArmClip item2 = null;
			FVRFireArmRound item3 = null;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].attachedRigidbody != null && array[i].attachedRigidbody.gameObject.GetComponent<FVRFireArm>() != null)
				{
					FVRFireArm component = array[i].attachedRigidbody.gameObject.GetComponent<FVRFireArm>();
					if (component.ObjectWrapper != null && IM.HasSpawnedID(component.ObjectWrapper.SpawnedFromId))
					{
						fVRFireArm = array[i].attachedRigidbody.gameObject.GetComponent<FVRFireArm>();
						break;
					}
				}
			}
			if (fVRFireArm == null)
			{
				Debug.Log("no firearm found");
				return false;
			}
			List<FVRPhysicalObject> list = new List<FVRPhysicalObject>();
			list.Add(fVRFireArm);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (fVRFireArm.Magazine != null && !fVRFireArm.Magazine.IsIntegrated && fVRFireArm.Magazine.ObjectWrapper != null)
			{
				list.Add(fVRFireArm.Magazine);
				flag = true;
			}
			if (!flag)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].attachedRigidbody != null && array[j].attachedRigidbody.gameObject.GetComponent<Speedloader>() != null)
					{
						Speedloader component2 = array[j].attachedRigidbody.gameObject.GetComponent<Speedloader>();
						if (!(component2.QuickbeltSlot != null) && !component2.IsHeld && component2.Chambers[0].Type == fVRFireArm.RoundType && component2.ObjectWrapper != null && IM.HasSpawnedID(component2.ObjectWrapper.SpawnedFromId))
						{
							item = array[j].attachedRigidbody.gameObject.GetComponent<Speedloader>();
							flag2 = true;
							break;
						}
					}
				}
			}
			if (!flag2)
			{
				for (int k = 0; k < array.Length; k++)
				{
					if (array[k].attachedRigidbody != null && array[k].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>() != null)
					{
						FVRFireArmClip component3 = array[k].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>();
						if (!(component3.QuickbeltSlot != null) && !component3.IsHeld && !(component3.FireArm != null) && component3.RoundType == fVRFireArm.RoundType && component3.ObjectWrapper != null && IM.HasSpawnedID(component3.ObjectWrapper.SpawnedFromId))
						{
							item2 = array[k].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>();
							flag3 = true;
							Debug.Log("Found a clip");
							break;
						}
					}
				}
			}
			if (!flag3)
			{
				for (int l = 0; l < array.Length; l++)
				{
					if (array[l].attachedRigidbody != null && array[l].attachedRigidbody.gameObject.GetComponent<FVRFireArmRound>() != null)
					{
						FVRFireArmRound component4 = array[l].attachedRigidbody.gameObject.GetComponent<FVRFireArmRound>();
						if (!(component4.QuickbeltSlot != null) && !component4.IsHeld && component4.RoundType == fVRFireArm.RoundType && component4.ObjectWrapper != null)
						{
							item3 = array[l].attachedRigidbody.gameObject.GetComponent<FVRFireArmRound>();
							flag4 = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				if (flag2)
				{
					list.Add(item);
				}
				else if (flag3)
				{
					list.Add(item2);
				}
				else if (flag4)
				{
					list.Add(item3);
				}
			}
			for (int m = 0; m < fVRFireArm.Attachments.Count; m++)
			{
				if (fVRFireArm.Attachments[m].ObjectWrapper != null)
				{
					list.Add(fVRFireArm.Attachments[m]);
				}
			}
			EncodeFirearm(list);
			return true;
		}

		private void EncodeFirearm(List<FVRPhysicalObject> DetectedObjects)
		{
			SavedGun savedGun = new SavedGun();
			savedGun.DateMade = DateTime.Now;
			Vector3 forward = DetectedObjects[0].transform.forward;
			Vector3 up = DetectedObjects[0].transform.up;
			DetectedObjects[0].transform.rotation = Quaternion.identity;
			FVRFireArm fVRFireArm = null;
			for (int i = 0; i < DetectedObjects.Count; i++)
			{
				SavedGunComponent savedGunComponent = new SavedGunComponent();
				savedGunComponent.Index = i;
				savedGunComponent.ObjectID = DetectedObjects[i].ObjectWrapper.ItemID;
				Dictionary<string, string> flagDic = DetectedObjects[i].GetFlagDic();
				if (flagDic != null)
				{
					savedGunComponent.Flags = flagDic;
				}
				if (i > 0)
				{
					Transform transform = DetectedObjects[0].transform;
					savedGunComponent.PosOffset = transform.InverseTransformPoint(DetectedObjects[i].transform.position);
					savedGunComponent.OrientationForward = DetectedObjects[i].transform.forward;
					savedGunComponent.OrientationUp = DetectedObjects[i].transform.up;
				}
				if (DetectedObjects[i] is FVRFireArm)
				{
					savedGunComponent.isFirearm = true;
					fVRFireArm = DetectedObjects[i] as FVRFireArm;
					if (fVRFireArm.Magazine != null)
					{
						for (int j = 0; j < fVRFireArm.Magazine.m_numRounds; j++)
						{
							savedGun.LoadedRoundsInMag.Add(fVRFireArm.Magazine.LoadedRounds[j].LR_Class);
						}
					}
				}
				else if (DetectedObjects[i] is FVRFireArmMagazine)
				{
					savedGunComponent.isMagazine = true;
				}
				else if (DetectedObjects[i] is FVRFireArmClip)
				{
					FVRFireArmClip fVRFireArmClip = DetectedObjects[i] as FVRFireArmClip;
					for (int k = 0; k < fVRFireArmClip.m_numRounds; k++)
					{
						savedGun.LoadedRoundsInMag.Add(fVRFireArmClip.LoadedRounds[k].LR_Class);
					}
				}
				else if (DetectedObjects[i] is Speedloader)
				{
					Speedloader speedloader = DetectedObjects[i] as Speedloader;
					for (int l = 0; l < speedloader.Chambers.Count; l++)
					{
						if (speedloader.Chambers[l].IsLoaded)
						{
							savedGun.LoadedRoundsInMag.Add(speedloader.Chambers[l].LoadedClass);
						}
					}
				}
				else if (DetectedObjects[i] is FVRFireArmAttachment)
				{
					savedGunComponent.isAttachment = true;
					FVRFireArmAttachment fVRFireArmAttachment = DetectedObjects[i] as FVRFireArmAttachment;
					FVRFireArmAttachmentMount curMount = fVRFireArmAttachment.curMount;
					FVRPhysicalObject myObject = curMount.MyObject;
					savedGunComponent.ObjectAttachedTo = DetectedObjects.IndexOf(myObject);
					savedGunComponent.MountAttachedTo = myObject.AttachmentMounts.IndexOf(curMount);
				}
				savedGunComponent.DebugPrintData();
				savedGun.Components.Add(savedGunComponent);
			}
			List<FireArmRoundClass> chamberRoundList = fVRFireArm.GetChamberRoundList();
			List<string> flagList = fVRFireArm.GetFlagList();
			if (chamberRoundList != null)
			{
				savedGun.LoadedRoundsInChambers = chamberRoundList;
			}
			if (flagList != null)
			{
				savedGun.SavedFlags = flagList;
			}
			DetectedObjects[0].transform.rotation = Quaternion.LookRotation(forward, up);
			ItemSpawner.SaveGunToVault(savedGun);
		}

		private IEnumerator SpawnFirearmRoutine(SavedGun gun)
		{
			List<GameObject> toDealWith = new List<GameObject>();
			List<GameObject> toMoveToTrays = new List<GameObject>();
			FVRFireArm myGun = null;
			FVRFireArmMagazine myMagazine2 = null;
			List<int> validIndexes = new List<int>();
			Dictionary<GameObject, SavedGunComponent> dicGO = new Dictionary<GameObject, SavedGunComponent>();
			Dictionary<int, GameObject> dicByIndex = new Dictionary<int, GameObject>();
			List<AnvilCallback<GameObject>> callbackList = new List<AnvilCallback<GameObject>>();
			for (int i = 0; i < gun.Components.Count; i++)
			{
				callbackList.Add(IM.OD[gun.Components[i].ObjectID].GetGameObjectAsync());
			}
			yield return callbackList;
			for (int j = 0; j < gun.Components.Count; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(callbackList[j].Result);
				dicGO.Add(gameObject, gun.Components[j]);
				dicByIndex.Add(gun.Components[j].Index, gameObject);
				if (gun.Components[j].isFirearm)
				{
					myGun = gameObject.GetComponent<FVRFireArm>();
					validIndexes.Add(j);
					gameObject.transform.position = base.transform.position;
					gameObject.transform.rotation = Quaternion.identity;
				}
				else if (gun.Components[j].isMagazine)
				{
					myMagazine2 = gameObject.GetComponent<FVRFireArmMagazine>();
					validIndexes.Add(j);
					if (myMagazine2 != null)
					{
						gameObject.transform.position = myGun.GetMagMountPos(myMagazine2.IsBeltBox).position;
						gameObject.transform.rotation = myGun.GetMagMountPos(myMagazine2.IsBeltBox).rotation;
						myMagazine2.Load(myGun);
						myMagazine2.IsInfinite = false;
					}
				}
				else if (gun.Components[j].isAttachment)
				{
					toDealWith.Add(gameObject);
				}
				else
				{
					toMoveToTrays.Add(gameObject);
					if (gameObject.GetComponent<Speedloader>() != null && gun.LoadedRoundsInMag.Count > 0)
					{
						Debug.Log("Loading round set to speedloader");
						Speedloader component = gameObject.GetComponent<Speedloader>();
						component.ReloadSpeedLoaderWithList(gun.LoadedRoundsInMag);
					}
					else if (gameObject.GetComponent<FVRFireArmClip>() != null && gun.LoadedRoundsInMag.Count > 0)
					{
						Debug.Log("Loading round set to clip");
						FVRFireArmClip component2 = gameObject.GetComponent<FVRFireArmClip>();
						component2.ReloadClipWithList(gun.LoadedRoundsInMag);
					}
				}
				gameObject.GetComponent<FVRPhysicalObject>().ConfigureFromFlagDic(gun.Components[j].Flags);
			}
			if (myGun.Magazine != null && gun.LoadedRoundsInMag.Count > 0)
			{
				Debug.Log("Loading round set to magazine");
				myGun.Magazine.ReloadMagWithList(gun.LoadedRoundsInMag);
				myGun.Magazine.IsInfinite = false;
			}
			int BreakIterator = 200;
			while (toDealWith.Count > 0 && BreakIterator > 0)
			{
				BreakIterator--;
				for (int num = toDealWith.Count - 1; num >= 0; num--)
				{
					SavedGunComponent savedGunComponent = dicGO[toDealWith[num]];
					if (validIndexes.Contains(savedGunComponent.ObjectAttachedTo))
					{
						GameObject gameObject2 = toDealWith[num];
						FVRFireArmAttachment component3 = gameObject2.GetComponent<FVRFireArmAttachment>();
						FVRFireArmAttachmentMount mount = GetMount(dicByIndex[savedGunComponent.ObjectAttachedTo], savedGunComponent.MountAttachedTo);
						gameObject2.transform.rotation = Quaternion.LookRotation(savedGunComponent.OrientationForward, savedGunComponent.OrientationUp);
						gameObject2.transform.position = GetPositionRelativeToGun(savedGunComponent, myGun.transform);
						if (component3.CanScaleToMount && mount.CanThisRescale())
						{
							component3.ScaleToMount(mount);
						}
						component3.AttachToMount(mount, playSound: false);
						if (component3 is Suppressor)
						{
							(component3 as Suppressor).AutoMountWell();
						}
						validIndexes.Add(savedGunComponent.Index);
						toDealWith.RemoveAt(num);
					}
				}
			}
			int trayIndex = 0;
			int itemIndex = 0;
			for (int k = 0; k < toMoveToTrays.Count; k++)
			{
				toMoveToTrays[k].transform.position = ItemSpawner.SpawnPos_Small[trayIndex].position + (float)itemIndex * 0.1f * Vector3.up;
				toMoveToTrays[k].transform.rotation = ItemSpawner.SpawnPos_Small[trayIndex].rotation;
				itemIndex++;
				trayIndex++;
				if (trayIndex > 2)
				{
					trayIndex = 0;
				}
			}
			myGun.SetLoadedChambers(gun.LoadedRoundsInChambers);
			myGun.SetFromFlagList(gun.SavedFlags);
			myGun.transform.rotation = base.transform.rotation;
		}

		public void SpawnFirearm(SavedGun gun)
		{
			if (gun.Components.Count == 0 || !IM.HasSpawnedID(IM.OD[gun.Components[0].ObjectID].SpawnedFromId))
			{
				ItemSpawner.VaultGunAssemblyFinished();
			}
			else
			{
				AnvilManager.Run(SpawnFirearmRoutine(gun));
			}
		}

		private FVRFireArmAttachmentMount GetMount(GameObject g, int index)
		{
			return g.GetComponent<FVRPhysicalObject>().AttachmentMounts[index];
		}

		private Vector3 GetPositionRelativeToGun(SavedGunComponent data, Transform gun)
		{
			Vector3 position = gun.position;
			position += gun.up * data.PosOffset.y;
			position += gun.right * data.PosOffset.x;
			return position + gun.forward * data.PosOffset.z;
		}
	}
}
