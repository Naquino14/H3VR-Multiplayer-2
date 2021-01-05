// Decompiled with JetBrains decompiler
// Type: FistVR.FVRBeamerLocus
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRBeamerLocus : MonoBehaviour
  {
    public List<Rigidbody> IgnoredRBs;
    public FVRBeamer Beamer;
    private HashSet<Rigidbody> m_rbHash = new HashSet<Rigidbody>();
    private List<Rigidbody> m_rbList = new List<Rigidbody>();
    private bool m_isExists;
    private bool m_isGrav;
    private Vector3 lastPos = Vector3.zero;
    private Vector3 lastlastPos = Vector3.zero;
    private float m_size = 0.01f;
    private Vector3 m_targetPoint = Vector3.zero;
    public GameObject Lightning;
    public ParticleSystem Lightning2;

    public void Awake()
    {
      this.lastPos = this.transform.position;
      this.lastlastPos = this.transform.position;
      this.m_targetPoint = this.transform.position;
      this.Lightning.SetActive(false);
      this.m_size = 0.01f;
    }

    public void SetExistence(bool b) => this.m_isExists = b;

    public void SetGrav(bool b)
    {
      this.m_isGrav = b;
      this.Lightning.SetActive(b);
    }

    public void SetTargetPoint(Vector3 p) => this.m_targetPoint = p;

    public void Shunt()
    {
      if (this.m_rbList.Count > 0)
      {
        foreach (Rigidbody rigidbody in this.m_rbHash)
        {
          if ((Object) rigidbody != (Object) null)
            rigidbody.AddForceAtPosition(this.Beamer.transform.forward * 5000f, this.transform.position, ForceMode.Acceleration);
        }
      }
      this.Lightning2.Emit(30);
      this.Beamer.Shunt();
      this.SetGrav(false);
    }

    private void FixedUpdate()
    {
      this.transform.position = Vector3.Lerp(this.transform.position, this.m_targetPoint, Time.deltaTime * 4f);
      if (this.m_isExists)
      {
        this.m_size += Time.deltaTime * 0.5f;
        this.m_size = Mathf.Clamp(this.m_size, 0.01f, 1f);
        this.transform.localScale = new Vector3(this.m_size, this.m_size, this.m_size);
      }
      else
      {
        this.m_size -= Time.deltaTime * 3f;
        this.transform.localScale = new Vector3(this.m_size, this.m_size, this.m_size);
        if ((double) this.m_size <= 0.00999999977648258)
        {
          this.m_size = 0.01f;
          this.transform.localScale = new Vector3(this.m_size, this.m_size, this.m_size);
          this.gameObject.SetActive(false);
        }
      }
      if (this.m_isGrav)
      {
        foreach (Rigidbody rigidbody in this.m_rbHash)
        {
          if ((Object) rigidbody != (Object) null)
          {
            rigidbody.velocity = (this.transform.position - rigidbody.transform.position).normalized;
            rigidbody.transform.position += this.transform.position - this.lastPos;
          }
        }
      }
      else
      {
        foreach (Rigidbody rb in this.m_rbList)
        {
          if ((Object) rb != (Object) null)
            rb.AddForce((this.lastPos - this.lastlastPos) * (1f / Time.deltaTime) * 100f, ForceMode.Acceleration);
        }
        this.m_rbHash.Clear();
        this.m_rbList.Clear();
      }
      this.lastlastPos = this.lastPos;
      this.lastPos = this.transform.position;
    }

    private void OnTriggerStay(Collider col)
    {
      if (!this.m_isGrav || this.m_rbList.Count > 5 || (!((Object) col.attachedRigidbody != (Object) null) || col.attachedRigidbody.isKinematic) || this.IgnoredRBs.Contains(col.attachedRigidbody))
        return;
      Rigidbody attachedRigidbody = col.attachedRigidbody;
      if (!this.m_rbHash.Add(attachedRigidbody))
        return;
      this.m_rbList.Add(attachedRigidbody);
    }

    private void OnTriggerExit(Collider col)
    {
      if (!((Object) col.attachedRigidbody != (Object) null) || col.attachedRigidbody.isKinematic)
        return;
      Rigidbody attachedRigidbody = col.attachedRigidbody;
      if (!this.m_rbHash.Remove(attachedRigidbody))
        return;
      this.m_rbList.Remove(attachedRigidbody);
    }
  }
}
