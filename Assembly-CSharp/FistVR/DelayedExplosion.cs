// Decompiled with JetBrains decompiler
// Type: FistVR.DelayedExplosion
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class DelayedExplosion : MonoBehaviour
  {
    public float FuseTime = 1f;
    private bool HasSploded;
    public GameObject VFXPrefab;
    public GameObject ForcePrefab;

    private void Awake() => this.FuseTime *= Random.Range(0.6f, 1.2f);

    private void Update()
    {
      if (this.HasSploded)
        return;
      this.FuseTime -= Time.deltaTime;
      if ((double) this.FuseTime > 0.0)
        return;
      this.HasSploded = true;
      Object.Instantiate<GameObject>(this.VFXPrefab, this.transform.position, Quaternion.identity);
      Object.Instantiate<GameObject>(this.ForcePrefab, this.transform.position, Quaternion.identity);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
