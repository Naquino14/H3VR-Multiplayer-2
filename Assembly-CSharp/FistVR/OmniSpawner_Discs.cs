// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawner_Discs
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class OmniSpawner_Discs : OmniSpawner
  {
    private OmniSpawnDef_Discs m_def;
    public GameObject[] DiscPrefabs;
    private bool m_canSpawn;
    private OmniSpawnDef_Discs.DiscSpawnStyle m_spawnStyle;
    private int m_discIndex;
    private int m_totalDiscCount;
    private float m_timeTilNextDisc;
    private bool m_shouldSpawnNextOnHitDisc = true;
    private OmniSpawnDef_Discs.DiscSpawnPattern m_spawnPattern;
    private OmniSpawnDef_Discs.DiscSpawnOrdering m_spawnOrdering;
    private OmniSpawnDef_Discs.DiscZConfig m_zConfig;
    private int[] m_randIndexes;
    private int[] m_randIndexes2;
    private float m_discMoveSpeed = 1f;
    private List<OmniDisc> m_activeDiscs = new List<OmniDisc>();
    private int m_numShootableDiscs;

    public override void Configure(OmniSpawnDef def, OmniWaveEngagementRange range)
    {
      base.Configure(def, range);
      this.m_def = def as OmniSpawnDef_Discs;
      this.m_spawnStyle = this.m_def.SpawnStyle;
      this.m_spawnPattern = this.m_def.SpawnPattern;
      this.m_spawnOrdering = this.m_def.SpawnOrdering;
      this.m_zConfig = this.m_def.ZConfig;
      this.m_totalDiscCount = this.m_def.Discs.Count;
      this.m_discMoveSpeed = this.m_def.DiscMovementSpeed;
      if (this.m_spawnOrdering != OmniSpawnDef_Discs.DiscSpawnOrdering.Random)
        return;
      this.m_randIndexes = new int[this.m_totalDiscCount];
      this.m_randIndexes2 = new int[this.m_totalDiscCount];
      for (int index = 0; index < this.m_randIndexes.Length; ++index)
      {
        this.m_randIndexes[index] = index;
        this.m_randIndexes2[index] = index;
      }
      this.GenerateRandomIndices(this.m_randIndexes);
      this.GenerateRandomIndices(this.m_randIndexes2);
    }

    public override void BeginSpawning()
    {
      base.BeginSpawning();
      this.m_canSpawn = true;
    }

    public override void EndSpawning()
    {
      base.EndSpawning();
      this.m_canSpawn = false;
    }

    public override void Activate() => base.Activate();

    public override int Deactivate()
    {
      if (this.m_activeDiscs.Count > 0)
      {
        for (int index = this.m_activeDiscs.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) this.m_activeDiscs[index] != (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_activeDiscs[index].gameObject);
        }
        this.m_activeDiscs.Clear();
      }
      return base.Deactivate();
    }

    private void Update() => this.UpdateMe();

    private void UpdateMe()
    {
      if (!this.m_isConfigured)
        return;
      switch (this.m_state)
      {
        case OmniSpawner.SpawnerState.Deactivating:
          this.Deactivating();
          break;
        case OmniSpawner.SpawnerState.Activated:
          this.SpawningLoop();
          break;
        case OmniSpawner.SpawnerState.Activating:
          this.Activating();
          break;
      }
    }

    private void SpawningLoop()
    {
      if (this.m_canSpawn)
      {
        switch (this.m_spawnStyle)
        {
          case OmniSpawnDef_Discs.DiscSpawnStyle.AllAtOnce:
            while (this.m_discIndex < this.m_def.Discs.Count)
              this.SpawnDisc(this.m_discIndex);
            break;
          case OmniSpawnDef_Discs.DiscSpawnStyle.Sequential:
            if (this.m_discIndex < this.m_def.Discs.Count)
            {
              if ((double) this.m_timeTilNextDisc <= 0.0)
              {
                this.SpawnDisc(this.m_discIndex);
                this.m_timeTilNextDisc = this.m_def.TimeBetweenDiscSpawns;
                break;
              }
              this.m_timeTilNextDisc -= Time.deltaTime;
              break;
            }
            break;
          case OmniSpawnDef_Discs.DiscSpawnStyle.OnHit:
            if (this.m_discIndex < this.m_def.Discs.Count && this.m_shouldSpawnNextOnHitDisc)
            {
              this.m_shouldSpawnNextOnHitDisc = false;
              this.SpawnDisc(this.m_discIndex);
              break;
            }
            break;
        }
      }
      if (this.m_discIndex < this.m_def.Discs.Count)
        return;
      this.m_isDoneSpawning = true;
      if (this.m_numShootableDiscs > 0)
        return;
      this.m_isReadyForWaveEnd = true;
    }

    private void SpawnDisc(int index)
    {
      int index1 = index;
      if (this.m_spawnOrdering == OmniSpawnDef_Discs.DiscSpawnOrdering.Random)
      {
        index = this.m_randIndexes[index];
        index1 = this.m_randIndexes2[index];
      }
      Vector3 position = this.transform.position;
      Vector2 b = Vector2.zero;
      Vector3 targetEndingPos = this.GetTargetEndingPos(index1, out b);
      Vector3 vector3 = new Vector3(0.0f, 1.25f, 0.0f);
      Quaternion startRot = Quaternion.LookRotation(Vector3.forward);
      Quaternion endRot = Quaternion.LookRotation(-Vector3.forward);
      OmniDisc component = UnityEngine.Object.Instantiate<GameObject>(this.DiscPrefabs[(int) this.m_def.Discs[index]], position, Quaternion.identity).GetComponent<OmniDisc>();
      this.m_activeDiscs.Add(component);
      component.Init(this, position, targetEndingPos, startRot, endRot, this.m_def.MovementPattern, this.m_def.MovementStyle, b, this.m_discMoveSpeed);
      this.Invoke("PlaySpawnSound", 0.15f);
      if (component.Type != OmniSpawnDef_Discs.DiscType.NoShoot)
        ++this.m_numShootableDiscs;
      if (this.m_spawnStyle == OmniSpawnDef_Discs.DiscSpawnStyle.OnHit && this.m_def.Discs[index] == OmniSpawnDef_Discs.DiscType.NoShoot)
      {
        ++this.m_discIndex;
        if (this.m_discIndex >= this.m_def.Discs.Count)
          return;
        this.SpawnDisc(this.m_discIndex);
      }
      else
        ++this.m_discIndex;
    }

    public void ClearDisc(OmniDisc disc)
    {
      if (disc.Type != OmniSpawnDef_Discs.DiscType.NoShoot)
        --this.m_numShootableDiscs;
      this.m_activeDiscs.Remove(disc);
      this.m_shouldSpawnNextOnHitDisc = true;
    }

    private Vector3 GetTargetEndingPos(int index, out Vector2 b)
    {
      Vector3 zero = Vector3.zero;
      switch (this.m_zConfig)
      {
        case OmniSpawnDef_Discs.DiscZConfig.Homogenous:
          zero.z = this.GetRange();
          break;
        case OmniSpawnDef_Discs.DiscZConfig.Incremented:
          zero.z = this.GetRange() + (float) index * 0.1f;
          break;
      }
      float num1 = 0.0f;
      if (this.m_totalDiscCount > 1)
        num1 = (float) index / (float) (this.m_totalDiscCount - 1);
      float y = (float) (this.m_totalDiscCount - 1) * 2f;
      float x = (float) (this.m_totalDiscCount - 1) * 2f;
      b = new Vector2(x, y);
      switch (this.m_spawnPattern)
      {
        case OmniSpawnDef_Discs.DiscSpawnPattern.Circle:
          float num2 = 0.0f;
          float num3 = 0.0f;
          if (this.m_totalDiscCount > 1)
          {
            float num4 = (float) ((double) index / (double) this.m_totalDiscCount * 360.0);
            float num5 = Mathf.Sin((float) Math.PI / 180f * num4);
            float num6 = Mathf.Cos((float) Math.PI / 180f * num4);
            float num7 = (float) (1.0 + (double) this.m_totalDiscCount * 0.200000002980232);
            num2 = num5 * num7;
            num3 = num6 * num7;
            b = new Vector2(num7, num7);
          }
          float num8 = num3 + 1.25f;
          zero.x = num2;
          zero.y = num8;
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.Square:
          int num9 = Mathf.CeilToInt(Mathf.Sqrt((float) this.m_totalDiscCount));
          int num10 = index / num9;
          int num11 = index % num9;
          float num12 = (float) num9 * 2f;
          zero.x = (float) ((double) num11 * 2.0 - (double) num12 * 0.5 + 1.0);
          zero.y = (float) ((double) num10 * 2.0 - (double) num12 * 0.5 + 1.25 + 1.0);
          b = new Vector2(num12, num12);
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.LineXCentered:
          zero.y = 1.25f;
          zero.x = (float) ((double) index * 2.0 - (double) x * 0.5);
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.LineYCentered:
          zero.x = 0.0f;
          zero.y = (float) ((double) index * 2.0 - (double) x * 0.5 + 1.25);
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.LineXUp:
          zero.y = (float) ((double) x * 0.5 + 1.25);
          zero.x = (float) ((double) index * 2.0 - (double) x * 0.5);
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.LineXDown:
          zero.y = (float) (-((double) x * 0.5) + 1.25);
          zero.x = (float) ((double) index * 2.0 - (double) x * 0.5);
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.LineYLeft:
          zero.x = (float) -((double) x * 0.5);
          zero.y = (float) ((double) index * 2.0 - (double) x * 0.5 + 1.25);
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.LineYRight:
          zero.x = x * 0.5f;
          zero.y = (float) ((double) index * 2.0 - (double) x * 0.5 + 1.25);
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.DiagonalLeftUp:
          float num13 = (float) this.m_totalDiscCount * 1.4f;
          zero.x = (float) ((double) index * 1.39999997615814 - (double) num13 * 0.5);
          zero.y = (float) ((double) index * 1.39999997615814 - (double) num13 * 0.5 + 1.25);
          b = new Vector2(num13, num13);
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.DiagonalRightUp:
          float num14 = (float) this.m_totalDiscCount * 1.4f;
          zero.x = (float) -((double) index * 1.39999997615814 - (double) num14 * 0.5);
          zero.y = (float) ((double) index * 1.39999997615814 - (double) num14 * 0.5 + 1.25);
          b = new Vector2(num14, num14);
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.DiagonalLeftDown:
          float num15 = (float) this.m_totalDiscCount * 1.4f;
          zero.x = (float) ((double) index * 1.39999997615814 - (double) num15 * 0.5);
          zero.y = (float) (-((double) index * 1.39999997615814 - (double) num15 * 0.5) + 1.25);
          b = new Vector2(num15, num15);
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.DiagonalRightDown:
          float num16 = (float) this.m_totalDiscCount * 1.4f;
          zero.x = (float) -((double) index * 1.39999997615814 - (double) num16 * 0.5);
          zero.y = (float) (-((double) index * 1.39999997615814 - (double) num16 * 0.5) + 1.25);
          b = new Vector2(num16, num16);
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.SpiralCounterclockwise:
          float num17 = 0.0f;
          float num18 = 0.0f;
          if (this.m_totalDiscCount > 1)
          {
            float num4 = (float) ((double) index / (double) this.m_totalDiscCount * 360.0);
            float num5 = Mathf.Sin((float) Math.PI / 180f * num4);
            float num6 = Mathf.Cos((float) Math.PI / 180f * num4);
            float num7 = (float) (1.0 + (double) this.m_totalDiscCount * 0.200000002980232);
            float num19 = Mathf.Lerp(num7 * 0.5f, num7 * 2f, (float) index / (float) this.m_totalDiscCount);
            num17 = num5 * num19;
            num18 = num6 * num19;
            b = new Vector2(num19, num19);
          }
          float num20 = num18 + 1.25f;
          zero.x = num17;
          zero.y = num20;
          break;
        case OmniSpawnDef_Discs.DiscSpawnPattern.SpiralClockwise:
          float num21 = 0.0f;
          float num22 = 0.0f;
          if (this.m_totalDiscCount > 1)
          {
            float num4 = (float) ((double) index / (double) this.m_totalDiscCount * 360.0);
            float num5 = -Mathf.Sin((float) Math.PI / 180f * num4);
            float num6 = Mathf.Cos((float) Math.PI / 180f * num4);
            float num7 = (float) (1.0 + (double) this.m_totalDiscCount * 0.200000002980232);
            float num19 = Mathf.Lerp(num7 * 0.5f, num7 * 2f, (float) index / (float) this.m_totalDiscCount);
            num21 = num5 * num19;
            num22 = num6 * num19;
            b = new Vector2(num19, num19);
          }
          float num23 = num22 + 1.25f;
          zero.x = num21;
          zero.y = num23;
          break;
      }
      return zero;
    }

    private void GenerateRandomIndices(int[] indicies)
    {
      for (int max = indicies.Length - 1; max > 0; --max)
      {
        int index = UnityEngine.Random.Range(0, max);
        int indicy = indicies[max];
        indicies[max] = indicies[index];
        indicies[index] = indicy;
      }
    }
  }
}
