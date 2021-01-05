// Decompiled with JetBrains decompiler
// Type: FistVR.AR3_Lemon
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class AR3_Lemon : MonoBehaviour
  {
    public float InitialVel = 18.8f;
    public LayerMask LM_Vaporize;
    public LayerMask LM_Bounce;
    private Vector3 m_dir;
    private float m_vel;
    private RaycastHit m_hit;
    public int IFF;
    private float m_lifeLeft = 4f;
    public List<GameObject> SplodeOnSpawn;
    public Transform InnerSphere;
    public Transform Lemon;
    public Transform ParticleTrail;
    public AudioEvent AudEvent_Bounce;
    public AudioEvent AudEvent_NearMiss;
    private float m_lastDistanceToHead;
    private float m_timeSinceNearMiss;
    private bool m_isExploded;

    public void Start()
    {
      this.m_dir = this.transform.forward;
      this.m_vel = this.InitialVel;
      this.IFF = GM.CurrentPlayerBody.GetPlayerIFF();
      this.Lemon.rotation = Random.rotation;
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_NearMiss, this.transform.position).FollowThisTransform(this.transform);
      this.m_timeSinceNearMiss = 0.0f;
      this.m_lastDistanceToHead = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position);
    }

    private void Update()
    {
      if ((double) this.m_timeSinceNearMiss < 10.0)
        this.m_timeSinceNearMiss += Time.deltaTime;
      float num = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position);
      if ((double) this.m_timeSinceNearMiss > 0.300000011920929 && (double) num < 3.0 && (double) num < (double) this.m_lastDistanceToHead)
      {
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_NearMiss, this.transform.position).FollowThisTransform(this.transform);
        this.m_timeSinceNearMiss = 0.0f;
      }
      this.m_lastDistanceToHead = num;
      Vector3 position = this.transform.position;
      Vector3 forward = this.transform.forward;
      float maxDistance = this.m_vel * Time.deltaTime;
      Vector3 vector3 = this.transform.position + forward * maxDistance;
      if (Physics.Raycast(position, forward, out this.m_hit, maxDistance, (int) this.LM_Bounce, QueryTriggerInteraction.Ignore))
      {
        vector3 = this.m_hit.point + this.m_hit.normal * (1f / 500f);
        this.m_dir = Vector3.Reflect(this.m_dir, this.m_hit.normal);
        if ((double) num < 30.0)
          SM.PlayCoreSound(FVRPooledAudioType.Impacts, this.AudEvent_Bounce, this.transform.position);
        maxDistance = this.m_hit.distance;
        FXM.SpawnImpactEffect(this.m_hit.point, this.m_hit.normal, 1, ImpactEffectMagnitude.Large, false);
        FXM.InitiateMuzzleFlashLowPriority(this.m_hit.point, this.m_hit.normal, 1.5f, Color.white, 2.5f);
        this.transform.rotation = Quaternion.LookRotation(this.m_dir);
      }
      foreach (RaycastHit raycastHit in Physics.SphereCastAll(position, 0.1f, forward, maxDistance, (int) this.LM_Vaporize))
      {
        if ((Object) raycastHit.collider.attachedRigidbody != (Object) null)
          raycastHit.collider.attachedRigidbody.gameObject.GetComponent<IVaporizable>()?.Vaporize(this.IFF);
      }
      this.transform.position = vector3;
      this.InnerSphere.LookAt(GM.CurrentPlayerBody.Head.position);
      this.m_lifeLeft -= Time.deltaTime;
      if ((double) this.m_lifeLeft >= 0.0)
        return;
      this.Explode();
    }

    private void Explode()
    {
      if (this.m_isExploded)
        return;
      this.m_isExploded = true;
      for (int index = 0; index < this.SplodeOnSpawn.Count; ++index)
        Object.Instantiate<GameObject>(this.SplodeOnSpawn[index], this.transform.position, this.transform.rotation);
      this.ParticleTrail.SetParent((Transform) null);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
