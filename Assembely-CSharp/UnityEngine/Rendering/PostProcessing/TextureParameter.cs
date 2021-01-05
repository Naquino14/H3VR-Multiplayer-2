using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	public sealed class TextureParameter : ParameterOverride<Texture>
	{
		public override void Interp(Texture from, Texture to, float t)
		{
			if (from == null || to == null)
			{
				base.Interp(from, to, t);
			}
			else
			{
				value = TextureLerper.instance.Lerp(from, to, t);
			}
		}
	}
}
