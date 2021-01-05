// Decompiled with JetBrains decompiler
// Type: FmodGvrAudio
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class FmodGvrAudio
{
  public const float maxGainDb = 24f;
  public const float minGainDb = -24f;
  public const float maxReverbBrightness = 1f;
  public const float minReverbBrightness = -1f;
  public const float maxReverbTime = 3f;
  public const float maxReflectivity = 2f;
  private static readonly Matrix4x4 flipZ = Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
  private static readonly string listenerPluginName = "Google GVR Listener";
  private static readonly int roomPropertiesSize = Marshal.SizeOf(typeof (FmodGvrAudio.RoomProperties));
  private static readonly int roomPropertiesIndex = 1;
  private static Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
  private static List<FmodGvrAudioRoom> enabledRooms = new List<FmodGvrAudioRoom>();
  private static VECTOR listenerPositionFmod = new VECTOR();
  private static DSP listenerPlugin;

  public static void UpdateAudioRoom(FmodGvrAudioRoom room, bool roomEnabled)
  {
    if (roomEnabled)
    {
      if (!FmodGvrAudio.enabledRooms.Contains(room))
        FmodGvrAudio.enabledRooms.Add(room);
    }
    else
      FmodGvrAudio.enabledRooms.Remove(room);
    if (FmodGvrAudio.enabledRooms.Count > 0)
    {
      FmodGvrAudio.RoomProperties roomProperties = FmodGvrAudio.GetRoomProperties(FmodGvrAudio.enabledRooms[FmodGvrAudio.enabledRooms.Count - 1]);
      IntPtr num1 = Marshal.AllocHGlobal(FmodGvrAudio.roomPropertiesSize);
      Marshal.StructureToPtr((object) roomProperties, num1, false);
      int num2 = (int) FmodGvrAudio.ListenerPlugin.setParameterData(FmodGvrAudio.roomPropertiesIndex, FmodGvrAudio.GetBytes(num1, FmodGvrAudio.roomPropertiesSize));
      Marshal.FreeHGlobal(num1);
    }
    else
    {
      int num = (int) FmodGvrAudio.ListenerPlugin.setParameterData(FmodGvrAudio.roomPropertiesIndex, FmodGvrAudio.GetBytes(IntPtr.Zero, 0));
    }
  }

  public static bool IsListenerInsideRoom(FmodGvrAudioRoom room)
  {
    VECTOR vector;
    int num = (int) RuntimeManager.LowlevelSystem.get3DListenerAttributes(0, out FmodGvrAudio.listenerPositionFmod, out vector, out vector, out vector);
    Vector3 vector3 = new Vector3(FmodGvrAudio.listenerPositionFmod.x, FmodGvrAudio.listenerPositionFmod.y, FmodGvrAudio.listenerPositionFmod.z) - room.transform.position;
    Quaternion quaternion = Quaternion.Inverse(room.transform.rotation);
    FmodGvrAudio.bounds.size = Vector3.Scale(room.transform.lossyScale, room.size);
    return FmodGvrAudio.bounds.Contains(quaternion * vector3);
  }

  private static DSP ListenerPlugin
  {
    get
    {
      if (!FmodGvrAudio.listenerPlugin.hasHandle())
        FmodGvrAudio.listenerPlugin = FmodGvrAudio.Initialize();
      return FmodGvrAudio.listenerPlugin;
    }
  }

  private static float ConvertAmplitudeFromDb(float db) => Mathf.Pow(10f, 0.05f * db);

  private static void ConvertAudioTransformFromUnity(ref Vector3 position, ref Quaternion rotation)
  {
    Matrix4x4 matrix4x4_1 = Matrix4x4.TRS(position, rotation, Vector3.one);
    Matrix4x4 matrix4x4_2 = FmodGvrAudio.flipZ * matrix4x4_1 * FmodGvrAudio.flipZ;
    position = (Vector3) matrix4x4_2.GetColumn(3);
    rotation = Quaternion.LookRotation((Vector3) matrix4x4_2.GetColumn(2), (Vector3) matrix4x4_2.GetColumn(1));
  }

  private static byte[] GetBytes(IntPtr ptr, int length)
  {
    if (!(ptr != IntPtr.Zero))
      return new byte[1];
    byte[] destination = new byte[length];
    Marshal.Copy(ptr, destination, 0, length);
    return destination;
  }

  private static FmodGvrAudio.RoomProperties GetRoomProperties(FmodGvrAudioRoom room)
  {
    Vector3 position = room.transform.position;
    Quaternion rotation = room.transform.rotation;
    Vector3 vector3 = Vector3.Scale(room.transform.lossyScale, room.size);
    FmodGvrAudio.ConvertAudioTransformFromUnity(ref position, ref rotation);
    FmodGvrAudio.RoomProperties roomProperties;
    roomProperties.positionX = position.x;
    roomProperties.positionY = position.y;
    roomProperties.positionZ = position.z;
    roomProperties.rotationX = rotation.x;
    roomProperties.rotationY = rotation.y;
    roomProperties.rotationZ = rotation.z;
    roomProperties.rotationW = rotation.w;
    roomProperties.dimensionsX = vector3.x;
    roomProperties.dimensionsY = vector3.y;
    roomProperties.dimensionsZ = vector3.z;
    roomProperties.materialLeft = room.leftWall;
    roomProperties.materialRight = room.rightWall;
    roomProperties.materialBottom = room.floor;
    roomProperties.materialTop = room.ceiling;
    roomProperties.materialFront = room.frontWall;
    roomProperties.materialBack = room.backWall;
    roomProperties.reverbGain = FmodGvrAudio.ConvertAmplitudeFromDb(room.reverbGainDb);
    roomProperties.reverbTime = room.reverbTime;
    roomProperties.reverbBrightness = room.reverbBrightness;
    roomProperties.reflectionScalar = room.reflectivity;
    return roomProperties;
  }

  private static DSP Initialize()
  {
    int count1 = 0;
    DSP dsp1 = new DSP();
    Bank[] array1 = (Bank[]) null;
    int bankCount = (int) RuntimeManager.StudioSystem.getBankCount(out count1);
    int bankList = (int) RuntimeManager.StudioSystem.getBankList(out array1);
    for (int index1 = 0; index1 < count1; ++index1)
    {
      int count2 = 0;
      Bus[] array2 = (Bus[]) null;
      int busCount = (int) array1[index1].getBusCount(out count2);
      int busList = (int) array1[index1].getBusList(out array2);
      int num1 = (int) RuntimeManager.StudioSystem.flushCommands();
      for (int index2 = 0; index2 < count2; ++index2)
      {
        string path1 = (string) null;
        int path2 = (int) array2[index2].getPath(out path1);
        int bus = (int) RuntimeManager.StudioSystem.getBus(path1, out array2[index2]);
        int num2 = (int) RuntimeManager.StudioSystem.flushCommands();
        ChannelGroup group;
        int channelGroup = (int) array2[index2].getChannelGroup(out group);
        int num3 = (int) RuntimeManager.StudioSystem.flushCommands();
        if (group.hasHandle())
        {
          int numdsps = 0;
          int numDsPs = (int) group.getNumDSPs(out numdsps);
          for (int index3 = 0; index3 < numdsps; ++index3)
          {
            int dsp2 = (int) group.getDSP(index3, out dsp1);
            int num4 = 0;
            uint version = 0;
            string name;
            int info = (int) dsp1.getInfo(out name, out version, out num4, out num4, out num4);
            if (name.ToString().Equals(FmodGvrAudio.listenerPluginName) && dsp1.hasHandle())
              return dsp1;
          }
        }
      }
    }
    UnityEngine.Debug.LogError((object) (FmodGvrAudio.listenerPluginName + " not found in the FMOD project."));
    return dsp1;
  }

  private struct RoomProperties
  {
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float rotationW;
    public float dimensionsX;
    public float dimensionsY;
    public float dimensionsZ;
    public FmodGvrAudioRoom.SurfaceMaterial materialLeft;
    public FmodGvrAudioRoom.SurfaceMaterial materialRight;
    public FmodGvrAudioRoom.SurfaceMaterial materialBottom;
    public FmodGvrAudioRoom.SurfaceMaterial materialTop;
    public FmodGvrAudioRoom.SurfaceMaterial materialFront;
    public FmodGvrAudioRoom.SurfaceMaterial materialBack;
    public float reflectionScalar;
    public float reverbGain;
    public float reverbTime;
    public float reverbBrightness;
  }
}
