// Decompiled with JetBrains decompiler
// Type: FistVR.MainMenuTutScreen
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class MainMenuTutScreen : MonoBehaviour
  {
    private int m_curScreen;
    [Multiline(3)]
    public string[] Descrips;
    public Sprite[] Images;
    public Text DescripText;
    public Text Counter;
    public Image Image;

    public void Increment()
    {
      ++this.m_curScreen;
      if (this.m_curScreen > 3)
        this.m_curScreen = 0;
      this.UpdateScreen();
    }

    public void Decrement()
    {
      --this.m_curScreen;
      if (this.m_curScreen < 0)
        this.m_curScreen = 3;
      this.UpdateScreen();
    }

    private void UpdateScreen()
    {
      this.DescripText.text = this.Descrips[this.m_curScreen];
      this.Counter.text = (this.m_curScreen + 1).ToString() + "/4";
      this.Image.sprite = this.Images[this.m_curScreen];
    }
  }
}
