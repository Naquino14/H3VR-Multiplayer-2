using System;
using UnityEngine;

public class MediaPlayerEvent : MonoBehaviour
{
	public MediaPlayerCtrl m_srcVideo;

	private void Start()
	{
		MediaPlayerCtrl srcVideo = m_srcVideo;
		srcVideo.OnReady = (MediaPlayerCtrl.VideoReady)Delegate.Combine(srcVideo.OnReady, new MediaPlayerCtrl.VideoReady(OnReady));
		MediaPlayerCtrl srcVideo2 = m_srcVideo;
		srcVideo2.OnVideoFirstFrameReady = (MediaPlayerCtrl.VideoFirstFrameReady)Delegate.Combine(srcVideo2.OnVideoFirstFrameReady, new MediaPlayerCtrl.VideoFirstFrameReady(OnFirstFrameReady));
		MediaPlayerCtrl srcVideo3 = m_srcVideo;
		srcVideo3.OnVideoError = (MediaPlayerCtrl.VideoError)Delegate.Combine(srcVideo3.OnVideoError, new MediaPlayerCtrl.VideoError(OnError));
		MediaPlayerCtrl srcVideo4 = m_srcVideo;
		srcVideo4.OnEnd = (MediaPlayerCtrl.VideoEnd)Delegate.Combine(srcVideo4.OnEnd, new MediaPlayerCtrl.VideoEnd(OnEnd));
		MediaPlayerCtrl srcVideo5 = m_srcVideo;
		srcVideo5.OnResize = (MediaPlayerCtrl.VideoResize)Delegate.Combine(srcVideo5.OnResize, new MediaPlayerCtrl.VideoResize(OnResize));
	}

	private void Update()
	{
	}

	private void OnReady()
	{
		Debug.Log("OnReady");
	}

	private void OnFirstFrameReady()
	{
		Debug.Log("OnFirstFrameReady");
	}

	private void OnEnd()
	{
		Debug.Log("OnEnd");
	}

	private void OnResize()
	{
		Debug.Log("OnResize");
	}

	private void OnError(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra)
	{
		Debug.Log("OnError");
	}
}
