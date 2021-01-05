// Decompiled with JetBrains decompiler
// Type: MainMenuPlinth
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class MainMenuPlinth : MonoBehaviour
{
  private Vector3 startPos;
  private float startRot;
  private float sinTick;
  private float sinSpeed;

  private void Awake()
  {
    this.startPos = this.transform.position;
    this.sinTick = Random.Range(0.0f, 10f);
    this.startRot = this.transform.localEulerAngles.y;
    this.sinSpeed = Random.Range(0.02f, 0.06f);
  }

  private void Start()
  {
  }

  private void Update()
  {
    this.sinTick += Time.deltaTime * this.sinSpeed;
    Vector3 startPos = this.startPos;
    startPos.x = this.startPos.x + Mathf.PerlinNoise(Time.time * 0.05f, this.startPos.y);
    startPos.z = this.startPos.z + Mathf.PerlinNoise(Time.time * 0.08f, this.startPos.y + 2f);
    startPos.y = this.startPos.y + Mathf.Sin(this.sinTick) * 50f;
    this.transform.position = startPos;
    this.startRot += (float) (((double) Mathf.PerlinNoise(Time.time * 0.1f, this.startPos.y) - 0.5) * (double) Time.deltaTime * 40.0);
    this.transform.localEulerAngles = new Vector3(0.0f, this.startRot, 0.0f);
  }
}
