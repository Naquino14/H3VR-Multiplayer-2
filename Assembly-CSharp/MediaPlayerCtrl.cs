// Decompiled with JetBrains decompiler
// Type: MediaPlayerCtrl
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FFmpeg.AutoGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MediaPlayerCtrl : MonoBehaviour
{
  public string m_strFileName;
  public GameObject[] m_TargetMaterial;
  private Texture2D m_VideoTexture;
  private Texture2D m_VideoTextureDummy;
  private MediaPlayerCtrl.MEDIAPLAYER_STATE m_CurrentState;
  private int m_iCurrentSeekPosition;
  private float m_fVolume = 1f;
  private int m_iWidth;
  private int m_iHeight;
  private float m_fSpeed = 1f;
  public bool m_bFullScreen;
  public bool m_bSupportRockchip = true;
  public MediaPlayerCtrl.VideoResize OnResize;
  public MediaPlayerCtrl.VideoReady OnReady;
  public MediaPlayerCtrl.VideoEnd OnEnd;
  public MediaPlayerCtrl.VideoError OnVideoError;
  public MediaPlayerCtrl.VideoFirstFrameReady OnVideoFirstFrameReady;
  private IntPtr m_texPtr;
  private int m_iAndroidMgrID;
  private bool m_bIsFirstFrameReady;
  private bool m_bFirst;
  public MediaPlayerCtrl.MEDIA_SCALE m_ScaleValue;
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
  private MediaPlayerCtrl.VideoInterrupt action;
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

  [DllImport("EasyMovieTexture")]
  private static extern int SetTexture();

  [DllImport("EasyMovieTexture")]
  private static extern void ReleaseTexture(int iID);

  [DllImport("EasyMovieTexture")]
  private static extern void SetTextureFromUnity(
    int iID,
    IntPtr texture,
    int w,
    int h,
    byte[] data);

  [DllImport("EasyMovieTexture")]
  private static extern IntPtr GetRenderEventFunc();

  private void Awake()
  {
    string environmentVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
    string str = Application.dataPath + (object) Path.DirectorySeparatorChar + "Plugins";
    if (!environmentVariable.Contains(str))
      Environment.SetEnvironmentVariable("PATH", environmentVariable + (object) Path.PathSeparator + str, EnvironmentVariableTarget.Process);
    this.m_bSupportRockchip = SystemInfo.deviceModel.Contains("rockchip");
    if (this.m_TargetMaterial != null)
    {
      for (int index1 = 0; index1 < this.m_TargetMaterial.Length; ++index1)
      {
        if ((UnityEngine.Object) this.m_TargetMaterial[index1] != (UnityEngine.Object) null)
        {
          if ((UnityEngine.Object) this.m_TargetMaterial[index1].GetComponent<MeshFilter>() != (UnityEngine.Object) null)
          {
            Vector2[] uv = this.m_TargetMaterial[index1].GetComponent<MeshFilter>().mesh.uv;
            for (int index2 = 0; index2 < uv.Length; ++index2)
              uv[index2] = new Vector2(uv[index2].x, 1f - uv[index2].y);
            this.m_TargetMaterial[index1].GetComponent<MeshFilter>().mesh.uv = uv;
          }
          if ((UnityEngine.Object) this.m_TargetMaterial[index1].GetComponent<RawImage>() != (UnityEngine.Object) null)
            this.m_TargetMaterial[index1].GetComponent<RawImage>().uvRect = new Rect(0.0f, 1f, 1f, -1f);
        }
      }
    }
    this.Call_SetUnityActivity();
  }

  private void Start() => this.m_bInit = true;

  private void OnApplicationQuit()
  {
  }

  private void OnDisable()
  {
    if (this.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
      return;
    this.Pause();
  }

  private void OnEnable()
  {
    if (this.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
      return;
    this.Play();
  }

  private void Update()
  {
    if (string.IsNullOrEmpty(this.m_strFileName))
      return;
    if (this.checkNewActions)
    {
      this.checkNewActions = false;
      this.CheckThreading();
    }
    if (!this.m_bFirst)
    {
      this.Call_Load(this.m_strFileName.Trim(), 0);
      this.Call_SetLooping(this.m_bLoop);
      this.m_bFirst = true;
    }
    if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
    {
      if (!this.m_bCheckFBO)
      {
        if (this.Call_GetVideoWidth() <= 0 || this.Call_GetVideoHeight() <= 0)
          return;
        this.m_iWidth = this.Call_GetVideoWidth();
        this.m_iHeight = this.Call_GetVideoHeight();
        this.Resize();
        if ((UnityEngine.Object) this.m_VideoTexture != (UnityEngine.Object) null)
        {
          if ((UnityEngine.Object) this.m_VideoTextureDummy != (UnityEngine.Object) null)
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_VideoTextureDummy);
            this.m_VideoTextureDummy = (Texture2D) null;
          }
          this.m_VideoTextureDummy = this.m_VideoTexture;
          this.m_VideoTexture = (Texture2D) null;
        }
        this.m_VideoTexture = !this.m_bSupportRockchip ? new Texture2D(this.Call_GetVideoWidth(), this.Call_GetVideoHeight(), TextureFormat.RGBA32, false) : new Texture2D(this.Call_GetVideoWidth(), this.Call_GetVideoHeight(), TextureFormat.RGB565, false);
        this.m_VideoTexture.filterMode = FilterMode.Bilinear;
        this.m_VideoTexture.wrapMode = TextureWrapMode.Clamp;
        this.m_texPtr = this.m_VideoTexture.GetNativeTexturePtr();
        this.Call_SetUnityTexture(this.m_VideoTexture.GetNativeTextureID());
        this.Call_SetWindowSize();
        this.m_bCheckFBO = true;
        if (this.OnResize != null)
          this.OnResize();
      }
      else if (this.Call_GetVideoWidth() != this.m_iWidth || this.Call_GetVideoHeight() != this.m_iHeight)
      {
        this.m_iWidth = this.Call_GetVideoWidth();
        this.m_iHeight = this.Call_GetVideoHeight();
        if (this.OnResize != null)
          this.OnResize();
        this.ResizeTexture();
      }
      this.Call_UpdateVideoTexture();
      this.m_iCurrentSeekPosition = this.Call_GetSeekPosition();
    }
    if (this.m_CurrentState != this.Call_GetStatus())
    {
      this.m_CurrentState = this.Call_GetStatus();
      if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY)
      {
        if (this.OnReady != null)
          this.OnReady();
        if (this.m_bAutoPlay)
          this.Call_Play(0);
        if (this.m_bReadyPlay)
        {
          this.Call_Play(0);
          this.m_bReadyPlay = false;
        }
        this.SetVolume(this.m_fVolume);
      }
      else if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
      {
        if (this.OnEnd != null)
          this.OnEnd();
        if (this.m_bLoop)
          this.Call_Play(0);
      }
      else if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR)
        this.OnError((MediaPlayerCtrl.MEDIAPLAYER_ERROR) this.Call_GetError(), (MediaPlayerCtrl.MEDIAPLAYER_ERROR) this.Call_GetErrorExtra());
    }
    GL.InvalidateState();
  }

  public void DeleteVideoTexture()
  {
    if ((UnityEngine.Object) this.m_VideoTextureDummy != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_VideoTextureDummy);
      this.m_VideoTextureDummy = (Texture2D) null;
    }
    if (!((UnityEngine.Object) this.m_VideoTexture != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_VideoTexture);
    this.m_VideoTexture = (Texture2D) null;
  }

  public void ResizeTexture()
  {
    UnityEngine.Debug.Log((object) ("ResizeTexture " + (object) this.m_iWidth + " " + (object) this.m_iHeight));
    if (this.m_iWidth == 0 || this.m_iHeight == 0)
      return;
    if ((UnityEngine.Object) this.m_VideoTexture != (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) this.m_VideoTextureDummy != (UnityEngine.Object) null)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_VideoTextureDummy);
        this.m_VideoTextureDummy = (Texture2D) null;
      }
      this.m_VideoTextureDummy = this.m_VideoTexture;
      this.m_VideoTexture = (Texture2D) null;
    }
    this.m_VideoTexture = !this.m_bSupportRockchip ? new Texture2D(this.Call_GetVideoWidth(), this.Call_GetVideoHeight(), TextureFormat.RGBA32, false) : new Texture2D(this.Call_GetVideoWidth(), this.Call_GetVideoHeight(), TextureFormat.RGB565, false);
    this.m_VideoTexture.filterMode = FilterMode.Bilinear;
    this.m_VideoTexture.wrapMode = TextureWrapMode.Clamp;
    this.m_texPtr = this.m_VideoTexture.GetNativeTexturePtr();
    this.Call_SetUnityTexture(this.m_VideoTexture.GetNativeTextureID());
    this.Call_SetWindowSize();
  }

  public void Resize()
  {
    if (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.Call_GetVideoWidth() <= 0 || (this.Call_GetVideoHeight() <= 0 || this.m_objResize == null))
      return;
    float num1 = (float) Screen.height / (float) Screen.width;
    int videoWidth = this.Call_GetVideoWidth();
    float num2 = (float) this.Call_GetVideoHeight() / (float) videoWidth;
    float num3 = num1 / num2;
    for (int index = 0; index < this.m_objResize.Length; ++index)
    {
      if (!((UnityEngine.Object) this.m_objResize[index] == (UnityEngine.Object) null))
      {
        if (this.m_bFullScreen)
        {
          this.m_objResize[index].transform.localScale = new Vector3(20f / num1, 20f / num1, 1f);
          if ((double) num2 < 1.0)
          {
            if ((double) num1 < 1.0 && (double) num2 > (double) num1)
              this.m_objResize[index].transform.localScale *= num3;
            this.m_ScaleValue = MediaPlayerCtrl.MEDIA_SCALE.SCALE_X_TO_Y;
          }
          else
          {
            if ((double) num1 > 1.0)
            {
              if ((double) num2 >= (double) num1)
                this.m_objResize[index].transform.localScale *= num3;
            }
            else
              this.m_objResize[index].transform.localScale *= num3;
            this.m_ScaleValue = MediaPlayerCtrl.MEDIA_SCALE.SCALE_X_TO_Y;
          }
        }
        this.m_objResize[index].transform.localScale = this.m_ScaleValue != MediaPlayerCtrl.MEDIA_SCALE.SCALE_X_TO_Y ? (this.m_ScaleValue != MediaPlayerCtrl.MEDIA_SCALE.SCALE_X_TO_Y_2 ? (this.m_ScaleValue != MediaPlayerCtrl.MEDIA_SCALE.SCALE_X_TO_Z ? (this.m_ScaleValue != MediaPlayerCtrl.MEDIA_SCALE.SCALE_Y_TO_X ? (this.m_ScaleValue != MediaPlayerCtrl.MEDIA_SCALE.SCALE_Y_TO_Z ? (this.m_ScaleValue != MediaPlayerCtrl.MEDIA_SCALE.SCALE_Z_TO_X ? (this.m_ScaleValue != MediaPlayerCtrl.MEDIA_SCALE.SCALE_Z_TO_Y ? new Vector3(this.m_objResize[index].transform.localScale.x, this.m_objResize[index].transform.localScale.y, this.m_objResize[index].transform.localScale.z) : new Vector3(this.m_objResize[index].transform.localScale.x, this.m_objResize[index].transform.localScale.z * num2, this.m_objResize[index].transform.localScale.z)) : new Vector3(this.m_objResize[index].transform.localScale.z * num2, this.m_objResize[index].transform.localScale.y, this.m_objResize[index].transform.localScale.z)) : new Vector3(this.m_objResize[index].transform.localScale.x, this.m_objResize[index].transform.localScale.y, this.m_objResize[index].transform.localScale.y / num2)) : new Vector3(this.m_objResize[index].transform.localScale.y / num2, this.m_objResize[index].transform.localScale.y, this.m_objResize[index].transform.localScale.z)) : new Vector3(this.m_objResize[index].transform.localScale.x, this.m_objResize[index].transform.localScale.y, this.m_objResize[index].transform.localScale.x * num2)) : new Vector3(this.m_objResize[index].transform.localScale.x, (float) ((double) this.m_objResize[index].transform.localScale.x * (double) num2 / 2.0), this.m_objResize[index].transform.localScale.z)) : new Vector3(this.m_objResize[index].transform.localScale.x, this.m_objResize[index].transform.localScale.x * num2, this.m_objResize[index].transform.localScale.z);
      }
    }
  }

  private void OnError(
    MediaPlayerCtrl.MEDIAPLAYER_ERROR iCode,
    MediaPlayerCtrl.MEDIAPLAYER_ERROR iCodeExtra)
  {
    string empty = string.Empty;
    string str1;
    switch (iCode)
    {
      case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_UNKNOWN:
        str1 = "MEDIA_ERROR_UNKNOWN";
        break;
      case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_SERVER_DIED:
        str1 = "MEDIA_ERROR_SERVER_DIED";
        break;
      case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK:
        str1 = "MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK";
        break;
      default:
        str1 = "Unknown error " + (object) iCode;
        break;
    }
    string str2 = str1 + " ";
    string str3;
    switch (iCodeExtra + 1010)
    {
      case (MediaPlayerCtrl.MEDIAPLAYER_ERROR) 0:
        str3 = str2 + "MEDIA_ERROR_UNSUPPORTED";
        break;
      case (MediaPlayerCtrl.MEDIAPLAYER_ERROR) 3:
        str3 = str2 + "MEDIA_ERROR_MALFORMED";
        break;
      default:
        switch (iCodeExtra)
        {
          case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_IO:
            str3 = str2 + "MEDIA_ERROR_IO";
            break;
          case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_TIMED_OUT:
            str3 = str2 + "MEDIA_ERROR_TIMED_OUT";
            break;
          default:
            str3 = "Unknown error " + (object) iCode;
            break;
        }
        break;
    }
    UnityEngine.Debug.LogError((object) str3);
    if (this.OnVideoError == null)
      return;
    this.OnVideoError(iCode, iCodeExtra);
  }

  private void OnDestroy()
  {
    this.Call_UnLoad();
    if ((UnityEngine.Object) this.m_VideoTextureDummy != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_VideoTextureDummy);
      this.m_VideoTextureDummy = (Texture2D) null;
    }
    if ((UnityEngine.Object) this.m_VideoTexture != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_VideoTexture);
    this.Call_Destroy();
  }

  private void OnApplicationPause(bool bPause)
  {
    UnityEngine.Debug.Log((object) ("ApplicationPause : " + (object) bPause));
    if (bPause)
    {
      if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
        this.m_bPause = true;
      this.Call_Pause();
    }
    else
    {
      this.Call_RePlay();
      if (!this.m_bPause)
        return;
      this.Call_Pause();
      this.m_bPause = false;
    }
  }

  public MediaPlayerCtrl.MEDIAPLAYER_STATE GetCurrentState() => this.m_CurrentState;

  public Texture2D GetVideoTexture() => this.m_VideoTexture;

  public void Play()
  {
    if (this.m_bStop)
    {
      this.SeekTo(0);
      this.Call_Play(0);
      this.m_bStop = false;
    }
    if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
      this.Call_RePlay();
    else if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
    {
      this.Call_Play(0);
    }
    else
    {
      if (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
        return;
      this.m_bReadyPlay = true;
    }
  }

  public void Stop()
  {
    if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
      this.Call_Pause();
    this.m_bStop = true;
    this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED;
    this.m_iCurrentSeekPosition = 0;
  }

  public void Pause()
  {
    if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
      this.Call_Pause();
    this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED;
  }

  public void Load(string strFileName)
  {
    if (this.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
      this.UnLoad();
    this.m_bReadyPlay = false;
    this.m_bIsFirstFrameReady = false;
    this.m_bFirst = false;
    this.m_bCheckFBO = false;
    this.m_strFileName = strFileName;
    if (!this.m_bInit)
      return;
    this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY;
  }

  public void SetVolume(float fVolume)
  {
    if (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED && (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.END && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.READY) && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED)
      return;
    this.m_fVolume = fVolume;
    this.Call_SetVolume(fVolume);
  }

  public int GetSeekPosition() => this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END ? this.m_iCurrentSeekPosition : 0;

  public void SeekTo(int iSeek)
  {
    if (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.READY && (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.END) && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED)
      return;
    this.Call_SetSeekPosition(iSeek);
  }

  public void SetSpeed(float fSpeed)
  {
    if (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.READY && (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.END) && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED)
      return;
    this.m_fSpeed = fSpeed;
    this.Call_SetSpeed(fSpeed);
  }

  public int GetDuration() => this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED || (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY) || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED ? this.Call_GetDuration() : 0;

  public float GetSeekBarValue() => (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED || (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY) || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED) && this.GetDuration() != 0 ? (float) this.GetSeekPosition() / (float) this.GetDuration() : 0.0f;

  public void SetSeekBarValue(float fValue)
  {
    if (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED && (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.END && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.READY) && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED || this.GetDuration() == 0)
      return;
    this.SeekTo((int) ((double) this.GetDuration() * (double) fValue));
  }

  public int GetCurrentSeekPercent() => this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED || (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY) ? this.Call_GetCurrentSeekPercent() : 0;

  public int GetVideoWidth() => this.Call_GetVideoWidth();

  public int GetVideoHeight() => this.Call_GetVideoHeight();

  public void UnLoad()
  {
    this.m_bCheckFBO = false;
    this.Call_UnLoad();
    this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY;
  }

  private void Call_Destroy()
  {
    if (this.loader != null)
    {
      while (this.loader.IsAlive)
        this.loader.Abort();
      this.loader = (Thread) null;
    }
    if (this.threadVideo != null)
    {
      while (this.threadVideo.IsAlive)
        this.threadVideo.Abort();
      this.threadVideo = (Thread) null;
    }
    ffmpeg.avformat_network_deinit();
    MediaPlayerCtrl.ReleaseTexture(this.m_iID);
  }

  private unsafe void Call_UnLoad()
  {
    this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY;
    if (this.loader != null)
    {
      while (this.loader.IsAlive)
        this.loader.Abort();
      this.loader = (Thread) null;
    }
    if (this.threadVideo != null)
    {
      while (this.threadVideo.IsAlive)
        this.threadVideo.Abort();
      this.threadVideo = (Thread) null;
    }
    if (this.listAudio != null)
    {
      this.listAudio.Clear();
      this.listAudio = (List<float[]>) null;
    }
    if (this.listVideo != null)
    {
      this.listVideo.Clear();
      this.listVideo = (Queue<byte[]>) null;
    }
    if (this.listAudioPts != null)
    {
      this.listAudioPts.Clear();
      this.listAudioPts = (List<double>) null;
    }
    if (this.listAudioPtsTime != null)
    {
      this.listAudioPtsTime.Clear();
      this.listAudioPtsTime = (List<double>) null;
    }
    if (this.listVideoPts != null)
    {
      this.listVideoPts.Clear();
      this.listVideoPts = (Queue<float>) null;
    }
    this.fCurrentSeekTime = 0.0f;
    this.fLastFrameTime = 0.0f;
    if ((IntPtr) this.pPacket != IntPtr.Zero)
    {
      ffmpeg.av_free_packet(this.pPacket);
      Marshal.FreeCoTaskMem((IntPtr) (void*) this.pPacket);
      this.pPacket = (AVPacket*) null;
    }
    if ((IntPtr) this.pConvertedFrame != IntPtr.Zero)
    {
      ffmpeg.av_free((void*) this.pConvertedFrame);
      this.pConvertedFrame = (AVFrame*) null;
    }
    if ((IntPtr) this.pConvertedFrameBuffer != IntPtr.Zero)
    {
      ffmpeg.av_free((void*) this.pConvertedFrameBuffer);
      this.pConvertedFrameBuffer = (sbyte*) null;
    }
    if ((IntPtr) this.pConvertContext != IntPtr.Zero)
    {
      ffmpeg.sws_freeContext(this.pConvertContext);
      this.pConvertContext = (SwsContext*) null;
    }
    if ((IntPtr) this.pDecodedFrame != IntPtr.Zero)
    {
      ffmpeg.av_free((void*) this.pDecodedFrame);
      this.pDecodedFrame = (AVFrame*) null;
    }
    if ((IntPtr) this.pDecodedAudioFrame != IntPtr.Zero)
      ffmpeg.av_free((void*) this.pDecodedAudioFrame);
    this.pDecodedAudioFrame = (AVFrame*) null;
    if ((IntPtr) this.pCodecContext != IntPtr.Zero)
      ffmpeg.avcodec_close(this.pCodecContext);
    this.pCodecContext = (AVCodecContext*) null;
    if ((IntPtr) this.pAudioCodecContext != IntPtr.Zero)
      ffmpeg.avcodec_close(this.pAudioCodecContext);
    this.pAudioCodecContext = (AVCodecContext*) null;
    if ((IntPtr) this.pFormatContext != IntPtr.Zero)
      ffmpeg.avformat_close_input(&this.pFormatContext);
    this.pFormatContext = (AVFormatContext*) null;
    if ((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null)
      this.audioSource.Stop();
    if (!((UnityEngine.Object) this.audioClip != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.audioClip);
    this.audioClip = (AudioClip) null;
  }

  private unsafe bool Call_Load(string strFileName, int iSeek)
  {
    this.fCurrentSeekTime = 0.0f;
    this.fLastFrameTime = 0.0f;
    this.iSoundCount = 0;
    this.iInitCount = 0;
    this.bSeekTo = true;
    this.bEnd = false;
    if ((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null)
    {
      this.audioSource.Stop();
      this.audioSource.time = 0.0f;
    }
    if ((UnityEngine.Object) this.audioClip != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.audioClip);
      this.audioClip = (AudioClip) null;
    }
    this.pFormatContext = ffmpeg.avformat_alloc_context();
    if (!strFileName.Contains("://"))
    {
      strFileName = Application.streamingAssetsPath + "/" + strFileName;
      UnityEngine.Debug.Log((object) strFileName);
    }
    else if (strFileName.Contains("file://"))
      strFileName = strFileName.Replace("file://", string.Empty);
    this.loader = new Thread((ThreadStart) (() =>
    {
      AVFormatContext* avFormatContextPtr = (AVFormatContext*) null;
      if (ffmpeg.avformat_open_input(&avFormatContextPtr, strFileName, (AVInputFormat*) null, (AVDictionary**) null) != 0)
      {
        this.pFormatContext = (AVFormatContext*) null;
        this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR;
        throw new ApplicationException("Could not open file");
      }
      this.pFormatContext = avFormatContextPtr;
      if (ffmpeg.avformat_find_stream_info(this.pFormatContext, (AVDictionary**) null) != 0)
      {
        this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR;
        throw new ApplicationException("Could not find stream info");
      }
      this.AddActionForUnityMainThread((Action) (() => this.LoadVideoPart2()));
    }));
    this.loader.Priority = System.Threading.ThreadPriority.AboveNormal;
    this.loader.IsBackground = true;
    this.loader.Start();
    return true;
  }

  private unsafe void LoadVideoPart2()
  {
    this.pStream = (AVStream*) null;
    this.pStreamAudio = (AVStream*) null;
    bool flag = false;
    for (int index = 0; (long) index < (long) this.pFormatContext->nb_streams; ++index)
    {
      if (this.pFormatContext->streams[index]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
      {
        if (!flag)
        {
          flag = true;
          this.pStream = this.pFormatContext->streams[index];
          this.iStreamIndex = index;
          UnityEngine.Debug.Log((object) ("Video" + (object) this.iStreamIndex));
        }
      }
      else if (this.pFormatContext->streams[index]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
      {
        this.pStreamAudio = this.pFormatContext->streams[index];
        this.iStreamAudioIndex = index;
      }
    }
    if ((IntPtr) this.pStream == IntPtr.Zero)
    {
      this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR;
      throw new ApplicationException("Could not found video stream");
    }
    if ((IntPtr) this.pStreamAudio == IntPtr.Zero)
      this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR;
    AVCodecContext avCodecContext = *this.pStream->codec;
    this.m_iWidth = avCodecContext.width;
    this.m_iHeight = avCodecContext.height;
    AVPixelFormat pixFmt = avCodecContext.pix_fmt;
    AVCodecID codecId = avCodecContext.codec_id;
    AVPixelFormat avPixelFormat = AVPixelFormat.AV_PIX_FMT_RGBA;
    if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D9)
      avPixelFormat = AVPixelFormat.AV_PIX_FMT_BGRA;
    this.pConvertContext = ffmpeg.sws_getContext(this.m_iWidth, this.m_iHeight, pixFmt, this.m_iWidth, this.m_iHeight, avPixelFormat, 1, (SwsFilter*) null, (SwsFilter*) null, (double*) null);
    if ((IntPtr) this.pConvertContext == IntPtr.Zero)
    {
      this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR;
      throw new ApplicationException("Could not initialize the conversion context");
    }
    this.pConvertedFrame = ffmpeg.av_frame_alloc();
    this.pConvertedFrameBuffer = (sbyte*) ffmpeg.av_malloc((ulong) ffmpeg.avpicture_get_size(avPixelFormat, this.m_iWidth, this.m_iHeight));
    ffmpeg.avpicture_fill((AVPicture*) this.pConvertedFrame, this.pConvertedFrameBuffer, avPixelFormat, this.m_iWidth, this.m_iHeight);
    AVCodec* decoder1 = ffmpeg.avcodec_find_decoder(codecId);
    if ((IntPtr) decoder1 == IntPtr.Zero)
    {
      this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR;
      throw new ApplicationException("Unsupported codec");
    }
    this.pCodecContext = this.pStream->codec;
    this.pCodecContext->hwaccel = this.ff_find_hwaccel(this.pCodecContext->codec_id, pixFmt);
    if (ffmpeg.avcodec_open2(this.pCodecContext, decoder1, (AVDictionary**) null) < 0)
    {
      this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR;
      throw new ApplicationException("Could not open codec");
    }
    if ((IntPtr) this.pStreamAudio != IntPtr.Zero)
    {
      AVCodec* decoder2 = ffmpeg.avcodec_find_decoder(this.pStreamAudio->codec->codec_id);
      if ((IntPtr) decoder2 == IntPtr.Zero)
      {
        this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR;
        throw new ApplicationException("Unsupported codec");
      }
      this.pAudioCodecContext = this.pStreamAudio->codec;
      if (ffmpeg.avcodec_open2(this.pAudioCodecContext, decoder2, (AVDictionary**) null) < 0)
      {
        this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR;
        throw new ApplicationException("Could not open codec");
      }
    }
    this.pDecodedFrame = ffmpeg.av_frame_alloc();
    this.pDecodedAudioFrame = ffmpeg.av_frame_alloc();
    this.listAudio = new List<float[]>();
    this.listVideo = new Queue<byte[]>();
    this.listAudioPts = new List<double>();
    this.listAudioPtsTime = new List<double>();
    this.listVideoPts = new Queue<float>();
    if (!this.m_strFileName.StartsWith("rtsp", StringComparison.OrdinalIgnoreCase))
      this.action = new MediaPlayerCtrl.VideoInterrupt(this.Interrupt1);
    this.bVideoFirstFrameReady = false;
    this.threadVideo = new Thread(new ThreadStart(this.ThreadUpdate));
    this.threadVideo.IsBackground = true;
    this.threadVideo.Start();
    if (this.m_bAutoPlay)
    {
      if (this.OnReady != null)
        this.OnReady();
      this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING;
    }
    else
    {
      this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.READY;
      if (this.OnReady == null)
        return;
      this.OnReady();
    }
  }

  private unsafe AVHWAccel* ff_find_hwaccel(AVCodecID codec_id, AVPixelFormat pix_fmt)
  {
    AVHWAccel* hwaccel = (AVHWAccel*) null;
    while ((IntPtr) (hwaccel = ffmpeg.av_hwaccel_next(hwaccel)) != IntPtr.Zero)
    {
      if (hwaccel->id == codec_id && hwaccel->pix_fmt == pix_fmt)
        return hwaccel;
    }
    return (AVHWAccel*) null;
  }

  public unsafe void Interrupt1()
  {
    this.pFormatContext->interrupt_callback.callback = (IntPtr) (void*) null;
    this.bInterrupt = true;
  }

  private static double av_q2d(AVRational a) => (double) a.num / (double) a.den;

  private static void DebugMethod(string message) => UnityEngine.Debug.Log((object) ("EasyMovieTexture: " + message));

  private void ThreadUpdate()
  {
    while (true)
    {
      if (this.listVideo != null)
      {
        while (this.listVideo.Count > 30 || this.bEnd)
          Thread.Sleep(5);
      }
      this.UpdateVideo();
      Thread.Sleep(1);
    }
  }

  private unsafe void UpdateVideo()
  {
    int num1 = 0;
    int num2 = 0;
    if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
      return;
    if ((IntPtr) this.pPacket != IntPtr.Zero)
    {
      if (this.pPacket->stream_index == this.iStreamIndex)
      {
        if (ffmpeg.avcodec_decode_video2(this.pCodecContext, this.pDecodedFrame, &num1, this.pPacket) < 0)
          throw new ApplicationException(string.Format("Error while decoding frame "));
        this.pts = this.pPacket->dts == long.MinValue ? 0.0 : (double) ffmpeg.av_frame_get_best_effort_timestamp(this.pDecodedFrame);
        this.pts *= MediaPlayerCtrl.av_q2d(this.pStream->time_base);
        if (num1 == 1)
        {
          if (this.pts > 0.0 && this.listVideo.Count > 5 && !this.m_bIsFirstFrameReady)
          {
            this.m_bIsFirstFrameReady = true;
            this.bVideoFirstFrameReady = true;
          }
          sbyte** srcSlice = &this.pDecodedFrame->data0;
          sbyte** dst = &this.pConvertedFrame->data0;
          int* linesize1 = this.pDecodedFrame->linesize;
          int* linesize2 = this.pConvertedFrame->linesize;
          ffmpeg.sws_scale(this.pConvertContext, srcSlice, linesize1, 0, this.m_iHeight, dst, linesize2);
          IntPtr source = new IntPtr((void*) this.pConvertedFrame->data0);
          byte[] destination = new byte[4 * this.m_iWidth * this.m_iHeight];
          Marshal.Copy(source, destination, 0, 4 * this.m_iWidth * this.m_iHeight);
          lock ((object) this.listVideo)
          {
            this.listVideo.Enqueue(destination);
            lock ((object) this.listVideoPts)
              this.listVideoPts.Enqueue((float) this.pts);
          }
        }
      }
    }
    else
    {
      this.pPacket = (AVPacket*) (void*) Marshal.AllocCoTaskMem(sizeof (AVPacket));
      ffmpeg.av_init_packet(this.pPacket);
    }
    do
    {
      if ((IntPtr) this.pPacket != IntPtr.Zero)
        ffmpeg.av_free_packet(this.pPacket);
      int num3 = ffmpeg.av_read_frame(this.pFormatContext, this.pPacket);
      if (this.bInterrupt && this.listVideo.Count > 20)
      {
        this.bInterrupt = false;
        this.pFormatContext->interrupt_callback.callback = Marshal.GetFunctionPointerForDelegate((Delegate) this.action);
      }
      if (num3 < 0)
      {
        if (num3 != -541478725)
          throw new ApplicationException("Could not read frame");
        if (this.listVideo.Count < 3)
        {
          this.bEnd = true;
          break;
        }
      }
      if ((IntPtr) this.pStreamAudio != IntPtr.Zero && this.pPacket->stream_index == this.iStreamAudioIndex && (ffmpeg.avcodec_decode_audio4(this.pAudioCodecContext, this.pDecodedAudioFrame, &num2, this.pPacket) >= 0 && num2 == 1))
      {
        int bufferSize1 = ffmpeg.av_samples_get_buffer_size((int*) null, this.pAudioCodecContext->channels, this.pDecodedAudioFrame->nb_samples, this.pAudioCodecContext->sample_fmt, 1);
        int bufferSize2 = ffmpeg.av_samples_get_buffer_size((int*) null, this.pAudioCodecContext->channels, this.pDecodedAudioFrame->nb_samples, AVSampleFormat.AV_SAMPLE_FMT_FLT, 1);
        if (this.pAudioCodecContext->sample_fmt != AVSampleFormat.AV_SAMPLE_FMT_FLT)
        {
          sbyte* numPtr = (sbyte*) (void*) Marshal.AllocCoTaskMem(bufferSize2);
          SwrContext* s = ffmpeg.swr_alloc_set_opts((SwrContext*) null, (long) this.pAudioCodecContext->channel_layout, AVSampleFormat.AV_SAMPLE_FMT_FLT, this.pAudioCodecContext->sample_rate, (long) this.pAudioCodecContext->channel_layout, this.pAudioCodecContext->sample_fmt, this.pAudioCodecContext->sample_rate, 0, (void*) null);
          int num4;
          if ((num4 = ffmpeg.swr_init(s)) < 0)
            UnityEngine.Debug.Log((object) ("error " + (object) num4));
          ffmpeg.swr_convert(s, &numPtr, bufferSize2, this.pDecodedAudioFrame->extended_data, this.pDecodedAudioFrame->nb_samples);
          IntPtr source = new IntPtr((void*) numPtr);
          byte[] destination = new byte[bufferSize2];
          Marshal.Copy(source, destination, 0, bufferSize2);
          this.pts = this.pPacket->dts == long.MinValue ? 0.0 : (double) ffmpeg.av_frame_get_best_effort_timestamp(this.pDecodedAudioFrame);
          this.pts *= MediaPlayerCtrl.av_q2d(this.pStreamAudio->time_base);
          if (this.bSeekTo)
          {
            this.iSoundCount = (int) (this.pts * (double) this.pDecodedAudioFrame->sample_rate / ((double) bufferSize2 / 4.0 / (double) this.pDecodedAudioFrame->channels));
            this.bSeekTo = false;
          }
          for (; this.pts > 600.0 * (double) (this.iInitCount + 1); ++this.iInitCount)
            this.iSoundCount -= (int) (600.0 * (double) this.pDecodedAudioFrame->sample_rate / ((double) bufferSize2 / 4.0 / (double) this.pDecodedAudioFrame->channels));
          this.fAudioData = new float[destination.Length / 4];
          Buffer.BlockCopy((Array) destination, 0, (Array) this.fAudioData, 0, destination.Length);
          lock ((object) this.listAudio)
          {
            this.listAudio.Add(this.fAudioData);
            lock ((object) this.listAudioPts)
            {
              lock ((object) this.listAudioPtsTime)
              {
                this.listAudioPts.Add((double) (this.iSoundCount++ * bufferSize2 / 4 / this.pDecodedAudioFrame->channels));
                this.listAudioPtsTime.Add(this.pts);
              }
            }
          }
          ffmpeg.swr_free(&s);
          Marshal.FreeCoTaskMem((IntPtr) (void*) numPtr);
        }
        else
        {
          IntPtr source = new IntPtr((void*) *this.pDecodedAudioFrame->extended_data);
          byte[] destination = new byte[bufferSize1];
          Marshal.Copy(source, destination, 0, bufferSize1);
          this.pts = this.pPacket->dts == long.MinValue ? 0.0 : (double) ffmpeg.av_frame_get_best_effort_timestamp(this.pDecodedAudioFrame);
          this.pts *= MediaPlayerCtrl.av_q2d(this.pStreamAudio->time_base);
          this.fAudioData = new float[destination.Length / 4];
          Buffer.BlockCopy((Array) destination, 0, (Array) this.fAudioData, 0, destination.Length);
          lock ((object) this.listAudio)
          {
            this.listAudio.Add(this.fAudioData);
            lock ((object) this.listAudioPts)
            {
              lock ((object) this.listAudioPts)
              {
                this.listAudioPts.Add(this.pts);
                this.listAudioPtsTime.Add(this.pts);
              }
            }
          }
        }
      }
    }
    while (this.pPacket->stream_index != this.iStreamIndex);
  }

  private void OnAudioRead(float[] data)
  {
    if (this.listAudio.Count < 3)
      return;
    if (this.iSoundBufferCount == 0)
      this.iSoundBufferCount = this.listAudio[0].Length;
    if (this.iSoundBufferCount < data.Length)
    {
      Array.Copy((Array) this.listAudio[0], this.listAudio[0].Length - this.iSoundBufferCount, (Array) data, 0, this.iSoundBufferCount);
      if (data.Length - this.iSoundBufferCount > this.listAudio[1].Length)
      {
        Array.Copy((Array) this.listAudio[1], 0, (Array) data, 0, this.listAudio[1].Length);
        Array.Copy((Array) this.listAudio[2], 0, (Array) data, this.iSoundBufferCount + this.listAudio[1].Length, data.Length - this.iSoundBufferCount - this.listAudio[1].Length);
        this.iSoundBufferCount = this.listAudio[2].Length - (data.Length - this.iSoundBufferCount - this.listAudio[1].Length);
        this.listAudio.RemoveAt(0);
        this.listAudioPts.RemoveAt(0);
        this.listAudioPtsTime.RemoveAt(0);
      }
      else
      {
        Array.Copy((Array) this.listAudio[1], 0, (Array) data, this.iSoundBufferCount, data.Length - this.iSoundBufferCount);
        this.iSoundBufferCount = this.listAudio[1].Length - (data.Length - this.iSoundBufferCount);
      }
      this.listAudio.RemoveAt(0);
      this.listAudioPts.RemoveAt(0);
      this.listAudioPtsTime.RemoveAt(0);
    }
    else
    {
      Array.Copy((Array) this.listAudio[0], this.listAudio[0].Length - this.iSoundBufferCount, (Array) data, 0, data.Length);
      this.iSoundBufferCount -= data.Length;
    }
    if (this.iSoundBufferCount != 0)
      return;
    this.listAudio.RemoveAt(0);
    this.listAudioPts.RemoveAt(0);
    this.listAudioPtsTime.RemoveAt(0);
    this.iSoundBufferCount = this.listAudio[0].Length;
  }

  private unsafe void Call_UpdateVideoTexture()
  {
    if (this.bEnd && this.listVideo.Count == 0)
    {
      this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.END;
      if (this.OnEnd != null)
        this.OnEnd();
      if (this.m_bLoop)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.audioClip);
        this.audioClip = (AudioClip) null;
        this.Load(this.m_strFileName);
        this.bEnd = false;
      }
      else
        this.bEnd = false;
    }
    else
    {
      if (this.bInterrupt)
      {
        if ((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null)
          this.audioSource.Pause();
      }
      else if ((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null && this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING && (this.m_bIsFirstFrameReady && !this.audioSource.isPlaying))
        this.audioSource.Play();
      if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING && this.m_bIsFirstFrameReady && (!this.bInterrupt && this.listVideo.Count > 0))
        this.fCurrentSeekTime += Time.deltaTime * this.m_fSpeed;
      if (this.threadVideo == null && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.END && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
      {
        this.threadVideo = new Thread(new ThreadStart(this.ThreadUpdate));
        this.threadVideo.IsBackground = true;
        this.threadVideo.Start();
      }
      if ((double) this.fLastFrameTime > (double) this.fCurrentSeekTime - 0.100000001490116)
      {
        for (int index = 0; index < this.listAudio.Count; ++index)
        {
          if (this.listAudioPtsTime.Count > index && (UnityEngine.Object) this.audioSource == (UnityEngine.Object) null && (int) ((double) this.pAudioCodecContext->sample_rate * (this.listAudioPtsTime[index] + (double) this.Call_GetDuration() / 1000.0)) > 0)
          {
            this.audioSource = this.gameObject.AddComponent<AudioSource>();
            this.audioSource.volume = this.m_fVolume;
          }
          if ((UnityEngine.Object) this.audioClip == (UnityEngine.Object) null && (UnityEngine.Object) this.audioSource != (UnityEngine.Object) null)
          {
            this.audioClip = AudioClip.Create("videoAudio", (int) ((double) this.pAudioCodecContext->sample_rate * 600.0), this.pAudioCodecContext->channels, this.pAudioCodecContext->sample_rate, false);
            this.audioSource.clip = this.audioClip;
          }
          if ((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null && this.Call_GetDuration() > 0 && (this.listAudioPts.Count > index && this.listAudioPts[index] >= 0.0))
            this.audioClip.SetData(this.listAudio[index], (int) (this.listAudioPts[index] % ((double) this.pAudioCodecContext->sample_rate * 600.0)));
        }
        if (!((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null) || !this.audioSource.isPlaying || this.Call_GetDuration() <= 0)
          return;
        this.listAudio.Clear();
        this.listAudioPts.Clear();
        this.listAudioPtsTime.Clear();
      }
      else
      {
        if (this.listVideo.Count > 0)
        {
          this.m_VideoTexture.LoadRawTextureData(this.listVideo.Dequeue());
          this.m_VideoTexture.Apply();
        }
        if (this.listVideoPts.Count > 0)
        {
          float num = this.listVideoPts.Dequeue();
          if ((double) this.fLastFrameTime == 0.0)
          {
            if ((double) num > (double) this.fCurrentSeekTime)
              this.fLastFrameTime = this.fCurrentSeekTime;
          }
          else
            this.fLastFrameTime = num;
        }
        if (!((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null))
          ;
        if (this.m_TargetMaterial != null)
        {
          for (int index = 0; index < this.m_TargetMaterial.Length; ++index)
          {
            if (!((UnityEngine.Object) this.m_TargetMaterial[index] == (UnityEngine.Object) null))
            {
              if ((UnityEngine.Object) this.m_TargetMaterial[index].GetComponent<MeshRenderer>() != (UnityEngine.Object) null && (UnityEngine.Object) this.m_TargetMaterial[index].GetComponent<MeshRenderer>().material.mainTexture != (UnityEngine.Object) this.m_VideoTexture)
                this.m_TargetMaterial[index].GetComponent<MeshRenderer>().material.mainTexture = (Texture) this.m_VideoTexture;
              if ((UnityEngine.Object) this.m_TargetMaterial[index].GetComponent<RawImage>() != (UnityEngine.Object) null && (UnityEngine.Object) this.m_TargetMaterial[index].GetComponent<RawImage>().texture != (UnityEngine.Object) this.m_VideoTexture)
                this.m_TargetMaterial[index].GetComponent<RawImage>().texture = (Texture) this.m_VideoTexture;
            }
          }
        }
        if (!this.bVideoFirstFrameReady)
          return;
        if (this.OnVideoFirstFrameReady != null)
        {
          this.OnVideoFirstFrameReady();
          this.bVideoFirstFrameReady = false;
        }
        for (int index = 0; index < this.listAudio.Count; ++index)
        {
          if ((UnityEngine.Object) this.audioSource == (UnityEngine.Object) null)
            this.audioSource = this.gameObject.AddComponent<AudioSource>();
          if ((UnityEngine.Object) this.audioClip == (UnityEngine.Object) null && (UnityEngine.Object) this.audioSource != (UnityEngine.Object) null)
          {
            this.audioClip = AudioClip.Create("videoAudio", (int) ((double) this.pAudioCodecContext->sample_rate * 600.0), this.pAudioCodecContext->channels, this.pAudioCodecContext->sample_rate, false);
            this.audioSource.clip = this.audioClip;
          }
          if ((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null && this.Call_GetDuration() > 0 && (this.listAudioPts.Count > index && this.listAudioPts[index] >= 0.0))
            this.audioClip.SetData(this.listAudio[index], (int) (this.listAudioPts[index] % ((double) this.pAudioCodecContext->sample_rate * 600.0)));
        }
      }
    }
  }

  private void Call_SetVolume(float fVolume)
  {
    if (!((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null))
      return;
    this.audioSource.volume = fVolume;
  }

  private unsafe void Call_SetSeekPosition(int iSeek)
  {
    if (this.threadVideo != null)
    {
      while (this.threadVideo.IsAlive)
        this.threadVideo.Abort();
      this.threadVideo = (Thread) null;
    }
    this.bSeekTo = true;
    this.iInitCount = 0;
    long a = (long) iSeek * 1000L;
    UnityEngine.Debug.Log((object) a);
    long timestamp = ffmpeg.av_rescale_q(a, ffmpeg.av_get_time_base_q(), this.pStream->time_base);
    UnityEngine.Debug.Log((object) timestamp);
    if (ffmpeg.av_seek_frame(this.pFormatContext, this.iStreamIndex, timestamp, 1) >= 0)
      ;
    this.fCurrentSeekTime = (float) iSeek / 1000f;
    this.fLastFrameTime = 0.0f;
    this.listVideo.Clear();
    this.listVideoPts.Clear();
    ffmpeg.avcodec_flush_buffers(this.pCodecContext);
  }

  private int Call_GetSeekPosition() => (int) ((double) this.fCurrentSeekTime * 1000.0);

  private void Call_Play(int iSeek)
  {
    if (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.READY && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED && (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.END && this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED))
      return;
    this.SeekTo(iSeek);
    if ((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null)
    {
      if (!this.audioSource.isPlaying)
        this.audioSource.Play();
      this.audioSource.time = (float) iSeek / 1000f;
    }
    this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING;
  }

  private void Call_Reset()
  {
  }

  private void Call_Stop()
  {
    this.SeekTo(0);
    if ((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null)
    {
      this.audioSource.Stop();
      this.audioSource.time = 0.0f;
    }
    this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED;
  }

  private void Call_RePlay()
  {
    if ((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null && !this.audioSource.isPlaying)
      this.audioSource.Play();
    this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING;
  }

  private void Call_Pause()
  {
    if ((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null)
      this.audioSource.Pause();
    this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED;
  }

  private int Call_GetVideoWidth() => this.m_iWidth;

  private int Call_GetVideoHeight() => this.m_iHeight;

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
    this.m_iID = MediaPlayerCtrl.SetTexture();
  }

  private int Call_GetError() => 0;

  private int Call_GetErrorExtra() => 0;

  private unsafe int Call_GetDuration() => (IntPtr) this.pFormatContext != IntPtr.Zero ? (int) (this.pFormatContext->duration / 1000L) : 0;

  private int Call_GetCurrentSeekPercent() => -1;

  private void Call_SetSplitOBB(bool bValue, string strOBBName)
  {
  }

  private void Call_SetSpeed(float fSpeed)
  {
    if (!((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null))
      return;
    this.audioSource.pitch = fSpeed;
  }

  private MediaPlayerCtrl.MEDIAPLAYER_STATE Call_GetStatus() => this.m_CurrentState;

  [DebuggerHidden]
  public IEnumerator DownloadStreamingVideoAndLoad(string strURL) => (IEnumerator) new MediaPlayerCtrl.\u003CDownloadStreamingVideoAndLoad\u003Ec__Iterator0()
  {
    strURL = strURL,
    \u0024this = this
  };

  [DebuggerHidden]
  public IEnumerator DownloadStreamingVideoAndLoad2(string strURL) => (IEnumerator) new MediaPlayerCtrl.\u003CDownloadStreamingVideoAndLoad2\u003Ec__Iterator1()
  {
    strURL = strURL,
    \u0024this = this
  };

  [DebuggerHidden]
  private IEnumerator CopyStreamingAssetVideoAndLoad(string strURL) => (IEnumerator) new MediaPlayerCtrl.\u003CCopyStreamingAssetVideoAndLoad\u003Ec__Iterator2()
  {
    strURL = strURL,
    \u0024this = this
  };

  private void CheckThreading()
  {
    lock (this.thisLock)
    {
      if (this.unityMainThreadActionList.Count <= 0)
        return;
      foreach (Action mainThreadAction in this.unityMainThreadActionList)
        mainThreadAction();
      this.unityMainThreadActionList.Clear();
    }
  }

  private void AddActionForUnityMainThread(Action a)
  {
    lock (this.thisLock)
      this.unityMainThreadActionList.Add(a);
    this.checkNewActions = true;
  }

  public delegate void VideoEnd();

  public delegate void VideoReady();

  public delegate void VideoError(
    MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode,
    MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra);

  public delegate void VideoFirstFrameReady();

  public delegate void VideoResize();

  public enum MEDIAPLAYER_ERROR
  {
    MEDIA_ERROR_UNSUPPORTED = -1010, // 0xFFFFFC0E
    MEDIA_ERROR_MALFORMED = -1007, // 0xFFFFFC11
    MEDIA_ERROR_IO = -1004, // 0xFFFFFC14
    MEDIA_ERROR_TIMED_OUT = -110, // 0xFFFFFF92
    MEDIA_ERROR_UNKNOWN = 1,
    MEDIA_ERROR_SERVER_DIED = 100, // 0x00000064
    MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK = 200, // 0x000000C8
  }

  public enum MEDIAPLAYER_STATE
  {
    NOT_READY,
    READY,
    END,
    PLAYING,
    PAUSED,
    STOPPED,
    ERROR,
  }

  public enum MEDIA_SCALE
  {
    SCALE_X_TO_Y,
    SCALE_X_TO_Z,
    SCALE_Y_TO_X,
    SCALE_Y_TO_Z,
    SCALE_Z_TO_X,
    SCALE_Z_TO_Y,
    SCALE_X_TO_Y_2,
  }

  public delegate void VideoInterrupt();
}
