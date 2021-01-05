// Decompiled with JetBrains decompiler
// Type: MedaiPlayerSampleGUI
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class MedaiPlayerSampleGUI : MonoBehaviour
{
  public MediaPlayerCtrl scrMedia;
  public bool m_bFinish;

  private void Start() => this.scrMedia.OnEnd += new MediaPlayerCtrl.VideoEnd(this.OnEnd);

  private void Update()
  {
  }

  private void OnGUI()
  {
    if (GUI.Button(new Rect(50f, 50f, 100f, 100f), "Load"))
    {
      this.scrMedia.Load("EasyMovieTexture.mp4");
      this.m_bFinish = false;
    }
    if (GUI.Button(new Rect(50f, 200f, 100f, 100f), "Play"))
    {
      this.scrMedia.Play();
      this.m_bFinish = false;
    }
    if (GUI.Button(new Rect(50f, 350f, 100f, 100f), "stop"))
      this.scrMedia.Stop();
    if (GUI.Button(new Rect(50f, 500f, 100f, 100f), "pause"))
      this.scrMedia.Pause();
    if (GUI.Button(new Rect(50f, 650f, 100f, 100f), "Unload"))
      this.scrMedia.UnLoad();
    if (!GUI.Button(new Rect(50f, 800f, 100f, 100f), " " + (object) this.m_bFinish))
      ;
    if (GUI.Button(new Rect(200f, 50f, 100f, 100f), "SeekTo"))
      this.scrMedia.SeekTo(10000);
    if (this.scrMedia.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
    {
      if (GUI.Button(new Rect(200f, 200f, 100f, 100f), this.scrMedia.GetSeekPosition().ToString()))
        this.scrMedia.SetSpeed(2f);
      if (GUI.Button(new Rect(200f, 350f, 100f, 100f), this.scrMedia.GetDuration().ToString()))
        this.scrMedia.SetSpeed(1f);
      if (!GUI.Button(new Rect(200f, 450f, 100f, 100f), this.scrMedia.GetVideoWidth().ToString()))
        ;
      if (!GUI.Button(new Rect(200f, 550f, 100f, 100f), this.scrMedia.GetVideoHeight().ToString()))
        ;
    }
    if (!GUI.Button(new Rect(200f, 650f, 100f, 100f), this.scrMedia.GetCurrentSeekPercent().ToString()))
      ;
  }

  private void OnEnd() => this.m_bFinish = true;
}
