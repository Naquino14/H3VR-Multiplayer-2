// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_EncryptionTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TNH_EncryptionTarget : MonoBehaviour, IFVRDamageable
  {
    [Header("Core")]
    public TNH_EncryptionType Type;
    private TNH_HoldPoint m_point;
    public int NumHitsTilDestroyed = 1;
    private int m_maxHits = 1;
    private int m_numHitsLeft = 1;
    public bool UsesDamagePerHit;
    public float DamagePerHit = 500f;
    private float m_damLeftForAHit = 500f;
    public bool StartsRandomRot;
    public float RandomRotMag = 1f;
    [Header("SubParts")]
    public List<GameObject> SpawnOnDestruction;
    public List<Transform> SpawnPoints;
    public bool AreSomeSubpartsTargets;
    public bool UseSubpartForce;
    public Vector2 SubpartVelRange;
    public TNH_EncryptionTarget.ForceDir SubpartDir;
    [Header("Display")]
    public bool UsesMultipleDisplay;
    public List<GameObject> DisplayList;
    [Header("Flash")]
    public bool FlashOnDestroy;
    public Color FlashColor;
    public float FlashIntensity;
    public float FlashRange;
    [Header("Required SubTargets")]
    public bool UsesSubTargs;
    public List<GameObject> SubTargs;
    private int m_numSubTargsLeft;
    [Header("Recursive SubTargets")]
    public bool UsesRecursiveSubTarg;
    public int StartingSubTargs = 3;
    [Header("AgileMovement")]
    public bool UsesAgileMovement;
    public Rigidbody RB;
    public LayerMask LM_AgileMove;
    public Transform AgilePointer;
    public float CastRadius = 0.4f;
    public float DistFromSpawnMax = 10f;
    private Vector3 agileStartPos;
    private float m_nextDist;
    private List<Vector3> m_validAgilePos;
    private RaycastHit m_hit;
    private Quaternion m_fromRot;
    private float m_timeTilWarp;
    private float m_warpSpeed = 4f;
    private float m_warpSpeedFactor = 1f;
    private Vector3 nextWarpPos;
    private Quaternion nextWarpRot;
    private bool m_hasNextWarp;
    public AudioEvent AudEvent_WarpFrom;
    public AudioEvent AudEvent_WarpTo;
    [Header("RegenerativeSubTargets")]
    public bool UsesRegenerativeSubTarg;
    public int StartingRegenSubTarg = 5;
    public float MaxGrowthDistance = 16f;
    private float m_regenRotPieceRot;
    public LayerMask LM_RegenPlacement;
    public List<GameObject> Tendrils;
    public List<float> TendrilFloats;
    public List<Vector3> GrowthPoints;
    public LayerMask LM_Regen;
    public Transform Core2;
    private float m_regenGrowthSpeed = 1f;
    [Header("Sound")]
    public bool SoundOnDamage;
    public AudioEvent AudEvent_SoundOnDamage;
    private bool m_isDestroyed;
    private float damRefireLimited;

    public void SetHoldPoint(TNH_HoldPoint p) => this.m_point = p;

    public void Start()
    {
      this.m_numHitsLeft = this.NumHitsTilDestroyed;
      this.m_maxHits = this.NumHitsTilDestroyed;
      this.m_damLeftForAHit = this.DamagePerHit;
      this.agileStartPos = this.transform.position;
      this.m_fromRot = this.transform.rotation;
      this.m_timeTilWarp = 0.0f;
      this.m_warpSpeed = Random.Range(4f, 5f);
      if (this.UsesAgileMovement)
        this.m_validAgilePos = new List<Vector3>();
      if (this.UsesRegenerativeSubTarg)
      {
        for (int index = 0; index < this.Tendrils.Count; ++index)
        {
          this.Tendrils[index].transform.SetParent((Transform) null);
          this.SubTargs[index].transform.SetParent((Transform) null);
        }
        this.PopulateInitialRegen();
      }
      if (this.UsesRecursiveSubTarg)
      {
        for (int index1 = 0; index1 < this.StartingSubTargs; ++index1)
        {
          int index2 = Random.Range(0, this.SubTargs.Count);
          if (!this.SubTargs[index2].activeSelf)
          {
            this.SubTargs[index2].SetActive(true);
            ++this.m_numSubTargsLeft;
          }
        }
      }
      if (this.UsesSubTargs && !this.UsesRecursiveSubTarg && !this.UsesRegenerativeSubTarg)
        this.m_numSubTargsLeft = this.SubTargs.Count;
      if (!this.StartsRandomRot)
        return;
      this.transform.rotation = Random.rotation;
      this.GetComponent<Rigidbody>().angularVelocity = Random.onUnitSphere * this.RandomRotMag;
    }

    private void PopulateInitialRegen()
    {
      for (int index = 0; index < this.StartingRegenSubTarg; ++index)
      {
        Vector3 position = this.transform.position;
        Vector3 onUnitSphere = Random.onUnitSphere;
        float maxDistance = this.MaxGrowthDistance;
        if (Physics.Raycast(position, onUnitSphere, out this.m_hit, maxDistance, (int) this.LM_Regen))
          maxDistance = this.m_hit.distance;
        this.SpawnGrowth(index, position + onUnitSphere * maxDistance);
      }
      this.m_numSubTargsLeft = this.StartingRegenSubTarg;
    }

    private void ResetGrowth(int index, Vector3 point)
    {
      this.GrowthPoints[index] = point;
      this.TendrilFloats[index] = 0.0f;
      Vector3 forward = point - this.Tendrils[index].transform.position;
      this.Tendrils[index].transform.rotation = Quaternion.LookRotation(forward);
      this.Tendrils[index].transform.localScale = new Vector3(0.2f, 0.2f, forward.magnitude);
    }

    private void SpawnGrowth(int index, Vector3 point)
    {
      if (this.SubTargs[index].activeSelf)
        return;
      this.Tendrils[index].SetActive(true);
      this.GrowthPoints[index] = point;
      this.SubTargs[index].transform.position = point;
      this.SubTargs[index].SetActive(true);
      this.TendrilFloats[index] = 1f;
      Vector3 forward = point - this.Tendrils[index].transform.position;
      this.Tendrils[index].transform.rotation = Quaternion.LookRotation(forward);
      this.Tendrils[index].transform.localScale = new Vector3(0.2f, 0.2f, forward.magnitude);
      this.SubTargs[index].transform.rotation = Random.rotation;
      ++this.m_numSubTargsLeft;
    }

    public void DisableSubtarg(int i)
    {
      if (!this.SubTargs[i].activeSelf)
        return;
      if (this.UsesRegenerativeSubTarg)
      {
        Vector3 position = this.transform.position;
        Vector3 onUnitSphere = Random.onUnitSphere;
        float maxDistance = this.MaxGrowthDistance;
        if (Physics.Raycast(position, onUnitSphere, out this.m_hit, maxDistance, (int) this.LM_Regen))
          maxDistance = this.m_hit.distance;
        this.ResetGrowth(i, position + onUnitSphere * maxDistance);
      }
      --this.m_numSubTargsLeft;
      if (this.m_numSubTargsLeft <= 0)
      {
        this.Destroy();
      }
      else
      {
        if (this.UsesRegenerativeSubTarg)
          this.Tendrils[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        this.SubTargs[i].SetActive(false);
        if (!this.SoundOnDamage)
          return;
        SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_SoundOnDamage, this.transform.position);
      }
    }

    private void RespawnRandomSubTarg()
    {
      int index = Random.Range(0, this.SubTargs.Count);
      if (this.SubTargs[index].activeSelf)
        return;
      this.SubTargs[index].SetActive(true);
      ++this.m_numSubTargsLeft;
    }

    public void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Explosive)
        return;
      this.damRefireLimited = 0.05f;
      if (this.UsesRecursiveSubTarg)
        this.RespawnRandomSubTarg();
      if (this.UsesRegenerativeSubTarg)
        this.m_regenGrowthSpeed = 0.0f;
      if (this.UsesSubTargs)
        return;
      if (!this.UsesDamagePerHit)
      {
        --this.m_numHitsLeft;
      }
      else
      {
        float damTotalKinetic = d.Dam_TotalKinetic;
        if ((double) damTotalKinetic <= (double) this.m_damLeftForAHit)
        {
          this.m_damLeftForAHit -= damTotalKinetic;
          if ((double) this.m_damLeftForAHit <= 0.0)
          {
            this.m_damLeftForAHit = this.DamagePerHit;
            --this.m_numHitsLeft;
          }
        }
        else
        {
          --this.m_numHitsLeft;
          float num1 = damTotalKinetic - this.m_damLeftForAHit;
          int num2 = Mathf.FloorToInt(num1 / this.DamagePerHit);
          if (num2 > 1)
            --num2;
          this.m_numHitsLeft -= num2;
          this.m_damLeftForAHit = num1 % this.DamagePerHit;
        }
      }
      if (this.m_numHitsLeft <= 0)
      {
        this.Destroy();
      }
      else
      {
        if (this.UsesMultipleDisplay)
          this.UpdateDisplay();
        if ((double) this.damRefireLimited > 0.0 || !this.SoundOnDamage)
          return;
        SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_SoundOnDamage, this.transform.position);
      }
    }

    private void UpdateDisplay()
    {
      int numHitsLeft = this.m_numHitsLeft;
      for (int index = 0; index < this.DisplayList.Count; ++index)
      {
        if (index == numHitsLeft)
          this.DisplayList[index].SetActive(true);
        else
          this.DisplayList[index].SetActive(false);
      }
    }

    public void Update()
    {
      if ((double) this.damRefireLimited > 0.0)
        this.damRefireLimited -= Time.deltaTime;
      if (this.UsesAgileMovement && (double) this.m_validAgilePos.Count < 20.0)
      {
        Vector3 agileStartPos = this.agileStartPos;
        Vector3 onUnitSphere = Random.onUnitSphere;
        onUnitSphere.y *= 0.2f;
        onUnitSphere.Normalize();
        Vector3 vector3 = agileStartPos + onUnitSphere * Random.Range(this.DistFromSpawnMax * 0.2f, this.DistFromSpawnMax * 0.8f);
        if (Physics.SphereCast(agileStartPos, this.CastRadius, onUnitSphere, out this.m_hit, this.DistFromSpawnMax, (int) this.LM_AgileMove))
          vector3 = agileStartPos + onUnitSphere * Random.Range(this.m_hit.distance * 0.5f, this.m_hit.distance * 0.8f);
        this.m_validAgilePos.Add(vector3);
      }
      if (!this.UsesRegenerativeSubTarg)
        return;
      this.m_regenRotPieceRot = Mathf.Repeat(this.m_regenRotPieceRot + (float) ((double) this.m_regenGrowthSpeed * (double) this.m_regenGrowthSpeed * (double) this.m_regenGrowthSpeed * 360.0) * Time.deltaTime, 360f);
      this.Core2.localEulerAngles = new Vector3(this.m_regenRotPieceRot, this.m_regenRotPieceRot, this.m_regenRotPieceRot);
      this.m_regenGrowthSpeed = Mathf.MoveTowards(this.m_regenGrowthSpeed, 1f, Time.deltaTime * 0.2f);
      for (int index1 = 0; index1 < this.SubTargs.Count; ++index1)
      {
        if (!this.SubTargs[index1].activeSelf)
        {
          List<float> tendrilFloats;
          int index2;
          (tendrilFloats = this.TendrilFloats)[index2 = index1] = tendrilFloats[index2] + Time.deltaTime * (this.m_regenGrowthSpeed * this.m_regenGrowthSpeed);
          if ((double) this.TendrilFloats[index1] >= 1.0)
          {
            this.SpawnGrowth(index1, this.GrowthPoints[index1]);
          }
          else
          {
            Vector3 vector3 = this.GrowthPoints[index1] - this.transform.position;
            this.Tendrils[index1].transform.localScale = new Vector3(0.2f, 0.2f, vector3.magnitude * this.TendrilFloats[index1]);
          }
        }
      }
    }

    private void FixedUpdate()
    {
      if (!this.UsesAgileMovement || this.m_validAgilePos.Count <= 0)
        return;
      if (!this.m_hasNextWarp)
      {
        this.m_fromRot = this.transform.rotation;
        int index = Random.Range(0, this.m_validAgilePos.Count);
        this.nextWarpPos = this.m_validAgilePos[index];
        this.m_validAgilePos.RemoveAt(index);
        this.nextWarpRot = Quaternion.LookRotation(this.nextWarpPos - this.transform.position);
        this.m_hasNextWarp = true;
        this.m_timeTilWarp = 0.0f;
        this.m_nextDist = Vector3.Distance(this.transform.position, this.nextWarpPos);
      }
      else
      {
        this.m_timeTilWarp += Time.deltaTime * this.m_warpSpeed;
        if ((double) this.m_timeTilWarp > 1.0)
        {
          this.m_warpSpeed = Random.Range(2f, 4f) * this.m_warpSpeedFactor;
          this.m_warpSpeedFactor *= 0.92f;
          this.m_hasNextWarp = false;
          SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_WarpFrom, this.transform.position);
          this.RB.position = this.nextWarpPos;
          SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, this.AudEvent_WarpTo, this.nextWarpPos, 0.2f);
          this.AgilePointer.localScale = new Vector3(0.24f, 0.24f, 0.24f);
        }
        else
        {
          this.RB.rotation = Quaternion.Slerp(this.m_fromRot, this.nextWarpRot, this.m_timeTilWarp * 2f);
          if ((double) this.m_timeTilWarp <= 0.5)
            return;
          this.AgilePointer.localScale = new Vector3(0.1f, 0.1f, (float) (((double) this.m_timeTilWarp - 0.5) * 2.0 * (double) this.m_nextDist * 1.35137212276459));
        }
      }
    }

    private void Destroy()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      if (this.UsesRegenerativeSubTarg)
      {
        for (int index = 0; index < this.Tendrils.Count; ++index)
        {
          Object.Destroy((Object) this.Tendrils[index]);
          Object.Destroy((Object) this.SubTargs[index]);
        }
      }
      if (this.FlashOnDestroy)
        FXM.InitiateMuzzleFlash(this.transform.position, Vector3.up, this.FlashIntensity, this.FlashColor, this.FlashRange);
      for (int index = 0; index < this.SpawnOnDestruction.Count; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.SpawnOnDestruction[index], this.SpawnPoints[index].position, this.SpawnPoints[index].rotation);
        if (this.AreSomeSubpartsTargets)
        {
          TNH_EncryptionTarget component = gameObject.GetComponent<TNH_EncryptionTarget>();
          if ((Object) component != (Object) null)
            component.SetHoldPoint(this.m_point);
          this.m_point.RegisterNewTarget(this);
        }
        if (this.UseSubpartForce)
        {
          Rigidbody component = gameObject.GetComponent<Rigidbody>();
          if ((Object) component != (Object) null)
          {
            switch (this.SubpartDir)
            {
              case TNH_EncryptionTarget.ForceDir.Outward:
                Vector3 vector3 = (gameObject.transform.position - this.transform.position).normalized * Random.Range(this.SubpartVelRange.x, this.SubpartVelRange.y);
                component.velocity = vector3;
                component.angularVelocity = Random.onUnitSphere;
                continue;
              case TNH_EncryptionTarget.ForceDir.Random:
                component.velocity = Random.onUnitSphere * Random.Range(this.SubpartVelRange.x, this.SubpartVelRange.y);
                component.angularVelocity = Random.onUnitSphere;
                continue;
              case TNH_EncryptionTarget.ForceDir.Forward:
                component.velocity = this.SpawnPoints[index].forward * Random.Range(this.SubpartVelRange.x, this.SubpartVelRange.y);
                continue;
              default:
                continue;
            }
          }
        }
      }
      if ((Object) this.m_point != (Object) null)
        this.m_point.TargetDestroyed(this);
      Object.Destroy((Object) this.gameObject);
    }

    public enum ForceDir
    {
      Outward,
      Random,
      Forward,
    }
  }
}
