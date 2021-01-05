using UnityEngine;

namespace ErosionBrushPlugin
{
	public class Noise
	{
		public static int seed
		{
			get
			{
				return Random.seed;
			}
			set
			{
				Random.seed = value;
			}
		}

		public static void NoiseIteration(Matrix heightsMatrix, Matrix cliffMatrix, Matrix sedimentsMatrix, float size, float amount, float uplift, float maxHeight)
		{
			Coord min = heightsMatrix.rect.Min;
			Coord max = heightsMatrix.rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float num = Fractal(i, j, size);
					num = (num - (1f - uplift)) * amount;
					heightsMatrix[i, j] += num / maxHeight;
					if (cliffMatrix != null)
					{
						float num2 = Mathf.Max(0f, num);
						cliffMatrix[i, j] = num2 * 0.1f;
					}
					if (sedimentsMatrix != null)
					{
						float num3 = Mathf.Max(0f, 0f - num);
						sedimentsMatrix[i, j] = num3 * 0.1f;
					}
				}
			}
		}

		public static void NoiseIteration(Matrix heightMatrix, Matrix cliffMatrix, Matrix sedimentsMatrix, float size, float intensity = 1f, float detail = 0.5f, Vector2 offset = default(Vector2), int seed = 12345, float uplift = 0.5f, float maxHeight = 500f)
		{
			int num = (int)(4096f / (float)heightMatrix.rect.size.x);
			int num2 = ((int)offset.x + seed * 7) % 77777;
			int num3 = ((int)offset.y + seed * 3) % 73333;
			int num4 = 1;
			float num5 = size;
			for (int i = 0; i < 100; i++)
			{
				num5 /= 2f;
				if (num5 < 1f)
				{
					break;
				}
				num4++;
			}
			Coord min = heightMatrix.rect.Min;
			Coord max = heightMatrix.rect.Max;
			for (int j = min.x; j < max.x; j++)
			{
				for (int k = min.z; k < max.z; k++)
				{
					float num6 = 0.5f;
					float num7 = size * 10f;
					float num8 = 1f;
					for (int l = 0; l < num4; l++)
					{
						float num9 = Mathf.PerlinNoise((float)((j + num2 + 1000 * (l + 1)) * num) / (num7 + 1f), (float)((k + num3 + 100 * l) * num) / (num7 + 1f));
						num9 = (num9 - 0.5f) * num8 + 0.5f;
						num6 = ((!(num9 > 0.5f)) ? (2f * num9 * num6) : (1f - 2f * (1f - num6) * (1f - num9)));
						num7 *= 0.5f;
						num8 *= detail;
					}
					if (num6 < 0f)
					{
						num6 = 0f;
					}
					if (num6 > 1f)
					{
						num6 = 1f;
					}
					float num10 = (num6 - (1f - uplift)) * intensity;
					heightMatrix[j, k] += num10 / maxHeight;
					if (cliffMatrix != null)
					{
						cliffMatrix[j, k] = ((!(num10 > 0f)) ? 0f : (num10 * 0.1f));
					}
					if (sedimentsMatrix != null)
					{
						sedimentsMatrix[j, k] = ((!(num10 < 0f)) ? 0f : ((0f - num10) * 0.1f));
					}
				}
			}
		}

		public static float Fractal(int x, int z, float size, float detail = 0.5f)
		{
			float num = 0.5f;
			float num2 = size;
			float num3 = 1f;
			int num4 = 1;
			for (int i = 0; i < 100; i++)
			{
				num2 /= 2f;
				if (num2 < 1f)
				{
					break;
				}
				num4++;
			}
			num2 = size;
			for (int j = 0; j < num4; j++)
			{
				float num5 = Mathf.PerlinNoise((float)x / (num2 + 1f), (float)z / (num2 + 1f));
				num5 = (num5 - 0.5f) * num3 + 0.5f;
				num = ((!(num5 > 0.5f)) ? (2f * num5 * num) : (1f - 2f * (1f - num) * (1f - num5)));
				num2 *= 0.5f;
				num3 *= detail;
			}
			if (num < 0f)
			{
				num = 0f;
			}
			if (num > 1f)
			{
				num = 1f;
			}
			return num;
		}
	}
}
