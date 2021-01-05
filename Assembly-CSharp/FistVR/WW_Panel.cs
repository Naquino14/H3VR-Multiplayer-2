// Decompiled with JetBrains decompiler
// Type: FistVR.WW_Panel
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class WW_Panel : MonoBehaviour
  {
    [Header("Page Stuff")]
    public List<GameObject> Pages;
    public GameObject Radar;
    public AudioEvent AudEvent_Beep;
    public AudioEvent AudEvent_Boop;
    public WW_TeleportMaster Master;
    public List<Image> Message_Buttons;
    public Sprite Message_Unread;
    public Sprite Message_Read;
    public Text LBL_NumMessagesUnread;
    public Text LBL_SelectedMessage;
    public GameObject BTN_PlayMessage;
    public AudioSource AudSource_MessagePlayback;
    public AudioEvent AudEvent_MessageReceived;
    public AudioEvent AudEvent_MessageStart;
    public AudioEvent AudEvent_MessageEnd;
    public List<WW_Panel.wwMessage> Messages;
    private List<int> m_messageDisplayIndices;
    private int m_curPage;
    private int m_maxPage;
    [Header("Radar Stuff")]
    public TAH_Reticle Reticle;
    public List<Transform> SatcommList;
    public GameObject SatcommSyncPulse;
    [Header("Map Stuff")]
    public List<GameObject> MapPieces;
    private float maxPos = 515f;
    public Transform MapExtentsMin;
    public Transform MapExtentsMax;
    public Transform MapPing;
    private int m_curMessage;
    public Color MapRingSolid;
    public Color MapRingTrans;
    private float m_bunkerpipPulse;
    public List<Image> BunkerSprites;
    public Transform BunkerMapExtentsMin;
    public Transform BunkerMapExtentsMax;

    public void SetPage(int p)
    {
      for (int index = 0; index < this.Pages.Count; ++index)
      {
        if (index == p)
          this.Pages[index].SetActive(true);
        else
          this.Pages[index].SetActive(false);
      }
      if (p == 1)
      {
        this.Radar.SetActive(true);
        this.MapPing.gameObject.SetActive(true);
      }
      else
      {
        this.Radar.SetActive(false);
        this.MapPing.gameObject.SetActive(false);
      }
      this.UpdateMessageDisplay();
    }

    public void UpdateMessageDisplay()
    {
      int num = 0;
      for (int index = 0; index < this.Message_Buttons.Count; ++index)
      {
        if (index < this.Messages.Count)
        {
          if (GM.Options.XmasFlags.MessagesAcquired[index])
          {
            this.Message_Buttons[index].gameObject.SetActive(true);
            if (GM.Options.XmasFlags.MessagesRead[index])
            {
              this.Message_Buttons[index].sprite = this.Message_Read;
            }
            else
            {
              ++num;
              this.Message_Buttons[index].sprite = this.Message_Unread;
            }
          }
          else
            this.Message_Buttons[index].gameObject.SetActive(false);
        }
        else
          this.Message_Buttons[index].gameObject.SetActive(false);
      }
      this.LBL_NumMessagesUnread.text = num.ToString() + " Unread Messages";
      this.LBL_SelectedMessage.text = (this.m_curMessage + 1).ToString() + ". " + this.Messages[this.m_curMessage].Tit;
    }

    private void RegisterInitialContacts()
    {
      for (int index = 0; index < this.SatcommList.Count; ++index)
        this.Reticle.RegisterTrackedObject(this.SatcommList[index], TAH_ReticleContact.ContactType.Supply);
    }

    public void BTN_SetMessage(int i)
    {
      this.m_curMessage = i;
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Beep, this.transform.position);
      this.UpdateMessageDisplay();
    }

    public void BTN_PlayCurrentMessage()
    {
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_MessageStart, this.transform.position);
      this.AudSource_MessagePlayback.Stop();
      this.AudSource_MessagePlayback.clip = this.Messages[this.m_curMessage].AudClip;
      this.AudSource_MessagePlayback.Play();
      GM.Options.XmasFlags.MessagesRead[this.m_curMessage] = true;
      GM.Options.SaveToFile();
      this.UpdateMessageDisplay();
    }

    private void Start()
    {
      for (int index = 0; index < 5; ++index)
        GM.Options.XmasFlags.MessagesAcquired[index] = true;
      this.UpdateMessageDisplay();
      this.RegisterInitialContacts();
      this.UpdateMap();
    }

    private void Update() => this.RadarCheck();

    private void RadarCheck()
    {
      Vector3 position1 = this.transform.position;
      position1.y = 0.0f;
      position1.x = (float) (((double) position1.x + 515.0) / 1030.0);
      position1.z = (float) (((double) position1.z + 515.0) / 1030.0);
      Vector3 vector3 = new Vector3(Mathf.Lerp(this.MapExtentsMin.localPosition.x, this.MapExtentsMax.localPosition.x, position1.x), this.MapPing.localPosition.y, Mathf.Lerp(this.MapExtentsMin.localPosition.z, this.MapExtentsMax.localPosition.z, position1.z));
      this.MapPing.localPosition = vector3;
      for (int index = 0; index < this.SatcommList.Count; ++index)
      {
        if (!GM.Options.XmasFlags.TowersActive[index] && (double) Vector3.Distance(new Vector3(this.SatcommList[index].transform.position.x, 0.0f, this.SatcommList[index].transform.position.z), new Vector3(this.transform.position.x, 0.0f, this.transform.position.z)) < 6.0)
        {
          this.SetSatcomm(index);
          this.UpdateMap();
          this.Master.BunkerUnlockedUpdate();
        }
      }
      for (int index = 0; index < this.BunkerSprites.Count; ++index)
      {
        if (this.Master.Bunkers[index].IsLockDown || this.Master.Bunkers[index].IsUnlocked)
        {
          this.BunkerSprites[index].gameObject.SetActive(false);
        }
        else
        {
          float num1 = Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.Master.Bunkers[index].transform.position);
          this.m_bunkerpipPulse += Time.deltaTime * 0.2f;
          this.m_bunkerpipPulse = Mathf.Repeat(this.m_bunkerpipPulse, 6.283185f);
          if ((double) num1 < 300.0)
          {
            float t = num1 / 250f;
            Color color = Color.Lerp(this.MapRingSolid, this.MapRingTrans, t) * Mathf.Abs(Mathf.Sin(this.m_bunkerpipPulse));
            float num2 = Mathf.Lerp(0.5f, 1.5f, t);
            this.BunkerSprites[index].gameObject.SetActive(true);
            Vector3 position2 = this.Master.Bunkers[index].transform.position;
            position2.x = (float) (((double) position2.x + 515.0) / 1030.0);
            position2.y = (float) (((double) position2.z + 515.0) / 1030.0);
            position2.z = 0.0f;
            vector3 = new Vector3(Mathf.Lerp(this.BunkerMapExtentsMin.localPosition.x, this.BunkerMapExtentsMax.localPosition.x, position2.x), Mathf.Lerp(this.BunkerMapExtentsMin.localPosition.y, this.BunkerMapExtentsMax.localPosition.y, position2.y), this.BunkerSprites[index].transform.localPosition.z);
            this.BunkerSprites[index].transform.localPosition = vector3;
            this.BunkerSprites[index].transform.localScale = new Vector3(num2, num2, num2);
            this.BunkerSprites[index].color = color;
          }
          else
            this.BunkerSprites[index].gameObject.SetActive(false);
        }
      }
    }

    private void UpdateMap()
    {
      for (int index = 0; index < this.MapPieces.Count; ++index)
        this.MapPieces[index].SetActive(GM.Options.XmasFlags.TowersActive[index]);
    }

    private void SetSatcomm(int i)
    {
      UnityEngine.Object.Instantiate<GameObject>(this.SatcommSyncPulse, this.transform.position, Quaternion.identity);
      GM.Options.XmasFlags.TowersActive[i] = true;
      GM.Options.SaveToFile();
    }

    [Serializable]
    public class wwMessage
    {
      public string Tit;
      public AudioClip AudClip;
    }
  }
}
