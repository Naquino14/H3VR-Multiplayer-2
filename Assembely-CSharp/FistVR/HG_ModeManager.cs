using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class HG_ModeManager : MonoBehaviour
	{
		public enum HG_Mode
		{
			None,
			TargetRelay_Sprint,
			TargetRelay_Jog,
			TargetRelay_Marathon,
			AssaultNPepper_Skirmish,
			AssaultNPepper_Brawl,
			AssaultNPepper_Maelstrom,
			MeatNMetal_Neophyte,
			MeatNMetal_Warrior,
			MeatNMetal_Veteran,
			BattlePetite_Open,
			BattlePetite_Sosiggun,
			BattlePetite_Melee,
			KingOfTheGrill_Invasion,
			KingOfTheGrill_Resurrection,
			KingOfTheGrill_Anachronism,
			MeatleGear_Open,
			MeatleGear_ScavengingSnake,
			MeatleGear_ThirdSnake
		}

		public HG_GameManager M;

		protected bool IsPlaying;

		protected HG_Mode m_mode;

		protected Vector3 InitialRespawnPos;

		protected Quaternion InitialRespawnRot;

		public virtual void InitMode(HG_Mode mode)
		{
		}

		public virtual bool IsModeComplete()
		{
			return true;
		}

		public virtual void HandlePlayerDeath()
		{
		}

		public virtual void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
		{
			M.IsNoLongerPlaying();
			M.FadeOutMusic();
			if (doesInvokeTeleport)
			{
				Invoke("TeleportPlayerBackAndRegisterScore", 5f);
			}
			if (immediateTeleportBackAndScore)
			{
				M.EndModeScore();
			}
		}

		private void TeleportPlayerBackAndRegisterScore()
		{
			GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.DeathResetPoint.position, isAbsolute: true, GM.CurrentSceneSettings.DeathResetPoint.forward);
			M.EndModeScore();
		}

		public virtual int GetScore()
		{
			return 0;
		}

		public virtual List<string> GetScoringReadOuts()
		{
			return null;
		}

		public virtual void TargetDestroyed(HG_Target t)
		{
		}
	}
}
