using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Action_Vibration_Source : SteamVR_Action_Out_Source
	{
		protected SteamVR_Action_Vibration vibrationAction;

		public override bool active => activeBinding && base.setActive;

		public override bool activeBinding => true;

		public override bool lastActive
		{
			get;
			protected set;
		}

		public override bool lastActiveBinding => true;

		public float timeLastExecuted
		{
			get;
			protected set;
		}

		public event SteamVR_Action_Vibration.ActiveChangeHandler onActiveChange;

		public event SteamVR_Action_Vibration.ActiveChangeHandler onActiveBindingChange;

		public event SteamVR_Action_Vibration.ExecuteHandler onExecute;

		public override void Initialize()
		{
			base.Initialize();
			lastActive = true;
		}

		public override void Preinitialize(SteamVR_Action wrappingAction, SteamVR_Input_Sources forInputSource)
		{
			base.Preinitialize(wrappingAction, forInputSource);
			vibrationAction = (SteamVR_Action_Vibration)wrappingAction;
		}

		public void Execute(float secondsFromNow, float durationSeconds, float frequency, float amplitude)
		{
			if (!SteamVR_Input.isStartupFrame)
			{
				timeLastExecuted = Time.realtimeSinceStartup;
				EVRInputError eVRInputError = OpenVR.Input.TriggerHapticVibrationAction(base.handle, secondsFromNow, durationSeconds, frequency, amplitude, inputSourceHandle);
				if (eVRInputError != 0)
				{
					Debug.LogError("<b>[SteamVR]</b> TriggerHapticVibrationAction (" + base.fullPath + ") error: " + eVRInputError.ToString() + " handle: " + base.handle);
				}
				if (this.onExecute != null)
				{
					this.onExecute(vibrationAction, base.inputSource, secondsFromNow, durationSeconds, frequency, amplitude);
				}
			}
		}
	}
}
