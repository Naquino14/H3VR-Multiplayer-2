// Decompiled with JetBrains decompiler
// Type: FistVR.AISensorSystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class AISensorSystem : MonoBehaviour
  {
    public string IFFvalue;
    public AISensorSystem.SensorPing PriorityTarget;
    public AISensorSystem.SensorPing LastPriorityTarget;
    private HashSet<Transform> m_knownTargetsHash = new HashSet<Transform>();
    private Dictionary<Transform, AISensorSystem.SensorPing> m_knownTargetsDic = new Dictionary<Transform, AISensorSystem.SensorPing>();
    private HashSet<AISensorSystem.SensorPing> m_detectedHash = new HashSet<AISensorSystem.SensorPing>();
    private List<AISensorSystem.SensorPing> m_detectedTargets = new List<AISensorSystem.SensorPing>();
    private float m_timeTilDisregard = 6f;
    public int SensorTestCount = 1;
    public AISensor[] Sensors;
    private int m_current_vision_tests;
    private int m_current_sensor;
    private AudioSource m_aud;
    public AudioClip AudClip_NewSensorContact;
    public AudioClip AudClip_ContactLost;
    private float soundCooldown = 0.6f;
    public float soundCooldownReset = 1.5f;

    private void Awake() => this.m_aud = this.GetComponent<AudioSource>();

    public void Regard(Transform t)
    {
      if (this.m_knownTargetsHash.Add(t))
      {
        AISensorSystem.SensorPing sensorPing = new AISensorSystem.SensorPing();
        sensorPing.Tr = t;
        if ((UnityEngine.Object) t.gameObject.GetComponent<Rigidbody>() != (UnityEngine.Object) null)
          sensorPing.Rb = t.gameObject.GetComponent<Rigidbody>();
        this.m_knownTargetsDic.Add(t, sensorPing);
      }
      if (this.m_detectedHash.Add(this.m_knownTargetsDic[t]))
        this.m_detectedTargets.Add(this.m_knownTargetsDic[t]);
      this.m_knownTargetsDic[t].LastKnownPosition = t.position;
      this.m_knownTargetsDic[t].TimeLastSeen = Time.time;
      this.m_knownTargetsDic[t].threat = -Vector3.Distance(this.transform.position, t.position);
    }

    public void UpdateSensorSystem()
    {
      if ((double) this.soundCooldown > 0.0)
        this.soundCooldown -= Time.deltaTime;
      if (this.m_detectedTargets.Count > 0)
      {
        for (int index = this.m_detectedTargets.Count - 1; index >= 0; --index)
        {
          if (this.m_detectedTargets[index] == null || (double) Time.time - (double) this.m_detectedTargets[index].TimeLastSeen > (double) this.m_timeTilDisregard)
          {
            this.m_detectedHash.Remove(this.m_detectedTargets[index]);
            this.m_detectedTargets.RemoveAt(index);
          }
        }
      }
      else
        this.SetPriorityTarget((AISensorSystem.SensorPing) null);
      if (this.m_detectedTargets.Count > 0)
      {
        this.m_detectedTargets.Sort();
        this.SetPriorityTarget(this.m_detectedTargets[0]);
      }
      for (; this.m_current_sensor < this.Sensors.Length && this.m_current_vision_tests < this.SensorTestCount; ++this.m_current_sensor)
      {
        if ((UnityEngine.Object) this.Sensors[this.m_current_sensor] != (UnityEngine.Object) null && this.Sensors[this.m_current_sensor].SensorLoop())
          ++this.m_current_vision_tests;
      }
      if (this.m_current_sensor >= this.Sensors.Length)
        this.m_current_sensor = 0;
      this.m_current_vision_tests = 0;
      for (int index = 0; index < this.m_detectedTargets.Count; ++index)
        Debug.DrawLine(this.transform.position, this.m_detectedTargets[index].LastKnownPosition, Color.red);
    }

    private void SetPriorityTarget(AISensorSystem.SensorPing ping)
    {
      if (this.PriorityTarget != null && ping == null)
      {
        if ((double) this.soundCooldown <= 0.0)
        {
          this.soundCooldown = this.soundCooldownReset;
          this.m_aud.PlayOneShot(this.AudClip_ContactLost, 1f);
        }
      }
      else if (ping != this.PriorityTarget && (double) this.soundCooldown <= 0.0)
      {
        this.soundCooldown = this.soundCooldownReset;
        this.m_aud.PlayOneShot(this.AudClip_NewSensorContact, 1f);
      }
      this.PriorityTarget = ping;
      if (ping == null)
        return;
      this.LastPriorityTarget = ping;
    }

    public class SensorPing : IComparable<AISensorSystem.SensorPing>
    {
      public Transform Tr;
      public Rigidbody Rb;
      public float threat;
      public Vector3 LastKnownPosition = Vector3.zero;
      public float TimeLastSeen;

      public int CompareTo(AISensorSystem.SensorPing other)
      {
        if (other == null)
          return 1;
        if ((double) this.threat > (double) other.threat)
          return -1;
        return (double) this.threat < (double) other.threat ? 1 : 0;
      }
    }
  }
}
