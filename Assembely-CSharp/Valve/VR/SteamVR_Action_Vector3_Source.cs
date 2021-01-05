using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Action_Vector3_Source : SteamVR_Action_In_Source, ISteamVR_Action_Vector3, ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		protected static uint actionData_size;

		public float changeTolerance = Mathf.Epsilon;

		protected InputAnalogActionData_t actionData = default(InputAnalogActionData_t);

		protected InputAnalogActionData_t lastActionData = default(InputAnalogActionData_t);

		protected SteamVR_Action_Vector3 vector3Action;

		public Vector3 axis
		{
			get;
			protected set;
		}

		public Vector3 lastAxis
		{
			get;
			protected set;
		}

		public Vector3 delta
		{
			get;
			protected set;
		}

		public Vector3 lastDelta
		{
			get;
			protected set;
		}

		public override bool changed
		{
			get;
			protected set;
		}

		public override bool lastChanged
		{
			get;
			protected set;
		}

		public override ulong activeOrigin
		{
			get
			{
				if (active)
				{
					return actionData.activeOrigin;
				}
				return 0uL;
			}
		}

		public override ulong lastActiveOrigin => lastActionData.activeOrigin;

		public override bool active => activeBinding && action.actionSet.IsActive(base.inputSource);

		public override bool activeBinding => actionData.bActive;

		public override bool lastActive
		{
			get;
			protected set;
		}

		public override bool lastActiveBinding => lastActionData.bActive;

		public event SteamVR_Action_Vector3.AxisHandler onAxis;

		public event SteamVR_Action_Vector3.ActiveChangeHandler onActiveChange;

		public event SteamVR_Action_Vector3.ActiveChangeHandler onActiveBindingChange;

		public event SteamVR_Action_Vector3.ChangeHandler onChange;

		public event SteamVR_Action_Vector3.UpdateHandler onUpdate;

		public override void Preinitialize(SteamVR_Action wrappingAction, SteamVR_Input_Sources forInputSource)
		{
			base.Preinitialize(wrappingAction, forInputSource);
			vector3Action = (SteamVR_Action_Vector3)wrappingAction;
		}

		public override void Initialize()
		{
			base.Initialize();
			if (actionData_size == 0)
			{
				actionData_size = (uint)Marshal.SizeOf(typeof(InputAnalogActionData_t));
			}
		}

		public override void UpdateValue()
		{
			lastActionData = actionData;
			lastActive = active;
			lastAxis = axis;
			lastDelta = delta;
			EVRInputError analogActionData = OpenVR.Input.GetAnalogActionData(base.handle, ref actionData, actionData_size, SteamVR_Input_Source.GetHandle(base.inputSource));
			if (analogActionData != 0)
			{
				Debug.LogError("<b>[SteamVR]</b> GetAnalogActionData error (" + base.fullPath + "): " + analogActionData.ToString() + " handle: " + base.handle);
			}
			base.updateTime = Time.realtimeSinceStartup;
			axis = new Vector3(actionData.x, actionData.y, actionData.z);
			delta = new Vector3(actionData.deltaX, actionData.deltaY, actionData.deltaZ);
			changed = false;
			if (active)
			{
				if (delta.magnitude > changeTolerance)
				{
					changed = true;
					base.changedTime = Time.realtimeSinceStartup + actionData.fUpdateTime;
					if (this.onChange != null)
					{
						this.onChange(vector3Action, base.inputSource, axis, delta);
					}
				}
				if (axis != Vector3.zero && this.onAxis != null)
				{
					this.onAxis(vector3Action, base.inputSource, axis, delta);
				}
				if (this.onUpdate != null)
				{
					this.onUpdate(vector3Action, base.inputSource, axis, delta);
				}
			}
			if (this.onActiveBindingChange != null && lastActiveBinding != activeBinding)
			{
				this.onActiveBindingChange(vector3Action, base.inputSource, activeBinding);
			}
			if (this.onActiveChange != null && lastActive != active)
			{
				this.onActiveChange(vector3Action, base.inputSource, activeBinding);
			}
		}
	}
}
