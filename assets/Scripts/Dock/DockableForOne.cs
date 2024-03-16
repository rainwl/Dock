using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Photon.Pun;

namespace Dock
{
    /// <summary>
    /// DockableForOne:挂载该组件对象具有dock在指定位置功能
    /// </summary>
    public class DockableForOne : MonoBehaviourPun 
    {
        public delegate void DockableForOneDelegate();

        #region 0.默认字段
        [SerializeField, ReadOnly]
        private DockingState dockingState = DockingState.Undocked;//默认状态为锁定状态
        public DockingState DockingState
        {
            get => DockingState;
        }
        [SerializeField]
        private float moveLerpTime = 0.1f;//进入dock时间为0.1秒
        [SerializeField]
        private float moveLerpTimeWhenDocked = 0.05f;//离开dock时间为0.05秒
        /// <summary>
        /// CanDock意味着当前对象的状态为Undocked 或者 Undocking
        /// </summary>
        public bool CanDock => dockingState == DockingState.Undocked || dockingState == DockingState.Undocking ;//CanDock意味着状态为undocked & undocking
        /// <summary>
        /// CanUndock意味着当前对象状态为Docked
        /// </summary>
        public bool CanUndock => dockingState == DockingState.Docked;//默认状态下可以解锁意味着处于docked状态

        // Constants常量
        private static readonly float distanceTolerance = 0.01f; // in meters ；距离公差
        private static readonly float angleTolerance = 3.0f; // in degrees
        private static readonly float scaleTolerance = 0.01f; // in percentage

        private DockPositionForOne dockPositionForOne = null;//当前对象对应的position是空的

        private Vector3 dockPositionForOneScale = Vector3.one;//进入dockposition后的大小
        private HashSet<DockPositionForOne> overlappingPositionsforone = new HashSet<DockPositionForOne>();//是否需要此量？
        private Vector3 originalScale = Vector3.one;//原始大小
        private static bool isDragging = false;//是否拖拽
        public static bool IsDragging
        {
            get => isDragging;
            set => isDragging = value;
        }


        public DockPositionForOne aimPosition = null; //目标position
        

        public bool isEnter = false;//是否进入碰撞体，默认为false
        [SerializeField]
        private static bool isRight = false;//具有dockableforone组件的对象是否为锁定状态，非锁定为true
        public static bool IsRight
        {
            get => isRight;
            set => isRight = value;
        }
        #endregion

        #region 0.同步字段isPunEnabled
        private bool isPunEnabled;//是否支持PUN
        public bool IsPunEnabled
        {
            set => isPunEnabled = value;
        }
        #endregion

        #region 1.主要方法：Update（）、Dock（）、Undock（）、DockOrNotByBool（）

