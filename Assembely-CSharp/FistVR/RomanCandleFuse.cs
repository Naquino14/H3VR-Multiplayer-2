namespace FistVR
{
	public class RomanCandleFuse : FVRFuse
	{
		public RomanCandle Candle;

		public override void Boom()
		{
			if (!hasBoomed)
			{
				hasBoomed = true;
				Candle.Ignite();
				FuseFire.SetActive(value: false);
			}
		}
	}
}
