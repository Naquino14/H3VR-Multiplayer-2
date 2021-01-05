using UnityEngine;

public class MainMenu : MonoBehaviour
{
	private bool m_isLoading;

	public void LoadRange(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("Testing2_RangeMockup");
		}
	}

	public void LoadArcade(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("Testing3_LaserSword");
		}
	}

	public void LoadIndoorRange(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("IndoorRange");
		}
	}

	public void LoadArizonaTargets(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("ArizonaTargets");
		}
	}

	public void LoadArizonaAtNight(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("ArizonaTargets_Night");
		}
	}

	public void LoadGrenadeSkeeball(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("GrenadeSkeeball");
		}
	}

	public void LoadModularFiringRange(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("ModularFiringRange");
		}
	}

	public void LoadBreaching(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("BreachAndClear_TestScene1");
		}
	}

	public void LoadGunnasium(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("ObstacleCourseScene1");
		}
	}

	public void LoadArena(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("ObstacleCourseScene2");
		}
	}

	public void LoadSniperRange(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("SniperRange");
		}
	}

	public void LoadHickockRange(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("HickockRange");
		}
	}

	public void LoadMeatGrinder(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("MeatGrinder_StartingScene");
		}
	}

	public void LoadHickockRangeNight(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("HickockRange_Night");
		}
	}

	public void LoadUGS(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("UGS_Lobby");
		}
	}

	public void LoadMeatmas(int i)
	{
		if (!m_isLoading)
		{
			m_isLoading = true;
			SteamVR_LoadLevel.Begin("Xmas");
		}
	}

	public void AppQuit(int i)
	{
		Application.Quit();
	}
}
