using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Dock
{
    /// <summary>
    /// 现在DockableOne成为原版脚本了，很多单一的改动直接在dockable上改动了
    /// 
    /// </summary>
    public class DockableOne : MonoBehaviour
    {
        #region 0.默认字段
        [SerializeField, ReadOnly]
        [Tooltip("此可停靠对象相对于停靠的当前状态")]
        private DockingState dockingState = DockingState.Undocked;//默认状态为锁定状态
        public DockingState DockingState
        {
            get => DockingState;
        }

        [SerializeField]
        [Tooltip("Time to animate any move/scale into or out of the dock.")]
        private float moveLerpTime = 0.1f;//进入dock时间为0.1秒

        [SerializeField]
        [Tooltip("Time to animate an element when it's following the dock (use 0 for tight attachment)")]
        private float moveLerpTimeWhenDocked = 0.05f;//离开dock时间为0.05秒

        /// <summary>
        /// 如果此对象当前可以停靠，则为True，否则为false
        /// </summary>
        public bool CanDock => dockingState == DockingState.Undocked || dockingState == DockingState.Undocking;

        /// <summary>
        /// 如果此对象当前可以解锁，则为True，否则为false
        /// </summary>
        public bool CanUndock => dockingState == DockingState.Docked;

        // Constants常量
        private static readonly float distanceTolerance = 0.01f; // in meters ；距离公差
        private static readonly float angleTolerance = 3.0f; // in degrees
        private static readonly float scaleTolerance = 0.01f; // in percentage

        private DockPositionOne dockedPosition = null;//默认dock位置是空的
        private Vector3 dockedPositionScale = Vector3.one;

        private HashSet<DockPositionOne> overlappingPositions = new HashSet<DockPositionOne>();//重叠位置的离散表，Hashset的插入速度非常快

        private Vector3 originalScale = Vector3.one;//原始大小
        public bool isDragging = false;//是否拖拽

        #endregion

        #region 1.主要方法：Update（）、Dock（）、Undock（）、DockOrNotByBool（）

        /// <summary>
        /// 根据isDragging和dock状态，每帧（更新对象的位置角度大小）
        /// </summary>
        /// <mark>
        /// 1.如果正在拖拽对象，那么获取最近的自由dock位置
        /// 2.靠近dock位置后，确保二者位置正确
        /// 3.停止拖拽的时候，位置角度大小变为dock的数值
        /// 4.如果取消进入，那么对象的位置角度大小回归原本的数值
        /// </mark>
        public void Update()
        {
            DockOrNotByBool();


            if (isDragging && overlappingPositions.Count > 0)//如果正在拖拽对象并且重叠已使用位置数量大于零
            {
                Debug.Log("执行了isDragging && overlappingPositions.Count>0");
                var closestPosition = GetClosestPosition();//获取最近的dock位置
                if (closestPosition.IsOccupied)//如果最近的dock被占用，尝试去移动到最近的自由位置
                {
                    closestPosition.GetComponentInParent<Dock>().TryMoveToFreeSpaceOne(closestPosition);
                }
            }

            if (dockingState == DockingState.Docked || dockingState == DockingState.Docking)//如果是被锁定或者正在入dock过程中
            {
                Assert.IsNotNull(dockedPosition, "When a dockable is docked, its dockedPosition must be valid.");//某个对象处于锁定状态时，他的锁定位置必须有效
                Assert.AreEqual(dockedPosition.DockedObject, this, "When a dockable is docked, its dockedPosition reference the dockable.");//比较两个引用是不是指向同一对象

                var lerpTime = dockingState == DockingState.Docked ? moveLerpTimeWhenDocked : moveLerpTime;//如果是docked状态，那么过渡时间是前者

                if (!isDragging)//如果没有在拖拽
                {
                    // Don't override dragging
                    //移动方法
                    transform.position = Solver.SmoothTo(transform.position, dockedPosition.transform.position, Time.deltaTime, lerpTime);//位置从本身位置过渡到dock位置
                    transform.rotation = Solver.SmoothTo(transform.rotation, dockedPosition.transform.rotation, Time.deltaTime, lerpTime);//角度从本身角度过渡到dock角度
                }

                transform.localScale = Solver.SmoothTo(transform.localScale, dockedPositionScale, Time.deltaTime, lerpTime);//大小从本身大小过渡到dock大小

                if (VectorExtensions.CloseEnough(dockedPosition.transform.position, transform.position, distanceTolerance) &&
                    QuaternionExtensions.AlignedEnough(dockedPosition.transform.rotation, transform.rotation, angleTolerance) &&
                    AboutTheSameSize(dockedPositionScale.x, transform.localScale.x))//当位置、角度、大小足够逼近的时候
                {
                    // Finished docking
                    //停止停靠状态
                    dockingState = DockingState.Docked;//状态切换为docked

                    // Snap to position（捕捉位置）将位置、角度、大小全部赋值为dock数据，完成进站
                    transform.position = dockedPosition.transform.position;
                    transform.rotation = dockedPosition.transform.rotation;
                    transform.localScale = dockedPositionScale;
                }
            }
            else if (dockedPosition == null && dockingState == DockingState.Undocking)//如果位置空并且状态为没有正在停靠，那么回到原来的大小
            {
                transform.localScale = Solver.SmoothTo(transform.localScale, originalScale, Time.deltaTime, moveLerpTime);

                if (AboutTheSameSize(originalScale.x, transform.localScale.x))
                {
                    // Finished undocking
                    dockingState = DockingState.Undocked;//在逼近却又离开dock的时候，切换为undocked状态并且回归到原本的大小

                    // Snap to size
                    transform.localScale = originalScale;
                }
            }

        }

        /// <summary>
        /// Dock:锁定对象到dockedPosition
        /// </summary>
        /// <mark>
        /// 将对象锁定在给定的 <see cref="DockPositionOne"/>.
        /// 如果能锁定，赋值位置大小，如果不锁定那么记录原始大小，将状态记为docking 
        /// </mark>
        /// <param name="position">The <see cref="DockPositionOne"/> 我们想把这个物体停靠在哪里</param>
        public void Dock(DockPositionOne position)
        {
            if (!CanDock)//如果不能锁定
            {
                Debug.LogError($"Trying to dock an object that was not undocked. State = {dockingState}");
                //试图dock一个不是undocked状态的对象
                return;
            }

            Debug.Log($"Docking object {gameObject.name} on position {position.gameObject.name}");//输出对象和位置的名称

            dockedPosition = position;//position是dock位置
            dockedPosition.DockedObject = this;//赋值当前停靠在此位置的对象
            float scaleToFit = gameObject.GetComponent<Collider>().bounds.GetScaleToFitInside(dockedPosition.GetComponent<Collider>().bounds);//缩放系数
            dockedPositionScale = transform.localScale * scaleToFit;//本身大小 * 缩放系数
            Debug.Log("大小缩放");
            if (dockingState == DockingState.Undocked)//如果没有锁定
            {
                // Only register the original scale when first docking
                //第一次docking的时候只记载原始大小
                originalScale = transform.localScale;
            }
            dockingState = DockingState.Docking;//状态转换为正在锁定
        }

        /// <summary>
        /// Undock：从dockedPosition解锁对象
        /// </summary>
        /// <mark>
        /// 将对象从当前dock位置解锁（移出dock位置）
        /// Undocks this <see cref="DockableOne"/> from the current <see cref="DockPositionOne"/> where it is docked.
        /// </mark>
        public void Undock()
        {
            if (!CanUndock)//能锁
            {
                Debug.LogError($"Trying to undock an object that was not docked. State = {dockingState}");//解锁
                return;
            }

            Debug.Log($"Undocking object {gameObject.name} from position {dockedPosition.gameObject.name}");//脱扣

            dockedPosition.DockedObject = null;//位置为空
            dockedPosition = null;
            dockedPositionScale = Vector3.one;//大小恢复
            dockingState = DockingState.Undocking;//正在锁定
        }


        /// <summary>
        /// 根据isDragging真假，进行锁定和解锁操作
        /// </summary>
        public void DockOrNotByBool()
        {

            if (isDragging == true)
            {
                if (CanUndock)//刚开始操作对象的时候，对象本身在锁定状态，如果可以解锁，那就解锁
                {
                    //Undock();
                    PUNUndock();//#
                }

            }
            //else//如果没有拖拽
            //{

            //    if (overlappingPositions.Count > 0 && CanDock)//如果重叠已经使用的位置数目大于0并且可以dock
            //    {

            //        var closestPosition = GetClosestPosition();//最近位置 

            //        if (closestPosition.IsOccupied)//如果最近的位置被占用，而且找到了合适的位置，那就停靠在合适的位置
            //        {
            //            if (!closestPosition.GetComponentInParent<Dock>().TryMoveToFreeSpaceOne(closestPosition))
            //            {
            //                return;
            //            }
            //        }
                    
            //        Dock(closestPosition);
                    
            //    }

            //}
        }

        public void EndToDOck()
        {
            isDragging = false;
            if (overlappingPositions.Count > 0 && CanDock)//如果重叠已经使用的位置数目大于0并且可以dock
            {

                var closestPosition = GetClosestPosition();//最近位置 

                if (closestPosition.IsOccupied)//如果最近的位置被占用，而且找到了合适的位置，那就停靠在合适的位置
                {
                    if (!closestPosition.GetComponentInParent<Dock>().TryMoveToFreeSpaceOne(closestPosition))
                    {
                        return;
                    }
                }

                Dock(closestPosition);

            }
        }
        #endregion

        #region 2.辅助方法：OnTriggerEnter/Exit()、(No)Dragging()、GetClosestPosition()、AboutTheSameSize()、GetBackOut()

        /// <summary>
        /// 碰撞事件：对象碰撞体进入dockposition碰撞体时的更新
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerEnter(Collider collider)
        {
            var dockPosition = collider.gameObject.GetComponent<DockPositionOne>();
            if (dockPosition != null)//如果dock位置有对象了，那么已使用列表增加一个位置
            {
                overlappingPositions.Add(dockPosition);
                Debug.Log($"{gameObject.name} collided with {dockPosition.name}");
            }
        }

        /// <summary>
        /// 碰撞事件：对象碰撞体离开dockposition碰撞体时的更新
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerExit(Collider collider)
        {
            var dockPosition = collider.gameObject.GetComponent<DockPositionOne>();
            if (overlappingPositions.Contains(dockPosition))//如果已使用列表包括这个位置，那么删除这个位置
            {
                overlappingPositions.Remove(dockPosition);
            }
        }

        /// <summary>
        /// 根据触摸状态，判断isDragging的真假
        /// </summary>
        public void Dragging()
        {
            isDragging = true;
        }
        public void NoDragging()
        {
            isDragging = false;
        }

        /// <summary>
        /// 获取最近的dock位置 
        /// </summary>
        /// <mark>
        /// GetClosestPosition方法
        /// Gets the overlapping <see cref="DockPositionOne"/> that is closest to this Dockable.
        /// </mark>
        /// <returns>The overlapping <see cref="DockPositionOne"/> that is closest to this <see cref="DockableOne"/>, or null if no positions overlap.</returns>
        private DockPositionOne GetClosestPosition()
        {
            var bounds = gameObject.GetComponent<Collider>().bounds;
            var minDistance = float.MaxValue;
            DockPositionOne closestPosition = null;
            foreach (var position in overlappingPositions)//遍历一遍重叠已使用的位置
            {
                var distance = (position.gameObject.GetComponent<Collider>().bounds.center - bounds.center).sqrMagnitude;//获取两个碰撞体中心距离，向量比较，sqrmagnitude是节约CPU的粗略比较
                if (closestPosition == null || distance < minDistance)//如果最近的位置没有或者距离小于最小值
                {
                    closestPosition = position;
                    minDistance = distance;
                }
            }

            return closestPosition;

        }

        /// <summary>
        /// 大概size的判断方法
        /// </summary>
        /// <param name="scale1"></param>
        /// <param name="scale2"></param>
        /// <returns></returns>
        private static bool AboutTheSameSize(float scale1, float scale2)
        {
            Assert.AreNotEqual(0.0f, scale2, "Cannot compare scales with an object that has scale zero.");
            return Mathf.Abs(scale1 / scale2 - 1.0f) < scaleTolerance;//比例差值小于大小公差
        }

        /// <summary>
        /// 将对象返回原来的位置
        /// </summary>



        #endregion

        #region 3.PUN网络同步
        public delegate void DockableOneDelegate();
        //public delegate void DockableDockDelegate(DockPosition position);
        private bool isPunEnabled;
        public bool IsPunEnabled
        {
            set => isPunEnabled = value;
        }
        
        private void PUNUndock()
        {
            if (isPunEnabled)
                OnPUNUndock?.Invoke();
            else
                Undock();
        }
        public event DockableOneDelegate OnPUNUndock;
        public void PUNEndToDock()
        {
            if (isPunEnabled)
                OnPUNEndToDock?.Invoke();
            else
                EndToDOck();
        }
        public event DockableOneDelegate OnPUNEndToDock;
        #endregion
    }
}
