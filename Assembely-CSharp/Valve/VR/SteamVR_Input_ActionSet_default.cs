namespace Valve.VR
{
	public class SteamVR_Input_ActionSet_default : SteamVR_ActionSet
	{
		public virtual SteamVR_Action_Pose Pose => SteamVR_Actions.default_Pose;

		public virtual SteamVR_Action_Skeleton SkeletonLeftHand => SteamVR_Actions.default_SkeletonLeftHand;

		public virtual SteamVR_Action_Skeleton SkeletonRightHand => SteamVR_Actions.default_SkeletonRightHand;

		public virtual SteamVR_Action_Boolean HeadsetOnHead => SteamVR_Actions.default_HeadsetOnHead;

		public virtual SteamVR_Action_Single Trigger_Axis => SteamVR_Actions.default_Trigger_Axis;

		public virtual SteamVR_Action_Boolean Trigger_Button => SteamVR_Actions.default_Trigger_Button;

		public virtual SteamVR_Action_Boolean Trigger_Touch => SteamVR_Actions.default_Trigger_Touch;

		public virtual SteamVR_Action_Vector2 Primary2Axis_Axes => SteamVR_Actions.default_Primary2Axis_Axes;

		public virtual SteamVR_Action_Boolean Primary2Axis_Button => SteamVR_Actions.default_Primary2Axis_Button;

		public virtual SteamVR_Action_Boolean Primary2Axis_Touch => SteamVR_Actions.default_Primary2Axis_Touch;

		public virtual SteamVR_Action_Vector2 Secondary2Axis_Axes => SteamVR_Actions.default_Secondary2Axis_Axes;

		public virtual SteamVR_Action_Boolean Secondary2Axis_Button => SteamVR_Actions.default_Secondary2Axis_Button;

		public virtual SteamVR_Action_Boolean Secondary2Axis_Touch => SteamVR_Actions.default_Secondary2Axis_Touch;

		public virtual SteamVR_Action_Boolean A_Button => SteamVR_Actions.default_A_Button;

		public virtual SteamVR_Action_Boolean B_Button => SteamVR_Actions.default_B_Button;

		public virtual SteamVR_Action_Boolean Grip_Button => SteamVR_Actions.default_Grip_Button;

		public virtual SteamVR_Action_Boolean Grip_Touch => SteamVR_Actions.default_Grip_Touch;

		public virtual SteamVR_Action_Single Grip_Squeeze => SteamVR_Actions.default_Grip_Squeeze;

		public virtual SteamVR_Action_Single Thumb_Squeeze => SteamVR_Actions.default_Thumb_Squeeze;

		public virtual SteamVR_Action_Vibration Haptic => SteamVR_Actions.default_Haptic;
	}
}
