// Decompiled with JetBrains decompiler
// Type: FistVR.Cubegame
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FistVR
{
  public class Cubegame : MonoBehaviour
  {
    private Cubegame.CubegameState State;
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
      get => this.m_score;
      set => this.m_score = value;
    }

    public void ScorePoints(float p)
    {
      if (this.State != Cubegame.CubegameState.PlayingWave)
        return;
      this.Score += p;
    }

    public void MusicOn(bool b) => this.m_isMusicEnabled = b;

    public void DamagePlayer()
    {
      if (this.State == Cubegame.CubegameState.NotPlaying)
        return;
      this.ScorePoints(-5000f);
      --this.Health;
      if ((double) this.Health > 0.0)
        return;
      this.Health = 0.1f;
      this.State = Cubegame.CubegameState.NotPlaying;
      this.StopAllCoroutines();
      for (int index = 0; index < this.Cubes.Count; ++index)
      {
        if ((Object) this.Cubes[index] != (Object) null)
          this.Cubes[index].GetComponent<BevelCubeTarget>().Boom(false);
      }
      this.Cubes.Clear();
    }

    public void TargetDown() => --this.TargetsInWave;

    private void Start()
    {
    }

    private void BeginGame(int a)
    {
      if (this.State != Cubegame.CubegameState.NotPlaying)
        return;
      this.Sequence = this.Sequences[this.SequenceSelector.curSelectedSequence];
      this.SequenceSelector.gameObject.SetActive(false);
      if ((Object) this.ItemSpawner != (Object) null)
        this.ItemSpawner.SetActive(false);
      this.Score = 0.0f;
      this.Music.volume = 0.2f;
      this.Music.Stop();
      if (this.m_isMusicEnabled)
        this.Music.Play();
      this.Health = 100f;
      this.m_curWave = -1;
      this.m_reloadTime = 5f;
      this.State = Cubegame.CubegameState.ReloadCountdown;
      if (this.m_curWave + 1 >= this.Sequence.Waves.Length)
        return;
      for (int index = 0; index < this.Sequence.Waves[this.m_curWave + 1].Elements.Length; ++index)
        this.Walls[this.Sequence.Waves[this.m_curWave + 1].Elements[index].WallIndex].TargetWarning.SetActive(true);
    }

    private void SpawnEnemy(GameObject enemy, Vector3 pos, Quaternion rot) => this.Cubes.Add(Object.Instantiate<GameObject>(enemy, pos, rot));

    [DebuggerHidden]
    private IEnumerator SpawnEnemyWithDelay(
      GameObject enemy,
      Vector3 pos,
      Quaternion rot,
      float delay)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Cubegame.\u003CSpawnEnemyWithDelay\u003Ec__Iterator0()
      {
        delay = delay,
        enemy = enemy,
        pos = pos,
        rot = rot,
        \u0024this = this
      };
    }

    private void StartNextWave()
    {
      this.State = Cubegame.CubegameState.PlayingWave;
      ++this.m_curWave;
      this.SpawnSound.Play();
      for (int index = 0; index < this.Walls.Length; ++index)
      {
        this.Walls[index].TargetWarning.SetActive(false);
        this.Walls[index].TimeText.color = Color.white;
      }
      this.TargetsInWave = 0;
      if (this.m_curWave < this.Sequence.Waves.Length)
      {
        CubeGameWaveType wave = this.Sequence.Waves[this.m_curWave];
        this.m_playTime = wave.TimeForWave;
        for (int index1 = 0; index1 < wave.Elements.Length; ++index1)
        {
          CubeGameWaveElement element = wave.Elements[index1];
          List<Transform> spawns = this.Walls[element.WallIndex].GetSpawns(Random.Range(element.MinEnemies, element.MaxEnemies), element.WallType);
          this.TargetsInWave += spawns.Count;
          for (int index2 = 0; index2 < spawns.Count; ++index2)
          {
            if (element.TrickleSpawn)
              this.StartCoroutine(this.SpawnEnemyWithDelay(element.Enemy.Prefab, spawns[index2].position, Quaternion.identity, (float) ((double) index2 / (double) spawns.Count * (double) wave.TimeForWave * 0.75)));
            else
              this.SpawnEnemy(element.Enemy.Prefab, spawns[index2].position, Quaternion.identity);
          }
        }
      }
      else
        this.EndGame();
    }

    private void EndWave()
    {
      for (int index = 0; index < this.Cubes.Count; ++index)
      {
        if ((Object) this.Cubes[index] != (Object) null)
          this.Cubes[index].SendMessage("Boom", (object) false);
      }
      this.Cubes.Clear();
      for (int index = 0; index < this.Walls.Length; ++index)
      {
        this.Walls[index].TargetWarning.SetActive(false);
        this.Walls[index].TimeText.color = Color.white;
      }
      if (this.m_curWave + 1 < this.Sequence.Waves.Length)
      {
        this.State = Cubegame.CubegameState.ReloadCountdown;
        this.m_reloadTime = this.Sequence.Waves[this.m_curWave].ReloadTimeAfter;
        for (int index = 0; index < this.Sequence.Waves[this.m_curWave + 1].Elements.Length; ++index)
          this.Walls[this.Sequence.Waves[this.m_curWave + 1].Elements[index].WallIndex].TargetWarning.SetActive(true);
      }
      else
        this.EndGame();
    }

    private void EndGame()
    {
      this.State = Cubegame.CubegameState.NotPlaying;
      for (int index = 0; index < this.Walls.Length; ++index)
      {
        this.Walls[index].WaveText.text = "Final Score";
        this.Walls[index].TimeText.text = 0.0.ToString("F2");
      }
      this.SequenceSelector.gameObject.SetActive(true);
      if (!((Object) this.ItemSpawner != (Object) null))
        return;
      this.ItemSpawner.SetActive(true);
    }

    private void Update()
    {
      for (int index = 0; index < this.Walls.Length; ++index)
        this.Walls[index].ScoreText.text = Mathf.RoundToInt(this.Score).ToString() + " Pts";
      switch (this.State)
      {
        case Cubegame.CubegameState.NotPlaying:
          if ((double) this.Music.volume <= 0.0)
            break;
          this.Music.volume = Mathf.Lerp(this.Music.volume, 0.0f, Time.deltaTime * 2f);
          break;
        case Cubegame.CubegameState.ReloadCountdown:
          for (int index = 0; index < this.Walls.Length; ++index)
          {
            this.Walls[index].WaveText.text = "Wave " + (this.m_curWave + 2).ToString() + " Incoming";
            this.Walls[index].TimeText.text = this.m_reloadTime.ToString("F2");
          }
          if ((double) this.m_reloadTime > 0.0)
          {
            this.m_reloadTime -= Time.deltaTime;
            break;
          }
          this.m_reloadTime = 0.0f;
          this.StartNextWave();
          break;
        case Cubegame.CubegameState.PlayingWave:
          for (int index = 0; index < this.Walls.Length; ++index)
          {
            this.Walls[index].WaveText.text = "Wave " + (this.m_curWave + 1).ToString();
            this.Walls[index].TimeText.text = this.m_playTime.ToString("F2");
          }
          if ((double) this.m_playTime > 0.0)
          {
            if (this.TargetsInWave <= 0)
            {
              this.m_playTime -= Time.deltaTime * 5f;
              this.ScorePoints(Time.deltaTime * 5000f);
              for (int index = 0; index < this.Walls.Length; ++index)
                this.Walls[index].TimeText.color = Color.green;
              break;
            }
            this.m_playTime -= Time.deltaTime;
            break;
          }
          this.m_playTime = 0.0f;
          this.EndWave();
          break;
      }
    }

    public enum CubegameState
    {
      NotPlaying,
      ReloadCountdown,
      PlayingWave,
      Ended,
    }
  }
}
