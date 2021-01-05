using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Rust.Lodestone.Plugins;
using UnityEngine;

namespace Assets.Rust.Lodestone
{
	public class Lodestone : MonoBehaviour
	{
		private class LogEntry
		{
			public readonly string endpointName;

			private readonly SortedList<string, object> _fields;

			private readonly string[] _tags;

			private readonly string _playerId;

			public LogEntry(string name, SortedList<string, object> fields, string playerId, string[] endpointTags = null)
			{
				endpointName = name;
				if (endpointTags != null)
				{
					_tags = endpointTags;
				}
				_fields = fields;
				_playerId = playerId;
			}

			public override string ToString()
			{
				string text = "{\"p\":\"" + _playerId + "\"";
				if (_tags != null)
				{
					text = text + ",\"t\":[\"" + string.Join("\",\"", _tags) + "\"]";
				}
				text += ",\"d\":{";
				int num = 0;
				foreach (KeyValuePair<string, object> field in _fields)
				{
					string text2;
					string text3;
					switch (field.Value.GetType().ToString().Replace("UnityEngine.", string.Empty)
						.Replace("System.", string.Empty))
					{
					case "Int32":
						text2 = "i";
						text3 = field.Value.ToString();
						break;
					case "Single":
					case "Double":
					case "Decimal":
						text2 = "f";
						text3 = field.Value.ToString();
						break;
					case "Boolean":
						text2 = "b";
						text3 = ((!(bool)field.Value) ? "0" : "1");
						break;
					case "Vector4":
					{
						text2 = "v4";
						Vector4 vector3 = (Vector4)field.Value;
						text3 = "[" + vector3.x + "," + vector3.y + "," + vector3.z + "," + vector3.w + "," + vector3.magnitude + "]";
						break;
					}
					case "Vector3":
					{
						text2 = "v3";
						Vector3 vector2 = (Vector3)field.Value;
						text3 = "[" + vector2.x + "," + vector2.y + "," + vector2.z + "," + vector2.magnitude + "]";
						break;
					}
					case "Vector2":
					{
						text2 = "v2";
						Vector2 vector = (Vector2)field.Value;
						text3 = "[" + vector.x + "," + vector.y + "," + vector.magnitude + "]";
						break;
					}
					default:
						text2 = "s";
						text3 = string.Concat("\"", field.Value, "\"");
						break;
					}
					string text4 = text;
					text = text4 + "\"" + field.Key + "\":[\"" + text2 + "\"," + text3 + "]" + ((++num >= _fields.Count) ? string.Empty : ",");
				}
				return text + "}}";
			}
		}

		[HideInInspector]
		public string testStatus = string.Empty;

		[SerializeField]
		private string _projectSecret = string.Empty;

		[SerializeField]
		private string _projectKey = string.Empty;

		[SerializeField]
		private string _projectVersion = "v1.0";

		[SerializeField]
		private string _serverLocation = "https://lodestone";

		public static string userId = string.Empty;

		private static Dictionary<IRequester, Coroutine> _currentRequests = new Dictionary<IRequester, Coroutine>();

		public bool showDebugInfo;

		private const float SecsBetweenPush = 0.5f;

		private const int LogsToPush = 5;

		private static Lodestone _instance;

		private static Queue<LogEntry> _logs = new Queue<LogEntry>();

		public static void Log(string endpointName, string fieldName, object fieldValue, string[] endpointTags = null)
		{
			SortedList<string, object> sortedList = new SortedList<string, object>();
			sortedList.Add(fieldName, fieldValue);
			SortedList<string, object> fields = sortedList;
			_logs.Enqueue(new LogEntry(endpointName, fields, userId, endpointTags));
			if (_instance.showDebugInfo)
			{
				Debug.Log("Added Log Entry With 1 Field");
			}
		}

		public static void Log(string endpointName, SortedList<string, object> fields, string[] endpointTags = null)
		{
			_logs.Enqueue(new LogEntry(endpointName, fields, userId, endpointTags));
			if (_instance.showDebugInfo)
			{
				Debug.Log("Added Log Entry With " + fields.Count + " Fields");
			}
		}

