// Decompiled with JetBrains decompiler
// Type: FistVR.MG_MeatMachine
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_MeatMachine : MonoBehaviour
  {
    public MG_StartingRoom room;
    public ParticleSystem PSystem_Sparks;
    public GameObject[] SpawnOnMeat;
    public Transform SpawnPoint;
    private int m_meatFedIn;
    private bool m_hasPlayedFinal;
    public Transform[] Rollers;
    private float m_roll;

    private void Update()
    {
      this.m_roll += Time.deltaTime * 1720f;
      this.m_roll = Mathf.Repeat(this.m_roll, 360f);
      this.Rollers[0].localEulerAngles = new Vector3(0.0f, 0.0f, -this.m_roll);
      this.Rollers[1].localEulerAngles = new Vector3(0.0f, 0.0f, this.m_roll);
    }

    public void FedMeatIn(int id)
    {
      GM.MGMaster.Narrator.PlayMeatFeedIn(id);
      ++this.m_meatFedIn;
      if (this.m_meatFedIn == 1)
      {
        GM.MGMaster.MeatRoom2.OpenDoors(true);
        GM.MGMaster.SpawnBadGuySet1();
      }
      if (this.m_meatFedIn == 2)
      {
        GM.MGMaster.MeatRoom3.OpenDoors(true);
        GM.MGMaster.SpawnBadGuySet2();
      }
      if (this.m_meatFedIn >= 3)
      {
        GM.MGMaster.IsCountingDown = false;
        this.PlayFinalWon();
      }
      this.PSystem_Sparks.Emit(50);
      FXM.InitiateMuzzleFlash(this.PSystem_Sparks.transform.position, this.PSystem_Sparks.transform.forward, 5f, Color.white, 0.7f);
      for (int index = 0; index < this.SpawnOnMeat.Length; ++index)
        Object.Instantiate<GameObject>(this.SpawnOnMeat[index], this.SpawnPoint.position, this.SpawnPoint.rotation);
    }

    public void FedObjIn()
    {
      FXM.InitiateMuzzleFlash(this.PSystem_Sparks.transform.position, this.PSystem_Sparks.transform.forward, 5f, Color.white, 0.7f);
      this.PSystem_Sparks.Emit(150);
    }

    public void PlayFinalWon()
    {
      if (!this.m_hasPlayedFinal)
      {
        this.m_hasPlayedFinal = true;
        this.Invoke("DelayedPlay", 13f);
      }
      this.room.CloseDoors();
    }

    private void DelayedPlay()
    {
      if (GM.Options.MeatGrinderFlags.HasPlayerEverWon)
      {
        GM.MGMaster.Narrator.PlayWonAgain(GM.Options.MeatGrinderFlags.SuccessEventVoiceIndex);
        if (GM.Options.MeatGrinderFlags.SuccessEventVoiceIndex <= 2)
          this.Invoke("LoadMainMenu", 30f);
        else
          this.Invoke("LoadMainMenu", 8f);
        ++GM.Options.MeatGrinderFlags.SuccessEventVoiceIndex;
        GM.Options.MeatGrinderFlags.SuccessEventVoiceIndex = Mathf.Clamp(GM.Options.MeatGrinderFlags.SuccessEventVoiceIndex, 0, GM.Options.MeatGrinderFlags.MaxSuccessEventVoiceIndex - 1);
      }
      else
      {
        GM.MGMaster.Narrator.PlayWonFirstTime();
        GM.Options.MeatGrinderFlags.HasPlayerEverWon = true;
        GM.Options.SaveToFile();
        this.Invoke("LoadMainMenu", 65f);
      }
    }

    private void LoadMainMenu() => SteamVR_LoadLevel.Begin("MainMenu3");
  }
}
