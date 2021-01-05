// Decompiled with JetBrains decompiler
// Type: FistVR.FVRQuickBeltSlot
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRQuickBeltSlot : MonoBehaviour
  {
    public Transform QuickbeltRoot;
    public Transform PoseOverride;
    public FVRPhysicalObject.FVRPhysicalObjectSize SizeLimit;
    public FVRQuickBeltSlot.QuickbeltSlotShape Shape;
    public FVRQuickBeltSlot.QuickbeltSlotType Type;
    public GameObject HoverGeo;
    private Renderer m_hoverGeoRend;
    public Transform RectBounds;
    public FVRPhysicalObject CurObject;
    public bool IsSelectable = true;
    public bool IsPlayer = true;
    public bool UseStraightAxisAlignment;
    private bool m_isKeepingTrackWithHead;
    private bool m_isHovered;
    [HideInInspector]
    public FVRInteractiveObject HeldObject;

    public bool IsKeepingTrackWithHead
    {
      get => this.m_isKeepingTrackWithHead;
      set
      {
        this.m_isKeepingTrackWithHead = value;
        if (this.m_isKeepingTrackWithHead)
          return;
        this.transform.localEulerAngles = Vector3.zero;
      }
    }

    public bool IsHovered
    {
      get => this.m_isHovered;
      set
      {
        bool isHovered = this.m_isHovered;
        if (value)
        {
          if (!this.HoverGeo.activeSelf)
            this.HoverGeo.SetActive(true);
        }
        else if (this.HoverGeo.activeSelf)
          this.HoverGeo.SetActive(false);
        this.m_isHovered = value;
        if (this.m_isHovered && !isHovered || (!this.m_isHovered || !isHovered))
          ;
      }
    }

    private void Awake()
    {
      this.HoverGeo.SetActive(false);
      this.m_hoverGeoRend = this.HoverGeo.GetComponent<Renderer>();
    }

    public bool IsPointInsideMe(Vector3 v)
    {
      if (!this.IsSelectable)
        return false;
      switch (this.Shape)
      {
        case FVRQuickBeltSlot.QuickbeltSlotShape.Sphere:
          return this.IsPointInsideSphereGeo(v);
        case FVRQuickBeltSlot.QuickbeltSlotShape.Rectalinear:
          return this.IsPointInsideRectBound(v);
        default:
          return false;
      }
    }

    private bool IsPointInsideSphereGeo(Vector3 p) => (double) this.HoverGeo.transform.InverseTransformPoint(p).magnitude < 0.5;

    private bool IsPointInsideRectBound(Vector3 p)
    {
      if ((Object) this.RectBounds == (Object) null)
        return false;
      Vector3 vector3 = this.RectBounds.InverseTransformPoint(p);
      return (double) Mathf.Abs(vector3.x) <= 0.5 && (double) Mathf.Abs(vector3.y) <= 0.5 && (double) Mathf.Abs(vector3.z) <= 0.5;
    }

    private void Update()
    {
      if (!GM.CurrentSceneSettings.IsSpawnLockingEnabled && (Object) this.HeldObject != (Object) null && (this.HeldObject as FVRPhysicalObject).m_isSpawnLock)
        (this.HeldObject as FVRPhysicalObject).m_isSpawnLock = false;
      if ((Object) this.HeldObject != (Object) null)
      {
        if ((this.HeldObject as FVRPhysicalObject).m_isSpawnLock)
        {
          if (!this.HoverGeo.activeSelf)
            this.HoverGeo.SetActive(true);
          this.m_hoverGeoRend.material.SetColor("_RimColor", new Color(0.3f, 0.3f, 1f, 1f));
        }
        else if ((this.HeldObject as FVRPhysicalObject).m_isHardnessed)
        {
          if (!this.HoverGeo.activeSelf)
            this.HoverGeo.SetActive(true);
          this.m_hoverGeoRend.material.SetColor("_RimColor", new Color(0.3f, 1f, 0.3f, 1f));
        }
        else
        {
          if (this.HoverGeo.activeSelf != this.IsHovered)
            this.HoverGeo.SetActive(this.IsHovered);
          this.m_hoverGeoRend.material.SetColor("_RimColor", new Color(1f, 1f, 1f, 1f));
        }
      }
      else
      {
        if (this.HoverGeo.activeSelf != this.IsHovered)
          this.HoverGeo.SetActive(this.IsHovered);
        this.m_hoverGeoRend.material.SetColor("_RimColor", new Color(1f, 1f, 1f, 1f));
      }
    }

    private void FixedUpdate()
    {
      if (!this.IsPlayer || !((Object) this.CurObject != (Object) null) || (!this.CurObject.DoesQuickbeltSlotFollowHead || this.Shape != FVRQuickBeltSlot.QuickbeltSlotShape.Sphere))
        return;
      if (this.CurObject.DoesQuickbeltSlotFollowHead)
        this.PoseOverride.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(GM.CurrentPlayerBody.Head.transform.forward, this.transform.right), GM.CurrentPlayerBody.Head.transform.up);
      else
        this.PoseOverride.localRotation = Quaternion.identity;
    }

    public void MoveContents(Vector3 dir)
    {
      if (!((Object) this.CurObject != (Object) null) || this.CurObject.IsHeld)
        return;
      this.CurObject.transform.position = this.CurObject.transform.position + dir;
      this.CurObject.RootRigidbody.velocity = Vector3.zero;
    }

    public void MoveContentsInstant(Vector3 dir)
    {
      if (!((Object) this.CurObject != (Object) null) || this.CurObject.IsHeld)
        return;
      this.CurObject.transform.position = this.CurObject.transform.position + dir;
      this.CurObject.RootRigidbody.velocity = Vector3.zero;
    }

    public void MoveContentsCheap(Vector3 dir)
    {
      if (!((Object) this.CurObject != (Object) null) || this.CurObject.IsHeld)
        return;
      this.CurObject.RootRigidbody.position += dir;
      this.CurObject.RootRigidbody.velocity = Vector3.zero;
    }

    public enum QuickbeltSlotShape
    {
      Sphere,
      Rectalinear,
    }

    public enum QuickbeltSlotType
    {
      Standard,
      Backpack,
      None,
    }
  }
}
