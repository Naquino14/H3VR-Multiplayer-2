// Decompiled with JetBrains decompiler
// Type: FistVR.SamplerPlatter_HelpScreenPlacer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class SamplerPlatter_HelpScreenPlacer : MonoBehaviour
  {
    private int m_closestIndex;
    public Transform Screen;
    public List<Transform> ScreenPoints;

    private void Update()
    {
      float num1 = Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.ScreenPoints[0].position);
      int num2 = 0;
      for (int index = 1; index < this.ScreenPoints.Count; ++index)
      {
        float num3 = Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.ScreenPoints[index].position);
        if ((double) num3 < (double) num1)
        {
          num1 = num3;
          num2 = index;
        }
      }
      if (num2 == this.m_closestIndex)
        return;
      this.m_closestIndex = num2;
      this.Screen.position = this.ScreenPoints[this.m_closestIndex].position;
      this.Screen.rotation = this.ScreenPoints[this.m_closestIndex].rotation;
    }
  }
}
