using System.Collections.Generic;
using UnityEngine;

namespace ErosionBrushPlugin
{
	public static class Erosion
	{
		private struct Cross
		{
			public float c;

			public float px;

			public float nx;

			public float pz;

			public float nz;

			public float max => Mathf.Max(Mathf.Max(Mathf.Max(px, nx), Mathf.Max(pz, nz)), c);

			public float min => Mathf.Min(Mathf.Min(Mathf.Min(px, nx), Mathf.Min(pz, nz)), c);

			public float sum => c + px + nx + pz + nz;

			public float avg => (c + px + nx + pz + nz) / 5f;

			public float avgAround => (px + nx + pz + nz) / 4f;

			public float maxAround => Mathf.Max(Mathf.Max(px, nx), Mathf.Max(pz, nz));

			public float minAround => Mathf.Min(Mathf.Min(px, nx), Mathf.Min(pz, nz));

			public bool isNaN => float.IsNaN(c) || float.IsNaN(px) || float.IsNaN(pz) || float.IsNaN(nx) || float.IsNaN(nz);

			public float this[int n]
			{
				get
				{
					return n switch
					{
						0 => c, 
						1 => px, 
						2 => nx, 
						3 => pz, 
						4 => nz, 
						_ => c, 
					};
				}
				set
				{
					switch (n)
					{
					case 0:
						c = value;
						break;
					case 1:
						px = value;
						break;
					case 2:
						nx = value;
						break;
					case 3:
						pz = value;
						break;
					case 4:
						nz = value;
						break;
					default:
						c = value;
						break;
					}
				}
			}

			public Cross(float c, float px, float nx, float pz, float nz)
			{
				this.c = c;
				this.px = px;
				this.nx = nx;
				this.pz = pz;
				this.nz = nz;
			}

			public Cross(Cross src)
			{
				c = src.c;
				px = src.px;
				nx = src.nx;
				pz = src.pz;
				nz = src.nz;
			}

			public Cross(Cross c1, Cross c2)
			{
				c = c1.c * c2.c;
				px = c1.px * c2.px;
				nx = c1.nx * c2.nx;
				pz = c1.pz * c2.pz;
				nz = c1.nz * c2.nz;
			}

			public Cross(float[] m, int sizeX, int sizeZ, int i)
			{
				px = m[i - 1];
				c = m[i];
				nx = m[i + 1];
				pz = m[i - sizeX];
				nz = m[i + sizeX];
			}

			public Cross(bool[] m, int sizeX, int sizeZ, int i)
			{
				px = ((!m[i - 1]) ? 0f : 1f);
				c = ((!m[i]) ? 0f : 1f);
				nx = ((!m[i + 1]) ? 0f : 1f);
				pz = ((!m[i - sizeX]) ? 0f : 1f);
				nz = ((!m[i + sizeX]) ? 0f : 1f);
			}

			public Cross(Matrix m, int i)
			{
				px = m.array[i - 1];
				c = m.array[i];
				nx = m.array[i + 1];
				pz = m.array[i - m.rect.size.x];
				nz = m.array[i + m.rect.size.x];
			}

			public void ToMatrix(float[] m, int sizeX, int sizeZ, int i)
			{
				m[i - 1] = px;
				m[i] = c;
				m[i + 1] = nx;
				m[i - sizeX] = pz;
				m[i + sizeX] = nz;
			}

			public void ToMatrix(Matrix m, int i)
			{
				m.array[i - 1] = px;
				m.array[i] = c;
				m.array[i + 1] = nx;
				m.array[i - m.rect.size.x] = pz;
				m.array[i + m.rect.size.x] = nz;
			}

			public void Percent()
			{
				float num = c + px + nx + pz + nz;
				if (num > 1E-05f)
				{
					c /= num;
					px /= num;
					nx /= num;
					pz /= num;
					nz /= num;
				}
				else
				{
					c = 0f;
					px = 0f;
					nx = 0f;
					pz = 0f;
					nz = 0f;
				}
			}

			public void ClampPositive()
			{
				c = ((!(c < 0f)) ? c : 0f);
				px = ((!(px < 0f)) ? px : 0f);
				nx = ((!(nx < 0f)) ? nx : 0f);
				pz = ((!(pz < 0f)) ? pz : 0f);
				nz = ((!(nz < 0f)) ? nz : 0f);
			}

			public void Multiply(Cross c2)
			{
				c *= c2.c;
				px *= c2.px;
				nx *= c2.nx;
				pz *= c2.pz;
				nz *= c2.nz;
			}

			public void Multiply(float f)
			{
				c *= f;
				px *= f;
				nx *= f;
				pz *= f;
				nz *= f;
			}

