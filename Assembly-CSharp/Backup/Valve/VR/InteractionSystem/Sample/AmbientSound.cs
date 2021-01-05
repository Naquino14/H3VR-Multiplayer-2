// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.AmbientSound
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class AmbientSound : MonoBehaviour
  {
    private AudioSource s;
    public float fadeintime;
    private float t;
    public bool fadeblack;
    private float vol;

    private void Start()
    {
      AudioListener.volume = 1f;
      this.s = this.GetComponent<AudioSource>();
      this.s.time = Random.Range(0.0f, this.s.clip.length);
      if ((double) this.fadeintime > 0.0)
        this.t = 0.0f;
      this.vol = this.s.volume;
      SteamVR_Fade.Start(Color.black, 0.0f);
      SteamVR_Fade.Start(Color.clear, 7f);
    }

    private void Update()
    {
      if ((double) this.fadeintime <= 0.0 || (double) this.t >= 1.0)
        return;
      this.t += Time.deltaTime / this.fadeintime;
      this.s.volume = this.t * this.vol;
    }
  }
}
