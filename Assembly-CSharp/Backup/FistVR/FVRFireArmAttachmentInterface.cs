// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmAttachmentInterface
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFireArmAttachmentInterface : FVRInteractiveObject
  {
    public FVRFireArmAttachment Attachment;
    public bool IsLocked;
    public bool ForceInteractable;
    public FVRFireArmAttachmentMount[] SubMounts;
    private Collider m_col;
    private bool m_hasCollider;

    protected override void Awake()
    {
      base.Awake();
      this.m_col = this.GetComponent<Collider>();
      if (!((Object) this.m_col != (Object) null))
        return;
      this.m_hasCollider = true;
    }

    public void SetTriggerState(bool b)
    {
      if (!this.m_hasCollider)
        return;
      this.m_col.enabled = b;
    }

    public override bool IsInteractable()
    {
      if (this.ForceInteractable)
        return true;
      return (Object) this.Attachment != (Object) null && !this.Attachment.IsPickUpLocked && base.IsInteractable();
    }

    public override bool IsDistantGrabbable() => (Object) this.Attachment != (Object) null && !this.Attachment.IsPickUpLocked && base.IsInteractable();

    public virtual void OnAttach()
    {
      this.Attachment.ClearQuickbeltState();
      SM.PlayCoreSoundOverrides(FVRPooledAudioType.GenericClose, this.Attachment.AudClipAttach, this.transform.position, new Vector2(0.2f, 0.2f), new Vector2(1f, 1.1f));
      for (int index1 = 0; index1 < this.SubMounts.Length; ++index1)
      {
        this.SubMounts[index1].Parent = this.Attachment.curMount.Parent;
        if (!this.Attachment.curMount.SubMounts.Contains(this.SubMounts[index1]))
        {
          this.Attachment.curMount.SubMounts.Add(this.SubMounts[index1]);
          for (int index2 = 0; index2 < this.SubMounts[index1].AttachmentsList.Count; ++index2)
            this.Attachment.curMount.Parent.RegisterAttachment(this.SubMounts[index1].AttachmentsList[index2]);
        }
      }
      if (this.Attachment.curMount.GetRootMount().Parent is FVRFireArm && this.Attachment is MuzzleDevice)
        (this.Attachment.curMount.GetRootMount().Parent as FVRFireArm).RegisterMuzzleDevice(this.Attachment as MuzzleDevice);
      this.Attachment.Sensor.SetTriggerState(false);
    }

    public virtual void OnDetach()
    {
      if (this.Attachment.curMount.GetRootMount().Parent is FVRFireArm && this.Attachment is MuzzleDevice)
        (this.Attachment.curMount.GetRootMount().Parent as FVRFireArm).DeRegisterMuzzleDevice(this.Attachment as MuzzleDevice);
      SM.PlayCoreSoundOverrides(FVRPooledAudioType.GenericClose, this.Attachment.AudClipDettach, this.transform.position, new Vector2(0.2f, 0.2f), new Vector2(1f, 1.1f));
      if ((Object) this.Attachment.curMount != (Object) null)
      {
        for (int index1 = 0; index1 < this.SubMounts.Length; ++index1)
        {
          if ((Object) this.SubMounts[index1] != (Object) null)
          {
            this.SubMounts[index1].Parent = (FVRPhysicalObject) null;
            if (this.Attachment.curMount.SubMounts.Contains(this.SubMounts[index1]))
            {
              this.Attachment.curMount.SubMounts.Remove(this.SubMounts[index1]);
              for (int index2 = 0; index2 < this.SubMounts[index1].AttachmentsList.Count; ++index2)
              {
                if ((Object) this.SubMounts[index1].AttachmentsList[index2] != (Object) null)
                  this.Attachment.curMount.Parent.DeRegisterAttachment(this.SubMounts[index1].AttachmentsList[index2]);
              }
            }
          }
        }
      }
      this.Attachment.Sensor.SetTriggerState(true);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      bool flag = false;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.AXButtonDown)
          flag = true;
      }
      else if (hand.Input.TouchpadDown && (double) hand.Input.TouchpadAxes.magnitude > 0.25 && (double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) <= 45.0)
        flag = true;
      if (!flag || this.IsLocked || (!((Object) this.Attachment != (Object) null) || !((Object) this.Attachment.curMount != (Object) null)) || (this.HasAttachmentsOnIt() || !this.Attachment.CanDetach()))
        return;
      this.DetachRoutine(hand);
    }

    public void DetachRoutine(FVRViveHand hand)
    {
      this.EndInteraction(hand);
      hand.ForceSetInteractable((FVRInteractiveObject) this.Attachment);
      this.Attachment.BeginInteraction(hand);
    }

    public bool HasAttachmentsOnIt()
    {
      if (this.SubMounts.Length == 0)
        return false;
      for (int index = 0; index < this.SubMounts.Length; ++index)
      {
        if (this.SubMounts[index].HasAttachmentsOnIt())
          return true;
      }
      return false;
    }
  }
}
