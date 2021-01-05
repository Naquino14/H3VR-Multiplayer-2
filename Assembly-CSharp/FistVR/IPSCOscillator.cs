// Decompiled with JetBrains decompiler
// Type: FistVR.IPSCOscillator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class IPSCOscillator : MonoBehaviour
  {
    public float LerpOscillator;
    public float Speed = 0.1f;
    public bool MovingUp = true;
    public Rigidbody Me;
    public Transform Point1;
    public Transform Point2;

    private void FixedUpdate()
    {
      if (this.MovingUp)
      {
        this.LerpOscillator += Time.deltaTime * this.Speed;
        if ((double) this.LerpOscillator >= 1.0)
        {
          this.MovingUp = false;
          this.LerpOscillator = 1f;
        }
      }
      else
      {
        this.LerpOscillator -= Time.deltaTime * this.Speed;
        if ((double) this.LerpOscillator <= 0.0)
        {
          this.MovingUp = true;
          this.LerpOscillator = 0.0f;
        }
      }
      this.Me.MovePosition(Vector3.Lerp(this.Point1.position, this.Point2.position, this.LerpOscillator));
    }
  }
}
