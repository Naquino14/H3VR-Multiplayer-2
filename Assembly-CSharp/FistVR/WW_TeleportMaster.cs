// Decompiled with JetBrains decompiler
// Type: FistVR.WW_TeleportMaster
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class WW_TeleportMaster : MonoBehaviour
  {
    public List<WW_Bunker> Bunkers;
    public WW_Panel Panel;
    public List<Transform> TowerPads;
    private bool m_setToRespawnInABunker;
    private int m_currentBunkerSpot;
    public GameObject TeleportInEffect;
    public int CurrentDay;
    public List<int> TiersNeeded;
    public List<WW_TeleportMaster.BunkerCase> CasesByDay;
    public AudioEvent AudEvent_MessageReceived;
    public List<WW_Checkpoint> Checkpoints;
    public List<Transform> SF;
    public ObjectTableDef TableReqPic;
    public OptionsPanel_ButtonSet OBS_Difficulty;
    public int Difficulty;
    [Header("WhiteOutSystem")]
    public Material SkyboxMat;
    public float WhiteOutThreshold;
    public Vector2 WhiteOutFogVals;
    private Vector2 RefHeights = new Vector2(0.0f, 60f);
    private Vector2 HorizonHeights = new Vector2(0.45f, 0.25f);
    private Vector2 HorizonBlends = new Vector2(0.25f, 0.5f);
    private bool m_isBossFighting;
    public GameObject Boss;
    public AudioEvent AudEvent_Alarm;
    public Text BossBUttonText;
    public List<WinterEnemySpawnZone> WSpawns;
    public List<EncryptionBotSpawner> ESPawns;
    private bool m_isInABunker;
    private int m_whichBunkerAmIIn = -1;
    private bool m_isSF;
    public GameObject SFSpawn;
    private GameObject m_mySF;
    private float m_SFTick;
    private int m_curCheckPointCheck;
    public Transform PSys_Rig;
    public ParticleSystem PSys_SnowDown;
    public ParticleSystem PSys_SnowBluster;
    public Material MTL_Snowbluster;
    public Color C_Snowbluster_MinIntensity;
    public Color C_Snowbluster_MaxIntensity;
    public float L_Snowbluster;
    public int Cy_Snowbluster = 2;
    public Vector3 windDirMin = new Vector3(2f, 0.0f, 2f);
    public Vector3 windDirMax = new Vector3(20f, 0.0f, 20f);
    public Vector3 Wind = Vector3.zero;
    public List<WW_WindNode> WindPoints;
    private int m_windCheckIndex;
    private HashSet<WW_WindNode> m_closestTs = new HashSet<WW_WindNode>();
    private List<WW_TeleportMaster.CloseWind> m_closestWind = new List<WW_TeleportMaster.CloseWind>();

    private void Awake()
    {
      this.Panel.UpdateMessageDisplay();
      this.OBS_Difficulty.SetSelectedButton(this.Difficulty);
    }

    public void SetDifficulty(int i)
    {
      this.Difficulty = i;
      if (i == 0)
      {
        GM.CurrentSceneSettings.DoesDamageGetRegistered = true;
        GM.CurrentPlayerBody.SetHealthThreshold(5000f);
      }
      else
      {
        GM.CurrentSceneSettings.DoesDamageGetRegistered = false;
        GM.CurrentPlayerBody.SetHealthThreshold(10000f);
      }
    }

    public void BeginBossFight()
    {
      if (this.m_isBossFighting)
        return;
      this.m_isBossFighting = true;
      for (int index = 0; index < this.WSpawns.Count; ++index)
      {
        this.WSpawns[index].DespawnAll();
        this.WSpawns[index].gameObject.SetActive(false);
      }
      for (int index = 0; index < this.ESPawns.Count; ++index)
      {
        this.ESPawns[index].ClearAll();
        this.ESPawns[index].gameObject.SetActive(false);
      }
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Alarm, this.BossBUttonText.transform.position);
      this.BossBUttonText.text = "WARNING!!! MASTER DRONE ACTIVE";
      this.Boss.SetActive(true);
    }

    private void UpdateWhiteOut()
    {
      this.WhiteOutThreshold = Mathf.Clamp(this.WhiteOutThreshold, 0.0f, 1f);
      this.SkyboxMat.SetFloat("_WhiteoutAmount", this.WhiteOutThreshold);
      RenderSettings.fogDensity = Mathf.Lerp(this.WhiteOutFogVals.x, this.WhiteOutFogVals.y, this.WhiteOutThreshold);
      float t = Mathf.InverseLerp(this.RefHeights.x, this.RefHeights.y, GM.CurrentPlayerBody.Head.position.y);
      float num1 = Mathf.Lerp(0.15f, Mathf.Lerp(this.HorizonHeights.x, this.HorizonHeights.y, t), this.WhiteOutThreshold);
      float num2 = Mathf.Lerp(0.4f, Mathf.Lerp(this.HorizonBlends.x, this.HorizonBlends.y, t), this.WhiteOutThreshold);
      this.SkyboxMat.SetFloat("_HorizonHeight", num1);
      this.SkyboxMat.SetFloat("_HorizonBlend", num2);
    }

    public void Start()
    {
      for (int index = 0; index < this.Bunkers.Count; ++index)
      {
        this.Bunkers[index].SetMaster(this);
        this.Bunkers[index].ConfigInitBunker(index, this.CurrentDay, this.TiersNeeded[index]);
        if (index < this.CasesByDay.Count)
        {
          this.Bunkers[index].Crate.PlaceWeaponInContainer(this.CasesByDay[index].Gun, this.CasesByDay[index].Ammo, this.CasesByDay[index].Extra1, this.CasesByDay[index].Extra2);
          this.Bunkers[index].Crate.PlaceWeaponInContainer(this.CasesByDay[index].GunFO, this.CasesByDay[index].ExtraFO1, this.CasesByDay[index].ExtraFO2, this.CasesByDay[index].AmmoFO, 3);
        }
        else
          this.Bunkers[index].Crate.gameObject.SetActive(false);
      }
      for (int index = 0; index < 4; ++index)
      {
        this.m_closestWind.Add(new WW_TeleportMaster.CloseWind());
        this.m_closestWind[index].Dist = 3000f;
        this.m_closestWind[index].W = this.WindPoints[0];
      }
      this.m_closestTs.Add(this.WindPoints[0]);
      ObjectTable requiredPic = new ObjectTable();
      requiredPic.Initialize(this.TableReqPic);
      for (int index = 0; index < this.Checkpoints.Count; ++index)
        this.Checkpoints[index].Init(requiredPic);
    }

    public void BunkerUnlockedUpdate()
    {
      for (int index = 0; index < this.Bunkers.Count; ++index)
        this.Bunkers[index].UpdateTPButtons();
    }

    public void TeleportTo(int i)
    {
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(this.Bunkers[i].TeleportPoint.position, true);
      UnityEngine.Object.Instantiate<GameObject>(this.TeleportInEffect, this.Bunkers[i].TeleportPoint.position, Quaternion.identity);
      this.m_currentBunkerSpot = i;
      GM.CurrentSceneSettings.DeathResetPoint.position = this.Bunkers[i].TeleportPoint.position;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = Quaternion.identity;
      GM.CurrentPlayerBody.HealPercent(1f);
    }

    public void TeleportToTowerPad(int i)
    {
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(this.TowerPads[i].position, true);
      UnityEngine.Object.Instantiate<GameObject>(this.TeleportInEffect, this.TowerPads[i].position, Quaternion.identity);
      GM.CurrentPlayerBody.HealPercent(1f);
    }

    public void EnteredBunker(int b)
    {
      for (int index = 0; index < this.CasesByDay[b].MessagesByDay.Count; ++index)
      {
        if (!GM.Options.XmasFlags.MessagesAcquired[this.CasesByDay[b].MessagesByDay[index]])
          this.UnlockMessage(this.CasesByDay[b].MessagesByDay[index]);
      }
    }

    public void UnlockMessage(int i)
    {
      if (GM.Options.XmasFlags.MessagesAcquired[i] || !((UnityEngine.Object) this.Panel.Messages[i].AudClip != (UnityEngine.Object) null))
        return;
      Debug.Log((object) ("MessageReceived " + (object) i));
      GM.Options.XmasFlags.MessagesAcquired[i] = true;
      GM.Options.SaveToFile();
      this.Panel.UpdateMessageDisplay();
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_MessageReceived, this.Panel.transform.position).FollowThisTransform(this.Panel.transform);
    }

    private void Update()
    {
      this.PSys_Rig.position = GM.CurrentPlayerBody.transform.position;
      this.m_isInABunker = false;
      this.m_whichBunkerAmIIn = -1;
      for (int index = 0; index < this.Bunkers.Count; ++index)
      {
        if (!this.Bunkers[index].IsLockDown && this.Bunkers[index].IsUnlocked && this.TestVolumeBool(this.Bunkers[index].BunkerBounds, GM.CurrentPlayerBody.Head.position))
        {
          if (index != this.m_whichBunkerAmIIn)
            this.EnteredBunker(index);
          this.m_whichBunkerAmIIn = index;
          this.m_isInABunker = true;
          break;
        }
      }
      ++this.m_curCheckPointCheck;
      if (this.m_curCheckPointCheck >= this.Checkpoints.Count)
        this.m_curCheckPointCheck = 0;
      if (this.Checkpoints[this.m_curCheckPointCheck].HasMessage && (double) Vector3.Distance(GM.CurrentPlayerBody.Head.position, this.Checkpoints[this.m_curCheckPointCheck].SatDish.position) < (double) this.Checkpoints[this.m_curCheckPointCheck].ActivationRange)
        this.UnlockMessage(this.Checkpoints[this.m_curCheckPointCheck].MessageToUnlock);
      for (int index = 0; index < this.SF.Count; ++index)
      {
        if (this.TestVolumeBool(this.SF[index], GM.CurrentPlayerBody.Head.position))
          this.InitSF();
      }
      this.FXManagement();
      if (!this.m_isSF)
        return;
      this.m_SFTick += Time.deltaTime;
      if ((double) this.m_SFTick <= 2.0)
        return;
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.DeathResetPoint.position, true, GM.CurrentSceneSettings.DeathResetPoint.forward);
      this.m_isSF = false;
      this.m_SFTick = 0.0f;
    }

    private void InitSF()
    {
      if (this.m_isSF)
        return;
      if (!this.m_isSF)
      {
        this.m_isSF = true;
        this.m_SFTick = 0.0f;
      }
      this.m_mySF = UnityEngine.Object.Instantiate<GameObject>(this.SFSpawn, GM.CurrentPlayerBody.Head.position, Quaternion.identity);
      this.m_mySF.transform.SetParent(GM.CurrentPlayerBody.Head);
    }

    private void FXManagement()
    {
      for (int index = 0; index < this.m_closestWind.Count; ++index)
        this.m_closestWind[index].Dist = Vector3.Distance(this.m_closestWind[index].W.transform.position, GM.CurrentPlayerBody.Head.position);
      for (int index = 0; index < 20; ++index)
      {
        ++this.m_windCheckIndex;
        if (this.m_windCheckIndex >= this.WindPoints.Count)
          this.m_windCheckIndex = 0;
        if (!this.m_closestTs.Contains(this.WindPoints[this.m_windCheckIndex]))
        {
          float num = Vector3.Distance(GM.CurrentPlayerBody.Head.position, this.WindPoints[this.m_windCheckIndex].transform.position);
          if ((double) num < (double) this.m_closestWind[this.m_closestWind.Count - 1].Dist)
          {
            this.m_closestWind[this.m_closestWind.Count - 1].Dist = num;
            this.m_closestTs.Remove(this.m_closestWind[this.m_closestWind.Count - 1].W);
            this.m_closestWind[this.m_closestWind.Count - 1].W = this.WindPoints[this.m_windCheckIndex];
            this.m_closestTs.Add(this.WindPoints[this.m_windCheckIndex]);
          }
        }
      }
      this.m_closestWind.Sort();
      float num1 = this.m_closestWind[0].Dist + this.m_closestWind[1].Dist + this.m_closestWind[2].Dist + this.m_closestWind[3].Dist;
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < 2; ++index)
      {
        float num2 = (float) (1.0 - (double) this.m_closestWind[index].Dist / (double) num1);
        zero += this.m_closestWind[index].W.transform.forward * this.m_closestWind[index].W.transform.localScale.z * num2;
      }
      zero.y = 0.0f;
      float target = 0.0f;
      for (int index = 0; index < 2; ++index)
      {
        float num2 = (float) (1.0 - (double) this.m_closestWind[index].Dist / (double) num1);
        target += this.m_closestWind[index].W.WhiteOut * num2;
      }
      this.WhiteOutThreshold = !this.m_isInABunker ? Mathf.MoveTowards(this.WhiteOutThreshold, target, Time.deltaTime * 0.1f) : Mathf.MoveTowards(this.WhiteOutThreshold, 0.0f, Time.deltaTime * 2f);
      this.UpdateWhiteOut();
      this.Wind = Vector3.Slerp(this.Wind, zero, Time.deltaTime * 2f);
      this.windDirMax = this.Wind * 0.7f;
      this.windDirMin = this.Wind * 0.05f;
      this.L_Snowbluster = this.Wind.magnitude / 30f;
      this.Cy_Snowbluster = (double) this.L_Snowbluster < 0.899999976158142 ? ((double) this.L_Snowbluster < 0.649999976158142 ? ((double) this.L_Snowbluster < 0.349999994039536 ? 1 : 2) : 3) : 4;
      if (this.m_isInABunker)
      {
        ParticleSystem.EmissionModule emission = this.PSys_SnowBluster.emission;
        emission.enabled = false;
        emission = this.PSys_SnowDown.emission;
        emission.enabled = false;
      }
      else
      {
        ParticleSystem.EmissionModule emission = this.PSys_SnowBluster.emission;
        emission.enabled = true;
        emission = this.PSys_SnowDown.emission;
        emission.enabled = true;
        ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.PSys_SnowDown.forceOverLifetime;
        ParticleSystem.MinMaxCurve minMaxCurve = forceOverLifetime.x;
        minMaxCurve.constantMin = (float) ((double) this.windDirMin.x * 0.5 - 1.0);
        minMaxCurve.constantMax = (float) ((double) this.windDirMax.x * 0.5 + 1.0);
        forceOverLifetime.x = minMaxCurve;
        minMaxCurve = forceOverLifetime.z;
        minMaxCurve.constantMin = (float) ((double) this.windDirMin.z * 0.5 - 1.0);
        minMaxCurve.constantMax = (float) ((double) this.windDirMax.z * 0.5 + 1.0);
        forceOverLifetime.z = minMaxCurve;
        forceOverLifetime = this.PSys_SnowBluster.forceOverLifetime;
        minMaxCurve = forceOverLifetime.x;
        minMaxCurve.constantMin = this.windDirMax.x * 0.5f;
        minMaxCurve.constantMax = this.windDirMax.x;
        forceOverLifetime.x = minMaxCurve;
        minMaxCurve = forceOverLifetime.z;
        minMaxCurve.constantMin = this.windDirMax.z * 0.5f;
        minMaxCurve.constantMax = this.windDirMax.z;
        forceOverLifetime.z = minMaxCurve;
        this.PSys_SnowBluster.textureSheetAnimation.cycleCount = this.Cy_Snowbluster;
        this.MTL_Snowbluster.SetColor("_TintColor", Color.Lerp(this.C_Snowbluster_MinIntensity, this.C_Snowbluster_MaxIntensity, this.L_Snowbluster));
      }
    }

    public bool TestVolumeBool(Transform t, Vector3 pos)
    {
      bool flag = true;
      Vector3 vector3 = t.InverseTransformPoint(pos);
      if ((double) Mathf.Abs(vector3.x) > 0.5 || (double) Mathf.Abs(vector3.y) > 0.5 || (double) Mathf.Abs(vector3.z) > 0.5)
        flag = false;
      return flag;
    }

    [Serializable]
    public class BunkerCase
    {
      public GameObject Gun;
      public GameObject Ammo;
      public GameObject Extra1;
      public GameObject Extra2;
      public FVRObject GunFO;
      public FVRObject AmmoFO;
      public FVRObject ExtraFO1;
      public FVRObject ExtraFO2;
      public List<int> MessagesByDay;
    }

    [Serializable]
    public class CloseWind : IComparable<WW_TeleportMaster.CloseWind>
    {
      public WW_WindNode W;
      public float Dist;

      public int CompareTo(WW_TeleportMaster.CloseWind other) => other == null ? 1 : this.Dist.CompareTo(other.Dist);
    }
  }
}
