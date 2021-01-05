// Decompiled with JetBrains decompiler
// Type: FistVR.ConfigurableSoldierBotSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ConfigurableSoldierBotSpawner : MonoBehaviour
  {
    public ClunkSpawner OldSpawner;
    public Text BotAmount;
    public Text BotAutoSpawnLabel;
    public wwBotWurstNavPointGroup NavGroup;
    public GameObject[] BotModernGunPrefabs;
    private List<List<GameObject>> BotModernGunsChart = new List<List<GameObject>>();
    public List<GameObject> BotModernGuns_Pistol;
    public List<GameObject> BotModernGuns_Shotgun;
    public List<GameObject> BotModernGuns_SMG;
    public List<GameObject> BotModernGuns_Rifle;
    public GameObject[] BotTypePrefabs;
    public GameObject HealthDrop;
    public wwBotWurstConfig[] BotConfigs;
    public GameObject[] BotSpecialPrefabs;
    public List<GameObject> BotSpecialWeapons;
    public wwBotWurstConfig[] BotSpecialConfigs;
    public int GunSetting = 4;
    public int ArmorSetting = 4;
    public int SpecialSetting;
    public int HealthSetting = 1;
    public int PerceptionSetting = 1;
    public int MovementSetting = 1;
    public int ManualSpawnSetting;
    public int AutoSpawnMaxSetting = 1;
    public int AutoSpawnSpeedSetting;
    private float[] HeadHealths = new float[3]
    {
      250f,
      2500f,
      25000f
    };
    private float[] BodyHealths = new float[3]
    {
      800f,
      8000f,
      80000f
    };
    private float[] MaxViewAngles = new float[3]
    {
      35f,
      70f,
      89f
    };
    private float[] MaxViewDistances = new float[3]
    {
      20f,
      50f,
      100f
    };
    private float[] MaxFiringAngles = new float[3]
    {
      15f,
      25f,
      35f
    };
    private float[] MaxFiringDistances = new float[3]
    {
      35f,
      55f,
      120f
    };
    private float[] MaxSpeed_Walks = new float[3]
    {
      0.4f,
      0.75f,
      1.2f
    };
    private float[] MaxSpeed_Combats = new float[3]
    {
      0.8f,
      1.5f,
      2f
    };
    private float[] MaxSpeed_Runs = new float[3]
    {
      1.5f,
      2.75f,
      4f
    };
    private float[] Accelerations = new float[3]
    {
      0.7f,
      1.5f,
      3f
    };
    private float[] MaxAngularSpeeds = new float[3]
    {
      1f,
      2f,
      3f
    };
    private int[] BotManualSpawnAmounts = new int[3]
    {
      1,
      2,
      3
    };
    public int[] AutoSpawnMaxAmounts = new int[5]
    {
      1,
      3,
      6,
      9,
      12
    };
    private float[] AutoSpawnSpeedTicks = new float[3]
    {
      10f,
      5f,
      1f
    };
    public OptionsPanel_ButtonSet OBS_GunSetting;
    public OptionsPanel_ButtonSet OBS_ArmorSetting;
    public OptionsPanel_ButtonSet OBS_SpecialSetting;
    public OptionsPanel_ButtonSet OBS_HealthSetting;
    public OptionsPanel_ButtonSet OBS_PerceptionSetting;
    public OptionsPanel_ButtonSet OBS_MovementSetting;
    public OptionsPanel_ButtonSet OBS_ManualSpawnAmount;
    public OptionsPanel_ButtonSet OBS_AutoSpawnMax;
    public OptionsPanel_ButtonSet OBS_AutoSpawnSpeed;
    public OptionsPanel_ButtonSet OBS_FastBullets;
    private List<GameObject> SpawnedBots = new List<GameObject>();
    public Transform[] SpawnPositions;
    public float DesiredSpawnRange = 10f;
    public float DesiredSpawnRangeMax = 10f;
    public bool UsesNavPointsToSpawn;
    private bool m_isAutoSpawnOn;
    public bool UsesDropList;
    public float DropChance = 0.1f;
    public SV_LootTable DropLootTable;
    [Header("GronchStuff")]
    public bool IsGronchActiveInScene;
    private bool m_isGronchFighting;
    public GameObject GronchPrefab;
    public GameObject GronchSpawningHotdogPrefab;
    private GameObject m_gronch;
    public Transform[] GronchNavPoints;
    public Transform[] GronchSpawnPoints;
    private Vector3 DisabledPos;
    private Vector3 m_InitialPos;
    public Transform HotDogSpawnPoint;
    private GameObject m_spawnedHotDog;
    private float SpawnTick;
    private Vector2 SpawnTimeRange = new Vector2(3f, 10f);
    private bool m_usingFastBullets;

    public void Awake()
    {
      this.OBS_GunSetting.SetSelectedButton(this.GunSetting);
      this.OBS_ArmorSetting.SetSelectedButton(this.ArmorSetting);
      if ((Object) this.OBS_SpecialSetting != (Object) null)
        this.OBS_SpecialSetting.SetSelectedButton(this.SpecialSetting);
      this.OBS_HealthSetting.SetSelectedButton(this.HealthSetting);
      this.OBS_PerceptionSetting.SetSelectedButton(this.PerceptionSetting);
      this.OBS_MovementSetting.SetSelectedButton(this.MovementSetting);
      this.OBS_ManualSpawnAmount.SetSelectedButton(this.ManualSpawnSetting);
      this.OBS_AutoSpawnMax.SetSelectedButton(this.AutoSpawnMaxSetting);
      this.OBS_AutoSpawnSpeed.SetSelectedButton(this.AutoSpawnSpeedSetting);
      if ((Object) this.OBS_FastBullets != (Object) null)
        this.OBS_FastBullets.SetSelectedButton(0);
      this.m_InitialPos = this.transform.root.position;
      this.DisabledPos = new Vector3(this.m_InitialPos.x, -1000f, this.m_InitialPos.z);
      this.BotModernGunsChart.Add(this.BotModernGuns_Pistol);
      this.BotModernGunsChart.Add(this.BotModernGuns_Shotgun);
      this.BotModernGunsChart.Add(this.BotModernGuns_SMG);
      this.BotModernGunsChart.Add(this.BotModernGuns_Rifle);
    }

    private void Update()
    {
      this.SpawnTimeRange = new Vector2(this.AutoSpawnSpeedTicks[this.AutoSpawnSpeedSetting], this.AutoSpawnSpeedTicks[this.AutoSpawnSpeedSetting] * 3f);
      if (this.m_isAutoSpawnOn && this.SpawnedBots.Count < this.AutoSpawnMaxAmounts[this.AutoSpawnMaxSetting])
      {
        if ((double) this.SpawnTick > 0.0)
        {
          this.SpawnTick -= Time.deltaTime;
        }
        else
        {
          this.SpawnTick = Random.Range(this.SpawnTimeRange.x, this.SpawnTimeRange.y);
          this.SpawnBot(this.GetRandomDistantSpawnPoint());
        }
      }
      for (int index = this.SpawnedBots.Count - 1; index >= 0; --index)
      {
        if ((Object) this.SpawnedBots[index] == (Object) null)
          this.SpawnedBots.RemoveAt(index);
      }
      this.BotAmount.text = "Bots Spawned = " + (object) this.SpawnedBots.Count;
      if (!this.IsGronchActiveInScene || this.m_isGronchFighting || !((Object) this.m_spawnedHotDog == (Object) null))
        return;
      GameObject gameObject = Object.Instantiate<GameObject>(this.GronchSpawningHotdogPrefab, this.HotDogSpawnPoint.position, this.HotDogSpawnPoint.rotation);
      gameObject.GetComponent<InvasionDog>().Spawner = this;
      this.m_spawnedHotDog = gameObject;
    }

    public void ToggleAutoSpawn()
    {
      this.m_isAutoSpawnOn = !this.m_isAutoSpawnOn;
      if (this.m_isAutoSpawnOn)
        this.BotAutoSpawnLabel.text = "Turn Off AutoSpawn";
      else
        this.BotAutoSpawnLabel.text = "Turn On AutoSpawn";
    }

    public void SetSetting_Gun(int i) => this.GunSetting = i;

    public void SetSetting_Armor(int i) => this.ArmorSetting = i;

    public void SetSetting_SpecialTypes(int i) => this.SpecialSetting = i;

    public void SetSetting_Health(int i) => this.HealthSetting = i;

    public void SetSetting_Perception(int i) => this.PerceptionSetting = i;

    public void SetSetting_Movement(int i) => this.MovementSetting = i;

    public void SetSetting_ManualSpawnAmount(int i) => this.ManualSpawnSetting = i;

    public void SetSetting_AutoSpawnMaxBots(int i) => this.AutoSpawnMaxSetting = i;

    public void SetSetting_AutoSpawnSpeed(int i) => this.AutoSpawnSpeedSetting = i;

    public void SetSetting_FastBullets(bool b) => this.m_usingFastBullets = b;

    public void SpawnManual()
    {
      int manualSpawnAmount = this.BotManualSpawnAmounts[this.ManualSpawnSetting];
      for (int index = 0; index < manualSpawnAmount; ++index)
        this.SpawnBot(this.GetRandomDistantSpawnPoint());
    }

    public void SpawnManualAtPoint(Transform point) => this.SpawnBot(point);

    public Transform GetRandomDistantSpawnPoint()
    {
      if (this.UsesNavPointsToSpawn)
      {
        List<wwBotWurstNavPoint> dynamicSet = this.NavGroup.GetDynamicSet();
        for (int index = 0; index < dynamicSet.Count; ++index)
        {
          float num = Vector3.Distance(dynamicSet[index].transform.position, GM.CurrentPlayerBody.transform.position);
          if ((double) num > (double) this.DesiredSpawnRange && (double) num < (double) this.DesiredSpawnRangeMax)
            return dynamicSet[index].transform;
        }
        return dynamicSet[0].transform;
      }
      int[] numArray = this.ChooseRandomIndicies(this.SpawnPositions.Length);
      for (int index = 0; index < numArray.Length; ++index)
      {
        if ((double) Vector3.Distance(this.SpawnPositions[numArray[index]].position, GM.CurrentPlayerBody.transform.position) > (double) this.DesiredSpawnRange)
          return this.SpawnPositions[numArray[index]];
      }
      return this.SpawnPositions[numArray[0]];
    }

    private int[] ChooseRandomIndicies(int length)
    {
      int[] numArray = new int[length];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = index;
      for (int min = 0; min < numArray.Length; ++min)
      {
        int num = numArray[min];
        int index = Random.Range(min, length);
        numArray[min] = numArray[index];
        numArray[index] = num;
      }
      return numArray;
    }

    public void SpawnBot(Transform spawnPoint)
    {
      bool flag = false;
      if (this.SpecialSetting == 1)
      {
        if ((double) Random.Range(0.0f, 1f) > 0.5)
          flag = true;
      }
      else if (this.SpecialSetting == 2)
        flag = true;
      int index1;
      GameObject gameObject;
      if (flag)
      {
        index1 = Random.Range(0, this.BotSpecialPrefabs.Length);
        gameObject = Object.Instantiate<GameObject>(this.BotSpecialPrefabs[index1], spawnPoint.position, spawnPoint.rotation);
      }
      else
      {
        index1 = this.ArmorSetting;
        if (this.ArmorSetting >= 4)
          index1 = Random.Range(0, 4);
        gameObject = Object.Instantiate<GameObject>(this.BotTypePrefabs[index1], spawnPoint.position, spawnPoint.rotation);
      }
      wwBotWurst component1 = gameObject.GetComponent<wwBotWurst>();
      int index2;
      GameObject botSpecialWeapon;
      if (flag)
      {
        index2 = index1;
        botSpecialWeapon = this.BotSpecialWeapons[index2];
      }
      else
      {
        index2 = this.GunSetting;
        if (this.GunSetting >= 4)
          index2 = Random.Range(0, 4);
        List<GameObject> gameObjectList = this.BotModernGunsChart[index2];
        botSpecialWeapon = gameObjectList[Random.Range(0, gameObjectList.Count)];
      }
      wwBotWurstModernGun component2 = Object.Instantiate<GameObject>(botSpecialWeapon, component1.ModernGunMount.position, component1.ModernGunMount.rotation).GetComponent<wwBotWurstModernGun>();
      component1.ModernGuns.Add(component2);
      component2.Bot = component1;
      component2.transform.SetParent(component1.ModernGunMount.parent);
      if (this.m_usingFastBullets)
        component2.SetUseFastProjectile(true);
      component1.Config = this.BotConfigs[index2];
      component1.ReConfig(this.BotConfigs[index2]);
      int index3 = this.HealthSetting;
      if (this.HealthSetting >= 3)
        index3 = Random.Range(0, 3);
      component1.Pieces[0].SetLife(this.HeadHealths[index3]);
      component1.Pieces[1].SetLife(this.BodyHealths[index3]);
      component1.Pieces[2].SetLife(this.BodyHealths[index3]);
      int num1 = this.PerceptionSetting;
      if (this.PerceptionSetting >= 3)
        num1 = Random.Range(0, 3);
      int index4 = this.MovementSetting;
      if (this.MovementSetting >= 3)
        index4 = Random.Range(0, 3);
      component1.Max_LinearSpeed_Walk = this.MaxSpeed_Walks[index4];
      component1.Max_LinearSpeed_Combat = this.MaxSpeed_Combats[index4];
      component1.Max_LinearSpeed_Run = this.MaxSpeed_Runs[index4];
      component1.Acceleration = this.Accelerations[index4];
      component1.Max_AngularSpeed = this.MaxAngularSpeeds[index4];
      component1.NavPointGroup = this.NavGroup;
      float num2 = Random.Range(0.0f, 1f);
      if (this.UsesDropList)
      {
        if ((double) num2 < (double) this.DropChance)
        {
          SV_LootTableEntry weightedRandomEntry = this.DropLootTable.GetWeightedRandomEntry();
          component1.DropOnDeath = weightedRandomEntry.MainObj.GetGameObject();
        }
      }
      else if ((double) num2 > 0.75)
        component1.DropOnDeath = this.HealthDrop;
      this.SpawnedBots.Add(gameObject);
      GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
    }

    public void DeleteBots()
    {
      if (this.SpawnedBots.Count > 0)
      {
        for (int index = this.SpawnedBots.Count - 1; index >= 0; --index)
          Object.Destroy((Object) this.SpawnedBots[index]);
      }
      this.SpawnedBots.Clear();
    }

    public void PlayerDied()
    {
      this.DeleteBots();
      this.m_isAutoSpawnOn = false;
      this.BotAutoSpawnLabel.text = "Turn On AutoSpawn";
      this.transform.root.position = this.m_InitialPos;
      if ((Object) this.OldSpawner != (Object) null)
        this.OldSpawner.PlayerDied();
      if (!this.IsGronchActiveInScene)
        return;
      if ((Object) this.m_gronch != (Object) null)
        Object.Destroy((Object) this.m_gronch);
      this.m_isGronchFighting = false;
      if (!((Object) this.m_spawnedHotDog == (Object) null))
        return;
      GameObject gameObject = Object.Instantiate<GameObject>(this.GronchSpawningHotdogPrefab, this.HotDogSpawnPoint.position, this.HotDogSpawnPoint.rotation);
      gameObject.GetComponent<InvasionDog>().Spawner = this;
      this.m_spawnedHotDog = gameObject;
    }

    public void SpawnGronch()
    {
      if (this.m_isGronchFighting)
        return;
      this.m_isGronchFighting = true;
      GameObject gameObject = Object.Instantiate<GameObject>(this.GronchPrefab, this.GronchNavPoints[0].position, this.GronchNavPoints[0].rotation);
      MM_GronchShip component = gameObject.GetComponent<MM_GronchShip>();
      this.m_gronch = gameObject;
      component.BotSpawner = this;
      component.NavPoints = this.GronchNavPoints;
      component.SpawnPoints = this.GronchSpawnPoints;
      this.transform.root.position = this.DisabledPos;
      this.DeleteBots();
      this.m_isAutoSpawnOn = false;
      this.BotAutoSpawnLabel.text = "Turn On AutoSpawn";
    }

    public void GronchIsDead()
    {
      this.m_isGronchFighting = false;
      if ((Object) this.m_spawnedHotDog == (Object) null)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.GronchSpawningHotdogPrefab, this.HotDogSpawnPoint.position, this.HotDogSpawnPoint.rotation);
        gameObject.GetComponent<InvasionDog>().Spawner = this;
        this.m_spawnedHotDog = gameObject;
      }
      this.transform.root.position = this.m_InitialPos;
    }
  }
}
