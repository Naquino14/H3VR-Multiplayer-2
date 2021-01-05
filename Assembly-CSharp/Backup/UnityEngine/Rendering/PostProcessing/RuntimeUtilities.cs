// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.RuntimeUtilities
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine.VR;

namespace UnityEngine.Rendering.PostProcessing
{
  public static class RuntimeUtilities
  {
    private static Texture2D m_WhiteTexture;
    private static Texture2D m_BlackTexture;
    private static Texture2D m_TransparentTexture;
    private static Mesh s_FullscreenTriangle;
    private static Material s_CopyStdMaterial;
    private static Material s_CopyMaterial;
    private static PropertySheet s_CopySheet;

    public static Texture2D whiteTexture
    {
      get
      {
        if ((UnityEngine.Object) RuntimeUtilities.m_WhiteTexture == (UnityEngine.Object) null)
        {
          RuntimeUtilities.m_WhiteTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
          RuntimeUtilities.m_WhiteTexture.SetPixel(0, 0, Color.white);
          RuntimeUtilities.m_WhiteTexture.Apply();
        }
        return RuntimeUtilities.m_WhiteTexture;
      }
    }

    public static Texture2D blackTexture
    {
      get
      {
        if ((UnityEngine.Object) RuntimeUtilities.m_BlackTexture == (UnityEngine.Object) null)
        {
          RuntimeUtilities.m_BlackTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
          RuntimeUtilities.m_BlackTexture.SetPixel(0, 0, Color.black);
          RuntimeUtilities.m_BlackTexture.Apply();
        }
        return RuntimeUtilities.m_BlackTexture;
      }
    }

    public static Texture2D transparentTexture
    {
      get
      {
        if ((UnityEngine.Object) RuntimeUtilities.m_TransparentTexture == (UnityEngine.Object) null)
        {
          RuntimeUtilities.m_TransparentTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
          RuntimeUtilities.m_TransparentTexture.SetPixel(0, 0, Color.clear);
          RuntimeUtilities.m_TransparentTexture.Apply();
        }
        return RuntimeUtilities.m_TransparentTexture;
      }
    }

    public static Mesh fullscreenTriangle
    {
      get
      {
        if ((UnityEngine.Object) RuntimeUtilities.s_FullscreenTriangle != (UnityEngine.Object) null)
          return RuntimeUtilities.s_FullscreenTriangle;
        Mesh mesh = new Mesh();
        mesh.name = "Fullscreen Triangle";
        RuntimeUtilities.s_FullscreenTriangle = mesh;
        RuntimeUtilities.s_FullscreenTriangle.SetVertices(new List<Vector3>()
        {
          new Vector3(-1f, -1f, 0.0f),
          new Vector3(-1f, 3f, 0.0f),
          new Vector3(3f, -1f, 0.0f)
        });
        RuntimeUtilities.s_FullscreenTriangle.SetIndices(new int[3]
        {
          0,
          1,
          2
        }, MeshTopology.Triangles, 0, false);
        RuntimeUtilities.s_FullscreenTriangle.UploadMeshData(false);
        return RuntimeUtilities.s_FullscreenTriangle;
      }
    }

    public static Material copyStdMaterial
    {
      get
      {
        if ((UnityEngine.Object) RuntimeUtilities.s_CopyStdMaterial != (UnityEngine.Object) null)
          return RuntimeUtilities.s_CopyStdMaterial;
        Material material = new Material(Shader.Find("Hidden/PostProcessing/CopyStd"));
        material.name = "PostProcess - CopyStd";
        material.hideFlags = HideFlags.HideAndDontSave;
        RuntimeUtilities.s_CopyStdMaterial = material;
        return RuntimeUtilities.s_CopyStdMaterial;
      }
    }

