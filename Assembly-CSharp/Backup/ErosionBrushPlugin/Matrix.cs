// Decompiled with JetBrains decompiler
// Type: ErosionBrushPlugin.Matrix
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.IO;
using UnityEngine;

namespace ErosionBrushPlugin
{
  [Serializable]
  public class Matrix : Matrix2<float>
  {
    public Matrix()
    {
      this.array = new float[0];
      this.rect = new CoordRect(0, 0, 0, 0);
      this.count = 0;
    }

    public Matrix(CoordRect rect, float[] array = null)
    {
      this.rect = rect;
      this.count = rect.size.x * rect.size.z;
      if (array != null && array.Length < this.count)
        Debug.Log((object) ("Array length: " + (object) array.Length + " is lower then matrix capacity: " + (object) this.count));
      if (array != null && array.Length >= this.count)
        this.array = array;
      else
        this.array = new float[this.count];
    }

    public Matrix(Coord offset, Coord size, float[] array = null)
    {
      this.rect = new CoordRect(offset, size);
      this.count = this.rect.size.x * this.rect.size.z;
      if (array != null && array.Length < this.count)
        Debug.Log((object) ("Array length: " + (object) array.Length + " is lower then matrix capacity: " + (object) this.count));
      if (array != null && array.Length >= this.count)
        this.array = array;
      else
        this.array = new float[this.count];
    }

    public float GetInterpolatedValue(Vector2 pos)
    {
      int x = Mathf.FloorToInt(pos.x);
      int z = Mathf.FloorToInt(pos.y);
      float num1 = pos.x - (float) x;
      float num2 = pos.y - (float) z;
      float num3 = this[x, z];
      float num4 = this[x + 1, z];
      float num5 = (float) ((double) num3 * (1.0 - (double) num1) + (double) num4 * (double) num1);
      float num6 = this[x, z + 1];
      float num7 = this[x + 1, z + 1];
      float num8 = (float) ((double) num6 * (1.0 - (double) num1) + (double) num7 * (double) num1);
      return (float) ((double) num5 * (1.0 - (double) num2) + (double) num8 * (double) num2);
    }

    public float GetAveragedValue(int x, int z, int steps)
    {
      float num1 = 0.0f;
      int num2 = 0;
      for (int index1 = 0; index1 < steps; ++index1)
      {
        for (int index2 = 0; index2 < steps; ++index2)
        {
          if (x + index1 < this.rect.offset.x + this.rect.size.x && z + index2 < this.rect.offset.z + this.rect.size.z)
          {
            num1 += this[x + index1, z + index2];
            ++num2;
          }
        }
      }
      return num1 / (float) num2;
    }

    public override object Clone() => (object) this.Copy();

    public Matrix Copy(Matrix result = null)
    {
      if (result == null)
        result = new Matrix(this.rect);
      result.rect = this.rect;
      result.pos = this.pos;
      result.count = this.count;
      if (result.array.Length != this.array.Length)
        result.array = new float[this.array.Length];
      for (int index = 0; index < this.array.Length; ++index)
        result.array[index] = this.array[index];
      return result;
    }

    public bool[] InRect(CoordRect area = default (CoordRect))
    {
      Matrix2<bool> matrix2 = new Matrix2<bool>(this.rect);
      CoordRect coordRect = CoordRect.Intersect(this.rect, area);
      Coord min = coordRect.Min;
      Coord max = coordRect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
          matrix2[x, z] = true;
      }
      return matrix2.array;
    }

