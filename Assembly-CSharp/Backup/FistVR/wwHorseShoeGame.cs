// Decompiled with JetBrains decompiler
// Type: FistVR.wwHorseShoeGame
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwHorseShoeGame : MonoBehaviour
  {
    public wwParkManager ParkManager;
    public wwHorseShoePlinth[] Plinths;
    public GameObject[] CompletionCheckMarks;
    private float CheckComplettionTick = 1f;

    public void RegisterSuccess(int i)
    {
      this.ParkManager.RegisterHorseshoeCompletion(i);
      Debug.Log((object) "Registering Success");
      this.CompletionCheckMarks[i].SetActive(true);
    }

    public void UpdateCheckMarks(int[] states)
    {
      for (int index = 0; index < states.Length; ++index)
      {
        if (states[index] == 0)
          this.CompletionCheckMarks[index].SetActive(false);
        else
          this.CompletionCheckMarks[index].SetActive(true);
      }
    }

    public void SetPlinthStates(int[] states)
    {
      for (int index = 0; index < states.Length; ++index)
      {
        if (states[index] > 0)
          this.Plinths[index].SetCompleted();
      }
      this.UpdateCheckMarks(states);
    }

    public void Update()
    {
      if (this.ParkManager.RewardChests[2].GetState() >= 1)
        return;
      if ((double) this.CheckComplettionTick > 0.0)
      {
        this.CheckComplettionTick -= Time.deltaTime;
      }
      else
      {
        this.CheckComplettionTick = 1f;
        bool flag = true;
        for (int index = 0; index < this.Plinths.Length; ++index)
        {
          if (!this.Plinths[index].IsCompleted())
            flag = false;
        }
        if (!flag)
          return;
        this.ParkManager.UnlockChest(2);
      }
    }
  }
}