    public static Material copyMaterial
    {
      get
      {
        if ((UnityEngine.Object) RuntimeUtilities.s_CopyMaterial != (UnityEngine.Object) null)
          return RuntimeUtilities.s_CopyMaterial;
        Material material = new Material(Shader.Find("Hidden/PostProcessing/Copy"));
        material.name = "PostProcess - Copy";
        material.hideFlags = HideFlags.HideAndDontSave;
        RuntimeUtilities.s_CopyMaterial = material;
        return RuntimeUtilities.s_CopyMaterial;
      }
    }

    public static PropertySheet copySheet
    {
      get
      {
        if (RuntimeUtilities.s_CopySheet == null)
          RuntimeUtilities.s_CopySheet = new PropertySheet(RuntimeUtilities.copyMaterial);
        return RuntimeUtilities.s_CopySheet;
      }
    }

    public static void BlitFullscreenTriangle(
      this CommandBuffer cmd,
      RenderTargetIdentifier source,
      RenderTargetIdentifier destination,
      bool clear = false)
    {
      cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
      cmd.SetRenderTarget(destination);
      if (clear)
        cmd.ClearRenderTarget(true, true, Color.clear);
      cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, RuntimeUtilities.copyMaterial, 0, 0);
    }

    public static void BlitFullscreenTriangle(
      this CommandBuffer cmd,
      RenderTargetIdentifier source,
      RenderTargetIdentifier destination,
      PropertySheet propertySheet,
      int pass,
      bool clear = false)
    {
      cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
      cmd.SetRenderTarget(destination);
      if (clear)
        cmd.ClearRenderTarget(true, true, Color.clear);
      cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
    }

    public static void BlitFullscreenTriangle(
      this CommandBuffer cmd,
      RenderTargetIdentifier source,
      RenderTargetIdentifier destination,
      RenderTargetIdentifier depth,
      PropertySheet propertySheet,
      int pass,
      bool clear = false)
    {
      cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
      cmd.SetRenderTarget(destination, depth);
      if (clear)
        cmd.ClearRenderTarget(true, true, Color.clear);
      cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
    }

    public static void BlitFullscreenTriangle(
      this CommandBuffer cmd,
      RenderTargetIdentifier source,
      RenderTargetIdentifier[] destinations,
      RenderTargetIdentifier depth,
      PropertySheet propertySheet,
      int pass,
      bool clear = false)
    {
      cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
      cmd.SetRenderTarget(destinations, depth);
      if (clear)
        cmd.ClearRenderTarget(true, true, Color.clear);
      cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
    }

    public static void BlitFullscreenTriangle(
      Texture source,
      RenderTexture destination,
      Material material,
      int pass)
    {
      RenderTexture active = RenderTexture.active;
      material.SetPass(pass);
      if ((UnityEngine.Object) source != (UnityEngine.Object) null)
        material.SetTexture(ShaderIDs.MainTex, source);
      Graphics.SetRenderTarget(destination);
      Graphics.DrawMeshNow(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity);
      RenderTexture.active = active;
    }

    public static void CopyTexture(
      CommandBuffer cmd,
      RenderTargetIdentifier source,
      RenderTargetIdentifier destination)
    {
      if (SystemInfo.copyTextureSupport > CopyTextureSupport.None)
        cmd.CopyTexture(source, destination);
      else
        cmd.BlitFullscreenTriangle(source, destination);
    }

    public static bool scriptableRenderPipelineActive => (UnityEngine.Object) GraphicsSettings.renderPipelineAsset != (UnityEngine.Object) null;

    public static bool isSinglePassStereoEnabled => true;

    public static bool isVREnabled => VRSettings.enabled;

    public static bool isAndroidOpenGL => Application.platform == RuntimePlatform.Android && SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan;

