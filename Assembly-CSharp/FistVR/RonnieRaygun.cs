// Decompiled with JetBrains decompiler
// Type: FistVR.RonnieRaygun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class RonnieRaygun : MonoBehaviour
  {
    public Transform Gun;
    public Transform GunMuzzle;
    public GameObject Projectile;
    private Vector3 currentTarget;
    public AudioSource GunAudio;
    public AudioClip[] AudClip_Raygun;
    public float AngleThreshold = 40f;
    private float m_RefireTick = 0.2f;
    public Vector2 RefireRange = new Vector2(0.2f, 0.5f);
    public NavMeshAgent Agent;
    public Transform Facing;
    public Vector2 SpeedRange;
    public float AccurateFireDistance = 20f;
    public Vector2 PitchRange = new Vector2(0.95f, 1.05f);
    private float NavTick = 1f;

    private void Start()
    {
      this.NavTick = Random.Range(0.1f, 5f);
      this.Agent.enabled = true;
      this.Agent.speed = Random.Range(this.SpeedRange.x, this.SpeedRange.y);
    }

    private void Update()
    {
      this.currentTarget = GM.CurrentPlayerBody.gameObject.transform.position + Vector3.up * 1.2f;
      if ((double) this.m_RefireTick <= 0.0)
      {
        if ((double) Vector3.Angle(this.currentTarget - this.Gun.position, this.Facing.forward) <= (double) this.AngleThreshold)
        {
          if ((double) Vector3.Distance(this.currentTarget, this.transform.position) < (double) this.AccurateFireDistance)
          {
            this.Gun.transform.LookAt(this.currentTarget + Random.onUnitSphere * 0.3f, Vector3.up);
            this.m_RefireTick = Random.Range(this.RefireRange.x, this.RefireRange.y);
          }
          else
          {
            this.Gun.transform.LookAt(this.Facing.position + this.Facing.forward * 10f + this.Facing.up * 10f + Random.onUnitSphere * 4f);
            this.m_RefireTick = Random.Range(this.RefireRange.x * 2f, this.RefireRange.y * 2.5f);
          }
          this.Fire();
        }
      }
      else
        this.m_RefireTick -= Time.deltaTime;
      this.NavUpdate();
    }

    private void NavUpdate()
    {
      if ((double) Vector3.Distance(this.currentTarget, this.transform.position) < 20.0)
      {
        Vector3 forward = this.currentTarget - this.Facing.position;
        forward.y = 0.0f;
        this.Facing.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
      }
      else
        this.Facing.transform.localEulerAngles = Vector3.zero;
      if ((double) this.NavTick <= 0.0)
      {
        this.NavTick = Random.Range(2f, 6f);
        this.GrabPath();
      }
      else
        this.NavTick -= Time.deltaTime;
    }

    private void GrabPath()
    {
      Vector3 onUnitSphere = Random.onUnitSphere;
      onUnitSphere.y = 0.0f;
      onUnitSphere.Normalize();
      this.Agent.SetDestination(this.currentTarget + onUnitSphere * 2f);
    }

    private void Fire()
    {
      FVRProjectile component = Object.Instantiate<GameObject>(this.Projectile, this.GunMuzzle.position, this.GunMuzzle.rotation).GetComponent<FVRProjectile>();
      component.Fire(component.transform.forward, (FVRFireArm) null);
      this.GunAudio.pitch = Random.Range(this.PitchRange.x, this.PitchRange.y);
      this.GunAudio.PlayOneShot(this.AudClip_Raygun[Random.Range(0, this.AudClip_Raygun.Length)], 1f);
      FXM.InitiateMuzzleFlash(this.GunMuzzle.position, this.GunMuzzle.forward, 4.5f, Color.red, 2f);
    }
  }
}
