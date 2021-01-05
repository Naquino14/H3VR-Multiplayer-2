// Decompiled with JetBrains decompiler
// Type: FistVR.SpectatorPanel
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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

    public void Beep() => SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Beep, this.transform.position);

    public void Boop() => SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Boop, this.transform.position);

    private void Start()
    {
      this.BTN_Template_SetGroup(0);
      this.OBS_PlayerBody.SetSelectedButton((int) GM.Options.ControlOptions.MBMode);
      this.OBS_CamMode.SetSelectedButton((int) GM.Options.ControlOptions.CamMode);
      this.OBS_LinearSmooth.SetSelectedButton((int) GM.Options.ControlOptions.CamSmoothingLinear);
      this.OBS_AngularSmooth.SetSelectedButton((int) GM.Options.ControlOptions.CamSmoothingRotational);
      this.OBS_LevelingSmooth.SetSelectedButton((int) GM.Options.ControlOptions.CamLeveling);
      this.FOVLabel.text = GM.Options.ControlOptions.CamFOV.ToString();
      this.OBS_CameraEye.SetSelectedButton((int) GM.Options.ControlOptions.CamEye);
      this.OBS_RenderQuality.SetSelectedButton((int) GM.Options.ControlOptions.CamQual);
      this.OBS_CamPreview.SetSelectedButton((int) GM.Options.ControlOptions.PCamMode);
      this.OBS_TPCDistanceIndex.SetSelectedButton(GM.Options.ControlOptions.TPCDistanceIndex);
      this.OBS_TPCLateralIndex.SetSelectedButton(GM.Options.ControlOptions.TPCLateralIndex);
    }

    public void BTN_SpawnCamcorder()
    {
      Object.Instantiate<GameObject>(this.CamcorderPrefab.GetGameObject(), this.transform.position + Vector3.up * 0.4f, Quaternion.identity);
      this.Boop();
    }

    public void BTN_SpawnRailCam()
    {
      Object.Instantiate<GameObject>(this.RailCamPrefab.GetGameObject(), this.transform.position + Vector3.up * 0.4f, Quaternion.identity);
      this.Boop();
    }

    public void BTN_SpawnMuzzleCam()
    {
      Object.Instantiate<GameObject>(this.MuzzleCamPrefab.GetGameObject(), this.transform.position + Vector3.up * 0.4f, Quaternion.identity);
      this.Boop();
    }

    public void BTN_Template_SetGroup(int i)
    {
      if (i >= this.Cats.Count)
        return;
      SosigEnemyCategory cat = this.Cats[i];
      this.m_curDisplayedIDsToAdd = ManagerSingleton<IM>.Instance.odicSosigIDsByCategory[cat];
      this.m_curDisplayedTemplatesToAdd = ManagerSingleton<IM>.Instance.odicSosigObjsByCategory[cat];
      this.UpdatePanel();
      this.Beep();
    }

    public void BTN_Template_Set(int i)
    {
      GM.CurrentPlayerBody.SetOutfit(this.m_curDisplayedTemplatesToAdd[i]);
      this.Boop();
    }

    public void BTN_SetBodyMode(int i)
    {
      GM.Options.ControlOptions.MBMode = i != 0 ? ControlOptions.MeatBody.Enabled : ControlOptions.MeatBody.Disabled;
      GM.CurrentPlayerBody.UpdateSosigPlayerBodyState();
      GM.Options.SaveToFile();
      this.OBS_PlayerBody.SetSelectedButton(i);
      this.Beep();
    }

    public void BTN_SetCamMode(int i)
    {
      GM.Options.ControlOptions.CamMode = (ControlOptions.DesktopCameraMode) i;
      GM.Options.SaveToFile();
      this.OBS_CamMode.SetSelectedButton(i);
      this.Beep();
    }

    public void BTN_SetFOV(float i)
    {
      GM.Options.ControlOptions.CamFOV = Mathf.Clamp(GM.Options.ControlOptions.CamFOV + i, 1f, 150f);
      this.FOVLabel.text = GM.Options.ControlOptions.CamFOV.ToString();
      GM.Options.SaveToFile();
      this.Beep();
    }

    public void BTN_SetCamLinearSmooth(float i)
    {
      GM.Options.ControlOptions.CamSmoothingLinear = i;
      GM.Options.SaveToFile();
      this.OBS_LinearSmooth.SetSelectedButton((int) i);
      this.Beep();
    }

    public void BTN_SetCamAngularSmooth(float i)
    {
      GM.Options.ControlOptions.CamSmoothingRotational = i;
      GM.Options.SaveToFile();
      this.OBS_AngularSmooth.SetSelectedButton((int) i);
      this.Beep();
    }

    public void BTN_SetCamLevelingSmooth(float i)
    {
      GM.Options.ControlOptions.CamLeveling = i;
      GM.Options.SaveToFile();
      this.OBS_LevelingSmooth.SetSelectedButton((int) i);
      this.Beep();
    }

    public void BTN_SetCamEye(int i)
    {
      GM.Options.ControlOptions.CamEye = (ControlOptions.DesktopCameraEye) i;
      GM.Options.SaveToFile();
      this.OBS_CameraEye.SetSelectedButton(i);
      this.Beep();
    }

    public void BTN_SetCamQual(int i)
    {
      GM.Options.ControlOptions.CamQual = (ControlOptions.DesktopRenderQuality) i;
      GM.Options.SaveToFile();
      this.OBS_RenderQuality.SetSelectedButton(i);
      this.Beep();
    }

    public void BTN_SetPCam(int i)
    {
      GM.Options.ControlOptions.PCamMode = (ControlOptions.PreviewCamMode) i;
      GM.Options.SaveToFile();
      this.OBS_CamPreview.SetSelectedButton(i);
      this.Beep();
    }

    public void BTN_TPCDistanceIndex(int i)
    {
      GM.Options.ControlOptions.TPCDistanceIndex = i;
      GM.Options.SaveToFile();
      this.OBS_TPCDistanceIndex.SetSelectedButton(i);
      this.Beep();
    }

    public void BTN_TPCLateralIndex(int i)
    {
      GM.Options.ControlOptions.TPCLateralIndex = i;
      GM.Options.SaveToFile();
      this.OBS_TPCLateralIndex.SetSelectedButton(i);
      this.Beep();
    }

    private void OnCollisionEnter(Collision collision)
    {
      if (!((Object) collision.rigidbody != (Object) null))
        return;
      SosigLink component = collision.rigidbody.gameObject.GetComponent<SosigLink>();
      if (!((Object) component != (Object) null) || !((Object) component.S != (Object) null) || !((Object) component.S.Links[0] != (Object) null))
        return;
      GM.CurrentSceneSettings.SetAttachedLink(component.S.Links[0]);
      this.Boop();
    }

    private void Update()
    {
    }

    private void UpdatePanel()
    {
      for (int index = 0; index < this.List_Categories.Count; ++index)
      {
        if (index < this.Cats.Count)
        {
          this.List_Categories[index].gameObject.SetActive(true);
          this.List_Categories[index].text = this.Cats[index].ToString();
        }
        else
          this.List_Categories[index].gameObject.SetActive(false);
      }
      for (int index = 0; index < this.List_TemplatesToAdd.Count; ++index)
      {
        if (index < this.m_curDisplayedTemplatesToAdd.Count)
        {
          this.List_TemplatesToAdd[index].gameObject.SetActive(true);
          this.List_TemplatesToAdd[index].text = this.m_curDisplayedTemplatesToAdd[index].DisplayName.ToString();
        }
        else
          this.List_TemplatesToAdd[index].gameObject.SetActive(false);
      }
    }
  }
}
