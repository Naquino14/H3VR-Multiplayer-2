// Decompiled with JetBrains decompiler
// Type: FistVR.SmokeSolidEmitter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SmokeSolidEmitter : MonoBehaviour
  {
    public bool Engaged = true;
    public GameObject[] SmokePrefabs;
    public Vector2 DecayRange;
    public Vector2 TickToNextSmokeRange;
    public Vector2 VelRange;
    public int NumSmokeLeftToSpawn = 20;
    private float tickToNextSmoke;

    private void Start()
    {
    }

    private void Update()
    {
      if (!this.Engaged)
        return;
      if ((double) this.tickToNextSmoke > 0.0)
      {
        this.tickToNextSmoke -= Time.deltaTime;
      }
      else
      {
        this.tickToNextSmoke = Random.Range(this.TickToNextSmokeRange.x, this.TickToNextSmokeRange.y);
        --this.NumSmokeLeftToSpawn;
        if (this.NumSmokeLeftToSpawn > 0)
          this.SpawnSmoke();
        else
          Object.Destroy((Object) this.gameObject);
      }
    }

    private void SpawnSmoke()
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.SmokePrefabs[Random.Range(0, this.SmokePrefabs.Length)], this.transform.position + Random.onUnitSphere * 0.05f, Random.rotation);
      gameObject.GetComponent<Rigidbody>().velocity = (Random.onUnitSphere + Vector3.up) * Random.Range(this.VelRange.x, this.VelRange.y);
      gameObject.GetComponent<SmokeSolid>().DecaySpeed = Random.Range(this.DecayRange.x, this.DecayRange.y);
    }
  }
}
