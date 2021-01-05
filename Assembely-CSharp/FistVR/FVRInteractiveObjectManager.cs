namespace FistVR
{
	public class FVRInteractiveObjectManager : ManagerSingleton<FVRInteractiveObjectManager>
	{
		private int executeFrameTick;

		protected override void Awake()
		{
			base.Awake();
		}

		public void Update()
		{
			executeFrameTick = 0;
			FVRInteractiveObject.GlobalUpdate();
		}

		public void FixedUpdate()
		{
			if (executeFrameTick < 2)
			{
				executeFrameTick++;
				FVRInteractiveObject.GlobalFixedUpdate();
			}
		}
	}
}
