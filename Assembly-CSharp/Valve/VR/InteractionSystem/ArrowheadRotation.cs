// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.ArrowheadRotation
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class ArrowheadRotation : MonoBehaviour
  {
    private void Start() => this.transform.localEulerAngles = new Vector3(Random.Range(0.0f, 180f), -90f, 90f);
  }
}
