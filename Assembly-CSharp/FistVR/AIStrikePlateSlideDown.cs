// Decompiled with JetBrains decompiler
// Type: FistVR.AIStrikePlateSlideDown
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIStrikePlateSlideDown : AIStrikePlate
  {
    public Vector3 UpperPosition;
    public Vector3 LowerPosition;
    public GameObject Target;
    public string Message;

    public override void Damage(FistVR.Damage dam)
    {
      base.Damage(dam);
      this.transform.localPosition = Vector3.Lerp(this.LowerPosition, this.UpperPosition, (float) this.NumStrikesLeft / (float) this.m_originalNumStrikesLeft);
    }

    public override void Reset()
    {
      this.transform.localPosition = this.UpperPosition;
      base.Reset();
    }

    public override void PlateFelled() => this.Target.SendMessage(this.Message);
  }
}
