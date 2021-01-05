using System;

namespace FistVR
{
	[Serializable]
	public class ControlOptions
	{
		public enum ButtonControlStyle
		{
			Instant,
			Hold1Second,
			DoubleClick
		}

		public enum CoreControlMode
		{
			Standard,
			Streamlined
		}

		public enum GripButtonToHoldOverrideMode
		{
			Disabled,
			OculusOverride,
			ViveOverride
		}

		public enum MeatFingerMode
		{
			Disabled,
			Enabled
		}

		public enum MeatBody
		{
			Disabled,
			Enabled
		}

		public enum DesktopCameraMode
		{
			Default,
			HDSpectator,
			ThirdPerson,
			SpawnedObject,
			SosigView
		}

		public enum DesktopCameraEye
		{
			Right,
			Left
		}

		public enum DesktopRenderQuality
		{
			Low,
			Med,
			High
		}

		public enum PreviewCamMode
		{
			Disabled,
			Enabled
		}

		public enum HapticsMode
		{
			Enabled,
			Disabled
		}

		public enum WIPGrabbity
		{
			Enabled,
			Disabled
		}

		public enum WIPGrabbityButton
		{
			Trigger,
			Grab
		}

		public enum WristMenuMode
		{
			BothHands,
			RightHand,
			LeftHand
		}

		public CoreControlMode CCM;

		public bool HasConfirmedControls;

		public ButtonControlStyle GripButtonDropStyle;

		public bool UseInvertedHandgunMagPose;

		public bool UseEasyMagLoading;

		public bool UseGunRigMode2;

		public bool UseVirtualStock = true;

		public GripButtonToHoldOverrideMode GripButtonToHoldOverride;

		public MeatFingerMode MFMode;

		public MeatBody MBMode;

		public SosigEnemyID MBClothing = SosigEnemyID.None;

		public DesktopCameraMode CamMode;

		public float CamFOV = 55f;

		public float CamSmoothingLinear;

		public float CamSmoothingRotational;

		public float CamLeveling;

		public DesktopCameraEye CamEye;

		public DesktopRenderQuality CamQual;

		public PreviewCamMode PCamMode;

		public int TPCDistanceIndex = 2;

		public int TPCLateralIndex = 2;

		public HapticsMode HapticsState;

		public WIPGrabbity WIPGrabbityState = WIPGrabbity.Disabled;

		public WIPGrabbityButton WIPGrabbityButtonState;

		public WristMenuMode WristMenuState;

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("CCM"))
			{
				CCM = reader.Read<CoreControlMode>("CCM");
			}
			if (reader.TagExists("GripButtonDropStyle"))
			{
				GripButtonDropStyle = reader.Read<ButtonControlStyle>("GripButtonDropStyle");
			}
			if (reader.TagExists("UseInvertedHandgunMagPose"))
			{
				UseInvertedHandgunMagPose = reader.Read<bool>("UseInvertedHandgunMagPose");
			}
			if (reader.TagExists("UseEasyMagLoading"))
			{
				UseEasyMagLoading = reader.Read<bool>("UseEasyMagLoading");
			}
			if (reader.TagExists("UseGunRigMode2"))
			{
				UseGunRigMode2 = reader.Read<bool>("UseGunRigMode2");
			}
			if (reader.TagExists("UseVirtualStock"))
			{
				UseVirtualStock = reader.Read<bool>("UseVirtualStock");
			}
			if (reader.TagExists("MFMode"))
			{
				MFMode = reader.Read<MeatFingerMode>("MFMode");
			}
			if (reader.TagExists("MBMode"))
			{
				MBMode = reader.Read<MeatBody>("MBMode");
			}
			if (reader.TagExists("MBClothing"))
			{
				MBClothing = reader.Read<SosigEnemyID>("MBClothing");
			}
			if (reader.TagExists("GripButtonToHoldOverride"))
			{
				GripButtonToHoldOverride = reader.Read<GripButtonToHoldOverrideMode>("GripButtonToHoldOverride");
			}
			if (reader.TagExists("HapticsState"))
			{
				HapticsState = reader.Read<HapticsMode>("HapticsState");
			}
			if (reader.TagExists("WIPGrabbityState"))
			{
				WIPGrabbityState = reader.Read<WIPGrabbity>("WIPGrabbityState");
			}
			if (reader.TagExists("WIPGrabbityButtonState"))
			{
				WIPGrabbityButtonState = reader.Read<WIPGrabbityButton>("WIPGrabbityButtonState");
			}
			if (reader.TagExists("WristMenuState"))
			{
				WristMenuState = reader.Read<WristMenuMode>("WristMenuState");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(CCM, "CCM");
			writer.Write(GripButtonDropStyle, "GripButtonDropStyle");
			writer.Write(UseInvertedHandgunMagPose, "UseInvertedHandgunMagPose");
			writer.Write(UseEasyMagLoading, "UseEasyMagLoading");
			writer.Write(UseGunRigMode2, "UseGunRigMode2");
			writer.Write(UseVirtualStock, "UseVirtualStock");
			writer.Write(MFMode, "MFMode");
			writer.Write(MBMode, "MBMode");
			writer.Write(MBClothing, "MBClothing");
			writer.Write(GripButtonToHoldOverride, "GripButtonToHoldOverride");
			writer.Write(HapticsState, "HapticsState");
			writer.Write(WIPGrabbityState, "WIPGrabbityState");
			writer.Write(WIPGrabbityButtonState, "WIPGrabbityButtonState");
			writer.Write(WristMenuState, "WristMenuState");
		}
	}
}
