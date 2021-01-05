// Decompiled with JetBrains decompiler
// Type: FistVR.OmniShape
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class OmniShape : MonoBehaviour, IFVRDamageable
  {
    private bool m_isDestroyed;
    private OmniSpawner_Shapes m_spawner;
    private bool m_isCorrectTarget;
    public Renderer Renderer;
    public OmniSpawnDef_Shape.OmniShapeColor Color;
    public List<UnityEngine.Color> DisplayColors = new List<UnityEngine.Color>()
    {
      new UnityEngine.Color(1f, 0.0f, 0.0f, 1f),
      new UnityEngine.Color(1f, 0.6f, 0.0f, 1f),
      new UnityEngine.Color(1f, 1f, 0.0f, 1f),
      new UnityEngine.Color(0.2f, 0.8f, 0.2f, 1f),
      new UnityEngine.Color(0.0f, 0.4f, 1f, 1f),
      new UnityEngine.Color(0.6f, 0.0f, 0.6f, 1f),
      new UnityEngine.Color(1f, 0.07f, 0.58f, 1f),
      new UnityEngine.Color(0.63f, 0.32f, 0.16f, 1f)
    };
    private float m_tick;
    private OmniShape.ShapeState m_state;

    public void Init(
      OmniSpawner_Shapes spawner,
      bool isCorrectTarget,
      OmniSpawnDef_Shape.OmniShapeColor color)
    {
      this.m_spawner = spawner;
      this.m_isCorrectTarget = isCorrectTarget;
      this.m_state = OmniShape.ShapeState.Growing;
      this.Color = color;
      this.Renderer.material.SetColor("_Color", this.DisplayColors[(int) color]);
    }

    public void Damage(FistVR.Damage dam)
    {
      if (this.m_state != OmniShape.ShapeState.Shootable || this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      this.m_spawner.ShapeStruck(this.m_isCorrectTarget);
      this.TurnOff();
    }

    public void Update()
    {
      switch (this.m_state)
      {
        case OmniShape.ShapeState.Growing:
          this.m_tick += Time.deltaTime * 4f;
          bool flag1 = false;
          if ((double) this.m_tick > 1.0)
          {
            this.m_tick = 1f;
            flag1 = true;
          }
          this.transform.localScale = Vector3.Lerp(Vector3.one * 0.02f, Vector3.one, this.m_tick);
          if (!flag1)
            break;
          this.m_state = OmniShape.ShapeState.Shootable;
          break;
        case OmniShape.ShapeState.Shrinking:
          this.m_tick -= Time.deltaTime * 6f;
          bool flag2 = false;
          if ((double) this.m_tick <= 0.0)
          {
            this.m_tick = 0.0f;
            flag2 = true;
          }
          this.transform.localScale = Vector3.Lerp(Vector3.one * 0.02f, Vector3.one, this.m_tick);
          if (!flag2)
            break;
          Object.Destroy((Object) this.Renderer.material);
          Object.Destroy((Object) this.gameObject);
          break;
      }
    }

    public void TurnOff() => this.m_state = OmniShape.ShapeState.Shrinking;

    public enum ShapeState
    {
      Growing,
      Shootable,
      Shrinking,
    }
  }
}
