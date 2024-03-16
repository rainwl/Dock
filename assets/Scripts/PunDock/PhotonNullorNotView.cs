using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Dock
{
    public class PhotonNullorNotView : MonoBehaviourPun, IPunObservable
    {
        
        //实现接口
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
            
        }
        


       
    }
}
