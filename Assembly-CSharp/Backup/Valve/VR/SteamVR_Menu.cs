// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Menu
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Menu : MonoBehaviour
  {
    public Texture cursor;
    public Texture background;
    public Texture logo;
    public float logoHeight;
    public float menuOffset;
    public Vector2 scaleLimits = new Vector2(0.1f, 5f);
    public float scaleRate = 0.5f;
    private SteamVR_Overlay overlay;
    private Camera overlayCam;
    private Vector4 uvOffset;
    private float distance;
    private string scaleLimitX;
    private string scaleLimitY;
    private string scaleRateText;
    private CursorLockMode savedCursorLockState;
    private bool savedCursorVisible;

    public RenderTexture texture => (bool) (UnityEngine.Object) this.overlay ? this.overlay.texture as RenderTexture : (RenderTexture) null;

    public float scale { get; private set; }

    private void Awake()
    {
      this.scaleLimitX = string.Format("{0:N1}", (object) this.scaleLimits.x);
      this.scaleLimitY = string.Format("{0:N1}", (object) this.scaleLimits.y);
      this.scaleRateText = string.Format("{0:N1}", (object) this.scaleRate);
      SteamVR_Overlay instance = SteamVR_Overlay.instance;
      if (!((UnityEngine.Object) instance != (UnityEngine.Object) null))
        return;
      this.uvOffset = instance.uvOffset;
      this.distance = instance.distance;
    }

    private void OnGUI()
    {
      if ((UnityEngine.Object) this.overlay == (UnityEngine.Object) null)
        return;
      RenderTexture texture = this.overlay.texture as RenderTexture;
      RenderTexture active = RenderTexture.active;
      RenderTexture.active = texture;
      if (UnityEngine.Event.current.type == EventType.Repaint)
        GL.Clear(false, true, Color.clear);
      Rect screenRect = new Rect(0.0f, 0.0f, (float) texture.width, (float) texture.height);
      if (Screen.width < texture.width)
      {
        screenRect.width = (float) Screen.width;
        this.overlay.uvOffset.x = (float) -(double) (texture.width - Screen.width) / (float) (2 * texture.width);
      }
      if (Screen.height < texture.height)
      {
        screenRect.height = (float) Screen.height;
        this.overlay.uvOffset.y = (float) (texture.height - Screen.height) / (float) (2 * texture.height);
      }
      GUILayout.BeginArea(screenRect);
      if ((UnityEngine.Object) this.background != (UnityEngine.Object) null)
        GUI.DrawTexture(new Rect((float) (((double) screenRect.width - (double) this.background.width) / 2.0), (float) (((double) screenRect.height - (double) this.background.height) / 2.0), (float) this.background.width, (float) this.background.height), this.background);
      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.BeginVertical();
      if ((UnityEngine.Object) this.logo != (UnityEngine.Object) null)
      {
        GUILayout.Space(screenRect.height / 2f - this.logoHeight);
        GUILayout.Box(this.logo);
      }
      GUILayout.Space(this.menuOffset);
      bool flag = GUILayout.Button("[Esc] - Close menu");
      GUILayout.BeginHorizontal();
      GUILayout.Label(string.Format("Scale: {0:N4}", (object) this.scale));
      float scale = GUILayout.HorizontalSlider(this.scale, this.scaleLimits.x, this.scaleLimits.y);
      if ((double) scale != (double) this.scale)
        this.SetScale(scale);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label(string.Format("Scale limits:"));
      string s1 = GUILayout.TextField(this.scaleLimitX);
      if (s1 != this.scaleLimitX && float.TryParse(s1, out this.scaleLimits.x))
        this.scaleLimitX = s1;
      string s2 = GUILayout.TextField(this.scaleLimitY);
      if (s2 != this.scaleLimitY && float.TryParse(s2, out this.scaleLimits.y))
        this.scaleLimitY = s2;
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label(string.Format("Scale rate:"));
      string s3 = GUILayout.TextField(this.scaleRateText);
      if (s3 != this.scaleRateText && float.TryParse(s3, out this.scaleRate))
        this.scaleRateText = s3;
      GUILayout.EndHorizontal();
      if (SteamVR.active)
      {
        SteamVR instance = SteamVR.instance;
        GUILayout.BeginHorizontal();
        float sceneResolutionScale = SteamVR_Camera.sceneResolutionScale;
        int num1 = (int) ((double) instance.sceneWidth * (double) sceneResolutionScale);
        int num2 = (int) ((double) instance.sceneHeight * (double) sceneResolutionScale);
        int num3 = (int) (100.0 * (double) sceneResolutionScale);
        GUILayout.Label(string.Format("Scene quality: {0}x{1} ({2}%)", (object) num1, (object) num2, (object) num3));
        int num4 = Mathf.RoundToInt(GUILayout.HorizontalSlider((float) num3, 50f, 200f));
        if (num4 != num3)
          SteamVR_Camera.sceneResolutionScale = (float) num4 / 100f;
        GUILayout.EndHorizontal();
      }
      this.overlay.highquality = GUILayout.Toggle(this.overlay.highquality, "High quality");
      if (this.overlay.highquality)
      {
        this.overlay.curved = GUILayout.Toggle(this.overlay.curved, "Curved overlay");
        this.overlay.antialias = GUILayout.Toggle(this.overlay.antialias, "Overlay RGSS(2x2)");
      }
      else
      {
        this.overlay.curved = false;
        this.overlay.antialias = false;
      }
      SteamVR_Camera steamVrCamera = SteamVR_Render.Top();
      if ((UnityEngine.Object) steamVrCamera != (UnityEngine.Object) null)
      {
        steamVrCamera.wireframe = GUILayout.Toggle(steamVrCamera.wireframe, "Wireframe");
        if (SteamVR.settings.trackingSpace == ETrackingUniverseOrigin.TrackingUniverseSeated)
        {
          if (GUILayout.Button("Switch to Standing"))
            SteamVR.settings.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
          if (GUILayout.Button("Center View"))
            OpenVR.System?.ResetSeatedZeroPose();
        }
        else if (GUILayout.Button("Switch to Seated"))
          SteamVR.settings.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseSeated;
      }
      if (GUILayout.Button("Exit"))
        Application.Quit();
      GUILayout.Space(this.menuOffset);
      string environmentVariable = Environment.GetEnvironmentVariable("VR_OVERRIDE");
      if (environmentVariable != null)
        GUILayout.Label("VR_OVERRIDE=" + environmentVariable);
      GUILayout.Label("Graphics device: " + SystemInfo.graphicsDeviceVersion);
      GUILayout.EndVertical();
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.EndArea();
      if ((UnityEngine.Object) this.cursor != (UnityEngine.Object) null)
        GUI.DrawTexture(new Rect(Input.mousePosition.x, (float) Screen.height - Input.mousePosition.y, (float) this.cursor.width, (float) this.cursor.height), this.cursor);
      RenderTexture.active = active;
      if (!flag)
        return;
      this.HideMenu();
    }

    public void ShowMenu()
    {
      SteamVR_Overlay instance = SteamVR_Overlay.instance;
      if ((UnityEngine.Object) instance == (UnityEngine.Object) null)
        return;
      RenderTexture texture = instance.texture as RenderTexture;
      if ((UnityEngine.Object) texture == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "<b>[SteamVR]</b> Menu requires overlay texture to be a render texture.");
      }
      else
      {
        this.SaveCursorState();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        this.overlay = instance;
        this.uvOffset = instance.uvOffset;
        this.distance = instance.distance;
        foreach (Camera camera in UnityEngine.Object.FindObjectsOfType(typeof (Camera)) as Camera[])
        {
          if (camera.enabled && (UnityEngine.Object) camera.targetTexture == (UnityEngine.Object) texture)
          {
            this.overlayCam = camera;
            this.overlayCam.enabled = false;
            break;
          }
        }
        SteamVR_Camera steamVrCamera = SteamVR_Render.Top();
        if (!((UnityEngine.Object) steamVrCamera != (UnityEngine.Object) null))
          return;
        this.scale = steamVrCamera.origin.localScale.x;
      }
    }

    public void HideMenu()
    {
      this.RestoreCursorState();
      if ((UnityEngine.Object) this.overlayCam != (UnityEngine.Object) null)
        this.overlayCam.enabled = true;
      if (!((UnityEngine.Object) this.overlay != (UnityEngine.Object) null))
        return;
      this.overlay.uvOffset = this.uvOffset;
      this.overlay.distance = this.distance;
      this.overlay = (SteamVR_Overlay) null;
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
      {
        if ((UnityEngine.Object) this.overlay == (UnityEngine.Object) null)
          this.ShowMenu();
        else
          this.HideMenu();
      }
      else if (Input.GetKeyDown(KeyCode.Home))
        this.SetScale(1f);
      else if (Input.GetKey(KeyCode.PageUp))
      {
        this.SetScale(Mathf.Clamp(this.scale + this.scaleRate * Time.deltaTime, this.scaleLimits.x, this.scaleLimits.y));
      }
      else
      {
        if (!Input.GetKey(KeyCode.PageDown))
          return;
        this.SetScale(Mathf.Clamp(this.scale - this.scaleRate * Time.deltaTime, this.scaleLimits.x, this.scaleLimits.y));
      }
    }

    private void SetScale(float scale)
    {
      this.scale = scale;
      SteamVR_Camera steamVrCamera = SteamVR_Render.Top();
      if (!((UnityEngine.Object) steamVrCamera != (UnityEngine.Object) null))
        return;
      steamVrCamera.origin.localScale = new Vector3(scale, scale, scale);
    }

    private void SaveCursorState()
    {
      this.savedCursorVisible = Cursor.visible;
      this.savedCursorLockState = Cursor.lockState;
    }

    private void RestoreCursorState()
    {
      Cursor.visible = this.savedCursorVisible;
      Cursor.lockState = this.savedCursorLockState;
    }
  }
}
