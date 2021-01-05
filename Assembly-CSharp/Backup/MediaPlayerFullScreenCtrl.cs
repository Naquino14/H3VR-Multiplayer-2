// Decompiled with JetBrains decompiler
// Type: MediaPlayerFullScreenCtrl
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class MediaPlayerFullScreenCtrl : MonoBehaviour
{
  public GameObject m_objVideo;
  private int m_iOrgWidth;
  private int m_iOrgHeight;

  private void Start() => this.Resize();

  private void Update()
  {
    if (this.m_iOrgWidth != Screen.width)
      this.Resize();
    if (this.m_iOrgHeight == Screen.height)
      return;
    this.Resize();
  }

  private void Resize()
  {
    this.m_iOrgWidth = Screen.width;
    this.m_iOrgHeight = Screen.height;
    float num = (float) this.m_iOrgHeight / (float) this.m_iOrgWidth;
    this.m_objVideo.transform.localScale = new Vector3(20f / num, 20f / num, 1f);
    this.m_objVideo.transform.GetComponent<MediaPlayerCtrl>().Resize();
  }
}
