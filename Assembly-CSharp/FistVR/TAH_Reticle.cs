// Decompiled with JetBrains decompiler
// Type: FistVR.TAH_Reticle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TAH_Reticle : MonoBehaviour
  {
    [Header("Prefabs")]
    public GameObject P_Contact;
    public AIEntity IFFScanner;
    public AITargetPrioritySystem Priority;
    public List<TAH_ReticleContact> Contacts = new List<TAH_ReticleContact>();
    private HashSet<Transform> m_trackedTransforms = new HashSet<Transform>();
    public float Range = 50f;
    public bool UsesHealthRing;
    public Renderer HealthRingRend;
    public Color Health_Full;
    public Color Health_Empty;
    public bool UsesVerticality = true;
    public bool ShowArrows = true;
    public bool RegistersEnemies = true;
    public bool UsesReticlePos;

    private void Start()
    {
      this.Priority.Init(this.IFFScanner, 20, 1f, 2f);
      this.IFFScanner.AIEventReceiveEvent += new AIEntity.AIEventReceive(this.EventReceive);
    }

    private void OnDestroy() => this.IFFScanner.AIEventReceiveEvent -= new AIEntity.AIEventReceive(this.EventReceive);

    public void EventReceive(AIEvent e)
    {
      if (!this.RegistersEnemies || e.IsEntity && e.Entity.IFFCode == this.IFFScanner.IFFCode || e.Type != AIEvent.AIEType.Visual)
        return;
      this.Priority.ProcessEvent(e);
    }

    public TAH_ReticleContact RegisterTrackedObject(
      Transform obj,
      TAH_ReticleContact.ContactType type)
    {
      if (this.m_trackedTransforms.Contains(obj))
        return (TAH_ReticleContact) null;
      this.m_trackedTransforms.Add(obj);
      TAH_ReticleContact component = Object.Instantiate<GameObject>(this.P_Contact, this.transform.position, this.transform.rotation, this.transform).GetComponent<TAH_ReticleContact>();
      component.UsesVerticality = this.UsesVerticality;
      component.ShowArrows = this.ShowArrows;
      this.Contacts.Add(component);
      component.InitContact(type, obj, this.Range);
      return component;
    }

    public void DeRegisterTrackedType(TAH_ReticleContact.ContactType type)
    {
      for (int index = this.Contacts.Count - 1; index >= 0; --index)
      {
        if (this.Contacts[index].Type == type)
        {
          this.m_trackedTransforms.Remove(this.Contacts[index].TrackedTransform);
          Object.Destroy((Object) this.Contacts[index].gameObject);
          this.Contacts.RemoveAt(index);
        }
      }
    }

    private void Update()
    {
      if (this.UsesHealthRing)
      {
        float playerHealth = GM.GetPlayerHealth();
        Color color = Color.Lerp(this.Health_Empty, this.Health_Full, playerHealth);
        this.HealthRingRend.material.SetTextureOffset("_MainTex", new Vector2(0.0f, -playerHealth + 0.5f));
        this.HealthRingRend.material.SetColor("_EmissionColor", color);
        this.HealthRingRend.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(GM.CurrentPlayerBody.Head.forward, this.transform.up), this.transform.up);
      }
      this.Priority.Compute();
      for (int index = 0; index < this.Priority.RecentEvents.Count; ++index)
      {
        if (this.Priority.RecentEvents[index].IsEntity)
          this.RegisterTrackedObject(this.Priority.RecentEvents[index].Entity.transform, TAH_ReticleContact.ContactType.Enemy);
      }
      this.UpdateContacts();
    }

    private void UpdateContacts()
    {
      Vector3 position = GM.CurrentPlayerRoot.position;
      if (this.UsesReticlePos)
        position = this.transform.position;
      for (int index = this.Contacts.Count - 1; index >= 0; --index)
      {
        if (!this.Contacts[index].Tick(position))
        {
          this.m_trackedTransforms.Remove(this.Contacts[index].TrackedTransform);
          Object.Destroy((Object) this.Contacts[index].gameObject);
          this.Contacts.RemoveAt(index);
        }
      }
    }
  }
}
