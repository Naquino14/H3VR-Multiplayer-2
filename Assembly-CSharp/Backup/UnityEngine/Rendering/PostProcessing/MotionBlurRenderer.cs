// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.MotionBlurRenderer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class MotionBlurRenderer : PostProcessEffectRenderer<MotionBlur>
  {
    public override DepthTextureMode GetCameraFlags() => DepthTextureMode.Depth | DepthTextureMode.MotionVectors;

    public override void Render(PostProcessRenderContext context)
    {
      CommandBuffer command = context.command;
      if (this.m_ResetHistory)
      {
        command.BlitFullscreenTriangle(context.source, context.destination);
        this.m_ResetHistory = false;
      }
      else
      {
        RenderTextureFormat format1 = RenderTextureFormat.RGHalf;
        RenderTextureFormat format2 = !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB2101010) ? RenderTextureFormat.ARGB32 : RenderTextureFormat.ARGB2101010;
        PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.motionBlur);
        command.BeginSample("MotionBlur");
        int num1 = (int) (5.0 * (double) context.height / 100.0);
        int num2 = ((num1 - 1) / 8 + 1) * 8;
        float num3 = (float) (ParameterOverride<float>) this.settings.shutterAngle / 360f;
        propertySheet.properties.SetFloat(ShaderIDs.VelocityScale, num3);
        propertySheet.properties.SetFloat(ShaderIDs.MaxBlurRadius, (float) num1);
        propertySheet.properties.SetFloat(ShaderIDs.RcpMaxBlurRadius, 1f / (float) num1);
        int velocityTex = ShaderIDs.VelocityTex;
        command.GetTemporaryRT(velocityTex, context.width, context.height, 0, FilterMode.Point, format2, RenderTextureReadWrite.Linear);
        command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) velocityTex, propertySheet, 0);
        int tile2Rt = ShaderIDs.Tile2RT;
        command.GetTemporaryRT(tile2Rt, context.width / 2, context.height / 2, 0, FilterMode.Point, format1, RenderTextureReadWrite.Linear);
        command.BlitFullscreenTriangle((RenderTargetIdentifier) velocityTex, (RenderTargetIdentifier) tile2Rt, propertySheet, 1);
        int tile4Rt = ShaderIDs.Tile4RT;
        command.GetTemporaryRT(tile4Rt, context.width / 4, context.height / 4, 0, FilterMode.Point, format1, RenderTextureReadWrite.Linear);
        command.BlitFullscreenTriangle((RenderTargetIdentifier) tile2Rt, (RenderTargetIdentifier) tile4Rt, propertySheet, 2);
        command.ReleaseTemporaryRT(tile2Rt);
        int tile8Rt = ShaderIDs.Tile8RT;
        command.GetTemporaryRT(tile8Rt, context.width / 8, context.height / 8, 0, FilterMode.Point, format1, RenderTextureReadWrite.Linear);
        command.BlitFullscreenTriangle((RenderTargetIdentifier) tile4Rt, (RenderTargetIdentifier) tile8Rt, propertySheet, 2);
        command.ReleaseTemporaryRT(tile4Rt);
        Vector2 vector2 = Vector2.one * (float) ((double) num2 / 8.0 - 1.0) * -0.5f;
        propertySheet.properties.SetVector(ShaderIDs.TileMaxOffs, (Vector4) vector2);
        propertySheet.properties.SetFloat(ShaderIDs.TileMaxLoop, (float) (int) ((double) num2 / 8.0));
        int tileVrt = ShaderIDs.TileVRT;
        command.GetTemporaryRT(tileVrt, context.width / num2, context.height / num2, 0, FilterMode.Point, format1, RenderTextureReadWrite.Linear);
        command.BlitFullscreenTriangle((RenderTargetIdentifier) tile8Rt, (RenderTargetIdentifier) tileVrt, propertySheet, 3);
        command.ReleaseTemporaryRT(tile8Rt);
        int neighborMaxTex = ShaderIDs.NeighborMaxTex;
        int width = context.width / num2;
        int height = context.height / num2;
        command.GetTemporaryRT(neighborMaxTex, width, height, 0, FilterMode.Point, format1, RenderTextureReadWrite.Linear);
        command.BlitFullscreenTriangle((RenderTargetIdentifier) tileVrt, (RenderTargetIdentifier) neighborMaxTex, propertySheet, 4);
        command.ReleaseTemporaryRT(tileVrt);
        propertySheet.properties.SetFloat(ShaderIDs.LoopCount, (float) Mathf.Clamp((int) (ParameterOverride<int>) this.settings.sampleCount / 2, 1, 64));
        command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 5);
        command.ReleaseTemporaryRT(velocityTex);
        command.ReleaseTemporaryRT(neighborMaxTex);
        command.EndSample("MotionBlur");
      }
    }

    private enum Pass
    {
      VelocitySetup,
      TileMax1,
      TileMax2,
      TileMaxV,
      NeighborMax,
      Reconstruction,
    }
  }
}
