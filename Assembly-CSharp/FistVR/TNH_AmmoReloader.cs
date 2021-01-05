// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_AmmoReloader
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TNH_AmmoReloader : MonoBehaviour
  {
    public TNH_Manager M;
    public Transform Spawnpoint_Round;
    public Transform ScanningVolume;
    public LayerMask ScanningLM;
    public AudioEvent AudEvent_Fail;
    public AudioEvent AudEvent_Spawn;
    public AudioEvent AudEvent_Reload;
    private List<FVRFireArmMagazine> m_detectedMags = new List<FVRFireArmMagazine>();
    private List<FVRFireArmClip> m_detectedClips = new List<FVRFireArmClip>();
    private List<Speedloader> m_detectedSLs = new List<Speedloader>();
    private List<FireArmRoundType> m_roundTypes = new List<FireArmRoundType>();
    private Collider[] colbuffer;
    private Dictionary<FireArmRoundType, FireArmRoundClass> m_decidedTypes = new Dictionary<FireArmRoundType, FireArmRoundClass>();
    private List<FVRObject.OTagEra> m_validEras = new List<FVRObject.OTagEra>();
    private List<FVRObject.OTagSet> m_validSets = new List<FVRObject.OTagSet>();
    private float m_scanTick = 1f;

    private void Start() => this.colbuffer = new Collider[50];

    public void SetValidErasSets(List<FVRObject.OTagEra> eras, List<FVRObject.OTagSet> sets)
    {
      for (int index = 0; index < eras.Count; ++index)
        this.m_validEras.Add(eras[index]);
      for (int index = 0; index < sets.Count; ++index)
        this.m_validSets.Add(sets[index]);
    }

    private FireArmRoundClass GetClassFromType(FireArmRoundType t)
    {
      if (!this.m_decidedTypes.ContainsKey(t))
      {
        List<FireArmRoundClass> fireArmRoundClassList = new List<FireArmRoundClass>();
        for (int index = 0; index < AM.SRoundDisplayDataDic[t].Classes.Length; ++index)
        {
          FVRObject objectId = AM.SRoundDisplayDataDic[t].Classes[index].ObjectID;
          if (this.m_validEras.Contains(objectId.TagEra) && this.m_validSets.Contains(objectId.TagSet))
            fireArmRoundClassList.Add(AM.SRoundDisplayDataDic[t].Classes[index].Class);
        }
        if (fireArmRoundClassList.Count > 0)
          this.m_decidedTypes.Add(t, fireArmRoundClassList[Random.Range(0, fireArmRoundClassList.Count)]);
        else
          this.m_decidedTypes.Add(t, AM.GetRandomValidRoundClass(t));
      }
      return this.m_decidedTypes[t];
    }

    public void Button_SpawnRound()
    {
      if (this.m_roundTypes.Count < 1)
      {
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Fail, this.transform.position);
      }
      else
      {
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Spawn, this.transform.position);
        for (int index = 0; index < this.m_roundTypes.Count; ++index)
        {
          FireArmRoundType roundType = this.m_roundTypes[index];
          FireArmRoundClass classFromType = this.GetClassFromType(roundType);
          Object.Instantiate<GameObject>(AM.GetRoundSelfPrefab(roundType, classFromType).GetGameObject(), this.Spawnpoint_Round.position + Vector3.up * (float) index * 0.1f, this.Spawnpoint_Round.rotation);
        }
      }
    }

    public void Button_ReloadGuns()
    {
      if (this.m_detectedMags.Count < 1 && this.m_detectedClips.Count < 1 && this.m_detectedSLs.Count < 1)
      {
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Fail, this.transform.position);
      }
      else
      {
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Reload, this.transform.position);
        for (int index = 0; index < this.m_detectedMags.Count; ++index)
        {
          FireArmRoundClass classFromType = this.GetClassFromType(this.m_detectedMags[index].RoundType);
          this.m_detectedMags[index].ReloadMagWithType(classFromType);
        }
        for (int index = 0; index < this.m_detectedClips.Count; ++index)
        {
          FireArmRoundClass classFromType = this.GetClassFromType(this.m_detectedClips[index].RoundType);
          this.m_detectedClips[index].ReloadClipWithType(classFromType);
        }
        for (int index = 0; index < this.m_detectedSLs.Count; ++index)
        {
          FireArmRoundClass classFromType = this.GetClassFromType(this.m_detectedSLs[index].Chambers[0].Type);
          this.m_detectedSLs[index].ReloadClipWithType(classFromType);
        }
      }
    }

    private void Update()
    {
      this.m_scanTick -= Time.deltaTime;
      if ((double) this.m_scanTick > 0.0)
        return;
      this.m_scanTick = Random.Range(0.8f, 1f);
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) >= 12.0)
        return;
      this.Scan();
    }

    private void Scan()
    {
      int num = Physics.OverlapBoxNonAlloc(this.ScanningVolume.position, this.ScanningVolume.localScale * 0.5f, this.colbuffer, this.ScanningVolume.rotation, (int) this.ScanningLM, QueryTriggerInteraction.Collide);
      this.m_roundTypes.Clear();
      this.m_detectedMags.Clear();
      this.m_detectedClips.Clear();
      this.m_detectedSLs.Clear();
      for (int index1 = 0; index1 < num; ++index1)
      {
        if ((Object) this.colbuffer[index1].attachedRigidbody != (Object) null)
        {
          FVRFireArm component1 = this.colbuffer[index1].attachedRigidbody.gameObject.GetComponent<FVRFireArm>();
          if ((Object) component1 != (Object) null)
          {
            if (!this.m_roundTypes.Contains(component1.RoundType))
              this.m_roundTypes.Add(component1.RoundType);
            if ((Object) component1.Magazine != (Object) null && !this.m_detectedMags.Contains(component1.Magazine))
              this.m_detectedMags.Add(component1.Magazine);
            if (component1.Attachments.Count > 0)
            {
              for (int index2 = 0; index2 < component1.Attachments.Count; ++index2)
              {
                if (component1.Attachments[index2] is AttachableFirearmPhysicalObject && !this.m_roundTypes.Contains((component1.Attachments[index2] as AttachableFirearmPhysicalObject).FA.RoundType))
                  this.m_roundTypes.Add((component1.Attachments[index2] as AttachableFirearmPhysicalObject).FA.RoundType);
              }
            }
            if ((Object) component1.GetIntegratedAttachableFirearm() != (Object) null && !this.m_roundTypes.Contains(component1.GetIntegratedAttachableFirearm().RoundType))
              this.m_roundTypes.Add(component1.GetIntegratedAttachableFirearm().RoundType);
          }
          AttachableFirearmPhysicalObject component2 = this.colbuffer[index1].attachedRigidbody.gameObject.GetComponent<AttachableFirearmPhysicalObject>();
          if ((Object) component2 != (Object) null && !this.m_roundTypes.Contains(component2.FA.RoundType))
            this.m_roundTypes.Add(component2.FA.RoundType);
          FVRFireArmMagazine component3 = this.colbuffer[index1].attachedRigidbody.gameObject.GetComponent<FVRFireArmMagazine>();
          if ((Object) component3 != (Object) null && (Object) component3.FireArm == (Object) null && !this.m_detectedMags.Contains(component3))
            this.m_detectedMags.Add(component3);
          FVRFireArmClip component4 = this.colbuffer[index1].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>();
          if ((Object) component4 != (Object) null && (Object) component4.FireArm == (Object) null && !this.m_detectedClips.Contains(component4))
            this.m_detectedClips.Add(component4);
          Speedloader component5 = this.colbuffer[index1].attachedRigidbody.gameObject.GetComponent<Speedloader>();
          if ((Object) component5 != (Object) null && !this.m_detectedSLs.Contains(component5))
            this.m_detectedSLs.Add(component5);
        }
      }
    }
  }
}