    public static void Destroy(UnityEngine.Object obj)
    {
      if (!(obj != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy(obj);
    }

    public static bool isLinearColorSpace => QualitySettings.activeColorSpace == ColorSpace.Linear;

    public static bool IsResolvedDepthAvailable(Camera camera)
    {
      GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
      if (camera.actualRenderingPath != RenderingPath.DeferredShading)
        return false;
      return graphicsDeviceType == GraphicsDeviceType.Direct3D11 || graphicsDeviceType == GraphicsDeviceType.Direct3D12 || graphicsDeviceType == GraphicsDeviceType.XboxOne;
    }

    public static void DestroyProfile(PostProcessProfile profile, bool destroyEffects)
    {
      if (destroyEffects)
      {
        foreach (UnityEngine.Object setting in profile.settings)
          RuntimeUtilities.Destroy(setting);
      }
      RuntimeUtilities.Destroy((UnityEngine.Object) profile);
    }

    public static void DestroyVolume(PostProcessVolume volume, bool destroySharedProfile)
    {
      if (destroySharedProfile)
        RuntimeUtilities.DestroyProfile(volume.sharedProfile, true);
      RuntimeUtilities.Destroy((UnityEngine.Object) volume);
    }

    [DebuggerHidden]
    public static IEnumerable<T> GetAllSceneObjects<T>() where T : Component
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      RuntimeUtilities.\u003CGetAllSceneObjects\u003Ec__Iterator0<T> objectsCIterator0_1 = new RuntimeUtilities.\u003CGetAllSceneObjects\u003Ec__Iterator0<T>();
      // ISSUE: variable of a compiler-generated type
      RuntimeUtilities.\u003CGetAllSceneObjects\u003Ec__Iterator0<T> objectsCIterator0_2 = objectsCIterator0_1;
      // ISSUE: reference to a compiler-generated field
      objectsCIterator0_2.\u0024PC = -2;
      return (IEnumerable<T>) objectsCIterator0_2;
    }

    public static void CreateIfNull<T>(ref T obj) where T : class, new()
    {
      if ((object) obj != null)
        return;
      obj = new T();
    }

    public static float Exp2(float x) => Mathf.Exp(x * 0.6931472f);

    public static Matrix4x4 GetJitteredPerspectiveProjectionMatrix(
      Camera camera,
      Vector2 offset)
    {
      float num1 = Mathf.Tan((float) Math.PI / 360f * camera.fieldOfView);
      float num2 = num1 * camera.aspect;
      float nearClipPlane = camera.nearClipPlane;
      float farClipPlane = camera.farClipPlane;
      offset.x *= num2 / (0.5f * (float) camera.pixelWidth);
      offset.y *= num1 / (0.5f * (float) camera.pixelHeight);
      float num3 = (offset.x - num2) * nearClipPlane;
      float num4 = (offset.x + num2) * nearClipPlane;
      float num5 = (offset.y + num1) * nearClipPlane;
      float num6 = (offset.y - num1) * nearClipPlane;
      return new Matrix4x4()
      {
        [0, 0] = (float) (2.0 * (double) nearClipPlane / ((double) num4 - (double) num3)),
        [0, 1] = 0.0f,
        [0, 2] = (float) (((double) num4 + (double) num3) / ((double) num4 - (double) num3)),
        [0, 3] = 0.0f,
        [1, 0] = 0.0f,
        [1, 1] = (float) (2.0 * (double) nearClipPlane / ((double) num5 - (double) num6)),
        [1, 2] = (float) (((double) num5 + (double) num6) / ((double) num5 - (double) num6)),
        [1, 3] = 0.0f,
        [2, 0] = 0.0f,
        [2, 1] = 0.0f,
        [2, 2] = (float) (-((double) farClipPlane + (double) nearClipPlane) / ((double) farClipPlane - (double) nearClipPlane)),
        [2, 3] = (float) (-(2.0 * (double) farClipPlane * (double) nearClipPlane) / ((double) farClipPlane - (double) nearClipPlane)),
        [3, 0] = 0.0f,
        [3, 1] = 0.0f,
        [3, 2] = -1f,
        [3, 3] = 0.0f
      };
    }

    public static Matrix4x4 GetJitteredOrthographicProjectionMatrix(
      Camera camera,
      Vector2 offset)
    {
      float orthographicSize = camera.orthographicSize;
      float num = orthographicSize * camera.aspect;
      offset.x *= num / (0.5f * (float) camera.pixelWidth);
      offset.y *= orthographicSize / (0.5f * (float) camera.pixelHeight);
      float left = offset.x - num;
      float right = offset.x + num;
      float top = offset.y + orthographicSize;
      float bottom = offset.y - orthographicSize;
      return Matrix4x4.Ortho(left, right, bottom, top, camera.nearClipPlane, camera.farClipPlane);
    }

    public static Matrix4x4 GenerateJitteredProjectionMatrixFromOriginal(
      PostProcessRenderContext context,
      Matrix4x4 origProj,
      Vector2 jitter)
    {
      float num1 = (1f + origProj[0, 2]) / origProj[0, 0];
      float num2 = (origProj[0, 2] - 1f) / origProj[0, 0];
      float num3 = (1f + origProj[1, 2]) / origProj[1, 1];
      float num4 = (origProj[1, 2] - 1f) / origProj[1, 1];
      float num5 = Math.Abs(num3) + Math.Abs(num4);
      float num6 = Math.Abs(num2) + Math.Abs(num1);
      jitter.x *= num6 / (float) context.xrSingleEyeWidth;
      jitter.y *= num5 / (float) context.height;
      float num7 = jitter.x + num2;
      float num8 = jitter.x + num1;
      float num9 = jitter.y + num3;
      float num10 = jitter.y + num4;
      return new Matrix4x4()
      {
        [0, 0] = (float) (2.0 / ((double) num8 - (double) num7)),
        [0, 1] = 0.0f,
        [0, 2] = (float) (((double) num8 + (double) num7) / ((double) num8 - (double) num7)),
        [0, 3] = 0.0f,
        [1, 0] = 0.0f,
        [1, 1] = (float) (2.0 / ((double) num9 - (double) num10)),
        [1, 2] = (float) (((double) num9 + (double) num10) / ((double) num9 - (double) num10)),
        [1, 3] = 0.0f,
        [2, 0] = 0.0f,
        [2, 1] = 0.0f,
        [2, 2] = origProj[2, 2],
        [2, 3] = origProj[2, 3],
        [3, 0] = 0.0f,
        [3, 1] = 0.0f,
        [3, 2] = -1f,
        [3, 3] = 0.0f
      };
    }

    public static T GetAttribute<T>(this System.Type type) where T : Attribute => (T) type.GetCustomAttributes(typeof (T), false)[0];

    public static Attribute[] GetMemberAttributes<TType, TValue>(
      Expression<Func<TType, TValue>> expr)
    {
      Expression expression = (Expression) expr;
      if (expression is LambdaExpression)
        expression = ((LambdaExpression) expression).Body;
      return expression.NodeType == ExpressionType.MemberAccess ? ((MemberExpression) expression).Member.GetCustomAttributes(false).Cast<Attribute>().ToArray<Attribute>() : throw new InvalidOperationException();
    }

    public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
    {
      if (expr.Body.NodeType != ExpressionType.MemberAccess)
        throw new InvalidOperationException();
      MemberExpression memberExpression = expr.Body as MemberExpression;
      List<string> stringList = new List<string>();
      for (; memberExpression != null; memberExpression = memberExpression.Expression as MemberExpression)
        stringList.Add(memberExpression.Member.Name);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = stringList.Count - 1; index >= 0; --index)
      {
        stringBuilder.Append(stringList[index]);
        if (index > 0)
          stringBuilder.Append('.');
      }
      return stringBuilder.ToString();
    }

    public static object GetParentObject(string path, object obj)
    {
      string[] strArray = path.Split('.');
      if (strArray.Length == 1)
        return obj;
      obj = obj.GetType().GetField(strArray[0], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(obj);
      return RuntimeUtilities.GetParentObject(string.Join(".", strArray, 1, strArray.Length - 1), obj);
    }
  }
}
