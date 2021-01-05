// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.TeleportPoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem
{
  public class TeleportPoint : TeleportMarkerBase
  {
    public TeleportPoint.TeleportPointType teleportType;
    public string title;
    public string switchToScene;
    public Color titleVisibleColor;
    public Color titleHighlightedColor;
    public Color titleLockedColor;
    public bool playerSpawnPoint;
    private bool gotReleventComponents;
    private MeshRenderer markerMesh;
    private MeshRenderer switchSceneIcon;
    private MeshRenderer moveLocationIcon;
    private MeshRenderer lockedIcon;
    private MeshRenderer pointIcon;
    private Transform lookAtJointTransform;
    private Animation animation;
    private Text titleText;
    private Player player;
    private Vector3 lookAtPosition = Vector3.zero;
    private int tintColorID;
    private Color tintColor = Color.clear;
    private Color titleColor = Color.clear;
    private float fullTitleAlpha;
    private const string switchSceneAnimation = "switch_scenes_idle";
    private const string moveLocationAnimation = "move_location_idle";
    private const string lockedAnimation = "locked_idle";

    public override bool showReticle => false;

    private void Awake()
    {
      this.GetRelevantComponents();
      this.animation = this.GetComponent<Animation>();
      this.tintColorID = Shader.PropertyToID("_TintColor");
      this.moveLocationIcon.gameObject.SetActive(false);
      this.switchSceneIcon.gameObject.SetActive(false);
      this.lockedIcon.gameObject.SetActive(false);
      this.UpdateVisuals();
    }

    private void Start() => this.player = Player.instance;

    private void Update()
    {
      if (!Application.isPlaying)
        return;
      this.lookAtPosition.x = this.player.hmdTransform.position.x;
      this.lookAtPosition.y = this.lookAtJointTransform.position.y;
      this.lookAtPosition.z = this.player.hmdTransform.position.z;
      this.lookAtJointTransform.LookAt(this.lookAtPosition);
    }

    public override bool ShouldActivate(Vector3 playerPosition) => (double) Vector3.Distance(this.transform.position, playerPosition) > 1.0;

    public override bool ShouldMovePlayer() => true;

    public override void Highlight(bool highlight)
    {
      if (!this.locked)
      {
        if (highlight)
          this.SetMeshMaterials(Teleport.instance.pointHighlightedMaterial, this.titleHighlightedColor);
        else
          this.SetMeshMaterials(Teleport.instance.pointVisibleMaterial, this.titleVisibleColor);
      }
      if (highlight)
      {
        this.pointIcon.gameObject.SetActive(true);
        this.animation.Play();
      }
      else
      {
        this.pointIcon.gameObject.SetActive(false);
        this.animation.Stop();
      }
    }

    public override void UpdateVisuals()
    {
      if (!this.gotReleventComponents)
        return;
      if (this.locked)
      {
        this.SetMeshMaterials(Teleport.instance.pointLockedMaterial, this.titleLockedColor);
        this.pointIcon = this.lockedIcon;
        this.animation.clip = this.animation.GetClip("locked_idle");
      }
      else
      {
        this.SetMeshMaterials(Teleport.instance.pointVisibleMaterial, this.titleVisibleColor);
        switch (this.teleportType)
        {
          case TeleportPoint.TeleportPointType.MoveToLocation:
            this.pointIcon = this.moveLocationIcon;
            this.animation.clip = this.animation.GetClip("move_location_idle");
            break;
          case TeleportPoint.TeleportPointType.SwitchToNewScene:
            this.pointIcon = this.switchSceneIcon;
            this.animation.clip = this.animation.GetClip("switch_scenes_idle");
            break;
        }
      }
      this.titleText.text = this.title;
    }

    public override void SetAlpha(float tintAlpha, float alphaPercent)
    {
      this.tintColor = this.markerMesh.material.GetColor(this.tintColorID);
      this.tintColor.a = tintAlpha;
      this.markerMesh.material.SetColor(this.tintColorID, this.tintColor);
      this.switchSceneIcon.material.SetColor(this.tintColorID, this.tintColor);
      this.moveLocationIcon.material.SetColor(this.tintColorID, this.tintColor);
      this.lockedIcon.material.SetColor(this.tintColorID, this.tintColor);
      this.titleColor.a = this.fullTitleAlpha * alphaPercent;
      this.titleText.color = this.titleColor;
    }

    public void SetMeshMaterials(Material material, Color textColor)
    {
      this.markerMesh.material = material;
      this.switchSceneIcon.material = material;
      this.moveLocationIcon.material = material;
      this.lockedIcon.material = material;
      this.titleColor = textColor;
      this.fullTitleAlpha = textColor.a;
      this.titleText.color = this.titleColor;
    }

    public void TeleportToScene()
    {
      if (!string.IsNullOrEmpty(this.switchToScene))
        Debug.Log((object) ("<b>[SteamVR Interaction]</b> TeleportPoint: Hook up your level loading logic to switch to new scene: " + this.switchToScene));
      else
        Debug.LogError((object) ("<b>[SteamVR Interaction]</b> TeleportPoint: Invalid scene name to switch to: " + this.switchToScene));
    }

    public void GetRelevantComponents()
    {
      this.markerMesh = this.transform.Find("teleport_marker_mesh").GetComponent<MeshRenderer>();
      this.switchSceneIcon = this.transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/switch_scenes_icon").GetComponent<MeshRenderer>();
      this.moveLocationIcon = this.transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/move_location_icon").GetComponent<MeshRenderer>();
      this.lockedIcon = this.transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/locked_icon").GetComponent<MeshRenderer>();
      this.lookAtJointTransform = this.transform.Find("teleport_marker_lookat_joint");
      this.titleText = this.transform.Find("teleport_marker_lookat_joint/teleport_marker_canvas/teleport_marker_canvas_text").GetComponent<Text>();
      this.gotReleventComponents = true;
    }

    public void ReleaseRelevantComponents()
    {
      this.markerMesh = (MeshRenderer) null;
      this.switchSceneIcon = (MeshRenderer) null;
      this.moveLocationIcon = (MeshRenderer) null;
      this.lockedIcon = (MeshRenderer) null;
      this.lookAtJointTransform = (Transform) null;
      this.titleText = (Text) null;
    }

    public void UpdateVisualsInEditor()
    {
      if (Application.isPlaying)
        return;
      this.GetRelevantComponents();
      if (this.locked)
      {
        this.lockedIcon.gameObject.SetActive(true);
        this.moveLocationIcon.gameObject.SetActive(false);
        this.switchSceneIcon.gameObject.SetActive(false);
        this.markerMesh.sharedMaterial = Teleport.instance.pointLockedMaterial;
        this.lockedIcon.sharedMaterial = Teleport.instance.pointLockedMaterial;
        this.titleText.color = this.titleLockedColor;
      }
      else
      {
        this.lockedIcon.gameObject.SetActive(false);
        this.markerMesh.sharedMaterial = Teleport.instance.pointVisibleMaterial;
        this.switchSceneIcon.sharedMaterial = Teleport.instance.pointVisibleMaterial;
        this.moveLocationIcon.sharedMaterial = Teleport.instance.pointVisibleMaterial;
        this.titleText.color = this.titleVisibleColor;
        switch (this.teleportType)
        {
          case TeleportPoint.TeleportPointType.MoveToLocation:
            this.moveLocationIcon.gameObject.SetActive(true);
            this.switchSceneIcon.gameObject.SetActive(false);
            break;
          case TeleportPoint.TeleportPointType.SwitchToNewScene:
            this.moveLocationIcon.gameObject.SetActive(false);
            this.switchSceneIcon.gameObject.SetActive(true);
            break;
        }
      }
      this.titleText.text = this.title;
      this.ReleaseRelevantComponents();
    }

    public enum TeleportPointType
    {
      MoveToLocation,
      SwitchToNewScene,
    }
  }
}
