using UnityEngine;
using UnityEngine.Audio;

namespace FistVR
{
	public class FVRPooledAudioSource : MonoBehaviour
	{
		public AudioSource Source;

		public AudioLowPassFilter LowPassFilter;

		private bool m_hasLowPass;

		private AudioMixerGroup m_originalMixer;

		public AnimationCurve OcclusionFactorCurve;

		public AnimationCurve OcclusionVolumeCurve;

		public LayerMask OcclusionLM;

		private bool m_hasFollowTrans;

		private Transform m_followTrans;

		private bool m_isBeingDestroyed;

		public void Awake()
		{
			m_originalMixer = Source.outputAudioMixerGroup;
			if (LowPassFilter != null)
			{
				m_hasLowPass = true;
			}
		}

		public void Play(AudioEvent audioEvent, Vector3 pos, Vector2 pitch, Vector2 volume, AudioMixerGroup mixerOverride = null)
		{
			if (Source.isPlaying)
			{
				Source.Stop();
			}
			m_hasFollowTrans = false;
			m_followTrans = null;
			base.transform.position = pos;
			Source.clip = audioEvent.Clips[Random.Range(0, audioEvent.Clips.Count)];
			Source.volume = Random.Range(volume.x, volume.y);
			Source.pitch = Random.Range(pitch.x, pitch.y);
			if (mixerOverride != null)
			{
				Source.outputAudioMixerGroup = mixerOverride;
			}
			else if (Source.outputAudioMixerGroup != m_originalMixer)
			{
				Source.outputAudioMixerGroup = m_originalMixer;
			}
			if (m_hasLowPass)
			{
				float lowPassOcclusionValue = GetLowPassOcclusionValue(base.transform.position, GM.CurrentPlayerBody.Head.position);
				LowPassFilter.cutoffFrequency = lowPassOcclusionValue;
			}
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			Source.enabled = true;
			if (!m_isBeingDestroyed)
			{
				Source.Play();
			}
		}

		public void Tick(float t)
		{
			if (m_hasFollowTrans)
			{
				if (m_followTrans == null)
				{
					m_hasFollowTrans = false;
				}
				else
				{
					base.transform.position = m_followTrans.position;
				}
			}
		}

		public void FollowThisTransform(Transform t)
		{
			m_followTrans = t;
			if (t != null)
			{
				m_hasFollowTrans = true;
			}
		}

		public void SetLowPassFreq(float freq)
		{
			if (m_hasLowPass)
			{
				LowPassFilter.cutoffFrequency = freq;
			}
		}

		private float GetLowPassOcclusionValue(Vector3 start, Vector3 end)
		{
			if (Physics.Linecast(start, end, OcclusionLM, QueryTriggerInteraction.Ignore))
			{
				float time = Vector3.Distance(start, end);
				Source.volume *= OcclusionVolumeCurve.Evaluate(time);
				return OcclusionFactorCurve.Evaluate(time);
			}
			return 22000f;
		}

		private void OnDestroy()
		{
			m_isBeingDestroyed = true;
		}
	}
}
