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
			LoadSceneButton.SetActive(value: false);
			Label_Title.text = "Please Select A Scene";
			Label_Type.text = string.Empty;
			Label_Description.text = string.Empty;
		}

		private void Start()
		{
			Debug.Log(GM.HMDMode.ToString() + " detected with " + SteamVR.instance.hmd_DisplayFrequency + " frequency");
			if (GM.HMDMode == ControlMode.Index)
			{
				Time.fixedDeltaTime = Time.timeScale / SteamVR.instance.hmd_DisplayFrequency;
				Time.maximumDeltaTime = Time.fixedDeltaTime * 2f;
				Debug.Log("Scene delta is:" + Time.fixedDeltaTime);
			}
		}

		public void SetSelectedScene(MainMenuSceneDef Def)
		{
			m_def = Def;
			Label_Title.text = m_def.Name;
			Label_Type.text = m_def.Type;
			Label_Description.text = m_def.Desciption;
			Image_Preview.gameObject.SetActive(value: true);
			Image_Preview.sprite = m_def.Image;
		}

		private void Update()
		{
			if (GM.LoadingCallback.IsCompleted && !m_hasFinishedInitialPrewarm)
			{
				m_hasFinishedInitialPrewarm = true;
				Debug.Log("Prewarm Completed GUI Check");
			}
			if (GM.LoadingCallback.keepWaiting)
			{
				Label_Title.text = "Still Loading - Progress: " + (int)(GM.LoadingCallback.Progress * 100f) + "%";
			}
			else if (m_def == null)
			{
				Label_Title.text = "Loading Complete - Select Scene";
				if (LoadSceneButton.activeSelf)
				{
					LoadSceneButton.SetActive(value: false);
				}
			}
			else if (!LoadSceneButton.activeSelf)
			{
				LoadSceneButton.SetActive(value: true);
			}
			if (GM.LoadingCallback.Progress > 0.98f && !m_hasFinishedInitialPrewarm)
			{
				m_timeForButtonToAppear += Time.deltaTime;
				if (m_timeForButtonToAppear > 5f && !ForceLoadButton.activeSelf)
				{
					ForceLoadButton.SetActive(value: true);
				}
			}
			else if (ForceLoadButton.activeSelf)
			{
				ForceLoadButton.SetActive(value: false);
			}
			if (!m_hasFinishedInitialPrewarm && Input.GetKeyDown(KeyCode.L))
			{
				ForceLoad();
			}
		}

		public void ForceLoad()
		{
			if (!m_hasFinishedInitialPrewarm)
			{
				Debug.Log("Forcing Load Attempt");
				m_hasFinishedInitialPrewarm = true;
				GM.LoadingCallback.CompleteNow();
			}
		}

		public void LoadScene()
		{
			if (m_def != null && !m_isLoading)
			{
				m_isLoading = true;
				SteamVR_LoadLevel.Begin(m_def.SceneName);
			}
		}
	}
}
