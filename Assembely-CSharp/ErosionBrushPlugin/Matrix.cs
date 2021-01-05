using System;
using System.IO;
using UnityEngine;

namespace ErosionBrushPlugin
{
	[Serializable]
	public class Matrix : Matrix2<float>
	{
		public class Stacker
		{
			public CoordRect smallRect;

			public CoordRect bigRect;

			public bool preserveDetail = true;

			private Matrix downscaled;

			private Matrix upscaled;

			private Matrix difference;

			private bool isDownscaled;

			public Matrix matrix
			{
				get
				{
					if (isDownscaled)
					{
						return downscaled;
					}
					return upscaled;
				}
			}

			public Stacker(CoordRect smallRect, CoordRect bigRect)
			{
				this.smallRect = smallRect;
				this.bigRect = bigRect;
				isDownscaled = false;
				if (bigRect == smallRect)
				{
					upscaled = (downscaled = new Matrix(bigRect));
					return;
				}
				downscaled = new Matrix(smallRect);
				upscaled = new Matrix(bigRect);
				difference = new Matrix(bigRect);
			}

			public void ToSmall()
			{
				if (!(bigRect == smallRect))
				{
					Matrix matrix = upscaled;
					CoordRect newRect = smallRect;
					Matrix result = downscaled;
					downscaled = matrix.OutdatedResize(newRect, 1f, result);
					if (preserveDetail)
					{
						Matrix matrix2 = downscaled;
						newRect = bigRect;
						result = difference;
						difference = matrix2.OutdatedResize(newRect, 1f, result);
						difference.Blur();
						difference.InvSubtract(upscaled);
					}
					isDownscaled = true;
				}
			}

			public void ToBig()
			{
				if (!(bigRect == smallRect))
				{
					Matrix matrix = downscaled;
					CoordRect newRect = bigRect;
					Matrix result = upscaled;
					upscaled = matrix.OutdatedResize(newRect, 1f, result);
					upscaled.Blur();
					if (preserveDetail)
					{
						upscaled.Add(difference);
					}
					isDownscaled = false;
				}
			}
		}

		public Matrix()
		{
			array = new float[0];
			rect = new CoordRect(0, 0, 0, 0);
			count = 0;
		}

		public Matrix(CoordRect rect, float[] array = null)
		{
			base.rect = rect;
			count = rect.size.x * rect.size.z;
			if (array != null && array.Length < count)
			{
				Debug.Log("Array length: " + array.Length + " is lower then matrix capacity: " + count);
			}
			if (array != null && array.Length >= count)
			{
				base.array = array;
			}
			else
			{
				base.array = new float[count];
			}
		}

		public Matrix(Coord offset, Coord size, float[] array = null)
		{
			rect = new CoordRect(offset, size);
			count = rect.size.x * rect.size.z;
			if (array != null && array.Length < count)
			{
				Debug.Log("Array length: " + array.Length + " is lower then matrix capacity: " + count);
			}
			if (array != null && array.Length >= count)
			{
				base.array = array;
			}
			else
			{
				base.array = new float[count];
			}
		}

		public float GetInterpolatedValue(Vector2 pos)
		{
			int num = Mathf.FloorToInt(pos.x);
			int num2 = Mathf.FloorToInt(pos.y);
			float num3 = pos.x - (float)num;
			float num4 = pos.y - (float)num2;
			float num5 = base[num, num2];
			float num6 = base[num + 1, num2];
			float num7 = num5 * (1f - num3) + num6 * num3;
			float num8 = base[num, num2 + 1];
			float num9 = base[num + 1, num2 + 1];
			float num10 = num8 * (1f - num3) + num9 * num3;
			return num7 * (1f - num4) + num10 * num4;
		}

		public float GetAveragedValue(int x, int z, int steps)
		{
			float num = 0f;
			int num2 = 0;
			for (int i = 0; i < steps; i++)
			{
				for (int j = 0; j < steps; j++)
				{
					if (x + i < rect.offset.x + rect.size.x && z + j < rect.offset.z + rect.size.z)
					{
						num += base[x + i, z + j];
						num2++;
					}
				}
			}
			return num / (float)num2;
		}

		public override object Clone()
		{
			return Copy();
		}

		public Matrix Copy(Matrix result = null)
		{
			if (result == null)
			{
				result = new Matrix(rect);
			}
			result.rect = rect;
			result.pos = pos;
			result.count = count;
			if (result.array.Length != array.Length)
			{
				result.array = new float[array.Length];
			}
			for (int i = 0; i < array.Length; i++)
			{
				result.array[i] = array[i];
			}
			return result;
		}

