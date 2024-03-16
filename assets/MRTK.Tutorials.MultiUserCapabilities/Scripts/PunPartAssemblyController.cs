using MRTK.Tutorials.GettingStarted;
using Photon.Pun;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    /// <summary>
    ///处理PartAssemblyController的PUN RPC。
    ///     Handles PUN RPC for PartAssemblyController.
    /// </summary>
    public class PunPartAssemblyController : MonoBehaviourPun
    {
        private PartAssemblyController partAssemblyController;
        
        private void Start()
        {
            // Cache references 缓存引用
            partAssemblyController = GetComponent<PartAssemblyController>();

            // 订阅PartAssemblyController事件Subscribe to PartAssemblyController events
            partAssemblyController.OnSetPlacement += OnSetPlacementHandler;
            partAssemblyController.OnResetPlacement += OnResetPlacementHandler;

            // Enable PUN feature
            partAssemblyController.IsPunEnabled = true;
        }

        private void OnSetPlacementHandler()
        {
            photonView.RPC("PunRPC_SetPlacement", RpcTarget.All);
        }

        [PunRPC]
        private void PunRPC_SetPlacement()
        {
            partAssemblyController.Set();
        }


        private void OnResetPlacementHandler()
        {
            photonView.RPC("PunRPC_ResetPlacement", RpcTarget.All);
        }

        [PunRPC]
        private void PunRPC_ResetPlacement()
        {
            partAssemblyController.Reset();
        }
    }
}
