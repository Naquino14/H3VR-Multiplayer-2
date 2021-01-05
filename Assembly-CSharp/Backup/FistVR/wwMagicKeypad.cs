// Decompiled with JetBrains decompiler
// Type: FistVR.wwMagicKeypad
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace FistVR
{
  public class wwMagicKeypad : MonoBehaviour
  {
    public KeyCodeDisplay KeyDisplay;
    private string m_curInput = string.Empty;
    public Transform[] SpawnPoints;
    public GameObject s;
    public wwMagicKeypad.KeyCodeReward[] RewardCodes;
    private SHA512 shaaaaaaaaaaaaaaaaaaaa;

    private void Start() => this.shaaaaaaaaaaaaaaaaaaaa = SHA512.Create();

    private void Update()
    {
      if (!(this.KeyDisplay.MyText != this.m_curInput))
        return;
      this.m_curInput = this.KeyDisplay.MyText;
      this.EvaluateCode();
    }

    private void EvaluateCode()
    {
      byte[] hash = this.shaaaaaaaaaaaaaaaaaaaa.ComputeHash(Encoding.UTF8.GetBytes(this.s.name + this.m_curInput));
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in hash)
        stringBuilder.Append(num.ToString("x2"));
      string strA = stringBuilder.ToString();
      for (int index = 0; index < this.RewardCodes.Length; ++index)
      {
        if (string.CompareOrdinal(strA, this.RewardCodes[index].Code) == 0)
          this.UnlockAndSpawn(this.RewardCodes[index]);
      }
    }

    private void UnlockAndSpawn(wwMagicKeypad.KeyCodeReward key)
    {
      for (int index = 0; index < key.Rewards.Length; ++index)
      {
        if (!GM.Rewards.RewardUnlocks.IsRewardUnlocked(key.Rewards[index]))
        {
          GM.Rewards.RewardUnlocks.UnlockReward(key.Rewards[index]);
          GM.Rewards.SaveToFile();
        }
        UnityEngine.Object.Instantiate<GameObject>(key.Rewards[index].MainObject.GetGameObject(), this.SpawnPoints[key.SpawnPointID[index]].position, this.SpawnPoints[key.SpawnPointID[index]].rotation);
      }
    }

    [Serializable]
    public class KeyCodeReward
    {
      public string Code;
      public ItemSpawnerID[] Rewards;
      public int[] SpawnPointID;
    }
  }
}
