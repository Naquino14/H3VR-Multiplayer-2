// Decompiled with JetBrains decompiler
// Type: FistVR.HeadTriggerDetect
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class HeadTriggerDetect : MonoBehaviour
  {
    public ObstacleCourseGame Game;

    private void Awake()
    {
      if (!((Object) Object.FindObjectOfType<ObstacleCourseGame>() != (Object) null))
        return;
      this.Game = Object.FindObjectOfType<ObstacleCourseGame>();
    }

    private void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.layer != LayerMask.NameToLayer("ColOnlyHead") || !((Object) this.Game != (Object) null))
        return;
      this.Game.RegisterHeadPenalty();
    }
  }
}
