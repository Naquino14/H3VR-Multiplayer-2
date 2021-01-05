// Decompiled with JetBrains decompiler
// Type: FistVR.LAPD2019
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LAPD2019 : FVRFireArm
  {
    [Header("LAPD2019 Components Config")]
    public Transform Hammer;
    private float m_hammerForwardRot;
    private float m_hammerBackwardRot = -49f;
    private float m_hammerCurrentRot;
    public Transform Trigger1;
    public Transform Trigger2;
    private float m_triggerForwardRot;
    private float m_triggerBackwardRot = 30f;
    private float m_triggerCurrentRot1;
    private float m_triggerCurrentRot2;
    private bool m_hasTriggerCycled;
    public Transform CylinderReleaseButton;
    public Transform CylinderReleaseButtonForwardPos;
    public Transform CylinderReleaseButtonRearPos;
    private bool m_isCylinderReleasePressed;
    private float m_cylinderReleaseButtonLerp;
    public LAPD2019BoltHandle BoltHandle;
    [Header("LAPD2019 TransformPoints")]
    public Transform BoltForward;
    public Transform BoltRearward;
    [Header("Trigger Config")]
    public float TriggerFireThreshold = 0.75f;
    public float TriggerResetThreshold = 0.25f;
    private float m_triggerFloat;
    private float m_mechanicalCycleLerp;
    [Header("Cylinder Config")]
    public Transform CylinderArm;
    private bool m_isCylinderArmLocked = true;
    private bool m_wasCylinderArmLocked = true;
    private float CylinderArmRot;
    public Vector2 CylinderRotRange = new Vector2(0.0f, 105f);
    public bool GravityRotsCylinderPositive = true;
    public LAPD2019Cylinder Cylinder;
    private int m_curChamber;
    private float m_tarChamberLerp;
    private float m_curChamberLerp;
    [Header("Chambers Config")]
    public FVRFireArmChamber[] Chambers;
    [Header("Spinning Config")]
    public Transform PoseSpinHolder;
    public bool CanSpin = true;
    private bool m_isSpinning;
    [Header("Audio Config")]
    public AudioEvent ThermalClip_In;
    public AudioEvent ThermalClip_Eject;
    public AudioEvent CapacitorCharge;
    public AudioEvent Chirp_FullyCharged;
    public AudioEvent Chirp_LowBattery;
    public AudioEvent Chirp_FreshBattery;
    public AudioEvent Chirp_DeadBattery;
    [Header("Pose Config")]
    public bool UsesAltPoseSwitch = true;
    public Transform Pose_Main;
    public Transform Pose_Reloading;
    private bool m_isInMainpose = true;
    [Header("Electrical System Config")]
    public GameObject FauxBattery;
    public Renderer LED_Rear;
    public Renderer LED_FauxBattery_Side;
    public Renderer LED_FauxBattery_Under;
    public LAPD2019Laser LaserSystem;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    public Color Color_Emissive_Red;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    public Color Color_Emissive_Green;
    private bool m_isAutoChargeEnabled;
    private float m_capacitorCharge;
    private bool m_isCapacitorCharging;
    private bool m_isCapacitorCharged;
    public LAPD2019BatteryTriggerWell BatteryWellTrigger;
    private bool m_hasBattery;
    private float m_batteryCharge;
    [Header("Thermal Clip Config")]
    public GameObject ThermalClipProxy;
    public GameObject ThermalClipLoadTrigger;
    public ParticleSystem PSystem_ThermalSteam;
    public ParticleSystem PSystem_ThermalSteam2;
    public ParticleSystem PSystem_SparksShot1;
    public ParticleSystem PSystem_SparksShot2;
    public Transform ThermalClipEjectPos;
    private ParticleSystem.EmissionModule m_pSystem_ThermalSteam_Emission;
    private ParticleSystem.EmissionModule m_pSystem_ThermalSteam_Emission2;
    private bool m_hasThermalClip = true;
    public Renderer[] GlowingParts;
    public Renderer ProxyClipRenderer;
    private float m_thermalClipLoadTriggerEnableTick;
    private float m_heatThermalClip;
    private float m_heatSystem;
    private float m_barrelHeatDamage;
    public FVRObject BatteryPrefab;
    public FVRObject ThermalClipPrefab;
    public GameObject[] ProxMalfunctionPrefab;
    private float xSpinRot;
    private float xSpinVel;
    private float m_CylCloseVel;

    public bool isCylinderArmLocked => this.m_isCylinderArmLocked;

    public int CurChamber
    {
      get => this.m_curChamber;
      set => this.m_curChamber = value % this.Cylinder.numChambers;
    }

    public bool HasBattery => this.m_hasBattery;

    protected override void Awake()
    {
      base.Awake();
      this.FauxBattery.SetActive(false);
      if ((Object) this.PoseOverride_Touch != (Object) null)
      {
        this.Pose_Main.localPosition = this.PoseOverride_Touch.localPosition;
        this.Pose_Main.localRotation = this.PoseOverride_Touch.localRotation;
      }
      this.m_pSystem_ThermalSteam_Emission = this.PSystem_ThermalSteam.emission;
      this.m_pSystem_ThermalSteam_Emission.rateOverTimeMultiplier = 0.0f;
      this.m_pSystem_ThermalSteam_Emission2 = this.PSystem_ThermalSteam2.emission;
      this.m_pSystem_ThermalSteam_Emission2.rateOverTimeMultiplier = 0.0f;
      this.ThermalClipLoadTrigger.SetActive(false);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.UpdateElectricalSystem();
      this.UpdateThermalSystem();
    }

    private void UpdateThermalSystem()
    {
      float num = 0.0f;
      if ((double) this.m_heatSystem > 0.0)
        num = Mathf.Min(this.m_heatSystem, Time.deltaTime * 0.2f);
      if (this.m_hasThermalClip && (double) this.m_heatThermalClip < 1.0)
      {
        this.m_heatThermalClip += num * 0.4f;
        this.m_heatSystem -= num;
      }
      if ((double) this.m_heatSystem > 0.0)
        this.m_heatSystem -= Time.deltaTime * 0.005f;
      this.m_heatSystem = Mathf.Clamp(this.m_heatSystem, 0.0f, 2f);
      for (int index = 0; index < this.GlowingParts.Length; ++index)
        this.GlowingParts[index].material.SetFloat("_EmissionWeight", Mathf.Clamp(Mathf.Pow(this.m_heatSystem, 2.5f), 0.0f, 1f));
      if (this.m_hasThermalClip)
        this.ProxyClipRenderer.material.SetFloat("_EmissionWeight", Mathf.Clamp(Mathf.Pow(this.m_heatThermalClip, 1.5f), 0.0f, 1f));
      bool flag1 = false;
      if (this.BoltHandle.HandleRot != LAPD2019BoltHandle.BoltActionHandleRot.Down)
        flag1 = true;
      this.m_pSystem_ThermalSteam_Emission.rateOverTimeMultiplier = (!flag1 ? (!this.m_hasThermalClip ? Mathf.Clamp((float) (((double) this.m_heatSystem - 0.5) * 2.0), 0.0f, 1f) : Mathf.Clamp((float) (((double) Mathf.Max(this.m_heatThermalClip, this.m_heatSystem) - 0.5) * 2.0), 0.0f, 1f)) : (!this.m_hasThermalClip ? this.m_heatSystem : Mathf.Max(this.m_heatThermalClip, this.m_heatSystem))) * 25f;
      this.m_pSystem_ThermalSteam_Emission2.rateOverTimeMultiplier = Mathf.Clamp(Mathf.Pow((float) (((double) this.m_heatSystem - 0.5) * 2.0), 2.5f), 0.0f, 1f) * 35f;
      if (!this.m_hasThermalClip && (double) this.m_thermalClipLoadTriggerEnableTick > 0.0)
        this.m_thermalClipLoadTriggerEnableTick -= Time.deltaTime;
      bool flag2 = false;
      if (!this.m_hasThermalClip && (double) this.m_thermalClipLoadTriggerEnableTick < 0.0 && this.BoltHandle.HandleRot == LAPD2019BoltHandle.BoltActionHandleRot.Up)
        flag2 = true;
      if (flag2)
      {
        if (this.ThermalClipLoadTrigger.activeSelf)
          return;
        this.ThermalClipLoadTrigger.SetActive(true);
      }
      else
      {
        if (!this.ThermalClipLoadTrigger.activeSelf)
          return;
        this.ThermalClipLoadTrigger.SetActive(false);
      }
    }

    public bool LoadThermalClip(float heat)
    {
      if (this.m_hasThermalClip)
        return false;
      this.m_hasThermalClip = true;
      this.ThermalClipProxy.SetActive(true);
      this.m_heatThermalClip = heat;
      this.m_pool_handling.PlayClip(this.ThermalClip_In, this.transform.position);
      return true;
    }

    public void EjectThermalClip()
    {
      if (!this.m_hasThermalClip)
        return;
      this.m_hasThermalClip = false;
      this.ThermalClipProxy.SetActive(false);
      GameObject gameObject = Object.Instantiate<GameObject>(this.ThermalClipPrefab.GetGameObject(), this.ThermalClipEjectPos.position, this.ThermalClipEjectPos.rotation);
      gameObject.GetComponent<Rigidbody>().velocity = this.ThermalClipEjectPos.up * 2.5f;
      gameObject.GetComponent<LAPD2019ThermalClip>().SetHeat(this.m_heatThermalClip);
      this.m_pool_handling.PlayClip(this.ThermalClip_Eject, this.transform.position);
      this.m_heatThermalClip = 0.0f;
      this.m_thermalClipLoadTriggerEnableTick = 0.5f;
    }

    private void ChargeCapacitor()
    {
      if (!this.m_hasBattery || (double) this.m_batteryCharge <= 0.0 || (this.m_isCapacitorCharging || this.m_isCapacitorCharged))
        return;
      this.m_isCapacitorCharging = true;
      this.m_pool_handling.PlayClip(this.CapacitorCharge, this.transform.position);
    }

    private void UpdateElectricalSystem()
    {
      if (!this.m_hasBattery)
      {
        this.m_batteryCharge = 0.0f;
        this.m_isCapacitorCharging = false;
      }
      if (this.m_hasBattery)
        this.LED_FauxBattery_Side.material.SetColor("_Color", Color.Lerp(this.Color_Emissive_Red, this.Color_Emissive_Green, this.m_batteryCharge));
      this.LED_Rear.material.SetColor("_Color", Color.Lerp(this.Color_Emissive_Red, this.Color_Emissive_Green, this.m_capacitorCharge));
      if (!this.m_isCapacitorCharging || !this.m_hasBattery)
        return;
      if ((double) this.m_batteryCharge > 0.0)
      {
        if ((double) this.m_capacitorCharge < 1.0)
        {
          float batteryCharge = this.m_batteryCharge;
          this.m_batteryCharge -= Time.deltaTime * 0.05f * (1f + this.m_heatSystem);
          this.m_capacitorCharge += Time.deltaTime * 0.75f;
          if ((double) batteryCharge <= 0.200000002980232 || (double) this.m_batteryCharge > 0.200000002980232)
            return;
          this.m_pool_handling.PlayClip(this.Chirp_LowBattery, this.transform.position);
        }
        else
        {
          this.m_capacitorCharge = 1f;
          this.m_isCapacitorCharging = false;
          this.m_isCapacitorCharged = true;
          this.m_pool_handling.PlayClip(this.Chirp_FullyCharged, this.transform.position);
        }
      }
      else
      {
        this.m_isCapacitorCharging = false;
        this.m_batteryCharge = 0.0f;
        this.m_pool_handling.PlayClip(this.Chirp_DeadBattery, this.transform.position);
      }
    }

    public bool LoadBattery(LAPD2019Battery battery)
    {
      if (this.m_hasBattery)
        return false;
      this.FauxBattery.SetActive(true);
      this.BatteryWellTrigger.gameObject.SetActive(false);
      this.m_hasBattery = true;
      this.m_batteryCharge = battery.GetEnergy();
      this.PlayAudioEvent(FirearmAudioEventType.MagazineIn);
      if ((double) this.m_batteryCharge > 0.200000002980232)
        this.m_pool_handling.PlayClip(this.Chirp_FreshBattery, this.transform.position);
      else
        this.m_pool_handling.PlayClip(this.Chirp_LowBattery, this.transform.position);
      return true;
    }

    public float ExtractBattery(FVRViveHand hand)
    {
      if (!this.m_hasBattery)
        return -1f;
      this.FauxBattery.SetActive(false);
      this.m_hasBattery = false;
      float batteryCharge = this.m_batteryCharge;
      this.m_batteryCharge = 0.0f;
      this.PlayAudioEvent(FirearmAudioEventType.MagazineOut);
      this.Invoke("EnableWellTrigger", 1f);
      return batteryCharge;
    }

    private void EnableWellTrigger() => this.BatteryWellTrigger.gameObject.SetActive(true);

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      if (!this.IsAltHeld)
      {
        if (this.m_isInMainpose)
        {
          this.PoseOverride.localPosition = this.Pose_Main.localPosition;
          this.PoseOverride.localRotation = this.Pose_Main.localRotation;
          this.m_grabPointTransform.localPosition = this.Pose_Main.localPosition;
          this.m_grabPointTransform.localRotation = this.Pose_Main.localRotation;
        }
        else
        {
          this.PoseOverride.localPosition = this.Pose_Reloading.localPosition;
          this.PoseOverride.localRotation = this.Pose_Reloading.localRotation;
          this.m_grabPointTransform.localPosition = this.Pose_Reloading.localPosition;
          this.m_grabPointTransform.localRotation = this.Pose_Reloading.localRotation;
        }
      }
      this.LaserSystem.TurnOn();
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.IsAltHeld && !this.m_isInMainpose)
      {
        this.m_isInMainpose = true;
        this.PoseOverride.localPosition = this.Pose_Main.localPosition;
        this.PoseOverride.localRotation = this.Pose_Main.localRotation;
        this.m_grabPointTransform.localPosition = this.Pose_Main.localPosition;
        this.m_grabPointTransform.localRotation = this.Pose_Main.localRotation;
      }
      this.m_isSpinning = false;
      if (this.m_hand.IsInStreamlinedMode)
      {
        if (hand.Input.AXButtonDown)
          this.ToggleChargeMode();
      }
      else
      {
        if (hand.Input.TouchpadPressed && (double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) < 45.0)
          this.m_isSpinning = true;
        if (hand.Input.TouchpadDown)
        {
          if ((double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.right) < 45.0 && this.UsesAltPoseSwitch)
          {
            this.m_isInMainpose = !this.m_isInMainpose;
            if (this.m_isInMainpose)
            {
              this.PoseOverride.localPosition = this.Pose_Main.localPosition;
              this.PoseOverride.localRotation = this.Pose_Main.localRotation;
              this.m_grabPointTransform.localPosition = this.Pose_Main.localPosition;
              this.m_grabPointTransform.localRotation = this.Pose_Main.localRotation;
            }
            else
            {
              this.PoseOverride.localPosition = this.Pose_Reloading.localPosition;
              this.PoseOverride.localRotation = this.Pose_Reloading.localRotation;
              this.m_grabPointTransform.localPosition = this.Pose_Reloading.localPosition;
              this.m_grabPointTransform.localRotation = this.Pose_Reloading.localRotation;
            }
          }
          else if ((double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) < 45.0)
            this.ToggleChargeMode();
        }
      }
      this.UpdateTriggerHammer();
      this.UpdateCylinderRelease();
      if (this.IsHeld && !this.IsAltHeld && !((Object) this.AltGrip != (Object) null))
        return;
      this.m_isSpinning = false;
    }

    private void ToggleChargeMode()
    {
      this.m_isAutoChargeEnabled = !this.m_isAutoChargeEnabled;
      if (!this.m_isAutoChargeEnabled || (double) this.m_batteryCharge > 0.0)
        return;
      this.m_pool_handling.PlayClip(this.Chirp_DeadBattery, this.transform.position);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.m_triggerFloat = 0.0f;
      base.EndInteraction(hand);
      this.LaserSystem.TurnOff();
      this.RootRigidbody.AddRelativeTorque(new Vector3(this.xSpinVel, 0.0f, 0.0f), ForceMode.Impulse);
    }

    protected override void FVRFixedUpdate()
    {
      this.UpdateSpinning();
      if (this.m_isAutoChargeEnabled && this.m_hasBattery)
        this.ChargeCapacitor();
      base.FVRFixedUpdate();
    }

    public void EjectChambers()
    {
      bool flag = false;
      for (int index = 0; index < this.Chambers.Length; ++index)
      {
        if (this.Chambers[index].IsFull)
        {
          flag = true;
          this.Chambers[index].EjectRound(this.Chambers[index].transform.position + -this.Chambers[index].transform.forward * this.Chambers[index].GetRound().PalmingDimensions.z * 0.5f, -this.Chambers[index].transform.forward, Vector3.zero);
        }
      }
      if (!flag)
        return;
      this.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
    }

    private void UpdateSpinning()
    {
      if (!this.IsHeld || this.IsAltHeld || (Object) this.AltGrip != (Object) null)
        this.m_isSpinning = false;
      if (this.m_isSpinning)
      {
        Vector3 vector3 = Vector3.zero;
        if ((Object) this.m_hand != (Object) null)
          vector3 = this.m_hand.Input.VelLinearLocal;
        float f = Mathf.Clamp(Vector3.Dot(vector3.normalized, this.transform.up), -vector3.magnitude, vector3.magnitude);
        if ((double) Mathf.Abs(this.xSpinVel) < 90.0)
          this.xSpinVel += (float) ((double) f * (double) Time.deltaTime * 600.0);
        else if ((double) Mathf.Sign(f) == (double) Mathf.Sign(this.xSpinVel))
          this.xSpinVel += (float) ((double) f * (double) Time.deltaTime * 600.0);
        if ((double) Mathf.Abs(this.xSpinVel) < 90.0)
        {
          if ((double) Vector3.Dot(this.transform.up, Vector3.down) >= 0.0 && (double) Mathf.Sign(this.xSpinVel) == 1.0)
            this.xSpinVel += Time.deltaTime * 50f;
          if ((double) Vector3.Dot(this.transform.up, Vector3.down) < 0.0 && (double) Mathf.Sign(this.xSpinVel) == -1.0)
            this.xSpinVel -= Time.deltaTime * 50f;
        }
        this.xSpinVel = Mathf.Clamp(this.xSpinVel, -500f, 500f);
        this.xSpinRot += (float) ((double) this.xSpinVel * (double) Time.deltaTime * 5.0);
        this.PoseSpinHolder.localEulerAngles = new Vector3(this.xSpinRot, 0.0f, 0.0f);
        this.xSpinVel = Mathf.Lerp(this.xSpinVel, 0.0f, Time.deltaTime * 0.6f);
      }
      else
      {
        this.xSpinRot = 0.0f;
        this.xSpinVel = 0.0f;
        this.PoseSpinHolder.localRotation = Quaternion.RotateTowards(this.PoseSpinHolder.localRotation, Quaternion.identity, Time.deltaTime * 500f);
        this.PoseSpinHolder.localEulerAngles = new Vector3(this.PoseSpinHolder.localEulerAngles.x, 0.0f, 0.0f);
      }
    }

    private void UpdateTriggerHammer()
    {
      this.m_triggerFloat = 0.0f;
      if (this.m_hasTriggeredUpSinceBegin && !this.m_isSpinning && !this.IsAltHeld)
        this.m_triggerFloat = this.m_hand.Input.TriggerFloat;
      this.m_mechanicalCycleLerp = Mathf.Clamp(this.m_triggerFloat / this.TriggerFireThreshold, 0.0f, 1f);
      if (this.m_isCapacitorCharged)
      {
        this.m_triggerCurrentRot1 = 0.0f;
        this.m_triggerCurrentRot2 = Mathf.Lerp(this.m_triggerForwardRot, this.m_triggerBackwardRot, this.m_triggerFloat / this.TriggerFireThreshold);
      }
      else
      {
        this.m_triggerCurrentRot1 = Mathf.Lerp(this.m_triggerForwardRot, this.m_triggerBackwardRot, this.m_triggerFloat / this.TriggerFireThreshold);
        this.m_triggerCurrentRot2 = 0.0f;
      }
      this.Trigger1.localEulerAngles = new Vector3(this.m_triggerCurrentRot1, 0.0f, 0.0f);
      this.Trigger2.localEulerAngles = new Vector3(this.m_triggerCurrentRot2, 0.0f, 0.0f);
      if (!this.m_hasTriggerCycled)
      {
        if ((double) this.m_mechanicalCycleLerp >= 1.0)
        {
          if (this.m_isCylinderArmLocked)
          {
            this.m_hasTriggerCycled = true;
            ++this.CurChamber;
            this.m_curChamberLerp = 0.0f;
            this.m_tarChamberLerp = 0.0f;
            this.PlayAudioEvent(FirearmAudioEventType.Prefire);
            this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
            if (this.Chambers[this.CurChamber].IsFull && !this.Chambers[this.CurChamber].IsSpent)
            {
              this.Chambers[this.CurChamber].Fire();
              this.Fire();
              if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
              {
                this.Chambers[this.CurChamber].IsSpent = false;
                this.Chambers[this.CurChamber].UpdateProxyDisplay();
              }
            }
          }
          else
            this.m_hasTriggerCycled = true;
        }
      }
      else if (this.m_hasTriggerCycled && (double) this.m_triggerFloat <= (double) this.TriggerResetThreshold)
      {
        this.m_hasTriggerCycled = false;
        this.PlayAudioEvent(FirearmAudioEventType.TriggerReset);
      }
      this.m_hammerCurrentRot = !this.m_hasTriggerCycled ? Mathf.Lerp(this.m_hammerForwardRot, this.m_hammerBackwardRot, this.m_mechanicalCycleLerp) : Mathf.Lerp(this.m_hammerCurrentRot, this.m_hammerForwardRot, Time.deltaTime * 30f);
      this.Hammer.localEulerAngles = new Vector3(this.m_hammerCurrentRot, 0.0f, 0.0f);
    }

    private void Fire()
    {
      FVRFireArmChamber chamber = this.Chambers[this.CurChamber];
      bool twoHandStabilized = this.IsTwoHandStabilized();
      bool foregripStabilized = this.IsForegripStabilized();
      bool shoulderStabilized = this.IsShoulderStabilized();
      bool flag = false;
      FVRFireArmRound round = chamber.GetRound();
      if (this.m_isCapacitorCharged)
      {
        flag = true;
        this.m_isCapacitorCharged = false;
        this.m_capacitorCharge = 0.0f;
        float num1 = Mathf.Clamp((float) (4.19999980926514 * (1.0 - (double) this.m_barrelHeatDamage * 0.800000011920929)), 1f, 4.2f);
        for (int index1 = 0; index1 < round.NumProjectiles; ++index1)
        {
          float max = round.ProjectileSpread + this.m_barrelHeatDamage * 1f;
          if ((Object) round.ProjectilePrefab != (Object) null)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(chamber.GetRound().ProjectilePrefab, this.GetMuzzle().position, this.GetMuzzle().rotation);
            gameObject.transform.Rotate(new Vector3(Random.Range(-max, max), Random.Range(-max, max), 0.0f));
            BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
            if (round.RoundClass == FireArmRoundClass.DSM_Mine)
            {
              float num2 = Random.Range(0.0f, 2f);
              float heatSystem = this.m_heatSystem;
              if ((double) num2 <= (double) this.m_heatSystem)
              {
                Object.Destroy((Object) gameObject);
                int index2 = 0;
                for (; index1 < this.ProxMalfunctionPrefab.Length; ++index1)
                  Object.Instantiate<GameObject>(this.ProxMalfunctionPrefab[index2], this.MuzzlePos.position + this.MuzzlePos.forward, Quaternion.identity);
              }
            }
            component.Fire(component.MuzzleVelocityBase * num1, gameObject.transform.forward, (FVRFireArm) this);
          }
        }
        this.m_barrelHeatDamage += this.m_heatSystem * 0.1f;
        this.m_barrelHeatDamage = Mathf.Clamp(this.m_barrelHeatDamage, 0.0f, 1f);
        this.m_heatSystem += 0.2f;
        this.PSystem_SparksShot2.Emit(50);
        this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
        this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
      }
      else
      {
        for (int index = 0; index < round.NumProjectiles; ++index)
        {
          float max = round.ProjectileSpread + this.m_barrelHeatDamage * 1f;
          if ((Object) round.ProjectilePrefab != (Object) null)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(round.ProjectilePrefab, this.GetMuzzle().position, this.GetMuzzle().rotation);
            gameObject.transform.Rotate(new Vector3(Random.Range(-max, max), Random.Range(-max, max), 0.0f));
            gameObject.GetComponent<BallisticProjectile>().Fire(gameObject.transform.forward, (FVRFireArm) this);
          }
        }
        this.m_barrelHeatDamage += this.m_heatSystem * 0.04f;
        this.m_barrelHeatDamage = Mathf.Clamp(this.m_barrelHeatDamage, 0.0f, 1f);
        this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
      }
      int count = (int) Mathf.Lerp(0.0f, 60f, Mathf.Pow(this.m_heatSystem, 2.5f));
      if (count > 0)
        this.PSystem_SparksShot1.Emit(count);
      this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
      Vector3 position = this.transform.position;
      if (flag)
        this.PlayAudioEvent(FirearmAudioEventType.Shots_Suppressed);
      else
        this.PlayAudioEvent(FirearmAudioEventType.Shots_Main);
      this.m_pool_tail.PlayClip(SM.GetTailSet(round.TailClass, GM.CurrentPlayerBody.GetCurrentSoundEnvironment()), position);
      this.FireMuzzleSmoke();
    }

    public void AddCylinderCloseVel(float f) => this.m_CylCloseVel = f;

    private void UpdateCylinderRelease()
    {
      this.m_isCylinderReleasePressed = false;
      if (this.m_hand.IsInStreamlinedMode)
      {
        if (this.m_hand.Input.BYButtonPressed)
          this.m_isCylinderReleasePressed = true;
      }
      else if (this.m_hand.Input.TouchpadPressed && (double) Vector2.Angle(this.m_hand.Input.TouchpadAxes, Vector2.left) < 45.0)
        this.m_isCylinderReleasePressed = true;
      if (this.m_isCylinderReleasePressed)
      {
        if ((double) this.m_cylinderReleaseButtonLerp < 1.0)
          this.m_cylinderReleaseButtonLerp += Time.deltaTime * 5f;
        else
          this.m_cylinderReleaseButtonLerp = 1f;
      }
      else if ((double) this.m_cylinderReleaseButtonLerp > 0.0)
        this.m_cylinderReleaseButtonLerp -= Time.deltaTime * 5f;
      else
        this.m_cylinderReleaseButtonLerp = 0.0f;
      this.CylinderReleaseButton.localPosition = Vector3.Lerp(this.CylinderReleaseButtonRearPos.localPosition, this.CylinderReleaseButtonForwardPos.localPosition, this.m_cylinderReleaseButtonLerp);
      if (this.m_isCylinderReleasePressed)
        this.m_isCylinderArmLocked = false;
      else if ((double) Mathf.Abs(this.CylinderArm.localEulerAngles.z) <= 1.0 && !this.m_isCylinderArmLocked)
      {
        this.m_isCylinderArmLocked = true;
        this.CylinderArm.localEulerAngles = Vector3.zero;
      }
      float num1 = 160f;
      if (!this.GravityRotsCylinderPositive)
        num1 *= -1f;
      if (!this.m_isCylinderArmLocked)
      {
        float z1 = this.transform.InverseTransformDirection(this.m_hand.Input.VelAngularWorld).z;
        float x = this.transform.InverseTransformDirection(this.m_hand.Input.VelLinearWorld).x;
        float num2 = num1 + z1 * 70f + x * -250f + this.m_CylCloseVel;
        this.m_CylCloseVel = 0.0f;
        float z2 = Mathf.Clamp(this.CylinderArmRot + num2 * Time.deltaTime, this.CylinderRotRange.x, this.CylinderRotRange.y);
        if ((double) z2 != (double) this.CylinderArmRot)
        {
          this.CylinderArmRot = z2;
          this.CylinderArm.localEulerAngles = new Vector3(0.0f, 0.0f, z2);
        }
      }
      if ((double) Mathf.Abs(this.CylinderArm.localEulerAngles.z) > 30.0)
      {
        for (int index = 0; index < this.Chambers.Length; ++index)
          this.Chambers[index].IsAccessible = true;
      }
      else
      {
        for (int index = 0; index < this.Chambers.Length; ++index)
          this.Chambers[index].IsAccessible = false;
      }
      if ((double) Mathf.Abs(this.CylinderArmRot - this.CylinderRotRange.y) < 5.0 && (double) this.transform.InverseTransformDirection(this.m_hand.Input.VelLinearLocal).z < -2.0)
        this.EjectChambers();
      if (this.m_isCylinderArmLocked && !this.m_wasCylinderArmLocked)
      {
        this.m_curChamber = this.Cylinder.GetClosestChamberIndex();
        this.Cylinder.transform.localRotation = this.Cylinder.GetLocalRotationFromCylinder(this.m_curChamber);
        this.m_curChamberLerp = 0.0f;
        this.m_tarChamberLerp = 0.0f;
        this.PlayAudioEvent(FirearmAudioEventType.BreachClose);
      }
      if (!this.m_isCylinderArmLocked && this.m_wasCylinderArmLocked)
        this.PlayAudioEvent(FirearmAudioEventType.BreachOpen);
      if (!this.m_hasTriggerCycled)
        this.m_tarChamberLerp = this.m_mechanicalCycleLerp;
      this.m_curChamberLerp = Mathf.Lerp(this.m_curChamberLerp, this.m_tarChamberLerp, Time.deltaTime * 16f);
      int cylinder = (this.CurChamber + 1) % this.Cylinder.numChambers;
      if (this.isCylinderArmLocked)
        this.Cylinder.transform.localRotation = Quaternion.Slerp(this.Cylinder.GetLocalRotationFromCylinder(this.CurChamber), this.Cylinder.GetLocalRotationFromCylinder(cylinder), this.m_curChamberLerp);
      this.m_wasCylinderArmLocked = this.m_isCylinderArmLocked;
    }
  }
}
