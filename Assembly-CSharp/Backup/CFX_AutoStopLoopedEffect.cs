// Decompiled with JetBrains decompiler
// Type: CFX_AutoStopLoopedEffect
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (ParticleSystem))]
public class CFX_AutoStopLoopedEffect : MonoBehaviour
{
  public float effectDuration = 2.5f;
  private float d;

  private void OnEnable() => this.d = this.effectDuration;

  private void Update()
  {
    if ((double) this.d <= 0.0)
      return;
    this.d -= Time.deltaTime;
    if ((double) this.d > 0.0)
      return;
    this.GetComponent<ParticleSystem>().Stop(true);
    CFX_Demo_Translate component = this.gameObject.GetComponent<CFX_Demo_Translate>();
    if (!((Object) component != (Object) null))
      return;
    component.enabled = false;
  }
}
