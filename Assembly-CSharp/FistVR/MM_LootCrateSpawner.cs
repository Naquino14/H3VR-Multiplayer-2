// Decompiled with JetBrains decompiler
// Type: FistVR.MM_LootCrateSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class MM_LootCrateSpawner : MonoBehaviour
  {
    public GameObject Prefab_GunCrateSmall;
    public GameObject Prefab_Buy_GunCrateLarge;
    public GameObject Prefab_LootBox;
    public Transform SpawnPoint;
    public AudioSource Aud_Buy;
    public GameObject FXOnSpawnPrefab;
    public TAH_LootTable[] LT_SmallGunCases;
    public TAH_LootTable[] LT_LargeGunCases;
    public TAH_LootTable[] LT_Attachments;
    public Text SAUCEREADOUT;
    private float tick = 0.1f;

    public void Update()
    {
      this.tick -= Time.deltaTime;
      if ((double) this.tick > 0.0)
        return;
      this.tick = Random.Range(0.1f, 0.25f);
      this.SAUCEREADOUT.text = "You Have -- " + GM.Omni.OmniUnlocks.SaucePackets.ToString() + " -- S.A.U.C.E.";
    }

    public void Buy_GunCrateSmall()
    {
      if (this.Aud_Buy.isPlaying || !GM.Omni.OmniUnlocks.HasCurrencyForPurchase(100))
        return;
      this.PlaySound();
      GM.Omni.OmniUnlocks.SpendCurrency(100);
      Object.Instantiate<GameObject>(this.FXOnSpawnPrefab, this.SpawnPoint.position, this.SpawnPoint.rotation);
      this.SpawnIntoCase(true, Object.Instantiate<GameObject>(this.Prefab_GunCrateSmall, this.SpawnPoint.position, Random.rotation).GetComponent<MM_GunCase>());
      GM.Omni.SaveUnlocksToFile();
    }

    public void Buy_GunCrateLarge()
    {
      if (this.Aud_Buy.isPlaying || !GM.Omni.OmniUnlocks.HasCurrencyForPurchase(250))
        return;
      this.PlaySound();
      GM.Omni.OmniUnlocks.SpendCurrency(250);
      Object.Instantiate<GameObject>(this.FXOnSpawnPrefab, this.SpawnPoint.position, this.SpawnPoint.rotation);
      this.SpawnIntoCase(false, Object.Instantiate<GameObject>(this.Prefab_Buy_GunCrateLarge, this.SpawnPoint.position, Random.rotation).GetComponent<MM_GunCase>());
      GM.Omni.SaveUnlocksToFile();
    }

    public void Buy_LootBox()
    {
      if (this.Aud_Buy.isPlaying || !GM.Omni.OmniUnlocks.HasCurrencyForPurchase(1000))
        return;
      this.PlaySound();
      GM.Omni.OmniUnlocks.SpendCurrency(1000);
      Object.Instantiate<GameObject>(this.FXOnSpawnPrefab, this.SpawnPoint.position, this.SpawnPoint.rotation);
      Object.Instantiate<GameObject>(this.Prefab_LootBox, this.SpawnPoint.position, this.SpawnPoint.rotation);
      GM.Omni.SaveUnlocksToFile();
    }

    private void PlaySound()
    {
      if (this.Aud_Buy.isPlaying)
        return;
      this.Aud_Buy.pitch += 0.1f;
      if ((double) this.Aud_Buy.pitch > 1.5)
        this.Aud_Buy.pitch = 0.8f;
      this.Aud_Buy.Play();
    }

    private void SpawnIntoCase(bool isSmall, MM_GunCase c)
    {
      TAH_LootTableEntry tahLootTableEntry = !isSmall ? this.LT_LargeGunCases[Random.Range(0, this.LT_LargeGunCases.Length)].GetWeightedRandomEntry() : this.LT_SmallGunCases[Random.Range(0, this.LT_SmallGunCases.Length)].GetWeightedRandomEntry();
      int attachmentSpawn = tahLootTableEntry.AttachmentSpawn;
      GameObject go_mag = (GameObject) null;
      GameObject go_attach1 = (GameObject) null;
      GameObject go_attach2 = (GameObject) null;
      GameObject go_attach3 = (GameObject) null;
      GameObject gameObject = tahLootTableEntry.MainObj.GetGameObject();
      if ((Object) tahLootTableEntry.SecondaryObj != (Object) null)
        go_mag = tahLootTableEntry.SecondaryObj.GetGameObject();
      if (attachmentSpawn == 3)
        go_attach1 = this.LT_Attachments[3].GetWeightedRandomEntry().MainObj.GetGameObject();
      else if (attachmentSpawn > 0)
      {
        TAH_LootTableEntry weightedRandomEntry = this.LT_Attachments[attachmentSpawn].GetWeightedRandomEntry();
        go_attach2 = weightedRandomEntry.MainObj.GetGameObject();
        if ((Object) weightedRandomEntry.SecondaryObj != (Object) null)
          go_attach3 = weightedRandomEntry.SecondaryObj.GetGameObject();
      }
      c.PlaceItemsInCrate(gameObject, go_mag, go_attach1, go_attach2, go_attach3);
    }
  }
}
