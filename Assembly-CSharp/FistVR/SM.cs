// Decompiled with JetBrains decompiler
// Type: FistVR.SM
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FistVR
{
  public class SM : ManagerSingleton<SM>
  {
    public FVRPooledAudioPrefabDirectory PrefabDirectory;
    public FVRSoundTailsDirectory[] TailsDirectories;
    public FVRReverbSettingProfile[] ReverbSettingProfiles;
    public AudioImpactSet[] AudioImpactSets;
    public AudioBulletImpactSet[] AudioBulletImpactSets;
    public LayerMask OcclusionLM;
    public AnimationCurve OcclusionFactorCurve;
    private FVRReverbSystem reverbSystem;
    private Dictionary<int, PoolTypePrefabBinding> m_prefabBindingDic = new Dictionary<int, PoolTypePrefabBinding>();
    private Dictionary<FVRTailSoundClass, Dictionary<FVRSoundEnvironment, AudioEvent>> m_tailsDic = new Dictionary<FVRTailSoundClass, Dictionary<FVRSoundEnvironment, AudioEvent>>();
    private Dictionary<FVRSoundEnvironment, FVRReverbSettingProfile> m_reverbDic = new Dictionary<FVRSoundEnvironment, FVRReverbSettingProfile>();
    private Dictionary<ImpactType, Dictionary<MatSoundType, Dictionary<AudioImpactIntensity, AudioEvent>>> m_impactDic = new Dictionary<ImpactType, Dictionary<MatSoundType, Dictionary<AudioImpactIntensity, AudioEvent>>>();
    private Dictionary<BulletImpactSoundType, AudioBulletImpactSet> m_bulletHitDic = new Dictionary<BulletImpactSoundType, AudioBulletImpactSet>();
    private Dictionary<HandlingGrabType, HandlingGrabSet> m_handlingGrabDic = new Dictionary<HandlingGrabType, HandlingGrabSet>();
    private Dictionary<HandlingReleaseType, HandlingReleaseSet> m_handlingReleaseDic = new Dictionary<HandlingReleaseType, HandlingReleaseSet>();
    private Dictionary<HandlingReleaseIntoSlotType, HandlingReleaseIntoSlotSet> m_handlingReleaseIntoSlotDic = new Dictionary<HandlingReleaseIntoSlotType, HandlingReleaseIntoSlotSet>();
    private List<SM.AudioSourcePool> m_activePools = new List<SM.AudioSourcePool>();
    private SM.AudioSourcePool m_pool_generic;
    private SM.AudioSourcePool m_pool_generic_close;
    private SM.AudioSourcePool m_pool_generic_long;
    private SM.AudioSourcePool m_pool_generic_verylong;
    private SM.AudioSourcePool m_pool_explosion;
    private SM.AudioSourcePool m_pool_explosionTail;
    private SM.AudioSourcePool m_pool_UIChirp;
    private SM.AudioSourcePool m_pool_NPCShotNear;
    private SM.AudioSourcePool m_pool_NPCShotFarDistant;
    private SM.AudioSourcePool m_pool_NPCBarks;
    private SM.AudioSourcePool m_pool_NPCHandling;
    private SM.AudioSourcePool m_pool_casings;
    private SM.AudioSourcePool m_pool_impacts;
    public AudioMixer MasterMixer;
    private SM.ReverbSettings m_reverbSettings = new SM.ReverbSettings();
    private int m_numImpactSoundsThisFrame;

    public static FVRReverbSystem ReverbSystem
    {
      get => ManagerSingleton<SM>.Instance.reverbSystem;
      set => ManagerSingleton<SM>.Instance.reverbSystem = value;
    }

    public static List<SM.AudioSourcePool> ActivePools => ManagerSingleton<SM>.Instance.m_activePools;

    protected override void Awake()
    {
      base.Awake();
      this.generatePoolTypePrefabBindingDic();
      this.generateTailsDictionary();
      this.generateReverbDictionary();
      this.generateImpactDictionary();
      this.generateHandlingDictionaries();
      ManagerSingleton<SM>.Instance.m_reverbSettings.MasterMixer = ManagerSingleton<SM>.Instance.MasterMixer;
    }

    public static void SetReverbEnvironment(FVRSoundEnvironment e) => ManagerSingleton<SM>.Instance.m_reverbSettings.Set(SM.GetReverbSettingProfile(e));

    public static bool DoesReverbSystemExist() => !((UnityEngine.Object) SM.ReverbSystem == (UnityEngine.Object) null);

    public static FVRSoundEnvironment GetSoundEnvironment(Vector3 pos) => (UnityEngine.Object) SM.ReverbSystem == (UnityEngine.Object) null ? GM.CurrentSceneSettings.DefaultSoundEnvironment : SM.ReverbSystem.GetSoundEnvironment(pos).Environment;

    public static FVRReverbEnvironment GetReverbEnvironment(Vector3 pos) => (UnityEngine.Object) SM.ReverbSystem == (UnityEngine.Object) null ? (FVRReverbEnvironment) null : SM.ReverbSystem.GetSoundEnvironment(pos);

    public static FVRReverbEnvironment GetPlayerReverbEnvironment() => SM.DoesReverbSystemExist() ? SM.ReverbSystem.CurrentReverbEnvironment : (FVRReverbEnvironment) null;

    public static void TransitionToReverbEnvironment(FVRSoundEnvironment e, float s) => ManagerSingleton<SM>.Instance.m_reverbSettings.TransitionTo(SM.GetReverbSettingProfile(e), s);

    public static void WarmupGenericPools()
    {
      ManagerSingleton<SM>.Instance.m_pool_generic = SM.CreatePool(3, 12, FVRPooledAudioType.Generic);
      ManagerSingleton<SM>.Instance.m_pool_generic_close = SM.CreatePool(1, 6, FVRPooledAudioType.GenericClose);
      ManagerSingleton<SM>.Instance.m_pool_generic_long = SM.CreatePool(1, 6, FVRPooledAudioType.GenericLongRange);
      ManagerSingleton<SM>.Instance.m_pool_generic_verylong = SM.CreatePool(1, 6, FVRPooledAudioType.GenericVeryLongRange);
      ManagerSingleton<SM>.Instance.m_pool_explosion = SM.CreatePool(1, 6, FVRPooledAudioType.Explosion);
      ManagerSingleton<SM>.Instance.m_pool_explosionTail = SM.CreatePool(1, 6, FVRPooledAudioType.ExplosionTail);
      ManagerSingleton<SM>.Instance.m_pool_UIChirp = SM.CreatePool(1, 6, FVRPooledAudioType.UIChirp);
      ManagerSingleton<SM>.Instance.m_pool_NPCShotNear = SM.CreatePool(1, 6, FVRPooledAudioType.NPCShotNear);
      ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant = SM.CreatePool(1, 6, FVRPooledAudioType.NPCShotFarDistant);
      ManagerSingleton<SM>.Instance.m_pool_NPCHandling = SM.CreatePool(1, 3, FVRPooledAudioType.NPCHandling);
      ManagerSingleton<SM>.Instance.m_pool_NPCBarks = SM.CreatePool(1, 6, FVRPooledAudioType.NPCBarks);
      ManagerSingleton<SM>.Instance.m_pool_casings = SM.CreatePool(1, 8, FVRPooledAudioType.Casings);
      ManagerSingleton<SM>.Instance.m_pool_impacts = SM.CreatePool(1, 6, FVRPooledAudioType.Impacts);
    }

    public static void ClearGenericPools()
    {
      ManagerSingleton<SM>.Instance.m_pool_generic.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_generic = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_generic_close.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_generic_close = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_generic_long.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_generic_long = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_generic_verylong.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_generic_verylong = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_explosion.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_explosion = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_explosionTail.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_explosionTail = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_UIChirp.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_UIChirp = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_NPCShotNear.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_NPCShotNear = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_NPCHandling.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_NPCHandling = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_NPCBarks.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_NPCBarks = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_casings.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_casings = (SM.AudioSourcePool) null;
      ManagerSingleton<SM>.Instance.m_pool_impacts.Dispose();
      ManagerSingleton<SM>.Instance.m_pool_impacts = (SM.AudioSourcePool) null;
      SM.ActivePools.Clear();
    }

    public static void PlayGenericSound(AudioEvent ClipSet, Vector3 pos) => ManagerSingleton<SM>.Instance.m_pool_generic.PlayClip(ClipSet, pos);

    public static FVRPooledAudioSource PlayCoreSound(
      FVRPooledAudioType type,
      AudioEvent ClipSet,
      Vector3 pos)
    {
      switch (type)
      {
        case FVRPooledAudioType.Explosion:
          return ManagerSingleton<SM>.Instance.m_pool_explosion.PlayClip(ClipSet, pos);
        case FVRPooledAudioType.ExplosionTail:
          return ManagerSingleton<SM>.Instance.m_pool_explosionTail.PlayClip(ClipSet, pos);
        case FVRPooledAudioType.GenericClose:
          return ManagerSingleton<SM>.Instance.m_pool_generic_close.PlayClip(ClipSet, pos);
        case FVRPooledAudioType.GenericLongRange:
          return ManagerSingleton<SM>.Instance.m_pool_generic_long.PlayClip(ClipSet, pos);
        case FVRPooledAudioType.GenericVeryLongRange:
          return ManagerSingleton<SM>.Instance.m_pool_generic_verylong.PlayClip(ClipSet, pos);
        default:
          switch (type - 30)
          {
            case FVRPooledAudioType.Generic:
              return ManagerSingleton<SM>.Instance.m_pool_NPCShotNear.PlayClip(ClipSet, pos);
            case FVRPooledAudioType.GunShot:
              return ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant.PlayClip(ClipSet, pos);
            case FVRPooledAudioType.GunTail:
              return ManagerSingleton<SM>.Instance.m_pool_NPCHandling.PlayClip(ClipSet, pos);
            case FVRPooledAudioType.GunMech:
              return ManagerSingleton<SM>.Instance.m_pool_NPCBarks.PlayClip(ClipSet, pos);
            default:
              if (type == FVRPooledAudioType.Casings)
                return ManagerSingleton<SM>.Instance.m_pool_casings.PlayClip(ClipSet, pos);
              if (type == FVRPooledAudioType.Impacts)
                return ManagerSingleton<SM>.Instance.m_pool_impacts.PlayClip(ClipSet, pos);
              if (type == FVRPooledAudioType.Generic)
                return ManagerSingleton<SM>.Instance.m_pool_generic.PlayClip(ClipSet, pos);
              return type == FVRPooledAudioType.UIChirp ? ManagerSingleton<SM>.Instance.m_pool_UIChirp.PlayClip(ClipSet, pos) : (FVRPooledAudioSource) null;
          }
      }
    }

    public static void PlayCoreSoundOverrides(
      FVRPooledAudioType type,
      AudioEvent ClipSet,
      Vector3 pos,
      Vector2 volMult,
      Vector2 pitchMult)
    {
      switch (type)
      {
        case FVRPooledAudioType.Explosion:
          ManagerSingleton<SM>.Instance.m_pool_explosion.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
          break;
        case FVRPooledAudioType.ExplosionTail:
          ManagerSingleton<SM>.Instance.m_pool_explosionTail.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
          break;
        case FVRPooledAudioType.GenericClose:
          ManagerSingleton<SM>.Instance.m_pool_generic_close.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
          break;
        case FVRPooledAudioType.GenericLongRange:
          ManagerSingleton<SM>.Instance.m_pool_generic_long.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
          break;
        case FVRPooledAudioType.GenericVeryLongRange:
          ManagerSingleton<SM>.Instance.m_pool_generic_verylong.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
          break;
        default:
          switch (type - 30)
          {
            case FVRPooledAudioType.Generic:
              ManagerSingleton<SM>.Instance.m_pool_NPCShotNear.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
              return;
            case FVRPooledAudioType.GunShot:
              ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
              return;
            case FVRPooledAudioType.GunTail:
              ManagerSingleton<SM>.Instance.m_pool_NPCHandling.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
              return;
            case FVRPooledAudioType.GunMech:
              ManagerSingleton<SM>.Instance.m_pool_NPCBarks.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
              return;
            default:
              if (type != FVRPooledAudioType.Casings)
              {
                if (type != FVRPooledAudioType.Impacts)
                {
                  if (type != FVRPooledAudioType.Generic)
                  {
                    if (type != FVRPooledAudioType.UIChirp)
                      return;
                    ManagerSingleton<SM>.Instance.m_pool_UIChirp.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
                    return;
                  }
                  ManagerSingleton<SM>.Instance.m_pool_generic.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
                  return;
                }
                ManagerSingleton<SM>.Instance.m_pool_impacts.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
                return;
              }
              ManagerSingleton<SM>.Instance.m_pool_casings.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
              return;
          }
      }
    }

    public static void PlayCoreSoundDelayed(
      FVRPooledAudioType type,
      AudioEvent ClipSet,
      Vector3 pos,
      float delay)
    {
      switch (type)
      {
        case FVRPooledAudioType.Explosion:
          ManagerSingleton<SM>.Instance.m_pool_explosion.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
          break;
        case FVRPooledAudioType.ExplosionTail:
          ManagerSingleton<SM>.Instance.m_pool_explosionTail.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
          break;
        case FVRPooledAudioType.GenericClose:
          ManagerSingleton<SM>.Instance.m_pool_generic_close.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
          break;
        case FVRPooledAudioType.GenericLongRange:
          ManagerSingleton<SM>.Instance.m_pool_generic_long.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
          break;
        case FVRPooledAudioType.GenericVeryLongRange:
          ManagerSingleton<SM>.Instance.m_pool_generic_verylong.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
          break;
        default:
          switch (type - 30)
          {
            case FVRPooledAudioType.Generic:
              ManagerSingleton<SM>.Instance.m_pool_NPCShotNear.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
              return;
            case FVRPooledAudioType.GunShot:
              ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
              return;
            case FVRPooledAudioType.GunTail:
              ManagerSingleton<SM>.Instance.m_pool_NPCHandling.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
              return;
            case FVRPooledAudioType.GunMech:
              ManagerSingleton<SM>.Instance.m_pool_NPCBarks.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
              return;
            default:
              if (type != FVRPooledAudioType.Casings)
              {
                if (type != FVRPooledAudioType.Impacts)
                {
                  if (type != FVRPooledAudioType.Generic)
                  {
                    if (type != FVRPooledAudioType.UIChirp)
                      return;
                    ManagerSingleton<SM>.Instance.m_pool_UIChirp.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
                    return;
                  }
                  ManagerSingleton<SM>.Instance.m_pool_generic.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
                  return;
                }
                ManagerSingleton<SM>.Instance.m_pool_impacts.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
                return;
              }
              ManagerSingleton<SM>.Instance.m_pool_casings.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
              return;
          }
      }
    }

    public static void PlayCoreSoundDelayedOverrides(
      FVRPooledAudioType type,
      AudioEvent ClipSet,
      Vector3 pos,
      Vector2 vol,
      Vector2 pitch,
      float delay)
    {
      switch (type)
      {
        case FVRPooledAudioType.Explosion:
          ManagerSingleton<SM>.Instance.m_pool_explosion.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
          break;
        case FVRPooledAudioType.ExplosionTail:
          ManagerSingleton<SM>.Instance.m_pool_explosionTail.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
          break;
        case FVRPooledAudioType.GenericClose:
          ManagerSingleton<SM>.Instance.m_pool_generic_close.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
          break;
        case FVRPooledAudioType.GenericLongRange:
          ManagerSingleton<SM>.Instance.m_pool_generic_long.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
          break;
        case FVRPooledAudioType.GenericVeryLongRange:
          ManagerSingleton<SM>.Instance.m_pool_generic_verylong.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
          break;
        default:
          switch (type - 30)
          {
            case FVRPooledAudioType.Generic:
              ManagerSingleton<SM>.Instance.m_pool_NPCShotNear.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
              return;
            case FVRPooledAudioType.GunShot:
              ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
              return;
            case FVRPooledAudioType.GunTail:
              ManagerSingleton<SM>.Instance.m_pool_NPCHandling.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
              return;
            case FVRPooledAudioType.GunMech:
              ManagerSingleton<SM>.Instance.m_pool_NPCBarks.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
              return;
            default:
              if (type != FVRPooledAudioType.Casings)
              {
                if (type != FVRPooledAudioType.Impacts)
                {
                  if (type != FVRPooledAudioType.Generic)
                  {
                    if (type != FVRPooledAudioType.UIChirp)
                      return;
                    ManagerSingleton<SM>.Instance.m_pool_UIChirp.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
                    return;
                  }
                  ManagerSingleton<SM>.Instance.m_pool_generic.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
                  return;
                }
                ManagerSingleton<SM>.Instance.m_pool_impacts.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
                return;
              }
              ManagerSingleton<SM>.Instance.m_pool_casings.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
              return;
          }
      }
    }

    public static SM.AudioSourcePool GetCorePool(FVRPooledAudioType type)
    {
      switch (type)
      {
        case FVRPooledAudioType.Explosion:
          return ManagerSingleton<SM>.Instance.m_pool_explosion;
        case FVRPooledAudioType.ExplosionTail:
          return ManagerSingleton<SM>.Instance.m_pool_explosionTail;
        case FVRPooledAudioType.GenericClose:
          return ManagerSingleton<SM>.Instance.m_pool_generic_close;
        case FVRPooledAudioType.GenericLongRange:
          return ManagerSingleton<SM>.Instance.m_pool_generic_long;
        case FVRPooledAudioType.GenericVeryLongRange:
          return ManagerSingleton<SM>.Instance.m_pool_generic_verylong;
        default:
          if (type == FVRPooledAudioType.NPCHandling)
            return ManagerSingleton<SM>.Instance.m_pool_NPCHandling;
          if (type == FVRPooledAudioType.NPCBarks)
            return ManagerSingleton<SM>.Instance.m_pool_NPCBarks;
          if (type == FVRPooledAudioType.Casings)
            return ManagerSingleton<SM>.Instance.m_pool_casings;
          if (type == FVRPooledAudioType.Impacts)
            return ManagerSingleton<SM>.Instance.m_pool_impacts;
          if (type == FVRPooledAudioType.Generic)
            return ManagerSingleton<SM>.Instance.m_pool_generic;
          return type == FVRPooledAudioType.UIChirp ? ManagerSingleton<SM>.Instance.m_pool_UIChirp : (SM.AudioSourcePool) null;
      }
    }

    private void generatePoolTypePrefabBindingDic()
    {
      for (int index = 0; index < this.PrefabDirectory.PrefabBindings.Count; ++index)
        this.m_prefabBindingDic.Add((int) this.PrefabDirectory.PrefabBindings[index].Type, this.PrefabDirectory.PrefabBindings[index]);
    }

    private void generateTailsDictionary()
    {
      for (int index1 = 0; index1 < this.TailsDirectories.Length; ++index1)
      {
        Dictionary<FVRSoundEnvironment, AudioEvent> dictionary = !this.m_tailsDic.ContainsKey(this.TailsDirectories[index1].SoundClass) ? new Dictionary<FVRSoundEnvironment, AudioEvent>() : this.m_tailsDic[this.TailsDirectories[index1].SoundClass];
        for (int index2 = 0; index2 < this.TailsDirectories[index1].SoundSets.Count; ++index2)
          dictionary.Add(this.TailsDirectories[index1].SoundSets[index2].Environment, this.TailsDirectories[index1].SoundSets[index2].AudioEvent);
        this.m_tailsDic.Add(this.TailsDirectories[index1].SoundClass, dictionary);
      }
    }

    private void generateReverbDictionary()
    {
      for (int index = 0; index < this.ReverbSettingProfiles.Length; ++index)
        this.m_reverbDic.Add(this.ReverbSettingProfiles[index].Settings.Environment, this.ReverbSettingProfiles[index]);
    }

    private void generateImpactDictionary()
    {
      for (int index = 0; index < this.AudioImpactSets.Length; ++index)
      {
        AudioImpactSet audioImpactSet = this.AudioImpactSets[index];
        Dictionary<MatSoundType, Dictionary<AudioImpactIntensity, AudioEvent>> dictionary = new Dictionary<MatSoundType, Dictionary<AudioImpactIntensity, AudioEvent>>();
        Dictionary<AudioImpactIntensity, AudioEvent> impactMaterialGroup1 = this.GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Carpet, audioImpactSet.PitchRange);
        Dictionary<AudioImpactIntensity, AudioEvent> impactMaterialGroup2 = this.GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.HardSurface, audioImpactSet.PitchRange);
        Dictionary<AudioImpactIntensity, AudioEvent> impactMaterialGroup3 = this.GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.LooseSurface, audioImpactSet.PitchRange);
        Dictionary<AudioImpactIntensity, AudioEvent> impactMaterialGroup4 = this.GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Meat, audioImpactSet.PitchRange);
        Dictionary<AudioImpactIntensity, AudioEvent> impactMaterialGroup5 = this.GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Metal, audioImpactSet.PitchRange);
        Dictionary<AudioImpactIntensity, AudioEvent> impactMaterialGroup6 = this.GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Plastic, audioImpactSet.PitchRange);
        Dictionary<AudioImpactIntensity, AudioEvent> impactMaterialGroup7 = this.GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.SoftSurface, audioImpactSet.PitchRange);
        Dictionary<AudioImpactIntensity, AudioEvent> impactMaterialGroup8 = this.GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Tile, audioImpactSet.PitchRange);
        Dictionary<AudioImpactIntensity, AudioEvent> impactMaterialGroup9 = this.GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Water, audioImpactSet.PitchRange);
        Dictionary<AudioImpactIntensity, AudioEvent> impactMaterialGroup10 = this.GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Wood, audioImpactSet.PitchRange);
        dictionary.Add(MatSoundType.Carpet, impactMaterialGroup1);
        dictionary.Add(MatSoundType.HardSurface, impactMaterialGroup2);
        dictionary.Add(MatSoundType.LooseSurface, impactMaterialGroup3);
        dictionary.Add(MatSoundType.Meat, impactMaterialGroup4);
        dictionary.Add(MatSoundType.Metal, impactMaterialGroup5);
        dictionary.Add(MatSoundType.Plastic, impactMaterialGroup6);
        dictionary.Add(MatSoundType.SoftSurface, impactMaterialGroup7);
        dictionary.Add(MatSoundType.Tile, impactMaterialGroup8);
        dictionary.Add(MatSoundType.Water, impactMaterialGroup9);
        dictionary.Add(MatSoundType.Wood, impactMaterialGroup10);
        this.m_impactDic.Add(audioImpactSet.ImpactType, dictionary);
      }
    }

    private void generateHandlingDictionaries()
    {
      HandlingGrabSet[] handlingGrabSetArray = Resources.LoadAll<HandlingGrabSet>("HandlingAudioDefs/Grab");
      HandlingReleaseSet[] handlingReleaseSetArray = Resources.LoadAll<HandlingReleaseSet>("HandlingAudioDefs/Release");
      HandlingReleaseIntoSlotSet[] releaseIntoSlotSetArray = Resources.LoadAll<HandlingReleaseIntoSlotSet>("HandlingAudioDefs/ReleaseIntoSlot");
      for (int index = 0; index < handlingGrabSetArray.Length; ++index)
        this.m_handlingGrabDic.Add(handlingGrabSetArray[index].Type, handlingGrabSetArray[index]);
      for (int index = 0; index < handlingReleaseSetArray.Length; ++index)
        this.m_handlingReleaseDic.Add(handlingReleaseSetArray[index].Type, handlingReleaseSetArray[index]);
      for (int index = 0; index < releaseIntoSlotSetArray.Length; ++index)
        this.m_handlingReleaseIntoSlotDic.Add(releaseIntoSlotSetArray[index].Type, releaseIntoSlotSetArray[index]);
      for (int index = 0; index < this.AudioBulletImpactSets.Length; ++index)
        this.m_bulletHitDic.Add(this.AudioBulletImpactSets[index].Type, this.AudioBulletImpactSets[index]);
    }

    private Dictionary<AudioImpactIntensity, AudioEvent> GenerateAudioEventDicFromImpactMaterialGroup(
      AudioImpactMaterialGroup g,
      Vector2 pitchRange)
    {
      Dictionary<AudioImpactIntensity, AudioEvent> dictionary = new Dictionary<AudioImpactIntensity, AudioEvent>();
      AudioEvent audioEvent1 = new AudioEvent();
      AudioEvent audioEvent2 = new AudioEvent();
      AudioEvent audioEvent3 = new AudioEvent();
      audioEvent1.VolumeRange = new Vector2(0.35f * g.Volumes.x, 0.4f * g.Volumes.x);
      audioEvent2.VolumeRange = new Vector2(0.35f * g.Volumes.y, 0.4f * g.Volumes.y);
      audioEvent3.VolumeRange = new Vector2(0.35f * g.Volumes.z, 0.4f * g.Volumes.z);
      audioEvent1.PitchRange = pitchRange;
      audioEvent2.PitchRange = pitchRange;
      audioEvent3.PitchRange = pitchRange;
      for (int index = 0; index < g.Clips_Light.Count; ++index)
        audioEvent1.Clips.Add(g.Clips_Light[index]);
      for (int index = 0; index < g.Clips_Medium.Count; ++index)
        audioEvent2.Clips.Add(g.Clips_Medium[index]);
      for (int index = 0; index < g.Clips_Hard.Count; ++index)
        audioEvent3.Clips.Add(g.Clips_Hard[index]);
      audioEvent1.SetLengthRange();
      audioEvent2.SetLengthRange();
      audioEvent3.SetLengthRange();
      dictionary.Add(AudioImpactIntensity.Light, audioEvent1);
      dictionary.Add(AudioImpactIntensity.Medium, audioEvent2);
      dictionary.Add(AudioImpactIntensity.Hard, audioEvent3);
      return dictionary;
    }

    public static AudioEvent GetImpactAudioEvent(
      ImpactType impacttype,
      MatSoundType mat,
      AudioImpactIntensity impactIntensity)
    {
      return ManagerSingleton<SM>.Instance.m_impactDic.ContainsKey(impacttype) ? ManagerSingleton<SM>.Instance.m_impactDic[impacttype][mat][impactIntensity] : ManagerSingleton<SM>.Instance.m_impactDic[ImpactType.Generic][mat][impactIntensity];
    }

    public static AudioEvent GetButtletImpactAudioEvent(BulletImpactSoundType t) => ManagerSingleton<SM>.Instance.m_bulletHitDic[t].AudEvent_Set;

    public static void PlayHandlingGrabSound(HandlingGrabType t, Vector3 pos, bool isHard)
    {
      if (!ManagerSingleton<SM>.Instance.m_handlingGrabDic.ContainsKey(t))
        return;
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, !isHard ? ManagerSingleton<SM>.Instance.m_handlingGrabDic[t].GrabSet_Light : ManagerSingleton<SM>.Instance.m_handlingGrabDic[t].GrabSet_Hard, pos);
    }

    public static void PlayHandlingReleaseSound(HandlingReleaseType t, Vector3 pos)
    {
      if (!ManagerSingleton<SM>.Instance.m_handlingReleaseDic.ContainsKey(t))
        return;
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, ManagerSingleton<SM>.Instance.m_handlingReleaseDic[t].ReleaseSet, pos);
    }

    public static void PlayHandlingReleaseIntoSlotSound(HandlingReleaseIntoSlotType t, Vector3 pos)
    {
      if (!ManagerSingleton<SM>.Instance.m_handlingReleaseIntoSlotDic.ContainsKey(t))
        return;
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, ManagerSingleton<SM>.Instance.m_handlingReleaseIntoSlotDic[t].ReleaseIntoSlotSet, pos);
    }

    public static float PlayImpactSound(
      ImpactType impacttype,
      MatSoundType mat,
      AudioImpactIntensity impactIntensity,
      Vector3 pos,
      FVRPooledAudioType pool,
      float distanceLimit)
    {
      if (ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame > 2)
        return -1f;
      if ((double) Vector3.Distance(pos, GM.CurrentPlayerBody.Head.position) > (double) distanceLimit)
        return 0.0f;
      AudioEvent impactAudioEvent = SM.GetImpactAudioEvent(impacttype, mat, impactIntensity);
      SM.PlayCoreSound(pool, impactAudioEvent, pos);
      ++ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame;
      return impactAudioEvent.ClipLengthRange.y;
    }

    public static float PlayImpactSound(
      ImpactType impacttype,
      MatSoundType mat,
      AudioImpactIntensity impactIntensity,
      Vector3 pos,
      FVRPooledAudioType pool,
      float distanceLimit,
      float volumeMult,
      float pitchMult)
    {
      if (ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame > 2 || (double) Vector3.Distance(pos, GM.CurrentPlayerBody.Head.position) > (double) distanceLimit)
        return 0.0f;
      AudioEvent impactAudioEvent = SM.GetImpactAudioEvent(impacttype, mat, impactIntensity);
      SM.PlayCoreSoundOverrides(pool, impactAudioEvent, pos, impactAudioEvent.VolumeRange * volumeMult, impactAudioEvent.PitchRange * pitchMult);
      ++ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame;
      return impactAudioEvent.ClipLengthRange.y;
    }

    public static float PlayBulletImpactHit(
      BulletImpactSoundType type,
      Vector3 pos,
      float distanceLimit,
      float volumeMult,
      float pitchmult)
    {
      if (ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame > 2 || type == BulletImpactSoundType.None || (double) Vector3.Distance(pos, GM.CurrentPlayerBody.Head.position) > (double) distanceLimit)
        return 0.0f;
      AudioEvent impactAudioEvent = SM.GetButtletImpactAudioEvent(type);
      SM.PlayCoreSoundOverrides(FVRPooledAudioType.Impacts, impactAudioEvent, pos, impactAudioEvent.VolumeRange * volumeMult, impactAudioEvent.PitchRange * pitchmult);
      ++ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame;
      return impactAudioEvent.ClipLengthRange.y;
    }

    public static float PlayBulletImpactHit(
      BulletImpactSoundType type,
      Vector3 pos,
      float volumeMult,
      float pitchmult)
    {
      if (ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame > 2 || type == BulletImpactSoundType.None)
        return 0.0f;
      AudioEvent impactAudioEvent = SM.GetButtletImpactAudioEvent(type);
      SM.PlayCoreSoundOverrides(FVRPooledAudioType.Impacts, impactAudioEvent, pos, impactAudioEvent.VolumeRange * volumeMult, impactAudioEvent.PitchRange * pitchmult);
      ++ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame;
      return impactAudioEvent.ClipLengthRange.y;
    }

    public static GameObject GetPrefabForType(FVRPooledAudioType type) => ManagerSingleton<SM>.Instance.m_prefabBindingDic[(int) type].Prefab;

    public static AudioEvent GetTailSet(
      FVRTailSoundClass tClass,
      FVRSoundEnvironment tEnvironment)
    {
      return ManagerSingleton<SM>.Instance.m_tailsDic[tClass][tEnvironment];
    }

    public static float GetLowPassOcclusionValue(Vector3 start, Vector3 end) => Physics.Linecast(start, end, (int) ManagerSingleton<SM>.Instance.OcclusionLM, QueryTriggerInteraction.Ignore) ? ManagerSingleton<SM>.Instance.OcclusionFactorCurve.Evaluate(Vector3.Distance(start, end)) : 22000f;

    public static float GetOcclusionFactor(Vector3 start, Vector3 end) => Physics.Linecast(start, end, (int) ManagerSingleton<SM>.Instance.OcclusionLM, QueryTriggerInteraction.Ignore) ? Mathf.Pow(Mathf.InverseLerp(1f, 1000f, Vector3.Distance(start, end)), 0.1f) : 0.0f;

    public static FVRReverbSettingProfile GetReverbSettingProfile(
      FVRSoundEnvironment rEnvironment)
    {
      return ManagerSingleton<SM>.Instance.m_reverbDic[rEnvironment];
    }

    public static SM.AudioSourcePool CreatePool(
      int initSize,
      int maxSize,
      FVRPooledAudioType type)
    {
      SM.AudioSourcePool audioSourcePool = new SM.AudioSourcePool(initSize, maxSize, type);
      SM.ActivePools.Add(audioSourcePool);
      return audioSourcePool;
    }

    public static float GetSoundTravelDistanceMultByEnvironment(FVRSoundEnvironment se)
    {
      switch (se)
      {
        case FVRSoundEnvironment.InsideNarrow:
          return 0.75f;
        case FVRSoundEnvironment.InsideSmall:
          return 0.7f;
        case FVRSoundEnvironment.InsideWarehouse:
          return 1f;
        case FVRSoundEnvironment.InsideNarrowSmall:
          return 0.7f;
        case FVRSoundEnvironment.InsideLarge:
          return 1f;
        case FVRSoundEnvironment.InsideWarehouseSmall:
          return 1f;
        case FVRSoundEnvironment.InsideMedium:
          return 1f;
        case FVRSoundEnvironment.InsideLargeHighCeiling:
          return 1f;
        case FVRSoundEnvironment.OutsideOpen:
          return 2.5f;
        case FVRSoundEnvironment.OutsideEnclosed:
          return 2f;
        case FVRSoundEnvironment.OutsideEnclosedNarrow:
          return 1.75f;
        case FVRSoundEnvironment.SniperRange:
          return 1f;
        case FVRSoundEnvironment.ShootingRange:
          return 1f;
        default:
          return se == FVRSoundEnvironment.Forest ? 2f : 1f;
      }
    }

    public static float GetImpactSoundVolumeMultFromMaterial(MatSoundType m)
    {
      switch (m)
      {
        case MatSoundType.HardSurface:
          return 1f;
        case MatSoundType.Wood:
          return 0.8f;
        case MatSoundType.SoftSurface:
          return 0.4f;
        case MatSoundType.LooseSurface:
          return 0.8f;
        case MatSoundType.Metal:
          return 1.8f;
        case MatSoundType.Plastic:
          return 0.8f;
        case MatSoundType.Tile:
          return 1.2f;
        case MatSoundType.Carpet:
          return 0.3f;
        case MatSoundType.Meat:
          return 0.5f;
        case MatSoundType.Water:
          return 1.1f;
        default:
          return 1f;
      }
    }

    public void Update()
    {
      for (int index = 0; index < this.m_activePools.Count; ++index)
        this.m_activePools[index].Tick();
      ManagerSingleton<SM>.Instance.m_reverbSettings.Tick(Time.deltaTime);
      this.m_numImpactSoundsThisFrame = 0;
    }

    public class AudioSourcePool
    {
      public Queue<FVRPooledAudioSource> SourceQueue_Disabled;
      public List<FVRPooledAudioSource> ActiveSources;
      public FVRPooledAudioType Type;
      public List<SM.AudioSourcePool.DelayedAudioEvent> DelayedEvents;
      private int m_maxSize;
      private int m_curSize;

      public AudioSourcePool(int initSize, int maxSize, FVRPooledAudioType type)
      {
        this.SourceQueue_Disabled = new Queue<FVRPooledAudioSource>();
        this.ActiveSources = new List<FVRPooledAudioSource>();
        this.DelayedEvents = new List<SM.AudioSourcePool.DelayedAudioEvent>();
        this.Type = type;
        this.m_maxSize = maxSize;
        if (initSize <= 0)
          return;
        GameObject prefabForType = SM.GetPrefabForType(type);
        for (int index = 0; index < initSize; ++index)
          this.InstantiateAndEnqueue(prefabForType, false);
      }

      public void InstantiateAndEnqueue(GameObject prefab, bool active)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
        FVRPooledAudioSource component = gameObject.GetComponent<FVRPooledAudioSource>();
        if (!active)
          gameObject.SetActive(false);
        this.SourceQueue_Disabled.Enqueue(component);
        ++this.m_curSize;
      }

      public void PlayDelayedClip(
        float delay,
        AudioEvent clipset,
        Vector3 pos,
        Vector2 vol,
        Vector2 pitch,
        AudioMixerGroup mixerOverride = null)
      {
        this.DelayedEvents.Add(new SM.AudioSourcePool.DelayedAudioEvent(delay, clipset, pos, vol, pitch, mixerOverride));
      }

      public FVRPooledAudioSource PlayClip(
        AudioEvent clipSet,
        Vector3 pos,
        AudioMixerGroup mixerOverride = null)
      {
        if (clipSet.Clips.Count <= 0)
          return (FVRPooledAudioSource) null;
        if (this.SourceQueue_Disabled.Count > 0)
          return this.DequeueAndPlay(clipSet, pos, clipSet.PitchRange, clipSet.VolumeRange, mixerOverride);
        if (this.m_curSize < this.m_maxSize)
        {
          this.InstantiateAndEnqueue(SM.GetPrefabForType(this.Type), true);
          return this.DequeueAndPlay(clipSet, pos, clipSet.PitchRange, clipSet.VolumeRange, mixerOverride);
        }
        FVRPooledAudioSource activeSource = this.ActiveSources[0];
        this.ActiveSources.RemoveAt(0);
        if (!activeSource.gameObject.activeSelf)
          activeSource.gameObject.SetActive(true);
        activeSource.Play(clipSet, pos, clipSet.PitchRange, clipSet.VolumeRange, mixerOverride);
        this.ActiveSources.Add(activeSource);
        return activeSource;
      }

      public FVRPooledAudioSource PlayClipPitchOverride(
        AudioEvent clipSet,
        Vector3 pos,
        Vector2 pitchOverride,
        AudioMixerGroup mixerOverride = null)
      {
        if (clipSet.Clips.Count <= 0)
          return (FVRPooledAudioSource) null;
        if (this.SourceQueue_Disabled.Count > 0)
          return this.DequeueAndPlay(clipSet, pos, pitchOverride, clipSet.VolumeRange, mixerOverride);
        if (this.m_curSize < this.m_maxSize)
        {
          this.InstantiateAndEnqueue(SM.GetPrefabForType(this.Type), true);
          return this.DequeueAndPlay(clipSet, pos, pitchOverride, clipSet.VolumeRange, mixerOverride);
        }
        FVRPooledAudioSource activeSource = this.ActiveSources[0];
        this.ActiveSources.RemoveAt(0);
        if (!activeSource.gameObject.activeSelf)
          activeSource.gameObject.SetActive(true);
        activeSource.Play(clipSet, pos, pitchOverride, clipSet.VolumeRange, mixerOverride);
        this.ActiveSources.Add(activeSource);
        return activeSource;
      }

      public FVRPooledAudioSource PlayClipVolumePitchOverride(
        AudioEvent clipSet,
        Vector3 pos,
        Vector2 volumeOverride,
        Vector2 pitchOverride,
        AudioMixerGroup mixerOverride = null)
      {
        if (clipSet.Clips.Count <= 0)
          return (FVRPooledAudioSource) null;
        if (this.SourceQueue_Disabled.Count > 0)
          return this.DequeueAndPlay(clipSet, pos, pitchOverride, volumeOverride, mixerOverride);
        if (this.m_curSize < this.m_maxSize)
        {
          this.InstantiateAndEnqueue(SM.GetPrefabForType(this.Type), true);
          return this.DequeueAndPlay(clipSet, pos, pitchOverride, volumeOverride, mixerOverride);
        }
        FVRPooledAudioSource activeSource = this.ActiveSources[0];
        this.ActiveSources.RemoveAt(0);
        if (!activeSource.gameObject.activeSelf)
          activeSource.gameObject.SetActive(true);
        activeSource.Play(clipSet, pos, pitchOverride, volumeOverride, mixerOverride);
        this.ActiveSources.Add(activeSource);
        return activeSource;
      }

      private FVRPooledAudioSource DequeueAndPlay(
        AudioEvent clipSet,
        Vector3 pos,
        Vector2 pitch,
        Vector2 volume,
        AudioMixerGroup mixerOverride = null)
      {
        FVRPooledAudioSource pooledAudioSource = this.SourceQueue_Disabled.Dequeue();
        pooledAudioSource.gameObject.SetActive(true);
        pooledAudioSource.Play(clipSet, pos, pitch, volume, mixerOverride);
        this.ActiveSources.Add(pooledAudioSource);
        return pooledAudioSource;
      }

      public void Tick()
      {
        float deltaTime = Time.deltaTime;
        if (this.DelayedEvents.Count > 0)
        {
          for (int index = this.DelayedEvents.Count - 1; index >= 0; --index)
          {
            this.DelayedEvents[index].tickDown -= deltaTime;
            if ((double) this.DelayedEvents[index].tickDown <= 0.0)
            {
              this.PlayClipVolumePitchOverride(this.DelayedEvents[index].e, this.DelayedEvents[index].pos, this.DelayedEvents[index].vol, this.DelayedEvents[index].pitch, this.DelayedEvents[index].mixOverride);
              this.DelayedEvents[index].mixOverride = (AudioMixerGroup) null;
              this.DelayedEvents[index].e = (AudioEvent) null;
              this.DelayedEvents.RemoveAt(index);
            }
          }
        }
        if (this.ActiveSources == null || this.ActiveSources.Count == 0)
          return;
        for (int index = this.ActiveSources.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) this.ActiveSources[index] == (UnityEngine.Object) null)
          {
            this.ActiveSources.RemoveAt(index);
          }
          else
          {
            FVRPooledAudioSource activeSource = this.ActiveSources[index];
            if (!activeSource.Source.isPlaying)
            {
              this.ActiveSources.RemoveAt(index);
              activeSource.gameObject.SetActive(false);
              this.SourceQueue_Disabled.Enqueue(activeSource);
            }
            else
              activeSource.Tick(deltaTime);
          }
        }
      }

      public void Dispose()
      {
        for (int index = this.ActiveSources.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) this.ActiveSources[index] != (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) this.ActiveSources[index].gameObject);
        }
        this.ActiveSources.Clear();
        this.ActiveSources = (List<FVRPooledAudioSource>) null;
        while (this.SourceQueue_Disabled.Count > 0)
        {
          FVRPooledAudioSource pooledAudioSource = this.SourceQueue_Disabled.Dequeue();
          if ((UnityEngine.Object) pooledAudioSource != (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) pooledAudioSource.gameObject);
        }
        this.SourceQueue_Disabled.Clear();
        this.SourceQueue_Disabled = (Queue<FVRPooledAudioSource>) null;
        this.DelayedEvents.Clear();
        this.DelayedEvents = (List<SM.AudioSourcePool.DelayedAudioEvent>) null;
      }

      public class DelayedAudioEvent
      {
        public float tickDown;
        public AudioEvent e;
        public Vector3 pos;
        public Vector2 vol;
        public Vector2 pitch;
        public AudioMixerGroup mixOverride;

        public DelayedAudioEvent(
          float TickDown,
          AudioEvent E,
          Vector3 Pos,
          Vector2 Vol,
          Vector2 Pitch,
          AudioMixerGroup MixOverride = null)
        {
          this.tickDown = TickDown;
          this.e = E;
          this.pos = Pos;
          this.vol = Vol;
          this.pitch = Pitch;
          this.mixOverride = MixOverride;
        }
      }
    }

    [Serializable]
    public class ReverbSettings
    {
      public AudioMixer MasterMixer;
      public FVRSoundEnvironment Environment;
      public float Room = -1000f;
      public float RoomHF = -100f;
      public float RoomLF;
      public float DecayTime = 1.49f;
      public float DecayHFRatio = 0.83f;
      public float Reflections = -2602f;
      public float ReflectionsDelay = 0.007f;
      public float Reverb = 200f;
      public float ReverbDelay = 11f / 1000f;
      public float HFReference = 5000f;
      public float LFReference = 250f;
      public float Diffusion = 100f;
      public float Density = 100f;
      private bool m_isTransitioningToReverbEnvironment;
      private float m_reverbTransitionSpeed = 1f;
      private float m_reverbTransitionTick;
      private FVRReverbSettingProfile m_reverbFromProfile;
      private FVRReverbSettingProfile m_reverbToProfile;

      public void Set(FVRReverbSettingProfile p)
      {
        this.m_reverbFromProfile = p;
        this.Environment = p.Settings.Environment;
        this.Room = p.Settings.Room;
        this.RoomHF = p.Settings.RoomHF;
        this.RoomLF = p.Settings.RoomLF;
        this.DecayTime = p.Settings.DecayTime;
        this.DecayHFRatio = p.Settings.DecayHFRatio;
        this.Reflections = p.Settings.Reflections;
        this.ReflectionsDelay = p.Settings.ReflectionsDelay;
        this.Reverb = p.Settings.Reverb;
        this.ReverbDelay = p.Settings.ReverbDelay;
        this.HFReference = p.Settings.HFReference;
        this.LFReference = p.Settings.LFReference;
        this.Diffusion = p.Settings.Diffusion;
        this.Density = p.Settings.Density;
        this.UpdateMixerParams();
      }

      public void TransitionTo(FVRReverbSettingProfile newProfile, float Speed)
      {
        if (this.Environment == newProfile.Settings.Environment)
          return;
        this.m_reverbToProfile = newProfile;
        this.m_reverbTransitionSpeed = Speed;
        this.m_reverbTransitionTick = 0.0f;
        this.m_isTransitioningToReverbEnvironment = true;
      }

      public void Tick(float t)
      {
        if (!this.m_isTransitioningToReverbEnvironment)
          return;
        this.m_reverbTransitionTick += t * this.m_reverbTransitionSpeed;
        this.Room = Mathf.Lerp(this.m_reverbFromProfile.Settings.Room, this.m_reverbToProfile.Settings.Room, this.m_reverbTransitionTick);
        this.RoomHF = Mathf.Lerp(this.m_reverbFromProfile.Settings.RoomHF, this.m_reverbToProfile.Settings.RoomHF, this.m_reverbTransitionTick);
        this.RoomLF = Mathf.Lerp(this.m_reverbFromProfile.Settings.RoomLF, this.m_reverbToProfile.Settings.RoomLF, this.m_reverbTransitionTick);
        this.DecayTime = Mathf.Lerp(this.m_reverbFromProfile.Settings.DecayTime, this.m_reverbToProfile.Settings.DecayTime, this.m_reverbTransitionTick);
        this.DecayHFRatio = Mathf.Lerp(this.m_reverbFromProfile.Settings.DecayHFRatio, this.m_reverbToProfile.Settings.DecayHFRatio, this.m_reverbTransitionTick);
        this.Reflections = Mathf.Lerp(this.m_reverbFromProfile.Settings.Reflections, this.m_reverbToProfile.Settings.Reflections, this.m_reverbTransitionTick);
        this.ReflectionsDelay = Mathf.Lerp(this.m_reverbFromProfile.Settings.ReflectionsDelay, this.m_reverbToProfile.Settings.ReflectionsDelay, this.m_reverbTransitionTick);
        this.Reverb = Mathf.Lerp(this.m_reverbFromProfile.Settings.Reverb, this.m_reverbToProfile.Settings.Reverb, this.m_reverbTransitionTick);
        this.ReverbDelay = Mathf.Lerp(this.m_reverbFromProfile.Settings.ReverbDelay, this.m_reverbToProfile.Settings.ReverbDelay, this.m_reverbTransitionTick);
        this.HFReference = Mathf.Lerp(this.m_reverbFromProfile.Settings.HFReference, this.m_reverbToProfile.Settings.HFReference, this.m_reverbTransitionTick);
        this.LFReference = Mathf.Lerp(this.m_reverbFromProfile.Settings.LFReference, this.m_reverbToProfile.Settings.LFReference, this.m_reverbTransitionTick);
        this.Diffusion = Mathf.Lerp(this.m_reverbFromProfile.Settings.Diffusion, this.m_reverbToProfile.Settings.Diffusion, this.m_reverbTransitionTick);
        this.Density = Mathf.Lerp(this.m_reverbFromProfile.Settings.Density, this.m_reverbToProfile.Settings.Density, this.m_reverbTransitionTick);
        if ((double) this.m_reverbTransitionTick >= 1.0)
        {
          this.m_reverbTransitionTick = 0.0f;
          this.m_isTransitioningToReverbEnvironment = false;
          this.m_reverbFromProfile = this.m_reverbToProfile;
        }
        this.UpdateMixerParams();
      }

      private void UpdateMixerParams()
      {
        this.MasterMixer.SetFloat("MReverb_Room", this.Room);
        this.MasterMixer.SetFloat("MReverb_RoomHF", this.RoomHF);
        this.MasterMixer.SetFloat("MReverb_RoomLF", this.RoomLF);
        this.MasterMixer.SetFloat("MReverb_DecayTime", this.DecayTime);
        this.MasterMixer.SetFloat("MReverb_DecayHFRatio", this.DecayHFRatio);
        this.MasterMixer.SetFloat("MReverb_Reflections", this.Reflections);
        this.MasterMixer.SetFloat("MReverb_ReflectionsDelay", this.ReflectionsDelay);
        this.MasterMixer.SetFloat("MReverb_Reverb", this.Reverb);
        this.MasterMixer.SetFloat("MReverb_ReverbDelay", this.ReverbDelay);
        this.MasterMixer.SetFloat("MReverb_HFReference", this.HFReference);
        this.MasterMixer.SetFloat("MReverb_LFReference", this.LFReference);
        this.MasterMixer.SetFloat("MReverb_Diffusion", this.Diffusion);
        this.MasterMixer.SetFloat("MReverb_Density", this.Density);
      }
    }

    public class ProgressiveOcclusionSampler
    {
      public Transform Root;
    }
  }
}
