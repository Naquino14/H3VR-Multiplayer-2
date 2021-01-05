// Decompiled with JetBrains decompiler
// Type: fixchildissue
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class fixchildissue : MonoBehaviour
{
  public Transform Parent;
  public Transform Child;

  [ContextMenu("FixMe")]
  public void FixMe()
  {
    Vector3 position = this.Child.position;
    Quaternion rotation = this.Child.rotation;
    this.Parent.position = position;
    this.Parent.rotation = rotation;
  }

  [ContextMenu("FixMe2")]
  public void FixMe2()
  {
    this.Child.position = this.Parent.position;
    this.Child.rotation = this.Parent.rotation;
  }
}
