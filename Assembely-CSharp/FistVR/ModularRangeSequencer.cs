using System;
using UnityEngine;

namespace FistVR
{
	public class ModularRangeSequencer : MonoBehaviour
	{
		public enum RangeSequencerState
		{
			NotRunning,
			Warmup,
			InWave,
			ReloadTime,
			SequenceOver
		}

		public enum RangeSequencerType
		{
			Speed
		}

		protected ModularRangeMaster m_master;

		protected ModularRangeSequenceDefinition m_sequenceDefinition;

		protected ModularRangeSequenceDefinition.WaveDefinition[] m_waves;

		protected int curWave;

		protected float m_warmupTickDown;

		protected float m_waveTickDown;

		protected float m_wavelength;

		protected float m_reloadTickDown;

		protected float m_reloadLength;

		protected Vector3[] m_spawnPositions;

		protected bool[] m_isNoShoot;

		protected int m_curTarget;

		protected int m_score;

		protected RangeSequencerState m_state;

		public RangeSequencerType Type;

		public ModularRangeSequenceDefinition SequenceDefinition => m_sequenceDefinition;

		public virtual void BeginSequence(ModularRangeMaster master, ModularRangeSequenceDefinition sequenceDef)
		{
			m_master = master;
			m_sequenceDefinition = sequenceDef;
			m_waves = m_sequenceDefinition.Waves;
		}

		public virtual void AbortSequence()
		{
			CancelInvoke();
			ClearSpawnedTargets();
			m_state = RangeSequencerState.NotRunning;
		}

		public virtual void SequenceOver()
		{
			ClearSpawnedTargets();
			m_state = RangeSequencerState.SequenceOver;
			m_master.GoToHighScoreBoard();
		}

		public virtual void Update()
		{
		}

