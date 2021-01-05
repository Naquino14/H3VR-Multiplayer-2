// Decompiled with JetBrains decompiler
// Type: CFX_Demo_RotateCamera
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class CFX_Demo_RotateCamera : MonoBehaviour
{
  public static bool rotating = true;
  public float speed = 30f;
  public Transform rotationCenter;

  private void Update()
  {
    if (!CFX_Demo_RotateCamera.rotating)
      return;
    this.transform.RotateAround(this.rotationCenter.position, Vector3.up, this.speed * Time.deltaTime);
  }
}
