using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigGameManager : MonoBehaviour
	{
		public ZosigSpawnManager SpawnM;

		public ZosigFlagManager FlagM;

		public ZosigMusicController MusicController;

		public TranslocatorManager TranslocatorM;

		public List<Texture2D> NPCSpeechIcons;

		public bool IsVerboseDebug = true;

		public List<FVRObject> MeatCorePrefabs;

		public List<FVRObject> HerbPrefabs;

		public List<FVRObject> Banger_TinCanPrefabs;

		public List<FVRObject> Banger_CoffeeCanPrefabs;

		public List<FVRObject> Banger_BucketPrefabs;

		public List<FVRObject> Banger_MechanismPrefabs;

		public AudioEvent AudEvent_DryHeave;

		public AudioEvent AudEvent_Vomit;

		public ParticleSystem PSystem_Vomit;

		public GameObject SplodePrefab;

		private ZosigGastroCycler gcycler;

		public GameObject IS;

		public GameObject DisableOnNoIntro;

		public ZosigLemonManager LemonManager;

		private float m_tickToSave = 60f;

		private float curMusicVolume = 0.22f;

		private float tarMusicVolume = 0.22f;

		private float m_VomitRefire = 1f;

		private Vector3 lastHeadPos;

		public void SetGCycler(ZosigGastroCycler c)
		{
			gcycler = c;
		}

		private void Awake()
		{
			GM.ZMaster = this;
		}

		public void QUIT()
		{
			GM.ZMaster.FlagM.Save();
		}

		private void Start()
		{
			FlagM.Init();
			LemonManager.InitLemons();
			if (FlagM.GetFlagValue("flag_Difficulty") == 1)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(10000f);
				GM.CurrentSceneSettings.IsSpawnLockingEnabled = true;
				IS.SetActive(value: true);
				GM.CurrentSceneSettings.UsesMaxSpeedClamp = false;
				GM.CurrentSceneSettings.DoesTeleportUseCooldown = false;
			}
			else if (FlagM.GetFlagValue("flag_Difficulty") == 2)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(2500f);
				FlagM.SetFlag("num_meatcoreA", 0);
				FlagM.SetFlag("num_meatcoreB", 0);
				FlagM.SetFlag("num_meatcoreC", 0);
				FlagM.SetFlag("num_meatcoreD", 0);
				FlagM.SetFlag("num_meatcoreE", 0);
				FlagM.SetFlag("num_meatcoreF", 0);
				FlagM.SetFlag("num_meatcoreG", 0);
				FlagM.SetFlag("num_meatcoreH", 0);
				FlagM.SetFlag("num_herbA", 0);
				FlagM.SetFlag("num_herbB", 0);
				FlagM.SetFlag("num_herbC", 0);
				FlagM.SetFlag("num_herbD", 0);
				FlagM.SetFlag("num_herbE", 0);
				FlagM.SetFlag("num_bangerJunk_TinCan_0", 0);
				FlagM.SetFlag("num_bangerJunk_TinCan_1", 0);
				FlagM.SetFlag("num_bangerJunk_TinCan_2", 0);
				FlagM.SetFlag("num_bangerJunk_CoffeeCan_0", 0);
				FlagM.SetFlag("num_bangerJunk_CoffeeCan_1", 0);
				FlagM.SetFlag("num_bangerJunk_CoffeeCan_2", 0);
				FlagM.SetFlag("num_bangerJunk_CoffeeCan_3", 0);
				FlagM.SetFlag("num_bangerJunk_Bucket", 0);
				FlagM.SetFlag("num_bangerJunk_Bangsnaps", 0);
				FlagM.SetFlag("num_bangerJunk_EggTimer", 0);
				FlagM.SetFlag("num_bangerJunk_Radio", 0);
				FlagM.SetFlag("num_bangerJunk_FishFinder", 0);
			}
			if (FlagM.GetFlagValue("skip_intro") > 0 || FlagM.GetNumEntries() > 2)
			{
				Debug.Log("skip_intro:" + FlagM.GetFlagValue("skip_intro"));
				Debug.Log("num entries:" + FlagM.GetNumEntries());
				FlagM.PrintAll();
				FlagM.SetFlagMaxBlend("npc00_quest", 13);
				FlagM.SetFlagMaxBlend("quest00_final_state", 1);
				DisableOnNoIntro.SetActive(value: false);
				if (FlagM.GetFlagValue("skip_intro") > 0 || !FlagM.ContainsKey("skip_intro"))
				{
					FlagM.SetFlagMaxBlend("npc00_introduction", 2);
				}
			}
			TranslocatorM.Init(FlagM);
			SpawnM.Init();
			ZosigQuestManager[] array = Object.FindObjectsOfType<ZosigQuestManager>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Init(this);
			}
			MusicController.Init();
			MusicController.SetMasterVolume(0.22f);
			SetMusicTrack(ZosigMusicController.ZosigTrackName.HBH);
			Vector3 position = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.1f;
			PSystem_Vomit.transform.SetParent(GM.CurrentPlayerBody.Head);
			PSystem_Vomit.transform.position = position;
			PSystem_Vomit.transform.rotation = Quaternion.LookRotation(GM.CurrentPlayerBody.Head.forward);
		}

		private void Update()
		{
			MusicController.Tick(Time.deltaTime);
			CheckVomit();
			lastHeadPos = GM.CurrentPlayerBody.Head.position;
			m_tickToSave -= Time.deltaTime;
			if (m_tickToSave <= 0f)
			{
				m_tickToSave = 60f;
				GM.ZMaster.FlagM.Save();
			}
			if (DisableOnNoIntro.activeSelf && FlagM.GetFlagValue("quest00_final_state") > 0)
			{
				DisableOnNoIntro.SetActive(value: false);
			}
			curMusicVolume = Mathf.MoveTowards(curMusicVolume, tarMusicVolume, Time.deltaTime * 1f);
			MusicController.SetMasterVolume(curMusicVolume);
		}

		public void SetMusic_Speaking()
		{
			tarMusicVolume = 0.12f;
		}

		public void SetMusic_Gameplay()
		{
			tarMusicVolume = 0.22f;
		}

		public void SetMusicTrack(ZosigMusicController.ZosigTrackName track)
		{
			MusicController.SwitchToTrack(track);
		}

		public FVRObject GetRandomEquippedFirearm()
		{
			List<FVRObject> list = new List<FVRObject>();
			FVRInteractiveObject currentInteractable = GM.CurrentMovementManager.Hands[0].CurrentInteractable;
			if (currentInteractable is FVRFireArm && (currentInteractable as FVRFireArm).ObjectWrapper != null)
			{
				list.Add((currentInteractable as FVRFireArm).ObjectWrapper);
			}
			for (int i = 0; i < GM.CurrentPlayerBody.QuickbeltSlots.Count; i++)
			{
				if (GM.CurrentPlayerBody.QuickbeltSlots[i].CurObject is FVRFireArm && (GM.CurrentPlayerBody.QuickbeltSlots[i].CurObject as FVRFireArm).ObjectWrapper != null)
				{
					list.Add((GM.CurrentPlayerBody.QuickbeltSlots[i].CurObject as FVRFireArm).ObjectWrapper);
				}
			}
			if (list.Count > 0)
			{
				return list[Random.Range(0, list.Count)];
			}
			return null;
		}

		public void EatMeatCore(RotrwMeatCore.CoreType t)
		{
			switch (t)
			{
			case RotrwMeatCore.CoreType.Tasty:
				FlagM.AddToFlag("num_meatcoreA", 1);
				break;
			case RotrwMeatCore.CoreType.Moldy:
				FlagM.AddToFlag("num_meatcoreB", 1);
				break;
			case RotrwMeatCore.CoreType.Spikey:
				FlagM.AddToFlag("num_meatcoreC", 1);
				break;
			case RotrwMeatCore.CoreType.Zippy:
				FlagM.AddToFlag("num_meatcoreD", 1);
				break;
			case RotrwMeatCore.CoreType.Weighty:
				FlagM.AddToFlag("num_meatcoreE", 1);
				break;
			case RotrwMeatCore.CoreType.Juicy:
				FlagM.AddToFlag("num_meatcoreF", 1);
				break;
			case RotrwMeatCore.CoreType.Shiny:
				FlagM.AddToFlag("num_meatcoreG", 1);
				break;
			case RotrwMeatCore.CoreType.Burny:
				FlagM.AddToFlag("num_meatcoreH", 1);
				break;
			}
			gcycler.UpdateState();
		}

		public void EatHerb(RotrwHerb.HerbType type)
		{
			switch (type)
			{
			case RotrwHerb.HerbType.KatchupLeaf:
				FlagM.AddToFlag("num_herbA", 1);
				break;
			case RotrwHerb.HerbType.MustardWillow:
				FlagM.AddToFlag("num_herbB", 1);
				break;
			case RotrwHerb.HerbType.PricklyPickle:
				FlagM.AddToFlag("num_herbC", 1);
				break;
			case RotrwHerb.HerbType.GiantBlueRaspberry:
				FlagM.AddToFlag("num_herbD", 1);
				break;
			case RotrwHerb.HerbType.DeadlyEggplant:
				FlagM.AddToFlag("num_herbE", 1);
				break;
			}
			gcycler.UpdateState();
		}

		public void EatBangerJunk(RotrwBangerJunk.BangerJunkType type, int matIndex)
		{
			switch (type)
			{
			case RotrwBangerJunk.BangerJunkType.TinCan:
				switch (matIndex)
				{
				case 0:
					FlagM.AddToFlag("num_bangerJunk_TinCan_0", 1);
					break;
				case 1:
					FlagM.AddToFlag("num_bangerJunk_TinCan_1", 1);
					break;
				case 2:
					FlagM.AddToFlag("num_bangerJunk_TinCan_2", 1);
					FlagM.SetFlagMaxBlend("npc00_quest", 3);
					break;
				}
				break;
			case RotrwBangerJunk.BangerJunkType.CoffeeCan:
				switch (matIndex)
				{
				case 0:
					FlagM.AddToFlag("num_bangerJunk_CoffeeCan_0", 1);
					break;
				case 1:
					FlagM.AddToFlag("num_bangerJunk_CoffeeCan_1", 1);
					break;
				case 2:
					FlagM.AddToFlag("num_bangerJunk_CoffeeCan_2", 1);
					break;
				case 3:
					FlagM.AddToFlag("num_bangerJunk_CoffeeCan_3", 1);
					break;
				}
				break;
			case RotrwBangerJunk.BangerJunkType.Bucket:
				FlagM.AddToFlag("num_bangerJunk_Bucket", 1);
				break;
			case RotrwBangerJunk.BangerJunkType.BangSnaps:
				FlagM.AddToFlag("num_bangerJunk_Bangsnaps", 1);
				break;
			case RotrwBangerJunk.BangerJunkType.EggTimer:
				FlagM.AddToFlag("num_bangerJunk_EggTimer", 1);
				break;
			case RotrwBangerJunk.BangerJunkType.Radio:
				FlagM.AddToFlag("num_bangerJunk_Radio", 1);
				break;
			case RotrwBangerJunk.BangerJunkType.FishFinder:
				FlagM.AddToFlag("num_bangerJunk_FishFinder", 1);
				break;
			}
			gcycler.UpdateState();
		}

		public void CheckVomit()
		{
			if (m_VomitRefire > 0f)
			{
				m_VomitRefire -= Time.deltaTime;
				return;
			}
			float num = Vector3.Angle(GM.CurrentPlayerBody.Head.forward, -Vector3.up);
			if (num > 45f)
			{
				return;
			}
			Vector3 a = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.1f;
			for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
			{
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable != null)
				{
					return;
				}
				float num2 = Vector3.Distance(a, GM.CurrentMovementManager.Hands[i].transform.position);
				if (num2 > 0.3f)
				{
					return;
				}
			}
			for (int j = 0; j < GM.CurrentMovementManager.Hands.Length; j++)
			{
				Vector3 velLinearWorld = GM.CurrentMovementManager.Hands[j].Input.VelLinearWorld;
				if (velLinearWorld.magnitude < 1f)
				{
					return;
				}
				float num3 = Vector3.Angle(velLinearWorld, GM.CurrentPlayerBody.Head.forward);
				if (num3 > 45f)
				{
					return;
				}
			}
			Vector3 from = GM.CurrentPlayerBody.Head.position - lastHeadPos;
			float num4 = Vector3.Angle(from, -Vector3.up);
			if (!(num4 > 45f))
			{
				VomitRandomThing();
			}
		}

		public bool VomitCore(int i)
		{
			switch (i)
			{
			case 0:
				if (FlagM.GetFlagValue("num_meatcoreA") > 0)
				{
					FlagM.SubstractFromFlag("num_meatcoreA", 1);
					return VomitObject(MeatCorePrefabs[0]);
				}
				break;
			case 1:
				if (FlagM.GetFlagValue("num_meatcoreB") > 0)
				{
					FlagM.SubstractFromFlag("num_meatcoreB", 1);
					return VomitObject(MeatCorePrefabs[1]);
				}
				break;
			case 2:
				if (FlagM.GetFlagValue("num_meatcoreC") > 0)
				{
					FlagM.SubstractFromFlag("num_meatcoreC", 1);
					return VomitObject(MeatCorePrefabs[2]);
				}
				break;
			case 3:
				if (FlagM.GetFlagValue("num_meatcoreD") > 0)
				{
					FlagM.SubstractFromFlag("num_meatcoreD", 1);
					return VomitObject(MeatCorePrefabs[3]);
				}
				break;
			case 4:
				if (FlagM.GetFlagValue("num_meatcoreE") > 0)
				{
					FlagM.SubstractFromFlag("num_meatcoreE", 1);
					return VomitObject(MeatCorePrefabs[4]);
				}
				break;
			case 5:
				if (FlagM.GetFlagValue("num_meatcoreF") > 0)
				{
					FlagM.SubstractFromFlag("num_meatcoreF", 1);
					return VomitObject(MeatCorePrefabs[5]);
				}
				break;
			case 6:
				if (FlagM.GetFlagValue("num_meatcoreG") > 0)
				{
					FlagM.SubstractFromFlag("num_meatcoreG", 1);
					return VomitObject(MeatCorePrefabs[6]);
				}
				break;
			case 7:
				if (FlagM.GetFlagValue("num_meatcoreH") > 0)
				{
					FlagM.SubstractFromFlag("num_meatcoreH", 1);
					return VomitObject(MeatCorePrefabs[7]);
				}
				break;
			}
			return false;
		}

		public bool VomitHerb(int i)
		{
			switch (i)
			{
			case 0:
				if (FlagM.GetFlagValue("num_herbA") > 0)
				{
					FlagM.SubstractFromFlag("num_herbA", 1);
					return VomitObject(HerbPrefabs[0]);
				}
				break;
			case 1:
				if (FlagM.GetFlagValue("num_herbB") > 0)
				{
					FlagM.SubstractFromFlag("num_herbB", 1);
					return VomitObject(HerbPrefabs[1]);
				}
				break;
			case 2:
				if (FlagM.GetFlagValue("num_herbC") > 0)
				{
					FlagM.SubstractFromFlag("num_herbC", 1);
					return VomitObject(HerbPrefabs[2]);
				}
				break;
			case 3:
				if (FlagM.GetFlagValue("num_herbD") > 0)
				{
					FlagM.SubstractFromFlag("num_herbD", 1);
					return VomitObject(HerbPrefabs[3]);
				}
				break;
			case 4:
				if (FlagM.GetFlagValue("num_herbE") > 0)
				{
					FlagM.SubstractFromFlag("num_herbE", 1);
					return VomitObject(HerbPrefabs[4]);
				}
				break;
			}
			return false;
		}

		public bool VomitBangerJunk(int i)
		{
			switch (i)
			{
			case 0:
				if (FlagM.GetFlagValue("num_bangerJunk_TinCan_0") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_TinCan_0", 1);
					return VomitObject(Banger_TinCanPrefabs[0]);
				}
				if (FlagM.GetFlagValue("num_bangerJunk_TinCan_1") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_TinCan_1", 1);
					return VomitObject(Banger_TinCanPrefabs[1]);
				}
				if (FlagM.GetFlagValue("num_bangerJunk_TinCan_2") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_TinCan_2", 1);
					return VomitObject(Banger_TinCanPrefabs[2]);
				}
				break;
			case 1:
				if (FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_0") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_CoffeeCan_0", 1);
					return VomitObject(Banger_CoffeeCanPrefabs[0]);
				}
				if (FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_1") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_CoffeeCan_1", 1);
					return VomitObject(Banger_CoffeeCanPrefabs[1]);
				}
				if (FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_2") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_CoffeeCan_2", 1);
					return VomitObject(Banger_CoffeeCanPrefabs[2]);
				}
				if (FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_3") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_CoffeeCan_3", 1);
					return VomitObject(Banger_CoffeeCanPrefabs[3]);
				}
				break;
			case 2:
				if (FlagM.GetFlagValue("num_bangerJunk_Bucket") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_Bucket", 1);
					return VomitObject(Banger_BucketPrefabs[0]);
				}
				break;
			case 3:
				if (FlagM.GetFlagValue("num_bangerJunk_Bangsnaps") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_Bangsnaps", 1);
					return VomitObject(Banger_MechanismPrefabs[0]);
				}
				break;
			case 4:
				if (FlagM.GetFlagValue("num_bangerJunk_EggTimer") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_EggTimer", 1);
					return VomitObject(Banger_MechanismPrefabs[1]);
				}
				break;
			case 5:
				if (FlagM.GetFlagValue("num_bangerJunk_Radio") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_Radio", 1);
					return VomitObject(Banger_MechanismPrefabs[2]);
				}
				break;
			case 6:
				if (FlagM.GetFlagValue("num_bangerJunk_FishFinder") > 0)
				{
					FlagM.SubstractFromFlag("num_bangerJunk_FishFinder", 1);
					return VomitObject(Banger_MechanismPrefabs[3]);
				}
				break;
			}
			return false;
		}

		private bool VomitObject(FVRObject o)
		{
			Vector3 vector = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f + GM.CurrentPlayerBody.Head.transform.forward * 0.15f;
			SM.PlayGenericSound(AudEvent_Vomit, vector);
			PSystem_Vomit.Emit(10);
			GameObject gameObject = Object.Instantiate(o.GetGameObject(), vector + Random.onUnitSphere * 0.1f, Random.rotation);
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			component.velocity = GM.CurrentPlayerBody.Head.forward * Random.Range(1f, 3f);
			return true;
		}

		public void VomitRandomThing()
		{
			bool flag = false;
			if (!flag)
			{
				for (int i = 0; i < 7; i++)
				{
					if (VomitBangerJunk(i))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				for (int j = 0; j < 5; j++)
				{
					if (VomitHerb(j))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				return;
			}
			for (int k = 0; k < 8; k++)
			{
				if (VomitCore(k))
				{
					flag = true;
					break;
				}
			}
		}
	}
}
