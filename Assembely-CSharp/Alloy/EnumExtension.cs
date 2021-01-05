using System;

namespace Alloy
{
	public static class EnumExtension
	{
		public static bool HasFlag(this Enum keys, Enum flag)
		{
			int num = Convert.ToInt32(keys);
			int num2 = Convert.ToInt32(flag);
			return (num & num2) == num2;
		}
	}
}