        /// <summary>
        /// 根据isDragging和dock状态，每帧（更新对象的位置角度大小）
        /// </summary>
        /// <mark>
        /// 1.判断是否处于拖曳状态
        /// 2.如果是，那么判断能不能解锁，能解就解；如果没有在拖曳，那么？
        /// 3.如果对象正处于锁定状态或者入栈过程，那么判断，如果拖曳了，那么入栈
        /// 4.如果没有被锁定或者入栈，也就是处于自由或出栈的状态，那么回归本身大小      
        /// </mark>
        public void Update()
        {
            if (isDragging)//如果正在拖曳
            {
                if (CanUndock)//如果本身为docked状态，那么直接解锁
                {
                    //Undockforone();
                    PUNUndock();//网络部分改动
                }
            }
            else//如果没在拖曳
            {                
                if (overlappingPositionsforone.Count > 0 && CanDock )//这个点位存在对象，并且当前为自由状态
                {
                    var closestPosition = GetClosestPosition();//找到最近的点位，停靠在最近点位                    
                    if(closestPosition == aimPosition)
                    {
                        Dockforone(closestPosition);
                        //PUNDock(closestPosition);
                    }
                }
                
            }
           
            if (dockingState == DockingState.Docked || dockingState == DockingState.Docking)
            {

                Assert.IsNotNull(dockPositionForOne, "When a dockable is docked, its dockedPosition must be valid.");
                Assert.AreEqual(dockPositionForOne.DockedObject, this, "当一个具有dockable组件的对象停靠的时候，他的位置引用dockable");//报错了
                //也就是说target_1的dockedobject,和target_2的this不相等，

                var lerpTime = dockingState == DockingState.Docked ? moveLerpTimeWhenDocked : moveLerpTime;

                #region 拖拽停止时执行入栈方法
                if (!isDragging)
                {
                    // Don't override dragging                   
                    transform.position = Solver.SmoothTo(transform.position, dockPositionForOne.transform.position, Time.deltaTime, lerpTime);
                    transform.rotation = Solver.SmoothTo(transform.rotation, dockPositionForOne.transform.rotation, Time.deltaTime, lerpTime);
                }

                transform.localScale = Solver.SmoothTo(transform.localScale, dockPositionForOneScale, Time.deltaTime, lerpTime);

                if (VectorExtensions.CloseEnough(dockPositionForOne.transform.position, transform.position, distanceTolerance) &&
                    QuaternionExtensions.AlignedEnough(dockPositionForOne.transform.rotation, transform.rotation, angleTolerance) &&
                    AboutTheSameSize(dockPositionForOneScale.x, transform.localScale.x))
                {
                    // Finished docking
                    dockingState = DockingState.Docked;

                    // Snap to position
                    transform.position = dockPositionForOne.transform.position;
                    transform.rotation = dockPositionForOne.transform.rotation;
                    transform.localScale = dockPositionForOneScale;
                }
                #endregion
            }
            else if (dockPositionForOne == null && dockingState == DockingState.Undocking )
            {
                #region 回归本身大小
                transform.localScale = Solver.SmoothTo(transform.localScale, originalScale, Time.deltaTime, moveLerpTime);

                if (AboutTheSameSize(originalScale.x, transform.localScale.x))
                {
                    // Finished undocking
                    dockingState = DockingState.Undocked;

                    // Snap to size
                    transform.localScale = originalScale;
                }
                #endregion
            }
            
            if (dockPositionForOne != aimPosition)
            {
                //Debug.Log("如果名字不对，那么执行离开的操作");
                if (dockingState == DockingState.Docked)
                {
                    isRight = true;                                        
                    dockingState = DockingState.Undocked;
                    transform.localScale = originalScale;

                }
            }
            
        }

        /// <summary>
        /// 根据isDragging真假，进行锁定和解锁操作
        /// </summary>
        public void DockOrNotByBool()
        {
            //如果处于拖曳状态，此时如果对象状态为docked状态，即默认状态，那么可以解锁
            if (isDragging == true)
            {
                if (CanUndock)//刚开始操作对象的时候，对象本身在锁定状态，如果可以解锁，那就解锁
                {
                    Debug.Log("2.解锁");
                    //Undockforone();
                    PUNUndock();//网络部分改动
                }
                
            }
            
            else
            {
                //没有拖拽的情况下
                if (overlappingPositionsforone.Count >= 0 && CanDock)//如果重叠已经使用的位置数目大于0并且可以dock
                {                   
                    Debug.Log("4.执行了DockOrNotByBool 的else");
                }
            }
            

        }

       
        /// <summary>
        /// 锁定过程，position是一个DockPosition的实例
        /// </summary>
        /// <param name="positionforone"></param>
        public void Dockforone(DockPositionForOne position)
        {
            //如果当前状态为docked或者docking（即已经锁定或者入栈过程中），并且dockedposition和aimposition相等，那么输出信息“正在锁定一个错误对象”            
            if (!CanDock)
            {
                Debug.LogError($"试图去锁定一个不在自由状态的对象. 他的当前状态为 = {dockingState}");                
                return;
            }
            //正常执行dock功能，即当前状态为自由状态（undocked、undocking）
            //输出信息
            Debug.Log($"正在锁定对象： {gameObject.name} 在这个位置： {position.gameObject.name}");            
            
            dockPositionForOne = position;//告诉对象，要把对象停靠在这个位置           
            dockPositionForOne.DockedObject = this;//把当前对象填充到dockposition的DockedObject位置
            var zanObj = this;
            
            if(dockPositionForOne == aimPosition)//如果这个位置就是目标位置，那么执行dock
            {
                float scaleToFit = gameObject.GetComponent<Collider>().bounds.GetScaleToFitInside(dockPositionForOne.GetComponent<Collider>().bounds);//缩放系数
                dockPositionForOneScale = transform.localScale * scaleToFit;//进去的大小=本身大小 * 缩放系数

                if (dockingState == DockingState.Undocked)//如果处于undocked自由状态
                {
                    //首次锁定时只记录原始大小
                    originalScale = transform.localScale;
                }
                dockingState = DockingState.Docking;//状态转换为正在锁定
            }
            else//如果这个位置不是目标位置，那么
            {
                
                    dockPositionForOne.DockedObject = zanObj;
                
            }           
        }