		private static IEnumerator PushLogs()
		{
			while (Application.isPlaying)
			{
				if (_logs.Count > 0)
				{
					SortedList<string, string> logEndpoints = new SortedList<string, string>();
					float i = 0f;
					while (_logs.Count > 0 && i <= 5f)
					{
						LogEntry logEntry = _logs.Dequeue();
						if (logEndpoints.ContainsKey(logEntry.endpointName))
						{
							SortedList<string, string> sortedList;
							string endpointName;
							(sortedList = logEndpoints)[endpointName = logEntry.endpointName] = sortedList[endpointName] + ',' + logEntry.ToString();
						}
						else
						{
							logEndpoints.Add(logEntry.endpointName, logEntry.ToString());
						}
						i += 1f;
					}
					int index = 0;
					string data2 = "[\"" + SystemInfo.deviceUniqueIdentifier + "\",\"" + _instance._projectVersion + "\",";
					foreach (KeyValuePair<string, string> item in logEndpoints)
					{
						string text = data2;
						data2 = text + "{\"" + item.Key + "\":" + item.Value + "}";
						int num;
						index = (num = index + 1);
						if (num < logEndpoints.Count)
						{
							data2 += ',';
						}
					}
					data2 += "]";
					if (_instance.showDebugInfo)
					{
						Debug.Log("Sending: " + data2);
					}
					WWWForm form = new WWWForm();
					form.AddField("data", data2);
					string hash = MD5.Sum(_instance._projectSecret + data2);
					form.AddField("hash", hash);
					form.AddField("key", _instance._projectKey);
					WWW response = new WWW(_instance._serverLocation + "/q/logs/create", form);
					yield return response;
					if (!string.IsNullOrEmpty(response.error))
					{
						Debug.LogError("ERROR: " + response.error + response.url);
					}
					else if (!string.IsNullOrEmpty(response.text))
					{
						Debug.LogError("ERROR: " + response.text);
					}
				}
				yield return new WaitForSeconds(0.5f);
			}
		}

		public void Test()
		{
			StartCoroutine(RunTest());
		}

		public void Awake()
		{
			if (userId == string.Empty)
			{
				userId = SystemInfo.deviceUniqueIdentifier;
			}
			Lodestone[] array = UnityEngine.Object.FindObjectsOfType<Lodestone>();
			if (array.Length > 1 && array[0] != this)
			{
				Debug.LogWarning("There are currently " + array.Length + " Lodestone instances. There should only be one. Temporarily removing all but first instance.");
				UnityEngine.Object.Destroy(this);
			}
			_instance = this;
			testStatus = string.Empty;
			StartCoroutine(PushLogs());
		}

		private IEnumerator RunTest()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			string text = Guid.NewGuid().ToString();
			string text2 = "{\"test\":\"" + text + "\"}";
			string value = MD5.Sum(_projectSecret + text2);
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("data", text2);
			wWWForm.AddField("hash", value);
			wWWForm.AddField("key", _projectKey);
			WWW wWW = new WWW(_serverLocation + "/q/test", wWWForm);
			testStatus = "Loading...";
			float num = Time.realtimeSinceStartup + 30f;
			while (Time.realtimeSinceStartup < num && !wWW.isDone)
			{
			}
			if (!wWW.isDone)
			{
				testStatus = "ERROR: Timeout! Please check Base URL.";
			}
			else if (!string.IsNullOrEmpty(wWW.error))
			{
				Debug.Log("Error testing: " + wWW.error + " (" + wWW.url + ")");
				testStatus = "ERROR: See console.";
			}
			else if (wWW.text == text)
			{
				testStatus = "Success! (" + (Time.realtimeSinceStartup - realtimeSinceStartup) * 1000f + "ms)";
			}
			else
			{
				testStatus = "ERROR: " + wWW.text;
			}
			yield break;
		}

