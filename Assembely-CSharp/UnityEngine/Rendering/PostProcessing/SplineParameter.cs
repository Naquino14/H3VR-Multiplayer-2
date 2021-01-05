using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	public sealed class SplineParameter : ParameterOverride<Spline>
	{
		public override void Interp(Spline from, Spline to, float t)
		{
			int renderedFrameCount = Time.renderedFrameCount;
			from.Cache(renderedFrameCount);
			to.Cache(renderedFrameCount);
			for (int i = 0; i < 128; i++)
			{
				float num = from.cachedData[i];
				float num2 = to.cachedData[i];
				value.cachedData[i] = num + (num2 - num) * t;
			}
		}
	}
}
