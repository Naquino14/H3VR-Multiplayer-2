// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.HaltonSeq
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public static class HaltonSeq
  {
    public static float Get(int index, int radix)
    {
      float num1 = 0.0f;
      float num2 = 1f / (float) radix;
      while (index > 0)
      {
        num1 += (float) (index % radix) * num2;
        index /= radix;
        num2 /= (float) radix;
      }
      return num1;
    }
  }
}
