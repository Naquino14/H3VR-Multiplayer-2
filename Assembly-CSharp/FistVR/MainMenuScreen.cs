// Decompiled with JetBrains decompiler
// Type: FistVR.MainMenuScreen
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

namespace FistVR
{
  public class MainMenuScreen : MonoBehaviour
  {
    public Text Label_Title;
    public Text Label_Type;
    public Text Label_Description;
    public Image Image_Preview;
    public GameObject ForceLoadButton;
    private MainMenuSceneDef m_def;
    private bool m_isLoading;
    public GameObject LoadSceneButton;
    private bool m_hasFinishedInitialPrewarm;
    private float m_timeForButtonToAppear;

    private void Awake()
    {
      this.LoadSceneButton.SetActive(false);
      this.Label_Title.text = "Please Select A Scene";
      this.Label_Type.text = string.Empty;
      this.Label_Description.text = string.Empty;
    }

    private void Start()
    {
      Debug.Log((object) (GM.HMDMode.ToString() + " detected with " + SteamVR.instance.hmd_DisplayFrequency.ToString() + " frequency"));
      if (GM.HMDMode != ControlMode.Index)
        return;
      Time.fixedDeltaTime = Time.timeScale / SteamVR.instance.hmd_DisplayFrequency;
      Time.maximumDeltaTime = Time.fixedDeltaTime * 2f;
      Debug.Log((object) ("Scene delta is:" + (object) Time.fixedDeltaTime));
    }

    public void SetSelectedScene(MainMenuSceneDef Def)
    {
      this.m_def = Def;
      this.Label_Title.text = this.m_def.Name;
      this.Label_Type.text = this.m_def.Type;
      this.Label_Description.text = this.m_def.Desciption;
      this.Image_Preview.gameObject.SetActive(true);
      this.Image_Preview.sprite = this.m_def.Image;
    }

    private void Update()
    {
      if (GM.LoadingCallback.IsCompleted && !this.m_hasFinishedInitialPrewarm)
      {
        this.m_hasFinishedInitialPrewarm = true;
        Debug.Log((object) "Prewarm Completed GUI Check");
      }
      if (GM.LoadingCallback.keepWaiting)
        this.Label_Title.text = "Still Loading - Progress: " + (object) (int) ((double) GM.LoadingCallback.Progress * 100.0) + "%";
      else if ((Object) this.m_def == (Object) null)
      {
        this.Label_Title.text = "Loading Complete - Select Scene";
        if (this.LoadSceneButton.activeSelf)
          this.LoadSceneButton.SetActive(false);
      }
      else if (!this.LoadSceneButton.activeSelf)
        this.LoadSceneButton.SetActive(true);
      if ((double) GM.LoadingCallback.Progress > 0.980000019073486 && !this.m_hasFinishedInitialPrewarm)
      {
        this.m_timeForButtonToAppear += Time.deltaTime;
        if ((double) this.m_timeForButtonToAppear > 5.0 && !this.ForceLoadButton.activeSelf)
          this.ForceLoadButton.SetActive(true);
      }
      else if (this.ForceLoadButton.activeSelf)
        this.ForceLoadButton.SetActive(false);
      if (this.m_hasFinishedInitialPrewarm || !Input.GetKeyDown(KeyCode.L))
        return;
      this.ForceLoad();
    }

    public void ForceLoad()
    {
      if (this.m_hasFinishedInitialPrewarm)
        return;
      Debug.Log((object) "Forcing Load Attempt");
      this.m_hasFinishedInitialPrewarm = true;
      GM.LoadingCallback.CompleteNow();
    }

    public void LoadScene()
    {
      if (!((Object) this.m_def != (Object) null) || this.m_isLoading)
        return;
      this.m_isLoading = true;
      SteamVR_LoadLevel.Begin(this.m_def.SceneName);
    }
  }
}
