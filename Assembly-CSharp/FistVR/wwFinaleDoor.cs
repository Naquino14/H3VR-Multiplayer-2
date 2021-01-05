// Decompiled with JetBrains decompiler
// Type: FistVR.wwFinaleDoor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwFinaleDoor : MonoBehaviour
  {
    public Vector3 UpPosition;
    public Vector3 DownPosition;
    private float m_doorLerp;
    public float DoorLerpSpeed = 1f;
    private bool m_isOpening;
    private int m_doorState;
    public GameObject KeyProxy;
    public GameObject KeyDetectProxy;
    private AudioSource Aud;
    public bool TestDoor;
    public int Index;

    public void Awake()
    {
      this.Aud = this.GetComponent<AudioSource>();
      if (this.TestDoor)
        this.Invoke("OpenDoor", 3f);
      this.KeyProxy.SetActive(false);
      this.KeyDetectProxy.SetActive(true);
    }

    public void OpenDoor()
    {
      if (this.m_doorState != 0)
        return;
      this.KeyProxy.SetActive(true);
      this.KeyDetectProxy.SetActive(false);
      this.m_isOpening = true;
      this.Aud.Play();
    }

    public void ConfigureDoorState(int state)
    {
      this.m_doorState = state;
      if (state == 0)
      {
        this.transform.localPosition = this.UpPosition;
        this.m_doorLerp = 0.0f;
        this.m_isOpening = false;
        this.KeyProxy.SetActive(false);
        this.KeyDetectProxy.SetActive(true);
      }
      else
      {
        if (state != 1)
          return;
        this.transform.localPosition = this.DownPosition;
        this.m_doorLerp = 1f;
        this.m_isOpening = false;
        this.KeyProxy.SetActive(true);
        this.KeyDetectProxy.SetActive(false);
      }
    }

    public void Update()
    {
      if (!this.m_isOpening)
        return;
      if ((double) this.m_doorLerp < 1.0)
      {
        this.m_doorLerp += Time.deltaTime * this.DoorLerpSpeed;
      }
      else
      {
        this.m_isOpening = false;
        this.m_doorLerp = 1f;
        this.m_doorState = 1;
      }
      this.transform.localPosition = Vector3.Lerp(this.UpPosition, this.DownPosition, this.m_doorLerp);
    }
  }
}
