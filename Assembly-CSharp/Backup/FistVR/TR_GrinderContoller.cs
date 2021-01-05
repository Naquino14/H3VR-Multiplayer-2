// Decompiled with JetBrains decompiler
// Type: FistVR.TR_GrinderContoller
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TR_GrinderContoller : MonoBehaviour, IRoomTriggerable
  {
    public GameObject GrinderBladesPrefab;
    public AudioSource GrinderSound;
    private TR_Grinder[] Grinders;
    private float m_startYPos = 10f;
    private float m_endYPos = 0.6f;
    private float m_curYPos = 10f;
    private float m_YSpeed = 1f;
    private float m_startZPos;
    private float m_endZPos;
    private float m_curZPos;
    private float m_timeLeft = 50f;
    private float m_maxTime = 50f;
    private float m_grinderVolume;
    private float m_grinderPitch = 0.4f;
    private bool m_isGrinding;
    private bool m_isLowering;
    private bool m_isSliding;
    private bool m_isRaising;
    private int m_life = 10;
    private int m_Maxlife = 10;
    private RedRoom m_room;
    public GameObject ShatterableMeatPrefab_Metal;
    public GameObject ShatterableMeatPrefab_Meat;
    public Transform MeatSpawnPoint;
    private bool m_isSpawningMeat;
    private Vector3 meatSpawnMin = Vector3.zero;
    private Vector3 meatSpawnMax = Vector3.zero;
    private int m_meatLeftToSpawn = 10;
    private float m_meatSpawnTick = 0.25f;

    public void SetRoom(RedRoom room) => this.m_room = room;

    public void Init(int roomTileSize, RedRoom room)
    {
      GM.MGMaster.Narrator.PlayTrapRoomInit();
      this.GetComponent<AudioSource>().Play();
      this.m_room = room;
      switch (roomTileSize)
      {
        case 2:
          this.Grinders = new TR_Grinder[2];
          this.m_startZPos = (float) -((double) roomTileSize - 0.5);
          this.m_endZPos = (float) roomTileSize - 0.5f;
          this.m_curZPos = this.m_startZPos;
          for (int index = 0; index < 2; ++index)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(this.GrinderBladesPrefab, new Vector3(0.0f, 10f, 0.0f), Quaternion.identity);
            gameObject.transform.SetParent(this.transform);
            gameObject.transform.localPosition = new Vector3((float) ((double) index * 2.0 - 1.0), this.m_startYPos, this.m_startZPos);
            gameObject.transform.localEulerAngles = Vector3.zero;
            this.Grinders[index] = gameObject.GetComponent<TR_Grinder>();
            this.Grinders[index].StartSpinning();
            this.Grinders[index].SetGController(this);
            this.GrinderSound.transform.localPosition = new Vector3(0.0f, this.m_startYPos, this.m_startZPos);
            this.meatSpawnMin = new Vector3(-1.5f, 8f, 0.0f);
            this.meatSpawnMax = new Vector3(1.5f, 8f, this.m_endZPos);
            this.m_meatLeftToSpawn = 7;
            this.m_life = 6;
            this.m_Maxlife = 6;
            this.m_isSpawningMeat = true;
          }
          break;
        case 3:
          this.Grinders = new TR_Grinder[3];
          this.m_startZPos = (float) -((double) roomTileSize - 0.5);
          this.m_endZPos = (float) roomTileSize - 0.5f;
          this.m_curZPos = this.m_startZPos;
          for (int index = 0; index < 3; ++index)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(this.GrinderBladesPrefab, new Vector3(0.0f, 10f, 0.0f), Quaternion.identity);
            gameObject.transform.SetParent(this.transform);
            gameObject.transform.localPosition = new Vector3((float) ((double) index * 2.0 - 2.0), this.m_startYPos, this.m_startZPos);
            gameObject.transform.localEulerAngles = Vector3.zero;
            this.Grinders[index] = gameObject.GetComponent<TR_Grinder>();
            this.Grinders[index].StartSpinning();
            this.Grinders[index].SetGController(this);
            this.GrinderSound.transform.localPosition = new Vector3(0.0f, this.m_startYPos, this.m_startZPos);
            this.meatSpawnMin = new Vector3(-2.5f, 8f, 1f);
            this.meatSpawnMax = new Vector3(2.5f, 8f, this.m_endZPos);
            this.m_meatLeftToSpawn = 9;
            this.m_life = 8;
            this.m_Maxlife = 8;
            this.m_isSpawningMeat = true;
          }
          break;
        case 4:
          this.Grinders = new TR_Grinder[4];
          this.m_startZPos = (float) -((double) roomTileSize - 0.5);
          this.m_endZPos = (float) roomTileSize - 0.5f;
          this.m_curZPos = this.m_startZPos;
          for (int index = 0; index < 4; ++index)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(this.GrinderBladesPrefab, new Vector3(0.0f, 10f, 0.0f), Quaternion.identity);
            gameObject.transform.SetParent(this.transform);
            gameObject.transform.localPosition = new Vector3((float) ((double) index * 2.0 - 3.0), this.m_startYPos, this.m_startZPos);
            gameObject.transform.localEulerAngles = Vector3.zero;
            this.Grinders[index] = gameObject.GetComponent<TR_Grinder>();
            this.Grinders[index].StartSpinning();
            this.Grinders[index].SetGController(this);
            this.GrinderSound.transform.localPosition = new Vector3(0.0f, this.m_startYPos, this.m_startZPos);
            this.meatSpawnMin = new Vector3(-3.5f, 8f, 2f);
            this.meatSpawnMax = new Vector3(3.5f, 8f, this.m_endZPos);
            this.m_meatLeftToSpawn = 12;
            this.m_life = 10;
            this.m_Maxlife = 10;
            this.m_isSpawningMeat = true;
          }
          break;
      }
      this.BeginLowering();
    }

    private void BeginLowering()
    {
      this.m_isLowering = true;
      this.GrinderSound.Play();
      this.m_isGrinding = true;
    }

    public void DamageGrinder()
    {
      if (!this.m_isGrinding)
        return;
      --this.m_life;
      if (this.m_life <= 0)
      {
        this.m_isGrinding = false;
        for (int index = 0; index < this.Grinders.Length; ++index)
        {
          this.SpinDown();
          this.Grinders[index].StopSpinning();
          this.Invoke("Raise", 3f);
        }
      }
      else if ((double) this.m_life / (double) this.m_Maxlife < 0.699999988079071)
      {
        for (int index = 0; index < this.Grinders.Length; ++index)
        {
          this.Grinders[index].FirePEffects[0].SetActive(true);
          this.Grinders[index].FirePEffects[1].SetActive(true);
        }
      }
      else
      {
        if ((double) this.m_life / (double) this.m_Maxlife >= 0.400000005960464)
          return;
        for (int index = 0; index < this.Grinders.Length; ++index)
        {
          this.Grinders[index].SmokePEffects[0].SetActive(true);
          this.Grinders[index].SmokePEffects[1].SetActive(true);
        }
      }
    }

    private void Raise()
    {
      this.m_isRaising = true;
      this.m_room.OpenDoors(true);
    }

    private void SpinDown() => this.m_isSliding = false;

    private void Update()
    {
      if (this.m_isSpawningMeat && this.m_meatLeftToSpawn > 0)
      {
        if ((double) this.m_meatSpawnTick > 0.0)
        {
          this.m_meatSpawnTick -= Time.deltaTime;
        }
        else
        {
          --this.m_meatLeftToSpawn;
          this.m_meatSpawnTick = Random.Range(0.75f, 1.5f);
          this.MeatSpawnPoint.localPosition = new Vector3(Random.Range(this.meatSpawnMin.x, this.meatSpawnMax.x), Random.Range(this.meatSpawnMin.y, this.meatSpawnMax.y), Random.Range(this.meatSpawnMin.z, this.meatSpawnMax.z));
          Object.Instantiate<GameObject>(this.ShatterableMeatPrefab_Metal, this.MeatSpawnPoint.position, Random.rotation);
          this.MeatSpawnPoint.localPosition = new Vector3(Random.Range(this.meatSpawnMin.x, this.meatSpawnMax.x), Random.Range(this.meatSpawnMin.y, this.meatSpawnMax.y), Random.Range(this.meatSpawnMin.z, this.meatSpawnMax.z));
          Object.Instantiate<GameObject>(this.ShatterableMeatPrefab_Meat, this.MeatSpawnPoint.position, Random.rotation);
        }
      }
      if (this.m_isLowering)
      {
        this.m_curYPos -= Time.deltaTime * this.m_YSpeed;
        if ((double) this.m_curYPos < (double) this.m_endYPos)
        {
          this.m_curYPos = this.m_endYPos;
          this.m_isLowering = false;
          this.m_isSliding = true;
        }
        for (int index = 0; index < this.Grinders.Length; ++index)
          this.Grinders[index].transform.localPosition = new Vector3(this.Grinders[index].transform.localPosition.x, this.m_curYPos, this.Grinders[index].transform.localPosition.z);
        this.GrinderSound.transform.localPosition = new Vector3(0.0f, this.m_curYPos, this.GrinderSound.transform.localPosition.z);
        this.m_grinderVolume = Mathf.Lerp(this.m_grinderVolume, 0.4f, Time.deltaTime * 0.3f);
        this.m_grinderPitch = Mathf.Lerp(this.m_grinderPitch, 0.7f, Time.deltaTime * 0.3f);
        this.GrinderSound.volume = this.m_grinderVolume;
        this.GrinderSound.pitch = this.m_grinderPitch;
      }
      else if (this.m_isSliding)
      {
        this.m_timeLeft -= Time.deltaTime;
        if ((double) this.m_timeLeft < 0.0)
        {
          this.m_timeLeft = 0.0f;
          this.m_isSliding = false;
        }
        this.m_curZPos = Mathf.Lerp(this.m_startZPos, this.m_endZPos, (float) (1.0 - (double) this.m_timeLeft / (double) this.m_maxTime));
        for (int index = 0; index < this.Grinders.Length; ++index)
          this.Grinders[index].transform.localPosition = new Vector3(this.Grinders[index].transform.localPosition.x, this.Grinders[index].transform.localPosition.y, this.m_curZPos);
        this.GrinderSound.transform.localPosition = new Vector3(0.0f, this.GrinderSound.transform.localPosition.y, this.m_curZPos);
      }
      else if (this.m_isRaising)
      {
        this.m_grinderVolume = Mathf.Lerp(this.m_grinderVolume, 0.0f, Time.deltaTime * 0.7f);
        this.m_grinderPitch = Mathf.Lerp(this.m_grinderPitch, 0.0f, Time.deltaTime * 0.7f);
        this.GrinderSound.volume = this.m_grinderVolume;
        this.GrinderSound.pitch = this.m_grinderPitch;
        this.m_curYPos += Time.deltaTime * this.m_YSpeed;
        if ((double) this.m_curYPos > (double) this.m_startYPos)
          Object.Destroy((Object) this.gameObject);
        for (int index = 0; index < this.Grinders.Length; ++index)
          this.Grinders[index].transform.localPosition = new Vector3(this.Grinders[index].transform.localPosition.x, this.m_curYPos, this.Grinders[index].transform.localPosition.z);
        this.GrinderSound.transform.localPosition = new Vector3(0.0f, this.m_curYPos, this.GrinderSound.transform.localPosition.z);
      }
      else
      {
        this.m_grinderVolume = Mathf.Lerp(this.m_grinderVolume, 0.0f, Time.deltaTime * 0.7f);
        this.m_grinderPitch = Mathf.Lerp(this.m_grinderPitch, 0.0f, Time.deltaTime * 0.7f);
        this.GrinderSound.volume = this.m_grinderVolume;
        this.GrinderSound.pitch = this.m_grinderPitch;
      }
    }
  }
}
