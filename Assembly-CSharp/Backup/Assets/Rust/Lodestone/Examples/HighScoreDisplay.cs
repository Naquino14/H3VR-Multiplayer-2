// Decompiled with JetBrains decompiler
// Type: Assets.Rust.Lodestone.Examples.HighScoreDisplay
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Rust.Lodestone.Examples
{
  public class HighScoreDisplay : MonoBehaviour, IRequester
  {
    [SerializeField]
    private InputField nameText;
    [SerializeField]
    private InputField scoreText;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private Text[] highScoresTexts;
    [SerializeField]
    private float updateTick = 1f;
    [SerializeField]
    private bool showOneEntryPerPlayerName;
    private float updateTimer;
    private bool hasActiveQuery;

    private void Start() => this.updateTimer = this.updateTick;

    private void Update()
    {
      if ((double) this.updateTimer >= (double) this.updateTick)
      {
        if (this.hasActiveQuery)
          return;
        if (this.showOneEntryPerPlayerName)
          this.hasActiveQuery = (double) Assets.Rust.Lodestone.Lodestone.GetLogs((IRequester) this, "testHighScore", "score", 10, false, filterFieldValues: new SortedList<string, string>()
          {
            {
              "name",
              "unique"
            }
          }) > 0.0;
        else
          this.hasActiveQuery = (double) Assets.Rust.Lodestone.Lodestone.GetLogs((IRequester) this, "testHighScore", "score", 10, false) > 0.0;
        this.updateTimer = 0.0f;
      }
      else
        this.updateTimer += Time.deltaTime;
    }

    public void LogScore()
    {
      if (this.nameText.text == string.Empty || this.scoreText.text == string.Empty)
        return;
      Assets.Rust.Lodestone.Lodestone.Log("testHighScore", new SortedList<string, object>()
      {
        {
          "name",
          (object) this.nameText.text
        },
        {
          "score",
          (object) int.Parse(this.scoreText.text)
        }
      });
      this.nameText.text = string.Empty;
      this.scoreText.text = string.Empty;
    }

    public void HandleResponse(
      string endpointName,
      float startTime,
      List<KeyValuePair<string, string>> fieldTypes,
      List<Dictionary<string, object>> fieldValues)
    {
      for (int index = 0; index < this.highScoresTexts.Length; ++index)
      {
        if (index < fieldValues.Count)
          this.highScoresTexts[index].text = (index + 1).ToString() + ": " + fieldValues[index]["name"] + " - " + fieldValues[index]["score"];
        else
          this.highScoresTexts[index].text = string.Empty;
      }
      this.hasActiveQuery = false;
    }
  }
}
