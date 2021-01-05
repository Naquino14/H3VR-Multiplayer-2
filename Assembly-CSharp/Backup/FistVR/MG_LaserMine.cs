// Decompiled with JetBrains decompiler
// Type: FistVR.MG_LaserMine
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_LaserMine : MonoBehaviour, IFVRDamageable
  {
    private bool m_isExploded;
    public GameObject[] Spawns;
    public Transform SpawnPoint;
    public GameObject LaserBeam;
    public AnimationCurve[] ActivityCurves;
    private int m_curveIndex;
    private bool m_isActive = true;
    private float m_cycleTime = 3f;
    private float m_cycleTick;

    public void Awake()
    {
      this.m_curveIndex = Random.Range(0, this.ActivityCurves.Length - 1);
      this.m_cycleTime = Random.Range(5f, 8f);
      this.m_cycleTick = Random.Range(0.0f, 3f);
    }

    public void Update()
    {
      this.m_cycleTick += Time.deltaTime;
      if ((double) this.m_cycleTick > (double) this.m_cycleTime)
        this.m_cycleTick -= this.m_cycleTime;
      if ((double) this.ActivityCurves[this.m_curveIndex].Evaluate(this.m_cycleTick / this.m_cycleTime) >= 0.5)
      {
        if (this.LaserBeam.activeSelf)
          return;
        this.LaserBeam.SetActive(true);
      }
      else
      {
        if (!this.LaserBeam.activeSelf)
          return;
        this.LaserBeam.SetActive(false);
      }
    }

    public void Explode()
    {
      if (this.m_isExploded)
        return;
      this.m_isExploded = true;
      for (int index = 0; index < this.Spawns.Length; ++index)
        Object.Instantiate<GameObject>(this.Spawns[index], this.SpawnPoint.position, this.SpawnPoint.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    public void Damage(FistVR.Damage d) => this.Explode();
  }
}
