using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using FFmpeg.AutoGen;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MediaPlayerCtrl : MonoBehaviour
{
	public delegate void VideoEnd();

	public delegate void VideoReady();

	public delegate void VideoError(MEDIAPLAYER_ERROR errorCode, MEDIAPLAYER_ERROR errorCodeExtra);

	public delegate void VideoFirstFrameReady();

	public delegate void VideoResize();

	public enum MEDIAPLAYER_ERROR
	{
		MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK = 200,
		MEDIA_ERROR_IO = -1004,
		MEDIA_ERROR_MALFORMED = -1007,
		MEDIA_ERROR_TIMED_OUT = -110,
		MEDIA_ERROR_UNSUPPORTED = -1010,
		MEDIA_ERROR_SERVER_DIED = 100,
		MEDIA_ERROR_UNKNOWN = 1
	}

	public enum MEDIAPLAYER_STATE
	{
		NOT_READY,
		READY,
		END,
		PLAYING,
		PAUSED,
		STOPPED,
		ERROR
	}

	public enum MEDIA_SCALE
	{
		SCALE_X_TO_Y,
		SCALE_X_TO_Z,
		SCALE_Y_TO_X,
		SCALE_Y_TO_Z,
		SCALE_Z_TO_X,
		SCALE_Z_TO_Y,
		SCALE_X_TO_Y_2
	}

	public delegate void VideoInterrupt();

	public string m_strFileName;

	public GameObject[] m_TargetMaterial;

	private Texture2D m_VideoTexture;

	private Texture2D m_VideoTextureDummy;

	private MEDIAPLAYER_STATE m_CurrentState;

	private int m_iCurrentSeekPosition;

	private float m_fVolume = 1f;

	private int m_iWidth;

	private int m_iHeight;

	private float m_fSpeed = 1f;

	public bool m_bFullScreen;

	public bool m_bSupportRockchip = true;

	public VideoResize OnResize;

	public VideoReady OnReady;

	public VideoEnd OnEnd;

	public VideoError OnVideoError;

	public VideoFirstFrameReady OnVideoFirstFrameReady;

	private IntPtr m_texPtr;

	private int m_iAndroidMgrID;

	private bool m_bIsFirstFrameReady;

	private bool m_bFirst;

	public MEDIA_SCALE m_ScaleValue;

	public GameObject[] m_objResize;

	public bool m_bLoop;

	public bool m_bAutoPlay = true;

	private bool m_bStop;

	private bool m_bInit;

	private bool m_bCheckFBO;

	private bool m_bPause;

	private bool m_bReadyPlay;

	private unsafe AVFrame* pConvertedFrame;

	private unsafe sbyte* pConvertedFrameBuffer;

	private unsafe SwsContext* pConvertContext;

	private unsafe AVCodecContext* pCodecContext;

	private unsafe AVCodecContext* pAudioCodecContext;

	private unsafe AVFrame* pDecodedFrame;

	private unsafe AVFrame* pDecodedAudioFrame;

	private unsafe AVFormatContext* pFormatContext;

	private unsafe AVPacket* pPacket;

	private unsafe AVStream* pStream;

	private unsafe AVStream* pStreamAudio;

	private int iStreamAudioIndex;

	private int iStreamIndex;

	private AudioClip audioClip;

	private AudioSource audioSource;

	private int iSoundCount;

	private int iInitCount;

	private double pts;

	private bool bVideoFirstFrameReady;

	private int m_iID;

	private Thread loader;

	private VideoInterrupt action;

	private bool bInterrupt;

	private float fLastFrameTime;

	private float fCurrentSeekTime;

	private float[] fAudioData;

	private bool bEnd;

	private Thread threadVideo;

	private List<float[]> listAudio;

	private Queue<byte[]> listVideo;

	private List<double> listAudioPts;

	private List<double> listAudioPtsTime;

	private Queue<float> listVideoPts;

	private int iSoundBufferCount;

	private bool bSeekTo;

	private List<Action> unityMainThreadActionList = new List<Action>();

	private bool checkNewActions;

	private object thisLock = new object();

	static MediaPlayerCtrl()
	{
	}

	[DllImport("EasyMovieTexture")]
	private static extern int SetTexture();

	[DllImport("EasyMovieTexture")]
	private static extern void ReleaseTexture(int iID);

	[DllImport("EasyMovieTexture")]
	private static extern void SetTextureFromUnity(int iID, IntPtr texture, int w, int h, byte[] data);

	[DllImport("EasyMovieTexture")]
	private static extern IntPtr GetRenderEventFunc();

	private void Awake()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
		string text = Application.dataPath + Path.DirectorySeparatorChar + "Plugins";
		if (!environmentVariable.Contains(text))
		{
			Environment.SetEnvironmentVariable("PATH", environmentVariable + Path.PathSeparator + text, EnvironmentVariableTarget.Process);
		}
		if (SystemInfo.deviceModel.Contains("rockchip"))
		{
			m_bSupportRockchip = true;
		}
		else
		{
			m_bSupportRockchip = false;
		}
		if (m_TargetMaterial != null)
		{
			for (int i = 0; i < m_TargetMaterial.Length; i++)
			{
				if (!(m_TargetMaterial[i] != null))
				{
					continue;
				}
				if (m_TargetMaterial[i].GetComponent<MeshFilter>() != null)
				{
					Vector2[] uv = m_TargetMaterial[i].GetComponent<MeshFilter>().mesh.uv;
					for (int j = 0; j < uv.Length; j++)
					{
						ref Vector2 reference = ref uv[j];
						reference = new Vector2(uv[j].x, 1f - uv[j].y);
					}
					m_TargetMaterial[i].GetComponent<MeshFilter>().mesh.uv = uv;
				}
				if (m_TargetMaterial[i].GetComponent<RawImage>() != null)
				{
					m_TargetMaterial[i].GetComponent<RawImage>().uvRect = new Rect(0f, 1f, 1f, -1f);
				}
			}
		}
		Call_SetUnityActivity();
	}

	private void Start()
	{
		m_bInit = true;
	}

	private void OnApplicationQuit()
	{
	}

	private void OnDisable()
	{
		if (GetCurrentState() == MEDIAPLAYER_STATE.PLAYING)
		{
			Pause();
		}
	}

	private void OnEnable()
	{
		if (GetCurrentState() == MEDIAPLAYER_STATE.PAUSED)
		{
			Play();
		}
	}

	private void Update()
	{
		if (string.IsNullOrEmpty(m_strFileName))
		{
			return;
		}
		if (checkNewActions)
		{
			checkNewActions = false;
			CheckThreading();
		}
		if (!m_bFirst)
		{
			string strFileName = m_strFileName.Trim();
			Call_Load(strFileName, 0);
			Call_SetLooping(m_bLoop);
			m_bFirst = true;
		}
		if (m_CurrentState == MEDIAPLAYER_STATE.PLAYING || m_CurrentState == MEDIAPLAYER_STATE.PAUSED)
		{
			if (!m_bCheckFBO)
			{
				if (Call_GetVideoWidth() <= 0 || Call_GetVideoHeight() <= 0)
				{
					return;
				}
				m_iWidth = Call_GetVideoWidth();
				m_iHeight = Call_GetVideoHeight();
				Resize();
				if (m_VideoTexture != null)
				{
					if (m_VideoTextureDummy != null)
					{
						UnityEngine.Object.Destroy(m_VideoTextureDummy);
						m_VideoTextureDummy = null;
					}
					m_VideoTextureDummy = m_VideoTexture;
					m_VideoTexture = null;
				}
				if (m_bSupportRockchip)
				{
					m_VideoTexture = new Texture2D(Call_GetVideoWidth(), Call_GetVideoHeight(), TextureFormat.RGB565, mipmap: false);
				}
				else
				{
					m_VideoTexture = new Texture2D(Call_GetVideoWidth(), Call_GetVideoHeight(), TextureFormat.RGBA32, mipmap: false);
				}
				m_VideoTexture.filterMode = FilterMode.Bilinear;
				m_VideoTexture.wrapMode = TextureWrapMode.Clamp;
				m_texPtr = m_VideoTexture.GetNativeTexturePtr();
				Call_SetUnityTexture(m_VideoTexture.GetNativeTextureID());
				Call_SetWindowSize();
				m_bCheckFBO = true;
				if (OnResize != null)
				{
					OnResize();
				}
			}
			else if (Call_GetVideoWidth() != m_iWidth || Call_GetVideoHeight() != m_iHeight)
			{
				m_iWidth = Call_GetVideoWidth();
				m_iHeight = Call_GetVideoHeight();
				if (OnResize != null)
				{
					OnResize();
				}
				ResizeTexture();
			}
			Call_UpdateVideoTexture();
			m_iCurrentSeekPosition = Call_GetSeekPosition();
		}
		if (m_CurrentState != Call_GetStatus())
		{
			m_CurrentState = Call_GetStatus();
			if (m_CurrentState == MEDIAPLAYER_STATE.READY)
			{
				if (OnReady != null)
				{
					OnReady();
				}
				if (m_bAutoPlay)
				{
					Call_Play(0);
				}
				if (m_bReadyPlay)
				{
					Call_Play(0);
					m_bReadyPlay = false;
				}
				SetVolume(m_fVolume);
			}
			else if (m_CurrentState == MEDIAPLAYER_STATE.END)
			{
				if (OnEnd != null)
				{
					OnEnd();
				}
				if (m_bLoop)
				{
					Call_Play(0);
				}
			}
			else if (m_CurrentState == MEDIAPLAYER_STATE.ERROR)
			{
				OnError((MEDIAPLAYER_ERROR)Call_GetError(), (MEDIAPLAYER_ERROR)Call_GetErrorExtra());
			}
		}
		GL.InvalidateState();
	}

	public void DeleteVideoTexture()
	{
		if (m_VideoTextureDummy != null)
		{
			UnityEngine.Object.Destroy(m_VideoTextureDummy);
			m_VideoTextureDummy = null;
		}
		if (m_VideoTexture != null)
		{
			UnityEngine.Object.Destroy(m_VideoTexture);
			m_VideoTexture = null;
		}
	}

	public void ResizeTexture()
	{
		Debug.Log("ResizeTexture " + m_iWidth + " " + m_iHeight);
		if (m_iWidth == 0 || m_iHeight == 0)
		{
			return;
		}
		if (m_VideoTexture != null)
		{
			if (m_VideoTextureDummy != null)
			{
				UnityEngine.Object.Destroy(m_VideoTextureDummy);
				m_VideoTextureDummy = null;
			}
			m_VideoTextureDummy = m_VideoTexture;
			m_VideoTexture = null;
		}
		if (m_bSupportRockchip)
		{
			m_VideoTexture = new Texture2D(Call_GetVideoWidth(), Call_GetVideoHeight(), TextureFormat.RGB565, mipmap: false);
		}
		else
		{
			m_VideoTexture = new Texture2D(Call_GetVideoWidth(), Call_GetVideoHeight(), TextureFormat.RGBA32, mipmap: false);
		}
		m_VideoTexture.filterMode = FilterMode.Bilinear;
		m_VideoTexture.wrapMode = TextureWrapMode.Clamp;
		m_texPtr = m_VideoTexture.GetNativeTexturePtr();
		Call_SetUnityTexture(m_VideoTexture.GetNativeTextureID());
		Call_SetWindowSize();
	}

	public void Resize()
	{
		if (m_CurrentState != MEDIAPLAYER_STATE.PLAYING || Call_GetVideoWidth() <= 0 || Call_GetVideoHeight() <= 0 || m_objResize == null)
		{
			return;
		}
		int width = Screen.width;
		int height = Screen.height;
		float num = (float)height / (float)width;
		int num2 = Call_GetVideoWidth();
		int num3 = Call_GetVideoHeight();
		float num4 = (float)num3 / (float)num2;
		float num5 = num / num4;
		for (int i = 0; i < m_objResize.Length; i++)
		{
			if (m_objResize[i] == null)
			{
				continue;
			}
			if (m_bFullScreen)
			{
				m_objResize[i].transform.localScale = new Vector3(20f / num, 20f / num, 1f);
				if (num4 < 1f)
				{
					if (num < 1f && num4 > num)
					{
						m_objResize[i].transform.localScale *= num5;
					}
					m_ScaleValue = MEDIA_SCALE.SCALE_X_TO_Y;
				}
				else
				{
					if (num > 1f)
					{
						if (num4 >= num)
						{
							m_objResize[i].transform.localScale *= num5;
						}
					}
					else
					{
						m_objResize[i].transform.localScale *= num5;
					}
					m_ScaleValue = MEDIA_SCALE.SCALE_X_TO_Y;
				}
			}
			if (m_ScaleValue == MEDIA_SCALE.SCALE_X_TO_Y)
			{
				m_objResize[i].transform.localScale = new Vector3(m_objResize[i].transform.localScale.x, m_objResize[i].transform.localScale.x * num4, m_objResize[i].transform.localScale.z);
			}
			else if (m_ScaleValue == MEDIA_SCALE.SCALE_X_TO_Y_2)
			{
				m_objResize[i].transform.localScale = new Vector3(m_objResize[i].transform.localScale.x, m_objResize[i].transform.localScale.x * num4 / 2f, m_objResize[i].transform.localScale.z);
			}
			else if (m_ScaleValue == MEDIA_SCALE.SCALE_X_TO_Z)
			{
				m_objResize[i].transform.localScale = new Vector3(m_objResize[i].transform.localScale.x, m_objResize[i].transform.localScale.y, m_objResize[i].transform.localScale.x * num4);
			}
			else if (m_ScaleValue == MEDIA_SCALE.SCALE_Y_TO_X)
			{
				m_objResize[i].transform.localScale = new Vector3(m_objResize[i].transform.localScale.y / num4, m_objResize[i].transform.localScale.y, m_objResize[i].transform.localScale.z);
			}
			else if (m_ScaleValue == MEDIA_SCALE.SCALE_Y_TO_Z)
			{
				m_objResize[i].transform.localScale = new Vector3(m_objResize[i].transform.localScale.x, m_objResize[i].transform.localScale.y, m_objResize[i].transform.localScale.y / num4);
			}
			else if (m_ScaleValue == MEDIA_SCALE.SCALE_Z_TO_X)
			{
				m_objResize[i].transform.localScale = new Vector3(m_objResize[i].transform.localScale.z * num4, m_objResize[i].transform.localScale.y, m_objResize[i].transform.localScale.z);
			}
			else if (m_ScaleValue == MEDIA_SCALE.SCALE_Z_TO_Y)
			{
				m_objResize[i].transform.localScale = new Vector3(m_objResize[i].transform.localScale.x, m_objResize[i].transform.localScale.z * num4, m_objResize[i].transform.localScale.z);
			}
			else
			{
				m_objResize[i].transform.localScale = new Vector3(m_objResize[i].transform.localScale.x, m_objResize[i].transform.localScale.y, m_objResize[i].transform.localScale.z);
			}
		}
	}

	private void OnError(MEDIAPLAYER_ERROR iCode, MEDIAPLAYER_ERROR iCodeExtra)
	{
		string empty = string.Empty;
		empty = iCode switch
		{
			MEDIAPLAYER_ERROR.MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK => "MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK", 
			MEDIAPLAYER_ERROR.MEDIA_ERROR_SERVER_DIED => "MEDIA_ERROR_SERVER_DIED", 
			MEDIAPLAYER_ERROR.MEDIA_ERROR_UNKNOWN => "MEDIA_ERROR_UNKNOWN", 
			_ => "Unknown error " + iCode, 
		} + " ";
		Debug.LogError(iCodeExtra switch
		{
			MEDIAPLAYER_ERROR.MEDIA_ERROR_IO => empty + "MEDIA_ERROR_IO", 
			MEDIAPLAYER_ERROR.MEDIA_ERROR_MALFORMED => empty + "MEDIA_ERROR_MALFORMED", 
			MEDIAPLAYER_ERROR.MEDIA_ERROR_TIMED_OUT => empty + "MEDIA_ERROR_TIMED_OUT", 
			MEDIAPLAYER_ERROR.MEDIA_ERROR_UNSUPPORTED => empty + "MEDIA_ERROR_UNSUPPORTED", 
			_ => "Unknown error " + iCode, 
		});
		if (OnVideoError != null)
		{
			OnVideoError(iCode, iCodeExtra);
		}
	}

	private void OnDestroy()
	{
		Call_UnLoad();
		if (m_VideoTextureDummy != null)
		{
			UnityEngine.Object.Destroy(m_VideoTextureDummy);
			m_VideoTextureDummy = null;
		}
		if (m_VideoTexture != null)
		{
			UnityEngine.Object.Destroy(m_VideoTexture);
		}
		Call_Destroy();
	}

	private void OnApplicationPause(bool bPause)
	{
		Debug.Log("ApplicationPause : " + bPause);
		if (bPause)
		{
			if (m_CurrentState == MEDIAPLAYER_STATE.PAUSED)
			{
				m_bPause = true;
			}
			Call_Pause();
			return;
		}
		Call_RePlay();
		if (m_bPause)
		{
			Call_Pause();
			m_bPause = false;
		}
	}

	public MEDIAPLAYER_STATE GetCurrentState()
	{
		return m_CurrentState;
	}

	public Texture2D GetVideoTexture()
	{
		return m_VideoTexture;
	}

	public void Play()
	{
		if (m_bStop)
		{
			SeekTo(0);
			Call_Play(0);
			m_bStop = false;
		}
		if (m_CurrentState == MEDIAPLAYER_STATE.PAUSED)
		{
			Call_RePlay();
		}
		else if (m_CurrentState == MEDIAPLAYER_STATE.READY || m_CurrentState == MEDIAPLAYER_STATE.STOPPED || m_CurrentState == MEDIAPLAYER_STATE.END)
		{
			Call_Play(0);
		}
		else if (m_CurrentState == MEDIAPLAYER_STATE.NOT_READY)
		{
			m_bReadyPlay = true;
		}
	}

	public void Stop()
	{
		if (m_CurrentState == MEDIAPLAYER_STATE.PLAYING)
		{
			Call_Pause();
		}
		m_bStop = true;
		m_CurrentState = MEDIAPLAYER_STATE.STOPPED;
		m_iCurrentSeekPosition = 0;
	}

	public void Pause()
	{
		if (m_CurrentState == MEDIAPLAYER_STATE.PLAYING)
		{
			Call_Pause();
		}
		m_CurrentState = MEDIAPLAYER_STATE.PAUSED;
	}

	public void Load(string strFileName)
	{
		if (GetCurrentState() != 0)
		{
			UnLoad();
		}
		m_bReadyPlay = false;
		m_bIsFirstFrameReady = false;
		m_bFirst = false;
		m_bCheckFBO = false;
		m_strFileName = strFileName;
		if (m_bInit)
		{
			m_CurrentState = MEDIAPLAYER_STATE.NOT_READY;
		}
	}

	public void SetVolume(float fVolume)
	{
		if (m_CurrentState == MEDIAPLAYER_STATE.PLAYING || m_CurrentState == MEDIAPLAYER_STATE.PAUSED || m_CurrentState == MEDIAPLAYER_STATE.END || m_CurrentState == MEDIAPLAYER_STATE.READY || m_CurrentState == MEDIAPLAYER_STATE.STOPPED)
		{
			m_fVolume = fVolume;
			Call_SetVolume(fVolume);
		}
	}

	public int GetSeekPosition()
	{
		if (m_CurrentState == MEDIAPLAYER_STATE.PLAYING || m_CurrentState == MEDIAPLAYER_STATE.PAUSED || m_CurrentState == MEDIAPLAYER_STATE.END)
		{
			return m_iCurrentSeekPosition;
		}
		return 0;
	}

	public void SeekTo(int iSeek)
	{
		if (m_CurrentState == MEDIAPLAYER_STATE.PLAYING || m_CurrentState == MEDIAPLAYER_STATE.READY || m_CurrentState == MEDIAPLAYER_STATE.PAUSED || m_CurrentState == MEDIAPLAYER_STATE.END || m_CurrentState == MEDIAPLAYER_STATE.STOPPED)
		{
			Call_SetSeekPosition(iSeek);
		}
	}

	public void SetSpeed(float fSpeed)
	{
		if (m_CurrentState == MEDIAPLAYER_STATE.PLAYING || m_CurrentState == MEDIAPLAYER_STATE.READY || m_CurrentState == MEDIAPLAYER_STATE.PAUSED || m_CurrentState == MEDIAPLAYER_STATE.END || m_CurrentState == MEDIAPLAYER_STATE.STOPPED)
		{
			m_fSpeed = fSpeed;
			Call_SetSpeed(fSpeed);
		}
	}

	public int GetDuration()
	{
		if (m_CurrentState == MEDIAPLAYER_STATE.PLAYING || m_CurrentState == MEDIAPLAYER_STATE.PAUSED || m_CurrentState == MEDIAPLAYER_STATE.END || m_CurrentState == MEDIAPLAYER_STATE.READY || m_CurrentState == MEDIAPLAYER_STATE.STOPPED)
		{
			return Call_GetDuration();
		}
		return 0;
	}

	public float GetSeekBarValue()
	{
		if (m_CurrentState == MEDIAPLAYER_STATE.PLAYING || m_CurrentState == MEDIAPLAYER_STATE.PAUSED || m_CurrentState == MEDIAPLAYER_STATE.END || m_CurrentState == MEDIAPLAYER_STATE.READY || m_CurrentState == MEDIAPLAYER_STATE.STOPPED)
		{
			if (GetDuration() == 0)
			{
				return 0f;
			}
			return (float)GetSeekPosition() / (float)GetDuration();
		}
		return 0f;
	}

	public void SetSeekBarValue(float fValue)
	{
		if ((m_CurrentState == MEDIAPLAYER_STATE.PLAYING || m_CurrentState == MEDIAPLAYER_STATE.PAUSED || m_CurrentState == MEDIAPLAYER_STATE.END || m_CurrentState == MEDIAPLAYER_STATE.READY || m_CurrentState == MEDIAPLAYER_STATE.STOPPED) && GetDuration() != 0)
		{
			SeekTo((int)((float)GetDuration() * fValue));
		}
	}

	public int GetCurrentSeekPercent()
	{
		if (m_CurrentState == MEDIAPLAYER_STATE.PLAYING || m_CurrentState == MEDIAPLAYER_STATE.PAUSED || m_CurrentState == MEDIAPLAYER_STATE.END || m_CurrentState == MEDIAPLAYER_STATE.READY)
		{
			return Call_GetCurrentSeekPercent();
		}
		return 0;
	}

	public int GetVideoWidth()
	{
		return Call_GetVideoWidth();
	}

	public int GetVideoHeight()
	{
		return Call_GetVideoHeight();
	}

	public void UnLoad()
	{
		m_bCheckFBO = false;
		Call_UnLoad();
		m_CurrentState = MEDIAPLAYER_STATE.NOT_READY;
	}

	private void Call_Destroy()
	{
		if (loader != null)
		{
			while (loader.IsAlive)
			{
				loader.Abort();
			}
			loader = null;
		}
		if (threadVideo != null)
		{
			while (threadVideo.IsAlive)
			{
				threadVideo.Abort();
			}
			threadVideo = null;
		}
		ffmpeg.avformat_network_deinit();
		ReleaseTexture(m_iID);
	}

	private unsafe void Call_UnLoad()
	{
		m_CurrentState = MEDIAPLAYER_STATE.NOT_READY;
		if (loader != null)
		{
			while (loader.IsAlive)
			{
				loader.Abort();
			}
			loader = null;
		}
		if (threadVideo != null)
		{
			while (threadVideo.IsAlive)
			{
				threadVideo.Abort();
			}
			threadVideo = null;
		}
		if (listAudio != null)
		{
			listAudio.Clear();
			listAudio = null;
		}
		if (listVideo != null)
		{
			listVideo.Clear();
			listVideo = null;
		}
		if (listAudioPts != null)
		{
			listAudioPts.Clear();
			listAudioPts = null;
		}
		if (listAudioPtsTime != null)
		{
			listAudioPtsTime.Clear();
			listAudioPtsTime = null;
		}
		if (listVideoPts != null)
		{
			listVideoPts.Clear();
			listVideoPts = null;
		}
		fCurrentSeekTime = 0f;
		fLastFrameTime = 0f;
		if (pPacket != null)
		{
			ffmpeg.av_free_packet(pPacket);
			Marshal.FreeCoTaskMem((IntPtr)pPacket);
			pPacket = null;
		}
		if (pConvertedFrame != null)
		{
			ffmpeg.av_free(pConvertedFrame);
			pConvertedFrame = null;
		}
		if (pConvertedFrameBuffer != null)
		{
			ffmpeg.av_free(pConvertedFrameBuffer);
			pConvertedFrameBuffer = null;
		}
		if (pConvertContext != null)
		{
			ffmpeg.sws_freeContext(pConvertContext);
			pConvertContext = null;
		}
		if (pDecodedFrame != null)
		{
			ffmpeg.av_free(pDecodedFrame);
			pDecodedFrame = null;
		}
		if (pDecodedAudioFrame != null)
		{
			ffmpeg.av_free(pDecodedAudioFrame);
		}
		pDecodedAudioFrame = null;
		if (pCodecContext != null)
		{
			ffmpeg.avcodec_close(pCodecContext);
		}
		pCodecContext = null;
		if (pAudioCodecContext != null)
		{
			ffmpeg.avcodec_close(pAudioCodecContext);
		}
		pAudioCodecContext = null;
		if (pFormatContext != null)
		{
			AVFormatContext* ptr = pFormatContext;
			ffmpeg.avformat_close_input(&ptr);
		}
		pFormatContext = null;
		if (audioSource != null)
		{
			audioSource.Stop();
		}
		if (audioClip != null)
		{
			UnityEngine.Object.Destroy(audioClip);
			audioClip = null;
		}
	}

	private unsafe bool Call_Load(string strFileName, int iSeek)
	{
		fCurrentSeekTime = 0f;
		fLastFrameTime = 0f;
		iSoundCount = 0;
		iInitCount = 0;
		bSeekTo = true;
		bEnd = false;
		if (audioSource != null)
		{
			audioSource.Stop();
			audioSource.time = 0f;
		}
		if (audioClip != null)
		{
			UnityEngine.Object.Destroy(audioClip);
			audioClip = null;
		}
		pFormatContext = ffmpeg.avformat_alloc_context();
		if (!strFileName.Contains("://"))
		{
			strFileName = Application.streamingAssetsPath + "/" + strFileName;
			Debug.Log(strFileName);
		}
		else if (strFileName.Contains("file://"))
		{
			strFileName = strFileName.Replace("file://", string.Empty);
		}
		loader = new Thread((ThreadStart)delegate
		{
			AVFormatContext* ptr = null;
			if (ffmpeg.avformat_open_input(&ptr, strFileName, null, null) != 0)
			{
				pFormatContext = null;
				m_CurrentState = MEDIAPLAYER_STATE.ERROR;
				throw new ApplicationException("Could not open file");
			}
			pFormatContext = ptr;
			if (ffmpeg.avformat_find_stream_info(pFormatContext, null) != 0)
			{
				m_CurrentState = MEDIAPLAYER_STATE.ERROR;
				throw new ApplicationException("Could not find stream info");
			}
			AddActionForUnityMainThread(delegate
			{
				LoadVideoPart2();
			});
		});
		loader.Priority = System.Threading.ThreadPriority.AboveNormal;
		loader.IsBackground = true;
		loader.Start();
		return true;
	}

	private unsafe void LoadVideoPart2()
	{
		pStream = null;
		pStreamAudio = null;
		bool flag = false;
		for (int i = 0; i < pFormatContext->nb_streams; i++)
		{
			if (pFormatContext->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
			{
				if (!flag)
				{
					flag = true;
					pStream = pFormatContext->streams[i];
					iStreamIndex = i;
					Debug.Log("Video" + iStreamIndex);
				}
			}
			else if (pFormatContext->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
			{
				pStreamAudio = pFormatContext->streams[i];
				iStreamAudioIndex = i;
			}
		}
		if (pStream == null)
		{
			m_CurrentState = MEDIAPLAYER_STATE.ERROR;
			throw new ApplicationException("Could not found video stream");
		}
		if (pStreamAudio == null)
		{
			m_CurrentState = MEDIAPLAYER_STATE.ERROR;
		}
		AVCodecContext codec = *pStream->codec;
		m_iWidth = codec.width;
		m_iHeight = codec.height;
		AVPixelFormat pix_fmt = codec.pix_fmt;
		AVCodecID codec_id = codec.codec_id;
		AVPixelFormat aVPixelFormat = AVPixelFormat.AV_PIX_FMT_RGBA;
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D9)
		{
			aVPixelFormat = AVPixelFormat.AV_PIX_FMT_BGRA;
		}
		pConvertContext = ffmpeg.sws_getContext(m_iWidth, m_iHeight, pix_fmt, m_iWidth, m_iHeight, aVPixelFormat, 1, null, null, null);
		if (pConvertContext == null)
		{
			m_CurrentState = MEDIAPLAYER_STATE.ERROR;
			throw new ApplicationException("Could not initialize the conversion context");
		}
		pConvertedFrame = ffmpeg.av_frame_alloc();
		int num = ffmpeg.avpicture_get_size(aVPixelFormat, m_iWidth, m_iHeight);
		pConvertedFrameBuffer = (sbyte*)ffmpeg.av_malloc((ulong)num);
		AVPicture* picture = (AVPicture*)pConvertedFrame;
		ffmpeg.avpicture_fill(picture, pConvertedFrameBuffer, aVPixelFormat, m_iWidth, m_iHeight);
		AVCodec* ptr = ffmpeg.avcodec_find_decoder(codec_id);
		if (ptr == null)
		{
			m_CurrentState = MEDIAPLAYER_STATE.ERROR;
			throw new ApplicationException("Unsupported codec");
		}
		pCodecContext = pStream->codec;
		pCodecContext->hwaccel = ff_find_hwaccel(pCodecContext->codec_id, pix_fmt);
		if (ffmpeg.avcodec_open2(pCodecContext, ptr, null) < 0)
		{
			m_CurrentState = MEDIAPLAYER_STATE.ERROR;
			throw new ApplicationException("Could not open codec");
		}
		if (pStreamAudio != null)
		{
			AVCodecContext codec2 = *pStreamAudio->codec;
			AVCodec* ptr2 = ffmpeg.avcodec_find_decoder(codec2.codec_id);
			if (ptr2 == null)
			{
				m_CurrentState = MEDIAPLAYER_STATE.ERROR;
				throw new ApplicationException("Unsupported codec");
			}
			pAudioCodecContext = pStreamAudio->codec;
			if (ffmpeg.avcodec_open2(pAudioCodecContext, ptr2, null) < 0)
			{
				m_CurrentState = MEDIAPLAYER_STATE.ERROR;
				throw new ApplicationException("Could not open codec");
			}
		}
		pDecodedFrame = ffmpeg.av_frame_alloc();
		pDecodedAudioFrame = ffmpeg.av_frame_alloc();
		listAudio = new List<float[]>();
		listVideo = new Queue<byte[]>();
		listAudioPts = new List<double>();
		listAudioPtsTime = new List<double>();
		listVideoPts = new Queue<float>();
		if (!m_strFileName.StartsWith("rtsp", StringComparison.OrdinalIgnoreCase))
		{
			action = Interrupt1;
		}
		bVideoFirstFrameReady = false;
		threadVideo = new Thread(ThreadUpdate);
		threadVideo.IsBackground = true;
		threadVideo.Start();
		if (m_bAutoPlay)
		{
			if (OnReady != null)
			{
				OnReady();
			}
			m_CurrentState = MEDIAPLAYER_STATE.PLAYING;
		}
		else
		{
			m_CurrentState = MEDIAPLAYER_STATE.READY;
			if (OnReady != null)
			{
				OnReady();
			}
		}
	}

	private unsafe AVHWAccel* ff_find_hwaccel(AVCodecID codec_id, AVPixelFormat pix_fmt)
	{
		AVHWAccel* ptr = null;
		while ((ptr = ffmpeg.av_hwaccel_next(ptr)) != null)
		{
			if (ptr->id == codec_id && ptr->pix_fmt == pix_fmt)
			{
				return ptr;
			}
		}
		return null;
	}

	public unsafe void Interrupt1()
	{
		pFormatContext->interrupt_callback.callback = (IntPtr)(void*)null;
		bInterrupt = true;
	}

	private static double av_q2d(AVRational a)
	{
		return (double)a.num / (double)a.den;
	}

	private static void DebugMethod(string message)
	{
		Debug.Log("EasyMovieTexture: " + message);
	}

	private void ThreadUpdate()
	{
		while (true)
		{
			if (listVideo != null)
			{
				while (listVideo.Count > 30 || bEnd)
				{
					Thread.Sleep(5);
				}
			}
			UpdateVideo();
			Thread.Sleep(1);
		}
	}

	private unsafe void UpdateVideo()
	{
		int num = 0;
		int num2 = 0;
		if (m_CurrentState == MEDIAPLAYER_STATE.PAUSED)
		{
			return;
		}
		if (pPacket != null)
		{
			if (pPacket->stream_index == iStreamIndex)
			{
				int num3 = ffmpeg.avcodec_decode_video2(pCodecContext, pDecodedFrame, &num, pPacket);
				if (num3 < 0)
				{
					throw new ApplicationException($"Error while decoding frame ");
				}
				if (pPacket->dts != long.MinValue)
				{
					pts = ffmpeg.av_frame_get_best_effort_timestamp(pDecodedFrame);
				}
				else
				{
					pts = 0.0;
				}
				pts *= av_q2d(pStream->time_base);
				if (num == 1)
				{
					if (pts > 0.0 && listVideo.Count > 5 && !m_bIsFirstFrameReady)
					{
						m_bIsFirstFrameReady = true;
						bVideoFirstFrameReady = true;
					}
					sbyte** data = &pDecodedFrame->data0;
					sbyte** data2 = &pConvertedFrame->data0;
					int* fixedElementField = pDecodedFrame->linesize;
					int* fixedElementField2 = pConvertedFrame->linesize;
					ffmpeg.sws_scale(pConvertContext, data, fixedElementField, 0, m_iHeight, data2, fixedElementField2);
					sbyte* data3 = pConvertedFrame->data0;
					IntPtr source = new IntPtr(data3);
					byte[] array = new byte[4 * m_iWidth * m_iHeight];
					Marshal.Copy(source, array, 0, 4 * m_iWidth * m_iHeight);
					lock (listVideo)
					{
						listVideo.Enqueue(array);
						lock (listVideoPts)
						{
							listVideoPts.Enqueue((float)pts);
						}
					}
				}
			}
		}
		else
		{
			pPacket = (AVPacket*)(void*)Marshal.AllocCoTaskMem(sizeof(AVPacket));
			ffmpeg.av_init_packet(pPacket);
		}
		do
		{
			if (pPacket != null)
			{
				ffmpeg.av_free_packet(pPacket);
			}
			int num4 = ffmpeg.av_read_frame(pFormatContext, pPacket);
			if (bInterrupt && listVideo.Count > 20)
			{
				bInterrupt = false;
				pFormatContext->interrupt_callback.callback = Marshal.GetFunctionPointerForDelegate(action);
			}
			if (num4 < 0)
			{
				if (num4 != -541478725)
				{
					throw new ApplicationException("Could not read frame");
				}
				if (listVideo.Count < 3)
				{
					bEnd = true;
					break;
				}
			}
			if (pStreamAudio == null || pPacket->stream_index != iStreamAudioIndex)
			{
				continue;
			}
			int num5 = ffmpeg.avcodec_decode_audio4(pAudioCodecContext, pDecodedAudioFrame, &num2, pPacket);
			if (num5 < 0 || num2 != 1)
			{
				continue;
			}
			int num6 = ffmpeg.av_samples_get_buffer_size(null, pAudioCodecContext->channels, pDecodedAudioFrame->nb_samples, pAudioCodecContext->sample_fmt, 1);
			int num7 = ffmpeg.av_samples_get_buffer_size(null, pAudioCodecContext->channels, pDecodedAudioFrame->nb_samples, AVSampleFormat.AV_SAMPLE_FMT_FLT, 1);
			if (pAudioCodecContext->sample_fmt != AVSampleFormat.AV_SAMPLE_FMT_FLT)
			{
				sbyte* ptr = (sbyte*)(void*)Marshal.AllocCoTaskMem(num7);
				SwrContext* s = null;
				s = ffmpeg.swr_alloc_set_opts(null, (long)pAudioCodecContext->channel_layout, AVSampleFormat.AV_SAMPLE_FMT_FLT, pAudioCodecContext->sample_rate, (long)pAudioCodecContext->channel_layout, pAudioCodecContext->sample_fmt, pAudioCodecContext->sample_rate, 0, null);
				int num8 = 0;
				if ((num8 = ffmpeg.swr_init(s)) < 0)
				{
					Debug.Log("error " + num8);
				}
				ffmpeg.swr_convert(s, &ptr, num7, pDecodedAudioFrame->extended_data, pDecodedAudioFrame->nb_samples);
				sbyte* value = ptr;
				IntPtr source2 = new IntPtr(value);
				byte[] array2 = new byte[num7];
				Marshal.Copy(source2, array2, 0, num7);
				if (pPacket->dts != long.MinValue)
				{
					pts = ffmpeg.av_frame_get_best_effort_timestamp(pDecodedAudioFrame);
				}
				else
				{
					pts = 0.0;
				}
				pts *= av_q2d(pStreamAudio->time_base);
				if (bSeekTo)
				{
					double num9 = pts * (double)pDecodedAudioFrame->sample_rate / ((double)num7 / 4.0 / (double)pDecodedAudioFrame->channels);
					iSoundCount = (int)num9;
					bSeekTo = false;
				}
				while (pts > (double)(600f * (float)(iInitCount + 1)))
				{
					iSoundCount -= (int)(600.0 * (double)pDecodedAudioFrame->sample_rate / ((double)num7 / 4.0 / (double)pDecodedAudioFrame->channels));
					iInitCount++;
				}
				fAudioData = new float[array2.Length / 4];
				Buffer.BlockCopy(array2, 0, fAudioData, 0, array2.Length);
				lock (listAudio)
				{
					listAudio.Add(fAudioData);
					lock (listAudioPts)
					{
						lock (listAudioPtsTime)
						{
							listAudioPts.Add(iSoundCount++ * num7 / 4 / pDecodedAudioFrame->channels);
							listAudioPtsTime.Add(pts);
						}
					}
				}
				ffmpeg.swr_free(&s);
				Marshal.FreeCoTaskMem((IntPtr)ptr);
				continue;
			}
			sbyte* extended_data = *pDecodedAudioFrame->extended_data;
			IntPtr source3 = new IntPtr(extended_data);
			byte[] array3 = new byte[num6];
			Marshal.Copy(source3, array3, 0, num6);
			if (pPacket->dts != long.MinValue)
			{
				pts = ffmpeg.av_frame_get_best_effort_timestamp(pDecodedAudioFrame);
			}
			else
			{
				pts = 0.0;
			}
			pts *= av_q2d(pStreamAudio->time_base);
			fAudioData = new float[array3.Length / 4];
			Buffer.BlockCopy(array3, 0, fAudioData, 0, array3.Length);
			lock (listAudio)
			{
				listAudio.Add(fAudioData);
				lock (listAudioPts)
				{
					lock (listAudioPts)
					{
						listAudioPts.Add(pts);
						listAudioPtsTime.Add(pts);
					}
				}
			}
		}
		while (pPacket->stream_index != iStreamIndex);
	}

	private void OnAudioRead(float[] data)
	{
		if (listAudio.Count < 3)
		{
			return;
		}
		if (iSoundBufferCount == 0)
		{
			iSoundBufferCount = listAudio[0].Length;
		}
		if (iSoundBufferCount < data.Length)
		{
			Array.Copy(listAudio[0], listAudio[0].Length - iSoundBufferCount, data, 0, iSoundBufferCount);
			if (data.Length - iSoundBufferCount > listAudio[1].Length)
			{
				Array.Copy(listAudio[1], 0, data, 0, listAudio[1].Length);
				Array.Copy(listAudio[2], 0, data, iSoundBufferCount + listAudio[1].Length, data.Length - iSoundBufferCount - listAudio[1].Length);
				iSoundBufferCount = listAudio[2].Length - (data.Length - iSoundBufferCount - listAudio[1].Length);
				listAudio.RemoveAt(0);
				listAudioPts.RemoveAt(0);
				listAudioPtsTime.RemoveAt(0);
			}
			else
			{
				Array.Copy(listAudio[1], 0, data, iSoundBufferCount, data.Length - iSoundBufferCount);
				iSoundBufferCount = listAudio[1].Length - (data.Length - iSoundBufferCount);
			}
			listAudio.RemoveAt(0);
			listAudioPts.RemoveAt(0);
			listAudioPtsTime.RemoveAt(0);
		}
		else
		{
			Array.Copy(listAudio[0], listAudio[0].Length - iSoundBufferCount, data, 0, data.Length);
			iSoundBufferCount -= data.Length;
		}
		if (iSoundBufferCount == 0)
		{
			listAudio.RemoveAt(0);
			listAudioPts.RemoveAt(0);
			listAudioPtsTime.RemoveAt(0);
			iSoundBufferCount = listAudio[0].Length;
		}
	}

	private unsafe void Call_UpdateVideoTexture()
	{
		if (bEnd && listVideo.Count == 0)
		{
			m_CurrentState = MEDIAPLAYER_STATE.END;
			if (OnEnd != null)
			{
				OnEnd();
			}
			if (m_bLoop)
			{
				UnityEngine.Object.Destroy(audioClip);
				audioClip = null;
				Load(m_strFileName);
				bEnd = false;
			}
			else
			{
				bEnd = false;
			}
			return;
		}
		if (bInterrupt)
		{
			if (audioSource != null)
			{
				audioSource.Pause();
			}
		}
		else if (audioSource != null && m_CurrentState == MEDIAPLAYER_STATE.PLAYING && m_bIsFirstFrameReady && !audioSource.isPlaying)
		{
			audioSource.Play();
		}
		if (m_CurrentState == MEDIAPLAYER_STATE.PLAYING && m_bIsFirstFrameReady && !bInterrupt && listVideo.Count > 0)
		{
			fCurrentSeekTime += Time.deltaTime * m_fSpeed;
		}
		if (threadVideo == null && m_CurrentState != MEDIAPLAYER_STATE.END && m_CurrentState != 0)
		{
			threadVideo = new Thread(ThreadUpdate);
			threadVideo.IsBackground = true;
			threadVideo.Start();
		}
		if (fLastFrameTime > fCurrentSeekTime - 0.1f)
		{
			for (int i = 0; i < listAudio.Count; i++)
			{
				if (listAudioPtsTime.Count > i && audioSource == null && (int)((float)pAudioCodecContext->sample_rate * ((float)listAudioPtsTime[i] + (float)Call_GetDuration() / 1000f)) > 0)
				{
					audioSource = base.gameObject.AddComponent<AudioSource>();
					audioSource.volume = m_fVolume;
				}
				if (audioClip == null && audioSource != null)
				{
					audioClip = AudioClip.Create("videoAudio", (int)((float)pAudioCodecContext->sample_rate * 600f), pAudioCodecContext->channels, pAudioCodecContext->sample_rate, stream: false);
					audioSource.clip = audioClip;
				}
				if (audioSource != null && Call_GetDuration() > 0 && listAudioPts.Count > i && listAudioPts[i] >= 0.0)
				{
					audioClip.SetData(listAudio[i], (int)(listAudioPts[i] % (double)((float)pAudioCodecContext->sample_rate * 600f)));
				}
			}
			if (audioSource != null && audioSource.isPlaying && Call_GetDuration() > 0)
			{
				listAudio.Clear();
				listAudioPts.Clear();
				listAudioPtsTime.Clear();
			}
			return;
		}
		if (listVideo.Count > 0)
		{
			m_VideoTexture.LoadRawTextureData(listVideo.Dequeue());
			m_VideoTexture.Apply();
		}
		if (listVideoPts.Count > 0)
		{
			float num = listVideoPts.Dequeue();
			if (fLastFrameTime == 0f)
			{
				if (num > fCurrentSeekTime)
				{
					fLastFrameTime = fCurrentSeekTime;
				}
			}
			else
			{
				fLastFrameTime = num;
			}
		}
		if (audioSource != null)
		{
		}
		if (m_TargetMaterial != null)
		{
			for (int j = 0; j < m_TargetMaterial.Length; j++)
			{
				if (!(m_TargetMaterial[j] == null))
				{
					if (m_TargetMaterial[j].GetComponent<MeshRenderer>() != null && m_TargetMaterial[j].GetComponent<MeshRenderer>().material.mainTexture != m_VideoTexture)
					{
						m_TargetMaterial[j].GetComponent<MeshRenderer>().material.mainTexture = m_VideoTexture;
					}
					if (m_TargetMaterial[j].GetComponent<RawImage>() != null && m_TargetMaterial[j].GetComponent<RawImage>().texture != m_VideoTexture)
					{
						m_TargetMaterial[j].GetComponent<RawImage>().texture = m_VideoTexture;
					}
				}
			}
		}
		if (!bVideoFirstFrameReady)
		{
			return;
		}
		if (OnVideoFirstFrameReady != null)
		{
			OnVideoFirstFrameReady();
			bVideoFirstFrameReady = false;
		}
		for (int k = 0; k < listAudio.Count; k++)
		{
			if (audioSource == null)
			{
				audioSource = base.gameObject.AddComponent<AudioSource>();
			}
			if (audioClip == null && audioSource != null)
			{
				audioClip = AudioClip.Create("videoAudio", (int)((float)pAudioCodecContext->sample_rate * 600f), pAudioCodecContext->channels, pAudioCodecContext->sample_rate, stream: false);
				audioSource.clip = audioClip;
			}
			if (audioSource != null && Call_GetDuration() > 0 && listAudioPts.Count > k && listAudioPts[k] >= 0.0)
			{
				audioClip.SetData(listAudio[k], (int)(listAudioPts[k] % (double)((float)pAudioCodecContext->sample_rate * 600f)));
			}
		}
	}

	private void Call_SetVolume(float fVolume)
	{
		if (audioSource != null)
		{
			audioSource.volume = fVolume;
		}
	}

	private unsafe void Call_SetSeekPosition(int iSeek)
	{
		if (threadVideo != null)
		{
			while (threadVideo.IsAlive)
			{
				threadVideo.Abort();
			}
			threadVideo = null;
		}
		bSeekTo = true;
		iInitCount = 0;
		long num = (long)iSeek * 1000L;
		Debug.Log(num);
		num = ffmpeg.av_rescale_q(num, ffmpeg.av_get_time_base_q(), pStream->time_base);
		Debug.Log(num);
		if (ffmpeg.av_seek_frame(pFormatContext, iStreamIndex, num, 1) < 0)
		{
		}
		fCurrentSeekTime = (float)iSeek / 1000f;
		fLastFrameTime = 0f;
		listVideo.Clear();
		listVideoPts.Clear();
		ffmpeg.avcodec_flush_buffers(pCodecContext);
	}

	private int Call_GetSeekPosition()
	{
		return (int)(fCurrentSeekTime * 1000f);
	}

	private void Call_Play(int iSeek)
	{
		if (m_CurrentState != MEDIAPLAYER_STATE.READY && m_CurrentState != MEDIAPLAYER_STATE.STOPPED && m_CurrentState != MEDIAPLAYER_STATE.END && m_CurrentState != MEDIAPLAYER_STATE.PAUSED)
		{
			return;
		}
		SeekTo(iSeek);
		if (audioSource != null)
		{
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
			audioSource.time = (float)iSeek / 1000f;
		}
		m_CurrentState = MEDIAPLAYER_STATE.PLAYING;
	}

	private void Call_Reset()
	{
	}

	private void Call_Stop()
	{
		SeekTo(0);
		if (audioSource != null)
		{
			audioSource.Stop();
			audioSource.time = 0f;
		}
		m_CurrentState = MEDIAPLAYER_STATE.STOPPED;
	}

	private void Call_RePlay()
	{
		if (audioSource != null && !audioSource.isPlaying)
		{
			audioSource.Play();
		}
		m_CurrentState = MEDIAPLAYER_STATE.PLAYING;
	}

	private void Call_Pause()
	{
		if (audioSource != null)
		{
			audioSource.Pause();
		}
		m_CurrentState = MEDIAPLAYER_STATE.PAUSED;
	}

	private int Call_GetVideoWidth()
	{
		return m_iWidth;
	}

	private int Call_GetVideoHeight()
	{
		return m_iHeight;
	}

	private void Call_SetUnityTexture(int iTextureID)
	{
	}

	private void Call_SetWindowSize()
	{
	}

	private void Call_SetLooping(bool bLoop)
	{
	}

	private void Call_SetRockchip(bool bValue)
	{
	}

	public void Call_SetUnityActivity()
	{
		ffmpeg.av_register_all();
		ffmpeg.avcodec_register_all();
		ffmpeg.avformat_network_init();
		m_iID = SetTexture();
	}

	private int Call_GetError()
	{
		return 0;
	}

	private int Call_GetErrorExtra()
	{
		return 0;
	}

	private unsafe int Call_GetDuration()
	{
		if (pFormatContext != null)
		{
			return (int)(pFormatContext->duration / 1000);
		}
		return 0;
	}

	private int Call_GetCurrentSeekPercent()
	{
		return -1;
	}

	private void Call_SetSplitOBB(bool bValue, string strOBBName)
	{
	}

	private void Call_SetSpeed(float fSpeed)
	{
		if (audioSource != null)
		{
			audioSource.pitch = fSpeed;
		}
	}

	private MEDIAPLAYER_STATE Call_GetStatus()
	{
		return m_CurrentState;
	}

	public IEnumerator DownloadStreamingVideoAndLoad(string strURL)
	{
		strURL = strURL.Trim();
		Debug.Log("DownloadStreamingVideo : " + strURL);
		WWW www2 = new WWW(strURL);
		yield return www2;
		if (string.IsNullOrEmpty(www2.error))
		{
			if (!Directory.Exists(Application.persistentDataPath + "/Data"))
			{
				Directory.CreateDirectory(Application.persistentDataPath + "/Data");
			}
			string text = Application.persistentDataPath + "/Data" + strURL.Substring(strURL.LastIndexOf("/"));
			File.WriteAllBytes(text, www2.bytes);
			Load("file://" + text);
		}
		else
		{
			Debug.Log(www2.error);
		}
		www2.Dispose();
		www2 = null;
		Resources.UnloadUnusedAssets();
	}

	public IEnumerator DownloadStreamingVideoAndLoad2(string strURL)
	{
		strURL = strURL.Trim();
		string write_path = Application.persistentDataPath + "/Data" + strURL.Substring(strURL.LastIndexOf("/"));
		if (File.Exists(write_path))
		{
			Load("file://" + write_path);
			yield break;
		}
		WWW www2 = new WWW(strURL);
		yield return www2;
		if (string.IsNullOrEmpty(www2.error))
		{
			if (!Directory.Exists(Application.persistentDataPath + "/Data"))
			{
				Directory.CreateDirectory(Application.persistentDataPath + "/Data");
			}
			File.WriteAllBytes(write_path, www2.bytes);
			Load("file://" + write_path);
		}
		else
		{
			Debug.Log(www2.error);
		}
		www2.Dispose();
		www2 = null;
		Resources.UnloadUnusedAssets();
	}

	private IEnumerator CopyStreamingAssetVideoAndLoad(string strURL)
	{
		strURL = strURL.Trim();
		string write_path = Application.persistentDataPath + "/" + strURL;
		if (!File.Exists(write_path))
		{
			Debug.Log("CopyStreamingAssetVideoAndLoad : " + strURL);
			WWW www2 = new WWW(Application.streamingAssetsPath + "/" + strURL);
			yield return www2;
			if (string.IsNullOrEmpty(www2.error))
			{
				Debug.Log(write_path);
				File.WriteAllBytes(write_path, www2.bytes);
				Load("file://" + write_path);
			}
			else
			{
				Debug.Log(www2.error);
			}
			www2.Dispose();
			www2 = null;
		}
		else
		{
			Load("file://" + write_path);
		}
	}

	private void CheckThreading()
	{
		lock (thisLock)
		{
			if (unityMainThreadActionList.Count <= 0)
			{
				return;
			}
			foreach (Action unityMainThreadAction in unityMainThreadActionList)
			{
				unityMainThreadAction();
			}
			unityMainThreadActionList.Clear();
		}
	}

	private void AddActionForUnityMainThread(Action a)
	{
		lock (thisLock)
		{
			unityMainThreadActionList.Add(a);
		}
		checkNewActions = true;
	}
}
