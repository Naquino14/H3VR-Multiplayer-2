namespace Valve.VR
{
	public class SteamVR_Actions
	{
		private static SteamVR_Input_ActionSet_default p__default;

		private static SteamVR_Input_ActionSet_mixedreality p_mixedreality;

		private static SteamVR_Action_Pose p_default_Pose;

		private static SteamVR_Action_Skeleton p_default_SkeletonLeftHand;

		private static SteamVR_Action_Skeleton p_default_SkeletonRightHand;

		private static SteamVR_Action_Boolean p_default_HeadsetOnHead;

		private static SteamVR_Action_Single p_default_Trigger_Axis;

		private static SteamVR_Action_Boolean p_default_Trigger_Button;

		private static SteamVR_Action_Boolean p_default_Trigger_Touch;

		private static SteamVR_Action_Vector2 p_default_Primary2Axis_Axes;

		private static SteamVR_Action_Boolean p_default_Primary2Axis_Button;

		private static SteamVR_Action_Boolean p_default_Primary2Axis_Touch;

		private static SteamVR_Action_Vector2 p_default_Secondary2Axis_Axes;

		private static SteamVR_Action_Boolean p_default_Secondary2Axis_Button;

		private static SteamVR_Action_Boolean p_default_Secondary2Axis_Touch;

		private static SteamVR_Action_Boolean p_default_A_Button;

		private static SteamVR_Action_Boolean p_default_B_Button;

		private static SteamVR_Action_Boolean p_default_Grip_Button;

		private static SteamVR_Action_Boolean p_default_Grip_Touch;

		private static SteamVR_Action_Single p_default_Grip_Squeeze;

		private static SteamVR_Action_Single p_default_Thumb_Squeeze;

		private static SteamVR_Action_Vibration p_default_Haptic;

		private static SteamVR_Action_Pose p_mixedreality_ExternalCamera;

		public static SteamVR_Input_ActionSet_default _default => p__default.GetCopy<SteamVR_Input_ActionSet_default>();

		public static SteamVR_Input_ActionSet_mixedreality mixedreality => p_mixedreality.GetCopy<SteamVR_Input_ActionSet_mixedreality>();

		public static SteamVR_Action_Pose default_Pose => p_default_Pose.GetCopy<SteamVR_Action_Pose>();

		public static SteamVR_Action_Skeleton default_SkeletonLeftHand => p_default_SkeletonLeftHand.GetCopy<SteamVR_Action_Skeleton>();

		public static SteamVR_Action_Skeleton default_SkeletonRightHand => p_default_SkeletonRightHand.GetCopy<SteamVR_Action_Skeleton>();

		public static SteamVR_Action_Boolean default_HeadsetOnHead => p_default_HeadsetOnHead.GetCopy<SteamVR_Action_Boolean>();

		public static SteamVR_Action_Single default_Trigger_Axis => p_default_Trigger_Axis.GetCopy<SteamVR_Action_Single>();

		public static SteamVR_Action_Boolean default_Trigger_Button => p_default_Trigger_Button.GetCopy<SteamVR_Action_Boolean>();

		public static SteamVR_Action_Boolean default_Trigger_Touch => p_default_Trigger_Touch.GetCopy<SteamVR_Action_Boolean>();

		public static SteamVR_Action_Vector2 default_Primary2Axis_Axes => p_default_Primary2Axis_Axes.GetCopy<SteamVR_Action_Vector2>();

		public static SteamVR_Action_Boolean default_Primary2Axis_Button => p_default_Primary2Axis_Button.GetCopy<SteamVR_Action_Boolean>();

		public static SteamVR_Action_Boolean default_Primary2Axis_Touch => p_default_Primary2Axis_Touch.GetCopy<SteamVR_Action_Boolean>();

		public static SteamVR_Action_Vector2 default_Secondary2Axis_Axes => p_default_Secondary2Axis_Axes.GetCopy<SteamVR_Action_Vector2>();

		public static SteamVR_Action_Boolean default_Secondary2Axis_Button => p_default_Secondary2Axis_Button.GetCopy<SteamVR_Action_Boolean>();

		public static SteamVR_Action_Boolean default_Secondary2Axis_Touch => p_default_Secondary2Axis_Touch.GetCopy<SteamVR_Action_Boolean>();

		public static SteamVR_Action_Boolean default_A_Button => p_default_A_Button.GetCopy<SteamVR_Action_Boolean>();

		public static SteamVR_Action_Boolean default_B_Button => p_default_B_Button.GetCopy<SteamVR_Action_Boolean>();

		public static SteamVR_Action_Boolean default_Grip_Button => p_default_Grip_Button.GetCopy<SteamVR_Action_Boolean>();

		public static SteamVR_Action_Boolean default_Grip_Touch => p_default_Grip_Touch.GetCopy<SteamVR_Action_Boolean>();

		public static SteamVR_Action_Single default_Grip_Squeeze => p_default_Grip_Squeeze.GetCopy<SteamVR_Action_Single>();

		public static SteamVR_Action_Single default_Thumb_Squeeze => p_default_Thumb_Squeeze.GetCopy<SteamVR_Action_Single>();

		public static SteamVR_Action_Vibration default_Haptic => p_default_Haptic.GetCopy<SteamVR_Action_Vibration>();

		public static SteamVR_Action_Pose mixedreality_ExternalCamera => p_mixedreality_ExternalCamera.GetCopy<SteamVR_Action_Pose>();

		private static void StartPreInitActionSets()
		{
			p__default = SteamVR_ActionSet.Create<SteamVR_Input_ActionSet_default>("/actions/default");
			p_mixedreality = SteamVR_ActionSet.Create<SteamVR_Input_ActionSet_mixedreality>("/actions/mixedreality");
			SteamVR_Input.actionSets = new SteamVR_ActionSet[2]
			{
				_default,
				mixedreality
			};
		}

		private static void InitializeActionArrays()
		{
			SteamVR_Input.actions = new SteamVR_Action[21]
			{
				default_Pose,
				default_SkeletonLeftHand,
				default_SkeletonRightHand,
				default_HeadsetOnHead,
				default_Trigger_Axis,
				default_Trigger_Button,
				default_Trigger_Touch,
				default_Primary2Axis_Axes,
				default_Primary2Axis_Button,
				default_Primary2Axis_Touch,
				default_Secondary2Axis_Axes,
				default_Secondary2Axis_Button,
				default_Secondary2Axis_Touch,
				default_A_Button,
				default_B_Button,
				default_Grip_Button,
				default_Grip_Touch,
				default_Grip_Squeeze,
				default_Thumb_Squeeze,
				default_Haptic,
				mixedreality_ExternalCamera
			};
			SteamVR_Input.actionsIn = new ISteamVR_Action_In[20]
			{
				default_Pose,
				default_SkeletonLeftHand,
				default_SkeletonRightHand,
				default_HeadsetOnHead,
				default_Trigger_Axis,
				default_Trigger_Button,
				default_Trigger_Touch,
				default_Primary2Axis_Axes,
				default_Primary2Axis_Button,
				default_Primary2Axis_Touch,
				default_Secondary2Axis_Axes,
				default_Secondary2Axis_Button,
				default_Secondary2Axis_Touch,
				default_A_Button,
				default_B_Button,
				default_Grip_Button,
				default_Grip_Touch,
				default_Grip_Squeeze,
				default_Thumb_Squeeze,
				mixedreality_ExternalCamera
			};
			SteamVR_Input.actionsOut = new ISteamVR_Action_Out[1]
			{
				default_Haptic
			};
			SteamVR_Input.actionsVibration = new SteamVR_Action_Vibration[1]
			{
				default_Haptic
			};
			SteamVR_Input.actionsPose = new SteamVR_Action_Pose[2]
			{
				default_Pose,
				mixedreality_ExternalCamera
			};
			SteamVR_Input.actionsBoolean = new SteamVR_Action_Boolean[11]
			{
				default_HeadsetOnHead,
				default_Trigger_Button,
				default_Trigger_Touch,
				default_Primary2Axis_Button,
				default_Primary2Axis_Touch,
				default_Secondary2Axis_Button,
				default_Secondary2Axis_Touch,
				default_A_Button,
				default_B_Button,
				default_Grip_Button,
				default_Grip_Touch
			};
			SteamVR_Input.actionsSingle = new SteamVR_Action_Single[3]
			{
				default_Trigger_Axis,
				default_Grip_Squeeze,
				default_Thumb_Squeeze
			};
			SteamVR_Input.actionsVector2 = new SteamVR_Action_Vector2[2]
			{
				default_Primary2Axis_Axes,
				default_Secondary2Axis_Axes
			};
			SteamVR_Input.actionsVector3 = new SteamVR_Action_Vector3[0];
			SteamVR_Input.actionsSkeleton = new SteamVR_Action_Skeleton[2]
			{
				default_SkeletonLeftHand,
				default_SkeletonRightHand
			};
			SteamVR_Input.actionsNonPoseNonSkeletonIn = new ISteamVR_Action_In[16]
			{
				default_HeadsetOnHead,
				default_Trigger_Axis,
				default_Trigger_Button,
				default_Trigger_Touch,
				default_Primary2Axis_Axes,
				default_Primary2Axis_Button,
				default_Primary2Axis_Touch,
				default_Secondary2Axis_Axes,
				default_Secondary2Axis_Button,
				default_Secondary2Axis_Touch,
				default_A_Button,
				default_B_Button,
				default_Grip_Button,
				default_Grip_Touch,
				default_Grip_Squeeze,
				default_Thumb_Squeeze
			};
		}

		private static void PreInitActions()
		{
			p_default_Pose = SteamVR_Action.Create<SteamVR_Action_Pose>("/actions/default/in/Pose");
			p_default_SkeletonLeftHand = SteamVR_Action.Create<SteamVR_Action_Skeleton>("/actions/default/in/SkeletonLeftHand");
			p_default_SkeletonRightHand = SteamVR_Action.Create<SteamVR_Action_Skeleton>("/actions/default/in/SkeletonRightHand");
			p_default_HeadsetOnHead = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/HeadsetOnHead");
			p_default_Trigger_Axis = SteamVR_Action.Create<SteamVR_Action_Single>("/actions/default/in/Trigger_Axis");
			p_default_Trigger_Button = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Trigger_Button");
			p_default_Trigger_Touch = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Trigger_Touch");
			p_default_Primary2Axis_Axes = SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/default/in/Primary2Axis_Axes");
			p_default_Primary2Axis_Button = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Primary2Axis_Button");
			p_default_Primary2Axis_Touch = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Primary2Axis_Touch");
			p_default_Secondary2Axis_Axes = SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/default/in/Secondary2Axis_Axes");
			p_default_Secondary2Axis_Button = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Secondary2Axis_Button");
			p_default_Secondary2Axis_Touch = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Secondary2Axis_Touch");
			p_default_A_Button = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/A_Button");
			p_default_B_Button = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/B_Button");
			p_default_Grip_Button = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Grip_Button");
			p_default_Grip_Touch = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Grip_Touch");
			p_default_Grip_Squeeze = SteamVR_Action.Create<SteamVR_Action_Single>("/actions/default/in/Grip_Squeeze");
			p_default_Thumb_Squeeze = SteamVR_Action.Create<SteamVR_Action_Single>("/actions/default/in/Thumb_Squeeze");
			p_default_Haptic = SteamVR_Action.Create<SteamVR_Action_Vibration>("/actions/default/out/Haptic");
			p_mixedreality_ExternalCamera = SteamVR_Action.Create<SteamVR_Action_Pose>("/actions/mixedreality/in/ExternalCamera");
		}

		public static void PreInitialize()
		{
			StartPreInitActionSets();
			SteamVR_Input.PreinitializeActionSetDictionaries();
			PreInitActions();
			InitializeActionArrays();
			SteamVR_Input.PreinitializeActionDictionaries();
			SteamVR_Input.PreinitializeFinishActionSets();
		}
	}
}
