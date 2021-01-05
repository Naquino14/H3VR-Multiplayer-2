using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DiscordPresence;

namespace H3VRMultiplayer2.DRP
{
    class DRP : MonoBehaviour
    {
        #pragma warning disable IDE0044

        public static DRP _DRP;
        [SerializeField]
        private string detail = "Test Detail";
        [SerializeField]
        private string state = "Test State";
        private long elapsedTime;

        private void Start()
        {
            GameObject DRPContainer = new GameObject();
            DRPContainer.AddComponent<DRP>();
            _DRP = DRPContainer.GetComponent<DRP>();
            Instantiate(DRPContainer);

            elapsedTime = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            PresenceManager.UpdatePresence(detail, state, elapsedTime, largeKey: "header");
        }
        public static void UpdateState(string _detail)
        {
            Debug.Log("Updating DRP");
            PresenceManager.UpdatePresence(_detail);
        }
    }
}
