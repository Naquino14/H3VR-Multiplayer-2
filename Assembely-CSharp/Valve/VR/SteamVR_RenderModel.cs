using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

namespace Valve.VR
{
	[ExecuteInEditMode]
	public class SteamVR_RenderModel : MonoBehaviour
	{
		public class RenderModel
		{
			public Mesh mesh
			{
				get;
				private set;
			}

			public Material material
			{
				get;
				private set;
			}

			public RenderModel(Mesh mesh, Material material)
			{
				this.mesh = mesh;
				this.material = material;
			}
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
					if (_instance == null && !failedLoadInterface)
					{
						if (Application.isEditor && !Application.isPlaying)
						{
							needsShutdown = SteamVR.InitializeTemporarySession();
						}
						_instance = OpenVR.RenderModels;
						if (_instance == null)
						{
							Debug.LogError("<b>[SteamVR]</b> Failed to load IVRRenderModels interface version IVRRenderModels_006");
							failedLoadInterface = true;
						}
					}
					return _instance;
				}
			}

			public void Dispose()
			{
				if (needsShutdown)
				{
					SteamVR.ExitTemporarySession();
				}
			}
		}

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

		public string renderModelName
		{
			get;
			private set;
		}

		public bool initializedAttachPoints
		{
			get;
			set;
		}

		private SteamVR_RenderModel()
		{
			deviceConnectedAction = SteamVR_Events.DeviceConnectedAction(OnDeviceConnected);
			hideRenderModelsAction = SteamVR_Events.HideRenderModelsAction(OnHideRenderModels);
			modelSkinSettingsHaveChangedAction = SteamVR_Events.SystemAction(EVREventType.VREvent_ModelSkinSettingsHaveChanged, OnModelSkinSettingsHaveChanged);
		}

		private void OnModelSkinSettingsHaveChanged(VREvent_t vrEvent)
		{
			if (!string.IsNullOrEmpty(renderModelName))
			{
				renderModelName = string.Empty;
				UpdateModel();
			}
		}

		public void SetMeshRendererState(bool state)
		{
			for (int i = 0; i < meshRenderers.Count; i++)
			{
				MeshRenderer meshRenderer = meshRenderers[i];
				if (meshRenderer != null)
				{
					meshRenderer.enabled = state;
				}
			}
		}

		private void OnHideRenderModels(bool hidden)
		{
			SetMeshRendererState(!hidden);
		}

		private void OnDeviceConnected(int i, bool connected)
		{
			if (i == (int)index && connected)
			{
				UpdateModel();
			}
		}

		public void UpdateModel()
		{
			CVRSystem system = OpenVR.System;
			if (system == null || index == SteamVR_TrackedObject.EIndex.None)
			{
				return;
			}
			ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
			uint stringTrackedDeviceProperty = system.GetStringTrackedDeviceProperty((uint)index, ETrackedDeviceProperty.Prop_RenderModelName_String, null, 0u, ref pError);
			if (stringTrackedDeviceProperty <= 1)
			{
				Debug.LogError("<b>[SteamVR]</b> Failed to get render model name for tracked object " + index);
				return;
			}
			StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
			system.GetStringTrackedDeviceProperty((uint)index, ETrackedDeviceProperty.Prop_RenderModelName_String, stringBuilder, stringTrackedDeviceProperty, ref pError);
			string text = stringBuilder.ToString();
			if (renderModelName != text)
			{
				StartCoroutine(SetModelAsync(text));
			}
		}

		private IEnumerator SetModelAsync(string newRenderModelName)
		{
			meshRenderers.Clear();
			if (string.IsNullOrEmpty(newRenderModelName))
			{
				yield break;
			}
			RenderModelInterfaceHolder holder = new RenderModelInterfaceHolder();
			try
			{
				CVRRenderModels renderModels = holder.instance;
				if (renderModels == null)
				{
					yield break;
				}
				uint count = renderModels.GetComponentCount(newRenderModelName);
				string[] renderModelNames;
				if (count != 0)
				{
					renderModelNames = new string[count];
					for (int i = 0; i < count; i++)
					{
						uint componentName = renderModels.GetComponentName(newRenderModelName, (uint)i, null, 0u);
						if (componentName == 0)
						{
							continue;
						}
						StringBuilder stringBuilder = new StringBuilder((int)componentName);
						if (renderModels.GetComponentName(newRenderModelName, (uint)i, stringBuilder, componentName) == 0)
						{
							continue;
						}
						string pchComponentName = stringBuilder.ToString();
						componentName = renderModels.GetComponentRenderModelName(newRenderModelName, pchComponentName, null, 0u);
						if (componentName == 0)
						{
							continue;
						}
						StringBuilder stringBuilder2 = new StringBuilder((int)componentName);
						if (renderModels.GetComponentRenderModelName(newRenderModelName, pchComponentName, stringBuilder2, componentName) != 0)
						{
							string text = stringBuilder2.ToString();
							RenderModel renderModel = models[text] as RenderModel;
							if (renderModel == null || renderModel.mesh == null)
							{
								renderModelNames[i] = text;
							}
						}
					}
				}
				else
				{
					RenderModel renderModel2 = models[newRenderModelName] as RenderModel;
					renderModelNames = ((renderModel2 != null && !(renderModel2.mesh == null)) ? new string[0] : new string[1]
					{
						newRenderModelName
					});
				}
				while (true)
				{
					bool loading = false;
					for (int j = 0; j < renderModelNames.Length; j++)
					{
						if (string.IsNullOrEmpty(renderModelNames[j]))
						{
							continue;
						}
						IntPtr ppRenderModel = IntPtr.Zero;
						switch (renderModels.LoadRenderModel_Async(renderModelNames[j], ref ppRenderModel))
						{
						case EVRRenderModelError.Loading:
							loading = true;
							break;
						case EVRRenderModelError.None:
						{
							RenderModel_t renderModel_t = MarshalRenderModel(ppRenderModel);
							Material material = materials[renderModel_t.diffuseTextureId] as Material;
							if (material == null || material.mainTexture == null)
							{
								IntPtr ppTexture = IntPtr.Zero;
								EVRRenderModelError eVRRenderModelError = renderModels.LoadTexture_Async(renderModel_t.diffuseTextureId, ref ppTexture);
								if (eVRRenderModelError == EVRRenderModelError.Loading)
								{
									loading = true;
								}
							}
							break;
						}
						}
					}
					if (loading)
					{
						yield return new WaitForSecondsRealtime(0.1f);
						continue;
					}
					break;
				}
			}
			finally
			{
				// base._003C_003E__Finally0(); 
				// this method either wasnt decompiled properly, or it doesnt exist
			}
			bool success = SetModel(newRenderModelName);
			renderModelName = newRenderModelName;
			SteamVR_Events.RenderModelLoaded.Send(this, success);
		}

		private bool SetModel(string renderModelName)
		{
			StripMesh(base.gameObject);
			using (RenderModelInterfaceHolder renderModelInterfaceHolder = new RenderModelInterfaceHolder())
			{
				if (createComponents)
				{
					componentAttachPoints.Clear();
					if (LoadComponents(renderModelInterfaceHolder, renderModelName))
					{
						UpdateComponents(renderModelInterfaceHolder.instance);
						return true;
					}
					Debug.Log("<b>[SteamVR]</b> [" + base.gameObject.name + "] Render model does not support components, falling back to single mesh.");
				}
				if (!string.IsNullOrEmpty(renderModelName))
				{
					RenderModel renderModel = models[renderModelName] as RenderModel;
					if (renderModel == null || renderModel.mesh == null)
					{
						CVRRenderModels instance = renderModelInterfaceHolder.instance;
						if (instance == null)
						{
							return false;
						}
						if (verbose)
						{
							Debug.Log("<b>[SteamVR]</b> Loading render model " + renderModelName);
						}
						renderModel = LoadRenderModel(instance, renderModelName, renderModelName);
						if (renderModel == null)
						{
							return false;
						}
						models[renderModelName] = renderModel;
					}
					base.gameObject.AddComponent<MeshFilter>().mesh = renderModel.mesh;
					MeshRenderer meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
					meshRenderer.sharedMaterial = renderModel.material;
					meshRenderers.Add(meshRenderer);
					return true;
				}
			}
			return false;
		}

		private RenderModel LoadRenderModel(CVRRenderModels renderModels, string renderModelName, string baseName)
		{
			IntPtr ppRenderModel = IntPtr.Zero;
			while (true)
			{
				EVRRenderModelError eVRRenderModelError = renderModels.LoadRenderModel_Async(renderModelName, ref ppRenderModel);
				switch (eVRRenderModelError)
				{
				case EVRRenderModelError.Loading:
					break;
				default:
					Debug.LogError($"<b>[SteamVR]</b> Failed to load render model {renderModelName} - {eVRRenderModelError.ToString()}");
					return null;
				case EVRRenderModelError.None:
				{
					RenderModel_t renderModel_t = MarshalRenderModel(ppRenderModel);
					Vector3[] array = new Vector3[renderModel_t.unVertexCount];
					Vector3[] array2 = new Vector3[renderModel_t.unVertexCount];
					Vector2[] array3 = new Vector2[renderModel_t.unVertexCount];
					Type typeFromHandle = typeof(RenderModel_Vertex_t);
					for (int i = 0; i < renderModel_t.unVertexCount; i++)
					{
						IntPtr ptr = new IntPtr(renderModel_t.rVertexData.ToInt64() + i * Marshal.SizeOf(typeFromHandle));
						RenderModel_Vertex_t renderModel_Vertex_t = (RenderModel_Vertex_t)Marshal.PtrToStructure(ptr, typeFromHandle);
						ref Vector3 reference = ref array[i];
						reference = new Vector3(renderModel_Vertex_t.vPosition.v0, renderModel_Vertex_t.vPosition.v1, 0f - renderModel_Vertex_t.vPosition.v2);
						ref Vector3 reference2 = ref array2[i];
						reference2 = new Vector3(renderModel_Vertex_t.vNormal.v0, renderModel_Vertex_t.vNormal.v1, 0f - renderModel_Vertex_t.vNormal.v2);
						ref Vector2 reference3 = ref array3[i];
						reference3 = new Vector2(renderModel_Vertex_t.rfTextureCoord0, renderModel_Vertex_t.rfTextureCoord1);
					}
					int num = (int)(renderModel_t.unTriangleCount * 3);
					short[] array4 = new short[num];
					Marshal.Copy(renderModel_t.rIndexData, array4, 0, array4.Length);
					int[] array5 = new int[num];
					for (int j = 0; j < renderModel_t.unTriangleCount; j++)
					{
						array5[j * 3] = array4[j * 3 + 2];
						array5[j * 3 + 1] = array4[j * 3 + 1];
						array5[j * 3 + 2] = array4[j * 3];
					}
					Mesh mesh = new Mesh();
					mesh.vertices = array;
					mesh.normals = array2;
					mesh.uv = array3;
					mesh.triangles = array5;
					Material material = materials[renderModel_t.diffuseTextureId] as Material;
					if (material == null || material.mainTexture == null)
					{
						IntPtr ppTexture = IntPtr.Zero;
						while (true)
						{
							eVRRenderModelError = renderModels.LoadTexture_Async(renderModel_t.diffuseTextureId, ref ppTexture);
							switch (eVRRenderModelError)
							{
							case EVRRenderModelError.Loading:
								goto IL_0273;
							case EVRRenderModelError.None:
							{
								RenderModel_TextureMap_t renderModel_TextureMap_t = MarshalRenderModel_TextureMap(ppTexture);
								Texture2D texture2D = new Texture2D(renderModel_TextureMap_t.unWidth, renderModel_TextureMap_t.unHeight, TextureFormat.RGBA32, mipmap: false);
								if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11)
								{
									texture2D.Apply();
									IntPtr nativeTexturePtr = texture2D.GetNativeTexturePtr();
									while (true)
									{
										eVRRenderModelError = renderModels.LoadIntoTextureD3D11_Async(renderModel_t.diffuseTextureId, nativeTexturePtr);
										if (eVRRenderModelError != EVRRenderModelError.Loading)
										{
											break;
										}
										Sleep();
									}
								}
								else
								{
									byte[] array6 = new byte[renderModel_TextureMap_t.unWidth * renderModel_TextureMap_t.unHeight * 4];
									Marshal.Copy(renderModel_TextureMap_t.rubTextureMapData, array6, 0, array6.Length);
									Color32[] array7 = new Color32[renderModel_TextureMap_t.unWidth * renderModel_TextureMap_t.unHeight];
									int num2 = 0;
									for (int k = 0; k < renderModel_TextureMap_t.unHeight; k++)
									{
										for (int l = 0; l < renderModel_TextureMap_t.unWidth; l++)
										{
											byte r = array6[num2++];
											byte g = array6[num2++];
											byte b = array6[num2++];
											byte a = array6[num2++];
											ref Color32 reference4 = ref array7[k * renderModel_TextureMap_t.unWidth + l];
											reference4 = new Color32(r, g, b, a);
										}
									}
									texture2D.SetPixels32(array7);
									texture2D.Apply();
								}
								material = new Material((!(shader != null)) ? Shader.Find("Standard") : shader);
								material.mainTexture = texture2D;
								materials[renderModel_t.diffuseTextureId] = material;
								renderModels.FreeTexture(ppTexture);
								break;
							}
							default:
								Debug.Log("<b>[SteamVR]</b> Failed to load render model texture for render model " + renderModelName + ". Error: " + eVRRenderModelError);
								break;
							}
							break;
							IL_0273:
							Sleep();
						}
					}
					StartCoroutine(FreeRenderModel(ppRenderModel));
					return new RenderModel(mesh, material);
				}
				}
				Sleep();
			}
		}

		private IEnumerator FreeRenderModel(IntPtr pRenderModel)
		{
			yield return new WaitForSeconds(1f);
			using RenderModelInterfaceHolder renderModelInterfaceHolder = new RenderModelInterfaceHolder();
			CVRRenderModels instance = renderModelInterfaceHolder.instance;
			instance.FreeRenderModel(pRenderModel);
		}

		public Transform FindTransformByName(string componentName, Transform inTransform = null)
		{
			if (inTransform == null)
			{
				inTransform = base.transform;
			}
			for (int i = 0; i < inTransform.childCount; i++)
			{
				Transform child = inTransform.GetChild(i);
				if (child.name == componentName)
				{
					return child;
				}
			}
			return null;
		}

		public Transform GetComponentTransform(string componentName)
		{
			if (componentName == null)
			{
				return base.transform;
			}
			if (componentAttachPoints.ContainsKey(componentName))
			{
				return componentAttachPoints[componentName];
			}
			return null;
		}

		private void StripMesh(GameObject go)
		{
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
			MeshFilter component2 = go.GetComponent<MeshFilter>();
			if (component2 != null)
			{
				UnityEngine.Object.DestroyImmediate(component2);
			}
		}

		private bool LoadComponents(RenderModelInterfaceHolder holder, string renderModelName)
		{
			Transform transform = base.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				child.gameObject.SetActive(value: false);
				StripMesh(child.gameObject);
			}
			if (string.IsNullOrEmpty(renderModelName))
			{
				return true;
			}
			CVRRenderModels instance = holder.instance;
			if (instance == null)
			{
				return false;
			}
			uint componentCount = instance.GetComponentCount(renderModelName);
			if (componentCount == 0)
			{
				return false;
			}
			for (int j = 0; j < componentCount; j++)
			{
				uint componentName = instance.GetComponentName(renderModelName, (uint)j, null, 0u);
				if (componentName == 0)
				{
					continue;
				}
				StringBuilder stringBuilder = new StringBuilder((int)componentName);
				if (instance.GetComponentName(renderModelName, (uint)j, stringBuilder, componentName) == 0)
				{
					continue;
				}
				string text = stringBuilder.ToString();
				transform = FindTransformByName(text);
				if (transform != null)
				{
					transform.gameObject.SetActive(value: true);
					componentAttachPoints[text] = FindTransformByName("attach", transform);
				}
				else
				{
					transform = new GameObject(text).transform;
					transform.parent = base.transform;
					transform.gameObject.layer = base.gameObject.layer;
					Transform transform2 = new GameObject("attach").transform;
					transform2.parent = transform;
					transform2.localPosition = Vector3.zero;
					transform2.localRotation = Quaternion.identity;
					transform2.localScale = Vector3.one;
					transform2.gameObject.layer = base.gameObject.layer;
					componentAttachPoints[text] = transform2;
				}
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				componentName = instance.GetComponentRenderModelName(renderModelName, text, null, 0u);
				if (componentName == 0)
				{
					continue;
				}
				StringBuilder stringBuilder2 = new StringBuilder((int)componentName);
				if (instance.GetComponentRenderModelName(renderModelName, text, stringBuilder2, componentName) == 0)
				{
					continue;
				}
				string text2 = stringBuilder2.ToString();
				RenderModel renderModel = models[text2] as RenderModel;
				if (renderModel == null || renderModel.mesh == null)
				{
					if (verbose)
					{
						Debug.Log("<b>[SteamVR]</b> Loading render model " + text2);
					}
					renderModel = LoadRenderModel(instance, text2, renderModelName);
					if (renderModel == null)
					{
						continue;
					}
					models[text2] = renderModel;
				}
				transform.gameObject.AddComponent<MeshFilter>().mesh = renderModel.mesh;
				MeshRenderer meshRenderer = transform.gameObject.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = renderModel.material;
				meshRenderers.Add(meshRenderer);
			}
			return true;
		}

		private void OnEnable()
		{
			if (!string.IsNullOrEmpty(modelOverride))
			{
				Debug.Log("<b>[SteamVR]</b> Model override is really only meant to be used in the scene view for lining things up; using it at runtime is discouraged.  Use tracked device index instead to ensure the correct model is displayed for all users.");
				base.enabled = false;
				return;
			}
			CVRSystem system = OpenVR.System;
			if (system != null && system.IsTrackedDeviceConnected((uint)index))
			{
				UpdateModel();
			}
			deviceConnectedAction.enabled = true;
			hideRenderModelsAction.enabled = true;
			modelSkinSettingsHaveChangedAction.enabled = true;
		}

		private void OnDisable()
		{
			deviceConnectedAction.enabled = false;
			hideRenderModelsAction.enabled = false;
			modelSkinSettingsHaveChangedAction.enabled = false;
		}

		private void Update()
		{
			if (updateDynamically)
			{
				UpdateComponents(OpenVR.RenderModels);
			}
		}

		public void UpdateComponents(CVRRenderModels renderModels)
		{
			if (renderModels == null || base.transform.childCount == 0)
			{
				return;
			}
			if (nameCache == null)
			{
				nameCache = new Dictionary<int, string>();
			}
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				if (!nameCache.TryGetValue(child.GetInstanceID(), out var value))
				{
					value = child.name;
					nameCache.Add(child.GetInstanceID(), value);
				}
				RenderModel_ComponentState_t pComponentState = default(RenderModel_ComponentState_t);
				if (!renderModels.GetComponentStateForDevicePath(renderModelName, value, SteamVR_Input_Source.GetHandle(inputSource), ref controllerModeState, ref pComponentState))
				{
					continue;
				}
				child.localPosition = SteamVR_Utils.GetPosition(pComponentState.mTrackingToComponentRenderModel);
				child.localRotation = SteamVR_Utils.GetRotation(pComponentState.mTrackingToComponentRenderModel);
				Transform transform = null;
				for (int j = 0; j < child.childCount; j++)
				{
					Transform child2 = child.GetChild(j);
					int instanceID = child2.GetInstanceID();
					if (!nameCache.TryGetValue(instanceID, out var value2))
					{
						value2 = child2.name;
						nameCache.Add(instanceID, value);
					}
					if (value2 == "attach")
					{
						transform = child2;
					}
				}
				if (transform != null)
				{
					transform.position = base.transform.TransformPoint(SteamVR_Utils.GetPosition(pComponentState.mTrackingToComponentLocal));
					transform.rotation = base.transform.rotation * SteamVR_Utils.GetRotation(pComponentState.mTrackingToComponentLocal);
					initializedAttachPoints = true;
				}
				bool flag = (pComponentState.uProperties & 2) != 0;
				if (flag != child.gameObject.activeSelf)
				{
					child.gameObject.SetActive(flag);
				}
			}
		}

		public void SetDeviceIndex(int newIndex)
		{
			index = (SteamVR_TrackedObject.EIndex)newIndex;
			modelOverride = string.Empty;
			if (base.enabled)
			{
				UpdateModel();
			}
		}

		public void SetInputSource(SteamVR_Input_Sources newInputSource)
		{
			inputSource = newInputSource;
		}

		private static void Sleep()
		{
			Thread.Sleep(1);
		}

		private RenderModel_t MarshalRenderModel(IntPtr pRenderModel)
		{
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
			{
				RenderModel_t_Packed renderModel_t_Packed = (RenderModel_t_Packed)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_t_Packed));
				RenderModel_t unpacked = default(RenderModel_t);
				renderModel_t_Packed.Unpack(ref unpacked);
				return unpacked;
			}
			return (RenderModel_t)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_t));
		}

		private RenderModel_TextureMap_t MarshalRenderModel_TextureMap(IntPtr pRenderModel)
		{
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
			{
				RenderModel_TextureMap_t_Packed renderModel_TextureMap_t_Packed = (RenderModel_TextureMap_t_Packed)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_TextureMap_t_Packed));
				RenderModel_TextureMap_t unpacked = default(RenderModel_TextureMap_t);
				renderModel_TextureMap_t_Packed.Unpack(ref unpacked);
				return unpacked;
			}
			return (RenderModel_TextureMap_t)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_TextureMap_t));
		}
	}
}
