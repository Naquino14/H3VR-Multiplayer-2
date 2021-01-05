// Decompiled with JetBrains decompiler
// Type: FistVR.RedRoom
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RedRoom : MonoBehaviour
  {
    public bool HasBeenConfigured;
    public RedRoom.RedRoomType RoomType;
    public int RoomSize;
    public GameObject Triggerable;
    public bool DoDoorsShutWhenTriggered;
    public GameObject EntryTrigger;
    public AudioEvent AudEvent_DoorOpen;
    public AudioEvent AudEvent_DoorClose;
    public RedRoom.Quadrant MyQuadrant;
    private bool m_hasTriggered;
    public Transform[] Doors;

    private void Awake()
    {
      if (this.RoomType == RedRoom.RedRoomType.Starting)
        return;
      this.OpenDoors(false);
    }

    public void SetRoomType(RedRoom.RedRoomType t)
    {
      this.RoomType = t;
      switch (t)
      {
        case RedRoom.RedRoomType.Starting:
          this.DoDoorsShutWhenTriggered = false;
          break;
        case RedRoom.RedRoomType.Trap:
          this.DoDoorsShutWhenTriggered = true;
          break;
        case RedRoom.RedRoomType.Meat:
          this.DoDoorsShutWhenTriggered = false;
          break;
      }
    }

    public void TriggerRoom()
    {
      if (this.m_hasTriggered)
        return;
      this.m_hasTriggered = true;
      if (this.Triggerable.GetComponent<IRoomTriggerable>() != null)
        this.Triggerable.GetComponent<IRoomTriggerable>()?.Init(this.RoomSize, this);
      if (this.DoDoorsShutWhenTriggered)
        this.CloseDoors(true);
      this.EntryTrigger.SetActive(false);
    }

    public void SetTriggerable(GameObject trig)
    {
      this.Triggerable = trig;
      this.Triggerable.transform.position = this.transform.position;
    }

    public void OpenDoors(bool playSound)
    {
      if (playSound)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_DoorOpen, this.transform.position);
      for (int index = 0; index < this.Doors.Length; ++index)
        this.Doors[index].localPosition = new Vector3(this.Doors[index].localPosition.x, 2.2f, this.Doors[index].localPosition.z);
    }

    public void CloseDoors(bool playSound)
    {
      if (playSound)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_DoorClose, this.transform.position);
      for (int index = 0; index < this.Doors.Length; ++index)
        this.Doors[index].localPosition = new Vector3(this.Doors[index].localPosition.x, 0.0f, this.Doors[index].localPosition.z);
    }

    public enum RedRoomType
    {
      Starting,
      Trap,
      Meat,
      MonsterCloset,
      Items,
    }

    public enum Quadrant
    {
      None,
      Office,
      Boiler,
      ColdStorage,
      Restaurant,
    }
  }
}
