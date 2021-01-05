using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New SosigSpeechSet", menuName = "Sosig/SosigSpeechSet", order = 0)]
	public class SosigSpeechSet : ScriptableObject
	{
		public float BasePitch = 1f;

		public float BaseVolume = 0.4f;

		public bool ForceDeathSpeech;

		public bool UseAltDeathOnHeadExplode;

		public bool LessTalkativeSkirmish;

		[Header("Pain Speech")]
		public List<AudioClip> OnJointBreak;

		public List<AudioClip> OnJointSlice;

		public List<AudioClip> OnJointSever;

		public List<AudioClip> OnDeath;

		public List<AudioClip> OnBackBreak;

		public List<AudioClip> OnNeckBreak;

		public List<AudioClip> OnPain;

		public List<AudioClip> OnConfusion;

		public List<AudioClip> OnDeathAlt;

		[Header("State Speech")]
		public List<AudioClip> OnWander;

		public List<AudioClip> OnSkirmish;

		public List<AudioClip> OnInvestigate;

		public List<AudioClip> OnSearchingForGuns;

		public List<AudioClip> OnTakingCover;

		public List<AudioClip> OnBeingAimedAt;

		public List<AudioClip> OnAssault;

		public List<AudioClip> OnReloading;

		public List<AudioClip> OnMedic;

		[Header("CallAndResponse")]
		public List<AudioClip> OnCall_Skirmish;

		public List<AudioClip> OnRespond_Skirmish;

		public List<AudioClip> OnCall_Assistance;

		public List<AudioClip> OnRespond_Assistance;

		public List<AudioClip> Test;

		[ContextMenu("dist")]
		public void dist()
		{
			for (int i = 0; i < Test.Count; i++)
			{
				if (Test[i].name.Contains("pain"))
				{
					OnJointBreak.Add(Test[i]);
					OnBackBreak.Add(Test[i]);
					OnNeckBreak.Add(Test[i]);
					OnPain.Add(Test[i]);
				}
				else if (Test[i].name.Contains("death"))
				{
					OnJointSlice.Add(Test[i]);
					OnJointSever.Add(Test[i]);
					OnDeath.Add(Test[i]);
				}
				else if (Test[i].name.Contains("panic"))
				{
					OnSearchingForGuns.Add(Test[i]);
				}
				else if (Test[i].name.Contains("combat"))
				{
					OnSkirmish.Add(Test[i]);
				}
				else if (Test[i].name.Contains("invest"))
				{
					OnInvestigate.Add(Test[i]);
				}
				else if (Test[i].name.Contains("aim"))
				{
					OnBeingAimedAt.Add(Test[i]);
				}
			}
		}
	}
}