			public void Add(Cross c2)
			{
				c += c2.c;
				px += c2.px;
				nx += c2.nx;
				pz += c2.pz;
				nz += c2.nz;
			}

			public void Divide(Cross c2)
			{
				c /= c2.c;
				px /= c2.px;
				nx /= c2.nx;
				pz /= c2.pz;
				nz /= c2.nz;
			}

			public void Divide(float f)
			{
				c /= f;
				px /= f;
				nx /= f;
				pz /= f;
				nz /= f;
			}

			public void Subtract(float f)
			{
				c -= f;
				px -= f;
				nx -= f;
				pz -= f;
				nz -= f;
			}

			public void SubtractInverse(float f)
			{
				c = f - c;
				px = f - px;
				nx = f - nx;
				pz = f - pz;
				nz = f - nz;
			}

			public float GetMultipliedMax(Cross c2)
			{
				return Mathf.Max(Mathf.Max(Mathf.Max(px * c2.px, nx * c2.nx), Mathf.Max(pz * c2.pz, nz * c2.nz)), c * c2.c);
			}

			public float GetMultipliedSum(Cross c2)
			{
				return c * c2.c + px * c2.px + nx * c2.nx + pz * c2.pz + nz * c2.nz;
			}

			public void SortByHeight(int[] sorted)
			{
				for (int i = 0; i < 5; i++)
				{
					sorted[i] = i;
				}
				for (int j = 0; j < 5; j++)
				{
					for (int k = 0; k < 4; k++)
					{
						if (this[sorted[k]] > this[sorted[k + 1]])
						{
							int num = sorted[k];
							sorted[k] = sorted[k + 1];
							sorted[k + 1] = num;
						}
					}
				}
			}

			public IEnumerable<int> Sorted()
			{
				float _c = c;
				float _px = px;
				float _nx = nx;
				float _pz = pz;
				float _nz = nz;
				if (c > px && c > nx && c > pz && c > nz)
				{
					_c = 0f;
					yield return 0;
					if (px > nx && px > pz && px > nz)
					{
						_px = 0f;
						yield return 1;
					}
					else if (nx > px && nx > pz && nx > nz)
					{
						_nx = 0f;
						yield return 2;
					}
					else if (pz > px && pz > nx && pz > nz)
					{
						_pz = 0f;
						yield return 3;
					}
					else if (nz > px && nz > nx && nz > pz)
					{
						_nz = 0f;
						yield return 4;
					}
				}
				if (px > c && px > nx && px > pz && px > nz)
				{
					_px = 0f;
					yield return 1;
					if (c > nx && c > pz && c > nz)
					{
						_c = 0f;
						yield return 0;
					}
					else if (nx > c && nx > pz && nx > nz)
					{
						_nx = 0f;
						yield return 2;
					}
					else if (pz > c && pz > nx && pz > nz)
					{
						_pz = 0f;
						yield return 3;
					}
					else if (nz > c && nz > nx && nz > pz)
					{
						_nz = 0f;
						yield return 4;
					}
				}
				if (nx > c && nx > px && nx > pz && nx > nz)
				{
					_nx = 0f;
					yield return 2;
					if (c > px && c > pz && c > nz)
					{
						_c = 0f;
						yield return 0;
					}
					else if (px > c && px > pz && px > nz)
					{
						_px = 0f;
						yield return 1;
					}
					else if (pz > c && pz > px && pz > nz)
					{
						_pz = 0f;
						yield return 3;
					}
					else if (nz > c && nz > px && nz > pz)
					{
						_nz = 0f;
						yield return 4;
					}
				}
				if (pz > c && pz > px && pz > nx && pz > nz)
				{
					_pz = 0f;
					yield return 3;
					if (c > px && c > nx && c > nz)
					{
						_c = 0f;
						yield return 0;
					}
					else if (px > c && px > nx && px > nz)
					{
						_px = 0f;
						yield return 1;
					}
					else if (nx > c && nx > px && nx > nz)
					{
						_nx = 0f;
						yield return 2;
					}
					else if (nz > c && nz > px && nz > nx)
					{
						_nz = 0f;
						yield return 4;
					}
				}
				if (nz > c && nz > px && nz > nx && nz > pz)
				{
					_nz = 0f;
					yield return 4;
					if (c > px && c > nx && c > pz)
					{
						_c = 0f;
						yield return 0;
					}
					else if (px > c && px > nx && px > pz)
					{
						_px = 0f;
						yield return 1;
					}
					else if (nx > c && nx > px && nx > pz)
					{
						_nx = 0f;
						yield return 2;
					}
					else if (pz > c && pz > px && pz > nx)
					{
						_pz = 0f;
						yield return 3;
					}
				}
				for (int i = 0; i < 3; i++)
				{
					if (_c > _px && _c > _nx && _c > _pz && _c > _nz)
					{
						_c = 0f;
						yield return 0;
					}
					else if (_px > _c && _px > _nx && _px > _pz && _px > _nz)
					{
						_px = 0f;
						yield return 1;
					}
					else if (_nx > _c && _nx > _px && _nx > _pz && _nx > _nz)
					{
						_nx = 0f;
						yield return 2;
					}
					else if (_pz > _c && _pz > _px && _pz > _nx && _pz > _nz)
					{
						_pz = 0f;
						yield return 3;
					}
					else if (_nz > _c && _nz > _px && _nz > _nx && _nz > _pz)
					{
						_nz = 0f;
						yield return 4;
					}
				}
			}

