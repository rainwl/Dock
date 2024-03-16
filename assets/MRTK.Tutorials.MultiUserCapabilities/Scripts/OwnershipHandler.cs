using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class OwnershipHandler : MonoBehaviourPun, IPunOwnershipCallbacks
{   
    public void OnMouseEnter()
    {
        photonView.RequestOwnership();
    }
    public void OnMouseDown()
    {
        photonView.RequestOwnership();

    }
    public void SliderDrag()
    {
        photonView.RequestOwnership();
        Debug.Log("改变了Slider的控制权");

    }


    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        targetView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
    }

    private void TransferControl(Player idPlayer)
    {
        if (photonView.IsMine) photonView.TransferOwnership(idPlayer);
    }    

    public void RequestOwnership()
    {
        photonView.RequestOwnership();
    }
}

