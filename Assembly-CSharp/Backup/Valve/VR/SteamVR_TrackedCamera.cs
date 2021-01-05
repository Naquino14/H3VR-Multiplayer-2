// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_TrackedCamera
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_TrackedCamera
  {
    private static SteamVR_TrackedCamera.VideoStreamTexture[] distorted;
    private static SteamVR_TrackedCamera.VideoStreamTexture[] undistorted;
    private static SteamVR_TrackedCamera.VideoStream[] videostreams;

    public static SteamVR_TrackedCamera.VideoStreamTexture Distorted(
      int deviceIndex = 0)
    {
      if (SteamVR_TrackedCamera.distorted == null)
        SteamVR_TrackedCamera.distorted = new SteamVR_TrackedCamera.VideoStreamTexture[new IntPtr(64)];
      if (SteamVR_TrackedCamera.distorted[deviceIndex] == null)
        SteamVR_TrackedCamera.distorted[deviceIndex] = new SteamVR_TrackedCamera.VideoStreamTexture((uint) deviceIndex, false);
      return SteamVR_TrackedCamera.distorted[deviceIndex];
    }

    public static SteamVR_TrackedCamera.VideoStreamTexture Undistorted(
      int deviceIndex = 0)
    {
      if (SteamVR_TrackedCamera.undistorted == null)
        SteamVR_TrackedCamera.undistorted = new SteamVR_TrackedCamera.VideoStreamTexture[new IntPtr(64)];
      if (SteamVR_TrackedCamera.undistorted[deviceIndex] == null)
        SteamVR_TrackedCamera.undistorted[deviceIndex] = new SteamVR_TrackedCamera.VideoStreamTexture((uint) deviceIndex, true);
      return SteamVR_TrackedCamera.undistorted[deviceIndex];
    }

    public static SteamVR_TrackedCamera.VideoStreamTexture Source(
      bool undistorted,
      int deviceIndex = 0)
    {
      return undistorted ? SteamVR_TrackedCamera.Undistorted(deviceIndex) : SteamVR_TrackedCamera.Distorted(deviceIndex);
    }

    private static SteamVR_TrackedCamera.VideoStream Stream(uint deviceIndex)
    {
      if (SteamVR_TrackedCamera.videostreams == null)
        SteamVR_TrackedCamera.videostreams = new SteamVR_TrackedCamera.VideoStream[new IntPtr(64)];
      if (SteamVR_TrackedCamera.videostreams[(IntPtr) deviceIndex] == null)
        SteamVR_TrackedCamera.videostreams[(IntPtr) deviceIndex] = new SteamVR_TrackedCamera.VideoStream(deviceIndex);
      return SteamVR_TrackedCamera.videostreams[(IntPtr) deviceIndex];
    }

    public class VideoStreamTexture
    {
      private Texture2D _texture;
      private int prevFrameCount = -1;
      private uint glTextureId;
      private SteamVR_TrackedCamera.VideoStream videostream;
      private CameraVideoStreamFrameHeader_t header;

      public VideoStreamTexture(uint deviceIndex, bool undistorted)
      {
        this.undistorted = undistorted;
        this.videostream = SteamVR_TrackedCamera.Stream(deviceIndex);
      }

      public bool undistorted { get; private set; }

      public uint deviceIndex => this.videostream.deviceIndex;

      public bool hasCamera => this.videostream.hasCamera;

      public bool hasTracking
      {
        get
        {
          this.Update();
          return this.header.standingTrackedDevicePose.bPoseIsValid;
        }
      }

      public uint frameId
      {
        get
        {
          this.Update();
          return this.header.nFrameSequence;
        }
      }

      public VRTextureBounds_t frameBounds { get; private set; }

      public EVRTrackedCameraFrameType frameType => this.undistorted ? EVRTrackedCameraFrameType.Undistorted : EVRTrackedCameraFrameType.Distorted;

      public Texture2D texture
      {
        get
        {
          this.Update();
          return this._texture;
        }
      }

      public SteamVR_Utils.RigidTransform transform
      {
        get
        {
          this.Update();
          return new SteamVR_Utils.RigidTransform(this.header.standingTrackedDevicePose.mDeviceToAbsoluteTracking);
        }
      }

      public Vector3 velocity
      {
        get
        {
          this.Update();
          TrackedDevicePose_t trackedDevicePose = this.header.standingTrackedDevicePose;
          return new Vector3(trackedDevicePose.vVelocity.v0, trackedDevicePose.vVelocity.v1, -trackedDevicePose.vVelocity.v2);
        }
      }

      public Vector3 angularVelocity
      {
        get
        {
          this.Update();
          TrackedDevicePose_t trackedDevicePose = this.header.standingTrackedDevicePose;
          return new Vector3(-trackedDevicePose.vAngularVelocity.v0, -trackedDevicePose.vAngularVelocity.v1, trackedDevicePose.vAngularVelocity.v2);
        }
      }

      public TrackedDevicePose_t GetPose()
      {
        this.Update();
        return this.header.standingTrackedDevicePose;
      }

      public ulong Acquire() => this.videostream.Acquire();

      public ulong Release()
      {
        ulong num = this.videostream.Release();
        if (this.videostream.handle == 0UL)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this._texture);
          this._texture = (Texture2D) null;
        }
        return num;
      }

      private void Update()
      {
        if (Time.frameCount == this.prevFrameCount)
          return;
        this.prevFrameCount = Time.frameCount;
        if (this.videostream.handle == 0UL)
          return;
        SteamVR instance = SteamVR.instance;
        if (instance == null)
          return;
        CVRTrackedCamera trackedCamera = OpenVR.TrackedCamera;
        if (trackedCamera == null)
          return;
        IntPtr ppD3D11ShaderResourceView = IntPtr.Zero;
        Texture2D texture2D = !((UnityEngine.Object) this._texture != (UnityEngine.Object) null) ? new Texture2D(2, 2) : this._texture;
        uint nFrameHeaderSize = (uint) Marshal.SizeOf(this.header.GetType());
        if (instance.textureType == ETextureType.OpenGL)
        {
          if (this.glTextureId != 0U)
          {
            int num = (int) trackedCamera.ReleaseVideoStreamTextureGL(this.videostream.handle, this.glTextureId);
          }
          if (trackedCamera.GetVideoStreamTextureGL(this.videostream.handle, this.frameType, ref this.glTextureId, ref this.header, nFrameHeaderSize) != EVRTrackedCameraError.None)
            return;
          ppD3D11ShaderResourceView = (IntPtr) (long) this.glTextureId;
        }
        else if (instance.textureType == ETextureType.DirectX && trackedCamera.GetVideoStreamTextureD3D11(this.videostream.handle, this.frameType, texture2D.GetNativeTexturePtr(), ref ppD3D11ShaderResourceView, ref this.header, nFrameHeaderSize) != EVRTrackedCameraError.None)
          return;
        if ((UnityEngine.Object) this._texture == (UnityEngine.Object) null)
        {
          this._texture = Texture2D.CreateExternalTexture((int) this.header.nWidth, (int) this.header.nHeight, TextureFormat.RGBA32, false, false, ppD3D11ShaderResourceView);
          uint pnWidth = 0;
          uint pnHeight = 0;
          VRTextureBounds_t pTextureBounds = new VRTextureBounds_t();
          if (trackedCamera.GetVideoStreamTextureSize(this.deviceIndex, this.frameType, ref pTextureBounds, ref pnWidth, ref pnHeight) != EVRTrackedCameraError.None)
            return;
          pTextureBounds.vMin = 1f - pTextureBounds.vMin;
          pTextureBounds.vMax = 1f - pTextureBounds.vMax;
          this.frameBounds = pTextureBounds;
        }
        else
          this._texture.UpdateExternalTexture(ppD3D11ShaderResourceView);
      }
    }

    private class VideoStream
    {
      private ulong _handle;
      private bool _hasCamera;
      private ulong refCount;

      public VideoStream(uint deviceIndex)
      {
        this.deviceIndex = deviceIndex;
        CVRTrackedCamera trackedCamera = OpenVR.TrackedCamera;
        if (trackedCamera == null)
          return;
        int num = (int) trackedCamera.HasCamera(deviceIndex, ref this._hasCamera);
      }

      public uint deviceIndex { get; private set; }

      public ulong handle => this._handle;

      public bool hasCamera => this._hasCamera;

      public ulong Acquire()
      {
        if (this._handle == 0UL && this.hasCamera)
        {
          CVRTrackedCamera trackedCamera = OpenVR.TrackedCamera;
          if (trackedCamera != null)
          {
            int num = (int) trackedCamera.AcquireVideoStreamingService(this.deviceIndex, ref this._handle);
          }
        }
        return ++this.refCount;
      }

      public ulong Release()
      {
        if (this.refCount > 0UL && (--this.refCount == 0UL && this._handle != 0UL))
        {
          CVRTrackedCamera trackedCamera = OpenVR.TrackedCamera;
          if (trackedCamera != null)
          {
            int num = (int) trackedCamera.ReleaseVideoStreamingService(this._handle);
          }
          this._handle = 0UL;
        }
        return this.refCount;
      }
    }
  }
}
