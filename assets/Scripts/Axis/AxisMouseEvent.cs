using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dock
{
    public class AxisMouseEvent : MonoBehaviour
    {
        /*
        private void Update()
        {           
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f, 1 << LayerMask.NameToLayer("Axis")))
                {
                    Debug.Log("发出来射线检测是否点击Z");
                    MoveModel.IsMoveModel = true;
                    MoveModel.M_lastMousePos = Input.mousePosition;                   
                }
            }
        }
        */

        Transform m_gameManager;

        void Start()
        {
            m_gameManager = GameObject.Find("AxisManager").transform;
        }

        void OnMouseEnter()
        {
            m_gameManager.SendMessage("MoseHoverEnter", gameObject.name);
        }

        void OnMouseDown()
        {
            m_gameManager.SendMessage("MouseDown", gameObject.name);
        }

        void OnMouseExit()
        {
            m_gameManager.SendMessage("MouseHoverExis", gameObject.name);
        }
    }
}
