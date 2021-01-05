// Decompiled with JetBrains decompiler
// Type: FistVR.ControlOptions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class ControlOptions
  {
    public ControlOptions.CoreControlMode CCM;
    public bool HasConfirmedControls;
    public ControlOptions.ButtonControlStyle GripButtonDropStyle;
    public bool UseInvertedHandgunMagPose;
    public bool UseEasyMagLoading;
    public bool UseGunRigMode2;
    public bool UseVirtualStock = true;
    public ControlOptions.GripButtonToHoldOverrideMode GripButtonToHoldOverride;
    public ControlOptions.MeatFingerMode MFMode;
    public ControlOptions.MeatBody MBMode;
    public SosigEnemyID MBClothing = SosigEnemyID.None;
    public ControlOptions.DesktopCameraMode CamMode;
    public float CamFOV = 55f;
    public float CamSmoothingLinear;
    public float CamSmoothingRotational;
    public float CamLeveling;
    public ControlOptions.DesktopCameraEye CamEye;
    public ControlOptions.DesktopRenderQuality CamQual;
    public ControlOptions.PreviewCamMode PCamMode;
    public int TPCDistanceIndex = 2;
    public int TPCLateralIndex = 2;
    public ControlOptions.HapticsMode HapticsState;
    public ControlOptions.WIPGrabbity WIPGrabbityState = ControlOptions.WIPGrabbity.Disabled;
    public ControlOptions.WIPGrabbityButton WIPGrabbityButtonState;
    public ControlOptions.WristMenuMode WristMenuState;

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("CCM"))
        this.CCM = reader.Read<ControlOptions.CoreControlMode>("CCM");
      if (reader.TagExists("GripButtonDropStyle"))
        this.GripButtonDropStyle = reader.Read<ControlOptions.ButtonControlStyle>("GripButtonDropStyle");
      if (reader.TagExists("UseInvertedHandgunMagPose"))
        this.UseInvertedHandgunMagPose = reader.Read<bool>("UseInvertedHandgunMagPose");
      if (reader.TagExists("UseEasyMagLoading"))
        this.UseEasyMagLoading = reader.Read<bool>("UseEasyMagLoading");
      if (reader.TagExists("UseGunRigMode2"))
        this.UseGunRigMode2 = reader.Read<bool>("UseGunRigMode2");
      if (reader.TagExists("UseVirtualStock"))
        this.UseVirtualStock = reader.Read<bool>("UseVirtualStock");
      if (reader.TagExists("MFMode"))
        this.MFMode = reader.Read<ControlOptions.MeatFingerMode>("MFMode");
      if (reader.TagExists("MBMode"))
        this.MBMode = reader.Read<ControlOptions.MeatBody>("MBMode");
      if (reader.TagExists("MBClothing"))
        this.MBClothing = reader.Read<SosigEnemyID>("MBClothing");
      if (reader.TagExists("GripButtonToHoldOverride"))
        this.GripButtonToHoldOverride = reader.Read<ControlOptions.GripButtonToHoldOverrideMode>("GripButtonToHoldOverride");
      if (reader.TagExists("HapticsState"))
        this.HapticsState = reader.Read<ControlOptions.HapticsMode>("HapticsState");
      if (reader.TagExists("WIPGrabbityState"))
        this.WIPGrabbityState = reader.Read<ControlOptions.WIPGrabbity>("WIPGrabbityState");
      if (reader.TagExists("WIPGrabbityButtonState"))
        this.WIPGrabbityButtonState = reader.Read<ControlOptions.WIPGrabbityButton>("WIPGrabbityButtonState");
      if (!reader.TagExists("WristMenuState"))
        return;
      this.WristMenuState = reader.Read<ControlOptions.WristMenuMode>("WristMenuState");
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<ControlOptions.CoreControlMode>(this.CCM, "CCM");
      writer.Write<ControlOptions.ButtonControlStyle>(this.GripButtonDropStyle, "GripButtonDropStyle");
      writer.Write<bool>(this.UseInvertedHandgunMagPose, "UseInvertedHandgunMagPose");
      writer.Write<bool>(this.UseEasyMagLoading, "UseEasyMagLoading");
      writer.Write<bool>(this.UseGunRigMode2, "UseGunRigMode2");
      writer.Write<bool>(this.UseVirtualStock, "UseVirtualStock");
      writer.Write<ControlOptions.MeatFingerMode>(this.MFMode, "MFMode");
      writer.Write<ControlOptions.MeatBody>(this.MBMode, "MBMode");
      writer.Write<SosigEnemyID>(this.MBClothing, "MBClothing");
      writer.Write<ControlOptions.GripButtonToHoldOverrideMode>(this.GripButtonToHoldOverride, "GripButtonToHoldOverride");
      writer.Write<ControlOptions.HapticsMode>(this.HapticsState, "HapticsState");
      writer.Write<ControlOptions.WIPGrabbity>(this.WIPGrabbityState, "WIPGrabbityState");
      writer.Write<ControlOptions.WIPGrabbityButton>(this.WIPGrabbityButtonState, "WIPGrabbityButtonState");
      writer.Write<ControlOptions.WristMenuMode>(this.WristMenuState, "WristMenuState");
    }

    public enum ButtonControlStyle
    {
      Instant,
      Hold1Second,
      DoubleClick,
    }

    public enum CoreControlMode
    {
      Standard,
      Streamlined,
    }

    public enum GripButtonToHoldOverrideMode
    {
      Disabled,
      OculusOverride,
      ViveOverride,
    }

    public enum MeatFingerMode
    {
      Disabled,
      Enabled,
    }

    public enum MeatBody
    {
      Disabled,
      Enabled,
    }

    public enum DesktopCameraMode
    {
      Default,
      HDSpectator,
      ThirdPerson,
      SpawnedObject,
      SosigView,
    }

    public enum DesktopCameraEye
    {
      Right,
      Left,
    }

    public enum DesktopRenderQuality
    {
      Low,
      Med,
      High,
    }

    public enum PreviewCamMode
    {
      Disabled,
      Enabled,
    }

    public enum HapticsMode
    {
      Enabled,
      Disabled,
    }

    public enum WIPGrabbity
    {
      Enabled,
      Disabled,
    }

    public enum WIPGrabbityButton
    {
      Trigger,
      Grab,
    }

    public enum WristMenuMode
    {
      BothHands,
      RightHand,
      LeftHand,
    }
  }
}
