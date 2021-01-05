using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem
{
	public class TeleportPoint : TeleportMarkerBase
	{
		public enum TeleportPointType
		{
			MoveToLocation,
			SwitchToNewScene
		}

		public TeleportPointType teleportType;

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
			GetRelevantComponents();
			animation = GetComponent<Animation>();
			tintColorID = Shader.PropertyToID("_TintColor");
			moveLocationIcon.gameObject.SetActive(value: false);
			switchSceneIcon.gameObject.SetActive(value: false);
			lockedIcon.gameObject.SetActive(value: false);
			UpdateVisuals();
		}

		private void Start()
		{
			player = Player.instance;
		}

		private void Update()
		{
			if (Application.isPlaying)
			{
				lookAtPosition.x = player.hmdTransform.position.x;
				lookAtPosition.y = lookAtJointTransform.position.y;
				lookAtPosition.z = player.hmdTransform.position.z;
				lookAtJointTransform.LookAt(lookAtPosition);
			}
		}

		public override bool ShouldActivate(Vector3 playerPosition)
		{
			return Vector3.Distance(base.transform.position, playerPosition) > 1f;
		}

		public override bool ShouldMovePlayer()
		{
			return true;
		}

		public override void Highlight(bool highlight)
		{
			if (!locked)
			{
				if (highlight)
				{
					SetMeshMaterials(Teleport.instance.pointHighlightedMaterial, titleHighlightedColor);
				}
				else
				{
					SetMeshMaterials(Teleport.instance.pointVisibleMaterial, titleVisibleColor);
				}
			}
			if (highlight)
			{
				pointIcon.gameObject.SetActive(value: true);
				animation.Play();
			}
			else
			{
				pointIcon.gameObject.SetActive(value: false);
				animation.Stop();
			}
		}

		public override void UpdateVisuals()
		{
			if (!gotReleventComponents)
			{
				return;
			}
			if (locked)
			{
				SetMeshMaterials(Teleport.instance.pointLockedMaterial, titleLockedColor);
				pointIcon = lockedIcon;
				animation.clip = animation.GetClip("locked_idle");
			}
			else
			{
				SetMeshMaterials(Teleport.instance.pointVisibleMaterial, titleVisibleColor);
				switch (teleportType)
				{
				case TeleportPointType.MoveToLocation:
					pointIcon = moveLocationIcon;
					animation.clip = animation.GetClip("move_location_idle");
					break;
				case TeleportPointType.SwitchToNewScene:
					pointIcon = switchSceneIcon;
					animation.clip = animation.GetClip("switch_scenes_idle");
					break;
				}
			}
			titleText.text = title;
		}

		public override void SetAlpha(float tintAlpha, float alphaPercent)
		{
			tintColor = markerMesh.material.GetColor(tintColorID);
			tintColor.a = tintAlpha;
			markerMesh.material.SetColor(tintColorID, tintColor);
			switchSceneIcon.material.SetColor(tintColorID, tintColor);
			moveLocationIcon.material.SetColor(tintColorID, tintColor);
			lockedIcon.material.SetColor(tintColorID, tintColor);
			titleColor.a = fullTitleAlpha * alphaPercent;
			titleText.color = titleColor;
		}

		public void SetMeshMaterials(Material material, Color textColor)
		{
			markerMesh.material = material;
			switchSceneIcon.material = material;
			moveLocationIcon.material = material;
			lockedIcon.material = material;
			titleColor = textColor;
			fullTitleAlpha = textColor.a;
			titleText.color = titleColor;
		}

		public void TeleportToScene()
		{
			if (!string.IsNullOrEmpty(switchToScene))
			{
				Debug.Log("<b>[SteamVR Interaction]</b> TeleportPoint: Hook up your level loading logic to switch to new scene: " + switchToScene);
			}
			else
			{
				Debug.LogError("<b>[SteamVR Interaction]</b> TeleportPoint: Invalid scene name to switch to: " + switchToScene);
			}
		}

		public void GetRelevantComponents()
		{
			markerMesh = base.transform.Find("teleport_marker_mesh").GetComponent<MeshRenderer>();
			switchSceneIcon = base.transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/switch_scenes_icon").GetComponent<MeshRenderer>();
			moveLocationIcon = base.transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/move_location_icon").GetComponent<MeshRenderer>();
			lockedIcon = base.transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/locked_icon").GetComponent<MeshRenderer>();
			lookAtJointTransform = base.transform.Find("teleport_marker_lookat_joint");
			titleText = base.transform.Find("teleport_marker_lookat_joint/teleport_marker_canvas/teleport_marker_canvas_text").GetComponent<Text>();
			gotReleventComponents = true;
		}

		public void ReleaseRelevantComponents()
		{
			markerMesh = null;
			switchSceneIcon = null;
			moveLocationIcon = null;
			lockedIcon = null;
			lookAtJointTransform = null;
			titleText = null;
		}

		public void UpdateVisualsInEditor()
		{
			if (Application.isPlaying)
			{
				return;
			}
			GetRelevantComponents();
			if (locked)
			{
				lockedIcon.gameObject.SetActive(value: true);
				moveLocationIcon.gameObject.SetActive(value: false);
				switchSceneIcon.gameObject.SetActive(value: false);
				markerMesh.sharedMaterial = Teleport.instance.pointLockedMaterial;
				lockedIcon.sharedMaterial = Teleport.instance.pointLockedMaterial;
				titleText.color = titleLockedColor;
			}
			else
			{
				lockedIcon.gameObject.SetActive(value: false);
				markerMesh.sharedMaterial = Teleport.instance.pointVisibleMaterial;
				switchSceneIcon.sharedMaterial = Teleport.instance.pointVisibleMaterial;
				moveLocationIcon.sharedMaterial = Teleport.instance.pointVisibleMaterial;
				titleText.color = titleVisibleColor;
				switch (teleportType)
				{
				case TeleportPointType.MoveToLocation:
					moveLocationIcon.gameObject.SetActive(value: true);
					switchSceneIcon.gameObject.SetActive(value: false);
					break;
				case TeleportPointType.SwitchToNewScene:
					moveLocationIcon.gameObject.SetActive(value: false);
					switchSceneIcon.gameObject.SetActive(value: true);
					break;
				}
			}
			titleText.text = title;
			ReleaseRelevantComponents();
		}
	}
}
