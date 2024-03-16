using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dock
{
    enum AxisState
    {
        X,
        Y,
        Z,
        Idle
    }
    public class MoveModel : MonoBehaviour
    {
        #region 字段
        private const float MOVE_SPEED = 200f;//移动速度
        public Transform m_model;//要移动的模型
        public Transform m_axis;//坐标轴
        private Color[] m_axisColors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow };//坐标轴颜色 分别对应x、y、z、选中轴
        
        [SerializeField]
        private static bool m_isMoveModel = false;//是否正在移动物体，是否可替换为dockableforone中的isdragging
        public static bool IsMoveModel
        {
            get => m_isMoveModel;
            set => m_isMoveModel = value;
        }



        private static Vector3 m_lastMousePos;//上一帧鼠标位置
        public static Vector3 M_lastMousePos
        {
            get => m_lastMousePos;
            set => m_lastMousePos = value;
        }
        private AxisState m_axisState = AxisState.Idle;//当前选中坐标轴
        private Transform[] m_xyz = new Transform[3];//坐标轴的三个轴
        #endregion

        #region 
        void Update()
        {
            
            if(ControAxis.Axispre != null && ControAxis.Target != null)
            {
                m_model = ControAxis.Target.transform;
                m_axis = ControAxis.Axispre.transform;
                //Debug.Log("Move Model初始化完成，为target和axispre赋值");
                //if (Input.GetMouseButton(0) && m_isMoveModel)//当点击鼠标左键的时候并且正在拖动的时候
                    if (Input.GetMouseButton(0) && m_isMoveModel == true)//当点击鼠标左键的时候并且正在拖动的时候
                {
                    MovingModel();//执行移动模型的方法
                   // Debug.Log("执行了移动模型的方法");
                }
                //移动完成
                else if (Input.GetMouseButtonUp(0) && m_isMoveModel == true)//当松开的时候并且还在拖动
                {
                    MoveComplete();//执行移动结束的方法
                }
            }                        
        }
        #endregion

        /// <summary>
        /// 移动方法
        /// </summary>
        void MovingModel()//移动方法，首先获取鼠标移动方向，
        {
            //鼠标在屏幕上的方向
            Vector3 mouseDir = Input.mousePosition - m_lastMousePos;
            Vector3 currentPos = Input.mousePosition;
            //鼠标坐标转世界坐标
            Vector3 startWorld = Camera.main.ScreenToWorldPoint(new Vector3(m_lastMousePos.x, m_lastMousePos.y, 1));
            Vector3 endWorld = Camera.main.ScreenToWorldPoint(new Vector3(currentPos.x, currentPos.y, 1));
            Vector3 mouseWorldDir = (endWorld - startWorld);

            Vector3 offset = Vector3.zero;
            Vector3 axisStart = Camera.main.WorldToScreenPoint(m_axis.position);
            Debug.Log("准备寻找Z轴");
            switch (m_axisState)//找到Z轴，然后转换坐标
            {
                case AxisState.X:
                    Transform x = m_axis.Find("X");
                    Vector3 screenDir = Camera.main.WorldToScreenPoint(x.forward);
                    float similar = Vector3.Dot(mouseWorldDir, x.forward);
                    offset = new Vector3(similar * Time.deltaTime * MOVE_SPEED, 0, 0);

                    break;
                case AxisState.Y:
                    Transform y = m_axis.Find("Y");
                    screenDir = Camera.main.WorldToScreenPoint(y.forward);
                    similar = Vector3.Dot(mouseWorldDir, y.forward);
                    offset = new Vector3(0, similar * Time.deltaTime * MOVE_SPEED, 0);
                    break;
                case AxisState.Z:
                    Transform z = m_axis.Find("Z");
                    screenDir = Camera.main.WorldToScreenPoint(z.forward);
                    similar = Vector3.Dot(mouseWorldDir, z.forward);
                    offset = new Vector3(0, 0, similar * Time.deltaTime * MOVE_SPEED);
                    Debug.Log("移动了Z轴");
                    break;
                default: break;
            }
            m_model.transform.position += offset;//让二者的位置都加上偏移量
            m_axis.position += offset;

            m_lastMousePos = Input.mousePosition;//鼠标最后的位置就是最后的位置
        }



        void MoveComplete()
        {
            m_isMoveModel = false;

          
        }

        void MoseHoverEnter(string axisName)
        {
            
        }

        void MouseHoverExis(string axisName)
        {
            
        }

        //选中坐标轴
        void MouseDown(string axisName)
        {
            m_isMoveModel = true;
            m_lastMousePos = Input.mousePosition;
            switch (axisName)
            {
                case "X":
                    m_axisState = AxisState.X;
                    break;
                case "Y":
                    m_axisState = AxisState.Y;
                    break;
                case "Z":
                    m_axisState = AxisState.Z;
                    break;
                default:
                    m_axisState = AxisState.Idle;
                    break;
            }
        }



    }
}
