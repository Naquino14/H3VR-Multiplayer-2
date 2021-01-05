// Decompiled with JetBrains decompiler
// Type: FistVR.RedRoomEntryTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RedRoomEntryTrigger : MonoBehaviour
  {
    private bool m_hasTriggered;
    public RedRoom Room;

    private void OnTriggerEnter(Collider col)
    {
      if (this.m_hasTriggered)
        return;
      this.m_hasTriggered = true;
      this.Room.TriggerRoom();
    }
  }
}
