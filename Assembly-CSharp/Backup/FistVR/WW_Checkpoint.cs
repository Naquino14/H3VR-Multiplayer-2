// Decompiled with JetBrains decompiler
// Type: FistVR.WW_Checkpoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
      ObjectTableDef d = this.PoolsUsed[Random.Range(0, this.PoolsUsed.Count)];
      ObjectTable objectTable = new ObjectTable();
      objectTable.Initialize(d);
      FVRObject randomObject = objectTable.GetRandomObject();
      FVRObject magazineClipSpeedLoaderRound = (FVRObject) null;
      int numClipSpeedLoaderRound = 0;
      FVRObject requiredAttachment_A = (FVRObject) null;
      FVRObject requiredAttachment_B = (FVRObject) null;
      if (randomObject.Category == FVRObject.ObjectCategory.Firearm)
      {
        magazineClipSpeedLoaderRound = randomObject.GetRandomAmmoObject(randomObject, d.Eras, d.MinAmmoCapacity, d.MaxAmmoCapacity, d.Sets);
        numClipSpeedLoaderRound = !((Object) magazineClipSpeedLoaderRound != (Object) null) || magazineClipSpeedLoaderRound.Category != FVRObject.ObjectCategory.Cartridge ? 2 : 2;
        if (randomObject.RequiresPicatinnySight || this.ForcePic)
        {
          requiredAttachment_A = requiredPic.GetRandomObject();
          if (requiredAttachment_A.RequiredSecondaryPieces.Count > 0)
            requiredAttachment_B = requiredAttachment_A.RequiredSecondaryPieces[0];
          else if (randomObject.BespokeAttachments.Count > 0)
            requiredAttachment_A = randomObject.BespokeAttachments[Random.Range(0, randomObject.BespokeAttachments.Count)];
        }
      }
      this.Crate.PlaceWeaponInContainer(randomObject, requiredAttachment_A, requiredAttachment_B, magazineClipSpeedLoaderRound, numClipSpeedLoaderRound);
    }
  }
}
