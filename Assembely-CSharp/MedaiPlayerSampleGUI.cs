using System;
using UnityEngine;

public class MedaiPlayerSampleGUI : MonoBehaviour
{
	public MediaPlayerCtrl scrMedia;

	public bool m_bFinish;

	private void Start()
	{
		MediaPlayerCtrl mediaPlayerCtrl = scrMedia;
		mediaPlayerCtrl.OnEnd = (MediaPlayerCtrl.VideoEnd)Delegate.Combine(mediaPlayerCtrl.OnEnd, new MediaPlayerCtrl.VideoEnd(OnEnd));
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(50f, 50f, 100f, 100f), "Load"))
		{
			scrMedia.Load("EasyMovieTexture.mp4");
			m_bFinish = false;
		}
		if (GUI.Button(new Rect(50f, 200f, 100f, 100f), "Play"))
		{
			scrMedia.Play();
			m_bFinish = false;
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
		if (GUI.Button(new Rect(50f, 800f, 100f, 100f), " " + m_bFinish))
		{
		}
		if (GUI.Button(new Rect(200f, 50f, 100f, 100f), "SeekTo"))
		{
			scrMedia.SeekTo(10000);
		}
		if (scrMedia.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
		{
			if (GUI.Button(new Rect(200f, 200f, 100f, 100f), scrMedia.GetSeekPosition().ToString()))
			{
				scrMedia.SetSpeed(2f);
			}
			if (GUI.Button(new Rect(200f, 350f, 100f, 100f), scrMedia.GetDuration().ToString()))
			{
				scrMedia.SetSpeed(1f);
			}
			if (GUI.Button(new Rect(200f, 450f, 100f, 100f), scrMedia.GetVideoWidth().ToString()))
			{
			}
			if (!GUI.Button(new Rect(200f, 550f, 100f, 100f), scrMedia.GetVideoHeight().ToString()))
			{
			}
		}
		if (!GUI.Button(new Rect(200f, 650f, 100f, 100f), scrMedia.GetCurrentSeekPercent().ToString()))
		{
		}
	}

	private void OnEnd()
	{
		m_bFinish = true;
	}
}
