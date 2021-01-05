using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Boolean : MonoBehaviour
	{
		public delegate void StateDownHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource);

		public delegate void StateUpHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource);

		public delegate void StateHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource);

		public delegate void ActiveChangeHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource, bool active);

		public delegate void ChangeHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState);

		public delegate void UpdateHandler(SteamVR_Behaviour_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState);

		[Tooltip("The SteamVR boolean action that this component should use")]
		public SteamVR_Action_Boolean booleanAction;

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		public SteamVR_Behaviour_BooleanEvent onChange;

		public SteamVR_Behaviour_BooleanEvent onUpdate;

		public SteamVR_Behaviour_BooleanEvent onPress;

		public SteamVR_Behaviour_BooleanEvent onPressDown;

		public SteamVR_Behaviour_BooleanEvent onPressUp;

		public bool isActive => booleanAction[inputSource].active;

		public SteamVR_ActionSet actionSet
		{
			get
			{
				if (booleanAction != null)
				{
					return booleanAction.actionSet;
				}
				return null;
			}
		}

		public event ChangeHandler onChangeEvent;

		public event UpdateHandler onUpdateEvent;

		public event StateHandler onPressEvent;

		public event StateDownHandler onPressDownEvent;

		public event StateUpHandler onPressUpEvent;

		protected virtual void OnEnable()
		{
			if (booleanAction == null)
			{
				Debug.LogError("[SteamVR] Boolean action not set.", this);
			}
			else
			{
				AddHandlers();
			}
		}

		protected virtual void OnDisable()
		{
			RemoveHandlers();
		}

		protected void AddHandlers()
		{
			booleanAction[inputSource].onUpdate += SteamVR_Behaviour_Boolean_OnUpdate;
			booleanAction[inputSource].onChange += SteamVR_Behaviour_Boolean_OnChange;
			booleanAction[inputSource].onState += SteamVR_Behaviour_Boolean_OnState;
			booleanAction[inputSource].onStateDown += SteamVR_Behaviour_Boolean_OnStateDown;
			booleanAction[inputSource].onStateUp += SteamVR_Behaviour_Boolean_OnStateUp;
		}

		protected void RemoveHandlers()
		{
			if (booleanAction != null)
			{
				booleanAction[inputSource].onUpdate -= SteamVR_Behaviour_Boolean_OnUpdate;
				booleanAction[inputSource].onChange -= SteamVR_Behaviour_Boolean_OnChange;
				booleanAction[inputSource].onState -= SteamVR_Behaviour_Boolean_OnState;
				booleanAction[inputSource].onStateDown -= SteamVR_Behaviour_Boolean_OnStateDown;
				booleanAction[inputSource].onStateUp -= SteamVR_Behaviour_Boolean_OnStateUp;
			}
		}

		private void SteamVR_Behaviour_Boolean_OnStateUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
		{
			if (onPressUp != null)
			{
				onPressUp.Invoke(this, fromSource, arg2: false);
			}
			if (this.onPressUpEvent != null)
			{
				this.onPressUpEvent(this, fromSource);
			}
		}

		private void SteamVR_Behaviour_Boolean_OnStateDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
		{
			if (onPressDown != null)
			{
				onPressDown.Invoke(this, fromSource, arg2: true);
			}
			if (this.onPressDownEvent != null)
			{
				this.onPressDownEvent(this, fromSource);
			}
		}

		private void SteamVR_Behaviour_Boolean_OnState(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
		{
			if (onPress != null)
			{
				onPress.Invoke(this, fromSource, arg2: true);
			}
			if (this.onPressEvent != null)
			{
				this.onPressEvent(this, fromSource);
			}
		}

		private void SteamVR_Behaviour_Boolean_OnUpdate(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
		{
			if (onUpdate != null)
			{
				onUpdate.Invoke(this, fromSource, newState);
			}
			if (this.onUpdateEvent != null)
			{
				this.onUpdateEvent(this, fromSource, newState);
			}
		}

		private void SteamVR_Behaviour_Boolean_OnChange(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
		{
			if (onChange != null)
			{
				onChange.Invoke(this, fromSource, newState);
			}
			if (this.onChangeEvent != null)
			{
				this.onChangeEvent(this, fromSource, newState);
			}
		}

		public string GetLocalizedName(params EVRInputStringBits[] localizedParts)
		{
			if (booleanAction != null)
			{
				return booleanAction.GetLocalizedOriginPart(inputSource, localizedParts);
			}
			return null;
		}
	}
}
