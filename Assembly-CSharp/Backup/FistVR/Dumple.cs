// Decompiled with JetBrains decompiler
// Type: FistVR.Dumple
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Dumple : MonoBehaviour, IFVRDamageable
  {
    public Dumple.DumpleState State = Dumple.DumpleState.Scanning;
    public AIEntity E;
    public AITargetPrioritySystem Priority;
    private bool m_hasPriority;
    [Header("Refs")]
    public Transform Eye;
    [Header("DamagePoints")]
    public Transform DP_Eye;
    public Transform DP_GunLeft;
    public Transform DP_GunRight;

    private void Start() => this.E.AIEventReceiveEvent += new AIEntity.AIEventReceive(this.EventReceive);

    public void EventReceive(AIEvent e)
    {
      if (this.State == Dumple.DumpleState.Static || e.IsEntity && e.Entity.IFFCode == this.E.IFFCode)
        return;
      if (e.Type == AIEvent.AIEType.Damage)
      {
        if (this.State != Dumple.DumpleState.Scanning && this.State != Dumple.DumpleState.Wandering)
          return;
        this.SetState(Dumple.DumpleState.Combat);
      }
      else
      {
        if (e.Type != AIEvent.AIEType.Visual || !this.m_hasPriority)
          return;
        this.Priority.ProcessEvent(e);
        this.SetState(Dumple.DumpleState.Combat);
      }
    }

    private void SetState(Dumple.DumpleState s)
    {
      this.State = s;
      switch (this.State)
      {
        case Dumple.DumpleState.Static:
          this.UpdateState_Static();
          break;
        case Dumple.DumpleState.Scanning:
          this.UpdateState_Scanning();
          break;
        case Dumple.DumpleState.Wandering:
          this.UpdateState_Wandering();
          break;
        case Dumple.DumpleState.Combat:
          this.UpdateState_Combat();
          break;
      }
    }

    private void Update()
    {
    }

    private void UpdateState_Static()
    {
    }

    private void UpdateState_Scanning()
    {
    }

    private void UpdateState_Wandering()
    {
    }

    private void UpdateState_Combat()
    {
    }

    public void Damage(FistVR.Damage d)
    {
    }

    public enum DumpleState
    {
      Static,
      Scanning,
      Wandering,
      Combat,
    }
  }
}
