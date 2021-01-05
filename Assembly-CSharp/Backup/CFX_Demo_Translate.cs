// Decompiled with JetBrains decompiler
// Type: CFX_Demo_Translate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class CFX_Demo_Translate : MonoBehaviour
{
  public float speed = 30f;
  public Vector3 rotation = Vector3.forward;
  public Vector3 axis = Vector3.forward;
  public bool gravity;
  private Vector3 dir;

  private void Start()
  {
    this.dir = new Vector3(Random.Range(0.0f, 360f), Random.Range(0.0f, 360f), Random.Range(0.0f, 360f));
    this.dir.Scale(this.rotation);
    this.transform.localEulerAngles = this.dir;
  }

  private void Update() => this.transform.Translate(this.axis * this.speed * Time.deltaTime, Space.Self);
}
