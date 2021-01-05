// Decompiled with JetBrains decompiler
// Type: FistVR.TAH_WeaponCrate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
      this.ClearUnusedSpawnsFromWorld();
      this.Cover.Reset();
      this.Latches[0].Reset();
      this.Latches[1].Reset();
      this.m_storedPrefab_Gun = (GameObject) null;
      this.m_storedPrefab_Mag = (GameObject) null;
      this.m_storedPrefab_Attachment1 = (GameObject) null;
      this.m_storedPrefab_Attachment2 = (GameObject) null;
      this.m_storedPrefab_Attachment3 = (GameObject) null;
      this.m_isOpen = false;
      this.m_containsItems = false;
    }

    private void Update()
    {
      if (this.m_isOpen || !this.m_containsItems || (!this.Latches[0].IsOpen() || !this.Latches[1].IsOpen()))
        return;
      this.SpawnItemsInCrate();
    }

    public void PlaceItemsInCrate(
      GameObject go_gun,
      GameObject go_mag,
      GameObject go_attach1,
      GameObject go_attach2,
      GameObject go_attach3)
    {
      this.m_containsItems = true;
      this.m_storedPrefab_Gun = go_gun;
      this.m_storedPrefab_Mag = go_mag;
      this.m_storedPrefab_Attachment1 = go_attach1;
      this.m_storedPrefab_Attachment2 = go_attach2;
      this.m_storedPrefab_Attachment3 = go_attach3;
    }

    private void SpawnItemsInCrate()
    {
      if (!this.m_containsItems)
        return;
      GameObject g1 = (GameObject) null;
      GameObject g2 = (GameObject) null;
      GameObject g3 = (GameObject) null;
      GameObject g4 = (GameObject) null;
      GameObject g5 = (GameObject) null;
      if ((Object) this.m_storedPrefab_Gun != (Object) null)
        g1 = Object.Instantiate<GameObject>(this.m_storedPrefab_Gun, this.Point_Gun.position, this.Point_Gun.rotation);
      if ((Object) this.m_storedPrefab_Mag != (Object) null)
      {
        g2 = Object.Instantiate<GameObject>(this.m_storedPrefab_Mag, this.Point_Mag.position, this.Point_Mag.rotation);
        FVRFireArmMagazine component = g2.GetComponent<FVRFireArmMagazine>();
        if ((Object) component != (Object) null && component.RoundType != FireArmRoundType.aFlameThrowerFuel)
          component.ReloadMagWithType(AM.GetRandomValidRoundClass(component.RoundType));
      }
      if ((Object) this.m_storedPrefab_Attachment1 != (Object) null)
        g3 = Object.Instantiate<GameObject>(this.m_storedPrefab_Attachment1, this.Point_Attachment1.position, this.Point_Attachment1.rotation);
      if ((Object) this.m_storedPrefab_Attachment2 != (Object) null)
        g4 = Object.Instantiate<GameObject>(this.m_storedPrefab_Attachment2, this.Point_Attachment2.position, this.Point_Attachment2.rotation);
      if ((Object) this.m_storedPrefab_Attachment3 != (Object) null)
        g5 = Object.Instantiate<GameObject>(this.m_storedPrefab_Attachment3, this.Point_Attachment3.position, this.Point_Attachment3.rotation);
      if ((Object) g1 != (Object) null)
        this.Manager.AddObjectToTrackedList(g1);
      if ((Object) g2 != (Object) null)
        this.Manager.AddObjectToTrackedList(g2);
      if ((Object) g3 != (Object) null)
        this.Manager.AddObjectToTrackedList(g3);
      if ((Object) g4 != (Object) null)
        this.Manager.AddObjectToTrackedList(g4);
      if ((Object) g5 != (Object) null)
        this.Manager.AddObjectToTrackedList(g5);
      this.m_containsItems = false;
    }

    public void ClearUnusedSpawnsFromWorld()
    {
      for (int index = this.SpawnedStuff.Count - 1; index >= 0; --index)
      {
        if (!((Object) this.SpawnedStuff[index] == (Object) null) && (double) Vector3.Distance(this.SpawnedStuff[index].transform.position, GM.CurrentPlayerBody.transform.position) > 12.0)
        {
          Object.Destroy((Object) this.SpawnedStuff[index]);
          this.SpawnedStuff.RemoveAt(index);
        }
      }
    }
  }
}
