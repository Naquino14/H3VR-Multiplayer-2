namespace Valve.VR
{
	public abstract class SteamVR_Action_Source : ISteamVR_Action_Source
	{
		protected ulong inputSourceHandle;

		protected SteamVR_Action action;

		public string fullPath => action.fullPath;

		public ulong handle => action.handle;

		public SteamVR_ActionSet actionSet => action.actionSet;

		public SteamVR_ActionDirections direction => action.direction;

		public SteamVR_Input_Sources inputSource
		{
			get;
			protected set;
		}

		public bool setActive => actionSet.IsActive(inputSource);

		public abstract bool active
		{
			get;
		}

		public abstract bool activeBinding
		{
			get;
		}

		public abstract bool lastActive
		{
			get;
			protected set;
		}

		public abstract bool lastActiveBinding
		{
			get;
		}

		public SteamVR_Action_Source()
		{
		}

		public virtual void Preinitialize(SteamVR_Action wrappingAction, SteamVR_Input_Sources forInputSource)
		{
			action = wrappingAction;
			inputSource = forInputSource;
		}

		public virtual void Initialize()
		{
			inputSourceHandle = SteamVR_Input_Source.GetHandle(inputSource);
		}
	}
}
