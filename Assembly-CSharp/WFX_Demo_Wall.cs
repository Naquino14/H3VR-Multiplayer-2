// Decompiled with JetBrains decompiler
// Type: WFX_Demo_Wall
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class WFX_Demo_Wall : MonoBehaviour
{
  public WFX_Demo_New demo;

  private void OnMouseDown()
  {
    RaycastHit hitInfo = new RaycastHit();
    if (!this.GetComponent<Collider>().Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 9999f))
      return;
    GameObject gameObject = this.demo.spawnParticle();
    gameObject.transform.position = hitInfo.point;
    gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hitInfo.normal);
  }
}
