// Decompiled with JetBrains decompiler
// Type: FistVR.FVREntityProxy
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVREntityProxy : FVRPhysicalObject
  {
    public FVREntityProxyData Data = new FVREntityProxyData();
    [Header("Entity Proxy Params")]
    public FVREntityFlagUsage Flags;
    private bool m_isLocked;

    protected override void Awake()
    {
      base.Awake();
      this.Data.PrimeDataLists(this.Flags);
    }

    public virtual FVREntityProxyData SaveEntityProxyData()
    {
      this.Data.Position = this.transform.position;
      this.Data.EulerAngles = this.transform.eulerAngles;
      return this.Data;
    }

    public virtual void DecodeFromProxyData(FVREntityProxyData proxyData)
    {
      this.Data = proxyData;
      this.transform.position = this.Data.Position;
      this.transform.eulerAngles = this.Data.EulerAngles;
    }

    public virtual void UpdateProxyState()
    {
    }

    public virtual void SetBool(bool b, int index)
    {
      if (this.Data.StoredBools.Length <= index)
        return;
      this.Data.StoredBools[index] = b;
      this.UpdateProxyState();
    }

    public virtual void SetInt(int a, int index)
    {
      if (this.Data.StoredInts.Length <= index)
        return;
      a = Mathf.Clamp(a, this.Flags.IntFlags[index].MinValue, this.Flags.IntFlags[index].MaxValue);
      this.Data.StoredInts[index] = a;
      this.UpdateProxyState();
    }

    public virtual void SetVector4(Vector4 a, int index)
    {
      if (this.Data.StoredVector4s.Length <= index)
        return;
      a = (Vector4) new Vector3(Mathf.Clamp(a.x, this.Flags.Vector4Flags[index].MinValues.x, this.Flags.Vector4Flags[index].MaxValues.x), Mathf.Clamp(a.y, this.Flags.Vector4Flags[index].MinValues.y, this.Flags.Vector4Flags[index].MaxValues.y), Mathf.Clamp(a.z, this.Flags.Vector4Flags[index].MinValues.z, this.Flags.Vector4Flags[index].MaxValues.z));
      this.Data.StoredVector4s[index] = a;
      this.UpdateProxyState();
    }

    public virtual void SetString(string a, int index)
    {
      if (this.Data.StoredStrings.Length <= index)
        return;
      a = a.Substring(0, this.Flags.StringFlags[index].MaxLength);
      this.Data.StoredStrings[index] = a;
      this.UpdateProxyState();
    }
  }
}
