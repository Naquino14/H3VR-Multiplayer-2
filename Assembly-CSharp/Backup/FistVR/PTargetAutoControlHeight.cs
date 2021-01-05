// Decompiled with JetBrains decompiler
// Type: FistVR.PTargetAutoControlHeight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class PTargetAutoControlHeight : MonoBehaviour
  {
    public Vector3 StartPos = new Vector3(0.0f, -1f, 0.677f);
    private float[] HeightThresholds = new float[4]
    {
      0.9f,
      3f,
      5.7f,
      10f
    };
    private float[] SetToHeights = new float[4]
    {
      -1f,
      0.0f,
      4f,
      5f
    };
    private int m_currentHeightIndex = -1;

    private void Start()
    {
    }

    private void Update()
    {
      int num = -1;
      for (int index = 0; index < 4; ++index)
      {
        if ((double) GM.CurrentPlayerBody.Head.position.y < (double) this.HeightThresholds[index])
        {
          num = index;
          break;
        }
      }
      if (num < 0 || num == this.m_currentHeightIndex)
        return;
      this.m_currentHeightIndex = num;
      this.transform.position = new Vector3(this.StartPos.x, this.SetToHeights[this.m_currentHeightIndex], this.StartPos.z);
    }
  }
}