		public virtual void GenerateSpawnPositions()
		{
			int num = m_waves[curWave].TargetNum + m_waves[curWave].NumNoShootTarget;
			m_spawnPositions = new Vector3[num];
			m_isNoShoot = new bool[num];
			for (int i = 0; i < m_isNoShoot.Length; i++)
			{
				m_isNoShoot[i] = false;
			}
			for (int j = 0; j < m_waves[curWave].NumNoShootTarget; j++)
			{
				Debug.Log("Target set to noshoot");
				m_isNoShoot[j] = true;
			}
			ShuffleNoShoot();
			ShuffleNoShoot();
			switch (m_waves[curWave].Layout)
			{
			case ModularRangeSequenceDefinition.TargetLayout.HorizontalLeft:
			{
				for (int num33 = 0; num33 < num; num33++)
				{
					float x3 = 0f;
					float z8 = m_waves[curWave].Distance;
					if (num > 1)
					{
						float num34 = Mathf.Min(4f, num);
						x3 = (float)num33 / ((float)num - 1f) * num34 - num34 * 0.5f;
						z8 = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)num33 / ((float)num - 1f));
					}
					Vector3 vector8 = new Vector3(x3, 1f, z8);
					m_spawnPositions[num33] = vector8;
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetLayout.HorizontalRight:
			{
				for (int num44 = 0; num44 < num; num44++)
				{
					float num45 = 0f;
					float z12 = m_waves[curWave].Distance;
					if (num > 1)
					{
						float num46 = Mathf.Min(4f, num);
						num45 = (float)num44 / ((float)num - 1f) * num46 - num46 * 0.5f;
						z12 = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)num44 / ((float)num - 1f));
					}
					Vector3 vector12 = new Vector3(0f - num45, 1f, z12);
					m_spawnPositions[num44] = vector12;
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetLayout.VerticalUp:
			{
				for (int n = 0; n < num; n++)
				{
					float num8 = 0f;
					float z4 = m_waves[curWave].Distance;
					if (num > 1)
					{
						float num9 = Mathf.Min(4f, num);
						num8 = (float)n / ((float)num - 1f) * num9 - num9 * 0.5f;
						z4 = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)n / ((float)num - 1f));
					}
					Vector3 vector4 = new Vector3(0f, num8 + 1f, z4);
					m_spawnPositions[n] = vector4;
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetLayout.VerticalDown:
			{
				for (int num38 = 0; num38 < num; num38++)
				{
					float num39 = 0f;
					float z10 = m_waves[curWave].Distance;
					if (num > 1)
					{
						float num40 = Mathf.Min(4f, num);
						num39 = (float)num38 / ((float)num - 1f) * num40 - num40 * 0.5f;
						z10 = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)num38 / ((float)num - 1f));
					}
					Vector3 vector10 = new Vector3(0f, 0f - num39 + 1f, z10);
					m_spawnPositions[num38] = vector10;
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetLayout.DiagonalLeftUp:
			{
				for (int num20 = 0; num20 < num; num20++)
				{
					float num21 = 0f;
					float z6 = m_waves[curWave].Distance;
					if (num > 1)
					{
						float num22 = Mathf.Min(4f, num);
						num21 = (float)num20 / ((float)num - 1f) * num22 - num22 * 0.5f;
						z6 = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)num20 / ((float)num - 1f));
					}
					Vector3 vector6 = new Vector3(num21, num21 + 1f, z6);
					m_spawnPositions[num20] = vector6;
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetLayout.DiagonalRightUp:
			{
				for (int l = 0; l < num; l++)
				{
					float num4 = 0f;
					float z2 = m_waves[curWave].Distance;
					if (num > 1)
					{
						float num5 = Mathf.Min(4f, num);
						num4 = (float)l / ((float)num - 1f) * num5 - num5 * 0.5f;
						z2 = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)l / ((float)num - 1f));
					}
					Vector3 vector2 = new Vector3(0f - num4, num4 + 1f, z2);
					m_spawnPositions[l] = vector2;
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetLayout.DiagonalLeftDown:
			{
				for (int num41 = 0; num41 < num; num41++)
				{
					float num42 = 0f;
					float z11 = m_waves[curWave].Distance;
					if (num > 1)
					{
						float num43 = Mathf.Min(4f, num);
						num42 = (float)num41 / ((float)num - 1f) * num43 - num43 * 0.5f;
						z11 = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)num41 / ((float)num - 1f));
					}
					Vector3 vector11 = new Vector3(num42, 0f - num42 + 1f, z11);
					m_spawnPositions[num41] = vector11;
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetLayout.DiagonalRightDown:
			{
				for (int num35 = 0; num35 < num; num35++)
				{
					float num36 = 0f;
					float z9 = m_waves[curWave].Distance;
					if (num > 1)
					{
						float num37 = Mathf.Min(4f, num);
						num36 = (float)num35 / ((float)num - 1f) * num37 - num37 * 0.5f;
						z9 = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)num35 / ((float)num - 1f));
					}
					Vector3 vector9 = new Vector3(0f - num36, 0f - num36 + 1f, z9);
					m_spawnPositions[num35] = vector9;
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetLayout.SquareUp:
			{
				int num23 = Mathf.CeilToInt(Mathf.Sqrt(num));
				int num24 = 0;
				float num25 = 0f;
				float num26 = 0f;
				for (int num27 = 0; num27 < num23; num27++)
				{
					float num28 = Mathf.Min(4f, num23);
					num26 = (float)num27 / ((float)num23 - 1f) * num28 - num28 * 0.5f;
					int num29 = Mathf.Min(num - num24, num23);
					for (int num30 = 0; num30 < num29; num30++)
					{
						if (num24 < num)
						{
							int num31 = 0;
							float num32 = 0f;
							if (num29 > 1)
							{
								num31 = Mathf.Min(num29, num23);
								num32 = Mathf.Min(4f, num31);
								num25 = (float)num30 / ((float)num29 - 1f) * num32 - num32 * 0.5f;
							}
							else
							{
								num25 = 0f;
							}
							float z7 = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)num24 / ((float)num - 1f));
							Vector3 vector7 = new Vector3(num25, num26 + 1f, z7);
							m_spawnPositions[num24] = vector7;
						}
						num24++;
					}
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetLayout.SquareDown:
			{
				int num10 = Mathf.CeilToInt(Mathf.Sqrt(num));
				int num11 = 0;
				float num12 = 0f;
				float num13 = 0f;
				for (int num14 = 0; num14 < num10; num14++)
				{
					float num15 = Mathf.Min(4f, num10);
					num13 = (float)num14 / ((float)num10 - 1f) * num15 - num15 * 0.5f;
					int num16 = Mathf.Min(num - num11, num10);
					for (int num17 = 0; num17 < num16; num17++)
					{
						if (num11 < num)
						{
							int num18 = 0;
							float num19 = 0f;
							if (num16 > 1)
							{
								num18 = Mathf.Min(num16, num10);
								num19 = Mathf.Min(4f, num18);
								num12 = (float)num17 / ((float)num16 - 1f) * num19 - num19 * 0.5f;
							}
							else
							{
								num12 = 0f;
							}
							float z5 = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)num11 / ((float)num - 1f));
							Vector3 vector5 = new Vector3(num12, num13 + 1f, z5);
							m_spawnPositions[num11] = vector5;
						}
						num11++;
					}
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetLayout.CircleClockWise:
			{
				for (int m = 0; m < num; m++)
				{
					float x2 = 0f;
					float num6 = 0f;
					if (num > 1)
					{
						float num7 = (float)m / (float)num * 360f;
						x2 = Mathf.Sin((float)Math.PI / 180f * num7);
						num6 = Mathf.Cos((float)Math.PI / 180f * num7);
						x2 *= 2f;
						num6 *= 2f;
					}
					float z3 = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)m / ((float)num - 1f));
					Vector3 vector3 = new Vector3(x2, num6 + 1f, z3);
					m_spawnPositions[m] = vector3;
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetLayout.CircleCounterClockWise:
			{
				for (int k = 0; k < num; k++)
				{
					float x = 0f;
					float num2 = 0f;
					if (num > 1)
					{
						float num3 = (float)k / (float)num * 360f;
						x = 0f - Mathf.Sin((float)Math.PI / 180f * num3);
						num2 = Mathf.Cos((float)Math.PI / 180f * num3);
						x *= 2f;
						num2 *= 2f;
					}
					float z = Mathf.Lerp(m_waves[curWave].Distance, m_waves[curWave].EndDistance, (float)k / ((float)num - 1f));
					Vector3 vector = new Vector3(x, num2 + 1f, z);
					m_spawnPositions[k] = vector;
				}
				break;
			}
			}
		}

		public virtual void ClearSpawnedTargets()
		{
		}

		protected void ShuffleSpawnPoints()
		{
			for (int num = m_spawnPositions.Length - 1; num > 0; num--)
			{
				int num2 = UnityEngine.Random.Range(0, num);
				Vector3 vector = m_spawnPositions[num];
				ref Vector3 reference = ref m_spawnPositions[num];
				reference = m_spawnPositions[num2];
				m_spawnPositions[num2] = vector;
			}
		}

		protected void ShuffleNoShoot()
		{
			for (int num = m_isNoShoot.Length - 1; num > 0; num--)
			{
				int num2 = UnityEngine.Random.Range(0, num);
				bool flag = m_isNoShoot[num];
				m_isNoShoot[num] = m_isNoShoot[num2];
				m_isNoShoot[num2] = flag;
			}
		}

		public virtual void RegisterTargetHit()
		{
			m_curTarget++;
		}

		public void SetScore(int s)
		{
			m_score = s;
			m_master.ScoreDisplay.text = m_score + " pts";
		}

		public void AddScore(int s)
		{
			m_score += s;
			m_master.ScoreDisplay.text = m_score + " pts";
		}

		public int GetScore()
		{
			return m_score;
		}
	}
}
