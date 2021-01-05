// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Source_Map`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Valve.VR
{
  public abstract class SteamVR_Action_Source_Map<SourceElement> : SteamVR_Action_Source_Map
    where SourceElement : SteamVR_Action_Source, new()
  {
    protected Dictionary<SteamVR_Input_Sources, SourceElement> sources = new Dictionary<SteamVR_Input_Sources, SourceElement>((IEqualityComparer<SteamVR_Input_Sources>) new SteamVR_Input_Sources_Comparer());

    public SourceElement this[SteamVR_Input_Sources inputSource] => this.GetSourceElementForIndexer(inputSource);

    protected virtual void OnAccessSource(SteamVR_Input_Sources inputSource)
    {
    }

    public override void Initialize()
    {
      base.Initialize();
      Dictionary<SteamVR_Input_Sources, SourceElement>.Enumerator enumerator = this.sources.GetEnumerator();
      while (enumerator.MoveNext())
        enumerator.Current.Value.Initialize();
    }

    protected override void PreinitializeMap(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action wrappingAction)
    {
      this.sources.Add(inputSource, new SourceElement());
      this.sources[inputSource].Preinitialize(wrappingAction, inputSource);
    }

    protected virtual SourceElement GetSourceElementForIndexer(SteamVR_Input_Sources inputSource)
    {
      this.OnAccessSource(inputSource);
      return this.sources[inputSource];
    }
  }
}
