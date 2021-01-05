// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_ObjectConstructorIcon
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class TNH_ObjectConstructorIcon : MonoBehaviour
  {
    public Image Image;
    public List<GameObject> Costs;
    private Sprite m_sprite;
    public int Cost;
    public Sprite Sprite_Accept;
    public Sprite Sprite_Cancel;
    public TNH_ObjectConstructorIcon.IconState State;

    public void Init()
    {
    }

    public void SetOption(TNH_ObjectConstructorIcon.IconState state, Sprite s, int cost)
    {
      this.State = state;
      this.m_sprite = s;
      this.Cost = cost;
      this.Cost = Mathf.Clamp(this.Cost, this.Cost, 12);
      this.UpdateCostDisplay();
      this.UpdateIconDisplay();
    }

    private void UpdateCostDisplay()
    {
      for (int index = 0; index < this.Costs.Count; ++index)
      {
        if (index + 1 == this.Cost && this.State == TNH_ObjectConstructorIcon.IconState.Item)
          this.Costs[index].SetActive(true);
        else
          this.Costs[index].SetActive(false);
      }
    }

    private void UpdateIconDisplay()
    {
      switch (this.State)
      {
        case TNH_ObjectConstructorIcon.IconState.Item:
          this.Image.sprite = this.m_sprite;
          break;
        case TNH_ObjectConstructorIcon.IconState.Accept:
          this.Image.sprite = this.Sprite_Accept;
          break;
        case TNH_ObjectConstructorIcon.IconState.Cancel:
          this.Image.sprite = this.Sprite_Cancel;
          break;
      }
    }

    public enum IconState
    {
      Item,
      Accept,
      Cancel,
    }
  }
}
