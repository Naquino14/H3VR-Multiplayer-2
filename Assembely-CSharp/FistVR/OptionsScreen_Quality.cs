using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_Quality : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_CurQualitySetting;

		public OptionsPanel_ButtonSet OBS_Post_CC;

		public OptionsPanel_ButtonSet OBS_Post_Bloom;

		public OptionsPanel_ButtonSet OBS_Post_AO;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_CurQualitySetting.SetSelectedButton((int)GM.Options.PerformanceOptions.CurrentQualitySetting);
			OBS_Post_CC.SetSelectedButton(!GM.Options.PerformanceOptions.IsPostEnabled_CC);
			OBS_Post_Bloom.SetSelectedButton(!GM.Options.PerformanceOptions.IsPostEnabled_Bloom);
			OBS_Post_AO.SetSelectedButton(!GM.Options.PerformanceOptions.IsPostEnabled_AO);
		}

		public void SetCurQualitySetting(int i)
		{
			GM.Options.PerformanceOptions.CurrentQualitySetting = (PerformanceOptions.QualitySetting)i;
			GM.Options.SaveToFile();
			GM.RefreshQuality();
		}

		public void SetCurPost_CC(bool b)
		{
			GM.Options.PerformanceOptions.IsPostEnabled_CC = b;
			GM.Options.SaveToFile();
			GM.RefreshQuality();
		}

		public void SetCurPost_Bloom(bool b)
		{
			GM.Options.PerformanceOptions.IsPostEnabled_Bloom = b;
			GM.Options.SaveToFile();
			GM.RefreshQuality();
		}

		public void SetCurPost_AO(bool b)
		{
			GM.Options.PerformanceOptions.IsPostEnabled_AO = b;
			GM.Options.SaveToFile();
			GM.RefreshQuality();
		}
	}
}
