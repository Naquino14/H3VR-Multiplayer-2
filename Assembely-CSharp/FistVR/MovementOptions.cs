using System;

namespace FistVR
{
	[Serializable]
	public class MovementOptions
	{
		public enum AXButtonSnapTurnMode
		{
			Disabled,
			Snapturn,
			Jump,
			Smoothturn
		}

		public enum ArmSwingerSnapTurnMode
		{
			Disabled,
			Enabled,
			Smooth
		}

		public enum ArmSwingerJumpMode
		{
			Disabled,
			Enabled
		}

		public enum TwinStickSprintMode
		{
			LeftStickClick,
			RightStickForward
		}

		public enum TwinStickSprintToggleMode
		{
			Disabled,
			Enabled
		}

		public enum TwinStickSnapturnMode
		{
			Disabled,
			Enabled,
			Smooth
		}

		public enum TwinStickJumpMode
		{
			Disabled,
			Enabled
		}

		public enum TwinStickLeftRightSetup
		{
			LeftStickMove,
			RightStickMove
		}

		public enum GlobalTurningMode
		{
			Snapturn,
			Smoothturn
		}

		public FVRMovementManager.MovementMode CurrentMovementMode;

		public FVRMovementManager.MovementAxialOrigin Teleport_AxialOrigin;

		public FVRMovementManager.MovementAxialOrigin Dash_AxialOrigin;

		public FVRMovementManager.MovementAxialOrigin Slide_AxialOrigin;

		public FVRMovementManager.TeleportMode Teleport_Mode;

		public FVRMovementManager.TwoAxisMovementMode Touchpad_MovementMode;

		public FVRMovementManager.TwoAxisMovementConfirm Touchpad_Confirm;

		public FVRMovementManager.TwoAxisMovementStyle Touchpad_Style;

		public AXButtonSnapTurnMode AXButtonSnapTurnState = AXButtonSnapTurnMode.Snapturn;

		public ArmSwingerSnapTurnMode ArmSwingerSnapTurnState = ArmSwingerSnapTurnMode.Enabled;

		public ArmSwingerJumpMode ArmSwingerJumpState = ArmSwingerJumpMode.Enabled;

		public TwinStickSprintMode TwinStickSprintState;

		public TwinStickSprintToggleMode TwinStickSprintToggleState;

		public TwinStickSnapturnMode TwinStickSnapturnState = TwinStickSnapturnMode.Enabled;

		public TwinStickJumpMode TwinStickJumpState = TwinStickJumpMode.Enabled;

		public TwinStickLeftRightSetup TwinStickLeftRightState;

