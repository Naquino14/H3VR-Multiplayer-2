using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MeatmasAdventBox : MonoBehaviour
	{
		public MM_MusicManager MM;

		public Transform Cover;

		public AudioEvent AudEvent_Music;

		public GameObject MeatmasFetti;

		public GameObject Blocker;

		public GameObject TextInfo;

		private bool m_isOpening;

		private bool m_isOpened;

		private float m_openLerp;

		public ZosigEnemyTemplate ElfWiener;

		public Transform ElfPoint1;

		public Transform ElfPoint2;

		public List<Transform> SpawnPoints;

		public List<FVRObject> ObjectsToSpawn;

		private bool m_shouldRestartMusic;

		private float HeadJitterTick = 0.1f;

		private List<Sosig> m_civvieSosigs = new List<Sosig>();

		public void Open()
		{
			if (!m_isOpening && !m_isOpened)
			{
				m_isOpening = true;
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Music, MeatmasFetti.transform.position);
				MeatmasFetti.SetActive(value: true);
				Blocker.SetActive(value: false);
				TextInfo.SetActive(value: true);
				for (int i = 0; i < ObjectsToSpawn.Count; i++)
				{
					Object.Instantiate(ObjectsToSpawn[i].GetGameObject(), SpawnPoints[i].position, SpawnPoints[i].rotation);
				}
				SpawnEnemy(ElfWiener, ElfPoint1, 0);
				SpawnEnemy(ElfWiener, ElfPoint2, 0);
				if (MM.IsMusicEnabled())
				{
					m_shouldRestartMusic = true;
					MM.DisableMusic();
				}
			}
		}

		public void Update()
		{
			if (!m_isOpening)
			{
				return;
			}
			m_openLerp += Time.deltaTime * 0.078f;
			if (m_openLerp > 1f)
			{
				m_openLerp = 1f;
				m_isOpening = false;
				m_isOpened = true;
				if (m_shouldRestartMusic)
				{
					MM.EnableMusic();
					m_shouldRestartMusic = false;
				}
				for (int i = 0; i < m_civvieSosigs.Count; i++)
				{
					m_civvieSosigs[i].ClearSosig();
				}
				m_civvieSosigs.Clear();
			}
			else if (HeadJitterTick > 0f)
			{
				HeadJitterTick -= Time.deltaTime;
			}
			else
			{
				HeadJitterTick = Random.Range(0.05f, 0.2f);
				for (int j = 0; j < m_civvieSosigs.Count; j++)
				{
					if (m_civvieSosigs != null && m_civvieSosigs[j].Links[0] != null)
					{
						m_civvieSosigs[j].Links[0].R.AddForceAtPosition(Random.onUnitSphere * Random.Range(0.5f, 1.6f), m_civvieSosigs[j].Links[0].transform.position + Vector3.up * 0.3f, ForceMode.Impulse);
					}
				}
			}
			Cover.localPosition = new Vector3(0f, m_openLerp, 0f);
		}

		private void SpawnEnemy(ZosigEnemyTemplate t, Transform point, int IFF)
		{
			GameObject weaponPrefab = null;
			if (t.WeaponOptions.Count > 0)
			{
				weaponPrefab = t.WeaponOptions[Random.Range(0, t.WeaponOptions.Count)].GetGameObject();
			}
			GameObject weaponPrefab2 = null;
			if (t.WeaponOptions_Secondary.Count > 0)
			{
				weaponPrefab2 = t.WeaponOptions_Secondary[Random.Range(0, t.WeaponOptions_Secondary.Count)].GetGameObject();
			}
			Sosig item = SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)], weaponPrefab, weaponPrefab2, point.position, point.rotation, t.ConfigTemplates[Random.Range(0, t.ConfigTemplates.Count)], t.WearableTemplates[Random.Range(0, t.WearableTemplates.Count)], IFF);
			m_civvieSosigs.Add(item);
		}

		private Sosig SpawnEnemySosig(GameObject prefab, GameObject weaponPrefab, GameObject weaponPrefab2, Vector3 pos, Quaternion rot, SosigConfigTemplate t, SosigWearableConfig w, int IFF)
		{
			GameObject gameObject = Object.Instantiate(prefab, pos, rot);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			componentInChildren.Configure(t);
			componentInChildren.E.IFFCode = IFF;
			if (weaponPrefab != null)
			{
				SosigWeapon component = Object.Instantiate(weaponPrefab, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
				component.SetAutoDestroy(b: true);
				if (component.Type == SosigWeapon.SosigWeaponType.Gun)
				{
					component.FlightVelocityMultiplier = 0.15f;
					componentInChildren.Inventory.FillAmmoWithType(component.AmmoType);
				}
				componentInChildren.Inventory.FillAllAmmo();
				if (component != null)
				{
					componentInChildren.InitHands();
					componentInChildren.ForceEquip(component);
				}
				if (weaponPrefab2 != null)
				{
					SosigWeapon component2 = Object.Instantiate(weaponPrefab2, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
					component2.SetAutoDestroy(b: true);
					if (component2.Type == SosigWeapon.SosigWeaponType.Gun)
					{
						component2.FlightVelocityMultiplier = 0.15f;
						componentInChildren.Inventory.FillAmmoWithType(component2.AmmoType);
					}
					if (component2 != null)
					{
						componentInChildren.ForceEquip(component2);
					}
				}
			}
			float num = 0f;
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Headwear)
			{
				SpawnAccesoryToLink(w.Headwear, componentInChildren.Links[0]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Facewear)
			{
				SpawnAccesoryToLink(w.Facewear, componentInChildren.Links[0]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Torsowear)
			{
				SpawnAccesoryToLink(w.Torsowear, componentInChildren.Links[1]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Pantswear)
			{
				SpawnAccesoryToLink(w.Pantswear, componentInChildren.Links[2]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Pantswear_Lower)
			{
				SpawnAccesoryToLink(w.Pantswear_Lower, componentInChildren.Links[3]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Backpacks)
			{
				SpawnAccesoryToLink(w.Backpacks, componentInChildren.Links[1]);
			}
			if (t.UsesLinkSpawns)
			{
				for (int i = 0; i < componentInChildren.Links.Count; i++)
				{
					float num2 = Random.Range(0f, 1f);
					if (num2 < t.LinkSpawnChance[i])
					{
						componentInChildren.Links[i].RegisterSpawnOnDestroy(t.LinkSpawns[i]);
					}
				}
			}
			componentInChildren.CurrentOrder = Sosig.SosigOrder.Disabled;
			componentInChildren.FallbackOrder = Sosig.SosigOrder.Disabled;
			componentInChildren.SetDominantGuardDirection(base.transform.forward);
			return componentInChildren;
		}

		private void SpawnAccesoryToLink(List<GameObject> gs, SosigLink l)
		{
			GameObject gameObject = Object.Instantiate(gs[Random.Range(0, gs.Count)], l.transform.position, l.transform.rotation);
			gameObject.transform.SetParent(l.transform);
			SosigWearable component = gameObject.GetComponent<SosigWearable>();
			component.RegisterWearable(l);
		}
	}
}
