// Decompiled with JetBrains decompiler
// Type: FistVR.Amplifier
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Amplifier : FVRFireArmAttachmentInterface
  {
    public ScopeCam ScopeCam;
    public bool DoesFlip;
    public float FlipUp;
    public float FlipDown;
    private float m_curFlip;
    private bool m_flippedUp = true;
    public Transform FlipPart;
    [Header("Zoom Shit")]
    public int m_zoomSettingIndex;
    public List<Amplifier.ZoomSetting> ZoomSettings;
    public bool UsesZoomPiece;
    public Transform ZoomPiece;
    [Header("Zero Shit")]
    public int ZeroDistanceIndex;
    public List<float> ZeroDistances = new List<float>()
    {
      100f,
      150f,
      200f,
      250f,
      300f,
      350f,
      400f,
      450f,
      500f,
      600f,
      700f,
      800f,
      900f,
      1000f
    };
    public int ElevationStep;
    public int WindageStep;
    public Transform TargetAimer;
    public Transform BackupMuzzle;
    public FVRFireArm BackupFireArm;
    public Transform UISpawnPoint;
    public OpticUI UI;
    public List<OpticOptionType> OptionTypes;
    public int CurSelectedOptionIndex;
    private bool isOnGrab = true;
    private bool isUIActive;

    protected override void Awake()
    {
      base.Awake();
      this.UpdateScopeCam();
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ManagerSingleton<AM>.Instance.Prefab_OpticUI, this.UISpawnPoint.position, this.UISpawnPoint.rotation);
      this.UI = gameObject.GetComponent<OpticUI>();
      this.UI.UpdateUI(this);
      this.UI.SetAmp(this);
      gameObject.SetActive(false);
      this.UXGeo_Held = gameObject;
    }

    protected override void Start()
    {
      base.Start();
      this.Zero();
      this.UpdateScopeCam();
    }

    private void CheckOnGrab()
    {
      if (GM.Options.ControlOptions.CCM == ControlOptions.CoreControlMode.Standard)
      {
        if (this.isOnGrab)
          return;
        this.isOnGrab = true;
        this.UXGeo_Held = this.UI.gameObject;
        this.isUIActive = false;
      }
      else
      {
        if (!this.isOnGrab)
          return;
        this.isOnGrab = false;
        this.UXGeo_Held = (GameObject) null;
      }
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.CheckOnGrab();
      if (this.isUIActive || this.IsHeld)
      {
        this.UI.transform.position = this.UISpawnPoint.position;
        this.UI.transform.rotation = this.UISpawnPoint.rotation;
      }
      this.ScopeCam.MagnificationEnabled = !((UnityEngine.Object) this.Attachment != (UnityEngine.Object) null) ? this.BackupFireArm.IsHeld || this.BackupFireArm.IsPivotLocked || (UnityEngine.Object) this.BackupFireArm.Bipod != (UnityEngine.Object) null && this.BackupFireArm.Bipod.IsBipodActive : (UnityEngine.Object) this.Attachment.curMount != (UnityEngine.Object) null && (UnityEngine.Object) this.Attachment.curMount.Parent != (UnityEngine.Object) null && (this.Attachment.curMount.Parent.IsHeld || this.Attachment.curMount.Parent.IsPivotLocked || (UnityEngine.Object) this.Attachment.curMount.Parent.Bipod != (UnityEngine.Object) null && this.Attachment.curMount.Parent.Bipod.IsBipodActive);
      if (!this.DoesFlip)
        return;
      this.m_curFlip = !this.m_flippedUp ? Mathf.Lerp(this.m_curFlip, this.FlipDown, Time.deltaTime * 4f) : Mathf.Lerp(this.m_curFlip, this.FlipUp, Time.deltaTime * 4f);
      this.FlipPart.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_curFlip);
    }

    private void UpdateScopeCam()
    {
      this.ScopeCam.Magnification = this.ZoomSettings[this.m_zoomSettingIndex].Magnification;
      this.ScopeCam.AngleBlurStrength = this.ZoomSettings[this.m_zoomSettingIndex].AngleBlueStrength;
      this.ScopeCam.CutoffSoftness = this.ZoomSettings[this.m_zoomSettingIndex].CutoffSoftness;
      this.ScopeCam.AngularOccludeSensitivity = this.ZoomSettings[this.m_zoomSettingIndex].AngularOccludeSensitivity;
      if (!this.UsesZoomPiece)
        return;
      this.ZoomPiece.localEulerAngles = this.ZoomSettings[this.m_zoomSettingIndex].ZoomPiecePosRot;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
        {
          this.isUIActive = !this.isUIActive;
          this.UI.gameObject.SetActive(this.isUIActive);
        }
      }
      else if (hand.Input.TouchpadDown && (double) touchpadAxes.magnitude > 0.25)
      {
        if ((double) Vector2.Angle(touchpadAxes, Vector2.left) <= 45.0)
        {
          this.SetCurSettingDown();
          this.UI.UpdateUI(this);
        }
        else if ((double) Vector2.Angle(touchpadAxes, Vector2.right) <= 45.0)
        {
          this.SetCurSettingUp(false);
          this.UI.UpdateUI(this);
        }
        else if ((double) Vector2.Angle(touchpadAxes, Vector2.up) <= 45.0)
        {
          this.GoToNextSetting();
          this.UI.UpdateUI(this);
        }
      }
      base.UpdateInteraction(hand);
    }

    public void SetCurSettingUp(bool cycle)
    {
      switch (this.OptionTypes[this.CurSelectedOptionIndex])
      {
        case OpticOptionType.Zero:
          ++this.ZeroDistanceIndex;
          if (cycle)
          {
            if (this.ZeroDistanceIndex >= this.ZeroDistances.Count)
              this.ZeroDistanceIndex = 0;
          }
          else
            this.ZeroDistanceIndex = Mathf.Clamp(this.ZeroDistanceIndex, 0, this.ZeroDistances.Count - 1);
          this.Zero();
          break;
        case OpticOptionType.Magnification:
          ++this.m_zoomSettingIndex;
          if (cycle)
          {
            if (this.m_zoomSettingIndex >= this.ZoomSettings.Count)
              this.m_zoomSettingIndex = 0;
          }
          else
            this.m_zoomSettingIndex = Mathf.Clamp(this.m_zoomSettingIndex, 0, this.ZoomSettings.Count - 1);
          this.UpdateScopeCam();
          break;
        case OpticOptionType.ElevationTweak:
          if (cycle)
            break;
          ++this.ElevationStep;
          this.Zero();
          break;
        case OpticOptionType.WindageTweak:
          if (cycle)
            break;
          ++this.WindageStep;
          this.Zero();
          break;
      }
    }

    public void SetCurSettingDown()
    {
      switch (this.OptionTypes[this.CurSelectedOptionIndex])
      {
        case OpticOptionType.Zero:
          --this.ZeroDistanceIndex;
          this.ZeroDistanceIndex = Mathf.Clamp(this.ZeroDistanceIndex, 0, this.ZeroDistances.Count - 1);
          this.Zero();
          break;
        case OpticOptionType.Magnification:
          --this.m_zoomSettingIndex;
          this.m_zoomSettingIndex = Mathf.Clamp(this.m_zoomSettingIndex, 0, this.ZoomSettings.Count - 1);
          this.UpdateScopeCam();
          break;
        case OpticOptionType.ElevationTweak:
          --this.ElevationStep;
          this.Zero();
          break;
        case OpticOptionType.WindageTweak:
          --this.WindageStep;
          this.Zero();
          break;
      }
    }

    private void GoToNextSetting()
    {
      if (this.OptionTypes.Count < 2)
        return;
      ++this.CurSelectedOptionIndex;
      if (this.CurSelectedOptionIndex < this.OptionTypes.Count)
        return;
      this.CurSelectedOptionIndex = 0;
    }

    public void GoToSetting(int i) => this.CurSelectedOptionIndex = i;

    public override void OnAttach()
    {
      base.OnAttach();
      this.ScopeCam.MagnificationEnabled = true;
      this.Zero();
    }

    public override void OnDetach()
    {
      base.OnDetach();
      this.ScopeCam.MagnificationEnabled = false;
    }

    [ContextMenu("PopulateDefaultZoom")]
    public void PopulateDefaultZoom()
    {
      if (this.ZoomSettings.Count != 0)
        return;
      this.ZoomSettings.Add(new Amplifier.ZoomSetting()
      {
        Magnification = this.ScopeCam.Magnification,
        AngleBlueStrength = this.ScopeCam.AngleBlurStrength,
        CutoffSoftness = this.ScopeCam.CutoffSoftness,
        AngularOccludeSensitivity = this.ScopeCam.AngularOccludeSensitivity
      });
    }

    public void Zero()
    {
      if (!((UnityEngine.Object) this.TargetAimer != (UnityEngine.Object) null))
        return;
      if ((UnityEngine.Object) this.Attachment != (UnityEngine.Object) null && (UnityEngine.Object) this.Attachment.curMount != (UnityEngine.Object) null && ((UnityEngine.Object) this.Attachment.curMount.Parent != (UnityEngine.Object) null && this.Attachment.curMount.Parent is FVRFireArm))
      {
        FVRFireArm parent = this.Attachment.curMount.Parent as FVRFireArm;
        FireArmRoundType roundType = parent.RoundType;
        float zeroDistance = this.ZeroDistances[this.ZeroDistanceIndex];
        float num = 0.0f;
        if (AM.SRoundDisplayDataDic.ContainsKey(roundType))
          num = AM.SRoundDisplayDataDic[roundType].BulletDropCurve.Evaluate(zeroDistance * (1f / 1000f));
        Vector3 p = parent.MuzzlePos.position + parent.GetMuzzle().forward * zeroDistance + parent.GetMuzzle().up * num;
        Vector3 vector3_1 = Vector3.ProjectOnPlane(p - this.TargetAimer.transform.position, this.transform.right);
        Vector3 vector3_2 = Quaternion.AngleAxis(0.004166675f * (float) this.ElevationStep, this.transform.right) * vector3_1;
        Vector3 forward = Quaternion.AngleAxis(0.004166675f * (float) this.WindageStep, this.transform.up) * vector3_2;
        this.TargetAimer.rotation = Quaternion.LookRotation(forward, this.transform.up);
        this.ScopeCam.PointTowards(p);
        this.ScopeCam.ScopeCamera.transform.rotation = Quaternion.LookRotation(forward, this.transform.up);
      }
      else if ((UnityEngine.Object) this.BackupFireArm != (UnityEngine.Object) null)
      {
        FVRFireArm backupFireArm = this.BackupFireArm;
        FireArmRoundType roundType = backupFireArm.RoundType;
        float zeroDistance = this.ZeroDistances[this.ZeroDistanceIndex];
        float num = 0.0f;
        if (AM.SRoundDisplayDataDic.ContainsKey(roundType))
          num = AM.SRoundDisplayDataDic[roundType].BulletDropCurve.Evaluate(zeroDistance * (1f / 1000f));
        Vector3 p = backupFireArm.MuzzlePos.position + backupFireArm.GetMuzzle().forward * zeroDistance + backupFireArm.GetMuzzle().up * num;
        Vector3 vector3_1 = Vector3.ProjectOnPlane(p - this.TargetAimer.transform.position, this.transform.right);
        Vector3 vector3_2 = Quaternion.AngleAxis(0.004166675f * (float) this.ElevationStep, this.transform.right) * vector3_1;
        this.TargetAimer.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(0.004166675f * (float) this.WindageStep, this.transform.up) * vector3_2, this.transform.up);
        this.ScopeCam.PointTowards(p);
      }
      else if ((UnityEngine.Object) this.BackupMuzzle != (UnityEngine.Object) null)
        this.TargetAimer.LookAt(this.BackupMuzzle.position + this.BackupMuzzle.forward * this.ZeroDistances[this.ZeroDistanceIndex], Vector3.up);
      else
        this.TargetAimer.localRotation = Quaternion.identity;
    }

    [Serializable]
    public class ZoomSetting
    {
      public float Magnification;
      public float AngleBlueStrength;
      public float CutoffSoftness;
      public float AngularOccludeSensitivity;
      public Vector3 ZoomPiecePosRot;
    }
  }
}
