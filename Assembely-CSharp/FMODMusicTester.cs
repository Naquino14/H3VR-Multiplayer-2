using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODMusicTester : MonoBehaviour
{
	[EventRef]
	public string StateEvent;

	public EventInstance EventInstance;

	private void Start()
	{
		EventInstance = RuntimeManager.CreateInstance(StateEvent);
	}

	private void Play()
	{
		EventInstance.start();
	}

	private void FadeOut()
	{
		EventInstance.stop(STOP_MODE.ALLOWFADEOUT);
	}

	private void StopImmediate()
	{
		EventInstance.stop(STOP_MODE.IMMEDIATE);
	}

	private void SetIntensity(float f)
	{
		EventInstance.setParameterValue("Intensity", f);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			Play();
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			FadeOut();
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			StopImmediate();
		}
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			SetIntensity(0f);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SetIntensity(1f);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SetIntensity(2f);
		}
	}
}
