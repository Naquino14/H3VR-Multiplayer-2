// Decompiled with JetBrains decompiler
// Type: FistVR.SmartTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SmartTrigger : FVRFireArmAttachmentInterface
  {
    public LayerMask SensingLayer;
    public string SensingString = "Geo_Head";
    public InputOverrider Overrider;
    private RaycastHit m_hit;
    private bool m_hasTriggeredUpSinceDischarge = true;

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      bool flag = false;
      if ((Object) this.Attachment.curMount != (Object) null && (Object) this.Attachment.curMount.MyObject != (Object) null && this.Attachment.curMount.MyObject.IsHeld)
        flag = true;
      if (flag)
      {
        this.Overrider.ConnectToHand(this.Attachment.curMount.MyObject.m_hand);
        this.RunSensingProgram(this.Attachment.curMount.MyObject.m_hand);
      }
      else
        this.Overrider.FlushHandConnection();
    }

    private void RunSensingProgram(FVRViveHand h)
    {
      if (this.Overrider.Real_triggerUp)
        this.m_hasTriggeredUpSinceDischarge = true;
      bool m_pressed = false;
      if (this.Overrider.Real_triggerPressed && this.m_hasTriggeredUpSinceDischarge)
      {
        FVRFireArm fvrFireArm = this.Attachment.curMount.MyObject as FVRFireArm;
        if (Physics.Raycast(fvrFireArm.GetMuzzle().position, fvrFireArm.GetMuzzle().forward, out this.m_hit, 100f, (int) this.SensingLayer) && (Object) this.m_hit.collider.attachedRigidbody != (Object) null)
        {
          SosigLink component = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
          if ((Object) component != (Object) null && component.BodyPart == SosigLink.SosigBodyPart.Head)
            m_pressed = true;
        }
      }
      this.Overrider.UpdateTrigger(m_pressed);
    }
  }
}
