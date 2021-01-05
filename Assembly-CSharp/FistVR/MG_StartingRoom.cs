// Decompiled with JetBrains decompiler
// Type: FistVR.MG_StartingRoom
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_StartingRoom : MonoBehaviour, IRoomTriggerable
  {
    private RedRoom m_room;
    public AlloyAreaLight MyLight;
    public Transform PlayerStartPos;
    public Transform[] Spawn_CardboardBox;
    public Transform Spawn_FlashLight;
    public Transform Spawn_AmbientLight;
    public Transform Spawn_Melee;
    public Transform Spawn_StartingPistol;
    public Transform Spawn_StartingPistolMag;
    public Transform Spawn_StartingShotgun;
    public Transform Spawn_StartingShotgunRounds;
    public Transform Spawn_ItemSpawner;

    public void Init(int size, RedRoom room) => this.m_room = room;

    public void Awake() => this.Invoke("StartAudio", 3f);

    public void Start()
    {
    }

    public void SetRoom(RedRoom room) => this.m_room = room;

    private void StartAudio()
    {
      if (GM.Options.MeatGrinderFlags.HasNarratorDoneLongIntro)
      {
        GM.MGMaster.Narrator.PlayIntroShort(GM.Options.MeatGrinderFlags.ShortIntroIndex);
        ++GM.Options.MeatGrinderFlags.ShortIntroIndex;
        GM.Options.MeatGrinderFlags.ShortIntroIndex = Mathf.Clamp(GM.Options.MeatGrinderFlags.ShortIntroIndex, 0, GM.Options.MeatGrinderFlags.MaxShortIntroIndex - 1);
        this.Invoke("UnlockDoorsAtEndOfIntro", 6f);
      }
      else
      {
        GM.MGMaster.Narrator.PlayIntroPt1();
        this.Invoke("PlayPt2Audio", 51f);
      }
    }

    private void PlayPt2Audio()
    {
      GM.MGMaster.Narrator.PlayIntroPt2();
      GM.Options.MeatGrinderFlags.HasNarratorDoneLongIntro = true;
      GM.Options.SaveToFile();
      this.Invoke("UnlockDoorsAtEndOfIntro", 47f);
    }

    private void EnablePlayer()
    {
    }

    private void UnlockDoorsAtEndOfIntro() => this.m_room.OpenDoors(true);

    public void CloseDoors() => this.m_room.CloseDoors(true);
  }
}
