using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.ObjectModel;
using UnityEngine.UI;

namespace Dock
{
    /// <summary>
    /// Dock:组件挂载于dockedPosition集合的父对象
    /// </summary>
    /// <mark>
    /// 不断更新dock position列表，提供两个方法：TryMoveToFreeSpace和MoveDockedObject
    /// 此控件允许对象移入移出位置,创建选项板、工具架和导航栏的步骤.
    /// </mark>   
    /// <seealso cref="Dockable"/>//另请参阅Dockable和DockPosition类
    /// <seealso cref="DockPosition"/>
    public class Dock : MonoBehaviour
    {
        public delegate void DockDelegate();
        #region 0.同步字段isPunEnabled
        private bool isPunEnabled;//是否支持PUN
        public bool IsPunEnabled
        {
            set => isPunEnabled = value;
        }
        #endregion

        #region 0.默认字段

        [SerializeField]
        [Tooltip("A read-only list of possible positions in this dock.")]//鼠标悬停后显示的文字
        private ReadOnlyCollection<DockPosition> dockPositions;

        /// <summary>
        /// 此Dock中可能位置的只读列表
        /// </summary>
        public ReadOnlyCollection<DockPosition> DockPositions => dockPositions;
        
        //自己加的一个新属性，给forone用的
        private ReadOnlyCollection<DockPositionForOne> dockPositionsforone;
        public ReadOnlyCollection<DockPositionForOne> DockPositionsforone => dockPositionsforone;

        //给One用的
        private ReadOnlyCollection<DockPositionOne> dockPositionOnes;
        public ReadOnlyCollection<DockPositionOne> DockPositionOne => dockPositionOnes;

        #endregion

        #region 1.主要方法：UpdatePositions()、 TryMoveToFreeSpace()、 MoveDockedObject()
        
        /// <summary>
        /// 3.更新Dock中dockposition列表的方法
        /// </summary>
        public void UpdatePositions()
        {
            dockPositions = gameObject.GetComponentsInChildren<DockPosition>().ToReadOnlyCollection();
            dockPositionsforone = gameObject.GetComponentsInChildren<DockPositionForOne>().ToReadOnlyCollection();
            dockPositionOnes = gameObject.GetComponentsInChildren<DockPositionOne>().ToReadOnlyCollection();
        }

        /// <summary>
        /// 4.将老对象移到附近的自由位置，为新对象腾出空间：TryMoveToFreeSpace 
        /// 确定dock position是否空，位置是否正确，找最近自由位置，判断左移或右移
        /// </summary>
        /// <param name="position">对象要停靠的所需位置</param>
        /// <returns>所需位置现在可用,返回true，否则false</returns>
        public bool TryMoveToFreeSpace(DockPosition position)
        {
            if (dockPositions == null)//如果dock位置为空，那么更新位置列表
            {
                UpdatePositions();
            }

            if (!dockPositions.Contains(position))//如果位置不对
            {
                Debug.LogError("Looking for a DockPosition in the wrong Dock.");
                return false;
            }

            var index = dockPositions.IndexOf(position);//位置索引

            if (!dockPositions[index].IsOccupied)//所有位置都没有被占用
            {
                // Already free
                return true;
            }

            // Where is the closest free space? (on a tie, favor left)
            //找最近的自由位置
            int? closestFreeSpace = null;
            int distanceToClosestFreeSpace = int.MaxValue;
            for (int i = 0; i < dockPositions.Count; i++)
            {
                var distance = Math.Abs(index - i);
                if (!dockPositions[i].IsOccupied && distance < distanceToClosestFreeSpace)//如果位置都没被占用并且距离小于到最近位置距离，那么赋值
                {
                    closestFreeSpace = i;
                    distanceToClosestFreeSpace = distance;
                }
            }

            if (closestFreeSpace == null)//如果没有空闲位置
            {
                // No free space
                return false;
            }

            if (closestFreeSpace < index)
            {
                // Move left

                // Check if we can undock all of them
                for (int i = closestFreeSpace.Value + 1; i <= index; i++)
                {
                    if (!dockPositions[i].DockedObject.CanUndock)//位置不能解开
                    {
                        return false;
                    }
                }

                for (int i = closestFreeSpace.Value + 1; i <= index; i++)
                {
                    MoveDockedObject(i, i - 1);
                }
            }
            else
            {
                // Move right

                // Check if we can undock all of them
                for (int i = closestFreeSpace.Value - 1; i >= index; i--)
                {
                    if (!dockPositions[i].DockedObject.CanUndock)
                    {
                        return false;
                    }
                }

                for (int i = closestFreeSpace.Value - 1; i >= index; i--)
                {
                    MoveDockedObject(i, i + 1);
                }
            }

            return true;
        }

