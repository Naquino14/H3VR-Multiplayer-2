namespace FistVR
{
	public class GPTestingPlaceable : GPPlaceable
	{
		public enum GPColor
		{
			Red,
			Blue,
			Green,
			Yellow
		}

		public GPColor GPC;

		public override void Init(GPSceneMode mode)
		{
			base.Init(mode);
			Flags.Add("Color", "Red");
		}

		private void UpdateDisplayColor()
		{
		}
	}
}