		public static float GetLogs(IRequester origin, string endpointName, string sortBy, int count = 100, bool asc = true, bool overridePreviousRequest = false, SortedList<string, string> filterFieldValues = null, bool currentPlayerOnly = false)
		{
			if (_currentRequests.ContainsKey(origin))
			{
				if (!overridePreviousRequest)
				{
					return -1f;
				}
				_instance.StopCoroutine(_currentRequests[origin]);
				_currentRequests.Remove(origin);
			}
			float time = Time.time;
			_currentRequests.Add(origin, _instance.StartCoroutine(LoadLogs(origin, time, endpointName, sortBy + ((!asc) ? " DESC" : " ASC"), count, filterFieldValues, currentPlayerOnly)));
			return time;
		}

		private static IEnumerator LoadLogs(IRequester requester, float startTime, string endpointName, string sortBy, int count, SortedList<string, string> filterFieldValues = null, bool currentPlayerOnly = false)
		{
			WWWForm form = new WWWForm();
			string data = "{\"endpoint\":\"" + endpointName + "\",\"sort\":\"" + sortBy + "\",\"count\":" + count;
			if (filterFieldValues != null)
			{
				data += ",\"filter\":{";
				IList<string> keys = filterFieldValues.Keys;
				int num = 0;
				while (num < keys.Count)
				{
					string text = data;
					data = text + "\"" + keys[num] + "\":\"" + filterFieldValues[keys[num]] + "\"";
					if (++num < keys.Count)
					{
						data += ",";
					}
				}
				data += "}";
			}
			if (currentPlayerOnly)
			{
				data = data + ",\"limitPlayer\":\"" + userId + "\"";
			}
			data += "}";
			form.AddField("data", data);
			if (_instance.showDebugInfo)
			{
				Debug.Log("Sending: " + data);
			}
			string hash = MD5.Sum(_instance._projectSecret + data);
			form.AddField("hash", hash);
			form.AddField("key", _instance._projectKey);
			WWW response = new WWW(_instance._serverLocation + "/q/logs/query", form);
			yield return response;
			if (!string.IsNullOrEmpty(response.error) || string.IsNullOrEmpty(response.text))
			{
				yield break;
			}
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			List<Dictionary<string, object>> list2 = new List<Dictionary<string, object>>();
			string[] array = response.text.Split(new char[1]
			{
				'|'
			}, 2);
			string[] array2 = array[0].Split(',');
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(new char[1]
				{
					':'
				}, 2);
				list.Add(new KeyValuePair<string, string>(array3[0], array3[1]));
			}
			string[] array4 = array[1].Split('&');
			string[] array5 = array4;
			foreach (string text2 in array5)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (text2 == string.Empty)
				{
					continue;
				}
				string[] array6 = text2.Split(',');
				for (int k = 0; k < array6.Length; k++)
				{
					object value = null;
					if (array6.Length > k)
					{
						switch (list[k].Value)
						{
						case "i":
							value = int.Parse(array6[k]);
							break;
						case "f":
							value = float.Parse(array6[k]);
							break;
						case "b":
							value = array6[k] == "true";
							break;
						case "v2":
						case "v3":
						case "v4":
						{
							string[] array7 = array6[k].Split(';');
							value = list[k].Value switch
							{
								"v2" => new Vector2(float.Parse(array7[0]), float.Parse(array7[1])), 
								"v3" => new Vector3(float.Parse(array7[0]), float.Parse(array7[1]), float.Parse(array7[2])), 
								_ => new Vector4(float.Parse(array7[0]), float.Parse(array7[1]), float.Parse(array7[2]), float.Parse(array7[3])), 
							};
							break;
						}
						default:
							value = WWW.UnEscapeURL(array6[k]);
							break;
						}
					}
					dictionary.Add(list[k].Key, value);
				}
				list2.Add(dictionary);
			}
			_currentRequests.Remove(requester);
			requester.HandleResponse(endpointName, startTime, list, list2);
		}
	}
}
