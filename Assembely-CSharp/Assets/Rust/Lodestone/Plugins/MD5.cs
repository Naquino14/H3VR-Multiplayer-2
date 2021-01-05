using System;
using System.Security.Cryptography;
using System.Text;

namespace Assets.Rust.Lodestone.Plugins
{
	public static class MD5
	{
		public static string Sum(string strToHash)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(strToHash);
			byte[] array = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
			return BitConverter.ToString(array).Replace("-", string.Empty).ToLower();
		}
	}
}
