// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFirearmAudioSet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Audio Set", menuName = "AudioPooling/FireArmAudioSet", order = 0)]
  public class FVRFirearmAudioSet : ScriptableObject
  {
    public float Loudness_Primary = 100f;
    public float Loudness_Suppressed = 65f;
    public float Loudness_OperationMult = 1f;
    public FVRFirearmAudioSet.ForceTypeProperties FTP;
    public AudioEvent Shots_Main;
    public AudioEvent Shots_Suppressed;
    public AudioEvent Shots_LowPressure;
    public Vector2 TailPitchMod_Main = new Vector2(1f, 1.03f);
    public Vector2 TailPitchMod_Suppressed = new Vector2(1f, 1.03f);
    public Vector2 TailPitchMod_LowPressure = new Vector2(1f, 1.03f);
    public int TailConcurrentLimit = 2;
    public bool UsesTail_Main = true;
    public bool UsesTail_Suppressed = true;
    public bool UsesLowPressureSet;
    public AudioEvent BoltRelease;
    public AudioEvent BoltSlideBack;
    public AudioEvent BoltSlideBackHeld;
    public AudioEvent BoltSlideBackLocked;
    public AudioEvent BoltSlideForward;
    public AudioEvent BoltSlideForwardHeld;
    public AudioEvent BreachOpen;
    public AudioEvent BreachClose;
    public AudioEvent BeltBulletSet;
    public AudioEvent CatchOnSear;
    public AudioEvent ChamberManual;
    public AudioEvent FireSelector;
    public AudioEvent HammerHit;
    public AudioEvent HandleBack;
    public AudioEvent HandleBackEmpty;
    public AudioEvent HandleForward;
    public AudioEvent HandleForwardEmpty;
    public AudioEvent HandleUp;
    public AudioEvent HandleDown;
    public AudioEvent HandleGrab;
    public AudioEvent MagazineIn;
    public AudioEvent MagazineOut;
    public AudioEvent MagazineInsertRound;
    public AudioEvent MagazineEjectRound;
    public AudioEvent Prefire;
    public AudioEvent Safety;
    public AudioEvent TriggerReset;
    public AudioEvent TopCoverRelease;
    public AudioEvent TopCoverUp;
    public AudioEvent TopCoverDown;
    public AudioEvent StockOpen;
    public AudioEvent StockClosed;
    public AudioEvent BipodOpen;
    public AudioEvent BipodClosed;
    public AudioEvent BeltGrab;
    public AudioEvent BeltRelease;
    public AudioEvent BeltSeat;
    public AudioEvent BeltSettle;
    public int BeltSettlingLimit;

    [Serializable]
    public class ForceTypeProperties
    {
      public byte Kick_Shot;
      public byte Rumble_Shot_Intensity = 100;
      public float Rumble_Shot_Duration = 0.03f;
      public byte Rumble_Handling_Intensity = 50;
      public float Rumble_Handling_Duration = 0.022f;
    }
  }
}
