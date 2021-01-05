// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Fade
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR
{
  public class SteamVR_Fade : MonoBehaviour
  {
    private Color currentColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    private Color targetColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    private Color deltaColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    private bool fadeOverlay;
    private static Material fadeMaterial;
    private static int fadeMaterialColorID = -1;

    public static void Start(Color newColor, float duration, bool fadeOverlay = false) => SteamVR_Events.Fade.Send(newColor, duration, fadeOverlay);

    public static void View(Color newColor, float duration) => OpenVR.Compositor?.FadeToColor(duration, newColor.r, newColor.g, newColor.b, newColor.a, false);

    public void OnStartFade(Color newColor, float duration, bool fadeOverlay)
    {
      if ((double) duration > 0.0)
      {
        this.targetColor = newColor;
        this.deltaColor = (this.targetColor - this.currentColor) / duration;
      }
      else
        this.currentColor = newColor;
    }

    private void OnEnable()
    {
      if ((Object) SteamVR_Fade.fadeMaterial == (Object) null)
      {
        SteamVR_Fade.fadeMaterial = new Material(Shader.Find("Custom/SteamVR_Fade"));
        SteamVR_Fade.fadeMaterialColorID = Shader.PropertyToID("fadeColor");
      }
      SteamVR_Events.Fade.Listen(new UnityAction<Color, float, bool>(this.OnStartFade));
      SteamVR_Events.FadeReady.Send();
    }

    private void OnDisable() => SteamVR_Events.Fade.Remove(new UnityAction<Color, float, bool>(this.OnStartFade));

    private void OnPostRender()
    {
      if (this.currentColor != this.targetColor)
      {
        if ((double) Mathf.Abs(this.currentColor.a - this.targetColor.a) < (double) Mathf.Abs(this.deltaColor.a) * (double) Time.deltaTime)
        {
          this.currentColor = this.targetColor;
          this.deltaColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        else
          this.currentColor += this.deltaColor * Time.deltaTime;
        if (this.fadeOverlay)
        {
          SteamVR_Overlay instance = SteamVR_Overlay.instance;
          if ((Object) instance != (Object) null)
            instance.alpha = 1f - this.currentColor.a;
        }
      }
      if ((double) this.currentColor.a <= 0.0 || !(bool) (Object) SteamVR_Fade.fadeMaterial)
        return;
      SteamVR_Fade.fadeMaterial.SetColor(SteamVR_Fade.fadeMaterialColorID, this.currentColor);
      SteamVR_Fade.fadeMaterial.SetPass(0);
      GL.Begin(7);
      GL.Vertex3(-1f, -1f, 0.0f);
      GL.Vertex3(1f, -1f, 0.0f);
      GL.Vertex3(1f, 1f, 0.0f);
      GL.Vertex3(-1f, 1f, 0.0f);
      GL.End();
    }
  }
}
