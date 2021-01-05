// Decompiled with JetBrains decompiler
// Type: FistVR.ScalpelDrumDisplay
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ScalpelDrumDisplay : MonoBehaviour
  {
    private int m_numRoundsDisplayed;
    public List<float> RotsForCapacity;
    public FVRFireArmMagazine Mag;
    public Transform RotPiece;

    private void Start()
    {
    }

    private void Update()
    {
      int numRounds = this.Mag.m_numRounds;
      if (this.m_numRoundsDisplayed == numRounds)
        return;
      this.m_numRoundsDisplayed = numRounds;
      this.SetRot(numRounds);
    }

    private void SetRot(int i) => this.RotPiece.localEulerAngles = new Vector3(0.0f, 0.0f, this.RotsForCapacity[i]);
  }
}
