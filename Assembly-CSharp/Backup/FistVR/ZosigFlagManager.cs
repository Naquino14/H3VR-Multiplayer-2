// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigFlagManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigFlagManager : MonoBehaviour
  {
    private Dictionary<string, int> m_flags = new Dictionary<string, int>();
    public List<string> ResetFlagsOnGameStart = new List<string>();
    public List<ZosigFlagManager.TestingFlagPreset> TestingStartFlags = new List<ZosigFlagManager.TestingFlagPreset>();
    public bool DebugPrintEnabled = true;
    private int curSaveGame;

    public bool ContainsKey(string key) => this.m_flags.ContainsKey(key);

    public int GetNumEntries() => this.m_flags.Count;

    public void Init()
    {
      this.curSaveGame = GM.ROTRWSaves.CurrentSaveGame;
      if (this.curSaveGame > 0)
      {
        this.m_flags.Clear();
        switch (this.curSaveGame)
        {
          case 1:
            using (Dictionary<string, int>.Enumerator enumerator = GM.ROTRWSaves.SaveGame1.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<string, int> current = enumerator.Current;
                this.m_flags.Add(current.Key, current.Value);
              }
              break;
            }
          case 2:
            using (Dictionary<string, int>.Enumerator enumerator = GM.ROTRWSaves.SaveGame2.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<string, int> current = enumerator.Current;
                this.m_flags.Add(current.Key, current.Value);
              }
              break;
            }
          case 3:
            using (Dictionary<string, int>.Enumerator enumerator = GM.ROTRWSaves.SaveGame3.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<string, int> current = enumerator.Current;
                this.m_flags.Add(current.Key, current.Value);
              }
              break;
            }
        }
      }
      for (int index = 0; index < this.ResetFlagsOnGameStart.Count; ++index)
        this.SetFlag(this.ResetFlagsOnGameStart[index], false);
      for (int index = 0; index < this.TestingStartFlags.Count; ++index)
        this.SetFlag(this.TestingStartFlags[index].Flag, this.TestingStartFlags[index].Value);
    }

    public void OnDestroy()
    {
      if (this.curSaveGame <= 0)
        return;
      this.PlayerDeathSaveToSaveFile();
    }

    public void PrintAll()
    {
      foreach (KeyValuePair<string, int> flag in this.m_flags)
        Debug.Log((object) (flag.Key + " " + (object) flag.Value));
    }

    public void Save()
    {
      GM.ROTRWSaves.WriteToSaveFromFlagM(this.m_flags);
      GM.ROTRWSaves.SaveToFile();
    }

    public void PlayerDeathSaveToSaveFile()
    {
      GM.ROTRWSaves.WriteToSaveFromFlagM(this.m_flags);
      GM.ROTRWSaves.SaveToFile();
    }

    public void SetFlag(string flag, bool val)
    {
      if (flag == string.Empty)
        return;
      if (this.m_flags.ContainsKey(flag))
      {
        if (val)
          return;
        this.m_flags.Remove(flag);
        if (!this.DebugPrintEnabled)
          return;
        Debug.Log((object) ("Removing Flag: " + flag));
      }
      else
      {
        if (!val)
          return;
        this.m_flags.Add(flag, 1);
        if (!this.DebugPrintEnabled)
          return;
        Debug.Log((object) ("Adding Flag: " + flag + " Set To: " + (object) 1));
      }
    }

    public void SetFlag(string flag, int val)
    {
      if (flag == string.Empty)
        return;
      if (this.m_flags.ContainsKey(flag))
      {
        if (val == 0)
        {
          this.m_flags.Remove(flag);
          if (!this.DebugPrintEnabled)
            return;
          Debug.Log((object) ("Removing Flag: " + flag));
        }
        else
        {
          this.m_flags[flag] = val;
          if (!this.DebugPrintEnabled)
            return;
          Debug.Log((object) ("Setting Flag: " + flag + " Set To: " + (object) val));
        }
      }
      else
      {
        if (val == 0)
          return;
        this.m_flags.Add(flag, val);
        if (!this.DebugPrintEnabled)
          return;
        Debug.Log((object) ("Adding Flag: " + flag + " Set To: " + (object) val));
      }
    }

    public void AddToFlag(string flag, int val)
    {
      if (flag == string.Empty)
        return;
      if (this.m_flags.ContainsKey(flag))
      {
        Dictionary<string, int> flags;
        string key;
        (flags = this.m_flags)[key = flag] = flags[key] + 1;
      }
      else
        this.m_flags.Add(flag, val);
    }

    public bool SubstractFromFlag(string flag, int val)
    {
      if (flag == string.Empty || !this.m_flags.ContainsKey(flag) || this.m_flags[flag] < val)
        return false;
      Dictionary<string, int> flags;
      string key;
      (flags = this.m_flags)[key = flag] = flags[key] - val;
      return true;
    }

    public void SetFlagMaxBlend(string flag, int val)
    {
      if (flag == string.Empty)
        return;
      if (this.m_flags.ContainsKey(flag))
      {
        if (val == 0)
        {
          this.m_flags.Remove(flag);
          if (!this.DebugPrintEnabled)
            return;
          Debug.Log((object) ("Removing Flag: " + flag));
        }
        else
        {
          int num = Mathf.Max(this.m_flags[flag], val);
          this.m_flags[flag] = num;
          if (!this.DebugPrintEnabled)
            return;
          Debug.Log((object) ("Setting Flag: " + flag + " Set To: " + (object) num));
        }
      }
      else
      {
        if (val == 0)
          return;
        this.m_flags.Add(flag, val);
        if (!this.DebugPrintEnabled)
          return;
        Debug.Log((object) ("Adding Flag: " + flag + " Set To: " + (object) val));
      }
    }

    public bool IsFlag(string flag) => flag == string.Empty || this.m_flags.ContainsKey(flag);

    public int GetFlagValue(string flag)
    {
      if (flag == string.Empty)
        return 1;
      return this.m_flags.ContainsKey(flag) ? this.m_flags[flag] : 0;
    }

    [Serializable]
    public class TestingFlagPreset
    {
      public string Flag;
      public int Value;
    }
  }
}
