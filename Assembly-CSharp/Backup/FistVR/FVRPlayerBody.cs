// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPlayerBody
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace FistVR
{
  public class FVRPlayerBody : MonoBehaviour
  {
    public Transform Head;
    public Transform LeftHand;
    public Transform RightHand;
    public Transform Neck;
    public Transform Torso;
    public ParticleSystem PSystemDamage;
    public ParticleSystem PSystemBlind;
    public GameObject NavMeshOb;
    public FVRPlayerHitbox[] Hitboxes;
    public List<FVRQuickBeltSlot> QuickbeltSlots = new List<FVRQuickBeltSlot>();
    public Transform NeckJointTransform;
    public Transform NeckTransform;
    public Transform TorsoTransform;
    public Transform NeckJointTargetsThis;
    public Transform NeckTargetsThis;
    public Transform TorsoTargetsThis;
    public Camera EyeCam;
    public PostProcessLayer PostLayer;
    public GameObject WristMenuPrefab;
    private float m_startingHealth = 5000f;
    public float Health = 5000f;
    public GameObject HealthBarPrefab;
    public GameObject HealthBar;
    public GameObject VFX_Farout;
    public GameObject VFX_Cyclops;
    public GameObject VFX_Biclops;
    private int m_playerIFF;
    private int m_backupIFF;
    public List<AIEntity> PlayerEntities;
    public FVRObject PlayerSosigBodyPrefab;
    private PlayerSosigBody m_sosigPlayerBody;
    public float VisibilityMult = 1f;
    public string DebugString;
    public float GlobalHearingTarget = 1f;
    public float GlobalHearing = 1f;
    private float m_blindAmount;
    private float m_highVisibilityEventValue;
    public LayerMask LM_OcclusionTesting;
    private RaycastHit hit;
    private float m_averageOcclusionDistance = 2.5f;
    private bool m_isHealing;
    private bool m_isDamResist;
    private bool m_isDamPowerUp;
    private bool m_isInfiniteAmmo;
    private bool m_isGhosted;
    private bool m_isMuscleMeat;
    private bool m_isCyclops;
    private bool m_isSnakeEyes;
    private bool m_isBlort;
    private bool m_isFarOutMan;
    private float m_buffTime_Heal;
    private float m_buffTime_DamResist;
    private float m_buffTime_DamPowerUp;
    private float m_buffTime_InfiniteAmmo;
    private float m_buffTime_Ghosted;
    private float m_buffTime_MuscleMeat;
    private float m_buffTime_Cyclops;
    private float m_buffTime_SnakeEyes;
    private float m_buffTime_Blort;
    private float m_buffTime_FarOutMan;
    private bool m_isHurting;
    private bool m_isDamMult;
    private bool m_isDamPowerDown;
    private bool m_isAmmoDrain;
    private bool m_isSuperVisible;
    private bool m_isWeakMeat;
    private bool m_isBiClops;
    private bool m_isMoleEye;
    private bool m_isDlort;
    private bool m_isBadTrip;
    private float m_debuffTime_Hurt;
    private float m_debuffTime_DamMult;
    private float m_debuffTime_DamPowerDown;
    private float m_debuffTime_AmmoDrain;
    private float m_debuffTime_SuperVisible;
    private float m_debuffTime_WeakMeat;
    private float m_debuffTime_BiClops;
    private float m_debuffTime_MoleEye;
    private float m_debuffTime_Dlort;
    private float m_debuffTime_BadTrip;
    private float m_buffIntensity_HealHarm = 20f;
    private float m_damageResist;
    private float m_damageMult = 1f;
    private float m_regenMult = 1f;
    private float m_muscleMeatPower = 1f;
    private float m_cyclopsPower = 1f;
    private GameObject[] BuffSystems_LeftHand = new GameObject[14];
    private GameObject[] BuffSystems_RightHand = new GameObject[14];
    private GameObject[] DeBuffSystems_LeftHand = new GameObject[14];
    private GameObject[] DeBuffSystems_RightHand = new GameObject[14];

    private void Start() => this.UpdateSosigPlayerBodyState();

    public void SetOutfit(SosigEnemyTemplate tem)
    {
      if ((Object) this.m_sosigPlayerBody == (Object) null)
        return;
      GM.Options.ControlOptions.MBClothing = tem.SosigEnemyID;
      SosigEnemyID mbClothing = GM.Options.ControlOptions.MBClothing;
      if (mbClothing == SosigEnemyID.None)
        return;
      SosigEnemyTemplate sosigEnemyTemplate = ManagerSingleton<IM>.Instance.odicSosigObjsByID[mbClothing];
      if (sosigEnemyTemplate.OutfitConfig.Count <= 0)
        return;
      this.m_sosigPlayerBody.ApplyOutfit(sosigEnemyTemplate.OutfitConfig[Random.Range(0, sosigEnemyTemplate.OutfitConfig.Count)]);
    }

    public void UpdateSosigPlayerBodyState()
    {
      if (GM.Options.ControlOptions.MBMode == ControlOptions.MeatBody.Enabled)
      {
        if (!((Object) this.m_sosigPlayerBody == (Object) null))
          return;
        this.m_sosigPlayerBody = Object.Instantiate<GameObject>(this.PlayerSosigBodyPrefab.GetGameObject(), GM.CurrentPlayerBody.Torso.position, GM.CurrentPlayerBody.Torso.rotation).GetComponent<PlayerSosigBody>();
        SosigEnemyID mbClothing = GM.Options.ControlOptions.MBClothing;
        Debug.Log((object) ("Setting to:" + (object) mbClothing));
        if (mbClothing == SosigEnemyID.None)
          return;
        SosigEnemyTemplate sosigEnemyTemplate = ManagerSingleton<IM>.Instance.odicSosigObjsByID[mbClothing];
        if (sosigEnemyTemplate.OutfitConfig.Count <= 0)
          return;
        this.m_sosigPlayerBody.ApplyOutfit(sosigEnemyTemplate.OutfitConfig[Random.Range(0, sosigEnemyTemplate.OutfitConfig.Count)]);
      }
      else
      {
        if (!((Object) this.m_sosigPlayerBody != (Object) null))
          return;
        Object.Destroy((Object) this.m_sosigPlayerBody.gameObject);
        this.m_sosigPlayerBody = (PlayerSosigBody) null;
      }
    }

    public void Update()
    {
      this.UpdatePowerUps();
      if ((double) this.m_blindAmount > 0.0)
      {
        this.m_blindAmount -= Time.deltaTime;
        this.GlobalHearingTarget = 1f - this.m_blindAmount;
        this.GlobalHearingTarget = Mathf.Clamp(this.GlobalHearingTarget, 0.1f, 1f);
      }
      else
        this.GlobalHearingTarget = 1f;
      this.GlobalHearing = (double) this.GlobalHearingTarget >= (double) this.GlobalHearing ? Mathf.Lerp(this.GlobalHearing, this.GlobalHearingTarget, Time.deltaTime * 3f) : Mathf.Lerp(this.GlobalHearing, this.GlobalHearingTarget, Time.deltaTime * 8f);
      AudioListener.volume = this.GlobalHearing;
      this.PlayerEntityUpdate();
    }

    public void BlindPlayer(float f)
    {
      this.m_blindAmount = Mathf.Max(this.m_blindAmount, f + 0.5f);
      if ((double) this.m_blindAmount < 1.0)
        return;
      this.PSystemBlind.Emit(1);
    }

    private void PlayerEntityUpdate()
    {
      this.m_highVisibilityEventValue = Mathf.MoveTowards(this.m_highVisibilityEventValue, 0.0f, Time.deltaTime * 1f);
      float num1 = 1.5f;
      float num2 = Mathf.Clamp(GM.CurrentMovementManager.GetTopSpeedInLastSecond() * 0.2f, 0.0f, 0.2f);
      float num3 = num1 - num2;
      float num4 = Mathf.Clamp(this.GetBodyMovementSpeed() * 0.01f, 0.0f, 0.3f);
      float num5 = num3 - num4 - this.m_highVisibilityEventValue;
      float y = this.Head.transform.localPosition.y;
      float num6 = 0.0f;
      if ((double) y < 1.20000004768372)
        num6 = (float) (((double) y - 1.20000004768372) * 1.0);
      this.VisibilityMult = Mathf.Clamp(num5 + Mathf.Clamp(num6, 0.0f, 0.6f), 0.9f, 2.1f);
      this.DebugString = "Total:" + this.VisibilityMult.ToString("#.##") + " TopSpeedPenalty:" + (object) num2 + " HandPenalty:" + (object) num4 + " VisibleEvent:" + (object) this.m_highVisibilityEventValue + " HeightBonus:" + (object) num6 + " Occlusion:" + (object) this.GetBodyOcclusion();
      float num7 = 0.1f;
      if (!this.HasAGunInHand())
        num7 += 0.03f;
      if (!this.HasAWeaponInHand())
        num7 += 0.1f;
      if ((double) this.GetPlayerHealth() < 0.100000001490116)
        num7 += 0.5f;
      else if ((double) this.GetPlayerHealth() < 0.400000005960464)
        num7 += 0.1f;
      for (int index = 0; index < this.PlayerEntities.Count; ++index)
      {
        AIEntity playerEntity = this.PlayerEntities[index];
        playerEntity.VisibilityMultiplier = !this.m_isGhosted ? (!this.m_isSuperVisible ? this.VisibilityMult : 0.1f) : 2.2f;
        playerEntity.DangerMultiplier = num7;
      }
      if (Physics.Raycast(this.Head.position, Random.onUnitSphere, out this.hit, 5f, (int) this.LM_OcclusionTesting, QueryTriggerInteraction.Ignore))
        this.MergeOcclusionDistance(this.hit.distance);
      else
        this.MergeOcclusionDistance(5f);
    }

    private bool HasAWeaponInHand()
    {
      for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
      {
        if ((Object) GM.CurrentMovementManager.Hands[index].CurrentInteractable != (Object) null && (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArm || GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRMeleeWeapon || GM.CurrentMovementManager.Hands[index].CurrentInteractable is SosigWeaponPlayerInterface))
          return true;
      }
      return false;
    }

    private bool HasAGunInHand()
    {
      for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
      {
        if ((Object) GM.CurrentMovementManager.Hands[index].CurrentInteractable != (Object) null && (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArm || GM.CurrentMovementManager.Hands[index].CurrentInteractable is SosigWeaponPlayerInterface && (GM.CurrentMovementManager.Hands[index].CurrentInteractable as SosigWeaponPlayerInterface).W.Type == SosigWeapon.SosigWeaponType.Gun))
          return true;
      }
      return false;
    }

    public void VisibleEvent(float intensity) => this.m_highVisibilityEventValue = Mathf.Max(this.m_highVisibilityEventValue, intensity);

    private float GetBodyMovementSpeed() => Mathf.Max(GM.CurrentMovementManager.Hands[0].Input.VelLinearLocal.magnitude, GM.CurrentMovementManager.Hands[1].Input.VelLinearLocal.magnitude);

    private void MergeOcclusionDistance(float distance) => this.m_averageOcclusionDistance = Mathf.Lerp(this.m_averageOcclusionDistance, distance, 0.05f);

    private float GetBodyOcclusion() => (double) this.m_averageOcclusionDistance < 3.5 ? (float) (1.0 - (double) this.m_averageOcclusionDistance / 3.5) : 0.0f;

    public FVRSoundEnvironment GetCurrentSoundEnvironment() => GM.CurrentSceneSettings.DefaultSoundEnvironment;

    public int GetPlayerIFF() => this.m_playerIFF;

    public void SetPlayerIFF(int iff)
    {
      this.m_playerIFF = iff;
      for (int index = 0; index < this.PlayerEntities.Count; ++index)
        this.PlayerEntities[index].IFFCode = this.m_playerIFF;
    }

    public bool IsHealing => this.m_isHealing;

    public bool IsDamResist => this.m_isDamResist;

    public bool isDamPowerUp => this.m_isDamPowerUp;

    public bool IsInfiniteAmmo => this.m_isInfiniteAmmo;

    public bool IsGhosted => this.m_isGhosted;

    public bool IsMuscleMeat => this.m_isMuscleMeat;

    public bool IsCyclops => this.m_isCyclops;

    public bool IsBlort => this.m_isBlort;

    public bool IsHurting => this.m_isHurting;

    public bool IsDamMult => this.m_isDamMult;

    public bool IsDamPowerDown => this.m_isDamPowerDown;

    public bool IsAmmoDrain => this.m_isAmmoDrain;

    public bool IsSuperVisible => this.m_isSuperVisible;

    public bool IsWeakMeat => this.m_isWeakMeat;

    public bool IsBiClops => this.m_isBiClops;

    public bool IsMoleEye => this.m_isMoleEye;

    public bool IsDlort => this.m_isDlort;

    public bool IsBadTrip => this.m_isBadTrip;

    public float GetDamageResist() => this.m_damageResist;

    public float GetDamageMult() => this.m_damageMult;

    public float GetRegenMult() => this.m_regenMult;

    public float GetMuscleMeatPower() => this.m_muscleMeatPower;

    public float GetCyclopsPower() => this.m_cyclopsPower;

    private void ActivateBuff(int i, bool isInverted)
    {
      if (!PUM.HasEffectPlayer(i, isInverted))
        return;
      if (!isInverted && (Object) this.BuffSystems_LeftHand[i] == (Object) null)
      {
        GameObject gameObject1 = Object.Instantiate<GameObject>(PUM.GetEffectPlayer(i, isInverted), GM.CurrentPlayerBody.LeftHand.transform.position, GM.CurrentPlayerBody.LeftHand.transform.rotation);
        gameObject1.transform.SetParent(GM.CurrentPlayerBody.LeftHand.transform);
        this.BuffSystems_LeftHand[i] = gameObject1;
        GameObject gameObject2 = Object.Instantiate<GameObject>(PUM.GetEffectPlayer(i, isInverted), GM.CurrentPlayerBody.RightHand.transform.position, GM.CurrentPlayerBody.RightHand.transform.rotation);
        gameObject2.transform.SetParent(GM.CurrentPlayerBody.RightHand.transform);
        this.BuffSystems_RightHand[i] = gameObject2;
      }
      else
      {
        if (!isInverted || !((Object) this.DeBuffSystems_LeftHand[i] == (Object) null))
          return;
        GameObject gameObject1 = Object.Instantiate<GameObject>(PUM.GetEffectPlayer(i, isInverted), GM.CurrentPlayerBody.LeftHand.transform.position, GM.CurrentPlayerBody.LeftHand.transform.rotation);
        gameObject1.transform.SetParent(GM.CurrentPlayerBody.LeftHand.transform);
        this.DeBuffSystems_LeftHand[i] = gameObject1;
        GameObject gameObject2 = Object.Instantiate<GameObject>(PUM.GetEffectPlayer(i, isInverted), GM.CurrentPlayerBody.RightHand.transform.position, GM.CurrentPlayerBody.RightHand.transform.rotation);
        gameObject2.transform.SetParent(GM.CurrentPlayerBody.RightHand.transform);
        this.DeBuffSystems_RightHand[i] = gameObject2;
      }
    }

    private void DeActivateBuff(int i)
    {
      if ((Object) this.BuffSystems_LeftHand[i] != (Object) null)
      {
        Object.Destroy((Object) this.BuffSystems_LeftHand[i]);
        this.BuffSystems_LeftHand[i] = (GameObject) null;
      }
      if (!((Object) this.BuffSystems_RightHand[i] != (Object) null))
        return;
      Object.Destroy((Object) this.BuffSystems_RightHand[i]);
      this.BuffSystems_RightHand[i] = (GameObject) null;
    }

    private void DeActivateDeBuff(int i)
    {
      if ((Object) this.DeBuffSystems_LeftHand[i] != (Object) null)
      {
        Object.Destroy((Object) this.DeBuffSystems_LeftHand[i]);
        this.DeBuffSystems_LeftHand[i] = (GameObject) null;
      }
      if (!((Object) this.DeBuffSystems_RightHand[i] != (Object) null))
        return;
      Object.Destroy((Object) this.DeBuffSystems_RightHand[i]);
      this.DeBuffSystems_RightHand[i] = (GameObject) null;
    }

    private void DeActivateAllBuffSystems()
    {
      for (int i = 0; i < 13; ++i)
      {
        this.DeActivateBuff(i);
        this.DeActivateDeBuff(i);
      }
    }

    public void DeActivatePowerIfActive(PowerupType type)
    {
    }

    public void ActivatePower(
      PowerupType type,
      PowerUpIntensity intensity,
      PowerUpDuration d,
      bool isPuke,
      bool isInverted,
      float DurationOverride = -1f)
    {
      float b = 1f;
      if (isPuke && (Object) GM.ZMaster != (Object) null)
      {
        int num = Random.Range(3, 5);
        for (int index = 0; index < num; ++index)
          GM.ZMaster.VomitRandomThing();
      }
      switch (d)
      {
        case PowerUpDuration.Full:
          b = 30f;
          break;
        case PowerUpDuration.Short:
          b = 20f;
          break;
        case PowerUpDuration.VeryShort:
          b = 10f;
          break;
        case PowerUpDuration.Blip:
          b = 2f;
          break;
        case PowerUpDuration.SuperLong:
          b = 40f;
          break;
      }
      if ((double) DurationOverride > 0.0)
        b = DurationOverride;
      switch (type)
      {
        case PowerupType.Health:
          float f = 0.0f;
          switch (intensity)
          {
            case PowerUpIntensity.High:
              f = 1f;
              break;
            case PowerUpIntensity.Medium:
              f = 0.5f;
              break;
            case PowerUpIntensity.Low:
              f = 0.25f;
              break;
          }
          if (!isInverted)
          {
            this.HealPercent(f);
            break;
          }
          this.HarmPercent(f);
          break;
        case PowerupType.QuadDamage:
          if (!isInverted)
          {
            switch (intensity)
            {
              case PowerUpIntensity.High:
                this.m_damageMult = 4f;
                break;
              case PowerUpIntensity.Medium:
                this.m_damageMult = 3f;
                break;
              case PowerUpIntensity.Low:
                this.m_damageMult = 2f;
                break;
            }
            this.m_isDamPowerUp = true;
            this.m_isDamPowerDown = false;
            this.DeActivateDeBuff(1);
            this.m_buffTime_DamPowerUp = Mathf.Max(this.m_buffTime_DamPowerUp, b);
            break;
          }
          switch (intensity)
          {
            case PowerUpIntensity.High:
              this.m_damageMult = 0.125f;
              break;
            case PowerUpIntensity.Medium:
              this.m_damageMult = 0.25f;
              break;
            case PowerUpIntensity.Low:
              this.m_damageMult = 0.5f;
              break;
          }
          this.m_isDamPowerDown = true;
          this.m_isDamPowerUp = false;
          this.DeActivateBuff(1);
          this.m_debuffTime_DamPowerDown = Mathf.Max(this.m_debuffTime_DamPowerDown, b);
          break;
        case PowerupType.InfiniteAmmo:
          if (!isInverted)
          {
            this.m_isInfiniteAmmo = true;
            this.m_isAmmoDrain = false;
            this.DeActivateDeBuff(2);
            this.m_buffTime_InfiniteAmmo = Mathf.Max(this.m_buffTime_InfiniteAmmo, b);
            break;
          }
          this.m_isAmmoDrain = true;
          this.m_isInfiniteAmmo = false;
          this.DeActivateBuff(2);
          this.m_debuffTime_AmmoDrain = Mathf.Max(this.m_debuffTime_AmmoDrain, b);
          break;
        case PowerupType.Invincibility:
          if (!isInverted)
          {
            switch (intensity)
            {
              case PowerUpIntensity.High:
                this.m_isDamResist = true;
                this.m_isDamMult = false;
                this.DeActivateDeBuff(3);
                this.m_damageResist = 0.0f;
                this.m_buffTime_DamResist = Mathf.Max(this.m_buffTime_DamResist, b);
                break;
              case PowerUpIntensity.Medium:
                this.m_isDamResist = true;
                this.m_isDamMult = false;
                this.DeActivateDeBuff(3);
                this.m_damageResist = 0.5f;
                this.m_buffTime_DamResist = Mathf.Max(this.m_buffTime_DamResist, b);
                break;
              case PowerUpIntensity.Low:
                this.m_isDamResist = true;
                this.m_isDamMult = false;
                this.DeActivateDeBuff(3);
                this.m_damageResist = 0.75f;
                this.m_buffTime_DamResist = Mathf.Max(this.m_buffTime_DamResist, b);
                break;
            }
          }
          else
          {
            switch (intensity)
            {
              case PowerUpIntensity.High:
                this.m_isDamMult = true;
                this.m_isDamResist = false;
                this.DeActivateBuff(3);
                this.m_damageResist = 4f;
                this.m_debuffTime_DamMult = Mathf.Max(this.m_debuffTime_DamMult, b);
                break;
              case PowerUpIntensity.Medium:
                this.m_isDamMult = true;
                this.m_isDamResist = false;
                this.DeActivateBuff(3);
                this.m_damageResist = 3f;
                this.m_debuffTime_DamMult = Mathf.Max(this.m_debuffTime_DamMult, b);
                break;
              case PowerUpIntensity.Low:
                this.m_isDamResist = true;
                this.m_isDamMult = false;
                this.DeActivateBuff(3);
                this.m_damageResist = 2f;
                this.m_debuffTime_DamMult = Mathf.Max(this.m_debuffTime_DamMult, b);
                break;
            }
          }
          break;
        case PowerupType.Ghosted:
          if (!isInverted)
          {
            this.m_isGhosted = true;
            this.m_isSuperVisible = false;
            this.DeActivateDeBuff(4);
            this.m_buffTime_Ghosted = Mathf.Max(this.m_buffTime_Ghosted, b);
            break;
          }
          this.m_isSuperVisible = true;
          this.m_isGhosted = false;
          this.DeActivateBuff(4);
          this.m_debuffTime_SuperVisible = Mathf.Max(this.m_debuffTime_SuperVisible, b);
          break;
        case PowerupType.FarOutMeat:
          if (!isInverted)
          {
            GM.CurrentSceneSettings.SetFarOutMan(true);
            GM.CurrentSceneSettings.SetBadTrip(false);
            this.m_buffTime_FarOutMan = Mathf.Max(this.m_buffTime_FarOutMan, b);
            this.m_isFarOutMan = true;
            this.m_isBadTrip = false;
            break;
          }
          GM.CurrentSceneSettings.SetBadTrip(true);
          GM.CurrentSceneSettings.SetFarOutMan(false);
          this.m_debuffTime_BadTrip = Mathf.Max(this.m_debuffTime_BadTrip, b);
          this.m_isBadTrip = true;
          this.m_isFarOutMan = false;
          break;
        case PowerupType.MuscleMeat:
          if (!isInverted)
          {
            switch (intensity)
            {
              case PowerUpIntensity.High:
                this.m_muscleMeatPower = 4f;
                break;
              case PowerUpIntensity.Medium:
                this.m_muscleMeatPower = 3f;
                break;
              case PowerUpIntensity.Low:
                this.m_muscleMeatPower = 2f;
                break;
            }
            this.m_isMuscleMeat = true;
            this.m_isWeakMeat = false;
            this.DeActivateDeBuff(6);
            this.m_buffTime_MuscleMeat = Mathf.Max(this.m_buffTime_MuscleMeat, b);
            break;
          }
          switch (intensity)
          {
            case PowerUpIntensity.High:
              this.m_muscleMeatPower = 0.125f;
              break;
            case PowerUpIntensity.Medium:
              this.m_muscleMeatPower = 0.25f;
              break;
            case PowerUpIntensity.Low:
              this.m_muscleMeatPower = 0.5f;
              break;
          }
          this.m_isWeakMeat = true;
          this.m_isMuscleMeat = false;
          this.DeActivateBuff(6);
          this.m_debuffTime_WeakMeat = Mathf.Max(this.m_debuffTime_WeakMeat, b);
          break;
        case PowerupType.HomeTown:
          if (!isInverted)
          {
            double point = (double) GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.PowerupPoint_HomeTown.position, true, GM.CurrentSceneSettings.PowerupPoint_HomeTown.forward);
            break;
          }
          double point1 = (double) GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.PowerupPoint_InverseHomeTown.position, true, GM.CurrentSceneSettings.PowerupPoint_InverseHomeTown.forward);
          break;
        case PowerupType.SnakeEye:
          if (!isInverted)
          {
            GM.CurrentSceneSettings.SetSnakeEyes(true);
            GM.CurrentSceneSettings.SetMoleEye(false);
            this.m_buffTime_SnakeEyes = Mathf.Max(this.m_buffTime_SnakeEyes, b);
            this.m_isSnakeEyes = true;
            this.m_isMoleEye = false;
            break;
          }
          GM.CurrentSceneSettings.SetMoleEye(true);
          GM.CurrentSceneSettings.SetSnakeEyes(false);
          this.m_debuffTime_MoleEye = Mathf.Max(this.m_debuffTime_MoleEye, b);
          this.m_isMoleEye = true;
          this.m_isSnakeEyes = false;
          break;
        case PowerupType.Blort:
          if (!isInverted)
          {
            GM.CurrentSceneSettings.SetBlort(true);
            GM.CurrentSceneSettings.SetDlort(false);
            this.m_buffTime_Blort = Mathf.Max(this.m_buffTime_Blort, b);
            this.m_isBlort = true;
            this.m_isDlort = false;
            break;
          }
          GM.CurrentSceneSettings.SetDlort(true);
          GM.CurrentSceneSettings.SetBlort(false);
          this.m_debuffTime_Dlort = Mathf.Max(this.m_debuffTime_Dlort, b);
          this.m_isDlort = true;
          this.m_isBlort = false;
          break;
        case PowerupType.Regen:
          if (!isInverted)
          {
            switch (intensity)
            {
              case PowerUpIntensity.High:
                this.m_buffIntensity_HealHarm = 0.2f;
                break;
              case PowerUpIntensity.Medium:
                this.m_buffIntensity_HealHarm = 0.1f;
                break;
              case PowerUpIntensity.Low:
                this.m_buffIntensity_HealHarm = 0.05f;
                break;
            }
            this.m_isHealing = true;
            this.m_isHurting = false;
            this.DeActivateDeBuff(10);
            this.m_buffTime_Heal = Mathf.Max(this.m_buffTime_Heal, b);
            break;
          }
          switch (intensity)
          {
            case PowerUpIntensity.High:
              this.m_buffIntensity_HealHarm = 0.2f;
              break;
            case PowerUpIntensity.Medium:
              this.m_buffIntensity_HealHarm = 0.1f;
              break;
            case PowerUpIntensity.Low:
              this.m_buffIntensity_HealHarm = 0.05f;
              break;
          }
          this.m_isHurting = true;
          this.m_isHealing = false;
          this.DeActivateBuff(10);
          this.m_debuffTime_Hurt = Mathf.Max(this.m_debuffTime_Hurt, b);
          break;
        case PowerupType.Cyclops:
          switch (intensity)
          {
            case PowerUpIntensity.High:
              this.m_cyclopsPower = 4f;
              break;
            case PowerUpIntensity.Medium:
              this.m_cyclopsPower = 3f;
              break;
            case PowerUpIntensity.Low:
              this.m_cyclopsPower = 2f;
              break;
          }
          if (!isInverted)
          {
            this.m_isCyclops = true;
            this.m_isBiClops = false;
            this.DeActivateDeBuff(11);
            this.VFX_Biclops.SetActive(false);
            this.m_buffTime_Cyclops = Mathf.Max(this.m_buffTime_Cyclops, b);
          }
          else
          {
            this.m_isBiClops = true;
            this.m_isCyclops = false;
            this.DeActivateBuff(11);
            this.VFX_Cyclops.SetActive(false);
            this.m_debuffTime_BiClops = Mathf.Max(this.m_debuffTime_BiClops, b);
          }
          if (!isInverted)
          {
            this.VFX_Cyclops.SetActive(true);
            break;
          }
          this.VFX_Biclops.SetActive(true);
          break;
        case PowerupType.WheredIGo:
          if (GM.CurrentMovementManager.Mode == FVRMovementManager.MovementMode.Teleport || GM.CurrentMovementManager.Mode == FVRMovementManager.MovementMode.Dash || (GM.CurrentMovementManager.Mode == FVRMovementManager.MovementMode.JoystickTeleport || GM.CurrentMovementManager.Mode == FVRMovementManager.MovementMode.SlideToTarget))
          {
            int index = Random.Range(0, GM.CurrentSceneSettings.PowerPoints_WheredIGo_TP.Count);
            double point2 = (double) GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.PowerPoints_WheredIGo_TP[index].position, false, GM.CurrentSceneSettings.PowerPoints_WheredIGo_TP[index].forward);
            break;
          }
          int index1 = Random.Range(0, GM.CurrentSceneSettings.PowerPoints_WheredIGo_Grav.Count);
          double point3 = (double) GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.PowerPoints_WheredIGo_Grav[index1].position, false, GM.CurrentSceneSettings.PowerPoints_WheredIGo_Grav[index1].forward);
          break;
      }
      this.ActivateBuff((int) type, isInverted);
    }

    private void UpdatePowerUps()
    {
      if (this.m_isHealing && (double) this.m_buffTime_Heal > 0.0)
      {
        this.m_buffTime_Heal -= Time.deltaTime;
        if ((double) this.m_buffTime_Heal <= 0.0)
        {
          this.DeActivateBuff(10);
          this.m_isHealing = false;
        }
      }
      if (this.m_isDamResist && (double) this.m_buffTime_DamResist > 0.0)
      {
        this.m_buffTime_DamResist -= Time.deltaTime;
        if ((double) this.m_buffTime_DamResist <= 0.0)
        {
          this.DeActivateBuff(3);
          this.m_isDamResist = false;
        }
      }
      if (this.m_isDamPowerUp && (double) this.m_buffTime_DamPowerUp > 0.0)
      {
        this.m_buffTime_DamPowerUp -= Time.deltaTime;
        if ((double) this.m_buffTime_DamPowerUp <= 0.0)
        {
          this.DeActivateBuff(1);
          this.m_isDamPowerUp = false;
        }
      }
      if (this.m_isInfiniteAmmo && (double) this.m_buffTime_InfiniteAmmo > 0.0)
      {
        this.m_buffTime_InfiniteAmmo -= Time.deltaTime;
        if ((double) this.m_buffTime_InfiniteAmmo <= 0.0)
        {
          this.DeActivateBuff(2);
          this.m_isInfiniteAmmo = false;
        }
      }
      if (this.m_isGhosted && (double) this.m_buffTime_Ghosted > 0.0)
      {
        this.m_buffTime_Ghosted -= Time.deltaTime;
        if ((double) this.m_buffTime_Ghosted <= 0.0)
        {
          this.DeActivateBuff(4);
          this.m_isGhosted = false;
        }
      }
      if (this.m_isMuscleMeat && (double) this.m_buffTime_MuscleMeat > 0.0)
      {
        this.m_buffTime_MuscleMeat -= Time.deltaTime;
        if ((double) this.m_buffTime_MuscleMeat <= 0.0)
        {
          this.DeActivateBuff(6);
          this.m_isMuscleMeat = false;
        }
      }
      if (this.m_isCyclops && (double) this.m_buffTime_Cyclops > 0.0)
      {
        this.m_buffTime_Cyclops -= Time.deltaTime;
        if ((double) this.m_buffTime_Cyclops <= 0.0)
        {
          this.DeActivateBuff(11);
          this.m_isCyclops = false;
          this.VFX_Cyclops.SetActive(false);
        }
      }
      if (this.m_isSnakeEyes && (double) this.m_buffTime_SnakeEyes > 0.0)
      {
        this.m_buffTime_SnakeEyes -= Time.deltaTime;
        if ((double) this.m_buffTime_SnakeEyes <= 0.0)
        {
          this.m_isSnakeEyes = false;
          GM.CurrentSceneSettings.SetSnakeEyes(false);
        }
      }
      if (this.m_isMoleEye && (double) this.m_debuffTime_MoleEye > 0.0)
      {
        this.m_debuffTime_MoleEye -= Time.deltaTime;
        if ((double) this.m_debuffTime_MoleEye <= 0.0)
        {
          this.m_isMoleEye = false;
          GM.CurrentSceneSettings.SetMoleEye(false);
        }
      }
      if (this.m_isBlort && (double) this.m_buffTime_Blort > 0.0)
      {
        this.m_buffTime_Blort -= Time.deltaTime;
        if ((double) this.m_buffTime_Blort <= 0.0)
        {
          this.DeActivateBuff(9);
          this.m_isBlort = false;
          GM.CurrentSceneSettings.SetBlort(false);
        }
      }
      if (this.m_isDlort && (double) this.m_debuffTime_Dlort > 0.0)
      {
        this.m_debuffTime_Dlort -= Time.deltaTime;
        if ((double) this.m_debuffTime_Dlort <= 0.0)
        {
          this.DeActivateBuff(9);
          this.m_isDlort = false;
          GM.CurrentSceneSettings.SetDlort(false);
        }
      }
      if (this.m_isFarOutMan && (double) this.m_buffTime_FarOutMan > 0.0)
      {
        this.m_buffTime_FarOutMan -= Time.deltaTime;
        if ((double) this.m_buffTime_FarOutMan <= 0.0)
        {
          this.m_isFarOutMan = false;
          GM.CurrentSceneSettings.SetFarOutMan(false);
        }
      }
      if (this.m_isBadTrip && (double) this.m_debuffTime_BadTrip > 0.0)
      {
        this.m_debuffTime_BadTrip -= Time.deltaTime;
        if ((double) this.m_debuffTime_BadTrip <= 0.0)
        {
          this.m_isBadTrip = false;
          GM.CurrentSceneSettings.SetBadTrip(false);
        }
      }
      if (this.m_isHurting && (double) this.m_debuffTime_Hurt > 0.0)
      {
        this.m_debuffTime_Hurt -= Time.deltaTime;
        if ((double) this.m_debuffTime_Hurt <= 0.0)
        {
          this.DeActivateDeBuff(10);
          this.m_isHurting = false;
        }
      }
      if (this.m_isDamMult && (double) this.m_debuffTime_DamMult > 0.0)
      {
        this.m_debuffTime_DamMult -= Time.deltaTime;
        if ((double) this.m_debuffTime_DamMult <= 0.0)
        {
          this.DeActivateDeBuff(3);
          this.m_isDamMult = false;
        }
      }
      if (this.m_isDamPowerDown && (double) this.m_debuffTime_DamPowerDown > 0.0)
      {
        this.m_debuffTime_DamPowerDown -= Time.deltaTime;
        if ((double) this.m_debuffTime_DamPowerDown <= 0.0)
        {
          this.DeActivateDeBuff(1);
          this.m_isDamPowerDown = false;
        }
      }
      if (this.m_isAmmoDrain && (double) this.m_debuffTime_AmmoDrain > 0.0)
      {
        this.m_debuffTime_AmmoDrain -= Time.deltaTime;
        if ((double) this.m_debuffTime_AmmoDrain <= 0.0)
        {
          this.DeActivateDeBuff(2);
          this.m_isAmmoDrain = false;
        }
      }
      if (this.m_isSuperVisible && (double) this.m_debuffTime_SuperVisible > 0.0)
      {
        this.m_debuffTime_SuperVisible -= Time.deltaTime;
        if ((double) this.m_debuffTime_SuperVisible <= 0.0)
        {
          this.DeActivateDeBuff(4);
          this.m_isSuperVisible = false;
        }
      }
      if (this.m_isWeakMeat && (double) this.m_debuffTime_WeakMeat > 0.0)
      {
        this.m_debuffTime_WeakMeat -= Time.deltaTime;
        if ((double) this.m_debuffTime_WeakMeat <= 0.0)
        {
          this.DeActivateDeBuff(6);
          this.m_isWeakMeat = false;
        }
      }
      if (this.m_isBiClops && (double) this.m_debuffTime_BiClops > 0.0)
      {
        this.m_debuffTime_BiClops -= Time.deltaTime;
        if ((double) this.m_debuffTime_BiClops <= 0.0)
        {
          this.DeActivateDeBuff(11);
          this.m_isBiClops = false;
          this.VFX_Biclops.SetActive(false);
        }
      }
      if (this.m_isHealing)
        this.HealPercent(this.m_buffIntensity_HealHarm * Time.deltaTime);
      if (this.m_isHurting)
        this.HarmPercent(this.m_buffIntensity_HealHarm * Time.deltaTime);
      if (this.m_isCyclops)
        this.VFX_Cyclops.SetActive(true);
      else
        this.VFX_Cyclops.SetActive(false);
      if (this.m_isBiClops)
        this.VFX_Biclops.SetActive(true);
      else
        this.VFX_Biclops.SetActive(false);
    }

    public void UpdateCameraPost()
    {
      if (!((Object) this.PostLayer != (Object) null))
        return;
      if (GM.Options.PerformanceOptions.IsPostEnabled_AO || GM.Options.PerformanceOptions.IsPostEnabled_Bloom || GM.Options.PerformanceOptions.IsPostEnabled_CC)
        this.PostLayer.enabled = true;
      else
        this.PostLayer.enabled = false;
    }

    public void Init(FVRSceneSettings SceneSettings)
    {
      this.SetPlayerIFF(SceneSettings.DefaultPlayerIFF);
      for (int index = 0; index < this.Hitboxes.Length; ++index)
      {
        if ((Object) this.Hitboxes[index] != (Object) null)
          this.Hitboxes[index].IsActivated = SceneSettings.AreHitboxesEnabled;
      }
      if (SceneSettings.AreQuickbeltSlotsEnabled)
        this.ConfigureQuickbelt(GM.Options.QuickbeltOptions.QuickbeltPreset);
      Object.Instantiate<GameObject>(this.WristMenuPrefab, Vector3.zero, Quaternion.identity).GetComponent<FVRWristMenu>().SetHandsAndFace(this.RightHand.GetComponent<FVRViveHand>(), this.LeftHand.GetComponent<FVRViveHand>(), this.EyeCam.transform);
      if (GM.CurrentSceneSettings.DoesUseHealthBar)
        this.HealthBar = Object.Instantiate<GameObject>(this.HealthBarPrefab, Vector3.zero, Quaternion.identity);
      this.m_startingHealth = this.Health;
    }

    public bool RegisterPlayerHit(float DamagePoints, bool FromSelf)
    {
      GM.CurrentSceneSettings.OnPlayerTookDamage(DamagePoints / this.m_startingHealth);
      if (GM.CurrentSceneSettings.DoesDamageGetRegistered && (Object) GM.CurrentSceneSettings.DeathResetPoint != (Object) null && !GM.IsDead())
      {
        this.Health -= DamagePoints;
        this.HitEffect();
        if ((double) this.Health <= 0.0)
        {
          if ((Object) GM.CurrentMovementManager.Hands[0].CurrentInteractable != (Object) null && !(GM.CurrentMovementManager.Hands[0].CurrentInteractable is FVRPhysicalObject))
            GM.CurrentMovementManager.Hands[0].CurrentInteractable.ForceBreakInteraction();
          if ((Object) GM.CurrentMovementManager.Hands[1].CurrentInteractable != (Object) null && !(GM.CurrentMovementManager.Hands[1].CurrentInteractable is FVRPhysicalObject))
            GM.CurrentMovementManager.Hands[1].CurrentInteractable.ForceBreakInteraction();
          ManagerSingleton<GM>.Instance.KillPlayer(FromSelf);
          return true;
        }
      }
      return false;
    }

    public void KillPlayer(bool FromSelf)
    {
      if (GM.IsDead())
        return;
      if ((Object) GM.CurrentMovementManager.Hands[0].CurrentInteractable != (Object) null && !(GM.CurrentMovementManager.Hands[0].CurrentInteractable is FVRPhysicalObject))
        GM.CurrentMovementManager.Hands[0].CurrentInteractable.ForceBreakInteraction();
      if ((Object) GM.CurrentMovementManager.Hands[1].CurrentInteractable != (Object) null && !(GM.CurrentMovementManager.Hands[1].CurrentInteractable is FVRPhysicalObject))
        GM.CurrentMovementManager.Hands[1].CurrentInteractable.ForceBreakInteraction();
      ManagerSingleton<GM>.Instance.KillPlayer(FromSelf);
    }

    public float GetPlayerHealth() => Mathf.Clamp(this.Health / this.m_startingHealth, 0.0f, 1f);

    public void HealPercent(float f)
    {
      this.Health += this.m_startingHealth * f;
      this.Health = Mathf.Clamp(this.Health, 0.0f, this.m_startingHealth);
    }

    public void HarmPercent(float f)
    {
      this.Health -= this.m_startingHealth * f;
      if ((double) this.Health > 0.0)
        return;
      this.KillPlayer(false);
    }

    public void ResetHealth() => this.Health = this.m_startingHealth;

    public void SetHealthThreshold(float h)
    {
      this.m_startingHealth = h;
      this.Health = h;
    }

    public int GetPlayerHealthRaw() => (int) this.Health;

    public int GetMaxHealthPlayerRaw() => (int) this.m_startingHealth;

    public void DisableHands()
    {
      this.LeftHand.gameObject.SetActive(false);
      this.RightHand.gameObject.SetActive(false);
    }

    public void EnableHands()
    {
      this.LeftHand.gameObject.SetActive(true);
      this.RightHand.gameObject.SetActive(true);
    }

    public void UpdatePlayerBodyPositions()
    {
      this.MoveBodyTargets();
      this.MoveHitBoxes();
    }

    public void MoveBodyTargets()
    {
      this.MoveTransformRelativeToTarget(this.NeckJointTransform, this.NeckJointTargetsThis, false);
      this.MoveTransformRelativeToTarget(this.NeckTransform, this.NeckTargetsThis, true);
      this.MoveTransformRelativeToTarget(this.TorsoTransform, this.TorsoTargetsThis, true);
    }

    public void MoveHitBoxes()
    {
      for (int index = 0; index < this.Hitboxes.Length; ++index)
        this.Hitboxes[index].UpdatePositions();
    }

    public void EnableHitBoxes()
    {
      for (int index = 0; index < this.Hitboxes.Length; ++index)
        this.Hitboxes[index].IsActivated = true;
    }

    public void DisableHitBoxes()
    {
      for (int index = 0; index < this.Hitboxes.Length; ++index)
        this.Hitboxes[index].IsActivated = false;
    }

    public void HitEffect() => this.PSystemDamage.Emit(1);

    private void MoveTransformRelativeToTarget(
      Transform trans,
      Transform target,
      bool tracksPosition)
    {
      if (tracksPosition)
        trans.position = target.position;
      Vector3 forward1 = target.forward;
      Vector3 vector3 = forward1;
      vector3.y = 0.0f;
      vector3.Normalize();
      Vector3 zero = Vector3.zero;
      Vector3 a = (double) forward1.y <= 0.0 ? target.up : -target.up;
      a.y = 0.0f;
      a.Normalize();
      float num = Mathf.Clamp(Vector3.Dot(vector3, forward1), 0.0f, 1f);
      Vector3 forward2 = Vector3.Lerp(a, vector3, num * num);
      trans.rotation = Quaternion.LookRotation(forward2, Vector3.up);
    }

    public void MoveQuickbeltContents(Vector3 dir)
    {
      for (int index = 0; index < this.QuickbeltSlots.Count; ++index)
        this.QuickbeltSlots[index].MoveContentsInstant(dir);
    }

    public void MoveQuickbeltContentsCheap(Vector3 dir)
    {
      for (int index = 0; index < this.QuickbeltSlots.Count; ++index)
        this.QuickbeltSlots[index].MoveContentsCheap(dir);
    }

    public void ConfigureQuickbelt(int index)
    {
      if (this.QuickbeltSlots.Count > 0)
      {
        for (int index1 = this.QuickbeltSlots.Count - 1; index1 >= 0; --index1)
        {
          if ((Object) this.QuickbeltSlots[index1] == (Object) null)
            this.QuickbeltSlots.RemoveAt(index1);
          else if (this.QuickbeltSlots[index1].IsPlayer)
          {
            if ((Object) this.QuickbeltSlots[index1].CurObject != (Object) null)
              this.QuickbeltSlots[index1].CurObject.ClearQuickbeltState();
            Object.Destroy((Object) this.QuickbeltSlots[index1].gameObject);
            this.QuickbeltSlots.RemoveAt(index1);
          }
        }
      }
      GameObject gameObject = Object.Instantiate<GameObject>(ManagerSingleton<GM>.Instance.QuickbeltConfigurations[Mathf.Clamp(index, 0, ManagerSingleton<GM>.Instance.QuickbeltConfigurations.Length)], this.Torso.position, this.Torso.rotation);
      gameObject.transform.SetParent(this.Torso.transform);
      gameObject.transform.localPosition = Vector3.zero;
      foreach (Transform transform in gameObject.transform)
      {
        if (transform.gameObject.tag == "QuickbeltSlot")
        {
          FVRQuickBeltSlot component = transform.GetComponent<FVRQuickBeltSlot>();
          if (GM.Options.QuickbeltOptions.QuickbeltHandedness > 0)
          {
            Vector3 forward1 = component.PoseOverride.forward;
            Vector3 up = component.PoseOverride.up;
            Vector3 forward2 = Vector3.Reflect(forward1, component.transform.right);
            Vector3 upwards = Vector3.Reflect(up, component.transform.right);
            component.PoseOverride.rotation = Quaternion.LookRotation(forward2, upwards);
          }
          this.QuickbeltSlots.Add(component);
        }
      }
      for (int index1 = 0; index1 < this.QuickbeltSlots.Count; ++index1)
      {
        if (this.QuickbeltSlots[index1].IsPlayer)
        {
          this.QuickbeltSlots[index1].transform.SetParent(this.Torso);
          this.QuickbeltSlots[index1].QuickbeltRoot = (Transform) null;
          if (GM.Options.QuickbeltOptions.QuickbeltHandedness > 0)
            this.QuickbeltSlots[index1].transform.localPosition = new Vector3(-this.QuickbeltSlots[index1].transform.localPosition.x, this.QuickbeltSlots[index1].transform.localPosition.y, this.QuickbeltSlots[index1].transform.localPosition.z);
        }
      }
      foreach (PlayerBackPack playerBackPack in Object.FindObjectsOfType<PlayerBackPack>())
        playerBackPack.RegisterQuickbeltSlots();
      Object.Destroy((Object) gameObject);
    }
  }
}
