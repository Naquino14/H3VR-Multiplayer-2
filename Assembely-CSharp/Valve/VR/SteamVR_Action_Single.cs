using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Action_Single : SteamVR_Action_In<SteamVR_Action_Single_Source_Map, SteamVR_Action_Single_Source>, ISteamVR_Action_Single, ISerializationCallbackReceiver, ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		public delegate void AxisHandler(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);

		public delegate void ActiveChangeHandler(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, bool active);

		public delegate void ChangeHandler(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);

		public delegate void UpdateHandler(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);

		public float axis => sourceMap[SteamVR_Input_Sources.Any].axis;

		public float lastAxis => sourceMap[SteamVR_Input_Sources.Any].lastAxis;

		public float delta => sourceMap[SteamVR_Input_Sources.Any].delta;

		public float lastDelta => sourceMap[SteamVR_Input_Sources.Any].lastDelta;

		public event ChangeHandler onChange
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onChange += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onChange -= value;
			}
		}

		public event UpdateHandler onUpdate
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onUpdate += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onUpdate -= value;
			}
		}

		public event AxisHandler onAxis
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onAxis += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onAxis -= value;
			}
		}

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

		public float GetAxis(SteamVR_Input_Sources inputSource)
		{
			return sourceMap[inputSource].axis;
		}

		public float GetAxisDelta(SteamVR_Input_Sources inputSource)
		{
			return sourceMap[inputSource].delta;
		}

		public float GetLastAxis(SteamVR_Input_Sources inputSource)
		{
			return sourceMap[inputSource].lastAxis;
		}

		public float GetLastAxisDelta(SteamVR_Input_Sources inputSource)
		{
			return sourceMap[inputSource].lastDelta;
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

		public void AddOnChangeListener(ChangeHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onChange += functionToCall;
		}

		public void RemoveOnChangeListener(ChangeHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onChange -= functionToStopCalling;
		}

		public void AddOnUpdateListener(UpdateHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onUpdate += functionToCall;
		}

		public void RemoveOnUpdateListener(UpdateHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onUpdate -= functionToStopCalling;
		}

		public void AddOnAxisListener(AxisHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onAxis += functionToCall;
		}

		public void RemoveOnAxisListener(AxisHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onAxis -= functionToStopCalling;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			InitAfterDeserialize();
		}
	}
}
