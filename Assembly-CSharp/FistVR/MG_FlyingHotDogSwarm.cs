// Decompiled with JetBrains decompiler
// Type: FistVR.MG_FlyingHotDogSwarm
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class MG_FlyingHotDogSwarm : MonoBehaviour
  {
    public GameObject FlyingHotDogPrefab;
    public Transform FlyingHotDogTargetRoot;
    public Transform[] FlyingHotDogTargets;
    public NavMeshAgent Agent;
    private List<FVRPhysicalObject> m_flyers = new List<FVRPhysicalObject>();
    private List<float> m_diveTicks = new List<float>();
    private List<AudioSource> m_auds = new List<AudioSource>();
    public float NavStabilizePower = 1f;
    public float NavStabilizeDeltaPower = 1f;
    public float NavStabilizeRotPower = 100f;
    public float NavStabilizeRotDeltaPower = 100f;
    public AudioClip Clip_DrillOn;
    public AudioClip Clip_DrillDive;
    private float strikeDistance = 0.27f;
    public LayerMask DamageLM_player;
    private RaycastHit m_hit;
    private float m_refireTick;
    private float ReOrientTick = 1f;

    private void Awake()
    {
      for (int index = 0; index < this.FlyingHotDogTargets.Length; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.FlyingHotDogPrefab, this.FlyingHotDogTargets[index].position, this.FlyingHotDogTargets[index].rotation);
        this.m_flyers.Add(gameObject.GetComponent<FVRPhysicalObject>());
        this.m_diveTicks.Add(Random.Range(1f, 4f));
        this.m_auds.Add(gameObject.GetComponent<AudioSource>());
      }
    }

    private void Start()
    {
      if (!((Object) this.Agent != (Object) null))
        return;
      this.Agent.enabled = true;
    }

    private void FireShot(int i)
    {
      if (i >= this.m_flyers.Count || (Object) this.m_flyers[i] == (Object) null || !Physics.Raycast(this.m_flyers[i].transform.position, this.m_flyers[i].transform.forward, out this.m_hit, this.strikeDistance, (int) this.DamageLM_player, QueryTriggerInteraction.Collide))
        return;
      Damage dam = new Damage();
      dam.Class = Damage.DamageClass.Melee;
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

    private void FixedUpdate()
    {
      for (int index = 0; index < this.m_flyers.Count; ++index)
      {
        if (!((Object) this.m_flyers[index] == (Object) null) && !this.m_flyers[index].IsHeld)
        {
          if ((double) Vector3.Distance(this.m_flyers[index].transform.position, this.FlyingHotDogTargets[index].position) > 4.0)
            this.m_flyers[index].transform.position = this.FlyingHotDogTargets[index].position;
          this.StabilizeTowardsTarget(this.m_flyers[index], this.FlyingHotDogTargets[index], this.m_diveTicks[index]);
          this.TurnTowardsTarget(this.m_flyers[index], this.FlyingHotDogTargets[index]);
        }
      }
      if ((double) this.ReOrientTick > 0.0)
      {
        this.ReOrientTick -= Time.deltaTime;
      }
      else
      {
        this.FlyingHotDogTargetRoot.localRotation = Random.rotation;
        this.ReOrientTick = Random.Range(1f, 5f);
      }
      this.FlyingHotDogTargetRoot.transform.localPosition = new Vector3(0.0f, (float) (1.20000004768372 + (double) Mathf.Sin(Time.time) * 0.100000001490116), 0.0f);
      if ((Object) GM.CurrentPlayerBody != (Object) null && (Object) GM.CurrentPlayerBody.Head != (Object) null)
      {
        Vector3 vector3 = GM.CurrentPlayerBody.Head.transform.position + Vector3.up * -0.25f;
        for (int index = 0; index < this.FlyingHotDogTargets.Length; ++index)
          this.FlyingHotDogTargets[index].LookAt(vector3 + Random.onUnitSphere * 0.04f, Vector3.up);
      }
      for (int index1 = 0; index1 < this.m_diveTicks.Count; ++index1)
      {
        List<float> diveTicks;
        int index2;
        (diveTicks = this.m_diveTicks)[index2 = index1] = diveTicks[index2] - Time.deltaTime;
        if ((double) this.m_diveTicks[index1] < -1.0)
          this.m_diveTicks[index1] = Random.Range(0.5f, 3f);
        if ((double) this.m_diveTicks[index1] > 0.0)
        {
          if ((Object) this.m_auds[index1] != (Object) null && (Object) this.m_auds[index1].clip != (Object) this.Clip_DrillOn)
          {
            this.m_auds[index1].clip = this.Clip_DrillOn;
            if (!this.m_auds[index1].isPlaying)
              this.m_auds[index1].Play();
          }
        }
        else
        {
          if ((double) this.m_refireTick <= 0.0)
          {
            this.m_refireTick = Random.Range(0.05f, 0.1f);
            this.FireShot(index1);
          }
          if ((Object) this.m_auds[index1] != (Object) null && (Object) this.m_auds[index1].clip != (Object) this.Clip_DrillDive)
          {
            this.m_auds[index1].clip = this.Clip_DrillDive;
            if (!this.m_auds[index1].isPlaying)
              this.m_auds[index1].Play();
          }
        }
      }
      if ((double) this.m_refireTick <= 0.0)
        return;
      this.m_refireTick -= Time.deltaTime;
    }

    private void Update()
    {
      int num1 = 0;
      for (int index = 0; index < this.m_flyers.Count; ++index)
      {
        if ((Object) this.m_flyers[index] == (Object) null)
          ++num1;
      }
      if (num1 >= 3)
        Object.Destroy((Object) this.gameObject);
      float num2 = Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.transform.position);
      if ((double) num2 > 20.0 || this.m_flyers.Count <= 0)
      {
        for (int index = 0; index < this.m_flyers.Count; ++index)
          this.m_flyers[index].GetComponent<FVRDestroyableObject>().DestroyEvent();
        Object.Destroy((Object) this.gameObject);
      }
      else
      {
        if ((double) num2 <= 2.0 || this.Agent.pathPending)
          return;
        this.Agent.SetDestination(GM.CurrentPlayerBody.transform.position + GM.CurrentPlayerBody.Head.forward * 0.5f);
      }
    }

    private void StabilizeTowardsTarget(FVRPhysicalObject obj, Transform targ, float diveTick)
    {
      Vector3 vector3 = targ.position;
      if ((double) diveTick < 0.0)
        vector3 = targ.position + targ.forward * 1.7f;
      Vector3 position = obj.transform.position;
      Vector3 target = (vector3 - position) * this.NavStabilizePower * Time.fixedDeltaTime;
      obj.RootRigidbody.velocity = Vector3.MoveTowards(obj.RootRigidbody.velocity, target, this.NavStabilizeDeltaPower * Time.fixedDeltaTime);
    }

    private void TurnTowardsTarget(FVRPhysicalObject obj, Transform targ)
    {
      float angle;
      Vector3 axis;
      (targ.rotation * Quaternion.Inverse(obj.transform.rotation)).ToAngleAxis(out angle, out axis);
      if ((double) angle > 180.0)
        angle -= 360f;
      if ((double) angle == 0.0)
        return;
      Vector3 target = Time.fixedDeltaTime * angle * axis * this.NavStabilizeRotPower;
      obj.RootRigidbody.angularVelocity = Vector3.MoveTowards(obj.RootRigidbody.angularVelocity, target, this.NavStabilizeRotDeltaPower * Time.fixedDeltaTime);
    }
  }
}
