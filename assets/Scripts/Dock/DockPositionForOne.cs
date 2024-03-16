using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Dock
{

    /// <summary>
    /// DockPositionForOne:可以用于停靠指定对象的DockPosition
    /// </summary>
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class DockPositionForOne : MonoBehaviourPun
    {
        public delegate void DockPositionForOneDelegate();
        #region 默认字段区域 

        #region dockedObject：属性，锁定的对象（默认为空）
        [SerializeField]
        private DockableForOne dockedObject = null;
        public DockableForOne DockedObject
        {
            get => dockedObject;
            set => dockedObject = value;
        }
        #endregion

        public bool IsOccupied => dockedObject != null;
        #endregion

        #region 同步字段
        private bool isPunEnabled;//是否支持PUN
        public bool IsPunEnabled
        {
            set => isPunEnabled = value;
        }

        #endregion

        #region 默认方法
        public void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");//防止碰撞
            var collider = gameObject.GetComponent<Collider>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<BoxCollider>();
            }
            collider.isTrigger = true;

            var rigidBody = gameObject.EnsureComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        public void Start()
        {
            if (dockedObject != null)
            {
                dockedObject.Dockforone(this);
            }
        }
        private void func()
        {
            photonView.RPC("Update", RpcTarget.All);
        }

        [PunRPC]
        private void Update()
        {
            if (DockableForOne.IsRight == true)
            {
                dockedObject = null;

                Debug.Log("dockedOjbect为空");
            }
            
        }
        /*
        public void Func()
        {
            if(dockedObject != null)
            {
                if (dockedObject.DockingState == DockingState.Undocking || dockedObject.DockingState == DockingState.Undocked)
                {
                    dockedObject = null;
                }
            }
            
        }

        private void PUNFunc()
        {
            if (isPunEnabled)
                OnPUNFunc?.Invoke();
            else
                Func();
        }
        public event DockPositionForOneDelegate OnPUNFunc;
        */
        #endregion


    }
}