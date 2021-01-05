// Decompiled with JetBrains decompiler
// Type: WFX_BulletHoleDecal
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof (MeshFilter))]
public class WFX_BulletHoleDecal : MonoBehaviour
{
  private static Vector2[] quadUVs = new Vector2[4]
  {
    new Vector2(0.0f, 0.0f),
    new Vector2(0.0f, 1f),
    new Vector2(1f, 0.0f),
    new Vector2(1f, 1f)
  };
  public float lifetime = 10f;
  public float fadeoutpercent = 80f;
  public Vector2 frames;
  public bool randomRotation;
  public bool deactivate;
  private float life;
  private float fadeout;
  private Color color;
  private float orgAlpha;

  private void Awake()
  {
    this.color = this.GetComponent<Renderer>().material.GetColor("_TintColor");
    this.orgAlpha = this.color.a;
  }

  private void OnEnable()
  {
    int num1 = Random.Range(0, (int) ((double) this.frames.x * (double) this.frames.y));
    int num2 = (int) ((double) num1 % (double) this.frames.x);
    int num3 = (int) ((double) num1 / (double) this.frames.y);
    Vector2[] vector2Array = new Vector2[4];
    for (int index = 0; index < 4; ++index)
    {
      vector2Array[index].x = (float) (((double) WFX_BulletHoleDecal.quadUVs[index].x + (double) num2) * (1.0 / (double) this.frames.x));
      vector2Array[index].y = (float) (((double) WFX_BulletHoleDecal.quadUVs[index].y + (double) num3) * (1.0 / (double) this.frames.y));
    }
    this.GetComponent<MeshFilter>().mesh.uv = vector2Array;
    if (this.randomRotation)
      this.transform.Rotate(0.0f, 0.0f, Random.Range(0.0f, 360f), Space.Self);
    this.life = this.lifetime;
    this.fadeout = this.life * (this.fadeoutpercent / 100f);
    this.color.a = this.orgAlpha;
    this.GetComponent<Renderer>().material.SetColor("_TintColor", this.color);
    this.StopAllCoroutines();
    this.StartCoroutine("holeUpdate");
  }

  [DebuggerHidden]
  private IEnumerator holeUpdate() => (IEnumerator) new WFX_BulletHoleDecal.\u003CholeUpdate\u003Ec__Iterator0()
  {
    \u0024this = this
  };
}
