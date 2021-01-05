// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockFlint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FlintlockFlint : FVRPhysicalObject
  {
    public Vector3 m_flintUses = Vector3.one;
    public MeshFilter FlintMesh;
    public List<Mesh> FlintMeshes;

    protected override void Awake()
    {
      base.Awake();
      this.m_flintUses = new Vector3((float) Random.Range(8, 15), (float) Random.Range(5, 9), (float) Random.Range(4, 8));
    }

    public void UpdateState()
    {
      if ((double) this.m_flintUses.x > 0.0)
        this.SetFlintState(FlintlockWeapon.FlintState.New);
      else if ((double) this.m_flintUses.y > 0.0)
        this.SetFlintState(FlintlockWeapon.FlintState.Used);
      else if ((double) this.m_flintUses.z > 0.0)
        this.SetFlintState(FlintlockWeapon.FlintState.Worn);
      else
        this.SetFlintState(FlintlockWeapon.FlintState.Broken);
    }

    public void SetFlintState(FlintlockWeapon.FlintState f) => this.FlintMesh.mesh = this.FlintMeshes[(int) f];
  }
}