        public bool TryMoveToFreeSpaceOne(DockPositionOne position)
        {
            if (dockPositionOnes == null)//如果dock位置为空，那么更新位置列表
            {
                UpdatePositions();
            }

            if (!dockPositionOnes.Contains(position))//如果位置不对
            {
                Debug.LogError("Looking for a DockPosition in the wrong Dock.");
                return false;
            }

            var index = dockPositionOnes.IndexOf(position);//位置索引

            if (!dockPositionOnes[index].IsOccupied)//所有位置都没有被占用
            {
                // Already free
                return true;
            }

            // Where is the closest free space? (on a tie, favor left)
            //找最近的自由位置
            int? closestFreeSpace = null;
            int distanceToClosestFreeSpace = int.MaxValue;
            for (int i = 0; i < dockPositionOnes.Count; i++)
            {
                var distance = Math.Abs(index - i);
                if (!dockPositionOnes[i].IsOccupied && distance < distanceToClosestFreeSpace)//如果位置都没被占用并且距离小于到最近位置距离，那么赋值
                {
                    closestFreeSpace = i;
                    distanceToClosestFreeSpace = distance;
                }
            }

            if (closestFreeSpace == null)//如果没有空闲位置
            {
                // No free space
                return false;
            }

            if (closestFreeSpace < index)
            {
                // Move left

                // Check if we can undock all of them
                for (int i = closestFreeSpace.Value + 1; i <= index; i++)
                {
                    if (!dockPositionOnes[i].DockedObject.CanUndock)//位置不能解开
                    {
                        return false;
                    }
                }

                for (int i = closestFreeSpace.Value + 1; i <= index; i++)
                {
                    MoveDockedObject(i, i - 1);
                }
            }
            else
            {
                // Move right

                // Check if we can undock all of them
                for (int i = closestFreeSpace.Value - 1; i >= index; i--)
                {
                    if (!dockPositionOnes[i].DockedObject.CanUndock)
                    {
                        return false;
                    }
                }

                for (int i = closestFreeSpace.Value - 1; i >= index; i--)
                {
                    MoveDockedObject(i, i + 1);
                }
            }

            return true;
        }

        public bool TryMoveToFreeSpaceForOne(DockPositionForOne position)
        {
            if (dockPositionsforone == null)//如果dock位置为空，那么更新位置列表
            {
                //UpdatePositions();//测试用，测试结束还回去
                PUNUpdatePositions();
            }

            if (!dockPositionsforone.Contains(position))//如果位置不对
            {
                Debug.LogError("Looking for a DockPosition in the wrong Dock.");
                return false;
            }

            var index = dockPositionsforone.IndexOf(position);//位置索引

            if (!dockPositionsforone[index].IsOccupied)//所有位置都没有被占用
            {
                // Already free
                return true;
            }

            // Where is the closest free space? (on a tie, favor left)
            //找最近的自由位置
            int? closestFreeSpace = null;
            int distanceToClosestFreeSpace = int.MaxValue;
            for (int i = 0; i < dockPositionsforone.Count; i++)
            {
                var distance = Math.Abs(index - i);
                if (!dockPositionsforone[i].IsOccupied && distance < distanceToClosestFreeSpace)//如果位置都没被占用并且距离小于到最近位置距离，那么赋值
                {
                    closestFreeSpace = i;
                    distanceToClosestFreeSpace = distance;
                }
            }

            if (closestFreeSpace == null)//如果没有空闲位置
            {
                // No free space
                return false;
            }

            if (closestFreeSpace < index)
            {
                // Move left

                // Check if we can undock all of them
                for (int i = closestFreeSpace.Value + 1; i <= index; i++)
                {
                    if (!dockPositionsforone[i].DockedObject.CanUndock)//位置不能解开
                    {
                        return false;
                    }
                }

                for (int i = closestFreeSpace.Value + 1; i <= index; i++)
                {
                    MoveDockedObject(i, i - 1);
                }
            }
            else
            {
                // Move right

                // Check if we can undock all of them
                for (int i = closestFreeSpace.Value - 1; i >= index; i--)
                {
                    if (!dockPositionsforone[i].DockedObject.CanUndock)
                    {
                        return false;
                    }
                }

                for (int i = closestFreeSpace.Value - 1; i >= index; i--)
                {
                    MoveDockedObject(i, i + 1);
                }
            }

            return true;
        }

        /// <summary>
        /// 5.将老对象从当前位置移动到附近自由位置，方法是解锁并将其锁在新位置：MoveDockedObject
        /// </summary>
        /// <param name="from">我们移动物体的位置</param>
        /// <param name="to">我们正在移动物体的位置.</param>
        private void MoveDockedObject(int from, int to)
        {
            var objectToMove = dockPositions[from].DockedObject;//需要移动的物体
            objectToMove.Undock();//解锁
            objectToMove.Dock(dockPositions[to]);//所在目标dock位置
            //断言我们刚刚移动的对象需要与停靠在新位置的对象匹配
            Assert.AreEqual(dockPositions[to].DockedObject, objectToMove, "The object we just moved needs to match the object docked in the new position.");
        }

        #endregion

        #region 2.辅助方法：OnEnable()、OnTransformChildrenChanged()

        /// <summary>
        /// 1.初始化此Dock中的位置列表。
        /// </summary>
        private void OnEnable()//调用updateposition方法，
        {
            //UpdatePositions();
            PUNUpdatePositions();
        }

        /// <summary>
        /// 2.当dockable对象更改时更新此Dock中的postion列表，调用UpdatePositions方法
        /// </summary>
        private void OnTransformChildrenChanged()
        {
            //UpdatePositions();
            PUNUpdatePositions();
        }
        #endregion

        #region PUN方法
        private void PUNUpdatePositions()
        {
            if (isPunEnabled)
                OnUpdatePositions?.Invoke();
            else
                UpdatePositions();
        }
        public event DockDelegate OnUpdatePositions;

        #endregion
        
        

    }
}