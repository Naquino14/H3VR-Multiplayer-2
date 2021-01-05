using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New GridEquation List", menuName = "OmniSequencer/GridEquationList", order = 0)]
	public class OmniGridEquations : ScriptableObject
	{
		public List<Vector3> MultiplicationEquations;

		public List<int> MultiplesOf3;

		public List<int> MultiplesOf4;

		public List<int> Primes;
	}
}
