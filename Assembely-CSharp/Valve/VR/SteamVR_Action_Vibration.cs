using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Action_Vibration : SteamVR_Action_Out<SteamVR_Action_Vibration_Source_Map, SteamVR_Action_Vibration_Source>, ISerializationCallbackReceiver
	{
		public delegate void ActiveChangeHandler(SteamVR_Action_Vibration fromAction, SteamVR_Input_Sources fromSource, bool active);

		public delegate void ExecuteHandler(SteamVR_Action_Vibration fromAction, SteamVR_Input_Sources fromSource, float secondsFromNow, float durationSeconds, float frequency, float amplitude);

		public event ActiveChangeHandler onActiveChange
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveChange += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= value;
			}
		}

		public event ActiveChangeHandler onActiveBindingChange
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange -= value;
			}
		}

		public event ExecuteHandler onExecute
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onExecute += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onExecute -= value;
			}
		}

		public void Execute(float secondsFromNow, float durationSeconds, float frequency, float amplitude, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].Execute(secondsFromNow, durationSeconds, frequency, amplitude);
		}

		public void AddOnActiveChangeListener(ActiveChangeHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onActiveChange += functionToCall;
		}

		public void RemoveOnActiveChangeListener(ActiveChangeHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onActiveChange -= functionToStopCalling;
		}

		public void AddOnActiveBindingChangeListener(ActiveChangeHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onActiveBindingChange += functionToCall;
		}

		public void RemoveOnActiveBindingChangeListener(ActiveChangeHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onActiveBindingChange -= functionToStopCalling;
		}

		public void AddOnExecuteListener(ExecuteHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onExecute += functionToCall;
		}

		public void RemoveOnExecuteListener(ExecuteHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onExecute -= functionToStopCalling;
		}

		public override float GetTimeLastChanged(SteamVR_Input_Sources inputSource)
		{
			return sourceMap[inputSource].timeLastExecuted;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			InitAfterDeserialize();
		}

		public override bool IsUpdating(SteamVR_Input_Sources inputSource)
		{
			return sourceMap.IsUpdating(inputSource);
		}
	}
}
