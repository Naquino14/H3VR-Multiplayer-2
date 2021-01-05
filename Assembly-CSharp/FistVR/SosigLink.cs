// Decompiled with JetBrains decompiler
// Type: FistVR.SosigLink
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class SosigLink : MonoBehaviour, IFVRDamageable, IVaporizable
  {
    [Header("Connections")]
    public Sosig S;
    public FVRPhysicalObject O;
    public Collider C;
    public Rigidbody R;
    public CharacterJoint J;
    public SosigLink.SosigBodyPart BodyPart;
    [Header("Damage Stuff")]
    public float StaggerMagnitude = 1f;
    public float DamMult = 1f;
    public float CollisionBluntDamageMultiplier = 1f;
    private float m_fullintegrity = 100f;
    private float m_integrity = 100f;
    private bool m_isExploded;
    private bool m_isJointBroken;
    private bool m_isJointSevered;
    [Header("Audio Stuff")]
    public AudioEvent AudEvent_JointBreak;
    public AudioEvent AudEvent_JointSever;
    private List<SosigWearable> m_wearables = new List<SosigWearable>();
    private List<Collider> m_wearableColliders = new List<Collider>();
    [Header("Spawn Stuff")]
    public bool HasSpawnOnDestroy;
    public FVRObject SpawnOnDestroy;
    [Header("Meshes")]
    public Mesh[] Meshes_Whole;
    public Mesh[] Meshes_Severed_Top;
    public Mesh[] Meshes_Severed_Bottom;
    public Mesh[] Meshes_Severed_Both;
    private RaycastHit m_hit;
    private bool m_hasJustBeenSevered;
    private float timeSinceCollision = 0.1f;
    private float m_timeSeperate;

    public void SetColDamMult(float f) => this.CollisionBluntDamageMultiplier = Mathf.Min(this.CollisionBluntDamageMultiplier, f);

    public float GetIntegrityRatio() => Mathf.Clamp(this.m_integrity / this.m_fullintegrity, 0.0f, 1f);

    public void SetIntegrity(float f) => this.m_integrity = f;

    public void HealIntegrity(float percentage)
    {
      float integrity = this.m_integrity;
      this.m_integrity += this.m_fullintegrity * percentage;
      this.m_integrity = Mathf.Clamp(this.m_integrity, this.m_integrity, this.m_fullintegrity);
      if ((double) this.m_integrity <= (double) integrity)
        return;
      this.S.UpdateRendererOnLink((int) this.BodyPart);
    }

    public void RemoveIntegrity(float percentage, FistVR.Damage.DamageClass c)
    {
      float integrity = this.m_integrity;
      this.m_integrity -= this.m_fullintegrity * percentage;
      if ((double) this.m_integrity >= (double) integrity)
        return;
      if ((double) this.m_integrity <= 0.0)
        this.LinkExplodes(c);
      else
        this.S.UpdateRendererOnLink((int) this.BodyPart);
    }

    public bool IsExploded => this.m_isExploded;

    public void Vaporize(int IFF) => this.S.Vaporize(this.S.DamageFX_Vaporize, IFF);

    public void RegisterWearable(SosigWearable w)
    {
      if (!this.m_wearables.Contains(w))
        this.m_wearables.Add(w);
      for (int index = 0; index < w.Cols.Count; ++index)
        this.m_wearableColliders.Add(w.Cols[index]);
    }

    public void DeRegisterWearable(SosigWearable w)
    {
      if (this.m_wearables.Contains(w))
        this.m_wearables.Remove(w);
      for (int index = 0; index < w.Cols.Count; ++index)
        this.m_wearableColliders.Remove(w.Cols[index]);
    }

    public void DisableWearables()
    {
      for (int index = 0; index < this.m_wearables.Count; ++index)
        this.m_wearables[index].Hide();
    }

    public void EnableWearables()
    {
      for (int index = 0; index < this.m_wearables.Count; ++index)
        this.m_wearables[index].Show();
    }

    public void RegisterSpawnOnDestroy(FVRObject o)
    {
      this.SpawnOnDestroy = o;
      this.HasSpawnOnDestroy = true;
    }

    public void BreakJoint(bool isStart, FistVR.Damage.DamageClass damClass)
    {
      if (this.m_isJointBroken || !((Object) this.J != (Object) null))
        return;
      this.m_isJointBroken = true;
      if (!isStart)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_JointBreak, this.transform.position);
      this.S.BreakJoint(this, isStart, damClass);
    }

    private void SeverJoint(FistVR.Damage.DamageClass damClass, bool isPullApart)
    {
      if (this.m_isJointSevered || !((Object) this.J != (Object) null))
        return;
      this.m_isJointSevered = true;
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_JointSever, this.transform.position);
      this.S.SeverJoint(this, false, damClass, isPullApart);
    }

    public int GetDamageStateIndex()
    {
      if ((double) this.m_integrity > 70.0)
        return 0;
      return (double) this.m_integrity > 40.0 ? 1 : 2;
    }

    public void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Projectile)
      {
        float num = Mathf.Clamp((float) ((1.0 - (double) Vector3.Angle(d.strikeDir, -d.hitNormal) / 90.0) * 1.5), 0.4f, 1.5f);
        d.Dam_Blunt *= num;
        d.Dam_Cutting *= num;
        d.Dam_Piercing *= num;
        d.Dam_TotalKinetic *= num;
      }
      if (this.S.IsFrozen)
      {
        d.Dam_Blunt *= 3f;
        d.Dam_Cutting *= 0.2f;
        d.Dam_Piercing *= 0.02f;
      }
      if ((Object) this.S != (Object) null)
      {
        if (this.S.IsInvuln)
          return;
        float num = 1f;
        if (this.S.IsDamResist || this.S.IsDamMult)
          num = this.S.BuffIntensity_DamResistHarm;
        if (this.S.IsFragile)
          num *= 100f;
        d.Dam_Blunt *= num;
        d.Dam_Cutting *= num;
        d.Dam_Piercing *= num;
        d.Dam_TotalKinetic *= num;
        d.Dam_Thermal *= num;
        d.Dam_Chilling *= num;
        d.Dam_EMP *= num;
        if (d.Class == FistVR.Damage.DamageClass.Projectile)
        {
          d.Dam_Blunt *= this.S.DamMult_Projectile;
          d.Dam_Cutting *= this.S.DamMult_Projectile;
          d.Dam_Piercing *= this.S.DamMult_Projectile;
          d.Dam_TotalKinetic *= this.S.DamMult_Projectile;
        }
        else if (d.Class == FistVR.Damage.DamageClass.Melee)
        {
          d.Dam_Blunt *= this.S.DamMult_Melee;
          d.Dam_Cutting *= this.S.DamMult_Melee;
          d.Dam_Piercing *= this.S.DamMult_Melee;
          d.Dam_TotalKinetic *= this.S.DamMult_Melee;
        }
        else if (d.Class == FistVR.Damage.DamageClass.Explosive)
        {
          d.Dam_Blunt *= this.S.DamMult_Explosive;
          d.Dam_Cutting *= this.S.DamMult_Explosive;
          d.Dam_Piercing *= this.S.DamMult_Explosive;
          d.Dam_TotalKinetic *= this.S.DamMult_Explosive;
        }
      }
      if (d.Class == FistVR.Damage.DamageClass.Melee && this.m_wearableColliders.Count > 0)
      {
        SosigWearable outwear = (SosigWearable) null;
        if (this.HitsWearable(d.point + d.hitNormal, -d.hitNormal, 1.5f, out outwear))
        {
          d.Dam_Blunt *= outwear.MeleeDamMult_Blunt;
          d.Dam_Cutting *= outwear.MeleeDamMult_Cutting;
          d.Dam_Piercing *= outwear.MeleeDamMult_Piercing;
          d.Dam_TotalKinetic = d.Dam_Blunt + d.Dam_Cutting + d.Dam_Piercing;
        }
      }
      d.Dam_Thermal *= this.S.DamMult_Thermal;
      d.Dam_Chilling *= this.S.DamMult_Chilling;
      d.Dam_EMP *= this.S.DamMult_EMP;
      this.S.ProcessDamage(d, this);
      bool flag = false;
      if ((double) this.m_integrity > 0.0)
        flag = this.DamageIntegrity(d);
      if ((double) d.Dam_TotalKinetic > 80.0)
      {
        if (d.Class == FistVR.Damage.DamageClass.Projectile)
          this.S.RequestHitDecal(d.point, d.hitNormal, d.damageSize * 2f, this);
        else if (d.Class == FistVR.Damage.DamageClass.Melee && !flag)
        {
          if ((double) d.Dam_Blunt > (double) d.Dam_Cutting && (double) d.Dam_Blunt > (double) d.Dam_Piercing && (double) d.Dam_Blunt > 100.0)
            this.S.RequestHitDecal(d.point, d.hitNormal, d.damageSize * 2f, this);
          else if ((double) d.Dam_Cutting > (double) d.Dam_Piercing && (double) d.Dam_Cutting > 100.0 && (double) d.edgeNormal.magnitude > 0.100000001490116)
            this.S.RequestHitDecal(d.point, d.hitNormal, d.edgeNormal, d.damageSize * 2f, this);
          else if ((double) d.Dam_Piercing > 100.0)
            this.S.RequestHitDecal(d.point, d.hitNormal, d.damageSize * 2f, this);
        }
      }
      if ((double) this.S.Mustard <= 0.0 || flag)
        return;
      float bloodAmount = Mathf.Clamp((float) (((double) d.Dam_Piercing + (double) d.Dam_Cutting - 50.0) * 0.0500000007450581) * this.DamMult, 0.0f, 100f);
      this.S.SetLastIFFDamageSource(d.Source_IFF);
      this.S.AccurueBleedingHit(this, d.point, d.strikeDir, bloodAmount);
    }

    public bool HitsWearable(
      Vector3 startPoint,
      Vector3 direction,
      float distance,
      out SosigWearable outwear)
    {
      bool flag = false;
      outwear = (SosigWearable) null;
      if (this.C.Raycast(new Ray(startPoint, direction), out this.m_hit, distance))
      {
        float distance1 = this.m_hit.distance;
        flag = false;
        SosigWearable sosigWearable = (SosigWearable) null;
        for (int index1 = 0; index1 < this.m_wearables.Count; ++index1)
        {
          SosigWearable wearable = this.m_wearables[index1];
          if (wearable.Cols.Count > 0)
          {
            for (int index2 = 0; index2 < wearable.Cols.Count; ++index2)
            {
              if (wearable.Cols[index2].Raycast(new Ray(startPoint, direction), out this.m_hit, distance) && (double) this.m_hit.distance < (double) distance1)
              {
                flag = true;
                sosigWearable = wearable;
                outwear = wearable;
                break;
              }
            }
            if (flag)
              break;
          }
        }
      }
      return flag;
    }

    public bool GetHasJustBeenSevered() => this.m_hasJustBeenSevered;

    public bool DamageIntegrity(FistVR.Damage d) => this.DamageIntegrity(d.Dam_Blunt, d.Dam_Piercing, d.Dam_Cutting, d.Dam_Thermal, d.strikeDir, d.point, d.Class, d.Source_IFF);

    public bool DamageIntegrity(
      float b,
      float p,
      float c,
      float t,
      Vector3 dir,
      Vector3 pos,
      FistVR.Damage.DamageClass damClass,
      int iff)
    {
      if (this.S.IsInvuln)
        return false;
      this.S.SetLastIFFDamageSource(iff);
      if (this.S.AppliesDamageResistToIntegrityLoss)
      {
        p *= this.S.DamMult_Piercing;
        c *= this.S.DamMult_Cutting;
        b *= this.S.DamMult_Blunt;
        t *= this.S.DamMult_Thermal;
      }
      float num = Mathf.Clamp((b * 0.032f + Mathf.Clamp((float) (((double) p - 500.0) * 0.00800000037997961), 0.0f, 100f) + Mathf.Clamp((float) (((double) c - 500.0) * 0.00800000037997961), 0.0f, 100f) + Mathf.Clamp(t * 0.01f, 0.0f, 100f)) * this.DamMult, 0.0f, 100f);
      if (this.S.CanBeSevered && (double) c > 350.0 && ((double) c > (double) b && (Object) this.J != (Object) null) && ((double) Vector3.Distance(pos, this.transform.position) > 0.119999997317791 && (double) Vector3.Angle(dir, this.transform.up) > 65.0 && (double) Vector3.Angle(dir, this.transform.up) < 115.0))
      {
        this.m_hasJustBeenSevered = true;
        this.SeverJoint(damClass, false);
        return true;
      }
      this.m_integrity -= num;
      if ((double) this.m_integrity <= 0.0)
      {
        this.LinkExplodes(damClass);
        return true;
      }
      this.S.UpdateRendererOnLink((int) this.BodyPart);
      return false;
    }

    public void LinkExplodes(FistVR.Damage.DamageClass damClass)
    {
      if (this.m_isExploded)
        return;
      this.m_isExploded = true;
      this.S.DestroyLink(this, damClass);
    }

    public void OnCollisionEnter(Collision col) => this.ProcessCollision(col);

    private void ProcessCollision(Collision col)
    {
      if ((double) this.timeSinceCollision > 0.0)
        return;
      this.S.ProcessCollision(this, col);
      this.timeSinceCollision = 0.1f;
    }

    public void Update()
    {
      if ((double) this.timeSinceCollision >= 0.0)
        this.timeSinceCollision -= Time.deltaTime;
      bool flag = this.S.CanCurrentlyBeHeld();
      if (this.O.IsHeld)
      {
        if (!flag)
          this.O.ForceBreakInteraction();
        if ((Object) this.S.CoreRB != (Object) null && (double) this.S.DistanceFromCoreTarget() > 1.0)
          this.S.Stagger(1f);
      }
      if (!((Object) this.J != (Object) null) || !((Object) this.J.connectedBody != (Object) null))
        return;
      SosigLink component = this.J.connectedBody.gameObject.GetComponent<SosigLink>();
      if (!((Object) component != (Object) null))
        return;
      if (this.O.IsHeld && component.O.IsHeld && ((double) Vector3.Angle(this.transform.up, component.transform.up) > 135.0 || (double) Vector3.Angle(this.transform.forward, component.transform.forward) > 75.0))
      {
        this.S.SetLastIFFDamageSource(GM.CurrentPlayerBody.GetPlayerIFF());
        this.BreakJoint(false, FistVR.Damage.DamageClass.Melee);
      }
      if ((double) Vector3.Distance(this.transform.position, component.transform.position) > 0.5)
      {
        this.m_timeSeperate += Time.deltaTime;
        if ((double) this.m_timeSeperate <= 0.100000001490116)
          return;
        if (this.O.IsHeld || component.O.IsHeld)
          this.S.SetLastIFFDamageSource(GM.CurrentPlayerBody.GetPlayerIFF());
        this.SeverJoint(FistVR.Damage.DamageClass.Melee, true);
      }
      else
        this.m_timeSeperate = 0.0f;
    }

    public static bool ClosestPointsOnTwoLines(
      out Vector3 closestPointLine1,
      out Vector3 closestPointLine2,
      Vector3 linePoint1,
      Vector3 lineVec1,
      Vector3 linePoint2,
      Vector3 lineVec2)
    {
      closestPointLine1 = Vector3.zero;
      closestPointLine2 = Vector3.zero;
      float num1 = Vector3.Dot(lineVec1, lineVec1);
      float num2 = Vector3.Dot(lineVec1, lineVec2);
      float num3 = Vector3.Dot(lineVec2, lineVec2);
      float num4 = (float) ((double) num1 * (double) num3 - (double) num2 * (double) num2);
      if ((double) num4 == 0.0)
        return false;
      Vector3 rhs = linePoint1 - linePoint2;
      float num5 = Vector3.Dot(lineVec1, rhs);
      float num6 = Vector3.Dot(lineVec2, rhs);
      float num7 = (float) ((double) num2 * (double) num6 - (double) num5 * (double) num3) / num4;
      float num8 = (float) ((double) num1 * (double) num6 - (double) num5 * (double) num2) / num4;
      closestPointLine1 = linePoint1 + lineVec1 * num7;
      closestPointLine2 = linePoint2 + lineVec2 * num8;
      return true;
    }

    public enum SosigBodyPart
    {
      Head,
      Torso,
      UpperLink,
      LowerLink,
    }
  }
}
