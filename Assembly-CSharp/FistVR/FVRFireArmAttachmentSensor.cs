// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmAttachmentSensor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFireArmAttachmentSensor : MonoBehaviour
  {
    public FVRFireArmAttachment Attachment;
    [HideInInspector]
    public FVRFireArmAttachmentMount CurHoveredMount;
    private float m_storedScaleMagnified = 1f;
    private Collider m_col;
    private bool m_hasCollider;

    private void Awake()
    {
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

    private void OnTriggerEnter(Collider collider)
    {
      if (!this.Attachment.IsHeld || !((Object) this.CurHoveredMount == (Object) null) || (!this.Attachment.CanAttach() || !(collider.gameObject.tag == "FVRFireArmAttachmentMount")))
        return;
      FVRFireArmAttachmentMount component = collider.gameObject.GetComponent<FVRFireArmAttachmentMount>();
      if (component.Type != this.Attachment.Type || !component.isMountableOn(this.Attachment))
        return;
      this.SetHoveredMount(component);
      component.BeginHover();
    }

    private void OnTriggerExit(Collider collider)
    {
      if (!((Object) this.CurHoveredMount != (Object) null) || !(collider.gameObject.tag == "FVRFireArmAttachmentMount"))
        return;
      FVRFireArmAttachmentMount component = collider.gameObject.GetComponent<FVRFireArmAttachmentMount>();
      if (!((Object) component == (Object) this.CurHoveredMount))
        return;
      component.EndHover();
      this.SetHoveredMount((FVRFireArmAttachmentMount) null);
    }

    private void Update()
    {
      if (!((Object) this.CurHoveredMount != (Object) null) || this.CurHoveredMount.isMountableOn(this.Attachment))
        return;
      this.CurHoveredMount = (FVRFireArmAttachmentMount) null;
    }

    private void SetHoveredMount(FVRFireArmAttachmentMount newMount)
    {
      if ((Object) newMount == (Object) this.CurHoveredMount)
        return;
      this.CurHoveredMount = newMount;
      if (this.Attachment.CanScaleToMount && (Object) this.CurHoveredMount != (Object) null && this.CurHoveredMount.CanThisRescale())
      {
        FVRFireArmAttachmentMount rootMount = this.CurHoveredMount.GetRootMount();
        if ((double) this.m_storedScaleMagnified == (double) rootMount.ScaleModifier)
          return;
        this.m_storedScaleMagnified = rootMount.ScaleModifier;
        this.Attachment.transform.localScale = new Vector3(this.m_storedScaleMagnified, this.m_storedScaleMagnified, this.m_storedScaleMagnified);
      }
      else
      {
        if ((double) this.m_storedScaleMagnified == 1.0)
          return;
        this.m_storedScaleMagnified = 1f;
        this.Attachment.transform.localScale = new Vector3(this.m_storedScaleMagnified, this.m_storedScaleMagnified, this.m_storedScaleMagnified);
      }
    }
  }
}
