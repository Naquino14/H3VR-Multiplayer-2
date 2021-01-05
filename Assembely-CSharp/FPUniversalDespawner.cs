using System.Collections;
using UnityEngine;

public class FPUniversalDespawner : MonoBehaviour, IFastPoolItem
{
	[SerializeField]
	private int targetPoolID;

	[SerializeField]
	private bool despawnDelayed;

	[SerializeField]
	private float delay;

	[SerializeField]
	private bool despawnOnParticlesDead;

	[SerializeField]
	private bool resetParticleSystem;

	[SerializeField]
	private bool despawnOnAudioSourceStop;

	private bool needCheck;

	private AudioSource aSource;

	private ParticleSystem pSystem;

	public int TargetPoolID
	{
		get
		{
			return targetPoolID;
		}
		set
		{
			targetPoolID = value;
		}
	}

	public bool DespawnDelayed => despawnDelayed;

	public float Delay => delay;

	public bool DespawnOnParticlesDead => despawnOnParticlesDead;

	public bool ResetParticleSystem => resetParticleSystem;

	public bool DespawnOnAudioSourceStop => despawnOnAudioSourceStop;

	private void Start()
	{
		if (despawnDelayed)
		{
			StartCoroutine(Despawn(delay));
		}
		if (despawnOnAudioSourceStop)
		{
			aSource = GetComponentInChildren<AudioSource>();
			needCheck = true;
		}
		if (despawnOnParticlesDead)
		{
			pSystem = GetComponentInChildren<ParticleSystem>();
			needCheck = true;
		}
		if (needCheck)
		{
			StartCoroutine(CheckAlive());
		}
	}

	public void OnFastInstantiate()
	{
		if (despawnDelayed)
		{
			StartCoroutine(Despawn(delay));
		}
		if (needCheck)
		{
			StartCoroutine(CheckAlive());
		}
		if (despawnOnParticlesDead && pSystem != null && resetParticleSystem)
		{
			pSystem.Play(withChildren: true);
		}
	}

	public void OnFastDestroy()
	{
		StopAllCoroutines();
		if (despawnOnParticlesDead && pSystem != null && resetParticleSystem)
		{
			pSystem.Clear(withChildren: true);
		}
	}

	private IEnumerator Despawn(float despawn_delay)
	{
		yield return new WaitForSeconds(despawn_delay);
		StopAllCoroutines();
		if (FastPoolManager.Instance != null)
		{
			FastPoolManager.GetPool(targetPoolID, base.gameObject).FastDestroy(base.gameObject);
		}
		else
		{
			Debug.LogError("FastPoolManager is not present in the scene or being disabled! AutoDespawn will not working on GameObject " + base.name);
		}
	}

	private IEnumerator CheckAlive()
	{
		do
		{
			yield return new WaitForSeconds(1f);
			if (despawnOnAudioSourceStop && aSource != null && !aSource.isPlaying)
			{
				StartCoroutine(Despawn(0f));
				yield break;
			}
		}
		while (!despawnOnParticlesDead || !(pSystem != null) || pSystem.IsAlive(withChildren: true));
		StartCoroutine(Despawn(0f));
	}
}
