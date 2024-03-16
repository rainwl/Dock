using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Dock
{
    public class PUNDockableOne : MonoBehaviourPun
    {
        private DockableOne dockableOne;
        private void Start()
        {
            dockableOne = GetComponent<DockableOne>();
            dockableOne.OnPUNUndock += OnPUNUndockHandler;
            dockableOne.OnPUNEndToDock += OnPUNEndToDockHandler;

        }
        private void OnPUNUndockHandler()
        {
            photonView.RPC("PunRPC_PUNUndock1", RpcTarget.All);
        }
        [PunRPC]
        private void PunRPC_PUNUndock1()
        {
            dockableOne.Undock();
            Debug.Log("PUN同步解锁");
        }
        private void OnPUNEndToDockHandler()
        {
            photonView.RPC("PunRPC_PUNEndToDock1", RpcTarget.All);

        }
        [PunRPC]
        private void PunRPC_PUNEndToDock1()
        {
            dockableOne.EndToDOck();
        }


    }
}
       