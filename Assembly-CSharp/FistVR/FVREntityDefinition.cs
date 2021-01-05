// Decompiled with JetBrains decompiler
// Type: FistVR.FVREntityDefinition
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Definition", menuName = "Entities/EntityDefinition", order = 0)]
  public class FVREntityDefinition : ScriptableObjectHasWeakLink
  {
    public string EntityID = string.Empty;
    public GameObject MainPrefab;
    public GameObject ProxyPrefab;
    public Sprite Sprite;
    public string DisplayName;
    [TextArea(3, 13)]
    public string Details;
    public List<FVREntityTag> Tags;
    public Vector3 ProxyExtents = new Vector3(1f, 1f, 1f);

    public override void OnBuild()
    {
    }
  }
}
