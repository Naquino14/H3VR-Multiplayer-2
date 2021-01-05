// Decompiled with JetBrains decompiler
// Type: FistVR.MM_GronchShip
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MM_GronchShip : MonoBehaviour
  {
    public ConfigurableSoldierBotSpawner BotSpawner;
    public Rigidbody RB;
    public Transform Head;
    public Transform[] NavPoints;
    public Transform[] SpawnPoints;
    public GameObject SpawnEffectPrefab;
    public MM_GronchShip_Gun[] Guns;
    private MM_GronchShip_Gun m_curGun;
    public MM_GronchShip.GShipState State = MM_GronchShip.GShipState.MoveToPoint;
    private int m_currentNavPoint;
    private int m_nextNavPoint;
    private Vector3 m_currentDir;
    private Vector3 m_targetDir;
    private float m_hoverTickDown = 5f;
    private float m_moveToPointLerp;
    private float m_moveSpeed = 1f;
    private float m_spinDownTick = 5f;
    private float m_spawnSequenceTick = 1f;
    private int m_spawnAmountLeft = 5;
    private float m_timeSinceStart;
    public MM_GronchShip_DamagePiece[] DamagePieces;
    [Header("AudioStuff")]
    public AudioSource AUD;
    public AudioClip[] AudClip_Intro;
    public AudioClip[] AudClip_Firing;
    public AudioClip[] AudClip_Dodging;
    public AudioClip[] AudClip_Spawning;
    public AudioClip[] AudClip_MegaAttack;
    public AudioClip[] AudClip_Spinning;
    public AudioClip[] AudClip_Dying;
    private float m_speakingTick = 5f;
    [Header("DeathStuff")]
    public GameObject[] DeathVFXPrefabs;
    public Transform[] DeathVFXPoints;
    public GameObject DeathVFXFinal;
    private float splodeTick = 0.1f;
    private float m_dyingLerp;
    private Vector3 m_startPos;
    private Vector3 m_endPos;
    private bool m_isDeathSequenceFired;

    private void Start()
    {
      this.SetState(this.State);
      if ((double) this.m_speakingTick < 1.0 || this.AUD.isPlaying)
        return;
      this.AUD.clip = this.AudClip_Intro[Random.Range(0, this.AudClip_Intro.Length)];
      this.AUD.Play();
      this.m_speakingTick = 0.0f;
    }

    private void Update()
    {
      this.m_timeSinceStart += Time.deltaTime;
      this.DeathCheck();
      this.StateUpdate();
      if ((double) this.m_speakingTick <= 15.0)
        this.m_speakingTick += Time.deltaTime;
      if (!Input.GetKey(KeyCode.D))
        return;
      this.SetState(MM_GronchShip.GShipState.Dying);
    }

    private void RotateHeadTowardsPlayer()
    {
      Vector3 forward = GM.CurrentPlayerBody.Torso.position - this.Head.transform.position;
      forward.y = 0.0f;
      this.Head.rotation = Quaternion.RotateTowards(this.Head.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 5f);
      this.Head.localEulerAngles = new Vector3(0.0f, this.Head.localEulerAngles.y, 0.0f);
    }

    private void DeathCheck()
    {
      bool flag = false;
      int num1 = 0;
      for (int index = 0; index < this.Guns.Length; ++index)
      {
        if ((Object) this.Guns[index] != (Object) null && !this.Guns[index].IsDestroyed())
          ++num1;
      }
      int num2 = 0;
      for (int index = 0; index < this.DamagePieces.Length; ++index)
      {
        if ((Object) this.DamagePieces[index] != (Object) null && !this.DamagePieces[index].IsDestroyed())
          ++num2;
      }
      if (num2 == 0 || num1 == 0)
        flag = true;
      if (!flag || this.State == MM_GronchShip.GShipState.Dying)
        return;
      this.SetState(MM_GronchShip.GShipState.Dying);
    }

    private void StateUpdate()
    {
      switch (this.State)
      {
        case MM_GronchShip.GShipState.Hover:
          this.StateUpdate_Hover();
          break;
        case MM_GronchShip.GShipState.MoveToPoint:
          this.StateUpdate_MoveToPoint();
          break;
        case MM_GronchShip.GShipState.Spinning:
          this.StateUpdate_Spinning();
          break;
        case MM_GronchShip.GShipState.FiringSequence:
          this.StateUpdate_FiringSequence();
          this.RotateHeadTowardsPlayer();
          break;
        case MM_GronchShip.GShipState.SpawningSequence:
          this.StateUpdate_SpawningSequence();
          break;
        case MM_GronchShip.GShipState.FireEverything:
          this.StateUpdate_FireEverything();
          this.RotateHeadTowardsPlayer();
          break;
        case MM_GronchShip.GShipState.Dying:
          this.StateUpdate_Dying();
          break;
      }
    }

    private void StateUpdate_Dying()
    {
      if (!this.m_isDeathSequenceFired)
      {
        this.m_isDeathSequenceFired = true;
        this.BotSpawner.GronchIsDead();
        this.m_startPos = this.transform.position;
        this.m_endPos = new Vector3(this.m_startPos.x, 10f, this.m_startPos.z);
      }
      if ((double) this.splodeTick > 0.0)
      {
        this.splodeTick -= Time.deltaTime;
      }
      else
      {
        this.splodeTick = Random.Range(0.2f, 0.3f);
        Object.Instantiate<GameObject>(this.DeathVFXPrefabs[Random.Range(0, this.DeathVFXPrefabs.Length)], this.DeathVFXPoints[Random.Range(0, this.DeathVFXPoints.Length)].position, Random.rotation);
      }
      this.m_dyingLerp += Time.deltaTime * 0.2f;
      this.RB.MovePosition(Vector3.Lerp(this.RB.position, Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_dyingLerp * this.m_dyingLerp), Time.deltaTime * 6f));
      Vector3 forward = this.transform.forward;
      this.RB.MoveRotation(Quaternion.LookRotation(Quaternion.AngleAxis(1500f * Time.deltaTime * this.m_dyingLerp, Vector3.up) * forward, Vector3.up));
      if ((double) this.m_dyingLerp < 1.0)
        return;
      GameObject.Find("_AudioMusic").GetComponent<MM_MusicManager>().FadeOutMusic();
      Object.Instantiate<GameObject>(this.DeathVFXFinal, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    private void StateUpdate_Hover()
    {
      if ((double) this.m_hoverTickDown > 0.0)
      {
        this.m_hoverTickDown -= Time.deltaTime;
        this.RB.MovePosition(Vector3.Lerp(this.RB.position, this.NavPoints[this.m_currentNavPoint].position + Vector3.up * Mathf.Sin(Time.time * 2f) * 5f, Time.deltaTime * 3f));
      }
      else
        this.SetState(this.GetNextState(MM_GronchShip.GShipState.Hover));
    }

    private void StateUpdate_MoveToPoint()
    {
      if ((double) this.m_moveToPointLerp >= 1.0)
      {
        this.m_moveToPointLerp = 1f;
        this.RB.MovePosition(Vector3.Lerp(this.RB.position, Vector3.Lerp(this.NavPoints[this.m_currentNavPoint].position, this.NavPoints[this.m_nextNavPoint].position, this.m_moveToPointLerp) + Vector3.up * Mathf.Sin(Time.time * 2f) * 2f, Time.deltaTime * 3f));
        this.m_currentNavPoint = this.m_nextNavPoint;
        this.SetState(this.GetNextState(MM_GronchShip.GShipState.MoveToPoint));
      }
      else
      {
        this.m_moveToPointLerp += Time.deltaTime * this.m_moveSpeed;
        this.RB.MovePosition(Vector3.Lerp(this.RB.position, Vector3.Lerp(this.NavPoints[this.m_currentNavPoint].position, this.NavPoints[this.m_nextNavPoint].position, this.m_moveToPointLerp) + Vector3.up * Mathf.Sin(Time.time * 2f) * 2f, Time.deltaTime * 3f));
        this.RB.MoveRotation(Quaternion.LookRotation(Vector3.Lerp(this.m_currentDir, this.m_targetDir, this.m_moveToPointLerp), Vector3.up));
      }
    }

    private void StateUpdate_Spinning()
    {
      if ((double) this.m_spinDownTick > 0.0)
      {
        this.m_spinDownTick -= Time.deltaTime;
        Vector3 forward = this.transform.forward;
        this.RB.MoveRotation(Quaternion.LookRotation(Quaternion.AngleAxis(720f * Time.deltaTime, Vector3.up) * forward, Vector3.up));
        this.RB.MovePosition(Vector3.Lerp(this.RB.position, this.NavPoints[this.m_currentNavPoint].position + Vector3.up * Mathf.Sin(Time.time * 10f) * 5f, Time.deltaTime * 6f));
      }
      else
        this.SetState(this.GetNextState(MM_GronchShip.GShipState.Spinning));
    }

    private void StateUpdate_FiringSequence()
    {
      if (!((Object) this.m_curGun == (Object) null) && !this.m_curGun.IsDestroyed() && !this.m_curGun.IsFiringSequenceCompleted())
        return;
      this.SetState(this.GetNextState(MM_GronchShip.GShipState.FiringSequence));
    }

    private void StateUpdate_FireEverything()
    {
      bool flag = true;
      for (int index = 0; index < this.Guns.Length; ++index)
      {
        if ((Object) this.Guns[index] != (Object) null && !this.Guns[index].IsFiringSequenceCompleted())
          flag = false;
      }
      if (!flag)
        return;
      this.SetState(this.GetNextState(MM_GronchShip.GShipState.FireEverything));
    }

    private void StateUpdate_SpawningSequence()
    {
      this.m_spawnSequenceTick -= Time.deltaTime;
      if ((double) this.m_spawnSequenceTick <= 0.0)
      {
        this.SpawnBot();
        --this.m_spawnAmountLeft;
        this.m_spawnSequenceTick = Random.Range(0.5f, 1f);
      }
      this.RB.MovePosition(Vector3.Lerp(this.RB.position, this.NavPoints[this.m_currentNavPoint].position + Vector3.up * Mathf.Sin(Time.time * 2f) * 5f, Time.deltaTime * 3f));
      if (this.m_spawnAmountLeft > 0)
        return;
      this.SetState(this.GetNextState(MM_GronchShip.GShipState.SpawningSequence));
    }

    private void SpawnBot()
    {
      this.SetBotSettingsBasedOnDifficulty();
      this.BotSpawner.SpawnManualAtPoint(this.SpawnPoints[this.m_currentNavPoint]);
    }

    private void SetBotSettingsBasedOnDifficulty()
    {
      if ((double) this.m_timeSinceStart > 240.0)
      {
        this.BotSpawner.SetSetting_Gun(4);
        this.BotSpawner.SetSetting_Armor(3);
        this.BotSpawner.SetSetting_Health(1);
        this.BotSpawner.SetSetting_Movement(2);
      }
      else if ((double) this.m_timeSinceStart > 180.0)
      {
        this.BotSpawner.SetSetting_Gun(4);
        this.BotSpawner.SetSetting_Armor(3);
        this.BotSpawner.SetSetting_Health(1);
        this.BotSpawner.SetSetting_Movement(1);
      }
      else if ((double) this.m_timeSinceStart > 120.0)
      {
        this.BotSpawner.SetSetting_Gun(4);
        this.BotSpawner.SetSetting_Armor(3);
        this.BotSpawner.SetSetting_Health(0);
        this.BotSpawner.SetSetting_Movement(1);
      }
      else if ((double) this.m_timeSinceStart > 60.0)
      {
        this.BotSpawner.SetSetting_Gun(4);
        this.BotSpawner.SetSetting_Armor(0);
        this.BotSpawner.SetSetting_Health(0);
        this.BotSpawner.SetSetting_Movement(0);
      }
      else
      {
        if ((double) this.m_timeSinceStart <= 30.0)
          return;
        this.BotSpawner.SetSetting_Gun(0);
        this.BotSpawner.SetSetting_Armor(0);
        this.BotSpawner.SetSetting_Health(0);
        this.BotSpawner.SetSetting_Movement(0);
      }
    }

    private void SetState(MM_GronchShip.GShipState newState)
    {
      this.State = newState;
      this.AUD.volume = 0.35f;
      switch (newState)
      {
        case MM_GronchShip.GShipState.Hover:
          this.m_hoverTickDown = Random.Range(5f, 10f);
          break;
        case MM_GronchShip.GShipState.MoveToPoint:
          this.m_nextNavPoint = Random.Range(0, this.NavPoints.Length);
          this.m_moveToPointLerp = 0.0f;
          this.m_currentDir = this.transform.forward;
          this.m_targetDir = Random.onUnitSphere;
          this.m_targetDir.y = 0.0f;
          this.m_targetDir.Normalize();
          this.m_moveSpeed = Random.Range(0.2f, 0.65f);
          if ((double) this.m_speakingTick < 10.0 || this.AUD.isPlaying)
            break;
          this.AUD.clip = this.AudClip_Dodging[Random.Range(0, this.AudClip_Dodging.Length)];
          this.AUD.Play();
          this.m_speakingTick = 0.0f;
          break;
        case MM_GronchShip.GShipState.Spinning:
          this.m_spinDownTick = Random.Range(2.5f, 4f);
          if ((double) this.m_speakingTick < 3.0 || this.AUD.isPlaying)
            break;
          this.AUD.clip = this.AudClip_Spinning[Random.Range(0, this.AudClip_Spinning.Length)];
          this.AUD.Play();
          this.m_speakingTick = 0.0f;
          break;
        case MM_GronchShip.GShipState.FiringSequence:
          this.m_curGun = this.GetBestGunToFire();
          if ((Object) this.m_curGun == (Object) null)
            this.SetState(MM_GronchShip.GShipState.MoveToPoint);
          this.m_curGun.InitiateFiringSequence();
          if ((double) this.m_speakingTick < 6.0 || this.AUD.isPlaying)
            break;
          this.AUD.clip = this.AudClip_Firing[Random.Range(0, this.AudClip_Firing.Length)];
          this.AUD.Play();
          this.m_speakingTick = 0.0f;
          break;
        case MM_GronchShip.GShipState.SpawningSequence:
          this.m_spawnAmountLeft = Random.Range(Mathf.RoundToInt(Mathf.Lerp(1f, 3f, this.m_timeSinceStart * (1f / 500f))), Mathf.RoundToInt(Mathf.Lerp(3f, 6f, this.m_timeSinceStart * (1f / 500f))));
          this.m_spawnSequenceTick = Random.Range(0.5f, 1f);
          if ((double) this.m_speakingTick < 6.0 || this.AUD.isPlaying)
            break;
          this.AUD.clip = this.AudClip_Spawning[Random.Range(0, this.AudClip_Spawning.Length)];
          this.AUD.Play();
          this.m_speakingTick = 0.0f;
          break;
        case MM_GronchShip.GShipState.FireEverything:
          this.FireGoodGuns();
          if ((double) this.m_speakingTick < 2.0 || this.AUD.isPlaying)
            break;
          this.AUD.clip = this.AudClip_MegaAttack[Random.Range(0, this.AudClip_MegaAttack.Length)];
          this.AUD.Play();
          this.m_speakingTick = 0.0f;
          break;
        case MM_GronchShip.GShipState.Dying:
          this.AUD.Stop();
          this.AUD.clip = this.AudClip_Dying[Random.Range(0, this.AudClip_Dying.Length)];
          this.AUD.Play();
          break;
      }
    }

    private MM_GronchShip.GShipState GetNextState(MM_GronchShip.GShipState curState)
    {
      float num = Random.Range(0.0f, 1f);
      switch (curState)
      {
        case MM_GronchShip.GShipState.Hover:
          return (double) num > 0.600000023841858 ? MM_GronchShip.GShipState.SpawningSequence : MM_GronchShip.GShipState.MoveToPoint;
        case MM_GronchShip.GShipState.MoveToPoint:
          if ((double) num > 0.899999976158142)
            return MM_GronchShip.GShipState.MoveToPoint;
          if ((double) num > 0.400000005960464)
            return MM_GronchShip.GShipState.FiringSequence;
          return (double) num > 0.100000001490116 ? MM_GronchShip.GShipState.SpawningSequence : MM_GronchShip.GShipState.Spinning;
        case MM_GronchShip.GShipState.Spinning:
          return (double) num > 0.200000002980232 ? MM_GronchShip.GShipState.FireEverything : MM_GronchShip.GShipState.MoveToPoint;
        case MM_GronchShip.GShipState.FiringSequence:
          return MM_GronchShip.GShipState.Hover;
        case MM_GronchShip.GShipState.SpawningSequence:
          return MM_GronchShip.GShipState.MoveToPoint;
        case MM_GronchShip.GShipState.FireEverything:
          return MM_GronchShip.GShipState.Hover;
        default:
          return curState;
      }
    }

    private void FireGoodGuns()
    {
      for (int index = 0; index < this.Guns.Length; ++index)
      {
        if (!((Object) this.Guns[index] == (Object) null) && !this.Guns[index].IsDestroyed())
        {
          Vector3 to = GM.CurrentPlayerBody.transform.position - this.transform.root.position;
          to.y = 0.0f;
          if ((double) Vector3.Angle(this.Guns[index].transform.forward, to) <= 120.0)
            this.Guns[index].InitiateFiringSequence();
        }
      }
    }

    private MM_GronchShip_Gun GetBestGunToFire()
    {
      MM_GronchShip_Gun mmGronchShipGun1 = (MM_GronchShip_Gun) null;
      MM_GronchShip_Gun mmGronchShipGun2 = (MM_GronchShip_Gun) null;
      for (int index = 0; index < this.Guns.Length; ++index)
      {
        if (!((Object) this.Guns[index] == (Object) null))
        {
          if (!this.Guns[index].IsDestroyed() && (Object) mmGronchShipGun1 == (Object) null)
            mmGronchShipGun1 = this.Guns[index];
          Vector3 to = GM.CurrentPlayerBody.transform.position - this.transform.root.position;
          to.y = 0.0f;
          if ((double) Vector3.Angle(this.Guns[index].transform.forward, to) <= 45.0)
            mmGronchShipGun2 = this.Guns[index];
        }
      }
      return (Object) mmGronchShipGun2 != (Object) null ? mmGronchShipGun2 : mmGronchShipGun1;
    }

    public void GunDestroyed()
    {
    }

    public enum GShipState
    {
      Hover,
      MoveToPoint,
      Spinning,
      FiringSequence,
      SpawningSequence,
      FireEverything,
      Dying,
    }
  }
}