    public void Fill(float[,] array, CoordRect arrayRect)
    {
      CoordRect coordRect = CoordRect.Intersect(this.rect, arrayRect);
      Coord min = coordRect.Min;
      Coord max = coordRect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
          this[x, z] = array[z - arrayRect.offset.z, x - arrayRect.offset.x];
      }
    }

    public void Pour(float[,] array, CoordRect arrayRect)
    {
      CoordRect coordRect = CoordRect.Intersect(this.rect, arrayRect);
      Coord min = coordRect.Min;
      Coord max = coordRect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
          array[z - arrayRect.offset.z, x - arrayRect.offset.x] = this[x, z];
      }
    }

    public void Pour(float[,,] array, int channel, CoordRect arrayRect)
    {
      CoordRect coordRect = CoordRect.Intersect(this.rect, arrayRect);
      Coord min = coordRect.Min;
      Coord max = coordRect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
          array[z - arrayRect.offset.z, x - arrayRect.offset.x, channel] = this[x, z];
      }
    }

    public float[,] ReadHeighmap(TerrainData data, float height = 1f)
    {
      CoordRect centerRect = CoordRect.Intersect(this.rect, new CoordRect(0, 0, data.heightmapResolution, data.heightmapResolution));
      float[,] heights = data.GetHeights(centerRect.offset.x, centerRect.offset.z, centerRect.size.x, centerRect.size.z);
      Coord min = centerRect.Min;
      Coord max = centerRect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
          this[x, z] = heights[z - min.z, x - min.x] * height;
      }
      this.RemoveBorders(centerRect);
      return heights;
    }

    public void WriteHeightmap(TerrainData data, float[,] array = null, float brushFallof = 0.5f, bool delayLod = false)
    {
      CoordRect coordRect = CoordRect.Intersect(this.rect, new CoordRect(0, 0, data.heightmapResolution, data.heightmapResolution));
      if (array == null || array.Length != coordRect.size.x * coordRect.size.z)
        array = new float[coordRect.size.z, coordRect.size.x];
      Coord min = coordRect.Min;
      Coord max = coordRect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
        {
          float a = this.Fallof(x, z, brushFallof);
          if (!Mathf.Approximately(a, 0.0f))
            array[z - min.z, x - min.x] = (float) ((double) this[x, z] * (double) a + (double) array[z - min.z, x - min.x] * (1.0 - (double) a));
        }
      }
      if (delayLod)
        data.SetHeightsDelayLOD(coordRect.offset.x, coordRect.offset.z, array);
      else
        data.SetHeights(coordRect.offset.x, coordRect.offset.z, array);
    }

    public float[,,] ReadSplatmap(TerrainData data, int channel, float[,,] array = null)
    {
      CoordRect centerRect = CoordRect.Intersect(this.rect, new CoordRect(0, 0, data.alphamapResolution, data.alphamapResolution));
      if (array == null)
        array = data.GetAlphamaps(centerRect.offset.x, centerRect.offset.z, centerRect.size.x, centerRect.size.z);
      Coord min = centerRect.Min;
      Coord max = centerRect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
          this[x, z] = array[z - min.z, x - min.x, channel];
      }
      this.RemoveBorders(centerRect);
      return array;
    }

    public static void AddSplatmaps(
      TerrainData data,
      Matrix[] matrices,
      int[] channels,
      float[] opacity,
      float[,,] array = null,
      float brushFallof = 0.5f)
    {
      int alphamapLayers = data.alphamapLayers;
      bool[] flagArray = new bool[alphamapLayers];
      for (int index = 0; index < channels.Length; ++index)
        flagArray[channels[index]] = true;
      float[] numArray = new float[alphamapLayers];
      CoordRect coordRect = CoordRect.Intersect(new CoordRect(new Coord(0, 0), new Coord(data.alphamapResolution, data.alphamapResolution)), matrices[0].rect);
      if (array == null)
        array = data.GetAlphamaps(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z);
      Coord min = coordRect.Min;
      Coord max = coordRect.Max;
      for (int x1 = min.x; x1 < max.x; ++x1)
      {
        for (int z1 = min.z; z1 < max.z; ++z1)
        {
          float a = matrices[0].Fallof(x1, z1, brushFallof);
          if (!Mathf.Approximately(a, 0.0f))
          {
            for (int index = 0; index < alphamapLayers; ++index)
              numArray[index] = array[z1 - min.z, x1 - min.x, index];
            for (int index = 0; index < matrices.Length; ++index)
              matrices[index][x1, z1] = Mathf.Max(0.0f, matrices[index][x1, z1] - numArray[channels[index]]);
            for (int index = 0; index < matrices.Length; ++index)
            {
              Matrix matrix;
              int x2;
              int z2;
              (matrix = matrices[index])[x2 = x1, z2 = z1] = matrix[x2, z2] * (a * opacity[index]);
            }
            float num1 = 0.0f;
            for (int index = 0; index < matrices.Length; ++index)
              num1 += matrices[index][x1, z1];
            if ((double) num1 > 1.0)
            {
              for (int index = 0; index < matrices.Length; ++index)
              {
                Matrix matrix;
                int x2;
                int z2;
                (matrix = matrices[index])[x2 = x1, z2 = z1] = matrix[x2, z2] / num1;
              }
              num1 = 1f;
            }
            float num2 = 1f - num1;
            for (int index = 0; index < alphamapLayers; ++index)
              numArray[index] *= num2;
            for (int index = 0; index < matrices.Length; ++index)
              numArray[channels[index]] += matrices[index][x1, z1];
            for (int index = 0; index < alphamapLayers; ++index)
              array[z1 - min.z, x1 - min.x, index] = numArray[index];
          }
        }
      }
      data.SetAlphamaps(coordRect.offset.x, coordRect.offset.z, array);
    }

    public void ToTexture(
      Texture2D texture = null,
      Color[] colors = null,
      float rangeMin = 0.0f,
      float rangeMax = 1f,
      bool resizeTexture = false)
    {
      if ((UnityEngine.Object) texture == (UnityEngine.Object) null)
        texture = new Texture2D(this.rect.size.x, this.rect.size.z);
      if (resizeTexture)
        texture.Resize(this.rect.size.x, this.rect.size.z);
      CoordRect coordRect = CoordRect.Intersect(new CoordRect(new Coord(0, 0), new Coord(texture.width, texture.height)), this.rect);
      if (colors == null || colors.Length != coordRect.size.x * coordRect.size.z)
        colors = new Color[coordRect.size.x * coordRect.size.z];
      Coord min = coordRect.Min;
      Coord max = coordRect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
        {
          float num1 = (this[x, z] - rangeMin) / (rangeMax - rangeMin) * 256f;
          int num2 = (int) num1;
          float num3 = num1 - (float) num2;
          float r = (float) num2 / 256f;
          float num4 = (float) (num2 + 1) / 256f;
          int num5 = x - min.x;
          int num6 = z - min.z;
          colors[num6 * (max.x - min.x) + num5] = new Color(r, (double) num3 <= 0.333000004291534 ? r : num4, (double) num3 <= 0.666000008583069 ? r : num4);
        }
      }
      texture.SetPixels(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z, colors);
      texture.Apply();
    }

    public void FromTexture(Texture2D texture)
    {
      CoordRect coordRect = CoordRect.Intersect(new CoordRect(0, 0, texture.width, texture.height), this.rect);
      Color[] pixels = texture.GetPixels(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z);
      Coord min = coordRect.Min;
      Coord max = coordRect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
        {
          int num1 = x - min.x;
          int num2 = z - min.z;
          Color color = pixels[num2 * (max.x - min.x) + num1];
          this[x, z] = (float) (((double) color.r + (double) color.g + (double) color.b) / 3.0);
        }
      }
    }

    public void FromTextureTiled(Texture2D texture)
    {
      Color[] pixels = texture.GetPixels();
      int width = texture.width;
      int height = texture.height;
      Coord min = this.rect.Min;
      Coord max = this.rect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
        {
          int num1 = x % width;
          if (num1 < 0)
            num1 += width;
          int num2 = z % height;
          if (num2 < 0)
            num2 += height;
          Color color = pixels[num2 * width + num1];
          this[x, z] = (float) (((double) color.r + (double) color.g + (double) color.b) / 3.0);
        }
      }
    }

    public Texture2D SimpleToTexture(
      Texture2D texture = null,
      Color[] colors = null,
      float rangeMin = 0.0f,
      float rangeMax = 1f,
      string savePath = null)
    {
      if ((UnityEngine.Object) texture == (UnityEngine.Object) null)
        texture = new Texture2D(this.rect.size.x, this.rect.size.z);
      if (texture.width != this.rect.size.x || texture.height != this.rect.size.z)
        texture.Resize(this.rect.size.x, this.rect.size.z);
      if (colors == null || colors.Length != this.rect.size.x * this.rect.size.z)
        colors = new Color[this.rect.size.x * this.rect.size.z];
      for (int index = 0; index < this.count; ++index)
      {
        float num = (this.array[index] - rangeMin) / (rangeMax - rangeMin);
        colors[index] = new Color(num, num, num);
      }
      texture.SetPixels(colors);
      texture.Apply();
      return texture;
    }

    public void SimpleFromTexture(Texture2D texture)
    {
      this.ChangeRect(new CoordRect(this.rect.offset.x, this.rect.offset.z, texture.width, texture.height));
      Color[] pixels = texture.GetPixels();
      for (int index = 0; index < this.count; ++index)
      {
        Color color = pixels[index];
        this.array[index] = (float) (((double) color.r + (double) color.g + (double) color.b) / 3.0);
      }
    }

    public void ImportRaw(string path)
    {
      FileStream fileStream = new FileInfo(path).Open(FileMode.Open, FileAccess.Read);
      int num = (int) Mathf.Sqrt((float) (fileStream.Length / 2L));
      byte[] buffer = new byte[num * num * 2];
      fileStream.Read(buffer, 0, buffer.Length);
      fileStream.Close();
      this.ChangeRect(new CoordRect(0, 0, num, num));
      int index = 0;
      Coord min = this.rect.Min;
      Coord max = this.rect.Max;
      for (int z = max.z - 1; z >= min.z; --z)
      {
        for (int x = min.x; x < max.x; ++x)
        {
          this[x, z] = (float) (((double) buffer[index + 1] * 256.0 + (double) buffer[index]) / (double) ushort.MaxValue);
          index += 2;
        }
      }
    }

    public void Replicate(Matrix source, bool tile = false)
    {
      Coord min = this.rect.Min;
      Coord max = this.rect.Max;
      for (int x1 = min.x; x1 < max.x; ++x1)
      {
        for (int z1 = min.z; z1 < max.z; ++z1)
        {
          if (source.rect.CheckInRange(x1, z1))
            this[x1, z1] = source[x1, z1];
          else if (tile)
          {
            int num1 = x1 - source.rect.offset.x;
            int num2 = z1 - source.rect.offset.z;
            int num3 = num1 % source.rect.size.x;
            int num4 = num2 % source.rect.size.z;
            if (num3 < 0)
              num3 += source.rect.size.x;
            if (num4 < 0)
              num4 += source.rect.size.z;
            int x2 = num3 + source.rect.offset.x;
            int z2 = num4 + source.rect.offset.z;
            this[x1, z1] = source[x2, z2];
          }
        }
      }
    }

    public float GetArea(int x, int z, int range)
    {
      if (range == 0)
      {
        if (x < this.rect.offset.x)
          x = this.rect.offset.x;
        if (x >= this.rect.offset.x + this.rect.size.x)
          x = this.rect.offset.x + this.rect.size.x - 1;
        if (z < this.rect.offset.z)
          z = this.rect.offset.z;
        if (z >= this.rect.offset.z + this.rect.size.z)
          z = this.rect.offset.z + this.rect.size.z - 1;
        return this.array[(z - this.rect.offset.z) * this.rect.size.x + x - this.rect.offset.x];
      }
      float num1 = 0.0f;
      int num2 = 0;
      for (int index1 = x - range; index1 <= x + range; ++index1)
      {
        if (index1 >= this.rect.offset.x && index1 < this.rect.offset.x + this.rect.size.x)
        {
          for (int index2 = z - range; index2 <= z + range; ++index2)
          {
            if (index2 >= this.rect.offset.z && index2 < this.rect.offset.z + this.rect.size.z)
            {
              num1 += this.array[(index2 - this.rect.offset.z) * this.rect.size.x + index1 - this.rect.offset.x];
              ++num2;
            }
          }
        }
      }
      return num1 / (float) num2;
    }

    public float GetInterpolated(float x, float z)
    {
      int num1 = (int) x;
      if ((double) x < 0.0)
        --num1;
      if (num1 < this.rect.offset.x)
        num1 = this.rect.offset.x;
      int num2 = num1 + 1;
      if (num2 >= this.rect.offset.x + this.rect.size.x)
        num2 = this.rect.offset.x + this.rect.size.x - 1;
      int num3 = (int) z;
      if ((double) z < 0.0)
        --num3;
      if (num3 < this.rect.offset.z)
        num3 = this.rect.offset.z;
      int num4 = num3 + 1;
      if (num4 >= this.rect.offset.z + this.rect.size.z)
        num4 = this.rect.offset.z + this.rect.size.z - 1;
      float num5 = x - (float) num1;
      float num6 = z - (float) num3;
      float num7 = this.array[(num3 - this.rect.offset.z) * this.rect.size.x + num1 - this.rect.offset.x];
      float num8 = this.array[(num3 - this.rect.offset.z) * this.rect.size.x + num2 - this.rect.offset.x];
      float num9 = this.array[(num4 - this.rect.offset.z) * this.rect.size.x + num1 - this.rect.offset.x];
      float num10 = this.array[(num4 - this.rect.offset.z) * this.rect.size.x + num2 - this.rect.offset.x];
      float num11 = (float) ((double) num7 * (1.0 - (double) num5) + (double) num8 * (double) num5);
      float num12 = (float) ((double) num9 * (1.0 - (double) num5) + (double) num10 * (double) num5);
      return (float) ((double) num11 * (1.0 - (double) num6) + (double) num12 * (double) num6);
    }

    public Matrix Resize(CoordRect newRect, Matrix result = null)
    {
      if (result == null)
        result = new Matrix(newRect);
      else
        result.ChangeRect(newRect);
      Coord min = result.rect.Min;
      Coord max = result.rect.Max;
      for (int x1 = min.x; x1 < max.x; ++x1)
      {
        for (int z1 = min.z; z1 < max.z; ++z1)
        {
          float x2 = 1f * (float) (x1 - result.rect.offset.x) / (float) result.rect.size.x * (float) this.rect.size.x + (float) this.rect.offset.x;
          float z2 = 1f * (float) (z1 - result.rect.offset.z) / (float) result.rect.size.z * (float) this.rect.size.z + (float) this.rect.offset.z;
          result[x1, z1] = this.GetInterpolated(x2, z2);
        }
      }
      return result;
    }

    public Matrix Downscale(int factor, Matrix result = null) => this.Resize(this.rect / factor, result);

    public Matrix Upscale(int factor, Matrix result = null) => this.Resize(this.rect * factor, result);

    public Matrix BlurredUpscale(int factor)
    {
      Matrix matrix = new Matrix(this.rect, new float[this.count * factor]);
      Matrix result = new Matrix(this.rect, new float[this.count * factor]);
      matrix.Fill((Matrix2<float>) this, false);
      int num = Mathf.RoundToInt(Mathf.Sqrt((float) factor));
      for (int index = 0; index < num; ++index)
      {
        matrix.Resize(matrix.rect * 2, result);
        matrix.ChangeRect(result.rect);
        matrix.Fill((Matrix2<float>) result, false);
        matrix.Blur(intensity: 0.5f);
      }
      return matrix;
    }

    public Matrix OutdatedResize(CoordRect newRect, float smoothness = 1f, Matrix result = null)
    {
      int factor1 = newRect.size.x / this.rect.size.x;
      int factor2 = this.rect.size.x / newRect.size.x;
      if (factor1 > 1 && !newRect.Divisible((float) factor1))
        Debug.LogError((object) ("Matrix rect " + (object) this.rect + " could not be upscaled to " + (object) newRect + " with factor " + (object) factor1));
      if (factor2 > 1 && !this.rect.Divisible((float) factor2))
        Debug.LogError((object) ("Matrix rect " + (object) this.rect + " could not be downscaled to " + (object) newRect + " with factor " + (object) factor2));
      if (factor1 > 1)
        result = this.OutdatedUpscale(factor1, result);
      if (factor2 > 1)
        result = this.OutdatedDownscale(factor2, smoothness, result);
      return factor1 <= 1 && factor2 <= 1 ? this.Copy(result) : result;
    }

    public Matrix OutdatedUpscale(int factor, Matrix result = null)
    {
      if (result == null)
        result = new Matrix(this.rect * factor);
      result.ChangeRect(this.rect * factor);
      if (factor == 1)
        return this.Copy(result);
      Coord min = this.rect.Min;
      Coord coord = this.rect.Max - 1;
      float num = 1f / (float) factor;
      for (int x = min.x; x < coord.x; ++x)
      {
        for (int z = min.z; z < coord.z; ++z)
        {
          float a1 = this[x, z];
          float a2 = this[x + 1, z];
          float b1 = this[x, z + 1];
          float b2 = this[x + 1, z + 1];
          for (int index1 = 0; index1 < factor; ++index1)
          {
            for (int index2 = 0; index2 < factor; ++index2)
            {
              float t1 = (float) index1 * num;
              float t2 = (float) index2 * num;
              float a3 = Mathf.Lerp(a1, b1, t2);
              float b3 = Mathf.Lerp(a2, b2, t2);
              result[x * factor + index1, z * factor + index2] = Mathf.Lerp(a3, b3, t1);
            }
          }
        }
      }
      result.RemoveBorders(0, 0, factor + 1, factor + 1);
      return result;
    }

    public float OutdatedGetInterpolated(float x, float z)
    {
      int num1 = (int) x;
      int num2 = (int) ((double) x + 1.0);
      if (num2 >= this.rect.offset.x + this.rect.size.x)
        num2 = this.rect.offset.x + this.rect.size.x - 1;
      int num3 = (int) z;
      int num4 = (int) ((double) z + 1.0);
      if (num4 >= this.rect.offset.z + this.rect.size.z)
        num4 = this.rect.offset.z + this.rect.size.z - 1;
      float num5 = x - (float) num1;
      float num6 = z - (float) num3;
      float num7 = this.array[(num3 - this.rect.offset.z) * this.rect.size.x + num1 - this.rect.offset.x];
      float num8 = this.array[(num3 - this.rect.offset.z) * this.rect.size.x + num2 - this.rect.offset.x];
      float num9 = this.array[(num4 - this.rect.offset.z) * this.rect.size.x + num1 - this.rect.offset.x];
      float num10 = this.array[(num4 - this.rect.offset.z) * this.rect.size.x + num2 - this.rect.offset.x];
      float num11 = (float) ((double) num7 * (1.0 - (double) num5) + (double) num8 * (double) num5);
      float num12 = (float) ((double) num9 * (1.0 - (double) num5) + (double) num10 * (double) num5);
      return (float) ((double) num11 * (1.0 - (double) num6) + (double) num12 * (double) num6);
    }

    public Matrix TestResize(CoordRect newRect)
    {
      Matrix matrix = new Matrix(newRect);
      Coord min = matrix.rect.Min;
      Coord max = matrix.rect.Max;
      for (int x1 = min.x; x1 < max.x; ++x1)
      {
        for (int z1 = min.z; z1 < max.z; ++z1)
        {
          float x2 = 1f * (float) (x1 - matrix.rect.offset.x) / (float) matrix.rect.size.x * (float) this.rect.size.x + (float) this.rect.offset.x;
          float z2 = 1f * (float) (z1 - matrix.rect.offset.z) / (float) matrix.rect.size.z * (float) this.rect.size.z + (float) this.rect.offset.z;
          matrix[x1, z1] = this.OutdatedGetInterpolated(x2, z2);
        }
      }
      return matrix;
    }

    public Matrix OutdatedDownscale(int factor = 2, float smoothness = 1f, Matrix result = null)
    {
      if (!this.rect.Divisible((float) factor))
        Debug.LogError((object) ("Matrix rect " + (object) this.rect + " could not be downscaled with factor " + (object) factor));
      if (result == null)
        result = new Matrix(this.rect / factor);
      result.ChangeRect(this.rect / factor);
      if (factor == 1)
        return this.Copy(result);
      Coord min1 = this.rect.Min;
      Coord min2 = result.rect.Min;
      Coord max = result.rect.Max;
      if ((double) smoothness < 9.99999974737875E-05)
      {
        for (int x1 = min2.x; x1 < max.x; ++x1)
        {
          for (int z1 = min2.z; z1 < max.z; ++z1)
          {
            int x2 = (x1 - min2.x) * factor + min1.x;
            int z2 = (z1 - min2.z) * factor + min1.z;
            result[x1, z1] = this[x2, z2];
          }
        }
      }
      else
      {
        for (int x1 = min2.x; x1 < max.x; ++x1)
        {
          for (int z1 = min2.z; z1 < max.z; ++z1)
          {
            int x2 = (x1 - min2.x) * factor + min1.x;
            int z2 = (z1 - min2.z) * factor + min1.z;
            float num = 0.0f;
            for (int x3 = x2; x3 < x2 + factor; ++x3)
            {
              for (int z3 = z2; z3 < z2 + factor; ++z3)
                num += this[x3, z3];
            }
            result[x1, z1] = (float) ((double) num / (double) (factor * factor) * (double) smoothness + (double) this[x2, z2] * (1.0 - (double) smoothness));
          }
        }
      }
      return result;
    }

    public void Spread(float strength = 0.5f, int iterations = 4, Matrix copy = null)
    {
      Coord min = this.rect.Min;
      Coord max = this.rect.Max;
      for (int index = 0; index < this.count; ++index)
        this.array[index] = Mathf.Clamp(this.array[index], -1f, 1f);
      if (copy == null)
      {
        copy = this.Copy();
      }
      else
      {
        for (int index = 0; index < this.count; ++index)
          copy.array[index] = this.array[index];
      }
      for (int index1 = 0; index1 < iterations; ++index1)
      {
        for (int x = min.x; x < max.x; ++x)
        {
          float num1 = this[x, min.z];
          this.SetPos(x, min.z);
          for (int index2 = min.z + 1; index2 < max.z; ++index2)
          {
            num1 = (float) (((double) num1 + (double) this.array[this.pos]) / 2.0);
            this.array[this.pos] = num1;
            this.pos += this.rect.size.x;
          }
          float num2 = this[x, max.z - 1];
          this.SetPos(x, max.z - 1);
          for (int index2 = max.z - 2; index2 >= min.z; --index2)
          {
            num2 = (float) (((double) num2 + (double) this.array[this.pos]) / 2.0);
            this.array[this.pos] = num2;
            this.pos -= this.rect.size.x;
          }
        }
        for (int z = min.z; z < max.z; ++z)
        {
          float num1 = this[min.x, z];
          this.SetPos(min.x, z);
          for (int index2 = min.x + 1; index2 < max.x; ++index2)
          {
            num1 = (float) (((double) num1 + (double) this.array[this.pos]) / 2.0);
            this.array[this.pos] = num1;
            ++this.pos;
          }
          float num2 = this[max.x - 1, z];
          this.SetPos(max.x - 1, z);
          for (int index2 = max.x - 2; index2 >= min.x; --index2)
          {
            num2 = (float) (((double) num2 + (double) this.array[this.pos]) / 2.0);
            this.array[this.pos] = num2;
            --this.pos;
          }
        }
      }
      for (int index = 0; index < this.count; ++index)
        this.array[index] = copy.array[index] + this.array[index] * 2f * strength;
      float num = Mathf.Sqrt((float) iterations);
      for (int index = 0; index < this.count; ++index)
        this.array[index] /= num;
    }

    public void Spread(Func<float, float, float> spreadFn = null, int iterations = 4)
    {
      Coord min = this.rect.Min;
      Coord max = this.rect.Max;
      for (int index1 = 0; index1 < iterations; ++index1)
      {
        for (int x = min.x; x < max.x; ++x)
        {
          float num1 = this[x, min.z];
          this.SetPos(x, min.z);
          for (int index2 = min.z + 1; index2 < max.z; ++index2)
          {
            num1 = spreadFn(num1, this.array[this.pos]);
            this.array[this.pos] = num1;
            this.pos += this.rect.size.x;
          }
          float num2 = this[x, max.z - 1];
          this.SetPos(x, max.z - 1);
          for (int index2 = max.z - 2; index2 >= min.z; --index2)
          {
            num2 = spreadFn(num2, this.array[this.pos]);
            this.array[this.pos] = num2;
            this.pos -= this.rect.size.x;
          }
        }
        for (int z = min.z; z < max.z; ++z)
        {
          float num1 = this[min.x, z];
          this.SetPos(min.x, z);
          for (int index2 = min.x + 1; index2 < max.x; ++index2)
          {
            num1 = spreadFn(num1, this.array[this.pos]);
            this.array[this.pos] = num1;
            ++this.pos;
          }
          float num2 = this[max.x - 1, z];
          this.SetPos(max.x - 1, z);
          for (int index2 = max.x - 2; index2 >= min.x; --index2)
          {
            num2 = spreadFn(num2, this.array[this.pos]);
            this.array[this.pos] = num2;
            --this.pos;
          }
        }
      }
    }

    public void Blur(
      Func<float, float, float, float> blurFn = null,
      float intensity = 0.666f,
      bool additive = false,
      bool horizontal = true,
      bool vertical = true,
      Matrix reference = null)
    {
      if (reference == null)
        reference = this;
      Coord min = this.rect.Min;
      Coord max = this.rect.Max;
      if (horizontal)
      {
        for (int z = min.z; z < max.z; ++z)
        {
          this.SetPos(min.x, z);
          float num1 = reference[min.x, z];
          float num2 = num1;
          float num3 = num1;
          for (int x = min.x; x < max.x; ++x)
          {
            float num4 = num2;
            num2 = num3;
            if (x < max.x - 1)
              num3 = reference.array[this.pos + 1];
            float num5 = blurFn != null ? blurFn(num4, num2, num3) : (float) (((double) num4 + (double) num3) / 2.0);
            float num6 = (float) ((double) num2 * (1.0 - (double) intensity) + (double) num5 * (double) intensity);
            if (additive)
              this.array[this.pos] += num6;
            else
              this.array[this.pos] = num6;
            ++this.pos;
          }
        }
      }
      if (!vertical)
        return;
      for (int x = min.x; x < max.x; ++x)
      {
        this.SetPos(x, min.z);
        float num1 = reference[x, min.z];
        float num2 = num1;
        for (int z = min.z; z < max.z; ++z)
        {
          float num3 = num2;
          num2 = num1;
          if (z < max.z - 1)
            num1 = reference.array[this.pos + this.rect.size.x];
          float num4 = blurFn != null ? blurFn(num3, num2, num1) : (float) (((double) num3 + (double) num1) / 2.0);
          float num5 = (float) ((double) num2 * (1.0 - (double) intensity) + (double) num4 * (double) intensity);
          if (additive)
            this.array[this.pos] += num5;
          else
            this.array[this.pos] = num5;
          this.pos += this.rect.size.x;
        }
      }
    }

    public void LossBlur(int step = 2, bool horizontal = true, bool vertical = true, Matrix reference = null)
    {
      if (reference == null)
        reference = this;
      Coord min = this.rect.Min;
      Coord max = this.rect.Max;
      int num1 = step + step / 2;
      if (horizontal)
      {
        for (int z = min.z; z < max.z; ++z)
        {
          this.SetPos(min.x, z);
          float num2 = 0.0f;
          int num3 = 0;
          float num4 = this.array[this.pos];
          float num5 = this.array[this.pos];
          for (int x = min.x; x < max.x + num1; ++x)
          {
            if (x < max.x)
              num2 += reference.array[this.pos];
            ++num3;
            if (x % step == 0)
            {
              num5 = num4;
              if (x < max.x)
                num4 = num2 / (float) num3;
              num2 = 0.0f;
              num3 = 0;
            }
            if (x - num1 >= min.x)
            {
              float num6 = 1f * (float) (x % step) / (float) step;
              if ((double) num6 < 0.0)
                ++num6;
              this.array[this.pos - num1] = (float) ((double) num4 * (double) num6 + (double) num5 * (1.0 - (double) num6));
            }
            ++this.pos;
          }
        }
      }
      if (!vertical)
        return;
      for (int x = min.x; x < max.x; ++x)
      {
        this.SetPos(x, min.z);
        float num2 = 0.0f;
        int num3 = 0;
        float num4 = this.array[this.pos];
        float num5 = this.array[this.pos];
        for (int z = min.z; z < max.z + num1; ++z)
        {
          if (z < max.z)
            num2 += reference.array[this.pos];
          ++num3;
          if (z % step == 0)
          {
            num5 = num4;
            if (z < max.z)
              num4 = num2 / (float) num3;
            num2 = 0.0f;
            num3 = 0;
          }
          if (z - num1 >= min.z)
          {
            float num6 = 1f * (float) (z % step) / (float) step;
            if ((double) num6 < 0.0)
              ++num6;
            this.array[this.pos - num1 * this.rect.size.x] = (float) ((double) num4 * (double) num6 + (double) num5 * (1.0 - (double) num6));
          }
          this.pos += this.rect.size.x;
        }
      }
    }

    public static void BlendLayers(Matrix[] matrices, float[] opacity)
    {
      Coord min = matrices[0].rect.Min;
      Coord max = matrices[0].rect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
        {
          float num1 = 0.0f;
          for (int index = matrices.Length - 1; index >= 0; --index)
          {
            float num2 = matrices[index][x, z];
            float num3 = Mathf.Clamp01((float) ((double) num1 + (double) num2 - 1.0));
            matrices[index][x, z] = num2 - num3;
            num1 += num2 - num3;
          }
        }
      }
    }

    public static void NormalizeLayers(Matrix[] matrices, float[] opacity)
    {
      Coord min = matrices[0].rect.Min;
      Coord max = matrices[0].rect.Max;
      for (int x1 = min.x; x1 < max.x; ++x1)
      {
        for (int z1 = min.z; z1 < max.z; ++z1)
        {
          float num = 0.0f;
          for (int index = 0; index < matrices.Length; ++index)
            num += matrices[index][x1, z1];
          if ((double) num > 1.0)
          {
            for (int index = 0; index < matrices.Length; ++index)
            {
              Matrix matrix;
              int x2;
              int z2;
              (matrix = matrices[index])[x2 = x1, z2 = z1] = matrix[x2, z2] / num;
            }
          }
        }
      }
    }

    public float Fallof(int x, int z, float fallof)
    {
      if ((double) fallof < 0.0)
        return 1f;
      float num1 = (float) ((double) this.rect.size.x / 2.0 - 1.0);
      float num2 = ((float) x - ((float) this.rect.offset.x + num1)) / num1;
      float num3 = (float) ((double) this.rect.size.z / 2.0 - 1.0);
      float num4 = ((float) z - ((float) this.rect.offset.z + num3)) / num3;
      float num5 = Mathf.Clamp01((float) ((1.0 - (double) Mathf.Sqrt((float) ((double) num2 * (double) num2 + (double) num4 * (double) num4))) / (1.0 - (double) fallof)));
      return (float) (3.0 * (double) num5 * (double) num5 - 2.0 * (double) num5 * (double) num5 * (double) num5);
    }

    public static void Blend(Matrix src, Matrix dst, float factor)
    {
      if (dst.rect != src.rect)
        Debug.LogError((object) "Matrix Blend: maps have different sizes");
      for (int index = 0; index < dst.count; ++index)
        dst.array[index] = (float) ((double) dst.array[index] * (double) factor + (double) src.array[index] * (1.0 - (double) factor));
    }

    public static void Mask(Matrix src, Matrix dst, Matrix mask)
    {
      if (dst.rect != src.rect || dst.rect != mask.rect)
        Debug.LogError((object) "Matrix Mask: maps have different sizes");
      for (int index = 0; index < dst.count; ++index)
      {
        float num = mask.array[index];
        if ((double) num <= 1.0 && (double) num >= 0.0)
          dst.array[index] = (float) ((double) dst.array[index] * (double) num + (src != null ? (double) src.array[index] * (1.0 - (double) num) : 0.0));
      }
    }

    public static void SafeBorders(Matrix src, Matrix dst, int safeBorders)
    {
      Coord min = dst.rect.Min;
      Coord max = dst.rect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
        {
          float num = 1f * (float) Mathf.Min(Mathf.Min(x - min.x, max.x - x), Mathf.Min(z - min.z, max.z - z)) / (float) safeBorders;
          if ((double) num <= 1.0)
            dst[x, z] = (float) ((double) dst[x, z] * (double) num + (src != null ? (double) src[x, z] * (1.0 - (double) num) : 0.0));
        }
      }
    }

    public void Add(Matrix add)
    {
      for (int index = 0; index < this.count; ++index)
        this.array[index] += add.array[index];
    }

    public void Add(Matrix add, Matrix mask)
    {
      for (int index = 0; index < this.count; ++index)
        this.array[index] += add.array[index] * mask.array[index];
    }

    public void Add(float add)
    {
      for (int index = 0; index < this.count; ++index)
        this.array[index] += add;
    }

    public void Subtract(Matrix m)
    {
      for (int index = 0; index < this.count; ++index)
        this.array[index] -= m.array[index];
    }

    public void InvSubtract(Matrix m)
    {
      for (int index = 0; index < this.count; ++index)
        this.array[index] = m.array[index] - this.array[index];
    }

    public void ClampSubtract(Matrix m)
    {
      for (int index = 0; index < this.count; ++index)
        this.array[index] = Mathf.Clamp01(this.array[index] - m.array[index]);
    }

    public void Multiply(Matrix m)
    {
      for (int index = 0; index < this.count; ++index)
        this.array[index] *= m.array[index];
    }

    public void Multiply(float m)
    {
      for (int index = 0; index < this.count; ++index)
        this.array[index] *= m;
    }

    public bool CheckRange(float min, float max)
    {
      for (int index = 0; index < this.count; ++index)
      {
        if ((double) this.array[index] < (double) min || (double) this.array[index] > (double) max)
          return false;
      }
      return true;
    }

    public void InvertOne()
    {
      for (int index = 0; index < this.count; ++index)
        this.array[index] = 1f - this.array[index];
    }

    public void Clamp01()
    {
      for (int index = 0; index < this.count; ++index)
      {
        if ((double) this.array[index] > 1.0)
          this.array[index] = 1f;
        else if ((double) this.array[index] < 0.0)
          this.array[index] = 0.0f;
      }
    }

    public class Stacker
    {
      public CoordRect smallRect;
      public CoordRect bigRect;
      public bool preserveDetail = true;
      private Matrix downscaled;
      private Matrix upscaled;
      private Matrix difference;
      private bool isDownscaled;

      public Stacker(CoordRect smallRect, CoordRect bigRect)
      {
        this.smallRect = smallRect;
        this.bigRect = bigRect;
        this.isDownscaled = false;
        if (bigRect == smallRect)
        {
          this.upscaled = this.downscaled = new Matrix(bigRect);
        }
        else
        {
          this.downscaled = new Matrix(smallRect);
          this.upscaled = new Matrix(bigRect);
          this.difference = new Matrix(bigRect);
        }
      }

      public Matrix matrix => this.isDownscaled ? this.downscaled : this.upscaled;

      public void ToSmall()
      {
        if (this.bigRect == this.smallRect)
          return;
        Matrix upscaled = this.upscaled;
        CoordRect smallRect = this.smallRect;
        Matrix downscaled1 = this.downscaled;
        CoordRect newRect1 = smallRect;
        Matrix result1 = downscaled1;
        this.downscaled = upscaled.OutdatedResize(newRect1, result: result1);
        if (this.preserveDetail)
        {
          Matrix downscaled2 = this.downscaled;
          CoordRect bigRect = this.bigRect;
          Matrix difference = this.difference;
          CoordRect newRect2 = bigRect;
          Matrix result2 = difference;
          this.difference = downscaled2.OutdatedResize(newRect2, result: result2);
          this.difference.Blur();
          this.difference.InvSubtract(this.upscaled);
        }
        this.isDownscaled = true;
      }

      public void ToBig()
      {
        if (this.bigRect == this.smallRect)
          return;
        Matrix downscaled = this.downscaled;
        CoordRect bigRect = this.bigRect;
        Matrix upscaled = this.upscaled;
        CoordRect newRect = bigRect;
        Matrix result = upscaled;
        this.upscaled = downscaled.OutdatedResize(newRect, result: result);
        this.upscaled.Blur();
        if (this.preserveDetail)
          this.upscaled.Add(this.difference);
        this.isDownscaled = false;
      }
    }
  }
}
