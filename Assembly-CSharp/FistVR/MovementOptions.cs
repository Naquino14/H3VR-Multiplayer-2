// Decompiled with JetBrains decompiler
// Type: FistVR.MovementOptions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class MovementOptions
  {
    public FVRMovementManager.MovementMode CurrentMovementMode;
    public FVRMovementManager.MovementAxialOrigin Teleport_AxialOrigin;
    public FVRMovementManager.MovementAxialOrigin Dash_AxialOrigin;
    public FVRMovementManager.MovementAxialOrigin Slide_AxialOrigin;
    public FVRMovementManager.TeleportMode Teleport_Mode;
    public FVRMovementManager.TwoAxisMovementMode Touchpad_MovementMode;
    public FVRMovementManager.TwoAxisMovementConfirm Touchpad_Confirm;
    public FVRMovementManager.TwoAxisMovementStyle Touchpad_Style;
    public MovementOptions.AXButtonSnapTurnMode AXButtonSnapTurnState = MovementOptions.AXButtonSnapTurnMode.Snapturn;
    public MovementOptions.ArmSwingerSnapTurnMode ArmSwingerSnapTurnState = MovementOptions.ArmSwingerSnapTurnMode.Enabled;
    public MovementOptions.ArmSwingerJumpMode ArmSwingerJumpState = MovementOptions.ArmSwingerJumpMode.Enabled;
    public MovementOptions.TwinStickSprintMode TwinStickSprintState;
    public MovementOptions.TwinStickSprintToggleMode TwinStickSprintToggleState;
    public MovementOptions.TwinStickSnapturnMode TwinStickSnapturnState = MovementOptions.TwinStickSnapturnMode.Enabled;
    public MovementOptions.TwinStickJumpMode TwinStickJumpState = MovementOptions.TwinStickJumpMode.Enabled;
    public MovementOptions.TwinStickLeftRightSetup TwinStickLeftRightState;
    public MovementOptions.GlobalTurningMode GlobalTurningModeState;
    public int SlidingSpeedTick = 2;
    public float[] SlidingSpeeds = new float[5]
    {
      0.4f,
      0.8f,
      1.2f,
      1.8f,
      2.4f
    };
    public int TPLocoSpeedIndex = 1;
    public float[] TPLocoSpeeds = new float[6]
    {
      0.7f,
      1.3f,
      1.8f,
      2.6f,
      4f,
      6.5f
    };
    public int SnapTurnMagnitudeIndex = 1;
    public float[] SnapTurnMagnitudes = new float[5]
    {
      90f,
      45f,
      22.5f,
      15f,
      5f
    };
    public int SmoothTurnMagnitudeIndex = 1;
    public float[] SmoothTurnMagnitudes = new float[6]
    {
      360f,
      270f,
      180f,
      150f,
      120f,
      90f
    };
    public int ArmSwingerBaseSpeed_Left;
    public int ArmSwingerBaseSpeed_Right;
    public float[] ArmSwingerBaseSpeeMagnitudes = new float[6]
    {
      0.0f,
      0.15f,
      0.25f,
      0.5f,
      0.8f,
      1.2f
    };

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("CurrentMovementMode"))
        this.CurrentMovementMode = reader.Read<FVRMovementManager.MovementMode>("CurrentMovementMode");
      if (reader.TagExists("Teleport_AxialOrigin"))
        this.Teleport_AxialOrigin = reader.Read<FVRMovementManager.MovementAxialOrigin>("Teleport_AxialOrigin");
      if (reader.TagExists("Dash_AxialOrigin"))
        this.Dash_AxialOrigin = reader.Read<FVRMovementManager.MovementAxialOrigin>("Dash_AxialOrigin");
      if (reader.TagExists("Slide_AxialOrigin"))
        this.Slide_AxialOrigin = reader.Read<FVRMovementManager.MovementAxialOrigin>("Slide_AxialOrigin");
      if (reader.TagExists("Teleport_Mode"))
        this.Teleport_Mode = reader.Read<FVRMovementManager.TeleportMode>("Teleport_Mode");
      if (reader.TagExists("Touchpad_MovementMode"))
        this.Touchpad_MovementMode = reader.Read<FVRMovementManager.TwoAxisMovementMode>("Touchpad_MovementMode");
      if (reader.TagExists("Touchpad_Confirm"))
        this.Touchpad_Confirm = reader.Read<FVRMovementManager.TwoAxisMovementConfirm>("Touchpad_Confirm");
      if (reader.TagExists("Touchpad_Style"))
        this.Touchpad_Style = reader.Read<FVRMovementManager.TwoAxisMovementStyle>("Touchpad_Style");
      if (reader.TagExists("SlidingSpeedTick"))
        this.SlidingSpeedTick = reader.Read<int>("SlidingSpeedTick");
      if (reader.TagExists("TPLocoSpeedIndex"))
        this.TPLocoSpeedIndex = reader.Read<int>("TPLocoSpeedIndex");
      if (reader.TagExists("SnapTurnMagnitudeIndex"))
        this.SnapTurnMagnitudeIndex = reader.Read<int>("SnapTurnMagnitudeIndex");
      if (reader.TagExists("AXButtonSnapTurnState"))
        this.AXButtonSnapTurnState = reader.Read<MovementOptions.AXButtonSnapTurnMode>("AXButtonSnapTurnState");
      if (reader.TagExists("ArmSwingerSnapTurnState"))
        this.ArmSwingerSnapTurnState = reader.Read<MovementOptions.ArmSwingerSnapTurnMode>("ArmSwingerSnapTurnState");
      if (reader.TagExists("TwinStickSprintState"))
        this.TwinStickSprintState = reader.Read<MovementOptions.TwinStickSprintMode>("TwinStickSprintState");
      if (reader.TagExists("ArmSwingerJumpState"))
        this.ArmSwingerJumpState = reader.Read<MovementOptions.ArmSwingerJumpMode>("ArmSwingerJumpState");
      if (reader.TagExists("ArmSwingerBaseSpeed_Left"))
        this.ArmSwingerBaseSpeed_Left = reader.Read<int>("ArmSwingerBaseSpeed_Left");
      if (reader.TagExists("ArmSwingerBaseSpeed_Right"))
        this.ArmSwingerBaseSpeed_Right = reader.Read<int>("ArmSwingerBaseSpeed_Right");
      if (reader.TagExists("TwinStickSnapturnState"))
        this.TwinStickSnapturnState = reader.Read<MovementOptions.TwinStickSnapturnMode>("TwinStickSnapturnState");
      if (reader.TagExists("TwinStickSprintToggleState"))
        this.TwinStickSprintToggleState = reader.Read<MovementOptions.TwinStickSprintToggleMode>("TwinStickSprintToggleState");
      if (reader.TagExists("TwinStickJumpState"))
        this.TwinStickJumpState = reader.Read<MovementOptions.TwinStickJumpMode>("TwinStickJumpState");
      if (reader.TagExists("TwinStickLeftRightState"))
        this.TwinStickLeftRightState = reader.Read<MovementOptions.TwinStickLeftRightSetup>("TwinStickLeftRightState");
      if (reader.TagExists("GlobalTurningModeState"))
        this.GlobalTurningModeState = reader.Read<MovementOptions.GlobalTurningMode>("GlobalTurningModeState");
      if (!reader.TagExists("SmoothTurnMagnitudeIndex"))
        return;
      this.SmoothTurnMagnitudeIndex = reader.Read<int>("SmoothTurnMagnitudeIndex");
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<FVRMovementManager.MovementMode>(this.CurrentMovementMode, "CurrentMovementMode");
      writer.Write<FVRMovementManager.MovementAxialOrigin>(this.Teleport_AxialOrigin, "Teleport_AxialOrigin");
      writer.Write<FVRMovementManager.MovementAxialOrigin>(this.Dash_AxialOrigin, "Dash_AxialOrigin");
      writer.Write<FVRMovementManager.MovementAxialOrigin>(this.Slide_AxialOrigin, "Slide_AxialOrigin");
      writer.Write<FVRMovementManager.TeleportMode>(this.Teleport_Mode, "Teleport_Mode");
      writer.Write<FVRMovementManager.TwoAxisMovementMode>(this.Touchpad_MovementMode, "Touchpad_MovementMode");
      writer.Write<FVRMovementManager.TwoAxisMovementConfirm>(this.Touchpad_Confirm, "Touchpad_Confirm");
      writer.Write<FVRMovementManager.TwoAxisMovementStyle>(this.Touchpad_Style, "Touchpad_Style");
      writer.Write<int>(this.SlidingSpeedTick, "SlidingSpeedTick");
      writer.Write<int>(this.TPLocoSpeedIndex, "TPLocoSpeedIndex");
      writer.Write<int>(this.SnapTurnMagnitudeIndex, "SnapTurnMagnitudeIndex");
      writer.Write<MovementOptions.ArmSwingerJumpMode>(this.ArmSwingerJumpState, "ArmSwingerJumpState");
      writer.Write<int>(this.ArmSwingerBaseSpeed_Left, "ArmSwingerBaseSpeed_Left");
      writer.Write<int>(this.ArmSwingerBaseSpeed_Right, "ArmSwingerBaseSpeed_Right");
      writer.Write<MovementOptions.AXButtonSnapTurnMode>(this.AXButtonSnapTurnState, "AXButtonSnapTurnState");
      writer.Write<MovementOptions.ArmSwingerSnapTurnMode>(this.ArmSwingerSnapTurnState, "ArmSwingerSnapTurnState");
      writer.Write<MovementOptions.TwinStickSprintMode>(this.TwinStickSprintState, "TwinStickSprintState");
      writer.Write<MovementOptions.TwinStickSnapturnMode>(this.TwinStickSnapturnState, "TwinStickSnapturnState");
      writer.Write<MovementOptions.TwinStickSprintToggleMode>(this.TwinStickSprintToggleState, "TwinStickSprintToggleState");
      writer.Write<MovementOptions.TwinStickJumpMode>(this.TwinStickJumpState, "TwinStickJumpState");
      writer.Write<MovementOptions.TwinStickLeftRightSetup>(this.TwinStickLeftRightState, "TwinStickLeftRightState");
      writer.Write<MovementOptions.GlobalTurningMode>(this.GlobalTurningModeState, "GlobalTurningModeState");
      writer.Write<int>(this.SmoothTurnMagnitudeIndex, "SmoothTurnMagnitudeIndex");
    }

    public enum AXButtonSnapTurnMode
    {
      Disabled,
      Snapturn,
      Jump,
      Smoothturn,
    }

    public enum ArmSwingerSnapTurnMode
    {
      Disabled,
      Enabled,
      Smooth,
    }

    public enum ArmSwingerJumpMode
    {
      Disabled,
      Enabled,
    }

    public enum TwinStickSprintMode
    {
      LeftStickClick,
      RightStickForward,
    }

    public enum TwinStickSprintToggleMode
    {
      Disabled,
      Enabled,
    }

    public enum TwinStickSnapturnMode
    {
      Disabled,
      Enabled,
      Smooth,
    }

    public enum TwinStickJumpMode
    {
      Disabled,
      Enabled,
    }

    public enum TwinStickLeftRightSetup
    {
      LeftStickMove,
      RightStickMove,
    }

    public enum GlobalTurningMode
    {
      Snapturn,
      Smoothturn,
    }
  }
}
