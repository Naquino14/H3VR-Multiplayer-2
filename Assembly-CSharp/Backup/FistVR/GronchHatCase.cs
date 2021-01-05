// Decompiled with JetBrains decompiler
// Type: FistVR.GronchHatCase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class GronchHatCase : FVRPhysicalObject
  {
    public Transform Lid;
    public Transform KeyTarget;
    public Transform SpawnPoint;
    public Transform FXPoint;
    public string HID;
    private bool m_isOpen;
    public Vector2 CaseLidRots = new Vector2(0.0f, 0.0f);
    public GameObject SpawnOnUnlock;
    public GameObject SpawnOnAlreadyHave;

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.m_isOpen || !this.IsHeld || (!((Object) this.m_hand.OtherHand.CurrentInteractable != (Object) null) || !(this.m_hand.OtherHand.CurrentInteractable is GronchHatCaseKey)))
        return;
      GronchHatCaseKey currentInteractable = this.m_hand.OtherHand.CurrentInteractable as GronchHatCaseKey;
      bool flag1 = true;
      bool flag2;
      if ((double) Vector3.Distance(currentInteractable.transform.position, this.KeyTarget.position) > 0.0199999995529652)
        flag2 = false;
      else if ((double) Vector3.Angle(currentInteractable.transform.up, this.KeyTarget.up) > 10.0)
        flag2 = false;
      else if ((double) Vector3.Angle(currentInteractable.transform.forward, this.KeyTarget.forward) > 10.0)
      {
        flag2 = false;
      }
      else
      {
        if (!flag1)
          return;
        this.Open(currentInteractable);
      }
    }

    private void Open(GronchHatCaseKey k)
    {
      if (this.m_isOpen)
        return;
      this.m_isOpen = true;
      this.Lid.localEulerAngles = new Vector3(this.CaseLidRots.y, 0.0f, 0.0f);
      GM.MMFlags.AddHat(this.HID);
      GM.MMFlags.SaveToFile();
      if (GM.Rewards.RewardUnlocks.IsRewardUnlocked(this.HID))
      {
        Object.Instantiate<GameObject>(this.SpawnOnAlreadyHave, this.FXPoint.position, this.FXPoint.rotation);
      }
      else
      {
        GM.Rewards.RewardUnlocks.UnlockReward(this.HID);
        Object.Instantiate<GameObject>(this.SpawnOnAlreadyHave, this.FXPoint.position, this.FXPoint.rotation);
      }
      GM.Rewards.SaveToFile();
      if (IM.HasSpawnedID(this.HID))
        Object.Instantiate<GameObject>(IM.GetSpawnerID(this.HID).MainObject.GetGameObject(), this.SpawnPoint.position, this.SpawnPoint.rotation);
      SMEME objectOfType = Object.FindObjectOfType<SMEME>();
      objectOfType.UpdateInventory();
      objectOfType.DrawInventory();
      Object.Destroy((Object) k.gameObject);
    }
  }
}
