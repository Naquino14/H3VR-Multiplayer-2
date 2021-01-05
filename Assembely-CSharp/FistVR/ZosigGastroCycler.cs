using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigGastroCycler : ZosigQuestManager
	{
		public enum GastroState
		{
			Disabled,
			Menu,
			Cores,
			Fruit,
			Banger
		}

		[Header("Health")]
		public Renderer HealthRingRend;

		public Color Health_Full;

		public Color Health_Empty;

		public List<ZosigGastroCyclerPointZone> PointingZones;

		public GastroState State;

		[Header("Groups")]
		public Transform Root_Menu;

		public Transform Root_Cores;

		public Transform Root_Fruit;

		public Transform Root_Banger;

		public List<Transform> Group_Menu;

		public List<Transform> Group_Cores;

		public List<Transform> Group_Fruit;

		public List<Transform> Group_Banger;

		private ZosigGameManager M;

		private int m_curPointIndex = -1;

		public override void Init(ZosigGameManager m)
		{
			M = m;
			SetState(GastroState.Disabled);
			M.SetGCycler(this);
		}

		public void InitiatePoint(int i)
		{
		}

		public void EndPoint(int i)
		{
		}

		public void UpdateState()
		{
			SetState(State);
		}

		public void SetState(GastroState s)
		{
			State = s;
			switch (State)
			{
			case GastroState.Disabled:
				SetState_Disabled();
				break;
			case GastroState.Menu:
				SetState_Menu();
				break;
			case GastroState.Cores:
				SetState_Cores();
				break;
			case GastroState.Fruit:
				SetState_Fruit();
				break;
			case GastroState.Banger:
				SetState_Banger();
				break;
			}
		}

		public void SetState_Disabled()
		{
			PointingZones[0].IsEnabled = true;
			for (int i = 1; i < PointingZones.Count; i++)
			{
				PointingZones[i].IsEnabled = false;
			}
			Root_Menu.gameObject.SetActive(value: false);
			Root_Cores.gameObject.SetActive(value: false);
			Root_Fruit.gameObject.SetActive(value: false);
			Root_Banger.gameObject.SetActive(value: false);
		}

		public void SetState_Menu()
		{
			Root_Menu.gameObject.SetActive(value: true);
			Root_Cores.gameObject.SetActive(value: false);
			Root_Fruit.gameObject.SetActive(value: false);
			Root_Banger.gameObject.SetActive(value: false);
			for (int i = 0; i < 4; i++)
			{
				PointingZones[i].IsEnabled = true;
			}
			for (int j = 4; j < PointingZones.Count; j++)
			{
				PointingZones[j].IsEnabled = false;
			}
		}

		public void SetState_Cores()
		{
			Root_Menu.gameObject.SetActive(value: false);
			Root_Cores.gameObject.SetActive(value: true);
			Root_Fruit.gameObject.SetActive(value: false);
			Root_Banger.gameObject.SetActive(value: false);
			PointingZones[0].IsEnabled = true;
			PointingZones[1].IsEnabled = M.FlagM.GetFlagValue("num_meatcoreA") > 0;
			Group_Cores[1].gameObject.SetActive(PointingZones[1].IsEnabled);
			PointingZones[2].IsEnabled = M.FlagM.GetFlagValue("num_meatcoreB") > 0;
			Group_Cores[2].gameObject.SetActive(PointingZones[2].IsEnabled);
			PointingZones[3].IsEnabled = M.FlagM.GetFlagValue("num_meatcoreC") > 0;
			Group_Cores[3].gameObject.SetActive(PointingZones[3].IsEnabled);
			PointingZones[4].IsEnabled = M.FlagM.GetFlagValue("num_meatcoreD") > 0;
			Group_Cores[4].gameObject.SetActive(PointingZones[4].IsEnabled);
			PointingZones[5].IsEnabled = M.FlagM.GetFlagValue("num_meatcoreE") > 0;
			Group_Cores[5].gameObject.SetActive(PointingZones[5].IsEnabled);
			PointingZones[6].IsEnabled = M.FlagM.GetFlagValue("num_meatcoreF") > 0;
			Group_Cores[6].gameObject.SetActive(PointingZones[6].IsEnabled);
			PointingZones[7].IsEnabled = M.FlagM.GetFlagValue("num_meatcoreG") > 0;
			Group_Cores[7].gameObject.SetActive(PointingZones[7].IsEnabled);
			PointingZones[8].IsEnabled = M.FlagM.GetFlagValue("num_meatcoreH") > 0;
			Group_Cores[8].gameObject.SetActive(PointingZones[8].IsEnabled);
		}

		public void SetState_Fruit()
		{
			Root_Menu.gameObject.SetActive(value: false);
			Root_Cores.gameObject.SetActive(value: false);
			Root_Fruit.gameObject.SetActive(value: true);
			Root_Banger.gameObject.SetActive(value: false);
			PointingZones[0].IsEnabled = true;
			PointingZones[1].IsEnabled = M.FlagM.GetFlagValue("num_herbA") > 0;
			Group_Fruit[1].gameObject.SetActive(PointingZones[1].IsEnabled);
			PointingZones[2].IsEnabled = M.FlagM.GetFlagValue("num_herbB") > 0;
			Group_Fruit[2].gameObject.SetActive(PointingZones[2].IsEnabled);
			PointingZones[3].IsEnabled = M.FlagM.GetFlagValue("num_herbC") > 0;
			Group_Fruit[3].gameObject.SetActive(PointingZones[3].IsEnabled);
			PointingZones[4].IsEnabled = M.FlagM.GetFlagValue("num_herbD") > 0;
			Group_Fruit[4].gameObject.SetActive(PointingZones[4].IsEnabled);
			PointingZones[5].IsEnabled = M.FlagM.GetFlagValue("num_herbE") > 0;
			Group_Fruit[5].gameObject.SetActive(PointingZones[5].IsEnabled);
			for (int i = 6; i < PointingZones.Count; i++)
			{
				PointingZones[i].IsEnabled = false;
			}
		}

		public void SetState_Banger()
		{
			Root_Menu.gameObject.SetActive(value: false);
			Root_Cores.gameObject.SetActive(value: false);
			Root_Fruit.gameObject.SetActive(value: false);
			Root_Banger.gameObject.SetActive(value: true);
			PointingZones[0].IsEnabled = true;
			PointingZones[1].IsEnabled = M.FlagM.GetFlagValue("num_bangerJunk_TinCan_0") > 0 || M.FlagM.GetFlagValue("num_bangerJunk_TinCan_1") > 0 || M.FlagM.GetFlagValue("num_bangerJunk_TinCan_2") > 0;
			Group_Banger[1].gameObject.SetActive(PointingZones[1].IsEnabled);
			PointingZones[2].IsEnabled = M.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_0") > 0 || M.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_1") > 0 || M.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_2") > 0 || M.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_3") > 0;
			Group_Banger[2].gameObject.SetActive(PointingZones[2].IsEnabled);
			PointingZones[3].IsEnabled = M.FlagM.GetFlagValue("num_bangerJunk_Bucket") > 0;
			Group_Banger[3].gameObject.SetActive(PointingZones[3].IsEnabled);
			PointingZones[4].IsEnabled = M.FlagM.GetFlagValue("num_bangerJunk_Bangsnaps") > 0;
			Group_Banger[4].gameObject.SetActive(PointingZones[4].IsEnabled);
			PointingZones[5].IsEnabled = M.FlagM.GetFlagValue("num_bangerJunk_EggTimer") > 0;
			Group_Banger[5].gameObject.SetActive(PointingZones[5].IsEnabled);
			PointingZones[6].IsEnabled = M.FlagM.GetFlagValue("num_bangerJunk_Radio") > 0;
			Group_Banger[6].gameObject.SetActive(PointingZones[6].IsEnabled);
			PointingZones[7].IsEnabled = M.FlagM.GetFlagValue("num_bangerJunk_FishFinder") > 0;
			Group_Banger[7].gameObject.SetActive(PointingZones[7].IsEnabled);
			for (int i = 8; i < PointingZones.Count; i++)
			{
				PointingZones[i].IsEnabled = false;
			}
		}

		public void ClickPoint(int i)
		{
			switch (State)
			{
			case GastroState.Disabled:
				ClickPoint_Disabled(i);
				break;
			case GastroState.Menu:
				ClickPoint_Menu(i);
				break;
			case GastroState.Cores:
				ClickPoint_Cores(i);
				break;
			case GastroState.Fruit:
				ClickPoint_Fruit(i);
				break;
			case GastroState.Banger:
				ClickPoint_Banger(i);
				break;
			}
		}

		private void ClickPoint_Disabled(int i)
		{
			if (i == 0)
			{
				SetState(GastroState.Menu);
			}
		}

		private void ClickPoint_Menu(int i)
		{
			switch (i)
			{
			case 0:
				SetState(GastroState.Disabled);
				break;
			case 1:
				SetState(GastroState.Cores);
				break;
			case 2:
				SetState(GastroState.Fruit);
				break;
			case 3:
				SetState(GastroState.Banger);
				break;
			}
		}

		private void ClickPoint_Cores(int i)
		{
			if (i == 0)
			{
				SetState(GastroState.Menu);
				return;
			}
			M.VomitCore(i - 1);
			SetState_Cores();
		}

		private void ClickPoint_Fruit(int i)
		{
			if (i == 0)
			{
				SetState(GastroState.Menu);
				return;
			}
			M.VomitHerb(i - 1);
			SetState_Fruit();
		}

		private void ClickPoint_Banger(int i)
		{
			if (i == 0)
			{
				SetState(GastroState.Menu);
				return;
			}
			M.VomitBangerJunk(i - 1);
			SetState_Banger();
		}

		private void Update()
		{
			UpdateHealth();
			Vector3 position = GM.CurrentPlayerBody.LeftHand.position + GM.CurrentPlayerBody.LeftHand.forward * -0.2f;
			base.transform.position = position;
		}

		private void UpdateHealth()
		{
			float playerHealth = GM.GetPlayerHealth();
			Color value = Color.Lerp(Health_Empty, Health_Full, playerHealth);
			playerHealth = 0f - playerHealth;
			playerHealth += 0.5f;
			HealthRingRend.material.SetTextureOffset("_MainTex", new Vector2(0f, playerHealth));
			HealthRingRend.material.SetColor("_EmissionColor", value);
			Vector3 forward = Vector3.ProjectOnPlane(GM.CurrentPlayerBody.Head.forward, base.transform.up);
			HealthRingRend.transform.rotation = Quaternion.LookRotation(forward, base.transform.up);
		}
	}
}
