using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Single : MonoBehaviour
	{
		public delegate void AxisHandler(SteamVR_Behaviour_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);

		public delegate void ChangeHandler(SteamVR_Behaviour_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);

		public delegate void UpdateHandler(SteamVR_Behaviour_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta);

		public SteamVR_Action_Single singleAction;

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		[Tooltip("Fires whenever the action's value has changed since the last update.")]
		public SteamVR_Behaviour_SingleEvent onChange;

		[Tooltip("Fires whenever the action's value has been updated.")]
		public SteamVR_Behaviour_SingleEvent onUpdate;

		[Tooltip("Fires whenever the action's value has been updated and is non-zero.")]
		public SteamVR_Behaviour_SingleEvent onAxis;

		public ChangeHandler onChangeEvent;

		public UpdateHandler onUpdateEvent;

		public AxisHandler onAxisEvent;

		public bool isActive => singleAction.GetActive(inputSource);

		protected virtual void OnEnable()
		{
			if (singleAction == null)
			{
				Debug.LogError("[SteamVR] Single action not set.", this);
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
			singleAction[inputSource].onUpdate += SteamVR_Behaviour_Single_OnUpdate;
			singleAction[inputSource].onChange += SteamVR_Behaviour_Single_OnChange;
			singleAction[inputSource].onAxis += SteamVR_Behaviour_Single_OnAxis;
		}

		protected void RemoveHandlers()
		{
			if (singleAction != null)
			{
				singleAction[inputSource].onUpdate -= SteamVR_Behaviour_Single_OnUpdate;
				singleAction[inputSource].onChange -= SteamVR_Behaviour_Single_OnChange;
				singleAction[inputSource].onAxis -= SteamVR_Behaviour_Single_OnAxis;
			}
		}

		private void SteamVR_Behaviour_Single_OnUpdate(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
		{
			if (onUpdate != null)
			{
				onUpdate.Invoke(this, fromSource, newAxis, newDelta);
			}
			if (onUpdateEvent != null)
			{
				onUpdateEvent(this, fromSource, newAxis, newDelta);
			}
		}

		private void SteamVR_Behaviour_Single_OnChange(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
		{
			if (onChange != null)
			{
				onChange.Invoke(this, fromSource, newAxis, newDelta);
			}
			if (onChangeEvent != null)
			{
				onChangeEvent(this, fromSource, newAxis, newDelta);
			}
		}

		private void SteamVR_Behaviour_Single_OnAxis(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
		{
			if (onAxis != null)
			{
				onAxis.Invoke(this, fromSource, newAxis, newDelta);
			}
			if (onAxisEvent != null)
			{
				onAxisEvent(this, fromSource, newAxis, newDelta);
			}
		}

		public string GetLocalizedName(params EVRInputStringBits[] localizedParts)
		{
			if (singleAction != null)
			{
				return singleAction.GetLocalizedOriginPart(inputSource, localizedParts);
			}
			return null;
		}
	}
}
