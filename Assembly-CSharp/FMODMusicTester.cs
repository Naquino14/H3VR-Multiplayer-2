// Decompiled with JetBrains decompiler
// Type: FMODMusicTester
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODMusicTester : MonoBehaviour
{
  [EventRef]
  public string StateEvent;
  public EventInstance EventInstance;

  private void Start() => this.EventInstance = RuntimeManager.CreateInstance(this.StateEvent);

  private void Play()
  {
    int num = (int) this.EventInstance.start();
  }

  private void FadeOut()
  {
    int num = (int) this.EventInstance.stop(STOP_MODE.ALLOWFADEOUT);
  }

  private void StopImmediate()
  {
    int num = (int) this.EventInstance.stop(STOP_MODE.IMMEDIATE);
  }

  private void SetIntensity(float f)
  {
    int num = (int) this.EventInstance.setParameterValue("Intensity", f);
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.P))
      this.Play();
    if (Input.GetKeyDown(KeyCode.F))
      this.FadeOut();
    if (Input.GetKeyDown(KeyCode.S))
      this.StopImmediate();
    if (Input.GetKeyDown(KeyCode.Alpha0))
      this.SetIntensity(0.0f);
    if (Input.GetKeyDown(KeyCode.Alpha1))
      this.SetIntensity(1f);
    if (!Input.GetKeyDown(KeyCode.Alpha2))
      return;
    this.SetIntensity(2f);
  }
}
