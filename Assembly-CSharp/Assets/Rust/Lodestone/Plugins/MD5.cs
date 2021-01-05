// Decompiled with JetBrains decompiler
// Type: Assets.Rust.Lodestone.Plugins.MD5
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Text;

namespace Assets.Rust.Lodestone.Plugins
{
  public static class MD5
  {
    public static string Sum(string strToHash)
    {
      byte[] bytes = Encoding.ASCII.GetBytes(strToHash);
      return BitConverter.ToString(System.Security.Cryptography.MD5.Create().ComputeHash(bytes)).Replace("-", string.Empty).ToLower();
    }
  }
}
