// Decompiled with JetBrains decompiler
// Type: GrenadeSkeeballGame
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeSkeeballGame : MonoBehaviour
{
  public GameObject TargetPrefabSet;
  public GameObject GrenadePrefab;
  public Transform GrenadeSpawnPoint;
  public Vector3 GrenadeForce;
  private GameObject TargetGroup;
  private List<GameObject> Grenades = new List<GameObject>();
  public AudioSource AudioBeep1;
  public AudioSource AudioBegin1;
  private int m_score;
  public Text ScoreReadout;
  public ParticleSystem PSystem_500;
  public ParticleSystem PSystem_1000;
  public ParticleSystem PSystem_5000;
  public AudioSource AudioPointsBlue;
  public AudioSource AudioPointsYellow;
  public AudioSource AudioPointsRed;

  public void AddPoints(int p)
  {
    this.m_score += p;
    this.UpdateScoreScreen();
  }

  private void UpdateScoreScreen() => this.ScoreReadout.text = this.m_score.ToString("000000");

  public void ClearGame()
  {
    this.CancelInvoke();
    this.m_score = 0;
    this.UpdateScoreScreen();
    Object.Destroy((Object) this.TargetGroup);
    if (this.Grenades.Count > 0)
    {
      for (int index = this.Grenades.Count - 1; index >= 0; --index)
      {
        if ((Object) this.Grenades[index] != (Object) null)
          Object.Destroy((Object) this.Grenades[index]);
      }
    }
    this.Grenades.Clear();
  }

  public void BeginGame(int j)
  {
    this.ClearGame();
    this.AudioBegin1.PlayOneShot(this.AudioBegin1.clip, 0.3f);
    this.TargetGroup = Object.Instantiate<GameObject>(this.TargetPrefabSet, Vector3.zero, Quaternion.identity);
    this.TargetGroup.GetComponent<GrenadeSkeeballTargetCollection>().SetGameReference(this);
    for (int index = 0; index < 6; ++index)
      this.Invoke("SpawnGrenade", (float) (index + 1));
  }

  private void SpawnGrenade()
  {
    this.AudioBeep1.PlayOneShot(this.AudioBeep1.clip, 0.6f);
    GameObject gameObject = Object.Instantiate<GameObject>(this.GrenadePrefab, this.GrenadeSpawnPoint.position, this.GrenadeSpawnPoint.rotation);
    this.Grenades.Add(gameObject);
    gameObject.GetComponent<Rigidbody>().AddForce(this.GrenadeForce);
  }

  public void SetPlayHeight(int i)
  {
    switch (i)
    {
      case 1:
        this.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        break;
      case 2:
        this.transform.position = new Vector3(0.0f, -0.04f, 0.0f);
        break;
      case 3:
        this.transform.position = new Vector3(0.0f, -0.08f, 0.0f);
        break;
      case 4:
        this.transform.position = new Vector3(0.0f, -0.12f, 0.0f);
        break;
      case 5:
        this.transform.position = new Vector3(0.0f, -0.16f, 0.0f);
        break;
      case 6:
        this.transform.position = new Vector3(0.0f, -0.2f, 0.0f);
        break;
      case 7:
        this.transform.position = new Vector3(0.0f, -0.24f, 0.0f);
        break;
    }
  }

  public void SpawnPointsFX(Vector3 pos, GrenadeSkeeballGame.PointsParticleType type)
  {
    ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
    switch (type)
    {
      case GrenadeSkeeballGame.PointsParticleType.Blue:
        emitParams.position = pos;
        emitParams.velocity = Vector3.up * 5f;
        this.AudioPointsBlue.PlayOneShot(this.AudioPointsBlue.clip, 0.1f);
        this.PSystem_500.Emit(emitParams, 1);
        break;
      case GrenadeSkeeballGame.PointsParticleType.Yellow:
        emitParams.position = pos;
        emitParams.velocity = Vector3.up * 5f;
        this.AudioPointsYellow.PlayOneShot(this.AudioPointsYellow.clip, 0.1f);
        this.PSystem_1000.Emit(emitParams, 1);
        break;
      case GrenadeSkeeballGame.PointsParticleType.Red:
        emitParams.position = pos;
        emitParams.velocity = Vector3.up * 5f;
        this.AudioPointsRed.PlayOneShot(this.AudioPointsRed.clip, 0.1f);
        this.PSystem_5000.Emit(emitParams, 1);
        break;
    }
  }

  public enum PointsParticleType
  {
    Blue,
    Yellow,
    Red,
  }
}
