// Decompiled with JetBrains decompiler
// Type: FistVR.MG_Narrator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_Narrator : MonoBehaviour
  {
    public MeatGrinderMaster Master;
    [Header("Intercom System")]
    public Transform[] Intercoms;
    public AudioSource AUD;
    private bool m_isPlaying;
    [Header("Intro")]
    public AudioClip AC_Pt1;
    public AudioClip AC_Pt2;
    public AudioClip[] AC_IntrosShort;
    [Header("Trap Rooms")]
    public AudioClip[] AC_TrapRoomInit;
    public AudioClip[] AC_TrapRoomFailing;
    [Header("MeatRoom Sets")]
    public AudioClip[] AC_MeatRoomDiscover;
    public AudioClip[] AC_MeatRoomAcquire;
    public AudioClip[] AC_MeatRoomFeedIn;
    [Header("Monster Closet")]
    public AudioClip[] AC_MonsterCloset;
    public AudioClip[] AC_MonsterRebuilt;
    [Header("JumpScareSets")]
    public AudioClip[] AC_JumpScareReactions;
    [Header("AreaEntry")]
    public AudioClip[] AC_AreaEntryBoiler;
    public AudioClip[] AC_ColdStorage;
    public AudioClip[] AC_Office;
    public AudioClip[] AC_Restaurant;
    [Header("FoundThings")]
    public AudioClip[] AC_FoundSpecialItem;
    public AudioClip[] AC_FoundJunkItem;
    public AudioClip[] AC_FoundNormalWeapon;
    public AudioClip[] AC_FoundRareWeapon;
    [Header("Death")]
    public AudioClip[] AC_PlayerAboutToDie;
    public AudioClip[] AC_PlayerDiedCheating;
    public AudioClip[] AC_PlayerDiedOutOfHealth;
    [Header("Winning")]
    public AudioClip AC_WonFirstTime;
    public AudioClip[] AC_WonAgain;
    [Header("TimeRemaining")]
    public AudioClip[] AC_TimeRemaining;
    private int curClipPriority;
    private int m_curTrapRoomInitIndex;
    private int m_curTrapRoomFailingIndex;
    private int m_curAEBoilerIndex;
    private int m_curAEColdStorageIndex;
    private int m_curAEOfficeIndex;
    private int m_curAERestaurantIndex;
    private int m_curFoundSpecialItemIndex;
    private int m_curFoundJunkItemIndex;
    private int m_curFoundNormalItemIndex;
    private int m_curFoundRareItemIndex;
    private int m_curAboutToDieIndex;
    private float TimeSinceObjectHook;
    private float TimeSinceAreaHook;
    private float TimeSinceHealthHook;
    private float TimeSinceJumpScareHook;
    private float TimeSinceTrapRoomFailingHook;
    private float m_intercomCheckTick = 1f;
    private Transform m_curIntercom;
    private Transform m_lastIntercom;

    private void Awake()
    {
      this.ShuffleClips(this.AC_TrapRoomInit);
      this.ShuffleClips(this.AC_TrapRoomFailing);
      this.ShuffleClips(this.AC_FoundSpecialItem);
      this.ShuffleClips(this.AC_FoundJunkItem);
      this.ShuffleClips(this.AC_FoundNormalWeapon);
      this.ShuffleClips(this.AC_FoundRareWeapon);
      this.ShuffleClips(this.AC_PlayerAboutToDie);
      this.CheckForClosestIntercom();
      if (GM.Options.MeatGrinderFlags.NarratorMode != MeatGrinderFlags.MeatGrinderNarratorMode.Silent)
        return;
      this.AUD.volume = 0.0f;
    }

    private bool CanPlay(int i) => GM.Options.MeatGrinderFlags.NarratorMode != MeatGrinderFlags.MeatGrinderNarratorMode.Silent && (GM.Options.MeatGrinderFlags.NarratorMode != MeatGrinderFlags.MeatGrinderNarratorMode.Terse || i >= 4) && (i >= this.curClipPriority || !this.AUD.isPlaying);

    public void PlayIntroPt1()
    {
      this.curClipPriority = 4;
      this.AUD.clip = this.AC_Pt1;
      this.AUD.Play();
    }

    public void PlayIntroPt2()
    {
      this.curClipPriority = 4;
      this.AUD.clip = this.AC_Pt2;
      this.AUD.Play();
      GM.Options.MeatGrinderFlags.HasNarratorDoneLongIntro = true;
    }

    public void PlayIntroShort(int i)
    {
      this.curClipPriority = 4;
      this.AUD.clip = this.AC_IntrosShort[i];
      this.AUD.Play();
    }

    public void PlayWonFirstTime()
    {
      this.curClipPriority = 5;
      this.AUD.clip = this.AC_WonFirstTime;
      this.AUD.Play();
    }

    public void PlayWonAgain(int i)
    {
      this.curClipPriority = 5;
      this.AUD.clip = this.AC_WonAgain[i];
      this.AUD.Play();
    }

    public void PlayTrapRoomInit()
    {
      if (!this.CanPlay(3) || this.m_curTrapRoomInitIndex >= this.AC_TrapRoomInit.Length)
        return;
      this.curClipPriority = 3;
      this.AUD.clip = this.AC_TrapRoomInit[this.m_curTrapRoomInitIndex];
      this.AUD.Play();
      ++this.m_curTrapRoomInitIndex;
    }

    public void PlayTrapRoomFailing()
    {
      if (!this.CanPlay(3) || this.m_curTrapRoomFailingIndex >= this.AC_TrapRoomFailing.Length || (double) this.TimeSinceTrapRoomFailingHook < 15.0)
        return;
      this.TimeSinceTrapRoomFailingHook = 0.0f;
      this.curClipPriority = 3;
      this.AUD.clip = this.AC_TrapRoomFailing[Random.Range(0, this.AC_TrapRoomFailing.Length)];
      this.AUD.Play();
      ++this.m_curTrapRoomFailingIndex;
    }

    public void PlayMeatDiscover(int i)
    {
      if (!this.CanPlay(4))
        return;
      this.curClipPriority = 4;
      this.AUD.clip = this.AC_MeatRoomDiscover[i];
      this.AUD.Play();
    }

    public void PlayMeatAcquire(int i)
    {
      if (!this.CanPlay(4))
        return;
      this.curClipPriority = 4;
      this.AUD.clip = this.AC_MeatRoomAcquire[i];
      this.AUD.Play();
    }

    public void PlayMeatFeedIn(int i)
    {
      if (!this.CanPlay(4))
        return;
      this.curClipPriority = 4;
      this.AUD.clip = this.AC_MeatRoomFeedIn[i];
      this.AUD.Play();
    }

    public void PlayMonsterCloset()
    {
      if (!this.CanPlay(3))
        return;
      this.curClipPriority = 3;
      this.AUD.clip = this.AC_MonsterCloset[Random.Range(0, this.AC_MonsterCloset.Length)];
      this.AUD.Play();
    }

    public void PlayMonsterRebuilt()
    {
      if (!this.CanPlay(1))
        return;
      this.curClipPriority = 1;
      this.AUD.clip = this.AC_MonsterRebuilt[Random.Range(0, this.AC_MonsterRebuilt.Length)];
      this.AUD.Play();
    }

    public void PlayJumpScare()
    {
      if (!this.CanPlay(1) || (double) this.TimeSinceJumpScareHook < 20.0)
        return;
      this.TimeSinceJumpScareHook = 0.0f;
      this.curClipPriority = 1;
      this.AUD.clip = this.AC_JumpScareReactions[Random.Range(0, this.AC_JumpScareReactions.Length)];
      this.AUD.Play();
    }

    public void PlayAreaEntryBoiler()
    {
      if (!this.CanPlay(3) || this.m_curAEBoilerIndex >= this.AC_AreaEntryBoiler.Length || (double) this.TimeSinceAreaHook < 120.0)
        return;
      this.TimeSinceAreaHook = 0.0f;
      this.curClipPriority = 3;
      this.AUD.clip = this.AC_AreaEntryBoiler[this.m_curAEBoilerIndex];
      this.AUD.Play();
      ++this.m_curAEBoilerIndex;
    }

    public void PlayAreaEntryColdStorage()
    {
      if (!this.CanPlay(3) || this.m_curAEColdStorageIndex >= this.AC_ColdStorage.Length || (double) this.TimeSinceAreaHook < 120.0)
        return;
      this.TimeSinceAreaHook = 0.0f;
      this.curClipPriority = 3;
      this.AUD.clip = this.AC_ColdStorage[this.m_curAEColdStorageIndex];
      this.AUD.Play();
      ++this.m_curAEColdStorageIndex;
    }

    public void PlayAreaEntryOffice()
    {
      if (!this.CanPlay(3) || this.m_curAEOfficeIndex >= this.AC_Office.Length || (double) this.TimeSinceAreaHook < 120.0)
        return;
      this.TimeSinceAreaHook = 0.0f;
      this.curClipPriority = 3;
      this.AUD.clip = this.AC_Office[this.m_curAEOfficeIndex];
      this.AUD.Play();
      ++this.m_curAEOfficeIndex;
    }

    public void PlayAreaEntryRestaurant()
    {
      if (!this.CanPlay(3) || this.m_curAERestaurantIndex >= this.AC_Restaurant.Length || (double) this.TimeSinceAreaHook < 120.0)
        return;
      this.TimeSinceAreaHook = 0.0f;
      this.curClipPriority = 3;
      this.AUD.clip = this.AC_Restaurant[this.m_curAERestaurantIndex];
      this.AUD.Play();
      ++this.m_curAERestaurantIndex;
    }

    public void PlayFoundSpecialItem()
    {
      if (!this.CanPlay(2) || this.m_curFoundSpecialItemIndex >= this.AC_FoundSpecialItem.Length || (double) this.TimeSinceObjectHook < 20.0)
        return;
      this.TimeSinceObjectHook = 0.0f;
      this.curClipPriority = 2;
      this.AUD.clip = this.AC_FoundSpecialItem[this.m_curFoundSpecialItemIndex];
      this.AUD.Play();
      ++this.m_curFoundSpecialItemIndex;
    }

    public void PlayFoundJunkItem()
    {
      if (!this.CanPlay(2) || this.m_curFoundJunkItemIndex >= this.AC_FoundJunkItem.Length || (double) this.TimeSinceObjectHook < 20.0)
        return;
      this.TimeSinceObjectHook = 0.0f;
      this.curClipPriority = 2;
      this.AUD.clip = this.AC_FoundJunkItem[this.m_curFoundJunkItemIndex];
      this.AUD.Play();
      ++this.m_curFoundJunkItemIndex;
    }

    public void PlayFoundNormalItem()
    {
      if (!this.CanPlay(2) || this.m_curFoundNormalItemIndex >= this.AC_FoundNormalWeapon.Length || (double) this.TimeSinceObjectHook < 20.0)
        return;
      this.TimeSinceObjectHook = 0.0f;
      this.curClipPriority = 2;
      this.AUD.clip = this.AC_FoundNormalWeapon[this.m_curFoundNormalItemIndex];
      this.AUD.Play();
      ++this.m_curFoundNormalItemIndex;
    }

    public void PlayFoundRareItem()
    {
      if (!this.CanPlay(2) || this.m_curFoundRareItemIndex >= this.AC_FoundRareWeapon.Length || (double) this.TimeSinceObjectHook < 20.0)
        return;
      this.TimeSinceObjectHook = 0.0f;
      this.curClipPriority = 2;
      this.AUD.clip = this.AC_FoundRareWeapon[this.m_curFoundRareItemIndex];
      this.AUD.Play();
      ++this.m_curFoundRareItemIndex;
    }

    public void PlayAboutToDie()
    {
      if (!this.CanPlay(4) || this.m_curAboutToDieIndex >= this.AC_PlayerAboutToDie.Length || (double) this.TimeSinceHealthHook < 15.0)
        return;
      this.TimeSinceHealthHook = 0.0f;
      this.curClipPriority = 4;
      this.AUD.clip = this.AC_PlayerAboutToDie[this.m_curAboutToDieIndex];
      this.AUD.Play();
      ++this.m_curAboutToDieIndex;
    }

    public void PlayDiedCheating()
    {
      if (!this.CanPlay(4))
        return;
      this.curClipPriority = 4;
      this.AUD.clip = this.AC_PlayerDiedCheating[Random.Range(0, this.AC_PlayerDiedCheating.Length)];
      this.AUD.Play();
    }

    public void PlayDiedOutOfHealth()
    {
      if (!this.CanPlay(5))
        return;
      this.curClipPriority = 5;
      this.AUD.clip = this.AC_PlayerDiedOutOfHealth[Random.Range(0, this.AC_PlayerDiedOutOfHealth.Length)];
      this.AUD.Play();
    }

    public void PlayTimeWarning(int i)
    {
      if (!this.CanPlay(3))
        return;
      this.curClipPriority = 3;
      this.AUD.clip = this.AC_TimeRemaining[i];
      this.AUD.Play();
    }

    private void Update()
    {
      if ((double) this.m_intercomCheckTick > 0.0)
      {
        this.m_intercomCheckTick -= Time.deltaTime;
      }
      else
      {
        this.m_intercomCheckTick = Random.Range(1.5f, 2f);
        this.CheckForClosestIntercom();
      }
      if ((Object) this.m_curIntercom == (Object) null)
        this.CheckForClosestIntercom();
      this.TimeSinceObjectHook += Time.deltaTime;
      this.TimeSinceObjectHook += Time.deltaTime;
      this.TimeSinceAreaHook += Time.deltaTime;
      this.TimeSinceHealthHook += Time.deltaTime;
      this.TimeSinceJumpScareHook += Time.deltaTime;
      this.TimeSinceTrapRoomFailingHook += Time.deltaTime;
    }

    private void CheckForClosestIntercom()
    {
      float num1 = 100f;
      int index1 = 0;
      Vector3 position = Camera.main.transform.position;
      for (int index2 = 0; index2 < this.Intercoms.Length; ++index2)
      {
        float num2 = Vector3.Distance(this.Intercoms[index2].position, position);
        if ((double) num2 <= (double) num1)
        {
          num1 = num2;
          index1 = index2;
        }
      }
      if (!((Object) this.m_curIntercom != (Object) this.Intercoms[index1]))
        return;
      this.m_lastIntercom = this.m_curIntercom;
      this.m_curIntercom = this.Intercoms[index1];
      this.transform.position = this.m_curIntercom.position;
    }

    private void ShuffleClips(AudioClip[] clips)
    {
      for (int min = 0; min < clips.Length; ++min)
      {
        AudioClip clip = clips[min];
        int index = Random.Range(min, clips.Length);
        clips[min] = clips[index];
        clips[index] = clip;
      }
    }
  }
}
