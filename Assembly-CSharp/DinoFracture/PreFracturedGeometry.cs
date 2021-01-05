// Decompiled with JetBrains decompiler
// Type: DinoFracture.PreFracturedGeometry
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace DinoFracture
{
  public class PreFracturedGeometry : FractureGeometry
  {
    public GameObject GeneratedPieces;
    public Bounds EntireMeshBounds;
    private Action<PreFracturedGeometry> _completionCallback;
    private AsyncFractureResult _runningFracture;
    private bool _ignoreOnFractured;

    public AsyncFractureResult RunningFracture => this._runningFracture;

    private void Start() => this.Prime();

    public void Prime()
    {
      if (!((UnityEngine.Object) this.GeneratedPieces != (UnityEngine.Object) null))
        return;
      bool activeSelf = this.gameObject.activeSelf;
      this.gameObject.SetActive(false);
      this.GeneratedPieces.SetActive(true);
      this.GeneratedPieces.SetActive(false);
      this.gameObject.SetActive(activeSelf);
    }

    public void GenerateFractureMeshes(Action<PreFracturedGeometry> completedCallback) => this.GenerateFractureMeshes(Vector3.zero, completedCallback);

    public void GenerateFractureMeshes(
      Vector3 localPoint,
      Action<PreFracturedGeometry> completedCallback)
    {
      if (Application.isPlaying)
        Debug.LogWarning((object) "DinoFracture: Creating pre-fractured pieces at runtime.  This can be slow if there a lot of pieces.");
      if ((UnityEngine.Object) this.GeneratedPieces != (UnityEngine.Object) null)
      {
        if (Application.isPlaying)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.GeneratedPieces);
        else
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.GeneratedPieces);
      }
      this._runningFracture = this.Fracture(new FractureDetails()
      {
        NumPieces = this.NumFracturePieces,
        NumIterations = this.NumIterations,
        UVScale = FractureUVScale.Piece,
        Asynchronous = !Application.isPlaying,
        FractureCenter = localPoint,
        FractureRadius = this.FractureRadius
      }, false);
      this._completionCallback = completedCallback;
      if (!Application.isPlaying)
        return;
      if (!this._runningFracture.IsComplete)
        Debug.LogError((object) "DinoFracture: Prefracture task is not complete");
      this.OnPreFractureComplete();
    }

    public void StopRunningFracture()
    {
      this._runningFracture.StopFracture();
      this._runningFracture = (AsyncFractureResult) null;
      this.StopFracture();
    }

    private void EditorUpdate()
    {
      if (this._runningFracture == null || !this._runningFracture.IsComplete)
        return;
      this.OnPreFractureComplete();
    }

    private void OnPreFractureComplete()
    {
      this.GeneratedPieces = this._runningFracture.PiecesRoot;
      this.EntireMeshBounds = this._runningFracture.EntireMeshBounds;
      this.GeneratedPieces.SetActive(false);
      this._runningFracture = (AsyncFractureResult) null;
      if (this._completionCallback == null)
        return;
      this._completionCallback(this);
    }

    protected override AsyncFractureResult FractureInternal(Vector3 localPos)
    {
      if (this.gameObject.activeSelf)
      {
        if ((UnityEngine.Object) this.GeneratedPieces == (UnityEngine.Object) null)
        {
          this.GenerateFractureMeshes(localPos, (Action<PreFracturedGeometry>) null);
          this.EnableFracturePieces();
        }
        else
        {
          this.EnableFracturePieces();
          OnFractureEventArgs fractureEventArgs = new OnFractureEventArgs((FractureGeometry) this, this.GeneratedPieces);
          this._ignoreOnFractured = true;
          this.gameObject.SendMessage("OnFracture", (object) fractureEventArgs, SendMessageOptions.DontRequireReceiver);
          this._ignoreOnFractured = false;
          Transform transform = this.GeneratedPieces.transform;
          for (int index = 0; index < transform.childCount; ++index)
            transform.GetChild(index).gameObject.SendMessage("OnFracture", (object) fractureEventArgs, SendMessageOptions.DontRequireReceiver);
        }
        this.gameObject.SetActive(false);
        AsyncFractureResult asyncFractureResult = new AsyncFractureResult();
        asyncFractureResult.SetResult(this.GeneratedPieces, this.EntireMeshBounds);
        return asyncFractureResult;
      }
      AsyncFractureResult asyncFractureResult1 = new AsyncFractureResult();
      asyncFractureResult1.SetResult((GameObject) null, new Bounds());
      return asyncFractureResult1;
    }

    private void EnableFracturePieces()
    {
      Transform transform1 = this.GeneratedPieces.transform;
      Transform transform2 = this.transform;
      transform1.position = transform2.position;
      transform1.rotation = transform2.rotation;
      transform1.localScale = Vector3.one;
      this.GeneratedPieces.SetActive(true);
    }

    internal override void OnFracture(OnFractureEventArgs args)
    {
      if (this._ignoreOnFractured)
        return;
      base.OnFracture(args);
      this.GeneratedPieces = args.FracturePiecesRootObject;
      this.GeneratedPieces.SetActive(false);
    }
  }
}
