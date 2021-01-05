// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Utils
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Valve.VR
{
  public static class SteamVR_Utils
  {
    private const string secretKey = "foobar";
    private static Dictionary<int, GameObject> velocityCache = new Dictionary<int, GameObject>();

    public static Quaternion Slerp(Quaternion A, Quaternion B, float time)
    {
      float f1 = Mathf.Clamp((float) ((double) A.x * (double) B.x + (double) A.y * (double) B.y + (double) A.z * (double) B.z + (double) A.w * (double) B.w), -1f, 1f);
      if ((double) f1 < 0.0)
      {
        B = new Quaternion(-B.x, -B.y, -B.z, -B.w);
        f1 = -f1;
      }
      float num1;
      float num2;
      if (1.0 - (double) f1 > 9.99999974737875E-05)
      {
        float f2 = Mathf.Acos(f1);
        float num3 = Mathf.Sin(f2);
        num1 = Mathf.Sin((1f - time) * f2) / num3;
        num2 = Mathf.Sin(time * f2) / num3;
      }
      else
      {
        num1 = 1f - time;
        num2 = time;
      }
      return new Quaternion((float) ((double) num1 * (double) A.x + (double) num2 * (double) B.x), (float) ((double) num1 * (double) A.y + (double) num2 * (double) B.y), (float) ((double) num1 * (double) A.z + (double) num2 * (double) B.z), (float) ((double) num1 * (double) A.w + (double) num2 * (double) B.w));
    }

    public static Vector3 Lerp(Vector3 from, Vector3 to, float amount) => new Vector3(SteamVR_Utils.Lerp(from.x, to.x, amount), SteamVR_Utils.Lerp(from.y, to.y, amount), SteamVR_Utils.Lerp(from.z, to.z, amount));

    public static float Lerp(float from, float to, float amount) => from + (to - from) * amount;

    public static double Lerp(double from, double to, double amount) => from + (to - from) * amount;

    public static float InverseLerp(Vector3 from, Vector3 to, Vector3 result) => Vector3.Dot(result - from, to - from);

    public static float InverseLerp(float from, float to, float result) => (float) (((double) result - (double) from) / ((double) to - (double) from));

    public static double InverseLerp(double from, double to, double result) => (result - from) / (to - from);

    public static float Saturate(float A)
    {
      if ((double) A < 0.0)
        return 0.0f;
      return (double) A > 1.0 ? 1f : A;
    }

    public static Vector2 Saturate(Vector2 A) => new Vector2(SteamVR_Utils.Saturate(A.x), SteamVR_Utils.Saturate(A.y));

    public static Vector3 Saturate(Vector3 A) => new Vector3(SteamVR_Utils.Saturate(A.x), SteamVR_Utils.Saturate(A.y), SteamVR_Utils.Saturate(A.z));

    public static float Abs(float A) => (double) A < 0.0 ? -A : A;

    public static Vector2 Abs(Vector2 A) => new Vector2(SteamVR_Utils.Abs(A.x), SteamVR_Utils.Abs(A.y));

    public static Vector3 Abs(Vector3 A) => new Vector3(SteamVR_Utils.Abs(A.x), SteamVR_Utils.Abs(A.y), SteamVR_Utils.Abs(A.z));

    private static float _copysign(float sizeval, float signval) => (double) Mathf.Sign(signval) == 1.0 ? Mathf.Abs(sizeval) : -Mathf.Abs(sizeval);

    public static Quaternion GetRotation(this Matrix4x4 matrix)
    {
      Quaternion quaternion = new Quaternion()
      {
        w = Mathf.Sqrt(Mathf.Max(0.0f, 1f + matrix.m00 + matrix.m11 + matrix.m22)) / 2f,
        x = Mathf.Sqrt(Mathf.Max(0.0f, 1f + matrix.m00 - matrix.m11 - matrix.m22)) / 2f,
        y = Mathf.Sqrt(Mathf.Max(0.0f, 1f - matrix.m00 + matrix.m11 - matrix.m22)) / 2f,
        z = Mathf.Sqrt(Mathf.Max(0.0f, 1f - matrix.m00 - matrix.m11 + matrix.m22)) / 2f
      };
      quaternion.x = SteamVR_Utils._copysign(quaternion.x, matrix.m21 - matrix.m12);
      quaternion.y = SteamVR_Utils._copysign(quaternion.y, matrix.m02 - matrix.m20);
      quaternion.z = SteamVR_Utils._copysign(quaternion.z, matrix.m10 - matrix.m01);
      return quaternion;
    }

    public static Vector3 GetPosition(this Matrix4x4 matrix) => new Vector3(matrix.m03, matrix.m13, matrix.m23);

    public static Vector3 GetScale(this Matrix4x4 m) => new Vector3(Mathf.Sqrt((float) ((double) m.m00 * (double) m.m00 + (double) m.m01 * (double) m.m01 + (double) m.m02 * (double) m.m02)), Mathf.Sqrt((float) ((double) m.m10 * (double) m.m10 + (double) m.m11 * (double) m.m11 + (double) m.m12 * (double) m.m12)), Mathf.Sqrt((float) ((double) m.m20 * (double) m.m20 + (double) m.m21 * (double) m.m21 + (double) m.m22 * (double) m.m22)));

    public static Quaternion GetRotation(HmdMatrix34_t matrix) => ((double) matrix.m2 != 0.0 || (double) matrix.m6 != 0.0 || (double) matrix.m10 != 0.0) && ((double) matrix.m1 != 0.0 || (double) matrix.m5 != 0.0 || (double) matrix.m9 != 0.0) ? Quaternion.LookRotation(new Vector3(-matrix.m2, -matrix.m6, matrix.m10), new Vector3(matrix.m1, matrix.m5, -matrix.m9)) : Quaternion.identity;

    public static Vector3 GetPosition(HmdMatrix34_t matrix) => new Vector3(matrix.m3, matrix.m7, -matrix.m11);

    public static object CallSystemFn(SteamVR_Utils.SystemFn fn, params object[] args)
    {
      bool flag = !SteamVR.active && !SteamVR.usingNativeSupport;
      if (flag)
      {
        EVRInitError peError = EVRInitError.None;
        OpenVR.Init(ref peError, EVRApplicationType.VRApplication_Utility, string.Empty);
      }
      CVRSystem system = OpenVR.System;
      object obj = system == null ? (object) null : fn(system, args);
      if (flag)
        OpenVR.Shutdown();
      return obj;
    }

    public static void TakeStereoScreenshot(
      uint screenshotHandle,
      GameObject target,
      int cellSize,
      float ipd,
      ref string previewFilename,
      ref string VRFilename)
    {
      Texture2D texture2D1 = new Texture2D(4096, 4096, TextureFormat.ARGB32, false);
      Stopwatch stopwatch = new Stopwatch();
      Camera camera1 = (Camera) null;
      stopwatch.Start();
      Camera camera2 = target.GetComponent<Camera>();
      if ((UnityEngine.Object) camera2 == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) camera1 == (UnityEngine.Object) null)
          camera1 = new GameObject().AddComponent<Camera>();
        camera2 = camera1;
      }
      Texture2D texture2D2 = new Texture2D(2048, 2048, TextureFormat.ARGB32, false);
      RenderTexture renderTexture1 = new RenderTexture(2048, 2048, 24);
      RenderTexture targetTexture = camera2.targetTexture;
      bool orthographic = camera2.orthographic;
      float fieldOfView = camera2.fieldOfView;
      float aspect = camera2.aspect;
      StereoTargetEyeMask stereoTargetEye = camera2.stereoTargetEye;
      camera2.stereoTargetEye = StereoTargetEyeMask.None;
      camera2.fieldOfView = 60f;
      camera2.orthographic = false;
      camera2.targetTexture = renderTexture1;
      camera2.aspect = 1f;
      camera2.Render();
      RenderTexture.active = renderTexture1;
      texture2D2.ReadPixels(new Rect(0.0f, 0.0f, (float) renderTexture1.width, (float) renderTexture1.height), 0, 0);
      RenderTexture.active = (RenderTexture) null;
      camera2.targetTexture = (RenderTexture) null;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) renderTexture1);
      SteamVR_SphericalProjection sphericalProjection = camera2.gameObject.AddComponent<SteamVR_SphericalProjection>();
      Vector3 localPosition = target.transform.localPosition;
      Quaternion localRotation = target.transform.localRotation;
      Vector3 position = target.transform.position;
      Quaternion quaternion1 = Quaternion.Euler(0.0f, target.transform.rotation.eulerAngles.y, 0.0f);
      Transform transform = camera2.transform;
      int num1 = 1024 / cellSize;
      float num2 = 90f / (float) num1;
      float num3 = num2 / 2f;
      RenderTexture renderTexture2 = new RenderTexture(cellSize, cellSize, 24);
      renderTexture2.wrapMode = TextureWrapMode.Clamp;
      renderTexture2.antiAliasing = 8;
      camera2.fieldOfView = num2;
      camera2.orthographic = false;
      camera2.targetTexture = renderTexture2;
      camera2.aspect = aspect;
      camera2.stereoTargetEye = StereoTargetEyeMask.None;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        float x1 = (float) (90.0 - (double) index1 * (double) num2) - num3;
        int num4 = 4096 / renderTexture2.width;
        float num5 = 360f / (float) num4;
        float num6 = num5 / 2f;
        int num7 = index1 * 1024 / num1;
        for (int index2 = 0; index2 < 2; ++index2)
        {
          if (index2 == 1)
          {
            x1 = -x1;
            num7 = 2048 - num7 - cellSize;
          }
          for (int index3 = 0; index3 < num4; ++index3)
          {
            float y1 = (float) ((double) index3 * (double) num5 - 180.0) + num6;
            int destX = index3 * 4096 / num4;
            int num8 = 0;
            float x2 = (float) (-(double) ipd / 2.0) * Mathf.Cos(x1 * ((float) Math.PI / 180f));
            for (int index4 = 0; index4 < 2; ++index4)
            {
              if (index4 == 1)
              {
                num8 = 2048;
                x2 = -x2;
              }
              Vector3 vector3_1 = quaternion1 * Quaternion.Euler(0.0f, y1, 0.0f) * new Vector3(x2, 0.0f, 0.0f);
              transform.position = position + vector3_1;
              Quaternion quaternion2 = Quaternion.Euler(x1, y1, 0.0f);
              transform.rotation = quaternion1 * quaternion2;
              Vector3 vector3_2 = quaternion2 * Vector3.forward;
              float num9 = y1 - num5 / 2f;
              float num10 = num9 + num5;
              float num11 = x1 + num2 / 2f;
              float num12 = num11 - num2;
              float y2 = (float) (((double) num9 + (double) num10) / 2.0);
              float x3 = (double) Mathf.Abs(num11) >= (double) Mathf.Abs(num12) ? num12 : num11;
              Vector3 lhs1 = Quaternion.Euler(x3, num9, 0.0f) * Vector3.forward;
              Vector3 lhs2 = Quaternion.Euler(x3, num10, 0.0f) * Vector3.forward;
              Vector3 lhs3 = Quaternion.Euler(num11, y2, 0.0f) * Vector3.forward;
              Vector3 lhs4 = Quaternion.Euler(num12, y2, 0.0f) * Vector3.forward;
              Vector3 uOrigin = lhs1 / Vector3.Dot(lhs1, vector3_2);
              Vector3 vector3_3 = lhs2 / Vector3.Dot(lhs2, vector3_2);
              Vector3 vOrigin = lhs3 / Vector3.Dot(lhs3, vector3_2);
              Vector3 vector3_4 = lhs4 / Vector3.Dot(lhs4, vector3_2);
              Vector3 vector3_5 = vector3_3 - uOrigin;
              Vector3 vector3_6 = vector3_4 - vOrigin;
              float magnitude1 = vector3_5.magnitude;
              float magnitude2 = vector3_6.magnitude;
              float uScale = 1f / magnitude1;
              float vScale = 1f / magnitude2;
              Vector3 uAxis = vector3_5 * uScale;
              Vector3 vAxis = vector3_6 * vScale;
              sphericalProjection.Set(vector3_2, num9, num10, num11, num12, uAxis, uOrigin, uScale, vAxis, vOrigin, vScale);
              camera2.aspect = magnitude1 / magnitude2;
              camera2.Render();
              RenderTexture.active = renderTexture2;
              texture2D1.ReadPixels(new Rect(0.0f, 0.0f, (float) renderTexture2.width, (float) renderTexture2.height), destX, num7 + num8);
              RenderTexture.active = (RenderTexture) null;
            }
            float flProgress = ((float) index1 * ((float) num4 * 2f) + (float) index3 + (float) (index2 * num4)) / ((float) num1 * ((float) num4 * 2f));
            int num13 = (int) OpenVR.Screenshots.UpdateScreenshotProgress(screenshotHandle, flProgress);
          }
        }
      }
      int num14 = (int) OpenVR.Screenshots.UpdateScreenshotProgress(screenshotHandle, 1f);
      previewFilename += ".png";
      VRFilename += ".png";
      texture2D2.Apply();
      File.WriteAllBytes(previewFilename, texture2D2.EncodeToPNG());
      texture2D1.Apply();
      File.WriteAllBytes(VRFilename, texture2D1.EncodeToPNG());
      if ((UnityEngine.Object) camera2 != (UnityEngine.Object) camera1)
      {
        camera2.targetTexture = targetTexture;
        camera2.orthographic = orthographic;
        camera2.fieldOfView = fieldOfView;
        camera2.aspect = aspect;
        camera2.stereoTargetEye = stereoTargetEye;
        target.transform.localPosition = localPosition;
        target.transform.localRotation = localRotation;
      }
      else
        camera1.targetTexture = (RenderTexture) null;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) renderTexture2);
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) sphericalProjection);
      stopwatch.Stop();
      UnityEngine.Debug.Log((object) string.Format("<b>[SteamVR]</b> Screenshot took {0} seconds.", (object) stopwatch.Elapsed));
      if ((UnityEngine.Object) camera1 != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) camera1.gameObject);
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) texture2D2);
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) texture2D1);
    }

    public static string GetBadMD5Hash(string usedString) => SteamVR_Utils.GetBadMD5Hash(Encoding.UTF8.GetBytes(usedString + "foobar"));

    public static string GetBadMD5Hash(byte[] bytes)
    {
      byte[] hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < hash.Length; ++index)
        stringBuilder.Append(hash[index].ToString("x2"));
      return stringBuilder.ToString();
    }

    public static string GetBadMD5HashFromFile(string filePath) => !File.Exists(filePath) ? (string) null : SteamVR_Utils.GetBadMD5Hash(File.ReadAllText(filePath) + "foobar");

    public static string ConvertToForwardSlashes(string fromString) => fromString.Replace("\\\\", "\\").Replace("\\", "/");

    public static float GetLossyScale(Transform forTransform)
    {
      float num = 1f;
      while ((UnityEngine.Object) forTransform != (UnityEngine.Object) null && (UnityEngine.Object) forTransform.parent != (UnityEngine.Object) null)
      {
        forTransform = forTransform.parent;
        num *= forTransform.localScale.x;
      }
      return num;
    }

    public static bool IsValid(Vector3 vector) => !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z);

    public static bool IsValid(Quaternion rotation)
    {
      if (float.IsNaN(rotation.x) || float.IsNaN(rotation.y) || (float.IsNaN(rotation.z) || float.IsNaN(rotation.w)))
        return false;
      return (double) rotation.x != 0.0 || (double) rotation.y != 0.0 || (double) rotation.z != 0.0 || (double) rotation.w != 0.0;
    }

    public static void DrawVelocity(
      int key,
      Vector3 position,
      Vector3 velocity,
      float destroyAfterSeconds = 5f)
    {
      SteamVR_Utils.DrawVelocity(key, position, velocity, Color.green, destroyAfterSeconds);
    }

    public static void DrawVelocity(
      int key,
      Vector3 position,
      Vector3 velocity,
      Color color,
      float destroyAfterSeconds = 5f)
    {
      if (!SteamVR_Utils.velocityCache.ContainsKey(key) || (UnityEngine.Object) SteamVR_Utils.velocityCache[key] == (UnityEngine.Object) null)
      {
        GameObject primitive1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        primitive1.transform.localScale = Vector3.one * 0.025f;
        primitive1.transform.position = position;
        if (velocity != Vector3.zero)
          primitive1.transform.forward = velocity;
        GameObject primitive2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        primitive2.transform.parent = primitive1.transform;
        if (velocity != Vector3.zero)
        {
          primitive2.transform.localScale = new Vector3(0.25f, 0.25f, (float) (3.0 + (double) velocity.magnitude * 1.5));
          primitive2.transform.localPosition = new Vector3(0.0f, 0.0f, primitive2.transform.localScale.z / 2f);
        }
        else
        {
          primitive2.transform.localScale = Vector3.one;
          primitive2.transform.localPosition = Vector3.zero;
        }
        primitive2.transform.localRotation = Quaternion.identity;
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) primitive2.GetComponent<Collider>());
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) primitive1.GetComponent<Collider>());
        primitive1.GetComponent<MeshRenderer>().material.color = color;
        primitive2.GetComponent<MeshRenderer>().material.color = color;
        SteamVR_Utils.velocityCache[key] = primitive1;
        UnityEngine.Object.Destroy((UnityEngine.Object) primitive1, destroyAfterSeconds);
      }
      else
      {
        GameObject gameObject = SteamVR_Utils.velocityCache[key];
        gameObject.transform.position = position;
        if (velocity != Vector3.zero)
          gameObject.transform.forward = velocity;
        Transform child = gameObject.transform.GetChild(0);
        if (velocity != Vector3.zero)
        {
          child.localScale = new Vector3(0.25f, 0.25f, (float) (3.0 + (double) velocity.magnitude * 1.5));
          child.localPosition = new Vector3(0.0f, 0.0f, child.transform.localScale.z / 2f);
        }
        else
        {
          child.localScale = Vector3.one;
          child.localPosition = Vector3.zero;
        }
        child.localRotation = Quaternion.identity;
        UnityEngine.Object.Destroy((UnityEngine.Object) gameObject, destroyAfterSeconds);
      }
    }

    [Serializable]
    public struct RigidTransform
    {
      public Vector3 pos;
      public Quaternion rot;

      public RigidTransform(Vector3 position, Quaternion rotation)
      {
        this.pos = position;
        this.rot = rotation;
      }

      public RigidTransform(Transform fromTransform)
      {
        this.pos = fromTransform.position;
        this.rot = fromTransform.rotation;
      }

      public RigidTransform(Transform from, Transform to)
      {
        Quaternion quaternion = Quaternion.Inverse(from.rotation);
        this.rot = quaternion * to.rotation;
        this.pos = quaternion * (to.position - from.position);
      }

      public RigidTransform(HmdMatrix34_t pose)
      {
        Matrix4x4 identity = Matrix4x4.identity;
        identity[0, 0] = pose.m0;
        identity[0, 1] = pose.m1;
        identity[0, 2] = -pose.m2;
        identity[0, 3] = pose.m3;
        identity[1, 0] = pose.m4;
        identity[1, 1] = pose.m5;
        identity[1, 2] = -pose.m6;
        identity[1, 3] = pose.m7;
        identity[2, 0] = -pose.m8;
        identity[2, 1] = -pose.m9;
        identity[2, 2] = pose.m10;
        identity[2, 3] = -pose.m11;
        this.pos = identity.GetPosition();
        this.rot = identity.GetRotation();
      }

      public RigidTransform(HmdMatrix44_t pose)
      {
        Matrix4x4 identity = Matrix4x4.identity;
        identity[0, 0] = pose.m0;
        identity[0, 1] = pose.m1;
        identity[0, 2] = -pose.m2;
        identity[0, 3] = pose.m3;
        identity[1, 0] = pose.m4;
        identity[1, 1] = pose.m5;
        identity[1, 2] = -pose.m6;
        identity[1, 3] = pose.m7;
        identity[2, 0] = -pose.m8;
        identity[2, 1] = -pose.m9;
        identity[2, 2] = pose.m10;
        identity[2, 3] = -pose.m11;
        identity[3, 0] = pose.m12;
        identity[3, 1] = pose.m13;
        identity[3, 2] = -pose.m14;
        identity[3, 3] = pose.m15;
        this.pos = identity.GetPosition();
        this.rot = identity.GetRotation();
      }

      public static SteamVR_Utils.RigidTransform identity => new SteamVR_Utils.RigidTransform(Vector3.zero, Quaternion.identity);

      public static SteamVR_Utils.RigidTransform FromLocal(Transform fromTransform) => new SteamVR_Utils.RigidTransform(fromTransform.localPosition, fromTransform.localRotation);

      public HmdMatrix44_t ToHmdMatrix44()
      {
        Matrix4x4 matrix4x4 = Matrix4x4.TRS(this.pos, this.rot, Vector3.one);
        return new HmdMatrix44_t()
        {
          m0 = matrix4x4[0, 0],
          m1 = matrix4x4[0, 1],
          m2 = -matrix4x4[0, 2],
          m3 = matrix4x4[0, 3],
          m4 = matrix4x4[1, 0],
          m5 = matrix4x4[1, 1],
          m6 = -matrix4x4[1, 2],
          m7 = matrix4x4[1, 3],
          m8 = -matrix4x4[2, 0],
          m9 = -matrix4x4[2, 1],
          m10 = matrix4x4[2, 2],
          m11 = -matrix4x4[2, 3],
          m12 = matrix4x4[3, 0],
          m13 = matrix4x4[3, 1],
          m14 = -matrix4x4[3, 2],
          m15 = matrix4x4[3, 3]
        };
      }

      public HmdMatrix34_t ToHmdMatrix34()
      {
        Matrix4x4 matrix4x4 = Matrix4x4.TRS(this.pos, this.rot, Vector3.one);
        return new HmdMatrix34_t()
        {
          m0 = matrix4x4[0, 0],
          m1 = matrix4x4[0, 1],
          m2 = -matrix4x4[0, 2],
          m3 = matrix4x4[0, 3],
          m4 = matrix4x4[1, 0],
          m5 = matrix4x4[1, 1],
          m6 = -matrix4x4[1, 2],
          m7 = matrix4x4[1, 3],
          m8 = -matrix4x4[2, 0],
          m9 = -matrix4x4[2, 1],
          m10 = matrix4x4[2, 2],
          m11 = -matrix4x4[2, 3]
        };
      }

      public override bool Equals(object other) => other is SteamVR_Utils.RigidTransform rigidTransform && this.pos == rigidTransform.pos && this.rot == rigidTransform.rot;

      public override int GetHashCode() => this.pos.GetHashCode() ^ this.rot.GetHashCode();

      public static bool operator ==(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b) => a.pos == b.pos && a.rot == b.rot;

      public static bool operator !=(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b) => a.pos != b.pos || a.rot != b.rot;

      public static SteamVR_Utils.RigidTransform operator *(
        SteamVR_Utils.RigidTransform a,
        SteamVR_Utils.RigidTransform b)
      {
        return new SteamVR_Utils.RigidTransform()
        {
          rot = a.rot * b.rot,
          pos = a.pos + a.rot * b.pos
        };
      }

      public void Inverse()
      {
        this.rot = Quaternion.Inverse(this.rot);
        this.pos = -(this.rot * this.pos);
      }

      public SteamVR_Utils.RigidTransform GetInverse()
      {
        SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(this.pos, this.rot);
        rigidTransform.Inverse();
        return rigidTransform;
      }

      public void Multiply(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b)
      {
        this.rot = a.rot * b.rot;
        this.pos = a.pos + a.rot * b.pos;
      }

      public Vector3 InverseTransformPoint(Vector3 point) => Quaternion.Inverse(this.rot) * (point - this.pos);

      public Vector3 TransformPoint(Vector3 point) => this.pos + this.rot * point;

      public static Vector3 operator *(SteamVR_Utils.RigidTransform t, Vector3 v) => t.TransformPoint(v);

      public static SteamVR_Utils.RigidTransform Interpolate(
        SteamVR_Utils.RigidTransform a,
        SteamVR_Utils.RigidTransform b,
        float t)
      {
        return new SteamVR_Utils.RigidTransform(Vector3.Lerp(a.pos, b.pos, t), Quaternion.Slerp(a.rot, b.rot, t));
      }

      public void Interpolate(SteamVR_Utils.RigidTransform to, float t)
      {
        this.pos = SteamVR_Utils.Lerp(this.pos, to.pos, t);
        this.rot = SteamVR_Utils.Slerp(this.rot, to.rot, t);
      }
    }

    public delegate object SystemFn(CVRSystem system, params object[] args);
  }
}
