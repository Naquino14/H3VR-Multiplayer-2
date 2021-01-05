// Decompiled with JetBrains decompiler
// Type: FistVR.RevolvingShotgun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class RevolvingShotgun : FVRFireArm
  {
    [Header("Revolving Shotgun Config")]
    private bool m_isHammerLocked;
    private bool m_hasTriggerCycled;
    public bool DoesFiringRecock;
    private Vector2 TouchPadAxes = Vector2.zero;
    public Speedloader.SpeedLoaderType SLType;
    [Header("Cylinder Config")]
    public bool CylinderLoaded;
    public Transform ProxyCylinder;
    public bool IsCylinderRotClockwise = true;
    public int NumChambers;
    public FVRFireArmChamber[] Chambers;
    private int m_curChamber;
    private float m_tarChamberLerp;
    private float m_curChamberLerp;
    [Header("Trigger Config")]
    public Transform Trigger;
    public bool HasTrigger;
    public float TriggerFiringThreshold = 0.8f;
    public float TriggerResetThreshold = 0.4f;
    public float Trigger_ForwardValue;
    public float Trigger_RearwardValue;
    public FVRPhysicalObject.Axis TriggerAxis;
    public FVRPhysicalObject.InterpStyle TriggerInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    private float m_triggerCurrentRot;
    [Header("Fire Selector Config")]
    public Transform FireSelectorSwitch;
    public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    public FVRPhysicalObject.Axis FireSelector_Axis;
    public RevolvingShotgun.FireSelectorMode[] FireSelector_Modes;
    private int m_fireSelectorMode;
    public GameObject CylinderPrefab;
    public Transform CyclinderMountPoint;
    private float lastTriggerRot;
    private bool m_shouldRecock;

    public int CurChamber
    {
      get => this.m_curChamber;
      set
      {
        if (value < 0)
          this.m_curChamber = this.NumChambers - 1;
        else
          this.m_curChamber = value % this.NumChambers;
      }
    }

    public int FireSelectorModeIndex => this.m_fireSelectorMode;

    protected override void Awake()
    {
      base.Awake();
      this.ProxyCylinder.gameObject.SetActive(this.CylinderLoaded);
    }

    protected override void FVRUpdate() => base.FVRUpdate();

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.TouchPadAxes = hand.Input.TouchpadAxes;
      this.UpdateTriggerHammer();
      this.UpdateCylinderRelease();
      if (this.IsAltHeld)
        return;
      if (hand.IsInStreamlinedMode)
      {
        if (!hand.Input.BYButtonDown)
          return;
        this.ToggleFireSelector();
      }
      else
      {
        if (!hand.Input.TouchpadDown || (double) this.TouchPadAxes.magnitude <= 0.200000002980232 || (double) Vector2.Angle(this.TouchPadAxes, Vector2.left) > 45.0)
          return;
        this.ToggleFireSelector();
      }
    }

    private void ToggleFireSelector()
    {
      if (this.FireSelector_Modes.Length <= 1)
        return;
      ++this.m_fireSelectorMode;
      if (this.m_fireSelectorMode >= this.FireSelector_Modes.Length)
        this.m_fireSelectorMode -= this.FireSelector_Modes.Length;
      RevolvingShotgun.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
      this.PlayAudioEvent(FirearmAudioEventType.FireSelector);
      if (!((UnityEngine.Object) this.FireSelectorSwitch != (UnityEngine.Object) null))
        return;
      this.SetAnimatedComponent(this.FireSelectorSwitch, fireSelectorMode.SelectorPosition, this.FireSelector_InterpStyle, this.FireSelector_Axis);
    }

    private void UpdateTriggerHammer()
    {
      float t = 0.0f;
      bool flag = true;
      if (this.FireSelector_Modes[this.m_fireSelectorMode].ModeType != RevolvingShotgun.FireSelectorModeType.Safe)
        flag = false;
      if (this.m_hasTriggeredUpSinceBegin && !this.IsAltHeld && !flag)
        t = this.m_hand.Input.TriggerFloat;
      if (this.m_isHammerLocked)
        t += 0.8f;
      this.m_triggerCurrentRot = Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, t);
      if ((double) Mathf.Abs(this.m_triggerCurrentRot - this.lastTriggerRot) > 9.99999974737875E-05)
        this.SetAnimatedComponent(this.Trigger, this.m_triggerCurrentRot, this.TriggerInterpStyle, this.TriggerAxis);
      this.lastTriggerRot = this.m_triggerCurrentRot;
      if (this.m_shouldRecock)
      {
        this.m_shouldRecock = false;
        this.m_isHammerLocked = true;
        this.PlayAudioEvent(FirearmAudioEventType.Prefire);
      }
      if (!this.m_hasTriggerCycled)
      {
        if ((double) t < 0.980000019073486)
          return;
        this.m_hasTriggerCycled = true;
        this.m_isHammerLocked = false;
        if (this.IsCylinderRotClockwise)
          ++this.CurChamber;
        else
          --this.CurChamber;
        this.m_curChamberLerp = 0.0f;
        this.m_tarChamberLerp = 0.0f;
        this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
        if (!this.CylinderLoaded || !this.Chambers[this.CurChamber].IsFull || this.Chambers[this.CurChamber].IsSpent)
          return;
        this.Chambers[this.CurChamber].Fire();
        this.Fire();
        if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
        {
          this.Chambers[this.CurChamber].IsSpent = false;
          this.Chambers[this.CurChamber].UpdateProxyDisplay();
        }
        if (!this.DoesFiringRecock)
          return;
        this.m_shouldRecock = true;
      }
      else
      {
        if (!this.m_hasTriggerCycled || (double) this.m_hand.Input.TriggerFloat > 0.100000001490116)
          return;
        this.m_hasTriggerCycled = false;
        this.PlayAudioEvent(FirearmAudioEventType.TriggerReset);
      }
    }

    private void UpdateCylinderRelease()
    {
      float num = 0.0f;
      bool flag = true;
      if (this.FireSelector_Modes[this.m_fireSelectorMode].ModeType != RevolvingShotgun.FireSelectorModeType.Safe)
        flag = false;
      if (this.m_hasTriggeredUpSinceBegin && !this.IsAltHeld && !flag)
        num = this.m_hand.Input.TriggerFloat;
      if (this.m_isHammerLocked)
        this.m_tarChamberLerp = 1f;
      else if (!this.m_hasTriggerCycled)
        this.m_tarChamberLerp = num * 1.4f;
      this.m_curChamberLerp = Mathf.Lerp(this.m_curChamberLerp, this.m_tarChamberLerp, Time.deltaTime * 16f);
      int cylinder = !this.IsCylinderRotClockwise ? (this.CurChamber - 1) % this.NumChambers : (this.CurChamber + 1) % this.NumChambers;
      this.ProxyCylinder.transform.localRotation = Quaternion.Slerp(this.GetLocalRotationFromCylinder(this.CurChamber), this.GetLocalRotationFromCylinder(cylinder), this.m_curChamberLerp);
    }

    private void Fire()
    {
      FVRFireArmChamber chamber = this.Chambers[this.CurChamber];
      this.Fire(chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      if (chamber.GetRound().IsHighPressure)
        this.Recoil(this.IsTwoHandStabilized(), (UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null, this.IsShoulderStabilized());
      this.PlayAudioGunShot(chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
    }

    public bool LoadCylinder(Speedloader s)
    {
      if (this.CylinderLoaded)
        return false;
      this.CylinderLoaded = true;
      this.ProxyCylinder.gameObject.SetActive(this.CylinderLoaded);
      this.PlayAudioEvent(FirearmAudioEventType.MagazineIn);
      this.m_curChamber = 0;
      this.ProxyCylinder.localRotation = this.GetLocalRotationFromCylinder(this.m_curChamber);
      for (int index = 0; index < this.Chambers.Length; ++index)
      {
        if (s.Chambers[index].IsLoaded)
        {
          this.Chambers[index].Autochamber(s.Chambers[index].LoadedClass);
          this.Chambers[index].IsSpent = s.Chambers[index].IsSpent;
        }
        else
          this.Chambers[index].Unload();
        this.Chambers[index].UpdateProxyDisplay();
      }
      return true;
    }

    public Speedloader EjectCylinder()
    {
      Speedloader component = UnityEngine.Object.Instantiate<GameObject>(this.CylinderPrefab, this.CyclinderMountPoint.position, this.CyclinderMountPoint.rotation).GetComponent<Speedloader>();
      this.PlayAudioEvent(FirearmAudioEventType.MagazineOut);
      for (int index = 0; index < component.Chambers.Count; ++index)
      {
        if (!this.Chambers[index].IsFull)
        {
          int num = (int) component.Chambers[index].Unload();
        }
        else if (this.Chambers[index].IsSpent)
          component.Chambers[index].LoadEmpty(this.Chambers[index].GetRound().RoundClass);
        else
          component.Chambers[index].Load(this.Chambers[index].GetRound().RoundClass);
        this.Chambers[index].UpdateProxyDisplay();
      }
      this.EjectDelay = 0.4f;
      this.CylinderLoaded = false;
      this.ProxyCylinder.gameObject.SetActive(this.CylinderLoaded);
      return component;
    }

    public Quaternion GetLocalRotationFromCylinder(int cylinder) => Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Repeat((float) ((double) cylinder * (360.0 / (double) this.NumChambers) * -1.0), 360f)));

    public int GetClosestChamberIndex() => Mathf.CeilToInt(Mathf.Repeat(-this.transform.localEulerAngles.z + (float) (360.0 / (double) this.NumChambers * 0.5), 360f) / (360f / (float) this.NumChambers)) - 1;

    public enum FireSelectorModeType
    {
      Safe,
      Single,
      FullAuto,
    }

    [Serializable]
    public class FireSelectorMode
    {
      public float SelectorPosition;
      public RevolvingShotgun.FireSelectorModeType ModeType;
    }
  }
}
