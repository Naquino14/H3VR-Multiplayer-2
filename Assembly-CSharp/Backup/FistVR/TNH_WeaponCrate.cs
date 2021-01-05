// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_WeaponCrate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    public void PlaceWeaponInContainer(
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
      this.m_usesFVRObjects = true;
    }

    public void PlaceWeaponInContainer(
      GameObject gun,
      GameObject ammo,
      GameObject extra1,
      GameObject extra2)
    {
      this.m_storedGO_gun = gun;
      this.m_storedGO_ammo = ammo;
      this.m_storedGO_extra1 = extra1;
      this.m_storedGO_extra2 = extra2;
      this.m_containsItems = true;
      this.m_usesFVRObjects = false;
    }

    private void Update()
    {
      if (this.m_isOpen || !this.m_containsItems || (!this.Latches[0].IsOpen() || !this.Latches[1].IsOpen()) || (this.m_hasSpawnedContents || !this.m_containsItems))
        return;
      this.m_hasSpawnedContents = true;
      AnvilManager.Run(this.SpawnItemsInCrate());
      this.SpawnObjectsRaw();
    }

    private void SpawnObjectsRaw()
    {
      if ((Object) this.m_storedGO_gun != (Object) null)
        Object.Instantiate<GameObject>(this.m_storedGO_gun, this.Point_Gun.position, this.Point_Gun.rotation);
      if ((Object) this.m_storedGO_ammo != (Object) null)
      {
        Object.Instantiate<GameObject>(this.m_storedGO_ammo, this.Points_MagClipSpeedloader[0].position, this.Points_MagClipSpeedloader[0].rotation);
        Object.Instantiate<GameObject>(this.m_storedGO_ammo, this.Points_MagClipSpeedloader[1].position, this.Points_MagClipSpeedloader[1].rotation);
        Object.Instantiate<GameObject>(this.m_storedGO_ammo, this.Points_MagClipSpeedloader[2].position, this.Points_MagClipSpeedloader[2].rotation);
      }
      if ((Object) this.m_storedGO_extra1 != (Object) null)
        Object.Instantiate<GameObject>(this.m_storedGO_extra1, this.Points_Attachment[0].position, this.Points_Attachment[0].rotation);
      if (!((Object) this.m_storedGO_extra2 != (Object) null))
        return;
      Object.Instantiate<GameObject>(this.m_storedGO_extra1, this.Points_Attachment[1].position, this.Points_Attachment[1].rotation);
    }

    [DebuggerHidden]
    private IEnumerator SpawnItemsInCrate() => (IEnumerator) new TNH_WeaponCrate.\u003CSpawnItemsInCrate\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
