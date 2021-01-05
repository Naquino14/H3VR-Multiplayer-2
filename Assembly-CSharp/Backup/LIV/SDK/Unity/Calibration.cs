// Decompiled with JetBrains decompiler
// Type: LIV.SDK.Unity.Calibration
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace LIV.SDK.Unity
{
  public static class Calibration
  {
    private const string ConfigFileName = "externalcamera.cfg";
    public static float X;
    public static float Y;
    public static float Z;
    public static float Yaw;
    public static float Pitch;
    public static float Roll;
    public static float FieldOfVision;
    public static float NearClip;
    public static float FarClip;
    public static float HMDOffset;
    public static float NearOffset;
    private static readonly FileSystemWatcher ConfigWatcher;

    static Calibration()
    {
      try
      {
        FileInfo fileInfo = new FileInfo("externalcamera.cfg");
        Calibration.ConfigWatcher = new FileSystemWatcher(fileInfo.DirectoryName ?? string.Empty, fileInfo.Name)
        {
          NotifyFilter = NotifyFilters.LastWrite
        };
        Calibration.ConfigWatcher.Changed += (FileSystemEventHandler) ((o, e) => Calibration.Read());
        Calibration.ConfigWatcher.EnableRaisingEvents = true;
      }
      catch
      {
      }
      Calibration.Read();
    }

    public static event EventHandler Changed;

    public static Vector3 PositionOffset => new Vector3(Calibration.X, Calibration.Y, Calibration.Z);

    public static Quaternion RotationOffset => Quaternion.Euler(Calibration.Pitch, Calibration.Yaw, Calibration.Roll);

    private static void Reset()
    {
      double num1;
      Calibration.Z = (float) (num1 = 0.0);
      Calibration.Y = (float) num1;
      Calibration.X = (float) num1;
      double num2;
      Calibration.Roll = (float) (num2 = 0.0);
      Calibration.Yaw = (float) num2;
      Calibration.Pitch = (float) num2;
      Calibration.FieldOfVision = 60f;
      Calibration.NearClip = 0.01f;
      Calibration.FarClip = 1000f;
      Calibration.HMDOffset = 0.0f;
      Calibration.NearOffset = 0.0f;
    }

    public static void Read()
    {
      Calibration.Reset();
      string[] strArray1 = new string[0];
      try
      {
        strArray1 = File.ReadAllLines("externalcamera.cfg");
      }
      catch (Exception ex)
      {
        Debug.LogWarningFormat("LIV: Failed to read camera calibration from disk. Error: {0}", (object) ex);
      }
      foreach (string str in strArray1)
      {
        char[] separator = new char[1]{ '=' };
        string[] strArray2 = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        if (strArray2.Length == 2)
        {
          string lowerInvariant = strArray2[0].ToLowerInvariant();
          if (lowerInvariant != null)
          {
            // ISSUE: reference to a compiler-generated field
            if (Calibration.\u003C\u003Ef__switch\u0024map1 == null)
            {
              // ISSUE: reference to a compiler-generated field
              Calibration.\u003C\u003Ef__switch\u0024map1 = new Dictionary<string, int>(11)
              {
                {
                  "x",
                  0
                },
                {
                  "y",
                  1
                },
                {
                  "z",
                  2
                },
                {
                  "rx",
                  3
                },
                {
                  "ry",
                  4
                },
                {
                  "rz",
                  5
                },
                {
                  "fov",
                  6
                },
                {
                  "near",
                  7
                },
                {
                  "far",
                  8
                },
                {
                  "hmdoffset",
                  9
                },
                {
                  "nearoffset",
                  10
                }
              };
            }
            int num;
            // ISSUE: reference to a compiler-generated field
            if (Calibration.\u003C\u003Ef__switch\u0024map1.TryGetValue(lowerInvariant, out num))
            {
              switch (num)
              {
                case 0:
                  Calibration.TryParseInvariantFloat(strArray2[1], out Calibration.X);
                  continue;
                case 1:
                  Calibration.TryParseInvariantFloat(strArray2[1], out Calibration.Y);
                  continue;
                case 2:
                  Calibration.TryParseInvariantFloat(strArray2[1], out Calibration.Z);
                  continue;
                case 3:
                  Calibration.TryParseInvariantFloat(strArray2[1], out Calibration.Pitch);
                  continue;
                case 4:
                  Calibration.TryParseInvariantFloat(strArray2[1], out Calibration.Yaw);
                  continue;
                case 5:
                  Calibration.TryParseInvariantFloat(strArray2[1], out Calibration.Roll);
                  continue;
                case 6:
                  Calibration.TryParseInvariantFloat(strArray2[1], out Calibration.FieldOfVision);
                  continue;
                case 7:
                  Calibration.TryParseInvariantFloat(strArray2[1], out Calibration.NearClip);
                  continue;
                case 8:
                  Calibration.TryParseInvariantFloat(strArray2[1], out Calibration.FarClip);
                  continue;
                case 9:
                  Calibration.TryParseInvariantFloat(strArray2[1], out Calibration.HMDOffset);
                  continue;
                case 10:
                  Calibration.TryParseInvariantFloat(strArray2[1], out Calibration.NearOffset);
                  continue;
                default:
                  continue;
              }
            }
          }
        }
      }
      if (Calibration.Changed == null)
        return;
      Calibration.Changed((object) null, EventArgs.Empty);
    }

    private static bool TryParseInvariantFloat(string number, out float result) => float.TryParse(number, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result);
  }
}
