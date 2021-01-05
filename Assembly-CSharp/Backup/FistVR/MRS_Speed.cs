// Decompiled with JetBrains decompiler
// Type: FistVR.MRS_Speed
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FistVR
{
  public class MRS_Speed : ModularRangeSequencer
  {
    private List<MRT_SimpleBull> Targets = new List<MRT_SimpleBull>();
    public AudioClip AC_SequenceBegins;
    public AudioClip AC_WaveBegins;
    public AudioClip AC_ReloadTimeBegins;
    public AudioClip AC_SequenceOver;
    public Color Color_WarmUp;
    public Color Color_InWave;
    public Color Color_Reload;
    public Color Color_SequenceOver;

    public override void BeginSequence(
      ModularRangeMaster master,
      ModularRangeSequenceDefinition sequenceDef)
    {
      base.BeginSequence(master, sequenceDef);
      this.curWave = 0;
      this.m_waveTickDown = 0.0f;
      this.m_reloadTickDown = 0.0f;
      this.m_wavelength = 0.0f;
      this.m_warmupTickDown = 5f;
      this.SetScore(0);
      this.m_master.PlayAudioEvent(this.AC_SequenceBegins, 0.4f);
      this.BeginWarmup();
    }

    public override void AbortSequence() => base.AbortSequence();

    public override void Update()
    {
      base.Update();
      switch (this.m_state)
      {
        case ModularRangeSequencer.RangeSequencerState.Warmup:
          if ((double) this.m_warmupTickDown > 0.0)
          {
            this.m_warmupTickDown -= Time.deltaTime;
            this.m_master.InGameDisplay.text = "Warmup\n";
            this.m_master.InGameDisplay.text += this.m_warmupTickDown.ToString("00.00");
            this.m_master.InGameDisplay.color = this.Color_WarmUp;
            break;
          }
          this.BeginWave();
          break;
        case ModularRangeSequencer.RangeSequencerState.InWave:
          if ((double) this.m_waveTickDown < (double) this.m_wavelength)
          {
            this.m_waveTickDown += Time.deltaTime;
            this.m_master.InGameDisplay.text = "Wave " + (object) (this.curWave + 1) + " of " + (object) this.m_sequenceDefinition.Waves.Length + "\n";
            this.m_master.InGameDisplay.text += (this.m_wavelength - this.m_waveTickDown).ToString("00.00");
            this.m_master.InGameDisplay.color = this.Color_InWave;
            break;
          }
          this.CancelInvoke();
          if (this.curWave < this.m_sequenceDefinition.Waves.Length - 1)
          {
            this.BeginReloadTime();
            break;
          }
          this.EndSequence();
          this.m_state = ModularRangeSequencer.RangeSequencerState.NotRunning;
          break;
        case ModularRangeSequencer.RangeSequencerState.ReloadTime:
          if ((double) this.m_reloadTickDown < (double) this.m_reloadLength)
          {
            this.m_reloadTickDown += Time.deltaTime;
            this.m_master.InGameDisplay.text = "Reload!\n";
            this.m_master.InGameDisplay.text += (this.m_reloadLength - this.m_reloadTickDown).ToString("00.00");
            this.m_master.InGameDisplay.color = this.Color_Reload;
            break;
          }
          ++this.curWave;
          this.ClearSpawnedTargets();
          this.BeginWave();
          break;
        case ModularRangeSequencer.RangeSequencerState.SequenceOver:
          this.m_master.InGameDisplay.text = "Sequence Over";
          this.m_master.InGameDisplay.color = this.Color_SequenceOver;
          this.SequenceOver();
          break;
      }
    }

    private void BeginWarmup() => this.m_state = ModularRangeSequencer.RangeSequencerState.Warmup;

    private void BeginWave()
    {
      this.StopAllCoroutines();
      this.m_master.PlayAudioEvent(this.AC_WaveBegins, 0.4f);
      this.m_state = ModularRangeSequencer.RangeSequencerState.InWave;
      this.m_curTarget = 0;
      this.GenerateSpawnPositions();
      this.m_wavelength = this.m_waves[this.curWave].TimeForWave;
      this.m_reloadLength = this.m_waves[this.curWave].TimeForReload;
      this.m_waveTickDown = 0.0f;
      this.m_reloadTickDown = 0.0f;
      switch (this.m_waves[this.curWave].Timing)
      {
        case ModularRangeSequenceDefinition.TargetTiming.SequentialOnHit:
          float delayPerTarget1 = this.m_waves[this.curWave].DelayPerTarget;
          bool flag1 = false;
          for (int index = 0; index < this.m_isNoShoot.Length; ++index)
          {
            if (!this.m_isNoShoot[index])
            {
              flag1 = true;
              this.StartCoroutine(this.SpawnSimpleBull(delayPerTarget1, 0, true, false));
            }
            else
            {
              this.StartCoroutine(this.SpawnSimpleBull(delayPerTarget1, 0, true, true));
              ++this.m_curTarget;
            }
            if (flag1)
              break;
          }
          break;
        case ModularRangeSequenceDefinition.TargetTiming.SequentialTimed:
          for (int spawnIndex = 0; spawnIndex < this.m_isNoShoot.Length; ++spawnIndex)
          {
            float delay = this.m_waves[this.curWave].DelayPerTarget * (float) spawnIndex;
            if (!this.m_isNoShoot[spawnIndex])
              this.StartCoroutine(this.SpawnSimpleBull(delay, spawnIndex, true, false));
            else
              this.StartCoroutine(this.SpawnSimpleBull(delay, spawnIndex, true, true));
          }
          break;
        case ModularRangeSequenceDefinition.TargetTiming.RandomOnHit:
          this.ShuffleSpawnPoints();
          float delayPerTarget2 = this.m_waves[this.curWave].DelayPerTarget;
          bool flag2 = false;
          for (int index = 0; index < this.m_isNoShoot.Length; ++index)
          {
            if (!this.m_isNoShoot[index])
            {
              flag2 = true;
              this.StartCoroutine(this.SpawnSimpleBull(delayPerTarget2, 0, true, false));
            }
            else
            {
              this.StartCoroutine(this.SpawnSimpleBull(delayPerTarget2, 0, true, true));
              ++this.m_curTarget;
            }
            if (flag2)
              break;
          }
          break;
        case ModularRangeSequenceDefinition.TargetTiming.RandomTimed:
          this.ShuffleSpawnPoints();
          for (int spawnIndex = 0; spawnIndex < this.m_isNoShoot.Length; ++spawnIndex)
          {
            float delay = this.m_waves[this.curWave].DelayPerTarget * (float) spawnIndex;
            if (!this.m_isNoShoot[spawnIndex])
              this.StartCoroutine(this.SpawnSimpleBull(delay, spawnIndex, true, false));
            else
              this.StartCoroutine(this.SpawnSimpleBull(delay, spawnIndex, true, true));
          }
          break;
        case ModularRangeSequenceDefinition.TargetTiming.Flood:
          for (int spawnIndex = 0; spawnIndex < this.m_isNoShoot.Length; ++spawnIndex)
          {
            float num = this.m_waves[this.curWave].DelayPerTarget * (float) spawnIndex;
            if (!this.m_isNoShoot[spawnIndex])
              this.StartCoroutine(this.SpawnSimpleBull(0.0f, spawnIndex, true, false));
            else
              this.StartCoroutine(this.SpawnSimpleBull(0.0f, spawnIndex, true, true));
          }
          break;
      }
    }

    public override void RegisterTargetHit()
    {
      base.RegisterTargetHit();
      if (this.m_state != ModularRangeSequencer.RangeSequencerState.InWave)
        return;
      switch (this.m_waves[this.curWave].Timing)
      {
        case ModularRangeSequenceDefinition.TargetTiming.SequentialOnHit:
          bool flag1 = false;
          for (int curTarget = this.m_curTarget; curTarget < this.m_isNoShoot.Length; ++curTarget)
          {
            float delayPerTarget = this.m_waves[this.curWave].DelayPerTarget;
            if (!this.m_isNoShoot[curTarget])
            {
              flag1 = true;
              this.StartCoroutine(this.SpawnSimpleBull(delayPerTarget, this.m_curTarget, true, false));
            }
            else
            {
              this.StartCoroutine(this.SpawnSimpleBull(delayPerTarget, this.m_curTarget, true, true));
              ++this.m_curTarget;
            }
            if (flag1)
              break;
          }
          break;
        case ModularRangeSequenceDefinition.TargetTiming.RandomOnHit:
          bool flag2 = false;
          for (int curTarget = this.m_curTarget; curTarget < this.m_isNoShoot.Length; ++curTarget)
          {
            float delayPerTarget = this.m_waves[this.curWave].DelayPerTarget;
            if (!this.m_isNoShoot[curTarget])
            {
              flag2 = true;
              this.StartCoroutine(this.SpawnSimpleBull(delayPerTarget, this.m_curTarget, true, false));
            }
            else
            {
              this.StartCoroutine(this.SpawnSimpleBull(delayPerTarget, this.m_curTarget, true, true));
              ++this.m_curTarget;
            }
            if (flag2)
              break;
          }
          break;
      }
    }

    private void BeginReloadTime()
    {
      this.m_master.PlayAudioEvent(this.AC_ReloadTimeBegins, 0.4f);
      this.m_state = ModularRangeSequencer.RangeSequencerState.ReloadTime;
      this.ClearSpawnedTargets();
    }

    private void EndSequence()
    {
      this.m_master.PlayAudioEvent(this.AC_SequenceOver, 0.4f);
      this.ClearSpawnedTargets();
      this.SequenceOver();
    }

    [DebuggerHidden]
    private IEnumerator SpawnSimpleBull(
      float delay,
      int spawnIndex,
      bool autoActivate,
      bool isNoShoot)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MRS_Speed.\u003CSpawnSimpleBull\u003Ec__Iterator0()
      {
        delay = delay,
        isNoShoot = isNoShoot,
        spawnIndex = spawnIndex,
        autoActivate = autoActivate,
        \u0024this = this
      };
    }

    public override void ClearSpawnedTargets()
    {
      base.ClearSpawnedTargets();
      if (this.Targets.Count <= 0)
        return;
      for (int index = this.Targets.Count - 1; index >= 0; --index)
      {
        if ((Object) this.Targets[index] != (Object) null)
          Object.Destroy((Object) this.Targets[index].gameObject);
      }
      this.Targets.Clear();
    }
  }
}
