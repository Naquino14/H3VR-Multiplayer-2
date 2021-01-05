using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class SpectatorPanel : MonoBehaviour
	{
		[Header("SosigBody")]
		public List<Text> List_Categories;

		public List<Text> List_TemplatesToAdd;

		public List<SosigEnemyCategory> Cats;

		private List<SosigEnemyCategory> m_categories = new List<SosigEnemyCategory>();

		private List<SosigEnemyID> m_curDisplayedIDsToAdd = new List<SosigEnemyID>();

		private List<SosigEnemyTemplate> m_curDisplayedTemplatesToAdd = new List<SosigEnemyTemplate>();

		public Text FOVLabel;

		public OptionsPanel_ButtonSet OBS_PlayerBody;

		public OptionsPanel_ButtonSet OBS_CamMode;

		public OptionsPanel_ButtonSet OBS_LinearSmooth;

		public OptionsPanel_ButtonSet OBS_AngularSmooth;

		public OptionsPanel_ButtonSet OBS_LevelingSmooth;

		public OptionsPanel_ButtonSet OBS_CameraEye;

		public OptionsPanel_ButtonSet OBS_RenderQuality;

		public OptionsPanel_ButtonSet OBS_CamPreview;

		public OptionsPanel_ButtonSet OBS_TPCDistanceIndex;

		public OptionsPanel_ButtonSet OBS_TPCLateralIndex;

		public AudioEvent AudEvent_Beep;

		public AudioEvent AudEvent_Boop;

		public FVRObject CamcorderPrefab;

		public FVRObject RailCamPrefab;

		public FVRObject MuzzleCamPrefab;

		public void Beep()
		{
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Beep, base.transform.position);
		}

		public void Boop()
		{
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Boop, base.transform.position);
		}

		private void Start()
		{
			BTN_Template_SetGroup(0);
			OBS_PlayerBody.SetSelectedButton((int)GM.Options.ControlOptions.MBMode);
			OBS_CamMode.SetSelectedButton((int)GM.Options.ControlOptions.CamMode);
			OBS_LinearSmooth.SetSelectedButton((int)GM.Options.ControlOptions.CamSmoothingLinear);
			OBS_AngularSmooth.SetSelectedButton((int)GM.Options.ControlOptions.CamSmoothingRotational);
			OBS_LevelingSmooth.SetSelectedButton((int)GM.Options.ControlOptions.CamLeveling);
			FOVLabel.text = GM.Options.ControlOptions.CamFOV.ToString();
			OBS_CameraEye.SetSelectedButton((int)GM.Options.ControlOptions.CamEye);
			OBS_RenderQuality.SetSelectedButton((int)GM.Options.ControlOptions.CamQual);
			OBS_CamPreview.SetSelectedButton((int)GM.Options.ControlOptions.PCamMode);
			OBS_TPCDistanceIndex.SetSelectedButton(GM.Options.ControlOptions.TPCDistanceIndex);
			OBS_TPCLateralIndex.SetSelectedButton(GM.Options.ControlOptions.TPCLateralIndex);
		}

		public void BTN_SpawnCamcorder()
		{
			Object.Instantiate(CamcorderPrefab.GetGameObject(), base.transform.position + Vector3.up * 0.4f, Quaternion.identity);
			Boop();
		}

		public void BTN_SpawnRailCam()
		{
			Object.Instantiate(RailCamPrefab.GetGameObject(), base.transform.position + Vector3.up * 0.4f, Quaternion.identity);
			Boop();
		}

		public void BTN_SpawnMuzzleCam()
		{
			Object.Instantiate(MuzzleCamPrefab.GetGameObject(), base.transform.position + Vector3.up * 0.4f, Quaternion.identity);
			Boop();
		}

		public void BTN_Template_SetGroup(int i)
		{
			if (i < Cats.Count)
			{
				SosigEnemyCategory key = Cats[i];
				m_curDisplayedIDsToAdd = ManagerSingleton<IM>.Instance.odicSosigIDsByCategory[key];
				m_curDisplayedTemplatesToAdd = ManagerSingleton<IM>.Instance.odicSosigObjsByCategory[key];
				UpdatePanel();
				Beep();
			}
		}

		public void BTN_Template_Set(int i)
		{
			GM.CurrentPlayerBody.SetOutfit(m_curDisplayedTemplatesToAdd[i]);
			Boop();
		}

		public void BTN_SetBodyMode(int i)
		{
			if (i == 0)
			{
				GM.Options.ControlOptions.MBMode = ControlOptions.MeatBody.Disabled;
			}
			else
			{
				GM.Options.ControlOptions.MBMode = ControlOptions.MeatBody.Enabled;
			}
			GM.CurrentPlayerBody.UpdateSosigPlayerBodyState();
			GM.Options.SaveToFile();
			OBS_PlayerBody.SetSelectedButton(i);
			Beep();
		}

		public void BTN_SetCamMode(int i)
		{
			GM.Options.ControlOptions.CamMode = (ControlOptions.DesktopCameraMode)i;
			GM.Options.SaveToFile();
			OBS_CamMode.SetSelectedButton(i);
			Beep();
		}

		public void BTN_SetFOV(float i)
		{
			GM.Options.ControlOptions.CamFOV = Mathf.Clamp(GM.Options.ControlOptions.CamFOV + i, 1f, 150f);
			FOVLabel.text = GM.Options.ControlOptions.CamFOV.ToString();
			GM.Options.SaveToFile();
			Beep();
		}

		public void BTN_SetCamLinearSmooth(float i)
		{
			GM.Options.ControlOptions.CamSmoothingLinear = i;
			GM.Options.SaveToFile();
			OBS_LinearSmooth.SetSelectedButton((int)i);
			Beep();
		}

		public void BTN_SetCamAngularSmooth(float i)
		{
			GM.Options.ControlOptions.CamSmoothingRotational = i;
			GM.Options.SaveToFile();
			OBS_AngularSmooth.SetSelectedButton((int)i);
			Beep();
		}

		public void BTN_SetCamLevelingSmooth(float i)
		{
			GM.Options.ControlOptions.CamLeveling = i;
			GM.Options.SaveToFile();
			OBS_LevelingSmooth.SetSelectedButton((int)i);
			Beep();
		}

		public void BTN_SetCamEye(int i)
		{
			GM.Options.ControlOptions.CamEye = (ControlOptions.DesktopCameraEye)i;
			GM.Options.SaveToFile();
			OBS_CameraEye.SetSelectedButton(i);
			Beep();
		}

		public void BTN_SetCamQual(int i)
		{
			GM.Options.ControlOptions.CamQual = (ControlOptions.DesktopRenderQuality)i;
			GM.Options.SaveToFile();
			OBS_RenderQuality.SetSelectedButton(i);
			Beep();
		}

		public void BTN_SetPCam(int i)
		{
			GM.Options.ControlOptions.PCamMode = (ControlOptions.PreviewCamMode)i;
			GM.Options.SaveToFile();
			OBS_CamPreview.SetSelectedButton(i);
			Beep();
		}

		public void BTN_TPCDistanceIndex(int i)
		{
			GM.Options.ControlOptions.TPCDistanceIndex = i;
			GM.Options.SaveToFile();
			OBS_TPCDistanceIndex.SetSelectedButton(i);
			Beep();
		}

		public void BTN_TPCLateralIndex(int i)
		{
			GM.Options.ControlOptions.TPCLateralIndex = i;
			GM.Options.SaveToFile();
			OBS_TPCLateralIndex.SetSelectedButton(i);
			Beep();
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.rigidbody != null)
			{
				SosigLink component = collision.rigidbody.gameObject.GetComponent<SosigLink>();
				if (component != null && component.S != null && component.S.Links[0] != null)
				{
					GM.CurrentSceneSettings.SetAttachedLink(component.S.Links[0]);
					Boop();
				}
			}
		}

		private void Update()
		{
		}

		private void UpdatePanel()
		{
			for (int i = 0; i < List_Categories.Count; i++)
			{
				if (i < Cats.Count)
				{
					List_Categories[i].gameObject.SetActive(value: true);
					List_Categories[i].text = Cats[i].ToString();
				}
				else
				{
					List_Categories[i].gameObject.SetActive(value: false);
				}
			}
			for (int j = 0; j < List_TemplatesToAdd.Count; j++)
			{
				if (j < m_curDisplayedTemplatesToAdd.Count)
				{
					List_TemplatesToAdd[j].gameObject.SetActive(value: true);
					List_TemplatesToAdd[j].text = m_curDisplayedTemplatesToAdd[j].DisplayName.ToString();
				}
				else
				{
					List_TemplatesToAdd[j].gameObject.SetActive(value: false);
				}
			}
		}
	}
}
