// Decompiled with JetBrains decompiler
// Type: FistVR.MR_CrankPuzzle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MR_CrankPuzzle : MonoBehaviour, IRoomTriggerable, IMeatRoomAble
  {
    public MG_MeatChunk MeatChunk;
    private bool m_HasMeatBeenFreed;
    private RedRoom m_room;
    private float checkLightDistanceTick;
    private int m_MeatID;
    private bool m_hasBeenTriggered;

    private void Awake() => this.MeatChunk.transform.localRotation = Random.rotation;

    public void SetRoom(RedRoom room) => this.m_room = room;

    public void Init(int roomTileSize, RedRoom room)
    {
      this.m_room = room;
      this.m_hasBeenTriggered = true;
      GM.MGMaster.Narrator.PlayMeatDiscover(this.m_MeatID);
    }

    public void SetMeatID(int i)
    {
      this.m_MeatID = i;
      this.MeatChunk.MeatID = i;
    }
  }
}
