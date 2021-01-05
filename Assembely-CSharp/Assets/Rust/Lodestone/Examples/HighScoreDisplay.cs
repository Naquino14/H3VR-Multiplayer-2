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

		private void Start()
		{
			updateTimer = updateTick;
		}

		private void Update()
		{
			if (updateTimer >= updateTick)
			{
				if (!hasActiveQuery)
				{
					if (showOneEntryPerPlayerName)
					{
						hasActiveQuery = Lodestone.GetLogs(this, "testHighScore", "score", 10, asc: false, overridePreviousRequest: false, new SortedList<string, string>
						{
							{
								"name",
								"unique"
							}
						}) > 0f;
					}
					else
					{
						hasActiveQuery = Lodestone.GetLogs(this, "testHighScore", "score", 10, asc: false) > 0f;
					}
					updateTimer = 0f;
				}
			}
			else
			{
				updateTimer += Time.deltaTime;
			}
		}

		public void LogScore()
		{
			if (!(nameText.text == string.Empty) && !(scoreText.text == string.Empty))
			{
				Lodestone.Log("testHighScore", new SortedList<string, object>
				{
					{
						"name",
						nameText.text
					},
					{
						"score",
						int.Parse(scoreText.text)
					}
				});
				nameText.text = string.Empty;
				scoreText.text = string.Empty;
			}
		}

		public void HandleResponse(string endpointName, float startTime, List<KeyValuePair<string, string>> fieldTypes, List<Dictionary<string, object>> fieldValues)
		{
			for (int i = 0; i < highScoresTexts.Length; i++)
			{
				if (i < fieldValues.Count)
				{
					highScoresTexts[i].text = string.Concat(i + 1, ": ", fieldValues[i]["name"], " - ", fieldValues[i]["score"]);
				}
				else
				{
					highScoresTexts[i].text = string.Empty;
				}
			}
			hasActiveQuery = false;
		}
	}
}
