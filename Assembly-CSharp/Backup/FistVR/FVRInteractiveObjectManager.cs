// Decompiled with JetBrains decompiler
// Type: FistVR.FVRInteractiveObjectManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class FVRInteractiveObjectManager : ManagerSingleton<FVRInteractiveObjectManager>
  {
    private int executeFrameTick;

    protected override void Awake() => base.Awake();

    public void Update()
    {
      this.executeFrameTick = 0;
      FVRInteractiveObject.GlobalUpdate();
    }

    public void FixedUpdate()
    {
      if (this.executeFrameTick >= 2)
        return;
      ++this.executeFrameTick;
      FVRInteractiveObject.GlobalFixedUpdate();
    }
  }
}
