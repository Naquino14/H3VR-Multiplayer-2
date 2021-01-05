// Decompiled with JetBrains decompiler
// Type: DinoFracture.FractureEngine
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace DinoFracture
{
  public sealed class FractureEngine : MonoBehaviour
  {
    private static FractureEngine _instance;
    private bool _suspended;
    private List<FractureEngine.FractureInstance> _runningFractures = new List<FractureEngine.FractureInstance>();

    private static FractureEngine Instance
    {
      get
      {
        if ((Object) FractureEngine._instance == (Object) null)
          FractureEngine._instance = new GameObject("Fracture Engine").AddComponent<FractureEngine>();
        return FractureEngine._instance;
      }
    }

    public static bool Suspended
    {
      get => FractureEngine.Instance._suspended;
      set => FractureEngine.Instance._suspended = value;
    }

    public static bool HasFracturesInProgress => FractureEngine.Instance._runningFractures.Count > 0;

    private void OnDestroy()
    {
      if (!((Object) FractureEngine._instance == (Object) this))
        return;
      FractureEngine._instance = (FractureEngine) null;
    }

    public static AsyncFractureResult StartFracture(
      FractureDetails details,
      FractureGeometry callback,
      Transform piecesParent,
      bool transferMass,
      bool hideAfterFracture)
    {
      AsyncFractureResult result = new AsyncFractureResult();
      if (FractureEngine.Suspended)
        result.SetResult((GameObject) null, new Bounds());
      else if (details.Asynchronous)
      {
        IEnumerator enumerator = FractureEngine.Instance.WaitForResults(FractureBuilder.Fracture(details), callback, piecesParent, transferMass, hideAfterFracture, result);
        if (enumerator.MoveNext())
          FractureEngine.Instance._runningFractures.Add(new FractureEngine.FractureInstance(result, enumerator));
      }
      else
      {
        IEnumerator enumerator = FractureEngine.Instance.WaitForResults(FractureBuilder.Fracture(details), callback, piecesParent, transferMass, hideAfterFracture, result);
        while (enumerator.MoveNext())
          UnityEngine.Debug.LogWarning((object) "DinoFracture: Sync fracture taking more than one iteration");
      }
      return result;
    }

    private void OnEditorUpdate()
    {
      this.UpdateFractures();
      if (this._runningFractures.Count != 0)
        return;
      Object.DestroyImmediate((Object) this.gameObject);
    }

    private void Update() => this.UpdateFractures();

    private void UpdateFractures()
    {
      for (int index = this._runningFractures.Count - 1; index >= 0; --index)
      {
        if (this._runningFractures[index].Result.StopRequested)
          this._runningFractures.RemoveAt(index);
        else if (!this._runningFractures[index].Enumerator.MoveNext())
          this._runningFractures.RemoveAt(index);
      }
    }

    [DebuggerHidden]
    private IEnumerator WaitForResults(
      AsyncFractureOperation operation,
      FractureGeometry callback,
      Transform piecesParent,
      bool transferMass,
      bool hideAfterFracture,
      AsyncFractureResult result)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FractureEngine.\u003CWaitForResults\u003Ec__Iterator0()
      {
        operation = operation,
        transferMass = transferMass,
        callback = callback,
        piecesParent = piecesParent,
        hideAfterFracture = hideAfterFracture,
        result = result
      };
    }

    private struct FractureInstance
    {
      public AsyncFractureResult Result;
      public IEnumerator Enumerator;

      public FractureInstance(AsyncFractureResult result, IEnumerator enumerator)
      {
        this.Result = result;
        this.Enumerator = enumerator;
      }
    }
  }
}
