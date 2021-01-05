// Decompiled with JetBrains decompiler
// Type: FistVR.SMEMErow
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class SMEMErow : MonoBehaviour
  {
    public Image Image;
    public Text LBL_Name;
    public Text LBL_Rarity;
    public Text LBL_Amount;
    public Text LBL_BuySell;
    public Text LBL_Price;
    public Text BuySellButton;
    private ItemSpawnerID m_id;
    private int price;
    private int marketIndex;

    public ItemSpawnerID GetID() => this.m_id;

    public int GetPrice() => this.price;

    public void SetMarketPlaceListing(ItemSpawnerID id, SMEME.HatRarity r, int q, int p)
    {
      this.m_id = id;
      this.Image.sprite = id.Sprite;
      this.LBL_Name.text = id.DisplayName;
      this.LBL_Rarity.text = r.ToString();
      this.LBL_Amount.text = q.ToString();
      float num = (float) p * 0.01f;
      this.price = p;
      this.LBL_Price.text = "G" + num.ToString("C", (IFormatProvider) new CultureInfo("en-US"));
    }
  }
}
