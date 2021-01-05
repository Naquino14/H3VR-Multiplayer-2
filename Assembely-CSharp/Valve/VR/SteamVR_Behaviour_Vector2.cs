using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Vector2 : MonoBehaviour
	{
		public delegate void AxisHandler(SteamVR_Behaviour_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta);

		public delegate void ChangeHandler(SteamVR_Behaviour_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta);

		public delegate void UpdateHandler(SteamVR_Behaviour_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta);

		public SteamVR_Action_Vector2 vector2Action;

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		[Tooltip("Fires whenever the action's value has changed since the last update.")]
		public SteamVR_Behaviour_Vector2Event onChange;

		[Tooltip("Fires whenever the action's value has been updated.")]
		public SteamVR_Behaviour_Vector2Event onUpdate;

		[Tooltip("Fires whenever the action's value has been updated and is non-zero.")]
		public SteamVR_Behaviour_Vector2Event onAxis;

		public ChangeHandler onChangeEvent;

		public UpdateHandler onUpdateEvent;

		public AxisHandler onAxisEvent;

		public bool isActive => vector2Action.GetActive(inputSource);

		protected virtual void OnEnable()
		{
			if (vector2Action == null)
			{
				Debug.LogError("[SteamVR] Vector2 action not set.", this);
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
			vector2Action[inputSource].onUpdate += SteamVR_Behaviour_Vector2_OnUpdate;
			vector2Action[inputSource].onChange += SteamVR_Behaviour_Vector2_OnChange;
			vector2Action[inputSource].onAxis += SteamVR_Behaviour_Vector2_OnAxis;
		}

		protected void RemoveHandlers()
		{
			if (vector2Action != null)
			{
				vector2Action[inputSource].onUpdate -= SteamVR_Behaviour_Vector2_OnUpdate;
				vector2Action[inputSource].onChange -= SteamVR_Behaviour_Vector2_OnChange;
				vector2Action[inputSource].onAxis -= SteamVR_Behaviour_Vector2_OnAxis;
			}
		}

		private void SteamVR_Behaviour_Vector2_OnUpdate(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta)
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

		private void SteamVR_Behaviour_Vector2_OnChange(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta)
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

		private void SteamVR_Behaviour_Vector2_OnAxis(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 newAxis, Vector2 newDelta)
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
			if (vector2Action != null)
			{
				return vector2Action.GetLocalizedOriginPart(inputSource, localizedParts);
			}
			return null;
		}
	}
}
