// Decompiled with JetBrains decompiler
// Type: FistVR.HCBBolt
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class HCBBolt : MonoBehaviour
  {
    public HCBBolt.HBCBoltType BoltType;
    private bool m_isFlying;
    [Header("Ballistics")]
    public float BaseSpeed = 40f;
    public float GravityMultiplier = 1f;
    public float AirDragMultiplier = 1f;
    public Vector3 Dimensions;
    public float DragCoefficient;
    public float Mass;
    private Vector3 m_velocity = Vector3.zero;
    private Vector3 m_forward = Vector3.forward;
    private Vector3 m_lastPoint = Vector3.zero;
    private float m_gravMag = 9.81f;
    [Header("Raycasting")]
    public LayerMask LM_Hit;
    public LayerMask LM_Env;
    public LayerMask LM_Agent;
    private RaycastHit m_hit;
    private RaycastHit m_hit2;
    public float PinDistanceLimit = 1f;
    public Vector2 LinkPenetrationRange;
    private bool m_isTickingDownToDestroy;
    private float m_tickDownToDestroy = 30f;
    public Renderer Rend;
    [Header("VFX")]
    public GameObject MeatSplosion;
    [Header("Sound")]
    public AudioEvent AudEvent_Hit_Skewer;
    public AudioEvent AudEvent_Hit_Meat;
    public AudioEvent AudEvent_Hit_Solid;
    public AudioEvent AudEvent_Hit_Solid_Light;
    private int m_numBounces;
    private float m_cookedAmount;

    public void Fire(Vector3 dir, Vector3 initPos, float velMult)
    {
      switch (GM.Options.SimulationOptions.BallisticGravityMode)
      {
        case SimulationOptions.GravityMode.Realistic:
          this.m_gravMag = 9.81f;
          break;
        case SimulationOptions.GravityMode.Playful:
          this.m_gravMag = 5f;
          break;
        case SimulationOptions.GravityMode.OnTheMoon:
          this.m_gravMag = 1.622f;
          break;
        case SimulationOptions.GravityMode.None:
          this.m_gravMag = 0.0f;
          break;
      }
      this.m_velocity = dir.normalized * this.BaseSpeed * velMult;
      this.m_forward = dir;
      this.m_isFlying = true;
    }

    public void SetCookedAmount(float f)
    {
      this.Rend.material.SetFloat("_BlendScale", f);
      this.m_cookedAmount = f;
    }

    private void Update()
    {
      if (this.m_isTickingDownToDestroy)
      {
        this.m_tickDownToDestroy -= Time.deltaTime;
        if ((double) this.m_tickDownToDestroy <= 0.0)
          Object.Destroy((Object) this.gameObject);
      }
      if (!this.m_isFlying)
        return;
      float deltaTime = Time.deltaTime;
      this.m_velocity += Vector3.down * this.m_gravMag * deltaTime * this.GravityMultiplier;
      this.m_velocity = this.ApplyDrag(this.m_velocity, 1.225f * this.AirDragMultiplier, deltaTime);
      Vector3 position = this.transform.position;
      Vector3 normalized = this.m_velocity.normalized;
      float magnitude = this.m_velocity.magnitude;
      float maxDistance = magnitude * deltaTime;
      if (!Physics.Raycast(position, normalized, out this.m_hit, maxDistance, (int) this.LM_Hit, QueryTriggerInteraction.Collide))
      {
        this.transform.position = position + this.m_velocity * deltaTime;
        this.transform.rotation = Quaternion.LookRotation(normalized);
      }
      else
      {
        this.m_isFlying = false;
        Rigidbody attachedRigidbody = this.m_hit.collider.attachedRigidbody;
        if ((double) this.m_cookedAmount < 0.400000005960464)
        {
          SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.AudEvent_Hit_Meat, this.transform.position, Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
          Object.Instantiate<GameObject>(this.MeatSplosion, this.m_hit.point, Quaternion.identity);
          Object.Destroy((Object) this.gameObject);
        }
        else if ((Object) attachedRigidbody == (Object) null)
        {
          FXM.SpawnImpactEffect(this.m_hit.point, this.m_hit.normal, 1, ImpactEffectMagnitude.Medium, false);
          if ((double) Vector3.Angle(-normalized, this.m_hit.normal) > 60.0 && this.m_numBounces < 4)
          {
            ++this.m_numBounces;
            this.m_velocity = Vector3.Reflect(normalized, this.m_hit.normal).normalized * (magnitude * 0.8f);
            this.transform.rotation = Quaternion.LookRotation(this.m_velocity);
            this.transform.position = this.m_hit.point + this.m_hit.normal * (1f / 1000f);
            SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.AudEvent_Hit_Solid_Light, this.transform.position, Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
          }
          else
          {
            this.transform.rotation = Quaternion.LookRotation(normalized);
            this.StickIntoWall(this.m_hit.point);
            SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.AudEvent_Hit_Solid, this.transform.position, Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
          }
        }
        else
        {
          SosigLink component = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
          if ((Object) component == (Object) null)
          {
            this.DamageOtherThing(this.m_hit.collider, attachedRigidbody, this.m_hit.point - this.m_hit.normal * 0.005f, normalized, true);
            this.Explode(this.m_hit.point);
          }
          else
          {
            this.DamageOtherThing(this.m_hit.collider, attachedRigidbody, this.m_hit.point - this.m_hit.normal * 0.005f, normalized, true);
            this.ImpaleCheck(component, this.m_hit.point - this.m_hit.normal * 0.005f, normalized);
          }
        }
      }
    }

    private Vector3 ApplyDrag(Vector3 velocity, float materialDensity, float time)
    {
      float num = 3.141593f * Mathf.Pow(this.Dimensions.x * 0.5f, 2f);
      float magnitude = velocity.magnitude;
      Vector3 normalized = velocity.normalized;
      float dragCoefficient = this.DragCoefficient;
      Vector3 vector3 = -velocity * (materialDensity * 0.5f * dragCoefficient * num / this.Mass) * magnitude;
      return normalized * Mathf.Clamp(magnitude - vector3.magnitude * time, 0.0f, magnitude);
    }

    private void Explode(Vector3 point) => Object.Destroy((Object) this.gameObject);

    private void StickIntoWall(Vector3 point)
    {
      this.transform.position = point + this.transform.forward * Random.Range(this.LinkPenetrationRange.x, this.LinkPenetrationRange.y);
      this.m_isTickingDownToDestroy = true;
    }

    private void DamageOtherThing(
      Collider c,
      Rigidbody r,
      Vector3 hitPoint,
      Vector3 castDir,
      bool passOn)
    {
      IFVRDamageable component = c.transform.gameObject.GetComponent<IFVRDamageable>();
      if (component == null && passOn)
        component = r.gameObject.GetComponent<IFVRDamageable>();
      if (component == null)
        return;
      Damage dam = new Damage()
      {
        Class = Damage.DamageClass.Projectile,
        damageSize = this.Dimensions.x,
        hitNormal = -castDir,
        Source_IFF = GM.CurrentPlayerBody.GetPlayerIFF(),
        strikeDir = castDir,
        point = hitPoint,
        Dam_Piercing = 3000f,
        Dam_TotalKinetic = 3000f
      };
      dam.point = hitPoint;
      dam.Dam_Stunning = 2f;
      component.Damage(dam);
    }

    private void ImpaleCheck(SosigLink l, Vector3 hitPoint, Vector3 castDir)
    {
      float maxDistance = this.Dimensions.z + this.PinDistanceLimit;
      Damage d = new Damage();
      d.Class = Damage.DamageClass.Projectile;
      d.damageSize = this.Dimensions.x;
      d.hitNormal = -castDir;
      d.Source_IFF = GM.CurrentPlayerBody.GetPlayerIFF();
      d.strikeDir = castDir;
      d.Dam_Piercing = 500f;
      d.Dam_TotalKinetic = 500f;
      d.point = hitPoint;
      d.Dam_Stunning = 2f;
      Vector3 point1 = l.transform.position + l.transform.up * 0.15f;
      Vector3 point2 = l.transform.position - l.transform.up * 0.15f;
      bool flag1 = false;
      bool flag2 = false;
      if (Physics.CapsuleCast(point1, point2, 0.13f, castDir, out this.m_hit, maxDistance, (int) this.LM_Env, QueryTriggerInteraction.Ignore))
      {
        if (Physics.Raycast(hitPoint, castDir, out this.m_hit2, this.m_hit.distance + this.Dimensions.z, (int) this.LM_Env, QueryTriggerInteraction.Collide) && (Object) l.gameObject.GetComponent<FixedJoint>() == (Object) null)
        {
          l.transform.position = l.transform.position + this.m_hit.distance * castDir;
          this.transform.position = this.m_hit2.point + castDir * 0.05f;
          l.gameObject.AddComponent<FixedJoint>();
          l.S.KillSosig();
          this.transform.SetParent(l.transform);
          d.point += this.m_hit.distance * castDir;
          flag1 = true;
          l.Damage(d);
        }
      }
      else if (Physics.CapsuleCast(point1, point2, 0.13f, castDir, out this.m_hit, maxDistance, (int) this.LM_Agent, QueryTriggerInteraction.Ignore) && (Object) l.gameObject.GetComponent<FixedJoint>() == (Object) null)
      {
        RaycastHit[] raycastHitArray = Physics.RaycastAll(new Ray(hitPoint, castDir), this.m_hit.distance + this.Dimensions.z, (int) this.LM_Agent, QueryTriggerInteraction.Collide);
        for (int index = 0; index < raycastHitArray.Length; ++index)
        {
          if (!((Object) raycastHitArray[index].collider.attachedRigidbody == (Object) null))
          {
            SosigLink component = raycastHitArray[index].collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
            if (!((Object) component == (Object) l))
            {
              this.transform.position = hitPoint + castDir * Random.Range(this.LinkPenetrationRange.y * 0.8f, this.LinkPenetrationRange.y);
              this.transform.SetParent(l.transform);
              l.transform.position = l.transform.position + this.m_hit.distance * castDir;
              l.gameObject.AddComponent<FixedJoint>().connectedBody = component.R;
              l.S.KillSosig();
              component.S.KillSosig();
              d.point += raycastHitArray[index].distance * castDir;
              flag2 = true;
              l.Damage(d);
              component.Damage(d);
              break;
            }
          }
        }
      }
      if (!flag1 && !flag2)
      {
        this.transform.position = hitPoint + castDir * Random.Range(this.LinkPenetrationRange.x, this.LinkPenetrationRange.y);
        this.transform.SetParent(l.transform);
        l.Damage(d);
        if (l.S.BodyState != Sosig.SosigBodyState.Dead)
          l.S.KillSosig();
        l.R.AddForceAtPosition(castDir * 30f, hitPoint, ForceMode.Impulse);
        SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.AudEvent_Hit_Meat, this.transform.position, Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
      }
      else
        SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.AudEvent_Hit_Skewer, this.transform.position, Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
    }

    public enum HBCBoltType
    {
      Impaling,
      Explosive,
    }
  }
}
