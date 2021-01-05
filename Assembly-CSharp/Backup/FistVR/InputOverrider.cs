// Decompiled with JetBrains decompiler
// Type: FistVR.InputOverrider
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class InputOverrider
  {
    private FVRViveHand m_hand;
    public InputOverrider.OverriderType Type;
    private bool m_triggerPressed;
    private bool m_triggerUp;
    private bool m_triggerDown;
    private float m_triggerFloat;
    public bool Real_triggerPressed;
    public bool Real_triggerUp;
    public bool Real_triggerDown;
    public float Real_triggerFloat;

    public void ConnectToHand(FVRViveHand h)
    {
      this.m_hand = h;
      this.m_hand.SetOverrider(this);
    }

    public void FlushHandConnection()
    {
      if ((UnityEngine.Object) this.m_hand != (UnityEngine.Object) null)
        this.m_hand.FlushOverrideIfThis(this);
      this.m_hand = (FVRViveHand) null;
    }

    public void UpdateTrigger(bool m_pressed)
    {
      if (m_pressed && !this.m_triggerPressed)
        this.m_triggerDown = true;
      if (!m_pressed && this.m_triggerPressed)
        this.m_triggerUp = true;
      this.m_triggerPressed = m_pressed;
      if (m_pressed)
        this.m_triggerFloat = 1f;
      else
        this.m_triggerFloat = 0.0f;
    }

    public void Process(ref HandInput i)
    {
      if (this.Type != InputOverrider.OverriderType.Trigger)
        return;
      this.Real_triggerPressed = i.TriggerPressed;
      this.Real_triggerUp = i.TriggerUp;
      this.Real_triggerDown = i.TriggerDown;
      this.Real_triggerFloat = i.TriggerFloat;
      i.TriggerDown = this.m_triggerDown;
      i.TriggerUp = this.m_triggerUp;
      i.TriggerPressed = this.m_triggerPressed;
      i.TriggerFloat = this.m_triggerFloat;
    }

    public enum OverriderType
    {
      Trigger,
    }
  }
}
