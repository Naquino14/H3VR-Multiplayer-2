// Decompiled with JetBrains decompiler
// Type: FistVR.WW_Bunker
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class WW_Bunker : MonoBehaviour
  {
    public Transform TeleportPoint;
    public List<GameObject> TeleportButtons;
    public List<GameObject> TeleportTowerButtons;
    private WW_TeleportMaster m_master;
    public int BunkerIndex;
    public int TierNeeded;
    public GameObject TPPanel;
    public List<GameObject> Numbers_Outer;
    public List<GameObject> Numbers_Inner;
    public List<GameObject> TierLocks;
    public GameObject LockedUntil;
    public GameObject Door;
    public Transform Door_Up;
    public Transform Door_Down;
    private float m_doorTick;
    private bool m_isUnlocked;
    private bool m_isLockDown = true;
    public List<GameObject> TurnOffIfLockDown;
    public List<GameObject> TurnOnIfLockDown;
    public Transform UnlockCheckVolume;
    [Header("Probes")]
    public List<GameObject> ReflectionProbes;
    public WW_Bunker CopyFrom;
    public Transform ProbeCheckV;
    public Transform BunkerBounds;
    private bool m_isProbeEnabled;
    public TNH_WeaponCrate Crate;
    public GameObject Lever;
    public Transform Forcefield;
    public Transform FF_Up;
    public Transform FF_Down;
    public GameObject XmasFetti;
    public List<Transform> XmasFettiPoints;
    public AudioEvent AudEvent_Music;
    public AudioEvent AudEvent_DoorOpen;
    private float fieldTick;
    private bool m_isFieldOpened;
    private bool m_isFieldOpening;
    private float m_checkTick = 1f;
    private bool m_isDoorOpening;

    public bool IsUnlocked => this.m_isUnlocked;

    public bool IsLockDown => this.m_isLockDown;

    [ContextMenu("ProbeCopy")]
    public void ProbeCopy()
    {
      for (int index = 0; index < this.ReflectionProbes.Count; ++index)
      {
        ReflectionProbe component1 = this.ReflectionProbes[index].GetComponent<ReflectionProbe>();
        ReflectionProbe component2 = this.CopyFrom.ReflectionProbes[index].GetComponent<ReflectionProbe>();
        component1.size = component2.size;
        component1.center = component2.center;
        component1.customBakedTexture = component2.customBakedTexture;
      }
      this.CopyFrom = (WW_Bunker) null;
    }

    public void ConfigInitBunker(int index, int curDay, int tierNeeded)
    {
      this.BunkerIndex = index;
      this.TierNeeded = tierNeeded;
      if (this.BunkerIndex <= curDay)
      {
        for (int index1 = 0; index1 < this.TierLocks.Count; ++index1)
        {
          if (index1 == this.TierNeeded)
            this.TierLocks[index1].SetActive(true);
          else
            this.TierLocks[index1].SetActive(false);
        }
        this.LockedUntil.SetActive(false);
        this.m_isLockDown = false;
      }
      else
      {
        for (int index1 = 0; index1 < this.TierLocks.Count; ++index1)
          this.TierLocks[index1].SetActive(false);
        this.LockedUntil.SetActive(true);
        this.m_isLockDown = true;
        for (int index1 = 0; index1 < this.TurnOffIfLockDown.Count; ++index1)
          this.TurnOffIfLockDown[index1].SetActive(false);
        for (int index1 = 0; index1 < this.TurnOnIfLockDown.Count; ++index1)
          this.TurnOnIfLockDown[index1].SetActive(true);
      }
      this.UpdateTPButtons();
      if (GM.Options.XmasFlags.BunkersOpened[this.BunkerIndex])
      {
        this.Door.SetActive(false);
        this.m_isUnlocked = true;
      }
      for (int index1 = 0; index1 < this.Numbers_Outer.Count; ++index1)
      {
        if (index1 == this.BunkerIndex)
        {
          this.Numbers_Outer[index1].SetActive(true);
          this.Numbers_Inner[index1].SetActive(true);
        }
        else
        {
          this.Numbers_Outer[index1].SetActive(false);
          this.Numbers_Inner[index1].SetActive(false);
        }
      }
      this.Forcefield.SetParent((Transform) null);
      if (GM.Options.XmasFlags.FieldsOpened[this.BunkerIndex])
      {
        this.Forcefield.gameObject.SetActive(false);
        this.m_isFieldOpened = true;
        this.Lever.SetActive(false);
      }
      this.Door.transform.SetParent((Transform) null);
    }

    public void SetMaster(WW_TeleportMaster m) => this.m_master = m;

    private void Start()
    {
    }

    public void TeleportTo(int i) => this.m_master.TeleportTo(i);

    public void TeleportToTowerPad(int i) => this.m_master.TeleportToTowerPad(i);

    private void Update()
    {
      this.KeyCheck();
      if (this.m_isFieldOpening)
      {
        this.fieldTick += Time.deltaTime * 0.1f;
        if ((double) this.fieldTick >= 1.0)
          this.m_isFieldOpening = false;
        this.Forcefield.position = Vector3.Lerp(this.FF_Up.position, this.FF_Down.position, this.fieldTick);
      }
      if (!this.m_isDoorOpening)
        return;
      this.m_doorTick += Time.deltaTime * 1f;
      if ((double) this.m_doorTick >= 1.0)
        this.m_isDoorOpening = false;
      this.Door.transform.position = Vector3.Lerp(this.Door_Up.position, this.Door_Down.position, this.m_doorTick);
    }

    public void UpdateTPButtons()
    {
      for (int index = 0; index < this.TeleportButtons.Count; ++index)
        this.TeleportButtons[index].SetActive(GM.Options.XmasFlags.BunkersOpened[index]);
      for (int index = 0; index < this.TeleportTowerButtons.Count; ++index)
        this.TeleportTowerButtons[index].SetActive(GM.Options.XmasFlags.TowersActive[index + 1]);
    }

    private void Unlock()
    {
      GM.Options.XmasFlags.BunkersOpened[this.BunkerIndex] = true;
      GM.Options.SaveToFile();
      this.m_master.BunkerUnlockedUpdate();
    }

    public void OpenField()
    {
      if (this.m_isFieldOpened)
        return;
      this.m_isFieldOpened = true;
      GM.Options.XmasFlags.FieldsOpened[this.BunkerIndex] = true;
      GM.Options.SaveToFile();
      for (int index = 0; index < this.XmasFettiPoints.Count; ++index)
        Object.Instantiate<GameObject>(this.XmasFetti, this.XmasFettiPoints[index].position, this.XmasFettiPoints[index].rotation);
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Music, this.Forcefield.position);
      this.m_isFieldOpening = true;
    }

    public void Entered() => this.m_master.EnteredBunker(this.BunkerIndex);

    public void KeyCheck()
    {
      if (this.m_isLockDown)
        return;
      this.m_checkTick -= Time.deltaTime;
      if ((double) this.m_checkTick >= 0.0)
        return;
      this.m_checkTick = Random.Range(0.3f, 0.5f);
      if (this.TestVolumeBool(this.ProbeCheckV, GM.CurrentPlayerBody.Head.position))
      {
        if (!this.m_isProbeEnabled)
        {
          this.m_isProbeEnabled = true;
          for (int index = 0; index < this.ReflectionProbes.Count; ++index)
          {
            this.ReflectionProbes[index].SetActive(true);
            this.TPPanel.SetActive(true);
            this.UpdateTPButtons();
          }
        }
      }
      else if (this.m_isProbeEnabled)
      {
        this.m_isProbeEnabled = false;
        for (int index = 0; index < this.ReflectionProbes.Count; ++index)
        {
          this.ReflectionProbes[index].SetActive(false);
          this.TPPanel.SetActive(false);
        }
      }
      if (this.m_isUnlocked || !this.TestVolumeBool(this.UnlockCheckVolume, GM.CurrentPlayerBody.Head.position))
        return;
      for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
      {
        if ((Object) GM.CurrentMovementManager.Hands[index].CurrentInteractable != (Object) null && GM.CurrentMovementManager.Hands[index].CurrentInteractable is WW_Keycard && (GM.CurrentMovementManager.Hands[index].CurrentInteractable as WW_Keycard).TierType == this.TierNeeded)
          this.UnlockDoor();
      }
    }

    private void UnlockDoor()
    {
      if (this.m_isUnlocked)
        return;
      this.m_isUnlocked = true;
      GM.Options.XmasFlags.BunkersOpened[this.BunkerIndex] = true;
      GM.Options.SaveToFile();
      this.m_master.BunkerUnlockedUpdate();
      this.m_isDoorOpening = true;
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_DoorOpen, this.Door.transform.position);
    }

    public bool TestVolumeBool(Transform t, Vector3 pos)
    {
      bool flag = true;
      Vector3 vector3 = t.InverseTransformPoint(pos);
      if ((double) Mathf.Abs(vector3.x) > 0.5 || (double) Mathf.Abs(vector3.y) > 0.5 || (double) Mathf.Abs(vector3.z) > 0.5)
        flag = false;
      return flag;
    }
  }
}
