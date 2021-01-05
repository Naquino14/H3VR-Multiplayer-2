using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Cubegame : MonoBehaviour
	{
		public enum CubegameState
		{
			NotPlaying,
			ReloadCountdown,
			PlayingWave,
			Ended
		}

		private CubegameState State;

		public CubeGameWaveSequence[] Sequences;

		public CubeGameWaveSequence Sequence;

		public CubeGameSequenceSelectorv1 SequenceSelector;

		private int m_curWave = 25;

		private float m_reloadTime = 5f;

		private float m_playTime = 5f;

		public CubeSpawnWall[] Walls;

		private List<GameObject> Cubes = new List<GameObject>();

		public float Health = 100f;

		private int TargetsInWave;

		private float m_score;

		public AudioSource Music;

		public AudioSource SpawnSound;

		public GameObject ItemSpawner;

		private bool m_isMusicEnabled = true;

		public float Score
		{
			get
			{
				return m_score;
			}
			set
			{
				m_score = value;
			}
		}

		public void ScorePoints(float p)
		{
			if (State == CubegameState.PlayingWave)
			{
				Score += p;
			}
		}

		public void MusicOn(bool b)
		{
			m_isMusicEnabled = b;
		}

		public void DamagePlayer()
		{
			if (State == CubegameState.NotPlaying)
			{
				return;
			}
			ScorePoints(-5000f);
			Health -= 1f;
			if (!(Health <= 0f))
			{
				return;
			}
			Health = 0.1f;
			State = CubegameState.NotPlaying;
			StopAllCoroutines();
			for (int i = 0; i < Cubes.Count; i++)
			{
				if (Cubes[i] != null)
				{
					Cubes[i].GetComponent<BevelCubeTarget>().Boom(getPoints: false);
				}
			}
			Cubes.Clear();
		}

		public void TargetDown()
		{
			TargetsInWave--;
		}

		private void Start()
		{
		}

		private void BeginGame(int a)
		{
			if (State != 0)
			{
				return;
			}
			Sequence = Sequences[SequenceSelector.curSelectedSequence];
			SequenceSelector.gameObject.SetActive(value: false);
			if (ItemSpawner != null)
			{
				ItemSpawner.SetActive(value: false);
			}
			Score = 0f;
			Music.volume = 0.2f;
			Music.Stop();
			if (m_isMusicEnabled)
			{
				Music.Play();
			}
			Health = 100f;
			m_curWave = -1;
			m_reloadTime = 5f;
			State = CubegameState.ReloadCountdown;
			if (m_curWave + 1 < Sequence.Waves.Length)
			{
				for (int i = 0; i < Sequence.Waves[m_curWave + 1].Elements.Length; i++)
				{
					Walls[Sequence.Waves[m_curWave + 1].Elements[i].WallIndex].TargetWarning.SetActive(value: true);
				}
			}
		}

		private void SpawnEnemy(GameObject enemy, Vector3 pos, Quaternion rot)
		{
			GameObject item = Object.Instantiate(enemy, pos, rot);
			Cubes.Add(item);
		}

		private IEnumerator SpawnEnemyWithDelay(GameObject enemy, Vector3 pos, Quaternion rot, float delay)
		{
			yield return new WaitForSeconds(delay);
			GameObject tempGo = Object.Instantiate(enemy, pos, rot);
			Cubes.Add(tempGo);
		}

		private void StartNextWave()
		{
			State = CubegameState.PlayingWave;
			m_curWave++;
			SpawnSound.Play();
			for (int i = 0; i < Walls.Length; i++)
			{
				Walls[i].TargetWarning.SetActive(value: false);
				Walls[i].TimeText.color = Color.white;
			}
			TargetsInWave = 0;
			if (m_curWave < Sequence.Waves.Length)
			{
				CubeGameWaveType cubeGameWaveType = Sequence.Waves[m_curWave];
				m_playTime = cubeGameWaveType.TimeForWave;
				for (int j = 0; j < cubeGameWaveType.Elements.Length; j++)
				{
					CubeGameWaveElement cubeGameWaveElement = cubeGameWaveType.Elements[j];
					List<Transform> spawns = Walls[cubeGameWaveElement.WallIndex].GetSpawns(Random.Range(cubeGameWaveElement.MinEnemies, cubeGameWaveElement.MaxEnemies), cubeGameWaveElement.WallType);
					TargetsInWave += spawns.Count;
					for (int k = 0; k < spawns.Count; k++)
					{
						if (cubeGameWaveElement.TrickleSpawn)
						{
							StartCoroutine(SpawnEnemyWithDelay(cubeGameWaveElement.Enemy.Prefab, spawns[k].position, Quaternion.identity, (float)k / (float)spawns.Count * cubeGameWaveType.TimeForWave * 0.75f));
						}
						else
						{
							SpawnEnemy(cubeGameWaveElement.Enemy.Prefab, spawns[k].position, Quaternion.identity);
						}
					}
				}
			}
			else
			{
				EndGame();
			}
		}

		private void EndWave()
		{
			for (int i = 0; i < Cubes.Count; i++)
			{
				if (Cubes[i] != null)
				{
					Cubes[i].SendMessage("Boom", false);
				}
			}
			Cubes.Clear();
			for (int j = 0; j < Walls.Length; j++)
			{
				Walls[j].TargetWarning.SetActive(value: false);
				Walls[j].TimeText.color = Color.white;
			}
			if (m_curWave + 1 < Sequence.Waves.Length)
			{
				State = CubegameState.ReloadCountdown;
				m_reloadTime = Sequence.Waves[m_curWave].ReloadTimeAfter;
				for (int k = 0; k < Sequence.Waves[m_curWave + 1].Elements.Length; k++)
				{
					Walls[Sequence.Waves[m_curWave + 1].Elements[k].WallIndex].TargetWarning.SetActive(value: true);
				}
			}
			else
			{
				EndGame();
			}
		}

		private void EndGame()
		{
			State = CubegameState.NotPlaying;
			for (int i = 0; i < Walls.Length; i++)
			{
				Walls[i].WaveText.text = "Final Score";
				Walls[i].TimeText.text = 0.0.ToString("F2");
			}
			SequenceSelector.gameObject.SetActive(value: true);
			if (ItemSpawner != null)
			{
				ItemSpawner.SetActive(value: true);
			}
		}

		private void Update()
		{
			for (int i = 0; i < Walls.Length; i++)
			{
				Walls[i].ScoreText.text = Mathf.RoundToInt(Score) + " Pts";
			}
			switch (State)
			{
			case CubegameState.ReloadCountdown:
			{
				for (int j = 0; j < Walls.Length; j++)
				{
					Walls[j].WaveText.text = "Wave " + (m_curWave + 2) + " Incoming";
					Walls[j].TimeText.text = m_reloadTime.ToString("F2");
				}
				if (m_reloadTime > 0f)
				{
					m_reloadTime -= Time.deltaTime;
					break;
				}
				m_reloadTime = 0f;
				StartNextWave();
				break;
			}
			case CubegameState.PlayingWave:
			{
				for (int k = 0; k < Walls.Length; k++)
				{
					Walls[k].WaveText.text = "Wave " + (m_curWave + 1);
					Walls[k].TimeText.text = m_playTime.ToString("F2");
				}
				if (m_playTime > 0f)
				{
					if (TargetsInWave <= 0)
					{
						m_playTime -= Time.deltaTime * 5f;
						ScorePoints(Time.deltaTime * 5000f);
						for (int l = 0; l < Walls.Length; l++)
						{
							Walls[l].TimeText.color = Color.green;
						}
					}
					else
					{
						m_playTime -= Time.deltaTime;
					}
				}
				else
				{
					m_playTime = 0f;
					EndWave();
				}
				break;
			}
			case CubegameState.NotPlaying:
				if (Music.volume > 0f)
				{
					Music.volume = Mathf.Lerp(Music.volume, 0f, Time.deltaTime * 2f);
				}
				break;
			}
		}
	}
}
