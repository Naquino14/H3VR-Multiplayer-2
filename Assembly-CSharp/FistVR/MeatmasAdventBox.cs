// Decompiled with JetBrains decompiler
// Type: FistVR.MeatmasAdventBox
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MeatmasAdventBox : MonoBehaviour
  {
    public MM_MusicManager MM;
    public Transform Cover;
    public AudioEvent AudEvent_Music;
    public GameObject MeatmasFetti;
    public GameObject Blocker;
    public GameObject TextInfo;
    private bool m_isOpening;
    private bool m_isOpened;
    private float m_openLerp;
    public ZosigEnemyTemplate ElfWiener;
    public Transform ElfPoint1;
    public Transform ElfPoint2;
    public List<Transform> SpawnPoints;
    public List<FVRObject> ObjectsToSpawn;
    private bool m_shouldRestartMusic;
    private float HeadJitterTick = 0.1f;
    private List<Sosig> m_civvieSosigs = new List<Sosig>();

    public void Open()
    {
      if (this.m_isOpening || this.m_isOpened)
        return;
      this.m_isOpening = true;
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Music, this.MeatmasFetti.transform.position);
      this.MeatmasFetti.SetActive(true);
      this.Blocker.SetActive(false);
      this.TextInfo.SetActive(true);
      for (int index = 0; index < this.ObjectsToSpawn.Count; ++index)
        Object.Instantiate<GameObject>(this.ObjectsToSpawn[index].GetGameObject(), this.SpawnPoints[index].position, this.SpawnPoints[index].rotation);
      this.SpawnEnemy(this.ElfWiener, this.ElfPoint1, 0);
      this.SpawnEnemy(this.ElfWiener, this.ElfPoint2, 0);
      if (!this.MM.IsMusicEnabled())
        return;
      this.m_shouldRestartMusic = true;
      this.MM.DisableMusic();
    }

    public void Update()
    {
      if (!this.m_isOpening)
        return;
      this.m_openLerp += Time.deltaTime * 0.078f;
      if ((double) this.m_openLerp > 1.0)
      {
        this.m_openLerp = 1f;
        this.m_isOpening = false;
        this.m_isOpened = true;
        if (this.m_shouldRestartMusic)
        {
          this.MM.EnableMusic();
          this.m_shouldRestartMusic = false;
        }
        for (int index = 0; index < this.m_civvieSosigs.Count; ++index)
          this.m_civvieSosigs[index].ClearSosig();
        this.m_civvieSosigs.Clear();
      }
      else if ((double) this.HeadJitterTick > 0.0)
      {
        this.HeadJitterTick -= Time.deltaTime;
      }
      else
      {
        this.HeadJitterTick = Random.Range(0.05f, 0.2f);
        for (int index = 0; index < this.m_civvieSosigs.Count; ++index)
        {
          if (this.m_civvieSosigs != null && (Object) this.m_civvieSosigs[index].Links[0] != (Object) null)
            this.m_civvieSosigs[index].Links[0].R.AddForceAtPosition(Random.onUnitSphere * Random.Range(0.5f, 1.6f), this.m_civvieSosigs[index].Links[0].transform.position + Vector3.up * 0.3f, ForceMode.Impulse);
        }
      }
      this.Cover.localPosition = new Vector3(0.0f, this.m_openLerp, 0.0f);
    }

    private void SpawnEnemy(ZosigEnemyTemplate t, Transform point, int IFF)
    {
      GameObject weaponPrefab = (GameObject) null;
      if (t.WeaponOptions.Count > 0)
        weaponPrefab = t.WeaponOptions[Random.Range(0, t.WeaponOptions.Count)].GetGameObject();
      GameObject weaponPrefab2 = (GameObject) null;
      if (t.WeaponOptions_Secondary.Count > 0)
        weaponPrefab2 = t.WeaponOptions_Secondary[Random.Range(0, t.WeaponOptions_Secondary.Count)].GetGameObject();
      this.m_civvieSosigs.Add(this.SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)], weaponPrefab, weaponPrefab2, point.position, point.rotation, t.ConfigTemplates[Random.Range(0, t.ConfigTemplates.Count)], t.WearableTemplates[Random.Range(0, t.WearableTemplates.Count)], IFF));
    }

    private Sosig SpawnEnemySosig(
      GameObject prefab,
      GameObject weaponPrefab,
      GameObject weaponPrefab2,
      Vector3 pos,
      Quaternion rot,
      SosigConfigTemplate t,
      SosigWearableConfig w,
      int IFF)
    {
      Sosig componentInChildren = Object.Instantiate<GameObject>(prefab, pos, rot).GetComponentInChildren<Sosig>();
      componentInChildren.Configure(t);
      componentInChildren.E.IFFCode = IFF;
      if ((Object) weaponPrefab != (Object) null)
      {
        SosigWeapon component1 = Object.Instantiate<GameObject>(weaponPrefab, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
        component1.SetAutoDestroy(true);
        if (component1.Type == SosigWeapon.SosigWeaponType.Gun)
        {
          component1.FlightVelocityMultiplier = 0.15f;
          componentInChildren.Inventory.FillAmmoWithType(component1.AmmoType);
        }
        componentInChildren.Inventory.FillAllAmmo();
        if ((Object) component1 != (Object) null)
        {
          componentInChildren.InitHands();
          componentInChildren.ForceEquip(component1);
        }
        if ((Object) weaponPrefab2 != (Object) null)
        {
          SosigWeapon component2 = Object.Instantiate<GameObject>(weaponPrefab2, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
          component2.SetAutoDestroy(true);
          if (component2.Type == SosigWeapon.SosigWeaponType.Gun)
          {
            component2.FlightVelocityMultiplier = 0.15f;
            componentInChildren.Inventory.FillAmmoWithType(component2.AmmoType);
          }
          if ((Object) component2 != (Object) null)
            componentInChildren.ForceEquip(component2);
        }
      }
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
      if (t.UsesLinkSpawns)
      {
        for (int index = 0; index < componentInChildren.Links.Count; ++index)
        {
          if ((double) Random.Range(0.0f, 1f) < (double) t.LinkSpawnChance[index])
            componentInChildren.Links[index].RegisterSpawnOnDestroy(t.LinkSpawns[index]);
        }
      }
      componentInChildren.CurrentOrder = Sosig.SosigOrder.Disabled;
      componentInChildren.FallbackOrder = Sosig.SosigOrder.Disabled;
      componentInChildren.SetDominantGuardDirection(this.transform.forward);
      return componentInChildren;
    }

    private void SpawnAccesoryToLink(List<GameObject> gs, SosigLink l)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(gs[Random.Range(0, gs.Count)], l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }
  }
}
