// Decompiled with JetBrains decompiler
// Type: Assets.Rust.Lodestone.Lodestone
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Rust.Lodestone
{
  public class Lodestone : MonoBehaviour
  {
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
    private static Assets.Rust.Lodestone.Lodestone _instance;
    private static Queue<Assets.Rust.Lodestone.Lodestone.LogEntry> _logs = new Queue<Assets.Rust.Lodestone.Lodestone.LogEntry>();

    public static void Log(
      string endpointName,
      string fieldName,
      object fieldValue,
      string[] endpointTags = null)
    {
      SortedList<string, object> fields = new SortedList<string, object>()
      {
        {
          fieldName,
          fieldValue
        }
      };
      Assets.Rust.Lodestone.Lodestone._logs.Enqueue(new Assets.Rust.Lodestone.Lodestone.LogEntry(endpointName, fields, Assets.Rust.Lodestone.Lodestone.userId, endpointTags));
      if (!Assets.Rust.Lodestone.Lodestone._instance.showDebugInfo)
        return;
      UnityEngine.Debug.Log((object) "Added Log Entry With 1 Field");
    }

    public static void Log(
      string endpointName,
      SortedList<string, object> fields,
      string[] endpointTags = null)
    {
      Assets.Rust.Lodestone.Lodestone._logs.Enqueue(new Assets.Rust.Lodestone.Lodestone.LogEntry(endpointName, fields, Assets.Rust.Lodestone.Lodestone.userId, endpointTags));
      if (!Assets.Rust.Lodestone.Lodestone._instance.showDebugInfo)
        return;
      UnityEngine.Debug.Log((object) ("Added Log Entry With " + (object) fields.Count + " Fields"));
    }

    [DebuggerHidden]
    private static IEnumerator PushLogs()
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Assets.Rust.Lodestone.Lodestone.\u003CPushLogs\u003Ec__Iterator0 pushLogsCIterator0 = new Assets.Rust.Lodestone.Lodestone.\u003CPushLogs\u003Ec__Iterator0();
      return (IEnumerator) pushLogsCIterator0;
    }

    public void Test() => this.StartCoroutine(this.RunTest());

    public void Awake()
    {
      if (Assets.Rust.Lodestone.Lodestone.userId == string.Empty)
        Assets.Rust.Lodestone.Lodestone.userId = SystemInfo.deviceUniqueIdentifier;
      Assets.Rust.Lodestone.Lodestone[] objectsOfType = Object.FindObjectsOfType<Assets.Rust.Lodestone.Lodestone>();
      if (objectsOfType.Length > 1 && (Object) objectsOfType[0] != (Object) this)
      {
        UnityEngine.Debug.LogWarning((object) ("There are currently " + (object) objectsOfType.Length + " Lodestone instances. There should only be one. Temporarily removing all but first instance."));
        Object.Destroy((Object) this);
      }
      Assets.Rust.Lodestone.Lodestone._instance = this;
      this.testStatus = string.Empty;
      this.StartCoroutine(Assets.Rust.Lodestone.Lodestone.PushLogs());
    }

    [DebuggerHidden]
    private IEnumerator RunTest() => (IEnumerator) new Assets.Rust.Lodestone.Lodestone.\u003CRunTest\u003Ec__Iterator1()
    {
      \u0024this = this
    };

    public static float GetLogs(
      IRequester origin,
      string endpointName,
      string sortBy,
      int count = 100,
      bool asc = true,
      bool overridePreviousRequest = false,
      SortedList<string, string> filterFieldValues = null,
      bool currentPlayerOnly = false)
    {
      if (Assets.Rust.Lodestone.Lodestone._currentRequests.ContainsKey(origin))
      {
        if (!overridePreviousRequest)
          return -1f;
        Assets.Rust.Lodestone.Lodestone._instance.StopCoroutine(Assets.Rust.Lodestone.Lodestone._currentRequests[origin]);
        Assets.Rust.Lodestone.Lodestone._currentRequests.Remove(origin);
      }
      float time = Time.time;
      Assets.Rust.Lodestone.Lodestone._currentRequests.Add(origin, Assets.Rust.Lodestone.Lodestone._instance.StartCoroutine(Assets.Rust.Lodestone.Lodestone.LoadLogs(origin, time, endpointName, sortBy + (!asc ? " DESC" : " ASC"), count, filterFieldValues, currentPlayerOnly)));
      return time;
    }

    [DebuggerHidden]
    private static IEnumerator LoadLogs(
      IRequester requester,
      float startTime,
      string endpointName,
      string sortBy,
      int count,
      SortedList<string, string> filterFieldValues = null,
      bool currentPlayerOnly = false)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Assets.Rust.Lodestone.Lodestone.\u003CLoadLogs\u003Ec__Iterator2()
      {
        endpointName = endpointName,
        sortBy = sortBy,
        count = count,
        filterFieldValues = filterFieldValues,
        currentPlayerOnly = currentPlayerOnly,
        requester = requester,
        startTime = startTime
      };
    }

    private class LogEntry
    {
      public readonly string endpointName;
      private readonly SortedList<string, object> _fields;
      private readonly string[] _tags;
      private readonly string _playerId;

      public LogEntry(
        string name,
        SortedList<string, object> fields,
        string playerId,
        string[] endpointTags = null)
      {
        this.endpointName = name;
        if (endpointTags != null)
          this._tags = endpointTags;
        this._fields = fields;
        this._playerId = playerId;
      }

      public override string ToString()
      {
        string str1 = "{\"p\":\"" + this._playerId + "\"";
        if (this._tags != null)
          str1 = str1 + ",\"t\":[\"" + string.Join("\",\"", this._tags) + "\"]";
        string str2 = str1 + ",\"d\":{";
        int num1 = 0;
        foreach (KeyValuePair<string, object> field in this._fields)
        {
          string key = field.Value.GetType().ToString().Replace("UnityEngine.", string.Empty).Replace("System.", string.Empty);
          string str3;
          string str4;
          if (key != null)
          {
            // ISSUE: reference to a compiler-generated field
            if (Assets.Rust.Lodestone.Lodestone.LogEntry.\u003C\u003Ef__switch\u0024map4 == null)
            {
              // ISSUE: reference to a compiler-generated field
              Assets.Rust.Lodestone.Lodestone.LogEntry.\u003C\u003Ef__switch\u0024map4 = new Dictionary<string, int>(8)
              {
                {
                  "Int32",
                  0
                },
                {
                  "Single",
                  1
                },
                {
                  "Double",
                  1
                },
                {
                  "Decimal",
                  1
                },
                {
                  "Boolean",
                  2
                },
                {
                  "Vector4",
                  3
                },
                {
                  "Vector3",
                  4
                },
                {
                  "Vector2",
                  5
                }
              };
            }
            int num2;
            // ISSUE: reference to a compiler-generated field
            if (Assets.Rust.Lodestone.Lodestone.LogEntry.\u003C\u003Ef__switch\u0024map4.TryGetValue(key, out num2))
            {
              switch (num2)
              {
                case 0:
                  str3 = "i";
                  str4 = field.Value.ToString();
                  goto label_16;
                case 1:
                  str3 = "f";
                  str4 = field.Value.ToString();
                  goto label_16;
                case 2:
                  str3 = "b";
                  str4 = !(bool) field.Value ? "0" : "1";
                  goto label_16;
                case 3:
                  str3 = "v4";
                  Vector4 vector4 = (Vector4) field.Value;
                  str4 = "[" + (object) vector4.x + "," + (object) vector4.y + "," + (object) vector4.z + "," + (object) vector4.w + "," + (object) vector4.magnitude + "]";
                  goto label_16;
                case 4:
                  str3 = "v3";
                  Vector3 vector3 = (Vector3) field.Value;
                  str4 = "[" + (object) vector3.x + "," + (object) vector3.y + "," + (object) vector3.z + "," + (object) vector3.magnitude + "]";
                  goto label_16;
                case 5:
                  str3 = "v2";
                  Vector2 vector2 = (Vector2) field.Value;
                  str4 = "[" + (object) vector2.x + "," + (object) vector2.y + "," + (object) vector2.magnitude + "]";
                  goto label_16;
              }
            }
          }
          str3 = "s";
          str4 = "\"" + field.Value + "\"";
label_16:
          str2 = str2 + "\"" + field.Key + "\":[\"" + str3 + "\"," + str4 + "]" + (++num1 >= this._fields.Count ? string.Empty : ",");
        }
        return str2 + "}}";
      }
    }
  }
}
