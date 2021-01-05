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
			Data.PrimeDataLists(Flags);
		}

		public virtual FVREntityProxyData SaveEntityProxyData()
		{
			Data.Position = base.transform.position;
			Data.EulerAngles = base.transform.eulerAngles;
			return Data;
		}

		public virtual void DecodeFromProxyData(FVREntityProxyData proxyData)
		{
			Data = proxyData;
			base.transform.position = Data.Position;
			base.transform.eulerAngles = Data.EulerAngles;
		}

		public virtual void UpdateProxyState()
		{
		}

		public virtual void SetBool(bool b, int index)
		{
			if (Data.StoredBools.Length > index)
			{
				Data.StoredBools[index] = b;
				UpdateProxyState();
			}
		}

		public virtual void SetInt(int a, int index)
		{
			if (Data.StoredInts.Length > index)
			{
				a = Mathf.Clamp(a, Flags.IntFlags[index].MinValue, Flags.IntFlags[index].MaxValue);
				Data.StoredInts[index] = a;
				UpdateProxyState();
			}
		}

		public virtual void SetVector4(Vector4 a, int index)
		{
			if (Data.StoredVector4s.Length > index)
			{
				a = new Vector3(Mathf.Clamp(a.x, Flags.Vector4Flags[index].MinValues.x, Flags.Vector4Flags[index].MaxValues.x), Mathf.Clamp(a.y, Flags.Vector4Flags[index].MinValues.y, Flags.Vector4Flags[index].MaxValues.y), Mathf.Clamp(a.z, Flags.Vector4Flags[index].MinValues.z, Flags.Vector4Flags[index].MaxValues.z));
				Data.StoredVector4s[index] = a;
				UpdateProxyState();
			}
		}

		public virtual void SetString(string a, int index)
		{
			if (Data.StoredStrings.Length > index)
			{
				a = a.Substring(0, Flags.StringFlags[index].MaxLength);
				Data.StoredStrings[index] = a;
				UpdateProxyState();
			}
		}
	}
}
