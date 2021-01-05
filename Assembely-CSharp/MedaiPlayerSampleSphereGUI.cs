using UnityEngine;

public class MedaiPlayerSampleSphereGUI : MonoBehaviour
{
	public MediaPlayerCtrl scrMedia;

	private void Start()
	{
	}

	private void Update()
	{
		Touch[] touches = Input.touches;
		foreach (Touch touch in touches)
		{
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y + touch.deltaPosition.x, base.transform.localEulerAngles.z);
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(50f, 50f, 100f, 100f), "Load"))
		{
			scrMedia.Load("EasyMovieTexture.mp4");
		}
		if (GUI.Button(new Rect(50f, 200f, 100f, 100f), "Play"))
		{
			scrMedia.Play();
		}
		if (GUI.Button(new Rect(50f, 350f, 100f, 100f), "stop"))
		{
			scrMedia.Stop();
		}
		if (GUI.Button(new Rect(50f, 500f, 100f, 100f), "pause"))
		{
			scrMedia.Pause();
		}
		if (GUI.Button(new Rect(50f, 650f, 100f, 100f), "Unload"))
		{
			scrMedia.UnLoad();
		}
	}
}
