// Decompiled with JetBrains decompiler
// Type: FistVR.FVRWristMenu
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FistVR
{
  public class FVRWristMenu : MonoBehaviour
  {
    public Transform Face;
    public FVRViveHand[] Hands;
    private bool m_hasHands;
    private FVRViveHand m_currentHand;
    private bool m_isActive;
    private bool m_isButtonSelected;
    private float m_wristPointRange = 60f;
    private float m_faceAngleRange = 45f;
    public GameObject MenuGO;
    public OptionsPanel_ButtonSet OBS_Wrist;
    public List<Button> Buttons;
    private int m_curButton;
    public GameObject OptionsPanelPrefab;
    public GameObject SpectatorPanelPrefab;
    public AudioSource Aud;
    public AudioClip AudClip_Engage;
    public AudioClip AudClip_Nav;
    public AudioClip AudClip_Err;
    public Text LocomotionModeDisplay;
    public RectTransform SelectionBorder;
    [Header("SelectionLaser")]
    public Transform Laser;
    public Text Clock;
    [Header("ConfirmCrapOMGthisscriptisSOold,thisis like.... super embarrasing yo")]
    public Text TXT_ReloadScene;
    public Text TXT_ReturnMainmenu;
    public Text TXT_QuitToDesktop;
    public Text TXT_CleanupEmpties;
    public Text TXT_CleanupAllMags;
    public Text TXT_CleanupGuns;
    public Text TXT_HeldObject;
    private float m_heldTime;
    private bool m_hasCommandFired;
    private bool askConfirm_Reload;
    private bool askConfirm_ReturnMainmenu;
    private bool askConfirm_Quit;
    private bool askConfirm_CleanupEmpties;
    private bool askConfirm_CleanupAllMags;
    private bool askConfirm_CleanupGuns;

    public void Awake()
    {
      this.MenuGO.SetActive(false);
      this.OBS_Wrist.SetSelectedButton(0);
      this.LocomotionModeDisplay.text = "Locomotion Mode\n<<" + GM.Options.MovementOptions.CurrentMovementMode.ToString() + ">>";
    }

    public void SetHandsAndFace(FVRViveHand Hand0, FVRViveHand Hand1, Transform face)
    {
      this.Hands[0] = Hand0;
      this.Hands[1] = Hand1;
      this.Face = face;
      this.m_hasHands = true;
    }

    public void Update()
    {
      this.UpdateWristMenu();
      this.PositionWristMenu();
      if (!this.m_isActive)
        return;
      this.Clock.text = DateTime.Now.ToString("H:mm:ss");
    }

    private void UpdateWristMenu()
    {
      if (!this.m_hasHands)
        return;
      if (this.m_isActive)
      {
        if ((UnityEngine.Object) this.m_currentHand.CurrentInteractable != (UnityEngine.Object) null || (double) Vector3.Angle(this.m_currentHand.GetWristMenuTarget().forward, -this.Face.forward) >= (double) this.m_wristPointRange)
        {
          this.Deactivate();
          return;
        }
        if ((UnityEngine.Object) this.m_currentHand != (UnityEngine.Object) null && (double) Vector3.Angle(this.Face.forward, this.m_currentHand.GetWristMenuTarget().position - this.Face.position) >= (double) this.m_faceAngleRange)
        {
          this.Deactivate();
          return;
        }
      }
      if (!this.m_isActive)
      {
        for (int index = 0; index < this.Hands.Length; ++index)
        {
          if ((UnityEngine.Object) this.Hands[index].CurrentInteractable == (UnityEngine.Object) null && (double) Vector3.Angle(this.Hands[index].GetWristMenuTarget().forward, -this.Face.forward) < (double) this.m_wristPointRange)
          {
            Vector3 to = this.Hands[index].GetWristMenuTarget().position - this.Face.position;
            if ((!this.Hands[index].IsThisTheRightHand || GM.Options.ControlOptions.WristMenuState != ControlOptions.WristMenuMode.LeftHand) && (this.Hands[index].IsThisTheRightHand || GM.Options.ControlOptions.WristMenuState != ControlOptions.WristMenuMode.RightHand) && (double) Vector3.Angle(this.Face.forward, to) < (double) this.m_faceAngleRange)
              this.ActivateOnHand(this.Hands[index]);
          }
        }
      }
      if (!this.m_isActive)
        return;
      string str = string.Empty;
      FVRViveHand otherHand = this.m_currentHand.OtherHand;
      if (otherHand.CurrentInteractable is FVRPhysicalObject)
      {
        FVRPhysicalObject currentInteractable = otherHand.CurrentInteractable as FVRPhysicalObject;
        if ((UnityEngine.Object) currentInteractable.ObjectWrapper != (UnityEngine.Object) null)
          str = !IM.HasSpawnedID(currentInteractable.ObjectWrapper.ItemID) ? currentInteractable.ObjectWrapper.DisplayName : IM.GetSpawnerID(currentInteractable.ObjectWrapper.ItemID).DisplayName;
      }
      this.TXT_HeldObject.text = str;
    }

    private void PositionWristMenu()
    {
      if (!this.m_isActive || !((UnityEngine.Object) this.m_currentHand != (UnityEngine.Object) null))
        return;
      this.transform.position = this.m_currentHand.GetWristMenuTarget().position;
      this.transform.rotation = this.m_currentHand.GetWristMenuTarget().rotation;
    }

    public void UpdateWristMenuSelection(FVRViveHand hand)
    {
      this.m_isButtonSelected = false;
      if (!this.m_isActive || (UnityEngine.Object) hand != (UnityEngine.Object) this.m_currentHand)
        return;
      if ((UnityEngine.Object) hand.OtherHand.CurrentInteractable != (UnityEngine.Object) null)
      {
        if (this.SelectionBorder.gameObject.activeSelf)
          this.SelectionBorder.gameObject.SetActive(false);
        if (!this.Laser.gameObject.activeSelf)
          return;
        this.Laser.gameObject.SetActive(false);
      }
      else
      {
        Vector3 vector3 = this.transform.position - hand.OtherHand.PointingTransform.position;
        if ((double) Vector3.Dot(vector3.normalized, hand.OtherHand.PointingTransform.forward) < 0.5 || (double) vector3.magnitude > 0.400000005960464)
        {
          if (this.SelectionBorder.gameObject.activeSelf)
            this.SelectionBorder.gameObject.SetActive(false);
          if (!this.Laser.gameObject.activeSelf)
            return;
          this.Laser.gameObject.SetActive(false);
        }
        else
        {
          this.LocomotionModeDisplay.text = "Locomotion Mode\n<<" + GM.CurrentMovementManager.Mode.ToString() + ">>";
          for (int index = 0; index < this.Buttons.Count; ++index)
          {
            Ray ray = new Ray(hand.OtherHand.PointingTransform.position, hand.OtherHand.PointingTransform.forward);
            RaycastHit hitInfo;
            if (this.Buttons[index].GetComponent<Collider>().Raycast(ray, out hitInfo, 0.4f))
            {
              this.m_isButtonSelected = true;
              this.m_curButton = index;
              this.OBS_Wrist.SetSelectedButton(this.m_curButton);
              if (!this.SelectionBorder.gameObject.activeSelf)
                this.SelectionBorder.gameObject.SetActive(true);
              this.SelectionBorder.anchoredPosition = (this.Buttons[this.m_curButton].transform as RectTransform).anchoredPosition;
              this.SelectionBorder.sizeDelta = (this.Buttons[this.m_curButton].transform as RectTransform).sizeDelta;
              if (!this.Laser.gameObject.activeSelf)
                this.Laser.gameObject.SetActive(true);
              this.Laser.transform.position = hand.OtherHand.PointingTransform.position;
              this.Laser.transform.rotation = hand.OtherHand.PointingTransform.rotation;
              this.Laser.transform.localScale = new Vector3(1f / 500f, 1f / 500f, hitInfo.distance);
              break;
            }
          }
          if (!this.m_isButtonSelected)
          {
            if (this.SelectionBorder.gameObject.activeSelf)
              this.SelectionBorder.gameObject.SetActive(false);
            if (this.Laser.gameObject.activeSelf)
              this.Laser.gameObject.SetActive(false);
          }
          if (!hand.OtherHand.Input.TriggerDown || !this.m_isButtonSelected)
            return;
          this.Buttons[this.m_curButton].onClick.Invoke();
          this.m_currentHand.Buzz(this.m_currentHand.Buzzer.Buzz_BeginInteraction);
          this.m_currentHand.OtherHand.Buzz(this.m_currentHand.OtherHand.Buzzer.Buzz_BeginInteraction);
        }
      }
    }

    public void SetSelectedButton(int i)
    {
      this.m_curButton = i;
      this.OBS_Wrist.SetSelectedButton(this.m_curButton);
      if (!this.SelectionBorder.gameObject.activeSelf)
        this.SelectionBorder.gameObject.SetActive(true);
      this.SelectionBorder.anchoredPosition = (this.Buttons[this.m_curButton].transform as RectTransform).anchoredPosition;
      this.SelectionBorder.sizeDelta = (this.Buttons[this.m_curButton].transform as RectTransform).sizeDelta;
    }

    public void InvokeButton(int i)
    {
      this.Buttons[i].onClick.Invoke();
      this.m_currentHand.Buzz(this.m_currentHand.Buzzer.Buzz_BeginInteraction);
      this.m_currentHand.OtherHand.Buzz(this.m_currentHand.OtherHand.Buzzer.Buzz_BeginInteraction);
    }

    private void SelectionUp()
    {
      if (this.m_curButton <= 0)
        return;
      --this.m_curButton;
      this.OBS_Wrist.SetSelectedButton(this.m_curButton);
      this.m_currentHand.Buzz(this.m_currentHand.Buzzer.Buzz_BeginInteraction);
      this.Aud.PlayOneShot(this.AudClip_Nav, 1f);
    }

    private void SelectionDown()
    {
      if (this.m_curButton >= this.Buttons.Count - 1)
        return;
      ++this.m_curButton;
      this.OBS_Wrist.SetSelectedButton(this.m_curButton);
      this.m_currentHand.Buzz(this.m_currentHand.Buzzer.Buzz_BeginInteraction);
      this.Aud.PlayOneShot(this.AudClip_Nav, 1f);
    }

    private void ActivateOnHand(FVRViveHand hand)
    {
      if (this.m_isActive)
        return;
      this.m_isActive = true;
      this.m_currentHand = hand;
      this.m_currentHand.EnableWristMenu(this);
      this.MenuGO.SetActive(true);
      this.ResetConfirm();
    }

    private void Deactivate()
    {
      if (!this.m_isActive)
        return;
      this.m_isActive = false;
      this.m_currentHand.DisableWristMenu();
      this.m_currentHand = (FVRViveHand) null;
      this.MenuGO.SetActive(false);
      if (this.Laser.gameObject.activeSelf)
        this.Laser.gameObject.SetActive(false);
      this.ResetConfirm();
    }

    public void SpawnOptionsPanel()
    {
      if ((UnityEngine.Object) GM.CurrentOptionsPanel != (UnityEngine.Object) null)
      {
        this.m_currentHand.RetrieveObject(GM.CurrentOptionsPanel.GetComponent<FVRPhysicalObject>());
      }
      else
      {
        GM.CurrentOptionsPanel = UnityEngine.Object.Instantiate<GameObject>(this.OptionsPanelPrefab, Vector3.zero, Quaternion.identity);
        this.m_currentHand.RetrieveObject(GM.CurrentOptionsPanel.GetComponent<FVRPhysicalObject>());
      }
      this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
    }

    public void SpawnSpectatorPanel()
    {
      if ((UnityEngine.Object) GM.CurrentSpectatorPanel != (UnityEngine.Object) null)
      {
        this.m_currentHand.RetrieveObject(GM.CurrentSpectatorPanel.GetComponent<FVRPhysicalObject>());
      }
      else
      {
        GM.CurrentSpectatorPanel = UnityEngine.Object.Instantiate<GameObject>(this.SpectatorPanelPrefab, Vector3.zero, Quaternion.identity);
        this.m_currentHand.RetrieveObject(GM.CurrentSpectatorPanel.GetComponent<FVRPhysicalObject>());
      }
      this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
    }

    public void CleanUpScene_Empties()
    {
      this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
      if (!this.askConfirm_CleanupEmpties)
      {
        this.ResetConfirm();
        this.AskConfirm_CleanupEmpties();
      }
      else
      {
        this.ResetConfirm();
        FVRFireArmMagazine[] objectsOfType1 = UnityEngine.Object.FindObjectsOfType<FVRFireArmMagazine>();
        for (int index = objectsOfType1.Length - 1; index >= 0; --index)
        {
          if (!objectsOfType1[index].IsHeld && (UnityEngine.Object) objectsOfType1[index].QuickbeltSlot == (UnityEngine.Object) null && ((UnityEngine.Object) objectsOfType1[index].FireArm == (UnityEngine.Object) null && objectsOfType1[index].m_numRounds == 0))
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType1[index].gameObject);
        }
        FVRFireArmRound[] objectsOfType2 = UnityEngine.Object.FindObjectsOfType<FVRFireArmRound>();
        for (int index = objectsOfType2.Length - 1; index >= 0; --index)
        {
          if (!objectsOfType2[index].IsHeld && (UnityEngine.Object) objectsOfType2[index].QuickbeltSlot == (UnityEngine.Object) null && (UnityEngine.Object) objectsOfType2[index].RootRigidbody != (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType2[index].gameObject);
        }
        FVRFireArmClip[] objectsOfType3 = UnityEngine.Object.FindObjectsOfType<FVRFireArmClip>();
        for (int index = objectsOfType3.Length - 1; index >= 0; --index)
        {
          if (!objectsOfType3[index].IsHeld && (UnityEngine.Object) objectsOfType3[index].QuickbeltSlot == (UnityEngine.Object) null && ((UnityEngine.Object) objectsOfType3[index].FireArm == (UnityEngine.Object) null && objectsOfType3[index].m_numRounds == 0))
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType3[index].gameObject);
        }
        Speedloader[] objectsOfType4 = UnityEngine.Object.FindObjectsOfType<Speedloader>();
        for (int index = objectsOfType4.Length - 1; index >= 0; --index)
        {
          if (!objectsOfType4[index].IsHeld && (UnityEngine.Object) objectsOfType4[index].QuickbeltSlot == (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType4[index].gameObject);
        }
      }
    }

    public void CleanUpScene_AllMags()
    {
      this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
      if (!this.askConfirm_CleanupAllMags)
      {
        this.ResetConfirm();
        this.AskConfirm_CleanupAllMags();
      }
      else
      {
        this.ResetConfirm();
        FVRFireArmMagazine[] objectsOfType1 = UnityEngine.Object.FindObjectsOfType<FVRFireArmMagazine>();
        for (int index = objectsOfType1.Length - 1; index >= 0; --index)
        {
          if (!objectsOfType1[index].IsHeld && (UnityEngine.Object) objectsOfType1[index].QuickbeltSlot == (UnityEngine.Object) null && (UnityEngine.Object) objectsOfType1[index].FireArm == (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType1[index].gameObject);
        }
        FVRFireArmRound[] objectsOfType2 = UnityEngine.Object.FindObjectsOfType<FVRFireArmRound>();
        for (int index = objectsOfType2.Length - 1; index >= 0; --index)
        {
          if (!objectsOfType2[index].IsHeld && (UnityEngine.Object) objectsOfType2[index].QuickbeltSlot == (UnityEngine.Object) null && (UnityEngine.Object) objectsOfType2[index].RootRigidbody != (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType2[index].gameObject);
        }
        FVRFireArmClip[] objectsOfType3 = UnityEngine.Object.FindObjectsOfType<FVRFireArmClip>();
        for (int index = objectsOfType3.Length - 1; index >= 0; --index)
        {
          if (!objectsOfType3[index].IsHeld && (UnityEngine.Object) objectsOfType3[index].QuickbeltSlot == (UnityEngine.Object) null && (UnityEngine.Object) objectsOfType3[index].FireArm == (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType3[index].gameObject);
        }
        Speedloader[] objectsOfType4 = UnityEngine.Object.FindObjectsOfType<Speedloader>();
        for (int index = objectsOfType4.Length - 1; index >= 0; --index)
        {
          if (!objectsOfType4[index].IsHeld && (UnityEngine.Object) objectsOfType4[index].QuickbeltSlot == (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType4[index].gameObject);
        }
      }
    }

    public void CleanUpScene_Guns()
    {
      this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
      if (!this.askConfirm_CleanupGuns)
      {
        this.ResetConfirm();
        this.AskConfirm_CleanupGuns();
      }
      else
      {
        this.ResetConfirm();
        FVRFireArm[] objectsOfType1 = UnityEngine.Object.FindObjectsOfType<FVRFireArm>();
        for (int index = objectsOfType1.Length - 1; index >= 0; --index)
        {
          if (!objectsOfType1[index].IsHeld && (UnityEngine.Object) objectsOfType1[index].QuickbeltSlot == (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType1[index].gameObject);
        }
        SosigWeapon[] objectsOfType2 = UnityEngine.Object.FindObjectsOfType<SosigWeapon>();
        for (int index = objectsOfType2.Length - 1; index >= 0; --index)
        {
          if (!objectsOfType2[index].O.IsHeld && (UnityEngine.Object) objectsOfType2[index].O.QuickbeltSlot == (UnityEngine.Object) null && (!objectsOfType2[index].IsHeldByBot && !objectsOfType2[index].IsInBotInventory))
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType2[index].gameObject);
        }
        FVRMeleeWeapon[] objectsOfType3 = UnityEngine.Object.FindObjectsOfType<FVRMeleeWeapon>();
        for (int index = objectsOfType3.Length - 1; index >= 0; --index)
        {
          if (!objectsOfType3[index].IsHeld && (UnityEngine.Object) objectsOfType3[index].QuickbeltSlot == (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType3[index].gameObject);
        }
      }
    }

    public void ReloadScene()
    {
      this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
      if (!this.askConfirm_Reload)
      {
        this.ResetConfirm();
        this.AskConfirm_Reload();
      }
      else
      {
        this.ResetConfirm();
        for (int index = 0; index < GM.CurrentSceneSettings.QuitReceivers.Count; ++index)
          GM.CurrentSceneSettings.QuitReceivers[index].BroadcastMessage("QUIT", SendMessageOptions.DontRequireReceiver);
        if (!GM.LoadingCallback.IsCompleted)
          return;
        SteamVR_LoadLevel.Begin(SceneManager.GetActiveScene().name);
      }
    }

    public void ReturnToMainMenu()
    {
      this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
      if (!this.askConfirm_ReturnMainmenu)
      {
        this.ResetConfirm();
        this.AskConfirm_ReturnMainmenu();
      }
      else
      {
        this.ResetConfirm();
        for (int index = 0; index < GM.CurrentSceneSettings.QuitReceivers.Count; ++index)
          GM.CurrentSceneSettings.QuitReceivers[index].BroadcastMessage("QUIT", SendMessageOptions.DontRequireReceiver);
        if (!GM.LoadingCallback.IsCompleted)
          return;
        SteamVR_LoadLevel.Begin("MainMenu3");
      }
    }

    public void QuitToDesktop()
    {
      this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
      if (!this.askConfirm_Quit)
      {
        this.ResetConfirm();
        this.AskConfirm_Quit();
      }
      else
      {
        this.ResetConfirm();
        for (int index = 0; index < GM.CurrentSceneSettings.QuitReceivers.Count; ++index)
          GM.CurrentSceneSettings.QuitReceivers[index].BroadcastMessage("QUIT", SendMessageOptions.DontRequireReceiver);
        Application.Quit();
      }
    }

    private void ResetConfirm()
    {
      this.askConfirm_Reload = false;
      this.askConfirm_ReturnMainmenu = false;
      this.askConfirm_Quit = false;
      this.askConfirm_CleanupEmpties = false;
      this.askConfirm_CleanupAllMags = false;
      this.askConfirm_CleanupGuns = false;
      this.TXT_ReloadScene.text = "Reload Scene";
      this.TXT_ReturnMainmenu.text = "Return to Main Menu";
      this.TXT_QuitToDesktop.text = "Quit To Desktop";
      this.TXT_CleanupEmpties.text = "Clean Empty Mags";
      this.TXT_CleanupAllMags.text = "Clean All Mags";
      this.TXT_CleanupGuns.text = "Clean Guns & Melee";
    }

    private void AskConfirm_CleanupEmpties()
    {
      this.askConfirm_CleanupEmpties = true;
      this.TXT_CleanupEmpties.text = "Confirm ???";
    }

    private void AskConfirm_CleanupAllMags()
    {
      this.askConfirm_CleanupAllMags = true;
      this.TXT_CleanupAllMags.text = "Confirm ???";
    }

    private void AskConfirm_CleanupGuns()
    {
      this.askConfirm_CleanupGuns = true;
      this.TXT_CleanupGuns.text = "Confirm ???";
    }

    private void AskConfirm_Reload()
    {
      this.askConfirm_Reload = true;
      this.TXT_ReloadScene.text = "Confirm ???";
    }

    private void AskConfirm_ReturnMainmenu()
    {
      this.askConfirm_ReturnMainmenu = true;
      this.TXT_ReturnMainmenu.text = "Confirm ???";
    }

    private void AskConfirm_Quit()
    {
      this.askConfirm_Quit = true;
      this.TXT_QuitToDesktop.text = "Confirm ???";
    }

    public void TurnClockWise()
    {
      this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
      Vector3 position = GM.CurrentPlayerBody.Head.position;
      position.y = GM.CurrentPlayerBody.transform.position.y;
      Vector3 forward = GM.CurrentPlayerBody.transform.forward;
      Vector3 lookDir = Quaternion.AngleAxis(GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex], Vector3.up) * forward;
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(position, false, lookDir);
    }

    public void TurnCounterClockWise()
    {
      this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
      Vector3 position = GM.CurrentPlayerBody.Head.position;
      position.y = GM.CurrentPlayerBody.transform.position.y;
      Vector3 forward = GM.CurrentPlayerBody.transform.forward;
      Vector3 lookDir = Quaternion.AngleAxis(-GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex], Vector3.up) * forward;
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(position, false, lookDir);
    }

    public void SetLocomotionMode(int i)
    {
      if (this.Hands[0].IsInStreamlinedMode && (i == 2 || i == 3 || i == 5))
      {
        this.Aud.PlayOneShot(this.AudClip_Err, 1f);
      }
      else
      {
        this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
        GM.CurrentMovementManager.Mode = (FVRMovementManager.MovementMode) i;
        GM.CurrentMovementManager.CleanupFlagsForModeSwitch();
        GM.CurrentMovementManager.InitArmSwinger();
        GM.Options.MovementOptions.CurrentMovementMode = (FVRMovementManager.MovementMode) i;
        GM.Options.SaveToFile();
      }
    }

    public void CycleLocomotionMode()
    {
      this.Aud.PlayOneShot(this.AudClip_Engage, 1f);
      this.LocomotionModeDisplay.text = "Locomotion Mode\n<<" + GM.CurrentMovementManager.Mode.ToString() + ">>";
    }
  }
}
