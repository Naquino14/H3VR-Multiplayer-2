// Decompiled with JetBrains decompiler
// Type: FistVR.LawnDartPointDisplay
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class LawnDartPointDisplay : MonoBehaviour
  {
    public Text LabelText;
    private bool m_isFadingDown;
    private float m_alpha = 1f;
    private Color m_color = new Color(1f, 1f, 1f, 1f);
    private float m_upwardSpeed = 4f;
    public LawnDartGame Game;

    public void Activate(string txt, Vector3 pos, float upwardSpeed, int fireworks)
    {
      this.CancelInvoke();
      this.LabelText.text = txt;
      this.m_alpha = 1f;
      this.m_color.a = this.m_alpha;
      this.LabelText.color = this.m_color;
      this.transform.position = pos;
      this.m_upwardSpeed = upwardSpeed;
      this.m_isFadingDown = true;
      for (int index = 0; index < fireworks; ++index)
        this.Invoke("FireFireWork", 3.5f - (float) index);
    }

    private void FireFireWork() => this.Game.FireWork(this.transform.position + Random.onUnitSphere * 3f);

    public void Update()
    {
      if (this.m_isFadingDown)
      {
        if ((double) this.m_alpha > 0.0)
        {
          this.m_alpha -= Time.deltaTime * 0.25f;
        }
        else
        {
          this.m_alpha = 0.0f;
          this.m_isFadingDown = false;
        }
        this.transform.position += Vector3.up * this.m_upwardSpeed * Time.deltaTime;
      }
      this.m_color.a = this.m_alpha;
      this.LabelText.color = this.m_color;
      if ((double) this.m_alpha > 0.0)
        return;
      this.gameObject.SetActive(false);
    }
  }
}
