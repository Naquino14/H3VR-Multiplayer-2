// Decompiled with JetBrains decompiler
// Type: FistVR.FVRBodyPiece
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRBodyPiece : MonoBehaviour
  {
    public Transform TrackedPos;
    private Rigidbody m_trackedPosRB;
    public Transform AlternateYPos;
    private Rigidbody m_alternateYPosRB;
    public bool TracksPosition;
    private Rigidbody m_rb;

    public void Awake()
    {
      if ((Object) this.GetComponent<Rigidbody>() != (Object) null)
        this.m_rb = this.GetComponent<Rigidbody>();
      if (!((Object) this.TrackedPos != (Object) null) || !(bool) (Object) this.TrackedPos.gameObject.GetComponent<Rigidbody>())
        ;
      if (!((Object) this.AlternateYPos != (Object) null) || !((Object) this.AlternateYPos.gameObject.GetComponent<Rigidbody>() != (Object) null))
        return;
      this.m_alternateYPosRB = this.AlternateYPos.gameObject.GetComponent<Rigidbody>();
    }

    public void SetTrackedPos(Transform t)
    {
      this.TrackedPos = t;
      if (!(bool) (Object) t.gameObject.GetComponent<Rigidbody>())
        ;
    }

    private void LateUpdate()
    {
      if ((Object) this.m_rb == (Object) null)
      {
        if (this.TracksPosition)
        {
          if ((Object) this.AlternateYPos == (Object) null)
            this.transform.position = this.TrackedPos.position;
          else
            this.transform.position = new Vector3(this.TrackedPos.position.x, this.AlternateYPos.position.y, this.TrackedPos.position.z);
        }
        Vector3 forward = this.TrackedPos.forward;
        Vector3 vector3 = forward;
        vector3.y = 0.0f;
        vector3.Normalize();
        Vector3 zero = Vector3.zero;
        Vector3 a = (double) forward.y <= 0.0 ? this.TrackedPos.up : -this.TrackedPos.up;
        a.y = 0.0f;
        a.Normalize();
        float num = Mathf.Clamp(Vector3.Dot(vector3, forward), 0.0f, 1f);
        this.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(a, vector3, num * num), Vector3.up);
      }
      else if ((Object) this.m_trackedPosRB != (Object) null)
      {
        if (this.TracksPosition)
        {
          if ((Object) this.AlternateYPos == (Object) null)
            this.m_rb.MovePosition(this.m_trackedPosRB.position);
          else
            this.m_rb.MovePosition(new Vector3(this.m_trackedPosRB.position.x, this.AlternateYPos.position.y, this.m_trackedPosRB.position.z));
        }
        Vector3 forward = this.TrackedPos.forward;
        Vector3 vector3 = forward;
        vector3.y = 0.0f;
        vector3.Normalize();
        Vector3 zero = Vector3.zero;
        Vector3 a = (double) forward.y <= 0.0 ? this.TrackedPos.up : -this.TrackedPos.up;
        a.y = 0.0f;
        a.Normalize();
        float num = Mathf.Clamp(Vector3.Dot(vector3, forward), 0.0f, 1f);
        this.m_rb.rotation = Quaternion.LookRotation(Vector3.Lerp(a, vector3, num * num), Vector3.up);
      }
      else
      {
        if (this.TracksPosition)
        {
          if ((Object) this.AlternateYPos == (Object) null)
            this.m_rb.MovePosition(this.TrackedPos.position);
          else
            this.m_rb.MovePosition(new Vector3(this.TrackedPos.position.x, this.AlternateYPos.position.y, this.TrackedPos.position.z));
        }
        Vector3 forward = this.TrackedPos.forward;
        Vector3 vector3 = forward;
        vector3.y = 0.0f;
        vector3.Normalize();
        Vector3 zero = Vector3.zero;
        Vector3 a = (double) forward.y <= 0.0 ? this.TrackedPos.up : -this.TrackedPos.up;
        a.y = 0.0f;
        a.Normalize();
        float num = Mathf.Clamp(Vector3.Dot(vector3, forward), 0.0f, 1f);
        this.m_rb.MoveRotation(Quaternion.LookRotation(Vector3.Lerp(a, vector3, num * num), Vector3.up));
      }
    }
  }
}
