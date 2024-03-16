using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PUNRot : MonoBehaviourPun
{
    private Rot rot;
    // Start is called before the first frame update
    void Start()
    {
        rot = GetComponent<Rot>();
        rot.OnPUNADDY += OnPUNADDYHandler;
        rot.OnPUNReduceY += OnPUNReduceYHandler;
        rot.IsPunEnabled = true;
    }
    private void OnPUNADDYHandler()
    {
        photonView.RPC("PunRPC_PUNADDY", RpcTarget.All);

    }
    private void OnPUNReduceYHandler()
    {
        photonView.RPC("PunRPC_PUNReduceY", RpcTarget.All);

    }
    [PunRPC]
    private void PunRPC_PUNADDY()
    {
        rot.ADDY();
    }
    [PunRPC]
    private void PunRPC_PUNReduceY()
    {
        rot.ReduceY();
    }
}
