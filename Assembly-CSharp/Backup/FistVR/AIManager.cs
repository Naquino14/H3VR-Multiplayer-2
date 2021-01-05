// Decompiled with JetBrains decompiler
// Type: FistVR.AIManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class AIManager : MonoBehaviour
  {
    public LayerMask LM_Entity;
    public AnimationCurve LoudnessFalloff;
    public AnimationCurve SonicThresholdDecayCurve;
    private List<AIEntity> m_knownEntities = new List<AIEntity>();
    private int m_curCheckIndex;
    public int NumEntitiesToCheckPerFrame = 1;
    private List<AIManager.DelayedSonicEvent> m_delayedEvents = new List<AIManager.DelayedSonicEvent>();
    private float m_speakingBlockTick_Chat;
    private float m_speakingBlockTick_Pain;
    private float m_speakingBlockTick_Death;
    public float SpeakRang_Chat = 30f;
    public float SpeakRang_Pain = 12f;
    public float SpeakRang_Death = 50f;
    public AICoverPointManager CPM;
    private bool m_hasCPM;
    public bool HasInit;

    private void Awake() => GM.CurrentAIManager = this;

    private void OnDestroy() => GM.CurrentAIManager = (AIManager) null;

    public void RegisterAIEntity(AIEntity e) => this.m_knownEntities.Add(e);

    public void DeRegisterAIEntity(AIEntity e) => this.m_knownEntities.Remove(e);

    public void SpeakBlock_Chat(float f) => this.m_speakingBlockTick_Chat = Random.Range(f * 1.01f, f * 1.025f);

    public bool CanAgentSpeak_Chat() => (double) this.m_speakingBlockTick_Chat <= 0.0;

    public void SpeakBlock_Pain(float f) => this.m_speakingBlockTick_Pain = Random.Range(f * 1.01f, f * 1.025f);

    public bool CanAgentSpeak_Pain() => (double) this.m_speakingBlockTick_Pain <= 0.0;

    public void SpeakBlock_Death(float f) => this.m_speakingBlockTick_Death = Random.Range(f * 1.01f, f * 1.025f);

    public bool CanAgentSpeak_Death() => (double) this.m_speakingBlockTick_Death <= 0.0;

    public bool HasCPM => this.m_hasCPM;

    public void Start()
    {
      NavMesh.pathfindingIterationsPerFrame = 500;
      GM.CurrentSceneSettings.PerceiveableSoundEvent += new FVRSceneSettings.PerceiveableSound(this.SonicEvent);
      GM.CurrentSceneSettings.SuppressingEventEvent += new FVRSceneSettings.SuppressingEvent(this.SuppressionEvent);
      if ((Object) this.CPM != (Object) null)
      {
        this.m_hasCPM = true;
        this.CPM.Init();
      }
      this.HasInit = true;
    }

    private void OnDisable()
    {
      GM.CurrentSceneSettings.PerceiveableSoundEvent -= new FVRSceneSettings.PerceiveableSound(this.SonicEvent);
      GM.CurrentSceneSettings.SuppressingEventEvent -= new FVRSceneSettings.SuppressingEvent(this.SuppressionEvent);
    }

    public FVRPooledAudioSource Speak(
      AudioClip clip,
      float v,
      float p,
      Vector3 pos,
      AIManager.SpeakType stype)
    {
      bool flag = false;
      if (stype == AIManager.SpeakType.chat && this.CanAgentSpeak_Chat())
        flag = true;
      if (stype == AIManager.SpeakType.pain && this.CanAgentSpeak_Pain())
        flag = true;
      if (stype == AIManager.SpeakType.death && this.CanAgentSpeak_Death())
        flag = true;
      if (!flag)
        return (FVRPooledAudioSource) null;
      float num1 = Vector3.Distance(pos, GM.CurrentPlayerBody.Head.position);
      float num2 = 0.0f;
      switch (stype)
      {
        case AIManager.SpeakType.chat:
          num2 = this.SpeakRang_Chat;
          break;
        case AIManager.SpeakType.pain:
          num2 = this.SpeakRang_Pain;
          break;
        case AIManager.SpeakType.death:
          num2 = this.SpeakRang_Death;
          break;
      }
      if ((double) num1 >= (double) num2)
        return (FVRPooledAudioSource) null;
      switch (stype)
      {
        case AIManager.SpeakType.chat:
          this.SpeakBlock_Chat(clip.length);
          break;
        case AIManager.SpeakType.pain:
          this.SpeakBlock_Pain(clip.length);
          break;
        case AIManager.SpeakType.death:
          this.SpeakBlock_Death(clip.length);
          break;
      }
      return SM.PlayCoreSound(FVRPooledAudioType.NPCBarks, new AudioEvent()
      {
        Clips = {
          clip
        },
        VolumeRange = new Vector2(v, v * 1.02f),
        PitchRange = new Vector2(p, p * 1.02f)
      }, pos);
    }

    private void Update()
    {
      float deltaTime = Time.deltaTime;
      if ((double) this.m_speakingBlockTick_Chat > 0.0)
        this.m_speakingBlockTick_Chat -= deltaTime;
      if ((double) this.m_speakingBlockTick_Pain > 0.0)
        this.m_speakingBlockTick_Pain -= deltaTime;
      if ((double) this.m_speakingBlockTick_Death > 0.0)
        this.m_speakingBlockTick_Death -= deltaTime;
      for (int index = this.m_knownEntities.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_knownEntities[index] == (Object) null)
          this.m_knownEntities.RemoveAt(index);
        else
          this.m_knownEntities[index].Tick(deltaTime);
      }
      for (int index = 0; index < this.NumEntitiesToCheckPerFrame; ++index)
      {
        if (this.m_knownEntities.Count > 0)
        {
          if (this.m_curCheckIndex >= this.m_knownEntities.Count)
            this.m_curCheckIndex = this.m_knownEntities.Count - 1;
          if (this.m_knownEntities[this.m_curCheckIndex].ReadyForManagerCheck())
            this.EntityCheck(this.m_knownEntities[this.m_curCheckIndex]);
          --this.m_curCheckIndex;
          if (this.m_curCheckIndex < 0)
            this.m_curCheckIndex = this.m_knownEntities.Count - 1;
        }
      }
      if (this.m_delayedEvents.Count <= 0)
        return;
      for (int index = this.m_delayedEvents.Count - 1; index >= 0; --index)
      {
        this.m_delayedEvents[index].tickDown -= deltaTime;
        if ((double) this.m_delayedEvents[index].tickDown <= 0.0 && (Object) this.m_delayedEvents[index].entity != (Object) null)
        {
          AIEvent e = new AIEvent(this.m_delayedEvents[index].pos, AIEvent.AIEType.Sonic, this.m_delayedEvents[index].danger);
          this.m_delayedEvents[index].entity.OnAIEventReceive(e);
          this.m_delayedEvents.RemoveAt(index);
        }
      }
    }

    private void EntityCheck(AIEntity e)
    {
      e.ResetTick();
      if (!e.ReceivesEvent_Visual)
        return;
      Vector3 pos1 = e.GetPos();
      Vector3 position = pos1;
      Vector3 forward = e.SensoryFrame.forward;
      if (!e.IsVisualCheckOmni)
        position += forward * e.MaximumSightRange;
      Collider[] colliderArray = Physics.OverlapSphere(position, e.MaximumSightRange, (int) this.LM_Entity, QueryTriggerInteraction.Collide);
      if (colliderArray.Length <= 0)
        return;
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        AIEntity component = colliderArray[index].GetComponent<AIEntity>();
        if (!((Object) component == (Object) null) && !((Object) component == (Object) e) && component.IFFCode >= -1)
        {
          Vector3 pos2 = component.GetPos();
          Vector3 to = pos2 - pos1;
          float magnitude = to.magnitude;
          float b = e.MaximumSightRange;
          if ((double) magnitude <= (double) component.MaxDistanceVisibleFrom && (double) component.VisibilityMultiplier <= 2.0)
          {
            float num1 = (double) component.VisibilityMultiplier <= 1.0 ? Mathf.Lerp(0.0f, magnitude, component.VisibilityMultiplier) : Mathf.Lerp(magnitude, b, component.VisibilityMultiplier - 1f);
            if (!e.IsVisualCheckOmni)
            {
              float num2 = Vector3.Angle(forward, to);
              b = e.MaximumSightRange * e.SightDistanceByFOVMultiplier.Evaluate(num2 / e.MaximumSightFOV);
            }
            if ((double) num1 <= (double) b && !Physics.Linecast(pos1, pos2, (int) e.LM_VisualOcclusionCheck, QueryTriggerInteraction.Collide))
            {
              float v = num1 / e.MaximumSightRange * component.DangerMultiplier;
              AIEvent e1 = new AIEvent(component, AIEvent.AIEType.Visual, v);
              e.OnAIEventReceive(e1);
            }
          }
        }
      }
    }

    public void SonicEvent(float loudness, float maxDistanceHeard, Vector3 pos, int iffcode)
    {
      if ((double) loudness < (double) GM.CurrentSceneSettings.BaseLoudness || iffcode == -3)
        return;
      for (int index = 0; index < this.m_knownEntities.Count; ++index)
      {
        AIEntity knownEntity = this.m_knownEntities[index];
        if (knownEntity.ReceivesEvent_Sonic && (double) knownEntity.SonicThreshold < (double) loudness && knownEntity.IFFCode != iffcode)
        {
          float time = Vector3.Distance(knownEntity.GetPos(), pos);
          if ((double) time <= (double) knownEntity.MaxHearingDistance && (double) time <= (double) maxDistanceHeard)
          {
            float loudness1 = this.LoudnessFalloff.Evaluate(time) * loudness;
            if ((double) knownEntity.SonicThreshold <= (double) loudness1)
            {
              float num = Mathf.Clamp(time / 50f, 0.1f, 1f);
              float Danger = (float) (1.0 - (double) loudness1 / 200.0) * num;
              knownEntity.ProcessLoudness(loudness1);
              this.m_delayedEvents.Add(new AIManager.DelayedSonicEvent(time / 343f, knownEntity, Danger, pos));
            }
          }
        }
      }
    }

    public void SuppressionEvent(
      Vector3 pos,
      Vector3 dir,
      int iffcode,
      float intensity,
      float range)
    {
      for (int index = 0; index < this.m_knownEntities.Count; ++index)
      {
        AIEntity knownEntity = this.m_knownEntities[index];
        if (knownEntity.ReceivesEvent_Sonic || knownEntity.ReceivesEvent_Visual)
          knownEntity.OnAIReceiveSuppression(pos, dir, iffcode, intensity, range);
      }
    }

    public enum SpeakType
    {
      chat,
      pain,
      death,
    }

    public class DelayedSonicEvent
    {
      public float tickDown;
      public AIEntity entity;
      public float danger;
      public Vector3 pos;

      public DelayedSonicEvent(float TickDown, AIEntity Entity, float Danger, Vector3 Pos)
      {
        this.tickDown = TickDown;
        this.entity = Entity;
        this.danger = Danger;
        this.pos = Pos;
      }
    }
  }
}
