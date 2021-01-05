// Decompiled with JetBrains decompiler
// Type: TreeRotater
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class TreeRotater : MonoBehaviour
{
  private void Start()
  {
  }

  private void Update()
  {
  }

  [ContextMenu("rot")]
  public void Rot() => this.transform.eulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360f), 0.0f);
}
