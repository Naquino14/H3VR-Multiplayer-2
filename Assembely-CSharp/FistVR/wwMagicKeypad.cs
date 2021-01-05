using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace FistVR
{
	public class wwMagicKeypad : MonoBehaviour
	{
		[Serializable]
		public class KeyCodeReward
		{
			public string Code;

			public ItemSpawnerID[] Rewards;

			public int[] SpawnPointID;
		}

		public KeyCodeDisplay KeyDisplay;

		private string m_curInput = string.Empty;

		public Transform[] SpawnPoints;

		public GameObject s;

		public KeyCodeReward[] RewardCodes;

		private SHA512 shaaaaaaaaaaaaaaaaaaaa;

		private void Start()
		{
			shaaaaaaaaaaaaaaaaaaaa = SHA512.Create();
		}

		private void Update()
		{
			if (KeyDisplay.MyText != m_curInput)
			{
				m_curInput = KeyDisplay.MyText;
				EvaluateCode();
			}
		}

		private void EvaluateCode()
		{
			byte[] bytes = Encoding.UTF8.GetBytes(s.name + m_curInput);
			byte[] array = shaaaaaaaaaaaaaaaaaaaa.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			string strA = stringBuilder.ToString();
			for (int j = 0; j < RewardCodes.Length; j++)
			{
				if (string.CompareOrdinal(strA, RewardCodes[j].Code) == 0)
				{
					UnlockAndSpawn(RewardCodes[j]);
				}
			}
		}

		private void UnlockAndSpawn(KeyCodeReward key)
		{
			for (int i = 0; i < key.Rewards.Length; i++)
			{
				if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(key.Rewards[i]))
				{
					GM.Rewards.RewardUnlocks.UnlockReward(key.Rewards[i]);
					GM.Rewards.SaveToFile();
				}
				UnityEngine.Object.Instantiate(key.Rewards[i].MainObject.GetGameObject(), SpawnPoints[key.SpawnPointID[i]].position, SpawnPoints[key.SpawnPointID[i]].rotation);
			}
		}
	}
}