		public GlobalTurningMode GlobalTurningModeState;

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
			0f,
			0.15f,
			0.25f,
			0.5f,
			0.8f,
			1.2f
		};

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("CurrentMovementMode"))
			{
				CurrentMovementMode = reader.Read<FVRMovementManager.MovementMode>("CurrentMovementMode");
			}
			if (reader.TagExists("Teleport_AxialOrigin"))
			{
				Teleport_AxialOrigin = reader.Read<FVRMovementManager.MovementAxialOrigin>("Teleport_AxialOrigin");
			}
			if (reader.TagExists("Dash_AxialOrigin"))
			{
				Dash_AxialOrigin = reader.Read<FVRMovementManager.MovementAxialOrigin>("Dash_AxialOrigin");
			}
			if (reader.TagExists("Slide_AxialOrigin"))
			{
				Slide_AxialOrigin = reader.Read<FVRMovementManager.MovementAxialOrigin>("Slide_AxialOrigin");
			}
			if (reader.TagExists("Teleport_Mode"))
			{
				Teleport_Mode = reader.Read<FVRMovementManager.TeleportMode>("Teleport_Mode");
			}
			if (reader.TagExists("Touchpad_MovementMode"))
			{
				Touchpad_MovementMode = reader.Read<FVRMovementManager.TwoAxisMovementMode>("Touchpad_MovementMode");
			}
			if (reader.TagExists("Touchpad_Confirm"))
			{
				Touchpad_Confirm = reader.Read<FVRMovementManager.TwoAxisMovementConfirm>("Touchpad_Confirm");
			}
			if (reader.TagExists("Touchpad_Style"))
			{
				Touchpad_Style = reader.Read<FVRMovementManager.TwoAxisMovementStyle>("Touchpad_Style");
			}
			if (reader.TagExists("SlidingSpeedTick"))
			{
				SlidingSpeedTick = reader.Read<int>("SlidingSpeedTick");
			}
			if (reader.TagExists("TPLocoSpeedIndex"))
			{
				TPLocoSpeedIndex = reader.Read<int>("TPLocoSpeedIndex");
			}
			if (reader.TagExists("SnapTurnMagnitudeIndex"))
			{
				SnapTurnMagnitudeIndex = reader.Read<int>("SnapTurnMagnitudeIndex");
			}
			if (reader.TagExists("AXButtonSnapTurnState"))
			{
				AXButtonSnapTurnState = reader.Read<AXButtonSnapTurnMode>("AXButtonSnapTurnState");
			}
			if (reader.TagExists("ArmSwingerSnapTurnState"))
			{
				ArmSwingerSnapTurnState = reader.Read<ArmSwingerSnapTurnMode>("ArmSwingerSnapTurnState");
			}
			if (reader.TagExists("TwinStickSprintState"))
			{
				TwinStickSprintState = reader.Read<TwinStickSprintMode>("TwinStickSprintState");
			}
			if (reader.TagExists("ArmSwingerJumpState"))
			{
				ArmSwingerJumpState = reader.Read<ArmSwingerJumpMode>("ArmSwingerJumpState");
			}
			if (reader.TagExists("ArmSwingerBaseSpeed_Left"))
			{
				ArmSwingerBaseSpeed_Left = reader.Read<int>("ArmSwingerBaseSpeed_Left");
			}
			if (reader.TagExists("ArmSwingerBaseSpeed_Right"))
			{
				ArmSwingerBaseSpeed_Right = reader.Read<int>("ArmSwingerBaseSpeed_Right");
			}
			if (reader.TagExists("TwinStickSnapturnState"))
			{
				TwinStickSnapturnState = reader.Read<TwinStickSnapturnMode>("TwinStickSnapturnState");
			}
			if (reader.TagExists("TwinStickSprintToggleState"))
			{
				TwinStickSprintToggleState = reader.Read<TwinStickSprintToggleMode>("TwinStickSprintToggleState");
			}
			if (reader.TagExists("TwinStickJumpState"))
			{
				TwinStickJumpState = reader.Read<TwinStickJumpMode>("TwinStickJumpState");
			}
			if (reader.TagExists("TwinStickLeftRightState"))
			{
				TwinStickLeftRightState = reader.Read<TwinStickLeftRightSetup>("TwinStickLeftRightState");
			}
			if (reader.TagExists("GlobalTurningModeState"))
			{
				GlobalTurningModeState = reader.Read<GlobalTurningMode>("GlobalTurningModeState");
			}
			if (reader.TagExists("SmoothTurnMagnitudeIndex"))
			{
				SmoothTurnMagnitudeIndex = reader.Read<int>("SmoothTurnMagnitudeIndex");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(CurrentMovementMode, "CurrentMovementMode");
			writer.Write(Teleport_AxialOrigin, "Teleport_AxialOrigin");
			writer.Write(Dash_AxialOrigin, "Dash_AxialOrigin");
			writer.Write(Slide_AxialOrigin, "Slide_AxialOrigin");
			writer.Write(Teleport_Mode, "Teleport_Mode");
			writer.Write(Touchpad_MovementMode, "Touchpad_MovementMode");
			writer.Write(Touchpad_Confirm, "Touchpad_Confirm");
			writer.Write(Touchpad_Style, "Touchpad_Style");
			writer.Write(SlidingSpeedTick, "SlidingSpeedTick");
			writer.Write(TPLocoSpeedIndex, "TPLocoSpeedIndex");
			writer.Write(SnapTurnMagnitudeIndex, "SnapTurnMagnitudeIndex");
			writer.Write(ArmSwingerJumpState, "ArmSwingerJumpState");
			writer.Write(ArmSwingerBaseSpeed_Left, "ArmSwingerBaseSpeed_Left");
			writer.Write(ArmSwingerBaseSpeed_Right, "ArmSwingerBaseSpeed_Right");
			writer.Write(AXButtonSnapTurnState, "AXButtonSnapTurnState");
			writer.Write(ArmSwingerSnapTurnState, "ArmSwingerSnapTurnState");
			writer.Write(TwinStickSprintState, "TwinStickSprintState");
			writer.Write(TwinStickSnapturnState, "TwinStickSnapturnState");
			writer.Write(TwinStickSprintToggleState, "TwinStickSprintToggleState");
			writer.Write(TwinStickJumpState, "TwinStickJumpState");
			writer.Write(TwinStickLeftRightState, "TwinStickLeftRightState");
			writer.Write(GlobalTurningModeState, "GlobalTurningModeState");
			writer.Write(SmoothTurnMagnitudeIndex, "SmoothTurnMagnitudeIndex");
		}
	}
}
