using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FistVR
{
	public class SM : ManagerSingleton<SM>
	{
		public class AudioSourcePool
		{
			public class DelayedAudioEvent
			{
				public float tickDown;

				public AudioEvent e;

				public Vector3 pos;

				public Vector2 vol;

				public Vector2 pitch;

				public AudioMixerGroup mixOverride;

				public DelayedAudioEvent(float TickDown, AudioEvent E, Vector3 Pos, Vector2 Vol, Vector2 Pitch, AudioMixerGroup MixOverride = null)
				{
					tickDown = TickDown;
					e = E;
					pos = Pos;
					vol = Vol;
					pitch = Pitch;
					mixOverride = MixOverride;
				}
			}

			public Queue<FVRPooledAudioSource> SourceQueue_Disabled;

			public List<FVRPooledAudioSource> ActiveSources;

			public FVRPooledAudioType Type;

			public List<DelayedAudioEvent> DelayedEvents;

			private int m_maxSize;

			private int m_curSize;

			public AudioSourcePool(int initSize, int maxSize, FVRPooledAudioType type)
			{
				SourceQueue_Disabled = new Queue<FVRPooledAudioSource>();
				ActiveSources = new List<FVRPooledAudioSource>();
				DelayedEvents = new List<DelayedAudioEvent>();
				Type = type;
				m_maxSize = maxSize;
				if (initSize > 0)
				{
					GameObject prefabForType = GetPrefabForType(type);
					for (int i = 0; i < initSize; i++)
					{
						InstantiateAndEnqueue(prefabForType, active: false);
					}
				}
			}

			public void InstantiateAndEnqueue(GameObject prefab, bool active)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
				FVRPooledAudioSource component = gameObject.GetComponent<FVRPooledAudioSource>();
				if (!active)
				{
					gameObject.SetActive(value: false);
				}
				SourceQueue_Disabled.Enqueue(component);
				m_curSize++;
			}

			public void PlayDelayedClip(float delay, AudioEvent clipset, Vector3 pos, Vector2 vol, Vector2 pitch, AudioMixerGroup mixerOverride = null)
			{
				DelayedAudioEvent item = new DelayedAudioEvent(delay, clipset, pos, vol, pitch, mixerOverride);
				DelayedEvents.Add(item);
			}

			public FVRPooledAudioSource PlayClip(AudioEvent clipSet, Vector3 pos, AudioMixerGroup mixerOverride = null)
			{
				if (clipSet.Clips.Count <= 0)
				{
					return null;
				}
				if (SourceQueue_Disabled.Count > 0)
				{
					return DequeueAndPlay(clipSet, pos, clipSet.PitchRange, clipSet.VolumeRange, mixerOverride);
				}
				if (m_curSize < m_maxSize)
				{
					GameObject prefabForType = GetPrefabForType(Type);
					InstantiateAndEnqueue(prefabForType, active: true);
					return DequeueAndPlay(clipSet, pos, clipSet.PitchRange, clipSet.VolumeRange, mixerOverride);
				}
				FVRPooledAudioSource fVRPooledAudioSource = ActiveSources[0];
				ActiveSources.RemoveAt(0);
				if (!fVRPooledAudioSource.gameObject.activeSelf)
				{
					fVRPooledAudioSource.gameObject.SetActive(value: true);
				}
				fVRPooledAudioSource.Play(clipSet, pos, clipSet.PitchRange, clipSet.VolumeRange, mixerOverride);
				ActiveSources.Add(fVRPooledAudioSource);
				return fVRPooledAudioSource;
			}

			public FVRPooledAudioSource PlayClipPitchOverride(AudioEvent clipSet, Vector3 pos, Vector2 pitchOverride, AudioMixerGroup mixerOverride = null)
			{
				if (clipSet.Clips.Count <= 0)
				{
					return null;
				}
				if (SourceQueue_Disabled.Count > 0)
				{
					return DequeueAndPlay(clipSet, pos, pitchOverride, clipSet.VolumeRange, mixerOverride);
				}
				if (m_curSize < m_maxSize)
				{
					GameObject prefabForType = GetPrefabForType(Type);
					InstantiateAndEnqueue(prefabForType, active: true);
					return DequeueAndPlay(clipSet, pos, pitchOverride, clipSet.VolumeRange, mixerOverride);
				}
				FVRPooledAudioSource fVRPooledAudioSource = ActiveSources[0];
				ActiveSources.RemoveAt(0);
				if (!fVRPooledAudioSource.gameObject.activeSelf)
				{
					fVRPooledAudioSource.gameObject.SetActive(value: true);
				}
				fVRPooledAudioSource.Play(clipSet, pos, pitchOverride, clipSet.VolumeRange, mixerOverride);
				ActiveSources.Add(fVRPooledAudioSource);
				return fVRPooledAudioSource;
			}

			public FVRPooledAudioSource PlayClipVolumePitchOverride(AudioEvent clipSet, Vector3 pos, Vector2 volumeOverride, Vector2 pitchOverride, AudioMixerGroup mixerOverride = null)
			{
				if (clipSet.Clips.Count <= 0)
				{
					return null;
				}
				if (SourceQueue_Disabled.Count > 0)
				{
					return DequeueAndPlay(clipSet, pos, pitchOverride, volumeOverride, mixerOverride);
				}
				if (m_curSize < m_maxSize)
				{
					GameObject prefabForType = GetPrefabForType(Type);
					InstantiateAndEnqueue(prefabForType, active: true);
					return DequeueAndPlay(clipSet, pos, pitchOverride, volumeOverride, mixerOverride);
				}
				FVRPooledAudioSource fVRPooledAudioSource = ActiveSources[0];
				ActiveSources.RemoveAt(0);
				if (!fVRPooledAudioSource.gameObject.activeSelf)
				{
					fVRPooledAudioSource.gameObject.SetActive(value: true);
				}
				fVRPooledAudioSource.Play(clipSet, pos, pitchOverride, volumeOverride, mixerOverride);
				ActiveSources.Add(fVRPooledAudioSource);
				return fVRPooledAudioSource;
			}

			private FVRPooledAudioSource DequeueAndPlay(AudioEvent clipSet, Vector3 pos, Vector2 pitch, Vector2 volume, AudioMixerGroup mixerOverride = null)
			{
				FVRPooledAudioSource fVRPooledAudioSource = SourceQueue_Disabled.Dequeue();
				fVRPooledAudioSource.gameObject.SetActive(value: true);
				fVRPooledAudioSource.Play(clipSet, pos, pitch, volume, mixerOverride);
				ActiveSources.Add(fVRPooledAudioSource);
				return fVRPooledAudioSource;
			}

			public void Tick()
			{
				float deltaTime = Time.deltaTime;
				if (DelayedEvents.Count > 0)
				{
					for (int num = DelayedEvents.Count - 1; num >= 0; num--)
					{
						DelayedEvents[num].tickDown -= deltaTime;
						if (DelayedEvents[num].tickDown <= 0f)
						{
							PlayClipVolumePitchOverride(DelayedEvents[num].e, DelayedEvents[num].pos, DelayedEvents[num].vol, DelayedEvents[num].pitch, DelayedEvents[num].mixOverride);
							DelayedEvents[num].mixOverride = null;
							DelayedEvents[num].e = null;
							DelayedEvents.RemoveAt(num);
						}
					}
				}
				if (ActiveSources == null || ActiveSources.Count == 0)
				{
					return;
				}
				for (int num2 = ActiveSources.Count - 1; num2 >= 0; num2--)
				{
					if (ActiveSources[num2] == null)
					{
						ActiveSources.RemoveAt(num2);
					}
					else
					{
						FVRPooledAudioSource fVRPooledAudioSource = ActiveSources[num2];
						if (!fVRPooledAudioSource.Source.isPlaying)
						{
							ActiveSources.RemoveAt(num2);
							fVRPooledAudioSource.gameObject.SetActive(value: false);
							SourceQueue_Disabled.Enqueue(fVRPooledAudioSource);
						}
						else
						{
							fVRPooledAudioSource.Tick(deltaTime);
						}
					}
				}
			}

			public void Dispose()
			{
				for (int num = ActiveSources.Count - 1; num >= 0; num--)
				{
					if (ActiveSources[num] != null)
					{
						UnityEngine.Object.Destroy(ActiveSources[num].gameObject);
					}
				}
				ActiveSources.Clear();
				ActiveSources = null;
				while (SourceQueue_Disabled.Count > 0)
				{
					FVRPooledAudioSource fVRPooledAudioSource = SourceQueue_Disabled.Dequeue();
					if (fVRPooledAudioSource != null)
					{
						UnityEngine.Object.Destroy(fVRPooledAudioSource.gameObject);
					}
				}
				SourceQueue_Disabled.Clear();
				SourceQueue_Disabled = null;
				DelayedEvents.Clear();
				DelayedEvents = null;
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

			public float ReverbDelay = 0.011f;

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
				m_reverbFromProfile = p;
				Environment = p.Settings.Environment;
				Room = p.Settings.Room;
				RoomHF = p.Settings.RoomHF;
				RoomLF = p.Settings.RoomLF;
				DecayTime = p.Settings.DecayTime;
				DecayHFRatio = p.Settings.DecayHFRatio;
				Reflections = p.Settings.Reflections;
				ReflectionsDelay = p.Settings.ReflectionsDelay;
				Reverb = p.Settings.Reverb;
				ReverbDelay = p.Settings.ReverbDelay;
				HFReference = p.Settings.HFReference;
				LFReference = p.Settings.LFReference;
				Diffusion = p.Settings.Diffusion;
				Density = p.Settings.Density;
				UpdateMixerParams();
			}

			public void TransitionTo(FVRReverbSettingProfile newProfile, float Speed)
			{
				if (Environment != newProfile.Settings.Environment)
				{
					m_reverbToProfile = newProfile;
					m_reverbTransitionSpeed = Speed;
					m_reverbTransitionTick = 0f;
					m_isTransitioningToReverbEnvironment = true;
				}
			}

			public void Tick(float t)
			{
				if (m_isTransitioningToReverbEnvironment)
				{
					m_reverbTransitionTick += t * m_reverbTransitionSpeed;
					Room = Mathf.Lerp(m_reverbFromProfile.Settings.Room, m_reverbToProfile.Settings.Room, m_reverbTransitionTick);
					RoomHF = Mathf.Lerp(m_reverbFromProfile.Settings.RoomHF, m_reverbToProfile.Settings.RoomHF, m_reverbTransitionTick);
					RoomLF = Mathf.Lerp(m_reverbFromProfile.Settings.RoomLF, m_reverbToProfile.Settings.RoomLF, m_reverbTransitionTick);
					DecayTime = Mathf.Lerp(m_reverbFromProfile.Settings.DecayTime, m_reverbToProfile.Settings.DecayTime, m_reverbTransitionTick);
					DecayHFRatio = Mathf.Lerp(m_reverbFromProfile.Settings.DecayHFRatio, m_reverbToProfile.Settings.DecayHFRatio, m_reverbTransitionTick);
					Reflections = Mathf.Lerp(m_reverbFromProfile.Settings.Reflections, m_reverbToProfile.Settings.Reflections, m_reverbTransitionTick);
					ReflectionsDelay = Mathf.Lerp(m_reverbFromProfile.Settings.ReflectionsDelay, m_reverbToProfile.Settings.ReflectionsDelay, m_reverbTransitionTick);
					Reverb = Mathf.Lerp(m_reverbFromProfile.Settings.Reverb, m_reverbToProfile.Settings.Reverb, m_reverbTransitionTick);
					ReverbDelay = Mathf.Lerp(m_reverbFromProfile.Settings.ReverbDelay, m_reverbToProfile.Settings.ReverbDelay, m_reverbTransitionTick);
					HFReference = Mathf.Lerp(m_reverbFromProfile.Settings.HFReference, m_reverbToProfile.Settings.HFReference, m_reverbTransitionTick);
					LFReference = Mathf.Lerp(m_reverbFromProfile.Settings.LFReference, m_reverbToProfile.Settings.LFReference, m_reverbTransitionTick);
					Diffusion = Mathf.Lerp(m_reverbFromProfile.Settings.Diffusion, m_reverbToProfile.Settings.Diffusion, m_reverbTransitionTick);
					Density = Mathf.Lerp(m_reverbFromProfile.Settings.Density, m_reverbToProfile.Settings.Density, m_reverbTransitionTick);
					if (m_reverbTransitionTick >= 1f)
					{
						m_reverbTransitionTick = 0f;
						m_isTransitioningToReverbEnvironment = false;
						m_reverbFromProfile = m_reverbToProfile;
					}
					UpdateMixerParams();
				}
			}

			private void UpdateMixerParams()
			{
				MasterMixer.SetFloat("MReverb_Room", Room);
				MasterMixer.SetFloat("MReverb_RoomHF", RoomHF);
				MasterMixer.SetFloat("MReverb_RoomLF", RoomLF);
				MasterMixer.SetFloat("MReverb_DecayTime", DecayTime);
				MasterMixer.SetFloat("MReverb_DecayHFRatio", DecayHFRatio);
				MasterMixer.SetFloat("MReverb_Reflections", Reflections);
				MasterMixer.SetFloat("MReverb_ReflectionsDelay", ReflectionsDelay);
				MasterMixer.SetFloat("MReverb_Reverb", Reverb);
				MasterMixer.SetFloat("MReverb_ReverbDelay", ReverbDelay);
				MasterMixer.SetFloat("MReverb_HFReference", HFReference);
				MasterMixer.SetFloat("MReverb_LFReference", LFReference);
				MasterMixer.SetFloat("MReverb_Diffusion", Diffusion);
				MasterMixer.SetFloat("MReverb_Density", Density);
			}
		}

		public class ProgressiveOcclusionSampler
		{
			public Transform Root;
		}

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

		private List<AudioSourcePool> m_activePools = new List<AudioSourcePool>();

		private AudioSourcePool m_pool_generic;

		private AudioSourcePool m_pool_generic_close;

		private AudioSourcePool m_pool_generic_long;

		private AudioSourcePool m_pool_generic_verylong;

		private AudioSourcePool m_pool_explosion;

		private AudioSourcePool m_pool_explosionTail;

		private AudioSourcePool m_pool_UIChirp;

		private AudioSourcePool m_pool_NPCShotNear;

		private AudioSourcePool m_pool_NPCShotFarDistant;

		private AudioSourcePool m_pool_NPCBarks;

		private AudioSourcePool m_pool_NPCHandling;

		private AudioSourcePool m_pool_casings;

		private AudioSourcePool m_pool_impacts;

		public AudioMixer MasterMixer;

		private ReverbSettings m_reverbSettings = new ReverbSettings();

		private int m_numImpactSoundsThisFrame;

		public static FVRReverbSystem ReverbSystem
		{
			get
			{
				return ManagerSingleton<SM>.Instance.reverbSystem;
			}
			set
			{
				ManagerSingleton<SM>.Instance.reverbSystem = value;
			}
		}

		public static List<AudioSourcePool> ActivePools => ManagerSingleton<SM>.Instance.m_activePools;

		protected override void Awake()
		{
			base.Awake();
			generatePoolTypePrefabBindingDic();
			generateTailsDictionary();
			generateReverbDictionary();
			generateImpactDictionary();
			generateHandlingDictionaries();
			ManagerSingleton<SM>.Instance.m_reverbSettings.MasterMixer = ManagerSingleton<SM>.Instance.MasterMixer;
		}

		public static void SetReverbEnvironment(FVRSoundEnvironment e)
		{
			ManagerSingleton<SM>.Instance.m_reverbSettings.Set(GetReverbSettingProfile(e));
		}

		public static bool DoesReverbSystemExist()
		{
			if (ReverbSystem == null)
			{
				return false;
			}
			return true;
		}

		public static FVRSoundEnvironment GetSoundEnvironment(Vector3 pos)
		{
			if (ReverbSystem == null)
			{
				return GM.CurrentSceneSettings.DefaultSoundEnvironment;
			}
			return ReverbSystem.GetSoundEnvironment(pos).Environment;
		}

		public static FVRReverbEnvironment GetReverbEnvironment(Vector3 pos)
		{
			if (ReverbSystem == null)
			{
				return null;
			}
			return ReverbSystem.GetSoundEnvironment(pos);
		}

		public static FVRReverbEnvironment GetPlayerReverbEnvironment()
		{
			if (DoesReverbSystemExist())
			{
				return ReverbSystem.CurrentReverbEnvironment;
			}
			return null;
		}

		public static void TransitionToReverbEnvironment(FVRSoundEnvironment e, float s)
		{
			ManagerSingleton<SM>.Instance.m_reverbSettings.TransitionTo(GetReverbSettingProfile(e), s);
		}

		public static void WarmupGenericPools()
		{
			ManagerSingleton<SM>.Instance.m_pool_generic = CreatePool(3, 12, FVRPooledAudioType.Generic);
			ManagerSingleton<SM>.Instance.m_pool_generic_close = CreatePool(1, 6, FVRPooledAudioType.GenericClose);
			ManagerSingleton<SM>.Instance.m_pool_generic_long = CreatePool(1, 6, FVRPooledAudioType.GenericLongRange);
			ManagerSingleton<SM>.Instance.m_pool_generic_verylong = CreatePool(1, 6, FVRPooledAudioType.GenericVeryLongRange);
			ManagerSingleton<SM>.Instance.m_pool_explosion = CreatePool(1, 6, FVRPooledAudioType.Explosion);
			ManagerSingleton<SM>.Instance.m_pool_explosionTail = CreatePool(1, 6, FVRPooledAudioType.ExplosionTail);
			ManagerSingleton<SM>.Instance.m_pool_UIChirp = CreatePool(1, 6, FVRPooledAudioType.UIChirp);
			ManagerSingleton<SM>.Instance.m_pool_NPCShotNear = CreatePool(1, 6, FVRPooledAudioType.NPCShotNear);
			ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant = CreatePool(1, 6, FVRPooledAudioType.NPCShotFarDistant);
			ManagerSingleton<SM>.Instance.m_pool_NPCHandling = CreatePool(1, 3, FVRPooledAudioType.NPCHandling);
			ManagerSingleton<SM>.Instance.m_pool_NPCBarks = CreatePool(1, 6, FVRPooledAudioType.NPCBarks);
			ManagerSingleton<SM>.Instance.m_pool_casings = CreatePool(1, 8, FVRPooledAudioType.Casings);
			ManagerSingleton<SM>.Instance.m_pool_impacts = CreatePool(1, 6, FVRPooledAudioType.Impacts);
		}

		public static void ClearGenericPools()
		{
			ManagerSingleton<SM>.Instance.m_pool_generic.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_generic = null;
			ManagerSingleton<SM>.Instance.m_pool_generic_close.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_generic_close = null;
			ManagerSingleton<SM>.Instance.m_pool_generic_long.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_generic_long = null;
			ManagerSingleton<SM>.Instance.m_pool_generic_verylong.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_generic_verylong = null;
			ManagerSingleton<SM>.Instance.m_pool_explosion.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_explosion = null;
			ManagerSingleton<SM>.Instance.m_pool_explosionTail.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_explosionTail = null;
			ManagerSingleton<SM>.Instance.m_pool_UIChirp.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_UIChirp = null;
			ManagerSingleton<SM>.Instance.m_pool_NPCShotNear.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_NPCShotNear = null;
			ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant = null;
			ManagerSingleton<SM>.Instance.m_pool_NPCHandling.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_NPCHandling = null;
			ManagerSingleton<SM>.Instance.m_pool_NPCBarks.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_NPCBarks = null;
			ManagerSingleton<SM>.Instance.m_pool_casings.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_casings = null;
			ManagerSingleton<SM>.Instance.m_pool_impacts.Dispose();
			ManagerSingleton<SM>.Instance.m_pool_impacts = null;
			ActivePools.Clear();
		}

		public static void PlayGenericSound(AudioEvent ClipSet, Vector3 pos)
		{
			ManagerSingleton<SM>.Instance.m_pool_generic.PlayClip(ClipSet, pos);
		}

		public static FVRPooledAudioSource PlayCoreSound(FVRPooledAudioType type, AudioEvent ClipSet, Vector3 pos)
		{
			return type switch
			{
				FVRPooledAudioType.Generic => ManagerSingleton<SM>.Instance.m_pool_generic.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.Explosion => ManagerSingleton<SM>.Instance.m_pool_explosion.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.ExplosionTail => ManagerSingleton<SM>.Instance.m_pool_explosionTail.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.GenericClose => ManagerSingleton<SM>.Instance.m_pool_generic_close.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.GenericLongRange => ManagerSingleton<SM>.Instance.m_pool_generic_long.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.GenericVeryLongRange => ManagerSingleton<SM>.Instance.m_pool_generic_verylong.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.UIChirp => ManagerSingleton<SM>.Instance.m_pool_UIChirp.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.NPCShotNear => ManagerSingleton<SM>.Instance.m_pool_NPCShotNear.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.NPCShotFarDistant => ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.NPCHandling => ManagerSingleton<SM>.Instance.m_pool_NPCHandling.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.NPCBarks => ManagerSingleton<SM>.Instance.m_pool_NPCBarks.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.Casings => ManagerSingleton<SM>.Instance.m_pool_casings.PlayClip(ClipSet, pos), 
				FVRPooledAudioType.Impacts => ManagerSingleton<SM>.Instance.m_pool_impacts.PlayClip(ClipSet, pos), 
				_ => null, 
			};
		}

		public static void PlayCoreSoundOverrides(FVRPooledAudioType type, AudioEvent ClipSet, Vector3 pos, Vector2 volMult, Vector2 pitchMult)
		{
			switch (type)
			{
			case FVRPooledAudioType.Generic:
				ManagerSingleton<SM>.Instance.m_pool_generic.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
				break;
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
			case FVRPooledAudioType.UIChirp:
				ManagerSingleton<SM>.Instance.m_pool_UIChirp.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
				break;
			case FVRPooledAudioType.NPCShotNear:
				ManagerSingleton<SM>.Instance.m_pool_NPCShotNear.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
				break;
			case FVRPooledAudioType.NPCShotFarDistant:
				ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
				break;
			case FVRPooledAudioType.NPCHandling:
				ManagerSingleton<SM>.Instance.m_pool_NPCHandling.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
				break;
			case FVRPooledAudioType.NPCBarks:
				ManagerSingleton<SM>.Instance.m_pool_NPCBarks.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
				break;
			case FVRPooledAudioType.Casings:
				ManagerSingleton<SM>.Instance.m_pool_casings.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
				break;
			case FVRPooledAudioType.Impacts:
				ManagerSingleton<SM>.Instance.m_pool_impacts.PlayClipVolumePitchOverride(ClipSet, pos, volMult, pitchMult);
				break;
			}
		}

		public static void PlayCoreSoundDelayed(FVRPooledAudioType type, AudioEvent ClipSet, Vector3 pos, float delay)
		{
			switch (type)
			{
			case FVRPooledAudioType.Generic:
				ManagerSingleton<SM>.Instance.m_pool_generic.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
				break;
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
			case FVRPooledAudioType.UIChirp:
				ManagerSingleton<SM>.Instance.m_pool_UIChirp.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
				break;
			case FVRPooledAudioType.NPCShotNear:
				ManagerSingleton<SM>.Instance.m_pool_NPCShotNear.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
				break;
			case FVRPooledAudioType.NPCShotFarDistant:
				ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
				break;
			case FVRPooledAudioType.NPCHandling:
				ManagerSingleton<SM>.Instance.m_pool_NPCHandling.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
				break;
			case FVRPooledAudioType.NPCBarks:
				ManagerSingleton<SM>.Instance.m_pool_NPCBarks.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
				break;
			case FVRPooledAudioType.Casings:
				ManagerSingleton<SM>.Instance.m_pool_casings.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
				break;
			case FVRPooledAudioType.Impacts:
				ManagerSingleton<SM>.Instance.m_pool_impacts.PlayDelayedClip(delay, ClipSet, pos, ClipSet.VolumeRange, ClipSet.PitchRange);
				break;
			}
		}

		public static void PlayCoreSoundDelayedOverrides(FVRPooledAudioType type, AudioEvent ClipSet, Vector3 pos, Vector2 vol, Vector2 pitch, float delay)
		{
			switch (type)
			{
			case FVRPooledAudioType.Generic:
				ManagerSingleton<SM>.Instance.m_pool_generic.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
				break;
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
			case FVRPooledAudioType.UIChirp:
				ManagerSingleton<SM>.Instance.m_pool_UIChirp.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
				break;
			case FVRPooledAudioType.NPCShotNear:
				ManagerSingleton<SM>.Instance.m_pool_NPCShotNear.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
				break;
			case FVRPooledAudioType.NPCShotFarDistant:
				ManagerSingleton<SM>.Instance.m_pool_NPCShotFarDistant.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
				break;
			case FVRPooledAudioType.NPCHandling:
				ManagerSingleton<SM>.Instance.m_pool_NPCHandling.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
				break;
			case FVRPooledAudioType.NPCBarks:
				ManagerSingleton<SM>.Instance.m_pool_NPCBarks.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
				break;
			case FVRPooledAudioType.Casings:
				ManagerSingleton<SM>.Instance.m_pool_casings.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
				break;
			case FVRPooledAudioType.Impacts:
				ManagerSingleton<SM>.Instance.m_pool_impacts.PlayDelayedClip(delay, ClipSet, pos, vol, pitch);
				break;
			}
		}

		public static AudioSourcePool GetCorePool(FVRPooledAudioType type)
		{
			return type switch
			{
				FVRPooledAudioType.Generic => ManagerSingleton<SM>.Instance.m_pool_generic, 
				FVRPooledAudioType.Explosion => ManagerSingleton<SM>.Instance.m_pool_explosion, 
				FVRPooledAudioType.ExplosionTail => ManagerSingleton<SM>.Instance.m_pool_explosionTail, 
				FVRPooledAudioType.GenericClose => ManagerSingleton<SM>.Instance.m_pool_generic_close, 
				FVRPooledAudioType.GenericLongRange => ManagerSingleton<SM>.Instance.m_pool_generic_long, 
				FVRPooledAudioType.GenericVeryLongRange => ManagerSingleton<SM>.Instance.m_pool_generic_verylong, 
				FVRPooledAudioType.UIChirp => ManagerSingleton<SM>.Instance.m_pool_UIChirp, 
				FVRPooledAudioType.NPCHandling => ManagerSingleton<SM>.Instance.m_pool_NPCHandling, 
				FVRPooledAudioType.NPCBarks => ManagerSingleton<SM>.Instance.m_pool_NPCBarks, 
				FVRPooledAudioType.Casings => ManagerSingleton<SM>.Instance.m_pool_casings, 
				FVRPooledAudioType.Impacts => ManagerSingleton<SM>.Instance.m_pool_impacts, 
				_ => null, 
			};
		}

		private void generatePoolTypePrefabBindingDic()
		{
			for (int i = 0; i < PrefabDirectory.PrefabBindings.Count; i++)
			{
				m_prefabBindingDic.Add((int)PrefabDirectory.PrefabBindings[i].Type, PrefabDirectory.PrefabBindings[i]);
			}
		}

		private void generateTailsDictionary()
		{
			for (int i = 0; i < TailsDirectories.Length; i++)
			{
				Dictionary<FVRSoundEnvironment, AudioEvent> dictionary = null;
				dictionary = ((!m_tailsDic.ContainsKey(TailsDirectories[i].SoundClass)) ? new Dictionary<FVRSoundEnvironment, AudioEvent>() : m_tailsDic[TailsDirectories[i].SoundClass]);
				for (int j = 0; j < TailsDirectories[i].SoundSets.Count; j++)
				{
					dictionary.Add(TailsDirectories[i].SoundSets[j].Environment, TailsDirectories[i].SoundSets[j].AudioEvent);
				}
				m_tailsDic.Add(TailsDirectories[i].SoundClass, dictionary);
			}
		}

		private void generateReverbDictionary()
		{
			for (int i = 0; i < ReverbSettingProfiles.Length; i++)
			{
				m_reverbDic.Add(ReverbSettingProfiles[i].Settings.Environment, ReverbSettingProfiles[i]);
			}
		}

		private void generateImpactDictionary()
		{
			for (int i = 0; i < AudioImpactSets.Length; i++)
			{
				AudioImpactSet audioImpactSet = AudioImpactSets[i];
				Dictionary<MatSoundType, Dictionary<AudioImpactIntensity, AudioEvent>> dictionary = new Dictionary<MatSoundType, Dictionary<AudioImpactIntensity, AudioEvent>>();
				Dictionary<AudioImpactIntensity, AudioEvent> value = GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Carpet, audioImpactSet.PitchRange);
				Dictionary<AudioImpactIntensity, AudioEvent> value2 = GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.HardSurface, audioImpactSet.PitchRange);
				Dictionary<AudioImpactIntensity, AudioEvent> value3 = GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.LooseSurface, audioImpactSet.PitchRange);
				Dictionary<AudioImpactIntensity, AudioEvent> value4 = GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Meat, audioImpactSet.PitchRange);
				Dictionary<AudioImpactIntensity, AudioEvent> value5 = GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Metal, audioImpactSet.PitchRange);
				Dictionary<AudioImpactIntensity, AudioEvent> value6 = GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Plastic, audioImpactSet.PitchRange);
				Dictionary<AudioImpactIntensity, AudioEvent> value7 = GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.SoftSurface, audioImpactSet.PitchRange);
				Dictionary<AudioImpactIntensity, AudioEvent> value8 = GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Tile, audioImpactSet.PitchRange);
				Dictionary<AudioImpactIntensity, AudioEvent> value9 = GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Water, audioImpactSet.PitchRange);
				Dictionary<AudioImpactIntensity, AudioEvent> value10 = GenerateAudioEventDicFromImpactMaterialGroup(audioImpactSet.Wood, audioImpactSet.PitchRange);
				dictionary.Add(MatSoundType.Carpet, value);
				dictionary.Add(MatSoundType.HardSurface, value2);
				dictionary.Add(MatSoundType.LooseSurface, value3);
				dictionary.Add(MatSoundType.Meat, value4);
				dictionary.Add(MatSoundType.Metal, value5);
				dictionary.Add(MatSoundType.Plastic, value6);
				dictionary.Add(MatSoundType.SoftSurface, value7);
				dictionary.Add(MatSoundType.Tile, value8);
				dictionary.Add(MatSoundType.Water, value9);
				dictionary.Add(MatSoundType.Wood, value10);
				m_impactDic.Add(audioImpactSet.ImpactType, dictionary);
			}
		}

		private void generateHandlingDictionaries()
		{
			HandlingGrabSet[] array = Resources.LoadAll<HandlingGrabSet>("HandlingAudioDefs/Grab");
			HandlingReleaseSet[] array2 = Resources.LoadAll<HandlingReleaseSet>("HandlingAudioDefs/Release");
			HandlingReleaseIntoSlotSet[] array3 = Resources.LoadAll<HandlingReleaseIntoSlotSet>("HandlingAudioDefs/ReleaseIntoSlot");
			for (int i = 0; i < array.Length; i++)
			{
				m_handlingGrabDic.Add(array[i].Type, array[i]);
			}
			for (int j = 0; j < array2.Length; j++)
			{
				m_handlingReleaseDic.Add(array2[j].Type, array2[j]);
			}
			for (int k = 0; k < array3.Length; k++)
			{
				m_handlingReleaseIntoSlotDic.Add(array3[k].Type, array3[k]);
			}
			for (int l = 0; l < AudioBulletImpactSets.Length; l++)
			{
				m_bulletHitDic.Add(AudioBulletImpactSets[l].Type, AudioBulletImpactSets[l]);
			}
		}

		private Dictionary<AudioImpactIntensity, AudioEvent> GenerateAudioEventDicFromImpactMaterialGroup(AudioImpactMaterialGroup g, Vector2 pitchRange)
		{
			Dictionary<AudioImpactIntensity, AudioEvent> dictionary = new Dictionary<AudioImpactIntensity, AudioEvent>();
			AudioEvent audioEvent = new AudioEvent();
			AudioEvent audioEvent2 = new AudioEvent();
			AudioEvent audioEvent3 = new AudioEvent();
			audioEvent.VolumeRange = new Vector2(0.35f * g.Volumes.x, 0.4f * g.Volumes.x);
			audioEvent2.VolumeRange = new Vector2(0.35f * g.Volumes.y, 0.4f * g.Volumes.y);
			audioEvent3.VolumeRange = new Vector2(0.35f * g.Volumes.z, 0.4f * g.Volumes.z);
			audioEvent.PitchRange = pitchRange;
			audioEvent2.PitchRange = pitchRange;
			audioEvent3.PitchRange = pitchRange;
			for (int i = 0; i < g.Clips_Light.Count; i++)
			{
				audioEvent.Clips.Add(g.Clips_Light[i]);
			}
			for (int j = 0; j < g.Clips_Medium.Count; j++)
			{
				audioEvent2.Clips.Add(g.Clips_Medium[j]);
			}
			for (int k = 0; k < g.Clips_Hard.Count; k++)
			{
				audioEvent3.Clips.Add(g.Clips_Hard[k]);
			}
			audioEvent.SetLengthRange();
			audioEvent2.SetLengthRange();
			audioEvent3.SetLengthRange();
			dictionary.Add(AudioImpactIntensity.Light, audioEvent);
			dictionary.Add(AudioImpactIntensity.Medium, audioEvent2);
			dictionary.Add(AudioImpactIntensity.Hard, audioEvent3);
			return dictionary;
		}

		public static AudioEvent GetImpactAudioEvent(ImpactType impacttype, MatSoundType mat, AudioImpactIntensity impactIntensity)
		{
			if (ManagerSingleton<SM>.Instance.m_impactDic.ContainsKey(impacttype))
			{
				return ManagerSingleton<SM>.Instance.m_impactDic[impacttype][mat][impactIntensity];
			}
			return ManagerSingleton<SM>.Instance.m_impactDic[ImpactType.Generic][mat][impactIntensity];
		}

		public static AudioEvent GetButtletImpactAudioEvent(BulletImpactSoundType t)
		{
			return ManagerSingleton<SM>.Instance.m_bulletHitDic[t].AudEvent_Set;
		}

		public static void PlayHandlingGrabSound(HandlingGrabType t, Vector3 pos, bool isHard)
		{
			if (ManagerSingleton<SM>.Instance.m_handlingGrabDic.ContainsKey(t))
			{
				AudioEvent audioEvent = null;
				audioEvent = ((!isHard) ? ManagerSingleton<SM>.Instance.m_handlingGrabDic[t].GrabSet_Light : ManagerSingleton<SM>.Instance.m_handlingGrabDic[t].GrabSet_Hard);
				PlayCoreSound(FVRPooledAudioType.GenericClose, audioEvent, pos);
			}
		}

		public static void PlayHandlingReleaseSound(HandlingReleaseType t, Vector3 pos)
		{
			if (ManagerSingleton<SM>.Instance.m_handlingReleaseDic.ContainsKey(t))
			{
				AudioEvent audioEvent = null;
				audioEvent = ManagerSingleton<SM>.Instance.m_handlingReleaseDic[t].ReleaseSet;
				PlayCoreSound(FVRPooledAudioType.GenericClose, audioEvent, pos);
			}
		}

		public static void PlayHandlingReleaseIntoSlotSound(HandlingReleaseIntoSlotType t, Vector3 pos)
		{
			if (ManagerSingleton<SM>.Instance.m_handlingReleaseIntoSlotDic.ContainsKey(t))
			{
				AudioEvent audioEvent = null;
				audioEvent = ManagerSingleton<SM>.Instance.m_handlingReleaseIntoSlotDic[t].ReleaseIntoSlotSet;
				PlayCoreSound(FVRPooledAudioType.GenericClose, audioEvent, pos);
			}
		}

		public static float PlayImpactSound(ImpactType impacttype, MatSoundType mat, AudioImpactIntensity impactIntensity, Vector3 pos, FVRPooledAudioType pool, float distanceLimit)
		{
			if (ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame > 2)
			{
				return -1f;
			}
			float num = Vector3.Distance(pos, GM.CurrentPlayerBody.Head.position);
			if (num > distanceLimit)
			{
				return 0f;
			}
			AudioEvent impactAudioEvent = GetImpactAudioEvent(impacttype, mat, impactIntensity);
			PlayCoreSound(pool, impactAudioEvent, pos);
			ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame++;
			return impactAudioEvent.ClipLengthRange.y;
		}

		public static float PlayImpactSound(ImpactType impacttype, MatSoundType mat, AudioImpactIntensity impactIntensity, Vector3 pos, FVRPooledAudioType pool, float distanceLimit, float volumeMult, float pitchMult)
		{
			if (ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame > 2)
			{
				return 0f;
			}
			float num = Vector3.Distance(pos, GM.CurrentPlayerBody.Head.position);
			if (num > distanceLimit)
			{
				return 0f;
			}
			AudioEvent impactAudioEvent = GetImpactAudioEvent(impacttype, mat, impactIntensity);
			PlayCoreSoundOverrides(pool, impactAudioEvent, pos, impactAudioEvent.VolumeRange * volumeMult, impactAudioEvent.PitchRange * pitchMult);
			ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame++;
			return impactAudioEvent.ClipLengthRange.y;
		}

		public static float PlayBulletImpactHit(BulletImpactSoundType type, Vector3 pos, float distanceLimit, float volumeMult, float pitchmult)
		{
			if (ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame > 2)
			{
				return 0f;
			}
			if (type == BulletImpactSoundType.None)
			{
				return 0f;
			}
			float num = Vector3.Distance(pos, GM.CurrentPlayerBody.Head.position);
			if (num > distanceLimit)
			{
				return 0f;
			}
			AudioEvent buttletImpactAudioEvent = GetButtletImpactAudioEvent(type);
			PlayCoreSoundOverrides(FVRPooledAudioType.Impacts, buttletImpactAudioEvent, pos, buttletImpactAudioEvent.VolumeRange * volumeMult, buttletImpactAudioEvent.PitchRange * pitchmult);
			ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame++;
			return buttletImpactAudioEvent.ClipLengthRange.y;
		}

		public static float PlayBulletImpactHit(BulletImpactSoundType type, Vector3 pos, float volumeMult, float pitchmult)
		{
			if (ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame > 2)
			{
				return 0f;
			}
			if (type == BulletImpactSoundType.None)
			{
				return 0f;
			}
			AudioEvent buttletImpactAudioEvent = GetButtletImpactAudioEvent(type);
			PlayCoreSoundOverrides(FVRPooledAudioType.Impacts, buttletImpactAudioEvent, pos, buttletImpactAudioEvent.VolumeRange * volumeMult, buttletImpactAudioEvent.PitchRange * pitchmult);
			ManagerSingleton<SM>.Instance.m_numImpactSoundsThisFrame++;
			return buttletImpactAudioEvent.ClipLengthRange.y;
		}

		public static GameObject GetPrefabForType(FVRPooledAudioType type)
		{
			return ManagerSingleton<SM>.Instance.m_prefabBindingDic[(int)type].Prefab;
		}

		public static AudioEvent GetTailSet(FVRTailSoundClass tClass, FVRSoundEnvironment tEnvironment)
		{
			return ManagerSingleton<SM>.Instance.m_tailsDic[tClass][tEnvironment];
		}

		public static float GetLowPassOcclusionValue(Vector3 start, Vector3 end)
		{
			if (Physics.Linecast(start, end, ManagerSingleton<SM>.Instance.OcclusionLM, QueryTriggerInteraction.Ignore))
			{
				float time = Vector3.Distance(start, end);
				return ManagerSingleton<SM>.Instance.OcclusionFactorCurve.Evaluate(time);
			}
			return 22000f;
		}

		public static float GetOcclusionFactor(Vector3 start, Vector3 end)
		{
			if (Physics.Linecast(start, end, ManagerSingleton<SM>.Instance.OcclusionLM, QueryTriggerInteraction.Ignore))
			{
				float value = Vector3.Distance(start, end);
				float f = Mathf.InverseLerp(1f, 1000f, value);
				return Mathf.Pow(f, 0.1f);
			}
			return 0f;
		}

		public static FVRReverbSettingProfile GetReverbSettingProfile(FVRSoundEnvironment rEnvironment)
		{
			return ManagerSingleton<SM>.Instance.m_reverbDic[rEnvironment];
		}

		public static AudioSourcePool CreatePool(int initSize, int maxSize, FVRPooledAudioType type)
		{
			AudioSourcePool audioSourcePool = new AudioSourcePool(initSize, maxSize, type);
			ActivePools.Add(audioSourcePool);
			return audioSourcePool;
		}

		public static float GetSoundTravelDistanceMultByEnvironment(FVRSoundEnvironment se)
		{
			return se switch
			{
				FVRSoundEnvironment.Forest => 2f, 
				FVRSoundEnvironment.InsideNarrow => 0.75f, 
				FVRSoundEnvironment.InsideSmall => 0.7f, 
				FVRSoundEnvironment.InsideWarehouse => 1f, 
				FVRSoundEnvironment.InsideNarrowSmall => 0.7f, 
				FVRSoundEnvironment.InsideLarge => 1f, 
				FVRSoundEnvironment.InsideWarehouseSmall => 1f, 
				FVRSoundEnvironment.InsideMedium => 1f, 
				FVRSoundEnvironment.InsideLargeHighCeiling => 1f, 
				FVRSoundEnvironment.OutsideOpen => 2.5f, 
				FVRSoundEnvironment.OutsideEnclosed => 2f, 
				FVRSoundEnvironment.OutsideEnclosedNarrow => 1.75f, 
				FVRSoundEnvironment.SniperRange => 1f, 
				FVRSoundEnvironment.ShootingRange => 1f, 
				_ => 1f, 
			};
		}

		public static float GetImpactSoundVolumeMultFromMaterial(MatSoundType m)
		{
			return m switch
			{
				MatSoundType.Carpet => 0.3f, 
				MatSoundType.HardSurface => 1f, 
				MatSoundType.LooseSurface => 0.8f, 
				MatSoundType.Meat => 0.5f, 
				MatSoundType.Metal => 1.8f, 
				MatSoundType.Plastic => 0.8f, 
				MatSoundType.SoftSurface => 0.4f, 
				MatSoundType.Tile => 1.2f, 
				MatSoundType.Water => 1.1f, 
				MatSoundType.Wood => 0.8f, 
				_ => 1f, 
			};
		}

		public void Update()
		{
			for (int i = 0; i < m_activePools.Count; i++)
			{
				m_activePools[i].Tick();
			}
			ManagerSingleton<SM>.Instance.m_reverbSettings.Tick(Time.deltaTime);
			m_numImpactSoundsThisFrame = 0;
		}
	}
}
