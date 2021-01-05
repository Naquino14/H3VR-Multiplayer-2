using System;
using UnityEngine;

namespace LIV.SDK.Unity
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true)]
	public class HelpAttribute : PropertyAttribute
	{
		public readonly string text;

		public HelpAttribute(string text)
		{
			this.text = text;
		}
	}
}
