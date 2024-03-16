using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Dock
{
    public class PUNDock : MonoBehaviourPun
    {
        private Dock dock;
        void Start()
        {
            dock = GetComponent<Dock>();

            dock.OnUpdatePositions += OnUpdatePositionsHandler;

            dock.IsPunEnabled = true;
        }
        private void OnUpdatePositionsHandler()
        {
            photonView.RPC("PunRPC_OnUpdatePositionsHandler", RpcTarget.All);
        }
        [PunRPC]
        private void PunRPC_OnUpdatePositionsHandler()
        {
            dock.UpdatePositions();
            Debug.Log("执行了PUNDock");
        }
    }
}
