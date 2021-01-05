// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmBeltSegment
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRFireArmBeltSegment : FVRPhysicalObject
  {
    public List<FVRLoadedRound> RoundList = new List<FVRLoadedRound>();
    public List<GameObject> ProxyRounds = new List<GameObject>();
    public List<Renderer> ProxyRends = new List<Renderer>();
    public List<MeshFilter> ProxyMeshes = new List<MeshFilter>();
    private FVRFireArmBeltRemovalTrigger m_trig;

    protected override void Awake()
    {
      base.Awake();
      this.UpdateBulletDisplay();
    }

    public void UpdateBulletDisplay()
    {
      for (int index = 0; index < this.RoundList.Count; ++index)
      {
        if (!this.ProxyRounds[index].activeSelf)
          this.ProxyRounds[index].SetActive(true);
        this.ProxyRends[index].material = this.RoundList[index].LR_Material;
        this.ProxyMeshes[index].mesh = this.RoundList[index].LR_Mesh;
      }
      for (int count = this.RoundList.Count; count < this.ProxyRounds.Count; ++count)
      {
        if (this.ProxyRounds[count].activeSelf)
          this.ProxyRounds[count].SetActive(false);
      }
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      if ((Object) this.m_trig != (Object) null && !this.m_trig.FireArm.HasBelt && (!this.m_trig.FireArm.UsesTopCover || this.m_trig.FireArm.IsTopCoverUp) && (((Object) this.m_trig.FireArm.Magazine == (Object) null || this.m_trig.FireArm.Magazine.IsBeltBox) && this.RoundList.Count > 0))
      {
        this.m_trig.FireArm.BeltDD.MountBeltSegment(this);
        Object.Destroy((Object) this.gameObject);
      }
      else
        base.EndInteraction(hand);
    }

    public void OnTriggerEnter(Collider col)
    {
      FVRFireArmBeltRemovalTrigger component = col.gameObject.GetComponent<FVRFireArmBeltRemovalTrigger>();
      if (!((Object) component != (Object) null))
        return;
      this.m_trig = component;
    }

    public void OnTriggerExit(Collider col)
    {
      FVRFireArmBeltRemovalTrigger component = col.gameObject.GetComponent<FVRFireArmBeltRemovalTrigger>();
      if (!((Object) component != (Object) null) || !((Object) component == (Object) this.m_trig))
        return;
      this.m_trig = (FVRFireArmBeltRemovalTrigger) null;
    }
  }
}
