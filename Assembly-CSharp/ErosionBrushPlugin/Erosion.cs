// Decompiled with JetBrains decompiler
// Type: ErosionBrushPlugin.Erosion
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ErosionBrushPlugin
{
  public static class Erosion
  {
    public static void ErosionIteration(
      Matrix heights,
      Matrix erosion,
      Matrix sedimentSum,
      CoordRect area = default (CoordRect),
      float erosionDurability = 0.9f,
      float erosionAmount = 1f,
      float sedimentAmount = 0.5f,
      int erosionFluidityIterations = 3,
      float ruffle = 0.1f,
      Matrix torrents = null,
      Matrix sediments = null,
      int[] stepsArray = null,
      int[] heightsInt = null,
      int[] order = null)
    {
      if (area.isZero)
        area = heights.rect;
      int count = heights.count;
      int num1 = 12345;
      int num2 = 1000000;
      if (heightsInt == null)
        heightsInt = new int[count];
      for (int index = 0; index < heights.count; ++index)
        heightsInt[index] = (int) ((double) Mathf.Clamp01(heights.array[index]) * (double) num2);
      if (order == null)
        order = new int[count];
      order = Extensions.ArrayOrder(heightsInt, order, heights.count, stepsArray: stepsArray);
      for (int index = 0; index < heights.count; ++index)
      {
        int num3 = order[index];
        Coord coord = heights.rect.CoordByNum(num3);
        if (!area.CheckInRangeAndBounds(coord))
          order[index] = -1;
      }
      if (torrents == null)
        torrents = new Matrix(heights.rect);
      torrents.ChangeRect(heights.rect);
      torrents.Fill(1f);
      for (int index1 = count - 1; index1 >= 0; --index1)
      {
        int num3 = order[index1];
        if (num3 >= 0)
        {
          float[] array1 = heights.array;
          int index2 = num3;
          int x = heights.rect.size.x;
          float num4 = array1[index2];
          float num5 = array1[index2 - 1];
          float num6 = array1[index2 + 1];
          float num7 = array1[index2 - x];
          float num8 = array1[index2 + x];
          float num9 = array1[index2 - 1 - x];
          float num10 = array1[index2 + 1 - x];
          float num11 = array1[index2 - 1 + x];
          float num12 = array1[index2 + 1 + x];
          float num13 = num4 - num4;
          float num14 = num4 - num5;
          float num15 = num4 - num6;
          float num16 = num4 - num7;
          float num17 = num4 - num8;
          float num18 = num4 - num9;
          float num19 = num4 - num10;
          float num20 = num4 - num11;
          float num21 = num4 - num12;
          float num22 = (double) num13 <= 0.0 ? 0.0f : num13;
          float num23 = (double) num14 <= 0.0 ? 0.0f : num14;
          float num24 = (double) num15 <= 0.0 ? 0.0f : num15;
          float num25 = (double) num16 <= 0.0 ? 0.0f : num16;
          float num26 = (double) num17 <= 0.0 ? 0.0f : num17;
          float num27 = (double) num18 <= 0.0 ? 0.0f : num18;
          float num28 = (double) num19 <= 0.0 ? 0.0f : num19;
          float num29 = (double) num20 <= 0.0 ? 0.0f : num20;
          float num30 = (double) num21 <= 0.0 ? 0.0f : num21;
          float num31 = 0.0f;
          float num32 = 0.0f;
          float num33 = 0.0f;
          float num34 = 0.0f;
          float num35 = 0.0f;
          float num36 = 0.0f;
          float num37 = 0.0f;
          float num38 = 0.0f;
          float num39 = 0.0f;
          float num40 = num22 + num23 + num24 + num25 + num26 + num27 + num28 + num29 + num30;
          if ((double) num40 > 9.99999974737875E-06)
          {
            num31 = num22 / num40;
            num32 = num23 / num40;
            num33 = num24 / num40;
            num34 = num25 / num40;
            num35 = num26 / num40;
            num36 = num27 / num40;
            num37 = num28 / num40;
            num38 = num29 / num40;
            num39 = num30 / num40;
          }
          float num41 = torrents.array[index2];
          if ((double) num41 > 2000000000.0)
            num41 = 2E+09f;
          float[] array2 = torrents.array;
          array2[index2] += num41 * num31;
          array2[index2 - 1] += num41 * num32;
          array2[index2 + 1] += num41 * num33;
          array2[index2 - x] += num41 * num34;
          array2[index2 + x] += num41 * num35;
          array2[index2 - 1 - x] += num41 * num36;
          array2[index2 + 1 - x] += num41 * num37;
          array2[index2 - 1 + x] += num41 * num38;
          array2[index2 + 1 + x] += num41 * num39;
        }
      }
      if (sediments == null)
        sediments = new Matrix(heights.rect);
      else
        sediments.ChangeRect(heights.rect);
      sediments.Clear();
      for (int index1 = count - 1; index1 >= 0; --index1)
      {
        int index2 = order[index1];
        if (index2 >= 0)
        {
          float[] array = heights.array;
          int index3 = index2;
          int x = heights.rect.size.x;
          float num3 = array[index3];
          float num4 = array[index3 - 1];
          float num5 = array[index3 + 1];
          float num6 = array[index3 - x];
          float num7 = array[index3 + x];
          float num8 = num3;
          if ((double) num4 < (double) num8)
            num8 = num4;
          if ((double) num5 < (double) num8)
            num8 = num5;
          if ((double) num6 < (double) num8)
            num8 = num6;
          if ((double) num7 < (double) num8)
            num8 = num7;
          float num9 = (float) (((double) num3 + (double) num8) / 2.0);
          if ((double) num3 >= (double) num9)
          {
            float num10 = num3 - num9;
            float num11 = (float) ((double) num10 * ((double) torrents.array[index2] - 1.0) * (1.0 - (double) erosionDurability));
            if ((double) num10 > (double) num11)
              num10 = num11;
            float num12 = num10 * erosionAmount;
            heights.array[index2] -= num12;
            sediments.array[index2] += num12 * sedimentAmount;
            if (erosion != null)
              erosion.array[index2] += num12;
          }
        }
      }
      for (int index1 = 0; index1 < erosionFluidityIterations; ++index1)
      {
        for (int index2 = count - 1; index2 >= 0; --index2)
        {
          int index3 = order[index2];
          if (index3 >= 0)
          {
            float[] array1 = heights.array;
            int x = heights.rect.size.x;
            float num3 = array1[index3];
            float num4 = array1[index3 - 1];
            float num5 = array1[index3 + 1];
            float num6 = array1[index3 - x];
            float num7 = array1[index3 + x];
            float[] array2 = sediments.array;
            float num8 = array2[index3] + array2[index3 - 1] + array2[index3 + 1] + array2[index3 - x] + array2[index3 + x];
            if ((double) num8 >= 9.99999974737875E-06)
            {
              float num9 = num8 / 5f;
              float num10 = num9;
              float num11 = num9;
              float num12 = num9;
              float num13 = num9;
              float num14 = num9;
              float num15 = (float) (((double) num3 + (double) num10 + (double) num11 + (double) num4) / 2.0);
              float num16;
              float num17;
              if ((double) num3 + (double) num10 > (double) num4 + (double) num11)
              {
                float num18 = num10 + num3 - num15;
                if ((double) num18 > (double) num10)
                  num18 = num10;
                num16 = num10 - num18;
                num17 = num11 + num18;
              }
              else
              {
                float num18 = num11 + num4 - num15;
                if ((double) num18 > (double) num11)
                  num18 = num11;
                num17 = num11 - num18;
                num16 = num10 + num18;
              }
              float num19 = (float) (((double) num4 + (double) num17 + (double) num12 + (double) num5) / 2.0);
              float num20;
              float num21;
              if ((double) num4 + (double) num17 > (double) num5 + (double) num12)
              {
                float num18 = num17 + num4 - num19;
                if ((double) num18 > (double) num17)
                  num18 = num17;
                num20 = num17 - num18;
                num21 = num12 + num18;
              }
              else
              {
                float num18 = num12 + num5 - num19;
                if ((double) num18 > (double) num12)
                  num18 = num12;
                num21 = num12 - num18;
                num20 = num17 + num18;
              }
              float num22 = (float) (((double) num3 + (double) num16 + (double) num21 + (double) num5) / 2.0);
              float num23;
              float num24;
              if ((double) num3 + (double) num16 > (double) num5 + (double) num21)
              {
                float num18 = num16 + num3 - num22;
                if ((double) num18 > (double) num16)
                  num18 = num16;
                num23 = num16 - num18;
                num24 = num21 + num18;
              }
              else
              {
                float num18 = num21 + num5 - num22;
                if ((double) num18 > (double) num21)
                  num18 = num21;
                num24 = num21 - num18;
                num23 = num16 + num18;
              }
              float num25 = (float) (((double) num3 + (double) num23 + (double) num13 + (double) num6) / 2.0);
              float num26;
              float num27;
              if ((double) num3 + (double) num23 > (double) num6 + (double) num13)
              {
                float num18 = num23 + num3 - num25;
                if ((double) num18 > (double) num23)
                  num18 = num23;
                num26 = num23 - num18;
                num27 = num13 + num18;
              }
              else
              {
                float num18 = num13 + num6 - num25;
                if ((double) num18 > (double) num13)
                  num18 = num13;
                num27 = num13 - num18;
                num26 = num23 + num18;
              }
              float num28 = (float) (((double) num7 + (double) num14 + (double) num27 + (double) num6) / 2.0);
              float num29;
              float num30;
              if ((double) num7 + (double) num14 > (double) num6 + (double) num27)
              {
                float num18 = num14 + num7 - num28;
                if ((double) num18 > (double) num14)
                  num18 = num14;
                num29 = num14 - num18;
                num30 = num27 + num18;
              }
              else
              {
                float num18 = num27 + num6 - num28;
                if ((double) num18 > (double) num27)
                  num18 = num27;
                num30 = num27 - num18;
                num29 = num14 + num18;
              }
              float num31 = (float) (((double) num3 + (double) num26 + (double) num30 + (double) num6) / 2.0);
              float num32;
              float num33;
              if ((double) num3 + (double) num26 > (double) num6 + (double) num30)
              {
                float num18 = num26 + num3 - num31;
                if ((double) num18 > (double) num26)
                  num18 = num26;
                num32 = num26 - num18;
                num33 = num30 + num18;
              }
              else
              {
                float num18 = num30 + num6 - num31;
                if ((double) num18 > (double) num30)
                  num18 = num30;
                num33 = num30 - num18;
                num32 = num26 + num18;
              }
              float num34 = (float) (((double) num4 + (double) num20 + (double) num33 + (double) num6) / 2.0);
              float num35;
              float num36;
              if ((double) num4 + (double) num20 > (double) num6 + (double) num33)
              {
                float num18 = num20 + num4 - num34;
                if ((double) num18 > (double) num20)
                  num18 = num20;
                num35 = num20 - num18;
                num36 = num33 + num18;
              }
              else
              {
                float num18 = num33 + num6 - num34;
                if ((double) num18 > (double) num33)
                  num18 = num33;
                num36 = num33 - num18;
                num35 = num20 + num18;
              }
              float num37 = (float) (((double) num5 + (double) num24 + (double) num29 + (double) num7) / 2.0);
              float num38;
              float num39;
              if ((double) num5 + (double) num24 > (double) num7 + (double) num29)
              {
                float num18 = num24 + num5 - num37;
                if ((double) num18 > (double) num24)
                  num18 = num24;
                num38 = num24 - num18;
                num39 = num29 + num18;
              }
              else
              {
                float num18 = num29 + num7 - num37;
                if ((double) num18 > (double) num29)
                  num18 = num29;
                num39 = num29 - num18;
                num38 = num24 + num18;
              }
              float num40 = (float) (((double) num4 + (double) num35 + (double) num39 + (double) num7) / 2.0);
              float num41;
              float num42;
              if ((double) num4 + (double) num35 > (double) num7 + (double) num39)
              {
                float num18 = num35 + num4 - num40;
                if ((double) num18 > (double) num35)
                  num18 = num35;
                num41 = num35 - num18;
                num42 = num39 + num18;
              }
              else
              {
                float num18 = num39 + num7 - num40;
                if ((double) num18 > (double) num39)
                  num18 = num39;
                num42 = num39 - num18;
                num41 = num35 + num18;
              }
              float num43 = (float) (((double) num5 + (double) num38 + (double) num36 + (double) num6) / 2.0);
              float num44;
              float num45;
              if ((double) num5 + (double) num38 > (double) num6 + (double) num36)
              {
                float num18 = num38 + num5 - num43;
                if ((double) num18 > (double) num38)
                  num18 = num38;
                num44 = num38 - num18;
                num45 = num36 + num18;
              }
              else
              {
                float num18 = num36 + num6 - num43;
                if ((double) num18 > (double) num36)
                  num18 = num36;
                num45 = num36 - num18;
                num44 = num38 + num18;
              }
              float[] array3 = sediments.array;
              array3[index3] = num32;
              array3[index3 - 1] = num41;
              array3[index3 + 1] = num44;
              array3[index3 - x] = num45;
              array3[index3 + x] = num42;
              if (sedimentSum != null)
              {
                float[] array4 = sedimentSum.array;
                array4[index3] += num32;
                array4[index3 - 1] += num41;
                array4[index3 + 1] += num44;
                array4[index3 - x] += num45;
                array4[index3 + x] += num42;
              }
            }
          }
        }
      }
      for (int index1 = count - 1; index1 >= 0; --index1)
      {
        heights.array[index1] += sediments.array[index1];
        num1 = 214013 * num1 + 2531011;
        float num3 = (float) (num1 >> 16 & (int) short.MaxValue) / 32768f;
        int index2 = order[index1];
        if (index2 >= 0)
        {
          float[] array = heights.array;
          int x = heights.rect.size.x;
          float num4 = array[index2];
          float num5 = array[index2 - 1];
          float num6 = array[index2 + 1];
          float num7 = array[index2 - x];
          float num8 = array[index2 + x];
          float num9 = sediments.array[index2];
          if ((double) num9 > 9.99999974737875E-05)
          {
            float num10 = num9 / 2f;
            if ((double) num10 > 0.75)
              num10 = 0.75f;
            heights.array[index2] = (float) ((double) num4 * (1.0 - (double) num10) + ((double) num5 + (double) num6 + (double) num7 + (double) num8) / 4.0 * (double) num10);
          }
          else
          {
            float num10 = num5;
            if ((double) num6 > (double) num10)
              num10 = num6;
            if ((double) num7 > (double) num10)
              num10 = num7;
            if ((double) num8 > (double) num10)
              num10 = num8;
            float num11 = num5;
            if ((double) num6 < (double) num11)
              num11 = num6;
            if ((double) num7 < (double) num11)
              num11 = num7;
            if ((double) num8 < (double) num11)
              num11 = num8;
            float num12 = num3 * (num10 - num11) + num11;
            heights.array[index2] = (float) ((double) heights.array[index2] * (1.0 - (double) ruffle) + (double) num12 * (double) ruffle);
          }
        }
      }
    }

    private static void LevelCells(float hX, float hz, ref float sX, ref float sz)
    {
      float num1 = (float) (((double) hX + (double) sX + (double) sz + (double) hz) / 2.0);
      if ((double) hX + (double) sX > (double) hz + (double) sz)
      {
        float num2 = sX + hX - num1;
        if ((double) num2 > (double) sX)
          num2 = sX;
        sX -= num2;
        sz += num2;
      }
      else
      {
        float num2 = sz + hz - num1;
        if ((double) num2 > (double) sz)
          num2 = sz;
        sz -= num2;
        sX += num2;
      }
    }

    private static void LevelCells(
      float h1,
      float h2,
      float h3,
      ref float s1,
      ref float s2,
      ref float s3)
    {
      Erosion.LevelCells(h1, h2, ref s1, ref s2);
      Erosion.LevelCells(h2, h3, ref s2, ref s3);
      Erosion.LevelCells(h3, h1, ref s3, ref s1);
    }

    private static void LevelCells(
      float h,
      float hx,
      float hX,
      float hz,
      float hZ,
      ref float s,
      ref float sx,
      ref float sX,
      ref float sz,
      ref float sZ)
    {
    }

    private struct Cross
    {
      public float c;
      public float px;
      public float nx;
      public float pz;
      public float nz;

      public Cross(float c, float px, float nx, float pz, float nz)
      {
        this.c = c;
        this.px = px;
        this.nx = nx;
        this.pz = pz;
        this.nz = nz;
      }

      public Cross(Erosion.Cross src)
      {
        this.c = src.c;
        this.px = src.px;
        this.nx = src.nx;
        this.pz = src.pz;
        this.nz = src.nz;
      }

      public Cross(Erosion.Cross c1, Erosion.Cross c2)
      {
        this.c = c1.c * c2.c;
        this.px = c1.px * c2.px;
        this.nx = c1.nx * c2.nx;
        this.pz = c1.pz * c2.pz;
        this.nz = c1.nz * c2.nz;
      }

      public Cross(float[] m, int sizeX, int sizeZ, int i)
      {
        this.px = m[i - 1];
        this.c = m[i];
        this.nx = m[i + 1];
        this.pz = m[i - sizeX];
        this.nz = m[i + sizeX];
      }

      public Cross(bool[] m, int sizeX, int sizeZ, int i)
      {
        this.px = !m[i - 1] ? 0.0f : 1f;
        this.c = !m[i] ? 0.0f : 1f;
        this.nx = !m[i + 1] ? 0.0f : 1f;
        this.pz = !m[i - sizeX] ? 0.0f : 1f;
        this.nz = !m[i + sizeX] ? 0.0f : 1f;
      }

      public Cross(Matrix m, int i)
      {
        this.px = m.array[i - 1];
        this.c = m.array[i];
        this.nx = m.array[i + 1];
        this.pz = m.array[i - m.rect.size.x];
        this.nz = m.array[i + m.rect.size.x];
      }

      public void ToMatrix(float[] m, int sizeX, int sizeZ, int i)
      {
        m[i - 1] = this.px;
        m[i] = this.c;
        m[i + 1] = this.nx;
        m[i - sizeX] = this.pz;
        m[i + sizeX] = this.nz;
      }

      public void ToMatrix(Matrix m, int i)
      {
        m.array[i - 1] = this.px;
        m.array[i] = this.c;
        m.array[i + 1] = this.nx;
        m.array[i - m.rect.size.x] = this.pz;
        m.array[i + m.rect.size.x] = this.nz;
      }

      public void Percent()
      {
        float num = this.c + this.px + this.nx + this.pz + this.nz;
        if ((double) num > 9.99999974737875E-06)
        {
          this.c /= num;
          this.px /= num;
          this.nx /= num;
          this.pz /= num;
          this.nz /= num;
        }
        else
        {
          this.c = 0.0f;
          this.px = 0.0f;
          this.nx = 0.0f;
          this.pz = 0.0f;
          this.nz = 0.0f;
        }
      }

      public void ClampPositive()
      {
        this.c = (double) this.c >= 0.0 ? this.c : 0.0f;
        this.px = (double) this.px >= 0.0 ? this.px : 0.0f;
        this.nx = (double) this.nx >= 0.0 ? this.nx : 0.0f;
        this.pz = (double) this.pz >= 0.0 ? this.pz : 0.0f;
        this.nz = (double) this.nz >= 0.0 ? this.nz : 0.0f;
      }

      public float max => Mathf.Max(Mathf.Max(Mathf.Max(this.px, this.nx), Mathf.Max(this.pz, this.nz)), this.c);

      public float min => Mathf.Min(Mathf.Min(Mathf.Min(this.px, this.nx), Mathf.Min(this.pz, this.nz)), this.c);

      public float sum => this.c + this.px + this.nx + this.pz + this.nz;

      public float avg => (float) (((double) this.c + (double) this.px + (double) this.nx + (double) this.pz + (double) this.nz) / 5.0);

      public float avgAround => (float) (((double) this.px + (double) this.nx + (double) this.pz + (double) this.nz) / 4.0);

      public float maxAround => Mathf.Max(Mathf.Max(this.px, this.nx), Mathf.Max(this.pz, this.nz));

      public float minAround => Mathf.Min(Mathf.Min(this.px, this.nx), Mathf.Min(this.pz, this.nz));

      public void Multiply(Erosion.Cross c2)
      {
        this.c *= c2.c;
        this.px *= c2.px;
        this.nx *= c2.nx;
        this.pz *= c2.pz;
        this.nz *= c2.nz;
      }

      public void Multiply(float f)
      {
        this.c *= f;
        this.px *= f;
        this.nx *= f;
        this.pz *= f;
        this.nz *= f;
      }

      public void Add(Erosion.Cross c2)
      {
        this.c += c2.c;
        this.px += c2.px;
        this.nx += c2.nx;
        this.pz += c2.pz;
        this.nz += c2.nz;
      }

      public void Divide(Erosion.Cross c2)
      {
        this.c /= c2.c;
        this.px /= c2.px;
        this.nx /= c2.nx;
        this.pz /= c2.pz;
        this.nz /= c2.nz;
      }

      public void Divide(float f)
      {
        this.c /= f;
        this.px /= f;
        this.nx /= f;
        this.pz /= f;
        this.nz /= f;
      }

      public void Subtract(float f)
      {
        this.c -= f;
        this.px -= f;
        this.nx -= f;
        this.pz -= f;
        this.nz -= f;
      }

      public void SubtractInverse(float f)
      {
        this.c = f - this.c;
        this.px = f - this.px;
        this.nx = f - this.nx;
        this.pz = f - this.pz;
        this.nz = f - this.nz;
      }

      public float GetMultipliedMax(Erosion.Cross c2) => Mathf.Max(Mathf.Max(Mathf.Max(this.px * c2.px, this.nx * c2.nx), Mathf.Max(this.pz * c2.pz, this.nz * c2.nz)), this.c * c2.c);

      public float GetMultipliedSum(Erosion.Cross c2) => (float) ((double) this.c * (double) c2.c + (double) this.px * (double) c2.px + (double) this.nx * (double) c2.nx + (double) this.pz * (double) c2.pz + (double) this.nz * (double) c2.nz);

      public bool isNaN => float.IsNaN(this.c) || float.IsNaN(this.px) || (float.IsNaN(this.pz) || float.IsNaN(this.nx)) || float.IsNaN(this.nz);

      public float this[int n]
      {
        get
        {
          switch (n)
          {
            case 0:
              return this.c;
            case 1:
              return this.px;
            case 2:
              return this.nx;
            case 3:
              return this.pz;
            case 4:
              return this.nz;
            default:
              return this.c;
          }
        }
        set
        {
          switch (n)
          {
            case 0:
              this.c = value;
              break;
            case 1:
              this.px = value;
              break;
            case 2:
              this.nx = value;
              break;
            case 3:
              this.pz = value;
              break;
            case 4:
              this.nz = value;
              break;
            default:
              this.c = value;
              break;
          }
        }
      }

      public void SortByHeight(int[] sorted)
      {
        for (int index = 0; index < 5; ++index)
          sorted[index] = index;
        for (int index1 = 0; index1 < 5; ++index1)
        {
          for (int index2 = 0; index2 < 4; ++index2)
          {
            if ((double) this[sorted[index2]] > (double) this[sorted[index2 + 1]])
            {
              int num = sorted[index2];
              sorted[index2] = sorted[index2 + 1];
              sorted[index2 + 1] = num;
            }
          }
        }
      }

      [DebuggerHidden]
      public IEnumerable<int> Sorted()
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        Erosion.Cross.\u003CSorted\u003Ec__Iterator0 sortedCIterator0 = new Erosion.Cross.\u003CSorted\u003Ec__Iterator0()
        {
          \u0024this = this
        };
        // ISSUE: reference to a compiler-generated field
        sortedCIterator0.\u0024PC = -2;
        return (IEnumerable<int>) sortedCIterator0;
      }

      public static Erosion.Cross operator +(Erosion.Cross c1, Erosion.Cross c2) => new Erosion.Cross(c1.c + c2.c, c1.px + c2.px, c1.nx + c2.nx, c1.pz + c2.pz, c1.nz + c2.nz);

      public static Erosion.Cross operator +(Erosion.Cross c1, float f) => new Erosion.Cross(c1.c + f, c1.px + f, c1.nx + f, c1.pz + f, c1.nz + f);

      public static Erosion.Cross operator -(Erosion.Cross c1, Erosion.Cross c2) => new Erosion.Cross(c1.c - c2.c, c1.px - c2.px, c1.nx - c2.nx, c1.pz - c2.pz, c1.nz - c2.nz);

      public static Erosion.Cross operator -(float f, Erosion.Cross c2) => new Erosion.Cross(f - c2.c, f - c2.px, f - c2.nx, f - c2.pz, f - c2.nz);

      public static Erosion.Cross operator -(Erosion.Cross c1, float f) => new Erosion.Cross(c1.c - f, c1.px - f, c1.nx - f, c1.pz - f, c1.nz - f);

      public static Erosion.Cross operator *(Erosion.Cross c1, Erosion.Cross c2) => new Erosion.Cross(c1.c * c2.c, c1.px * c2.px, c1.nx * c2.nx, c1.pz * c2.pz, c1.nz * c2.nz);

      public static Erosion.Cross operator *(float f, Erosion.Cross c2) => new Erosion.Cross(f * c2.c, f * c2.px, f * c2.nx, f * c2.pz, f * c2.nz);

      public static Erosion.Cross operator *(Erosion.Cross c1, float f) => new Erosion.Cross(c1.c * f, c1.px * f, c1.nx * f, c1.pz * f, c1.nz * f);

      public static Erosion.Cross operator /(Erosion.Cross c1, float f) => (double) f > 9.99999974737875E-06 ? new Erosion.Cross(c1.c / f, c1.px / f, c1.nx / f, c1.pz / f, c1.nz / f) : new Erosion.Cross(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

      public Erosion.Cross PercentObsolete()
      {
        float num = this.c + this.px + this.nx + this.pz + this.nz;
        return (double) num > 9.99999974737875E-06 ? new Erosion.Cross(this.c / num, this.px / num, this.nx / num, this.pz / num, this.nz / num) : new Erosion.Cross(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
      }

      public Erosion.Cross ClampPositiveObsolete() => new Erosion.Cross((double) this.c >= 0.0 ? this.c : 0.0f, (double) this.px >= 0.0 ? this.px : 0.0f, (double) this.nx >= 0.0 ? this.nx : 0.0f, (double) this.pz >= 0.0 ? this.pz : 0.0f, (double) this.nz >= 0.0 ? this.nz : 0.0f);
    }

    private struct MooreCross
    {
      public float c;
      public float px;
      public float nx;
      public float pxpz;
      public float nxpz;
      public float pz;
      public float nz;
      public float pxnz;
      public float nxnz;

      public MooreCross(
        float c,
        float px,
        float nx,
        float pz,
        float nz,
        float pxpz,
        float nxpz,
        float pxnz,
        float nxnz)
      {
        this.c = c;
        this.px = px;
        this.nx = nx;
        this.pz = pz;
        this.nz = nz;
        this.pxpz = pxpz;
        this.nxpz = nxpz;
        this.pxnz = pxnz;
        this.nxnz = nxnz;
      }

      public MooreCross(Erosion.MooreCross src)
      {
        this.c = src.c;
        this.px = src.px;
        this.nx = src.nx;
        this.pz = src.pz;
        this.nz = src.nz;
        this.pxpz = src.pxpz;
        this.nxpz = src.nxpz;
        this.pxnz = src.pxnz;
        this.nxnz = src.nxnz;
      }

      public MooreCross(float[] m, int sizeX, int sizeZ, int i)
      {
        this.px = m[i - 1];
        this.c = m[i];
        this.nx = m[i + 1];
        this.pz = m[i - sizeX];
        this.nz = m[i + sizeX];
        this.pxpz = m[i - 1 - sizeX];
        this.nxpz = m[i + 1 - sizeX];
        this.pxnz = m[i - 1 + sizeX];
        this.nxnz = m[i + 1 + sizeX];
      }

      public MooreCross(Matrix m, int i)
      {
        this.px = m.array[i - 1];
        this.c = m.array[i];
        this.nx = m.array[i + 1];
        this.pz = m.array[i - m.rect.size.x];
        this.nz = m.array[i + m.rect.size.x];
        this.pxpz = m.array[i - 1 - m.rect.size.x];
        this.nxpz = m.array[i + 1 - m.rect.size.x];
        this.pxnz = m.array[i - 1 + m.rect.size.x];
        this.nxnz = m.array[i + 1 + m.rect.size.x];
      }

      public void ToMatrix(float[] m, int sizeX, int sizeZ, int i)
      {
        m[i - 1] = this.px;
        m[i] = this.c;
        m[i + 1] = this.nx;
        m[i - sizeX] = this.pz;
        m[i + sizeX] = this.nz;
        m[i - 1 - sizeX] = this.pxpz;
        m[i + 1 - sizeX] = this.nxpz;
        m[i - 1 + sizeX] = this.pxnz;
        m[i + 1 + sizeX] = this.nxnz;
      }

      public void ToMatrix(Matrix m, int i)
      {
        m.array[i - 1] = this.px;
        m.array[i] = this.c;
        m.array[i + 1] = this.nx;
        m.array[i - m.rect.size.x] = this.pz;
        m.array[i + m.rect.size.x] = this.nz;
        m.array[i - 1 - m.rect.size.x] = this.pxpz;
        m.array[i + 1 - m.rect.size.x] = this.nxpz;
        m.array[i - 1 + m.rect.size.x] = this.pxnz;
        m.array[i + 1 + m.rect.size.x] = this.nxnz;
      }

      public void Percent()
      {
        float num = this.c + this.px + this.nx + this.pz + this.nz + this.pxpz + this.nxpz + this.pxnz + this.nxnz;
        if ((double) num > 9.99999974737875E-06)
        {
          this.c /= num;
          this.px /= num;
          this.nx /= num;
          this.pz /= num;
          this.nz /= num;
          this.pxpz /= num;
          this.nxpz /= num;
          this.pxnz /= num;
          this.nxnz /= num;
        }
        else
        {
          this.c = 0.0f;
          this.px = 0.0f;
          this.nx = 0.0f;
          this.pz = 0.0f;
          this.nz = 0.0f;
          this.pxpz = 0.0f;
          this.nxpz = 0.0f;
          this.pxnz = 0.0f;
          this.nxnz = 0.0f;
        }
      }

      public bool isNaN => float.IsNaN(this.c) || float.IsNaN(this.px) || (float.IsNaN(this.pz) || float.IsNaN(this.nx)) || (float.IsNaN(this.nz) || float.IsNaN(this.pxpz) || (float.IsNaN(this.pxnz) || float.IsNaN(this.nxpz))) || float.IsNaN(this.nxnz);

      public override string ToString() => "MooreCross: " + (object) this.c + ", " + (object) this.px + ", " + (object) this.pz + ", " + (object) this.nx + ", " + (object) this.nz + ", " + (object) this.pxpz + ", " + (object) this.nxpz + ", " + (object) this.pxnz + ", " + (object) this.nxnz;

      public void ClampPositive()
      {
        this.c = (double) this.c >= 0.0 ? this.c : 0.0f;
        this.px = (double) this.px >= 0.0 ? this.px : 0.0f;
        this.nx = (double) this.nx >= 0.0 ? this.nx : 0.0f;
        this.pz = (double) this.pz >= 0.0 ? this.pz : 0.0f;
        this.nz = (double) this.nz >= 0.0 ? this.nz : 0.0f;
        this.pxpz = (double) this.pxpz >= 0.0 ? this.pxpz : 0.0f;
        this.nxpz = (double) this.nxpz >= 0.0 ? this.nxpz : 0.0f;
        this.pxnz = (double) this.pxnz >= 0.0 ? this.pxnz : 0.0f;
        this.nxnz = (double) this.nxnz >= 0.0 ? this.nxnz : 0.0f;
      }

      public float max => Mathf.Max(Mathf.Max(Mathf.Max(this.px, this.nx), Mathf.Max(this.pz, this.nz)), this.c);

      public float min => Mathf.Min(Mathf.Min(Mathf.Min(this.px, this.nx), Mathf.Min(this.pz, this.nz)), this.c);

      public float sum => this.c + this.px + this.nx + this.pz + this.nz;

      public void Multiply(float f)
      {
        this.c *= f;
        this.px *= f;
        this.nx *= f;
        this.pz *= f;
        this.nz *= f;
        this.pxpz *= f;
        this.nxpz *= f;
        this.pxnz *= f;
        this.nxnz *= f;
      }

      public void Add(float f)
      {
        this.c += f;
        this.px += f;
        this.nx += f;
        this.pz += f;
        this.nz += f;
        this.pxpz += f;
        this.nxpz += f;
        this.pxnz += f;
        this.nxnz += f;
      }

      public void Add(Erosion.MooreCross c2)
      {
        this.c += c2.c;
        this.px += c2.px;
        this.nx += c2.nx;
        this.pz += c2.pz;
        this.nz += c2.nz;
        this.pxpz += c2.pxpz;
        this.nxpz += c2.nxpz;
        this.pxnz += c2.pxnz;
        this.nxnz += c2.nxnz;
      }

      public void Subtract(float f)
      {
        this.c -= f;
        this.px -= f;
        this.nx -= f;
        this.pz -= f;
        this.nz -= f;
        this.pxpz -= f;
        this.nxpz -= f;
        this.pxnz -= f;
        this.nxnz -= f;
      }

      public void SubtractInverse(float f)
      {
        this.c = f - this.c;
        this.px = f - this.px;
        this.nx = f - this.nx;
        this.pz = f - this.pz;
        this.nz = f - this.nz;
        this.pxpz = f - this.pxpz;
        this.nxpz = f - this.nxpz;
        this.pxnz = f - this.pxnz;
        this.nxnz = f - this.nxnz;
      }

      public static Erosion.MooreCross operator +(
        Erosion.MooreCross c1,
        Erosion.MooreCross c2)
      {
        return new Erosion.MooreCross(c1.c + c2.c, c1.px + c2.px, c1.nx + c2.nx, c1.pz + c2.pz, c1.nz + c2.nz, c1.pxpz + c2.pxpz, c1.nxpz + c2.nxpz, c1.pxnz + c2.pxnz, c1.nxnz + c2.nxnz);
      }

      public static Erosion.MooreCross operator +(Erosion.MooreCross c1, float f) => new Erosion.MooreCross(c1.c + f, c1.px + f, c1.nx + f, c1.pz + f, c1.nz + f, c1.pxpz + f, c1.nxpz + f, c1.pxnz + f, c1.nxnz + f);

      public static Erosion.MooreCross operator -(
        Erosion.MooreCross c1,
        Erosion.MooreCross c2)
      {
        return new Erosion.MooreCross(c1.c - c2.c, c1.px - c2.px, c1.nx - c2.nx, c1.pz - c2.pz, c1.nz - c2.nz, c1.pxpz - c2.pxpz, c1.nxpz - c2.nxpz, c1.pxnz - c2.pxnz, c1.nxnz - c2.nxnz);
      }

      public static Erosion.MooreCross operator -(float f, Erosion.MooreCross c2) => new Erosion.MooreCross(f - c2.c, f - c2.px, f - c2.nx, f - c2.pz, f - c2.nz, f - c2.pxpz, f - c2.nxpz, f - c2.pxnz, f - c2.nxnz);

      public static Erosion.MooreCross operator -(Erosion.MooreCross c1, float f) => new Erosion.MooreCross(c1.c - f, c1.px - f, c1.nx - f, c1.pz - f, c1.nz - f, c1.pxpz - f, c1.nxpz - f, c1.pxnz - f, c1.nxnz - f);

      public static Erosion.MooreCross operator *(
        Erosion.MooreCross c1,
        Erosion.MooreCross c2)
      {
        return new Erosion.MooreCross(c1.c * c2.c, c1.px * c2.px, c1.nx * c2.nx, c1.pz * c2.pz, c1.nz * c2.nz, c1.pxpz * c2.pxpz, c1.nxpz * c2.nxpz, c1.pxnz * c2.pxnz, c1.nxnz * c2.nxnz);
      }

      public static Erosion.MooreCross operator *(float f, Erosion.MooreCross c2) => new Erosion.MooreCross(f * c2.c, f * c2.px, f * c2.nx, f * c2.pz, f * c2.nz, f * c2.pxpz, f * c2.nxpz, f * c2.pxnz, f * c2.nxnz);

      public static Erosion.MooreCross operator *(Erosion.MooreCross c1, float f) => new Erosion.MooreCross(c1.c * f, c1.px * f, c1.nx * f, c1.pz * f, c1.nz * f, c1.pxpz * f, c1.nxpz * f, c1.pxnz * f, c1.nxnz * f);

      public static Erosion.MooreCross operator /(Erosion.MooreCross c1, float f) => (double) f > 9.99999974737875E-06 ? new Erosion.MooreCross(c1.c / f, c1.px / f, c1.nx / f, c1.pz / f, c1.nz / f, c1.pxpz / f, c1.nxpz / f, c1.pxnz / f, c1.nxnz / f) : new Erosion.MooreCross(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

      public Erosion.MooreCross PercentObsolete()
      {
        float num = this.c + this.px + this.nx + this.pz + this.nz + this.pxpz + this.nxpz + this.pxnz + this.nxnz;
        return (double) num > 9.99999974737875E-06 ? new Erosion.MooreCross(this.c / num, this.px / num, this.nx / num, this.pz / num, this.nz / num, this.pxpz / num, this.nxpz / num, this.pxnz / num, this.nxnz / num) : new Erosion.MooreCross(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
      }

      public Erosion.MooreCross ClampPositiveObsolete() => new Erosion.MooreCross((double) this.c >= 0.0 ? this.c : 0.0f, (double) this.px >= 0.0 ? this.px : 0.0f, (double) this.nx >= 0.0 ? this.nx : 0.0f, (double) this.pz >= 0.0 ? this.pz : 0.0f, (double) this.nz >= 0.0 ? this.nz : 0.0f, (double) this.pxpz >= 0.0 ? this.pxpz : 0.0f, (double) this.nxpz >= 0.0 ? this.nxpz : 0.0f, (double) this.pxnz >= 0.0 ? this.pxnz : 0.0f, (double) this.nxnz >= 0.0 ? this.nxnz : 0.0f);
    }
  }
}
