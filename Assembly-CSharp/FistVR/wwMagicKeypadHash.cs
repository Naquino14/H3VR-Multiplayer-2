// Decompiled with JetBrains decompiler
// Type: FistVR.wwMagicKeypadHash
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Magic Keypad Code", menuName = "MagicKeypad/Code", order = 0)]
  public class wwMagicKeypadHash : ScriptableObject
  {
    public string SaltyBitch;
    public wwMagicKeypadHash.HashHolder[] Hashes;

    [ContextMenu("Calc")]
    public void UpdateHashes()
    {
      SHA512 shA512 = SHA512.Create();
      for (int index = 0; index < this.Hashes.Length; ++index)
      {
        byte[] bytes = Encoding.UTF8.GetBytes(this.SaltyBitch + this.Hashes[index].Code);
        byte[] hash = shA512.ComputeHash(bytes);
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte num in hash)
          stringBuilder.Append(num.ToString("x2"));
        this.Hashes[index].HashedCode = stringBuilder.ToString();
      }
    }

    [Serializable]
    public struct HashHolder
    {
      public string Code;
      public string HashedCode;
    }
  }
}
