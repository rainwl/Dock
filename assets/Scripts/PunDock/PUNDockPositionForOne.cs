using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Dock
{
    public class PUNDockPositionForOne : MonoBehaviourPun
    {
        private DockPositionForOne dockPositionForOne;
        private void Start()
        {
            dockPositionForOne = GetComponent<DockPositionForOne>();

            dockPositionForOne.IsPunEnabled = true;

            //dockPositionForOne.OnPUNFunc += OnPUNFuncHandler;
        }
        private void OnPUNFuncHandler()
        {
            photonView.RPC("PunRPC_OnPUNFunc", RpcTarget.All);

        }
        [PunRPC]
        private void PunRPC_OnPUNFunc()
        {
            //dockPositionForOne.Func();
        }
    }
}
