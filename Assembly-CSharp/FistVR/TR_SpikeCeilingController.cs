// Decompiled with JetBrains decompiler
// Type: FistVR.TR_SpikeCeilingController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TR_SpikeCeilingController : MonoBehaviour, IRoomTriggerable
  {
    public TR_SpikeCeilingBase[] Bases;
    private TR_SpikeCeilingPlate[] Plates;
    public GameObject SpikeCeilingPlatePrefab;
    public AlloyAreaLight MyLight;
    private float curIntensity;
    private float tarIntensity;
    private bool m_isTriggered;
    private bool m_isDescending;
    private bool m_isRetracting;
    private float m_trapHeight = 11f;
    private RedRoom m_room;
    public AudioSource CeilingAudio;
    public AudioClip RetractClip;
    public AudioClip ExpandClip;
    public AudioSource RumblingAudio;
    private float m_tickTilToggle = 1f;
    private bool m_isRetracted;

    public void SetRoom(RedRoom room) => this.m_room = room;

    public void Init(int roomTileSize, RedRoom room)
    {
      GM.MGMaster.Narrator.PlayTrapRoomInit();
      this.GetComponent<AudioSource>().Play();
      this.m_room = room;
      this.m_isTriggered = true;
      this.MyLight.gameObject.SetActive(true);
      this.tarIntensity = 0.45f;
      for (int index = 0; index < this.Bases.Length; ++index)
      {
        this.Bases[index].gameObject.SetActive(true);
        this.Bases[index].transform.position = new Vector3(this.Bases[index].transform.position.x, 0.0f, this.Bases[index].transform.position.z);
      }
      int index1 = 0;
      switch (roomTileSize)
      {
        case 2:
          this.Plates = new TR_SpikeCeilingPlate[4];
          for (int index2 = 0; index2 < 2; ++index2)
          {
            for (int index3 = 0; index3 < 2; ++index3)
            {
              GameObject gameObject = Object.Instantiate<GameObject>(this.SpikeCeilingPlatePrefab, Vector3.zero, Quaternion.identity);
              gameObject.transform.SetParent(this.transform);
              gameObject.transform.localPosition = new Vector3((float) ((double) index2 * 2.0 - 1.0), 11f, (float) ((double) index3 * 2.0 - 1.0));
              this.Plates[index1] = gameObject.GetComponent<TR_SpikeCeilingPlate>();
              ++index1;
            }
          }
          break;
        case 3:
          this.Plates = new TR_SpikeCeilingPlate[9];
          for (int index2 = 0; index2 < 3; ++index2)
          {
            for (int index3 = 0; index3 < 3; ++index3)
            {
              GameObject gameObject = Object.Instantiate<GameObject>(this.SpikeCeilingPlatePrefab, Vector3.zero, Quaternion.identity);
              gameObject.transform.SetParent(this.transform);
              gameObject.transform.localPosition = new Vector3((float) ((double) index2 * 2.0 - 2.0), 11f, (float) ((double) index3 * 2.0 - 2.0));
              this.Plates[index1] = gameObject.GetComponent<TR_SpikeCeilingPlate>();
              ++index1;
            }
          }
          break;
        case 4:
          this.Plates = new TR_SpikeCeilingPlate[16];
          for (int index2 = 0; index2 < 4; ++index2)
          {
            for (int index3 = 0; index3 < 4; ++index3)
            {
              GameObject gameObject = Object.Instantiate<GameObject>(this.SpikeCeilingPlatePrefab, Vector3.zero, Quaternion.identity);
              gameObject.transform.SetParent(this.transform);
              gameObject.transform.localPosition = new Vector3((float) ((double) index2 * 2.0 - 3.0), 11f, (float) ((double) index3 * 2.0 - 3.0));
              this.Plates[index1] = gameObject.GetComponent<TR_SpikeCeilingPlate>();
              ++index1;
            }
          }
          break;
      }
      this.m_isDescending = true;
      this.RumblingAudio.Play();
    }

    private void StopTrap()
    {
      this.m_isDescending = false;
      this.tarIntensity = 0.0f;
      this.m_isRetracting = true;
      this.m_room.OpenDoors(true);
      this.Invoke("KillTrap", 15f);
    }

    private void KillTrap() => Object.Destroy((Object) this.gameObject);

    private void Descend()
    {
      this.m_trapHeight -= Time.deltaTime * 0.17f;
      this.CeilingAudio.transform.localPosition = new Vector3(0.0f, this.m_trapHeight, 0.0f);
      int num = 0;
      for (int index = 0; index < this.Bases.Length; ++index)
      {
        if ((double) this.Bases[index].CurHeight >= (double) this.m_trapHeight)
          ++num;
      }
      if (num >= 3 || (double) this.m_trapHeight <= 0.800000011920929)
      {
        this.StopTrap();
      }
      else
      {
        for (int index = 0; index < this.Bases.Length; ++index)
          this.Bases[index].LowerTo(this.m_trapHeight);
        for (int index = 0; index < this.Plates.Length; ++index)
          this.Plates[index].transform.localPosition = new Vector3(this.Plates[index].transform.localPosition.x, this.m_trapHeight, this.Plates[index].transform.localPosition.z);
        if ((double) this.m_tickTilToggle <= 0.0)
        {
          this.m_tickTilToggle = 3f;
          this.ToggleSpikes();
        }
        else
          this.m_tickTilToggle -= Time.deltaTime;
      }
    }

    private void ToggleSpikes()
    {
      this.m_isRetracted = !this.m_isRetracted;
      if (this.m_isRetracted)
      {
        for (int index = 0; index < this.Plates.Length; ++index)
        {
          this.Plates[index].Retract();
          this.CeilingAudio.PlayOneShot(this.RetractClip, 0.15f);
        }
      }
      else
      {
        for (int index = 0; index < this.Plates.Length; ++index)
        {
          this.Plates[index].Expand();
          this.CeilingAudio.PlayOneShot(this.ExpandClip, 0.15f);
        }
      }
    }

    public void Update()
    {
      if (this.m_isTriggered)
      {
        if ((double) this.curIntensity < (double) this.tarIntensity)
        {
          this.curIntensity += Time.deltaTime * 0.3f;
          this.MyLight.Intensity = this.curIntensity;
          this.RumblingAudio.volume = this.curIntensity * 1.5f;
        }
        if (this.m_isDescending)
          this.Descend();
      }
      if (!this.m_isRetracting)
        return;
      this.transform.position += Vector3.up * Time.deltaTime;
      if ((double) this.curIntensity > (double) this.tarIntensity)
      {
        this.curIntensity -= Time.deltaTime * 0.2f;
        this.MyLight.Intensity = this.curIntensity;
        this.RumblingAudio.volume = this.curIntensity * 1.5f;
      }
      else
      {
        this.MyLight.gameObject.SetActive(false);
        this.RumblingAudio.Stop();
      }
    }
  }
}
