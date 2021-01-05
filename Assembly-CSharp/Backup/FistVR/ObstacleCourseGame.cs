// Decompiled with JetBrains decompiler
// Type: FistVR.ObstacleCourseGame
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ObstacleCourseGame : MonoBehaviour
  {
    public AudioSource AudioMusic;
    public AudioSource Audio2d;
    public AudioClip AudClip_BeginGame;
    public AudioClip AudClip_EndGame;
    public AudioClip AudClip_Penalty;
    private int m_numBuzzersHit;
    private int m_numTargetsHit;
    public GameObject TargetRoot;
    private float m_timer;
    private bool m_isPlaying;
    private bool m_hasPlayed;
    public Text Timer;
    private float m_headPenaltyCooldown = 1f;

    public void BeginGame()
    {
      if (this.m_isPlaying || this.m_hasPlayed)
        return;
      this.m_isPlaying = true;
      this.m_hasPlayed = true;
      this.m_numBuzzersHit = 0;
      this.m_numTargetsHit = 0;
      this.TargetRoot.SetActive(true);
      if (!((Object) this.AudioMusic != (Object) null))
        return;
      this.AudioMusic.Play();
    }

    public void Update()
    {
      if (!this.m_isPlaying)
        return;
      this.m_timer += Time.deltaTime;
      this.Timer.text = "TIME - " + this.FloatToTime(this.m_timer, "#0:00.00");
      this.Timer.text += "\n";
      Text timer1 = this.Timer;
      timer1.text = timer1.text + "BUZZERS - " + this.m_numBuzzersHit.ToString() + "/20";
      this.Timer.text += "\n";
      Text timer2 = this.Timer;
      timer2.text = timer2.text + "TARGETS - " + this.m_numTargetsHit.ToString() + "/60";
      if ((double) this.m_headPenaltyCooldown <= 0.0)
        return;
      this.m_headPenaltyCooldown -= Time.deltaTime;
    }

    public void RegisterHeadPenalty()
    {
      if (!this.m_isPlaying || (double) this.m_headPenaltyCooldown > 0.0)
        return;
      this.m_headPenaltyCooldown = 1f;
      this.m_timer += 10f;
      this.Audio2d.PlayOneShot(this.AudClip_Penalty, 0.5f);
    }

    public void EndGame()
    {
      if (!this.m_isPlaying || !this.m_hasPlayed)
        return;
      this.m_isPlaying = false;
      this.TargetRoot.SetActive(false);
      if (!((Object) this.AudioMusic != (Object) null))
        return;
      this.AudioMusic.Stop();
    }

    public void RegisterBuzzerTouch()
    {
      if (!this.m_isPlaying || !this.m_hasPlayed)
        return;
      ++this.m_numBuzzersHit;
    }

    public void RegisterTargetHit()
    {
      if (!this.m_isPlaying || !this.m_hasPlayed)
        return;
      ++this.m_numTargetsHit;
    }

    public string FloatToTime(float toConvert, string format)
    {
      if (format != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (ObstacleCourseGame.\u003C\u003Ef__switch\u0024map3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          ObstacleCourseGame.\u003C\u003Ef__switch\u0024map3 = new Dictionary<string, int>(13)
          {
            {
              "00.0",
              0
            },
            {
              "#0.0",
              1
            },
            {
              "00.00",
              2
            },
            {
              "00.000",
              3
            },
            {
              "#00.000",
              4
            },
            {
              "#0:00",
              5
            },
            {
              "#00:00",
              6
            },
            {
              "0:00.0",
              7
            },
            {
              "#0:00.0",
              8
            },
            {
              "0:00.00",
              9
            },
            {
              "#0:00.00",
              10
            },
            {
              "0:00.000",
              11
            },
            {
              "#0:00.000",
              12
            }
          };
        }
        int num;
        // ISSUE: reference to a compiler-generated field
        if (ObstacleCourseGame.\u003C\u003Ef__switch\u0024map3.TryGetValue(format, out num))
        {
          switch (num)
          {
            case 0:
              return string.Format("{0:00}:{1:0}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 1:
              return string.Format("{0:#0}:{1:0}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 2:
              return string.Format("{0:00}:{1:00}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 3:
              return string.Format("{0:00}:{1:000}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 4:
              return string.Format("{0:#00}:{1:000}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 5:
              return string.Format("{0:#0}:{1:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0));
            case 6:
              return string.Format("{0:#00}:{1:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0));
            case 7:
              return string.Format("{0:0}:{1:00}.{2:0}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 8:
              return string.Format("{0:#0}:{1:00}.{2:0}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 9:
              return string.Format("{0:0}:{1:00}.{2:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 10:
              return string.Format("{0:#0}:{1:00}.{2:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 11:
              return string.Format("{0:0}:{1:00}.{2:000}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 12:
              return string.Format("{0:#0}:{1:00}.{2:000}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
          }
        }
      }
      return "error";
    }
  }
}
