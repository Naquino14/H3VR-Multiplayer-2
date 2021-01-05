using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FXM : ManagerSingleton<FXM>
	{
		public enum FireFXType
		{
			Sosig,
			MolotovWick,
			RW_Nodule
		}

		public GameObject FastPoolManagerPrefab;

		private FastPoolManager m_poolManager;

		public GameObject[] VFX_Impact_Generic;

		public GameObject[] VFX_Impact_Sparks;

		public GameObject[] VFX_Impact_BotMeat;

		public GameObject[] VFX_Impact_RotMeat;

		public GameObject[] VFX_Impact_Water;

		public GameObject[] VFX_Impact_SparksRed;

		public GameObject[] VFX_Impact_SparksGreen;

		public GameObject[] VFX_Impact_SparksBlue;

		public GameObject MuzzleFireLightPrefab;

		private AlloyAreaLight MuzzleFireLight;

		private Light MuzzleFireLightOG;

		private float m_muzzleFireTick;

		public List<GameObject> FIREFX;

		public List<GameObject> ClownModeFX;

		private List<SLAM> m_registeredSLAAMS;

		[Header("BulletHoles")]
		public GameObject[] BulletHolePrefabs_Wood;

		public GameObject[] BulletHolePrefabs_Metal;

		public GameObject[] BulletHolePrefabs_Plaster;

		public GameObject[] BulletHolePrefabs_PlasticRubber;

		public GameObject[] BulletHolePrefabs_Tile;

		public GameObject[] BulletHolePrefabs_Glass;

		public GameObject[] BulletHolePrefabs_Brick;

		public GameObject[] BulletHolePrefabs_Rock;

		public GameObject[] BulletHolePrefabs_SandDirt;

		public GameObject[] BulletHolePrefabs;

		public Material[] BulletHoleMaterials;

		private List<Renderer> m_decals = new List<Renderer>();

		private int m_maxDecalCount = 200;

		private int decalIndex;

		private Dictionary<MuzzleEffectEntry, MuzzleEffectConfig> muzzleDic = new Dictionary<MuzzleEffectEntry, MuzzleEffectConfig>();

		public static List<SLAM> RegisteredSLAAMS => ManagerSingleton<FXM>.Instance.m_registeredSLAAMS;

		public static Dictionary<MuzzleEffectEntry, MuzzleEffectConfig> MuzzleDic => ManagerSingleton<FXM>.Instance.muzzleDic;

		public static void ResetDecals()
		{
			if (ManagerSingleton<FXM>.Instance.m_decals.Count > 0)
			{
				for (int num = ManagerSingleton<FXM>.Instance.m_decals.Count - 1; num >= 0; num--)
				{
					Object.Destroy(ManagerSingleton<FXM>.Instance.m_decals[num]);
				}
			}
			ManagerSingleton<FXM>.Instance.m_decals.Clear();
			ManagerSingleton<FXM>.Instance.decalIndex = 0;
			ManagerSingleton<FXM>.Instance.m_maxDecalCount = GM.Options.SimulationOptions.MaxHitDecals[GM.Options.SimulationOptions.MaxHitDecalIndex];
		}

		public static void SpawnBulletDecal(BulletHoleDecalType t, Vector3 point, Vector3 normal, float damageSize)
		{
			switch (t)
			{
			case BulletHoleDecalType.Wood:
				ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[0], point, normal, damageSize);
				break;
			case BulletHoleDecalType.Metal:
				ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[1], point, normal, damageSize);
				break;
			case BulletHoleDecalType.Plaster:
				ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[2], point, normal, damageSize);
				break;
			case BulletHoleDecalType.PlasticRubber:
				ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[3], point, normal, damageSize);
				break;
			case BulletHoleDecalType.Tile:
				ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[4], point, normal, damageSize);
				break;
			case BulletHoleDecalType.Glass:
				ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[5], point, normal, damageSize);
				break;
			case BulletHoleDecalType.Brick:
				ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[6], point, normal, damageSize);
				break;
			case BulletHoleDecalType.Rock:
				ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[7], point, normal, damageSize);
				break;
			case BulletHoleDecalType.SandDirt:
				ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[8], point, normal, damageSize);
				break;
			case BulletHoleDecalType.GlowBlue:
				ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[9], point, normal, damageSize);
				ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[10], point, normal, damageSize * 0.6f);
				break;
			case BulletHoleDecalType.Singe:
				break;
			}
		}

		private void SpawnBulletDecal(Material m, Vector3 point, Vector3 normal, float damageSize)
		{
			if (m_decals.Count < m_maxDecalCount)
			{
				GameObject gameObject = Object.Instantiate(ManagerSingleton<FXM>.Instance.BulletHolePrefabs[Random.Range(0, ManagerSingleton<FXM>.Instance.BulletHolePrefabs.Length)], point, Quaternion.LookRotation(normal));
				float num = Random.Range(15f, 18f);
				gameObject.transform.localScale = new Vector3(damageSize * num, damageSize * num, damageSize * num);
				Renderer component = gameObject.GetComponent<Renderer>();
				component.material = m;
				ManagerSingleton<FXM>.Instance.m_decals.Add(component);
				return;
			}
			m_decals[decalIndex].material = m;
			m_decals[decalIndex].transform.SetPositionAndRotation(point, Quaternion.LookRotation(normal));
			float num2 = Random.Range(15f, 18f);
			m_decals[decalIndex].transform.localScale = new Vector3(damageSize * num2, damageSize * num2, damageSize * num2);
			decalIndex++;
			if (decalIndex >= m_maxDecalCount)
			{
				decalIndex = 0;
			}
		}

		public static void ClearDecalPools()
		{
			ManagerSingleton<FXM>.Instance.m_decals.Clear();
			ManagerSingleton<FXM>.Instance.decalIndex = 0;
		}

		public static GameObject GetClownFX(int i)
		{
			return ManagerSingleton<FXM>.Instance.ClownModeFX[i];
		}

		protected override void Awake()
		{
			base.Awake();
			CheckForPoolManager();
			GenerateMuzzleFlashLight();
			GenerateMuzzleEffectDictionaries();
			m_registeredSLAAMS = new List<SLAM>();
			m_maxDecalCount = GM.Options.SimulationOptions.MaxHitDecals[GM.Options.SimulationOptions.MaxHitDecalIndex];
		}

		public static MuzzleEffectConfig GetMuzzleConfig(MuzzleEffectEntry entry)
		{
			return MuzzleDic[entry];
		}

		private void GenerateMuzzleEffectDictionaries()
		{
			MuzzleEffectConfig[] array = Resources.LoadAll<MuzzleEffectConfig>("MuzzleEffects");
			for (int i = 0; i < array.Length; i++)
			{
				muzzleDic.Add(array[i].Entry, array[i]);
			}
		}

		public static void DetonateSPAAMS()
		{
			for (int i = 0; i < RegisteredSLAAMS.Count; i++)
			{
				if (RegisteredSLAAMS[i] != null && RegisteredSLAAMS[i].Mode == SLAM.SLAMMode.ThrownArmed)
				{
					RegisteredSLAAMS[i].Invoke("Detonate", 0.1f);
				}
			}
		}

		public static void RegisterSLAM(SLAM s)
		{
			RegisteredSLAAMS.Add(s);
		}

		public static void DeRegisterSLAM(SLAM s)
		{
			RegisteredSLAAMS.Remove(s);
		}

		private void GenerateMuzzleFlashLight()
		{
			if (MuzzleFireLight == null)
			{
				GameObject gameObject = Object.Instantiate(MuzzleFireLightPrefab, Vector3.zero, Quaternion.identity);
				MuzzleFireLight = gameObject.GetComponent<AlloyAreaLight>();
				MuzzleFireLightOG = gameObject.GetComponent<Light>();
				MuzzleFireLight.gameObject.SetActive(value: false);
			}
		}

		public static void Ignite(FVRIgnitable i, float ignitionPower)
		{
			if (i.IsIgniteable() && !(ignitionPower < i.IgnitionThreshold))
			{
				GameObject original = ManagerSingleton<FXM>.Instance.FIREFX[(int)i.FireType];
				Transform spawnPos = i.GetSpawnPos();
				GameObject gameObject = Object.Instantiate(original, spawnPos.position, spawnPos.rotation);
				gameObject.transform.SetParent(spawnPos);
				ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
				component.Emit(1);
				i.Ignite(component);
			}
		}

		private void Update()
		{
			if (m_muzzleFireTick <= 0f && MuzzleFireLight.gameObject.activeSelf)
			{
				MuzzleFireLight.gameObject.SetActive(value: false);
				m_muzzleFireTick = 0f;
			}
			MuzzleFireLight.Intensity = m_muzzleFireTick * 2f;
			if (m_muzzleFireTick > 0f)
			{
				m_muzzleFireTick -= Time.deltaTime * 20f;
			}
		}

		public static void InitiateMuzzleFlashLowPriority(Vector3 pos, Vector3 dir, float intensity, Color col, float rangeMult)
		{
			ManagerSingleton<FXM>.Instance.initiateMuzzleFlashLowPriority(pos, dir, intensity, col, rangeMult);
		}

		private void initiateMuzzleFlashLowPriority(Vector3 pos, Vector3 dir, float intensity, Color col, float rangeMult)
		{
			if (MuzzleFireLight.gameObject.activeInHierarchy && GM.CurrentPlayerBody != null)
			{
				float num = Vector3.Distance(MuzzleFireLight.transform.position, GM.CurrentPlayerBody.Head.transform.position);
				float num2 = Vector3.Distance(pos, GM.CurrentPlayerBody.Head.transform.position);
				if (num2 < num)
				{
					ManagerSingleton<FXM>.Instance.initiateMuzzleFlash(pos, dir, intensity, col, rangeMult);
				}
			}
			else
			{
				ManagerSingleton<FXM>.Instance.initiateMuzzleFlash(pos, dir, intensity, col, rangeMult);
			}
		}

		public static void InitiateMuzzleFlash(Vector3 pos, Vector3 dir, float intensity, Color col, float rangeMult)
		{
			ManagerSingleton<FXM>.Instance.initiateMuzzleFlash(pos, dir, intensity, col, rangeMult);
		}

		private void initiateMuzzleFlash(Vector3 pos, Vector3 dir, float intensity, Color col, float rangeMult)
		{
			MuzzleFireLight.gameObject.SetActive(value: true);
			MuzzleFireLight.transform.position = pos;
			MuzzleFireLight.transform.up = dir;
			m_muzzleFireTick = intensity;
			MuzzleFireLight.Intensity = m_muzzleFireTick * 2f;
			MuzzleFireLight.Color = col;
			MuzzleFireLightOG.range = rangeMult * 5f;
		}

		private void CheckForPoolManager()
		{
			if (m_poolManager == null)
			{
				if (Object.FindObjectOfType<FastPoolManager>() != null)
				{
					m_poolManager = Object.FindObjectOfType<FastPoolManager>();
					return;
				}
				GameObject gameObject = Object.Instantiate(FastPoolManagerPrefab);
				m_poolManager = gameObject.GetComponent<FastPoolManager>();
			}
		}

		private void OnLevelWasLoaded(int i)
		{
			CheckForPoolManager();
			GenerateMuzzleFlashLight();
		}

		public static void SpawnImpactEffect(Vector3 pos, Vector3 lookDir, int mat, ImpactEffectMagnitude mag, bool forwardBack)
		{
			ManagerSingleton<FXM>.Instance.spawnImpactEffect(pos, lookDir, mat, mag, forwardBack);
		}

		private void spawnImpactEffect(Vector3 pos, Vector3 lookDir, int mat, ImpactEffectMagnitude mag, bool forwardBack)
		{
			switch (mat)
			{
			case 0:
				switch (mag)
				{
				case ImpactEffectMagnitude.Tiny:
					VFX_Impact_Generic[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Small:
					VFX_Impact_Generic[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Medium:
					VFX_Impact_Generic[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Large:
					VFX_Impact_Generic[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				}
				break;
			case 1:
				switch (mag)
				{
				case ImpactEffectMagnitude.Tiny:
					VFX_Impact_Sparks[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Small:
					VFX_Impact_Sparks[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_Sparks[1].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 1.5f), Color.white, Random.Range(0.5f, 1f));
					break;
				case ImpactEffectMagnitude.Medium:
					VFX_Impact_Sparks[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_Sparks[2].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 2.5f), Color.white, Random.Range(0.5f, 1.5f));
					break;
				case ImpactEffectMagnitude.Large:
					VFX_Impact_Sparks[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_Sparks[3].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(1.5f, 3.5f), Color.white, Random.Range(1.7f, 2.2f));
					break;
				}
				break;
			case 2:
				switch (mag)
				{
				case ImpactEffectMagnitude.Tiny:
					VFX_Impact_BotMeat[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Small:
					VFX_Impact_BotMeat[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Medium:
					VFX_Impact_BotMeat[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Large:
					VFX_Impact_BotMeat[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				}
				break;
			case 3:
				switch (mag)
				{
				case ImpactEffectMagnitude.Tiny:
					VFX_Impact_Water[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Small:
					VFX_Impact_Water[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Medium:
					VFX_Impact_Water[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Large:
					VFX_Impact_Water[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				}
				break;
			case 4:
				switch (mag)
				{
				case ImpactEffectMagnitude.Tiny:
					VFX_Impact_SparksRed[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Small:
					VFX_Impact_SparksRed[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_SparksRed[1].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 1.5f), new Color(1f, 0.4f, 0.4f, 1f), Random.Range(0.5f, 1f));
					break;
				case ImpactEffectMagnitude.Medium:
					VFX_Impact_SparksRed[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_SparksRed[2].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 2.5f), new Color(1f, 0.4f, 0.4f, 1f), Random.Range(0.5f, 1.5f));
					break;
				case ImpactEffectMagnitude.Large:
					VFX_Impact_SparksRed[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_SparksRed[3].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(1.5f, 3.5f), new Color(1f, 0.4f, 0.4f, 1f), Random.Range(1.7f, 2.2f));
					break;
				}
				break;
			case 5:
				switch (mag)
				{
				case ImpactEffectMagnitude.Tiny:
					VFX_Impact_SparksGreen[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Small:
					VFX_Impact_SparksGreen[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_SparksGreen[1].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 1.5f), new Color(0.4f, 1f, 0.4f, 1f), Random.Range(0.5f, 1f));
					break;
				case ImpactEffectMagnitude.Medium:
					VFX_Impact_SparksGreen[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_SparksGreen[2].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 2.5f), new Color(0.4f, 1f, 0.4f, 1f), Random.Range(0.5f, 1.5f));
					break;
				case ImpactEffectMagnitude.Large:
					VFX_Impact_SparksGreen[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_SparksGreen[3].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(1.5f, 3.5f), new Color(0.4f, 1f, 0.4f, 1f), Random.Range(1.7f, 2.2f));
					break;
				}
				break;
			case 6:
				switch (mag)
				{
				case ImpactEffectMagnitude.Tiny:
					VFX_Impact_SparksBlue[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Small:
					VFX_Impact_SparksBlue[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_SparksBlue[1].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 1.5f), new Color(0.4f, 0.4f, 1f, 1f), Random.Range(0.5f, 1f));
					break;
				case ImpactEffectMagnitude.Medium:
					VFX_Impact_SparksBlue[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_SparksBlue[2].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 2.5f), new Color(0.4f, 0.4f, 1f, 1f), Random.Range(0.5f, 1.5f));
					break;
				case ImpactEffectMagnitude.Large:
					VFX_Impact_SparksBlue[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					if (forwardBack)
					{
						VFX_Impact_SparksBlue[3].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
					}
					initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(1.5f, 3.5f), new Color(0.4f, 0.4f, 1f, 1f), Random.Range(1.7f, 2.2f));
					break;
				}
				break;
			case 7:
				switch (mag)
				{
				case ImpactEffectMagnitude.Tiny:
					VFX_Impact_RotMeat[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Small:
					VFX_Impact_RotMeat[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Medium:
					VFX_Impact_RotMeat[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				case ImpactEffectMagnitude.Large:
					VFX_Impact_RotMeat[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
					break;
				}
				break;
			}
		}
	}
}
