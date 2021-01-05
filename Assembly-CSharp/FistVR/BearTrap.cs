// Decompiled with JetBrains decompiler
// Type: FistVR.BearTrap
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class BearTrap : MonoBehaviour
  {
    public List<BearTrapInteractiblePiece> Pieces;

    private void Awake()
    {
      for (int index = 0; index < this.Pieces.Count; ++index)
        this.Pieces[index].SetBearTrap(this);
    }

    public bool IsOpen() => this.Pieces[0].State == BearTrapInteractiblePiece.BearTrapState.Open && this.Pieces[1].State == BearTrapInteractiblePiece.BearTrapState.Open;

    public void ThingDetected()
    {
      if (this.Pieces[0].CanSnapShut())
        this.Pieces[0].SnapShut();
      if (!this.Pieces[1].CanSnapShut())
        return;
      this.Pieces[1].SnapShut();
    }

    public void ForceOpen()
    {
      this.Pieces[0].Open();
      this.Pieces[1].Open();
    }

    public void ForceClose()
    {
      this.Pieces[0].Close();
      this.Pieces[1].Close();
    }
  }
}
