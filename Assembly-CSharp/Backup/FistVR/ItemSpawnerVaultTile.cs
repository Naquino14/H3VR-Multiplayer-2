// Decompiled with JetBrains decompiler
// Type: FistVR.ItemSpawnerVaultTile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ItemSpawnerVaultTile : MonoBehaviour
  {
    public Image Image;
    public Text Text_Name;
    public Text Text_Date;
    public Image LockedCorner;
    public Text LockedText;
    public GameObject DeleteButton;
    public GameObject ConfirmButton;
    public Image[] AttachedComponents;
    public SavedGun SavedGun;
  }
}
