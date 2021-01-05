// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigContainer_WeaponCase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
      if (!((Object) obj1 != (Object) null))
        return;
      this.LoadIntoCrate(obj1, minAmmo, maxAmmo);
    }

    public void LoadIntoCrate(FVRObject obj, int minAmmo, int maxAmmo)
    {
      FVRObject magazineClipSpeedLoaderRound = (FVRObject) null;
      int numClipSpeedLoaderRound = 0;
      if ((Object) obj == (Object) null)
      {
        this.gameObject.name = "case" + Random.Range(0, 314).ToString();
        UnityEngine.Debug.Log((object) ("oh shit null case:" + this.gameObject.name));
      }
      if (obj.Category == FVRObject.ObjectCategory.Firearm)
      {
        magazineClipSpeedLoaderRound = obj.GetRandomAmmoObject(obj, Min: minAmmo, Max: maxAmmo);
        numClipSpeedLoaderRound = !((Object) magazineClipSpeedLoaderRound != (Object) null) || magazineClipSpeedLoaderRound.Category != FVRObject.ObjectCategory.Cartridge ? 2 : 5;
      }
      FVRObject gun = (FVRObject) null;
      FVRObject requiredAttachment_A = (FVRObject) null;
      FVRObject requiredAttachment_B = (FVRObject) null;
      if (obj.Category == FVRObject.ObjectCategory.Attachment)
      {
        requiredAttachment_A = obj;
        if (obj.RequiredSecondaryPieces.Count > 0)
          requiredAttachment_B = obj.RequiredSecondaryPieces[0];
      }
      else
        gun = obj;
      this.PlaceWeaponInContainer(gun, requiredAttachment_A, requiredAttachment_B, magazineClipSpeedLoaderRound, numClipSpeedLoaderRound);
    }

    private void PlaceWeaponInContainer(
      FVRObject gun,
      FVRObject requiredAttachment_A,
      FVRObject requiredAttachment_B,
      FVRObject magazineClipSpeedLoaderRound,
      int numClipSpeedLoaderRound)
    {
      this.m_storedObject_gun = gun;
      this.m_storedObject_attachA = requiredAttachment_A;
      this.m_storedObject_attachB = requiredAttachment_B;
      this.m_magazineClipSpeedLoaderRound = magazineClipSpeedLoaderRound;
      this.m_numClipSpeedLoaderRound = numClipSpeedLoaderRound;
      this.m_containsItems = true;
    }

    public void SetMagMinMax(int min, int max)
    {
    }

    private void Start()
    {
      if (this.UsesLatches)
        return;
      this.Cover.ForceOpen();
    }

    private void Update()
    {
      if (this.m_isOpen || !this.m_containsItems)
        return;
      if (this.UsesLatches)
      {
        if (!this.Latches[0].IsOpen() || !this.Latches[1].IsOpen() || this.m_hasSpawnedContents)
          return;
        if ((Object) GM.ZMaster != (Object) null)
          GM.ZMaster.FlagM.AddToFlag("s_l", 1);
        this.m_hasSpawnedContents = true;
        AnvilManager.Run(this.SpawnItemsInCrate());
      }
      else
      {
        if (!this.Cover.IsHeld || this.m_hasSpawnedContents)
          return;
        this.m_hasSpawnedContents = true;
        AnvilManager.Run(this.SpawnItemsInCrate());
      }
    }

    [DebuggerHidden]
    private IEnumerator SpawnItemsInCrate() => (IEnumerator) new ZosigContainer_WeaponCase.\u003CSpawnItemsInCrate\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
