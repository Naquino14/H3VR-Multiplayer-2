// Decompiled with JetBrains decompiler
// Type: FistVR.TR_SpinJackRoom
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TR_SpinJackRoom : MonoBehaviour, IRoomTriggerable
  {
    private RedRoom m_room;
    public GameObject[] SpinJackPrefabs;
    public AlloyAreaLight MyLight;
    private float curIntensity;
    private float tarIntensity;
    private bool m_isTriggered;
    private bool m_isDeactivated;
    public Transform[] JackSpawns_Size2;
    public Transform[] JackSpawns_Size3;
    public Transform[] JackSpawns_Size4;
    private List<TR_SpinJack> m_spawnedJacks = new List<TR_SpinJack>();

    public void Start()
    {
    }

    public void SetRoom(RedRoom room) => this.m_room = room;

    public void Init(int roomTileSize, RedRoom room)
    {
      if ((Object) room != (Object) null)
        this.m_room = room;
      GM.MGMaster.Narrator.PlayTrapRoomInit();
      this.GetComponent<AudioSource>().Play();
      if ((Object) this.m_room != (Object) null)
        this.m_room.CloseDoors(true);
      this.m_isTriggered = true;
      this.MyLight.gameObject.SetActive(true);
      this.tarIntensity = 0.75f;
      Transform[] transformArray = (Transform[]) null;
      switch (roomTileSize)
      {
        case 2:
          transformArray = this.JackSpawns_Size2;
          break;
        case 3:
          transformArray = this.JackSpawns_Size3;
          break;
        case 4:
          transformArray = this.JackSpawns_Size4;
          break;
      }
      for (int index = 0; index < transformArray.Length; ++index)
        this.m_spawnedJacks.Add(Object.Instantiate<GameObject>(this.SpinJackPrefabs[index], transformArray[index].position, transformArray[index].rotation).GetComponent<TR_SpinJack>());
    }

    public void Update()
    {
      if (this.m_isTriggered)
      {
        if ((double) this.curIntensity < (double) this.tarIntensity)
        {
          this.curIntensity += Time.deltaTime * 0.3f;
          this.MyLight.Intensity = this.curIntensity;
        }
      }
      else if (this.m_isDeactivated)
      {
        if ((double) this.curIntensity > (double) this.tarIntensity)
        {
          this.curIntensity -= Time.deltaTime * 0.2f;
          this.MyLight.Intensity = this.curIntensity;
        }
        else
          this.MyLight.gameObject.SetActive(false);
      }
      for (int index = this.m_spawnedJacks.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_spawnedJacks[index] == (Object) null)
          this.m_spawnedJacks.RemoveAt(index);
      }
      if (!this.m_isTriggered || this.m_spawnedJacks.Count > 0 || this.m_isDeactivated)
        return;
      this.TrapOver();
    }

    private void TrapOver()
    {
      Debug.Log((object) "trap over");
      this.m_isDeactivated = true;
      this.m_room.OpenDoors(true);
    }
  }
}
