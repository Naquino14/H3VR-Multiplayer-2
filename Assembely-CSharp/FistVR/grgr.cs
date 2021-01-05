using System.Diagnostics;
using UnityEngine;

namespace FistVR
{
	public class grgr : MonoBehaviour
	{
		private float t = 1f;

		private void Start()
		{
			gr();
		}

		private void gr()
		{
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				if (process.ProcessName.ToLower().Contains("cheat") && process.ProcessName.ToLower().Contains("engine"))
				{
					Application.Quit();
				}
			}
			if (GM.MMFlags.GB > 5000000)
			{
				GM.MMFlags.GB = 0;
				GM.MMFlags.SaveToFile();
				Application.Quit();
			}
		}
	}
}