			public static Cross operator +(Cross c1, Cross c2)
			{
				return new Cross(c1.c + c2.c, c1.px + c2.px, c1.nx + c2.nx, c1.pz + c2.pz, c1.nz + c2.nz);
			}

			public static Cross operator +(Cross c1, float f)
			{
				return new Cross(c1.c + f, c1.px + f, c1.nx + f, c1.pz + f, c1.nz + f);
			}

			public static Cross operator -(Cross c1, Cross c2)
			{
				return new Cross(c1.c - c2.c, c1.px - c2.px, c1.nx - c2.nx, c1.pz - c2.pz, c1.nz - c2.nz);
			}

			public static Cross operator -(float f, Cross c2)
			{
				return new Cross(f - c2.c, f - c2.px, f - c2.nx, f - c2.pz, f - c2.nz);
			}

			public static Cross operator -(Cross c1, float f)
			{
				return new Cross(c1.c - f, c1.px - f, c1.nx - f, c1.pz - f, c1.nz - f);
			}

			public static Cross operator *(Cross c1, Cross c2)
			{
				return new Cross(c1.c * c2.c, c1.px * c2.px, c1.nx * c2.nx, c1.pz * c2.pz, c1.nz * c2.nz);
			}

			public static Cross operator *(float f, Cross c2)
			{
				return new Cross(f * c2.c, f * c2.px, f * c2.nx, f * c2.pz, f * c2.nz);
			}

			public static Cross operator *(Cross c1, float f)
			{
				return new Cross(c1.c * f, c1.px * f, c1.nx * f, c1.pz * f, c1.nz * f);
			}

			public static Cross operator /(Cross c1, float f)
			{
				if (f > 1E-05f)
				{
					return new Cross(c1.c / f, c1.px / f, c1.nx / f, c1.pz / f, c1.nz / f);
				}
				return new Cross(0f, 0f, 0f, 0f, 0f);
			}

			public Cross PercentObsolete()
			{
				float num = c + px + nx + pz + nz;
				if (num > 1E-05f)
				{
					return new Cross(c / num, px / num, nx / num, pz / num, nz / num);
				}
				return new Cross(0f, 0f, 0f, 0f, 0f);
			}

			public Cross ClampPositiveObsolete()
			{
				return new Cross((!(c < 0f)) ? c : 0f, (!(px < 0f)) ? px : 0f, (!(nx < 0f)) ? nx : 0f, (!(pz < 0f)) ? pz : 0f, (!(nz < 0f)) ? nz : 0f);
			}
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

			public bool isNaN => float.IsNaN(c) || float.IsNaN(px) || float.IsNaN(pz) || float.IsNaN(nx) || float.IsNaN(nz) || float.IsNaN(pxpz) || float.IsNaN(pxnz) || float.IsNaN(nxpz) || float.IsNaN(nxnz);

			public float max => Mathf.Max(Mathf.Max(Mathf.Max(px, nx), Mathf.Max(pz, nz)), c);

			public float min => Mathf.Min(Mathf.Min(Mathf.Min(px, nx), Mathf.Min(pz, nz)), c);

			public float sum => c + px + nx + pz + nz;

			public MooreCross(float c, float px, float nx, float pz, float nz, float pxpz, float nxpz, float pxnz, float nxnz)
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

			public MooreCross(MooreCross src)
			{
				c = src.c;
				px = src.px;
				nx = src.nx;
				pz = src.pz;
				nz = src.nz;
				pxpz = src.pxpz;
				nxpz = src.nxpz;
				pxnz = src.pxnz;
				nxnz = src.nxnz;
			}

			public MooreCross(float[] m, int sizeX, int sizeZ, int i)
			{
				px = m[i - 1];
				c = m[i];
				nx = m[i + 1];
				pz = m[i - sizeX];
				nz = m[i + sizeX];
				pxpz = m[i - 1 - sizeX];
				nxpz = m[i + 1 - sizeX];
				pxnz = m[i - 1 + sizeX];
				nxnz = m[i + 1 + sizeX];
			}

