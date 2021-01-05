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
			MenuGO.SetActive(value: false);
			OBS_Wrist.SetSelectedButton(0);
			LocomotionModeDisplay.text = "Locomotion Mode\n<<" + GM.Options.MovementOptions.CurrentMovementMode.ToString() + ">>";
		}

		public void SetHandsAndFace(FVRViveHand Hand0, FVRViveHand Hand1, Transform face)
		{
			Hands[0] = Hand0;
			Hands[1] = Hand1;
			Face = face;
			m_hasHands = true;
		}

		public void Update()
		{
			UpdateWristMenu();
			PositionWristMenu();
			if (m_isActive)
			{
				Clock.text = DateTime.Now.ToString("H:mm:ss");
			}
		}

		private void UpdateWristMenu()
		{
			if (!m_hasHands)
			{
				return;
			}
			if (m_isActive)
			{
				if (m_currentHand.CurrentInteractable != null || Vector3.Angle(m_currentHand.GetWristMenuTarget().forward, -Face.forward) >= m_wristPointRange)
				{
					Deactivate();
					return;
				}
				if (m_currentHand != null)
				{
					Vector3 to = m_currentHand.GetWristMenuTarget().position - Face.position;
					if (Vector3.Angle(Face.forward, to) >= m_faceAngleRange)
					{
						Deactivate();
						return;
					}
				}
			}
			if (!m_isActive)
			{
				for (int i = 0; i < Hands.Length; i++)
				{
					if (Hands[i].CurrentInteractable == null && Vector3.Angle(Hands[i].GetWristMenuTarget().forward, -Face.forward) < m_wristPointRange)
					{
						Vector3 to2 = Hands[i].GetWristMenuTarget().position - Face.position;
						if ((!Hands[i].IsThisTheRightHand || GM.Options.ControlOptions.WristMenuState != ControlOptions.WristMenuMode.LeftHand) && (Hands[i].IsThisTheRightHand || GM.Options.ControlOptions.WristMenuState != ControlOptions.WristMenuMode.RightHand) && Vector3.Angle(Face.forward, to2) < m_faceAngleRange)
						{
							ActivateOnHand(Hands[i]);
						}
					}
				}
			}
			if (!m_isActive)
			{
				return;
			}
			string text = string.Empty;
			FVRViveHand otherHand = m_currentHand.OtherHand;
			if (otherHand.CurrentInteractable is FVRPhysicalObject)
			{
				FVRPhysicalObject fVRPhysicalObject = otherHand.CurrentInteractable as FVRPhysicalObject;
				if (fVRPhysicalObject.ObjectWrapper != null)
				{
					if (IM.HasSpawnedID(fVRPhysicalObject.ObjectWrapper.ItemID))
					{
						ItemSpawnerID spawnerID = IM.GetSpawnerID(fVRPhysicalObject.ObjectWrapper.ItemID);
						text = spawnerID.DisplayName;
					}
					else
					{
						text = fVRPhysicalObject.ObjectWrapper.DisplayName;
					}
				}
			}
			TXT_HeldObject.text = text;
		}

		private void PositionWristMenu()
		{
			if (m_isActive && m_currentHand != null)
			{
				base.transform.position = m_currentHand.GetWristMenuTarget().position;
				base.transform.rotation = m_currentHand.GetWristMenuTarget().rotation;
			}
		}

		public void UpdateWristMenuSelection(FVRViveHand hand)
		{
			m_isButtonSelected = false;
			if (!m_isActive || hand != m_currentHand)
			{
				return;
			}
			if (hand.OtherHand.CurrentInteractable != null)
			{
				if (SelectionBorder.gameObject.activeSelf)
				{
					SelectionBorder.gameObject.SetActive(value: false);
				}
				if (Laser.gameObject.activeSelf)
				{
					Laser.gameObject.SetActive(value: false);
				}
				return;
			}
			Vector3 vector = base.transform.position - hand.OtherHand.PointingTransform.position;
			if (Vector3.Dot(vector.normalized, hand.OtherHand.PointingTransform.forward) < 0.5f || vector.magnitude > 0.4f)
			{
				if (SelectionBorder.gameObject.activeSelf)
				{
					SelectionBorder.gameObject.SetActive(value: false);
				}
				if (Laser.gameObject.activeSelf)
				{
					Laser.gameObject.SetActive(value: false);
				}
				return;
			}
			LocomotionModeDisplay.text = "Locomotion Mode\n<<" + GM.CurrentMovementManager.Mode.ToString() + ">>";
			for (int i = 0; i < Buttons.Count; i++)
			{
				Ray ray = new Ray(hand.OtherHand.PointingTransform.position, hand.OtherHand.PointingTransform.forward);
				if (Buttons[i].GetComponent<Collider>().Raycast(ray, out var hitInfo, 0.4f))
				{
					m_isButtonSelected = true;
					m_curButton = i;
					OBS_Wrist.SetSelectedButton(m_curButton);
					if (!SelectionBorder.gameObject.activeSelf)
					{
						SelectionBorder.gameObject.SetActive(value: true);
					}
					SelectionBorder.anchoredPosition = (Buttons[m_curButton].transform as RectTransform).anchoredPosition;
					SelectionBorder.sizeDelta = (Buttons[m_curButton].transform as RectTransform).sizeDelta;
					if (!Laser.gameObject.activeSelf)
					{
						Laser.gameObject.SetActive(value: true);
					}
					Laser.transform.position = hand.OtherHand.PointingTransform.position;
					Laser.transform.rotation = hand.OtherHand.PointingTransform.rotation;
					Laser.transform.localScale = new Vector3(0.002f, 0.002f, hitInfo.distance);
					break;
				}
			}
			if (!m_isButtonSelected)
			{
				if (SelectionBorder.gameObject.activeSelf)
				{
					SelectionBorder.gameObject.SetActive(value: false);
				}
				if (Laser.gameObject.activeSelf)
				{
					Laser.gameObject.SetActive(value: false);
				}
			}
			if (hand.OtherHand.Input.TriggerDown && m_isButtonSelected)
			{
				Buttons[m_curButton].onClick.Invoke();
				m_currentHand.Buzz(m_currentHand.Buzzer.Buzz_BeginInteraction);
				m_currentHand.OtherHand.Buzz(m_currentHand.OtherHand.Buzzer.Buzz_BeginInteraction);
			}
		}

		public void SetSelectedButton(int i)
		{
			m_curButton = i;
			OBS_Wrist.SetSelectedButton(m_curButton);
			if (!SelectionBorder.gameObject.activeSelf)
			{
				SelectionBorder.gameObject.SetActive(value: true);
			}
			SelectionBorder.anchoredPosition = (Buttons[m_curButton].transform as RectTransform).anchoredPosition;
			SelectionBorder.sizeDelta = (Buttons[m_curButton].transform as RectTransform).sizeDelta;
		}

		public void InvokeButton(int i)
		{
			Buttons[i].onClick.Invoke();
			m_currentHand.Buzz(m_currentHand.Buzzer.Buzz_BeginInteraction);
			m_currentHand.OtherHand.Buzz(m_currentHand.OtherHand.Buzzer.Buzz_BeginInteraction);
		}

		private void SelectionUp()
		{
			if (m_curButton > 0)
			{
				m_curButton--;
				OBS_Wrist.SetSelectedButton(m_curButton);
				m_currentHand.Buzz(m_currentHand.Buzzer.Buzz_BeginInteraction);
				Aud.PlayOneShot(AudClip_Nav, 1f);
			}
		}

		private void SelectionDown()
		{
			if (m_curButton < Buttons.Count - 1)
			{
				m_curButton++;
				OBS_Wrist.SetSelectedButton(m_curButton);
				m_currentHand.Buzz(m_currentHand.Buzzer.Buzz_BeginInteraction);
				Aud.PlayOneShot(AudClip_Nav, 1f);
			}
		}

		private void ActivateOnHand(FVRViveHand hand)
		{
			if (!m_isActive)
			{
				m_isActive = true;
				m_currentHand = hand;
				m_currentHand.EnableWristMenu(this);
				MenuGO.SetActive(value: true);
				ResetConfirm();
			}
		}

		private void Deactivate()
		{
			if (m_isActive)
			{
				m_isActive = false;
				m_currentHand.DisableWristMenu();
				m_currentHand = null;
				MenuGO.SetActive(value: false);
				if (Laser.gameObject.activeSelf)
				{
					Laser.gameObject.SetActive(value: false);
				}
				ResetConfirm();
			}
		}

		public void SpawnOptionsPanel()
		{
			if (GM.CurrentOptionsPanel != null)
			{
				m_currentHand.RetrieveObject(GM.CurrentOptionsPanel.GetComponent<FVRPhysicalObject>());
			}
			else
			{
				GameObject gameObject2 = (GM.CurrentOptionsPanel = UnityEngine.Object.Instantiate(OptionsPanelPrefab, Vector3.zero, Quaternion.identity));
				m_currentHand.RetrieveObject(GM.CurrentOptionsPanel.GetComponent<FVRPhysicalObject>());
			}
			Aud.PlayOneShot(AudClip_Engage, 1f);
		}

		public void SpawnSpectatorPanel()
		{
			if (GM.CurrentSpectatorPanel != null)
			{
				m_currentHand.RetrieveObject(GM.CurrentSpectatorPanel.GetComponent<FVRPhysicalObject>());
			}
			else
			{
				GameObject gameObject2 = (GM.CurrentSpectatorPanel = UnityEngine.Object.Instantiate(SpectatorPanelPrefab, Vector3.zero, Quaternion.identity));
				m_currentHand.RetrieveObject(GM.CurrentSpectatorPanel.GetComponent<FVRPhysicalObject>());
			}
			Aud.PlayOneShot(AudClip_Engage, 1f);
		}

		public void CleanUpScene_Empties()
		{
			Aud.PlayOneShot(AudClip_Engage, 1f);
			if (!askConfirm_CleanupEmpties)
			{
				ResetConfirm();
				AskConfirm_CleanupEmpties();
				return;
			}
			ResetConfirm();
			FVRFireArmMagazine[] array = UnityEngine.Object.FindObjectsOfType<FVRFireArmMagazine>();
			for (int num = array.Length - 1; num >= 0; num--)
			{
				if (!array[num].IsHeld && array[num].QuickbeltSlot == null && array[num].FireArm == null && array[num].m_numRounds == 0)
				{
					UnityEngine.Object.Destroy(array[num].gameObject);
				}
			}
			FVRFireArmRound[] array2 = UnityEngine.Object.FindObjectsOfType<FVRFireArmRound>();
			for (int num2 = array2.Length - 1; num2 >= 0; num2--)
			{
				if (!array2[num2].IsHeld && array2[num2].QuickbeltSlot == null && array2[num2].RootRigidbody != null)
				{
					UnityEngine.Object.Destroy(array2[num2].gameObject);
				}
			}
			FVRFireArmClip[] array3 = UnityEngine.Object.FindObjectsOfType<FVRFireArmClip>();
			for (int num3 = array3.Length - 1; num3 >= 0; num3--)
			{
				if (!array3[num3].IsHeld && array3[num3].QuickbeltSlot == null && array3[num3].FireArm == null && array3[num3].m_numRounds == 0)
				{
					UnityEngine.Object.Destroy(array3[num3].gameObject);
				}
			}
			Speedloader[] array4 = UnityEngine.Object.FindObjectsOfType<Speedloader>();
			for (int num4 = array4.Length - 1; num4 >= 0; num4--)
			{
				if (!array4[num4].IsHeld && array4[num4].QuickbeltSlot == null)
				{
					UnityEngine.Object.Destroy(array4[num4].gameObject);
				}
			}
		}

		public void CleanUpScene_AllMags()
		{
			Aud.PlayOneShot(AudClip_Engage, 1f);
			if (!askConfirm_CleanupAllMags)
			{
				ResetConfirm();
				AskConfirm_CleanupAllMags();
				return;
			}
			ResetConfirm();
			FVRFireArmMagazine[] array = UnityEngine.Object.FindObjectsOfType<FVRFireArmMagazine>();
			for (int num = array.Length - 1; num >= 0; num--)
			{
				if (!array[num].IsHeld && array[num].QuickbeltSlot == null && array[num].FireArm == null)
				{
					UnityEngine.Object.Destroy(array[num].gameObject);
				}
			}
			FVRFireArmRound[] array2 = UnityEngine.Object.FindObjectsOfType<FVRFireArmRound>();
			for (int num2 = array2.Length - 1; num2 >= 0; num2--)
			{
				if (!array2[num2].IsHeld && array2[num2].QuickbeltSlot == null && array2[num2].RootRigidbody != null)
				{
					UnityEngine.Object.Destroy(array2[num2].gameObject);
				}
			}
			FVRFireArmClip[] array3 = UnityEngine.Object.FindObjectsOfType<FVRFireArmClip>();
			for (int num3 = array3.Length - 1; num3 >= 0; num3--)
			{
				if (!array3[num3].IsHeld && array3[num3].QuickbeltSlot == null && array3[num3].FireArm == null)
				{
					UnityEngine.Object.Destroy(array3[num3].gameObject);
				}
			}
			Speedloader[] array4 = UnityEngine.Object.FindObjectsOfType<Speedloader>();
			for (int num4 = array4.Length - 1; num4 >= 0; num4--)
			{
				if (!array4[num4].IsHeld && array4[num4].QuickbeltSlot == null)
				{
					UnityEngine.Object.Destroy(array4[num4].gameObject);
				}
			}
		}

		public void CleanUpScene_Guns()
		{
			Aud.PlayOneShot(AudClip_Engage, 1f);
			if (!askConfirm_CleanupGuns)
			{
				ResetConfirm();
				AskConfirm_CleanupGuns();
				return;
			}
			ResetConfirm();
			FVRFireArm[] array = UnityEngine.Object.FindObjectsOfType<FVRFireArm>();
			for (int num = array.Length - 1; num >= 0; num--)
			{
				if (!array[num].IsHeld && array[num].QuickbeltSlot == null)
				{
					UnityEngine.Object.Destroy(array[num].gameObject);
				}
			}
			SosigWeapon[] array2 = UnityEngine.Object.FindObjectsOfType<SosigWeapon>();
			for (int num2 = array2.Length - 1; num2 >= 0; num2--)
			{
				if (!array2[num2].O.IsHeld && array2[num2].O.QuickbeltSlot == null && !array2[num2].IsHeldByBot && !array2[num2].IsInBotInventory)
				{
					UnityEngine.Object.Destroy(array2[num2].gameObject);
				}
			}
			FVRMeleeWeapon[] array3 = UnityEngine.Object.FindObjectsOfType<FVRMeleeWeapon>();
			for (int num3 = array3.Length - 1; num3 >= 0; num3--)
			{
				if (!array3[num3].IsHeld && array3[num3].QuickbeltSlot == null)
				{
					UnityEngine.Object.Destroy(array3[num3].gameObject);
				}
			}
		}

		public void ReloadScene()
		{
			Aud.PlayOneShot(AudClip_Engage, 1f);
			if (!askConfirm_Reload)
			{
				ResetConfirm();
				AskConfirm_Reload();
				return;
			}
			ResetConfirm();
			for (int i = 0; i < GM.CurrentSceneSettings.QuitReceivers.Count; i++)
			{
				GM.CurrentSceneSettings.QuitReceivers[i].BroadcastMessage("QUIT", SendMessageOptions.DontRequireReceiver);
			}
			if (GM.LoadingCallback.IsCompleted)
			{
				SteamVR_LoadLevel.Begin(SceneManager.GetActiveScene().name);
			}
		}

		public void ReturnToMainMenu()
		{
			Aud.PlayOneShot(AudClip_Engage, 1f);
			if (!askConfirm_ReturnMainmenu)
			{
				ResetConfirm();
				AskConfirm_ReturnMainmenu();
				return;
			}
			ResetConfirm();
			for (int i = 0; i < GM.CurrentSceneSettings.QuitReceivers.Count; i++)
			{
				GM.CurrentSceneSettings.QuitReceivers[i].BroadcastMessage("QUIT", SendMessageOptions.DontRequireReceiver);
			}
			if (GM.LoadingCallback.IsCompleted)
			{
				SteamVR_LoadLevel.Begin("MainMenu3");
			}
		}

		public void QuitToDesktop()
		{
			Aud.PlayOneShot(AudClip_Engage, 1f);
			if (!askConfirm_Quit)
			{
				ResetConfirm();
				AskConfirm_Quit();
				return;
			}
			ResetConfirm();
			for (int i = 0; i < GM.CurrentSceneSettings.QuitReceivers.Count; i++)
			{
				GM.CurrentSceneSettings.QuitReceivers[i].BroadcastMessage("QUIT", SendMessageOptions.DontRequireReceiver);
			}
			Application.Quit();
		}

		private void ResetConfirm()
		{
			askConfirm_Reload = false;
			askConfirm_ReturnMainmenu = false;
			askConfirm_Quit = false;
			askConfirm_CleanupEmpties = false;
			askConfirm_CleanupAllMags = false;
			askConfirm_CleanupGuns = false;
			TXT_ReloadScene.text = "Reload Scene";
			TXT_ReturnMainmenu.text = "Return to Main Menu";
			TXT_QuitToDesktop.text = "Quit To Desktop";
			TXT_CleanupEmpties.text = "Clean Empty Mags";
			TXT_CleanupAllMags.text = "Clean All Mags";
			TXT_CleanupGuns.text = "Clean Guns & Melee";
		}

		private void AskConfirm_CleanupEmpties()
		{
			askConfirm_CleanupEmpties = true;
			TXT_CleanupEmpties.text = "Confirm ???";
		}

		private void AskConfirm_CleanupAllMags()
		{
			askConfirm_CleanupAllMags = true;
			TXT_CleanupAllMags.text = "Confirm ???";
		}

		private void AskConfirm_CleanupGuns()
		{
			askConfirm_CleanupGuns = true;
			TXT_CleanupGuns.text = "Confirm ???";
		}

		private void AskConfirm_Reload()
		{
			askConfirm_Reload = true;
			TXT_ReloadScene.text = "Confirm ???";
		}

		private void AskConfirm_ReturnMainmenu()
		{
			askConfirm_ReturnMainmenu = true;
			TXT_ReturnMainmenu.text = "Confirm ???";
		}

		private void AskConfirm_Quit()
		{
			askConfirm_Quit = true;
			TXT_QuitToDesktop.text = "Confirm ???";
		}

		public void TurnClockWise()
		{
			Aud.PlayOneShot(AudClip_Engage, 1f);
			Vector3 position = GM.CurrentPlayerBody.Head.position;
			position.y = GM.CurrentPlayerBody.transform.position.y;
			Vector3 forward = GM.CurrentPlayerBody.transform.forward;
			float angle = GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex];
			forward = Quaternion.AngleAxis(angle, Vector3.up) * forward;
			GM.CurrentMovementManager.TeleportToPoint(position, isAbsolute: false, forward);
		}

		public void TurnCounterClockWise()
		{
			Aud.PlayOneShot(AudClip_Engage, 1f);
			Vector3 position = GM.CurrentPlayerBody.Head.position;
			position.y = GM.CurrentPlayerBody.transform.position.y;
			Vector3 forward = GM.CurrentPlayerBody.transform.forward;
			float num = GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex];
			forward = Quaternion.AngleAxis(0f - num, Vector3.up) * forward;
			GM.CurrentMovementManager.TeleportToPoint(position, isAbsolute: false, forward);
		}

		public void SetLocomotionMode(int i)
		{
			if (Hands[0].IsInStreamlinedMode && (i == 2 || i == 3 || i == 5))
			{
				Aud.PlayOneShot(AudClip_Err, 1f);
				return;
			}
			Aud.PlayOneShot(AudClip_Engage, 1f);
			GM.CurrentMovementManager.Mode = (FVRMovementManager.MovementMode)i;
			GM.CurrentMovementManager.CleanupFlagsForModeSwitch();
			GM.CurrentMovementManager.InitArmSwinger();
			GM.Options.MovementOptions.CurrentMovementMode = (FVRMovementManager.MovementMode)i;
			GM.Options.SaveToFile();
		}

		public void CycleLocomotionMode()
		{
			Aud.PlayOneShot(AudClip_Engage, 1f);
			LocomotionModeDisplay.text = "Locomotion Mode\n<<" + GM.CurrentMovementManager.Mode.ToString() + ">>";
		}
	}
}
