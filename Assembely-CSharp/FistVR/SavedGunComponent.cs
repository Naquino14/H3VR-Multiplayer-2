using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class SavedGunComponent
	{
		public int Index = -1;

		public string ObjectID = string.Empty;

		public Vector3 PosOffset = Vector3.zero;

		public Vector3 OrientationForward = Vector3.zero;

		public Vector3 OrientationUp = Vector3.zero;

		public int ObjectAttachedTo = -1;

		public int MountAttachedTo = -1;

		public bool isFirearm;

		public bool isMagazine;

		public bool isAttachment;

		public Dictionary<string, string> Flags = new Dictionary<string, string>();

		public void DebugPrintData()
		{
		}
	}
}