		public bool[] InRect(CoordRect area = default(CoordRect))
		{
			Matrix2<bool> matrix = new Matrix2<bool>(rect);
			CoordRect coordRect = CoordRect.Intersect(rect, area);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					matrix[i, j] = true;
				}
			}
			return matrix.array;
		}

		public void Fill(float[,] array, CoordRect arrayRect)
		{
			CoordRect coordRect = CoordRect.Intersect(rect, arrayRect);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					base[i, j] = array[j - arrayRect.offset.z, i - arrayRect.offset.x];
				}
			}
		}

		public void Pour(float[,] array, CoordRect arrayRect)
		{
			CoordRect coordRect = CoordRect.Intersect(rect, arrayRect);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					array[j - arrayRect.offset.z, i - arrayRect.offset.x] = base[i, j];
				}
			}
		}

		public void Pour(float[,,] array, int channel, CoordRect arrayRect)
		{
			CoordRect coordRect = CoordRect.Intersect(rect, arrayRect);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					array[j - arrayRect.offset.z, i - arrayRect.offset.x, channel] = base[i, j];
				}
			}
		}

		public float[,] ReadHeighmap(TerrainData data, float height = 1f)
		{
			CoordRect centerRect = CoordRect.Intersect(rect, new CoordRect(0, 0, data.heightmapResolution, data.heightmapResolution));
			float[,] heights = data.GetHeights(centerRect.offset.x, centerRect.offset.z, centerRect.size.x, centerRect.size.z);
			Coord min = centerRect.Min;
			Coord max = centerRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					base[i, j] = heights[j - min.z, i - min.x] * height;
				}
			}
			RemoveBorders(centerRect);
			return heights;
		}

		public void WriteHeightmap(TerrainData data, float[,] array = null, float brushFallof = 0.5f, bool delayLod = false)
		{
			CoordRect coordRect = CoordRect.Intersect(rect, new CoordRect(0, 0, data.heightmapResolution, data.heightmapResolution));
			if (array == null || array.Length != coordRect.size.x * coordRect.size.z)
			{
				array = new float[coordRect.size.z, coordRect.size.x];
			}
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float num = Fallof(i, j, brushFallof);
					if (!Mathf.Approximately(num, 0f))
					{
						array[j - min.z, i - min.x] = base[i, j] * num + array[j - min.z, i - min.x] * (1f - num);
					}
				}
			}
			if (delayLod)
			{
				data.SetHeightsDelayLOD(coordRect.offset.x, coordRect.offset.z, array);
			}
			else
			{
				data.SetHeights(coordRect.offset.x, coordRect.offset.z, array);
			}
		}

		public float[,,] ReadSplatmap(TerrainData data, int channel, float[,,] array = null)
		{
			CoordRect centerRect = CoordRect.Intersect(rect, new CoordRect(0, 0, data.alphamapResolution, data.alphamapResolution));
			if (array == null)
			{
				array = data.GetAlphamaps(centerRect.offset.x, centerRect.offset.z, centerRect.size.x, centerRect.size.z);
			}
			Coord min = centerRect.Min;
			Coord max = centerRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					base[i, j] = array[j - min.z, i - min.x, channel];
				}
			}
			RemoveBorders(centerRect);
			return array;
		}

		public static void AddSplatmaps(TerrainData data, Matrix[] matrices, int[] channels, float[] opacity, float[,,] array = null, float brushFallof = 0.5f)
		{
			int alphamapLayers = data.alphamapLayers;
			bool[] array2 = new bool[alphamapLayers];
			for (int i = 0; i < channels.Length; i++)
			{
				array2[channels[i]] = true;
			}
			float[] array3 = new float[alphamapLayers];
			CoordRect c = new CoordRect(size: new Coord(data.alphamapResolution, data.alphamapResolution), offset: new Coord(0, 0));
			CoordRect coordRect = CoordRect.Intersect(c, matrices[0].rect);
			if (array == null)
			{
				array = data.GetAlphamaps(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z);
			}
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int j = min.x; j < max.x; j++)
			{
				for (int k = min.z; k < max.z; k++)
				{
					float num = matrices[0].Fallof(j, k, brushFallof);
					if (Mathf.Approximately(num, 0f))
					{
						continue;
					}
					for (int l = 0; l < alphamapLayers; l++)
					{
						array3[l] = array[k - min.z, j - min.x, l];
					}
					for (int m = 0; m < matrices.Length; m++)
					{
						matrices[m][j, k] = Mathf.Max(0f, matrices[m][j, k] - array3[channels[m]]);
					}
					for (int n = 0; n < matrices.Length; n++)
					{
						matrices[n][j, k] *= num * opacity[n];
					}
					float num2 = 0f;
					for (int num3 = 0; num3 < matrices.Length; num3++)
					{
						num2 += matrices[num3][j, k];
					}
					if (num2 > 1f)
					{
						for (int num4 = 0; num4 < matrices.Length; num4++)
						{
							matrices[num4][j, k] /= num2;
						}
						num2 = 1f;
					}
					float num5 = 1f - num2;
					for (int num6 = 0; num6 < alphamapLayers; num6++)
					{
						array3[num6] *= num5;
					}
					for (int num7 = 0; num7 < matrices.Length; num7++)
					{
						array3[channels[num7]] += matrices[num7][j, k];
					}
					for (int num8 = 0; num8 < alphamapLayers; num8++)
					{
						array[k - min.z, j - min.x, num8] = array3[num8];
					}
				}
			}
			data.SetAlphamaps(coordRect.offset.x, coordRect.offset.z, array);
		}

		public void ToTexture(Texture2D texture = null, Color[] colors = null, float rangeMin = 0f, float rangeMax = 1f, bool resizeTexture = false)
		{
			if (texture == null)
			{
				texture = new Texture2D(rect.size.x, rect.size.z);
			}
			if (resizeTexture)
			{
				texture.Resize(rect.size.x, rect.size.z);
			}
			CoordRect c = new CoordRect(size: new Coord(texture.width, texture.height), offset: new Coord(0, 0));
			CoordRect coordRect = CoordRect.Intersect(c, rect);
			if (colors == null || colors.Length != coordRect.size.x * coordRect.size.z)
			{
				colors = new Color[coordRect.size.x * coordRect.size.z];
			}
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float num = base[i, j];
					num -= rangeMin;
					num /= rangeMax - rangeMin;
					float num2 = num * 256f;
					int num3 = (int)num2;
					float num4 = num2 - (float)num3;
					float num5 = (float)num3 / 256f;
					float num6 = (float)(num3 + 1) / 256f;
					int num7 = i - min.x;
					int num8 = j - min.z;
					ref Color reference = ref colors[num8 * (max.x - min.x) + num7];
					reference = new Color(num5, (!(num4 > 0.333f)) ? num5 : num6, (!(num4 > 0.666f)) ? num5 : num6);
				}
			}
			texture.SetPixels(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z, colors);
			texture.Apply();
		}

		public void FromTexture(Texture2D texture)
		{
			CoordRect c = new CoordRect(0, 0, texture.width, texture.height);
			CoordRect coordRect = CoordRect.Intersect(c, rect);
			Color[] pixels = texture.GetPixels(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					int num = i - min.x;
					int num2 = j - min.z;
					Color color = pixels[num2 * (max.x - min.x) + num];
					base[i, j] = (color.r + color.g + color.b) / 3f;
				}
			}
		}

		public void FromTextureTiled(Texture2D texture)
		{
			Color[] pixels = texture.GetPixels();
			int width = texture.width;
			int height = texture.height;
			Coord min = rect.Min;
			Coord max = rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					int num = i % width;
					if (num < 0)
					{
						num += width;
					}
					int num2 = j % height;
					if (num2 < 0)
					{
						num2 += height;
					}
					Color color = pixels[num2 * width + num];
					base[i, j] = (color.r + color.g + color.b) / 3f;
				}
			}
		}

		public Texture2D SimpleToTexture(Texture2D texture = null, Color[] colors = null, float rangeMin = 0f, float rangeMax = 1f, string savePath = null)
		{
			if (texture == null)
			{
				texture = new Texture2D(rect.size.x, rect.size.z);
			}
			if (texture.width != rect.size.x || texture.height != rect.size.z)
			{
				texture.Resize(rect.size.x, rect.size.z);
			}
			if (colors == null || colors.Length != rect.size.x * rect.size.z)
			{
				colors = new Color[rect.size.x * rect.size.z];
			}
			for (int i = 0; i < count; i++)
			{
				float num = array[i];
				num -= rangeMin;
				num /= rangeMax - rangeMin;
				ref Color reference = ref colors[i];
				reference = new Color(num, num, num);
			}
			texture.SetPixels(colors);
			texture.Apply();
			return texture;
		}

		public void SimpleFromTexture(Texture2D texture)
		{
			ChangeRect(new CoordRect(rect.offset.x, rect.offset.z, texture.width, texture.height));
			Color[] pixels = texture.GetPixels();
			for (int i = 0; i < count; i++)
			{
				Color color = pixels[i];
				array[i] = (color.r + color.g + color.b) / 3f;
			}
		}

		public void ImportRaw(string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read);
			int num = (int)Mathf.Sqrt(fileStream.Length / 2);
			byte[] array = new byte[num * num * 2];
			fileStream.Read(array, 0, array.Length);
			fileStream.Close();
			ChangeRect(new CoordRect(0, 0, num, num));
			int num2 = 0;
			Coord min = rect.Min;
			Coord max = rect.Max;
			for (int num3 = max.z - 1; num3 >= min.z; num3--)
			{
				for (int i = min.x; i < max.x; i++)
				{
					base[i, num3] = ((float)(int)array[num2 + 1] * 256f + (float)(int)array[num2]) / 65535f;
					num2 += 2;
				}
			}
		}

		public void Replicate(Matrix source, bool tile = false)
		{
			Coord min = rect.Min;
			Coord max = rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					if (source.rect.CheckInRange(i, j))
					{
						base[i, j] = source[i, j];
					}
					else if (tile)
					{
						int num = i - source.rect.offset.x;
						int num2 = j - source.rect.offset.z;
						int num3 = num % source.rect.size.x;
						int num4 = num2 % source.rect.size.z;
						if (num3 < 0)
						{
							num3 += source.rect.size.x;
						}
						if (num4 < 0)
						{
							num4 += source.rect.size.z;
						}
						int x = num3 + source.rect.offset.x;
						int z = num4 + source.rect.offset.z;
						base[i, j] = source[x, z];
					}
				}
			}
		}

		public float GetArea(int x, int z, int range)
		{
			if (range == 0)
			{
				if (x < rect.offset.x)
				{
					x = rect.offset.x;
				}
				if (x >= rect.offset.x + rect.size.x)
				{
					x = rect.offset.x + rect.size.x - 1;
				}
				if (z < rect.offset.z)
				{
					z = rect.offset.z;
				}
				if (z >= rect.offset.z + rect.size.z)
				{
					z = rect.offset.z + rect.size.z - 1;
				}
				return array[(z - rect.offset.z) * rect.size.x + x - rect.offset.x];
			}
			float num = 0f;
			int num2 = 0;
			for (int i = x - range; i <= x + range; i++)
			{
				if (i < rect.offset.x || i >= rect.offset.x + rect.size.x)
				{
					continue;
				}
				for (int j = z - range; j <= z + range; j++)
				{
					if (j >= rect.offset.z && j < rect.offset.z + rect.size.z)
					{
						num += array[(j - rect.offset.z) * rect.size.x + i - rect.offset.x];
						num2++;
					}
				}
			}
			return num / (float)num2;
		}

		public float GetInterpolated(float x, float z)
		{
			int num = (int)x;
			if (x < 0f)
			{
				num--;
			}
			if (num < rect.offset.x)
			{
				num = rect.offset.x;
			}
			int num2 = num + 1;
			if (num2 >= rect.offset.x + rect.size.x)
			{
				num2 = rect.offset.x + rect.size.x - 1;
			}
			int num3 = (int)z;
			if (z < 0f)
			{
				num3--;
			}
			if (num3 < rect.offset.z)
			{
				num3 = rect.offset.z;
			}
			int num4 = num3 + 1;
			if (num4 >= rect.offset.z + rect.size.z)
			{
				num4 = rect.offset.z + rect.size.z - 1;
			}
			float num5 = x - (float)num;
			float num6 = z - (float)num3;
			int num7 = (num3 - rect.offset.z) * rect.size.x + num - rect.offset.x;
			float num8 = array[num7];
			float num9 = array[(num3 - rect.offset.z) * rect.size.x + num2 - rect.offset.x];
			float num10 = array[(num4 - rect.offset.z) * rect.size.x + num - rect.offset.x];
			float num11 = array[(num4 - rect.offset.z) * rect.size.x + num2 - rect.offset.x];
			float num12 = num8 * (1f - num5) + num9 * num5;
			float num13 = num10 * (1f - num5) + num11 * num5;
			return num12 * (1f - num6) + num13 * num6;
		}

		public Matrix Resize(CoordRect newRect, Matrix result = null)
		{
			if (result == null)
			{
				result = new Matrix(newRect);
			}
			else
			{
				result.ChangeRect(newRect);
			}
			Coord min = result.rect.Min;
			Coord max = result.rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float num = 1f * (float)(i - result.rect.offset.x) / (float)result.rect.size.x;
					float x = num * (float)rect.size.x + (float)rect.offset.x;
					float num2 = 1f * (float)(j - result.rect.offset.z) / (float)result.rect.size.z;
					float z = num2 * (float)rect.size.z + (float)rect.offset.z;
					result[i, j] = GetInterpolated(x, z);
				}
			}
			return result;
		}

		public Matrix Downscale(int factor, Matrix result = null)
		{
			return Resize(rect / factor, result);
		}

		public Matrix Upscale(int factor, Matrix result = null)
		{
			return Resize(rect * factor, result);
		}

		public Matrix BlurredUpscale(int factor)
		{
			Matrix matrix = new Matrix(rect, new float[count * factor]);
			Matrix matrix2 = new Matrix(rect, new float[count * factor]);
			matrix.Fill(this);
			int num = Mathf.RoundToInt(Mathf.Sqrt(factor));
			for (int i = 0; i < num; i++)
			{
				matrix.Resize(matrix.rect * 2, matrix2);
				matrix.ChangeRect(matrix2.rect);
				matrix.Fill(matrix2);
				matrix.Blur(null, 0.5f);
			}
			return matrix;
		}

		public Matrix OutdatedResize(CoordRect newRect, float smoothness = 1f, Matrix result = null)
		{
			int num = newRect.size.x / rect.size.x;
			int num2 = rect.size.x / newRect.size.x;
			if (num > 1 && !newRect.Divisible(num))
			{
				Debug.LogError(string.Concat("Matrix rect ", rect, " could not be upscaled to ", newRect, " with factor ", num));
			}
			if (num2 > 1 && !rect.Divisible(num2))
			{
				Debug.LogError(string.Concat("Matrix rect ", rect, " could not be downscaled to ", newRect, " with factor ", num2));
			}
			if (num > 1)
			{
				result = OutdatedUpscale(num, result);
			}
			if (num2 > 1)
			{
				result = OutdatedDownscale(num2, smoothness, result);
			}
			if (num <= 1 && num2 <= 1)
			{
				return Copy(result);
			}
			return result;
		}

		public Matrix OutdatedUpscale(int factor, Matrix result = null)
		{
			if (result == null)
			{
				result = new Matrix(rect * factor);
			}
			result.ChangeRect(rect * factor);
			if (factor == 1)
			{
				return Copy(result);
			}
			Coord min = rect.Min;
			Coord coord = rect.Max - 1;
			float num = 1f / (float)factor;
			for (int i = min.x; i < coord.x; i++)
			{
				for (int j = min.z; j < coord.z; j++)
				{
					float a = base[i, j];
					float a2 = base[i + 1, j];
					float b = base[i, j + 1];
					float b2 = base[i + 1, j + 1];
					for (int k = 0; k < factor; k++)
					{
						for (int l = 0; l < factor; l++)
						{
							float t = (float)k * num;
							float t2 = (float)l * num;
							float a3 = Mathf.Lerp(a, b, t2);
							float b3 = Mathf.Lerp(a2, b2, t2);
							result[i * factor + k, j * factor + l] = Mathf.Lerp(a3, b3, t);
						}
					}
				}
			}
			result.RemoveBorders(0, 0, factor + 1, factor + 1);
			return result;
		}

		public float OutdatedGetInterpolated(float x, float z)
		{
			int num = (int)x;
			int num2 = (int)(x + 1f);
			if (num2 >= rect.offset.x + rect.size.x)
			{
				num2 = rect.offset.x + rect.size.x - 1;
			}
			int num3 = (int)z;
			int num4 = (int)(z + 1f);
			if (num4 >= rect.offset.z + rect.size.z)
			{
				num4 = rect.offset.z + rect.size.z - 1;
			}
			float num5 = x - (float)num;
			float num6 = z - (float)num3;
			int num7 = (num3 - rect.offset.z) * rect.size.x + num - rect.offset.x;
			float num8 = array[num7];
			float num9 = array[(num3 - rect.offset.z) * rect.size.x + num2 - rect.offset.x];
			float num10 = array[(num4 - rect.offset.z) * rect.size.x + num - rect.offset.x];
			float num11 = array[(num4 - rect.offset.z) * rect.size.x + num2 - rect.offset.x];
			float num12 = num8 * (1f - num5) + num9 * num5;
			float num13 = num10 * (1f - num5) + num11 * num5;
			return num12 * (1f - num6) + num13 * num6;
		}

		public Matrix TestResize(CoordRect newRect)
		{
			Matrix matrix = new Matrix(newRect);
			Coord min = matrix.rect.Min;
			Coord max = matrix.rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float num = 1f * (float)(i - matrix.rect.offset.x) / (float)matrix.rect.size.x;
					float x = num * (float)rect.size.x + (float)rect.offset.x;
					float num2 = 1f * (float)(j - matrix.rect.offset.z) / (float)matrix.rect.size.z;
					float z = num2 * (float)rect.size.z + (float)rect.offset.z;
					matrix[i, j] = OutdatedGetInterpolated(x, z);
				}
			}
			return matrix;
		}

		public Matrix OutdatedDownscale(int factor = 2, float smoothness = 1f, Matrix result = null)
		{
			if (!rect.Divisible(factor))
			{
				Debug.LogError(string.Concat("Matrix rect ", rect, " could not be downscaled with factor ", factor));
			}
			if (result == null)
			{
				result = new Matrix(rect / factor);
			}
			result.ChangeRect(rect / factor);
			if (factor == 1)
			{
				return Copy(result);
			}
			Coord min = rect.Min;
			Coord min2 = result.rect.Min;
			Coord max = result.rect.Max;
			if (smoothness < 0.0001f)
			{
				for (int i = min2.x; i < max.x; i++)
				{
					for (int j = min2.z; j < max.z; j++)
					{
						int x = (i - min2.x) * factor + min.x;
						int z = (j - min2.z) * factor + min.z;
						result[i, j] = base[x, z];
					}
				}
			}
			else
			{
				for (int k = min2.x; k < max.x; k++)
				{
					for (int l = min2.z; l < max.z; l++)
					{
						int num = (k - min2.x) * factor + min.x;
						int num2 = (l - min2.z) * factor + min.z;
						float num3 = 0f;
						for (int m = num; m < num + factor; m++)
						{
							for (int n = num2; n < num2 + factor; n++)
							{
								num3 += base[m, n];
							}
						}
						result[k, l] = num3 / (float)(factor * factor) * smoothness + base[num, num2] * (1f - smoothness);
					}
				}
			}
			return result;
		}

		public void Spread(float strength = 0.5f, int iterations = 4, Matrix copy = null)
		{
			Coord min = rect.Min;
			Coord max = rect.Max;
			for (int i = 0; i < count; i++)
			{
				array[i] = Mathf.Clamp(array[i], -1f, 1f);
			}
			if (copy == null)
			{
				copy = Copy();
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					copy.array[j] = array[j];
				}
			}
			for (int k = 0; k < iterations; k++)
			{
				float num = 0f;
				for (int l = min.x; l < max.x; l++)
				{
					num = base[l, min.z];
					SetPos(l, min.z);
					for (int m = min.z + 1; m < max.z; m++)
					{
						num = (num + array[pos]) / 2f;
						array[pos] = num;
						pos += rect.size.x;
					}
					num = base[l, max.z - 1];
					SetPos(l, max.z - 1);
					for (int num2 = max.z - 2; num2 >= min.z; num2--)
					{
						num = (num + array[pos]) / 2f;
						array[pos] = num;
						pos -= rect.size.x;
					}
				}
				for (int n = min.z; n < max.z; n++)
				{
					num = base[min.x, n];
					SetPos(min.x, n);
					for (int num3 = min.x + 1; num3 < max.x; num3++)
					{
						num = (num + array[pos]) / 2f;
						array[pos] = num;
						pos++;
					}
					num = base[max.x - 1, n];
					SetPos(max.x - 1, n);
					for (int num4 = max.x - 2; num4 >= min.x; num4--)
					{
						num = (num + array[pos]) / 2f;
						array[pos] = num;
						pos--;
					}
				}
			}
			for (int num5 = 0; num5 < count; num5++)
			{
				array[num5] = copy.array[num5] + array[num5] * 2f * strength;
			}
			float num6 = Mathf.Sqrt(iterations);
			for (int num7 = 0; num7 < count; num7++)
			{
				array[num7] /= num6;
			}
		}

		public void Spread(Func<float, float, float> spreadFn = null, int iterations = 4)
		{
			Coord min = rect.Min;
			Coord max = rect.Max;
			for (int i = 0; i < iterations; i++)
			{
				float num = 0f;
				for (int j = min.x; j < max.x; j++)
				{
					num = base[j, min.z];
					SetPos(j, min.z);
					for (int k = min.z + 1; k < max.z; k++)
					{
						num = spreadFn(num, array[pos]);
						array[pos] = num;
						pos += rect.size.x;
					}
					num = base[j, max.z - 1];
					SetPos(j, max.z - 1);
					for (int num2 = max.z - 2; num2 >= min.z; num2--)
					{
						num = spreadFn(num, array[pos]);
						array[pos] = num;
						pos -= rect.size.x;
					}
				}
				for (int l = min.z; l < max.z; l++)
				{
					num = base[min.x, l];
					SetPos(min.x, l);
					for (int m = min.x + 1; m < max.x; m++)
					{
						num = spreadFn(num, array[pos]);
						array[pos] = num;
						pos++;
					}
					num = base[max.x - 1, l];
					SetPos(max.x - 1, l);
					for (int num3 = max.x - 2; num3 >= min.x; num3--)
					{
						num = spreadFn(num, array[pos]);
						array[pos] = num;
						pos--;
					}
				}
			}
		}

		public void Blur(Func<float, float, float, float> blurFn = null, float intensity = 0.666f, bool additive = false, bool horizontal = true, bool vertical = true, Matrix reference = null)
		{
			if (reference == null)
			{
				reference = this;
			}
			Coord min = rect.Min;
			Coord max = rect.Max;
			if (horizontal)
			{
				for (int i = min.z; i < max.z; i++)
				{
					SetPos(min.x, i);
					float num = reference[min.x, i];
					float num2 = num;
					float num3 = num;
					float num4 = 0f;
					for (int j = min.x; j < max.x; j++)
					{
						num = num2;
						num2 = num3;
						if (j < max.x - 1)
						{
							num3 = reference.array[pos + 1];
						}
						num4 = blurFn?.Invoke(num, num2, num3) ?? ((num + num3) / 2f);
						num4 = num2 * (1f - intensity) + num4 * intensity;
						if (additive)
						{
							array[pos] += num4;
						}
						else
						{
							array[pos] = num4;
						}
						pos++;
					}
				}
			}
			if (!vertical)
			{
				return;
			}
			for (int k = min.x; k < max.x; k++)
			{
				SetPos(k, min.z);
				float num5 = reference[k, min.z];
				float num6 = num5;
				float num7 = num5;
				float num8 = num5;
				for (int l = min.z; l < max.z; l++)
				{
					num7 = num6;
					num6 = num5;
					if (l < max.z - 1)
					{
						num5 = reference.array[pos + rect.size.x];
					}
					num8 = blurFn?.Invoke(num7, num6, num5) ?? ((num7 + num5) / 2f);
					num8 = num6 * (1f - intensity) + num8 * intensity;
					if (additive)
					{
						array[pos] += num8;
					}
					else
					{
						array[pos] = num8;
					}
					pos += rect.size.x;
				}
			}
		}

		public void LossBlur(int step = 2, bool horizontal = true, bool vertical = true, Matrix reference = null)
		{
			if (reference == null)
			{
				reference = this;
			}
			Coord min = rect.Min;
			Coord max = rect.Max;
			int num = step + step / 2;
			if (horizontal)
			{
				for (int i = min.z; i < max.z; i++)
				{
					SetPos(min.x, i);
					float num2 = 0f;
					int num3 = 0;
					float num4 = array[pos];
					float num5 = array[pos];
					for (int j = min.x; j < max.x + num; j++)
					{
						if (j < max.x)
						{
							num2 += reference.array[pos];
						}
						num3++;
						if (j % step == 0)
						{
							num5 = num4;
							if (j < max.x)
							{
								num4 = num2 / (float)num3;
							}
							num2 = 0f;
							num3 = 0;
						}
						if (j - num >= min.x)
						{
							float num6 = 1f * (float)(j % step) / (float)step;
							if (num6 < 0f)
							{
								num6 += 1f;
							}
							array[pos - num] = num4 * num6 + num5 * (1f - num6);
						}
						pos++;
					}
				}
			}
			if (!vertical)
			{
				return;
			}
			for (int k = min.x; k < max.x; k++)
			{
				SetPos(k, min.z);
				float num7 = 0f;
				int num8 = 0;
				float num9 = array[pos];
				float num10 = array[pos];
				for (int l = min.z; l < max.z + num; l++)
				{
					if (l < max.z)
					{
						num7 += reference.array[pos];
					}
					num8++;
					if (l % step == 0)
					{
						num10 = num9;
						if (l < max.z)
						{
							num9 = num7 / (float)num8;
						}
						num7 = 0f;
						num8 = 0;
					}
					if (l - num >= min.z)
					{
						float num11 = 1f * (float)(l % step) / (float)step;
						if (num11 < 0f)
						{
							num11 += 1f;
						}
						array[pos - num * rect.size.x] = num9 * num11 + num10 * (1f - num11);
					}
					pos += rect.size.x;
				}
			}
		}

		public static void BlendLayers(Matrix[] matrices, float[] opacity)
		{
			Coord min = matrices[0].rect.Min;
			Coord max = matrices[0].rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float num = 0f;
					for (int num2 = matrices.Length - 1; num2 >= 0; num2--)
					{
						float num3 = matrices[num2][i, j];
						float num4 = Mathf.Clamp01(num + num3 - 1f);
						matrices[num2][i, j] = num3 - num4;
						num += num3 - num4;
					}
				}
			}
		}

		public static void NormalizeLayers(Matrix[] matrices, float[] opacity)
		{
			Coord min = matrices[0].rect.Min;
			Coord max = matrices[0].rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float num = 0f;
					for (int k = 0; k < matrices.Length; k++)
					{
						num += matrices[k][i, j];
					}
					if (num > 1f)
					{
						for (int l = 0; l < matrices.Length; l++)
						{
							matrices[l][i, j] /= num;
						}
					}
				}
			}
		}

		public float Fallof(int x, int z, float fallof)
		{
			if (fallof < 0f)
			{
				return 1f;
			}
			float num = (float)rect.size.x / 2f - 1f;
			float num2 = ((float)x - ((float)rect.offset.x + num)) / num;
			float num3 = (float)rect.size.z / 2f - 1f;
			float num4 = ((float)z - ((float)rect.offset.z + num3)) / num3;
			float num5 = Mathf.Sqrt(num2 * num2 + num4 * num4);
			float num6 = Mathf.Clamp01((1f - num5) / (1f - fallof));
			return 3f * num6 * num6 - 2f * num6 * num6 * num6;
		}

		public static void Blend(Matrix src, Matrix dst, float factor)
		{
			if (dst.rect != src.rect)
			{
				Debug.LogError("Matrix Blend: maps have different sizes");
			}
			for (int i = 0; i < dst.count; i++)
			{
				dst.array[i] = dst.array[i] * factor + src.array[i] * (1f - factor);
			}
		}

		public static void Mask(Matrix src, Matrix dst, Matrix mask)
		{
			if (dst.rect != src.rect || dst.rect != mask.rect)
			{
				Debug.LogError("Matrix Mask: maps have different sizes");
			}
			for (int i = 0; i < dst.count; i++)
			{
				float num = mask.array[i];
				if (!(num > 1f) && !(num < 0f))
				{
					dst.array[i] = dst.array[i] * num + ((src != null) ? (src.array[i] * (1f - num)) : 0f);
				}
			}
		}

		public static void SafeBorders(Matrix src, Matrix dst, int safeBorders)
		{
			Coord min = dst.rect.Min;
			Coord max = dst.rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					int num = Mathf.Min(Mathf.Min(i - min.x, max.x - i), Mathf.Min(j - min.z, max.z - j));
					float num2 = 1f * (float)num / (float)safeBorders;
					if (!(num2 > 1f))
					{
						dst[i, j] = dst[i, j] * num2 + ((src != null) ? (src[i, j] * (1f - num2)) : 0f);
					}
				}
			}
		}

		public void Add(Matrix add)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] += add.array[i];
			}
		}

		public void Add(Matrix add, Matrix mask)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] += add.array[i] * mask.array[i];
			}
		}

		public void Add(float add)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] += add;
			}
		}

		public void Subtract(Matrix m)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] -= m.array[i];
			}
		}

		public void InvSubtract(Matrix m)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] = m.array[i] - array[i];
			}
		}

		public void ClampSubtract(Matrix m)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] = Mathf.Clamp01(array[i] - m.array[i]);
			}
		}

		public void Multiply(Matrix m)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] *= m.array[i];
			}
		}

		public void Multiply(float m)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] *= m;
			}
		}

		public bool CheckRange(float min, float max)
		{
			for (int i = 0; i < count; i++)
			{
				if (array[i] < min || array[i] > max)
				{
					return false;
				}
			}
			return true;
		}

		public void InvertOne()
		{
			for (int i = 0; i < count; i++)
			{
				array[i] = 1f - array[i];
			}
		}

		public void Clamp01()
		{
			for (int i = 0; i < count; i++)
			{
				if (array[i] > 1f)
				{
					array[i] = 1f;
				}
				else if (array[i] < 0f)
				{
					array[i] = 0f;
				}
			}
		}
	}
}
