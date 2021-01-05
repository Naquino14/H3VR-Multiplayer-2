// Decompiled with JetBrains decompiler
// Type: FractureOnInput
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using DinoFracture;
using UnityEngine;

[RequireComponent(typeof (FractureGeometry))]
public class FractureOnInput : MonoBehaviour
{
  public KeyCode Key;

  private void Update()
  {
    if (!Input.GetKeyDown(this.Key))
      return;
    this.GetComponent<FractureGeometry>().Fracture();
  }
}
