namespace FistVR
{
	public class SpyClockingSystem : SosigWearable
	{
		private void Update()
		{
			if (S != null && S.BodyState == Sosig.SosigBodyState.InControl && S.CurrentOrder != Sosig.SosigOrder.Skirmish && S.CurrentOrder != 0)
			{
				S.BuffHealing_Invis(0.1f);
			}
		}
	}
}
