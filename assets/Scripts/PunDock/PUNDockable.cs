using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Dock
{
    public class PUNDockable : MonoBehaviourPun
    {
        private Dockable dockable;
        private void Start()
        {
            dockable = GetComponent<Dockable>();

            dockable.OnPUNUndock += OnPUNUndockHandler;

            dockable.OnPUNDock += OnPUNDockHandler;

            dockable.OnPUNEndToDock += OnPUNEndToDockHandler;

            dockable.IsPunEnabled = true;

            posKey.Add(1,pos);
        }
        private void OnPUNUndockHandler()
        {
            photonView.RPC("PunRPC_PUNUndock", RpcTarget.All);
        }
        [PunRPC]
        private void PunRPC_PUNUndock()
        {
            dockable.Undock();
            Debug.Log("PUN同步解锁");
        }

        private Dictionary<int, DockPosition> posKey = new Dictionary<int, DockPosition>();
        private DockPosition pos;
        private void OnPUNDockHandler(DockPosition position)
        {
            photonView.RPC("PunRPC_PUNDock", RpcTarget.All , posKey[1]);

        }
        [PunRPC]
        private void PunRPC_PUNDock(DockPosition position)
        {
            dockable.Dock(position);
        }


        private void OnPUNEndToDockHandler()
        {
            photonView.RPC("PunRPC_PUNEndToDock", RpcTarget.All);

        }
        [PunRPC]
        private void PunRPC_PUNEndToDock()
        {
            dockable.EndToDock();
        }
    }
}
