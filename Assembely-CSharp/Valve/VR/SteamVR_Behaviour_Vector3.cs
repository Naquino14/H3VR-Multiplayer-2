using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Vector3 : MonoBehaviour
	{
		public delegate void AxisHandler(SteamVR_Behaviour_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta);

		public delegate void ChangeHandler(SteamVR_Behaviour_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta);

		public delegate void UpdateHandler(SteamVR_Behaviour_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta);

		public SteamVR_Action_Vector3 vector3Action;

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		[Tooltip("Fires whenever the action's value has changed since the last update.")]
		public SteamVR_Behaviour_Vector3Event onChange;

		[Tooltip("Fires whenever the action's value has been updated.")]
		public SteamVR_Behaviour_Vector3Event onUpdate;

		[Tooltip("Fires whenever the action's value has been updated and is non-zero.")]
		public SteamVR_Behaviour_Vector3Event onAxis;

		public ChangeHandler onChangeEvent;

		public UpdateHandler onUpdateEvent;

		public AxisHandler onAxisEvent;

		public bool isActive => vector3Action.GetActive(inputSource);

		protected virtual void OnEnable()
		{
			if (vector3Action == null)
			{
				Debug.LogError("[SteamVR] Vector3 action not set.", this);
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
			vector3Action[inputSource].onUpdate += SteamVR_Behaviour_Vector3_OnUpdate;
			vector3Action[inputSource].onChange += SteamVR_Behaviour_Vector3_OnChange;
			vector3Action[inputSource].onAxis += SteamVR_Behaviour_Vector3_OnAxis;
		}

		protected void RemoveHandlers()
		{
			if (vector3Action != null)
			{
				vector3Action[inputSource].onUpdate -= SteamVR_Behaviour_Vector3_OnUpdate;
				vector3Action[inputSource].onChange -= SteamVR_Behaviour_Vector3_OnChange;
				vector3Action[inputSource].onAxis -= SteamVR_Behaviour_Vector3_OnAxis;
			}
		}

		private void SteamVR_Behaviour_Vector3_OnUpdate(SteamVR_Action_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta)
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

		private void SteamVR_Behaviour_Vector3_OnChange(SteamVR_Action_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta)
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

		private void SteamVR_Behaviour_Vector3_OnAxis(SteamVR_Action_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 newAxis, Vector3 newDelta)
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
			if (vector3Action != null)
			{
				return vector3Action.GetLocalizedOriginPart(inputSource, localizedParts);
			}
			return null;
		}
	}
}
