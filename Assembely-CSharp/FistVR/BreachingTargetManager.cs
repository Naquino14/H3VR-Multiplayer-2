using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class BreachingTargetManager : MonoBehaviour
	{
		public GameObject HotDogTargetRef;

		public GameObject IPSCTargetRef;

		public GameObject DecorationRef;

		private GameObject SpawnedDogs;

		private GameObject SpawnedIPSC;

		private GameObject SpawnedDecoration;

		private int IPSCScore;

		private float m_time;

		public Text IPSCScoreText;

		private bool m_isCounting;

		private int numTargetsHit;

		private AudioSource Aud;

		public List<Transform> PosList;

		public List<Transform> PosList_IPSC;

		public List<Transform> PosList_Sosig;

		public ZosigEnemyTemplate Template;

		private List<Sosig> m_spawnedSosigs = new List<Sosig>();

		private void Start()
		{
			Aud = GetComponent<AudioSource>();
			HotDogTargetRef.SetActive(value: false);
			IPSCTargetRef.SetActive(value: false);
			DecorationRef.SetActive(value: false);
			GM.CurrentSceneSettings.SosigKillEvent += CheckIfDeadSosigWasMine;
		}

		private void OnDestroy()
		{
			GM.CurrentSceneSettings.SosigKillEvent -= CheckIfDeadSosigWasMine;
		}

		private void PlayerDied()
		{
			IPSCScore = 0;
			numTargetsHit = 0;
			m_time = 0f;
			m_isCounting = false;
			UpdateScore();
			ResetDecoration();
			ClearHotDogs();
			ClearIPSC();
			ClearSosigs();
		}

		public void ResetDecoration()
		{
			if (SpawnedDecoration != null)
			{
				Object.Destroy(SpawnedDecoration);
			}
			SpawnedDecoration = Object.Instantiate(DecorationRef, Vector3.zero, Quaternion.identity);
			SpawnedDecoration.SetActive(value: true);
		}

		public void InitiateHotDogSequence(bool isRandomPos)
		{
			IPSCScore = 0;
			numTargetsHit = 0;
			m_time = 0f;
			UpdateScore();
			ResetDecoration();
			m_isCounting = false;
			ClearHotDogs();
			ClearIPSC();
			ClearSosigs();
			SpawnHotDogs(isRandomPos);
		}

		public void InitiateIPSCSequence(bool isRandomPos)
		{
			IPSCScore = 0;
			numTargetsHit = 0;
			m_time = 0f;
			UpdateScore();
			ResetDecoration();
			ClearHotDogs();
			ClearIPSC();
			ClearSosigs();
			SpawnIPSC(isRandomPos);
			m_isCounting = true;
		}

		public void InitiateSosigSequence(int num)
		{
			IPSCScore = 0;
			numTargetsHit = 0;
			m_time = 0f;
			UpdateScore();
			ResetDecoration();
			ClearHotDogs();
			ClearIPSC();
			ClearSosigs();
			SpawnSosigs(num);
		}

		private void Update()
		{
			if (m_isCounting)
			{
				m_time += Time.deltaTime;
				UpdateScore();
			}
		}

		private void SpawnHotDogs(bool isRandomPos)
		{
			SpawnedDogs = Object.Instantiate(HotDogTargetRef, Vector3.zero, Quaternion.identity);
			SpawnedDogs.SetActive(value: true);
			if (isRandomPos)
			{
				BreachingTransformGroup component = SpawnedDogs.GetComponent<BreachingTransformGroup>();
				PosList.Shuffle();
				PosList.Shuffle();
				PosList.Shuffle();
				for (int i = 0; i < component.Set.Count; i++)
				{
					component.Set[i].position = PosList[i].position;
					component.Set[i].rotation = PosList[i].rotation;
				}
			}
		}

		private void SpawnIPSC(bool isRandomPos)
		{
			SpawnedIPSC = Object.Instantiate(IPSCTargetRef, Vector3.zero, Quaternion.identity);
			SpawnedIPSC.SetActive(value: true);
			if (isRandomPos)
			{
				BreachingTransformGroup component = SpawnedIPSC.GetComponent<BreachingTransformGroup>();
				PosList_IPSC.Shuffle();
				PosList_IPSC.Shuffle();
				PosList_IPSC.Shuffle();
				for (int i = 0; i < component.Set.Count; i++)
				{
					component.Set[i].position = PosList_IPSC[i].position;
					component.Set[i].rotation = PosList_IPSC[i].rotation;
				}
			}
		}

		private void ClearHotDogs()
		{
			if (SpawnedDogs != null)
			{
				Object.Destroy(SpawnedDogs);
			}
		}

		private void ClearIPSC()
		{
			if (SpawnedIPSC != null)
			{
				Object.Destroy(SpawnedIPSC);
			}
		}

		public void RegisterScore(int i)
		{
			IPSCScore += i;
			numTargetsHit++;
			if (numTargetsHit >= 20)
			{
				m_isCounting = false;
				Aud.Play();
			}
			UpdateScore();
		}

		private void UpdateScore()
		{
			IPSCScoreText.text = "IPSC Target Score: " + IPSCScore + "/100";
			Text iPSCScoreText = IPSCScoreText;
			iPSCScoreText.text = iPSCScoreText.text + "\nTime: " + FloatToTime(m_time, "#0:00.00");
			float num = Mathf.Round(m_time * 100f);
			num *= 0.01f;
			num += (float)(100 - IPSCScore);
			Text iPSCScoreText2 = IPSCScoreText;
			iPSCScoreText2.text = iPSCScoreText2.text + "\nFinal Score: " + FloatToTime(num, "#0:00.00");
		}

		public string FloatToTime(float toConvert, string format)
		{
			return format switch
			{
				"00.0" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"#0.0" => $"{Mathf.Floor(toConvert) % 60f:#0}:{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"00.00" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"00.000" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#00.000" => $"{Mathf.Floor(toConvert) % 60f:#00}:{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#0:00" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}", 
				"#00:00" => $"{Mathf.Floor(toConvert / 60f):#00}:{Mathf.Floor(toConvert) % 60f:00}", 
				"0:00.0" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"#0:00.0" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"0:00.00" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"#0:00.00" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"0:00.000" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#0:00.000" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				_ => "error", 
			};
		}

		private void SpawnSosigs(int num)
		{
			PosList_Sosig.Shuffle();
			PosList_Sosig.Shuffle();
			PosList_Sosig.Shuffle();
			for (int i = 0; i < num; i++)
			{
				SpawnEnemy(Template, PosList_Sosig[i], 1);
			}
		}

		private void ClearSosigs()
		{
			for (int i = 0; i < m_spawnedSosigs.Count; i++)
			{
				if (m_spawnedSosigs[i] != null)
				{
					m_spawnedSosigs[i].TickDownToClear(0.1f);
				}
			}
			m_spawnedSosigs.Clear();
		}

		private void SpawnEnemy(ZosigEnemyTemplate t, Transform point, int IFF)
		{
			Sosig sosig = SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)], t.WeaponOptions[Random.Range(0, t.WeaponOptions.Count)].GetGameObject(), point.position, point.rotation, t.ConfigTemplates[Random.Range(0, t.ConfigTemplates.Count)], t.WearableTemplates[Random.Range(0, t.WearableTemplates.Count)], IFF);
			sosig.CommandGuardPoint(point.position, hardguard: false);
			Vector3 onUnitSphere = Random.onUnitSphere;
			onUnitSphere.y = 0f;
			onUnitSphere.Normalize();
			sosig.SetDominantGuardDirection(onUnitSphere);
			m_spawnedSosigs.Add(sosig);
		}

		private Sosig SpawnEnemySosig(GameObject prefab, GameObject weaponPrefab, Vector3 pos, Quaternion rot, SosigConfigTemplate t, SosigWearableConfig w, int IFF)
		{
			GameObject gameObject = Object.Instantiate(prefab, pos, rot);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			componentInChildren.Configure(t);
			componentInChildren.Inventory.FillAllAmmo();
			componentInChildren.E.IFFCode = IFF;
			SosigWeapon component = Object.Instantiate(weaponPrefab, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
			component.SetAutoDestroy(b: true);
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
			if (component != null)
			{
				componentInChildren.InitHands();
				componentInChildren.ForceEquip(component);
			}
			return componentInChildren;
		}

		private void SpawnAccesoryToLink(List<GameObject> gs, SosigLink l)
		{
			GameObject gameObject = Object.Instantiate(gs[Random.Range(0, gs.Count)], l.transform.position, l.transform.rotation);
			gameObject.transform.SetParent(l.transform);
			SosigWearable component = gameObject.GetComponent<SosigWearable>();
			component.RegisterWearable(l);
		}

		public void CheckIfDeadSosigWasMine(Sosig s)
		{
			if (m_spawnedSosigs.Count >= 1 && m_spawnedSosigs.Contains(s))
			{
				s.TickDownToClear(15f);
				m_spawnedSosigs.Remove(s);
			}
		}
	}
}
