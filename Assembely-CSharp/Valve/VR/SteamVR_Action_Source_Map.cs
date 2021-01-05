using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
	public abstract class SteamVR_Action_Source_Map<SourceElement> : SteamVR_Action_Source_Map where SourceElement : SteamVR_Action_Source, new()
	{
		protected Dictionary<SteamVR_Input_Sources, SourceElement> sources = new Dictionary<SteamVR_Input_Sources, SourceElement>(default(SteamVR_Input_Sources_Comparer));

		public SourceElement this[SteamVR_Input_Sources inputSource] => GetSourceElementForIndexer(inputSource);

		protected virtual void OnAccessSource(SteamVR_Input_Sources inputSource)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Dictionary<SteamVR_Input_Sources, SourceElement>.Enumerator enumerator = sources.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SourceElement value = enumerator.Current.Value;
				value.Initialize();
			}
		}

		protected override void PreinitializeMap(SteamVR_Input_Sources inputSource, SteamVR_Action wrappingAction)
		{
			sources.Add(inputSource, new SourceElement());
			SourceElement val = sources[inputSource];
			val.Preinitialize(wrappingAction, inputSource);
		}

		protected virtual SourceElement GetSourceElementForIndexer(SteamVR_Input_Sources inputSource)
		{
			OnAccessSource(inputSource);
			return sources[inputSource];
		}
	}
	public abstract class SteamVR_Action_Source_Map
	{
		public SteamVR_Action action;

		public string fullPath
		{
			get;
			protected set;
		}

		public ulong handle
		{
			get;
			protected set;
		}

		public SteamVR_ActionSet actionSet
		{
			get;
			protected set;
		}

		public SteamVR_ActionDirections direction
		{
			get;
			protected set;
		}

		public virtual void PreInitialize(SteamVR_Action wrappingAction, string actionPath, bool throwErrors = true)
		{
			fullPath = actionPath;
			action = wrappingAction;
			actionSet = SteamVR_Input.GetActionSetFromPath(GetActionSetPath());
			direction = GetActionDirection();
			SteamVR_Input_Sources[] allSources = SteamVR_Input_Source.GetAllSources();
			for (int i = 0; i < allSources.Length; i++)
			{
				PreinitializeMap(allSources[i], wrappingAction);
			}
		}

		protected abstract void PreinitializeMap(SteamVR_Input_Sources inputSource, SteamVR_Action wrappingAction);

		public virtual void Initialize()
		{
			ulong pHandle = 0uL;
			EVRInputError actionHandle = OpenVR.Input.GetActionHandle(fullPath.ToLower(), ref pHandle);
			handle = pHandle;
			if (actionHandle != 0)
			{
				Debug.LogError("<b>[SteamVR]</b> GetActionHandle (" + fullPath.ToLower() + ") error: " + actionHandle);
			}
		}

		private string GetActionSetPath()
		{
			int num = fullPath.IndexOf('/', 1);
			int startIndex = num + 1;
			int num2 = fullPath.IndexOf('/', startIndex);
			int length = num2;
			return fullPath.Substring(0, length);
		}

		private SteamVR_ActionDirections GetActionDirection()
		{
			int num = fullPath.IndexOf('/', 1);
			int startIndex = num + 1;
			int num2 = fullPath.IndexOf('/', startIndex);
			int num3 = fullPath.IndexOf('/', num2 + 1);
			int length = num3 - num2 - 1;
			string text = fullPath.Substring(num2 + 1, length);
			if (text == "in")
			{
				return SteamVR_ActionDirections.In;
			}
			if (text == "out")
			{
				return SteamVR_ActionDirections.Out;
			}
			Debug.LogError("Could not find match for direction: " + text);
			return SteamVR_ActionDirections.In;
		}
	}
}
