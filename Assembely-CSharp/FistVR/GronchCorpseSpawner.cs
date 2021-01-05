using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class GronchCorpseSpawner : MonoBehaviour
	{
		public List<SosigEnemyTemplate> CorpseList;

		private bool m_isSpawning;

		private float m_timeTilSpawn = 4f;

		private Vector2 TickRange = new Vector2(3f, 10f);

		private GronchJobManager m_M;

		private List<Sosig> m_sosigs = new List<Sosig>();

		public void BeginJob(GronchJobManager m)
		{
			m_M = m;
			m_isSpawning = true;
		}

		public void EndJob(GronchJobManager m)
		{
			m_M = null;
			m_isSpawning = false;
			for (int num = m_sosigs.Count - 1; num >= 0; num--)
			{
				if (m_sosigs[num] != null)
				{
					m_sosigs[num].ClearSosig();
				}
			}
			m_sosigs.Clear();
		}

		private void Update()
		{
			if (m_isSpawning)
			{
				m_timeTilSpawn -= Time.deltaTime;
				if (m_timeTilSpawn <= 0f)
				{
					m_timeTilSpawn = Random.Range(TickRange.x, TickRange.y);
					SpawnEnemy(CorpseList[Random.Range(0, CorpseList.Count)], base.transform, 0, IsCivvie: false);
				}
			}
		}

		private void SpawnEnemy(SosigEnemyTemplate t, Transform point, int IFF, bool IsCivvie)
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
			Sosig sosig = SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)].GetGameObject(), weaponPrefab, weaponPrefab2, point.position, point.rotation, t.ConfigTemplates[Random.Range(0, t.ConfigTemplates.Count)], t.OutfitConfig[Random.Range(0, t.OutfitConfig.Count)], IFF);
			sosig.KillSosig();
			sosig.BreakJoint(sosig.Links[0], isStart: true, Damage.DamageClass.Abstract);
			sosig.BreakJoint(sosig.Links[1], isStart: true, Damage.DamageClass.Abstract);
			sosig.BreakJoint(sosig.Links[2], isStart: true, Damage.DamageClass.Abstract);
			sosig.BreakJoint(sosig.Links[3], isStart: true, Damage.DamageClass.Abstract);
			AudioImpactController audioImpactController = sosig.Links[0].gameObject.AddComponent<AudioImpactController>();
			AudioImpactController audioImpactController2 = sosig.Links[1].gameObject.AddComponent<AudioImpactController>();
			AudioImpactController audioImpactController3 = sosig.Links[2].gameObject.AddComponent<AudioImpactController>();
			AudioImpactController audioImpactController4 = sosig.Links[3].gameObject.AddComponent<AudioImpactController>();
			audioImpactController.ImpactType = ImpactType.MeatChunk;
			audioImpactController2.ImpactType = ImpactType.MeatChunk;
			audioImpactController3.ImpactType = ImpactType.MeatChunk;
			audioImpactController4.ImpactType = ImpactType.MeatChunk;
			m_sosigs.Add(sosig);
		}

		private Sosig SpawnEnemySosig(GameObject prefab, GameObject weaponPrefab, GameObject weaponPrefab2, Vector3 pos, Quaternion rot, SosigConfigTemplate t, SosigOutfitConfig w, int IFF)
		{
			GameObject gameObject = Object.Instantiate(prefab, pos, rot);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			componentInChildren.Configure(t);
			componentInChildren.E.IFFCode = IFF;
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
			componentInChildren.CurrentOrder = Sosig.SosigOrder.Disabled;
			componentInChildren.FallbackOrder = Sosig.SosigOrder.Disabled;
			return componentInChildren;
		}

		private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
		{
			GameObject gameObject = Object.Instantiate(gs[Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
			gameObject.transform.SetParent(l.transform);
			SosigWearable component = gameObject.GetComponent<SosigWearable>();
			component.RegisterWearable(l);
		}
	}
}
