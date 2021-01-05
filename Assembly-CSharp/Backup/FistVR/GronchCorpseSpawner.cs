// Decompiled with JetBrains decompiler
// Type: FistVR.GronchCorpseSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class GronchCorpseSpawner : MonoBehaviour
  {
    public List<SosigEnemyTemplate> CorpseList;
    private bool m_isSpawning;
    private float m_timeTilSpawn = 4f;
    private Vector2 TickRange = new Vector2(3f, 10f);
    private GronchJobManager m_M;
    private List<Sosig> m_sosigs = new List<Sosig>();

    public void BeginJob(GronchJobManager m)
    {
      this.m_M = m;
      this.m_isSpawning = true;
    }

    public void EndJob(GronchJobManager m)
    {
      this.m_M = (GronchJobManager) null;
      this.m_isSpawning = false;
      for (int index = this.m_sosigs.Count - 1; index >= 0; --index)
      {
        if ((Object) this.m_sosigs[index] != (Object) null)
          this.m_sosigs[index].ClearSosig();
      }
      this.m_sosigs.Clear();
    }

    private void Update()
    {
      if (!this.m_isSpawning)
        return;
      this.m_timeTilSpawn -= Time.deltaTime;
      if ((double) this.m_timeTilSpawn > 0.0)
        return;
      this.m_timeTilSpawn = Random.Range(this.TickRange.x, this.TickRange.y);
      this.SpawnEnemy(this.CorpseList[Random.Range(0, this.CorpseList.Count)], this.transform, 0, false);
    }

    private void SpawnEnemy(SosigEnemyTemplate t, Transform point, int IFF, bool IsCivvie)
    {
      GameObject weaponPrefab = (GameObject) null;
      if (t.WeaponOptions.Count > 0)
        weaponPrefab = t.WeaponOptions[Random.Range(0, t.WeaponOptions.Count)].GetGameObject();
      GameObject weaponPrefab2 = (GameObject) null;
      if (t.WeaponOptions_Secondary.Count > 0)
        weaponPrefab2 = t.WeaponOptions_Secondary[Random.Range(0, t.WeaponOptions_Secondary.Count)].GetGameObject();
      Sosig sosig = this.SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)].GetGameObject(), weaponPrefab, weaponPrefab2, point.position, point.rotation, t.ConfigTemplates[Random.Range(0, t.ConfigTemplates.Count)], t.OutfitConfig[Random.Range(0, t.OutfitConfig.Count)], IFF);
      sosig.KillSosig();
      sosig.BreakJoint(sosig.Links[0], true, Damage.DamageClass.Abstract);
      sosig.BreakJoint(sosig.Links[1], true, Damage.DamageClass.Abstract);
      sosig.BreakJoint(sosig.Links[2], true, Damage.DamageClass.Abstract);
      sosig.BreakJoint(sosig.Links[3], true, Damage.DamageClass.Abstract);
      AudioImpactController impactController1 = sosig.Links[0].gameObject.AddComponent<AudioImpactController>();
      AudioImpactController impactController2 = sosig.Links[1].gameObject.AddComponent<AudioImpactController>();
      AudioImpactController impactController3 = sosig.Links[2].gameObject.AddComponent<AudioImpactController>();
      AudioImpactController impactController4 = sosig.Links[3].gameObject.AddComponent<AudioImpactController>();
      impactController1.ImpactType = ImpactType.MeatChunk;
      impactController2.ImpactType = ImpactType.MeatChunk;
      impactController3.ImpactType = ImpactType.MeatChunk;
      impactController4.ImpactType = ImpactType.MeatChunk;
      this.m_sosigs.Add(sosig);
    }

    private Sosig SpawnEnemySosig(
      GameObject prefab,
      GameObject weaponPrefab,
      GameObject weaponPrefab2,
      Vector3 pos,
      Quaternion rot,
      SosigConfigTemplate t,
      SosigOutfitConfig w,
      int IFF)
    {
      Sosig componentInChildren = Object.Instantiate<GameObject>(prefab, pos, rot).GetComponentInChildren<Sosig>();
      componentInChildren.Configure(t);
      componentInChildren.E.IFFCode = IFF;
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Headwear)
        this.SpawnAccesoryToLink(w.Headwear, componentInChildren.Links[0]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Facewear)
        this.SpawnAccesoryToLink(w.Facewear, componentInChildren.Links[0]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Torsowear)
        this.SpawnAccesoryToLink(w.Torsowear, componentInChildren.Links[1]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear)
        this.SpawnAccesoryToLink(w.Pantswear, componentInChildren.Links[2]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear_Lower)
        this.SpawnAccesoryToLink(w.Pantswear_Lower, componentInChildren.Links[3]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Backpacks)
        this.SpawnAccesoryToLink(w.Backpacks, componentInChildren.Links[1]);
      componentInChildren.CurrentOrder = Sosig.SosigOrder.Disabled;
      componentInChildren.FallbackOrder = Sosig.SosigOrder.Disabled;
      return componentInChildren;
    }

    private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(gs[Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }
  }
}
