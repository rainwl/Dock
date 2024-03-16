using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dock
{
    /// <summary>
    /// 这个脚本目前还没有使用，因为颜色控制方面，貌似预制体在场景中是不能动态修改的，或者是我没有掌握到方法，等之后再解决
    /// </summary>
    public class AxisCallBack : MonoBehaviour
    {
        public Transform m_axis;//坐标轴

        private Color[] m_axisColors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow };//坐标轴颜色 分别对应x、y、z、选中轴
        private bool m_isMoveModel = false;//是否正在移动物体，是否可替换为dockableforone中的isdragging
        private Vector3 m_lastMousePos;//上一帧鼠标位置
        private AxisState m_axisState = AxisState.Idle;//当前选中坐标轴

        private Transform[] m_xyz = new Transform[3];//坐标轴的三个轴

        void Start()
        {
            m_axis = ControAxis.Axispre.transform;

            for (int i = 0; i < m_axis.childCount; i++)
            {
                m_xyz[i] = m_axis.GetChild(i);
            }
            //坐标轴颜色初始化
            m_xyz[0].GetComponent<MeshRenderer>().material.SetColor("_Color", m_axisColors[0]);
            m_xyz[1].GetComponent<MeshRenderer>().material.SetColor("_Color", m_axisColors[1]);
            m_xyz[2].GetComponent<MeshRenderer>().material.SetColor("_Color", m_axisColors[2]);
        }

        /// <summary>
        /// 鼠标悬浮坐标轴 黄色材质
        /// </summary>
        /// <param name="axisName"></param>
        void MoseHoverEnter(string axisName)
        {
            if (!m_isMoveModel)
            {
                switch (axisName)
                {
                    case "X":
                        m_xyz[0].GetComponent<MeshRenderer>().material.SetColor("_Color", m_axisColors[3]);
                        break;
                    case "Y":
                        m_xyz[1].GetComponent<MeshRenderer>().material.SetColor("_Color", m_axisColors[3]);
                        break;
                    case "Z":
                        m_xyz[2].GetComponent<MeshRenderer>().material.SetColor("_Color", m_axisColors[3]);
                        break;
                }
            }
        }
        /// <summary>
        /// 鼠标离开
        /// </summary>
        /// <param name="axisName"></param>
        void MouseHoverExis(string axisName)
        {
            if (!m_isMoveModel)
            {
                switch (axisName)
                {
                    case "X":
                        m_xyz[0].GetComponent<MeshRenderer>().material.SetColor("_Color", m_axisColors[0]);
                        break;
                    case "Y":
                        m_xyz[1].GetComponent<MeshRenderer>().material.SetColor("_Color", m_axisColors[1]);
                        break;
                    case "Z":
                        m_xyz[2].GetComponent<MeshRenderer>().material.SetColor("_Color", m_axisColors[2]);
                        break;
                }
            }
        }
        /// <summary>
        /// 选中坐标轴
        /// </summary>
        /// <param name="axisName"></param>
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
        void OnMouseDown()
        {
            m_isMoveModel = true;
            m_lastMousePos = Input.mousePosition;
        }

    }
}
