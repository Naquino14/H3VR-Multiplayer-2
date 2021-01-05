// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.TeleportArea
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class TeleportArea : TeleportMarkerBase
  {
    private MeshRenderer areaMesh;
    private int tintColorId;
    private Color visibleTintColor = Color.clear;
    private Color highlightedTintColor = Color.clear;
    private Color lockedTintColor = Color.clear;
    private bool highlighted;

    public Bounds meshBounds { get; private set; }

    public void Awake()
    {
      this.areaMesh = this.GetComponent<MeshRenderer>();
      this.tintColorId = Shader.PropertyToID("_TintColor");
      this.CalculateBounds();
    }

    public void Start()
    {
      this.visibleTintColor = Teleport.instance.areaVisibleMaterial.GetColor(this.tintColorId);
      this.highlightedTintColor = Teleport.instance.areaHighlightedMaterial.GetColor(this.tintColorId);
      this.lockedTintColor = Teleport.instance.areaLockedMaterial.GetColor(this.tintColorId);
    }

    public override bool ShouldActivate(Vector3 playerPosition) => true;

    public override bool ShouldMovePlayer() => true;

    public override void Highlight(bool highlight)
    {
      if (this.locked)
        return;
      this.highlighted = highlight;
      if (highlight)
        this.areaMesh.material = Teleport.instance.areaHighlightedMaterial;
      else
        this.areaMesh.material = Teleport.instance.areaVisibleMaterial;
    }

    public override void SetAlpha(float tintAlpha, float alphaPercent)
    {
      Color tintColor = this.GetTintColor();
      tintColor.a *= alphaPercent;
      this.areaMesh.material.SetColor(this.tintColorId, tintColor);
    }

    public override void UpdateVisuals()
    {
      if (this.locked)
        this.areaMesh.material = Teleport.instance.areaLockedMaterial;
      else
        this.areaMesh.material = Teleport.instance.areaVisibleMaterial;
    }

    public void UpdateVisualsInEditor()
    {
      if ((Object) Teleport.instance == (Object) null)
        return;
      this.areaMesh = this.GetComponent<MeshRenderer>();
      if (this.locked)
        this.areaMesh.sharedMaterial = Teleport.instance.areaLockedMaterial;
      else
        this.areaMesh.sharedMaterial = Teleport.instance.areaVisibleMaterial;
    }

    private bool CalculateBounds()
    {
      MeshFilter component = this.GetComponent<MeshFilter>();
      if ((Object) component == (Object) null)
        return false;
      Mesh sharedMesh = component.sharedMesh;
      if ((Object) sharedMesh == (Object) null)
        return false;
      this.meshBounds = sharedMesh.bounds;
      return true;
    }

    private Color GetTintColor()
    {
      if (this.locked)
        return this.lockedTintColor;
      return this.highlighted ? this.highlightedTintColor : this.visibleTintColor;
    }
  }
}
