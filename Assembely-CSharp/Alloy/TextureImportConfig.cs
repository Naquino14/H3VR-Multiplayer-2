using System;
using UnityEngine;

namespace Alloy
{
	[Serializable]
	public class TextureImportConfig
	{
		public bool IsLinear;

		public FilterMode Filter = FilterMode.Trilinear;

		public bool DefaultCompressed;
	}
}
