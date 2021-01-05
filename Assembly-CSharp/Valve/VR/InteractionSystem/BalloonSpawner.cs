// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.BalloonSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class BalloonSpawner : MonoBehaviour
  {
    public float minSpawnTime = 5f;
    public float maxSpawnTime = 15f;
    private float nextSpawnTime;
    public GameObject balloonPrefab;
    public bool autoSpawn = true;
    public bool spawnAtStartup = true;
    public bool playSounds = true;
    public SoundPlayOneshot inflateSound;
    public SoundPlayOneshot stretchSound;
    public bool sendSpawnMessageToParent;
    public float scale = 1f;
    public Transform spawnDirectionTransform;
    public float spawnForce;
    public bool attachBalloon;
    public Balloon.BalloonColor color = Balloon.BalloonColor.Random;

    private void Start()
    {
      if ((Object) this.balloonPrefab == (Object) null || !this.autoSpawn || !this.spawnAtStartup)
        return;
      this.SpawnBalloon(this.color);
      this.nextSpawnTime = Random.Range(this.minSpawnTime, this.maxSpawnTime) + Time.time;
    }

    private void Update()
    {
      if ((Object) this.balloonPrefab == (Object) null || (double) Time.time <= (double) this.nextSpawnTime || !this.autoSpawn)
        return;
      this.SpawnBalloon(this.color);
      this.nextSpawnTime = Random.Range(this.minSpawnTime, this.maxSpawnTime) + Time.time;
    }

    public GameObject SpawnBalloon(Balloon.BalloonColor color = Balloon.BalloonColor.Red)
    {
      if ((Object) this.balloonPrefab == (Object) null)
        return (GameObject) null;
      GameObject gameObject = Object.Instantiate<GameObject>(this.balloonPrefab, this.transform.position, this.transform.rotation);
      gameObject.transform.localScale = new Vector3(this.scale, this.scale, this.scale);
      if (this.attachBalloon)
        gameObject.transform.parent = this.transform;
      if (this.sendSpawnMessageToParent && (Object) this.transform.parent != (Object) null)
        this.transform.parent.SendMessage("OnBalloonSpawned", (object) gameObject, SendMessageOptions.DontRequireReceiver);
      if (this.playSounds)
      {
        if ((Object) this.inflateSound != (Object) null)
          this.inflateSound.Play();
        if ((Object) this.stretchSound != (Object) null)
          this.stretchSound.Play();
      }
      gameObject.GetComponentInChildren<Balloon>().SetColor(color);
      if ((Object) this.spawnDirectionTransform != (Object) null)
        gameObject.GetComponentInChildren<Rigidbody>().AddForce(this.spawnDirectionTransform.forward * this.spawnForce);
      return gameObject;
    }

    public void SpawnBalloonFromEvent(int color) => this.SpawnBalloon((Balloon.BalloonColor) color);
  }
}
