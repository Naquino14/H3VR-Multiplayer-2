// Decompiled with JetBrains decompiler
// Type: FistVR.EngieTurretGenerator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class EngieTurretGenerator : SosigWearable
  {
    [Header("Turret Spawning")]
    public FVRObject TurretObj;
    private AutoMeater m_spawnedTurret;
    private float m_timeToAllowTurretSpawn;
    public LayerMask LM_PlaceBlocker;

    public override void Start() => this.m_timeToAllowTurretSpawn = Random.Range(5f, 20f);

    private void Update()
    {
      if (!((Object) this.L != (Object) null))
        return;
      if ((double) this.m_timeToAllowTurretSpawn > 0.0)
      {
        this.m_timeToAllowTurretSpawn -= Time.deltaTime;
      }
      else
      {
        if (this.S.CurrentOrder != Sosig.SosigOrder.Skirmish)
          return;
        this.m_timeToAllowTurretSpawn = Random.Range(1f, 3f);
        this.PlaceCheck();
      }
    }

    private void PlaceCheck()
    {
      if ((Object) this.m_spawnedTurret != (Object) null)
        return;
      Vector3 position = this.S.transform.position + this.S.transform.forward + this.S.transform.up;
      if (Physics.CheckSphere(position, 0.3f, (int) this.LM_PlaceBlocker))
        return;
      this.m_spawnedTurret = Object.Instantiate<GameObject>(this.TurretObj.GetGameObject(), position, Quaternion.LookRotation(this.S.transform.forward, this.S.transform.up)).GetComponent<AutoMeater>();
    }
  }
}
