// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigRIsen
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ZosigRIsen : MonoBehaviour
  {
    private float speed;

    private void Start()
    {
    }

    private void Update()
    {
      if ((double) this.speed < 3.0)
        this.speed += Time.deltaTime * 0.25f;
      else if ((double) this.speed < 50.0)
        this.speed += Time.deltaTime;
      this.transform.Translate(Vector3.up * this.speed * Time.deltaTime);
      if ((double) this.transform.position.y <= 600.0)
        return;
      Object.Destroy((Object) this.gameObject);
      GM.ZMaster.FlagM.SetFlag("lj_free", 2);
    }
  }
}
