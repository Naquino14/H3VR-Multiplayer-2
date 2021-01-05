// Decompiled with JetBrains decompiler
// Type: RandomPitchAwake
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class RandomPitchAwake : MonoBehaviour
{
  public AudioSource aud;
  public float min = 0.9f;
  public float max = 1.1f;

  private void Awake() => this.aud.pitch = Random.Range(this.min, this.max);

  private void Update()
  {
  }
}
