// Decompiled with JetBrains decompiler
// Type: FistVR.MomStuff
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MomStuff : MonoBehaviour
  {
    public AudioClip[] OrnamentShatterSounds;
    private int m_ShatterIndex;
    public AudioSource MomSound;
    private float curHeight = -800f;
    private float tarHeight = -800f;
    private bool isSpeaking;

    private void Awake() => this.MomSound = this.GetComponent<AudioSource>();

    public void InitiateMom()
    {
      if (this.isSpeaking || this.MomSound.isPlaying || this.m_ShatterIndex >= this.OrnamentShatterSounds.Length)
        return;
      this.isSpeaking = true;
      this.tarHeight = -113f;
      this.MomSound.clip = this.OrnamentShatterSounds[this.m_ShatterIndex];
      this.Invoke("Speak", 2f);
      ++this.m_ShatterIndex;
    }

    private void Speak()
    {
      this.MomSound.Play();
      this.isSpeaking = false;
    }

    private void Update()
    {
      this.curHeight = !this.isSpeaking ? Mathf.Lerp(this.curHeight, this.tarHeight, Time.deltaTime * 0.2f) : Mathf.Lerp(this.curHeight, this.tarHeight, Time.deltaTime * 1f);
      this.transform.position = new Vector3(this.transform.position.x, this.curHeight, this.transform.position.z);
      if (this.MomSound.isPlaying || this.isSpeaking)
        return;
      this.tarHeight = -800f;
    }
  }
}
