using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Magic Keypad Code", menuName = "MagicKeypad/Code", order = 0)]
	public class wwMagicKeypadHash : ScriptableObject
	{
		[Serializable]
		public struct HashHolder
		{
			public string Code;

			public string HashedCode;
		}

		public string SaltyBitch;

		public HashHolder[] Hashes;

		[ContextMenu("Calc")]
		public void UpdateHashes()
		{
			SHA512 sHA = SHA512.Create();
			for (int i = 0; i < Hashes.Length; i++)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(SaltyBitch + Hashes[i].Code);
				byte[] array = sHA.ComputeHash(bytes);
				StringBuilder stringBuilder = new StringBuilder();
				byte[] array2 = array;
				foreach (byte b in array2)
				{
					stringBuilder.Append(b.ToString("x2"));
				}
				Hashes[i].HashedCode = stringBuilder.ToString();
			}
		}
	}
}
