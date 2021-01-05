// Decompiled with JetBrains decompiler
// Type: FistVR.wwBotWurstConfig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New BotwurstConfig", menuName = "Wurstwurld/BotwurstConfig", order = 0)]
  public class wwBotWurstConfig : ScriptableObject
  {
    [Header("Life")]
    public int Life_Head;
    public int Life_Torso;
    public int Life_Bottom;
    [Header("Movement")]
    public float LinearSpeed_Walk;
    public float LinearSpeed_Combat;
    public float LinearSpeed_Run;
    public float Acceleration = 1f;
    public float MaxAngularSpeed;
    public Vector2 PreferedDistanceRange;
    public Vector2 LookAroundNewPointFrequency;
    public Vector2 PatrolNewPointFrequency;
    public Vector2 WaitAtSearchPointRange;
    [Header("Sensing")]
    public float MaxViewDistance = 40f;
    public float MaxViewAngle = 45f;
    [Header("Firing")]
    public bool CanFight = true;
    public bool IsMelee;
    public float AngularFiringRange;
    public float MaximumFiringRange;
    public float AccuracyMultipler = 1f;
    public float TimeBlindFiring = 5f;
    [Header("Sound")]
    public AudioEvent SpeakEvent_InCombat;
    public AudioEvent SpeakEvent_Patrol;
    public AudioEvent SpeakEvent_Reloading;
    public AudioEvent SpeakEvent_Searching;
    public AudioEvent SpeakEvent_RunningAway;
    public AudioEvent SpeakEvent_Greetings;
    public Vector2 CalloutFrequencyRange = new Vector2(6f, 20f);

    [ContextMenu("Migrate")]
    public void Migrate()
    {
      this.SpeakEvent_InCombat.VolumeRange = new Vector2(0.3f, 0.4f);
      this.SpeakEvent_Patrol.VolumeRange = new Vector2(0.3f, 0.4f);
      this.SpeakEvent_Reloading.VolumeRange = new Vector2(0.3f, 0.4f);
      this.SpeakEvent_Searching.VolumeRange = new Vector2(0.3f, 0.4f);
      this.SpeakEvent_RunningAway.VolumeRange = new Vector2(0.3f, 0.4f);
      this.SpeakEvent_Greetings.VolumeRange = new Vector2(0.3f, 0.4f);
    }
  }
}
