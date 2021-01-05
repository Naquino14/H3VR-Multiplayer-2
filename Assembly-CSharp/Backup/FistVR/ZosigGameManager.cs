// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigGameManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigGameManager : MonoBehaviour
  {
    public ZosigSpawnManager SpawnM;
    public ZosigFlagManager FlagM;
    public ZosigMusicController MusicController;
    public TranslocatorManager TranslocatorM;
    public List<Texture2D> NPCSpeechIcons;
    public bool IsVerboseDebug = true;
    public List<FVRObject> MeatCorePrefabs;
    public List<FVRObject> HerbPrefabs;
    public List<FVRObject> Banger_TinCanPrefabs;
    public List<FVRObject> Banger_CoffeeCanPrefabs;
    public List<FVRObject> Banger_BucketPrefabs;
    public List<FVRObject> Banger_MechanismPrefabs;
    public AudioEvent AudEvent_DryHeave;
    public AudioEvent AudEvent_Vomit;
    public ParticleSystem PSystem_Vomit;
    public GameObject SplodePrefab;
    private ZosigGastroCycler gcycler;
    public GameObject IS;
    public GameObject DisableOnNoIntro;
    public ZosigLemonManager LemonManager;
    private float m_tickToSave = 60f;
    private float curMusicVolume = 0.22f;
    private float tarMusicVolume = 0.22f;
    private float m_VomitRefire = 1f;
    private Vector3 lastHeadPos;

    public void SetGCycler(ZosigGastroCycler c) => this.gcycler = c;

    private void Awake() => GM.ZMaster = this;

    public void QUIT() => GM.ZMaster.FlagM.Save();

    private void Start()
    {
      this.FlagM.Init();
      this.LemonManager.InitLemons();
      if (this.FlagM.GetFlagValue("flag_Difficulty") == 1)
      {
        GM.CurrentPlayerBody.SetHealthThreshold(10000f);
        GM.CurrentSceneSettings.IsSpawnLockingEnabled = true;
        this.IS.SetActive(true);
        GM.CurrentSceneSettings.UsesMaxSpeedClamp = false;
        GM.CurrentSceneSettings.DoesTeleportUseCooldown = false;
      }
      else if (this.FlagM.GetFlagValue("flag_Difficulty") == 2)
      {
        GM.CurrentPlayerBody.SetHealthThreshold(2500f);
        this.FlagM.SetFlag("num_meatcoreA", 0);
        this.FlagM.SetFlag("num_meatcoreB", 0);
        this.FlagM.SetFlag("num_meatcoreC", 0);
        this.FlagM.SetFlag("num_meatcoreD", 0);
        this.FlagM.SetFlag("num_meatcoreE", 0);
        this.FlagM.SetFlag("num_meatcoreF", 0);
        this.FlagM.SetFlag("num_meatcoreG", 0);
        this.FlagM.SetFlag("num_meatcoreH", 0);
        this.FlagM.SetFlag("num_herbA", 0);
        this.FlagM.SetFlag("num_herbB", 0);
        this.FlagM.SetFlag("num_herbC", 0);
        this.FlagM.SetFlag("num_herbD", 0);
        this.FlagM.SetFlag("num_herbE", 0);
        this.FlagM.SetFlag("num_bangerJunk_TinCan_0", 0);
        this.FlagM.SetFlag("num_bangerJunk_TinCan_1", 0);
        this.FlagM.SetFlag("num_bangerJunk_TinCan_2", 0);
        this.FlagM.SetFlag("num_bangerJunk_CoffeeCan_0", 0);
        this.FlagM.SetFlag("num_bangerJunk_CoffeeCan_1", 0);
        this.FlagM.SetFlag("num_bangerJunk_CoffeeCan_2", 0);
        this.FlagM.SetFlag("num_bangerJunk_CoffeeCan_3", 0);
        this.FlagM.SetFlag("num_bangerJunk_Bucket", 0);
        this.FlagM.SetFlag("num_bangerJunk_Bangsnaps", 0);
        this.FlagM.SetFlag("num_bangerJunk_EggTimer", 0);
        this.FlagM.SetFlag("num_bangerJunk_Radio", 0);
        this.FlagM.SetFlag("num_bangerJunk_FishFinder", 0);
      }
      if (this.FlagM.GetFlagValue("skip_intro") > 0 || this.FlagM.GetNumEntries() > 2)
      {
        Debug.Log((object) ("skip_intro:" + (object) this.FlagM.GetFlagValue("skip_intro")));
        Debug.Log((object) ("num entries:" + (object) this.FlagM.GetNumEntries()));
        this.FlagM.PrintAll();
        this.FlagM.SetFlagMaxBlend("npc00_quest", 13);
        this.FlagM.SetFlagMaxBlend("quest00_final_state", 1);
        this.DisableOnNoIntro.SetActive(false);
        if (this.FlagM.GetFlagValue("skip_intro") > 0 || !this.FlagM.ContainsKey("skip_intro"))
          this.FlagM.SetFlagMaxBlend("npc00_introduction", 2);
      }
      this.TranslocatorM.Init(this.FlagM);
      this.SpawnM.Init();
      foreach (ZosigQuestManager zosigQuestManager in Object.FindObjectsOfType<ZosigQuestManager>())
        zosigQuestManager.Init(this);
      this.MusicController.Init();
      this.MusicController.SetMasterVolume(0.22f);
      this.SetMusicTrack(ZosigMusicController.ZosigTrackName.HBH);
      Vector3 vector3 = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.1f;
      this.PSystem_Vomit.transform.SetParent(GM.CurrentPlayerBody.Head);
      this.PSystem_Vomit.transform.position = vector3;
      this.PSystem_Vomit.transform.rotation = Quaternion.LookRotation(GM.CurrentPlayerBody.Head.forward);
    }

    private void Update()
    {
      this.MusicController.Tick(Time.deltaTime);
      this.CheckVomit();
      this.lastHeadPos = GM.CurrentPlayerBody.Head.position;
      this.m_tickToSave -= Time.deltaTime;
      if ((double) this.m_tickToSave <= 0.0)
      {
        this.m_tickToSave = 60f;
        GM.ZMaster.FlagM.Save();
      }
      if (this.DisableOnNoIntro.activeSelf && this.FlagM.GetFlagValue("quest00_final_state") > 0)
        this.DisableOnNoIntro.SetActive(false);
      this.curMusicVolume = Mathf.MoveTowards(this.curMusicVolume, this.tarMusicVolume, Time.deltaTime * 1f);
      this.MusicController.SetMasterVolume(this.curMusicVolume);
    }

    public void SetMusic_Speaking() => this.tarMusicVolume = 0.12f;

    public void SetMusic_Gameplay() => this.tarMusicVolume = 0.22f;

    public void SetMusicTrack(ZosigMusicController.ZosigTrackName track) => this.MusicController.SwitchToTrack(track);

    public FVRObject GetRandomEquippedFirearm()
    {
      List<FVRObject> fvrObjectList = new List<FVRObject>();
      FVRInteractiveObject currentInteractable = GM.CurrentMovementManager.Hands[0].CurrentInteractable;
      if (currentInteractable is FVRFireArm && (Object) (currentInteractable as FVRFireArm).ObjectWrapper != (Object) null)
        fvrObjectList.Add((currentInteractable as FVRFireArm).ObjectWrapper);
      for (int index = 0; index < GM.CurrentPlayerBody.QuickbeltSlots.Count; ++index)
      {
        if (GM.CurrentPlayerBody.QuickbeltSlots[index].CurObject is FVRFireArm && (Object) (GM.CurrentPlayerBody.QuickbeltSlots[index].CurObject as FVRFireArm).ObjectWrapper != (Object) null)
          fvrObjectList.Add((GM.CurrentPlayerBody.QuickbeltSlots[index].CurObject as FVRFireArm).ObjectWrapper);
      }
      return fvrObjectList.Count > 0 ? fvrObjectList[Random.Range(0, fvrObjectList.Count)] : (FVRObject) null;
    }

    public void EatMeatCore(RotrwMeatCore.CoreType t)
    {
      switch (t)
      {
        case RotrwMeatCore.CoreType.Tasty:
          this.FlagM.AddToFlag("num_meatcoreA", 1);
          break;
        case RotrwMeatCore.CoreType.Moldy:
          this.FlagM.AddToFlag("num_meatcoreB", 1);
          break;
        case RotrwMeatCore.CoreType.Spikey:
          this.FlagM.AddToFlag("num_meatcoreC", 1);
          break;
        case RotrwMeatCore.CoreType.Zippy:
          this.FlagM.AddToFlag("num_meatcoreD", 1);
          break;
        case RotrwMeatCore.CoreType.Weighty:
          this.FlagM.AddToFlag("num_meatcoreE", 1);
          break;
        case RotrwMeatCore.CoreType.Juicy:
          this.FlagM.AddToFlag("num_meatcoreF", 1);
          break;
        case RotrwMeatCore.CoreType.Shiny:
          this.FlagM.AddToFlag("num_meatcoreG", 1);
          break;
        case RotrwMeatCore.CoreType.Burny:
          this.FlagM.AddToFlag("num_meatcoreH", 1);
          break;
      }
      this.gcycler.UpdateState();
    }

    public void EatHerb(RotrwHerb.HerbType type)
    {
      switch (type)
      {
        case RotrwHerb.HerbType.KatchupLeaf:
          this.FlagM.AddToFlag("num_herbA", 1);
          break;
        case RotrwHerb.HerbType.MustardWillow:
          this.FlagM.AddToFlag("num_herbB", 1);
          break;
        case RotrwHerb.HerbType.PricklyPickle:
          this.FlagM.AddToFlag("num_herbC", 1);
          break;
        case RotrwHerb.HerbType.GiantBlueRaspberry:
          this.FlagM.AddToFlag("num_herbD", 1);
          break;
        case RotrwHerb.HerbType.DeadlyEggplant:
          this.FlagM.AddToFlag("num_herbE", 1);
          break;
      }
      this.gcycler.UpdateState();
    }

    public void EatBangerJunk(RotrwBangerJunk.BangerJunkType type, int matIndex)
    {
      switch (type)
      {
        case RotrwBangerJunk.BangerJunkType.TinCan:
          switch (matIndex)
          {
            case 0:
              this.FlagM.AddToFlag("num_bangerJunk_TinCan_0", 1);
              break;
            case 1:
              this.FlagM.AddToFlag("num_bangerJunk_TinCan_1", 1);
              break;
            case 2:
              this.FlagM.AddToFlag("num_bangerJunk_TinCan_2", 1);
              this.FlagM.SetFlagMaxBlend("npc00_quest", 3);
              break;
          }
          break;
        case RotrwBangerJunk.BangerJunkType.CoffeeCan:
          switch (matIndex)
          {
            case 0:
              this.FlagM.AddToFlag("num_bangerJunk_CoffeeCan_0", 1);
              break;
            case 1:
              this.FlagM.AddToFlag("num_bangerJunk_CoffeeCan_1", 1);
              break;
            case 2:
              this.FlagM.AddToFlag("num_bangerJunk_CoffeeCan_2", 1);
              break;
            case 3:
              this.FlagM.AddToFlag("num_bangerJunk_CoffeeCan_3", 1);
              break;
          }
          break;
        case RotrwBangerJunk.BangerJunkType.Bucket:
          this.FlagM.AddToFlag("num_bangerJunk_Bucket", 1);
          break;
        case RotrwBangerJunk.BangerJunkType.Radio:
          this.FlagM.AddToFlag("num_bangerJunk_Radio", 1);
          break;
        case RotrwBangerJunk.BangerJunkType.FishFinder:
          this.FlagM.AddToFlag("num_bangerJunk_FishFinder", 1);
          break;
        case RotrwBangerJunk.BangerJunkType.BangSnaps:
          this.FlagM.AddToFlag("num_bangerJunk_Bangsnaps", 1);
          break;
        case RotrwBangerJunk.BangerJunkType.EggTimer:
          this.FlagM.AddToFlag("num_bangerJunk_EggTimer", 1);
          break;
      }
      this.gcycler.UpdateState();
    }

    public void CheckVomit()
    {
      if ((double) this.m_VomitRefire > 0.0)
      {
        this.m_VomitRefire -= Time.deltaTime;
      }
      else
      {
        if ((double) Vector3.Angle(GM.CurrentPlayerBody.Head.forward, -Vector3.up) > 45.0)
          return;
        Vector3 a = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.1f;
        for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
        {
          if ((Object) GM.CurrentMovementManager.Hands[index].CurrentInteractable != (Object) null || (double) Vector3.Distance(a, GM.CurrentMovementManager.Hands[index].transform.position) > 0.300000011920929)
            return;
        }
        for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
        {
          Vector3 velLinearWorld = GM.CurrentMovementManager.Hands[index].Input.VelLinearWorld;
          if ((double) velLinearWorld.magnitude < 1.0 || (double) Vector3.Angle(velLinearWorld, GM.CurrentPlayerBody.Head.forward) > 45.0)
            return;
        }
        if ((double) Vector3.Angle(GM.CurrentPlayerBody.Head.position - this.lastHeadPos, -Vector3.up) > 45.0)
          return;
        this.VomitRandomThing();
      }
    }

    public bool VomitCore(int i)
    {
      switch (i)
      {
        case 0:
          if (this.FlagM.GetFlagValue("num_meatcoreA") > 0)
          {
            this.FlagM.SubstractFromFlag("num_meatcoreA", 1);
            return this.VomitObject(this.MeatCorePrefabs[0]);
          }
          break;
        case 1:
          if (this.FlagM.GetFlagValue("num_meatcoreB") > 0)
          {
            this.FlagM.SubstractFromFlag("num_meatcoreB", 1);
            return this.VomitObject(this.MeatCorePrefabs[1]);
          }
          break;
        case 2:
          if (this.FlagM.GetFlagValue("num_meatcoreC") > 0)
          {
            this.FlagM.SubstractFromFlag("num_meatcoreC", 1);
            return this.VomitObject(this.MeatCorePrefabs[2]);
          }
          break;
        case 3:
          if (this.FlagM.GetFlagValue("num_meatcoreD") > 0)
          {
            this.FlagM.SubstractFromFlag("num_meatcoreD", 1);
            return this.VomitObject(this.MeatCorePrefabs[3]);
          }
          break;
        case 4:
          if (this.FlagM.GetFlagValue("num_meatcoreE") > 0)
          {
            this.FlagM.SubstractFromFlag("num_meatcoreE", 1);
            return this.VomitObject(this.MeatCorePrefabs[4]);
          }
          break;
        case 5:
          if (this.FlagM.GetFlagValue("num_meatcoreF") > 0)
          {
            this.FlagM.SubstractFromFlag("num_meatcoreF", 1);
            return this.VomitObject(this.MeatCorePrefabs[5]);
          }
          break;
        case 6:
          if (this.FlagM.GetFlagValue("num_meatcoreG") > 0)
          {
            this.FlagM.SubstractFromFlag("num_meatcoreG", 1);
            return this.VomitObject(this.MeatCorePrefabs[6]);
          }
          break;
        case 7:
          if (this.FlagM.GetFlagValue("num_meatcoreH") > 0)
          {
            this.FlagM.SubstractFromFlag("num_meatcoreH", 1);
            return this.VomitObject(this.MeatCorePrefabs[7]);
          }
          break;
      }
      return false;
    }

    public bool VomitHerb(int i)
    {
      switch (i)
      {
        case 0:
          if (this.FlagM.GetFlagValue("num_herbA") > 0)
          {
            this.FlagM.SubstractFromFlag("num_herbA", 1);
            return this.VomitObject(this.HerbPrefabs[0]);
          }
          break;
        case 1:
          if (this.FlagM.GetFlagValue("num_herbB") > 0)
          {
            this.FlagM.SubstractFromFlag("num_herbB", 1);
            return this.VomitObject(this.HerbPrefabs[1]);
          }
          break;
        case 2:
          if (this.FlagM.GetFlagValue("num_herbC") > 0)
          {
            this.FlagM.SubstractFromFlag("num_herbC", 1);
            return this.VomitObject(this.HerbPrefabs[2]);
          }
          break;
        case 3:
          if (this.FlagM.GetFlagValue("num_herbD") > 0)
          {
            this.FlagM.SubstractFromFlag("num_herbD", 1);
            return this.VomitObject(this.HerbPrefabs[3]);
          }
          break;
        case 4:
          if (this.FlagM.GetFlagValue("num_herbE") > 0)
          {
            this.FlagM.SubstractFromFlag("num_herbE", 1);
            return this.VomitObject(this.HerbPrefabs[4]);
          }
          break;
      }
      return false;
    }

    public bool VomitBangerJunk(int i)
    {
      switch (i)
      {
        case 0:
          if (this.FlagM.GetFlagValue("num_bangerJunk_TinCan_0") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_TinCan_0", 1);
            return this.VomitObject(this.Banger_TinCanPrefabs[0]);
          }
          if (this.FlagM.GetFlagValue("num_bangerJunk_TinCan_1") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_TinCan_1", 1);
            return this.VomitObject(this.Banger_TinCanPrefabs[1]);
          }
          if (this.FlagM.GetFlagValue("num_bangerJunk_TinCan_2") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_TinCan_2", 1);
            return this.VomitObject(this.Banger_TinCanPrefabs[2]);
          }
          break;
        case 1:
          if (this.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_0") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_CoffeeCan_0", 1);
            return this.VomitObject(this.Banger_CoffeeCanPrefabs[0]);
          }
          if (this.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_1") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_CoffeeCan_1", 1);
            return this.VomitObject(this.Banger_CoffeeCanPrefabs[1]);
          }
          if (this.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_2") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_CoffeeCan_2", 1);
            return this.VomitObject(this.Banger_CoffeeCanPrefabs[2]);
          }
          if (this.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_3") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_CoffeeCan_3", 1);
            return this.VomitObject(this.Banger_CoffeeCanPrefabs[3]);
          }
          break;
        case 2:
          if (this.FlagM.GetFlagValue("num_bangerJunk_Bucket") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_Bucket", 1);
            return this.VomitObject(this.Banger_BucketPrefabs[0]);
          }
          break;
        case 3:
          if (this.FlagM.GetFlagValue("num_bangerJunk_Bangsnaps") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_Bangsnaps", 1);
            return this.VomitObject(this.Banger_MechanismPrefabs[0]);
          }
          break;
        case 4:
          if (this.FlagM.GetFlagValue("num_bangerJunk_EggTimer") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_EggTimer", 1);
            return this.VomitObject(this.Banger_MechanismPrefabs[1]);
          }
          break;
        case 5:
          if (this.FlagM.GetFlagValue("num_bangerJunk_Radio") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_Radio", 1);
            return this.VomitObject(this.Banger_MechanismPrefabs[2]);
          }
          break;
        case 6:
          if (this.FlagM.GetFlagValue("num_bangerJunk_FishFinder") > 0)
          {
            this.FlagM.SubstractFromFlag("num_bangerJunk_FishFinder", 1);
            return this.VomitObject(this.Banger_MechanismPrefabs[3]);
          }
          break;
      }
      return false;
    }

    private bool VomitObject(FVRObject o)
    {
      Vector3 pos = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f + GM.CurrentPlayerBody.Head.transform.forward * 0.15f;
      SM.PlayGenericSound(this.AudEvent_Vomit, pos);
      this.PSystem_Vomit.Emit(10);
      Object.Instantiate<GameObject>(o.GetGameObject(), pos + Random.onUnitSphere * 0.1f, Random.rotation).GetComponent<Rigidbody>().velocity = GM.CurrentPlayerBody.Head.forward * Random.Range(1f, 3f);
      return true;
    }

    public void VomitRandomThing()
    {
      bool flag = false;
      if (!flag)
      {
        for (int i = 0; i < 7; ++i)
        {
          if (this.VomitBangerJunk(i))
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
      {
        for (int i = 0; i < 5; ++i)
        {
          if (this.VomitHerb(i))
          {
            flag = true;
            break;
          }
        }
      }
      if (flag)
        return;
      for (int i = 0; i < 8; ++i)
      {
        if (this.VomitCore(i))
          break;
      }
    }
  }
}
