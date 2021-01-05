// Decompiled with JetBrains decompiler
// Type: FistVR.Keyboard
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class Keyboard : MonoBehaviour
  {
    public List<KeyboardKey> Keys;
    public Text ActiveText;
    private string m_textString = string.Empty;
    private bool m_isCaps;
    private bool m_clearCapsAfterNextKey = true;
    public AudioSource KeyBoardAudio;
    private float timeSinceHit = 1f;
    public float reHitThreshold = 0.1f;

    public void SetActiveText(Text t)
    {
      this.ActiveText = t;
      this.m_textString = this.ActiveText.text;
      this.UpdateActiveText();
    }

    private void UpdateActiveText()
    {
      if (!((Object) this.ActiveText != (Object) null))
        return;
      this.ActiveText.text = this.m_textString;
    }

    private void Update()
    {
      if ((double) this.timeSinceHit >= 1.0)
        return;
      this.timeSinceHit += Time.deltaTime;
    }

    private void Start() => this.ReDraw();

    private void SetCaps(bool b)
    {
      this.m_isCaps = b;
      this.ReDraw();
    }

    private void ToggleCaps() => this.SetCaps(!this.m_isCaps);

    private void ReDraw()
    {
      for (int index = 0; index < this.Keys.Count; ++index)
        this.Keys[index].Text.text = !this.m_isCaps ? this.Keys[index].LowerCase : this.Keys[index].UpperCase;
    }

    public void KeyInput(KeyboardKey.KeyBoardKeyType type, string lowercase, string uppercase)
    {
      if ((double) this.timeSinceHit < (double) this.reHitThreshold)
        return;
      this.timeSinceHit = 0.0f;
      this.KeyBoardAudio.pitch = Random.Range(0.9f, 1f);
      this.KeyBoardAudio.PlayOneShot(this.KeyBoardAudio.clip, 0.3f);
      switch (type)
      {
        case KeyboardKey.KeyBoardKeyType.AlphaNumeric:
          if (this.m_isCaps)
          {
            this.m_textString += uppercase;
            if (this.m_clearCapsAfterNextKey)
            {
              this.SetCaps(false);
              break;
            }
            break;
          }
          this.m_textString += lowercase;
          break;
        case KeyboardKey.KeyBoardKeyType.Space:
          this.m_textString += " ";
          break;
        case KeyboardKey.KeyBoardKeyType.Tab:
          this.m_textString += "    ";
          break;
        case KeyboardKey.KeyBoardKeyType.Shift:
          this.ToggleCaps();
          this.m_clearCapsAfterNextKey = true;
          break;
        case KeyboardKey.KeyBoardKeyType.Caps:
          if (this.m_isCaps)
          {
            this.SetCaps(false);
            this.m_clearCapsAfterNextKey = true;
            break;
          }
          this.SetCaps(true);
          this.m_clearCapsAfterNextKey = false;
          break;
        case KeyboardKey.KeyBoardKeyType.Backspace:
          if (this.m_textString.Length > 0)
          {
            this.m_textString = this.m_textString.Substring(0, this.m_textString.Length - 1);
            break;
          }
          break;
      }
      this.UpdateActiveText();
    }
  }
}
