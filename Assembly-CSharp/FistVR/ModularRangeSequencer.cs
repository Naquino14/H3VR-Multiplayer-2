// Decompiled with JetBrains decompiler
// Type: FistVR.ModularRangeSequencer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class ModularRangeSequencer : MonoBehaviour
  {
    protected ModularRangeMaster m_master;
    protected ModularRangeSequenceDefinition m_sequenceDefinition;
    protected ModularRangeSequenceDefinition.WaveDefinition[] m_waves;
    protected int curWave;
    protected float m_warmupTickDown;
    protected float m_waveTickDown;
    protected float m_wavelength;
    protected float m_reloadTickDown;
    protected float m_reloadLength;
    protected Vector3[] m_spawnPositions;
    protected bool[] m_isNoShoot;
    protected int m_curTarget;
    protected int m_score;
    protected ModularRangeSequencer.RangeSequencerState m_state;
    public ModularRangeSequencer.RangeSequencerType Type;

    public ModularRangeSequenceDefinition SequenceDefinition => this.m_sequenceDefinition;

    public virtual void BeginSequence(
      ModularRangeMaster master,
      ModularRangeSequenceDefinition sequenceDef)
    {
      this.m_master = master;
      this.m_sequenceDefinition = sequenceDef;
      this.m_waves = this.m_sequenceDefinition.Waves;
    }

    public virtual void AbortSequence()
    {
      this.CancelInvoke();
      this.ClearSpawnedTargets();
      this.m_state = ModularRangeSequencer.RangeSequencerState.NotRunning;
    }

    public virtual void SequenceOver()
    {
      this.ClearSpawnedTargets();
      this.m_state = ModularRangeSequencer.RangeSequencerState.SequenceOver;
      this.m_master.GoToHighScoreBoard();
    }

    public virtual void Update()
    {
    }

    public virtual void GenerateSpawnPositions()
    {
      int length = this.m_waves[this.curWave].TargetNum + this.m_waves[this.curWave].NumNoShootTarget;
      this.m_spawnPositions = new Vector3[length];
      this.m_isNoShoot = new bool[length];
      for (int index = 0; index < this.m_isNoShoot.Length; ++index)
        this.m_isNoShoot[index] = false;
      for (int index = 0; index < this.m_waves[this.curWave].NumNoShootTarget; ++index)
      {
        Debug.Log((object) "Target set to noshoot");
        this.m_isNoShoot[index] = true;
      }
      this.ShuffleNoShoot();
      this.ShuffleNoShoot();
      switch (this.m_waves[this.curWave].Layout)
      {
        case ModularRangeSequenceDefinition.TargetLayout.HorizontalLeft:
          for (int index = 0; index < length; ++index)
          {
            float x = 0.0f;
            float z = this.m_waves[this.curWave].Distance;
            if (length > 1)
            {
              float num = Mathf.Min(4f, (float) length);
              x = (float) ((double) index / ((double) length - 1.0) * (double) num - (double) num * 0.5);
              z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index / ((float) length - 1f));
            }
            Vector3 vector3 = new Vector3(x, 1f, z);
            this.m_spawnPositions[index] = vector3;
          }
          break;
        case ModularRangeSequenceDefinition.TargetLayout.HorizontalRight:
          for (int index = 0; index < length; ++index)
          {
            float num1 = 0.0f;
            float z = this.m_waves[this.curWave].Distance;
            if (length > 1)
            {
              float num2 = Mathf.Min(4f, (float) length);
              num1 = (float) ((double) index / ((double) length - 1.0) * (double) num2 - (double) num2 * 0.5);
              z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index / ((float) length - 1f));
            }
            Vector3 vector3 = new Vector3(-num1, 1f, z);
            this.m_spawnPositions[index] = vector3;
          }
          break;
        case ModularRangeSequenceDefinition.TargetLayout.VerticalUp:
          for (int index = 0; index < length; ++index)
          {
            float num1 = 0.0f;
            float z = this.m_waves[this.curWave].Distance;
            if (length > 1)
            {
              float num2 = Mathf.Min(4f, (float) length);
              num1 = (float) ((double) index / ((double) length - 1.0) * (double) num2 - (double) num2 * 0.5);
              z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index / ((float) length - 1f));
            }
            Vector3 vector3 = new Vector3(0.0f, num1 + 1f, z);
            this.m_spawnPositions[index] = vector3;
          }
          break;
        case ModularRangeSequenceDefinition.TargetLayout.VerticalDown:
          for (int index = 0; index < length; ++index)
          {
            float num1 = 0.0f;
            float z = this.m_waves[this.curWave].Distance;
            if (length > 1)
            {
              float num2 = Mathf.Min(4f, (float) length);
              num1 = (float) ((double) index / ((double) length - 1.0) * (double) num2 - (double) num2 * 0.5);
              z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index / ((float) length - 1f));
            }
            Vector3 vector3 = new Vector3(0.0f, (float) (-(double) num1 + 1.0), z);
            this.m_spawnPositions[index] = vector3;
          }
          break;
        case ModularRangeSequenceDefinition.TargetLayout.DiagonalLeftUp:
          for (int index = 0; index < length; ++index)
          {
            float x = 0.0f;
            float z = this.m_waves[this.curWave].Distance;
            if (length > 1)
            {
              float num = Mathf.Min(4f, (float) length);
              x = (float) ((double) index / ((double) length - 1.0) * (double) num - (double) num * 0.5);
              z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index / ((float) length - 1f));
            }
            Vector3 vector3 = new Vector3(x, x + 1f, z);
            this.m_spawnPositions[index] = vector3;
          }
          break;
        case ModularRangeSequenceDefinition.TargetLayout.DiagonalRightUp:
          for (int index = 0; index < length; ++index)
          {
            float num1 = 0.0f;
            float z = this.m_waves[this.curWave].Distance;
            if (length > 1)
            {
              float num2 = Mathf.Min(4f, (float) length);
              num1 = (float) ((double) index / ((double) length - 1.0) * (double) num2 - (double) num2 * 0.5);
              z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index / ((float) length - 1f));
            }
            Vector3 vector3 = new Vector3(-num1, num1 + 1f, z);
            this.m_spawnPositions[index] = vector3;
          }
          break;
        case ModularRangeSequenceDefinition.TargetLayout.DiagonalLeftDown:
          for (int index = 0; index < length; ++index)
          {
            float x = 0.0f;
            float z = this.m_waves[this.curWave].Distance;
            if (length > 1)
            {
              float num = Mathf.Min(4f, (float) length);
              x = (float) ((double) index / ((double) length - 1.0) * (double) num - (double) num * 0.5);
              z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index / ((float) length - 1f));
            }
            Vector3 vector3 = new Vector3(x, (float) (-(double) x + 1.0), z);
            this.m_spawnPositions[index] = vector3;
          }
          break;
        case ModularRangeSequenceDefinition.TargetLayout.DiagonalRightDown:
          for (int index = 0; index < length; ++index)
          {
            float num1 = 0.0f;
            float z = this.m_waves[this.curWave].Distance;
            if (length > 1)
            {
              float num2 = Mathf.Min(4f, (float) length);
              num1 = (float) ((double) index / ((double) length - 1.0) * (double) num2 - (double) num2 * 0.5);
              z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index / ((float) length - 1f));
            }
            Vector3 vector3 = new Vector3(-num1, (float) (-(double) num1 + 1.0), z);
            this.m_spawnPositions[index] = vector3;
          }
          break;
        case ModularRangeSequenceDefinition.TargetLayout.SquareUp:
          int b1 = Mathf.CeilToInt(Mathf.Sqrt((float) length));
          int index1 = 0;
          for (int index2 = 0; index2 < b1; ++index2)
          {
            float num1 = Mathf.Min(4f, (float) b1);
            float num2 = (float) ((double) index2 / ((double) b1 - 1.0) * (double) num1 - (double) num1 * 0.5);
            int a = Mathf.Min(length - index1, b1);
            for (int index3 = 0; index3 < a; ++index3)
            {
              if (index1 < length)
              {
                float x;
                if (a > 1)
                {
                  float num3 = Mathf.Min(4f, (float) Mathf.Min(a, b1));
                  x = (float) ((double) index3 / ((double) a - 1.0) * (double) num3 - (double) num3 * 0.5);
                }
                else
                  x = 0.0f;
                float z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index1 / ((float) length - 1f));
                Vector3 vector3 = new Vector3(x, num2 + 1f, z);
                this.m_spawnPositions[index1] = vector3;
              }
              ++index1;
            }
          }
          break;
        case ModularRangeSequenceDefinition.TargetLayout.SquareDown:
          int b2 = Mathf.CeilToInt(Mathf.Sqrt((float) length));
          int index4 = 0;
          for (int index2 = 0; index2 < b2; ++index2)
          {
            float num1 = Mathf.Min(4f, (float) b2);
            float num2 = (float) ((double) index2 / ((double) b2 - 1.0) * (double) num1 - (double) num1 * 0.5);
            int a = Mathf.Min(length - index4, b2);
            for (int index3 = 0; index3 < a; ++index3)
            {
              if (index4 < length)
              {
                float x;
                if (a > 1)
                {
                  float num3 = Mathf.Min(4f, (float) Mathf.Min(a, b2));
                  x = (float) ((double) index3 / ((double) a - 1.0) * (double) num3 - (double) num3 * 0.5);
                }
                else
                  x = 0.0f;
                float z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index4 / ((float) length - 1f));
                Vector3 vector3 = new Vector3(x, num2 + 1f, z);
                this.m_spawnPositions[index4] = vector3;
              }
              ++index4;
            }
          }
          break;
        case ModularRangeSequenceDefinition.TargetLayout.CircleClockWise:
          for (int index2 = 0; index2 < length; ++index2)
          {
            float x = 0.0f;
            float num1 = 0.0f;
            if (length > 1)
            {
              float num2 = (float) ((double) index2 / (double) length * 360.0);
              float num3 = Mathf.Sin((float) Math.PI / 180f * num2);
              float num4 = Mathf.Cos((float) Math.PI / 180f * num2);
              x = num3 * 2f;
              num1 = num4 * 2f;
            }
            float z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index2 / ((float) length - 1f));
            Vector3 vector3 = new Vector3(x, num1 + 1f, z);
            this.m_spawnPositions[index2] = vector3;
          }
          break;
        case ModularRangeSequenceDefinition.TargetLayout.CircleCounterClockWise:
          for (int index2 = 0; index2 < length; ++index2)
          {
            float x = 0.0f;
            float num1 = 0.0f;
            if (length > 1)
            {
              float num2 = (float) ((double) index2 / (double) length * 360.0);
              float num3 = -Mathf.Sin((float) Math.PI / 180f * num2);
              float num4 = Mathf.Cos((float) Math.PI / 180f * num2);
              x = num3 * 2f;
              num1 = num4 * 2f;
            }
            float z = Mathf.Lerp(this.m_waves[this.curWave].Distance, this.m_waves[this.curWave].EndDistance, (float) index2 / ((float) length - 1f));
            Vector3 vector3 = new Vector3(x, num1 + 1f, z);
            this.m_spawnPositions[index2] = vector3;
          }
          break;
      }
    }

    public virtual void ClearSpawnedTargets()
    {
    }

    protected void ShuffleSpawnPoints()
    {
      for (int max = this.m_spawnPositions.Length - 1; max > 0; --max)
      {
        int index = UnityEngine.Random.Range(0, max);
        Vector3 spawnPosition = this.m_spawnPositions[max];
        this.m_spawnPositions[max] = this.m_spawnPositions[index];
        this.m_spawnPositions[index] = spawnPosition;
      }
    }

    protected void ShuffleNoShoot()
    {
      for (int max = this.m_isNoShoot.Length - 1; max > 0; --max)
      {
        int index = UnityEngine.Random.Range(0, max);
        bool flag = this.m_isNoShoot[max];
        this.m_isNoShoot[max] = this.m_isNoShoot[index];
        this.m_isNoShoot[index] = flag;
      }
    }

    public virtual void RegisterTargetHit() => ++this.m_curTarget;

    public void SetScore(int s)
    {
      this.m_score = s;
      this.m_master.ScoreDisplay.text = this.m_score.ToString() + " pts";
    }

    public void AddScore(int s)
    {
      this.m_score += s;
      this.m_master.ScoreDisplay.text = this.m_score.ToString() + " pts";
    }

    public int GetScore() => this.m_score;

    public enum RangeSequencerState
    {
      NotRunning,
      Warmup,
      InWave,
      ReloadTime,
      SequenceOver,
    }

    public enum RangeSequencerType
    {
      Speed,
    }
  }
}
