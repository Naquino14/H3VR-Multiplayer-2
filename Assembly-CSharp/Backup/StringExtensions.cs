// Decompiled with JetBrains decompiler
// Type: StringExtensions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

public static class StringExtensions
{
  public static int GetDeterministicHashCode(this string str)
  {
    int num1 = 352654597;
    int num2 = num1;
    for (int index = 0; index < str.Length; index += 2)
    {
      num1 = (num1 << 5) + num1 ^ (int) str[index];
      if (index != str.Length - 1)
        num2 = (num2 << 5) + num2 ^ (int) str[index + 1];
      else
        break;
    }
    return num1 + num2 * 1566083941;
  }
}
