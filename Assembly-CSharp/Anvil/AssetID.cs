// Decompiled with JetBrains decompiler
// Type: Anvil.AssetID
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace Anvil
{
  [Serializable]
  public struct AssetID : IEquatable<AssetID>
  {
    public string Guid;
    public string Bundle;
    public string AssetName;

    public bool Equals(AssetID other) => string.Equals(this.Bundle, other.Bundle) && string.Equals(this.AssetName, other.AssetName);

    public override bool Equals(object obj) => !object.ReferenceEquals((object) null, obj) && obj is AssetID other && this.Equals(other);

    public override int GetHashCode() => (this.Bundle == null ? 0 : this.Bundle.GetHashCode()) * 397 ^ (this.AssetName == null ? 0 : this.AssetName.GetHashCode());
  }
}