			public MooreCross(Matrix m, int i)
			{
				px = m.array[i - 1];
				c = m.array[i];
				nx = m.array[i + 1];
				pz = m.array[i - m.rect.size.x];
				nz = m.array[i + m.rect.size.x];
				pxpz = m.array[i - 1 - m.rect.size.x];
				nxpz = m.array[i + 1 - m.rect.size.x];
				pxnz = m.array[i - 1 + m.rect.size.x];
				nxnz = m.array[i + 1 + m.rect.size.x];
			}

			public void ToMatrix(float[] m, int sizeX, int sizeZ, int i)
			{
				m[i - 1] = px;
				m[i] = c;
				m[i + 1] = nx;
				m[i - sizeX] = pz;
				m[i + sizeX] = nz;
				m[i - 1 - sizeX] = pxpz;
				m[i + 1 - sizeX] = nxpz;
				m[i - 1 + sizeX] = pxnz;
				m[i + 1 + sizeX] = nxnz;
			}

			public void ToMatrix(Matrix m, int i)
			{
				m.array[i - 1] = px;
				m.array[i] = c;
				m.array[i + 1] = nx;
				m.array[i - m.rect.size.x] = pz;
				m.array[i + m.rect.size.x] = nz;
				m.array[i - 1 - m.rect.size.x] = pxpz;
				m.array[i + 1 - m.rect.size.x] = nxpz;
				m.array[i - 1 + m.rect.size.x] = pxnz;
				m.array[i + 1 + m.rect.size.x] = nxnz;
			}

			public void Percent()
			{
				float num = c + px + nx + pz + nz + pxpz + nxpz + pxnz + nxnz;
				if (num > 1E-05f)
				{
					c /= num;
					px /= num;
					nx /= num;
					pz /= num;
					nz /= num;
					pxpz /= num;
					nxpz /= num;
					pxnz /= num;
					nxnz /= num;
				}
				else
				{
					c = 0f;
					px = 0f;
					nx = 0f;
					pz = 0f;
					nz = 0f;
					pxpz = 0f;
					nxpz = 0f;
					pxnz = 0f;
					nxnz = 0f;
				}
			}

			public override string ToString()
			{
				return "MooreCross: " + c + ", " + px + ", " + pz + ", " + nx + ", " + nz + ", " + pxpz + ", " + nxpz + ", " + pxnz + ", " + nxnz;
			}

			public void ClampPositive()
			{
				c = ((!(c < 0f)) ? c : 0f);
				px = ((!(px < 0f)) ? px : 0f);
				nx = ((!(nx < 0f)) ? nx : 0f);
				pz = ((!(pz < 0f)) ? pz : 0f);
				nz = ((!(nz < 0f)) ? nz : 0f);
				pxpz = ((!(pxpz < 0f)) ? pxpz : 0f);
				nxpz = ((!(nxpz < 0f)) ? nxpz : 0f);
				pxnz = ((!(pxnz < 0f)) ? pxnz : 0f);
				nxnz = ((!(nxnz < 0f)) ? nxnz : 0f);
			}

			public void Multiply(float f)
			{
				c *= f;
				px *= f;
				nx *= f;
				pz *= f;
				nz *= f;
				pxpz *= f;
				nxpz *= f;
				pxnz *= f;
				nxnz *= f;
			}

			public void Add(float f)
			{
				c += f;
				px += f;
				nx += f;
				pz += f;
				nz += f;
				pxpz += f;
				nxpz += f;
				pxnz += f;
				nxnz += f;
			}

			public void Add(MooreCross c2)
			{
				c += c2.c;
				px += c2.px;
				nx += c2.nx;
				pz += c2.pz;
				nz += c2.nz;
				pxpz += c2.pxpz;
				nxpz += c2.nxpz;
				pxnz += c2.pxnz;
				nxnz += c2.nxnz;
			}

			public void Subtract(float f)
			{
				c -= f;
				px -= f;
				nx -= f;
				pz -= f;
				nz -= f;
				pxpz -= f;
				nxpz -= f;
				pxnz -= f;
				nxnz -= f;
			}

			public void SubtractInverse(float f)
			{
				c = f - c;
				px = f - px;
				nx = f - nx;
				pz = f - pz;
				nz = f - nz;
				pxpz = f - pxpz;
				nxpz = f - nxpz;
				pxnz = f - pxnz;
				nxnz = f - nxnz;
			}

