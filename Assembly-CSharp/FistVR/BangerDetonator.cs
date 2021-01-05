// Decompiled with JetBrains decompiler
// Type: FistVR.BangerDetonator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class BangerDetonator : FVRPhysicalObject
  {
    private List<Banger> m_bangers = new List<Banger>();
    private float m_triggerFloat;
    private float m_lastTriggerFloat;
    public Transform TriggerPiece;
    public Vector2 TriggerRange;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.m_hasTriggeredUpSinceBegin)
      {
        this.m_triggerFloat = hand.Input.TriggerFloat;
        if (!hand.Input.TriggerDown)
          return;
        this.Detonate();
      }
      else
        this.m_triggerFloat = 0.0f;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.m_triggerFloat = 0.0f;
    }

    public void RegisterBanger(Banger b)
    {
      if (this.m_bangers.Contains(b))
        return;
      this.m_bangers.Add(b);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.m_triggerFloat != (double) this.m_lastTriggerFloat)
        this.SetAnimatedComponent(this.TriggerPiece, Mathf.Lerp(this.TriggerRange.x, this.TriggerRange.y, this.m_triggerFloat), FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
      this.m_lastTriggerFloat = this.m_triggerFloat;
    }

    private void Detonate()
    {
      for (int index = this.m_bangers.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_bangers[index] != (Object) null && this.m_bangers[index].IsArmed)
        {
          this.m_bangers[index].StartExploding();
          this.m_bangers.RemoveAt(index);
        }
      }
      this.m_bangers.TrimExcess();
    }
  }
}
