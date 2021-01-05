// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsPanel_Screenmanager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class OptionsPanel_Screenmanager : MonoBehaviour
  {
    public GameObject[] Screens;
    public Transform[] Lasers;
    public Transform[] RedLasers;
    public Transform[] BlueLasers;
    public Transform[] ScreenCorners;
    public LayerMask ButtonMask;
    private RaycastHit m_hit;
    public AudioSource audsource;
    public AudioClip audConfirm;
    public OptionsScreen_Controls OPS_Controls;
    public OptionsScreen_GUns OPS_Guns;
    public OptionsScreen_Movement OPS_Movement;
    public OptionsScreen_Quality OPS_Quality;
    public bool IsDebug;
    public Text DebugText;
    public int n;
    public GameObject n2;

    public void SetScreen(int index)
    {
      for (int index1 = 0; index1 < this.Screens.Length; ++index1)
      {
        if (index1 != index)
          this.Screens[index1].SetActive(false);
      }
      this.Screens[index].SetActive(true);
      this.Screens[index].SendMessage("InitScreen", SendMessageOptions.DontRequireReceiver);
    }

    public void RefreshScreens()
    {
      this.OPS_Controls.InitScreen();
      this.OPS_Guns.InitScreen();
      this.OPS_Movement.InitScreen();
      this.OPS_Quality.InitScreen();
      if (!this.IsDebug)
        return;
      if (!this.DebugText.gameObject.activeSelf)
        this.DebugText.gameObject.SetActive(true);
      else
        this.DebugText.gameObject.SetActive(false);
    }

    public void Update()
    {
      if (!this.IsDebug)
        return;
      this.DebugText.text = GM.CurrentPlayerBody.DebugString;
    }

    public void ntest()
    {
      ++this.n;
      if (this.n <= 5)
        return;
      Object.Instantiate<GameObject>(this.n2, this.transform.position + Vector3.up, this.transform.rotation);
    }
  }
}
