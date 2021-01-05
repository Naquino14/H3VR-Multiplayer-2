// Decompiled with JetBrains decompiler
// Type: FistVR.ThingThrower
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ThingThrower : MonoBehaviour
  {
    public ThingThrowerScreen TTScreen;
    public FVRObject[] Things;
    private int m_currentThingToThrow;
    private bool m_isThrowing;
    private int m_throwSequencesLeft = 1;
    private int m_numThrowsPerSequence = 3;
    private int m_numThrowsLeft = 3;
    private float m_sequenceBreather = 10f;
    private float m_throwTime = 5f;
    private float m_tickDownToThrow = 15f;
    private int m_numObjectsPerThrow = 1;
    public Transform LaunchPos1;
    public Transform LaunchPos2;
    private float m_AngularRangeX;
    private float m_AngularRangeY = 5f;
    private float m_VelocityBase = 18f;
    private float m_VelocityRange = 1f;
    private float m_timeBetweenObjects = 0.5f;
    public AudioSource LaunchSoundSource;
    public AudioClip LaunchSound;

    private void Awake()
    {
      this.TTScreen.OBS_TargetToThrow.SetSelectedButton(0);
      this.TTScreen.OBS_NumberOfThrows.SetSelectedButton(2);
      this.TTScreen.OBS_NumberOfTargetsPerThrow.SetSelectedButton(0);
      this.TTScreen.OBS_TimeBetweenThrows.SetSelectedButton(2);
    }

    private void Start()
    {
      for (int index = 0; index < this.Things.Length; ++index)
        this.Things[index].GetGameObject();
    }

    public void BeginSequences()
    {
      this.m_tickDownToThrow = 5f;
      this.m_throwSequencesLeft = 1;
      this.m_numThrowsLeft = this.m_numThrowsPerSequence;
      this.m_isThrowing = true;
      this.TTScreen.OBS_TargetToThrow.gameObject.SetActive(false);
      this.TTScreen.OBS_NumberOfThrows.gameObject.SetActive(false);
      this.TTScreen.OBS_NumberOfTargetsPerThrow.gameObject.SetActive(false);
      this.TTScreen.OBS_TimeBetweenThrows.gameObject.SetActive(false);
      this.TTScreen.StartButton.SetActive(false);
      this.TTScreen.StopButton.SetActive(true);
    }

    public void StopSequences()
    {
      this.m_isThrowing = false;
      this.TTScreen.OBS_TargetToThrow.gameObject.SetActive(true);
      this.TTScreen.OBS_NumberOfThrows.gameObject.SetActive(true);
      this.TTScreen.OBS_NumberOfTargetsPerThrow.gameObject.SetActive(true);
      this.TTScreen.OBS_TimeBetweenThrows.gameObject.SetActive(true);
      this.TTScreen.StartButton.SetActive(true);
      this.TTScreen.StopButton.SetActive(false);
    }

    public void SetNumThrows(int i)
    {
      this.m_numThrowsPerSequence = i + 1;
      this.m_numThrowsLeft = i;
    }

    public void SetTargetToThrow(int i) => this.m_currentThingToThrow = i;

    public void SetNumTargetsPerThrow(int i) => this.m_numObjectsPerThrow = i + 1;

    public void SetTimeBetweenThrows(int i)
    {
      switch (i)
      {
        case 0:
          this.m_throwTime = 2f;
          break;
        case 1:
          this.m_throwTime = 3f;
          break;
        case 2:
          this.m_throwTime = 5f;
          break;
        case 3:
          this.m_throwTime = 8f;
          break;
        case 4:
          this.m_throwTime = 10f;
          break;
      }
    }

    private void Update() => this.UpdateThrower();

    private void UpdateThrower()
    {
      if (!this.m_isThrowing)
        return;
      if (this.m_throwSequencesLeft <= 0)
      {
        this.m_isThrowing = false;
        this.StopSequences();
      }
      else if (this.m_numThrowsLeft <= 0)
      {
        --this.m_throwSequencesLeft;
        this.m_numThrowsLeft = this.m_numThrowsPerSequence;
        this.m_tickDownToThrow = this.m_sequenceBreather;
      }
      else if ((double) this.m_tickDownToThrow <= 0.0)
      {
        this.m_tickDownToThrow = this.m_throwTime;
        --this.m_numThrowsLeft;
        this.Throw();
      }
      else
        this.m_tickDownToThrow -= Time.deltaTime;
    }

    private void Throw()
    {
      for (int index = 0; index < this.m_numObjectsPerThrow; ++index)
        this.Invoke("ThrowThing", (float) index * this.m_timeBetweenObjects);
    }

    private void ThrowThing()
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.Things[this.m_currentThingToThrow].GetGameObject(), Vector3.Lerp(this.LaunchPos1.position, this.LaunchPos2.position, Random.Range(0.0f, 1f)), this.LaunchPos1.rotation);
      gameObject.transform.Rotate(new Vector3(Random.Range(-this.m_AngularRangeX, this.m_AngularRangeX), Random.Range(-this.m_AngularRangeY, this.m_AngularRangeY), 0.0f));
      gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * (this.m_VelocityBase + Random.Range(-this.m_VelocityRange, this.m_VelocityRange));
      gameObject.GetComponent<Rigidbody>().angularVelocity = gameObject.transform.up * 10f;
      this.LaunchSoundSource.pitch = Random.Range(0.97f, 1.03f);
      this.LaunchSoundSource.PlayOneShot(this.LaunchSound, 0.4f);
    }
  }
}
