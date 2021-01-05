// Decompiled with JetBrains decompiler
// Type: FistVR.wwRewardChest
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwRewardChest : MonoBehaviour
  {
    public wwParkManager Manager;
    private int m_state;
    public GameObject ReadyToOpenParticles;
    public GameObject ReadyToOpenTrigger;
    public GameObject KeyPrefab;
    public Transform KeyPosition;
    public Transform Cap;
    public GameObject BlackOut;
    public GameObject Lock;
    private float XRot_Closed;
    private float XRot_Open = -70f;
    public int Index;
    public AudioEvent ChestUnlockedEvent;
    public AudioEvent ChestOpenEvent;
    public ItemSpawnerID[] RewardUnlocks;
    public GameObject[] RewardPrefabs;
    public Transform[] RewardPoints;

    public void Awake()
    {
    }

    public int GetState() => this.m_state;

    public void UnlockChest()
    {
      SM.PlayGenericSound(this.ChestUnlockedEvent, this.transform.position);
      this.SetState(1, true);
      this.Manager.RegisterRewardChestStateChange(this.Index, 1);
    }

    public void OpenChest()
    {
      this.SetState(2, true);
      this.Manager.RegisterRewardChestStateChange(this.Index, 2);
      wwKey component = Object.Instantiate<GameObject>(this.KeyPrefab, this.KeyPosition.position, this.KeyPosition.rotation).GetComponent<wwKey>();
      component.KeyIndex = this.Index;
      component.State = 1;
      component.Manager = this.Manager;
      SM.PlayGenericSound(this.ChestOpenEvent, this.transform.position);
      for (int index = 0; index < this.RewardPrefabs.Length; ++index)
        Object.Instantiate<GameObject>(this.RewardPrefabs[index], this.RewardPoints[index].position, this.RewardPoints[index].rotation);
      if (this.RewardUnlocks.Length > 0)
      {
        for (int index = 0; index < this.RewardUnlocks.Length; ++index)
          GM.Rewards.RewardUnlocks.UnlockReward(this.RewardUnlocks[index]);
        GM.Rewards.SaveToFile();
      }
      this.Manager.RegisterKeyStateChange(this.Index, 1);
    }

    public void SetState(int newState, bool stateEvent)
    {
      this.m_state = newState;
      switch (newState)
      {
        case 0:
          this.ReadyToOpenParticles.SetActive(false);
          this.ReadyToOpenTrigger.SetActive(false);
          this.BlackOut.SetActive(true);
          this.Lock.SetActive(true);
          this.Cap.localEulerAngles = new Vector3(this.XRot_Closed, 0.0f, 0.0f);
          break;
        case 1:
          this.ReadyToOpenParticles.SetActive(true);
          this.ReadyToOpenTrigger.SetActive(true);
          this.BlackOut.SetActive(true);
          this.Lock.SetActive(false);
          this.Cap.localEulerAngles = new Vector3(this.XRot_Closed, 0.0f, 0.0f);
          break;
        case 2:
          this.ReadyToOpenParticles.SetActive(false);
          this.ReadyToOpenTrigger.SetActive(false);
          this.BlackOut.SetActive(false);
          this.Lock.SetActive(false);
          this.Cap.localEulerAngles = new Vector3(this.XRot_Open, 0.0f, 0.0f);
          break;
      }
    }
  }
}
