using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Action_Single_Source : SteamVR_Action_In_Source, ISteamVR_Action_Single, ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		protected static uint actionData_size;

		public float changeTolerance = Mathf.Epsilon;

		protected InputAnalogActionData_t actionData = default(InputAnalogActionData_t);

		protected InputAnalogActionData_t lastActionData = default(InputAnalogActionData_t);

		protected SteamVR_Action_Single singleAction;

		public float axis
		{
			get
			{
				if (active)
				{
					return actionData.x;
				}
				return 0f;
			}
		}

		public float lastAxis
		{
			get
			{
				if (active)
				{
					return lastActionData.x;
				}
				return 0f;
			}
		}

		public float delta
		{
			get
			{
				if (active)
				{
					return actionData.deltaX;
				}
				return 0f;
			}
		}

		public float lastDelta
		{
			get
			{
				if (active)
				{
					return lastActionData.deltaX;
				}
				return 0f;
			}
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

		public event SteamVR_Action_Single.AxisHandler onAxis;

		public event SteamVR_Action_Single.ActiveChangeHandler onActiveChange;

		public event SteamVR_Action_Single.ActiveChangeHandler onActiveBindingChange;

		public event SteamVR_Action_Single.ChangeHandler onChange;

		public event SteamVR_Action_Single.UpdateHandler onUpdate;

		public override void Preinitialize(SteamVR_Action wrappingAction, SteamVR_Input_Sources forInputSource)
		{
			base.Preinitialize(wrappingAction, forInputSource);
			singleAction = (SteamVR_Action_Single)wrappingAction;
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
			EVRInputError analogActionData = OpenVR.Input.GetAnalogActionData(base.handle, ref actionData, actionData_size, SteamVR_Input_Source.GetHandle(base.inputSource));
			if (analogActionData != 0)
			{
				Debug.LogError("<b>[SteamVR]</b> GetAnalogActionData error (" + base.fullPath + "): " + analogActionData.ToString() + " handle: " + base.handle);
			}
			base.updateTime = Time.realtimeSinceStartup;
			changed = false;
			if (active)
			{
				if (delta > changeTolerance || delta < 0f - changeTolerance)
				{
					changed = true;
					base.changedTime = Time.realtimeSinceStartup + actionData.fUpdateTime;
					if (this.onChange != null)
					{
						this.onChange(singleAction, base.inputSource, axis, delta);
					}
				}
				if (axis != 0f && this.onAxis != null)
				{
					this.onAxis(singleAction, base.inputSource, axis, delta);
				}
				if (this.onUpdate != null)
				{
					this.onUpdate(singleAction, base.inputSource, axis, delta);
				}
			}
			if (this.onActiveBindingChange != null && lastActiveBinding != activeBinding)
			{
				this.onActiveBindingChange(singleAction, base.inputSource, activeBinding);
			}
			if (this.onActiveChange != null && lastActive != active)
			{
				this.onActiveChange(singleAction, base.inputSource, activeBinding);
			}
		}
	}
}
