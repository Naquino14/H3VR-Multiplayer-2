// Decompiled with JetBrains decompiler
// Type: FistVR.Tome
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Tome : MonoBehaviour
  {
    private Vector3 basePos;
    public List<FVRIgnitable> Igniteables;
    public Transform JerrySpawnPoint;
    public GameObject JerrySpawnIn;
    public GameObject JerrySpawnOut;
    public GameObject StaffSpawn;
    public AudioSource Chant;
    private bool m_hasSummoned;
    private float m_tick;
    private bool m_isCircleReady;

    private void Start()
    {
      this.basePos = this.transform.position;
      GM.CurrentSceneSettings.SosigKillEvent += new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);
    }

    private void OnDestroy() => GM.CurrentSceneSettings.SosigKillEvent -= new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);

    public void CheckIfDeadSosigWasMine(Sosig s)
    {
      if (!this.m_isCircleReady || this.m_hasSummoned || ((Object) s == (Object) null || (Object) s.Links[1] == (Object) null) || (s.E.IFFCode == 1 || s.E.IFFCode == 2 || s.GetDiedFromType() != Sosig.SosigDeathType.JointPullApart && s.GetDiedFromType() != Sosig.SosigDeathType.JointSever) || (double) Vector3.Distance(s.Links[1].transform.position, this.transform.position) >= 5.0)
        return;
      this.Summon();
    }

    private void Summon()
    {
      this.m_hasSummoned = true;
      Object.Instantiate<GameObject>(this.JerrySpawnIn, this.JerrySpawnPoint.position, this.JerrySpawnPoint.rotation);
      this.Invoke("SummonStaff", 5f);
    }

    private void SummonStaff()
    {
      Object.Instantiate<GameObject>(this.JerrySpawnOut, this.JerrySpawnPoint.position, this.JerrySpawnPoint.rotation);
      Object.Instantiate<GameObject>(this.StaffSpawn, this.JerrySpawnPoint.position, Random.rotation);
      GM.ZMaster.FlagM.SetFlag("m_ttt_sm", 1);
    }

    private void Update()
    {
      float num = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position);
      if ((double) num > 45.0)
      {
        if (this.Chant.isPlaying)
          this.Chant.Stop();
      }
      else if ((double) num < 40.0)
      {
        if (!this.Chant.isPlaying)
          this.Chant.Play();
        this.Chant.volume = Mathf.Lerp(0.3f, 0.0f, Mathf.InverseLerp(20f, 40f, num));
      }
      this.m_tick += Time.deltaTime * 0.2f;
      this.m_tick = Mathf.Repeat(this.m_tick, 6.283185f);
      this.transform.position = this.basePos + Mathf.Sin(this.m_tick) * Vector3.up * 0.1f;
      if (this.m_isCircleReady)
        return;
      bool flag = true;
      for (int index = 0; index < this.Igniteables.Count; ++index)
      {
        if (!this.Igniteables[index].IsOnFire())
        {
          flag = false;
          break;
        }
      }
      if (!flag)
        return;
      this.m_isCircleReady = true;
    }
  }
}
