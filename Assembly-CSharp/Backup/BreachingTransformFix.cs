// Decompiled with JetBrains decompiler
// Type: BreachingTransformFix
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class BreachingTransformFix : MonoBehaviour
{
  public List<Transform> refs;
  public List<Transform> points;

  [ContextMenu("DoIt")]
  public void DoIt()
  {
    for (int index = 0; index < this.points.Count; ++index)
    {
      this.points[index].position = this.refs[index].position;
      this.points[index].rotation = this.refs[index].rotation;
    }
  }
}
