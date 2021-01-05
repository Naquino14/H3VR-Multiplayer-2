// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigMusicVolumeTester
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigMusicVolumeTester : MonoBehaviour
  {
    public ZosigGameManager M;
    public List<ZosigMusicVolume> VolumeList = new List<ZosigMusicVolume>();
    private ZosigMusicVolume m_curVolume;
    private int m_startingIndex;

    private void Start() => this.m_curVolume = this.VolumeList[0];

    private void Update()
    {
      ++this.m_startingIndex;
      if (this.m_startingIndex >= this.VolumeList.Count)
        this.m_startingIndex = 0;
      if (!this.TestVolumeBool(this.VolumeList[this.m_startingIndex], GM.CurrentPlayerBody.Head.position))
        return;
      this.m_curVolume = this.VolumeList[this.m_startingIndex];
      this.M.SetMusicTrack(this.m_curVolume.TrackName);
    }

    public bool TestVolumeBool(ZosigMusicVolume z, Vector3 pos)
    {
      bool flag = true;
      Vector3 vector3 = z.transform.InverseTransformPoint(pos);
      if ((double) Mathf.Abs(vector3.x) > 0.5 || (double) Mathf.Abs(vector3.y) > 0.5 || (double) Mathf.Abs(vector3.z) > 0.5)
        flag = false;
      return flag;
    }
  }
}
