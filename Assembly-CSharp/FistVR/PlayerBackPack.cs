// Decompiled with JetBrains decompiler
// Type: FistVR.PlayerBackPack
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace FistVR
{
  public class PlayerBackPack : FVRPhysicalObject
  {
    [Header("Backpack Stuff")]
    public FVRQuickBeltSlot[] Slots;
    private bool m_isUsable = true;

    private new void Start()
    {
      for (int index = 0; index < this.Slots.Length; ++index)
        this.Slots[index].IsPlayer = false;
      this.StartCoroutine(this.DelayedRegister());
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((Object) this.QuickbeltSlot != (Object) null)
      {
        if (this.m_isHardnessed && this.IsHeld)
          this.SetUsable(false);
        else
          this.SetUsable(true);
      }
      else if ((double) Vector3.Angle(this.transform.position - GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.forward) > 80.0)
        this.SetUsable(false);
      else
        this.SetUsable(true);
    }

    [DebuggerHidden]
    public IEnumerator DelayedRegister() => (IEnumerator) new PlayerBackPack.\u003CDelayedRegister\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    public void RegisterQuickbeltSlots()
    {
      for (int index = 0; index < this.Slots.Length; ++index)
      {
        if (!GM.CurrentPlayerBody.QuickbeltSlots.Contains(this.Slots[index]))
          GM.CurrentPlayerBody.QuickbeltSlots.Add(this.Slots[index]);
      }
    }

    public void DeRegisterQuickbeltSlots()
    {
      for (int index = 0; index < this.Slots.Length; ++index)
      {
        if (GM.CurrentPlayerBody.QuickbeltSlots.Contains(this.Slots[index]))
          GM.CurrentPlayerBody.QuickbeltSlots.Remove(this.Slots[index]);
      }
    }

    private new void OnDestroy()
    {
      if (!((Object) GM.CurrentPlayerBody != (Object) null))
        return;
      this.DeRegisterQuickbeltSlots();
    }

    private void SetUsable(bool b)
    {
      if (b == this.m_isUsable)
        return;
      this.m_isUsable = b;
      if (this.m_isUsable)
      {
        for (int index = 0; index < this.Slots.Length; ++index)
          this.Slots[index].IsSelectable = true;
      }
      else
      {
        for (int index = 0; index < this.Slots.Length; ++index)
          this.Slots[index].IsSelectable = false;
      }
    }
  }
}
