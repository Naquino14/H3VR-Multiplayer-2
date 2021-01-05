using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MRS_Speed : ModularRangeSequencer
	{
		private List<MRT_SimpleBull> Targets = new List<MRT_SimpleBull>();

		public AudioClip AC_SequenceBegins;

		public AudioClip AC_WaveBegins;

		public AudioClip AC_ReloadTimeBegins;

		public AudioClip AC_SequenceOver;

		public Color Color_WarmUp;

		public Color Color_InWave;

		public Color Color_Reload;

		public Color Color_SequenceOver;

		public override void BeginSequence(ModularRangeMaster master, ModularRangeSequenceDefinition sequenceDef)
		{
			base.BeginSequence(master, sequenceDef);
			curWave = 0;
			m_waveTickDown = 0f;
			m_reloadTickDown = 0f;
			m_wavelength = 0f;
			m_warmupTickDown = 5f;
			SetScore(0);
			m_master.PlayAudioEvent(AC_SequenceBegins, 0.4f);
			BeginWarmup();
		}

		public override void AbortSequence()
		{
			base.AbortSequence();
		}

		public override void Update()
		{
			base.Update();
			switch (m_state)
			{
			case RangeSequencerState.NotRunning:
				break;
			case RangeSequencerState.Warmup:
				if (m_warmupTickDown > 0f)
				{
					m_warmupTickDown -= Time.deltaTime;
					m_master.InGameDisplay.text = "Warmup\n";
					m_master.InGameDisplay.text += m_warmupTickDown.ToString("00.00");
					m_master.InGameDisplay.color = Color_WarmUp;
				}
				else
				{
					BeginWave();
				}
				break;
			case RangeSequencerState.InWave:
				if (m_waveTickDown < m_wavelength)
				{
					m_waveTickDown += Time.deltaTime;
					m_master.InGameDisplay.text = "Wave " + (curWave + 1) + " of " + m_sequenceDefinition.Waves.Length + "\n";
					m_master.InGameDisplay.text += (m_wavelength - m_waveTickDown).ToString("00.00");
					m_master.InGameDisplay.color = Color_InWave;
				}
				else
				{
					CancelInvoke();
					if (curWave < m_sequenceDefinition.Waves.Length - 1)
					{
						BeginReloadTime();
						break;
					}
					EndSequence();
					m_state = RangeSequencerState.NotRunning;
				}
				break;
			case RangeSequencerState.ReloadTime:
				if (m_reloadTickDown < m_reloadLength)
				{
					m_reloadTickDown += Time.deltaTime;
					m_master.InGameDisplay.text = "Reload!\n";
					m_master.InGameDisplay.text += (m_reloadLength - m_reloadTickDown).ToString("00.00");
					m_master.InGameDisplay.color = Color_Reload;
				}
				else
				{
					curWave++;
					ClearSpawnedTargets();
					BeginWave();
				}
				break;
			case RangeSequencerState.SequenceOver:
				m_master.InGameDisplay.text = "Sequence Over";
				m_master.InGameDisplay.color = Color_SequenceOver;
				SequenceOver();
				break;
			}
		}

		private void BeginWarmup()
		{
			m_state = RangeSequencerState.Warmup;
		}

		private void BeginWave()
		{
			StopAllCoroutines();
			m_master.PlayAudioEvent(AC_WaveBegins, 0.4f);
			m_state = RangeSequencerState.InWave;
			m_curTarget = 0;
			GenerateSpawnPositions();
			m_wavelength = m_waves[curWave].TimeForWave;
			m_reloadLength = m_waves[curWave].TimeForReload;
			m_waveTickDown = 0f;
			m_reloadTickDown = 0f;
			switch (m_waves[curWave].Timing)
			{
			case ModularRangeSequenceDefinition.TargetTiming.SequentialTimed:
			{
				for (int j = 0; j < m_isNoShoot.Length; j++)
				{
					float delay = m_waves[curWave].DelayPerTarget * (float)j;
					if (!m_isNoShoot[j])
					{
						StartCoroutine(SpawnSimpleBull(delay, j, autoActivate: true, isNoShoot: false));
					}
					else
					{
						StartCoroutine(SpawnSimpleBull(delay, j, autoActivate: true, isNoShoot: true));
					}
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetTiming.RandomTimed:
			{
				ShuffleSpawnPoints();
				for (int l = 0; l < m_isNoShoot.Length; l++)
				{
					float delay2 = m_waves[curWave].DelayPerTarget * (float)l;
					if (!m_isNoShoot[l])
					{
						StartCoroutine(SpawnSimpleBull(delay2, l, autoActivate: true, isNoShoot: false));
					}
					else
					{
						StartCoroutine(SpawnSimpleBull(delay2, l, autoActivate: true, isNoShoot: true));
					}
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetTiming.SequentialOnHit:
			{
				float delayPerTarget2 = m_waves[curWave].DelayPerTarget;
				bool flag2 = false;
				for (int m = 0; m < m_isNoShoot.Length; m++)
				{
					if (!m_isNoShoot[m])
					{
						flag2 = true;
						StartCoroutine(SpawnSimpleBull(delayPerTarget2, 0, autoActivate: true, isNoShoot: false));
					}
					else
					{
						StartCoroutine(SpawnSimpleBull(delayPerTarget2, 0, autoActivate: true, isNoShoot: true));
						m_curTarget++;
					}
					if (flag2)
					{
						break;
					}
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetTiming.RandomOnHit:
			{
				ShuffleSpawnPoints();
				float delayPerTarget = m_waves[curWave].DelayPerTarget;
				bool flag = false;
				for (int k = 0; k < m_isNoShoot.Length; k++)
				{
					if (!m_isNoShoot[k])
					{
						flag = true;
						StartCoroutine(SpawnSimpleBull(delayPerTarget, 0, autoActivate: true, isNoShoot: false));
					}
					else
					{
						StartCoroutine(SpawnSimpleBull(delayPerTarget, 0, autoActivate: true, isNoShoot: true));
						m_curTarget++;
					}
					if (flag)
					{
						break;
					}
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetTiming.Flood:
			{
				for (int i = 0; i < m_isNoShoot.Length; i++)
				{
					float num = m_waves[curWave].DelayPerTarget * (float)i;
					if (!m_isNoShoot[i])
					{
						StartCoroutine(SpawnSimpleBull(0f, i, autoActivate: true, isNoShoot: false));
					}
					else
					{
						StartCoroutine(SpawnSimpleBull(0f, i, autoActivate: true, isNoShoot: true));
					}
				}
				break;
			}
			}
		}

		public override void RegisterTargetHit()
		{
			base.RegisterTargetHit();
			if (m_state != RangeSequencerState.InWave)
			{
				return;
			}
			switch (m_waves[curWave].Timing)
			{
			case ModularRangeSequenceDefinition.TargetTiming.SequentialOnHit:
			{
				bool flag2 = false;
				for (int j = m_curTarget; j < m_isNoShoot.Length; j++)
				{
					float delayPerTarget2 = m_waves[curWave].DelayPerTarget;
					if (!m_isNoShoot[j])
					{
						flag2 = true;
						StartCoroutine(SpawnSimpleBull(delayPerTarget2, m_curTarget, autoActivate: true, isNoShoot: false));
					}
					else
					{
						StartCoroutine(SpawnSimpleBull(delayPerTarget2, m_curTarget, autoActivate: true, isNoShoot: true));
						m_curTarget++;
					}
					if (flag2)
					{
						break;
					}
				}
				break;
			}
			case ModularRangeSequenceDefinition.TargetTiming.RandomOnHit:
			{
				bool flag = false;
				for (int i = m_curTarget; i < m_isNoShoot.Length; i++)
				{
					float delayPerTarget = m_waves[curWave].DelayPerTarget;
					if (!m_isNoShoot[i])
					{
						flag = true;
						StartCoroutine(SpawnSimpleBull(delayPerTarget, m_curTarget, autoActivate: true, isNoShoot: false));
					}
					else
					{
						StartCoroutine(SpawnSimpleBull(delayPerTarget, m_curTarget, autoActivate: true, isNoShoot: true));
						m_curTarget++;
					}
					if (flag)
					{
						break;
					}
				}
				break;
			}
			}
		}

		private void BeginReloadTime()
		{
			m_master.PlayAudioEvent(AC_ReloadTimeBegins, 0.4f);
			m_state = RangeSequencerState.ReloadTime;
			ClearSpawnedTargets();
		}

		private void EndSequence()
		{
			m_master.PlayAudioEvent(AC_SequenceOver, 0.4f);
			ClearSpawnedTargets();
			SequenceOver();
		}

		private IEnumerator SpawnSimpleBull(float delay, int spawnIndex, bool autoActivate, bool isNoShoot)
		{
			yield return new WaitForSeconds(delay);
			GameObject tempTarg2 = null;
			tempTarg2 = (isNoShoot ? Object.Instantiate(m_waves[curWave].NoShootTargetPrefabs[Random.Range(0, m_waves[curWave].NoShootTargetPrefabs.Length)], base.transform.position, base.transform.rotation) : Object.Instantiate(m_waves[curWave].TargetPrefabs[Random.Range(0, m_waves[curWave].TargetPrefabs.Length)], base.transform.position, base.transform.rotation));
			MRT_SimpleBull tempBull = tempTarg2.GetComponent<MRT_SimpleBull>();
			Targets.Add(tempBull);
			Vector3 endPos = m_spawnPositions[spawnIndex];
			Vector3 startPos = new Vector3(m_spawnPositions[spawnIndex].x, m_spawnPositions[spawnIndex].y, 150f);
			float timeInTransit = (1f / tempBull.MoveSpeed + 1f / tempBull.RotSpeed) * 2f;
			float timeLeft = m_waves[curWave].TimePerTarget - timeInTransit;
			tempBull.Init(this, startPos, endPos, m_waves[curWave].TimePerTarget, Vector3.forward, Vector3.back, m_waves[curWave], spawnIndex);
			if (autoActivate)
			{
				tempBull.Activate();
			}
		}

		public override void ClearSpawnedTargets()
		{
			base.ClearSpawnedTargets();
			if (Targets.Count <= 0)
			{
				return;
			}
			for (int num = Targets.Count - 1; num >= 0; num--)
			{
				if (Targets[num] != null)
				{
					Object.Destroy(Targets[num].gameObject);
				}
			}
			Targets.Clear();
		}
	}
}
