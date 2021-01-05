﻿// Decompiled with JetBrains decompiler
// Type: FistVR.LugerExtractor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LugerExtractor : MonoBehaviour
  {
    public FVRFireArmChamber Chamber;
    private bool isUp;
    public float RotDown;
    public float RotUp;

    private void Update()
    {
      if (this.isUp)
      {
        if (this.Chamber.IsFull)
          return;
        this.isUp = false;
        this.transform.localEulerAngles = new Vector3(this.RotDown, 0.0f, 0.0f);
      }
      else
      {
        if (!this.Chamber.IsFull)
          return;
        this.isUp = true;
        this.transform.localEulerAngles = new Vector3(this.RotUp, 0.0f, 0.0f);
      }
    }
  }
}
