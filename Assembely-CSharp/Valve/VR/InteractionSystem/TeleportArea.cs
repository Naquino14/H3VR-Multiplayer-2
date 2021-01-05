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

		public Bounds meshBounds
		{
			get;
			private set;
		}

		public void Awake()
		{
			areaMesh = GetComponent<MeshRenderer>();
			tintColorId = Shader.PropertyToID("_TintColor");
			CalculateBounds();
		}

		public void Start()
		{
			visibleTintColor = Teleport.instance.areaVisibleMaterial.GetColor(tintColorId);
			highlightedTintColor = Teleport.instance.areaHighlightedMaterial.GetColor(tintColorId);
			lockedTintColor = Teleport.instance.areaLockedMaterial.GetColor(tintColorId);
		}

		public override bool ShouldActivate(Vector3 playerPosition)
		{
			return true;
		}

		public override bool ShouldMovePlayer()
		{
			return true;
		}

		public override void Highlight(bool highlight)
		{
			if (!locked)
			{
				highlighted = highlight;
				if (highlight)
				{
					areaMesh.material = Teleport.instance.areaHighlightedMaterial;
				}
				else
				{
					areaMesh.material = Teleport.instance.areaVisibleMaterial;
				}
			}
		}

		public override void SetAlpha(float tintAlpha, float alphaPercent)
		{
			Color tintColor = GetTintColor();
			tintColor.a *= alphaPercent;
			areaMesh.material.SetColor(tintColorId, tintColor);
		}

		public override void UpdateVisuals()
		{
			if (locked)
			{
				areaMesh.material = Teleport.instance.areaLockedMaterial;
			}
			else
			{
				areaMesh.material = Teleport.instance.areaVisibleMaterial;
			}
		}

		public void UpdateVisualsInEditor()
		{
			if (!(Teleport.instance == null))
			{
				areaMesh = GetComponent<MeshRenderer>();
				if (locked)
				{
					areaMesh.sharedMaterial = Teleport.instance.areaLockedMaterial;
				}
				else
				{
					areaMesh.sharedMaterial = Teleport.instance.areaVisibleMaterial;
				}
			}
		}

		private bool CalculateBounds()
		{
			MeshFilter component = GetComponent<MeshFilter>();
			if (component == null)
			{
				return false;
			}
			Mesh sharedMesh = component.sharedMesh;
			if (sharedMesh == null)
			{
				return false;
			}
			meshBounds = sharedMesh.bounds;
			return true;
		}

		private Color GetTintColor()
		{
			if (locked)
			{
				return lockedTintColor;
			}
			if (highlighted)
			{
				return highlightedTintColor;
			}
			return visibleTintColor;
		}
	}
}
