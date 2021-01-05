// Decompiled with JetBrains decompiler
// Type: FistVR.InvasionDog
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class InvasionDog : MonoBehaviour
  {
    public ConfigurableSoldierBotSpawner Spawner;
    public FVRPhysicalObject PO;
    public GameObject Sound;

    private void Update()
    {
      if (!this.PO.IsHeld || (double) Vector3.Distance(GM.CurrentPlayerBody.Head.position + Vector3.up * -0.15f, this.transform.position) >= 0.150000005960464)
        return;
      this.BS();
    }

    public void BS()
    {
      GameObject.Find("_AudioMusic").GetComponent<MM_MusicManager>().PlayMusic();
      Object.Instantiate<GameObject>(this.Sound, this.transform.position, this.transform.rotation);
      this.Spawner.SpawnGronch();
      Object.Destroy((Object) this.gameObject);
    }
  }
}
