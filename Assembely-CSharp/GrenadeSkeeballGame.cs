using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeSkeeballGame : MonoBehaviour
{
	public enum PointsParticleType
	{
		Blue,
		Yellow,
		Red
	}

	public GameObject TargetPrefabSet;

	public GameObject GrenadePrefab;

	public Transform GrenadeSpawnPoint;

	public Vector3 GrenadeForce;

	private GameObject TargetGroup;

	private List<GameObject> Grenades = new List<GameObject>();

	public AudioSource AudioBeep1;

	public AudioSource AudioBegin1;

	private int m_score;

	public Text ScoreReadout;

	public ParticleSystem PSystem_500;

	public ParticleSystem PSystem_1000;

	public ParticleSystem PSystem_5000;

	public AudioSource AudioPointsBlue;

	public AudioSource AudioPointsYellow;

	public AudioSource AudioPointsRed;

	public void AddPoints(int p)
	{
		m_score += p;
		UpdateScoreScreen();
	}

	private void UpdateScoreScreen()
	{
		ScoreReadout.text = m_score.ToString("000000");
	}

	public void ClearGame()
	{
		CancelInvoke();
		m_score = 0;
		UpdateScoreScreen();
		Object.Destroy(TargetGroup);
		if (Grenades.Count > 0)
		{
			for (int num = Grenades.Count - 1; num >= 0; num--)
			{
				if (Grenades[num] != null)
				{
					Object.Destroy(Grenades[num]);
				}
			}
		}
		Grenades.Clear();
	}

	public void BeginGame(int j)
	{
		ClearGame();
		AudioBegin1.PlayOneShot(AudioBegin1.clip, 0.3f);
		TargetGroup = Object.Instantiate(TargetPrefabSet, Vector3.zero, Quaternion.identity);
		GrenadeSkeeballTargetCollection component = TargetGroup.GetComponent<GrenadeSkeeballTargetCollection>();
		component.SetGameReference(this);
		for (int i = 0; i < 6; i++)
		{
			Invoke("SpawnGrenade", i + 1);
		}
	}

	private void SpawnGrenade()
	{
		AudioBeep1.PlayOneShot(AudioBeep1.clip, 0.6f);
		GameObject gameObject = Object.Instantiate(GrenadePrefab, GrenadeSpawnPoint.position, GrenadeSpawnPoint.rotation);
		Grenades.Add(gameObject);
		gameObject.GetComponent<Rigidbody>().AddForce(GrenadeForce);
	}

	public void SetPlayHeight(int i)
	{
		switch (i)
		{
		case 1:
			base.transform.position = new Vector3(0f, 0f, 0f);
			break;
		case 2:
			base.transform.position = new Vector3(0f, -0.04f, 0f);
			break;
		case 3:
			base.transform.position = new Vector3(0f, -0.08f, 0f);
			break;
		case 4:
			base.transform.position = new Vector3(0f, -0.12f, 0f);
			break;
		case 5:
			base.transform.position = new Vector3(0f, -0.16f, 0f);
			break;
		case 6:
			base.transform.position = new Vector3(0f, -0.2f, 0f);
			break;
		case 7:
			base.transform.position = new Vector3(0f, -0.24f, 0f);
			break;
		}
	}

	public void SpawnPointsFX(Vector3 pos, PointsParticleType type)
	{
		ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
		switch (type)
		{
		case PointsParticleType.Blue:
			emitParams.position = pos;
			emitParams.velocity = Vector3.up * 5f;
			AudioPointsBlue.PlayOneShot(AudioPointsBlue.clip, 0.1f);
			PSystem_500.Emit(emitParams, 1);
			break;
		case PointsParticleType.Yellow:
			emitParams.position = pos;
			emitParams.velocity = Vector3.up * 5f;
			AudioPointsYellow.PlayOneShot(AudioPointsYellow.clip, 0.1f);
			PSystem_1000.Emit(emitParams, 1);
			break;
		case PointsParticleType.Red:
			emitParams.position = pos;
			emitParams.velocity = Vector3.up * 5f;
			AudioPointsRed.PlayOneShot(AudioPointsRed.clip, 0.1f);
			PSystem_5000.Emit(emitParams, 1);
			break;
		}
	}
}
