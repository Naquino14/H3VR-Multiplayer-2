// Decompiled with JetBrains decompiler
// Type: FistVR.AdventBox
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AdventBox : MonoBehaviour
  {
    public int Day;
    public GameObject[] Pictures;
    private bool m_isOpen;
    public FVRObject[] ObjsToSpawn;
    public Transform[] PositionToSpawn;
    public Transform Door;
    public Transform StartDoorPos;
    public Transform FinalDoorPos;
    private float m_doorOpenTick;
    private bool m_isDoorOpening;
    public AudioSource BoxAudio;
    public GameObject Fetti;
    public GameObject Blocker;
    public AdventBoxLever Lever;
    public GameObject TextDescription;

    private void Awake() => this.Init();

    private void Init() => this.Pictures[this.Day - 1].SetActive(true);

    public void OpenBox()
    {
      if (this.m_isOpen)
        return;
      this.m_isOpen = true;
      this.m_isDoorOpening = true;
      this.Blocker.SetActive(false);
      this.Fetti.SetActive(true);
      this.TextDescription.SetActive(true);
      for (int index = 0; index < this.ObjsToSpawn.Length; ++index)
      {
        if ((Object) this.ObjsToSpawn[index] != (Object) null)
          Object.Instantiate<GameObject>(this.ObjsToSpawn[index].GetGameObject(), this.PositionToSpawn[index].position, this.PositionToSpawn[index].rotation);
      }
      this.BoxAudio.Play();
      this.UpdateFlag();
    }

    private void Update()
    {
      if (!this.m_isDoorOpening)
        return;
      if ((double) this.m_doorOpenTick < 1.0)
      {
        this.m_doorOpenTick += Time.deltaTime * 0.1f;
        this.Door.transform.position = Vector3.Lerp(this.StartDoorPos.position, this.FinalDoorPos.position, this.m_doorOpenTick);
      }
      else
        this.m_isDoorOpening = false;
    }

    private void UpdateFlag()
    {
    }
  }
}
