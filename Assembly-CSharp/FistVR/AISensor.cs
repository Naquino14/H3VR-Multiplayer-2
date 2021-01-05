// Decompiled with JetBrains decompiler
// Type: FistVR.AISensor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class AISensor : FVRDestroyableObject
  {
    [Header("Sensor Params")]
    public bool IsOmni;
    public AISensorSystem m_sensorSystem;
    public Transform vision_CastPoint;
    public LayerMask vision_LayerMask;
    public LayerMask vision_LayerMask_Omni;
    public LayerMask vision_Targets;
    public float vision_Distance;
    public float vision_Angle = 25f;
    private RaycastHit m_hit;
    private Queue<Transform> SensorContactQueue = new Queue<Transform>();
    private HashSet<Transform> SensorContactHashes = new HashSet<Transform>();

    public bool SensorLoop()
    {
      Transform t = (Transform) null;
      if (this.SensorContactQueue.Count == 0)
        this.SensorContactHashes.Clear();
      else
        t = this.SensorContactQueue.Dequeue();
      Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.vision_Distance, (int) this.vision_Targets, QueryTriggerInteraction.Collide);
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        if (this.IsOmni)
          this.TriggerProxy(colliderArray[index]);
        else if ((double) Vector3.Angle((colliderArray[index].transform.position - this.transform.position).normalized, this.transform.forward) < (double) this.vision_Angle)
          this.TriggerProxy(colliderArray[index]);
      }
      if ((Object) t != (Object) null)
      {
        if (this.IsOmni)
        {
          if (Physics.Raycast(this.vision_CastPoint.position, t.position - this.vision_CastPoint.position, out this.m_hit, this.vision_Distance, (int) this.vision_LayerMask_Omni, QueryTriggerInteraction.Collide) && (Object) this.m_hit.collider.transform.root == (Object) t.root)
            this.m_sensorSystem.Regard(t);
        }
        else if (Physics.Raycast(this.vision_CastPoint.position, t.position - this.vision_CastPoint.position, out this.m_hit, this.vision_Distance, (int) this.vision_LayerMask, QueryTriggerInteraction.Collide) && (Object) this.m_hit.collider.transform.root == (Object) t.root)
          this.m_sensorSystem.Regard(t);
      }
      return true;
    }

    public void TriggerProxy(Collider other)
    {
      if ((Object) other.gameObject.transform == (Object) null || (Object) other.attachedRigidbody == (Object) null || (!other.attachedRigidbody.gameObject.activeSelf || other.attachedRigidbody.gameObject.tag == this.m_sensorSystem.IFFvalue) || !this.SensorContactHashes.Add(other.attachedRigidbody.gameObject.transform))
        return;
      this.SensorContactQueue.Enqueue(other.attachedRigidbody.gameObject.transform);
    }

    public new virtual void DestroyEvent()
    {
    }
  }
}
