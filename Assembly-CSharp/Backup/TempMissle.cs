// Decompiled with JetBrains decompiler
// Type: TempMissle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class TempMissle : MonoBehaviour
{
  public float TurnSpeed = 180f;
  public float LinearSpeed = 10f;
  public Transform Target;

  private void Start()
  {
  }

  private void Update()
  {
    Vector3 target = this.Target.position - this.transform.position;
    this.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(this.transform.forward, target, this.TurnSpeed * Time.deltaTime, 1f), Vector3.up);
    this.transform.position += Vector3.ClampMagnitude(this.transform.forward * this.LinearSpeed * Time.deltaTime, target.magnitude);
  }
}
