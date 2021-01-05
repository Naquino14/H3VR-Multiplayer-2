// Decompiled with JetBrains decompiler
// Type: FistVR.SlicerThrusterSoundController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SlicerThrusterSoundController : MonoBehaviour
  {
    public AudioSource ThrusterAudio;
    private float m_curThrusterAudioIntensity;
    private float m_tarThrusterAudioIntensity;
    public AIThrusterControlBox[] ControlBoxes;

    private void Update()
    {
      float num1 = 0.0f;
      for (int index1 = 0; index1 < this.ControlBoxes.Length; ++index1)
      {
        if ((Object) this.ControlBoxes[index1] != (Object) null)
        {
          for (int index2 = 0; index2 < this.ControlBoxes[index1].Thrusters.Length; ++index2)
          {
            if ((Object) this.ControlBoxes[index1].Thrusters[index2] != (Object) null)
              num1 += this.ControlBoxes[index1].Thrusters[index2].GetMagnitude();
          }
        }
      }
      this.m_tarThrusterAudioIntensity = num1 * 0.35f;
      float num2;
      if ((double) this.m_tarThrusterAudioIntensity > (double) this.m_curThrusterAudioIntensity)
      {
        this.m_curThrusterAudioIntensity = Mathf.Lerp(this.m_curThrusterAudioIntensity, this.m_tarThrusterAudioIntensity, Time.deltaTime * 5f);
        num2 = Mathf.Clamp((float) (((double) this.m_tarThrusterAudioIntensity - (double) this.m_curThrusterAudioIntensity) * 0.150000005960464), 0.0f, 0.15f);
      }
      else
      {
        this.m_curThrusterAudioIntensity = Mathf.Lerp(this.m_curThrusterAudioIntensity, this.m_tarThrusterAudioIntensity, Time.deltaTime * 1f);
        num2 = -Mathf.Clamp((float) (((double) this.m_curThrusterAudioIntensity - (double) this.m_tarThrusterAudioIntensity) * 0.150000005960464), 0.0f, 0.15f);
      }
      this.ThrusterAudio.pitch = 1f + num2;
      this.ThrusterAudio.volume = this.m_curThrusterAudioIntensity * 0.15f;
    }
  }
}
