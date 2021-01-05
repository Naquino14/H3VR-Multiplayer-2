using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class OmniSpawner_Discs : OmniSpawner
	{
		private OmniSpawnDef_Discs m_def;

		public GameObject[] DiscPrefabs;

		private bool m_canSpawn;

		private OmniSpawnDef_Discs.DiscSpawnStyle m_spawnStyle;

		private int m_discIndex;

		private int m_totalDiscCount;

		private float m_timeTilNextDisc;

		private bool m_shouldSpawnNextOnHitDisc = true;

		private OmniSpawnDef_Discs.DiscSpawnPattern m_spawnPattern;

		private OmniSpawnDef_Discs.DiscSpawnOrdering m_spawnOrdering;

		private OmniSpawnDef_Discs.DiscZConfig m_zConfig;

		private int[] m_randIndexes;

		private int[] m_randIndexes2;

		private float m_discMoveSpeed = 1f;

		private List<OmniDisc> m_activeDiscs = new List<OmniDisc>();

		private int m_numShootableDiscs;

		public override void Configure(OmniSpawnDef def, OmniWaveEngagementRange range)
		{
			base.Configure(def, range);
			m_def = def as OmniSpawnDef_Discs;
			m_spawnStyle = m_def.SpawnStyle;
			m_spawnPattern = m_def.SpawnPattern;
			m_spawnOrdering = m_def.SpawnOrdering;
			m_zConfig = m_def.ZConfig;
			m_totalDiscCount = m_def.Discs.Count;
			m_discMoveSpeed = m_def.DiscMovementSpeed;
			if (m_spawnOrdering == OmniSpawnDef_Discs.DiscSpawnOrdering.Random)
			{
				m_randIndexes = new int[m_totalDiscCount];
				m_randIndexes2 = new int[m_totalDiscCount];
				for (int i = 0; i < m_randIndexes.Length; i++)
				{
					m_randIndexes[i] = i;
					m_randIndexes2[i] = i;
				}
				GenerateRandomIndices(m_randIndexes);
				GenerateRandomIndices(m_randIndexes2);
			}
		}

		public override void BeginSpawning()
		{
			base.BeginSpawning();
			m_canSpawn = true;
		}

		public override void EndSpawning()
		{
			base.EndSpawning();
			m_canSpawn = false;
		}

		public override void Activate()
		{
			base.Activate();
		}

		public override int Deactivate()
		{
			if (m_activeDiscs.Count > 0)
			{
				for (int num = m_activeDiscs.Count - 1; num >= 0; num--)
				{
					if (m_activeDiscs[num] != null)
					{
						UnityEngine.Object.Destroy(m_activeDiscs[num].gameObject);
					}
				}
				m_activeDiscs.Clear();
			}
			return base.Deactivate();
		}

		private void Update()
		{
			UpdateMe();
		}

		private void UpdateMe()
		{
			if (m_isConfigured)
			{
				switch (m_state)
				{
				case SpawnerState.Activated:
					SpawningLoop();
					break;
				case SpawnerState.Activating:
					Activating();
					break;
				case SpawnerState.Deactivating:
					Deactivating();
					break;
				}
			}
		}

		private void SpawningLoop()
		{
			if (m_canSpawn)
			{
				OmniSpawnDef_Discs.DiscSpawnStyle spawnStyle = m_spawnStyle;
				if (spawnStyle != OmniSpawnDef_Discs.DiscSpawnStyle.AllAtOnce)
				{
					switch (spawnStyle)
					{
					case OmniSpawnDef_Discs.DiscSpawnStyle.OnHit:
						if (m_discIndex < m_def.Discs.Count && m_shouldSpawnNextOnHitDisc)
						{
							m_shouldSpawnNextOnHitDisc = false;
							SpawnDisc(m_discIndex);
						}
						break;
					case OmniSpawnDef_Discs.DiscSpawnStyle.Sequential:
						if (m_discIndex < m_def.Discs.Count)
						{
							if (m_timeTilNextDisc <= 0f)
							{
								SpawnDisc(m_discIndex);
								m_timeTilNextDisc = m_def.TimeBetweenDiscSpawns;
							}
							else
							{
								m_timeTilNextDisc -= Time.deltaTime;
							}
						}
						break;
					}
				}
				else
				{
					while (m_discIndex < m_def.Discs.Count)
					{
						SpawnDisc(m_discIndex);
					}
				}
			}
			if (m_discIndex >= m_def.Discs.Count)
			{
				m_isDoneSpawning = true;
				if (m_numShootableDiscs <= 0)
				{
					m_isReadyForWaveEnd = true;
				}
			}
		}

		private void SpawnDisc(int index)
		{
			int index2 = index;
			if (m_spawnOrdering == OmniSpawnDef_Discs.DiscSpawnOrdering.Random)
			{
				index = m_randIndexes[index];
				index2 = m_randIndexes2[index];
			}
			Vector3 position = base.transform.position;
			Vector2 b = Vector2.zero;
			Vector3 targetEndingPos = GetTargetEndingPos(index2, out b);
			Vector3 vector = new Vector3(0f, 1.25f, 0f);
			Quaternion startRot = Quaternion.LookRotation(Vector3.forward);
			Quaternion endRot = Quaternion.LookRotation(-Vector3.forward);
			GameObject gameObject = UnityEngine.Object.Instantiate(DiscPrefabs[(int)m_def.Discs[index]], position, Quaternion.identity);
			OmniDisc component = gameObject.GetComponent<OmniDisc>();
			m_activeDiscs.Add(component);
			component.Init(this, position, targetEndingPos, startRot, endRot, m_def.MovementPattern, m_def.MovementStyle, b, m_discMoveSpeed);
			Invoke("PlaySpawnSound", 0.15f);
			if (component.Type != OmniSpawnDef_Discs.DiscType.NoShoot)
			{
				m_numShootableDiscs++;
			}
			if (m_spawnStyle == OmniSpawnDef_Discs.DiscSpawnStyle.OnHit && m_def.Discs[index] == OmniSpawnDef_Discs.DiscType.NoShoot)
			{
				m_discIndex++;
				if (m_discIndex < m_def.Discs.Count)
				{
					SpawnDisc(m_discIndex);
				}
			}
			else
			{
				m_discIndex++;
			}
		}

		public void ClearDisc(OmniDisc disc)
		{
			if (disc.Type != OmniSpawnDef_Discs.DiscType.NoShoot)
			{
				m_numShootableDiscs--;
			}
			m_activeDiscs.Remove(disc);
			m_shouldSpawnNextOnHitDisc = true;
		}

		private Vector3 GetTargetEndingPos(int index, out Vector2 b)
		{
			Vector3 zero = Vector3.zero;
			switch (m_zConfig)
			{
			case OmniSpawnDef_Discs.DiscZConfig.Homogenous:
				zero.z = GetRange();
				break;
			case OmniSpawnDef_Discs.DiscZConfig.Incremented:
				zero.z = GetRange() + (float)index * 0.1f;
				break;
			}
			float num = 0f;
			if (m_totalDiscCount > 1)
			{
				num = (float)index / (float)(m_totalDiscCount - 1);
			}
			float y = (float)(m_totalDiscCount - 1) * 2f;
			float num2 = (float)(m_totalDiscCount - 1) * 2f;
			b = new Vector2(num2, y);
			float num3 = 0f;
			float num4 = 0f;
			switch (m_spawnPattern)
			{
			case OmniSpawnDef_Discs.DiscSpawnPattern.Circle:
				num3 = 0f;
				num4 = 0f;
				if (m_totalDiscCount > 1)
				{
					float num9 = (float)index / (float)m_totalDiscCount * 360f;
					num3 = Mathf.Sin((float)Math.PI / 180f * num9);
					num4 = Mathf.Cos((float)Math.PI / 180f * num9);
					float num10 = 1f + (float)m_totalDiscCount * 0.2f;
					num3 *= num10;
					num4 *= num10;
					b = new Vector2(num10, num10);
				}
				num4 += 1.25f;
				zero.x = num3;
				zero.y = num4;
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.Square:
			{
				int num11 = Mathf.CeilToInt(Mathf.Sqrt(m_totalDiscCount));
				int num12 = index / num11;
				int num13 = index % num11;
				num2 = (float)num11 * 2f;
				zero.x = (float)num13 * 2f - num2 * 0.5f + 1f;
				zero.y = (float)num12 * 2f - num2 * 0.5f + 1.25f + 1f;
				b = new Vector2(num2, num2);
				break;
			}
			case OmniSpawnDef_Discs.DiscSpawnPattern.LineXCentered:
				zero.y = 1.25f;
				zero.x = (float)index * 2f - num2 * 0.5f;
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.LineYCentered:
				zero.x = 0f;
				zero.y = (float)index * 2f - num2 * 0.5f + 1.25f;
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.LineXDown:
				zero.y = 0f - num2 * 0.5f + 1.25f;
				zero.x = (float)index * 2f - num2 * 0.5f;
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.LineXUp:
				zero.y = num2 * 0.5f + 1.25f;
				zero.x = (float)index * 2f - num2 * 0.5f;
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.LineYLeft:
				zero.x = 0f - num2 * 0.5f;
				zero.y = (float)index * 2f - num2 * 0.5f + 1.25f;
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.LineYRight:
				zero.x = num2 * 0.5f;
				zero.y = (float)index * 2f - num2 * 0.5f + 1.25f;
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.DiagonalLeftUp:
				num2 = (float)m_totalDiscCount * 1.4f;
				zero.x = (float)index * 1.4f - num2 * 0.5f;
				zero.y = (float)index * 1.4f - num2 * 0.5f + 1.25f;
				b = new Vector2(num2, num2);
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.DiagonalRightUp:
				num2 = (float)m_totalDiscCount * 1.4f;
				zero.x = 0f - ((float)index * 1.4f - num2 * 0.5f);
				zero.y = (float)index * 1.4f - num2 * 0.5f + 1.25f;
				b = new Vector2(num2, num2);
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.DiagonalLeftDown:
				num2 = (float)m_totalDiscCount * 1.4f;
				zero.x = (float)index * 1.4f - num2 * 0.5f;
				zero.y = 0f - ((float)index * 1.4f - num2 * 0.5f) + 1.25f;
				b = new Vector2(num2, num2);
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.DiagonalRightDown:
				num2 = (float)m_totalDiscCount * 1.4f;
				zero.x = 0f - ((float)index * 1.4f - num2 * 0.5f);
				zero.y = 0f - ((float)index * 1.4f - num2 * 0.5f) + 1.25f;
				b = new Vector2(num2, num2);
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.SpiralCounterclockwise:
				num3 = 0f;
				num4 = 0f;
				if (m_totalDiscCount > 1)
				{
					float num7 = (float)index / (float)m_totalDiscCount * 360f;
					num3 = Mathf.Sin((float)Math.PI / 180f * num7);
					num4 = Mathf.Cos((float)Math.PI / 180f * num7);
					float num8 = 1f + (float)m_totalDiscCount * 0.2f;
					float a2 = num8 * 0.5f;
					float b3 = num8 * 2f;
					num8 = Mathf.Lerp(a2, b3, (float)index / (float)m_totalDiscCount);
					num3 *= num8;
					num4 *= num8;
					b = new Vector2(num8, num8);
				}
				num4 += 1.25f;
				zero.x = num3;
				zero.y = num4;
				break;
			case OmniSpawnDef_Discs.DiscSpawnPattern.SpiralClockwise:
				num3 = 0f;
				num4 = 0f;
				if (m_totalDiscCount > 1)
				{
					float num5 = (float)index / (float)m_totalDiscCount * 360f;
					num3 = 0f - Mathf.Sin((float)Math.PI / 180f * num5);
					num4 = Mathf.Cos((float)Math.PI / 180f * num5);
					float num6 = 1f + (float)m_totalDiscCount * 0.2f;
					float a = num6 * 0.5f;
					float b2 = num6 * 2f;
					num6 = Mathf.Lerp(a, b2, (float)index / (float)m_totalDiscCount);
					num3 *= num6;
					num4 *= num6;
					b = new Vector2(num6, num6);
				}
				num4 += 1.25f;
				zero.x = num3;
				zero.y = num4;
				break;
			}
			return zero;
		}

		private void GenerateRandomIndices(int[] indicies)
		{
			for (int num = indicies.Length - 1; num > 0; num--)
			{
				int num2 = UnityEngine.Random.Range(0, num);
				int num3 = indicies[num];
				indicies[num] = indicies[num2];
				indicies[num2] = num3;
			}
		}
	}
}
