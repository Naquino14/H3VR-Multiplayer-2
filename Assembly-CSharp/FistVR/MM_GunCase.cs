// Decompiled with JetBrains decompiler
// Type: FistVR.MM_GunCase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
      if (!this.AutoSpawn)
        return;
      this.m_containsItems = true;
      if ((Object) this.FO_Gun != (Object) null)
        this.m_storedPrefab_Gun = this.FO_Gun.GetGameObject();
      if ((Object) this.FO_Mag != (Object) null)
        this.m_storedPrefab_Mag = this.FO_Mag.GetGameObject();
      if ((Object) this.FO_Attachment1 != (Object) null)
        this.m_storedPrefab_Attachment1 = this.FO_Attachment1.GetGameObject();
      if ((Object) this.FO_Attachment2 != (Object) null)
        this.m_storedPrefab_Attachment2 = this.FO_Attachment2.GetGameObject();
      if (!((Object) this.FO_Attachment3 != (Object) null))
        return;
      this.m_storedPrefab_Attachment3 = this.FO_Attachment3.GetGameObject();
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

    private void Update()
    {
      if (this.m_isOpen || !this.m_containsItems || (!this.Latches[0].IsOpen() || !this.Latches[1].IsOpen()))
        return;
      this.SpawnItemsInCrate();
    }

    private void SpawnItemsInCrate()
    {
      if (!this.m_containsItems)
        return;
      GameObject gameObject1 = (GameObject) null;
      GameObject gameObject2 = (GameObject) null;
      GameObject gameObject3 = (GameObject) null;
      GameObject gameObject4 = (GameObject) null;
      if ((Object) this.m_storedPrefab_Gun != (Object) null)
        gameObject1 = Object.Instantiate<GameObject>(this.m_storedPrefab_Gun, this.Point_Gun.position, this.Point_Gun.rotation);
      if ((Object) this.m_storedPrefab_Mag != (Object) null)
      {
        FVRFireArmMagazine component = Object.Instantiate<GameObject>(this.m_storedPrefab_Mag, this.Point_Mag.position, this.Point_Mag.rotation).GetComponent<FVRFireArmMagazine>();
        if ((Object) component != (Object) null && component.RoundType != FireArmRoundType.aFlameThrowerFuel)
          component.ReloadMagWithType(AM.GetRandomValidRoundClass(component.RoundType));
      }
      if ((Object) this.m_storedPrefab_Attachment1 != (Object) null)
        gameObject2 = Object.Instantiate<GameObject>(this.m_storedPrefab_Attachment1, this.Point_Attachment1.position, this.Point_Attachment1.rotation);
      if ((Object) this.m_storedPrefab_Attachment2 != (Object) null)
        gameObject3 = Object.Instantiate<GameObject>(this.m_storedPrefab_Attachment2, this.Point_Attachment2.position, this.Point_Attachment2.rotation);
      if ((Object) this.m_storedPrefab_Attachment3 != (Object) null)
        gameObject4 = Object.Instantiate<GameObject>(this.m_storedPrefab_Attachment3, this.Point_Attachment3.position, this.Point_Attachment3.rotation);
      this.m_containsItems = false;
    }
  }
}
