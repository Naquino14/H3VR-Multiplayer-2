// Decompiled with JetBrains decompiler
// Type: FistVR.wwEventPuzzle_SilverBullet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwEventPuzzle_SilverBullet : wwEventPuzzle
  {
    public GameObject[] StaticBullet;
    public GameObject[] TurnOnBullet;
    public GameObject[] ExplosionPrefabs;

    public void Explode()
    {
      if (this.PuzzleState != 0)
        return;
      foreach (GameObject gameObject in this.StaticBullet)
        gameObject.SetActive(false);
      foreach (GameObject gameObject in this.TurnOnBullet)
        gameObject.SetActive(true);
      foreach (GameObject explosionPrefab in this.ExplosionPrefabs)
        Object.Instantiate<GameObject>(explosionPrefab, this.transform.position, this.transform.rotation);
      this.PuzzleState = 1;
      this.ParkManager.RegisterEventPuzzleChange(this.PuzzleIndex, 1);
      if (this.ParkManager.RewardChests[this.ChestIndex].GetState() >= 1)
        return;
      this.ParkManager.UnlockChest(this.ChestIndex);
    }

    public override void SetState(int stateIndex)
    {
      base.SetState(stateIndex);
      if (this.PuzzleState == 0)
      {
        foreach (GameObject gameObject in this.StaticBullet)
          gameObject.SetActive(true);
        foreach (GameObject gameObject in this.TurnOnBullet)
          gameObject.SetActive(false);
      }
      else
      {
        foreach (GameObject gameObject in this.StaticBullet)
          gameObject.SetActive(false);
        foreach (GameObject gameObject in this.TurnOnBullet)
          gameObject.SetActive(true);
      }
    }
  }
}
