// Decompiled with JetBrains decompiler
// Type: FistVR.wwSilverBulletMoving
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwSilverBulletMoving : MonoBehaviour
  {
    public float Speed = 90f;

    private void Start()
    {
    }

    private void Update()
    {
      this.transform.position = this.transform.position + Vector3.up * this.Speed * Time.deltaTime;
      if ((double) this.transform.position.y <= 1000.0)
        return;
      Object.Destroy((Object) this.gameObject);
    }
  }
}
