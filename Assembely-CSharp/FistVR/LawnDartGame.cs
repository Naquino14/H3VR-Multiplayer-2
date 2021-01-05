using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class LawnDartGame : MonoBehaviour
	{
		[Header("LawnDart Config")]
		public GameObject DartPrefab;

		public Transform[] DartSpawnLocations;

		public ParticleSystem[] FireWorkLaunch;

		public ParticleSystem[] FireWorks;

		public LawnDartPointDisplay ScoreDisplay;

		private List<LawnDart> SpawnedDarts = new List<LawnDart>();

		public GameObject[] FireWorkSounds;

		public Text ScorePanel1;

		public Text ScorePanel2;

		public List<int> ScoreList = new List<int>();

		public List<string> ScoreLabelList = new List<string>();

		private void Start()
		{
			SpawnDartSet();
		}

		private void SpawnDartSet()
		{
			for (int i = 0; i < DartSpawnLocations.Length; i++)
			{
				GameObject gameObject = Object.Instantiate(DartPrefab, DartSpawnLocations[i].position, DartSpawnLocations[i].rotation);
				LawnDart component = gameObject.GetComponent<LawnDart>();
				component.SetGame(this);
				SpawnedDarts.Add(component);
			}
		}

		private void Update()
		{
		}

		public void FireWork(Vector3 pos)
		{
			ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
			emitParams.position = pos;
			emitParams.startSize = Random.Range(6f, 12f);
			FireWorks[Random.Range(0, FireWorks.Length)].Emit(emitParams, 1);
			GameObject gameObject = Object.Instantiate(FireWorkSounds[Random.Range(0, FireWorkSounds.Length)], pos, Quaternion.identity);
		}

		public void ScoreEvent(Vector3 pos, string displaytext, int points, int multiplier, LawnDart dart)
		{
			GameObject gameObject = Object.Instantiate(FireWorkSounds[Random.Range(0, FireWorkSounds.Length)], pos, Quaternion.identity);
			ScoreDisplay.gameObject.SetActive(value: true);
			ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
			string txt = string.Empty;
			if (points > 0)
			{
				if (displaytext == string.Empty)
				{
					switch (multiplier)
					{
					case 1:
						txt = points.ToString();
						break;
					case 2:
						txt = points + " x 2 = " + points * 2;
						break;
					case 3:
						txt = points + " x 3 = " + points * 3;
						break;
					}
				}
				else
				{
					txt = displaytext;
				}
				ScoreDisplay.Activate(txt, pos + Vector3.up * 3f, 12f, multiplier);
				emitParams.position = pos + Vector3.up * 3f;
				FireWorkLaunch[Random.Range(0, FireWorkLaunch.Length)].Emit(emitParams, 1);
			}
			else
			{
				ScoreDisplay.Activate(displaytext, pos + Vector3.up * 3f, 10f, multiplier);
				emitParams.position = pos + Vector3.up * 3f;
				FireWorkLaunch[Random.Range(0, FireWorkLaunch.Length)].Emit(emitParams, 1);
			}
		}
	}
}
