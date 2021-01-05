// Decompiled with JetBrains decompiler
// Type: FistVR.FirearmSaver
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FistVR
{
  public class FirearmSaver : MonoBehaviour
  {
    public ItemSpawnerUI ItemSpawner;

    public bool TryToScanGun()
    {
      Collider[] colliderArray = Physics.OverlapBox(this.transform.position, new Vector3(0.26f, 0.45f, 0.9f), this.transform.rotation);
      FVRFireArm fvrFireArm = (FVRFireArm) null;
      Speedloader speedloader = (Speedloader) null;
      FVRFireArmClip fvrFireArmClip = (FVRFireArmClip) null;
      FVRFireArmRound fvrFireArmRound = (FVRFireArmRound) null;
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        if ((UnityEngine.Object) colliderArray[index].attachedRigidbody != (UnityEngine.Object) null && (UnityEngine.Object) colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRFireArm>() != (UnityEngine.Object) null)
        {
          FVRFireArm component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRFireArm>();
          if ((UnityEngine.Object) component.ObjectWrapper != (UnityEngine.Object) null && IM.HasSpawnedID(component.ObjectWrapper.SpawnedFromId))
          {
            fvrFireArm = colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRFireArm>();
            break;
          }
        }
      }
      if ((UnityEngine.Object) fvrFireArm == (UnityEngine.Object) null)
      {
        UnityEngine.Debug.Log((object) "no firearm found");
        return false;
      }
      List<FVRPhysicalObject> DetectedObjects = new List<FVRPhysicalObject>();
      DetectedObjects.Add((FVRPhysicalObject) fvrFireArm);
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      if ((UnityEngine.Object) fvrFireArm.Magazine != (UnityEngine.Object) null && !fvrFireArm.Magazine.IsIntegrated && (UnityEngine.Object) fvrFireArm.Magazine.ObjectWrapper != (UnityEngine.Object) null)
      {
        DetectedObjects.Add((FVRPhysicalObject) fvrFireArm.Magazine);
        flag1 = true;
      }
      if (!flag1)
      {
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if ((UnityEngine.Object) colliderArray[index].attachedRigidbody != (UnityEngine.Object) null && (UnityEngine.Object) colliderArray[index].attachedRigidbody.gameObject.GetComponent<Speedloader>() != (UnityEngine.Object) null)
          {
            Speedloader component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<Speedloader>();
            if (!((UnityEngine.Object) component.QuickbeltSlot != (UnityEngine.Object) null) && !component.IsHeld && (component.Chambers[0].Type == fvrFireArm.RoundType && (UnityEngine.Object) component.ObjectWrapper != (UnityEngine.Object) null) && IM.HasSpawnedID(component.ObjectWrapper.SpawnedFromId))
            {
              speedloader = colliderArray[index].attachedRigidbody.gameObject.GetComponent<Speedloader>();
              flag2 = true;
              break;
            }
          }
        }
      }
      if (!flag2)
      {
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if ((UnityEngine.Object) colliderArray[index].attachedRigidbody != (UnityEngine.Object) null && (UnityEngine.Object) colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>() != (UnityEngine.Object) null)
          {
            FVRFireArmClip component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>();
            if (!((UnityEngine.Object) component.QuickbeltSlot != (UnityEngine.Object) null) && !component.IsHeld && (!((UnityEngine.Object) component.FireArm != (UnityEngine.Object) null) && component.RoundType == fvrFireArm.RoundType) && ((UnityEngine.Object) component.ObjectWrapper != (UnityEngine.Object) null && IM.HasSpawnedID(component.ObjectWrapper.SpawnedFromId)))
            {
              fvrFireArmClip = colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>();
              flag3 = true;
              UnityEngine.Debug.Log((object) "Found a clip");
              break;
            }
          }
        }
      }
      if (!flag3)
      {
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if ((UnityEngine.Object) colliderArray[index].attachedRigidbody != (UnityEngine.Object) null && (UnityEngine.Object) colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRFireArmRound>() != (UnityEngine.Object) null)
          {
            FVRFireArmRound component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRFireArmRound>();
            if (!((UnityEngine.Object) component.QuickbeltSlot != (UnityEngine.Object) null) && !component.IsHeld && (component.RoundType == fvrFireArm.RoundType && (UnityEngine.Object) component.ObjectWrapper != (UnityEngine.Object) null))
            {
              fvrFireArmRound = colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRFireArmRound>();
              flag4 = true;
              break;
            }
          }
        }
      }
      if (!flag1)
      {
        if (flag2)
          DetectedObjects.Add((FVRPhysicalObject) speedloader);
        else if (flag3)
          DetectedObjects.Add((FVRPhysicalObject) fvrFireArmClip);
        else if (flag4)
          DetectedObjects.Add((FVRPhysicalObject) fvrFireArmRound);
      }
      for (int index = 0; index < fvrFireArm.Attachments.Count; ++index)
      {
        if ((UnityEngine.Object) fvrFireArm.Attachments[index].ObjectWrapper != (UnityEngine.Object) null)
          DetectedObjects.Add((FVRPhysicalObject) fvrFireArm.Attachments[index]);
      }
      this.EncodeFirearm(DetectedObjects);
      return true;
    }

    private void EncodeFirearm(List<FVRPhysicalObject> DetectedObjects)
    {
      SavedGun g = new SavedGun();
      g.DateMade = DateTime.Now;
      Vector3 forward = DetectedObjects[0].transform.forward;
      Vector3 up = DetectedObjects[0].transform.up;
      DetectedObjects[0].transform.rotation = Quaternion.identity;
      FVRFireArm fvrFireArm = (FVRFireArm) null;
      for (int index1 = 0; index1 < DetectedObjects.Count; ++index1)
      {
        SavedGunComponent savedGunComponent = new SavedGunComponent();
        savedGunComponent.Index = index1;
        savedGunComponent.ObjectID = DetectedObjects[index1].ObjectWrapper.ItemID;
        Dictionary<string, string> flagDic = DetectedObjects[index1].GetFlagDic();
        if (flagDic != null)
          savedGunComponent.Flags = flagDic;
        if (index1 > 0)
        {
          Transform transform = DetectedObjects[0].transform;
          savedGunComponent.PosOffset = transform.InverseTransformPoint(DetectedObjects[index1].transform.position);
          savedGunComponent.OrientationForward = DetectedObjects[index1].transform.forward;
          savedGunComponent.OrientationUp = DetectedObjects[index1].transform.up;
        }
        if (DetectedObjects[index1] is FVRFireArm)
        {
          savedGunComponent.isFirearm = true;
          fvrFireArm = DetectedObjects[index1] as FVRFireArm;
          if ((UnityEngine.Object) fvrFireArm.Magazine != (UnityEngine.Object) null)
          {
            for (int index2 = 0; index2 < fvrFireArm.Magazine.m_numRounds; ++index2)
              g.LoadedRoundsInMag.Add(fvrFireArm.Magazine.LoadedRounds[index2].LR_Class);
          }
        }
        else if (DetectedObjects[index1] is FVRFireArmMagazine)
          savedGunComponent.isMagazine = true;
        else if (DetectedObjects[index1] is FVRFireArmClip)
        {
          FVRFireArmClip detectedObject = DetectedObjects[index1] as FVRFireArmClip;
          for (int index2 = 0; index2 < detectedObject.m_numRounds; ++index2)
            g.LoadedRoundsInMag.Add(detectedObject.LoadedRounds[index2].LR_Class);
        }
        else if (DetectedObjects[index1] is Speedloader)
        {
          Speedloader detectedObject = DetectedObjects[index1] as Speedloader;
          for (int index2 = 0; index2 < detectedObject.Chambers.Count; ++index2)
          {
            if (detectedObject.Chambers[index2].IsLoaded)
              g.LoadedRoundsInMag.Add(detectedObject.Chambers[index2].LoadedClass);
          }
        }
        else if (DetectedObjects[index1] is FVRFireArmAttachment)
        {
          savedGunComponent.isAttachment = true;
          FVRFireArmAttachmentMount curMount = (DetectedObjects[index1] as FVRFireArmAttachment).curMount;
          FVRPhysicalObject fvrPhysicalObject = curMount.MyObject;
          savedGunComponent.ObjectAttachedTo = DetectedObjects.IndexOf(fvrPhysicalObject);
          savedGunComponent.MountAttachedTo = fvrPhysicalObject.AttachmentMounts.IndexOf(curMount);
        }
        savedGunComponent.DebugPrintData();
        g.Components.Add(savedGunComponent);
      }
      List<FireArmRoundClass> chamberRoundList = fvrFireArm.GetChamberRoundList();
      List<string> flagList = fvrFireArm.GetFlagList();
      if (chamberRoundList != null)
        g.LoadedRoundsInChambers = chamberRoundList;
      if (flagList != null)
        g.SavedFlags = flagList;
      DetectedObjects[0].transform.rotation = Quaternion.LookRotation(forward, up);
      this.ItemSpawner.SaveGunToVault(g);
    }

    [DebuggerHidden]
    private IEnumerator SpawnFirearmRoutine(SavedGun gun) => (IEnumerator) new FirearmSaver.\u003CSpawnFirearmRoutine\u003Ec__Iterator0()
    {
      gun = gun,
      \u0024this = this
    };

    public void SpawnFirearm(SavedGun gun)
    {
      if (gun.Components.Count == 0 || !IM.HasSpawnedID(IM.OD[gun.Components[0].ObjectID].SpawnedFromId))
        this.ItemSpawner.VaultGunAssemblyFinished();
      else
        AnvilManager.Run(this.SpawnFirearmRoutine(gun));
    }

    private FVRFireArmAttachmentMount GetMount(GameObject g, int index) => g.GetComponent<FVRPhysicalObject>().AttachmentMounts[index];

    private Vector3 GetPositionRelativeToGun(SavedGunComponent data, Transform gun) => gun.position + gun.up * data.PosOffset.y + gun.right * data.PosOffset.x + gun.forward * data.PosOffset.z;
  }
}
