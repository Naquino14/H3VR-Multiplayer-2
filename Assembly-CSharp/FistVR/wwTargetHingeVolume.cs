// Decompiled with JetBrains decompiler
// Type: FistVR.wwTargetHingeVolume
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwTargetHingeVolume : MonoBehaviour
  {
    public AudioSource Aud;
    public HingeJoint Joint;
    private float m_lastAngle;
    public float volMult;
    public float volCutoff;
    public Vector2 PitchClamp;
    public float pitchMult;
    public float pitchMod;

    private void Update()
    {
      float angle = this.Joint.angle;
      float num = Mathf.Abs(angle - this.m_lastAngle) * Time.deltaTime;
      if ((Object) this.Aud != (Object) null && this.Aud.isPlaying)
      {
        this.Aud.volume = num * this.volMult - this.volCutoff;
        this.Aud.pitch = Mathf.Clamp(num * this.pitchMult - this.pitchMod, this.PitchClamp.x, this.PitchClamp.y);
      }
      this.m_lastAngle = angle;
    }
  }
}
