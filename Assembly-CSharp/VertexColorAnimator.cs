// Decompiled with JetBrains decompiler
// Type: VertexColorAnimator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class VertexColorAnimator : MonoBehaviour
{
  public List<MeshHolder> animationMeshes;
  public List<float> animationKeyframes;
  public float timeScale = 2f;
  public int mode;
  private float elapsedTime;

  public void initLists()
  {
    this.animationMeshes = new List<MeshHolder>();
    this.animationKeyframes = new List<float>();
  }

  public void addMesh(Mesh mesh, float atPosition)
  {
    MeshHolder meshHolder = new MeshHolder();
    meshHolder.setAnimationData(mesh);
    this.animationMeshes.Add(meshHolder);
    this.animationKeyframes.Add(atPosition);
  }

  private void Start() => this.elapsedTime = 0.0f;

  public void replaceKeyframe(int frameIndex, Mesh mesh) => this.animationMeshes[frameIndex].setAnimationData(mesh);

  public void deleteKeyframe(int frameIndex)
  {
    this.animationMeshes.RemoveAt(frameIndex);
    this.animationKeyframes.RemoveAt(frameIndex);
  }

  public void scrobble(float scrobblePos)
  {
    if (this.animationMeshes.Count == 0)
      return;
    Color[] _vertexColors = new Color[this.GetComponent<MeshFilter>().sharedMesh.colors.Length];
    int index1 = 0;
    for (int index2 = 0; index2 < this.animationKeyframes.Count; ++index2)
    {
      if ((double) scrobblePos >= (double) this.animationKeyframes[index2])
        index1 = index2;
    }
    if (index1 >= this.animationKeyframes.Count - 1)
    {
      this.GetComponent<VertexColorStream>().setColors(this.animationMeshes[index1]._colors);
    }
    else
    {
      float num = this.animationKeyframes[index1 + 1] - this.animationKeyframes[index1];
      float animationKeyframe = this.animationKeyframes[index1];
      float t = (scrobblePos - animationKeyframe) / num;
      for (int index2 = 0; index2 < _vertexColors.Length; ++index2)
        _vertexColors[index2] = Color.Lerp(this.animationMeshes[index1]._colors[index2], this.animationMeshes[index1 + 1]._colors[index2], t);
      this.GetComponent<VertexColorStream>().setColors(_vertexColors);
    }
  }

  private void Update()
  {
    if (this.mode == 0)
      this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
    else if (this.mode == 1)
    {
      this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
      if ((double) this.elapsedTime > 1.0)
        this.elapsedTime = 0.0f;
    }
    else if (this.mode == 2)
    {
      if (Mathf.FloorToInt(Time.fixedTime / this.timeScale) % 2 == 0)
        this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
      else
        this.elapsedTime -= Time.fixedDeltaTime / this.timeScale;
    }
    Color[] _vertexColors = new Color[this.GetComponent<MeshFilter>().sharedMesh.colors.Length];
    int index1 = 0;
    for (int index2 = 0; index2 < this.animationKeyframes.Count; ++index2)
    {
      if ((double) this.elapsedTime >= (double) this.animationKeyframes[index2])
        index1 = index2;
    }
    if (index1 < this.animationKeyframes.Count - 1)
    {
      float num = this.animationKeyframes[index1 + 1] - this.animationKeyframes[index1];
      float t = (this.elapsedTime - this.animationKeyframes[index1]) / num;
      for (int index2 = 0; index2 < _vertexColors.Length; ++index2)
        _vertexColors[index2] = Color.Lerp(this.animationMeshes[index1]._colors[index2], this.animationMeshes[index1 + 1]._colors[index2], t);
    }
    else
      _vertexColors = this.animationMeshes[index1]._colors;
    this.GetComponent<VertexColorStream>().setColors(_vertexColors);
  }
}