			public static MooreCross operator +(MooreCross c1, MooreCross c2)
			{
				return new MooreCross(c1.c + c2.c, c1.px + c2.px, c1.nx + c2.nx, c1.pz + c2.pz, c1.nz + c2.nz, c1.pxpz + c2.pxpz, c1.nxpz + c2.nxpz, c1.pxnz + c2.pxnz, c1.nxnz + c2.nxnz);
			}

			public static MooreCross operator +(MooreCross c1, float f)
			{
				return new MooreCross(c1.c + f, c1.px + f, c1.nx + f, c1.pz + f, c1.nz + f, c1.pxpz + f, c1.nxpz + f, c1.pxnz + f, c1.nxnz + f);
			}

			public static MooreCross operator -(MooreCross c1, MooreCross c2)
			{
				return new MooreCross(c1.c - c2.c, c1.px - c2.px, c1.nx - c2.nx, c1.pz - c2.pz, c1.nz - c2.nz, c1.pxpz - c2.pxpz, c1.nxpz - c2.nxpz, c1.pxnz - c2.pxnz, c1.nxnz - c2.nxnz);
			}

			public static MooreCross operator -(float f, MooreCross c2)
			{
				return new MooreCross(f - c2.c, f - c2.px, f - c2.nx, f - c2.pz, f - c2.nz, f - c2.pxpz, f - c2.nxpz, f - c2.pxnz, f - c2.nxnz);
			}

			public static MooreCross operator -(MooreCross c1, float f)
			{
				return new MooreCross(c1.c - f, c1.px - f, c1.nx - f, c1.pz - f, c1.nz - f, c1.pxpz - f, c1.nxpz - f, c1.pxnz - f, c1.nxnz - f);
			}

			public static MooreCross operator *(MooreCross c1, MooreCross c2)
			{
				return new MooreCross(c1.c * c2.c, c1.px * c2.px, c1.nx * c2.nx, c1.pz * c2.pz, c1.nz * c2.nz, c1.pxpz * c2.pxpz, c1.nxpz * c2.nxpz, c1.pxnz * c2.pxnz, c1.nxnz * c2.nxnz);
			}

			public static MooreCross operator *(float f, MooreCross c2)
			{
				return new MooreCross(f * c2.c, f * c2.px, f * c2.nx, f * c2.pz, f * c2.nz, f * c2.pxpz, f * c2.nxpz, f * c2.pxnz, f * c2.nxnz);
			}

			public static MooreCross operator *(MooreCross c1, float f)
			{
				return new MooreCross(c1.c * f, c1.px * f, c1.nx * f, c1.pz * f, c1.nz * f, c1.pxpz * f, c1.nxpz * f, c1.pxnz * f, c1.nxnz * f);
			}

