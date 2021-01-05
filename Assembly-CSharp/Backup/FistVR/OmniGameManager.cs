// Decompiled with JetBrains decompiler
// Type: FistVR.OmniGameManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OmniGameManager : MonoBehaviour
  {
    public OmniSequenceLibrary Library;
    private OmniGameManager.OmniGMScreen m_screen;
    public GameObject[] Screens;

    private void ReDrawScreen()
    {
      if (!this.Screens[(int) this.m_screen].activeSelf)
        this.Screens[(int) this.m_screen].SetActive(true);
      for (int index = 0; index < this.Screens.Length; ++index)
      {
        if ((OmniGameManager.OmniGMScreen) index != this.m_screen && this.Screens[index].activeSelf)
          this.Screens[index].SetActive(false);
      }
      switch (this.m_screen)
      {
      }
    }

    public void GotoTheme(int i)
    {
    }

    public void GotoCategory(int i)
    {
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    public enum OmniGMScreen
    {
      TileScreen,
      DetailsScreen,
    }
  }
}
