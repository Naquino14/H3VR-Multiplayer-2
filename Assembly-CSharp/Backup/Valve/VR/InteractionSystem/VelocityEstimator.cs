// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.VelocityEstimator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class VelocityEstimator : MonoBehaviour
  {
    [Tooltip("How many frames to average over for computing velocity")]
    public int velocityAverageFrames = 5;
    [Tooltip("How many frames to average over for computing angular velocity")]
    public int angularVelocityAverageFrames = 11;
    public bool estimateOnAwake;
    private Coroutine routine;
    private int sampleCount;
    private Vector3[] velocitySamples;
    private Vector3[] angularVelocitySamples;

    public void BeginEstimatingVelocity()
    {
      this.FinishEstimatingVelocity();
      this.routine = this.StartCoroutine(this.EstimateVelocityCoroutine());
    }

    public void FinishEstimatingVelocity()
    {
      if (this.routine == null)
        return;
      this.StopCoroutine(this.routine);
      this.routine = (Coroutine) null;
    }

    public Vector3 GetVelocityEstimate()
    {
      Vector3 zero = Vector3.zero;
      int num = Mathf.Min(this.sampleCount, this.velocitySamples.Length);
      if (num != 0)
      {
        for (int index = 0; index < num; ++index)
          zero += this.velocitySamples[index];
        zero *= 1f / (float) num;
      }
      return zero;
    }

    public Vector3 GetAngularVelocityEstimate()
    {
      Vector3 zero = Vector3.zero;
      int num = Mathf.Min(this.sampleCount, this.angularVelocitySamples.Length);
      if (num != 0)
      {
        for (int index = 0; index < num; ++index)
          zero += this.angularVelocitySamples[index];
        zero *= 1f / (float) num;
      }
      return zero;
    }

    public Vector3 GetAccelerationEstimate()
    {
      Vector3 zero = Vector3.zero;
      for (int index = 2 + this.sampleCount - this.velocitySamples.Length; index < this.sampleCount; ++index)
      {
        if (index >= 2)
        {
          int num1 = index - 2;
          int num2 = index - 1;
          Vector3 velocitySample1 = this.velocitySamples[num1 % this.velocitySamples.Length];
          Vector3 velocitySample2 = this.velocitySamples[num2 % this.velocitySamples.Length];
          zero += velocitySample2 - velocitySample1;
        }
      }
      return zero * (1f / Time.deltaTime);
    }

    private void Awake()
    {
      this.velocitySamples = new Vector3[this.velocityAverageFrames];
      this.angularVelocitySamples = new Vector3[this.angularVelocityAverageFrames];
      if (!this.estimateOnAwake)
        return;
      this.BeginEstimatingVelocity();
    }

    [DebuggerHidden]
    private IEnumerator EstimateVelocityCoroutine() => (IEnumerator) new VelocityEstimator.\u003CEstimateVelocityCoroutine\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
