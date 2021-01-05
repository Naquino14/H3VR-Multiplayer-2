using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigFlagManager : MonoBehaviour
	{
		[Serializable]
		public class TestingFlagPreset
		{
			public string Flag;

			public int Value;
		}

		private Dictionary<string, int> m_flags = new Dictionary<string, int>();

		public List<string> ResetFlagsOnGameStart = new List<string>();

		public List<TestingFlagPreset> TestingStartFlags = new List<TestingFlagPreset>();

		public bool DebugPrintEnabled = true;

		private int curSaveGame;

		public bool ContainsKey(string key)
		{
			return m_flags.ContainsKey(key);
		}

		public int GetNumEntries()
		{
			return m_flags.Count;
		}

		public void Init()
		{
			curSaveGame = GM.ROTRWSaves.CurrentSaveGame;
			if (curSaveGame > 0)
			{
				m_flags.Clear();
				switch (curSaveGame)
				{
				case 1:
					foreach (KeyValuePair<string, int> item in GM.ROTRWSaves.SaveGame1)
					{
						m_flags.Add(item.Key, item.Value);
					}
					break;
				case 2:
					foreach (KeyValuePair<string, int> item2 in GM.ROTRWSaves.SaveGame2)
					{
						m_flags.Add(item2.Key, item2.Value);
					}
					break;
				case 3:
					foreach (KeyValuePair<string, int> item3 in GM.ROTRWSaves.SaveGame3)
					{
						m_flags.Add(item3.Key, item3.Value);
					}
					break;
				}
			}
			for (int i = 0; i < ResetFlagsOnGameStart.Count; i++)
			{
				SetFlag(ResetFlagsOnGameStart[i], val: false);
			}
			for (int j = 0; j < TestingStartFlags.Count; j++)
			{
				SetFlag(TestingStartFlags[j].Flag, TestingStartFlags[j].Value);
			}
		}

		public void OnDestroy()
		{
			if (curSaveGame > 0)
			{
				PlayerDeathSaveToSaveFile();
			}
		}

		public void PrintAll()
		{
			foreach (KeyValuePair<string, int> flag in m_flags)
			{
				Debug.Log(flag.Key + " " + flag.Value);
			}
		}

		public void Save()
		{
			GM.ROTRWSaves.WriteToSaveFromFlagM(m_flags);
			GM.ROTRWSaves.SaveToFile();
		}

		public void PlayerDeathSaveToSaveFile()
		{
			GM.ROTRWSaves.WriteToSaveFromFlagM(m_flags);
			GM.ROTRWSaves.SaveToFile();
		}

		public void SetFlag(string flag, bool val)
		{
			if (flag == string.Empty)
			{
				return;
			}
			if (m_flags.ContainsKey(flag))
			{
				if (!val)
				{
					m_flags.Remove(flag);
					if (DebugPrintEnabled)
					{
						Debug.Log("Removing Flag: " + flag);
					}
				}
			}
			else if (val)
			{
				m_flags.Add(flag, 1);
				if (DebugPrintEnabled)
				{
					Debug.Log("Adding Flag: " + flag + " Set To: " + 1);
				}
			}
		}

		public void SetFlag(string flag, int val)
		{
			if (flag == string.Empty)
			{
				return;
			}
			if (m_flags.ContainsKey(flag))
			{
				if (val == 0)
				{
					m_flags.Remove(flag);
					if (DebugPrintEnabled)
					{
						Debug.Log("Removing Flag: " + flag);
					}
					return;
				}
				m_flags[flag] = val;
				if (DebugPrintEnabled)
				{
					Debug.Log("Setting Flag: " + flag + " Set To: " + val);
				}
			}
			else if (val != 0)
			{
				m_flags.Add(flag, val);
				if (DebugPrintEnabled)
				{
					Debug.Log("Adding Flag: " + flag + " Set To: " + val);
				}
			}
		}

		public void AddToFlag(string flag, int val)
		{
			if (!(flag == string.Empty))
			{
				if (m_flags.ContainsKey(flag))
				{
					m_flags[flag]++;
				}
				else
				{
					m_flags.Add(flag, val);
				}
			}
		}

		public bool SubstractFromFlag(string flag, int val)
		{
			if (flag == string.Empty)
			{
				return false;
			}
			if (m_flags.ContainsKey(flag) && m_flags[flag] >= val)
			{
				m_flags[flag] -= val;
				return true;
			}
			return false;
		}

		public void SetFlagMaxBlend(string flag, int val)
		{
			if (flag == string.Empty)
			{
				return;
			}
			if (m_flags.ContainsKey(flag))
			{
				if (val == 0)
				{
					m_flags.Remove(flag);
					if (DebugPrintEnabled)
					{
						Debug.Log("Removing Flag: " + flag);
					}
					return;
				}
				int num = Mathf.Max(m_flags[flag], val);
				m_flags[flag] = num;
				if (DebugPrintEnabled)
				{
					Debug.Log("Setting Flag: " + flag + " Set To: " + num);
				}
			}
			else if (val != 0)
			{
				m_flags.Add(flag, val);
				if (DebugPrintEnabled)
				{
					Debug.Log("Adding Flag: " + flag + " Set To: " + val);
				}
			}
		}

		public bool IsFlag(string flag)
		{
			if (flag == string.Empty)
			{
				return true;
			}
			if (m_flags.ContainsKey(flag))
			{
				return true;
			}
			return false;
		}

		public int GetFlagValue(string flag)
		{
			if (flag == string.Empty)
			{
				return 1;
			}
			if (m_flags.ContainsKey(flag))
			{
				return m_flags[flag];
			}
			return 0;
		}
	}
}