			public static MooreCross operator /(MooreCross c1, float f)
			{
				if (f > 1E-05f)
				{
					return new MooreCross(c1.c / f, c1.px / f, c1.nx / f, c1.pz / f, c1.nz / f, c1.pxpz / f, c1.nxpz / f, c1.pxnz / f, c1.nxnz / f);
				}
				return new MooreCross(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
			}

			public MooreCross PercentObsolete()
			{
				float num = c + px + nx + pz + nz + pxpz + nxpz + pxnz + nxnz;
				if (num > 1E-05f)
				{
					return new MooreCross(c / num, px / num, nx / num, pz / num, nz / num, pxpz / num, nxpz / num, pxnz / num, nxnz / num);
				}
				return new MooreCross(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
			}

			public MooreCross ClampPositiveObsolete()
			{
				return new MooreCross((!(c < 0f)) ? c : 0f, (!(px < 0f)) ? px : 0f, (!(nx < 0f)) ? nx : 0f, (!(pz < 0f)) ? pz : 0f, (!(nz < 0f)) ? nz : 0f, (!(pxpz < 0f)) ? pxpz : 0f, (!(nxpz < 0f)) ? nxpz : 0f, (!(pxnz < 0f)) ? pxnz : 0f, (!(nxnz < 0f)) ? nxnz : 0f);
			}
		}

		public static void ErosionIteration(Matrix heights, Matrix erosion, Matrix sedimentSum, CoordRect area = default(CoordRect), float erosionDurability = 0.9f, float erosionAmount = 1f, float sedimentAmount = 0.5f, int erosionFluidityIterations = 3, float ruffle = 0.1f, Matrix torrents = null, Matrix sediments = null, int[] stepsArray = null, int[] heightsInt = null, int[] order = null)
		{
			if (area.isZero)
			{
				area = heights.rect;
			}
			int count = heights.count;
			int num = 12345;
			int num2 = 1000000;
			if (heightsInt == null)
			{
				heightsInt = new int[count];
			}
			for (int i = 0; i < heights.count; i++)
			{
				heightsInt[i] = (int)(Mathf.Clamp01(heights.array[i]) * (float)num2);
			}
			if (order == null)
			{
				order = new int[count];
			}
			int[] array = heightsInt;
			int[] order2 = order;
			int count2 = heights.count;
			order = Extensions.ArrayOrder(array, order2, count2, 1000000, stepsArray);
			for (int j = 0; j < heights.count; j++)
			{
				int num3 = order[j];
				Coord coord = heights.rect.CoordByNum(num3);
				if (!area.CheckInRangeAndBounds(coord))
				{
					order[j] = -1;
				}
			}
			if (torrents == null)
			{
				torrents = new Matrix(heights.rect);
			}
			torrents.ChangeRect(heights.rect);
			torrents.Fill(1f);
			for (int num4 = count - 1; num4 >= 0; num4--)
			{
				int num5 = order[num4];
				if (num5 >= 0)
				{
					float[] array2 = heights.array;
					int num6 = num5;
					int x = heights.rect.size.x;
					float num7 = array2[num6];
					float num8 = array2[num6 - 1];
					float num9 = array2[num6 + 1];
					float num10 = array2[num6 - x];
					float num11 = array2[num6 + x];
					float num12 = array2[num6 - 1 - x];
					float num13 = array2[num6 + 1 - x];
					float num14 = array2[num6 - 1 + x];
					float num15 = array2[num6 + 1 + x];
					float num16 = num7 - num7;
					float num17 = num7 - num8;
					float num18 = num7 - num9;
					float num19 = num7 - num10;
					float num20 = num7 - num11;
					float num21 = num7 - num12;
					float num22 = num7 - num13;
					float num23 = num7 - num14;
					float num24 = num7 - num15;
					num16 = ((!(num16 > 0f)) ? 0f : num16);
					num17 = ((!(num17 > 0f)) ? 0f : num17);
					num18 = ((!(num18 > 0f)) ? 0f : num18);
					num19 = ((!(num19 > 0f)) ? 0f : num19);
					num20 = ((!(num20 > 0f)) ? 0f : num20);
					num21 = ((!(num21 > 0f)) ? 0f : num21);
					num22 = ((!(num22 > 0f)) ? 0f : num22);
					num23 = ((!(num23 > 0f)) ? 0f : num23);
					num24 = ((!(num24 > 0f)) ? 0f : num24);
					float num25 = 0f;
					float num26 = 0f;
					float num27 = 0f;
					float num28 = 0f;
					float num29 = 0f;
					float num30 = 0f;
					float num31 = 0f;
					float num32 = 0f;
					float num33 = 0f;
					float num34 = num16 + num17 + num18 + num19 + num20 + num21 + num22 + num23 + num24;
					if (num34 > 1E-05f)
					{
						num25 = num16 / num34;
						num26 = num17 / num34;
						num27 = num18 / num34;
						num28 = num19 / num34;
						num29 = num20 / num34;
						num30 = num21 / num34;
						num31 = num22 / num34;
						num32 = num23 / num34;
						num33 = num24 / num34;
					}
					float num35 = torrents.array[num6];
					if (num35 > 2E+09f)
					{
						num35 = 2E+09f;
					}
					array2 = torrents.array;
					array2[num6] += num35 * num25;
					array2[num6 - 1] += num35 * num26;
					array2[num6 + 1] += num35 * num27;
					array2[num6 - x] += num35 * num28;
					array2[num6 + x] += num35 * num29;
					array2[num6 - 1 - x] += num35 * num30;
					array2[num6 + 1 - x] += num35 * num31;
					array2[num6 - 1 + x] += num35 * num32;
					array2[num6 + 1 + x] += num35 * num33;
				}
			}
			if (sediments == null)
			{
				sediments = new Matrix(heights.rect);
			}
			else
			{
				sediments.ChangeRect(heights.rect);
			}
			sediments.Clear();
			for (int num36 = count - 1; num36 >= 0; num36--)
			{
				int num37 = order[num36];
				if (num37 >= 0)
				{
					float[] array3 = heights.array;
					int num38 = num37;
					int x2 = heights.rect.size.x;
					float num39 = array3[num38];
					float num40 = array3[num38 - 1];
					float num41 = array3[num38 + 1];
					float num42 = array3[num38 - x2];
					float num43 = array3[num38 + x2];
					float num44 = num39;
					if (num40 < num44)
					{
						num44 = num40;
					}
					if (num41 < num44)
					{
						num44 = num41;
					}
					if (num42 < num44)
					{
						num44 = num42;
					}
					if (num43 < num44)
					{
						num44 = num43;
					}
					float num45 = (num39 + num44) / 2f;
					if (!(num39 < num45))
					{
						float num46 = num39 - num45;
						float num47 = num46 * (torrents.array[num37] - 1f) * (1f - erosionDurability);
						if (num46 > num47)
						{
							num46 = num47;
						}
						num46 *= erosionAmount;
						heights.array[num37] -= num46;
						sediments.array[num37] += num46 * sedimentAmount;
						if (erosion != null)
						{
							erosion.array[num37] += num46;
						}
					}
				}
			}
			for (int k = 0; k < erosionFluidityIterations; k++)
			{
				for (int num48 = count - 1; num48 >= 0; num48--)
				{
					int num49 = order[num48];
					if (num49 >= 0)
					{
						float[] array4 = heights.array;
						int x3 = heights.rect.size.x;
						float num50 = array4[num49];
						float num51 = array4[num49 - 1];
						float num52 = array4[num49 + 1];
						float num53 = array4[num49 - x3];
						float num54 = array4[num49 + x3];
						array4 = sediments.array;
						float num55 = array4[num49];
						float num56 = array4[num49 - 1];
						float num57 = array4[num49 + 1];
						float num58 = array4[num49 - x3];
						float num59 = array4[num49 + x3];
						float num60 = num55 + num56 + num57 + num58 + num59;
						if (!(num60 < 1E-05f))
						{
							float num61 = num60 / 5f;
							num55 = num61;
							num56 = num61;
							num57 = num61;
							num58 = num61;
							num59 = num61;
							float num62 = (num50 + num55 + num56 + num51) / 2f;
							if (num50 + num55 > num51 + num56)
							{
								float num63 = num55 + num50 - num62;
								if (num63 > num55)
								{
									num63 = num55;
								}
								num55 -= num63;
								num56 += num63;
							}
							else
							{
								float num64 = num56 + num51 - num62;
								if (num64 > num56)
								{
									num64 = num56;
								}
								num56 -= num64;
								num55 += num64;
							}
							num62 = (num51 + num56 + num57 + num52) / 2f;
							if (num51 + num56 > num52 + num57)
							{
								float num65 = num56 + num51 - num62;
								if (num65 > num56)
								{
									num65 = num56;
								}
								num56 -= num65;
								num57 += num65;
							}
							else
							{
								float num66 = num57 + num52 - num62;
								if (num66 > num57)
								{
									num66 = num57;
								}
								num57 -= num66;
								num56 += num66;
							}
							num62 = (num50 + num55 + num57 + num52) / 2f;
							if (num50 + num55 > num52 + num57)
							{
								float num67 = num55 + num50 - num62;
								if (num67 > num55)
								{
									num67 = num55;
								}
								num55 -= num67;
								num57 += num67;
							}
							else
							{
								float num68 = num57 + num52 - num62;
								if (num68 > num57)
								{
									num68 = num57;
								}
								num57 -= num68;
								num55 += num68;
							}
							num62 = (num50 + num55 + num58 + num53) / 2f;
							if (num50 + num55 > num53 + num58)
							{
								float num69 = num55 + num50 - num62;
								if (num69 > num55)
								{
									num69 = num55;
								}
								num55 -= num69;
								num58 += num69;
							}
							else
							{
								float num70 = num58 + num53 - num62;
								if (num70 > num58)
								{
									num70 = num58;
								}
								num58 -= num70;
								num55 += num70;
							}
							num62 = (num54 + num59 + num58 + num53) / 2f;
							if (num54 + num59 > num53 + num58)
							{
								float num71 = num59 + num54 - num62;
								if (num71 > num59)
								{
									num71 = num59;
								}
								num59 -= num71;
								num58 += num71;
							}
							else
							{
								float num72 = num58 + num53 - num62;
								if (num72 > num58)
								{
									num72 = num58;
								}
								num58 -= num72;
								num59 += num72;
							}
							num62 = (num50 + num55 + num58 + num53) / 2f;
							if (num50 + num55 > num53 + num58)
							{
								float num73 = num55 + num50 - num62;
								if (num73 > num55)
								{
									num73 = num55;
								}
								num55 -= num73;
								num58 += num73;
							}
							else
							{
								float num74 = num58 + num53 - num62;
								if (num74 > num58)
								{
									num74 = num58;
								}
								num58 -= num74;
								num55 += num74;
							}
							num62 = (num51 + num56 + num58 + num53) / 2f;
							if (num51 + num56 > num53 + num58)
							{
								float num75 = num56 + num51 - num62;
								if (num75 > num56)
								{
									num75 = num56;
								}
								num56 -= num75;
								num58 += num75;
							}
							else
							{
								float num76 = num58 + num53 - num62;
								if (num76 > num58)
								{
									num76 = num58;
								}
								num58 -= num76;
								num56 += num76;
							}
							num62 = (num52 + num57 + num59 + num54) / 2f;
							if (num52 + num57 > num54 + num59)
							{
								float num77 = num57 + num52 - num62;
								if (num77 > num57)
								{
									num77 = num57;
								}
								num57 -= num77;
								num59 += num77;
							}
							else
							{
								float num78 = num59 + num54 - num62;
								if (num78 > num59)
								{
									num78 = num59;
								}
								num59 -= num78;
								num57 += num78;
							}
							num62 = (num51 + num56 + num59 + num54) / 2f;
							if (num51 + num56 > num54 + num59)
							{
								float num79 = num56 + num51 - num62;
								if (num79 > num56)
								{
									num79 = num56;
								}
								num56 -= num79;
								num59 += num79;
							}
							else
							{
								float num80 = num59 + num54 - num62;
								if (num80 > num59)
								{
									num80 = num59;
								}
								num59 -= num80;
								num56 += num80;
							}
							num62 = (num52 + num57 + num58 + num53) / 2f;
							if (num52 + num57 > num53 + num58)
							{
								float num81 = num57 + num52 - num62;
								if (num81 > num57)
								{
									num81 = num57;
								}
								num57 -= num81;
								num58 += num81;
							}
							else
							{
								float num82 = num58 + num53 - num62;
								if (num82 > num58)
								{
									num82 = num58;
								}
								num58 -= num82;
								num57 += num82;
							}
							array4 = sediments.array;
							array4[num49] = num55;
							array4[num49 - 1] = num56;
							array4[num49 + 1] = num57;
							array4[num49 - x3] = num58;
							array4[num49 + x3] = num59;
							if (sedimentSum != null)
							{
								array4 = sedimentSum.array;
								array4[num49] += num55;
								array4[num49 - 1] += num56;
								array4[num49 + 1] += num57;
								array4[num49 - x3] += num58;
								array4[num49 + x3] += num59;
							}
						}
					}
				}
			}
			for (int num83 = count - 1; num83 >= 0; num83--)
			{
				heights.array[num83] += sediments.array[num83];
				num = 214013 * num + 2531011;
				float num84 = (float)((num >> 16) & 0x7FFF) / 32768f;
				int num85 = order[num83];
				if (num85 >= 0)
				{
					float[] array5 = heights.array;
					int x4 = heights.rect.size.x;
					float num86 = array5[num85];
					float num87 = array5[num85 - 1];
					float num88 = array5[num85 + 1];
					float num89 = array5[num85 - x4];
					float num90 = array5[num85 + x4];
					float num91 = sediments.array[num85];
					if (num91 > 0.0001f)
					{
						float num92 = num91 / 2f;
						if (num92 > 0.75f)
						{
							num92 = 0.75f;
						}
						heights.array[num85] = num86 * (1f - num92) + (num87 + num88 + num89 + num90) / 4f * num92;
					}
					else
					{
						float num93 = num87;
						if (num88 > num93)
						{
							num93 = num88;
						}
						if (num89 > num93)
						{
							num93 = num89;
						}
						if (num90 > num93)
						{
							num93 = num90;
						}
						float num94 = num87;
						if (num88 < num94)
						{
							num94 = num88;
						}
						if (num89 < num94)
						{
							num94 = num89;
						}
						if (num90 < num94)
						{
							num94 = num90;
						}
						float num95 = num84 * (num93 - num94) + num94;
						heights.array[num85] = heights.array[num85] * (1f - ruffle) + num95 * ruffle;
					}
				}
			}
		}

		private static void LevelCells(float hX, float hz, ref float sX, ref float sz)
		{
			float num = (hX + sX + sz + hz) / 2f;
			if (hX + sX > hz + sz)
			{
				float num2 = sX + hX - num;
				if (num2 > sX)
				{
					num2 = sX;
				}
				sX -= num2;
				sz += num2;
			}
			else
			{
				float num3 = sz + hz - num;
				if (num3 > sz)
				{
					num3 = sz;
				}
				sz -= num3;
				sX += num3;
			}
		}

		private static void LevelCells(float h1, float h2, float h3, ref float s1, ref float s2, ref float s3)
		{
			LevelCells(h1, h2, ref s1, ref s2);
			LevelCells(h2, h3, ref s2, ref s3);
			LevelCells(h3, h1, ref s3, ref s1);
		}

		private static void LevelCells(float h, float hx, float hX, float hz, float hZ, ref float s, ref float sx, ref float sX, ref float sz, ref float sZ)
		{
		}
	}
}
