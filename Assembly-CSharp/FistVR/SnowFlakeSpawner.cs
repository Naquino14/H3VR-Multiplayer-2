// Decompiled with JetBrains decompiler
// Type: FistVR.SnowFlakeSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class SnowFlakeSpawner : MonoBehaviour
  {
    public GameObject SnowflakePrefab;
    public GameObject SnowflakeLootPrefab;
    public GameObject SnowflakeLootPrefab2;
    private List<GameObject> m_snowFlakes = new List<GameObject>();
    private float m_TickDownToSpawn = 5f;
    public int maxFlakes = 10;
    public float Radius = 75f;
    public Vector2 HeightRange = new Vector2(45f, 95f);
    public Vector2 SpawnDelay = new Vector2(3f, 5f);
    public bool Moving;
    public Vector2 MoveSPeed = new Vector2(10f, 30f);

    private void Start()
    {
    }

    private void Update()
    {
      if ((double) this.m_TickDownToSpawn <= 0.0)
      {
        this.m_TickDownToSpawn = Random.Range(this.SpawnDelay.x, this.SpawnDelay.y);
        this.ClearDeadFlakesAndSpawn();
      }
      else
        this.m_TickDownToSpawn -= Time.deltaTime;
    }

    private void ClearDeadFlakesAndSpawn()
    {
      for (int index = this.m_snowFlakes.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_snowFlakes[index] == (Object) null)
          this.m_snowFlakes.RemoveAt(index);
      }
      if (this.m_snowFlakes.Count >= this.maxFlakes)
        return;
      this.SpawnSnowflake();
    }

    private void SpawnSnowflake()
    {
      Vector2 vector2 = Random.insideUnitCircle * this.Radius;
      GameObject gameObject = Object.Instantiate<GameObject>(this.SnowflakePrefab, new Vector3(vector2.x, Random.Range(this.HeightRange.x, this.HeightRange.y), vector2.y), Random.rotation);
      Rigidbody component = gameObject.GetComponent<Rigidbody>();
      gameObject.GetComponent<FVRDestroyableObject>();
      float num = Random.Range(0.0f, 1f);
      if ((double) num > 0.949999988079071 || (double) num <= 0.899999976158142)
        ;
      component.angularVelocity = Random.onUnitSphere * Random.Range(0.25f, 3f);
      this.m_snowFlakes.Add(gameObject);
      if (!this.Moving)
        return;
      Vector3 onUnitSphere = Random.onUnitSphere;
      onUnitSphere.y = 0.0f;
      component.velocity = onUnitSphere.normalized * Random.Range(this.MoveSPeed.x, this.MoveSPeed.y);
    }
  }
}
