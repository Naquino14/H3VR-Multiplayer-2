// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_ExternalCamera
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace Valve.VR
{
  public class SteamVR_ExternalCamera : MonoBehaviour
  {
    public SteamVR_ExternalCamera.Config config;
    public string configPath;
    [Tooltip("This will automatically activate the action set the specified pose belongs to. And deactivate it when this component is disabled.")]
    public bool autoEnableDisableActionSet = true;
    private FileSystemWatcher watcher;
    private Camera cam;
    private Transform target;
    private GameObject clipQuad;
    private Material clipMaterial;
    protected SteamVR_ActionSet activatedActionSet;
    protected SteamVR_Input_Sources activatedInputSource;
    private Material colorMat;
    private Material alphaMat;
    private Camera[] cameras;
    private Rect[] cameraRects;
    private float sceneResolutionScale;

    public void ReadConfig()
    {
      try
      {
        HmdMatrix34_t pose = new HmdMatrix34_t();
        bool flag = false;
        object config = (object) this.config;
        foreach (string readAllLine in File.ReadAllLines(this.configPath))
        {
          char[] chArray = new char[1]{ '=' };
          string[] strArray1 = readAllLine.Split(chArray);
          if (strArray1.Length == 2)
          {
            string name = strArray1[0];
            if (name == "m")
            {
              string[] strArray2 = strArray1[1].Split(',');
              if (strArray2.Length == 12)
              {
                pose.m0 = float.Parse(strArray2[0]);
                pose.m1 = float.Parse(strArray2[1]);
                pose.m2 = float.Parse(strArray2[2]);
                pose.m3 = float.Parse(strArray2[3]);
                pose.m4 = float.Parse(strArray2[4]);
                pose.m5 = float.Parse(strArray2[5]);
                pose.m6 = float.Parse(strArray2[6]);
                pose.m7 = float.Parse(strArray2[7]);
                pose.m8 = float.Parse(strArray2[8]);
                pose.m9 = float.Parse(strArray2[9]);
                pose.m10 = float.Parse(strArray2[10]);
                pose.m11 = float.Parse(strArray2[11]);
                flag = true;
              }
            }
            else if (name == "disableStandardAssets")
              config.GetType().GetField(name)?.SetValue(config, (object) bool.Parse(strArray1[1]));
            else
              config.GetType().GetField(name)?.SetValue(config, (object) float.Parse(strArray1[1]));
          }
        }
        this.config = (SteamVR_ExternalCamera.Config) config;
        if (flag)
        {
          SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(pose);
          this.config.x = rigidTransform.pos.x;
          this.config.y = rigidTransform.pos.y;
          this.config.z = rigidTransform.pos.z;
          Vector3 eulerAngles = rigidTransform.rot.eulerAngles;
          this.config.rx = eulerAngles.x;
          this.config.ry = eulerAngles.y;
          this.config.rz = eulerAngles.z;
        }
      }
      catch
      {
      }
      this.target = (Transform) null;
      if (this.watcher != null)
        return;
      FileInfo fileInfo = new FileInfo(this.configPath);
      this.watcher = new FileSystemWatcher(fileInfo.DirectoryName, fileInfo.Name);
      this.watcher.NotifyFilter = NotifyFilters.LastWrite;
      this.watcher.Changed += new FileSystemEventHandler(this.OnChanged);
      this.watcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object source, FileSystemEventArgs e) => this.ReadConfig();

    public void AttachToCamera(SteamVR_Camera steamVR_Camera)
    {
      Camera camera;
      if ((UnityEngine.Object) steamVR_Camera == (UnityEngine.Object) null)
      {
        camera = Camera.main;
        if ((UnityEngine.Object) this.target == (UnityEngine.Object) camera.transform)
          return;
        this.target = camera.transform;
      }
      else
      {
        camera = steamVR_Camera.camera;
        if ((UnityEngine.Object) this.target == (UnityEngine.Object) steamVR_Camera.head)
          return;
        this.target = steamVR_Camera.head;
      }
      Transform parent1 = this.transform.parent;
      Transform parent2 = this.target.parent;
      parent1.parent = parent2;
      parent1.localPosition = Vector3.zero;
      parent1.localRotation = Quaternion.identity;
      parent1.localScale = Vector3.one;
      camera.enabled = false;
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(camera.gameObject);
      camera.enabled = true;
      gameObject.name = "camera";
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) gameObject.GetComponent<SteamVR_Camera>());
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) gameObject.GetComponent<SteamVR_Fade>());
      this.cam = gameObject.GetComponent<Camera>();
      this.cam.stereoTargetEye = StereoTargetEyeMask.None;
      this.cam.fieldOfView = this.config.fov;
      this.cam.useOcclusionCulling = false;
      this.cam.enabled = false;
      this.cam.rect = new Rect(0.0f, 0.0f, 1f, 1f);
      this.colorMat = new Material(Shader.Find("Custom/SteamVR_ColorOut"));
      this.alphaMat = new Material(Shader.Find("Custom/SteamVR_AlphaOut"));
      this.clipMaterial = new Material(Shader.Find("Custom/SteamVR_ClearAll"));
      Transform transform1 = gameObject.transform;
      transform1.parent = this.transform;
      transform1.localPosition = new Vector3(this.config.x, this.config.y, this.config.z);
      transform1.localRotation = Quaternion.Euler(this.config.rx, this.config.ry, this.config.rz);
      transform1.localScale = Vector3.one;
      while (transform1.childCount > 0)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) transform1.GetChild(0).gameObject);
      this.clipQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
      this.clipQuad.name = "ClipQuad";
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.clipQuad.GetComponent<MeshCollider>());
      MeshRenderer component = this.clipQuad.GetComponent<MeshRenderer>();
      component.material = this.clipMaterial;
      component.shadowCastingMode = ShadowCastingMode.Off;
      component.receiveShadows = false;
      component.lightProbeUsage = LightProbeUsage.Off;
      component.reflectionProbeUsage = ReflectionProbeUsage.Off;
      Transform transform2 = this.clipQuad.transform;
      transform2.parent = transform1;
      transform2.localScale = new Vector3(1000f, 1000f, 1f);
      transform2.localRotation = Quaternion.identity;
      this.clipQuad.SetActive(false);
    }

    public float GetTargetDistance()
    {
      if ((UnityEngine.Object) this.target == (UnityEngine.Object) null)
        return this.config.near + 0.01f;
      Transform transform = this.cam.transform;
      return Mathf.Clamp(-new Plane(new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized, this.target.position + new Vector3(this.target.forward.x, 0.0f, this.target.forward.z).normalized * this.config.hmdOffset).GetDistanceToPoint(transform.position), this.config.near + 0.01f, this.config.far - 0.01f);
    }

    public void RenderNear()
    {
      int width = Screen.width / 2;
      int height = Screen.height / 2;
      if ((UnityEngine.Object) this.cam.targetTexture == (UnityEngine.Object) null || this.cam.targetTexture.width != width || this.cam.targetTexture.height != height)
        this.cam.targetTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
        {
          antiAliasing = QualitySettings.antiAliasing != 0 ? QualitySettings.antiAliasing : 1
        };
      this.cam.nearClipPlane = this.config.near;
      this.cam.farClipPlane = this.config.far;
      CameraClearFlags clearFlags = this.cam.clearFlags;
      Color backgroundColor = this.cam.backgroundColor;
      this.cam.clearFlags = CameraClearFlags.Color;
      this.cam.backgroundColor = Color.clear;
      this.clipMaterial.color = new Color(this.config.r, this.config.g, this.config.b, this.config.a);
      float num = Mathf.Clamp(this.GetTargetDistance() + this.config.nearOffset, this.config.near, this.config.far);
      Transform parent = this.clipQuad.transform.parent;
      this.clipQuad.transform.position = parent.position + parent.forward * num;
      MonoBehaviour[] monoBehaviourArray = (MonoBehaviour[]) null;
      bool[] flagArray = (bool[]) null;
      if (this.config.disableStandardAssets)
      {
        monoBehaviourArray = this.cam.gameObject.GetComponents<MonoBehaviour>();
        flagArray = new bool[monoBehaviourArray.Length];
        for (int index = 0; index < monoBehaviourArray.Length; ++index)
        {
          MonoBehaviour monoBehaviour = monoBehaviourArray[index];
          if (monoBehaviour.enabled && monoBehaviour.GetType().ToString().StartsWith("UnityStandardAssets."))
          {
            monoBehaviour.enabled = false;
            flagArray[index] = true;
          }
        }
      }
      this.clipQuad.SetActive(true);
      this.cam.Render();
      Graphics.DrawTexture(new Rect(0.0f, 0.0f, (float) width, (float) height), (Texture) this.cam.targetTexture, this.colorMat);
      MonoBehaviour component = this.cam.gameObject.GetComponent("PostProcessingBehaviour") as MonoBehaviour;
      if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.enabled)
      {
        component.enabled = false;
        this.cam.Render();
        component.enabled = true;
      }
      Graphics.DrawTexture(new Rect((float) width, 0.0f, (float) width, (float) height), (Texture) this.cam.targetTexture, this.alphaMat);
      this.clipQuad.SetActive(false);
      if (monoBehaviourArray != null)
      {
        for (int index = 0; index < monoBehaviourArray.Length; ++index)
        {
          if (flagArray[index])
            monoBehaviourArray[index].enabled = true;
        }
      }
      this.cam.clearFlags = clearFlags;
      this.cam.backgroundColor = backgroundColor;
    }

    public void RenderFar()
    {
      this.cam.nearClipPlane = this.config.near;
      this.cam.farClipPlane = this.config.far;
      this.cam.Render();
      int num1 = Screen.width / 2;
      int num2 = Screen.height / 2;
      Graphics.DrawTexture(new Rect(0.0f, (float) num2, (float) num1, (float) num2), (Texture) this.cam.targetTexture, this.colorMat);
    }

    private void OnGUI()
    {
    }

    private void OnEnable()
    {
      this.cameras = UnityEngine.Object.FindObjectsOfType<Camera>();
      if (this.cameras != null)
      {
        int length = this.cameras.Length;
        this.cameraRects = new Rect[length];
        for (int index = 0; index < length; ++index)
        {
          Camera camera = this.cameras[index];
          this.cameraRects[index] = camera.rect;
          if (!((UnityEngine.Object) camera == (UnityEngine.Object) this.cam) && !((UnityEngine.Object) camera.targetTexture != (UnityEngine.Object) null) && !((UnityEngine.Object) camera.GetComponent<SteamVR_Camera>() != (UnityEngine.Object) null))
            camera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
        }
      }
      if ((double) this.config.sceneResolutionScale > 0.0)
      {
        this.sceneResolutionScale = SteamVR_Camera.sceneResolutionScale;
        SteamVR_Camera.sceneResolutionScale = this.config.sceneResolutionScale;
      }
      if (!this.autoEnableDisableActionSet)
        return;
      SteamVR_Behaviour_Pose componentInChildren = this.GetComponentInChildren<SteamVR_Behaviour_Pose>();
      if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null) || componentInChildren.poseAction.actionSet.IsActive(componentInChildren.inputSource))
        return;
      this.activatedActionSet = componentInChildren.poseAction.actionSet;
      this.activatedInputSource = componentInChildren.inputSource;
      componentInChildren.poseAction.actionSet.Activate(componentInChildren.inputSource, 0, false);
    }

    private void OnDisable()
    {
      if (this.autoEnableDisableActionSet && this.activatedActionSet != (SteamVR_ActionSet) null)
      {
        this.activatedActionSet.Deactivate(this.activatedInputSource);
        this.activatedActionSet = (SteamVR_ActionSet) null;
      }
      if (this.cameras != null)
      {
        int length = this.cameras.Length;
        for (int index = 0; index < length; ++index)
        {
          Camera camera = this.cameras[index];
          if ((UnityEngine.Object) camera != (UnityEngine.Object) null)
            camera.rect = this.cameraRects[index];
        }
        this.cameras = (Camera[]) null;
        this.cameraRects = (Rect[]) null;
      }
      if ((double) this.config.sceneResolutionScale <= 0.0)
        return;
      SteamVR_Camera.sceneResolutionScale = this.sceneResolutionScale;
    }

    [Serializable]
    public struct Config
    {
      public float x;
      public float y;
      public float z;
      public float rx;
      public float ry;
      public float rz;
      public float fov;
      public float near;
      public float far;
      public float sceneResolutionScale;
      public float frameSkip;
      public float nearOffset;
      public float farOffset;
      public float hmdOffset;
      public float r;
      public float g;
      public float b;
      public float a;
      public bool disableStandardAssets;
    }
  }
}
