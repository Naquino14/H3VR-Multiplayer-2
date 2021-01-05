// Decompiled with JetBrains decompiler
// Type: MedaiPlayerSampleSphereGUI
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class MedaiPlayerSampleSphereGUI : MonoBehaviour
{
  public MediaPlayerCtrl scrMedia;

  private void Start()
  {
  }

  private void Update()
  {
    foreach (Touch touch in Input.touches)
      this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y + touch.deltaPosition.x, this.transform.localEulerAngles.z);
  }

  private void OnGUI()
  {
    if (GUI.Button(new Rect(50f, 50f, 100f, 100f), "Load"))
      this.scrMedia.Load("EasyMovieTexture.mp4");
    if (GUI.Button(new Rect(50f, 200f, 100f, 100f), "Play"))
      this.scrMedia.Play();
    if (GUI.Button(new Rect(50f, 350f, 100f, 100f), "stop"))
      this.scrMedia.Stop();
    if (GUI.Button(new Rect(50f, 500f, 100f, 100f), "pause"))
      this.scrMedia.Pause();
    if (!GUI.Button(new Rect(50f, 650f, 100f, 100f), "Unload"))
      return;
    this.scrMedia.UnLoad();
  }
}
