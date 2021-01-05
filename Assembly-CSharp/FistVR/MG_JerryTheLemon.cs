// Decompiled with JetBrains decompiler
// Type: FistVR.MG_JerryTheLemon
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class MG_JerryTheLemon : MonoBehaviour, IFVRDamageable
  {
    public NavMeshAgent Agent;
    private float m_life = 9000f;
    public GameObject[] Spawns;
    private bool m_isExploded;
    private Vector3 pos;
    public GameObject[] BoltPrefabs;
    public AudioSource LightningSource;
    public AudioClip[] Audioclips;
    public LayerMask DamageLM_player;
    private RaycastHit m_hit;
    private float FireBoltTick = 0.25f;
    private int pulseTick;

    private void Start() => this.pos = this.transform.localPosition;

    private void Update()
    {
      float num = Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.transform.position);
      if ((double) num > 23.0)
        Object.Destroy((Object) this.Agent.gameObject);
      else if ((double) num > 5.0 && !this.Agent.pathPending)
        this.Agent.SetDestination(GM.CurrentPlayerBody.transform.position + GM.CurrentPlayerBody.Head.forward * 2f);
      if ((double) num < 12.0)
      {
        --this.pulseTick;
        if (this.pulseTick <= 0)
        {
          this.pulseTick = Random.Range(1, 4);
          FXM.InitiateMuzzleFlash(this.transform.position + Random.onUnitSphere, Random.onUnitSphere, Random.Range(2f, 4f), new Color(1f, 1f, 0.1f, 1f), Random.Range(2f, 4f));
        }
      }
      if ((double) this.FireBoltTick > 0.0)
      {
        this.FireBoltTick -= Time.deltaTime;
      }
      else
      {
        this.FireBoltTick = Random.Range(3.5f, 6f);
        if ((double) num < 6.0)
          this.FireBolt();
      }
      this.transform.localPosition = this.pos + Random.onUnitSphere * 0.01f;
    }

    private void FireBolt()
    {
      Vector3 vector3 = GM.CurrentPlayerBody.Head.position - Vector3.up * 0.35f + Random.onUnitSphere * 0.2f - this.transform.position;
      this.LightningSource.PlayOneShot(this.Audioclips[Random.Range(0, this.Audioclips.Length)], 1f);
      FXM.InitiateMuzzleFlash(Vector3.zero, vector3, Random.Range(4f, 10f), new Color(1f, 1f, 0.9f, 1f), Random.Range(3f, 5f));
      Object.Instantiate<GameObject>(this.BoltPrefabs[Random.Range(0, this.BoltPrefabs.Length)], this.transform.position, Quaternion.LookRotation(vector3));
      if (!Physics.Raycast(this.transform.position, vector3.normalized, out this.m_hit, vector3.magnitude + 1f, (int) this.DamageLM_player, QueryTriggerInteraction.Collide))
        return;
      FistVR.Damage dam = new FistVR.Damage();
      dam.Class = FistVR.Damage.DamageClass.Projectile;
      dam.Dam_Piercing = 500f;
      dam.Dam_TotalKinetic = 500f;
      dam.point = this.m_hit.point;
      dam.hitNormal = this.m_hit.normal;
      dam.strikeDir = this.transform.forward;
      IFVRDamageable component = this.m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
      if (component != null)
      {
        component.Damage(dam);
      }
      else
      {
        if (component != null || !((Object) this.m_hit.collider.attachedRigidbody != (Object) null))
          return;
        this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>()?.Damage(dam);
      }
    }

    public void Damage(FistVR.Damage d)
    {
      this.m_life -= d.Dam_TotalKinetic;
      if ((double) this.m_life > 0.0)
        return;
      this.Explode();
    }

    private void Explode()
    {
      if (this.m_isExploded)
        return;
      this.m_isExploded = true;
      for (int index = 0; index < this.Spawns.Length; ++index)
        Object.Instantiate<GameObject>(this.Spawns[index], this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
