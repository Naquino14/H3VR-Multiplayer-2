using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	public sealed class Vector2Parameter : ParameterOverride<Vector2>
	{
		public override void Interp(Vector2 from, Vector2 to, float t)
		{
			value.x = from.x + (to.x - from.x) * t;
			value.y = from.y + (to.y - from.y) * t;
		}
	}
}
