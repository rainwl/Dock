using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dock
{


    public class SceneRecorder : MonoBehaviour
    {
        private string buttonName;

        public void Record(Button button, MainSence main)
        {
            Button recordButton = null;
            foreach (var item in main.Buttons)
            {
                if (item.gameObject.name == buttonName)
                {
                    recordButton = item;
                }
            }
            if (recordButton == button)
            {
                return;
            }
            else
            {
                if (recordButton != null)
                    recordButton.targetGraphic.color *= 2f;
            }
            buttonName = button.gameObject.name;
            button.targetGraphic.color *= 0.5f;
        }

        public void Recover(MainSence main)
        {
            Button recordButton = null;
            foreach (var item in main.Buttons)
            {
                if (item.gameObject.name == buttonName)
                {
                    recordButton = item;
                }
            }
            if (recordButton != null)
            {
                recordButton.targetGraphic.color *= 0.5f;
                recordButton.onClick.Invoke();
            }
        }
    }
}