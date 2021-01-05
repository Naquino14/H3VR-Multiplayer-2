// Decompiled with JetBrains decompiler
// Type: FistVR.LawnDartGame
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class LawnDartGame : MonoBehaviour
  {
    [Header("LawnDart Config")]
    public GameObject DartPrefab;
    public Transform[] DartSpawnLocations;
    public ParticleSystem[] FireWorkLaunch;
    public ParticleSystem[] FireWorks;
    public LawnDartPointDisplay ScoreDisplay;
    private List<LawnDart> SpawnedDarts = new List<LawnDart>();
    public GameObject[] FireWorkSounds;
    public Text ScorePanel1;
    public Text ScorePanel2;
    public List<int> ScoreList = new List<int>();
    public List<string> ScoreLabelList = new List<string>();

    private void Start() => this.SpawnDartSet();

    private void SpawnDartSet()
    {
      for (int index = 0; index < this.DartSpawnLocations.Length; ++index)
      {
        LawnDart component = Object.Instantiate<GameObject>(this.DartPrefab, this.DartSpawnLocations[index].position, this.DartSpawnLocations[index].rotation).GetComponent<LawnDart>();
        component.SetGame(this);
        this.SpawnedDarts.Add(component);
      }
    }

    private void Update()
    {
    }

    public void FireWork(Vector3 pos)
    {
      this.FireWorks[Random.Range(0, this.FireWorks.Length)].Emit(new ParticleSystem.EmitParams()
      {
        position = pos,
        startSize = Random.Range(6f, 12f)
      }, 1);
      Object.Instantiate<GameObject>(this.FireWorkSounds[Random.Range(0, this.FireWorkSounds.Length)], pos, Quaternion.identity);
    }

    public void ScoreEvent(
      Vector3 pos,
      string displaytext,
      int points,
      int multiplier,
      LawnDart dart)
    {
      Object.Instantiate<GameObject>(this.FireWorkSounds[Random.Range(0, this.FireWorkSounds.Length)], pos, Quaternion.identity);
      this.ScoreDisplay.gameObject.SetActive(true);
      ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
      string txt = string.Empty;
      if (points > 0)
      {
        if (displaytext == string.Empty)
        {
          switch (multiplier)
          {
            case 1:
              txt = points.ToString();
              break;
            case 2:
              txt = points.ToString() + " x 2 = " + (points * 2).ToString();
              break;
            case 3:
              txt = points.ToString() + " x 3 = " + (points * 3).ToString();
              break;
          }
        }
        else
          txt = displaytext;
        this.ScoreDisplay.Activate(txt, pos + Vector3.up * 3f, 12f, multiplier);
        emitParams.position = pos + Vector3.up * 3f;
        this.FireWorkLaunch[Random.Range(0, this.FireWorkLaunch.Length)].Emit(emitParams, 1);
      }
      else
      {
        this.ScoreDisplay.Activate(displaytext, pos + Vector3.up * 3f, 10f, multiplier);
        emitParams.position = pos + Vector3.up * 3f;
        this.FireWorkLaunch[Random.Range(0, this.FireWorkLaunch.Length)].Emit(emitParams, 1);
      }
    }
  }
}
