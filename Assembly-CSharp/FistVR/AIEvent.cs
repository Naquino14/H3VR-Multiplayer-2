// Decompiled with JetBrains decompiler
// Type: FistVR.AIEvent
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class AIEvent : IComparable<AIEvent>
  {
    public float TimeSinceSensed;
    public float TimeSeen;
    public bool IsEntity;
    public AIEntity Entity;
    public Vector3 Pos;
    public AIEvent.AIEType Type;
    public float Value;

    public AIEvent(AIEntity e, AIEvent.AIEType t, float v)
    {
      this.Entity = e;
      this.IsEntity = true;
      this.Pos = e.GetPos();
      this.Type = t;
      this.Value = v;
    }

    public AIEvent(Vector3 p, AIEvent.AIEType t, float v)
    {
      this.IsEntity = false;
      this.Pos = p;
      this.Type = t;
      this.Value = v;
    }

    public Vector3 UpdatePos()
    {
      if (this.IsEntity)
        this.Pos = this.Entity.GetPos();
      return this.Pos;
    }

    public void Tick()
    {
      this.TimeSinceSensed += Time.deltaTime;
      this.TimeSeen += Time.deltaTime;
    }

    public void UpdateFrom(AIEvent e)
    {
      this.Pos = e.Pos;
      this.Type = e.Type;
      this.Value = e.Value;
      this.TimeSinceSensed = 0.0f;
    }

    public void Dispose()
    {
      this.Entity = (AIEntity) null;
      this.IsEntity = false;
      this.Type = AIEvent.AIEType.None;
    }

    public int CompareTo(AIEvent other) => other == null ? 1 : this.Value.CompareTo(other.Value);

    public enum AIEType
    {
      None,
      Visual,
      Sonic,
      Damage,
    }
  }
}
