// Decompiled with JetBrains decompiler
// Type: BushPrepper
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class BushPrepper : MonoBehaviour
{
  private void Start()
  {
  }

  private void Update()
  {
  }

  [ContextMenu("rot")]
  public void Rot()
  {
    float x = Random.Range(-15f, 15f);
    float z = Random.Range(-15f, 15f);
    this.transform.eulerAngles = new Vector3(x, Random.Range(0.0f, 360f), z);
    float num = Random.Range(1f, 2f);
    this.transform.localScale = new Vector3(num, num, num);
  }
}
