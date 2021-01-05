// Decompiled with JetBrains decompiler
// Type: FistVR.wwTargetManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class wwTargetManager : MonoBehaviour
  {
    public wwParkManager Manager;
    private List<wwTargetManager.TargetToRespawn> TargetsToRespawn = new List<wwTargetManager.TargetToRespawn>();
    private List<string> LastThreeTargetsStruck = new List<string>();
    private List<Sprite> LastThreeSprites = new List<Sprite>();
    public bool UsesWWExtendedSystems = true;
    public eSlab ESlab;
    public wwTargetPuzzle[] Puzzles;
    public GameObject[] CompletionCheckMarks;
    public AudioEvent TargSequenceCompletedEvent;
    private float CheckPuzzleComplettionTick = 0.1f;

    private void Awake()
    {
      foreach (wwTarget wwTarget in UnityEngine.Object.FindObjectsOfType<wwTarget>())
        wwTarget.SetManager(this);
      this.LastThreeTargetsStruck.Add(string.Empty);
      this.LastThreeTargetsStruck.Add(string.Empty);
      this.LastThreeTargetsStruck.Add(string.Empty);
      this.LastThreeTargetsStruck.Add(string.Empty);
      this.LastThreeSprites.Add((Sprite) null);
      this.LastThreeSprites.Add((Sprite) null);
      this.LastThreeSprites.Add((Sprite) null);
      this.LastThreeSprites.Add((Sprite) null);
    }

    public void ConfigurePuzzleStates(int[] PuzzleStates, int[] SafeStates)
    {
      for (int index = 0; index < this.Puzzles.Length; ++index)
        this.Puzzles[index].SetState(PuzzleStates[index], SafeStates[index]);
      this.UpdateCheckMarks(PuzzleStates);
    }

    public void UpdateCheckMarks(int[] states)
    {
      for (int index = 0; index < states.Length; ++index)
      {
        if (states[index] == 0 || states[index] == 1)
          this.CompletionCheckMarks[index].SetActive(false);
        else
          this.CompletionCheckMarks[index].SetActive(true);
      }
    }

    private void Update()
    {
      if (this.TargetsToRespawn.Count > 0)
      {
        for (int index = this.TargetsToRespawn.Count - 1; index >= 0; --index)
        {
          if ((double) this.TargetsToRespawn[index].TimeToRespawn > 0.0)
          {
            this.TargetsToRespawn[index].TimeToRespawn -= Time.deltaTime;
          }
          else
          {
            this.RespawnTarget(this.TargetsToRespawn[index]);
            this.TargetsToRespawn.RemoveAt(index);
          }
        }
      }
      if (!this.UsesWWExtendedSystems || this.Manager.RewardChests[0].GetState() >= 1)
        return;
      if ((double) this.CheckPuzzleComplettionTick > 0.0)
      {
        this.CheckPuzzleComplettionTick -= Time.deltaTime;
      }
      else
      {
        this.CheckPuzzleComplettionTick = 1f;
        bool flag = true;
        for (int index = 0; index < this.Puzzles.Length; ++index)
        {
          if (this.Puzzles[index].PuzzleState != 2)
            flag = false;
        }
        if (!flag)
          return;
        this.Manager.UnlockChest(0);
      }
    }

    public void PrimeForRespawn(
      wwTarget t,
      Vector3 pos,
      Quaternion rot,
      float Scale,
      bool reScale)
    {
      if (!(t is wwTargetShatterable))
        return;
      this.TargetsToRespawn.Add(new wwTargetManager.TargetToRespawn()
      {
        ObjectWrapper = (t as wwTargetShatterable).ObjectWrapper,
        Pos = pos,
        Rot = rot,
        Scale = Scale,
        DoesReScale = reScale,
        TimeToRespawn = t.RespawnTime,
        Sprite = t.TargetSprite
      });
    }

    public void StruckEvent(wwTarget t)
    {
      this.LastThreeTargetsStruck.Add(t.Ident);
      this.LastThreeSprites.Add(t.TargetSprite);
      this.LastThreeTargetsStruck.RemoveAt(0);
      this.LastThreeSprites.RemoveAt(0);
      if (!this.UsesWWExtendedSystems)
        return;
      this.ESlab.UpdateSprites(this.LastThreeSprites[0], this.LastThreeSprites[1], this.LastThreeSprites[2], this.LastThreeSprites[3]);
      this.TestPuzzles();
    }

    private void TestPuzzles()
    {
      string empty = string.Empty;
      for (int index = 0; index < this.LastThreeTargetsStruck.Count; ++index)
        empty += this.LastThreeTargetsStruck[index];
      for (int index = 0; index < this.Puzzles.Length; ++index)
      {
        if (!((UnityEngine.Object) this.Puzzles[index] == (UnityEngine.Object) null))
          this.Puzzles[index].TestSequence(empty);
      }
    }

    private void RespawnTarget(wwTargetManager.TargetToRespawn t)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(t.ObjectWrapper.GetGameObject(), t.Pos, t.Rot);
      gameObject.transform.position = t.Pos;
      gameObject.transform.rotation = t.Rot;
      if (t.DoesReScale)
        gameObject.transform.localScale = new Vector3(t.Scale, t.Scale, t.Scale);
      gameObject.GetComponent<wwTarget>().SetupAfterSpawn(this, t.Pos, t.Rot, t.Scale, t.DoesReScale);
    }

    public void RegisterPuzzleStateChange(int puzzle, int newState)
    {
      if (newState == 2)
      {
        this.CompletionCheckMarks[puzzle].SetActive(true);
        SM.PlayGenericSound(this.TargSequenceCompletedEvent, GM.CurrentPlayerBody.transform.position);
      }
      this.Manager.RegisterTargetPuzzleStateChange(puzzle, newState);
    }

    public void RegisterPuzzleSafeStateChange(int puzzle, int newState) => this.Manager.RegisterTargetPuzzleSafeStateChange(puzzle, newState);

    [Serializable]
    public class TargetToRespawn
    {
      public FVRObject ObjectWrapper;
      public float TimeToRespawn;
      public Vector3 Pos;
      public Quaternion Rot;
      public float Scale;
      public bool DoesReScale;
      public Sprite Sprite;
    }
  }
}
