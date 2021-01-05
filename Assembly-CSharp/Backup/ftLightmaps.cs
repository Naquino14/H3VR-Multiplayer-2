// Decompiled with JetBrains decompiler
// Type: ftLightmaps
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ftLightmaps
{
  private static List<int> lightmapRefCount;
  private static List<ftLightmaps.LightmapAdditionalData> globalMapsAdditional;
  private static int directionalMode;

  static ftLightmaps()
  {
    // ISSUE: reference to a compiler-generated field
    if (ftLightmaps.\u003C\u003Ef__mg\u0024cache0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      ftLightmaps.\u003C\u003Ef__mg\u0024cache0 = new UnityAction<Scene, Scene>(ftLightmaps.OnSceneChangedPlay);
    }
    // ISSUE: reference to a compiler-generated field
    SceneManager.activeSceneChanged -= ftLightmaps.\u003C\u003Ef__mg\u0024cache0;
    // ISSUE: reference to a compiler-generated field
    if (ftLightmaps.\u003C\u003Ef__mg\u0024cache1 == null)
    {
      // ISSUE: reference to a compiler-generated field
      ftLightmaps.\u003C\u003Ef__mg\u0024cache1 = new UnityAction<Scene, Scene>(ftLightmaps.OnSceneChangedPlay);
    }
    // ISSUE: reference to a compiler-generated field
    SceneManager.activeSceneChanged += ftLightmaps.\u003C\u003Ef__mg\u0024cache1;
  }

  private static void SetDirectionalMode()
  {
    if (ftLightmaps.directionalMode < 0)
      return;
    LightmapSettings.lightmapsMode = ftLightmaps.directionalMode != 1 ? LightmapsMode.NonDirectional : LightmapsMode.CombinedDirectional;
  }

  private static void OnSceneChangedPlay(Scene prev, Scene next) => ftLightmaps.SetDirectionalMode();

  public static void RefreshFull()
  {
    Scene activeScene = SceneManager.GetActiveScene();
    int sceneCount = SceneManager.sceneCount;
    for (int index = 0; index < sceneCount; ++index)
    {
      Scene sceneAt = SceneManager.GetSceneAt(index);
      if (sceneAt.isLoaded)
      {
        SceneManager.SetActiveScene(sceneAt);
        LightmapSettings.lightmaps = new LightmapData[0];
      }
    }
    for (int index = 0; index < sceneCount; ++index)
      ftLightmaps.RefreshScene(SceneManager.GetSceneAt(index), updateNonBaked: true);
    SceneManager.SetActiveScene(activeScene);
  }

  public static GameObject FindInScene(string nm, Scene scn)
  {
    GameObject[] rootGameObjects = scn.GetRootGameObjects();
    for (int index = 0; index < rootGameObjects.Length; ++index)
    {
      if (rootGameObjects[index].name == nm)
        return rootGameObjects[index];
      Transform transform = rootGameObjects[index].transform.Find(nm);
      if ((Object) transform != (Object) null)
        return transform.gameObject;
    }
    return (GameObject) null;
  }

  private static Texture2D GetEmptyDirectionTex(ftLightmapsStorage storage) => storage.emptyDirectionTex;

  public static void RefreshScene(Scene scene, ftLightmapsStorage storage = null, bool updateNonBaked = false)
  {
    int sceneCount = SceneManager.sceneCount;
    if (ftLightmaps.globalMapsAdditional == null)
      ftLightmaps.globalMapsAdditional = new List<ftLightmaps.LightmapAdditionalData>();
    List<LightmapData> lightmapDataList = new List<LightmapData>();
    List<ftLightmaps.LightmapAdditionalData> lightmapAdditionalDataList = new List<ftLightmaps.LightmapAdditionalData>();
    LightmapData[] lightmaps = LightmapSettings.lightmaps;
    List<ftLightmaps.LightmapAdditionalData> globalMapsAdditional = ftLightmaps.globalMapsAdditional;
    if ((Object) storage == (Object) null)
    {
      if (!scene.isLoaded)
        return;
      SceneManager.SetActiveScene(scene);
      GameObject inScene = ftLightmaps.FindInScene("!ftraceLightmaps", scene);
      if ((Object) inScene == (Object) null)
        return;
      storage = inScene.GetComponent<ftLightmapsStorage>();
      if ((Object) storage == (Object) null)
        return;
    }
    if (storage.idremap == null || storage.idremap.Length != storage.maps.Count)
      storage.idremap = new int[storage.maps.Count];
    ftLightmaps.directionalMode = storage.dirMaps.Count == 0 ? 0 : 1;
    bool flag1 = false;
    ftLightmaps.SetDirectionalMode();
    if (ftLightmaps.directionalMode == 1)
    {
      for (int index = 0; index < lightmaps.Length; ++index)
      {
        if ((Object) lightmaps[index].lightmapDir == (Object) null)
        {
          LightmapData lightmapData = lightmaps[index];
          lightmapData.lightmapDir = ftLightmaps.GetEmptyDirectionTex(storage);
          lightmaps[index] = lightmapData;
          flag1 = true;
        }
      }
    }
    bool flag2 = false;
    if (lightmaps.Length == storage.maps.Count)
    {
      flag2 = true;
      for (int index = 0; index < storage.maps.Count; ++index)
      {
        if ((Object) lightmaps[index].lightmapColor != (Object) storage.maps[index])
        {
          flag2 = false;
          break;
        }
        if (storage.rnmMaps0.Count > index && (globalMapsAdditional.Count <= index || (Object) globalMapsAdditional[index].rnm0 != (Object) storage.rnmMaps0[index]))
        {
          flag2 = false;
          break;
        }
      }
    }
    if (!flag2)
    {
      if (sceneCount >= 1)
      {
        for (int index = 0; index < lightmaps.Length; ++index)
        {
          if (lightmaps[index] != null && (!((Object) lightmaps[index].lightmapColor == (Object) null) || !((Object) lightmaps[index].shadowMask == (Object) null)) || index != 0 && index != lightmaps.Length - 1)
          {
            lightmapDataList.Add(lightmaps[index]);
            if (globalMapsAdditional.Count > index)
              lightmapAdditionalDataList.Add(globalMapsAdditional[index]);
          }
        }
      }
      for (int index1 = 0; index1 < storage.maps.Count; ++index1)
      {
        Texture2D map = storage.maps[index1];
        Texture2D texture2D1 = (Texture2D) null;
        Texture2D texture2D2 = (Texture2D) null;
        Texture2D texture2D3 = (Texture2D) null;
        Texture2D texture2D4 = (Texture2D) null;
        Texture2D texture2D5 = (Texture2D) null;
        int num = 0;
        if (storage.masks.Count > index1)
          texture2D1 = storage.masks[index1];
        if (storage.dirMaps.Count > index1)
          texture2D2 = storage.dirMaps[index1];
        if (storage.rnmMaps0.Count > index1)
        {
          texture2D3 = storage.rnmMaps0[index1];
          texture2D4 = storage.rnmMaps1[index1];
          texture2D5 = storage.rnmMaps2[index1];
          num = storage.mapsMode[index1];
        }
        bool flag3 = false;
        int index2 = -1;
        for (int index3 = 0; index3 < lightmapDataList.Count; ++index3)
        {
          if ((Object) lightmapDataList[index3].lightmapColor == (Object) map && (Object) lightmapDataList[index3].shadowMask == (Object) texture2D1)
          {
            storage.idremap[index1] = index3;
            flag3 = true;
            if ((Object) texture2D3 != (Object) null && (lightmapAdditionalDataList.Count <= index3 || (Object) lightmapAdditionalDataList[index3].rnm0 == (Object) null))
            {
              while (lightmapAdditionalDataList.Count <= index3)
                lightmapAdditionalDataList.Add(new ftLightmaps.LightmapAdditionalData());
              lightmapAdditionalDataList[index3] = new ftLightmaps.LightmapAdditionalData()
              {
                rnm0 = texture2D3,
                rnm1 = texture2D4,
                rnm2 = texture2D5,
                mode = num
              };
              break;
            }
            break;
          }
          if (index2 < 0 && (Object) lightmapDataList[index3].lightmapColor == (Object) null && (Object) lightmapDataList[index3].shadowMask == (Object) null)
          {
            storage.idremap[index1] = index3;
            index2 = index3;
          }
        }
        if (!flag3)
        {
          LightmapData lightmapData = index2 < 0 ? new LightmapData() : lightmapDataList[index2];
          lightmapData.lightmapColor = map;
          if (storage.masks.Count > index1)
            lightmapData.shadowMask = texture2D1;
          if (storage.dirMaps.Count > index1 && (Object) texture2D2 != (Object) null)
            lightmapData.lightmapDir = texture2D2;
          else if (ftLightmaps.directionalMode == 1)
            lightmapData.lightmapDir = ftLightmaps.GetEmptyDirectionTex(storage);
          if (index2 < 0)
          {
            lightmapDataList.Add(lightmapData);
            storage.idremap[index1] = lightmapDataList.Count - 1;
          }
          else
            lightmapDataList[index2] = lightmapData;
          if (storage.rnmMaps0.Count > index1)
          {
            ftLightmaps.LightmapAdditionalData lightmapAdditionalData = new ftLightmaps.LightmapAdditionalData();
            lightmapAdditionalData.rnm0 = texture2D3;
            lightmapAdditionalData.rnm1 = texture2D4;
            lightmapAdditionalData.rnm2 = texture2D5;
            lightmapAdditionalData.mode = num;
            if (index2 < 0)
            {
              while (lightmapAdditionalDataList.Count < lightmapDataList.Count - 1)
                lightmapAdditionalDataList.Add(new ftLightmaps.LightmapAdditionalData());
              lightmapAdditionalDataList.Add(lightmapAdditionalData);
            }
            else
            {
              while (lightmapAdditionalDataList.Count < index2 + 1)
                lightmapAdditionalDataList.Add(new ftLightmaps.LightmapAdditionalData());
              lightmapAdditionalDataList[index2] = lightmapAdditionalData;
            }
          }
        }
      }
    }
    else
    {
      for (int index = 0; index < storage.maps.Count; ++index)
        storage.idremap[index] = index;
    }
    if (flag2 && flag1)
      LightmapSettings.lightmaps = lightmaps;
    if (!flag2)
    {
      LightmapSettings.lightmaps = lightmapDataList.ToArray();
      ftLightmaps.globalMapsAdditional = lightmapAdditionalDataList;
    }
    if (RenderSettings.ambientMode == AmbientMode.Skybox)
    {
      SphericalHarmonicsL2 ambientProbe = RenderSettings.ambientProbe;
      int num1 = -1;
      for (int rgb = 0; rgb < 3; ++rgb)
      {
        for (int coefficient = 0; coefficient < 9; ++coefficient)
        {
          float num2 = Mathf.Abs(ambientProbe[rgb, coefficient]);
          if ((double) num2 > 1000.0 || (double) num2 < 9.99999997475243E-07)
          {
            num1 = 1;
            break;
          }
          if ((double) ambientProbe[rgb, coefficient] != 0.0)
          {
            num1 = 0;
            break;
          }
        }
        if (num1 >= 0)
          break;
      }
      if (num1 != 0)
        DynamicGI.UpdateEnvironment();
    }
    Vector4 vector4_1 = new Vector4(1f, 1f, 0.0f, 0.0f);
    for (int index1 = 0; index1 < storage.bakedRenderers.Count; ++index1)
    {
      Renderer bakedRenderer = storage.bakedRenderers[index1];
      if (!((Object) bakedRenderer == (Object) null))
      {
        int bakedId = storage.bakedIDs[index1];
        Mesh mesh = (Mesh) null;
        if (index1 < storage.bakedVertexColorMesh.Count)
          mesh = storage.bakedVertexColorMesh[index1];
        if ((Object) mesh != (Object) null)
        {
          MeshRenderer meshRenderer = bakedRenderer as MeshRenderer;
          if ((Object) meshRenderer == (Object) null)
          {
            Debug.LogError((object) "Unity cannot use additionalVertexStreams on non-MeshRenderer");
          }
          else
          {
            meshRenderer.additionalVertexStreams = mesh;
            meshRenderer.lightmapIndex = (int) ushort.MaxValue;
            MaterialPropertyBlock properties = new MaterialPropertyBlock();
            properties.SetFloat("bakeryLightmapMode", 1f);
            meshRenderer.SetPropertyBlock(properties);
          }
        }
        else
        {
          int index2 = bakedId < 0 || bakedId >= storage.idremap.Length ? bakedId : storage.idremap[bakedId];
          bakedRenderer.lightmapIndex = index2;
          if (!bakedRenderer.isPartOfStaticBatch)
          {
            Vector4 vector4_2 = bakedId >= 0 ? storage.bakedScaleOffset[index1] : vector4_1;
            bakedRenderer.lightmapScaleOffset = vector4_2;
          }
          if (bakedRenderer.lightmapIndex >= 0 && index2 < ftLightmaps.globalMapsAdditional.Count)
          {
            ftLightmaps.LightmapAdditionalData lightmapAdditionalData = ftLightmaps.globalMapsAdditional[index2];
            if ((Object) lightmapAdditionalData.rnm0 != (Object) null)
            {
              MaterialPropertyBlock properties = new MaterialPropertyBlock();
              properties.SetTexture("_RNM0", (Texture) lightmapAdditionalData.rnm0);
              properties.SetTexture("_RNM1", (Texture) lightmapAdditionalData.rnm1);
              properties.SetTexture("_RNM2", (Texture) lightmapAdditionalData.rnm2);
              properties.SetFloat("bakeryLightmapMode", (float) lightmapAdditionalData.mode);
              bakedRenderer.SetPropertyBlock(properties);
            }
          }
        }
      }
    }
    if (updateNonBaked)
    {
      for (int index = 0; index < storage.nonBakedRenderers.Count; ++index)
      {
        Renderer nonBakedRenderer = storage.nonBakedRenderers[index];
        if (!((Object) nonBakedRenderer == (Object) null) && !nonBakedRenderer.isPartOfStaticBatch)
          nonBakedRenderer.lightmapIndex = 65534;
      }
    }
    for (int index1 = 0; index1 < storage.bakedRenderersTerrain.Count; ++index1)
    {
      Terrain terrain = storage.bakedRenderersTerrain[index1];
      if (!((Object) terrain == (Object) null))
      {
        int index2 = storage.bakedIDsTerrain[index1];
        terrain.lightmapIndex = index2 < 0 || index2 >= storage.idremap.Length ? index2 : storage.idremap[index2];
        Vector4 vector4_2 = index2 >= 0 ? storage.bakedScaleOffsetTerrain[index1] : vector4_1;
        terrain.lightmapScaleOffset = vector4_2;
      }
    }
    int num3 = 0;
    while (num3 < storage.bakedLights.Count)
      ++num3;
    if (ftLightmaps.lightmapRefCount == null)
      ftLightmaps.lightmapRefCount = new List<int>();
    for (int index1 = 0; index1 < storage.idremap.Length; ++index1)
    {
      int index2 = storage.idremap[index1];
      while (ftLightmaps.lightmapRefCount.Count <= index2)
        ftLightmaps.lightmapRefCount.Add(0);
      if (ftLightmaps.lightmapRefCount[index2] < 0)
        ftLightmaps.lightmapRefCount[index2] = 0;
      List<int> lightmapRefCount;
      int index3;
      (lightmapRefCount = ftLightmaps.lightmapRefCount)[index3 = index2] = lightmapRefCount[index3] + 1;
    }
  }

  public static void UnloadScene(ftLightmapsStorage storage)
  {
    if (ftLightmaps.lightmapRefCount == null || storage.idremap == null)
      return;
    LightmapData[] lightmapDataArray = (LightmapData[]) null;
    List<ftLightmaps.LightmapAdditionalData> lightmapAdditionalDataList = (List<ftLightmaps.LightmapAdditionalData>) null;
    for (int index1 = 0; index1 < storage.idremap.Length; ++index1)
    {
      int index2 = storage.idremap[index1];
      if (index2 != 0 && ftLightmaps.lightmapRefCount.Count > index2)
      {
        List<int> lightmapRefCount;
        int index3;
        (lightmapRefCount = ftLightmaps.lightmapRefCount)[index3 = index2] = lightmapRefCount[index3] - 1;
        if (ftLightmaps.lightmapRefCount[index2] == 0)
        {
          if (lightmapDataArray == null)
            lightmapDataArray = LightmapSettings.lightmaps;
          if (lightmapDataArray.Length > index2)
          {
            lightmapDataArray[index2].lightmapColor = (Texture2D) null;
            lightmapDataArray[index2].lightmapDir = (Texture2D) null;
            lightmapDataArray[index2].shadowMask = (Texture2D) null;
            if (lightmapAdditionalDataList == null)
              lightmapAdditionalDataList = ftLightmaps.globalMapsAdditional;
            if (lightmapAdditionalDataList != null && lightmapAdditionalDataList.Count > index2)
            {
              ftLightmaps.LightmapAdditionalData lightmapAdditionalData = new ftLightmaps.LightmapAdditionalData();
              lightmapAdditionalDataList[index2] = lightmapAdditionalData;
            }
          }
        }
      }
    }
    if (lightmapDataArray == null)
      return;
    LightmapSettings.lightmaps = lightmapDataArray;
  }

  public static void RefreshScene2(Scene scene, ftLightmapsStorage storage)
  {
    for (int index = 0; index < storage.bakedRenderers.Count; ++index)
    {
      Renderer bakedRenderer = storage.bakedRenderers[index];
      if (!((Object) bakedRenderer == (Object) null))
      {
        int bakedId = storage.bakedIDs[index];
        bakedRenderer.lightmapIndex = bakedId < 0 || bakedId >= storage.idremap.Length ? bakedId : storage.idremap[bakedId];
      }
    }
    for (int index1 = 0; index1 < storage.bakedRenderersTerrain.Count; ++index1)
    {
      Terrain terrain = storage.bakedRenderersTerrain[index1];
      if (!((Object) terrain == (Object) null))
      {
        int index2 = storage.bakedIDsTerrain[index1];
        terrain.lightmapIndex = index2 < 0 || index2 >= storage.idremap.Length ? index2 : storage.idremap[index2];
      }
    }
  }

  private struct LightmapAdditionalData
  {
    public Texture2D rnm0;
    public Texture2D rnm1;
    public Texture2D rnm2;
    public int mode;
  }
}
