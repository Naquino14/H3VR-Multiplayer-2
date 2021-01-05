// Decompiled with JetBrains decompiler
// Type: FistVR.D6Room
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class D6Room : MonoBehaviour, IRoomTriggerable
  {
    public RedRoom m_room;
    private bool m_isTriggered;
    public GameObject Plinth;
    public MG_D6Lotto D6;

    public void Init(int size, RedRoom room)
    {
      this.m_room = room;
      this.D6.m_room = room;
      if (!this.m_isTriggered)
        this.m_isTriggered = true;
      this.m_room.CloseDoors(true);
      this.Plinth.SetActive(true);
      this.D6.gameObject.SetActive(true);
    }

    public void SetRoom(RedRoom room) => this.m_room = room;
  }
}
