// Decompiled with JetBrains decompiler
// Type: FistVR.SosigWearableConfig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Wearable Config", menuName = "Sosig/WearableConfig", order = 0)]
  public class SosigWearableConfig : ScriptableObject
  {
    public List<GameObject> Headwear;
    public float Chance_Headwear;
    [Space(5f)]
    public List<GameObject> Facewear;
    public float Chance_Facewear;
    [Space(5f)]
    public List<GameObject> Torsowear;
    public float Chance_Torsowear;
    [Space(5f)]
    public List<GameObject> Pantswear;
    public float Chance_Pantswear;
    [Space(5f)]
    public List<GameObject> Pantswear_Lower;
    public float Chance_Pantswear_Lower;
    [Space(5f)]
    public List<GameObject> Backpacks;
    public float Chance_Backpacks;
  }
}
