using UnityEngine;

namespace FistVR
{
	public class TAH_UIManager : MonoBehaviour
	{
		public TAH_Manager Manager;

		public OptionsPanel_ButtonSet OBS_PlayerHealth;

		public OptionsPanel_ButtonSet OBS_Difficulty;

		public OptionsPanel_ButtonSet OBS_BotBullets;

		public OptionsPanel_ButtonSet OBS_Music;

		public OptionsPanel_ButtonSet OBS_LootProgression;

		public OptionsPanel_ButtonSet OBS_ItemSpawner;

		private void Start()
		{
			OBS_PlayerHealth.SetSelectedButton(GM.TAHSettings.TAHOption_PlayerHealth);
			OBS_Difficulty.SetSelectedButton(GM.TAHSettings.TAHOption_DifficultyProgression);
			OBS_BotBullets.SetSelectedButton(GM.TAHSettings.TAHOption_BotBullets);
			OBS_Music.SetSelectedButton(GM.TAHSettings.TAHOption_Music);
			OBS_LootProgression.SetSelectedButton(GM.TAHSettings.TAHOption_LootProgression);
			OBS_ItemSpawner.SetSelectedButton(GM.TAHSettings.TAHOption_ItemSpawner);
		}

		public void SetPlayerHealth(int i)
		{
			GM.TAHSettings.TAHOption_PlayerHealth = i;
			Manager.UpdateHealth();
			GM.TAHSettings.SaveToFile();
		}

		public void SetDifficulty(int i)
		{
			GM.TAHSettings.TAHOption_DifficultyProgression = i;
			GM.TAHSettings.SaveToFile();
		}

		public void BotBullets(int i)
		{
			GM.TAHSettings.TAHOption_BotBullets = i;
			GM.TAHSettings.SaveToFile();
		}

		public void Music(int i)
		{
			if (i == 0)
			{
				Manager.MusicState(b: false);
			}
			else
			{
				Manager.MusicState(b: true);
			}
			GM.TAHSettings.TAHOption_Music = i;
			GM.TAHSettings.SaveToFile();
		}

		public void LootProgression(int i)
		{
			GM.TAHSettings.TAHOption_LootProgression = i;
			GM.TAHSettings.SaveToFile();
		}

		public void ItemSpawner(int i)
		{
			if (i == 0)
			{
				Manager.ItemSpawnerState(b: false);
			}
			else
			{
				Manager.ItemSpawnerState(b: true);
			}
			GM.TAHSettings.TAHOption_ItemSpawner = i;
			GM.TAHSettings.SaveToFile();
		}

		public void StartGame()
		{
			Manager.BeginGame();
		}
	}
}
