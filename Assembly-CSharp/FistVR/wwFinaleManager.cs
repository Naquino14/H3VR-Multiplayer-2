// Decompiled with JetBrains decompiler
// Type: FistVR.wwFinaleManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class wwFinaleManager : MonoBehaviour
  {
    public wwParkManager ParkManager;
    public GameObject OutdoorLight;
    public GameObject[] FinaleLights;
    public GameObject[] BlackOuts;
    public wwFinaleDoor[] Doors;
    public Transform EndSoundsPlacement;
    private bool m_isEndingMonologuePlaying;
    public float m_endingMonologueTick;
    public wwFinaleManager.EndSequenceSoundEvent[] End_Marc;
    public wwFinaleManager.EndSequenceSoundEvent[] End_SoundEvents;
    public wwFinaleManager.EndSequenceLightEvent[] End_LightEvents;
    public AudioSource AUD_MechanicalLoop;
    public AudioSource AUD_Ending_Marc;
    public AudioSource AUD_Ending_Sounds;
    public Transform ParkModel;
    public Vector3 ParkModelUp;
    public Vector3 ParkModelDown;
    private float m_parkModelLerp;
    public Transform BotDoor;
    public Vector3 BotDoorUp;
    public Vector3 BotDoorDown;
    private float m_botDoorLerp;
    public GameObject FinalBot;
    public Rigidbody[] FinalBotPieces;
    public Transform BotPoint1;
    public Transform BotPoint2;
    public Transform BotPoint3;
    private bool hasBotSploded;
    public Transform ExplosionPoint;
    public GameObject ExplosionPrefab;
    private bool m_hasMonologueConcluded;

    public void SwitchToFinaleLight(int index)
    {
      this.OutdoorLight.SetActive(false);
      for (int index1 = 0; index1 < this.FinaleLights.Length; ++index1)
      {
        if (index1 == index)
          this.FinaleLights[index1].SetActive(true);
        else
          this.FinaleLights[index1].SetActive(false);
      }
    }

    public void DisableAllFinaleLights()
    {
      foreach (GameObject finaleLight in this.FinaleLights)
        finaleLight.SetActive(false);
    }

    public void EnableOutdoorLight() => this.OutdoorLight.SetActive(true);

    public void OpenDoor(int index)
    {
      this.BlackOuts[index].SetActive(false);
      this.Doors[index].OpenDoor();
    }

    private void Start()
    {
    }

    public void BeginEnding()
    {
      if (this.m_hasMonologueConcluded || this.m_isEndingMonologuePlaying)
        return;
      this.AUD_MechanicalLoop.Play();
      this.m_isEndingMonologuePlaying = true;
      this.Doors[6].ConfigureDoorState(0);
    }

    public void ConfigureBlackOuts(int[] b)
    {
      for (int index = 0; index < b.Length; ++index)
      {
        if (b[index] == 0)
          this.BlackOuts[index].SetActive(true);
        else
          this.BlackOuts[index].SetActive(false);
      }
    }

    private void Update()
    {
      if (!this.m_isEndingMonologuePlaying)
        return;
      this.EndingSequencer();
    }

    private void EndingSequencer()
    {
      this.m_endingMonologueTick += Time.deltaTime;
      if ((double) this.m_endingMonologueTick > 56.2000007629395 && (double) this.m_parkModelLerp < 1.0)
      {
        if (!this.ParkModel.gameObject.activeSelf)
          this.ParkModel.gameObject.SetActive(true);
        this.m_parkModelLerp += Time.deltaTime * 0.008f;
        this.ParkModel.transform.localPosition = Vector3.Lerp(this.ParkModelUp, this.ParkModelDown, this.m_parkModelLerp);
      }
      if ((double) this.m_endingMonologueTick > 201.0 && (double) this.m_botDoorLerp < 1.0)
      {
        if ((UnityEngine.Object) this.FinalBot != (UnityEngine.Object) null && !this.FinalBot.activeSelf)
          this.FinalBot.SetActive(true);
        this.m_botDoorLerp += Time.deltaTime * 0.175f;
        this.BotDoor.transform.localPosition = Vector3.Lerp(this.BotDoorUp, this.BotDoorDown, this.m_botDoorLerp);
      }
      if ((double) this.m_endingMonologueTick >= 209.0 && (double) this.m_endingMonologueTick < 213.0)
        this.FinalBot.transform.position = Vector3.Lerp(this.BotPoint1.position, this.BotPoint2.position, (float) (((double) this.m_endingMonologueTick - 209.0) * 0.25));
      else if ((double) this.m_endingMonologueTick >= 213.0 && (double) this.m_endingMonologueTick < 215.0)
      {
        float num = (float) (((double) this.m_endingMonologueTick - 213.0) * 0.5);
        this.FinalBot.transform.position = Vector3.Lerp(this.BotPoint2.position, this.BotPoint3.position, num * num);
        this.FinalBot.transform.rotation = Quaternion.Slerp(this.BotPoint2.rotation, this.BotPoint3.rotation, num * num);
      }
      else if ((double) this.m_endingMonologueTick > 215.0 && !this.hasBotSploded)
      {
        this.hasBotSploded = true;
        foreach (Rigidbody finalBotPiece in this.FinalBotPieces)
        {
          finalBotPiece.gameObject.SetActive(true);
          finalBotPiece.transform.SetParent((Transform) null);
        }
        UnityEngine.Object.Instantiate<GameObject>(this.ExplosionPrefab, this.ExplosionPoint.position, this.ExplosionPoint.rotation);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.FinalBot);
      }
      if ((double) this.m_endingMonologueTick > 215.0 && this.AUD_MechanicalLoop.isPlaying)
      {
        this.AUD_MechanicalLoop.Stop();
        if (this.ParkModel.gameObject.activeSelf)
          this.ParkModel.gameObject.SetActive(false);
      }
      for (int index = 0; index < this.End_Marc.Length; ++index)
      {
        if (!this.End_Marc[index].HasPlayed && (double) this.m_endingMonologueTick > (double) this.End_Marc[index].TimeIndex && (double) this.m_endingMonologueTick < (double) this.End_Marc[index].TimeIndex + 1.0)
        {
          this.AUD_Ending_Marc.PlayOneShot(this.End_Marc[index].Clip, this.End_Marc[index].Volume);
          this.End_Marc[index].HasPlayed = true;
        }
      }
      for (int index = 0; index < this.End_SoundEvents.Length; ++index)
      {
        if (!this.End_SoundEvents[index].HasPlayed && (double) this.m_endingMonologueTick > (double) this.End_SoundEvents[index].TimeIndex && (double) this.m_endingMonologueTick < (double) this.End_SoundEvents[index].TimeIndex + 1.0)
        {
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.End_SoundEvents[index].AudioEvent, this.EndSoundsPlacement.position);
          this.End_SoundEvents[index].HasPlayed = true;
        }
      }
      for (int index = 0; index < this.End_LightEvents.Length; ++index)
      {
        if (!this.End_LightEvents[index].HasFired && (double) this.m_endingMonologueTick > (double) this.End_LightEvents[index].TimeIndex && (double) this.m_endingMonologueTick < (double) this.End_LightEvents[index].TimeIndex + 1.0)
        {
          this.SwitchToFinaleLight(this.End_LightEvents[index].LightToSwitchTo);
          this.End_LightEvents[index].HasFired = true;
        }
      }
      if ((double) this.m_endingMonologueTick <= 260.0 || this.m_hasMonologueConcluded)
        return;
      this.m_hasMonologueConcluded = true;
      this.m_isEndingMonologuePlaying = false;
      this.Doors[6].ConfigureDoorState(1);
    }

    [Serializable]
    public class EndSequenceSoundEvent
    {
      public AudioEvent AudioEvent;
      public AudioClip Clip;
      public bool HasPlayed;
      public float TimeIndex;
      public float Volume = 1f;
    }

    [Serializable]
    public class EndSequenceLightEvent
    {
      public float TimeIndex;
      public int LightToSwitchTo;
      public bool HasFired;
    }
  }
}
