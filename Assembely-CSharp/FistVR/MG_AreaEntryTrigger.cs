using UnityEngine;

namespace FistVR
{
	public class MG_AreaEntryTrigger : MonoBehaviour
	{
		public string Area;

		public RedRoom.Quadrant Quadrant;

		private void OnTriggerEnter()
		{
			if (Quadrant != GM.MGMaster.PlayerQuadrant)
			{
				GM.MGMaster.PlayerQuadrant = Quadrant;
				switch (Area)
				{
				case "boiler":
					GM.MGMaster.Narrator.PlayAreaEntryBoiler();
					break;
				case "office":
					GM.MGMaster.Narrator.PlayAreaEntryOffice();
					break;
				case "restaurant":
					GM.MGMaster.Narrator.PlayAreaEntryRestaurant();
					break;
				case "coldstorage":
					GM.MGMaster.Narrator.PlayAreaEntryColdStorage();
					break;
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
