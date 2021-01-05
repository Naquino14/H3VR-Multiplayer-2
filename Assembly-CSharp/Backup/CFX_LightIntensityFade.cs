// Decompiled with JetBrains decompiler
// Type: CFX_LightIntensityFade
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (Light))]
public class CFX_LightIntensityFade : MonoBehaviour
{
  public float duration = 1f;
  public float delay;
  public float finalIntensity;
  private float baseIntensity;
  public bool autodestruct;
  private float p_lifetime;
  private float p_delay;

  private void Start() => this.baseIntensity = this.GetComponent<Light>().intensity;

  private void OnEnable()
  {
    this.p_lifetime = 0.0f;
    this.p_delay = this.delay;
    if ((double) this.delay <= 0.0)
      return;
    this.GetComponent<Light>().enabled = false;
  }

  private void Update()
  {
    if ((double) this.p_delay > 0.0)
    {
      this.p_delay -= Time.deltaTime;
      if ((double) this.p_delay > 0.0)
        return;
      this.GetComponent<Light>().enabled = true;
    }
    else if ((double) this.p_lifetime / (double) this.duration < 1.0)
    {
      this.GetComponent<Light>().intensity = Mathf.Lerp(this.baseIntensity, this.finalIntensity, this.p_lifetime / this.duration);
      this.p_lifetime += Time.deltaTime;
    }
    else
    {
      if (!this.autodestruct)
        return;
      Object.Destroy((Object) this.gameObject);
    }
  }
}
