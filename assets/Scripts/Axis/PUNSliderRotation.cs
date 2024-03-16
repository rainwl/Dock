using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PUNSliderRotation : MonoBehaviourPun
{
    private SliderRotation sliderRotation;
    // Start is called before the first frame update
    void Start()
    {
        sliderRotation = GetComponent<SliderRotation>();

        sliderRotation.OnPUNRota += OnPUNRotaHandler;

        sliderRotation.IsPunEnabled = true;
    }
    private void OnPUNRotaHandler()
    {
        photonView.RPC("PunRPC_PUNRota", RpcTarget.All);

    }
    [PunRPC]
    private void PunRPC_PUNRota()
    {
        sliderRotation.RoTa();
        Debug.Log("执行了PUNRPC——PUNrota");
    }

}
