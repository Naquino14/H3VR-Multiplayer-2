// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmAttachmentMount
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRFireArmAttachmentMount : MonoBehaviour
  {
    public FVRPhysicalObject Parent;
    public List<FVRFireArmAttachmentMount> SubMounts;
    public Transform Point_Front;
    public Transform Point_Rear;
    public FVRFireArmAttachementMountType Type;
    public float ScaleModifier = 1f;
    public List<FVRFireArmAttachment> AttachmentsList = new List<FVRFireArmAttachment>();
    private HashSet<FVRFireArmAttachment> AttachmentsHash = new HashSet<FVRFireArmAttachment>();
    public FVRPhysicalObject MyObject;
    public bool HasHoverDisablePiece;
    public GameObject DisableOnHover;
    private int m_maxAttachments = 10;
    public bool ParentToThis;

    private void Awake()
    {
      if (this.Type == FVRFireArmAttachementMountType.Suppressor || this.Type.ToString().Contains("Bayonet"))
        this.m_maxAttachments = 1;
      if (!this.HasHoverDisablePiece || !((Object) this.DisableOnHover == (Object) null))
        return;
      this.HasHoverDisablePiece = false;
    }

    public bool isMountableOn(FVRFireArmAttachment possibleAttachment) => !((Object) this.Parent == (Object) null) && this.AttachmentsList.Count < this.m_maxAttachments && (!((Object) possibleAttachment.AttachmentInterface != (Object) null) || !(possibleAttachment.AttachmentInterface is AttachableBipodInterface) || !((Object) this.GetRootMount().MyObject.Bipod != (Object) null)) && (!(possibleAttachment is Suppressor) || (!(this.GetRootMount().MyObject is SingleActionRevolver) || (this.GetRootMount().MyObject as SingleActionRevolver).AllowsSuppressor) && (!(this.GetRootMount().MyObject is Revolver) || (this.GetRootMount().MyObject as Revolver).AllowsSuppressor)) && (!(possibleAttachment is AttachableMeleeWeapon) || !(this.GetRootMount().MyObject is FVRFireArm) || !((Object) (this.GetRootMount().MyObject as FVRFireArm).CurrentAttachableMeleeWeapon != (Object) null));

    public void BeginHover()
    {
      if (!this.HasHoverDisablePiece || !this.DisableOnHover.activeSelf)
        return;
      this.DisableOnHover.SetActive(false);
    }

    public void EndHover() => this.CheckStatus();

    private void CheckStatus()
    {
      if (!this.HasHoverDisablePiece || this.AttachmentsList.Count > 0 || this.DisableOnHover.activeSelf)
        return;
      this.DisableOnHover.SetActive(true);
    }

    public bool CanThisRescale()
    {
      FVRFireArmAttachment component = this.MyObject.GetComponent<FVRFireArmAttachment>();
      if (!((Object) component != (Object) null))
        return true;
      return component.CanScaleToMount && component.curMount.CanThisRescale();
    }

    public FVRFireArmAttachmentMount GetRootMount()
    {
      FVRFireArmAttachment component = this.MyObject.GetComponent<FVRFireArmAttachment>();
      return (Object) component != (Object) null ? component.curMount.GetRootMount() : this;
    }

    public void RegisterAttachment(FVRFireArmAttachment attachment)
    {
      if (!this.AttachmentsHash.Add(attachment))
        return;
      this.AttachmentsList.Add(attachment);
      if (!this.HasHoverDisablePiece || !this.DisableOnHover.activeSelf)
        return;
      this.DisableOnHover.SetActive(false);
    }

    public void DeRegisterAttachment(FVRFireArmAttachment attachment)
    {
      if (!this.AttachmentsHash.Remove(attachment))
        return;
      this.AttachmentsList.Remove(attachment);
    }

    public bool HasAttachmentsOnIt() => this.AttachmentsList.Count != 0;
  }
}
