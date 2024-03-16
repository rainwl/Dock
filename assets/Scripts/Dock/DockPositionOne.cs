using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dock
{
    public class DockPositionOne : MonoBehaviour
    {
        #region 0.默认字段
        /// <summary>
        /// 当前停靠在此位置的对象（可以为空）（私有）
        /// </summary>
        [SerializeField]
        [Tooltip("当前停靠在此位置的对象（可以为空）")]
        private DockableOne dockedObject = null;
        /// <summary>
        /// 当前停靠在此位置的对象(可以为空)（公有）
        /// </summary>
        public DockableOne DockedObject
        {
            get => dockedObject;
            set => dockedObject = value;
        }



        /// <summary>
        /// 如果此位置被占用，则为True，否则为false
        /// </summary>
        public bool IsOccupied => dockedObject != null;

        #endregion

        /// <summary>
        /// 1.确保这个物体有碰撞体默认为boxcollider和rigidbody, istrigger=true,isKinematic=true
        /// </summary>
        public void Awake()
        {
            // Don't raycast this object to prevent（防止） blocking collisions（阻塞碰撞）
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            // Ensure there's a trigger collider for this position
            // The shape can be customized, but this adds a box as default.
            var collider = gameObject.GetComponent<Collider>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<BoxCollider>();
            }

            collider.isTrigger = true;

            // Ensure this collider can be used as a trigger by having a RigidBody attached to it.
            var rigidBody = gameObject.EnsureComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }

        /// <summary>
        /// 2.如果某对象设置为在启动时锁定在此位置，请确保它已锁定(dock)。
        /// </summary>
        public void Start()
        {

            if (dockedObject != null)//如果当前具有Dockable组件的对象为空，那么具有Dockable组件的对象可以停靠进来
            {

                dockedObject.Dock(this);
            }



        }

    }
}
