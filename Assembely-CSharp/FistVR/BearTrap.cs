using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class BearTrap : MonoBehaviour
	{
		public List<BearTrapInteractiblePiece> Pieces;

		private void Awake()
		{
			for (int i = 0; i < Pieces.Count; i++)
			{
				Pieces[i].SetBearTrap(this);
			}
		}

		public bool IsOpen()
		{
			if (Pieces[0].State == BearTrapInteractiblePiece.BearTrapState.Open && Pieces[1].State == BearTrapInteractiblePiece.BearTrapState.Open)
			{
				return true;
			}
			return false;
		}

		public void ThingDetected()
		{
			if (Pieces[0].CanSnapShut())
			{
				Pieces[0].SnapShut();
			}
			if (Pieces[1].CanSnapShut())
			{
				Pieces[1].SnapShut();
			}
		}

		public void ForceOpen()
		{
			Pieces[0].Open();
			Pieces[1].Open();
		}

		public void ForceClose()
		{
			Pieces[0].Close();
			Pieces[1].Close();
		}
	}
}
