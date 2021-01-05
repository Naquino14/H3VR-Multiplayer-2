using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Action_Boolean : SteamVR_Action_In<SteamVR_Action_Boolean_Source_Map, SteamVR_Action_Boolean_Source>, ISteamVR_Action_Boolean, ISerializationCallbackReceiver, ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		public delegate void StateDownHandler(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource);

		public delegate void StateUpHandler(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource);

		public delegate void StateHandler(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource);

		public delegate void ActiveChangeHandler(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool active);

		public delegate void ChangeHandler(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState);

		public delegate void UpdateHandler(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState);

		public bool state => sourceMap[SteamVR_Input_Sources.Any].state;

		public bool stateDown => sourceMap[SteamVR_Input_Sources.Any].stateDown;

		public bool stateUp => sourceMap[SteamVR_Input_Sources.Any].stateUp;

		public bool lastState => sourceMap[SteamVR_Input_Sources.Any].lastState;

		public bool lastStateDown => sourceMap[SteamVR_Input_Sources.Any].lastStateDown;

		public bool lastStateUp => sourceMap[SteamVR_Input_Sources.Any].lastStateUp;

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

		public event StateHandler onState
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onState += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onState -= value;
			}
		}

		public event StateDownHandler onStateDown
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onStateDown += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onStateDown -= value;
			}
		}

		public event StateUpHandler onStateUp
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onStateUp += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onStateUp -= value;
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

		public bool GetStateDown(SteamVR_Input_Sources inputSource)
		{
			return sourceMap[inputSource].stateDown;
		}

		public bool GetStateUp(SteamVR_Input_Sources inputSource)
		{
			return sourceMap[inputSource].stateUp;
		}

		public bool GetState(SteamVR_Input_Sources inputSource)
		{
			return sourceMap[inputSource].state;
		}

		public bool GetLastStateDown(SteamVR_Input_Sources inputSource)
		{
			return sourceMap[inputSource].lastStateDown;
		}

		public bool GetLastStateUp(SteamVR_Input_Sources inputSource)
		{
			return sourceMap[inputSource].lastStateUp;
		}

		public bool GetLastState(SteamVR_Input_Sources inputSource)
		{
			return sourceMap[inputSource].lastState;
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

		public void AddOnStateDownListener(StateDownHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onStateDown += functionToCall;
		}

		public void RemoveOnStateDownListener(StateDownHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onStateDown -= functionToStopCalling;
		}

		public void AddOnStateUpListener(StateUpHandler functionToCall, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onStateUp += functionToCall;
		}

		public void RemoveOnStateUpListener(StateUpHandler functionToStopCalling, SteamVR_Input_Sources inputSource)
		{
			sourceMap[inputSource].onStateUp -= functionToStopCalling;
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
