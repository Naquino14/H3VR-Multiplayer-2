// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessLayer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.VR;

namespace UnityEngine.Rendering.PostProcessing
{
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  [ImageEffectAllowedInSceneView]
  [AddComponentMenu("Rendering/Post-process Layer", -1)]
  [RequireComponent(typeof (Camera))]
  public sealed class PostProcessLayer : MonoBehaviour
  {
    public Transform volumeTrigger;
    public LayerMask volumeLayer;
    public bool stopNaNPropagation = true;
    public PostProcessLayer.Antialiasing antialiasingMode;
    public TemporalAntialiasing temporalAntialiasing;
    public SubpixelMorphologicalAntialiasing subpixelMorphologicalAntialiasing;
    public FastApproximateAntialiasing fastApproximateAntialiasing;
    public Fog fog;
    public Dithering dithering;
    public PostProcessDebugLayer debugLayer;
    [SerializeField]
    private PostProcessResources m_Resources;
    [SerializeField]
    private bool m_ShowToolkit;
    [SerializeField]
    private bool m_ShowCustomSorter;
    public bool breakBeforeColorGrading;
    [SerializeField]
    private List<PostProcessLayer.SerializedBundleRef> m_BeforeTransparentBundles;
    [SerializeField]
    private List<PostProcessLayer.SerializedBundleRef> m_BeforeStackBundles;
    [SerializeField]
    private List<PostProcessLayer.SerializedBundleRef> m_AfterStackBundles;
    private Dictionary<System.Type, PostProcessBundle> m_Bundles;
    private PropertySheetFactory m_PropertySheetFactory;
    private CommandBuffer m_LegacyCmdBufferBeforeReflections;
    private CommandBuffer m_LegacyCmdBufferBeforeLighting;
    private CommandBuffer m_LegacyCmdBufferOpaque;
    private CommandBuffer m_LegacyCmdBuffer;
    private Camera m_Camera;
    private PostProcessRenderContext m_CurrentContext;
    private LogHistogram m_LogHistogram;
    private bool m_SettingsUpdateNeeded = true;
    private bool m_IsRenderingInSceneView;
    private TargetPool m_TargetPool;
    private bool m_NaNKilled;
    private readonly List<PostProcessEffectRenderer> m_ActiveEffects = new List<PostProcessEffectRenderer>();
    private readonly List<RenderTargetIdentifier> m_Targets = new List<RenderTargetIdentifier>();

    public Dictionary<PostProcessEvent, List<PostProcessLayer.SerializedBundleRef>> sortedBundles { get; private set; }

    public bool haveBundlesBeenInited { get; private set; }

