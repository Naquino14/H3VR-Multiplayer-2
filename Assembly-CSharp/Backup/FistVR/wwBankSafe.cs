// Decompiled with JetBrains decompiler
// Type: FistVR.wwBankSafe
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwBankSafe : MonoBehaviour
  {
    public wwTargetPuzzle Puzzle;
    public GameObject[] ContentsObjects;
    public Transform[] ContentsPoses;
    public int SafeState;
    public float[] DoorStateRotations;
    public Transform Door;
    public GameObject SafeOpenTrigger;
    public GameObject SafeParticles;
    public ParticleSystem PSystem;
    public AudioEvent PayoutSoundEvent;
    public AudioEvent UnlockEvent;

    public void SetState(int stateIndex, bool playSound, bool stateEvent)
    {
      this.SafeState = stateIndex;
      if (playSound)
        SM.PlayGenericSound(this.UnlockEvent, this.transform.position);
      this.Door.localEulerAngles = new Vector3(0.0f, this.DoorStateRotations[this.SafeState], 0.0f);
      if (this.SafeState == 1)
      {
        this.SafeOpenTrigger.SetActive(true);
        this.SafeParticles.SetActive(true);
        if (stateEvent)
        {
          SM.PlayGenericSound(this.UnlockEvent, this.transform.position);
          this.Puzzle.Manager.RegisterPuzzleSafeStateChange(this.Puzzle.PuzzleIndex, 1);
        }
      }
      else
      {
        this.SafeOpenTrigger.SetActive(false);
        this.SafeParticles.SetActive(false);
      }
      if (this.SafeState != 2 || !stateEvent)
        return;
      this.Invoke("PayoutBurst", 0.2f);
      this.Invoke("PayoutBurst", 1.2f);
      this.Invoke("PayoutBurst", 2.2f);
      for (int index = 0; index < this.ContentsObjects.Length; ++index)
        Object.Instantiate<GameObject>(this.ContentsObjects[index], this.ContentsPoses[index].position, this.ContentsPoses[index].rotation);
      this.Puzzle.Manager.RegisterPuzzleSafeStateChange(this.Puzzle.PuzzleIndex, 2);
    }

    public void PayoutBurst()
    {
      SM.PlayGenericSound(this.PayoutSoundEvent, this.transform.position);
      this.PSystem.Emit(100);
    }
  }
}
