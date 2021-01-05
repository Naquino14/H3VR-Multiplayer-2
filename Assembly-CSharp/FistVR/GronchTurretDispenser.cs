// Decompiled with JetBrains decompiler
// Type: FistVR.GronchTurretDispenser
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class GronchTurretDispenser : MonoBehaviour
  {
    private GronchJobManager m_M;
    public List<FVRObject> TurretsToSpawn = new List<FVRObject>();
    private bool m_isSpawning;
    private float m_timeTilSpawn = 4f;
    private Vector2 TickRange = new Vector2(2f, 5f);
    private AutoMeater[] m_turrets = new AutoMeater[13];
    public List<Transform> Positions;
    private int m_curPos;
    public AudioEvent AudEvent_Dispense;

    public void BeginJob(GronchJobManager m)
    {
      this.m_M = m;
      this.m_isSpawning = true;
    }

    public void EndJob(GronchJobManager m)
    {
      this.m_M = (GronchJobManager) null;
      this.m_isSpawning = false;
      this.KillTurrets();
    }

    public void PlayerDied(GronchJobManager m)
    {
      this.m_M = m;
      this.m_M.Promotion();
    }

    public void HitButton(int i)
    {
      if (!((Object) this.m_turrets[i] != (Object) null))
        return;
      this.m_turrets[i].KillMe();
      this.m_turrets[i].TickDownToClear(1f);
      this.m_turrets[i] = (AutoMeater) null;
      this.m_M.DidJobStuff();
    }

    private void KillTurrets()
    {
      for (int index = this.m_turrets.Length - 1; index >= 0; --index)
      {
        if ((Object) this.m_turrets[index] != (Object) null)
        {
          this.m_turrets[index].KillMe();
          this.m_turrets[index].TickDownToClear(1f);
          this.m_turrets[index] = (AutoMeater) null;
        }
      }
    }

    private int GetNextPos(int curPos)
    {
      List<int> ts = new List<int>();
      for (int index = 0; index < 13; ++index)
      {
        if (index != curPos)
          ts.Add(index);
      }
      ts.Shuffle<int>();
      ts.Shuffle<int>();
      ts.Shuffle<int>();
      ts.Shuffle<int>();
      return ts[0];
    }

    private void Update()
    {
      if (!this.m_isSpawning)
        return;
      this.m_timeTilSpawn -= Time.deltaTime;
      if ((double) this.m_timeTilSpawn > 0.0)
        return;
      this.m_timeTilSpawn = Random.Range(this.TickRange.x, this.TickRange.y);
      if (!((Object) this.m_turrets[this.m_curPos] == (Object) null))
        return;
      Transform position = this.Positions[this.m_curPos];
      this.m_turrets[this.m_curPos] = this.SpawnEnemy(this.TurretsToSpawn[Random.Range(0, this.TurretsToSpawn.Count)], position, 3);
      this.m_curPos = this.GetNextPos(this.m_curPos);
      if (this.m_curPos < this.Positions.Count)
        return;
      this.m_curPos = 0;
    }

    private AutoMeater SpawnEnemy(FVRObject o, Transform point, int IFF)
    {
      Vector3 onUnitSphere = Random.onUnitSphere;
      onUnitSphere.y = 0.0f;
      onUnitSphere.Normalize();
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Dispense, point.position);
      AutoMeater component = Object.Instantiate<GameObject>(o.GetGameObject(), point.position, Quaternion.LookRotation(onUnitSphere, Vector3.up)).GetComponent<AutoMeater>();
      component.E.IFFCode = IFF;
      return component;
    }
  }
}
