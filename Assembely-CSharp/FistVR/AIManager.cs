using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class AIManager : MonoBehaviour
	{
		public enum SpeakType
		{
			chat,
			pain,
			death
		}

		public class DelayedSonicEvent
		{
			public float tickDown;

			public AIEntity entity;

			public float danger;

			public Vector3 pos;

			public DelayedSonicEvent(float TickDown, AIEntity Entity, float Danger, Vector3 Pos)
			{
				tickDown = TickDown;
				entity = Entity;
				danger = Danger;
				pos = Pos;
			}
		}

		public LayerMask LM_Entity;

		public AnimationCurve LoudnessFalloff;

		public AnimationCurve SonicThresholdDecayCurve;

		private List<AIEntity> m_knownEntities = new List<AIEntity>();

		private int m_curCheckIndex;

		public int NumEntitiesToCheckPerFrame = 1;

		private List<DelayedSonicEvent> m_delayedEvents = new List<DelayedSonicEvent>();

		private float m_speakingBlockTick_Chat;

		private float m_speakingBlockTick_Pain;

		private float m_speakingBlockTick_Death;

		public float SpeakRang_Chat = 30f;

		public float SpeakRang_Pain = 12f;

		public float SpeakRang_Death = 50f;

		public AICoverPointManager CPM;

		private bool m_hasCPM;

		public bool HasInit;

		public bool HasCPM => m_hasCPM;

		private void Awake()
		{
			GM.CurrentAIManager = this;
		}

		private void OnDestroy()
		{
			GM.CurrentAIManager = null;
		}

		public void RegisterAIEntity(AIEntity e)
		{
			m_knownEntities.Add(e);
		}

		public void DeRegisterAIEntity(AIEntity e)
		{
			m_knownEntities.Remove(e);
		}

		public void SpeakBlock_Chat(float f)
		{
			m_speakingBlockTick_Chat = Random.Range(f * 1.01f, f * 1.025f);
		}

		public bool CanAgentSpeak_Chat()
		{
			if (m_speakingBlockTick_Chat <= 0f)
			{
				return true;
			}
			return false;
		}

		public void SpeakBlock_Pain(float f)
		{
			m_speakingBlockTick_Pain = Random.Range(f * 1.01f, f * 1.025f);
		}

		public bool CanAgentSpeak_Pain()
		{
			if (m_speakingBlockTick_Pain <= 0f)
			{
				return true;
			}
			return false;
		}

		public void SpeakBlock_Death(float f)
		{
			m_speakingBlockTick_Death = Random.Range(f * 1.01f, f * 1.025f);
		}

		public bool CanAgentSpeak_Death()
		{
			if (m_speakingBlockTick_Death <= 0f)
			{
				return true;
			}
			return false;
		}

		public void Start()
		{
			NavMesh.pathfindingIterationsPerFrame = 500;
			GM.CurrentSceneSettings.PerceiveableSoundEvent += SonicEvent;
			GM.CurrentSceneSettings.SuppressingEventEvent += SuppressionEvent;
			if (CPM != null)
			{
				m_hasCPM = true;
				CPM.Init();
			}
			HasInit = true;
		}

		private void OnDisable()
		{
			GM.CurrentSceneSettings.PerceiveableSoundEvent -= SonicEvent;
			GM.CurrentSceneSettings.SuppressingEventEvent -= SuppressionEvent;
		}

		public FVRPooledAudioSource Speak(AudioClip clip, float v, float p, Vector3 pos, SpeakType stype)
		{
			bool flag = false;
			if (stype == SpeakType.chat && CanAgentSpeak_Chat())
			{
				flag = true;
			}
			if (stype == SpeakType.pain && CanAgentSpeak_Pain())
			{
				flag = true;
			}
			if (stype == SpeakType.death && CanAgentSpeak_Death())
			{
				flag = true;
			}
			if (flag)
			{
				float num = Vector3.Distance(pos, GM.CurrentPlayerBody.Head.position);
				float num2 = 0f;
				switch (stype)
				{
				case SpeakType.chat:
					num2 = SpeakRang_Chat;
					break;
				case SpeakType.pain:
					num2 = SpeakRang_Pain;
					break;
				case SpeakType.death:
					num2 = SpeakRang_Death;
					break;
				}
				if (num < num2)
				{
					switch (stype)
					{
					case SpeakType.chat:
						SpeakBlock_Chat(clip.length);
						break;
					case SpeakType.pain:
						SpeakBlock_Pain(clip.length);
						break;
					case SpeakType.death:
						SpeakBlock_Death(clip.length);
						break;
					}
					AudioEvent audioEvent = new AudioEvent();
					audioEvent.Clips.Add(clip);
					audioEvent.VolumeRange = new Vector2(v, v * 1.02f);
					audioEvent.PitchRange = new Vector2(p, p * 1.02f);
					return SM.PlayCoreSound(FVRPooledAudioType.NPCBarks, audioEvent, pos);
				}
				return null;
			}
			return null;
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;
			if (m_speakingBlockTick_Chat > 0f)
			{
				m_speakingBlockTick_Chat -= deltaTime;
			}
			if (m_speakingBlockTick_Pain > 0f)
			{
				m_speakingBlockTick_Pain -= deltaTime;
			}
			if (m_speakingBlockTick_Death > 0f)
			{
				m_speakingBlockTick_Death -= deltaTime;
			}
			for (int num = m_knownEntities.Count - 1; num >= 0; num--)
			{
				if (m_knownEntities[num] == null)
				{
					m_knownEntities.RemoveAt(num);
				}
				else
				{
					m_knownEntities[num].Tick(deltaTime);
				}
			}
			for (int i = 0; i < NumEntitiesToCheckPerFrame; i++)
			{
				if (m_knownEntities.Count > 0)
				{
					if (m_curCheckIndex >= m_knownEntities.Count)
					{
						m_curCheckIndex = m_knownEntities.Count - 1;
					}
					if (m_knownEntities[m_curCheckIndex].ReadyForManagerCheck())
					{
						EntityCheck(m_knownEntities[m_curCheckIndex]);
					}
					m_curCheckIndex--;
					if (m_curCheckIndex < 0)
					{
						m_curCheckIndex = m_knownEntities.Count - 1;
					}
				}
			}
			if (m_delayedEvents.Count <= 0)
			{
				return;
			}
			for (int num2 = m_delayedEvents.Count - 1; num2 >= 0; num2--)
			{
				m_delayedEvents[num2].tickDown -= deltaTime;
				if (m_delayedEvents[num2].tickDown <= 0f && m_delayedEvents[num2].entity != null)
				{
					AIEvent e = new AIEvent(m_delayedEvents[num2].pos, AIEvent.AIEType.Sonic, m_delayedEvents[num2].danger);
					m_delayedEvents[num2].entity.OnAIEventReceive(e);
					m_delayedEvents.RemoveAt(num2);
				}
			}
		}

		private void EntityCheck(AIEntity e)
		{
			e.ResetTick();
			if (!e.ReceivesEvent_Visual)
			{
				return;
			}
			Vector3 pos = e.GetPos();
			Vector3 position = pos;
			Vector3 forward = e.SensoryFrame.forward;
			if (!e.IsVisualCheckOmni)
			{
				position += forward * e.MaximumSightRange;
			}
			Collider[] array = Physics.OverlapSphere(position, e.MaximumSightRange, LM_Entity, QueryTriggerInteraction.Collide);
			if (array.Length <= 0)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				AIEntity component = array[i].GetComponent<AIEntity>();
				if (component == null || component == e || component.IFFCode < -1)
				{
					continue;
				}
				Vector3 pos2 = component.GetPos();
				Vector3 to = pos2 - pos;
				float magnitude = to.magnitude;
				float num = e.MaximumSightRange;
				if (!(magnitude > component.MaxDistanceVisibleFrom) && !(component.VisibilityMultiplier > 2f))
				{
					magnitude = ((!(component.VisibilityMultiplier > 1f)) ? Mathf.Lerp(0f, magnitude, component.VisibilityMultiplier) : Mathf.Lerp(magnitude, num, component.VisibilityMultiplier - 1f));
					if (!e.IsVisualCheckOmni)
					{
						float num2 = Vector3.Angle(forward, to);
						num = e.MaximumSightRange * e.SightDistanceByFOVMultiplier.Evaluate(num2 / e.MaximumSightFOV);
					}
					if (!(magnitude > num) && !Physics.Linecast(pos, pos2, e.LM_VisualOcclusionCheck, QueryTriggerInteraction.Collide))
					{
						float v = magnitude / e.MaximumSightRange * component.DangerMultiplier;
						AIEvent e2 = new AIEvent(component, AIEvent.AIEType.Visual, v);
						e.OnAIEventReceive(e2);
					}
				}
			}
		}

		public void SonicEvent(float loudness, float maxDistanceHeard, Vector3 pos, int iffcode)
		{
			if (loudness < GM.CurrentSceneSettings.BaseLoudness || iffcode == -3)
			{
				return;
			}
			for (int i = 0; i < m_knownEntities.Count; i++)
			{
				AIEntity aIEntity = m_knownEntities[i];
				if (!aIEntity.ReceivesEvent_Sonic || aIEntity.SonicThreshold >= loudness || aIEntity.IFFCode == iffcode)
				{
					continue;
				}
				float num = Vector3.Distance(aIEntity.GetPos(), pos);
				if (!(num > aIEntity.MaxHearingDistance) && !(num > maxDistanceHeard))
				{
					float num2 = LoudnessFalloff.Evaluate(num) * loudness;
					if (!(aIEntity.SonicThreshold > num2))
					{
						float num3 = Mathf.Clamp(num / 50f, 0.1f, 1f);
						float num4 = 1f - num2 / 200f;
						float danger = num4 * num3;
						aIEntity.ProcessLoudness(num2);
						float tickDown = num / 343f;
						DelayedSonicEvent item = new DelayedSonicEvent(tickDown, aIEntity, danger, pos);
						m_delayedEvents.Add(item);
					}
				}
			}
		}

		public void SuppressionEvent(Vector3 pos, Vector3 dir, int iffcode, float intensity, float range)
		{
			for (int i = 0; i < m_knownEntities.Count; i++)
			{
				AIEntity aIEntity = m_knownEntities[i];
				if (aIEntity.ReceivesEvent_Sonic || aIEntity.ReceivesEvent_Visual)
				{
					aIEntity.OnAIReceiveSuppression(pos, dir, iffcode, intensity, range);
				}
			}
		}
	}
}
