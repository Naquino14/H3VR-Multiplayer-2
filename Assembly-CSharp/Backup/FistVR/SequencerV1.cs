// Decompiled with JetBrains decompiler
// Type: FistVR.SequencerV1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class SequencerV1 : MonoBehaviour
  {
    public FVRLever DistanceLever;
    public KeyCodeDisplay Display;
    public Text ScoreText;
    public Transform InitialPoint;
    private float m_floatScore;
    private int m_score;
    private bool m_isStarted;
    private float m_distance;
    private int m_numTargets;
    private int m_numWaves;
    private float m_wavelength;
    private float m_wavecooldown = 1f;
    private bool m_isCooldownTicking;
    private bool m_isShootTicking;
    private int m_wavesLeft;
    private float m_timeLeft;
    private float m_cooldownLeft;
    public GameObject SimpleTarget;
    private List<GameObject> Targets = new List<GameObject>();
    public Vector3 MinPos;
    public Vector3 MaxPos;
    public Renderer GreenLight;
    private AudioSource aud;
    private int m_possibleScore;

    public float FloatScore
    {
      get => this.m_floatScore;
      set
      {
        this.m_floatScore = value;
        this.Score = Mathf.RoundToInt(this.m_floatScore);
      }
    }

    public int Score
    {
      get => this.m_score;
      set
      {
        this.m_score = value;
        this.ScoreText.text = this.m_score.ToString() + "/" + this.m_possibleScore.ToString();
      }
    }

    public void Awake() => this.aud = this.GetComponent<AudioSource>();

    public void AddTargetPoints(int i) => this.FloatScore += (float) i;

    public void BeginSequence(int inty)
    {
      this.Reset(0);
      if (this.m_isStarted)
        return;
      string text = this.Display.MyText;
      if (text.Length < 3)
        return;
      this.m_numTargets = int.Parse(text[0].ToString());
      this.m_numWaves = int.Parse(text[1].ToString());
      string s = text[2].ToString();
      if (text.Length == 4)
        s += text[3].ToString();
      this.m_wavelength = (float) int.Parse(s);
      if ((double) this.m_wavelength < 1.0 || this.m_numTargets == 0 || this.m_numWaves == 0)
        return;
      this.m_distance = this.DistanceLever.GetLeverValue();
      this.m_isCooldownTicking = false;
      this.m_isShootTicking = false;
      this.m_wavesLeft = this.m_numWaves;
      this.m_timeLeft = this.m_wavelength;
      this.m_cooldownLeft = this.m_wavecooldown;
      this.FloatScore = 0.0f;
      this.m_isStarted = true;
      this.m_isCooldownTicking = true;
      this.m_possibleScore = 10 * this.m_numWaves * this.m_numTargets;
      this.GreenLight.material.SetFloat("_EmissionWeight", 1f);
    }

    public void Reset(int inty)
    {
      this.FloatScore = 0.0f;
      this.m_isStarted = false;
      for (int index = this.Targets.Count - 1; index >= 0; --index)
      {
        if ((Object) this.Targets[index] != (Object) null)
          Object.Destroy((Object) this.Targets[index]);
      }
      this.Targets.Clear();
      this.m_isCooldownTicking = false;
      this.m_isShootTicking = false;
      this.GreenLight.material.SetFloat("_EmissionWeight", 0.0f);
    }

    private void Update()
    {
      if (this.m_isStarted && this.m_isCooldownTicking)
      {
        if ((double) this.m_cooldownLeft > 0.0)
        {
          this.m_cooldownLeft -= Time.deltaTime;
        }
        else
        {
          this.m_isCooldownTicking = false;
          this.m_isShootTicking = true;
          this.m_timeLeft = this.m_wavelength + 2f;
          this.aud.PlayOneShot(this.aud.clip, 0.5f);
          --this.m_wavesLeft;
          for (int index = 0; index < this.m_numTargets; ++index)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(this.SimpleTarget, this.InitialPoint.position, this.InitialPoint.rotation);
            MaskedTarget component = gameObject.GetComponent<MaskedTarget>();
            this.Targets.Add(gameObject);
            Vector3 endPos = new Vector3(Random.Range(this.MinPos.x, this.MaxPos.x), Random.Range(this.MinPos.y, this.MaxPos.y), Mathf.Lerp(this.MinPos.z, this.MaxPos.z, this.m_distance));
            component.Init(this.InitialPoint.position, endPos, 1f, Vector3.forward, (this.transform.position - endPos).normalized, this);
          }
        }
      }
      if (!this.m_isShootTicking)
        return;
      if ((double) this.m_timeLeft > 0.0)
      {
        this.m_timeLeft -= Time.deltaTime;
      }
      else
      {
        for (int index = this.Targets.Count - 1; index >= 0; --index)
        {
          if ((Object) this.Targets[index] != (Object) null)
            Object.Destroy((Object) this.Targets[index]);
        }
        this.Targets.Clear();
        if (this.m_wavesLeft > 0)
        {
          this.m_isShootTicking = false;
          this.m_isCooldownTicking = true;
          this.m_cooldownLeft = this.m_wavecooldown;
        }
        else
          this.GreenLight.material.SetFloat("_EmissionWeight", 0.0f);
      }
    }
  }
}