    private void OnEnable()
    {
      this.Init((PostProcessResources) null);
      if (!this.haveBundlesBeenInited)
        this.InitBundles();
      this.m_LogHistogram = new LogHistogram();
      this.m_PropertySheetFactory = new PropertySheetFactory();
      this.m_TargetPool = new TargetPool();
      this.debugLayer.OnEnable();
      if (RuntimeUtilities.scriptableRenderPipelineActive)
        return;
      this.m_LegacyCmdBufferBeforeReflections = new CommandBuffer()
      {
        name = "Deferred Ambient Occlusion"
      };
      this.m_LegacyCmdBufferBeforeLighting = new CommandBuffer()
      {
        name = "Deferred Ambient Occlusion"
      };
      this.m_LegacyCmdBufferOpaque = new CommandBuffer()
      {
        name = "Opaque Only Post-processing"
      };
      this.m_LegacyCmdBuffer = new CommandBuffer()
      {
        name = "Post-processing"
      };
      this.m_Camera = this.GetComponent<Camera>();
      this.m_Camera.forceIntoRenderTexture = true;
      this.m_Camera.AddCommandBuffer(CameraEvent.BeforeReflections, this.m_LegacyCmdBufferBeforeReflections);
      this.m_Camera.AddCommandBuffer(CameraEvent.BeforeLighting, this.m_LegacyCmdBufferBeforeLighting);
      this.m_Camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.m_LegacyCmdBufferOpaque);
      this.m_Camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, this.m_LegacyCmdBuffer);
      this.m_CurrentContext = new PostProcessRenderContext();
    }

    public void Init(PostProcessResources resources)
    {
      if ((UnityEngine.Object) resources != (UnityEngine.Object) null)
        this.m_Resources = resources;
      RuntimeUtilities.CreateIfNull<TemporalAntialiasing>(ref this.temporalAntialiasing);
      RuntimeUtilities.CreateIfNull<SubpixelMorphologicalAntialiasing>(ref this.subpixelMorphologicalAntialiasing);
      RuntimeUtilities.CreateIfNull<FastApproximateAntialiasing>(ref this.fastApproximateAntialiasing);
      RuntimeUtilities.CreateIfNull<Dithering>(ref this.dithering);
      RuntimeUtilities.CreateIfNull<Fog>(ref this.fog);
      RuntimeUtilities.CreateIfNull<PostProcessDebugLayer>(ref this.debugLayer);
    }

    public void InitBundles()
    {
      if (this.haveBundlesBeenInited)
        return;
      RuntimeUtilities.CreateIfNull<List<PostProcessLayer.SerializedBundleRef>>(ref this.m_BeforeTransparentBundles);
      RuntimeUtilities.CreateIfNull<List<PostProcessLayer.SerializedBundleRef>>(ref this.m_BeforeStackBundles);
      RuntimeUtilities.CreateIfNull<List<PostProcessLayer.SerializedBundleRef>>(ref this.m_AfterStackBundles);
      this.m_Bundles = new Dictionary<System.Type, PostProcessBundle>();
      foreach (System.Type key in PostProcessManager.instance.settingsTypes.Keys)
      {
        PostProcessBundle postProcessBundle = new PostProcessBundle((PostProcessEffectSettings) ScriptableObject.CreateInstance(key));
        this.m_Bundles.Add(key, postProcessBundle);
      }
      this.UpdateBundleSortList(this.m_BeforeTransparentBundles, PostProcessEvent.BeforeTransparent);
      this.UpdateBundleSortList(this.m_BeforeStackBundles, PostProcessEvent.BeforeStack);
      this.UpdateBundleSortList(this.m_AfterStackBundles, PostProcessEvent.AfterStack);
      this.sortedBundles = new Dictionary<PostProcessEvent, List<PostProcessLayer.SerializedBundleRef>>((IEqualityComparer<PostProcessEvent>) new PostProcessEventComparer())
      {
        {
          PostProcessEvent.BeforeTransparent,
          this.m_BeforeTransparentBundles
        },
        {
          PostProcessEvent.BeforeStack,
          this.m_BeforeStackBundles
        },
        {
          PostProcessEvent.AfterStack,
          this.m_AfterStackBundles
        }
      };
      this.haveBundlesBeenInited = true;
    }

    private void UpdateBundleSortList(
      List<PostProcessLayer.SerializedBundleRef> sortedList,
      PostProcessEvent evt)
    {
      List<PostProcessBundle> effects = this.m_Bundles.Where<KeyValuePair<System.Type, PostProcessBundle>>((Func<KeyValuePair<System.Type, PostProcessBundle>, bool>) (kvp => kvp.Value.attribute.eventType == evt && !kvp.Value.attribute.builtinEffect)).Select<KeyValuePair<System.Type, PostProcessBundle>, PostProcessBundle>((Func<KeyValuePair<System.Type, PostProcessBundle>, PostProcessBundle>) (kvp => kvp.Value)).ToList<PostProcessBundle>();
      sortedList.RemoveAll((Predicate<PostProcessLayer.SerializedBundleRef>) (x =>
      {
        string searchStr = x.assemblyQualifiedName;
        return !effects.Exists((Predicate<PostProcessBundle>) (b => b.settings.GetType().AssemblyQualifiedName == searchStr));
      }));
      foreach (PostProcessBundle postProcessBundle in effects)
      {
        string typeName = postProcessBundle.settings.GetType().AssemblyQualifiedName;
        if (!sortedList.Exists((Predicate<PostProcessLayer.SerializedBundleRef>) (b => b.assemblyQualifiedName == typeName)))
        {
          PostProcessLayer.SerializedBundleRef serializedBundleRef = new PostProcessLayer.SerializedBundleRef()
          {
            assemblyQualifiedName = typeName
          };
          sortedList.Add(serializedBundleRef);
        }
      }
      foreach (PostProcessLayer.SerializedBundleRef sorted in sortedList)
      {
        string typeName = sorted.assemblyQualifiedName;
        PostProcessBundle postProcessBundle = effects.Find((Predicate<PostProcessBundle>) (b => b.settings.GetType().AssemblyQualifiedName == typeName));
        sorted.bundle = postProcessBundle;
      }
    }

    private void OnDisable()
    {
      if (!RuntimeUtilities.scriptableRenderPipelineActive)
      {
        this.m_Camera.RemoveCommandBuffer(CameraEvent.BeforeReflections, this.m_LegacyCmdBufferBeforeReflections);
        this.m_Camera.RemoveCommandBuffer(CameraEvent.BeforeLighting, this.m_LegacyCmdBufferBeforeLighting);
        this.m_Camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.m_LegacyCmdBufferOpaque);
        this.m_Camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, this.m_LegacyCmdBuffer);
      }
      this.temporalAntialiasing.Release();
      this.m_LogHistogram.Release();
      foreach (PostProcessBundle postProcessBundle in this.m_Bundles.Values)
        postProcessBundle.Release();
      this.m_Bundles.Clear();
      this.m_PropertySheetFactory.Release();
      if (this.debugLayer != null)
        this.debugLayer.OnDisable();
      TextureLerper.instance.Clear();
      this.haveBundlesBeenInited = false;
    }

    private void Reset() => this.volumeTrigger = this.transform;

    private void OnPreCull()
    {
      if (RuntimeUtilities.scriptableRenderPipelineActive)
        return;
      this.m_Camera.ResetProjectionMatrix();
      this.m_Camera.nonJitteredProjectionMatrix = this.m_Camera.projectionMatrix;
      if (VRSettings.isDeviceActive)
        this.m_Camera.ResetStereoProjectionMatrices();
      this.BuildCommandBuffers();
    }

    private void OnPreRender()
    {
      if (RuntimeUtilities.scriptableRenderPipelineActive || this.m_Camera.stereoActiveEye != Camera.MonoOrStereoscopicEye.Right)
        return;
      this.BuildCommandBuffers();
    }

    private void BuildCommandBuffers()
    {
      PostProcessRenderContext currentContext = this.m_CurrentContext;
      RenderTextureFormat format = !this.m_Camera.allowHDR ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
      currentContext.Reset();
      currentContext.camera = this.m_Camera;
      currentContext.sourceFormat = format;
      this.m_LegacyCmdBufferBeforeReflections.Clear();
      this.m_LegacyCmdBufferBeforeLighting.Clear();
      this.m_LegacyCmdBufferOpaque.Clear();
      this.m_LegacyCmdBuffer.Clear();
      this.SetupContext(currentContext);
      currentContext.command = this.m_LegacyCmdBufferOpaque;
      this.UpdateSettingsIfNeeded(currentContext);
      PostProcessBundle bundle1 = this.GetBundle<AmbientOcclusion>();
      AmbientOcclusion ambientOcclusion = bundle1.CastSettings<AmbientOcclusion>();
      AmbientOcclusionRenderer occlusionRenderer = bundle1.CastRenderer<AmbientOcclusionRenderer>();
      bool flag1 = ambientOcclusion.IsEnabledAndSupported(currentContext);
      bool flag2 = occlusionRenderer.IsAmbientOnly(currentContext);
      bool flag3 = flag1 && flag2;
      bool flag4 = flag1 && !flag2;
      PostProcessBundle bundle2 = this.GetBundle<ScreenSpaceReflections>();
      PostProcessEffectSettings settings = bundle2.settings;
      PostProcessEffectRenderer renderer = bundle2.renderer;
      bool flag5 = settings.IsEnabledAndSupported(currentContext);
      if (flag3)
      {
        IAmbientOcclusionMethod ambientOcclusionMethod = occlusionRenderer.Get();
        currentContext.command = this.m_LegacyCmdBufferBeforeReflections;
        ambientOcclusionMethod.RenderAmbientOnly(currentContext);
        currentContext.command = this.m_LegacyCmdBufferBeforeLighting;
        ambientOcclusionMethod.CompositeAmbientOnly(currentContext);
      }
      else if (flag4)
      {
        currentContext.command = this.m_LegacyCmdBufferOpaque;
        occlusionRenderer.Get().RenderAfterOpaque(currentContext);
      }
      bool flag6 = this.fog.IsEnabledAndSupported(currentContext);
      bool flag7 = this.HasOpaqueOnlyEffects(currentContext);
      int num = 0 + (!flag5 ? 0 : 1) + (!flag6 ? 0 : 1) + (!flag7 ? 0 : 1);
      RenderTargetIdentifier source1 = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
      if (num > 0)
      {
        CommandBuffer legacyCmdBufferOpaque = this.m_LegacyCmdBufferOpaque;
        currentContext.command = legacyCmdBufferOpaque;
        int nameID1 = this.m_TargetPool.Get();
        legacyCmdBufferOpaque.GetTemporaryRT(nameID1, currentContext.width, currentContext.height, 24, FilterMode.Bilinear, format);
        legacyCmdBufferOpaque.Blit(source1, (RenderTargetIdentifier) nameID1);
        currentContext.source = (RenderTargetIdentifier) nameID1;
        int nameID2 = -1;
        if (num > 1)
        {
          nameID2 = this.m_TargetPool.Get();
          legacyCmdBufferOpaque.GetTemporaryRT(nameID2, currentContext.width, currentContext.height, 24, FilterMode.Bilinear, format);
          currentContext.destination = (RenderTargetIdentifier) nameID2;
        }
        else
          currentContext.destination = source1;
        if (flag5)
        {
          renderer.Render(currentContext);
          --num;
          RenderTargetIdentifier source2 = currentContext.source;
          currentContext.source = currentContext.destination;
          currentContext.destination = num != 1 ? source2 : source1;
        }
        if (flag6)
        {
          this.fog.Render(currentContext);
          --num;
          RenderTargetIdentifier source2 = currentContext.source;
          currentContext.source = currentContext.destination;
          currentContext.destination = num != 1 ? source2 : source1;
        }
        if (flag7)
          this.RenderOpaqueOnly(currentContext);
        if (num > 1)
          legacyCmdBufferOpaque.ReleaseTemporaryRT(nameID2);
        legacyCmdBufferOpaque.ReleaseTemporaryRT(nameID1);
      }
      int nameID = this.m_TargetPool.Get();
      this.m_LegacyCmdBuffer.GetTemporaryRT(nameID, currentContext.width, currentContext.height, 24, FilterMode.Bilinear, format);
      this.m_LegacyCmdBuffer.Blit(source1, (RenderTargetIdentifier) nameID, RuntimeUtilities.copyStdMaterial, !this.stopNaNPropagation ? 0 : 1);
      this.m_NaNKilled = this.stopNaNPropagation;
      currentContext.command = this.m_LegacyCmdBuffer;
      currentContext.source = (RenderTargetIdentifier) nameID;
      currentContext.destination = source1;
      this.Render(currentContext);
      this.m_LegacyCmdBuffer.ReleaseTemporaryRT(nameID);
    }

    private void OnPostRender()
    {
      if (RuntimeUtilities.scriptableRenderPipelineActive || !this.m_CurrentContext.IsTemporalAntialiasingActive())
        return;
      this.m_Camera.ResetProjectionMatrix();
      if (!VRSettings.isDeviceActive || !RuntimeUtilities.isSinglePassStereoEnabled && this.m_Camera.stereoActiveEye != Camera.MonoOrStereoscopicEye.Right)
        return;
      this.m_Camera.ResetStereoProjectionMatrices();
    }

    private PostProcessBundle GetBundle<T>() where T : PostProcessEffectSettings => this.GetBundle(typeof (T));

    private PostProcessBundle GetBundle(System.Type settingsType) => this.m_Bundles[settingsType];

    internal void OverrideSettings(List<PostProcessEffectSettings> baseSettings, float interpFactor)
    {
      foreach (PostProcessEffectSettings baseSetting in baseSettings)
      {
        if (baseSetting.active)
        {
          PostProcessEffectSettings settings = this.GetBundle(baseSetting.GetType()).settings;
          int count = baseSetting.parameters.Count;
          for (int index = 0; index < count; ++index)
          {
            ParameterOverride parameter1 = baseSetting.parameters[index];
            if (parameter1.overrideState)
            {
              ParameterOverride parameter2 = settings.parameters[index];
              parameter2.Interp(parameter2, parameter1, interpFactor);
            }
          }
        }
      }
    }

    private void SetLegacyCameraFlags(PostProcessRenderContext context)
    {
      DepthTextureMode depthTextureMode = context.camera.depthTextureMode;
      foreach (KeyValuePair<System.Type, PostProcessBundle> bundle in this.m_Bundles)
      {
        if (bundle.Value.settings.IsEnabledAndSupported(context))
          depthTextureMode |= bundle.Value.renderer.GetCameraFlags();
      }
      if (context.IsTemporalAntialiasingActive())
        depthTextureMode |= this.temporalAntialiasing.GetCameraFlags();
      if (this.fog.IsEnabledAndSupported(context))
        depthTextureMode |= this.fog.GetCameraFlags();
      if (this.debugLayer.debugOverlay != DebugOverlay.None)
        depthTextureMode |= this.debugLayer.GetCameraFlags();
      context.camera.depthTextureMode = depthTextureMode;
    }

    public void ResetHistory()
    {
      foreach (KeyValuePair<System.Type, PostProcessBundle> bundle in this.m_Bundles)
        bundle.Value.ResetHistory();
      this.temporalAntialiasing.ResetHistory();
    }

    public bool HasOpaqueOnlyEffects(PostProcessRenderContext context) => this.HasActiveEffects(PostProcessEvent.BeforeTransparent, context);

    public bool HasActiveEffects(PostProcessEvent evt, PostProcessRenderContext context)
    {
      foreach (PostProcessLayer.SerializedBundleRef serializedBundleRef in this.sortedBundles[evt])
      {
        if (serializedBundleRef.bundle.settings.IsEnabledAndSupported(context))
          return true;
      }
      return false;
    }

    private void SetupContext(PostProcessRenderContext context)
    {
      this.m_IsRenderingInSceneView = context.camera.cameraType == CameraType.SceneView;
      context.isSceneView = this.m_IsRenderingInSceneView;
      context.resources = this.m_Resources;
      context.propertySheets = this.m_PropertySheetFactory;
      context.debugLayer = this.debugLayer;
      context.antialiasing = this.antialiasingMode;
      context.temporalAntialiasing = this.temporalAntialiasing;
      context.logHistogram = this.m_LogHistogram;
      this.SetLegacyCameraFlags(context);
      this.debugLayer.SetFrameSize(context.width, context.height);
      this.m_CurrentContext = context;
    }

    private void UpdateSettingsIfNeeded(PostProcessRenderContext context)
    {
      if (this.m_SettingsUpdateNeeded)
      {
        context.command.BeginSample("VolumeBlending");
        PostProcessManager.instance.UpdateSettings(this);
        context.command.EndSample("VolumeBlending");
        this.m_TargetPool.Reset();
      }
      this.m_SettingsUpdateNeeded = false;
    }

    public void RenderOpaqueOnly(PostProcessRenderContext context)
    {
      if (RuntimeUtilities.scriptableRenderPipelineActive)
        this.SetupContext(context);
      TextureLerper.instance.BeginFrame(context);
      this.UpdateSettingsIfNeeded(context);
      this.RenderList(this.sortedBundles[PostProcessEvent.BeforeTransparent], context, "OpaqueOnly");
    }

    public void Render(PostProcessRenderContext context)
    {
      if (RuntimeUtilities.scriptableRenderPipelineActive)
        this.SetupContext(context);
      TextureLerper.instance.BeginFrame(context);
      CommandBuffer command = context.command;
      this.UpdateSettingsIfNeeded(context);
      int num = -1;
      if (this.stopNaNPropagation && !this.m_NaNKilled)
      {
        num = this.m_TargetPool.Get();
        command.GetTemporaryRT(num, context.width, context.height, 24, FilterMode.Bilinear, context.sourceFormat);
        command.BlitFullscreenTriangle(context.source, (RenderTargetIdentifier) num, RuntimeUtilities.copySheet, 1);
        context.source = (RenderTargetIdentifier) num;
        this.m_NaNKilled = true;
      }
      if (context.IsTemporalAntialiasingActive())
      {
        if (!RuntimeUtilities.scriptableRenderPipelineActive)
        {
          if (VRSettings.isDeviceActive)
          {
            if (context.camera.stereoActiveEye != Camera.MonoOrStereoscopicEye.Right)
              this.temporalAntialiasing.ConfigureStereoJitteredProjectionMatrices(context);
          }
          else
            this.temporalAntialiasing.ConfigureJitteredProjectionMatrix(context);
        }
        int nameID = this.m_TargetPool.Get();
        RenderTargetIdentifier destination = context.destination;
        command.GetTemporaryRT(nameID, context.width, context.height, 24, FilterMode.Bilinear, context.sourceFormat);
        context.destination = (RenderTargetIdentifier) nameID;
        this.temporalAntialiasing.Render(context);
        context.source = (RenderTargetIdentifier) nameID;
        context.destination = destination;
        if (num > -1)
          command.ReleaseTemporaryRT(num);
        num = nameID;
      }
      bool flag1 = this.HasActiveEffects(PostProcessEvent.BeforeStack, context);
      bool flag2 = this.HasActiveEffects(PostProcessEvent.AfterStack, context) && !this.breakBeforeColorGrading;
      bool flag3 = (flag2 || this.antialiasingMode == PostProcessLayer.Antialiasing.FastApproximateAntialiasing || this.antialiasingMode == PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing && this.subpixelMorphologicalAntialiasing.IsSupported()) && !this.breakBeforeColorGrading;
      if (flag1)
        num = this.RenderInjectionPoint(PostProcessEvent.BeforeStack, context, "BeforeStack", num);
      int releaseTargetAfterUse = this.RenderBuiltins(context, !flag3, num);
      if (flag2)
        releaseTargetAfterUse = this.RenderInjectionPoint(PostProcessEvent.AfterStack, context, "AfterStack", releaseTargetAfterUse);
      if (flag3)
        this.RenderFinalPass(context, releaseTargetAfterUse);
      this.debugLayer.RenderSpecialOverlays(context);
      this.debugLayer.RenderMonitors(context);
      TextureLerper.instance.EndFrame();
      this.debugLayer.EndFrame();
      this.m_SettingsUpdateNeeded = true;
      this.m_NaNKilled = false;
    }

    private int RenderInjectionPoint(
      PostProcessEvent evt,
      PostProcessRenderContext context,
      string marker,
      int releaseTargetAfterUse = -1)
    {
      int nameID = this.m_TargetPool.Get();
      RenderTargetIdentifier destination = context.destination;
      CommandBuffer command = context.command;
      command.GetTemporaryRT(nameID, context.width, context.height, 24, FilterMode.Bilinear, context.sourceFormat);
      context.destination = (RenderTargetIdentifier) nameID;
      this.RenderList(this.sortedBundles[evt], context, marker);
      context.source = (RenderTargetIdentifier) nameID;
      context.destination = destination;
      if (releaseTargetAfterUse > -1)
        command.ReleaseTemporaryRT(releaseTargetAfterUse);
      return nameID;
    }

    private void RenderList(
      List<PostProcessLayer.SerializedBundleRef> list,
      PostProcessRenderContext context,
      string marker)
    {
      CommandBuffer command = context.command;
      command.BeginSample(marker);
      this.m_ActiveEffects.Clear();
      for (int index = 0; index < list.Count; ++index)
      {
        PostProcessBundle bundle = list[index].bundle;
        if (bundle.settings.IsEnabledAndSupported(context) && (!context.isSceneView || context.isSceneView && bundle.attribute.allowInSceneView))
          this.m_ActiveEffects.Add(bundle.renderer);
      }
      int count = this.m_ActiveEffects.Count;
      if (count == 1)
      {
        this.m_ActiveEffects[0].Render(context);
      }
      else
      {
        this.m_Targets.Clear();
        this.m_Targets.Add(context.source);
        int nameID1 = this.m_TargetPool.Get();
        int nameID2 = this.m_TargetPool.Get();
        for (int index = 0; index < count - 1; ++index)
          this.m_Targets.Add((RenderTargetIdentifier) (index % 2 != 0 ? nameID2 : nameID1));
        this.m_Targets.Add(context.destination);
        command.GetTemporaryRT(nameID1, context.width, context.height, 24, FilterMode.Bilinear, context.sourceFormat);
        if (count > 2)
          command.GetTemporaryRT(nameID2, context.width, context.height, 24, FilterMode.Bilinear, context.sourceFormat);
        for (int index = 0; index < count; ++index)
        {
          context.source = this.m_Targets[index];
          context.destination = this.m_Targets[index + 1];
          this.m_ActiveEffects[index].Render(context);
        }
        command.ReleaseTemporaryRT(nameID1);
        if (count > 2)
          command.ReleaseTemporaryRT(nameID2);
      }
      command.EndSample(marker);
    }

    private int RenderBuiltins(
      PostProcessRenderContext context,
      bool isFinalPass,
      int releaseTargetAfterUse = -1)
    {
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.uber);
      propertySheet.ClearKeywords();
      propertySheet.properties.Clear();
      context.uberSheet = propertySheet;
      context.autoExposureTexture = (Texture) RuntimeUtilities.whiteTexture;
      CommandBuffer command = context.command;
      command.BeginSample("BuiltinStack");
      int nameID1 = -1;
      RenderTargetIdentifier destination = context.destination;
      if (!isFinalPass)
      {
        nameID1 = this.m_TargetPool.Get();
        command.GetTemporaryRT(nameID1, context.width, context.height, 24, FilterMode.Bilinear, context.sourceFormat);
        context.destination = (RenderTargetIdentifier) nameID1;
        if (this.antialiasingMode == PostProcessLayer.Antialiasing.FastApproximateAntialiasing && !this.fastApproximateAntialiasing.keepAlpha)
          propertySheet.properties.SetFloat(ShaderIDs.LumaInAlpha, 1f);
      }
      int num = this.RenderEffect<DepthOfField>(context, true);
      int nameID2 = this.RenderEffect<MotionBlur>(context, true);
      if (this.ShouldGenerateLogHistogram(context))
        this.m_LogHistogram.Generate(context);
      this.RenderEffect<AutoExposure>(context);
      propertySheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);
      this.RenderEffect<ChromaticAberration>(context);
      this.RenderEffect<Bloom>(context);
      this.RenderEffect<Vignette>(context);
      this.RenderEffect<Grain>(context);
      if (!this.breakBeforeColorGrading)
        this.RenderEffect<ColorGrading>(context);
      int pass = 0;
      if (isFinalPass)
      {
        propertySheet.EnableKeyword("FINALPASS");
        this.dithering.Render(context);
        if (context.flip && !context.isSceneView)
          pass = 1;
      }
      command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, pass);
      context.source = context.destination;
      context.destination = destination;
      if (releaseTargetAfterUse > -1)
        command.ReleaseTemporaryRT(releaseTargetAfterUse);
      if (nameID2 > -1)
        command.ReleaseTemporaryRT(nameID2);
      if (num > -1)
        command.ReleaseTemporaryRT(nameID2);
      command.EndSample("BuiltinStack");
      return nameID1;
    }

    private void RenderFinalPass(PostProcessRenderContext context, int releaseTargetAfterUse = -1)
    {
      CommandBuffer command = context.command;
      command.BeginSample("FinalPass");
      if (this.breakBeforeColorGrading)
      {
        PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.discardAlpha);
        command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
      }
      else
      {
        PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.finalPass);
        propertySheet.ClearKeywords();
        propertySheet.properties.Clear();
        context.uberSheet = propertySheet;
        int nameID = -1;
        if (this.antialiasingMode == PostProcessLayer.Antialiasing.FastApproximateAntialiasing)
        {
          propertySheet.EnableKeyword(!this.fastApproximateAntialiasing.mobileOptimized ? "FXAA" : "FXAA_LOW");
          if (this.fastApproximateAntialiasing.keepAlpha)
            propertySheet.EnableKeyword("FXAA_KEEP_ALPHA");
        }
        else if (this.antialiasingMode == PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing && this.subpixelMorphologicalAntialiasing.IsSupported())
        {
          nameID = this.m_TargetPool.Get();
          RenderTargetIdentifier destination = context.destination;
          context.command.GetTemporaryRT(nameID, context.width, context.height, 24, FilterMode.Bilinear, context.sourceFormat);
          context.destination = (RenderTargetIdentifier) nameID;
          this.subpixelMorphologicalAntialiasing.Render(context);
          context.source = (RenderTargetIdentifier) nameID;
          context.destination = destination;
        }
        this.dithering.Render(context);
        command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, !context.flip || context.isSceneView ? 0 : 1);
        if (nameID > -1)
          command.ReleaseTemporaryRT(nameID);
      }
      if (releaseTargetAfterUse > -1)
        command.ReleaseTemporaryRT(releaseTargetAfterUse);
      command.EndSample("FinalPass");
    }

    private int RenderEffect<T>(PostProcessRenderContext context, bool useTempTarget = false) where T : PostProcessEffectSettings
    {
      PostProcessBundle bundle = this.GetBundle<T>();
      if (!bundle.settings.IsEnabledAndSupported(context) || this.m_IsRenderingInSceneView && !bundle.attribute.allowInSceneView)
        return -1;
      if (!useTempTarget)
      {
        bundle.renderer.Render(context);
        return -1;
      }
      RenderTargetIdentifier destination = context.destination;
      int nameID = this.m_TargetPool.Get();
      context.command.GetTemporaryRT(nameID, context.width, context.height, 24, FilterMode.Bilinear, context.sourceFormat);
      context.destination = (RenderTargetIdentifier) nameID;
      bundle.renderer.Render(context);
      context.source = (RenderTargetIdentifier) nameID;
      context.destination = destination;
      return nameID;
    }

    private bool ShouldGenerateLogHistogram(PostProcessRenderContext context)
    {
      bool flag1 = this.GetBundle<AutoExposure>().settings.IsEnabledAndSupported(context);
      bool flag2 = this.debugLayer.lightMeter.IsRequestedAndSupported();
      return flag1 || flag2;
    }

    public enum Antialiasing
    {
      None,
      FastApproximateAntialiasing,
      SubpixelMorphologicalAntialiasing,
      TemporalAntialiasing,
    }

    [Serializable]
    public sealed class SerializedBundleRef
    {
      public string assemblyQualifiedName;
      public PostProcessBundle bundle;
    }
  }
}
