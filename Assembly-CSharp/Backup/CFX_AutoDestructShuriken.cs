// Decompiled with JetBrains decompiler
// Type: CFX_AutoDestructShuriken
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof (ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
  public bool OnlyDeactivate;

  private void OnEnable() => this.StartCoroutine("CheckIfAlive");

  [DebuggerHidden]
  private IEnumerator CheckIfAlive() => (IEnumerator) new CFX_AutoDestructShuriken.\u003CCheckIfAlive\u003Ec__Iterator0()
  {
    \u0024this = this
  };
}
