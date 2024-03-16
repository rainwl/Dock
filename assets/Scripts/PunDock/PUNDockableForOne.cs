using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Dock
{
    public class PUNDockableForOne : MonoBehaviourPun
    {
        private DockableForOne dockableForOne;
        private DockPositionForOne position;
        private void Start()
        {
            dockableForOne = GetComponent<DockableForOne>();

            dockableForOne.OnPUNUdock += OnPUNUdockHandler;

            dockableForOne.OnPUNDock += OnPUNDockHandler;

            dockableForOne.IsPunEnabled = true;
        }

        private void OnPUNUdockHandler()
        {
            photonView.RPC("PunRPC_OnPUNUdock", RpcTarget.All);
        }
        [PunRPC]
        private void PunRPC_OnPUNUdock()
        {
            dockableForOne.Undockforone();
        }
        private void OnPUNDockHandler()
        {
            photonView.RPC("PunRPC_OnPUNDock", RpcTarget.All );

        }
        [PunRPC]
        private void PunRPC_OnPUNDock()
        {
            dockableForOne.Dockforone(position);
        }

    }
}
