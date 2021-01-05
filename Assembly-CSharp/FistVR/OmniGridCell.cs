// Decompiled with JetBrains decompiler
// Type: FistVR.OmniGridCell
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class OmniGridCell : MonoBehaviour, IFVRDamageable
  {
    public OmniGrid Grid;
    public Text CellText;
    public Image CellImage;
    public Color DefaultImageColor;
    public Color ErrorImageColor;
    private int m_cellNumber;
    private bool m_canBeShot;

    public void SetCanBeShot(bool b) => this.m_canBeShot = b;

    public void SetState(int i, string s)
    {
      this.CellText.text = s;
      this.m_cellNumber = i;
      this.CellImage.color = this.DefaultImageColor;
    }

    public void Damage(FistVR.Damage dam)
    {
      if (dam.Class != FistVR.Damage.DamageClass.Projectile || !this.m_canBeShot || this.Grid.InputNumber(this.m_cellNumber, this))
        return;
      this.CellImage.color = this.ErrorImageColor;
    }
  }
}
