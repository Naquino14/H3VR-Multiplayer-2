// Decompiled with JetBrains decompiler
// Type: FistVR.KeyCodeDisplay
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class KeyCodeDisplay : MonoBehaviour
  {
    public int MaxDigits;
    public Text DisplayText;
    public string MyText = string.Empty;

    public void ButtonMessage(int i)
    {
      if (i < 10 && i > -1 && this.MyText.Length < this.MaxDigits)
        this.MyText += i.ToString();
      this.DisplayText.text = this.MyText;
    }

    public void ClearNumber(int i)
    {
      this.MyText = string.Empty;
      this.DisplayText.text = string.Empty;
    }
  }
}
