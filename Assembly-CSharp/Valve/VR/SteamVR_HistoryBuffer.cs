// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_HistoryBuffer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_HistoryBuffer : SteamVR_RingBuffer<SteamVR_HistoryStep>
  {
    public SteamVR_HistoryBuffer(int size)
      : base(size)
    {
    }

    public void Update(
      Vector3 position,
      Quaternion rotation,
      Vector3 velocity,
      Vector3 angularVelocity)
    {
      if (this.buffer[this.currentIndex] == null)
        this.buffer[this.currentIndex] = new SteamVR_HistoryStep();
      this.buffer[this.currentIndex].position = position;
      this.buffer[this.currentIndex].rotation = rotation;
      this.buffer[this.currentIndex].velocity = velocity;
      this.buffer[this.currentIndex].angularVelocity = angularVelocity;
      this.buffer[this.currentIndex].timeInTicks = DateTime.Now.Ticks;
      this.StepForward();
    }

    public float GetVelocityMagnitudeTrend(int toIndex = -1, int fromIndex = -1)
    {
      if (toIndex == -1)
        toIndex = this.currentIndex - 1;
      if (toIndex < 0)
        toIndex += this.buffer.Length;
      if (fromIndex == -1)
        fromIndex = toIndex - 1;
      if (fromIndex < 0)
        fromIndex += this.buffer.Length;
      SteamVR_HistoryStep step1 = this.buffer[toIndex];
      SteamVR_HistoryStep step2 = this.buffer[fromIndex];
      return this.IsValid(step1) && this.IsValid(step2) ? step1.velocity.sqrMagnitude - step2.velocity.sqrMagnitude : 0.0f;
    }

    public bool IsValid(SteamVR_HistoryStep step) => step != null && step.timeInTicks != -1L;

    public int GetTopVelocity(int forFrames, int addFrames = 0)
    {
      int num1 = this.currentIndex;
      float num2 = 0.0f;
      int index = this.currentIndex;
      while (forFrames > 0)
      {
        --forFrames;
        --index;
        if (index < 0)
          index = this.buffer.Length - 1;
        if (this.IsValid(this.buffer[index]))
        {
          float sqrMagnitude = this.buffer[index].velocity.sqrMagnitude;
          if ((double) sqrMagnitude > (double) num2)
          {
            num1 = index;
            num2 = sqrMagnitude;
          }
        }
        else
          break;
      }
      int num3 = num1 + addFrames;
      if (num3 >= this.buffer.Length)
        num3 -= this.buffer.Length;
      return num3;
    }

    public void GetAverageVelocities(
      out Vector3 velocity,
      out Vector3 angularVelocity,
      int forFrames,
      int startFrame = -1)
    {
      velocity = Vector3.zero;
      angularVelocity = Vector3.zero;
      if (startFrame == -1)
        startFrame = this.currentIndex - 1;
      if (startFrame < 0)
        startFrame = this.buffer.Length - 1;
      int num1 = startFrame - forFrames;
      if (num1 < 0)
      {
        int num2 = num1 + this.buffer.Length;
      }
      Vector3 zero1 = Vector3.zero;
      Vector3 zero2 = Vector3.zero;
      float num3 = 0.0f;
      int index = startFrame;
      while (forFrames > 0)
      {
        --forFrames;
        --index;
        if (index < 0)
          index = this.buffer.Length - 1;
        SteamVR_HistoryStep step = this.buffer[index];
        if (this.IsValid(step))
        {
          ++num3;
          zero1 += step.velocity;
          zero2 += step.angularVelocity;
        }
        else
          break;
      }
      velocity = zero1 / num3;
      angularVelocity = zero2 / num3;
    }
  }
}
