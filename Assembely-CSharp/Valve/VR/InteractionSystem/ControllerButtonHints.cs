using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem
{
	public class ControllerButtonHints : MonoBehaviour
	{
		private enum OffsetType
		{
			Up,
			Right,
			Forward,
			Back
		}

		private class ActionHintInfo
		{
			public string componentName;

			public List<MeshRenderer> renderers;

			public Transform localTransform;

			public GameObject textHintObject;

			public Transform textStartAnchor;

			public Transform textEndAnchor;

			public Vector3 textEndOffsetDir;

			public Transform canvasOffset;

			public Text text;

			public TextMesh textMesh;

			public Canvas textCanvas;

			public LineRenderer line;

			public float distanceFromCenter;

			public bool textHintActive;
		}

		public Material controllerMaterial;

		public Color flashColor = new Color(1f, 0.557f, 0f);

		public GameObject textHintPrefab;

		public SteamVR_Action_Vibration hapticFlash = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");

		[Header("Debug")]
		public bool debugHints;

		private SteamVR_RenderModel renderModel;

		private Player player;

		private List<MeshRenderer> renderers = new List<MeshRenderer>();

		private List<MeshRenderer> flashingRenderers = new List<MeshRenderer>();

		private float startTime;

		private float tickCount;

		private Dictionary<ISteamVR_Action_In_Source, ActionHintInfo> actionHintInfos;

		private Transform textHintParent;

		private int colorID;

		private Vector3 centerPosition = Vector3.zero;

		private SteamVR_Events.Action renderModelLoadedAction;

		protected SteamVR_Input_Sources inputSource;

		private Dictionary<string, Transform> componentTransformMap = new Dictionary<string, Transform>();

		public bool initialized
		{
			get;
			private set;
		}

		private void Awake()
		{
			renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction(OnRenderModelLoaded);
			colorID = Shader.PropertyToID("_Color");
		}

		private void Start()
		{
			player = Player.instance;
		}

		private void HintDebugLog(string msg)
		{
			if (debugHints)
			{
				Debug.Log("<b>[SteamVR Interaction]</b> Hints: " + msg);
			}
		}

		private void OnEnable()
		{
			renderModelLoadedAction.enabled = true;
		}

		private void OnDisable()
		{
			renderModelLoadedAction.enabled = false;
			Clear();
		}

		private void OnParentHandInputFocusLost()
		{
			HideAllButtonHints();
			HideAllText();
		}

		public virtual void SetInputSource(SteamVR_Input_Sources newInputSource)
		{
			inputSource = newInputSource;
			if (renderModel != null)
			{
				renderModel.SetInputSource(newInputSource);
			}
		}

		private void OnHandInitialized(int deviceIndex)
		{
			renderModel = new GameObject("SteamVR_RenderModel").AddComponent<SteamVR_RenderModel>();
			renderModel.transform.parent = base.transform;
			renderModel.transform.localPosition = Vector3.zero;
			renderModel.transform.localRotation = Quaternion.identity;
			renderModel.transform.localScale = Vector3.one;
			renderModel.SetInputSource(inputSource);
			renderModel.SetDeviceIndex(deviceIndex);
			if (!initialized)
			{
				renderModel.gameObject.SetActive(value: true);
			}
		}

		private void OnRenderModelLoaded(SteamVR_RenderModel renderModel, bool succeess)
		{
			if (renderModel == this.renderModel)
			{
				if (initialized)
				{
					UnityEngine.Object.Destroy(textHintParent.gameObject);
					componentTransformMap.Clear();
					flashingRenderers.Clear();
				}
				renderModel.SetMeshRendererState(state: false);
				StartCoroutine(DoInitialize(renderModel));
			}
		}

		private IEnumerator DoInitialize(SteamVR_RenderModel renderModel)
		{
			while (!renderModel.initializedAttachPoints)
			{
				yield return null;
			}
			textHintParent = new GameObject("Text Hints").transform;
			textHintParent.SetParent(base.transform);
			textHintParent.localPosition = Vector3.zero;
			textHintParent.localRotation = Quaternion.identity;
			textHintParent.localScale = Vector3.one;
			CVRRenderModels renderModels = OpenVR.RenderModels;
			if (renderModels != null)
			{
				string text = string.Empty;
				if (debugHints)
				{
					text = "Components for render model " + renderModel.index;
				}
				for (int i = 0; i < renderModel.transform.childCount; i++)
				{
					Transform child = renderModel.transform.GetChild(i);
					if (componentTransformMap.ContainsKey(child.name))
					{
						if (debugHints)
						{
							text = text + "\n\t!    Child component already exists with name: " + child.name;
						}
					}
					else
					{
						componentTransformMap.Add(child.name, child);
					}
					if (debugHints)
					{
						text = text + "\n\t" + child.name + ".";
					}
				}
				HintDebugLog(text);
			}
			actionHintInfos = new Dictionary<ISteamVR_Action_In_Source, ActionHintInfo>();
			for (int j = 0; j < SteamVR_Input.actionsNonPoseNonSkeletonIn.Length; j++)
			{
				ISteamVR_Action_In steamVR_Action_In = SteamVR_Input.actionsNonPoseNonSkeletonIn[j];
				if (steamVR_Action_In.GetActive(inputSource))
				{
					CreateAndAddButtonInfo(steamVR_Action_In, inputSource);
				}
			}
			ComputeTextEndTransforms();
			initialized = true;
			renderModel.SetMeshRendererState(state: true);
			renderModel.gameObject.SetActive(value: false);
		}

		private void CreateAndAddButtonInfo(ISteamVR_Action_In action, SteamVR_Input_Sources inputSource)
		{
			Transform transform = null;
			List<MeshRenderer> list = new List<MeshRenderer>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Looking for action: ");
			stringBuilder.AppendLine(action.GetShortName());
			stringBuilder.Append("Action localized origin: ");
			stringBuilder.AppendLine(action.GetLocalizedOrigin(inputSource));
			string renderModelComponentName = action.GetRenderModelComponentName(inputSource);
			if (componentTransformMap.ContainsKey(renderModelComponentName))
			{
				stringBuilder.AppendLine($"Found component: {renderModelComponentName} for {action.GetShortName()}");
				Transform transform2 = componentTransformMap[renderModelComponentName];
				transform = transform2;
				stringBuilder.AppendLine($"Found componentTransform: {transform2}. buttonTransform: {transform}");
				list.AddRange(transform2.GetComponentsInChildren<MeshRenderer>());
			}
			else
			{
				stringBuilder.AppendLine($"Can't find component transform for action: {action.GetShortName()}. Component name: \"{renderModelComponentName}\"");
			}
			stringBuilder.AppendLine($"Found {list.Count} renderers for {action.GetShortName()}");
			foreach (MeshRenderer item in list)
			{
				stringBuilder.Append("\t");
				stringBuilder.AppendLine(item.name);
			}
			HintDebugLog(stringBuilder.ToString());
			if (transform == null)
			{
				HintDebugLog("Couldn't find buttonTransform for " + action.GetShortName());
				return;
			}
			ActionHintInfo actionHintInfo = new ActionHintInfo();
			actionHintInfos.Add(action, actionHintInfo);
			actionHintInfo.componentName = transform.name;
			actionHintInfo.renderers = list;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name == "attach")
				{
					actionHintInfo.localTransform = child;
				}
			}
			switch (1)
			{
			case 2:
				actionHintInfo.textEndOffsetDir = actionHintInfo.localTransform.forward;
				break;
			case 3:
				actionHintInfo.textEndOffsetDir = -actionHintInfo.localTransform.forward;
				break;
			case 1:
				actionHintInfo.textEndOffsetDir = actionHintInfo.localTransform.right;
				break;
			case 0:
				actionHintInfo.textEndOffsetDir = actionHintInfo.localTransform.up;
				break;
			}
			Vector3 position = actionHintInfo.localTransform.position + actionHintInfo.localTransform.forward * 0.01f;
			actionHintInfo.textHintObject = UnityEngine.Object.Instantiate(textHintPrefab, position, Quaternion.identity);
			actionHintInfo.textHintObject.name = "Hint_" + actionHintInfo.componentName + "_Start";
			actionHintInfo.textHintObject.transform.SetParent(textHintParent);
			actionHintInfo.textHintObject.layer = base.gameObject.layer;
			actionHintInfo.textHintObject.tag = base.gameObject.tag;
			actionHintInfo.textStartAnchor = actionHintInfo.textHintObject.transform.Find("Start");
			actionHintInfo.textEndAnchor = actionHintInfo.textHintObject.transform.Find("End");
			actionHintInfo.canvasOffset = actionHintInfo.textHintObject.transform.Find("CanvasOffset");
			actionHintInfo.line = actionHintInfo.textHintObject.transform.Find("Line").GetComponent<LineRenderer>();
			actionHintInfo.textCanvas = actionHintInfo.textHintObject.GetComponentInChildren<Canvas>();
			actionHintInfo.text = actionHintInfo.textCanvas.GetComponentInChildren<Text>();
			actionHintInfo.textMesh = actionHintInfo.textCanvas.GetComponentInChildren<TextMesh>();
			actionHintInfo.textHintObject.SetActive(value: false);
			actionHintInfo.textStartAnchor.position = position;
			if (actionHintInfo.text != null)
			{
				actionHintInfo.text.text = actionHintInfo.componentName;
			}
			if (actionHintInfo.textMesh != null)
			{
				actionHintInfo.textMesh.text = actionHintInfo.componentName;
			}
			centerPosition += actionHintInfo.textStartAnchor.position;
			actionHintInfo.textCanvas.transform.localScale = Vector3.Scale(actionHintInfo.textCanvas.transform.localScale, player.transform.localScale);
			actionHintInfo.textStartAnchor.transform.localScale = Vector3.Scale(actionHintInfo.textStartAnchor.transform.localScale, player.transform.localScale);
			actionHintInfo.textEndAnchor.transform.localScale = Vector3.Scale(actionHintInfo.textEndAnchor.transform.localScale, player.transform.localScale);
			actionHintInfo.line.transform.localScale = Vector3.Scale(actionHintInfo.line.transform.localScale, player.transform.localScale);
		}

		private void ComputeTextEndTransforms()
		{
			centerPosition /= (float)actionHintInfos.Count;
			float num = 0f;
			foreach (KeyValuePair<ISteamVR_Action_In_Source, ActionHintInfo> actionHintInfo in actionHintInfos)
			{
				actionHintInfo.Value.distanceFromCenter = Vector3.Distance(actionHintInfo.Value.textStartAnchor.position, centerPosition);
				if (actionHintInfo.Value.distanceFromCenter > num)
				{
					num = actionHintInfo.Value.distanceFromCenter;
				}
			}
			foreach (KeyValuePair<ISteamVR_Action_In_Source, ActionHintInfo> actionHintInfo2 in actionHintInfos)
			{
				Vector3 vector = actionHintInfo2.Value.textStartAnchor.position - centerPosition;
				vector.Normalize();
				vector = Vector3.Project(vector, renderModel.transform.forward);
				float num2 = actionHintInfo2.Value.distanceFromCenter / num;
				float num3 = actionHintInfo2.Value.distanceFromCenter * Mathf.Pow(2f, 10f * (num2 - 1f)) * 20f;
				float num4 = 0.1f;
				Vector3 vector2 = actionHintInfo2.Value.textStartAnchor.position + actionHintInfo2.Value.textEndOffsetDir * num4 + vector * num3 * 0.1f;
				if (SteamVR_Utils.IsValid(vector2))
				{
					actionHintInfo2.Value.textEndAnchor.position = vector2;
					actionHintInfo2.Value.canvasOffset.position = vector2;
				}
				else
				{
					Debug.LogWarning("<b>[SteamVR Interaction]</b> Invalid end position for: " + actionHintInfo2.Value.textStartAnchor.name, actionHintInfo2.Value.textStartAnchor.gameObject);
				}
				actionHintInfo2.Value.canvasOffset.localRotation = Quaternion.identity;
			}
		}

		private void ShowButtonHint(params ISteamVR_Action_In_Source[] actions)
		{
			renderModel.gameObject.SetActive(value: true);
			renderModel.GetComponentsInChildren(renderers);
			for (int i = 0; i < renderers.Count; i++)
			{
				Texture mainTexture = renderers[i].material.mainTexture;
				renderers[i].sharedMaterial = controllerMaterial;
				renderers[i].material.mainTexture = mainTexture;
				renderers[i].material.renderQueue = controllerMaterial.shader.renderQueue;
			}
			for (int j = 0; j < actions.Length; j++)
			{
				if (!actionHintInfos.ContainsKey(actions[j]))
				{
					continue;
				}
				ActionHintInfo actionHintInfo = actionHintInfos[actions[j]];
				foreach (MeshRenderer renderer in actionHintInfo.renderers)
				{
					if (!flashingRenderers.Contains(renderer))
					{
						flashingRenderers.Add(renderer);
					}
				}
			}
			startTime = Time.realtimeSinceStartup;
			tickCount = 0f;
		}

		private void HideAllButtonHints()
		{
			Clear();
			if (renderModel != null && renderModel.gameObject != null)
			{
				renderModel.gameObject.SetActive(value: false);
			}
		}

		private void HideButtonHint(params ISteamVR_Action_In_Source[] actions)
		{
			Color color = controllerMaterial.GetColor(colorID);
			for (int i = 0; i < actions.Length; i++)
			{
				if (!actionHintInfos.ContainsKey(actions[i]))
				{
					continue;
				}
				ActionHintInfo actionHintInfo = actionHintInfos[actions[i]];
				foreach (MeshRenderer renderer in actionHintInfo.renderers)
				{
					renderer.material.color = color;
					flashingRenderers.Remove(renderer);
				}
			}
			if (flashingRenderers.Count == 0)
			{
				renderModel.gameObject.SetActive(value: false);
			}
		}

		private bool IsButtonHintActive(ISteamVR_Action_In_Source action)
		{
			if (actionHintInfos.ContainsKey(action))
			{
				ActionHintInfo actionHintInfo = actionHintInfos[action];
				foreach (MeshRenderer renderer in actionHintInfo.renderers)
				{
					if (flashingRenderers.Contains(renderer))
					{
						return true;
					}
				}
			}
			return false;
		}

		private IEnumerator TestButtonHints()
		{
			while (true)
			{
				for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsNonPoseNonSkeletonIn.Length; actionIndex++)
				{
					ISteamVR_Action_In action = SteamVR_Input.actionsNonPoseNonSkeletonIn[actionIndex];
					if (action.GetActive(inputSource))
					{
						ShowButtonHint(action);
						yield return new WaitForSeconds(1f);
					}
					yield return null;
				}
			}
		}

		private IEnumerator TestTextHints()
		{
			while (true)
			{
				for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsNonPoseNonSkeletonIn.Length; actionIndex++)
				{
					ISteamVR_Action_In action = SteamVR_Input.actionsNonPoseNonSkeletonIn[actionIndex];
					if (action.GetActive(inputSource))
					{
						ShowText(action, action.GetShortName());
						yield return new WaitForSeconds(3f);
					}
					yield return null;
				}
				HideAllText();
				yield return new WaitForSeconds(3f);
			}
		}

		private void Update()
		{
			if (!(renderModel != null) || !renderModel.gameObject.activeInHierarchy || flashingRenderers.Count <= 0)
			{
				return;
			}
			Color color = controllerMaterial.GetColor(colorID);
			float f = (Time.realtimeSinceStartup - startTime) * (float)Math.PI * 2f;
			f = Mathf.Cos(f);
			f = Util.RemapNumberClamped(f, -1f, 1f, 0f, 1f);
			float num = Time.realtimeSinceStartup - startTime;
			if (num - tickCount > 1f)
			{
				tickCount += 1f;
				hapticFlash.Execute(0f, 0.005f, 0.005f, 1f, inputSource);
			}
			for (int i = 0; i < flashingRenderers.Count; i++)
			{
				Renderer renderer = flashingRenderers[i];
				renderer.material.SetColor(colorID, Color.Lerp(color, flashColor, f));
			}
			if (!initialized)
			{
				return;
			}
			foreach (KeyValuePair<ISteamVR_Action_In_Source, ActionHintInfo> actionHintInfo in actionHintInfos)
			{
				if (actionHintInfo.Value.textHintActive)
				{
					UpdateTextHint(actionHintInfo.Value);
				}
			}
		}

		private void UpdateTextHint(ActionHintInfo hintInfo)
		{
			Transform hmdTransform = player.hmdTransform;
			Vector3 forward = hmdTransform.position - hintInfo.canvasOffset.position;
			Quaternion a = Quaternion.LookRotation(forward, Vector3.up);
			Quaternion b = Quaternion.LookRotation(forward, hmdTransform.up);
			float t = ((!(hmdTransform.forward.y > 0f)) ? Util.RemapNumberClamped(hmdTransform.forward.y, -0.8f, -0.6f, 1f, 0f) : Util.RemapNumberClamped(hmdTransform.forward.y, 0.6f, 0.4f, 1f, 0f));
			hintInfo.canvasOffset.rotation = Quaternion.Slerp(a, b, t);
			Transform transform = hintInfo.line.transform;
			hintInfo.line.useWorldSpace = false;
			hintInfo.line.SetPosition(0, transform.InverseTransformPoint(hintInfo.textStartAnchor.position));
			hintInfo.line.SetPosition(1, transform.InverseTransformPoint(hintInfo.textEndAnchor.position));
		}

		private void Clear()
		{
			renderers.Clear();
			flashingRenderers.Clear();
		}

		private void ShowText(ISteamVR_Action_In_Source action, string text, bool highlightButton = true)
		{
			if (actionHintInfos.ContainsKey(action))
			{
				ActionHintInfo actionHintInfo = actionHintInfos[action];
				actionHintInfo.textHintObject.SetActive(value: true);
				actionHintInfo.textHintActive = true;
				if (actionHintInfo.text != null)
				{
					actionHintInfo.text.text = text;
				}
				if (actionHintInfo.textMesh != null)
				{
					actionHintInfo.textMesh.text = text;
				}
				UpdateTextHint(actionHintInfo);
				if (highlightButton)
				{
					ShowButtonHint(action);
				}
				renderModel.gameObject.SetActive(value: true);
			}
		}

		private void HideText(ISteamVR_Action_In_Source action)
		{
			if (actionHintInfos.ContainsKey(action))
			{
				ActionHintInfo actionHintInfo = actionHintInfos[action];
				actionHintInfo.textHintObject.SetActive(value: false);
				actionHintInfo.textHintActive = false;
				HideButtonHint(action);
			}
		}

		private void HideAllText()
		{
			if (actionHintInfos == null)
			{
				return;
			}
			foreach (KeyValuePair<ISteamVR_Action_In_Source, ActionHintInfo> actionHintInfo in actionHintInfos)
			{
				actionHintInfo.Value.textHintObject.SetActive(value: false);
				actionHintInfo.Value.textHintActive = false;
			}
			HideAllButtonHints();
		}

		private string GetActiveHintText(ISteamVR_Action_In_Source action)
		{
			if (actionHintInfos.ContainsKey(action))
			{
				ActionHintInfo actionHintInfo = actionHintInfos[action];
				if (actionHintInfo.textHintActive)
				{
					return actionHintInfo.text.text;
				}
			}
			return string.Empty;
		}

		private static ControllerButtonHints GetControllerButtonHints(Hand hand)
		{
			if (hand != null)
			{
				ControllerButtonHints componentInChildren = hand.GetComponentInChildren<ControllerButtonHints>();
				if (componentInChildren != null && componentInChildren.initialized)
				{
					return componentInChildren;
				}
			}
			return null;
		}

		public static void ShowButtonHint(Hand hand, params ISteamVR_Action_In_Source[] actions)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.ShowButtonHint(actions);
			}
		}

		public static void HideButtonHint(Hand hand, params ISteamVR_Action_In_Source[] actions)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideButtonHint(actions);
			}
		}

		public static void HideAllButtonHints(Hand hand)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideAllButtonHints();
			}
		}

		public static bool IsButtonHintActive(Hand hand, ISteamVR_Action_In_Source action)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				return controllerButtonHints.IsButtonHintActive(action);
			}
			return false;
		}

		public static void ShowTextHint(Hand hand, ISteamVR_Action_In_Source action, string text, bool highlightButton = true)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.ShowText(action, text, highlightButton);
			}
		}

		public static void HideTextHint(Hand hand, ISteamVR_Action_In_Source action)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideText(action);
			}
		}

		public static void HideAllTextHints(Hand hand)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideAllText();
			}
		}

		public static string GetActiveHintText(Hand hand, ISteamVR_Action_In_Source action)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				return controllerButtonHints.GetActiveHintText(action);
			}
			return string.Empty;
		}
	}
}
