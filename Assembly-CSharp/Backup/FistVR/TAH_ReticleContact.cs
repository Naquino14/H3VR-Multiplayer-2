// Decompiled with JetBrains decompiler
// Type: FistVR.TAH_ReticleContact
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TAH_ReticleContact : MonoBehaviour
  {
    public List<Mesh> Meshes_Arrow;
    public List<Mesh> Meshes_Icon;
    public List<Material> Mats_Arrow;
    public List<Material> Mats_Icon;
    public List<Material> Mats_Arrow_Visited;
    public List<Material> Mats_Icon_Visited;
    public Transform Icon;
    public Transform Vertical;
    public MeshFilter M_Arrow;
    public MeshFilter M_Icon;
    public MeshFilter M_Vertical;
    public MeshFilter M_BasePlate;
    public Renderer R_Arrow;
    public Renderer R_Icon;
    public Renderer R_Vertical;
    public Renderer R_BasePlate;
    public TAH_ReticleContact.ContactType Type = TAH_ReticleContact.ContactType.None;
    public TAH_ReticleContact.ContactState State;
    public Transform TrackedTransform;
    private float m_range = 50f;
    public bool UsesVerticality = true;
    public bool ShowArrows = true;
    private bool m_isVisited;

    public void InitContact(
      TAH_ReticleContact.ContactType type,
      Transform trackedTransform,
      float range)
    {
      this.TrackedTransform = trackedTransform;
      this.m_range = range;
      this.SetContactType(type);
    }

    public void SetVisited(bool b)
    {
      this.m_isVisited = b;
      if (!this.m_isVisited)
      {
        this.R_Arrow.material = this.Mats_Arrow[(int) this.Type];
        this.R_Icon.material = this.Mats_Icon[(int) this.Type];
      }
      else
      {
        this.R_Arrow.material = this.Mats_Arrow_Visited[(int) this.Type];
        this.R_Icon.material = this.Mats_Icon_Visited[(int) this.Type];
      }
    }

    public bool Tick(Vector3 cPoint)
    {
      if ((Object) this.TrackedTransform == (Object) null)
        return false;
      Vector3 lp = this.TrackedTransform.position - cPoint;
      Vector3 lpXZ = lp;
      lpXZ.y = 0.0f;
      if (this.Type == TAH_ReticleContact.ContactType.Enemy && (double) lpXZ.magnitude > (double) this.m_range * 1.10000002384186)
        return false;
      if ((double) lpXZ.magnitude > (double) this.m_range)
        this.UpdateContactAsArrow(lp, lpXZ);
      else
        this.UpdateContactAsIcon(lp, lpXZ);
      return true;
    }

    private void UpdateContactAsArrow(Vector3 lp, Vector3 lpXZ)
    {
      this.SetContactState(TAH_ReticleContact.ContactState.Arrow);
      this.transform.localPosition = Vector3.zero;
      this.transform.localRotation = Quaternion.LookRotation(lpXZ, Vector3.up);
    }

    private void UpdateContactAsIcon(Vector3 lp, Vector3 lpXZ)
    {
      this.SetContactState(TAH_ReticleContact.ContactState.Icon);
      this.transform.localPosition = lpXZ * (1f / this.m_range);
      float y = lp.y / this.m_range;
      if (!this.UsesVerticality)
        y = 0.0f;
      this.Vertical.localScale = new Vector3(1f, lp.y / this.m_range, 1f);
      this.Icon.localPosition = new Vector3(0.0f, y, 0.0f);
    }

    private void SetContactType(TAH_ReticleContact.ContactType t)
    {
      if (this.Type == t)
        return;
      this.Type = t;
      this.M_Arrow.mesh = this.Meshes_Arrow[(int) this.Type];
      this.M_Icon.mesh = this.Meshes_Icon[(int) this.Type];
      this.R_Arrow.material = this.Mats_Arrow[(int) this.Type];
      this.R_Icon.material = this.Mats_Icon[(int) this.Type];
    }

    private void SetContactState(TAH_ReticleContact.ContactState s)
    {
      if (this.State == s)
        return;
      this.State = s;
      switch (this.State)
      {
        case TAH_ReticleContact.ContactState.Icon:
          this.R_Arrow.enabled = false;
          this.R_Icon.enabled = true;
          this.R_Vertical.enabled = this.UsesVerticality;
          this.R_BasePlate.enabled = this.UsesVerticality;
          break;
        case TAH_ReticleContact.ContactState.Arrow:
          this.R_Arrow.enabled = this.ShowArrows;
          this.R_Icon.enabled = false;
          this.R_Vertical.enabled = false;
          this.R_BasePlate.enabled = false;
          break;
      }
    }

    public enum ContactState
    {
      Disabled,
      Icon,
      Arrow,
    }

    public enum ContactType
    {
      None = -1, // 0xFFFFFFFF
      Unknown = 0,
      Hold = 1,
      Health = 2,
      Supply = 3,
      Enemy = 4,
    }
  }
}
