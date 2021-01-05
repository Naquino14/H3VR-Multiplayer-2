// Decompiled with JetBrains decompiler
// Type: FistVR.CubeGameSequenceSelectorv1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class CubeGameSequenceSelectorv1 : MonoBehaviour
  {
    public Image[] SelectionLabels;
    public GameObject CanvasRoot;
    public int curSelectedSequence;
    public Color UnSelectedColor;
    public Color SelectedColor;
    private AudioSource aud;

    public void Awake()
    {
      this.UpdateLabelDisplay();
      this.aud = this.GetComponent<AudioSource>();
    }

    public void SelectSequence(int i)
    {
      this.curSelectedSequence = i;
      this.UpdateLabelDisplay();
      this.aud.PlayOneShot(this.aud.clip, 0.15f);
    }

    private void UpdateLabelDisplay()
    {
      for (int index = 0; index < this.SelectionLabels.Length; ++index)
        this.SelectionLabels[index].color = this.UnSelectedColor;
      this.SelectionLabels[this.curSelectedSequence].color = this.SelectedColor;
    }
  }
}
