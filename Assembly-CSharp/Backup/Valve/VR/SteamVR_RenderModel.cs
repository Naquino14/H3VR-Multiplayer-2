// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_RenderModel
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Valve.VR
{
  [ExecuteInEditMode]
  public class SteamVR_RenderModel : MonoBehaviour
  {
    public SteamVR_TrackedObject.EIndex index = SteamVR_TrackedObject.EIndex.None;
    protected SteamVR_Input_Sources inputSource;
    public const string modelOverrideWarning = "Model override is really only meant to be used in the scene view for lining things up; using it at runtime is discouraged.  Use tracked device index instead to ensure the correct model is displayed for all users.";
    [Tooltip("Model override is really only meant to be used in the scene view for lining things up; using it at runtime is discouraged.  Use tracked device index instead to ensure the correct model is displayed for all users.")]
    public string modelOverride;
    [Tooltip("Shader to apply to model.")]
    public Shader shader;
    [Tooltip("Enable to print out when render models are loaded.")]
    public bool verbose;
    [Tooltip("If available, break down into separate components instead of loading as a single mesh.")]
    public bool createComponents = true;
    [Tooltip("Update transforms of components at runtime to reflect user action.")]
    public bool updateDynamically = true;
    public RenderModel_ControllerMode_State_t controllerModeState;
    public const string k_localTransformName = "attach";
    private Dictionary<string, Transform> componentAttachPoints = new Dictionary<string, Transform>();
    private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    public static Hashtable models = new Hashtable();
    public static Hashtable materials = new Hashtable();
    private SteamVR_Events.Action deviceConnectedAction;
    private SteamVR_Events.Action hideRenderModelsAction;
    private SteamVR_Events.Action modelSkinSettingsHaveChangedAction;
    private Dictionary<int, string> nameCache;

    private SteamVR_RenderModel()
    {
      this.deviceConnectedAction = SteamVR_Events.DeviceConnectedAction(new UnityAction<int, bool>(this.OnDeviceConnected));
      this.hideRenderModelsAction = SteamVR_Events.HideRenderModelsAction(new UnityAction<bool>(this.OnHideRenderModels));
      this.modelSkinSettingsHaveChangedAction = SteamVR_Events.SystemAction(EVREventType.VREvent_ModelSkinSettingsHaveChanged, new UnityAction<VREvent_t>(this.OnModelSkinSettingsHaveChanged));
    }

    public string renderModelName { get; private set; }

    public bool initializedAttachPoints { get; set; }

    private void OnModelSkinSettingsHaveChanged(VREvent_t vrEvent)
    {
      if (string.IsNullOrEmpty(this.renderModelName))
        return;
      this.renderModelName = string.Empty;
      this.UpdateModel();
    }

    public void SetMeshRendererState(bool state)
    {
      for (int index = 0; index < this.meshRenderers.Count; ++index)
      {
        MeshRenderer meshRenderer = this.meshRenderers[index];
        if ((UnityEngine.Object) meshRenderer != (UnityEngine.Object) null)
          meshRenderer.enabled = state;
      }
    }

    private void OnHideRenderModels(bool hidden) => this.SetMeshRendererState(!hidden);

    private void OnDeviceConnected(int i, bool connected)
    {
      if ((SteamVR_TrackedObject.EIndex) i != this.index || !connected)
        return;
      this.UpdateModel();
    }

    public void UpdateModel()
    {
      CVRSystem system = OpenVR.System;
      if (system == null || this.index == SteamVR_TrackedObject.EIndex.None)
        return;
      ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
      uint trackedDeviceProperty1 = system.GetStringTrackedDeviceProperty((uint) this.index, ETrackedDeviceProperty.Prop_RenderModelName_String, (StringBuilder) null, 0U, ref pError);
      if (trackedDeviceProperty1 <= 1U)
      {
        UnityEngine.Debug.LogError((object) ("<b>[SteamVR]</b> Failed to get render model name for tracked object " + (object) this.index));
      }
      else
      {
        StringBuilder pchValue = new StringBuilder((int) trackedDeviceProperty1);
        int trackedDeviceProperty2 = (int) system.GetStringTrackedDeviceProperty((uint) this.index, ETrackedDeviceProperty.Prop_RenderModelName_String, pchValue, trackedDeviceProperty1, ref pError);
        string newRenderModelName = pchValue.ToString();
        if (!(this.renderModelName != newRenderModelName))
          return;
        this.StartCoroutine(this.SetModelAsync(newRenderModelName));
      }
    }

    [DebuggerHidden]
    private IEnumerator SetModelAsync(string newRenderModelName) => (IEnumerator) new SteamVR_RenderModel.\u003CSetModelAsync\u003Ec__Iterator0()
    {
      newRenderModelName = newRenderModelName,
      \u0024this = this
    };

    private bool SetModel(string renderModelName)
    {
      this.StripMesh(this.gameObject);
      using (SteamVR_RenderModel.RenderModelInterfaceHolder holder = new SteamVR_RenderModel.RenderModelInterfaceHolder())
      {
        if (this.createComponents)
        {
          this.componentAttachPoints.Clear();
          if (this.LoadComponents(holder, renderModelName))
          {
            this.UpdateComponents(holder.instance);
            return true;
          }
          UnityEngine.Debug.Log((object) ("<b>[SteamVR]</b> [" + this.gameObject.name + "] Render model does not support components, falling back to single mesh."));
        }
        if (!string.IsNullOrEmpty(renderModelName))
        {
          if (!(SteamVR_RenderModel.models[(object) renderModelName] is SteamVR_RenderModel.RenderModel renderModel) || (UnityEngine.Object) renderModel.mesh == (UnityEngine.Object) null)
          {
            CVRRenderModels instance = holder.instance;
            if (instance == null)
              return false;
            if (this.verbose)
              UnityEngine.Debug.Log((object) ("<b>[SteamVR]</b> Loading render model " + renderModelName));
            renderModel = this.LoadRenderModel(instance, renderModelName, renderModelName);
            if (renderModel == null)
              return false;
            SteamVR_RenderModel.models[(object) renderModelName] = (object) renderModel;
          }
          this.gameObject.AddComponent<MeshFilter>().mesh = renderModel.mesh;
          MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
          meshRenderer.sharedMaterial = renderModel.material;
          this.meshRenderers.Add(meshRenderer);
          return true;
        }
      }
      return false;
    }

    private SteamVR_RenderModel.RenderModel LoadRenderModel(
      CVRRenderModels renderModels,
      string renderModelName,
      string baseName)
    {
      IntPtr zero1 = IntPtr.Zero;
      EVRRenderModelError renderModelError1;
      while (true)
      {
        renderModelError1 = renderModels.LoadRenderModel_Async(renderModelName, ref zero1);
        switch (renderModelError1)
        {
          case EVRRenderModelError.None:
            goto label_4;
          case EVRRenderModelError.Loading:
            SteamVR_RenderModel.Sleep();
            continue;
          default:
            goto label_3;
        }
      }
label_3:
      UnityEngine.Debug.LogError((object) string.Format("<b>[SteamVR]</b> Failed to load render model {0} - {1}", (object) renderModelName, (object) renderModelError1.ToString()));
      return (SteamVR_RenderModel.RenderModel) null;
label_4:
      RenderModel_t renderModelT = this.MarshalRenderModel(zero1);
      Vector3[] vector3Array1 = new Vector3[(IntPtr) renderModelT.unVertexCount];
      Vector3[] vector3Array2 = new Vector3[(IntPtr) renderModelT.unVertexCount];
      Vector2[] vector2Array = new Vector2[(IntPtr) renderModelT.unVertexCount];
      System.Type type = typeof (RenderModel_Vertex_t);
      for (int index = 0; (long) index < (long) renderModelT.unVertexCount; ++index)
      {
        RenderModel_Vertex_t structure = (RenderModel_Vertex_t) Marshal.PtrToStructure(new IntPtr(renderModelT.rVertexData.ToInt64() + (long) (index * Marshal.SizeOf(type))), type);
        vector3Array1[index] = new Vector3(structure.vPosition.v0, structure.vPosition.v1, -structure.vPosition.v2);
        vector3Array2[index] = new Vector3(structure.vNormal.v0, structure.vNormal.v1, -structure.vNormal.v2);
        vector2Array[index] = new Vector2(structure.rfTextureCoord0, structure.rfTextureCoord1);
      }
      int length = (int) renderModelT.unTriangleCount * 3;
      short[] destination1 = new short[length];
      Marshal.Copy(renderModelT.rIndexData, destination1, 0, destination1.Length);
      int[] numArray1 = new int[length];
      for (int index = 0; (long) index < (long) renderModelT.unTriangleCount; ++index)
      {
        numArray1[index * 3] = (int) destination1[index * 3 + 2];
        numArray1[index * 3 + 1] = (int) destination1[index * 3 + 1];
        numArray1[index * 3 + 2] = (int) destination1[index * 3];
      }
      Mesh mesh = new Mesh();
      mesh.vertices = vector3Array1;
      mesh.normals = vector3Array2;
      mesh.uv = vector2Array;
      mesh.triangles = numArray1;
      Material material = SteamVR_RenderModel.materials[(object) renderModelT.diffuseTextureId] as Material;
      if ((UnityEngine.Object) material == (UnityEngine.Object) null || (UnityEngine.Object) material.mainTexture == (UnityEngine.Object) null)
      {
        IntPtr zero2 = IntPtr.Zero;
        EVRRenderModelError renderModelError2;
        while (true)
        {
          renderModelError2 = renderModels.LoadTexture_Async(renderModelT.diffuseTextureId, ref zero2);
          switch (renderModelError2)
          {
            case EVRRenderModelError.None:
              goto label_14;
            case EVRRenderModelError.Loading:
              SteamVR_RenderModel.Sleep();
              continue;
            default:
              goto label_26;
          }
        }
label_14:
        RenderModel_TextureMap_t modelTextureMapT = this.MarshalRenderModel_TextureMap(zero2);
        Texture2D texture2D = new Texture2D((int) modelTextureMapT.unWidth, (int) modelTextureMapT.unHeight, TextureFormat.RGBA32, false);
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11)
        {
          texture2D.Apply();
          IntPtr nativeTexturePtr = texture2D.GetNativeTexturePtr();
          while (renderModels.LoadIntoTextureD3D11_Async(renderModelT.diffuseTextureId, nativeTexturePtr) == EVRRenderModelError.Loading)
            SteamVR_RenderModel.Sleep();
        }
        else
        {
          byte[] destination2 = new byte[(int) modelTextureMapT.unWidth * (int) modelTextureMapT.unHeight * 4];
          Marshal.Copy(modelTextureMapT.rubTextureMapData, destination2, 0, destination2.Length);
          Color32[] colors = new Color32[(int) modelTextureMapT.unWidth * (int) modelTextureMapT.unHeight];
          int num1 = 0;
          for (int index1 = 0; index1 < (int) modelTextureMapT.unHeight; ++index1)
          {
            for (int index2 = 0; index2 < (int) modelTextureMapT.unWidth; ++index2)
            {
              byte[] numArray2 = destination2;
              int index3 = num1;
              int num2 = index3 + 1;
              byte r = numArray2[index3];
              byte[] numArray3 = destination2;
              int index4 = num2;
              int num3 = index4 + 1;
              byte g = numArray3[index4];
              byte[] numArray4 = destination2;
              int index5 = num3;
              int num4 = index5 + 1;
              byte b = numArray4[index5];
              byte[] numArray5 = destination2;
              int index6 = num4;
              num1 = index6 + 1;
              byte a = numArray5[index6];
              colors[index1 * (int) modelTextureMapT.unWidth + index2] = new Color32(r, g, b, a);
            }
          }
          texture2D.SetPixels32(colors);
          texture2D.Apply();
        }
        material = new Material(!((UnityEngine.Object) this.shader != (UnityEngine.Object) null) ? Shader.Find("Standard") : this.shader);
        material.mainTexture = (Texture) texture2D;
        SteamVR_RenderModel.materials[(object) renderModelT.diffuseTextureId] = (object) material;
        renderModels.FreeTexture(zero2);
        goto label_27;
label_26:
        UnityEngine.Debug.Log((object) ("<b>[SteamVR]</b> Failed to load render model texture for render model " + renderModelName + ". Error: " + renderModelError2.ToString()));
      }
label_27:
      this.StartCoroutine(this.FreeRenderModel(zero1));
      return new SteamVR_RenderModel.RenderModel(mesh, material);
    }

    [DebuggerHidden]
    private IEnumerator FreeRenderModel(IntPtr pRenderModel) => (IEnumerator) new SteamVR_RenderModel.\u003CFreeRenderModel\u003Ec__Iterator1()
    {
      pRenderModel = pRenderModel
    };

    public Transform FindTransformByName(string componentName, Transform inTransform = null)
    {
      if ((UnityEngine.Object) inTransform == (UnityEngine.Object) null)
        inTransform = this.transform;
      for (int index = 0; index < inTransform.childCount; ++index)
      {
        Transform child = inTransform.GetChild(index);
        if (child.name == componentName)
          return child;
      }
      return (Transform) null;
    }

    public Transform GetComponentTransform(string componentName)
    {
      if (componentName == null)
        return this.transform;
      return this.componentAttachPoints.ContainsKey(componentName) ? this.componentAttachPoints[componentName] : (Transform) null;
    }

    private void StripMesh(GameObject go)
    {
      MeshRenderer component1 = go.GetComponent<MeshRenderer>();
      if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component1);
      MeshFilter component2 = go.GetComponent<MeshFilter>();
      if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component2);
    }

    private bool LoadComponents(
      SteamVR_RenderModel.RenderModelInterfaceHolder holder,
      string renderModelName)
    {
      Transform transform1 = this.transform;
      for (int index = 0; index < transform1.childCount; ++index)
      {
        Transform child = transform1.GetChild(index);
        child.gameObject.SetActive(false);
        this.StripMesh(child.gameObject);
      }
      if (string.IsNullOrEmpty(renderModelName))
        return true;
      CVRRenderModels instance = holder.instance;
      if (instance == null)
        return false;
      uint componentCount = instance.GetComponentCount(renderModelName);
      if (componentCount == 0U)
        return false;
      for (int index = 0; (long) index < (long) componentCount; ++index)
      {
        uint componentName = instance.GetComponentName(renderModelName, (uint) index, (StringBuilder) null, 0U);
        if (componentName != 0U)
        {
          StringBuilder pchComponentName = new StringBuilder((int) componentName);
          if (instance.GetComponentName(renderModelName, (uint) index, pchComponentName, componentName) != 0U)
          {
            string str = pchComponentName.ToString();
            Transform inTransform = this.FindTransformByName(str);
            if ((UnityEngine.Object) inTransform != (UnityEngine.Object) null)
            {
              inTransform.gameObject.SetActive(true);
              this.componentAttachPoints[str] = this.FindTransformByName("attach", inTransform);
            }
            else
            {
              inTransform = new GameObject(str).transform;
              inTransform.parent = this.transform;
              inTransform.gameObject.layer = this.gameObject.layer;
              Transform transform2 = new GameObject("attach").transform;
              transform2.parent = inTransform;
              transform2.localPosition = Vector3.zero;
              transform2.localRotation = Quaternion.identity;
              transform2.localScale = Vector3.one;
              transform2.gameObject.layer = this.gameObject.layer;
              this.componentAttachPoints[str] = transform2;
            }
            inTransform.localPosition = Vector3.zero;
            inTransform.localRotation = Quaternion.identity;
            inTransform.localScale = Vector3.one;
            uint componentRenderModelName = instance.GetComponentRenderModelName(renderModelName, str, (StringBuilder) null, 0U);
            if (componentRenderModelName != 0U)
            {
              StringBuilder pchComponentRenderModelName = new StringBuilder((int) componentRenderModelName);
              if (instance.GetComponentRenderModelName(renderModelName, str, pchComponentRenderModelName, componentRenderModelName) != 0U)
              {
                string renderModelName1 = pchComponentRenderModelName.ToString();
                if (!(SteamVR_RenderModel.models[(object) renderModelName1] is SteamVR_RenderModel.RenderModel renderModel) || (UnityEngine.Object) renderModel.mesh == (UnityEngine.Object) null)
                {
                  if (this.verbose)
                    UnityEngine.Debug.Log((object) ("<b>[SteamVR]</b> Loading render model " + renderModelName1));
                  renderModel = this.LoadRenderModel(instance, renderModelName1, renderModelName);
                  if (renderModel != null)
                    SteamVR_RenderModel.models[(object) renderModelName1] = (object) renderModel;
                  else
                    continue;
                }
                inTransform.gameObject.AddComponent<MeshFilter>().mesh = renderModel.mesh;
                MeshRenderer meshRenderer = inTransform.gameObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = renderModel.material;
                this.meshRenderers.Add(meshRenderer);
              }
            }
          }
        }
      }
      return true;
    }

    private void OnEnable()
    {
      if (!string.IsNullOrEmpty(this.modelOverride))
      {
        UnityEngine.Debug.Log((object) "<b>[SteamVR]</b> Model override is really only meant to be used in the scene view for lining things up; using it at runtime is discouraged.  Use tracked device index instead to ensure the correct model is displayed for all users.");
        this.enabled = false;
      }
      else
      {
        CVRSystem system = OpenVR.System;
        if (system != null && system.IsTrackedDeviceConnected((uint) this.index))
          this.UpdateModel();
        this.deviceConnectedAction.enabled = true;
        this.hideRenderModelsAction.enabled = true;
        this.modelSkinSettingsHaveChangedAction.enabled = true;
      }
    }

    private void OnDisable()
    {
      this.deviceConnectedAction.enabled = false;
      this.hideRenderModelsAction.enabled = false;
      this.modelSkinSettingsHaveChangedAction.enabled = false;
    }

    private void Update()
    {
      if (!this.updateDynamically)
        return;
      this.UpdateComponents(OpenVR.RenderModels);
    }

    public void UpdateComponents(CVRRenderModels renderModels)
    {
      if (renderModels == null || this.transform.childCount == 0)
        return;
      if (this.nameCache == null)
        this.nameCache = new Dictionary<int, string>();
      for (int index1 = 0; index1 < this.transform.childCount; ++index1)
      {
        Transform child1 = this.transform.GetChild(index1);
        string name1;
        if (!this.nameCache.TryGetValue(child1.GetInstanceID(), out name1))
        {
          name1 = child1.name;
          this.nameCache.Add(child1.GetInstanceID(), name1);
        }
        RenderModel_ComponentState_t pComponentState = new RenderModel_ComponentState_t();
        if (renderModels.GetComponentStateForDevicePath(this.renderModelName, name1, SteamVR_Input_Source.GetHandle(this.inputSource), ref this.controllerModeState, ref pComponentState))
        {
          child1.localPosition = SteamVR_Utils.GetPosition(pComponentState.mTrackingToComponentRenderModel);
          child1.localRotation = SteamVR_Utils.GetRotation(pComponentState.mTrackingToComponentRenderModel);
          Transform transform = (Transform) null;
          for (int index2 = 0; index2 < child1.childCount; ++index2)
          {
            Transform child2 = child1.GetChild(index2);
            int instanceId = child2.GetInstanceID();
            string name2;
            if (!this.nameCache.TryGetValue(instanceId, out name2))
            {
              name2 = child2.name;
              this.nameCache.Add(instanceId, name1);
            }
            if (name2 == "attach")
              transform = child2;
          }
          if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
          {
            transform.position = this.transform.TransformPoint(SteamVR_Utils.GetPosition(pComponentState.mTrackingToComponentLocal));
            transform.rotation = this.transform.rotation * SteamVR_Utils.GetRotation(pComponentState.mTrackingToComponentLocal);
            this.initializedAttachPoints = true;
          }
          bool flag = ((int) pComponentState.uProperties & 2) != 0;
          if (flag != child1.gameObject.activeSelf)
            child1.gameObject.SetActive(flag);
        }
      }
    }

    public void SetDeviceIndex(int newIndex)
    {
      this.index = (SteamVR_TrackedObject.EIndex) newIndex;
      this.modelOverride = string.Empty;
      if (!this.enabled)
        return;
      this.UpdateModel();
    }

    public void SetInputSource(SteamVR_Input_Sources newInputSource) => this.inputSource = newInputSource;

    private static void Sleep() => Thread.Sleep(1);

    private RenderModel_t MarshalRenderModel(IntPtr pRenderModel)
    {
      if (Environment.OSVersion.Platform != PlatformID.MacOSX && Environment.OSVersion.Platform != PlatformID.Unix)
        return (RenderModel_t) Marshal.PtrToStructure(pRenderModel, typeof (RenderModel_t));
      RenderModel_t_Packed structure = (RenderModel_t_Packed) Marshal.PtrToStructure(pRenderModel, typeof (RenderModel_t_Packed));
      RenderModel_t unpacked = new RenderModel_t();
      structure.Unpack(ref unpacked);
      return unpacked;
    }

    private RenderModel_TextureMap_t MarshalRenderModel_TextureMap(
      IntPtr pRenderModel)
    {
      if (Environment.OSVersion.Platform != PlatformID.MacOSX && Environment.OSVersion.Platform != PlatformID.Unix)
        return (RenderModel_TextureMap_t) Marshal.PtrToStructure(pRenderModel, typeof (RenderModel_TextureMap_t));
      RenderModel_TextureMap_t_Packed structure = (RenderModel_TextureMap_t_Packed) Marshal.PtrToStructure(pRenderModel, typeof (RenderModel_TextureMap_t_Packed));
      RenderModel_TextureMap_t unpacked = new RenderModel_TextureMap_t();
      structure.Unpack(ref unpacked);
      return unpacked;
    }

    public class RenderModel
    {
      public RenderModel(Mesh mesh, Material material)
      {
        this.mesh = mesh;
        this.material = material;
      }

      public Mesh mesh { get; private set; }

      public Material material { get; private set; }
    }

    public sealed class RenderModelInterfaceHolder : IDisposable
    {
      private bool needsShutdown;
      private bool failedLoadInterface;
      private CVRRenderModels _instance;

      public CVRRenderModels instance
      {
        get
        {
          if (this._instance == null && !this.failedLoadInterface)
          {
            if (Application.isEditor && !Application.isPlaying)
              this.needsShutdown = SteamVR.InitializeTemporarySession();
            this._instance = OpenVR.RenderModels;
            if (this._instance == null)
            {
              UnityEngine.Debug.LogError((object) "<b>[SteamVR]</b> Failed to load IVRRenderModels interface version IVRRenderModels_006");
              this.failedLoadInterface = true;
            }
          }
          return this._instance;
        }
      }

      public void Dispose()
      {
        if (!this.needsShutdown)
          return;
        SteamVR.ExitTemporarySession();
      }
    }
  }
}
