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
		[Serializable]
		public struct RigidTransform
		{
			public Vector3 pos;

			public Quaternion rot;

			public static RigidTransform identity => new RigidTransform(Vector3.zero, Quaternion.identity);

			public RigidTransform(Vector3 position, Quaternion rotation)
			{
				pos = position;
				rot = rotation;
			}

			public RigidTransform(Transform fromTransform)
			{
				pos = fromTransform.position;
				rot = fromTransform.rotation;
			}

			public RigidTransform(Transform from, Transform to)
			{
				Quaternion quaternion = Quaternion.Inverse(from.rotation);
				rot = quaternion * to.rotation;
				pos = quaternion * (to.position - from.position);
			}

			public RigidTransform(HmdMatrix34_t pose)
			{
				Matrix4x4 identity = Matrix4x4.identity;
				identity[0, 0] = pose.m0;
				identity[0, 1] = pose.m1;
				identity[0, 2] = 0f - pose.m2;
				identity[0, 3] = pose.m3;
				identity[1, 0] = pose.m4;
				identity[1, 1] = pose.m5;
				identity[1, 2] = 0f - pose.m6;
				identity[1, 3] = pose.m7;
				identity[2, 0] = 0f - pose.m8;
				identity[2, 1] = 0f - pose.m9;
				identity[2, 2] = pose.m10;
				identity[2, 3] = 0f - pose.m11;
				pos = identity.GetPosition();
				rot = identity.GetRotation();
			}

			public RigidTransform(HmdMatrix44_t pose)
			{
				Matrix4x4 identity = Matrix4x4.identity;
				identity[0, 0] = pose.m0;
				identity[0, 1] = pose.m1;
				identity[0, 2] = 0f - pose.m2;
				identity[0, 3] = pose.m3;
				identity[1, 0] = pose.m4;
				identity[1, 1] = pose.m5;
				identity[1, 2] = 0f - pose.m6;
				identity[1, 3] = pose.m7;
				identity[2, 0] = 0f - pose.m8;
				identity[2, 1] = 0f - pose.m9;
				identity[2, 2] = pose.m10;
				identity[2, 3] = 0f - pose.m11;
				identity[3, 0] = pose.m12;
				identity[3, 1] = pose.m13;
				identity[3, 2] = 0f - pose.m14;
				identity[3, 3] = pose.m15;
				pos = identity.GetPosition();
				rot = identity.GetRotation();
			}

			public static RigidTransform FromLocal(Transform fromTransform)
			{
				return new RigidTransform(fromTransform.localPosition, fromTransform.localRotation);
			}

			public HmdMatrix44_t ToHmdMatrix44()
			{
				Matrix4x4 matrix4x = Matrix4x4.TRS(pos, rot, Vector3.one);
				HmdMatrix44_t result = default(HmdMatrix44_t);
				result.m0 = matrix4x[0, 0];
				result.m1 = matrix4x[0, 1];
				result.m2 = 0f - matrix4x[0, 2];
				result.m3 = matrix4x[0, 3];
				result.m4 = matrix4x[1, 0];
				result.m5 = matrix4x[1, 1];
				result.m6 = 0f - matrix4x[1, 2];
				result.m7 = matrix4x[1, 3];
				result.m8 = 0f - matrix4x[2, 0];
				result.m9 = 0f - matrix4x[2, 1];
				result.m10 = matrix4x[2, 2];
				result.m11 = 0f - matrix4x[2, 3];
				result.m12 = matrix4x[3, 0];
				result.m13 = matrix4x[3, 1];
				result.m14 = 0f - matrix4x[3, 2];
				result.m15 = matrix4x[3, 3];
				return result;
			}

			public HmdMatrix34_t ToHmdMatrix34()
			{
				Matrix4x4 matrix4x = Matrix4x4.TRS(pos, rot, Vector3.one);
				HmdMatrix34_t result = default(HmdMatrix34_t);
				result.m0 = matrix4x[0, 0];
				result.m1 = matrix4x[0, 1];
				result.m2 = 0f - matrix4x[0, 2];
				result.m3 = matrix4x[0, 3];
				result.m4 = matrix4x[1, 0];
				result.m5 = matrix4x[1, 1];
				result.m6 = 0f - matrix4x[1, 2];
				result.m7 = matrix4x[1, 3];
				result.m8 = 0f - matrix4x[2, 0];
				result.m9 = 0f - matrix4x[2, 1];
				result.m10 = matrix4x[2, 2];
				result.m11 = 0f - matrix4x[2, 3];
				return result;
			}

			public override bool Equals(object other)
			{
				if (other is RigidTransform)
				{
					RigidTransform rigidTransform = (RigidTransform)other;
					return pos == rigidTransform.pos && rot == rigidTransform.rot;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return pos.GetHashCode() ^ rot.GetHashCode();
			}

			public static bool operator ==(RigidTransform a, RigidTransform b)
			{
				return a.pos == b.pos && a.rot == b.rot;
			}

			public static bool operator !=(RigidTransform a, RigidTransform b)
			{
				return a.pos != b.pos || a.rot != b.rot;
			}

			public static RigidTransform operator *(RigidTransform a, RigidTransform b)
			{
				RigidTransform result = default(RigidTransform);
				result.rot = a.rot * b.rot;
				result.pos = a.pos + a.rot * b.pos;
				return result;
			}

			public void Inverse()
			{
				rot = Quaternion.Inverse(rot);
				pos = -(rot * pos);
			}

			public RigidTransform GetInverse()
			{
				RigidTransform result = new RigidTransform(pos, rot);
				result.Inverse();
				return result;
			}

			public void Multiply(RigidTransform a, RigidTransform b)
			{
				rot = a.rot * b.rot;
				pos = a.pos + a.rot * b.pos;
			}

			public Vector3 InverseTransformPoint(Vector3 point)
			{
				return Quaternion.Inverse(rot) * (point - pos);
			}

			public Vector3 TransformPoint(Vector3 point)
			{
				return pos + rot * point;
			}

			public static Vector3 operator *(RigidTransform t, Vector3 v)
			{
				return t.TransformPoint(v);
			}

			public static RigidTransform Interpolate(RigidTransform a, RigidTransform b, float t)
			{
				return new RigidTransform(Vector3.Lerp(a.pos, b.pos, t), Quaternion.Slerp(a.rot, b.rot, t));
			}

			public void Interpolate(RigidTransform to, float t)
			{
				pos = Lerp(pos, to.pos, t);
				rot = Slerp(rot, to.rot, t);
			}
		}

		public delegate object SystemFn(CVRSystem system, params object[] args);

		private const string secretKey = "foobar";

		private static Dictionary<int, GameObject> velocityCache = new Dictionary<int, GameObject>();

		public static Quaternion Slerp(Quaternion A, Quaternion B, float time)
		{
			float num = Mathf.Clamp(A.x * B.x + A.y * B.y + A.z * B.z + A.w * B.w, -1f, 1f);
			if (num < 0f)
			{
				B = new Quaternion(0f - B.x, 0f - B.y, 0f - B.z, 0f - B.w);
				num = 0f - num;
			}
			float num4;
			float num5;
			if (1f - num > 0.0001f)
			{
				float num2 = Mathf.Acos(num);
				float num3 = Mathf.Sin(num2);
				num4 = Mathf.Sin((1f - time) * num2) / num3;
				num5 = Mathf.Sin(time * num2) / num3;
			}
			else
			{
				num4 = 1f - time;
				num5 = time;
			}
			return new Quaternion(num4 * A.x + num5 * B.x, num4 * A.y + num5 * B.y, num4 * A.z + num5 * B.z, num4 * A.w + num5 * B.w);
		}

		public static Vector3 Lerp(Vector3 from, Vector3 to, float amount)
		{
			return new Vector3(Lerp(from.x, to.x, amount), Lerp(from.y, to.y, amount), Lerp(from.z, to.z, amount));
		}

		public static float Lerp(float from, float to, float amount)
		{
			return from + (to - from) * amount;
		}

		public static double Lerp(double from, double to, double amount)
		{
			return from + (to - from) * amount;
		}

		public static float InverseLerp(Vector3 from, Vector3 to, Vector3 result)
		{
			return Vector3.Dot(result - from, to - from);
		}

		public static float InverseLerp(float from, float to, float result)
		{
			return (result - from) / (to - from);
		}

		public static double InverseLerp(double from, double to, double result)
		{
			return (result - from) / (to - from);
		}

		public static float Saturate(float A)
		{
			return (A < 0f) ? 0f : ((!(A > 1f)) ? A : 1f);
		}

		public static Vector2 Saturate(Vector2 A)
		{
			return new Vector2(Saturate(A.x), Saturate(A.y));
		}

		public static Vector3 Saturate(Vector3 A)
		{
			return new Vector3(Saturate(A.x), Saturate(A.y), Saturate(A.z));
		}

		public static float Abs(float A)
		{
			return (!(A < 0f)) ? A : (0f - A);
		}

		public static Vector2 Abs(Vector2 A)
		{
			return new Vector2(Abs(A.x), Abs(A.y));
		}

		public static Vector3 Abs(Vector3 A)
		{
			return new Vector3(Abs(A.x), Abs(A.y), Abs(A.z));
		}

		private static float _copysign(float sizeval, float signval)
		{
			return (Mathf.Sign(signval) != 1f) ? (0f - Mathf.Abs(sizeval)) : Mathf.Abs(sizeval);
		}

		public static Quaternion GetRotation(this Matrix4x4 matrix)
		{
			Quaternion result = default(Quaternion);
			result.w = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 + matrix.m11 + matrix.m22)) / 2f;
			result.x = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 - matrix.m11 - matrix.m22)) / 2f;
			result.y = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 + matrix.m11 - matrix.m22)) / 2f;
			result.z = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 - matrix.m11 + matrix.m22)) / 2f;
			result.x = _copysign(result.x, matrix.m21 - matrix.m12);
			result.y = _copysign(result.y, matrix.m02 - matrix.m20);
			result.z = _copysign(result.z, matrix.m10 - matrix.m01);
			return result;
		}

		public static Vector3 GetPosition(this Matrix4x4 matrix)
		{
			float m = matrix.m03;
			float m2 = matrix.m13;
			float m3 = matrix.m23;
			return new Vector3(m, m2, m3);
		}

		public static Vector3 GetScale(this Matrix4x4 m)
		{
			float x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
			float y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
			float z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);
			return new Vector3(x, y, z);
		}

		public static Quaternion GetRotation(HmdMatrix34_t matrix)
		{
			if ((matrix.m2 != 0f || matrix.m6 != 0f || matrix.m10 != 0f) && (matrix.m1 != 0f || matrix.m5 != 0f || matrix.m9 != 0f))
			{
				return Quaternion.LookRotation(new Vector3(0f - matrix.m2, 0f - matrix.m6, matrix.m10), new Vector3(matrix.m1, matrix.m5, 0f - matrix.m9));
			}
			return Quaternion.identity;
		}

		public static Vector3 GetPosition(HmdMatrix34_t matrix)
		{
			return new Vector3(matrix.m3, matrix.m7, 0f - matrix.m11);
		}

		public static object CallSystemFn(SystemFn fn, params object[] args)
		{
			bool flag = !SteamVR.active && !SteamVR.usingNativeSupport;
			if (flag)
			{
				EVRInitError peError = EVRInitError.None;
				OpenVR.Init(ref peError, EVRApplicationType.VRApplication_Utility, string.Empty);
			}
			CVRSystem system = OpenVR.System;
			object result = ((system == null) ? null : fn(system, args));
			if (flag)
			{
				OpenVR.Shutdown();
			}
			return result;
		}

		public static void TakeStereoScreenshot(uint screenshotHandle, GameObject target, int cellSize, float ipd, ref string previewFilename, ref string VRFilename)
		{
			Texture2D texture2D = new Texture2D(4096, 4096, TextureFormat.ARGB32, mipmap: false);
			Stopwatch stopwatch = new Stopwatch();
			Camera camera = null;
			stopwatch.Start();
			Camera camera2 = target.GetComponent<Camera>();
			if (camera2 == null)
			{
				if (camera == null)
				{
					camera = new GameObject().AddComponent<Camera>();
				}
				camera2 = camera;
			}
			Texture2D texture2D2 = new Texture2D(2048, 2048, TextureFormat.ARGB32, mipmap: false);
			RenderTexture renderTexture = new RenderTexture(2048, 2048, 24);
			RenderTexture targetTexture = camera2.targetTexture;
			bool orthographic = camera2.orthographic;
			float fieldOfView = camera2.fieldOfView;
			float aspect = camera2.aspect;
			StereoTargetEyeMask stereoTargetEye = camera2.stereoTargetEye;
			camera2.stereoTargetEye = StereoTargetEyeMask.None;
			camera2.fieldOfView = 60f;
			camera2.orthographic = false;
			camera2.targetTexture = renderTexture;
			camera2.aspect = 1f;
			camera2.Render();
			RenderTexture.active = renderTexture;
			texture2D2.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
			RenderTexture.active = null;
			camera2.targetTexture = null;
			UnityEngine.Object.DestroyImmediate(renderTexture);
			SteamVR_SphericalProjection steamVR_SphericalProjection = camera2.gameObject.AddComponent<SteamVR_SphericalProjection>();
			Vector3 localPosition = target.transform.localPosition;
			Quaternion localRotation = target.transform.localRotation;
			Vector3 position = target.transform.position;
			Quaternion quaternion = Quaternion.Euler(0f, target.transform.rotation.eulerAngles.y, 0f);
			Transform transform = camera2.transform;
			int num = 1024 / cellSize;
			float num2 = 90f / (float)num;
			float num3 = num2 / 2f;
			RenderTexture renderTexture2 = new RenderTexture(cellSize, cellSize, 24);
			renderTexture2.wrapMode = TextureWrapMode.Clamp;
			renderTexture2.antiAliasing = 8;
			camera2.fieldOfView = num2;
			camera2.orthographic = false;
			camera2.targetTexture = renderTexture2;
			camera2.aspect = aspect;
			camera2.stereoTargetEye = StereoTargetEyeMask.None;
			for (int i = 0; i < num; i++)
			{
				float num4 = 90f - (float)i * num2 - num3;
				int num5 = 4096 / renderTexture2.width;
				float num6 = 360f / (float)num5;
				float num7 = num6 / 2f;
				int num8 = i * 1024 / num;
				for (int j = 0; j < 2; j++)
				{
					if (j == 1)
					{
						num4 = 0f - num4;
						num8 = 2048 - num8 - cellSize;
					}
					for (int k = 0; k < num5; k++)
					{
						float num9 = -180f + (float)k * num6 + num7;
						int destX = k * 4096 / num5;
						int num10 = 0;
						float num11 = (0f - ipd) / 2f * Mathf.Cos(num4 * ((float)Math.PI / 180f));
						for (int l = 0; l < 2; l++)
						{
							if (l == 1)
							{
								num10 = 2048;
								num11 = 0f - num11;
							}
							Vector3 vector = quaternion * Quaternion.Euler(0f, num9, 0f) * new Vector3(num11, 0f, 0f);
							transform.position = position + vector;
							Quaternion quaternion2 = Quaternion.Euler(num4, num9, 0f);
							transform.rotation = quaternion * quaternion2;
							Vector3 vector2 = quaternion2 * Vector3.forward;
							float num12 = num9 - num6 / 2f;
							float num13 = num12 + num6;
							float num14 = num4 + num2 / 2f;
							float num15 = num14 - num2;
							float y = (num12 + num13) / 2f;
							float x = ((!(Mathf.Abs(num14) < Mathf.Abs(num15))) ? num15 : num14);
							Vector3 vector3 = Quaternion.Euler(x, num12, 0f) * Vector3.forward;
							Vector3 vector4 = Quaternion.Euler(x, num13, 0f) * Vector3.forward;
							Vector3 vector5 = Quaternion.Euler(num14, y, 0f) * Vector3.forward;
							Vector3 vector6 = Quaternion.Euler(num15, y, 0f) * Vector3.forward;
							Vector3 vector7 = vector3 / Vector3.Dot(vector3, vector2);
							Vector3 vector8 = vector4 / Vector3.Dot(vector4, vector2);
							Vector3 vector9 = vector5 / Vector3.Dot(vector5, vector2);
							Vector3 vector10 = vector6 / Vector3.Dot(vector6, vector2);
							Vector3 vector11 = vector8 - vector7;
							Vector3 vector12 = vector10 - vector9;
							float magnitude = vector11.magnitude;
							float magnitude2 = vector12.magnitude;
							float num16 = 1f / magnitude;
							float num17 = 1f / magnitude2;
							Vector3 uAxis = vector11 * num16;
							Vector3 vAxis = vector12 * num17;
							steamVR_SphericalProjection.Set(vector2, num12, num13, num14, num15, uAxis, vector7, num16, vAxis, vector9, num17);
							camera2.aspect = magnitude / magnitude2;
							camera2.Render();
							RenderTexture.active = renderTexture2;
							texture2D.ReadPixels(new Rect(0f, 0f, renderTexture2.width, renderTexture2.height), destX, num8 + num10);
							RenderTexture.active = null;
						}
						float flProgress = ((float)i * ((float)num5 * 2f) + (float)k + (float)(j * num5)) / ((float)num * ((float)num5 * 2f));
						OpenVR.Screenshots.UpdateScreenshotProgress(screenshotHandle, flProgress);
					}
				}
			}
			OpenVR.Screenshots.UpdateScreenshotProgress(screenshotHandle, 1f);
			previewFilename += ".png";
			VRFilename += ".png";
			texture2D2.Apply();
			File.WriteAllBytes(previewFilename, texture2D2.EncodeToPNG());
			texture2D.Apply();
			File.WriteAllBytes(VRFilename, texture2D.EncodeToPNG());
			if (camera2 != camera)
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
			{
				camera.targetTexture = null;
			}
			UnityEngine.Object.DestroyImmediate(renderTexture2);
			UnityEngine.Object.DestroyImmediate(steamVR_SphericalProjection);
			stopwatch.Stop();
			UnityEngine.Debug.Log($"<b>[SteamVR]</b> Screenshot took {stopwatch.Elapsed} seconds.");
			if (camera != null)
			{
				UnityEngine.Object.DestroyImmediate(camera.gameObject);
			}
			UnityEngine.Object.DestroyImmediate(texture2D2);
			UnityEngine.Object.DestroyImmediate(texture2D);
		}

		public static string GetBadMD5Hash(string usedString)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(usedString + "foobar");
			return GetBadMD5Hash(bytes);
		}

		public static string GetBadMD5Hash(byte[] bytes)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		public static string GetBadMD5HashFromFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return null;
			}
			string text = File.ReadAllText(filePath);
			return GetBadMD5Hash(text + "foobar");
		}

		public static string ConvertToForwardSlashes(string fromString)
		{
			string text = fromString.Replace("\\\\", "\\");
			return text.Replace("\\", "/");
		}

		public static float GetLossyScale(Transform forTransform)
		{
			float num = 1f;
			while (forTransform != null && forTransform.parent != null)
			{
				forTransform = forTransform.parent;
				num *= forTransform.localScale.x;
			}
			return num;
		}

		public static bool IsValid(Vector3 vector)
		{
			return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z);
		}

		public static bool IsValid(Quaternion rotation)
		{
			return !float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w) && (rotation.x != 0f || rotation.y != 0f || rotation.z != 0f || rotation.w != 0f);
		}

		public static void DrawVelocity(int key, Vector3 position, Vector3 velocity, float destroyAfterSeconds = 5f)
		{
			DrawVelocity(key, position, velocity, Color.green, destroyAfterSeconds);
		}

		public static void DrawVelocity(int key, Vector3 position, Vector3 velocity, Color color, float destroyAfterSeconds = 5f)
		{
			if (!velocityCache.ContainsKey(key) || velocityCache[key] == null)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
				gameObject.transform.localScale = Vector3.one * 0.025f;
				gameObject.transform.position = position;
				if (velocity != Vector3.zero)
				{
					gameObject.transform.forward = velocity;
				}
				GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
				gameObject2.transform.parent = gameObject.transform;
				if (velocity != Vector3.zero)
				{
					gameObject2.transform.localScale = new Vector3(0.25f, 0.25f, 3f + velocity.magnitude * 1.5f);
					gameObject2.transform.localPosition = new Vector3(0f, 0f, gameObject2.transform.localScale.z / 2f);
				}
				else
				{
					gameObject2.transform.localScale = Vector3.one;
					gameObject2.transform.localPosition = Vector3.zero;
				}
				gameObject2.transform.localRotation = Quaternion.identity;
				UnityEngine.Object.DestroyImmediate(gameObject2.GetComponent<Collider>());
				UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<Collider>());
				gameObject.GetComponent<MeshRenderer>().material.color = color;
				gameObject2.GetComponent<MeshRenderer>().material.color = color;
				velocityCache[key] = gameObject;
				UnityEngine.Object.Destroy(gameObject, destroyAfterSeconds);
			}
			else
			{
				GameObject gameObject3 = velocityCache[key];
				gameObject3.transform.position = position;
				if (velocity != Vector3.zero)
				{
					gameObject3.transform.forward = velocity;
				}
				Transform child = gameObject3.transform.GetChild(0);
				if (velocity != Vector3.zero)
				{
					child.localScale = new Vector3(0.25f, 0.25f, 3f + velocity.magnitude * 1.5f);
					child.localPosition = new Vector3(0f, 0f, child.transform.localScale.z / 2f);
				}
				else
				{
					child.localScale = Vector3.one;
					child.localPosition = Vector3.zero;
				}
				child.localRotation = Quaternion.identity;
				UnityEngine.Object.Destroy(gameObject3, destroyAfterSeconds);
			}
		}
	}
}
