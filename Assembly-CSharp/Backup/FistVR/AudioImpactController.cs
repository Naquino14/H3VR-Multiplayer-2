// Decompiled with JetBrains decompiler
// Type: FistVR.AudioImpactController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class AudioImpactController : MonoBehaviour
  {
    [Header("Impact Stuff")]
    public ImpactType ImpactType = ImpactType.Generic;
    public bool SoundOnRB = true;
    public bool SoundOnNonRb = true;
    private float m_tickTilCollisionSound = 0.1f;
    public float DelayLengthMult = 1f;
    public float DistLimit = 25f;
    public FVRPooledAudioType PoolToUse = FVRPooledAudioType.Impacts;
    public float HitThreshold_High = 2f;
    public float HitThreshold_Medium = 1f;
    public float HitThreshold_Ignore = 0.25f;
    private bool m_hasPlayedAudioThisFrame;
    public List<Rigidbody> IgnoreRBs = new List<Rigidbody>();
    [Header("Sonic Event Stuff")]
    public bool CausesSonicEventOnSoundPlay;
    public float BaseLoudness = 1f;
    public float LoudnessVelocityMult = 1f;
    public float MaxLoudness = 10f;
    private int m_iFFForSonicEvent = -3;
    [Header("Alternate Type Stuff")]
    public bool UsesAltTypes;
    public List<AudioImpactController.AltImpactType> Alts;

    public void SetIFF(int i) => this.m_iFFForSonicEvent = i;

    private void Update()
    {
      if ((double) this.m_tickTilCollisionSound > 0.0)
        this.m_tickTilCollisionSound -= Time.deltaTime;
      this.m_hasPlayedAudioThisFrame = false;
    }

    private void OnCollisionEnter(Collision col)
    {
      if ((double) this.m_tickTilCollisionSound > 0.0)
        return;
      this.ProcessCollision(col);
    }

    public void SetCollisionsTickDown(float f) => this.m_tickTilCollisionSound = f;

    public void SetCollisionsTickDownMax(float f) => this.m_tickTilCollisionSound = Mathf.Max(this.m_tickTilCollisionSound, f);

    private void ProcessCollision(Collision col)
    {
      if (this.m_hasPlayedAudioThisFrame)
        return;
      bool flag1 = false;
      if ((UnityEngine.Object) col.collider.attachedRigidbody != (UnityEngine.Object) null)
        flag1 = true;
      bool flag2 = false;
      if (flag1 && this.SoundOnRB)
        flag2 = true;
      if (!flag1 && this.SoundOnNonRb)
        flag2 = true;
      if (!flag2 || flag1 && this.IgnoreRBs.Contains(col.collider.attachedRigidbody))
        return;
      this.m_hasPlayedAudioThisFrame = true;
      float magnitude = col.relativeVelocity.magnitude;
      if ((double) magnitude < (double) this.HitThreshold_Ignore)
        return;
      AudioImpactIntensity impactIntensity = AudioImpactIntensity.Light;
      if ((double) magnitude > (double) this.HitThreshold_High)
        impactIntensity = AudioImpactIntensity.Hard;
      else if ((double) magnitude > (double) this.HitThreshold_Medium)
        impactIntensity = AudioImpactIntensity.Medium;
      MatSoundType matSoundType = MatSoundType.HardSurface;
      PMat component = col.collider.transform.GetComponent<PMat>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null && flag1)
        component = col.collider.attachedRigidbody.transform.GetComponent<PMat>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null && (UnityEngine.Object) component.MatDef != (UnityEngine.Object) null)
        matSoundType = component.MatDef.SoundType;
      ImpactType impacttype = this.ImpactType;
      if (this.UsesAltTypes)
      {
        Collider thisCollider = col.contacts[0].thisCollider;
        bool flag3 = false;
        for (int index1 = 0; index1 < this.Alts.Count && !flag3; ++index1)
        {
          for (int index2 = 0; index2 < this.Alts[index1].Cols.Count; ++index2)
          {
            if ((UnityEngine.Object) this.Alts[index1].Cols[index2] == (UnityEngine.Object) thisCollider)
            {
              impacttype = this.Alts[index1].Type;
              flag3 = true;
            }
            if (flag3)
              break;
          }
        }
      }
      this.m_tickTilCollisionSound = (double) magnitude <= (double) this.HitThreshold_Medium ? SM.PlayImpactSound(impacttype, matSoundType, impactIntensity, this.transform.position, this.PoolToUse, this.DistLimit, magnitude / this.HitThreshold_Medium, 1f) * this.DelayLengthMult : SM.PlayImpactSound(impacttype, matSoundType, impactIntensity, this.transform.position, this.PoolToUse, this.DistLimit) * this.DelayLengthMult;
      if (this.CausesSonicEventOnSoundPlay && (double) this.m_tickTilCollisionSound >= 0.0)
      {
        float loudness = Mathf.Clamp(this.BaseLoudness + magnitude * this.LoudnessVelocityMult, 0.0f, this.MaxLoudness) * SM.GetImpactSoundVolumeMultFromMaterial(matSoundType);
        GM.CurrentSceneSettings.OnPerceiveableSound(loudness, Mathf.Clamp(loudness, 0.0f, GM.CurrentSceneSettings.MaxImpactSoundEventDistance), this.transform.position, this.m_iFFForSonicEvent);
      }
      if ((double) this.m_tickTilCollisionSound >= 0.0)
        return;
      this.m_tickTilCollisionSound = 0.0f;
    }

    [Serializable]
    public class AltImpactType
    {
      public ImpactType Type;
      public List<Collider> Cols;
    }
  }
}
