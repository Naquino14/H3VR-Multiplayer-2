// Decompiled with JetBrains decompiler
// Type: ErosionBrushPlugin.Noise
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace ErosionBrushPlugin
{
  public class Noise
  {
    public static int seed
    {
      get => Random.seed;
      set => Random.seed = value;
    }

    public static void NoiseIteration(
      Matrix heightsMatrix,
      Matrix cliffMatrix,
      Matrix sedimentsMatrix,
      float size,
      float amount,
      float uplift,
      float maxHeight)
    {
      Coord min = heightsMatrix.rect.Min;
      Coord max = heightsMatrix.rect.Max;
      for (int x1 = min.x; x1 < max.x; ++x1)
      {
        for (int z1 = min.z; z1 < max.z; ++z1)
        {
          float b = (Noise.Fractal(x1, z1, size) - (1f - uplift)) * amount;
          Matrix matrix;
          int x2;
          int z2;
          (matrix = heightsMatrix)[x2 = x1, z2 = z1] = matrix[x2, z2] + b / maxHeight;
          if (cliffMatrix != null)
          {
            float num = Mathf.Max(0.0f, b);
            cliffMatrix[x1, z1] = num * 0.1f;
          }
          if (sedimentsMatrix != null)
          {
            float num = Mathf.Max(0.0f, -b);
            sedimentsMatrix[x1, z1] = num * 0.1f;
          }
        }
      }
    }

    public static void NoiseIteration(
      Matrix heightMatrix,
      Matrix cliffMatrix,
      Matrix sedimentsMatrix,
      float size,
      float intensity = 1f,
      float detail = 0.5f,
      Vector2 offset = default (Vector2),
      int seed = 12345,
      float uplift = 0.5f,
      float maxHeight = 500f)
    {
      int num1 = (int) (4096.0 / (double) heightMatrix.rect.size.x);
      int num2 = ((int) offset.x + seed * 7) % 77777;
      int num3 = ((int) offset.y + seed * 3) % 73333;
      int num4 = 1;
      float num5 = size;
      for (int index = 0; index < 100; ++index)
      {
        num5 /= 2f;
        if ((double) num5 >= 1.0)
          ++num4;
        else
          break;
      }
      Coord min = heightMatrix.rect.Min;
      Coord max = heightMatrix.rect.Max;
      for (int x1 = min.x; x1 < max.x; ++x1)
      {
        for (int z1 = min.z; z1 < max.z; ++z1)
        {
          float num6 = 0.5f;
          float num7 = size * 10f;
          float num8 = 1f;
          for (int index = 0; index < num4; ++index)
          {
            float num9 = (float) (((double) Mathf.PerlinNoise((float) ((x1 + num2 + 1000 * (index + 1)) * num1) / (num7 + 1f), (float) ((z1 + num3 + 100 * index) * num1) / (num7 + 1f)) - 0.5) * (double) num8 + 0.5);
            num6 = (double) num9 <= 0.5 ? 2f * num9 * num6 : (float) (1.0 - 2.0 * (1.0 - (double) num6) * (1.0 - (double) num9));
            num7 *= 0.5f;
            num8 *= detail;
          }
          if ((double) num6 < 0.0)
            num6 = 0.0f;
          if ((double) num6 > 1.0)
            num6 = 1f;
          float num10 = (num6 - (1f - uplift)) * intensity;
          Matrix matrix;
          int x2;
          int z2;
          (matrix = heightMatrix)[x2 = x1, z2 = z1] = matrix[x2, z2] + num10 / maxHeight;
          if (cliffMatrix != null)
            cliffMatrix[x1, z1] = (double) num10 <= 0.0 ? 0.0f : num10 * 0.1f;
          if (sedimentsMatrix != null)
            sedimentsMatrix[x1, z1] = (double) num10 >= 0.0 ? 0.0f : (float) (-(double) num10 * 0.100000001490116);
        }
      }
    }

    public static float Fractal(int x, int z, float size, float detail = 0.5f)
    {
      float num1 = 0.5f;
      float num2 = size;
      float num3 = 1f;
      int num4 = 1;
      for (int index = 0; index < 100; ++index)
      {
        num2 /= 2f;
        if ((double) num2 >= 1.0)
          ++num4;
        else
          break;
      }
      float num5 = size;
      for (int index = 0; index < num4; ++index)
      {
        float num6 = (float) (((double) Mathf.PerlinNoise((float) x / (num5 + 1f), (float) z / (num5 + 1f)) - 0.5) * (double) num3 + 0.5);
        num1 = (double) num6 <= 0.5 ? 2f * num6 * num1 : (float) (1.0 - 2.0 * (1.0 - (double) num1) * (1.0 - (double) num6));
        num5 *= 0.5f;
        num3 *= detail;
      }
      if ((double) num1 < 0.0)
        num1 = 0.0f;
      if ((double) num1 > 1.0)
        num1 = 1f;
      return num1;
    }
  }
}
