// Decompiled with JetBrains decompiler
// Type: ftLocalStorage
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ftLocalStorage : ScriptableObject
{
  [SerializeField]
  public List<string> modifiedAssetPathList = new List<string>();
  [SerializeField]
  public List<int> modifiedAssetPaddingHash = new List<int>();
}