        /// <summary>
        /// 解锁过程
        /// </summary>
        public void Undockforone()
        {
            if (!CanUndock)//1.如果不是docked状态，那么输出报错
            {
                Debug.LogError($"试图去解锁一个并不在锁定状态的对象. State = {dockingState}");
                return;
            }
            //2.确定是docked状态下，然后清空对象位置，清空位置，
            Debug.Log($"解锁对象： {gameObject.name} 从这个位置： {dockPositionForOne.gameObject.name}");

            dockPositionForOne.DockedObject = null;//锁定的对象栏变成空的
            dockPositionForOne = null;//锁定的位置也变成空的
            dockPositionForOneScale = Vector3.one;//大小恢复原本大小
            dockingState = DockingState.Undocking;//状态为正在解锁
        }

       
        #endregion

        #region 2.辅助方法：OnTriggerEnter/Exit()、(No)Dragging()、GetClosestPosition()、AboutTheSameSize()、GetBackOut()

        private DockPositionForOne GetClosestPosition()
        {
            var bounds = gameObject.GetComponent<Collider>().bounds;//对象本身碰撞体的边界
            var minDistance = float.MaxValue;
            DockPositionForOne closestPosition = null;//定义最近的位置，默认为空
            foreach (var position in overlappingPositionsforone)//遍历一遍重叠已使用的位置
            {
                //获取两个碰撞体中心距离，向量比较，sqrmagnitude是节约CPU的粗略比较
                var distance = (position.gameObject.GetComponent<Collider>().bounds.center - bounds.center).sqrMagnitude;

                if (closestPosition == null || distance < minDistance )//如果最近的位置没有或者距离小于最小值
                {
                    if(position == aimPosition)
                    {
                        closestPosition = position;
                        minDistance = distance;
                    }
                    else
                    {
                        Debug.Log("没有找到最近位置");
                    }
                    //closestPosition = position;
                    //minDistance = distance;
                }
            }

            return closestPosition;
        }

        /// <summary>
        /// 碰撞事件：对象碰撞体进入dockposition碰撞体时的更新
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerEnter(Collider collider)
        {
            isEnter = true;
            
            var dockPosition = collider.gameObject.GetComponent<DockPositionForOne>();
            if (dockPosition != null)//如果dock位置有对象了，那么已使用列表增加一个位置
            {
                overlappingPositionsforone.Add(dockPosition);
                Debug.Log($"{gameObject.name} collided with {dockPosition.name}");
            }
           
        }

        /// <summary>
        /// 碰撞事件：对象碰撞体离开dockposition碰撞体时的更新
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerExit(Collider collider)
        {
            Debug.Log("碰撞体离开");
            isEnter = false;
            //dockingState = DockingState.Undocking;
            
            var dockPosition = collider.gameObject.GetComponent<DockPositionForOne>();
            if (overlappingPositionsforone.Contains(dockPosition))//如果已使用列表包括这个位置，那么删除这个位置
            {
                overlappingPositionsforone.Remove(dockPosition);
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

        #endregion

        #region 3.PUN方法
        
        private void PUNUndock()
        {
            if (isPunEnabled)
                OnPUNUdock?.Invoke();
            else
                Undockforone();
        }       
        public event DockableForOneDelegate OnPUNUdock;

        private void PUNDock(DockPositionForOne position)
        {
            if (isPunEnabled)
                OnPUNDock?.Invoke();
            else
                Dockforone(position);
        }

        public event DockableForOneDelegate OnPUNDock;
        #endregion
    }

}
