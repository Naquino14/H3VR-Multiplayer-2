using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine.SceneManagement;
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
				if (m_WhiteTexture == null)
				{
					m_WhiteTexture = new Texture2D(1, 1, TextureFormat.ARGB32, mipmap: false);
					m_WhiteTexture.SetPixel(0, 0, Color.white);
					m_WhiteTexture.Apply();
				}
				return m_WhiteTexture;
			}
		}

		public static Texture2D blackTexture
		{
			get
			{
				if (m_BlackTexture == null)
				{
					m_BlackTexture = new Texture2D(1, 1, TextureFormat.ARGB32, mipmap: false);
					m_BlackTexture.SetPixel(0, 0, Color.black);
					m_BlackTexture.Apply();
				}
				return m_BlackTexture;
			}
		}

		public static Texture2D transparentTexture
		{
			get
			{
				if (m_TransparentTexture == null)
				{
					m_TransparentTexture = new Texture2D(1, 1, TextureFormat.ARGB32, mipmap: false);
					m_TransparentTexture.SetPixel(0, 0, Color.clear);
					m_TransparentTexture.Apply();
				}
				return m_TransparentTexture;
			}
		}

		public static Mesh fullscreenTriangle
		{
			get
			{
				if (s_FullscreenTriangle != null)
				{
					return s_FullscreenTriangle;
				}
				Mesh mesh = new Mesh();
				mesh.name = "Fullscreen Triangle";
				s_FullscreenTriangle = mesh;
				s_FullscreenTriangle.SetVertices(new List<Vector3>
				{
					new Vector3(-1f, -1f, 0f),
					new Vector3(-1f, 3f, 0f),
					new Vector3(3f, -1f, 0f)
				});
				s_FullscreenTriangle.SetIndices(new int[3]
				{
					0,
					1,
					2
				}, MeshTopology.Triangles, 0, calculateBounds: false);
				s_FullscreenTriangle.UploadMeshData(markNoLogerReadable: false);
				return s_FullscreenTriangle;
			}
		}

		public static Material copyStdMaterial
		{
			get
			{
				if (s_CopyStdMaterial != null)
				{
					return s_CopyStdMaterial;
				}
				Shader shader = Shader.Find("Hidden/PostProcessing/CopyStd");
				Material material = new Material(shader);
				material.name = "PostProcess - CopyStd";
				material.hideFlags = HideFlags.HideAndDontSave;
				s_CopyStdMaterial = material;
				return s_CopyStdMaterial;
			}
		}

		public static Material copyMaterial
		{
			get
			{
				if (s_CopyMaterial != null)
				{
					return s_CopyMaterial;
				}
				Shader shader = Shader.Find("Hidden/PostProcessing/Copy");
				Material material = new Material(shader);
				material.name = "PostProcess - Copy";
				material.hideFlags = HideFlags.HideAndDontSave;
				s_CopyMaterial = material;
				return s_CopyMaterial;
			}
		}

		public static PropertySheet copySheet
		{
			get
			{
				if (s_CopySheet == null)
				{
					s_CopySheet = new PropertySheet(copyMaterial);
				}
				return s_CopySheet;
			}
		}

		public static bool scriptableRenderPipelineActive => GraphicsSettings.renderPipelineAsset != null;

		public static bool isSinglePassStereoEnabled => true;

		public static bool isVREnabled => VRSettings.enabled;

		public static bool isAndroidOpenGL => Application.platform == RuntimePlatform.Android && SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan;

		public static bool isLinearColorSpace => QualitySettings.activeColorSpace == ColorSpace.Linear;

		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, bool clear = false)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTarget(destination);
			if (clear)
			{
				cmd.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
			}
			cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, copyMaterial, 0, 0);
		}

		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTarget(destination);
			if (clear)
			{
				cmd.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
			}
			cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTarget(destination, depth);
			if (clear)
			{
				cmd.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
			}
			cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier[] destinations, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTarget(destinations, depth);
			if (clear)
			{
				cmd.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
			}
			cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		public static void BlitFullscreenTriangle(Texture source, RenderTexture destination, Material material, int pass)
		{
			RenderTexture active = RenderTexture.active;
			material.SetPass(pass);
			if (source != null)
			{
				material.SetTexture(ShaderIDs.MainTex, source);
			}
			Graphics.SetRenderTarget(destination);
			Graphics.DrawMeshNow(fullscreenTriangle, Matrix4x4.identity);
			RenderTexture.active = active;
		}

		public static void CopyTexture(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
		{
			if (SystemInfo.copyTextureSupport > CopyTextureSupport.None)
			{
				cmd.CopyTexture(source, destination);
			}
			else
			{
				cmd.BlitFullscreenTriangle(source, destination);
			}
		}

		public static void Destroy(Object obj)
		{
			if (obj != null)
			{
				Object.Destroy(obj);
			}
		}

		public static bool IsResolvedDepthAvailable(Camera camera)
		{
			GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
			return camera.actualRenderingPath == RenderingPath.DeferredShading && (graphicsDeviceType == GraphicsDeviceType.Direct3D11 || graphicsDeviceType == GraphicsDeviceType.Direct3D12 || graphicsDeviceType == GraphicsDeviceType.XboxOne);
		}

		public static void DestroyProfile(PostProcessProfile profile, bool destroyEffects)
		{
			if (destroyEffects)
			{
				foreach (PostProcessEffectSettings setting in profile.settings)
				{
					Destroy(setting);
				}
			}
			Destroy(profile);
		}

		public static void DestroyVolume(PostProcessVolume volume, bool destroySharedProfile)
		{
			if (destroySharedProfile)
			{
				DestroyProfile(volume.sharedProfile, destroyEffects: true);
			}
			Destroy(volume);
		}

		public static IEnumerable<T> GetAllSceneObjects<T>() where T : Component
		{
			Queue<Transform> queue = new Queue<Transform>();
			GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
			GameObject[] array = roots;
			foreach (GameObject root in array)
			{
				queue.Enqueue(root.transform);
				T comp = root.GetComponent<T>();
				if ((Object)comp != (Object)null)
				{
					yield return comp;
				}
			}
			while (queue.Count > 0)
			{
				IEnumerator enumerator = queue.Dequeue().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Transform child = (Transform)enumerator.Current;
						queue.Enqueue(child);
						T comp2 = child.GetComponent<T>();
						if ((Object)comp2 != (Object)null)
						{
							yield return comp2;
						}
					}
				}
				finally
				{
					IDisposable disposable;
					IDisposable disposable2 = (disposable = enumerator as IDisposable);
					if (disposable != null)
					{
						disposable2.Dispose();
					}
				}
			}
		}

		public static void CreateIfNull<T>(ref T obj) where T : class, new()
		{
			if (obj == null)
			{
				obj = new T();
			}
		}

		public static float Exp2(float x)
		{
			return Mathf.Exp(x * 0.6931472f);
		}

		public static Matrix4x4 GetJitteredPerspectiveProjectionMatrix(Camera camera, Vector2 offset)
		{
			float num = Mathf.Tan((float)Math.PI / 360f * camera.fieldOfView);
			float num2 = num * camera.aspect;
			float nearClipPlane = camera.nearClipPlane;
			float farClipPlane = camera.farClipPlane;
			offset.x *= num2 / (0.5f * (float)camera.pixelWidth);
			offset.y *= num / (0.5f * (float)camera.pixelHeight);
			float num3 = (offset.x - num2) * nearClipPlane;
			float num4 = (offset.x + num2) * nearClipPlane;
			float num5 = (offset.y + num) * nearClipPlane;
			float num6 = (offset.y - num) * nearClipPlane;
			Matrix4x4 result = default(Matrix4x4);
			result[0, 0] = 2f * nearClipPlane / (num4 - num3);
			result[0, 1] = 0f;
			result[0, 2] = (num4 + num3) / (num4 - num3);
			result[0, 3] = 0f;
			result[1, 0] = 0f;
			result[1, 1] = 2f * nearClipPlane / (num5 - num6);
			result[1, 2] = (num5 + num6) / (num5 - num6);
			result[1, 3] = 0f;
			result[2, 0] = 0f;
			result[2, 1] = 0f;
			result[2, 2] = (0f - (farClipPlane + nearClipPlane)) / (farClipPlane - nearClipPlane);
			result[2, 3] = (0f - 2f * farClipPlane * nearClipPlane) / (farClipPlane - nearClipPlane);
			result[3, 0] = 0f;
			result[3, 1] = 0f;
			result[3, 2] = -1f;
			result[3, 3] = 0f;
			return result;
		}

		public static Matrix4x4 GetJitteredOrthographicProjectionMatrix(Camera camera, Vector2 offset)
		{
			float orthographicSize = camera.orthographicSize;
			float num = orthographicSize * camera.aspect;
			offset.x *= num / (0.5f * (float)camera.pixelWidth);
			offset.y *= orthographicSize / (0.5f * (float)camera.pixelHeight);
			float left = offset.x - num;
			float right = offset.x + num;
			float top = offset.y + orthographicSize;
			float bottom = offset.y - orthographicSize;
			return Matrix4x4.Ortho(left, right, bottom, top, camera.nearClipPlane, camera.farClipPlane);
		}

		public static Matrix4x4 GenerateJitteredProjectionMatrixFromOriginal(PostProcessRenderContext context, Matrix4x4 origProj, Vector2 jitter)
		{
			float num = (1f + origProj[0, 2]) / origProj[0, 0];
			float num2 = (-1f + origProj[0, 2]) / origProj[0, 0];
			float num3 = (1f + origProj[1, 2]) / origProj[1, 1];
			float num4 = (-1f + origProj[1, 2]) / origProj[1, 1];
			float num5 = Math.Abs(num3) + Math.Abs(num4);
			float num6 = Math.Abs(num2) + Math.Abs(num);
			jitter.x *= num6 / (float)context.xrSingleEyeWidth;
			jitter.y *= num5 / (float)context.height;
			float num7 = jitter.x + num2;
			float num8 = jitter.x + num;
			float num9 = jitter.y + num3;
			float num10 = jitter.y + num4;
			Matrix4x4 result = default(Matrix4x4);
			result[0, 0] = 2f / (num8 - num7);
			result[0, 1] = 0f;
			result[0, 2] = (num8 + num7) / (num8 - num7);
			result[0, 3] = 0f;
			result[1, 0] = 0f;
			result[1, 1] = 2f / (num9 - num10);
			result[1, 2] = (num9 + num10) / (num9 - num10);
			result[1, 3] = 0f;
			result[2, 0] = 0f;
			result[2, 1] = 0f;
			result[2, 2] = origProj[2, 2];
			result[2, 3] = origProj[2, 3];
			result[3, 0] = 0f;
			result[3, 1] = 0f;
			result[3, 2] = -1f;
			result[3, 3] = 0f;
			return result;
		}

		public static T GetAttribute<T>(this Type type) where T : Attribute
		{
			return (T)type.GetCustomAttributes(typeof(T), inherit: false)[0];
		}

		public static Attribute[] GetMemberAttributes<TType, TValue>(Expression<Func<TType, TValue>> expr)
		{
			Expression expression = expr;
			if (expression is LambdaExpression)
			{
				expression = ((LambdaExpression)expression).Body;
			}
			ExpressionType nodeType = expression.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				FieldInfo fieldInfo = (FieldInfo)((MemberExpression)expression).Member;
				return fieldInfo.GetCustomAttributes(inherit: false).Cast<Attribute>().ToArray();
			}
			throw new InvalidOperationException();
		}

		public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
		{
			ExpressionType nodeType = expr.Body.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				MemberExpression memberExpression = expr.Body as MemberExpression;
				List<string> list = new List<string>();
				while (memberExpression != null)
				{
					list.Add(memberExpression.Member.Name);
					memberExpression = memberExpression.Expression as MemberExpression;
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int num = list.Count - 1; num >= 0; num--)
				{
					stringBuilder.Append(list[num]);
					if (num > 0)
					{
						stringBuilder.Append('.');
					}
				}
				return stringBuilder.ToString();
			}
			throw new InvalidOperationException();
		}

		public static object GetParentObject(string path, object obj)
		{
			string[] array = path.Split('.');
			if (array.Length == 1)
			{
				return obj;
			}
			FieldInfo field = obj.GetType().GetField(array[0], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			obj = field.GetValue(obj);
			return GetParentObject(string.Join(".", array, 1, array.Length - 1), obj);
		}
	}
}
