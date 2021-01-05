// Decompiled with JetBrains decompiler
// Type: AnimateTiledTextureOnTrail
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AnimateTiledTextureOnTrail : MonoBehaviour
{
  public int _columns = 2;
  public int _rows = 2;
  public Vector2 _scale = (Vector2) new Vector3(1f, 1f);
  public Vector2 _offset = Vector2.zero;
  public Vector2 _buffer = Vector2.zero;
  public float _framesPerSecond = 10f;
  public bool _playOnce;
  public bool _disableUponCompletion;
  public bool _enableEvents;
  public bool _playOnEnable = true;
  public bool _newMaterialInstance;
  private int _index;
  private Vector2 _textureSize = Vector2.zero;
  private Material _materialInstance;
  private bool _hasMaterialInstance;
  private bool _isPlaying;
  private List<AnimateTiledTextureOnTrail.VoidEvent> _voidEventCallbackList;

  public void RegisterCallback(AnimateTiledTextureOnTrail.VoidEvent cbFunction)
  {
    if (this._enableEvents)
      this._voidEventCallbackList.Add(cbFunction);
    else
      UnityEngine.Debug.LogWarning((object) "AnimateTiledTextureOnTrail: You are attempting to register a callback but the events of this object are not enabled!");
  }

  public void UnRegisterCallback(AnimateTiledTextureOnTrail.VoidEvent cbFunction)
  {
    if (this._enableEvents)
      this._voidEventCallbackList.Remove(cbFunction);
    else
      UnityEngine.Debug.LogWarning((object) "AnimateTiledTextureOnTrail: You are attempting to un-register a callback but the events of this object are not enabled!");
  }

  public void Play()
  {
    if (this._isPlaying)
    {
      this.StopCoroutine("updateTiling");
      this._isPlaying = false;
    }
    this.GetComponent<TrailRenderer>().enabled = true;
    this._index = this._columns;
    this.StartCoroutine(this.updateTiling());
  }

  public void ChangeMaterial(Material newMaterial, bool newInstance = false)
  {
    if (newInstance)
    {
      if (this._hasMaterialInstance)
        Object.Destroy((Object) this.GetComponent<TrailRenderer>().sharedMaterial);
      this._materialInstance = new Material(newMaterial);
      this.GetComponent<TrailRenderer>().sharedMaterial = this._materialInstance;
      this._hasMaterialInstance = true;
    }
    else
      this.GetComponent<TrailRenderer>().sharedMaterial = newMaterial;
    this.CalcTextureSize();
    this.GetComponent<TrailRenderer>().sharedMaterial.SetTextureScale("_MainTex", this._textureSize);
  }

  private void Awake()
  {
    if (this._enableEvents)
      this._voidEventCallbackList = new List<AnimateTiledTextureOnTrail.VoidEvent>();
    this.ChangeMaterial(this.GetComponent<TrailRenderer>().sharedMaterial, this._newMaterialInstance);
  }

  private void OnDestroy()
  {
    if (!this._hasMaterialInstance)
      return;
    Object.Destroy((Object) this.GetComponent<TrailRenderer>().sharedMaterial);
    this._hasMaterialInstance = false;
  }

  private void HandleCallbacks(List<AnimateTiledTextureOnTrail.VoidEvent> cbList)
  {
    for (int index = 0; index < cbList.Count; ++index)
      cbList[index]();
  }

  private void OnEnable()
  {
    this.CalcTextureSize();
    if (!this._playOnEnable)
      return;
    this.Play();
  }

  private void CalcTextureSize()
  {
    this._textureSize = new Vector2(1f / (float) this._columns, 1f / (float) this._rows);
    this._textureSize.x /= this._scale.x;
    this._textureSize.y /= this._scale.y;
    this._textureSize -= this._buffer;
  }

  [DebuggerHidden]
  private IEnumerator updateTiling() => (IEnumerator) new AnimateTiledTextureOnTrail.\u003CupdateTiling\u003Ec__Iterator0()
  {
    \u0024this = this
  };

  private void ApplyOffset()
  {
    Vector2 vector2 = new Vector2((float) this._index / (float) this._columns - (float) (this._index / this._columns), (float) (1.0 - (double) (this._index / this._columns) / (double) this._rows));
    if ((double) vector2.y == 1.0)
      vector2.y = 0.0f;
    vector2.x += (float) ((1.0 / (double) this._columns - (double) this._textureSize.x) / 2.0);
    vector2.y += (float) ((1.0 / (double) this._rows - (double) this._textureSize.y) / 2.0);
    vector2.x += this._offset.x;
    vector2.y += this._offset.y;
    this.GetComponent<TrailRenderer>().sharedMaterial.SetTextureOffset("_MainTex", vector2);
  }

  public delegate void VoidEvent();
}
