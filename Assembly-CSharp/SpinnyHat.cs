// Decompiled with JetBrains decompiler
// Type: SpinnyHat
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class SpinnyHat : MonoBehaviour
{
  private float r;

  private void Start()
  {
  }

  private void Update()
  {
    this.r += Time.deltaTime * 180f;
    this.r = Mathf.Repeat(this.r, 360f);
    this.transform.localEulerAngles = new Vector3(0.0f, this.r, 0.0f);
  }
}
