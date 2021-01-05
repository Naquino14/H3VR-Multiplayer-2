// Decompiled with JetBrains decompiler
// Type: FistVR.FlamingFireChunk
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlamingFireChunk : MonoBehaviour
  {
    public Rigidbody RB;
    public Vector2 StartVel = new Vector2(5f, 15f);

    private void Start() => this.RB.velocity = this.transform.forward * Random.Range(this.StartVel.x, this.StartVel.y);
  }
}
