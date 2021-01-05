// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.ControllerButtonHints
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
  public class ControllerButtonHints : MonoBehaviour
  {
    public Material controllerMaterial;
    public Color flashColor = new Color(1f, 0.557f, 0.0f);
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
    private Dictionary<ISteamVR_Action_In_Source, ControllerButtonHints.ActionHintInfo> actionHintInfos;
    private Transform textHintParent;
    private int colorID;
    private Vector3 centerPosition = Vector3.zero;
    private SteamVR_Events.Action renderModelLoadedAction;
    protected SteamVR_Input_Sources inputSource;
    private Dictionary<string, Transform> componentTransformMap = new Dictionary<string, Transform>();

    public bool initialized { get; private set; }

    private void Awake()
    {
      this.renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction(new UnityAction<SteamVR_RenderModel, bool>(this.OnRenderModelLoaded));
      this.colorID = Shader.PropertyToID("_Color");
    }

    private void Start() => this.player = Player.instance;

    private void HintDebugLog(string msg)
    {
      if (!this.debugHints)
        return;
      UnityEngine.Debug.Log((object) ("<b>[SteamVR Interaction]</b> Hints: " + msg));
    }

    private void OnEnable() => this.renderModelLoadedAction.enabled = true;

    private void OnDisable()
    {
      this.renderModelLoadedAction.enabled = false;
      this.Clear();
    }

    private void OnParentHandInputFocusLost()
    {
      this.HideAllButtonHints();
      this.HideAllText();
    }

    public virtual void SetInputSource(SteamVR_Input_Sources newInputSource)
    {
      this.inputSource = newInputSource;
      if (!((Object) this.renderModel != (Object) null))
        return;
      this.renderModel.SetInputSource(newInputSource);
    }

    private void OnHandInitialized(int deviceIndex)
    {
      this.renderModel = new GameObject("SteamVR_RenderModel").AddComponent<SteamVR_RenderModel>();
      this.renderModel.transform.parent = this.transform;
      this.renderModel.transform.localPosition = Vector3.zero;
      this.renderModel.transform.localRotation = Quaternion.identity;
      this.renderModel.transform.localScale = Vector3.one;
      this.renderModel.SetInputSource(this.inputSource);
      this.renderModel.SetDeviceIndex(deviceIndex);
      if (this.initialized)
        return;
      this.renderModel.gameObject.SetActive(true);
    }

    private void OnRenderModelLoaded(SteamVR_RenderModel renderModel, bool succeess)
    {
      if (!((Object) renderModel == (Object) this.renderModel))
        return;
      if (this.initialized)
      {
        Object.Destroy((Object) this.textHintParent.gameObject);
        this.componentTransformMap.Clear();
        this.flashingRenderers.Clear();
      }
      renderModel.SetMeshRendererState(false);
      this.StartCoroutine(this.DoInitialize(renderModel));
    }

    [DebuggerHidden]
    private IEnumerator DoInitialize(SteamVR_RenderModel renderModel) => (IEnumerator) new ControllerButtonHints.\u003CDoInitialize\u003Ec__Iterator0()
    {
      renderModel = renderModel,
      \u0024this = this
    };

    private void CreateAndAddButtonInfo(
      ISteamVR_Action_In action,
      SteamVR_Input_Sources inputSource)
    {
      Transform transform = (Transform) null;
      List<MeshRenderer> meshRendererList = new List<MeshRenderer>();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Looking for action: ");
      stringBuilder.AppendLine(action.GetShortName());
      stringBuilder.Append("Action localized origin: ");
      stringBuilder.AppendLine(action.GetLocalizedOrigin(inputSource));
      string modelComponentName = action.GetRenderModelComponentName(inputSource);
      if (this.componentTransformMap.ContainsKey(modelComponentName))
      {
        stringBuilder.AppendLine(string.Format("Found component: {0} for {1}", (object) modelComponentName, (object) action.GetShortName()));
        Transform componentTransform = this.componentTransformMap[modelComponentName];
        transform = componentTransform;
        stringBuilder.AppendLine(string.Format("Found componentTransform: {0}. buttonTransform: {1}", (object) componentTransform, (object) transform));
        meshRendererList.AddRange((IEnumerable<MeshRenderer>) componentTransform.GetComponentsInChildren<MeshRenderer>());
      }
      else
        stringBuilder.AppendLine(string.Format("Can't find component transform for action: {0}. Component name: \"{1}\"", (object) action.GetShortName(), (object) modelComponentName));
      stringBuilder.AppendLine(string.Format("Found {0} renderers for {1}", (object) meshRendererList.Count, (object) action.GetShortName()));
      foreach (MeshRenderer meshRenderer in meshRendererList)
      {
        stringBuilder.Append("\t");
        stringBuilder.AppendLine(meshRenderer.name);
      }
      this.HintDebugLog(stringBuilder.ToString());
      if ((Object) transform == (Object) null)
      {
        this.HintDebugLog("Couldn't find buttonTransform for " + action.GetShortName());
      }
      else
      {
        ControllerButtonHints.ActionHintInfo actionHintInfo = new ControllerButtonHints.ActionHintInfo();
        this.actionHintInfos.Add((ISteamVR_Action_In_Source) action, actionHintInfo);
        actionHintInfo.componentName = transform.name;
        actionHintInfo.renderers = meshRendererList;
        for (int index = 0; index < transform.childCount; ++index)
        {
          Transform child = transform.GetChild(index);
          if (child.name == "attach")
            actionHintInfo.localTransform = child;
        }
        switch (1)
        {
          case 0:
            actionHintInfo.textEndOffsetDir = actionHintInfo.localTransform.up;
            break;
          case 1:
            actionHintInfo.textEndOffsetDir = actionHintInfo.localTransform.right;
            break;
          case 2:
            actionHintInfo.textEndOffsetDir = actionHintInfo.localTransform.forward;
            break;
          case 3:
            actionHintInfo.textEndOffsetDir = -actionHintInfo.localTransform.forward;
            break;
        }
        Vector3 position = actionHintInfo.localTransform.position + actionHintInfo.localTransform.forward * 0.01f;
        actionHintInfo.textHintObject = Object.Instantiate<GameObject>(this.textHintPrefab, position, Quaternion.identity);
        actionHintInfo.textHintObject.name = "Hint_" + actionHintInfo.componentName + "_Start";
        actionHintInfo.textHintObject.transform.SetParent(this.textHintParent);
        actionHintInfo.textHintObject.layer = this.gameObject.layer;
        actionHintInfo.textHintObject.tag = this.gameObject.tag;
        actionHintInfo.textStartAnchor = actionHintInfo.textHintObject.transform.Find("Start");
        actionHintInfo.textEndAnchor = actionHintInfo.textHintObject.transform.Find("End");
        actionHintInfo.canvasOffset = actionHintInfo.textHintObject.transform.Find("CanvasOffset");
        actionHintInfo.line = actionHintInfo.textHintObject.transform.Find("Line").GetComponent<LineRenderer>();
        actionHintInfo.textCanvas = actionHintInfo.textHintObject.GetComponentInChildren<Canvas>();
        actionHintInfo.text = actionHintInfo.textCanvas.GetComponentInChildren<UnityEngine.UI.Text>();
        actionHintInfo.textMesh = actionHintInfo.textCanvas.GetComponentInChildren<TextMesh>();
        actionHintInfo.textHintObject.SetActive(false);
        actionHintInfo.textStartAnchor.position = position;
        if ((Object) actionHintInfo.text != (Object) null)
          actionHintInfo.text.text = actionHintInfo.componentName;
        if ((Object) actionHintInfo.textMesh != (Object) null)
          actionHintInfo.textMesh.text = actionHintInfo.componentName;
        this.centerPosition += actionHintInfo.textStartAnchor.position;
        actionHintInfo.textCanvas.transform.localScale = Vector3.Scale(actionHintInfo.textCanvas.transform.localScale, this.player.transform.localScale);
        actionHintInfo.textStartAnchor.transform.localScale = Vector3.Scale(actionHintInfo.textStartAnchor.transform.localScale, this.player.transform.localScale);
        actionHintInfo.textEndAnchor.transform.localScale = Vector3.Scale(actionHintInfo.textEndAnchor.transform.localScale, this.player.transform.localScale);
        actionHintInfo.line.transform.localScale = Vector3.Scale(actionHintInfo.line.transform.localScale, this.player.transform.localScale);
      }
    }

    private void ComputeTextEndTransforms()
    {
      this.centerPosition /= (float) this.actionHintInfos.Count;
      float num1 = 0.0f;
      foreach (KeyValuePair<ISteamVR_Action_In_Source, ControllerButtonHints.ActionHintInfo> actionHintInfo in this.actionHintInfos)
      {
        actionHintInfo.Value.distanceFromCenter = Vector3.Distance(actionHintInfo.Value.textStartAnchor.position, this.centerPosition);
        if ((double) actionHintInfo.Value.distanceFromCenter > (double) num1)
          num1 = actionHintInfo.Value.distanceFromCenter;
      }
      foreach (KeyValuePair<ISteamVR_Action_In_Source, ControllerButtonHints.ActionHintInfo> actionHintInfo in this.actionHintInfos)
      {
        Vector3 vector1 = actionHintInfo.Value.textStartAnchor.position - this.centerPosition;
        vector1.Normalize();
        Vector3 vector3 = Vector3.Project(vector1, this.renderModel.transform.forward);
        float num2 = actionHintInfo.Value.distanceFromCenter / num1;
        float num3 = (float) ((double) actionHintInfo.Value.distanceFromCenter * (double) Mathf.Pow(2f, (float) (10.0 * ((double) num2 - 1.0))) * 20.0);
        float num4 = 0.1f;
        Vector3 vector2 = actionHintInfo.Value.textStartAnchor.position + actionHintInfo.Value.textEndOffsetDir * num4 + vector3 * num3 * 0.1f;
        if (SteamVR_Utils.IsValid(vector2))
        {
          actionHintInfo.Value.textEndAnchor.position = vector2;
          actionHintInfo.Value.canvasOffset.position = vector2;
        }
        else
          UnityEngine.Debug.LogWarning((object) ("<b>[SteamVR Interaction]</b> Invalid end position for: " + actionHintInfo.Value.textStartAnchor.name), (Object) actionHintInfo.Value.textStartAnchor.gameObject);
        actionHintInfo.Value.canvasOffset.localRotation = Quaternion.identity;
      }
    }

    private void ShowButtonHint(params ISteamVR_Action_In_Source[] actions)
    {
      this.renderModel.gameObject.SetActive(true);
      this.renderModel.GetComponentsInChildren<MeshRenderer>(this.renderers);
      for (int index = 0; index < this.renderers.Count; ++index)
      {
        Texture mainTexture = this.renderers[index].material.mainTexture;
        this.renderers[index].sharedMaterial = this.controllerMaterial;
        this.renderers[index].material.mainTexture = mainTexture;
        this.renderers[index].material.renderQueue = this.controllerMaterial.shader.renderQueue;
      }
      for (int index = 0; index < actions.Length; ++index)
      {
        if (this.actionHintInfos.ContainsKey(actions[index]))
        {
          foreach (MeshRenderer renderer in this.actionHintInfos[actions[index]].renderers)
          {
            if (!this.flashingRenderers.Contains(renderer))
              this.flashingRenderers.Add(renderer);
          }
        }
      }
      this.startTime = Time.realtimeSinceStartup;
      this.tickCount = 0.0f;
    }

    private void HideAllButtonHints()
    {
      this.Clear();
      if (!((Object) this.renderModel != (Object) null) || !((Object) this.renderModel.gameObject != (Object) null))
        return;
      this.renderModel.gameObject.SetActive(false);
    }

    private void HideButtonHint(params ISteamVR_Action_In_Source[] actions)
    {
      Color color = this.controllerMaterial.GetColor(this.colorID);
      for (int index = 0; index < actions.Length; ++index)
      {
        if (this.actionHintInfos.ContainsKey(actions[index]))
        {
          foreach (MeshRenderer renderer in this.actionHintInfos[actions[index]].renderers)
          {
            renderer.material.color = color;
            this.flashingRenderers.Remove(renderer);
          }
        }
      }
      if (this.flashingRenderers.Count != 0)
        return;
      this.renderModel.gameObject.SetActive(false);
    }

    private bool IsButtonHintActive(ISteamVR_Action_In_Source action)
    {
      if (this.actionHintInfos.ContainsKey(action))
      {
        foreach (MeshRenderer renderer in this.actionHintInfos[action].renderers)
        {
          if (this.flashingRenderers.Contains(renderer))
            return true;
        }
      }
      return false;
    }

    [DebuggerHidden]
    private IEnumerator TestButtonHints() => (IEnumerator) new ControllerButtonHints.\u003CTestButtonHints\u003Ec__Iterator1()
    {
      \u0024this = this
    };

    [DebuggerHidden]
    private IEnumerator TestTextHints() => (IEnumerator) new ControllerButtonHints.\u003CTestTextHints\u003Ec__Iterator2()
    {
      \u0024this = this
    };

    private void Update()
    {
      if (!((Object) this.renderModel != (Object) null) || !this.renderModel.gameObject.activeInHierarchy || this.flashingRenderers.Count <= 0)
        return;
      Color color = this.controllerMaterial.GetColor(this.colorID);
      float t = Util.RemapNumberClamped(Mathf.Cos((float) (((double) Time.realtimeSinceStartup - (double) this.startTime) * 3.14159274101257 * 2.0)), -1f, 1f, 0.0f, 1f);
      if ((double) (Time.realtimeSinceStartup - this.startTime) - (double) this.tickCount > 1.0)
      {
        ++this.tickCount;
        this.hapticFlash.Execute(0.0f, 0.005f, 0.005f, 1f, this.inputSource);
      }
      for (int index = 0; index < this.flashingRenderers.Count; ++index)
        this.flashingRenderers[index].material.SetColor(this.colorID, Color.Lerp(color, this.flashColor, t));
      if (!this.initialized)
        return;
      foreach (KeyValuePair<ISteamVR_Action_In_Source, ControllerButtonHints.ActionHintInfo> actionHintInfo in this.actionHintInfos)
      {
        if (actionHintInfo.Value.textHintActive)
          this.UpdateTextHint(actionHintInfo.Value);
      }
    }

    private void UpdateTextHint(ControllerButtonHints.ActionHintInfo hintInfo)
    {
      Transform hmdTransform = this.player.hmdTransform;
      Vector3 forward = hmdTransform.position - hintInfo.canvasOffset.position;
      Quaternion a = Quaternion.LookRotation(forward, Vector3.up);
      Quaternion b = Quaternion.LookRotation(forward, hmdTransform.up);
      float t = (double) hmdTransform.forward.y <= 0.0 ? Util.RemapNumberClamped(hmdTransform.forward.y, -0.8f, -0.6f, 1f, 0.0f) : Util.RemapNumberClamped(hmdTransform.forward.y, 0.6f, 0.4f, 1f, 0.0f);
      hintInfo.canvasOffset.rotation = Quaternion.Slerp(a, b, t);
      Transform transform = hintInfo.line.transform;
      hintInfo.line.useWorldSpace = false;
      hintInfo.line.SetPosition(0, transform.InverseTransformPoint(hintInfo.textStartAnchor.position));
      hintInfo.line.SetPosition(1, transform.InverseTransformPoint(hintInfo.textEndAnchor.position));
    }

    private void Clear()
    {
      this.renderers.Clear();
      this.flashingRenderers.Clear();
    }

    private void ShowText(ISteamVR_Action_In_Source action, string text, bool highlightButton = true)
    {
      if (!this.actionHintInfos.ContainsKey(action))
        return;
      ControllerButtonHints.ActionHintInfo actionHintInfo = this.actionHintInfos[action];
      actionHintInfo.textHintObject.SetActive(true);
      actionHintInfo.textHintActive = true;
      if ((Object) actionHintInfo.text != (Object) null)
        actionHintInfo.text.text = text;
      if ((Object) actionHintInfo.textMesh != (Object) null)
        actionHintInfo.textMesh.text = text;
      this.UpdateTextHint(actionHintInfo);
      if (highlightButton)
        this.ShowButtonHint(action);
      this.renderModel.gameObject.SetActive(true);
    }

    private void HideText(ISteamVR_Action_In_Source action)
    {
      if (!this.actionHintInfos.ContainsKey(action))
        return;
      ControllerButtonHints.ActionHintInfo actionHintInfo = this.actionHintInfos[action];
      actionHintInfo.textHintObject.SetActive(false);
      actionHintInfo.textHintActive = false;
      this.HideButtonHint(action);
    }

    private void HideAllText()
    {
      if (this.actionHintInfos == null)
        return;
      foreach (KeyValuePair<ISteamVR_Action_In_Source, ControllerButtonHints.ActionHintInfo> actionHintInfo in this.actionHintInfos)
      {
        actionHintInfo.Value.textHintObject.SetActive(false);
        actionHintInfo.Value.textHintActive = false;
      }
      this.HideAllButtonHints();
    }

    private string GetActiveHintText(ISteamVR_Action_In_Source action)
    {
      if (this.actionHintInfos.ContainsKey(action))
      {
        ControllerButtonHints.ActionHintInfo actionHintInfo = this.actionHintInfos[action];
        if (actionHintInfo.textHintActive)
          return actionHintInfo.text.text;
      }
      return string.Empty;
    }

    private static ControllerButtonHints GetControllerButtonHints(Hand hand)
    {
      if ((Object) hand != (Object) null)
      {
        ControllerButtonHints componentInChildren = hand.GetComponentInChildren<ControllerButtonHints>();
        if ((Object) componentInChildren != (Object) null && componentInChildren.initialized)
          return componentInChildren;
      }
      return (ControllerButtonHints) null;
    }

    public static void ShowButtonHint(Hand hand, params ISteamVR_Action_In_Source[] actions)
    {
      ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
      if (!((Object) controllerButtonHints != (Object) null))
        return;
      controllerButtonHints.ShowButtonHint(actions);
    }

    public static void HideButtonHint(Hand hand, params ISteamVR_Action_In_Source[] actions)
    {
      ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
      if (!((Object) controllerButtonHints != (Object) null))
        return;
      controllerButtonHints.HideButtonHint(actions);
    }

    public static void HideAllButtonHints(Hand hand)
    {
      ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
      if (!((Object) controllerButtonHints != (Object) null))
        return;
      controllerButtonHints.HideAllButtonHints();
    }

    public static bool IsButtonHintActive(Hand hand, ISteamVR_Action_In_Source action)
    {
      ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
      return (Object) controllerButtonHints != (Object) null && controllerButtonHints.IsButtonHintActive(action);
    }

    public static void ShowTextHint(
      Hand hand,
      ISteamVR_Action_In_Source action,
      string text,
      bool highlightButton = true)
    {
      ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
      if (!((Object) controllerButtonHints != (Object) null))
        return;
      controllerButtonHints.ShowText(action, text, highlightButton);
    }

    public static void HideTextHint(Hand hand, ISteamVR_Action_In_Source action)
    {
      ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
      if (!((Object) controllerButtonHints != (Object) null))
        return;
      controllerButtonHints.HideText(action);
    }

    public static void HideAllTextHints(Hand hand)
    {
      ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
      if (!((Object) controllerButtonHints != (Object) null))
        return;
      controllerButtonHints.HideAllText();
    }

    public static string GetActiveHintText(Hand hand, ISteamVR_Action_In_Source action)
    {
      ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
      return (Object) controllerButtonHints != (Object) null ? controllerButtonHints.GetActiveHintText(action) : string.Empty;
    }

    private enum OffsetType
    {
      Up,
      Right,
      Forward,
      Back,
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
      public UnityEngine.UI.Text text;
      public TextMesh textMesh;
      public Canvas textCanvas;
      public LineRenderer line;
      public float distanceFromCenter;
      public bool textHintActive;
    }
  }
}
